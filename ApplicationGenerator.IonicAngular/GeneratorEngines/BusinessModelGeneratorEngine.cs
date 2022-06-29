// file:	GeneratorEngines\BusinessModelGeneratorEngine.cs
//
// summary:	Implements the business model generator engine class

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
using NetCoreReflectionShim.Agent;
using MailSlot;

namespace AbstraX.GeneratorEngines
{
    /// <summary>   The business model generator engine. </summary>
    ///
    /// <remarks>   Ken, 10/1/2020. </remarks>

    public class BusinessModelGeneratorEngine : IWorkspaceGeneratorEngine
    {
        /// <summary>   Gets or sets the generator configuration. </summary>
        ///
        /// <value> The generator configuration. </value>

        public GeneratorConfiguration GeneratorConfiguration { get; private set; }
        /// <summary>   The generator mode. </summary>
        private GeneratorMode generatorMode;
        /// <summary>   Options for controlling the generator. </summary>
        private GeneratorOptions generatorOptions;
        /// <summary>   Type of the project. </summary>
        private Guid projectType;
        /// <summary>   The project folder root. </summary>
        private string projectFolderRoot;
        private string callbackName = CallbackNames.Foundation;
        public NetCoreReflectionAgent NetCoreReflectionAgent { get; private set; }
        public MailslotClient MailslotClient { get; private set; }
        public Queue<string> LogMessageQueue { get; }
        public int ParentProcessId { get; }
        public string GenerationName => "BusinessModel";
        public int percentOfStart = 30;
        public int percentOf = 60;

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="projectType">          Type of the project. </param>
        /// <param name="projectFolderRoot">    The project folder root. </param>
        /// <param name="templateFile">         The template file. </param>
        /// <param name="additionalOptions">    Options for controlling the additional. </param>
        /// <param name="generatorMode">        (Optional) The generator mode. </param>
        /// <param name="generatorOptions">     (Optional) Options for controlling the generator. </param>

        public BusinessModelGeneratorEngine(Guid projectType, string projectFolderRoot, string templateFile, Dictionary<string, object> additionalOptions, GeneratorMode generatorMode = GeneratorMode.Console, GeneratorOptions generatorOptions = null)
        {
            var thisAssembly = Assembly.GetExecutingAssembly();
            var inputFiles = new Dictionary<string, string> { { "templateFile", templateFile } };
            List<Type> types;

            this.projectType = projectType;
            this.projectFolderRoot = projectFolderRoot;
            this.NetCoreReflectionAgent = new NetCoreReflectionAgent(generatorOptions.DebugShimService, generatorOptions.RunAsAutomated);
            this.MailslotClient = generatorOptions.MailslotClient;
            this.ParentProcessId = generatorOptions.ParentProcessId;
            this.LogMessageQueue = new Queue<string>();

            this.SendCallbackStatus(callbackName, "Initializing business model", 10, percentOfStart, percentOf);

            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                this.NetCoreReflectionAgent.Dispose();
            };

            types = thisAssembly.GetAllTypes(this.NetCoreReflectionAgent).Distinct().OrderBy(t => t.FullName).ToList();

            this.GeneratorConfiguration = new GeneratorConfiguration(projectType, projectFolderRoot, inputFiles, additionalOptions, generatorOptions, this, types);
            this.generatorMode = generatorMode;
            this.generatorOptions = generatorOptions;

            this.SendCallbackStatus(callbackName, "Finished initializing business model", 20, percentOfStart, percentOf);
        }

        /// <summary>   Process this.  </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <exception cref="ArgumentException">        Thrown when one or more arguments have
        ///                                             unsupported or illegal values. </exception>
        /// <exception cref="HandlerNotFoundException"> Thrown when a Handler Not Found error condition
        ///                                             occurs. </exception>

