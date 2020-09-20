using AbstraX.ServerInterfaces;
using CodeInterfaces;
using EntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AbstraX
{
    public interface IWorkspaceFileTypeHandler : IHandler
    {
        string[] FileNameExpressions { get; }
        string[] TokensToProcess { get; }
        string OutputContent { get; }
        bool PreProcess(Guid projectType, string appName, string rawFileRelativePath, string outputFileName, string[] supportedTokens, string content, IGeneratorConfiguration generatorConfiguration);
        bool Process(Dictionary<string, IWorkspaceTokenContentHandler> tokenContentHandlers, IWorkspaceTemplate workspaceTemplate, Guid projectType, string appName, string rawFileRelativePath, string outputFileName, string content, IGeneratorConfiguration generatorConfiguration);
    }
}
