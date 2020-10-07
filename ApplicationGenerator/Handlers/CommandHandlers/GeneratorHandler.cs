// file:	Handlers\CommandHandlers\GeneratorHandler.cs
//
// summary:	Implements the generator handler class

using AbstraX.GeneratorEngines;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Handlers.CommandHandlers
{
    /// <summary>   A generator handler. </summary>
    ///
    /// <remarks>   Ken, 10/5/2020. </remarks>

    [Command("Generate")]
    public class GeneratorHandler : ICommandHandler
    {
        /// <summary>   Gets or sets the generator engine. </summary>
        ///
        /// <value> The generator engine. </value>

        public IGeneratorEngine GeneratorEngine { get; private set; }

        /// <summary>   Gets or sets a value indicating whether the suppress debug output. </summary>
        ///
        /// <value> True if suppress debug output, false if not. </value>

        public bool SuppressDebugOutput { get; internal set; }

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
            var parentProcess = Process.GetCurrentProcess().GetParent();
            var runFromVs = false;
            var folderRoot = Environment.CurrentDirectory;
            var generatorKind = (GeneratorKind)arguments["GeneratorKind"];
            var options = (GeneratorOptions)arguments["GeneratorOptions"];
            var mode = (GeneratorMode)arguments["GeneratorMode"];
            GeneratorConfiguration config;
            Dictionary<string, object> additionalOptions = null;
            string projectFolderRoot;

            if (arguments.ContainsKey("AdditionalOptions"))
            {
                additionalOptions = (Dictionary<string, object>)arguments["AdditionalOptions"];
            }

            if (generatorKind == GeneratorKind.App)
            {
                var entitiesProjectPath = (string)arguments["EntitiesProjectPath"];
                var servicesProjectPath = (string)arguments["ServicesProjectPath"];
                string servicesFolderRoot;
                string packageCachePath = null;

                if (arguments.ContainsKey("PackageCachePath"))
                {
                    packageCachePath = (string)arguments["PackageCachePath"];
                }

                if (parentProcess.ProcessName.IsOneOf("devenv", "msvsmon"))
                {
                    folderRoot = Path.GetFullPath(Path.Combine(folderRoot, @"..\..\TestOutput"));
                    projectFolderRoot = Path.Combine(folderRoot, @"AngularProject");
                    servicesFolderRoot = Path.Combine(folderRoot, @"Services");

                    runFromVs = true;
                }
                else
                {
                    projectFolderRoot = folderRoot;
                    servicesFolderRoot = Path.GetDirectoryName(servicesProjectPath);
                }

                if (!File.Exists(entitiesProjectPath))
                {
                    throw new IOException(string.Format("Entities project does not exist. Project file: '{0}'", entitiesProjectPath));
                }

                if (!File.Exists(servicesProjectPath))
                {
                    throw new IOException(string.Format("Services project does not exist. Project file: '{0}'", servicesProjectPath));
                }

                options.ApplicationFolderHierarchy = new IonicFolderHierarchy(folderRoot, projectFolderRoot, servicesFolderRoot);

                this.GeneratorEngine = new AppGeneratorEngine(ProjectTypes.Ionic, projectFolderRoot, entitiesProjectPath, servicesProjectPath, packageCachePath, additionalOptions, mode, options);
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
            }
            else if (generatorKind == GeneratorKind.Workspace)
            {
                var appName = (string)arguments["AppName"];
                var appDescription = (string)arguments["AppDescription"];
                var organizationName = (string)arguments["OrganizationName"];

                if (parentProcess.ProcessName.IsOneOf("devenv", "msvsmon"))
                {
                    folderRoot = Path.GetFullPath(Path.Combine(folderRoot, @"..\..\TestOutput"));
                    projectFolderRoot = folderRoot;

                    runFromVs = true;
                }
                else
                {
                    projectFolderRoot = folderRoot;
                }

                if (System.IO.Directory.Exists(projectFolderRoot))
                {
                    var directory = new DirectoryInfo(projectFolderRoot);

                    directory.Backup();

                    directory.ForceDeleteAllFilesAndSubFolders();
                }
                else
                {
                    System.IO.Directory.CreateDirectory(projectFolderRoot);
                }

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
                    folderRoot = Path.GetFullPath(Path.Combine(folderRoot, @"..\..\TestOutput"));
                }
            }
            else if (generatorKind == GeneratorKind.BusinessModel)
            {
                var templateFile = (string)arguments["TemplateFile"];

                if (parentProcess.ProcessName.IsOneOf("devenv", "msvsmon"))
                {
                    folderRoot = Path.GetFullPath(Path.Combine(folderRoot, @"..\..\TestOutput"));
                    projectFolderRoot = folderRoot;

                    runFromVs = true;
                }
                else
                {
                    projectFolderRoot = folderRoot;
                }

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
                    folderRoot = Path.GetFullPath(Path.Combine(folderRoot, @"..\..\TestOutput"));
                }
            }
            else if (generatorKind == GeneratorKind.Entities)
            {
                var templateFile = (string)arguments.SingleOrDefault(a => a.Key == "TemplateFile").Value;
                var jsonFile = (string)arguments.SingleOrDefault(a => a.Key == "JsonFile").Value;
                var businessModelFile = (string)arguments.SingleOrDefault(a => a.Key == "BusinessModelFile").Value;
                var entitiesProjectPath = (string)arguments.SingleOrDefault(a => a.Key == "EntitiesProjectPath").Value;

                if (parentProcess.ProcessName.IsOneOf("devenv", "msvsmon"))
                {
                    folderRoot = Path.GetFullPath(Path.Combine(folderRoot, @"..\..\TestOutput"));
                    projectFolderRoot = folderRoot;

                    runFromVs = true;
                }
                else
                {
                    projectFolderRoot = folderRoot;
                }

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
                    folderRoot = Path.GetFullPath(Path.Combine(folderRoot, @"..\..\TestOutput"));
                }
            }

            if (runFromVs)
            {
                KeyValuePair<string, IGeneratorOverrides> keyValuePair;
                IGeneratorOverrides generatorOverrides;
                string argumentsKind;

                keyValuePair = this.GeneratorEngine.GetOverrides().Last();

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

        /// <summary>   Gets the generator configuration. </summary>
        ///
        /// <value> The generator configuration. </value>

        public GeneratorConfiguration GeneratorConfiguration
        {
            get
            {
                return this.GeneratorEngine.GeneratorConfiguration;
            }
        }

    }
}
        