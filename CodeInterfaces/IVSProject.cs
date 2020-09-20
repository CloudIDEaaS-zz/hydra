using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;

namespace CodeInterfaces
{
    public interface IVSProject : IProjectStructure, ICodeFile, IDisposable
    {
        string Name { get; }
        string ProjectType { get; }
        string ProjectGuid { get; }
        string ParentProjectGuid { get; }
        string OutputFile { get; }
        string OutputPath { get; }
        string DefaultNamespace { get; }
        string RelativePath { get; }
        IVSSolution ParentSolution { get; }
        XPathNodeIterator Select(string xPath);
        string XPathNamespacePrefix { get; }
        IEnumerable<IVSProjectItem> EDMXModels { get; }
        IEnumerable<IVSProjectItem> RestModels { get; }
        IEnumerable<IVSProjectItem> Schemas { get; }
        IEnumerable<IVSProjectItem> Items { get; }
        IEnumerable<IVSProjectItem> CompileItems { get; }
        IEnumerable<IVSProjectProperty> Properties { get; }
        IEnumerable<string> ReferencedAssemblies { get; }
        IEnumerable<IVSProject> ReferencedProjects { get; }
        bool IsSilverlightApplication { get; }
        bool IsWebApplication { get; }
        string XAPFilename { get; }
        IEnumerable<ISilverlightApp> SilverlightApplicationList { get; }
        IEnumerable<IVSProject> SilverlightWebTargets { get; }
        void Reparse(bool includeHiddenItems = false);
        IVSProject ParentProject { get; }
        List<IVSProject> ChildProjects { get; }
        string ParentHierarchy { get; }
        string Hierarchy { get; }
        bool IncludeHiddenItems { get; set; }
        void AddCompileFile(string fileNameSource, string relativeTargetPath = null);
        void AddItem(string fileNameSource, string itemType, string relativeTargetPath = null);
        void AddItem(string fileNameSource, string itemType, IVSProjectItem dependentUpon);
        void RemoveItem(string relativePath);
        bool HasItem(string relativePath);
        void Save();
    }
}
