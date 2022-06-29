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
    public abstract class CodeFileNode : FileNode
    {
        public abstract IVSProjectItem ProjectItem { get; }

        public string RelativePath
        {
            get
            {
                return this.ProjectItem.RelativePath;
            }
        }

        public override FileInfo FileInfo
        {
            get 
            {
                return new FileInfo(this.ProjectItem.FilePath);
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
