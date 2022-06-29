using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ApplicationGenerator.Client.NodeInfo
{
    public abstract class DirectoryNode : NodeInfoBase
    {
        public abstract DirectoryInfo DirectoryInfo { get; }
    }
}
