using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;

namespace AbstraX.Contracts
{
    public interface IVSProject : IProjectStructure, IDisposable
    {
        string Name { get; }
        string ProjectType { get; }
        string ProjectGuid { get; }
        string OutputFile { get; }
        string OutputPath { get; }
        string DefaultNamespace { get; }
        string RelativePath { get; }
        string FileName { get; }
        IVSSolution ParentSolution { get; }
        XPathNodeIterator Select(string xPath);
        string XPathNamespacePrefix { get; }
        IEnumerable<IVSProjectItem> EDMXModels { get; }
        IEnumerable<IVSProjectItem> Schemas { get; }
        IEnumerable<IVSProjectItem> Items { get; }
        IEnumerable<IVSProjectProperty> Properties { get; }
        bool IsSilverlightApplication { get; }
        string XAPFilename { get; }
        IEnumerable<ISilverlightApp> SilverlightApplicationList { get; }
        IEnumerable<IVSProject> SilverlightWebTargets { get; }
        void Reparse();
    }
}
