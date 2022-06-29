using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace HydraCrashAnalyzer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var processId = int.Parse(args[0]);
            var managedThreadId = int.Parse(args[1]);
            var threadId = int.Parse(args[2]);
            var dumpFile = args[3];
            var runAsAutomated = false;
            var process = Process.GetProcessById(processId);
            frmAnalyzer frmAnalyzer;

            if (args.Length > 4)
            {
                runAsAutomated = args[4] == "-runAsAutomated";
            }

            if (!runAsAutomated)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                frmAnalyzer = new frmAnalyzer();

                Task.Run(() =>
                {
                    try
                    {
                        var debugState = process.GetDebugState(managedThreadId);

                        frmAnalyzer.Invoke(() =>
                        {
                            frmAnalyzer.SetDebugInfo(debugState, dumpFile);
                        });

                        process.CreateDump(dumpFile, threadId);

                        frmAnalyzer.Invoke(() =>
                        {
                            frmAnalyzer.SetDumpFileCreated();
                        });
                    }
                    catch (Exception ex)
                    {
                        frmAnalyzer.Invoke(() =>
                        {
                            frmAnalyzer.SetInternalException(ex);
                        });
                    }
                });

                Application.Run(frmAnalyzer);
            }
            else
            {
                var analyzerReporter = new AnalyzerReporter();
                var completeEvent = new ManualResetEvent(false);

                Task.Run(() =>
                {
                    try
                    {
                        var debugState = process.GetDebugState(managedThreadId);

                        analyzerReporter.SetDebugInfo(debugState, dumpFile);

                        process.CreateDump(dumpFile, threadId);

                        analyzerReporter.SetDumpFileCreated();
                    }
                    catch (Exception ex)
                    {
                        analyzerReporter.SetInternalException(ex);
                    }

                    completeEvent.Set();
                });

                while (!completeEvent.WaitOne(100))
                {
                    Thread.Sleep(100);
                }
            }
        }
    }
}
