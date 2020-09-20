using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;
using AbstraX.Contracts;
using System.Collections;
using Metaspec;
using AbstraX.Contracts.CodeParsers;

namespace AbstraX.Contracts
{
    public delegate void WatchProcess(IGenerator generator, Process process, bool blockUntilFinished, int timeout);
    public delegate void OnGeneratorFileWithBuildPreBuildHandler(ref Type generatorType, ref IElementBuild build, ref FileInfo fileInfo, ref string _namespace, ref Dictionary<string, object> extraValues, out bool skipBuildFile, out bool abortStep);
    public delegate void OnGeneratorFileWithListPreBuildHandler(ref Type generatorType, ref FileInfo fileInfo, ref string _namespace, ref IEnumerable list, out bool skipBuildFile, out bool abortStep);
    public delegate void OnGeneratorFileWithBuildPostBuildHandler(IElementBuild build, ref FileInfo fileInfo, string _namespace, Dictionary<string, object> extraValues, ICodeParser codeParser, ref string output);
    public delegate void OnGeneratorFileWithListPostBuildHandler(ref FileInfo fileInfo, string _namespace, IEnumerable list, ICodeParser codeParser, ref string output);
    public delegate void OnGeneratorFileWithBuildCodeWriterHandler(IElementBuild build, FileInfo fileInfo, string _namespace, Dictionary<string, object> extraValues, ref ICodeWriter codeWriter);
    public delegate void OnGeneratorFileWithListCodeWriterHandler(FileInfo fileInfo, string _namespace, IEnumerable list, ref ICodeWriter codeWriter);
    public delegate void OnFileGeneratedHandler(IVSProject project, string subPath, FileInfo fileInfo);

    public interface IGenerator
    {
        Exception LastException { get; }
        IEventsService EventsService { get; set; }
        ICodeGenerationPackage GenerateFrom(Dictionary<string, IElementBuild> builds, DirectoryInfo workspaceDirectory);
        ICodeGenerationPackage GenerateFromPackage(ICodeGenerationPackage incomingPackage);
        IPipelineTemplateEngineHost TemplateEngineHost { set; get; }
        IDomainHostApplication DomainHostApplication { set; get; }
        IProgrammingLanguage ProgrammingLanguage { get; }
        IPipelineStep PipelineStep { set; }
        string SDKFolder { set; get; }
        string DotNetFxFolder { set; get; }
        event WatchProcess OnWatchProcess;
        event OnFileGeneratedHandler OnFileGenerated;
        void OnError(int errorCode, Exception exception, string error);
        void OnProcessExited(int exitCode);
        void OnProcessTimeout();
        void OnOutput(string output);
        void OnFileCreated(FileInfo file);
        void OnFileChanged(FileInfo file);
        void OnFileRenamed(FileInfo file, FileInfo oldFile);
        void OnFileDeleted(FileInfo file);
        string Name { get; }
        string Description { get; }
        string WorkspaceDirectory { get; }
        Dictionary<string, FileInfo> Files { get; }
    }
}
