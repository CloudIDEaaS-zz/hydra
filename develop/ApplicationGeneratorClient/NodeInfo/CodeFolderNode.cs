using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeInterfaces;
using System.IO;
using System.Diagnostics;
using Utils;

namespace Net2Html5ConfigurationTool.NodeInfo
{
    [DebuggerDisplay(" { DebugInfo } ")]
    public abstract class CodeFolderNode : DirectoryNode
    {
        public abstract IVSProjectItem ProjectItem { get; }

        public string RelativePath
        {
            get
            {
                return this.ProjectItem.RelativePath;
            }
        }

        public string DebugInfo
        {
            get
            {
                return string.Format("{0}, {1}, {2}", this.ProjectItem.Name, this.ProjectItem.Include, this.ProjectItem.ItemType);
            }
        }
    }
}
