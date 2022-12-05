// file:	Handlers\CommandHandlers\GeneratorHandler.cs
//
// summary:	Implements the generator handler class

using AbstraX.GeneratorEngines;
using MailSlot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Handlers.CommandHandlers
{
    /// <summary>   A generator handler. </summary>
    ///
    /// <remarks>   Ken, 10/5/2020. </remarks>

    [Command("Generate")]
    public class GeneratorHandler : ICommandHandler, IGeneratorHandler
    {
        /// <summary>   Gets or sets the generator engine. </summary>
        ///
        /// <value> The generator engine. </value>

        public IGeneratorEngine GeneratorEngine { get; private set; }

        /// <summary>   Gets or sets a value indicating whether the suppress debug output. </summary>
        ///
        /// <value> True if suppress debug output, false if not. </value>

        public bool SuppressDebugOutput { get; set; }

        /// <summary>   Executes the given arguments. </summary>
        ///
        /// <remarks>   Ken, 10/5/2020. </remarks>
        ///
        /// <param name="arguments">    The arguments. </param>

        public void Execute(params KeyValuePair<string, object>[] arguments)
        {
            this.Execute(arguments.ToDictionary(a => a.Key, a => a.Value));
        }

        /// <summary>   Executes the given arguments. </summary>
        ///
        /// <remarks>   Ken, 10/5/2020. </remarks>
        ///
        /// <exception cref="IOException">  Thrown when an IO failure occurred. </exception>
        ///
        /// <param name="arguments">    The arguments. </param>

        public void Execute(Dictionary<string, object> arguments)
        {
            var runFromVs = false;
            var folderRoot = Environment.CurrentDirectory;
            var generatorKind = (GeneratorKind)arguments["GeneratorKind"];
            var appFolderStructureSurveyor = new AppFolderStructureSurveyor();
            var debug = false;
            var runAsAutomated = false;
            Process parentProcess = null;
            GeneratorOptions options = null;
            GeneratorMode mode = GeneratorMode.Console;
            IGeneratorConfiguration config = null;
            Dictionary<string, object> additionalOptions = null;
            string projectFolderRoot;
            var useOverrides = false;
            MailslotClient mailslotClient = null;

            appFolderStructureSurveyor.DetermineLayout(arguments);

            if (arguments.ContainsKey("GeneratorOptions"))
            {
                options = (GeneratorOptions)arguments["GeneratorOptions"];
                debug = options.Debug;
            }

            if (arguments.ContainsKey("MailslotClient"))
            {
                mailslotClient = (MailslotClient)arguments["MailslotClient"];
            }

            if (arguments.ContainsKey("UseOverrides"))
            {
                useOverrides = (bool)arguments["UseOverrides"];
            }

            if (arguments.ContainsKey("GeneratorMode"))
            {
                mode = (GeneratorMode)arguments["GeneratorMode"];
            }

            if (!debug && arguments.ContainsKey("Debug"))
            {
                debug = (bool)arguments["Debug"];
            }

            if (!runAsAutomated && arguments.ContainsKey("RunAsAutomated"))
            {
                runAsAutomated = (bool)arguments["RunAsAutomated"];
            }

            if (arguments.ContainsKey("AdditionalOptions"))
            {
                additionalOptions = (Dictionary<string, object>)arguments["AdditionalOptions"];
            }

            if (arguments.ContainsKey("ParentProcess"))
            {
                parentProcess = (Process)arguments["ParentProcess"];
            }

            if (mode != GeneratorMode.RedirectedConsole)
            {
                Console.WriteLine($"Generator kind requested '{ generatorKind }'");
            }
            else
            {
                options.RedirectedWriter.WriteLine($"Generator kind requested '{ generatorKind }'");
            }

            options.AppFolderStructureSurveyor = appFolderStructureSurveyor;
            options.MailslotClient = mailslotClient;
            options.UseOverrides = useOverrides;
            options.RunAsAutomated = runAsAutomated;

            if (parentProcess != null)
            {
                options.ParentProcessId = parentProcess.Id;
            }

            if (generatorKind == GeneratorKind.App)
            {
                var entitiesProjectPath = (string)arguments["EntitiesProjectPath"];
                var servicesProjectPath = (string)arguments["ServicesProjectPath"];
                var logPackageListing = false;
                var useDynamicTemplates = false;
                string servicesFolderRoot;
                string packageCachePath = null;
                string webFrontEndRootPath = null;
                string clientPipe = null;
                string debugAssistantAddress = null;
                List<string> debugPackageInstalls = null;

                if (arguments.ContainsKey("DebugPackageInstalls"))
                {
                    debugPackageInstalls = (List<string>)arguments["DebugPackageInstalls"];
                }

                if (arguments.ContainsKey("PackageCachePath"))
                {
                    packageCachePath = (string)arguments["PackageCachePath"];
                }

                if (arguments.ContainsKey("WebFrontEndRootPath"))
                {
                    webFrontEndRootPath = (string)arguments["WebFrontEndRootPath"];

                    if (!Path.IsPathRooted(webFrontEndRootPath))
                    {
                        webFrontEndRootPath = Path.GetFullPath(Path.Combine(appFolderStructureSurveyor.ProjectFolderRoot, webFrontEndRootPath));
                    }

                    folderRoot = webFrontEndRootPath;
                }

                if (arguments.ContainsKey("LogPackageListing"))
                {
                    logPackageListing = (bool)arguments["LogPackageListing"];
                }

                if (arguments.ContainsKey("UseDynamicTemplates"))
                {
                    useDynamicTemplates = (bool)arguments["UseDynamicTemplates"];
                }

                if (arguments.ContainsKey("DebugAssistantAddress"))
                {
                    debugAssistantAddress = (string)arguments["DebugAssistantAddress"];
                }

                if (arguments.ContainsKey("ClientPipe"))
                {
                    clientPipe = (string)arguments["ClientPipe"];
                }

                if (parentProcess.ProcessName.IsOneOf("devenv", "msvsmon"))
                {
                    runFromVs = true;
                }

                if (!Path.IsPathRooted(entitiesProjectPath))
                {
                    entitiesProjectPath = Path.GetFullPath(Path.Combine(appFolderStructureSurveyor.ProjectFolderRoot, entitiesProjectPath));
                }

                if (!Path.IsPathRooted(servicesProjectPath))
                {
                    servicesProjectPath = Path.GetFullPath(Path.Combine(appFolderStructureSurveyor.ProjectFolderRoot, servicesProjectPath));
                }

                projectFolderRoot = folderRoot;
                servicesFolderRoot = Path.GetDirectoryName(servicesProjectPath);

                if (!Directory.Exists(folderRoot))
                {
                    throw new IOException(string.Format("Root folder specified for web root of client application does not exist. Folder: '{0}'", folderRoot));
                }

                if (!File.Exists(entitiesProjectPath))
                {
                    throw new IOException(string.Format("Entities project does not exist. Project file: '{0}'", entitiesProjectPath));
                }

                if (!File.Exists(servicesProjectPath))
                {
                    throw new IOException(string.Format("Services project does not exist. Project file: '{0}'", servicesProjectPath));
                }

                if (mailslotClient != null)
                {
                    mailslotClient.SendMessage("Initiating app generator engine");
                }

                if (mode != GeneratorMode.RedirectedConsole)
                {
                    Console.WriteLine("Initiating app generator engine");
                }
                else
                {
                    options.RedirectedWriter.WriteLine("Initiating app generator engine");
                }

                options.ApplicationFolderHierarchy = new IonicFolderHierarchy(folderRoot, projectFolderRoot, servicesFolderRoot, Path.GetDirectoryName(entitiesProjectPath));
                options.LogPackageListing = logPackageListing;
                options.UseDynamicTemplates = useDynamicTemplates;
                options.DebugAssistantAddress = debugAssistantAddress;
                options.DebugPackageInstalls = debugPackageInstalls;

                this.GeneratorEngine = new AppGeneratorEngine(ProjectTypes.Ionic, projectFolderRoot, entitiesProjectPath, servicesProjectPath, packageCachePath, additionalOptions, mode, options);
                config = this.GeneratorConfiguration;
                config.SuppressDebugOutput = this.SuppressDebugOutput;

                if (options.GeneratorPass == GeneratorPass.All)
                {
                    var engine = this.GeneratorEngine;

                    foreach (var pass in EnumUtils.GetValues<GeneratorPass>().Skip(2))
                    {
                        config.CurrentPass = pass;

                        if (options.TestMode == TestMode.TestProcessIterateOnly)
                        {
                            engine.TestProcess();
                        }
                        else
                        {
                            engine.Process();
                        }

                        if (pass != GeneratorPassCommon.Last)
                        {
                            engine.Reset();
                        }
                    }
                }
                else
                {
                    config.CurrentPass = options.GeneratorPass;
                    this.GeneratorEngine.Process();
                }
            }
            else if (generatorKind == GeneratorKind.Workspace)
            {
                var appName = (string)arguments["AppName"];
                var appDescription = (string)arguments["AppDescription"];
                var organizationName = (string)arguments["OrganizationName"];

                if (parentProcess.ProcessName.IsOneOf("devenv", "msvsmon"))
                {
                    folderRoot = appFolderStructureSurveyor.ProjectFolderRoot;
                    projectFolderRoot = folderRoot;

                    runFromVs = true;
                }
                else
                {
                    projectFolderRoot = folderRoot;
                }

                if (mode != GeneratorMode.RedirectedConsole)
                {
                    options.OutputWriter.WriteLine($"Attempting to delete files and folders in '{ projectFolderRoot }'");
                }
                else
                {
                    options.RedirectedWriter.WriteLine($"Attempting to delete files and folders in '{ projectFolderRoot }'");
                }

                if (mailslotClient != null)
                {
                    mailslotClient.SendMessage("Attempting to delete files and folders in projectFolderRoot");
                }

                if (System.IO.Directory.Exists(projectFolderRoot))
                {
                    try
                    {
                        var directory = new DirectoryInfo(projectFolderRoot);

                        directory.Backup();

                        directory.ForceDeleteAllFilesAndSubFolders(false, (f) =>
                        {
                            if (f.FileSystemInfo is FileInfo file)
                            {
                                if (file.DirectoryName == directory.FullName && (file.Name == "hydra.json" || file.Name == "hydra.json.lnk"))
                                {
                                    return false;
                                }
                                else if (file.Directory.GetAncestors(true).Any(d => d.Parent != null && d.Parent.FullName == directory.FullName && d.Name == "hydra_resources"))
                                {
                                    return false;
                                }
                            }
                            else if (f.FileSystemInfo is DirectoryInfo directory2)
                            {
                                if (directory2.Parent.FullName == directory.FullName && directory2.Name == "hydra_resources")
                                {
                                    return false;
                                }
                                else if (directory2.GetAncestors(true).Any(d => d.Parent != null && d.Parent.FullName == directory.FullName && d.Name == "hydra_resources"))
                                {
                                    return false;
                                }
                            }

                            return true;
                        });
                    }
                    catch (Exception ex)
                    {
                        mailslotClient.SendMessage($"Error deleting files and folders in projectFolderRoot. You may have later problems, Error: { ex.Message }");
                    }
                }
                else
                {
                    System.IO.Directory.CreateDirectory(projectFolderRoot);
                }

                if (mailslotClient != null)
                {
                    mailslotClient.SendMessage("Initiating workspace generator engine");
                }

                if (mode != GeneratorMode.RedirectedConsole)
                {
                    Console.WriteLine("Initiating workspace generator engine");
                }
                else
                {
                    options.RedirectedWriter.WriteLine("Initiating workspace generator engine");
                }

                options.AppFolderStructureSurveyor = appFolderStructureSurveyor;

                this.GeneratorEngine = new WorkspaceGeneratorEngine(ProjectTypes.Ionic, projectFolderRoot, appName, appDescription, organizationName, additionalOptions, mode, options);
                config = this.GeneratorConfiguration;
                config.SuppressDebugOutput = this.SuppressDebugOutput;

                if (options.GeneratorPass == GeneratorPass.All)
                {
                    var engine = this.GeneratorEngine;

                    foreach (var pass in EnumUtils.GetValues<GeneratorPass>().Skip(2))
                    {
                        config.CurrentPass = pass;
                        engine.Process();

                        if (pass != GeneratorPassCommon.Last)
                        {
                            engine.Reset();
                        }
                    }
                }
                else
                {
                    config.CurrentPass = options.GeneratorPass;
                    this.GeneratorEngine.Process();
                }

                if (parentProcess.ProcessName.IsOneOf("devenv", "msvsmon"))
                {
                    folderRoot = appFolderStructureSurveyor.ProjectFolderRoot;
                }
            }
            else if (generatorKind == GeneratorKind.BusinessModel)
            {
                var templateFile = (string)arguments["TemplateFile"];

                if (parentProcess.ProcessName.IsOneOf("devenv", "msvsmon"))
                {
                    folderRoot = appFolderStructureSurveyor.ProjectFolderRoot;
                    projectFolderRoot = folderRoot;

                    runFromVs = true;
                }
                else
                {
                    projectFolderRoot = folderRoot;
                }

                if (mode != GeneratorMode.RedirectedConsole)
                {
                    Console.WriteLine("Initiating business model generator engine");
                }
                else
                {
                    options.RedirectedWriter.WriteLine("Initiating business model generator engine");
                }

                options.AppFolderStructureSurveyor = appFolderStructureSurveyor;

                this.GeneratorEngine = new BusinessModelGeneratorEngine(ProjectTypes.Ionic, projectFolderRoot, templateFile, additionalOptions, mode, options);
                config = this.GeneratorConfiguration;
                config.SuppressDebugOutput = this.SuppressDebugOutput;

                if (options.GeneratorPass == GeneratorPass.All)
                {
                    var engine = this.GeneratorEngine;

                    foreach (var pass in EnumUtils.GetValues<GeneratorPass>().Skip(2))
                    {
                        config.CurrentPass = pass;
                        engine.Process();

                        if (pass != GeneratorPassCommon.Last)
                        {
                            engine.Reset();
                        }
                    }
                }
                else
                {
                    config.CurrentPass = options.GeneratorPass;
                    this.GeneratorEngine.Process();
                }

                if (parentProcess.ProcessName.IsOneOf("devenv", "msvsmon"))
                {
                    folderRoot = appFolderStructureSurveyor.ProjectFolderRoot;
                }
            }
            else if (generatorKind == GeneratorKind.Entities)
            {
                var templateFile = (string)arguments.SingleOrDefault(a => a.Key == "TemplateFile").Value;
                var jsonFile = (string)arguments.SingleOrDefault(a => a.Key == "JsonFile").Value;
                var businessModelFile = (string)arguments.SingleOrDefault(a => a.Key == "BusinessModelFile").Value;
                var entitiesProjectPath = (string)arguments.SingleOrDefault(a => a.Key == "EntitiesProjectPath").Value;
                var servicesProjectPath = (string)arguments.SingleOrDefault(a => a.Key == "ServicesProjectPath").Value;
                FileInfo solutionFile;
                DirectoryInfo rootDirectory;

                if (parentProcess.ProcessName.IsOneOf("devenv", "msvsmon"))
                {
                    folderRoot = appFolderStructureSurveyor.ProjectFolderRoot;
                    projectFolderRoot = folderRoot;

                    runFromVs = true;
                }
                else
                {
                    projectFolderRoot = folderRoot;
                }

                rootDirectory = new DirectoryInfo(projectFolderRoot);
                solutionFile = rootDirectory.GetFiles("*.sln", SearchOption.TopDirectoryOnly).SingleOrDefault();

                if (solutionFile == null)
                {
                    solutionFile = rootDirectory.Parent.Parent.GetFiles("*.sln", SearchOption.TopDirectoryOnly).SingleOrDefault();
                }

                if (rootDirectory.GetParts().Length > solutionFile.Directory.GetParts().Length)
                {
                    rootDirectory = solutionFile.Directory;

                    folderRoot = rootDirectory.FullName;
                    projectFolderRoot = folderRoot;
                }

                if (CompareExtensions.AllAreNotNull(servicesProjectPath, entitiesProjectPath))
                {
                    options.ApplicationFolderHierarchy = new IonicFolderHierarchy(folderRoot, projectFolderRoot, Path.GetDirectoryName(servicesProjectPath), Path.GetDirectoryName(entitiesProjectPath));
                }
                else
                {
                    appFolderStructureSurveyor.DetermineLayout(projectFolderRoot);

                    servicesProjectPath = appFolderStructureSurveyor.ServicesProjectPath;
                    entitiesProjectPath = appFolderStructureSurveyor.EntitiesProjectPath;

                    if (CompareExtensions.AllAreNotNull(servicesProjectPath, entitiesProjectPath))
                    {
                        options.ApplicationFolderHierarchy = new IonicFolderHierarchy(folderRoot, projectFolderRoot, Path.GetDirectoryName(servicesProjectPath), Path.GetDirectoryName(entitiesProjectPath));
                    }
                    else
                    {
                        if (mode != GeneratorMode.RedirectedConsole)
                        {
                            if (servicesProjectPath == null)
                            {
                                Console.WriteLine("Warning: servicesProjectPath == null");
                            }

                            if (entitiesProjectPath == null)
                            {
                                Console.WriteLine("Warning: entitiesProjectPath == null");
                            }
                        }
                        else
                        {
                            if (servicesProjectPath == null)
                            {
                                options.RedirectedWriter.WriteLine("Warning: servicesProjectPath == null");
                            }

                            if (entitiesProjectPath == null)
                            {
                                options.RedirectedWriter.WriteLine("Warning: entitiesProjectPath == null");
                            }
                        }
                    }
                }

                if (mode != GeneratorMode.RedirectedConsole)
                {
                    Console.WriteLine("Initiating entities generator engine");
                }
                else
                {
                    options.RedirectedWriter.WriteLine("Initiating entities generator engine");
                }

                options.AppFolderStructureSurveyor = appFolderStructureSurveyor;

                this.GeneratorEngine = new EntitiesGeneratorEngine(ProjectTypes.Ionic, projectFolderRoot, templateFile, jsonFile, businessModelFile, entitiesProjectPath, additionalOptions, mode, options);
                config = this.GeneratorConfiguration;
                config.SuppressDebugOutput = this.SuppressDebugOutput;

                if (options.GeneratorPass == GeneratorPass.All)
                {
                    var engine = this.GeneratorEngine;

                    foreach (var pass in EnumUtils.GetValues<GeneratorPass>().Skip(2))
                    {
                        config.CurrentPass = pass;
                        engine.Process();

                        if (pass != GeneratorPassCommon.Last)
                        {
                            engine.Reset();
                        }
                    }
                }
                else
                {
                    config.CurrentPass = options.GeneratorPass;
                    this.GeneratorEngine.Process();
                }

                if (parentProcess.ProcessName.IsOneOf("devenv", "msvsmon"))
                {
                    folderRoot = appFolderStructureSurveyor.ProjectFolderRoot;
                }
            }
            else if (generatorKind == GeneratorKind.Utility)
            {
                var argumentsKind = (string)arguments["ArgumentsKind"];

                if (argumentsKind == GeneratorArgumentsKind.GenerateStarterAppFrontend)
                {
                    string root = null;
                    string appName;
                    var currentWorkingDirectory = Environment.CurrentDirectory;
                    var thisAssembly = Assembly.GetExecutingAssembly();
                    IGeneratorConfiguration generatorConfiguration;
                    List<Type> types;

                    types = thisAssembly.GetAllTypes(null).Distinct().OrderBy(t => t.FullName).ToList();

                    generatorConfiguration = new GeneratorConfiguration(types);

                    using (var utilityHandler = generatorConfiguration.GetUtilityHandler(arguments))
                    {
                        System.IO.DirectoryInfo directory;

                        if (arguments != null)
                        {
                            root = (string)arguments.SingleOrDefault(a => a.Key.AsCaseless() == "WebFrontEndRootPath").Value;
                        }

                        if (root == null)
                        {
                            root = currentWorkingDirectory;
                        }

                        directory = new System.IO.DirectoryInfo(root);
                        Environment.CurrentDirectory = directory.Parent.FullName;

                        appName = directory.Name;

                        utilityHandler.GenerateStarterAppFrontend(appName, Environment.CurrentDirectory, debug);
                    }
                }
                else
                {
                    DebugUtils.Break();
                }
            }
            else
            {
                DebugUtils.Break();
            }

            if (config != null && mode == GeneratorMode.Console)
            {
                this.GeneratorEngine.EndProcessing(config);

                config.StopServices();
            }

            if (runFromVs)
            {
                KeyValuePair<string, IGeneratorOverrides> keyValuePair;
                IGeneratorOverrides generatorOverrides;
                string argumentsKind;

                keyValuePair = this.GeneratorEngine.GetOverrides(useOverrides, appFolderStructureSurveyor.TopMostFolder).Last();

                argumentsKind = keyValuePair.Key;
                generatorOverrides = keyValuePair.Value;

                if (generatorOverrides.CopiesToAlternateLocation)
                {
                    Console.Clear();
                    Console.WriteLine("Copy to override folders? (Y/N)");

                    if (Console.ReadKey().Key == ConsoleKey.Y)
                    {
                        Console.Clear();
                        Console.WriteLine("Copying...");

                        generatorOverrides.CopyFiles(this.GeneratorConfiguration, argumentsKind);
                    }
                }
            }
        }

        public void EndProcessing(IGeneratorConfiguration generatorConfiguration)
        {
            if (this.GeneratorEngine != null)
            {
                this.GeneratorEngine.EndProcessing(generatorConfiguration);
            }
        }

        /// <summary>   Gets the generator configuration. </summary>
        ///
        /// <value> The generator configuration. </value>

        public IGeneratorConfiguration GeneratorConfiguration
        {
            get
            {
                return this.GeneratorEngine.GeneratorConfiguration;
            }
        }
    }
}
        