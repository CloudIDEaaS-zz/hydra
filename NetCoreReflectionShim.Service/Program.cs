using System;
using System.Diagnostics;
using System.IO;
using Utils;
using System.Windows.Forms;
using System.Reflection;
using AbstraX;
using System.Linq;
using System.Collections.Generic;

namespace NetCoreReflectionShim.Service
{
    public static class Program
    {
        private static List<Type> allTypes;
        private static bool runAsAutomated;

        [STAThread]
        public static void Main(string[] args)
        {
            StandardStreamService streamService;
            var parentProcess = Process.GetCurrentProcess().GetParent();
            var debugAttach = false;
            var testCrashAnalyzer = false;
            var parseResult = CommandLineParser.ParseArgs<ParseResult>(args, (result, arg) =>
            {
            },
            (result, _switch, switchArg) =>
            {
                switch (_switch)
                {
                    case SwitchCommands.DEBUG:
                        debugAttach = true;
                        break;
                    case SwitchCommands.TEST_CRASH_ANALYZER:
                        testCrashAnalyzer = true;
                        break;
                    case SwitchCommands.RUN_AS_AUTOMATED:
                        runAsAutomated = true;
                        break;
                }
            });

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            if (debugAttach)
            {
                if (MessageBox.Show("Debug Shim Service?", "Debug Shim Service", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Debugger.Launch();
                }
            }

            if (testCrashAnalyzer)
            {
                throw new Exception("Testing crash analyzer");
            }

            streamService = new StandardStreamService(runAsAutomated);

            streamService.Start(parentProcess, Directory.GetCurrentDirectory());

            streamService.Started += (sender, e) =>
            {
                var entryAssembly = Assembly.GetEntryAssembly();

                allTypes = entryAssembly.GetAllTypes().Distinct().ToList();

            };

            streamService.GetTypeProxyEvent += (sender, e) =>
            {
                e.ProxyType = allTypes.FindProxyType(e.TypeToProxy);
            };

            streamService.Wait();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = (Exception)e.ExceptionObject;

            exception.HandleExitException(Environment.CurrentDirectory, runAsAutomated);
        }
    }
}
