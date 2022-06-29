﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Agent.Plugins.PipelineArtifact.Telemetry;
using Agent.Sdk;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.VisualStudio.Services.Agent.Util;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Common.Telemetry;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using Microsoft.VisualStudio.Services.WebApi;

namespace Agent.Plugins.PipelineArtifact
{
    internal class FileShareProvider: IArtifactProvider
    {
        private readonly AgentTaskPluginExecutionContext context;
        private readonly IAppTraceSource tracer;
        private const int defaultParallelCount = 1;
        private readonly VssConnection connection;
        private readonly IDedupManifestArtifactClientFactory factory;

        // Default stream buffer size set in the existing file share implementation https://github.com/microsoft/azure-pipelines-agent/blob/ffb3a9b3e2eb5a1f34a0f45d0f2b8639740d37d3/src/Agent.Worker/Release/Artifacts/FileShareArtifact.cs#L154
        private const int DefaultStreamBufferSize = 8192;

        public FileShareProvider(AgentTaskPluginExecutionContext context, VssConnection connection, IAppTraceSource tracer) : this(context, connection, tracer, DedupManifestArtifactClientFactory.Instance)
        {
        }

        internal FileShareProvider(AgentTaskPluginExecutionContext context, VssConnection connection, IAppTraceSource tracer, IDedupManifestArtifactClientFactory factory)
        {
            this.factory = factory;
            this.context = context;
            this.tracer = tracer;
            this.connection = connection;
        }

        public async Task DownloadSingleArtifactAsync(PipelineArtifactDownloadParameters downloadParameters, BuildArtifact buildArtifact, CancellationToken cancellationToken) 
        {
            await DownloadMultipleArtifactsAsync(downloadParameters, new List<BuildArtifact> { buildArtifact }, cancellationToken);
        }

        public async Task DownloadMultipleArtifactsAsync(PipelineArtifactDownloadParameters downloadParameters, IEnumerable<BuildArtifact> buildArtifacts, CancellationToken cancellationToken) 
        {
            BlobStoreClientTelemetry clientTelemetry;
            DedupManifestArtifactClient dedupManifestClient = this.factory.CreateDedupManifestClient(context, connection, cancellationToken, out clientTelemetry);
            using (clientTelemetry)
            {
                FileShareActionRecord downloadRecord = clientTelemetry.CreateRecord<FileShareActionRecord>((level, uri, type) =>
                    new FileShareActionRecord(level, uri, type, nameof(DownloadArtifactsAsync), context));

                await clientTelemetry.MeasureActionAsync(
                    record: downloadRecord,
                    actionAsync: async () =>
                    {
                        return await DownloadArtifactsAsync(downloadParameters, buildArtifacts, cancellationToken);
                    }
                );

                // Send results to CustomerIntelligence
                context.PublishTelemetry(area: PipelineArtifactConstants.AzurePipelinesAgent, feature: PipelineArtifactConstants.PipelineArtifact, record: downloadRecord);
            }
        }

        private async Task<FileShareDownloadResult> DownloadArtifactsAsync(PipelineArtifactDownloadParameters downloadParameters, IEnumerable<BuildArtifact> buildArtifacts, CancellationToken cancellationToken)
        {
            var records = new List<ArtifactRecord>();
            long totalContentSize = 0;
            int totalFileCount = 0;
            foreach (var buildArtifact in buildArtifacts)
            {
                var downloadRootPath = Path.Combine(buildArtifact.Resource.Data, buildArtifact.Name);
                var minimatchPatterns = downloadParameters.MinimatchFilters.Select(pattern => Path.Combine(buildArtifact.Resource.Data, pattern));
                var record = await this.DownloadFileShareArtifactAsync(downloadRootPath, Path.Combine(downloadParameters.TargetDirectory, buildArtifact.Name), defaultParallelCount, cancellationToken, minimatchPatterns);
                totalContentSize += record.ContentSize;
                totalFileCount += record.FileCount;
                records.Add(record);
            }
            
            return new FileShareDownloadResult(records, totalFileCount, totalContentSize);
        }

        public async Task PublishArtifactAsync(
            string sourcePath,
            string destPath,
            int parallelCount,
            CancellationToken cancellationToken) 
        {
            BlobStoreClientTelemetry clientTelemetry;
            DedupManifestArtifactClient dedupManifestClient = this.factory.CreateDedupManifestClient(context, connection, cancellationToken, out clientTelemetry);
            using (clientTelemetry)
            {
                FileShareActionRecord publishRecord = clientTelemetry.CreateRecord<FileShareActionRecord>((level, uri, type) =>
                    new FileShareActionRecord(level, uri, type, nameof(PublishArtifactAsync), context));

                await clientTelemetry.MeasureActionAsync(
                    record: publishRecord,
                    actionAsync: async () =>
                    {
                        return await PublishArtifactUsingRobocopyAsync(this.context, sourcePath, destPath, parallelCount, cancellationToken);
                    }
                );

                // Send results to CustomerIntelligence
                context.PublishTelemetry(area: PipelineArtifactConstants.AzurePipelinesAgent, feature: PipelineArtifactConstants.PipelineArtifact, record: publishRecord);
            }
        }

