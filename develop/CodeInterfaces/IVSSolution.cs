using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeInterfaces
{
    public interface IVSSolution : IProjectRoot, ICodeFile, IDisposable
    {
        string Name { get; }
        string SolutionPath { get; }
        string FileName { get; }
        int ProjectCount { get; }
        IEnumerable<IVSProject> Projects { get; }
        IEnumerable<IVSProjectItem> EDMXModels { get; }
        IEnumerable<IVSProjectItem> Schemas { get; }
        void Reparse();
    }
}
