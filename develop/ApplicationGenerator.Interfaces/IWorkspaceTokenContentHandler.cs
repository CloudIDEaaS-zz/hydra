using AbstraX.ServerInterfaces;
using EntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AbstraX
{
    public interface IWorkspaceTokenContentHandler : IHandler
    {
        string[] Tokens { get; }
        string Content { get;}
        bool Process(Guid projectType, string appName, string rawFileRelativePath, string outputFileName, string[] supportedTokens);
    }
}
