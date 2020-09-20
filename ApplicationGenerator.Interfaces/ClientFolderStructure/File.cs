using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using Utils;
using AbstraX.FolderStructure;
using System.Diagnostics;

namespace AbstraX.ClientFolderStructure
{
    [DebuggerDisplay(" { RelativeFullName } ")]
    public class File
    {
        public string Name { get; set; }
        public bool Exists { get; set; }
        public string FolderName { get; set; }
        public long Length { get; set; }
        public FileSystemObjectKind Kind { get; set; }
        public string FullName { get; set; }
        public string RelativeFullName { get; set; }
    }
}
