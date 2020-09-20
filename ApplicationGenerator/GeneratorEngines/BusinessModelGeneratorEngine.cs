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
    public class BusinessModelGeneratorEngine : IWorkspaceGeneratorEngine
    {
        public GeneratorConfiguration GeneratorConfiguration { get; private set; }
        private GeneratorMode generatorMode;
        private GeneratorOptions generatorOptions;
        private Guid projectType;
        private string projectFolderRoot;

        public BusinessModelGeneratorEngine(Guid projectType, string projectFolderRoot, string templateFile, Dictionary<string, object> additionalOptions, GeneratorMode generatorMode = GeneratorMode.Console, GeneratorOptions generatorOptions = null)
        {
            var appDomain = AppDomain.CurrentDomain;
            var inputFiles = new Dictionary<string, string> { { "templateFile", templateFile } };
            List<Type> types;

            this.projectType = projectType;
            this.projectFolderRoot = projectFolderRoot;

            types = appDomain.GetAssemblies().SelectMany(a => a.GetTypes()).OrderBy(t => t.Name).ToList();

            this.GeneratorConfiguration = new GeneratorConfiguration(projectType, projectFolderRoot, inputFiles, additionalOptions, generatorOptions, this, types);
            this.generatorMode = generatorMode;
            this.generatorOptions = generatorOptions;
        }

        public void Process()
        {
            var config = this.GeneratorConfiguration;
            var templateFile = config.InputFiles["templateFile"];
            var directory = new DirectoryInfo(projectFolderRoot);
            var solutionFile = directory.GetFiles("*.sln", SearchOption.TopDirectoryOnly).SingleOrDefault();
            IBusinessModelGeneratorHandler generatorHandler;
            FileInfo outputFile;
            BusinessModel businessModel;
            string fileName;

            if (templateFile.IsNullOrEmpty())
            {
                WriteLine("--template must be specified");
                throw new ArgumentException("Invalid arguments");
            }

            if (solutionFile != null)
            {
                var appName = Path.GetFileNameWithoutExtension(solutionFile.Name);

                fileName = Path.Combine(projectFolderRoot, string.Format("{0}.businessModel.json", appName));
            }
            else
            {
                fileName = Path.Combine(projectFolderRoot, "NoSolutionFile.entities.json");
            }

            outputFile = new FileInfo(fileName);

            if (config.CurrentPass == GeneratorPass.Files)
            {
                if (outputFile.Exists)
                {
                    outputFile.MakeWritable();
                }
            }

            generatorHandler = config.GetBusinessModelGeneratorHandler();
            businessModel = config.CreateBusinessModelFromTemplate(templateFile);

            if (generatorHandler != null)
            {
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
