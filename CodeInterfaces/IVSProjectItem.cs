using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;

namespace CodeInterfaces
{
    public interface IVSProjectItem : ICodeFile, IProjectStructure
    {
        byte[] FileContents { get; }
        T GetFileContents<T>();
        Stream FileStream { get; }
        bool HasMetadata { get; }
        string ItemInfo { get; }
        IEnumerable<IVSProjectMetadataElement> Metadata { get; }
        IVSProjectItem DependentUpon { get; }
        string Include { get; }
        string Name { get; }
        string ProjectPath { get; }
        bool IsLink { get; }
        bool IsHidden { get; }
        string ItemType { get; }
        string RelativePath { get; }
        string FilePath { get; }
        IVSProject ParentProject { get; }
        IVSProjectItem ParentItem { get; }
        int HierarchySort { get; }
        IEnumerable<IVSProjectItem> ChildItems { get; }
        void AddFromFile(string fileName);
        bool IsUnsaved { get; }
        bool IsSubItem { get; set; }
    }
}
