using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApplicationGenerator.Client.NodeInfo
{
    [Flags]
    public enum NodeInfoType
    {
        None = 0xffff00,
        Spinster = 0xffff01,
        Folder = 0xffff02,
        File = 0xffff04,
    }
}
