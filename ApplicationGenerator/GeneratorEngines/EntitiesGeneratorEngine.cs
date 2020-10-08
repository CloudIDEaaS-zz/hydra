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
            var thisAssembly = Assembly.GetEntryAssembly();
            var inputFiles = new Dictionary<string, string> { { "templateFile", templateFile }, { "jsonFile", jsonFile }, { "businessModelFile", businessModelFile }, { "entitiesProjectPath", entitiesProjectPath } };
            List<Type> types;

            this.projectType = projectType;
            this.projectFolderRoot = projectFolderRoot;

            types = thisAssembly.GetAllTypes().OrderBy(t => t.Name).ToList();

            this.GeneratorConfiguration = new GeneratorConfiguration(projectType, projectFolderRoot, inputFiles, additionalOptions, generatorOptions, this, types);
            this.generatorMode = generatorMode;
            this.generatorOptions = generatorOptions;
            this.entitiesProjectPath = entitiesProjectPath;
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
            var directory = new DirectoryInfo(projectFolderRoot);

            WriteLine("\r\n**** CurrentPass: {0} {1}\r\n", PrintMode.Any, config.CurrentPass, "*".Repeat(25));

            if (!jsonFile.IsNullOrEmpty())
            {
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
                    if (!Path.IsPathRooted(businessModelFile))
                    {
                        businessModelFile = Path.GetDirectoryName(Environment.ExpandEnvironmentVariables(businessModelFile));
                    }

                    if (!Path.IsPathRooted(businessModelFile))
                    {
                        businessModelFile = Path.GetFullPath(Path.Combine(projectFolderRoot, businessModelFile));
                    }
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
            var directory = new DirectoryInfo(projectFolderRoot);
            var solutionFile = directory.GetFiles("*.sln", SearchOption.TopDirectoryOnly).SingleOrDefault();
            var hydraJsonFile = directory.GetFiles("hydra.json", SearchOption.AllDirectories).FirstOrDefault();
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

            entityDomainModel = config.CreateEntityDomainModelFromTemplate(templateFile, appName, appDescription, organizationName);

            WriteLine("Loading business model");

            businessModel = config.CreateBusinessModelFromJson(businessModelJsonFile);
            generatorHandler = config.GetEntitiesModelGeneratorHandler();

            if (generatorHandler != null)
            {
                WriteLine("Processing entity model");

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
            var directory = new DirectoryInfo(projectFolderRoot);
            var solutionFile = directory.GetFiles("*.sln", SearchOption.TopDirectoryOnly).SingleOrDefault();
            var hydraJsonFile = directory.GetFiles("hydra.json", SearchOption.AllDirectories).FirstOrDefault();
            ConfigObject configObject = null;
            IModelAugmentationHandler modelAugmentationHandler;
            IEntitiesModelGeneratorHandler generatorHandler;
            FileInfo outputFile;
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

            outputFile = new FileInfo(fileName);

            if (config.CurrentPass == GeneratorPass.Files)
            {
                if (outputFile.Exists)
                {
                    outputFile.MakeWritable();
                }
            }

            WriteLine("Loading entity model from json file");

            entityDomainModel = config.CreateEntityDomainModelFromJsonFile(entityModelJsonFile);

            WriteLine("Loading business model");

            businessModel = config.CreateBusinessModelFromJson(businessModelJsonFile);

            modelAugmentationHandler = config.GetModelAugmentationHandler();
            entitiesProject = new VSProject(entitiesProjectPath);

            if (modelAugmentationHandler != null)
            {
                WriteLine("Augmenting entity model for code generation");

                modelAugmentationHandler.Process(entityDomainModel, businessModel, projectType, projectFolderRoot, entitiesProject, config, out appUIHierarchyNodeObject);
            }

            generatorHandler = config.GetEntitiesModelGeneratorHandler();

            if (generatorHandler != null)
            {
                WriteLine("Processing entity model and generating code file");

                generatorHandler.Process(entityDomainModel, businessModel, appUIHierarchyNodeObject, projectType, projectFolderRoot, entitiesProject, config);

                if (config.CurrentPass == GeneratorPass.Files)
                {
                    WriteLine($"Generated { outputFile.FullName }");
                }
            }
            else
            {
                WriteError("No handler found implementing IEntitiesModelGeneratorHandler");
                throw new HandlerNotFoundException("Entities model generation");
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
                        Console.WriteLine(this.CurrentTabText + output);
                    }
                    else if (generatorMode == GeneratorMode.RedirectedConsole)
                    {
                        generatorOptions.OutputWriter.WriteLine(this.CurrentTabText + output);
                        generatorOptions.OutputWriter.Flush();
                    }
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
                    Console.WriteLine(output);
                }
            }
            else if (generatorMode == GeneratorMode.RedirectedConsole)
            {
                generatorOptions.ErrorWriter.WriteLine(output);
                generatorOptions.ErrorWriter.Flush();
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
                Console.WriteLine(this.CurrentTabText + output);
            }
            else if (generatorMode == GeneratorMode.RedirectedConsole)
            {
                generatorOptions.OutputWriter.WriteLine(this.CurrentTabText + output);
                generatorOptions.OutputWriter.Flush();
            }
        }

        /// <summary>   Resets this. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>

        public void Reset()
        {
            this.GeneratorConfiguration.Reset();
        }
    }
}
