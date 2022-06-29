// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Pipelines = Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.VisualStudio.Services.Agent.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;

namespace Microsoft.VisualStudio.Services.Agent.Worker.Build
{
    public sealed class BuildJobExtension : JobExtension
    {
        public override Type ExtensionType => typeof(IJobExtension);
        public override HostTypes HostType => HostTypes.Build;

        public override IStep GetExtensionPreJobStep(IExecutionContext jobContext)
        {
            return null;
        }

        public override IStep GetExtensionPostJobStep(IExecutionContext jobContext)
        {
            return null;
        }

        // 1. use source provide to solve path, if solved result is rooted, return full path.
        // 2. prefix default path root (build.sourcesDirectory), if result is rooted, return full path.
        public override string GetRootedPath(IExecutionContext context, string path)
        {
            string rootedPath = null;
            TryGetRepositoryInfo(context, out RepositoryInfo repoInfo);

            if (repoInfo.SourceProvider != null &&
                repoInfo.Repository != null &&
                StringUtil.ConvertToBoolean(repoInfo.Repository.Properties.Get<string>("__AZP_READY")))
            {
                path = repoInfo.SourceProvider.GetLocalPath(context, repoInfo.Repository, path) ?? string.Empty;
                Trace.Info($"Build JobExtension resolving path use source provide: {path}");

                if (!string.IsNullOrEmpty(path) &&
                    path.IndexOfAny(Path.GetInvalidPathChars()) < 0 &&
                    Path.IsPathRooted(path))
                {
                    try
                    {
                        rootedPath = Path.GetFullPath(path);
                        Trace.Info($"Path resolved by source provider is a rooted path, return absolute path: {rootedPath}");
                        return rootedPath;
                    }
                    catch (Exception ex)
                    {
                        Trace.Info($"Path resolved by source provider is a rooted path, but it is not a full qualified path: {path}");
                        Trace.Error(ex);
                    }
                }
            }

            string defaultPathRoot = null;
            if (RepositoryUtil.HasMultipleCheckouts(context.JobSettings))
            {
                // If there are multiple checkouts, set the default directory to the sources root folder (_work/1/s)
                defaultPathRoot = context.Variables.Get(Constants.Variables.Build.SourcesDirectory);
                Trace.Info($"The Default Path Root of Build JobExtension is build.sourcesDirectory: {defaultPathRoot}");
            }
            else if (repoInfo.Repository != null)
            {
                // If there is only one checkout/repository, set it to the repository path
                defaultPathRoot = repoInfo.Repository.Properties.Get<string>(Pipelines.RepositoryPropertyNames.Path);
                Trace.Info($"The Default Path Root of Build JobExtension is repository.path: {defaultPathRoot}");
            }

            if (defaultPathRoot != null && defaultPathRoot.IndexOfAny(Path.GetInvalidPathChars()) < 0 &&
                path != null && path.IndexOfAny(Path.GetInvalidPathChars()) < 0)
            {
                path = Path.Combine(defaultPathRoot, path);
                Trace.Info($"After prefix Default Path Root provide by JobExtension: {path}");
                if (Path.IsPathRooted(path))
                {
                    try
                    {
                        rootedPath = Path.GetFullPath(path);
                        Trace.Info($"Return absolute path after prefix DefaultPathRoot: {rootedPath}");
                        return rootedPath;
                    }
                    catch (Exception ex)
                    {
                        Trace.Error(ex);
                        Trace.Info($"After prefix Default Path Root provide by JobExtension, the Path is a rooted path, but it is not full qualified, return the path: {path}.");
                        return path;
                    }
                }
            }

            return rootedPath;
        }

