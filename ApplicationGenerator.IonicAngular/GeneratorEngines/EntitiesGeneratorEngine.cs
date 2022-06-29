// file:	GeneratorEngines\EntitiesGeneratorEngine.cs
//
// summary:	Implements the entities generator engine class

using EntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VisualStudioProvider;
using Utils;
using System.ComponentModel.DataAnnotations;
using AbstraX.DataAnnotations;
using AbstraX;
using AbstraX.ServerInterfaces;
using AbstraX.FolderStructure;
using AbstraX.Angular;
using System.Data.Entity;
using RestEntityProvider.Web.Entities;
using AbstraX.Models.Interfaces;
using System.IO;
using AbstraX.Handlers.FacetHandlers;
using AbstraX.Generators.Server.WebAPIController;
using Utils.Hierarchies;
using System.Xml;
using AbstraX.Handlers.ExpressionHandlers;
using System.Configuration;
using System.IO.Packaging;
using VisualStudioProvider.Configuration;
using Microsoft.Build.Framework;
using CodeInterfaces;
using AbstraX.TemplateObjects;
using AbstraX.Handlers.CommandHandlers;
using NetCoreReflectionShim.Agent;
using MailSlot;
using System.Text.RegularExpressions;

namespace AbstraX.GeneratorEngines
{
    /// <summary>   The entities generator engine. </summary>
    ///
    /// <remarks>   Ken, 10/1/2020. </remarks>

    public class EntitiesGeneratorEngine : IWorkspaceGeneratorEngine
    {
        /// <summary>   Gets or sets the generator configuration. </summary>
        ///
        /// <value> The generator configuration. </value>

        public GeneratorConfiguration GeneratorConfiguration { get; private set; }
        /// <summary>   The generator mode. </summary>
        private GeneratorMode generatorMode;
        /// <summary>   Options for controlling the generator. </summary>
        private GeneratorOptions generatorOptions;
        /// <summary>   Full pathname of the entities project file. </summary>
        private string entitiesProjectPath;
        /// <summary>   Type of the project. </summary>
        private Guid projectType;
        /// <summary>   The project folder root. </summary>
        private string projectFolderRoot;
        private string callbackName = CallbackNames.Foundation;
        public NetCoreReflectionAgent NetCoreReflectionAgent { get; private set; }
        public MailslotClient MailslotClient { get; private set; }
        public Queue<string> LogMessageQueue { get; }

        private IAppFolderStructureSurveyor appFolderStructureSurveyor;

        public int ParentProcessId { get; }
        public string GenerationName => "Entities";
        private int percentOfStart = 60;
        private int percentOf = 100;


        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="projectType">          Type of the project. </param>
        /// <param name="projectFolderRoot">    The project folder root. </param>
        /// <param name="templateFile">         The template file. </param>
        /// <param name="jsonFile">             The JSON file. </param>
        /// <param name="businessModelFile">    The business model file. </param>
        /// <param name="entitiesProjectPath">  Full pathname of the entities project file. </param>
        /// <param name="additionalOptions">    Options for controlling the additional. </param>
        /// <param name="generatorMode">        (Optional) The generator mode. </param>
        /// <param name="generatorOptions">     (Optional) Options for controlling the generator. </param>

        public EntitiesGeneratorEngine(Guid projectType, string projectFolderRoot, string templateFile, string jsonFile, string businessModelFile, string entitiesProjectPath, Dictionary<string, object> additionalOptions, GeneratorMode generatorMode = GeneratorMode.Console, GeneratorOptions generatorOptions = null)
        {
            var thisAssembly = Assembly.GetExecutingAssembly();
            var inputFiles = new Dictionary<string, string> { { "templateFile", templateFile }, { "jsonFile", jsonFile }, { "businessModelFile", businessModelFile }, { "entitiesProjectPath", entitiesProjectPath } };
            List<Type> types;

            this.projectType = projectType;
            this.projectFolderRoot = projectFolderRoot;
            this.NetCoreReflectionAgent = new NetCoreReflectionAgent(generatorOptions.DebugShimService, generatorOptions.RunAsAutomated);
            this.MailslotClient = generatorOptions.MailslotClient;
            this.ParentProcessId = generatorOptions.ParentProcessId;
            this.LogMessageQueue = new Queue<string>();
            this.appFolderStructureSurveyor = generatorOptions.AppFolderStructureSurveyor;

            this.SendCallbackStatus(callbackName, "Initializing entities", 0, percentOfStart, percentOf);

            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                this.NetCoreReflectionAgent.Dispose();
            };

