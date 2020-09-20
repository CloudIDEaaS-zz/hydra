using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstraX.Contracts
{
    public interface IVSSolution : IProjectRoot, IDisposable
    {
        string Name { get; }
        string SolutionPath { get; }
        string FileName { get; }
        IEnumerable<IVSProject> Projects { get; }
        IEnumerable<IVSProjectItem> EDMXModels { get; }
        IEnumerable<IVSProjectItem> Schemas { get; }
        void Reparse();
    }
}
