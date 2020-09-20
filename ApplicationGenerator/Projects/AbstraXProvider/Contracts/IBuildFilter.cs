using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Collections;
using Metaspec;
using AbstraX.Contracts;
using AbstraX.Bindings;
using AbstraX.BuildEvents;
using AbstraX.Contracts.CodeParsers;

namespace AbstraX.Contracts
{
    public interface IBuildFilter
    {
        string Name { get; }
        bool Enabled { get; set; }
        void OnGenerationStarted();
        void OnGenerationComplete();
        void OnGenerationAbort();
        void OnBuildGeneralError(Exception ex);
        void OnBuildFatalError(Exception ex);
        void OnBuildGeneralError(Exception ex, string stackTrace);
        void OnBuildFatalError(Exception ex, string stackTrace);
        void OnGenerateFrom(List<IBindingsTree> bindings, out bool abortGeneration);
        void OnBindingsBuild(List<IBindingsTree> bindings, ref IBindingsBuilder builder);
        void OnStepsAdded(List<IPipelineStep> steps);
        void OnFileGenerated(string fileName);
        void OnStepPreGenerate(IPipelineStep step, Dictionary<string, IElementBuild> builds, DirectoryInfo workspaceDirectory, out bool skipStep, out bool abortGeneration);
        void OnStepPreGenerate(IPipelineStep step, ICodeGenerationPackage incomingPackage, out bool skipStep, out bool abortGeneration);
        void OnStepPostGenerate(IPipelineStep step, ICodeGenerationPackage outgoingPackage);
        void OnOutput(object sender, BuildMessageEventArgs eventArgs); //
        void OnGeneratorFileWithBuildPreBuild(IGenerator generator, ref Type generatorType, ref IElementBuild build, ref FileInfo fileInfo, ref string _namespace, ref Dictionary<string, object> extraValues, out bool skipBuildFile, out bool abortStep);
        void OnGeneratorFileWithListPreBuild(IGenerator generator, ref Type generatorType, ref FileInfo fileInfo, ref string _namespace, ref IEnumerable list, out bool skipBuildFile, out bool abortStep);
        void OnGeneratorFileWithBuildPostBuild(IGenerator generator, IElementBuild build, ref FileInfo fileInfo, string _namespace, Dictionary<string, object> extraValues, ICodeParser codeParser, ref string output);
        void OnGeneratorFileWithListPostBuild(IGenerator generator, ref FileInfo fileInfo, string _namespace, IEnumerable list, ICodeParser codeParser, ref string output);
        void OnGeneratorFileWithListCodeWriter(IGenerator generator, FileInfo fileInfo, string _namespace, IEnumerable list, ref ICodeWriter codeWriter);
        void OnGeneratorFileWithBuildCodeWriter(IGenerator generator, IElementBuild build, FileInfo fileInfo, string _namespace, Dictionary<string, object> extraValues, ref ICodeWriter codeWriter);
        void OnStepSkipped(IPipelineStep step);
        void OnGenerationAborted();
        void OnStepAborted(IGenerator generator, IPipelineStep lastStep, FileInfo lastFileInfo);
        void OnStepAborted(IGenerator generator, IPipelineStep lastStep);
        void OnBuildFileSkipped(IGenerator generator, IPipelineStep lastStep, FileInfo fileInfo);
    }
}
