using System;
using System.Diagnostics;
using System.IO;
using Utils;
using System.Windows.Forms;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using AssemblyAttributesShim;
using Serilog;

namespace AssemblyAttributesService
{
    public static class Program
    {
        private static string logLocation;
        private static ILogger logger;

        [STAThread]
        public static void Main(string[] args)
        {
            StandardStreamService streamService;
            var parentProcess = Process.GetCurrentProcess().GetParent();
            var debugAttach = false;
            var assembly = Assembly.GetEntryAssembly();
            var name = assembly.GetNameParts().AssemblyName;
            string logFile;

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
                    case SwitchCommands.LOG_LOCATION:
                        logLocation = switchArg;
                        break;
                }
            });

            logFile = Path.Combine(logLocation, name + ".log");

            logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(logFile, rollingInterval: RollingInterval.Day)
                .CreateLogger().ForContext<StandardStreamService>();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            if (debugAttach)
            {
                Debugger.Launch();
            }

            streamService = new StandardStreamService(logger);

            streamService.Start(parentProcess, Directory.GetCurrentDirectory());

            streamService.Started += (sender, e) =>
            {
                logger.Information("Stream service started");
            };

            streamService.Wait();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = (Exception)e.ExceptionObject;

            logger.Error(exception.ToString());
        }
    }
}
