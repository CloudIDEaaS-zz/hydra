using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydraDebugAssistant.Info
{
    [Serializable]
    public class BreakpointInfo
    {
        public string FilePath { get; set; }
        public bool Enabled { get; set; }

        public BreakpointInfo()
        {
        }

        public BreakpointInfo(string filePath, bool enabled = false)
        {
            this.FilePath = filePath;
            this.Enabled = enabled;
        }
    }
}
