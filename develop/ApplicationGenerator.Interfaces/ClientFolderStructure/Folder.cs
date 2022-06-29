using AbstraX.FolderStructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Web.Script.Serialization;

namespace AbstraX.ClientFolderStructure
{
    [DebuggerDisplay(" { RelativeFullName } ")]
    public class Folder
    {
        public string Name { get; set; }
        public bool Exists { get; set; }
        public FileSystemObjectKind Kind { get; set; }
        public bool HasFolders { get; set; }
        public bool HasFiles { get; set; }
        public string FullName { get; set; }
        public string RelativeFullName { get; set; }
    }
}