        public override void ConvertLocalPath(IExecutionContext context, string localPath, out string repoName, out string sourcePath)
        {
            repoName = "";
            TryGetRepositoryInfoFromLocalPath(context, localPath, out RepositoryInfo repoInfo);

            // If no repo was found, send back an empty repo with original path.
            sourcePath = localPath;

            if (!string.IsNullOrEmpty(localPath) &&
                File.Exists(localPath) &&
                repoInfo.Repository != null &&
                repoInfo.SourceProvider != null)
            {
                // If we found a repo, calculate the relative path to the file
                repoName = repoInfo.Repository.Properties.Get<string>(Pipelines.RepositoryPropertyNames.Name);
                var repoPath = repoInfo.Repository.Properties.Get<string>(Pipelines.RepositoryPropertyNames.Path);
                if (!string.IsNullOrEmpty(repoPath))
                {
                    sourcePath = IOUtil.MakeRelative(localPath, repoPath);
                }
            }
        }

        // Prepare build directory
        // Set all build related variables
        public override void InitializeJobExtension(IExecutionContext executionContext, IList<Pipelines.JobStep> steps, Pipelines.WorkspaceOptions workspace)
        {
            // Validate args.
            Trace.Entering();
            ArgUtil.NotNull(executionContext, nameof(executionContext));

            // This flag can be false for jobs like cleanup artifacts.
            // If syncSources = false, we will not set source related build variable, not create build folder, not sync source.
            bool syncSources = executionContext.Variables.Build_SyncSources ?? true;
            if (!syncSources)
            {
                Trace.Info($"{Constants.Variables.Build.SyncSources} = false, we will not set source related build variable, not create build folder and not sync source");
                return;
            }

            // We set the variables based on the 'self' repository
            if (!TryGetRepositoryInfo(executionContext, out RepositoryInfo repoInfo))
            {
                throw new Exception(StringUtil.Loc("SupportedRepositoryEndpointNotFound"));
            }

            executionContext.Debug($"Primary repository: {repoInfo.Repository.Properties.Get<string>(Pipelines.RepositoryPropertyNames.Name)}. repository type: {repoInfo.Repository.Type}");

            // Set the repo variables.
            if (!string.IsNullOrEmpty(repoInfo.Repository.Id)) // TODO: Move to const after source artifacts PR is merged.
            {
                executionContext.SetVariable(Constants.Variables.Build.RepoId, repoInfo.Repository.Id);
            }

            executionContext.SetVariable(Constants.Variables.Build.RepoName, repoInfo.Repository.Properties.Get<string>(Pipelines.RepositoryPropertyNames.Name));
            executionContext.SetVariable(Constants.Variables.Build.RepoProvider, ConvertToLegacyRepositoryType(repoInfo.Repository.Type));
            executionContext.SetVariable(Constants.Variables.Build.RepoUri, repoInfo.Repository.Url?.AbsoluteUri);

            // Prepare the build directory.
            executionContext.Output(StringUtil.Loc("PrepareBuildDir"));
            var directoryManager = HostContext.GetService<IBuildDirectoryManager>();
            TrackingConfig trackingConfig = directoryManager.PrepareDirectory(
                executionContext,
                executionContext.Repositories,
                workspace);

            string _workDirectory = HostContext.GetDirectory(WellKnownDirectory.Work);
            string pipelineWorkspaceDirectory = Path.Combine(_workDirectory, trackingConfig.BuildDirectory);

            UpdateCheckoutTasksAndVariables(executionContext, steps, repoInfo, pipelineWorkspaceDirectory);

            // Set the directory variables.
            executionContext.Output(StringUtil.Loc("SetBuildVars"));
            executionContext.SetVariable(Constants.Variables.Agent.BuildDirectory, pipelineWorkspaceDirectory, isFilePath: true);
            executionContext.SetVariable(Constants.Variables.System.ArtifactsDirectory, Path.Combine(_workDirectory, trackingConfig.ArtifactsDirectory), isFilePath: true);
            executionContext.SetVariable(Constants.Variables.System.DefaultWorkingDirectory, Path.Combine(_workDirectory, trackingConfig.SourcesDirectory), isFilePath: true);
            executionContext.SetVariable(Constants.Variables.Common.TestResultsDirectory, Path.Combine(_workDirectory, trackingConfig.TestResultsDirectory), isFilePath: true);
            executionContext.SetVariable(Constants.Variables.Build.BinariesDirectory, Path.Combine(_workDirectory, trackingConfig.BuildDirectory, Constants.Build.Path.BinariesDirectory), isFilePath: true);
            executionContext.SetVariable(Constants.Variables.Build.SourcesDirectory, Path.Combine(_workDirectory, trackingConfig.SourcesDirectory), isFilePath: true);
            executionContext.SetVariable(Constants.Variables.Build.StagingDirectory, Path.Combine(_workDirectory, trackingConfig.ArtifactsDirectory), isFilePath: true);
            executionContext.SetVariable(Constants.Variables.Build.ArtifactStagingDirectory, Path.Combine(_workDirectory, trackingConfig.ArtifactsDirectory), isFilePath: true);
            executionContext.SetVariable(Constants.Variables.Build.RepoLocalPath, Path.Combine(_workDirectory, trackingConfig.SourcesDirectory), isFilePath: true);
            executionContext.SetVariable(Constants.Variables.Pipeline.Workspace, pipelineWorkspaceDirectory, isFilePath: true);
        }

