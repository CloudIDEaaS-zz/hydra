using DllExport;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CLIBuilderProcessor
{
    public static class Processor
    {
        [DllExport("Process", CallingConvention = CallingConvention.Winapi)]
        public unsafe static void Process(IntPtr options, IntPtr context)
        {
            Debugger.Launch();
        }
    }
}