        private async Task<FileSharePublishResult> PublishArtifactUsingRobocopyAsync(
            AgentTaskPluginExecutionContext executionContext,
            string dropLocation,
            string downloadFolderPath,
            int parallelCount,
            CancellationToken cancellationToken)
        {
            executionContext.Output(StringUtil.Loc("PublishingArtifactUsingRobocopy"));
            using (var processInvoker = new ProcessInvoker(this.context))
            {
                // Save STDOUT from worker, worker will use STDOUT report unhandle exception.
                processInvoker.OutputDataReceived += delegate (object sender, ProcessDataReceivedEventArgs stdout)
                {
                    if (!string.IsNullOrEmpty(stdout.Data))
                    {
                        executionContext.Output(stdout.Data);
                    }
                };

                // Save STDERR from worker, worker will use STDERR on crash.
                processInvoker.ErrorDataReceived += delegate (object sender, ProcessDataReceivedEventArgs stderr)
                {
                    if (!string.IsNullOrEmpty(stderr.Data))
                    {
                        executionContext.Error(stderr.Data);
                    }
                };

                var trimChars = new[] { '\\', '/' };

                dropLocation = Path.Combine(dropLocation.TrimEnd(trimChars));
                downloadFolderPath = downloadFolderPath.TrimEnd(trimChars);

                string robocopyArguments = "\"" + dropLocation + "\" \"" + downloadFolderPath + "\" * /E /COPY:DA /NP /R:3";

                robocopyArguments += " /MT:" + parallelCount;

                int exitCode = await processInvoker.ExecuteAsync(
                        workingDirectory: "",
                        fileName: "robocopy",
                        arguments: robocopyArguments,
                        environment: null,
                        requireExitCodeZero: false,
                        outputEncoding: null,
                        killProcessOnCancel: true,
                        cancellationToken: cancellationToken);

                executionContext.Output(StringUtil.Loc("RobocopyBasedPublishArtifactTaskExitCode", exitCode));
                // Exit code returned from robocopy. For more info https://blogs.technet.microsoft.com/deploymentguys/2008/06/16/robocopy-exit-codes/
                if (exitCode >= 8)
                {
                    throw new Exception(StringUtil.Loc("RobocopyBasedPublishArtifactTaskFailed", exitCode));
                }

                return new FileSharePublishResult (exitCode);
            }
        }

        private async Task<ArtifactRecord> DownloadFileShareArtifactAsync(
            string sourcePath,
            string destPath,
            int parallelCount,
            CancellationToken cancellationToken,
            IEnumerable<string> minimatchPatterns = null)
        {
            Stopwatch watch = Stopwatch.StartNew();

            IEnumerable<Func<string, bool>> minimatchFuncs = MinimatchHelper.GetMinimatchFuncs(minimatchPatterns, this.tracer);

            var trimChars = new[] { '\\', '/' };

            sourcePath = sourcePath.TrimEnd(trimChars);

            var artifactName =  new DirectoryInfo(destPath).Name;

            IEnumerable<FileInfo> files =
                new DirectoryInfo(sourcePath).EnumerateFiles("*", SearchOption.AllDirectories);

            var parallelism = new ExecutionDataflowBlockOptions()
            {
                MaxDegreeOfParallelism = parallelCount,
                BoundedCapacity = 2 * parallelCount,
                CancellationToken = cancellationToken
            };

            var contentSize = 0;
            var fileCount = 0;

            var actionBlock = NonSwallowingActionBlock.Create<FileInfo>(
               action: async file =>
                {
                    if (minimatchFuncs == null || minimatchFuncs.Any(match => match(file.FullName))) 
                    {
                        string tempPath = Path.Combine(destPath, Path.GetRelativePath(sourcePath, file.FullName));
                        context.Output(StringUtil.Loc("CopyFileToDestination", file, tempPath));
                        FileInfo tempFile = new System.IO.FileInfo(tempPath);
                        using (StreamReader fileReader = GetFileReader(file.FullName))
                        {
                            await WriteStreamToFile(
                                fileReader.BaseStream,
                                tempFile.FullName,
                                DefaultStreamBufferSize,
                                cancellationToken);
                        }
                        Interlocked.Add(ref contentSize, tempPath.Length);
                        Interlocked.Increment(ref fileCount);
                    }
                },
                dataflowBlockOptions: parallelism);
                
                await actionBlock.SendAllAndCompleteAsync(files, actionBlock, cancellationToken);

            watch.Stop();

            return new ArtifactRecord(artifactName,
                                      fileCount,
                                      contentSize,
                                      watch.ElapsedMilliseconds);
        }

        private async Task WriteStreamToFile(Stream stream, string filePath, int bufferSize, CancellationToken cancellationToken)
        {
            ArgUtil.NotNull(stream, nameof(stream));
            ArgUtil.NotNullOrEmpty(filePath, nameof(filePath));

            EnsureDirectoryExists(Path.GetDirectoryName(filePath));
            using (var targetStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, useAsync: true))
            {
                await stream.CopyToAsync(targetStream, bufferSize, cancellationToken);
            }
        }

        private StreamReader GetFileReader(string filePath)
        {
            string path = Path.Combine(ValidatePath(filePath));
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(StringUtil.Loc("FileNotFound", path));
            }

            return new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, DefaultStreamBufferSize, true));
        }

        private void EnsureDirectoryExists(string directoryPath)
        {
            string path = ValidatePath(directoryPath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private string ValidatePath(string path)
        {
            ArgUtil.NotNullOrEmpty(path, nameof(path));
            return Path.GetFullPath(path);
        }
    }
}