        public void Process()
        {
            var config = this.GeneratorConfiguration;
            var templateFile = config.InputFiles["templateFile"];
            var directory = new DirectoryInfo(projectFolderRoot);
            var solutionFile = directory.GetFiles("*.sln", SearchOption.TopDirectoryOnly).SingleOrDefault();
            var hydraJsonFile = directory.GetFiles("hydra.json", SearchOption.AllDirectories).FirstOrDefault();
            ConfigObject configObject = null;
            IBusinessModelGeneratorHandler generatorHandler;
            FileInfo outputFile;
            BusinessModel businessModel;
            string fileName;
            string appName = null;
            string appDescription = null;
            string organizationName = null;

            WriteLine("\r\n**** CurrentPass: {0} {1}\r\n", PrintMode.Any, config.CurrentPass, "*".Repeat(25));

            if (templateFile.IsNullOrEmpty())
            {
                templateFile = "default";
            }

            if (templateFile == "default")
            {
                var assemblyLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

                templateFile = Path.Combine(assemblyLocation, @"GeneratorTemplates\Default\businessModel.template");
                config.InputFiles["templateFile"] = templateFile;
            }
            else if (!templateFile.RegexIsMatch(@"[\\/\.]"))
            {
                var assemblyLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

                templateFile = Path.Combine(assemblyLocation, @"GeneratorTemplates\" + templateFile.ToLower() + @"\businessModel.template");
                config.InputFiles["templateFile"] = templateFile;
            }

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
                
                fileName = Path.Combine(projectFolderRoot, "businessModel.json");
            }
            else
            {
                if (appName != null)
                {
                    fileName = Path.Combine(projectFolderRoot, "businessModel.json");
                }
                else
                {
                    fileName = Path.Combine(projectFolderRoot, "businessModel.json");

                    organizationName = "NoOrganization";
                    appName = "NoApp";
                    appDescription = "NoApp Description";
                }
            }

            config.AppName = appName;
            config.AppDescription = appDescription;
            config.KeyValuePairs = new Dictionary<string, object>();
            config.KeyValuePairs.Add("ConfigObject", configObject);

            outputFile = new FileInfo(fileName);

            if (config.CurrentPass == GeneratorPass.Files)
            {
                if (outputFile.Exists)
                {
                    outputFile.MakeWritable();
                }
            }

            WriteLine("Generating business model");
            this.SendCallbackStatus(config.CurrentPass, callbackName, "Generating business model", 35, percentOfStart, percentOf);


            businessModel = config.CreateBusinessModelFromTemplate(templateFile, appName, appDescription, organizationName);
            generatorHandler = config.GetBusinessModelGeneratorHandler();

            if (generatorHandler != null)
            {
                WriteLine("Processing business model");
                this.SendCallbackStatus(config.CurrentPass, callbackName, "Processing business model", 65, percentOfStart, percentOf);

                generatorHandler.Process(businessModel, projectType, projectFolderRoot, templateFile, outputFile.FullName, config);

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

            this.SendCallbackStatus(callbackName, "Processing business model complete", 100, percentOfStart, percentOf);

            if (config.CurrentPass == GeneratorPass.Files)
            {
                WriteLine("Processing business model complete");
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
                var tabText = new string(' ', tabs * 2);

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
                        generatorOptions.OutputWriter.WriteLine(output);
                    }
                    else if (generatorMode == GeneratorMode.RedirectedConsole)
                    {
                        generatorOptions.RedirectedWriter.WriteLine(output);
                        generatorOptions.RedirectedWriter.Flush();
                    }
                }
            }

            if (this.GeneratorConfiguration?.Logger != null)
            {
                var logger = this.GeneratorConfiguration.Logger;

                logger.Information(output);
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
                generatorOptions.OutputWriter.WriteLine(output);
            }
            else if (generatorMode == GeneratorMode.RedirectedConsole)
            {
                generatorOptions.RedirectedWriter.WriteLine(output);
                generatorOptions.RedirectedWriter.Flush();
            }

            if (this.GeneratorConfiguration?.Logger != null)
            {
                var logger = this.GeneratorConfiguration.Logger;

                logger.Information(output);
            }
        }

        /// <summary>   Resets this.  </summary>
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