            this.projectType = projectType;
            this.projectFolderRoot = projectFolderRoot;

            types = thisAssembly.GetAllTypes(this.NetCoreReflectionAgent).Distinct().OrderBy(t => t.FullName).ToList();

            this.GeneratorConfiguration = new GeneratorConfiguration(projectType, projectFolderRoot, inputFiles, additionalOptions, generatorOptions, this, types);
            this.generatorMode = generatorMode;
            this.generatorOptions = generatorOptions;
            this.entitiesProjectPath = entitiesProjectPath;

            this.SendCallbackStatus(callbackName, "Finished initializing entities", 10, percentOfStart, percentOf);
        }

        /// <summary>   Process this. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>

        public void Process()
        {
            var config = this.GeneratorConfiguration;
            var templateFile = config.InputFiles["templateFile"];
            var jsonFile = config.InputFiles["jsonFile"];
            var businessModelFile = config.InputFiles["businessModelFile"];
            var entitiesProjectPath = config.InputFiles["entitiesProjectPath"];

            WriteLine("\r\n**** CurrentPass: {0} {1}\r\n", PrintMode.Any, config.CurrentPass, "*".Repeat(25));

            if (!jsonFile.IsNullOrEmpty())
            {
                if (jsonFile == "default")
                {
                    jsonFile = Path.GetFullPath(Path.Combine(projectFolderRoot, "entities.json"));

                    if (!System.IO.File.Exists(jsonFile))
                    {
                        WriteError("No json file exists.  Searching for: {0}", jsonFile);
                        throw new HandlerNotFoundException("Entities model generation");
                    }
                }

                if (entitiesProjectPath == null)
                {
                    throw new ArgumentException("entitiesProjectPath argument is required when processing from json", "entitiesProjectPath");
                }

                if (!Path.IsPathRooted(jsonFile))
                {
                    jsonFile = Path.GetDirectoryName(Environment.ExpandEnvironmentVariables(jsonFile));
                }

                if (!Path.IsPathRooted(jsonFile))
                {
                    jsonFile = Path.GetFullPath(Path.Combine(projectFolderRoot, jsonFile));
                }

                if (businessModelFile.IsNullOrEmpty())
                {
                    businessModelFile = Path.GetFullPath(Path.Combine(projectFolderRoot, "businessModel.json"));

                    if (!System.IO.File.Exists(businessModelFile))
                    {
                        WriteError("No business model file exists. Searching for: {0}", businessModelFile);
                        throw new HandlerNotFoundException("Entities model generation");
                    }
                }

                if (!Path.IsPathRooted(businessModelFile))
                {
                    businessModelFile = Path.GetDirectoryName(Environment.ExpandEnvironmentVariables(businessModelFile));
                }

                if (!Path.IsPathRooted(businessModelFile))
                {
                    businessModelFile = Path.GetFullPath(Path.Combine(projectFolderRoot, businessModelFile));
                }

                if (entitiesProjectPath.IsNullOrEmpty())
                {
                    appFolderStructureSurveyor.DetermineLayout(projectFolderRoot);

                    entitiesProjectPath = this.appFolderStructureSurveyor.EntitiesProjectPath;
                }

                ProcessFromJson(jsonFile, businessModelFile, entitiesProjectPath);
            }
            else
            {
                if (templateFile.IsNullOrEmpty())
                {
                    templateFile = "default";
                }

                if (templateFile == "default")
                {
                    var assemblyLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

                    templateFile = Path.Combine(assemblyLocation, @"GeneratorTemplates\Default\entityDomainModel.template");
                    config.InputFiles["templateFile"] = templateFile;
                }
                else if (!templateFile.RegexIsMatch(@"[\\/\.]"))
                {
                    var assemblyLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

                    templateFile = Path.Combine(assemblyLocation, @"GeneratorTemplates\" + templateFile.ToLower() + @"\entityDomainModel.template");
                    config.InputFiles["templateFile"] = templateFile;
                }

                if (businessModelFile.IsNullOrEmpty())
                {
                    businessModelFile = Path.GetFullPath(Path.Combine(projectFolderRoot, "businessModel.json"));

                    if (!System.IO.File.Exists(businessModelFile))
                    {
                        WriteError("No business model file exists. Searching for: {0}", businessModelFile);
                        throw new HandlerNotFoundException("Entities model generation");
                    }
                }

                if (!Path.IsPathRooted(businessModelFile))
                {
                    businessModelFile = Path.GetDirectoryName(Environment.ExpandEnvironmentVariables(businessModelFile));
                }

                if (!Path.IsPathRooted(businessModelFile))
                {
                    businessModelFile = Path.GetFullPath(Path.Combine(projectFolderRoot, businessModelFile));
                }

                ProcessFromTemplate(templateFile, businessModelFile);
            }
        }

        /// <summary>   Process from template. </summary>
        ///
        /// <remarks>   Ken, 10/2/2020. </remarks>
        ///
        /// <param name="templateFile">         The template file. </param>
        /// <param name="businessModelJsonFile">    The business model file. </param>

        public void ProcessFromTemplate(string templateFile, string businessModelJsonFile)
        {
            var config = this.GeneratorConfiguration;
            var rootDirectory = new DirectoryInfo(projectFolderRoot);
            var solutionFile = rootDirectory.GetFiles("*.sln", SearchOption.TopDirectoryOnly).SingleOrDefault();
            var hydraJsonFile = rootDirectory.GetFiles("hydra.json", SearchOption.AllDirectories).FirstOrDefault();
            ConfigObject configObject = null;
            IEntitiesModelGeneratorHandler generatorHandler;
            FileInfo outputFile;
            BusinessModel businessModel;
            EntityDomainModel entityDomainModel;
            string fileName;
            string appName = null;
            string appDescription = null;
            string organizationName = null;

            if (hydraJsonFile != null)
            {
                configObject = ConfigObject.Load(hydraJsonFile.FullName);

                appName = configObject.AppName;
                appDescription = configObject.AppDescription;
                organizationName = configObject.OrganizationName;
            }

            if (solutionFile != null)
            {
                var solutionName = Path.GetFileNameWithoutExtension(solutionFile.Name);
                var solutionFolder = Path.GetDirectoryName(solutionFile.FullName);

                if (appName == null)
                {
                    appName = solutionName;
                    organizationName = solutionName;
                }

                fileName = Path.Combine(projectFolderRoot, "entities.json");
            }
            else
            {
                if (appName != null)
                {
                    fileName = Path.Combine(projectFolderRoot, "entities.json");
                }
                else
                {
                    fileName = Path.Combine(projectFolderRoot, "entities.json");

                    organizationName = "NoOrganization";
                    appName = "NoApp";
                }
            }

            config.KeyValuePairs = new Dictionary<string, object>();
            config.KeyValuePairs.Add("ConfigObject", configObject);
            config.AppName = appName;
            config.AppDescription = appDescription;

            outputFile = new FileInfo(fileName);

            if (config.CurrentPass == GeneratorPass.Files)
            {
                if (outputFile.Exists)
                {
                    outputFile.MakeWritable();
                }
            }

            WriteLine("Generating entity model from template");
            this.SendCallbackStatus(config.CurrentPass, callbackName, "Generating entity model from template", 20, percentOfStart, percentOf);

            entityDomainModel = config.CreateEntityDomainModelFromTemplate(templateFile, appName, appDescription, organizationName);

            this.SendCallbackStatus(config.CurrentPass, callbackName, "Loading business model", 30, percentOfStart, percentOf);

            businessModel = config.CreateBusinessModelFromJson(businessModelJsonFile);
            generatorHandler = config.GetEntitiesModelGeneratorHandler();

            if (generatorHandler != null)
            {
                WriteLine("Processing entity model");
                this.SendCallbackStatus(config.CurrentPass, callbackName, "Processing entity model", 45, percentOfStart, percentOf);

                generatorHandler.Process(entityDomainModel, businessModel, projectType, projectFolderRoot, templateFile, outputFile.FullName, config);

                if (config.CurrentPass == GeneratorPass.Files)
                {
                    WriteLine($"Generated { outputFile.FullName }");
                }
            }
            else
            {
                WriteError("No handler found implementing IBusinessModelGeneratorHandler");
                throw new HandlerNotFoundException("Business model generation");
            }

            this.SendCallbackStatus(config.CurrentPass, callbackName, "Processing entity model from template complete", 50, percentOfStart, percentOf);

            if (config.CurrentPass == GeneratorPass.Files)
            {
                WriteLine("Processing entity model from template complete");
            }
        }

        /// <summary>   Process from JSON. </summary>
        ///
        /// <remarks>   Ken, 10/2/2020. </remarks>
        ///
        /// <param name="entityModelJsonFile">      The entities JSON file. </param>
        /// <param name="businessModelJsonFile">    The business model file. </param>
        /// <param name="entitiesProjectPath">      Full pathname of the entities project file. </param>

        public void ProcessFromJson(string entityModelJsonFile, string businessModelJsonFile, string entitiesProjectPath)
        {
            var config = this.GeneratorConfiguration;
            var rootDirectory = new DirectoryInfo(projectFolderRoot);
            var solutionFile = rootDirectory.GetFiles("*.sln", SearchOption.TopDirectoryOnly).SingleOrDefault();
            var hydraJsonFile = rootDirectory.GetFiles("hydra.json", SearchOption.AllDirectories).FirstOrDefault();
            var jsonDirectory = new DirectoryInfo(Path.GetDirectoryName(hydraJsonFile.FullName));
            ConfigObject configObject = null;
            IModelAugmentationHandler modelAugmentationHandler;
            IEntitiesModelGeneratorHandler generatorHandler;
            BusinessModel businessModel;
            EntityDomainModel entityDomainModel;
            VSProject entitiesProject;
            string fileName;
            string appName = null;
            string appDescription = null;
            string organizationName = null;
            AppUIHierarchyNodeObject appUIHierarchyNodeObject = null;

            if (hydraJsonFile != null)
            {
                configObject = ConfigObject.Load(hydraJsonFile.FullName);

                appName = configObject.AppName;
                appDescription = configObject.AppDescription;
                organizationName = configObject.OrganizationName;
            }

            if (solutionFile != null)
            {
                var solutionName = Path.GetFileNameWithoutExtension(solutionFile.Name);
                var solutionFolder = Path.GetDirectoryName(solutionFile.FullName);

                if (appName == null)
                {
                    appName = solutionName;
                    organizationName = solutionName;
                }

                fileName = Path.Combine(projectFolderRoot, "entities.json");
            }
            else
            {
                if (appName != null)
                {
                    fileName = Path.Combine(projectFolderRoot, "entities.json");
                }
                else
                {
                    fileName = Path.Combine(projectFolderRoot, "entities.json");

                    organizationName = "NoOrganization";
                    appName = "NoApp";
                }
            }

            config.KeyValuePairs = new Dictionary<string, object>();
            config.KeyValuePairs.Add("ConfigObject", configObject);
            config.AppName = appName;
            config.AppDescription = appDescription;

            WriteLine("Loading entity model from json file");
            this.SendCallbackStatus(config.CurrentPass, callbackName, "Loading entity model from json file", 50, percentOfStart, percentOf);

            entityDomainModel = config.CreateEntityDomainModelFromJsonFile(entityModelJsonFile);

            WriteLine("Loading business model");
            this.SendCallbackStatus(config.CurrentPass, callbackName, "Loading business model", 60, percentOfStart, percentOf);

            businessModel = config.CreateBusinessModelFromJson(businessModelJsonFile);

            using (modelAugmentationHandler = config.GetModelAugmentationHandler())
            {
                var count = 0;

                entitiesProject = new VSProject(entitiesProjectPath);

                if (modelAugmentationHandler != null)
                {
                    WriteLine("Augmenting entity model for code generation");
                    this.SendCallbackStatus(config.CurrentPass, callbackName, "Augmenting entity model for code generation", 70, percentOfStart, percentOf);

                    modelAugmentationHandler.Process(entityDomainModel, businessModel, projectType, projectFolderRoot, entitiesProject, config, out appUIHierarchyNodeObject);
                }

                generatorHandler = config.GetEntitiesModelGeneratorHandler();

                if (generatorHandler != null)
                {
                    FolderStructure.FileSystemEventHandler fileSystemEvent;

                    WriteLine("Processing entity model and generating code files");
                    this.SendCallbackStatus(config.CurrentPass, callbackName, "Processing entity model and generating code files", 80, percentOfStart, percentOf);

                    fileSystemEvent = (sender, e) =>
                    {
                        ProjectFileSystemEvent(entitiesProject, e);
                        count++;
                    };

                    config.FileSystemEvent += fileSystemEvent;

                    generatorHandler.Process(entityDomainModel, businessModel, appUIHierarchyNodeObject, projectType, projectFolderRoot, entitiesProject, config);

                    config.FileSystemEvent -= fileSystemEvent;

                    if (config.CurrentPass == GeneratorPass.Files)
                    {
                        var dotNetCommandHandler = new DotNetCommandHandler();
                        var builder = new StringBuilder();
                        var regex = new Regex(@"\s(?<errors>\d*) Error\(s\)\s", RegexOptions.Multiline | RegexOptions.Singleline);
                        string fullOutput;
                        string error;

                        entitiesProject.Save();
                                                
                        WriteLine($"Generated { count } files.");

                        this.SendCallbackStatus(config.CurrentPass, callbackName, "Building entity model project", 90, percentOfStart, percentOf);

                        dotNetCommandHandler.OutputWriteLine = (format, args) =>
                        {
                            var output = string.Format(format, args);

                            builder.AppendLine(output);
                        };

                        dotNetCommandHandler.ErrorWriteLine = (format, args) =>
                        {
                            var output = string.Format(format, args);

                            builder.AppendLine(output);
                        };

                        dotNetCommandHandler.Build(Path.GetDirectoryName(entitiesProjectPath));

                        fullOutput = builder.ToString();

                        if (regex.IsMatch(fullOutput))
                        {
                            var match = regex.Match(fullOutput);
                            var errorCount = int.Parse(match.GetGroupValue("errors"));

                            if (errorCount > 0)
                            {
                                error = $"Build exception. { errorCount  } errors(s)";

                                WriteError(error);
                                throw new ValidationException(error);
                            }
                        }
                        else
                        {
                            error = "Unexpected output";

                            WriteError(error);
                            throw new InvalidDataException(error);
                        }
                    }
                }
                else
                {
                    WriteError("No handler found implementing IEntitiesModelGeneratorHandler");
                    throw new HandlerNotFoundException("Entities model generation");
                }
            }

            this.SendCallbackStatus(config.CurrentPass, callbackName, "Processing entity model from json complete", 95, percentOfStart, percentOf);
            WriteLine("Processing entity model from json complete");
        }

        private void ProjectFileSystemEvent(VSProject project, FolderStructure.FileSystemEventArgs e)
        {
            var projectPath = Path.GetDirectoryName(project.FileName);
            var fileInfo = e.FileInfo;

            if (fileInfo.FullName.AsCaseless().StartsWith(projectPath))
            {
                var existingItem = project.Items.FirstOrDefault(i => i.FilePath != null && i.FilePath.AsCaseless() == fileInfo.FullName);

                if (existingItem == null)
                {
                    switch (fileInfo.Extension)
                    {
                        case ".cs":
                            project.AddCompileFile(fileInfo.FullName);
                            break;
                        default:
                            DebugUtils.Break();
                            break;
                    }
                }
            }
        }

        /// <summary>   Gets the current tab text. </summary>
        ///
        /// <value> The current tab text. </value>

        public string CurrentTabText
        {
            get
            {
                var tabs = this.GeneratorConfiguration.IndentLevel;
                var tabText = new string(' ', tabs);

                return tabText;
            }
        }


        /// <summary>   Indents this.  </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>

        public void Indent()
        {
            this.GeneratorConfiguration.Indent();
        }

        /// <summary>   Dedents this.  </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>

        public void Dedent()
        {
            this.GeneratorConfiguration.Dedent();
        }

        /// <summary>   Writes a line. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="format">       Describes the format to use. </param>
        /// <param name="printMode">    The print mode. </param>
        /// <param name="args">         A variable-length parameters list containing arguments. </param>

        public void WriteLine(string format, PrintMode printMode, params object[] args)
        {
            var output = string.Format(format, args);

            if (output.Trim().Length > 0)
            {
                if ((generatorOptions.PrintMode == PrintMode.All && (!printMode.HasFlag(PrintMode.ExcludeFromAll))) || printMode.HasAnyFlag(generatorOptions.PrintMode) || printMode == PrintMode.Any)
                {
                    if (generatorMode == GeneratorMode.Console)
                    {
                        generatorOptions.OutputWriter.WriteLine(this.CurrentTabText + output);
                    }
                    else if (generatorMode == GeneratorMode.RedirectedConsole)
                    {
                        generatorOptions.RedirectedWriter.WriteLine(this.CurrentTabText + output);
                        generatorOptions.RedirectedWriter.Flush();
                    }
                }

                if (this.GeneratorConfiguration?.Logger != null)
                {
                    var logger = this.GeneratorConfiguration.Logger;

                    logger.Information(output);
                }
            }
        }

        /// <summary>   Writes an error. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="format">   Describes the format to use. </param>
        /// <param name="args">     A variable-length parameters list containing arguments. </param>

        public void WriteError(string format, params object[] args)
        {
            var output = string.Format(format, args);

            if (generatorMode == GeneratorMode.Console)
            {
                using (new ConsoleColorizer(ConsoleColor.Red))
                {
                    generatorOptions.OutputWriter.WriteLine(output);
                }
            }
            else if (generatorMode == GeneratorMode.RedirectedConsole)
            {
                generatorOptions.ErrorWriter.WriteLine(output);
                generatorOptions.ErrorWriter.Flush();
            }

            if (this.GeneratorConfiguration?.Logger != null)
            {
                var logger = this.GeneratorConfiguration.Logger;

                logger.Error(output);
            }
        }

        /// <summary>   Writes a line. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="format">   Describes the format to use. </param>
        /// <param name="args">     A variable-length parameters list containing arguments. </param>

        public void WriteLine(string format, params object[] args)
        {
            var output = string.Format(format, args);

            if (generatorMode == GeneratorMode.Console)
            {
                generatorOptions.OutputWriter.WriteLine(this.CurrentTabText + output);
            }
            else if (generatorMode == GeneratorMode.RedirectedConsole)
            {
                generatorOptions.RedirectedWriter.WriteLine(this.CurrentTabText + output);
                generatorOptions.RedirectedWriter.Flush();
            }

            if (this.GeneratorConfiguration?.Logger != null)
            {
                var logger = this.GeneratorConfiguration.Logger;

                logger.Information(output);
            }
        }

        /// <summary>   Resets this. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>

        public void Reset()
        {
            this.GeneratorConfiguration.Reset();
        }

        public void TestProcess()
        {
            throw new NotImplementedException();
        }

        public void EndProcessing(IGeneratorConfiguration generatorConfiguration)
        {
            this.SendCallbackStatus(generatorConfiguration.CurrentPass, callbackName, "Finished processing", 100, percentOfStart, percentOf);
        }
    }
}
