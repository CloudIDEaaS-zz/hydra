// file:	Program.cs
//
// summary:	Implements the program class

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EntityProvider.Web.Entities;
using Microsoft.CodeAnalysis.CSharp;
using Utils;
using VisualStudioProvider;
using AbstraX.FolderStructure;
using System.Security.Permissions;
using System.Configuration;
using Unity;
using System.Windows.Forms;
using Utils.NamedPipes;
using HydraDebugAssistant;
using System.Threading;
using System.Runtime.InteropServices;
using MailSlot;
using System.Globalization;

namespace AbstraX
{
    /// <summary>   A program. </summary>
    ///
    /// <remarks>   Ken, 10/10/2020. </remarks>

    public static class Program
    {
        /// <summary>   True to run unit tests. </summary>
        /// <summary>   The web API service. </summary>
        private static WebApiService webApiService;
        private static MailslotClient mailslotClient;
        private static Exception mailslotException;
        private static string currentWorkingDirectory;
        private static bool runAsAutomated;

        /// <summary>   Main entry-point for this application. </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="args"> An array of command-line argument strings. </param>

        [STAThread()]
        public static void Main(string[] args)
        {
            var waitForInput = false;
            var debugAttach = false;
            var logServiceMessages = false;
            var logPackageListing = false;
            var debugShimService = false;
            var useDynamicTemplates = false;
            string debugAssistantAddress = null;
            var runUnitTests = false;
            var testCrashAnalyzer = false;
            var useOverridesAssembly = false;
            string clientPipe = null;
            var mailslotName = "HydraStatus";
            var currentWorkingDirectory = Environment.CurrentDirectory;
            var thisProcess = Process.GetCurrentProcess();
            var process = Process.GetProcessesByName("ApplicationGenerator").Where(p => p.Id != thisProcess.Id).SingleOrDefault();
            var parentProcess = Process.GetCurrentProcess().GetParent();
            StandardStreamService streamService;
            IGeneratorHandler generatorHandler = null;
            ParseResult parseResult;

            LocaleExtensions.LoadApplicationCulture();

            parseResult = CommandLineParser.ParseArgs<ParseResult>(args, (result, arg) =>
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
                    case SwitchCommands.USE_OVERRIDES_ASSEMBLY:
                        useOverridesAssembly = true;
                        break;
                    case SwitchCommands.DEBUG_SHIM_SERVICE:
                        debugShimService = true;
                        break;
                    case SwitchCommands.WAIT_FOR_INPUT:
                        waitForInput = true;
                        break;
                    case SwitchCommands.RUN_AS_AUTOMATED:
                        runAsAutomated = true;
                        break;
                    case SwitchCommands.CLIENT_PIPE:
                        clientPipe = switchArg;
                        break;
                    case SwitchCommands.CWD:
                        currentWorkingDirectory = switchArg;
                        break;
                    case SwitchCommands.LOG_SERVICE_MESSAGES:
                        logServiceMessages = true;
                        break;
                    case SwitchCommands.LOG_PACKAGE_LISTING:
                        logPackageListing = true;
                        break;
                }
            });

            if (debugAttach)
            {
                AbstraXExtensions.DebugAttach(!waitForInput);
            }

            if (process != null)
            {
                process.Kill();
                process.WaitForExit();
            }

            Program.currentWorkingDirectory = currentWorkingDirectory;
            AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            debugAssistantAddress = ConfigurationManager.AppSettings["DebugAssistantAddress"];
            useDynamicTemplates = bool.Parse(ConfigurationManager.AppSettings["UseDynamicTemplates"]);
            runUnitTests = bool.Parse(ConfigurationManager.AppSettings["RunUnitTests"]);

            if (runUnitTests)
            {
                UnitTests.RunUnitTests();
            }

            if (testCrashAnalyzer)
            {
                throw new Exception("Testing crash analyzer");
            }

            try
            {
                mailslotClient = new MailslotClient(mailslotName);
            }
            catch (Exception ex)
            {
                if (!waitForInput)
                {
                    Console.WriteLine("Error creating mailslot, Exceptions: {0}", ex.Message);
                }

                mailslotClient = null;
                mailslotException = ex;
            }

