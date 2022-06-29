using Microsoft.VisualStudio.Services.Agent.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.VisualStudio.Services.Agent
{
    public class HydraTraceListener : TextWriterTraceListener
    {
        public HydraTraceListener() : base()
        {
        }

        // Copied and modified slightly from .Net Core source code. Modification was required to make it compile.
        // There must be some TraceFilter extension class that is missing in this source code.
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
        }

        public override void WriteLine(string message)
        {
            base.WriteLine(message);
            Flush();
        }

        public override void Write(string message)
        {
            base.Write(message);
            Flush();
        }
    }
}