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
    public class EntitiesGeneratorEngine : IWorkspaceGeneratorEngine
    {
        public GeneratorConfiguration GeneratorConfiguration { get; private set; }
        private GeneratorMode generatorMode;
        private GeneratorOptions generatorOptions;
        private string entitiesProjectPath;
        private Guid projectType;
        private string projectFolderRoot;

        public EntitiesGeneratorEngine(Guid projectType, string projectFolderRoot, string templateFile, string jsonFile, string businessModelFile, string entitiesProjectPath, Dictionary<string, object> additionalOptions, GeneratorMode generatorMode = GeneratorMode.Console, GeneratorOptions generatorOptions = null)
        {
            var appDomain = AppDomain.CurrentDomain;
            var inputFiles = new Dictionary<string, string> { { "templateFile", templateFile }, { "jsonFile", jsonFile }, { "businessModelFile", businessModelFile } };
            List<Type> types;

            this.projectType = projectType;
            this.projectFolderRoot = projectFolderRoot;

            types = appDomain.GetAssemblies().SelectMany(a => a.GetTypes()).OrderBy(t => t.Name).ToList();

            this.GeneratorConfiguration = new GeneratorConfiguration(projectType, projectFolderRoot, inputFiles, additionalOptions, generatorOptions, this, types);
            this.generatorMode = generatorMode;
            this.generatorOptions = generatorOptions;
            this.entitiesProjectPath = entitiesProjectPath;
        }

        public void Process()
        {
            var config = this.GeneratorConfiguration;
            var templateFile = config.InputFiles["templateFile"];
            var jsonFile = config.InputFiles["jsonFile"];
            var businessModelFile = config.InputFiles["businessModelFile"];
            var directory = new DirectoryInfo(projectFolderRoot);
            var solutionFile = directory.GetFiles("*.sln", SearchOption.TopDirectoryOnly).SingleOrDefault();
            FileInfo outputFile;
            EntityDomainModel entityDomainModel;
            string fileName;

            if (templateFile != null)
            {
                IEntitiesJsonGeneratorHandler generatorHandler;

                if (businessModelFile.IsNullOrEmpty())
                {
                    WriteError("--businessModel must be specified");
                    throw new ArgumentException("Invalid arguments");
                }

                if (solutionFile != null)
                {
                    var appName = Path.GetFileNameWithoutExtension(solutionFile.Name);

                    fileName = Path.Combine(projectFolderRoot, string.Format("{0}.entityDomainModel.json", appName));
                }
                else
                {
                    fileName = Path.Combine(projectFolderRoot, "NoSolutionFile.entityDomainModel.json");
                }

                outputFile = new FileInfo(fileName);

                if (outputFile.Exists)
                {
                    outputFile.MakeWritable();
                }

                generatorHandler = config.GetEntitiesJsonGeneratorHandler();
                entityDomainModel = config.CreateEntityDomainModelFromTemplate(templateFile);

                if (generatorHandler != null)
                {
                    generatorHandler.Process(entityDomainModel, projectType, projectFolderRoot, templateFile, outputFile.FullName, config);

                    if (config.CurrentPass == GeneratorPass.Files)
                    {
                        WriteLine($"Generated { outputFile.FullName }");
                    }
                }
                else
                {
                    WriteError("No handler found implementing IEntitiesJsonGeneratorHandler");
                    throw new HandlerNotFoundException("Entity json generation");
                }
            }
            else if (jsonFile != null)
            {
                IEntitiesModelGeneratorHandler generatorHandler;

                if (businessModelFile.IsNullOrEmpty())
                {
                    WriteLine("--businessModel must be specified");
                    throw new ArgumentException("Invalid arguments");
                }

                if (entitiesProjectPath.IsNullOrEmpty())
                {
                    WriteLine("--entitiesProjectPath must be specified");
                    throw new ArgumentException("Invalid arguments");
                }

                generatorHandler = config.GetEntitiesModelGeneratorHandler();
                entityDomainModel = config.CreateEntityDomainModelFromJsonFile(jsonFile);

                if (generatorHandler != null)
                {
                    List<string> existingFiles = null;
                    var entitiesDirectory = new DirectoryInfo(entitiesProjectPath);

                    if (config.CurrentPass == GeneratorPass.Files)
                    {
                        existingFiles = entitiesDirectory.GetFiles("*.*", SearchOption.AllDirectories).Select(f => f.FullName).ToList();
                    }

                    generatorHandler.Process(entityDomainModel, projectType, projectFolderRoot, templateFile, businessModelFile, entitiesProjectPath, config);

                    if (config.CurrentPass == GeneratorPass.Files)
                    {
                        var newFiles = entitiesDirectory.GetFiles("*.*", SearchOption.AllDirectories).Select(f => f.FullName).Where(f => !existingFiles.Any(e => e == f)).ToList();

                        WriteLine("Generated the following files:");
                        config.Indent();

                        foreach (var file in newFiles)
                        {
                            WriteLine(file);
                        }

                        config.Dedent();
                    }
                }
                else
                {
                    WriteError("No handler found implementing IEntitiesModelGeneratorHandler");
                    throw new HandlerNotFoundException("Entity model generation");
                }
            }
            else
            {
                WriteError("Invalid command line argments");
                throw new ArgumentException("Invalid arguments");
            }
        }

        public string CurrentTabText
        {
            get
            {
                var tabs = this.GeneratorConfiguration.IndentLevel;
                var tabText = new string(' ', tabs);

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
                Console.WriteLine(this.CurrentTabText + output);
            }
            else if (generatorMode == GeneratorMode.RedirectedConsole)
            {
                generatorOptions.OutputWriter.WriteLine(this.CurrentTabText + output);
                generatorOptions.OutputWriter.Flush();
            }
        }

        public void Reset()
        {
        }
    }
}
