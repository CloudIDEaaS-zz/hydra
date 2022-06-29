// file:	GeneratorEngines\WorkspaceGeneratorEngine.cs
//
// summary:	Implements the workspace generator engine class

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
    /// <summary>   A workspace generator engine. </summary>
    ///
    /// <remarks>   Ken, 10/3/2020. </remarks>

    public class WorkspaceGeneratorEngine : IWorkspaceGeneratorEngine
    {
        /// <summary>   Name of the application. </summary>
        private string appDescription;
        /// <summary>   Name of the application. </summary>
        private string appName;

        /// <summary>   Gets or sets the generator configuration. </summary>
        ///
        /// <value> The generator configuration. </value>

        public GeneratorConfiguration GeneratorConfiguration { get; private set; }
        /// <summary>   The generator mode. </summary>
        private GeneratorMode generatorMode;
        /// <summary>   Options for controlling the generator. </summary>
        private GeneratorOptions generatorOptions;
        /// <summary>   The generator override pairs. </summary>
        private List<KeyValuePair<string, IGeneratorOverrides>> generatorOverridePairs;
        /// <summary>   Type of the project. </summary>
        private Guid projectType;
        /// <summary>   The project folder root. </summary>
        private string projectFolderRoot;
        /// <summary>   Name of the organization. </summary>
        private string organizationName;
        /// <summary>   The supported tokens. </summary>
        private string[] supportedTokens;
        private string callbackName = CallbackNames.Foundation;
        public NetCoreReflectionAgent NetCoreReflectionAgent { get; private set; }
        public MailslotClient MailslotClient { get; private set; }
        public Queue<string> LogMessageQueue { get; }
        private IAppFolderStructureSurveyor appFolderStructureSurveyor;
        public int ParentProcessId { get; }
        public string GenerationName => "Workspace";
        public int percentOfStart = 0;
        public int percentOf = 30;

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="projectType">          Type of the project. </param>
        /// <param name="projectFolderRoot">    The project folder root. </param>
        /// <param name="appName">              Name of the application. </param>
        /// <param name="appDescription">       Information describing the application. </param>
        /// <param name="organizationName">     Name of the organization. </param>
        /// <param name="additionalOptions">    Options for controlling the additional. </param>
        /// <param name="generatorMode">        (Optional) The generator mode. </param>
        /// <param name="generatorOptions">     (Optional) Options for controlling the generator. </param>

        public WorkspaceGeneratorEngine(Guid projectType, string projectFolderRoot, string appName, string appDescription, string organizationName, Dictionary<string, object> additionalOptions, GeneratorMode generatorMode = GeneratorMode.Console, GeneratorOptions generatorOptions = null)
        {
            var thisAssembly = Assembly.GetExecutingAssembly();
            IGeneratorOverrides appNameOverride;
            IGeneratorOverrides appDescriptionOverride;
            KeyValuePair<string, IGeneratorOverrides> appNameOverridePair;
            KeyValuePair<string, IGeneratorOverrides> appDescriptionOverridePair;
            string argumentsKind;
            List<Type> types;

            this.generatorOverridePairs = this.GetOverrides(generatorOptions.UseOverrides, projectFolderRoot).ToList();
            this.projectType = projectType;
            this.projectFolderRoot = projectFolderRoot;
            this.organizationName = organizationName;
            this.NetCoreReflectionAgent = new NetCoreReflectionAgent();
            this.MailslotClient = generatorOptions.MailslotClient;
            this.ParentProcessId = generatorOptions.ParentProcessId;
            this.LogMessageQueue = new Queue<string>();
            this.appFolderStructureSurveyor = generatorOptions.AppFolderStructureSurveyor;

            this.SendCallbackStatus(callbackName, "Initializing workspace", 0, percentOfStart, percentOf);

            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                this.NetCoreReflectionAgent.Dispose();
            };

            appNameOverridePair = generatorOverridePairs.Where(o => o.Value.OverridesAppName).LastOrDefault();
            appDescriptionOverridePair = generatorOverridePairs.Where(o => o.Value.OverridesAppDescription).LastOrDefault();

            argumentsKind = appNameOverridePair.Key;
            appNameOverride = appNameOverridePair.Value;
            appDescriptionOverride = appDescriptionOverridePair.Value;

            if (appDescriptionOverride == null)
            {
                this.appDescription = appDescription;
            }
            else
            {
                this.appDescription = appDescriptionOverride.GetAppDescription(this.GeneratorConfiguration, argumentsKind);
            }

            if (appNameOverride == null)
            {
                this.appName = appName;
            }
            else
            {
                var originalNamespace = appName;

                appNameOverride.OriginalNamespace = originalNamespace;
                this.appName = appNameOverride.GetAppName(this.GeneratorConfiguration, argumentsKind);
            }

            supportedTokens = additionalOptions.Where(p => p.Key == "SupportedTokens").SelectMany(p => (List<string>)p.Value).ToArray();

            types = thisAssembly.GetAllTypes(this.NetCoreReflectionAgent).Distinct().OrderBy(t => t.FullName).ToList();

            this.GeneratorConfiguration = new GeneratorConfiguration(projectType, projectFolderRoot, appName, appDescription, additionalOptions, generatorOptions, this, types);
            this.generatorMode = generatorMode;
            this.generatorOptions = generatorOptions;

            this.SendCallbackStatus(callbackName, "Finished initializing workspace", 10, percentOfStart, percentOf);
        }

        /// <summary>   Process this.  </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>

        public void Process()
        {
            var config = this.GeneratorConfiguration;
            var assemblyLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var localTemplatesPath = Path.Combine(assemblyLocation, "Templates");
            var directory = new DirectoryInfo(localTemplatesPath);
            var percentComplete = 0;
            var fileTypeHandlers = new List<IWorkspaceFileTypeHandler>();
            var uniqueProfileDataFileHandler = config.GetHandler<IUniqueProfileDataFileHandler>();

            WriteLine("\r\n**** CurrentPass: {0} {1}\r\n", PrintMode.Any, config.CurrentPass, "*".Repeat(25));

            foreach (var file in directory.GetFiles("*.sln"))
            {
                var rawFileRelativePath = file.Name.ReverseSlashes();
                var tokenizedRelativePath = VSTemplate.ReplaceParameterText(rawFileRelativePath, GetTemplateParameters());
                var fileName = Path.Combine(projectFolderRoot, tokenizedRelativePath);

                WriteLine("{0}{1}", PrintMode.All, this.CurrentTabText, $"Creating solution { Path.GetFileNameWithoutExtension(fileName) }");
                percentComplete = 20;
                this.SendCallbackStatus(config.CurrentPass, callbackName, "Creating solution", percentComplete, percentOfStart, percentOf);

                this.Indent();
            }

            foreach (var file in directory.GetFiles())
            {   
                if (file.Extension == ".sln")
                {
                    var rawFileRelativePath = file.Name.ReverseSlashes();
                    var tokenizedRelativePath = VSTemplate.ReplaceParameterText(rawFileRelativePath, GetTemplateParameters());
                    var fileName = Path.Combine(projectFolderRoot, tokenizedRelativePath);
                    var outputFile = new FileInfo(fileName);

                    WriteLine("{0}{1}", PrintMode.All, this.CurrentTabText, $"Creating solution file { Path.GetFileNameWithoutExtension(fileName) }");

                    percentComplete += 10;
                    this.SendCallbackStatus(config.CurrentPass, callbackName, $"Creating solution file { Path.GetFileNameWithoutExtension(fileName) }", percentComplete, percentOfStart, percentOf);

                    using (var stream = System.IO.File.OpenRead(file.FullName))
                    {
                        var reader = new StreamReader(stream);
                        var content = reader.ReadToEnd();

                        content = VSTemplate.ReplaceParameterText(content, GetTemplateParameters());

                        if (config.CurrentPass == GeneratorPass.Files)
                        {
                            if (outputFile.Exists)
                            {
                                outputFile.MakeWritable();
                            }

                            System.IO.File.WriteAllText(outputFile.FullName, content);
                        }
                    }
                }
                else if (file.Extension == ".zip" && !file.Name.EndsWith("Content.zip"))
                {
                    var rawProjectName = Path.GetFileNameWithoutExtension(file.Name);
                    var rawFileRelativePath = file.Name.ReverseSlashes();
                    var templateParameters = this.GeneratorConfiguration.GetTemplateParameters(rawProjectName);
                    var tokenizedRelativePath = VSTemplate.ReplaceParameterText(rawFileRelativePath, GetTemplateParameters());
                    var fileName = Path.Combine(projectFolderRoot, tokenizedRelativePath);
                    var outputFile = new FileInfo(fileName);
                    string rootFolder;
                    string projectName;

                    percentComplete += 10;
                    WriteLine("{0}{1}", PrintMode.All, this.CurrentTabText, $"Creating project { Path.GetFileNameWithoutExtension(fileName) }");

                    if (config.CurrentPass == GeneratorPass.Files)
                    {
                        this.SendCallbackStatus(config.CurrentPass, callbackName, $"Creating project { Path.GetFileNameWithoutExtension(fileName) }", percentComplete, percentOfStart, percentOf);
                    }

                    if (templateParameters.Count > 0)
                    {
                        var tokenizedProjectName = VSTemplate.ReplaceParameterText(rawProjectName, GetTemplateParameters());

                        projectName = tokenizedProjectName;
                        rootFolder = Path.Combine(projectFolderRoot, tokenizedProjectName);
                    }
                    else
                    {
                        projectName = rawProjectName;
                        rootFolder = Path.Combine(projectFolderRoot, rawProjectName);
                    }

                    if (!System.IO.Directory.Exists(rootFolder))
                    {
                        if (config.CurrentPass == GeneratorPass.Files)
                        {
                            System.IO.Directory.CreateDirectory(rootFolder);
                        }
                    }

                    using (var package = ZipPackage.Open(file.FullName, FileMode.Open))
                    {
                        var templatePart = package.GetParts().Single(p => Path.GetExtension(p.Uri.OriginalString) == ".vstemplate");
                        VSProjectTemplate template = null;
                        VSTemplateProject project = null;

                        using (var stream = templatePart.GetStream())
                        {
                            var reader = new StreamReader(stream);
                            var contents = reader.ReadToEnd();

                            template = (VSProjectTemplate)VSConfigProvider.ParseTemplate(new StringBuilder(contents));
                            project = (VSTemplateProject)template.Projects.Single();
                        }

                        foreach (var part in package.GetParts())
                        {
                            VSTemplateProjectItem projectItem = null;
                            IWorkspaceTemplate workspaceTemplate = null;
                            IWorkspaceFileTypeHandler fileTypeHandler;

                            rawFileRelativePath = part.Uri.OriginalString.ReverseSlashes();

                            templateParameters = this.GeneratorConfiguration.GetTemplateParameters(rawFileRelativePath);

                            if (template != null)
                            {
                                if (rawFileRelativePath.AsCaseless() == project.RelativePath)
                                {
                                    workspaceTemplate = project;
                                }
                                else
                                {
                                    projectItem = (VSTemplateProjectItem)project.ProjectItems.SingleOrDefault(i => i.RelativePath.AsCaseless() == rawFileRelativePath);

                                    if (projectItem != null && projectItem.SubType != null)
                                    {
                                        var subTypes = projectItem.SubType.Split(",", StringSplitOptions.RemoveEmptyEntries);

                                        if (subTypes.Where(t => t.StartsWith("GENERATOR_TOKEN_")).Any(s => !supportedTokens.Any(t => t == s)))
                                        {
                                            continue;
                                        }
                                    }

                                    workspaceTemplate = projectItem;
                                }
                            }

                            if (templateParameters.Count > 0)
                            {
                                tokenizedRelativePath = template.ReplaceParameters(rawFileRelativePath, GetTemplateParameters(projectName));

                                fileName = Path.Combine(rootFolder, tokenizedRelativePath.RemoveStartIfMatches(@"\"));
                            }
                            else
                            {
                                fileName = Path.Combine(rootFolder, rawFileRelativePath.RemoveStartIfMatches(@"\"));
                            }

                            if (workspaceTemplate != null)
                            {
                                using (var stream = part.GetStream())
                                {
                                    var reader = new StreamReader(stream);
                                    var content = reader.ReadToEnd();
                                    
                                    outputFile = new FileInfo(fileName);

                                    if (workspaceTemplate.ReplaceParameters)
                                    {
                                        content = template.ReplaceParameters(content, GetTemplateParameters(projectName));
                                    }

                                    if (outputFile.Exists)
                                    {
                                        outputFile.MakeWritable();
                                    }

                                    fileTypeHandler = config.GetWorkspaceFileTypeHandler(fileName);

                                    if (fileTypeHandler != null)
                                    {
                                        var tokenContentHandlers = new Dictionary<string, IWorkspaceTokenContentHandler>();

                                        if (!fileTypeHandlers.Contains(fileTypeHandler))
                                        {
                                            fileTypeHandlers.Add(fileTypeHandler);
                                        }

                                        fileTypeHandler.PreProcess(projectType, appDescription, rawFileRelativePath, fileName, supportedTokens, content, config);

                                        if (fileTypeHandler.TokensToProcess != null)
                                        {
                                            foreach (var token in fileTypeHandler.TokensToProcess)
                                            {
                                                var tokenContentHandler = config.GetTokenContentHandler(token);

                                                if (tokenContentHandler != null)
                                                {
                                                    tokenContentHandler.Process(projectType, appDescription, rawFileRelativePath, fileName, this.supportedTokens);

                                                    tokenContentHandlers.Add(token, tokenContentHandler);
                                                }
                                            }
                                        }

                                        fileTypeHandler.Process(tokenContentHandlers, workspaceTemplate, projectType, appDescription, rawFileRelativePath, fileName, content, config);

                                        content = fileTypeHandler.OutputContent;
                                    }

                                    if (config.CurrentPass == GeneratorPass.Files)
                                    {
                                        if (!outputFile.Directory.Exists)
                                        {
                                            outputFile.Directory.Create();
                                        }

                                        System.IO.File.WriteAllText(outputFile.FullName, content);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (config.CurrentPass == GeneratorPass.Files)
            {
                var organizationUniqueName = organizationName.RemoveNonWordCharacters(true).ToLower();
                var appUniqueName = appName.RemoveNonWordCharacters(true).ToLower();

                this.appFolderStructureSurveyor.DetermineLayout(projectFolderRoot);

                fileTypeHandlers.ForEach(h => h.PostProcess(this.appFolderStructureSurveyor));

                uniqueProfileDataFileHandler.Process(projectType, appFolderStructureSurveyor, organizationUniqueName, appUniqueName, config);
            }

            this.SendCallbackStatus(config.CurrentPass, callbackName, "Finished creating solution", 95, percentOfStart, percentOf);

            if (config.CurrentPass == GeneratorPass.Files)
            {
                WriteLine("Finished creating solution");
            }

            this.Dedent();
        }

        /// <summary>   Gets template parameters. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="projectName">  (Optional) Name of the project. </param>
        ///
        /// <returns>   The template parameters. </returns>

        private WorkspaceTemplateParameters GetTemplateParameters(string projectName = null)
        {
            return new WorkspaceTemplateParameters
            {
                AppName = this.appName,
                AppDescription = this.appDescription,
                ProjectName = projectName,
                SolutionName = this.appName,
                RegisteredOrganization = organizationName,
                CopyrightYear = DateTime.UtcNow.ToString("yyyy"),
                FrameworkVersion = "netcoreapp3.1"
            };
        }

        /// <summary>   Query if this  has registered application configuration. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <returns>   True if registered application configuration, false if not. </returns>

        private bool HasRegisteredAppConfig()
        {
            var appName = this.GeneratorConfiguration.AppName;
            var appDescription = this.GeneratorConfiguration.AppDescription;
            var clientId = this.GeneratorConfiguration.ClientId;
            var clientSecret = this.GeneratorConfiguration.ClientSecret;

            return CompareExtensions.AnyAreNotNull(appName, appDescription, clientId, clientSecret);
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
        /// <remarks>   Ken, 10/3/2020. </remarks>
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

                if (this.GeneratorConfiguration?.Logger != null)
                {
                    var logger = this.GeneratorConfiguration.Logger;

                    logger.Information(output);
                }
            }
        }

        /// <summary>   Writes an error. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
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
        /// <remarks>   Ken, 10/3/2020. </remarks>
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
        /// <remarks>   Ken, 10/3/2020. </remarks>

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
            WriteLine("Finished processing");
        }
    }
}
