using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydraDebugAssistant.Info
{
    [Serializable]
    public class RootFolderInfo
    {
        public string Path { get; set; }
        public string DebuggerAttachedProcess { get; set; }
        public string DebuggerRemoteProcess { get; set; }
        public List<BreakpointInfo> Breakpoints { get; set; }

        public RootFolderInfo()
        {
            this.Breakpoints = new List<BreakpointInfo>();
        }

        public RootFolderInfo(string path) : this()
        {
            this.Path = path;
        }
    }
}
