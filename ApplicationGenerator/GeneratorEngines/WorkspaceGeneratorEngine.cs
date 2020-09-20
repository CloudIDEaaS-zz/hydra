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

namespace AbstraX.GeneratorEngines
{
    public class WorkspaceGeneratorEngine : IWorkspaceGeneratorEngine
    {
        private string appName;
        public GeneratorConfiguration GeneratorConfiguration { get; private set; }
        private GeneratorMode generatorMode;
        private GeneratorOptions generatorOptions;
        private List<KeyValuePair<string, IGeneratorOverrides>> generatorOverridePairs;
        private Guid projectType;
        private string projectFolderRoot;
        private string organizationName;
        private string[] supportedTokens;

        public WorkspaceGeneratorEngine(Guid projectType, string projectFolderRoot, string appName, string organizationName, Dictionary<string, object> additionalOptions, GeneratorMode generatorMode = GeneratorMode.Console, GeneratorOptions generatorOptions = null)
        {
            IGeneratorOverrides appNameOverride;
            KeyValuePair<string, IGeneratorOverrides> appNameOverridePair;
            string argumentsKind;
            var appDomain = AppDomain.CurrentDomain;
            List<Type> types;

            this.generatorOverridePairs = this.GetOverrides().ToList();
            this.projectType = projectType;
            this.projectFolderRoot = projectFolderRoot;
            this.organizationName = organizationName;

            appNameOverridePair = generatorOverridePairs.Where(o => o.Value.OverridesAppName).LastOrDefault();

            argumentsKind = appNameOverridePair.Key;
            appNameOverride = appNameOverridePair.Value;

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

            types = appDomain.GetAssemblies().SelectMany(a => a.GetTypes()).OrderBy(t => t.Name).ToList();

            this.GeneratorConfiguration = new GeneratorConfiguration(projectType, projectFolderRoot, appName, additionalOptions, generatorOptions, this, types);
            this.generatorMode = generatorMode;
            this.generatorOptions = generatorOptions;
        }

        public void Process()
        {
            var config = this.GeneratorConfiguration;
            var assemblyLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var localTemplatesPath = Path.Combine(assemblyLocation, "Templates");
            var directory = new DirectoryInfo(localTemplatesPath);

            foreach (var file in directory.GetFiles())
            {
                if (file.Extension == ".sln")
                {
                    using (var stream = System.IO.File.OpenRead(file.FullName))
                    {
                        var reader = new StreamReader(stream);
                        var content = reader.ReadToEnd();
                        var rawFileRelativePath = file.Name.ReverseSlashes();
                        var tokenizedRelativePath = VSTemplate.ReplaceParameterText(rawFileRelativePath, GetTemplateParameters());
                        var fileName = Path.Combine(projectFolderRoot, tokenizedRelativePath);
                        var outputFile = new FileInfo(fileName);

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
                else if (file.Extension == ".zip")
                {
                    var rawProjectName = Path.GetFileNameWithoutExtension(file.Name);
                    var templateParameters = this.GeneratorConfiguration.GetTemplateParameters(rawProjectName);
                    string rootFolder;
                    string projectName;

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
                            var rawFileRelativePath = part.Uri.OriginalString.ReverseSlashes();
                            VSTemplateProjectItem projectItem = null;
                            IWorkspaceTemplate workspaceTemplate = null;
                            IWorkspaceFileTypeHandler fileTypeHandler;
                            string fileName;

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
                                var tokenizedRelativePath = template.ReplaceParameters(rawFileRelativePath, GetTemplateParameters(projectName));

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
                                    var outputFile = new FileInfo(fileName);

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

                                        fileTypeHandler.PreProcess(projectType, appName, rawFileRelativePath, fileName, supportedTokens, content, config);

                                        if (fileTypeHandler.TokensToProcess != null)
                                        {
                                            foreach (var token in fileTypeHandler.TokensToProcess)
                                            {
                                                var tokenContentHandler = config.GetTokenContentHandler(token);

                                                if (tokenContentHandler != null)
                                                {
                                                    tokenContentHandler.Process(projectType, appName, rawFileRelativePath, fileName, this.supportedTokens);

                                                    tokenContentHandlers.Add(token, tokenContentHandler);
                                                }
                                            }
                                        }

                                        fileTypeHandler.Process(tokenContentHandlers, workspaceTemplate, projectType, appName, rawFileRelativePath, fileName, content, config);

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
        }

        private WorkspaceTemplateParameters GetTemplateParameters(string projectName = null)
        {
            return new WorkspaceTemplateParameters
            {
                ProjectName = projectName,
                SolutionName = appName,
                RegisteredOrganization = organizationName,
                CopyrightYear = DateTime.UtcNow.ToString("yyyy"),
                FrameworkVersion = "netcoreapp3.1"
            };
        }

        private bool HasRegisteredAppConfig()
        {
            var appName = this.GeneratorConfiguration.AppName;
            var appDescription = this.GeneratorConfiguration.AppDescription;
            var clientId = this.GeneratorConfiguration.ClientId;
            var clientSecret = this.GeneratorConfiguration.ClientSecret;

            return CompareExtensions.AnyAreNotNull(appName, appDescription, clientId, clientSecret);
        }

        public string CurrentTabText
        {
            get
            {
                var tabs = this.GeneratorConfiguration.HierarchyStack.Count;
                var tabText = new string(' ', tabs * 2);

                return tabText;
            }
        }

        public void WriteLine(string format, PrintMode printMode, params object[] args)
        {
            var output = string.Format(format, args);

            if (output.Trim().Length > 0)
            {
                if ((generatorOptions.PrintMode == PrintMode.All && (!printMode.HasFlag(PrintMode.ExcludeFromAll))) || printMode.HasAnyFlag(generatorOptions.PrintMode) || printMode == PrintMode.Any)
                {
                    if (generatorMode == GeneratorMode.Console)
                    {
                        Console.WriteLine(output);
                    }
                    else if (generatorMode == GeneratorMode.RedirectedConsole)
                    {
                        generatorOptions.OutputWriter.WriteLine(output);
                        generatorOptions.OutputWriter.Flush();
                    }
                }
            }
        }

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

        public void WriteLine(string format, params object[] args)
        {
            var output = string.Format(format, args);

            if (generatorMode == GeneratorMode.Console)
            {
                Console.WriteLine(output);
            }
            else if (generatorMode == GeneratorMode.RedirectedConsole)
            {
                generatorOptions.OutputWriter.WriteLine(output);
                generatorOptions.OutputWriter.Flush();
            }
        }

        public void Reset()
        {
        }
    }
}