        private void UpdateCheckoutTasksAndVariables(IExecutionContext executionContext, IList<JobStep> steps, RepositoryInfo repoInfo, string pipelineWorkspaceDirectory)
        {
            bool? submoduleCheckout = null;
            // RepoClean may be set from the server, so start with the server value
            bool? repoClean = executionContext.Variables.GetBoolean(Constants.Variables.Build.RepoClean);

            var checkoutTasks = steps.Where(x => x.IsCheckoutTask()).Select(x => x as TaskStep).ToList();
            var hasOnlyOneCheckoutTask = checkoutTasks.Count == 1;
            foreach (var checkoutTask in checkoutTasks)
            {
                if (!checkoutTask.Inputs.TryGetValue(PipelineConstants.CheckoutTaskInputs.Repository, out string repositoryAlias))
                {
                    // If the checkout task isn't associated with a repo, just skip it
                    Trace.Info($"Checkout task {checkoutTask.Name} does not have a repository property.");
                    continue;
                }

                // Update the checkout "Clean" property for all repos, if the variable was set by the server.
                if (repoClean != null)
                {
                    checkoutTask.Inputs[PipelineConstants.CheckoutTaskInputs.Clean] = repoClean.Value.ToString();
                }

                Trace.Info($"Checking repository name {repositoryAlias}");
                // If this is the primary repository, use it to get the variable values
                // A repository is considered the primary one if the name is 'self' or if there is only
                // one checkout task. This is because Designer builds set the name of the repository something
                // other than 'self'
                if (hasOnlyOneCheckoutTask || RepositoryUtil.IsPrimaryRepositoryName(repositoryAlias))
                {
                    submoduleCheckout = checkoutTask.Inputs.ContainsKey(PipelineConstants.CheckoutTaskInputs.Submodules);
                    repoClean = repoClean ?? checkoutTask.Inputs.ContainsKey(PipelineConstants.CheckoutTaskInputs.Clean);
                }

                // Update the checkout task display name if not already set
                if (string.IsNullOrEmpty(checkoutTask.DisplayName) ||
                    string.Equals(checkoutTask.DisplayName, "Checkout", StringComparison.OrdinalIgnoreCase) ||      // this is the default for jobs
                    string.Equals(checkoutTask.DisplayName, checkoutTask.Name, StringComparison.OrdinalIgnoreCase)) // this is the default for deployment jobs
                {
                    var repository = RepositoryUtil.GetRepository(executionContext.Repositories, repositoryAlias);
                    if (repository != null)
                    {
                        string repoName = repository.Properties.Get<string>(RepositoryPropertyNames.Name);
                        string version = RepositoryUtil.TrimStandardBranchPrefix(repository.Properties.Get<string>(RepositoryPropertyNames.Ref));
                        string path = null;
                        if (checkoutTask.Inputs.ContainsKey(PipelineConstants.CheckoutTaskInputs.Path))
                        {
                            path = checkoutTask.Inputs[PipelineConstants.CheckoutTaskInputs.Path];
                        }
                        else
                        {
                            path = IOUtil.MakeRelative(repository.Properties.Get<string>(RepositoryPropertyNames.Path), pipelineWorkspaceDirectory);
                        }
                        checkoutTask.DisplayName = StringUtil.Loc("CheckoutTaskDisplayNameFormat", repoName, version, path);
                    }
                    else
                    {
                        Trace.Info($"Checkout task {checkoutTask.Name} has a repository property {repositoryAlias} that does not match any repository resource.");
                    }
                }
            }

            // Set variables
            if (submoduleCheckout.HasValue)
            {
                executionContext.SetVariable(Constants.Variables.Build.RepoGitSubmoduleCheckout, submoduleCheckout.Value.ToString());
            }

            if (repoClean.HasValue)
            {
                executionContext.SetVariable(Constants.Variables.Build.RepoClean, repoClean.Value.ToString());
            }
        }

