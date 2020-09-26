using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AbstraX.Handlers.CommandHandlers;
using EntityProvider.Web.Entities;
using Microsoft.CodeAnalysis.CSharp;
using Utils;
using VisualStudioProvider;
using AbstraX.FolderStructure;
using System.Security.Permissions;
using System.Configuration;
using Unity;
 
namespace AbstraX
{
    public static class Program
    {
        private const bool RUN_UNIT_TESTS = true;
        private static WebApiService webApiService;

        public static void Main(string[] args)
        {
            var waitForInput = false;
            var debugAttach = false;
            var currentWorkingDirectory = Environment.CurrentDirectory;
            var parentProcess = Process.GetCurrentProcess().GetParent();
            StandardStreamService streamService;
            GeneratorHandler generatorHandler = null;
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
                    case SwitchCommands.WAIT_FOR_INPUT:
                        waitForInput = true;
                        break;
                    case SwitchCommands.CWD:
                        currentWorkingDirectory = switchArg;
                        break;
                }
            });

            AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;

            if (debugAttach)
            {
                Debugger.Launch();
            }

            if (waitForInput)
            {
                var url = ConfigurationManager.AppSettings["WebApiUrl"];

                webApiService = new WebApiService(url);
                streamService = new StandardStreamService(webApiService);

                streamService.Start(parentProcess, currentWorkingDirectory);
                webApiService.Start();

                streamService.Wait();
            }
            else
            {
                var packageCachePath = Environment.ExpandEnvironmentVariables(@"%APPDATA%\hydra\cache");
                var testWebApi = Environment.CommandLine.Contains("/TestWebApi");
                var container = new UnityContainer();
                KeyValuePair<string, IGeneratorOverrides> keyValuePair;
                IGeneratorOverrides generatorOverrides;
                string argumentsKind;

                AppDomain.CurrentDomain.SetData("UnityContainer", container);

                generatorHandler = new GeneratorHandler();
                keyValuePair = generatorHandler.GetOverrides().LastOrDefault();

                argumentsKind = keyValuePair.Key;
                generatorOverrides = keyValuePair.Value;

                if (testWebApi)
                {
                    var url = ConfigurationManager.AppSettings["WebApiUrl"];

                    webApiService = new WebApiService(url);
                    webApiService.GeneratorHandler = generatorHandler;
                    generatorHandler.SuppressDebugOutput = true;

                    Console.WriteLine($"Starting WebAPI Service at '{ url }'");

                    webApiService.Start();
                }

                if (RUN_UNIT_TESTS)
                {
                    UnitTests.RunUnitTests();
                }

                if (argumentsKind != null)
                {
                    generatorHandler.Execute(generatorOverrides.GetHandlerArguments(packageCachePath, argumentsKind));
                }
                else
                {
                    Console.WriteLine("This program was not intended to be run directly");
                }
            }
        }

        private static void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {
        }
    }
}
