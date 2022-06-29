using System;
using System.Collections.Generic;
using System.Text;

namespace AbstraX.FolderStructure
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FileSystemTypeAttribute : Attribute
    {
        public Enum Type { get; protected set; }
    }
}