        private bool TryGetRepositoryInfoFromLocalPath(IExecutionContext executionContext, string localPath, out RepositoryInfo repoInfo)
        {
            // Return the matching repository resource and its source provider.
            Trace.Entering();
            var repo = RepositoryUtil.GetRepositoryForLocalPath(executionContext.Repositories, localPath);
            repoInfo = new RepositoryInfo
            {
                Repository = repo,
                SourceProvider = GetSourceProvider(executionContext, repo),
            };

            return repoInfo.SourceProvider != null;
        }

        private bool TryGetRepositoryInfo(IExecutionContext executionContext, out RepositoryInfo repoInfo)
        {
            // Return the matching repository resource and its source provider.
            Trace.Entering();
            var repo = RepositoryUtil.GetPrimaryRepository(executionContext.Repositories);
            repoInfo = new RepositoryInfo
            {
                Repository = repo,
                SourceProvider = GetSourceProvider(executionContext, repo),
            };

            return repoInfo.SourceProvider != null;
        }

        private ISourceProvider GetSourceProvider(IExecutionContext executionContext, RepositoryResource repository)
        {
            if (repository != null)
            {
                var extensionManager = HostContext.GetService<IExtensionManager>();
                List<ISourceProvider> sourceProviders = extensionManager.GetExtensions<ISourceProvider>();
                var sourceProvider = sourceProviders.FirstOrDefault(x => string.Equals(x.RepositoryType, repository.Type, StringComparison.OrdinalIgnoreCase));
                return sourceProvider;
            }

            return null;
        }

        private string ConvertToLegacyRepositoryType(string pipelineRepositoryType)
        {
            if (String.Equals(pipelineRepositoryType, Pipelines.RepositoryTypes.Bitbucket, StringComparison.OrdinalIgnoreCase))
            {
                return "Bitbucket";
            }
            else if (String.Equals(pipelineRepositoryType, Pipelines.RepositoryTypes.ExternalGit, StringComparison.OrdinalIgnoreCase))
            {
                return "Git";
            }
            else if (String.Equals(pipelineRepositoryType, Pipelines.RepositoryTypes.Git, StringComparison.OrdinalIgnoreCase))
            {
                return "TfsGit";
            }
            else if (String.Equals(pipelineRepositoryType, Pipelines.RepositoryTypes.GitHub, StringComparison.OrdinalIgnoreCase))
            {
                return "GitHub";
            }
            else if (String.Equals(pipelineRepositoryType, Pipelines.RepositoryTypes.GitHubEnterprise, StringComparison.OrdinalIgnoreCase))
            {
                return "GitHubEnterprise";
            }
            else if (String.Equals(pipelineRepositoryType, Pipelines.RepositoryTypes.Svn, StringComparison.OrdinalIgnoreCase))
            {
                return "Svn";
            }
            else if (String.Equals(pipelineRepositoryType, Pipelines.RepositoryTypes.Tfvc, StringComparison.OrdinalIgnoreCase))
            {
                return "TfsVersionControl";
            }
            else
            {
                throw new NotSupportedException(pipelineRepositoryType);
            }
        }

        private class RepositoryInfo
        {
            public Pipelines.RepositoryResource Repository { set; get; }
            public ISourceProvider SourceProvider { set; get; }
        }
    }
}