            if (waitForInput)
            {
                List<string> debugPackageInstallsList = null;
                var url = ConfigurationManager.AppSettings["WebApiUrl"];
                var debugPackageInstalls = ConfigurationManager.AppSettings["DebugPackageInstalls"];
                var alertInfo = new AlertInfo
                {
                    AlertFromAddress = ConfigurationManager.AppSettings["AlertFromAddress"],
                    AlertToAddress = ConfigurationManager.AppSettings["AlertToAddress"],
                    AlertWhenLevel = ConfigurationManager.AppSettings["AlertWhenLevel"].Split(",", StringSplitOptions.RemoveEmptyEntries).Select(a => a.Trim()).ToList(),
                    AlertWhenIdle = TimeSpan.Parse(ConfigurationManager.AppSettings["AlertWhenIdle"]),
                    AlertUseSounds = bool.Parse(ConfigurationManager.AppSettings["AlertUseSounds"]),
                };

                if (debugPackageInstalls != null)
                {
                    debugPackageInstallsList = debugPackageInstalls.Split(",").Select(i => i.Trim()).ToList();
                }

                webApiService = new WebApiService(url, logServiceMessages);
                streamService = new StandardStreamService(webApiService, mailslotClient, mailslotException, logServiceMessages, useDynamicTemplates, runAsAutomated, debugAssistantAddress, alertInfo, debugPackageInstallsList);

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

                keyValuePair = parseResult.GetOverrides(useOverridesAssembly, currentWorkingDirectory).LastOrDefault();

                argumentsKind = keyValuePair.Key;
                generatorOverrides = keyValuePair.Value;

                if (testWebApi)
                {
                    var url = ConfigurationManager.AppSettings["WebApiUrl"];

                    webApiService = new WebApiService(url, logServiceMessages);
                    webApiService.GeneratorHandler = generatorHandler;
                    generatorHandler.SuppressDebugOutput = true;

                    Console.WriteLine($"Starting WebAPI Service at '{ url }'");

                    webApiService.Start();
                }

                if (argumentsKind == GeneratorArgumentsKind.GenerateHandlerArgumentInputs)
                {
                    var type = typeof(GeneratorArgumentsKind);

                    foreach (var kind in type.GetConstants().Select(f => (string)f.GetRawConstantValue()).Where(k => k != GeneratorArgumentsKind.GenerateHandlerArgumentInputs))
                    {
                        var arguments = generatorOverrides.GetHandlerArguments(packageCachePath, kind, currentWorkingDirectory);
                        var json = arguments.ToJsonText();
                    }
                }
                else if (argumentsKind != null)
                {
                    var arguments = generatorOverrides.GetHandlerArguments(packageCachePath, argumentsKind, currentWorkingDirectory);

                    arguments.Add("ParentProcess", parentProcess);

                    if (debugShimService)
                    {
                        var options = (GeneratorOptions)arguments["GeneratorOptions"];

                        options.DebugShimService = debugShimService;
                    }

                    if (runAsAutomated)
                    {
                        var options = (GeneratorOptions)arguments["GeneratorOptions"];

                        options.RunAsAutomated = runAsAutomated;
                    }

                    if (argumentsKind == GeneratorArgumentsKind.AddResourceBrowseFile)
                    {
                        StandardStreamService.AddResourceBrowseFile(arguments.Select(k => k).ToArray());
                    }
                    else if (argumentsKind == GeneratorArgumentsKind.AddResourceChooseColor)
                    {
                        StandardStreamService.AddResourceChooseColor(arguments.Select(k => k).ToArray());
                    }
                    else if (argumentsKind == GeneratorArgumentsKind.AddResourceCaptureImage)
                    {
                        StandardStreamService.AddResourceCaptureImage(arguments.Select(k => k).ToArray());
                    }
                    else if (argumentsKind == GeneratorArgumentsKind.ShowDesigner)
                    {
                        StandardStreamService.ShowDesigner(arguments.Select(k => k).ToArray());
                    }
                    else if (argumentsKind == GeneratorArgumentsKind.LaunchWeb)
                    {
                        MessageBox.Show("LaunchWeb");
                    }
                    else if (argumentsKind == GeneratorArgumentsKind.GenerateAppCore && !clientPipe.IsNullOrEmpty())
                    {
                        if (!clientPipe.IsNullOrEmpty())
                        {
                            arguments.Add("ClientPipe", clientPipe);
                        }

                        StandardStreamService.GenerateCompleteAppFrontend(arguments.Select(k => k).ToArray(), debugAttach);
                    }
                    else
                    {
                        var generatorHandlerType = (string)arguments.Single(a => a.Key == "GeneratorHandlerType").Value;

                        Console.WriteLine($"Loading handler for { generatorHandlerType }");

                        if (logPackageListing)
                        {
                            arguments.Add("LogPackageListing", true);
                        }

                        if (!arguments.ContainsKey("ArgumentsKind"))
                        {
                            arguments.Add("ArgumentsKind", argumentsKind);
                        }

                        arguments.Add("UseDynamicTemplates", useDynamicTemplates);
                        arguments.Add("DebugAssistantAddress", debugAssistantAddress);
                        arguments.Add("UseOverrides", useOverridesAssembly);
                        arguments.Add("MailslotClient", mailslotClient);
                        arguments.Add("Debug", debugAttach);
                        arguments.Add("RunAsAutomated", runAsAutomated);

                        generatorHandler = AbstraXExtensions.GetGeneratorHandler(generatorHandlerType);

                        if (arguments.ContainsKey("GeneratorKinds"))
                        {
                            var generatorKindsText = (string)arguments["GeneratorKinds"];
                            var generatorKinds = generatorKindsText.Split(',').Select(k => k.Trim());
                            var remainingKinds = generatorKinds.ToList();

                            Console.WriteLine($"Executing multiple generatorKinds: { generatorKindsText }");

                            foreach (var kind in generatorKinds)
                            {
                                Console.WriteLine($"\r\nExecuting { kind } {{0}}\r\n", "-".Repeat(50));

                                arguments = generatorOverrides.GetHandlerArguments(packageCachePath, kind, currentWorkingDirectory);

                                arguments.AddToDictionaryIfNotExist("ParentProcess", parentProcess);
                                arguments.AddAndOrUpdateDictionary("ArgumentsKind", kind);
                                arguments.AddToDictionaryIfNotExist("UseDynamicTemplates", useDynamicTemplates);
                                arguments.AddToDictionaryIfNotExist("DebugAssistantAddress", debugAssistantAddress);
                                arguments.AddToDictionaryIfNotExist("MailslotClient", mailslotClient);
                                arguments.AddToDictionaryIfNotExist("Debug", debugAttach);

                                remainingKinds.Remove(kind);

                                if (!clientPipe.IsNullOrEmpty())
                                {
                                    arguments.AddToDictionaryIfNotExist("ClientPipe", clientPipe);
                                    arguments.AddAndOrUpdateDictionary("RemainingKinds", remainingKinds.ToCommaDelimitedList());
                                }

                                generatorHandler.Execute(arguments);

                                Console.WriteLine($"\r\n{ kind } Complete.");

                                if (!clientPipe.IsNullOrEmpty())
                                {
                                    if (kind.IsOneOf(GeneratorArgumentsKind.GenerateStarterAppFrontend, GeneratorArgumentsKind.GenerateAppCore))
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            generatorHandler.Execute(arguments);
                        }
                    }
                }
                else
                {
                    Console.WriteLine(Properties.ApplicationGenerator.This_program_was_not_intended_to_be_run);
                }
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = (Exception)e.ExceptionObject;

            exception.HandleExitException(currentWorkingDirectory, runAsAutomated);
        }

        /// <summary>   Event handler. Called by CurrentDomain for domain unload events. </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>

        private static void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {
        }
    }
}
