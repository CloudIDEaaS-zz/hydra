using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydraDebugAssistant
{
    [Flags]
    public enum DebuggerState
    {
        None,
        BreakpointSet = 1 << 1,
        BreakpointPaused = 1 << 2,
        RequestContinue = (1 << 3) | BreakpointPaused,
        RequestRetry = (1 << 4) | BreakpointPaused,
    }
}
