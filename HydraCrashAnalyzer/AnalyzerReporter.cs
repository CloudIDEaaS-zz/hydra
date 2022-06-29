using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace HydraCrashAnalyzer
{
    public class AnalyzerReporter : IAnalyzer
    {
        private string processName;
        private string exceptionMessage;
        private StringBuilder builder;
        private string logFileName;

        public AnalyzerReporter()
        {
            builder = new StringBuilder();
        }

        public void SetDebugInfo(DebugState debugState, string dumpFile)
        {
            var file = new FileInfo(dumpFile);
            var logFile = file.Directory.GetFiles("*.log").First();

            processName = debugState.ProcessName;
            exceptionMessage = debugState.ExceptionMessage;

            builder.AppendLine(debugState.ExceptionText);
            builder.AppendLine(string.Empty);
            builder.AppendLine(debugState.StackTrace.ToString());

            logFileName = logFile.FullName;
        }

        public void SetDumpFileCreated()
        {
            SubmitDetails();
        }

        private void SubmitDetails()
        {
        }

        public void SetInternalException(Exception ex)
        {
            processName = Process.GetCurrentProcess().ProcessName;
            exceptionMessage = ex.Message;

            builder.AppendLine(ex.ToString());

            SubmitDetails();
        }
    }
}
