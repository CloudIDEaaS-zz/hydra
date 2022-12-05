using AbstraX.Handlers.WorkspaceHandlers.CSharpWorkspaceFileType;
using AbstraX.Handlers.WorkspaceHandlers.CSharpWorkspaceFileType.Nodes;
using CodeInterfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Utils;
using Utils.Hierarchies;

namespace AbstraX.Handlers.WorkspaceHandlers
{
    public class HydraJsonWorkspaceFileTypeHandler : IWorkspaceFileTypeHandler
    {
        public string[] FileNameExpressions { get; private set; }
        public string[] TokensToProcess { get; private set; }
        public string OutputContent { get; private set; }

        private string outputFileName;

        public float Priority => 1.0f;

        public HydraJsonWorkspaceFileTypeHandler()
        {
            this.FileNameExpressions = new string[] { @"hydra\.json$" };
        }

        public bool PreProcess(Guid projectType, string appName, string rawFileRelativePath, string outputFileName, string[] supportedTokens, string content, IGeneratorConfiguration generatorConfiguration)
        {

            return true;
        }

        public bool Process(Dictionary<string, IWorkspaceTokenContentHandler> tokenContentHandlers, IWorkspaceTemplate workspaceTemplate, Guid projectType, string appName, string rawFileRelativePath, string outputFileName, string content, IGeneratorConfiguration generatorConfiguration)
        {
            this.OutputContent = content;

            this.outputFileName = outputFileName;

            return true;
        }

        public void PostProcess(IAppFolderStructureSurveyor surveyor)
        {
            var appFolderStructureSurveyor = (AppFolderStructureSurveyor)surveyor;
            var hydraJsonFile = appFolderStructureSurveyor.HydraJsonFile;

            if (hydraJsonFile != null)
            {
                var path = hydraJsonFile.Directory;
                var newJsonFile = new FileInfo(outputFileName);

                newJsonFile.CreateShortcut(path.FullName);

                hydraJsonFile.Delete();
            }
        }
    }
}
