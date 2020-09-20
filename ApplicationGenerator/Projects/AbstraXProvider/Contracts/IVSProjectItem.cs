using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace AbstraX.Contracts
{
    public interface IVSProjectItem : IProjectStructure
    {
        byte[] FileContents { get; }
        T GetFileContents<T>();
        bool HasMetadata { get; }
        IEnumerable<IVSProjectMetadataElement> Metadata { get; }
        string Name { get; }
        string ItemType { get; }
        string RelativePath { get; }
        string FilePath { get; }
        IVSProject ParentProject { get; }
    }
}
