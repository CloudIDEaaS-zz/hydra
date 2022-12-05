// file:	OverrideHandlers\ConfigGeneratorOverrides.cs
//
// summary:	Implements the configuration generator overrides class

using AbstraX;
using AbstraX.ServerInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Handlers.OverrideHandlers
{
    /// <summary>   A configuration generator overrides. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 1/8/2021. </remarks>

    public class DefaultGeneratorOverrides : IGeneratorOverrides
    {
        private Dictionary<string, Dictionary<string, object>> cachedArguments;

        /// <summary>   Gets a value indicating whether the overrides namespace. </summary>
        ///
        /// <value> True if overrides namespace, false if not. </value>

        public bool OverridesNamespace => false;

        /// <summary>   Gets a value indicating whether the overrides application name. </summary>
        ///
        /// <value> True if overrides application name, false if not. </value>

        public bool OverridesAppName => false;

        /// <summary>   Gets a value indicating whether the overrides application description. </summary>
        ///
        /// <value> True if overrides application description, false if not. </value>

        public bool OverridesAppDescription => false;

        /// <summary>   Gets a value indicating whether the copies to alternate location. </summary>
        ///
        /// <value> True if copies to alternate location, false if not. </value>

        public bool CopiesToAlternateLocation => false;

        /// <summary>   Gets or sets the original namespace. </summary>
        ///
        /// <value> The original namespace. </value>

        public string OriginalNamespace { get; set; }

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/15/2021. </remarks>

        public DefaultGeneratorOverrides()
        {
            cachedArguments = new Dictionary<string, Dictionary<string, object>>();
        }

        /// <summary>   Copies the files. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/8/2021. </remarks>
        ///
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        /// <param name="argumentsKind">            The arguments kind. </param>

        public void CopyFiles(IGeneratorConfiguration generatorConfiguration, string argumentsKind)
        {
            throw new NotImplementedException();
        }

        /// <summary>   Gets application description. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/8/2021. </remarks>
        ///
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        /// <param name="argumentsKind">            The arguments kind. </param>
        ///
        /// <returns>   The application description. </returns>

        public string GetAppDescription(IGeneratorConfiguration generatorConfiguration, string argumentsKind)
        {
            throw new NotImplementedException();
        }

        /// <summary>   Gets application name. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/8/2021. </remarks>
        ///
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        /// <param name="argumentsKind">            The arguments kind. </param>
        ///
        /// <returns>   The application name. </returns>

        public string GetAppName(IGeneratorConfiguration generatorConfiguration, string argumentsKind)
        {
            throw new NotImplementedException();
        }

        /// <summary>   Gets handler arguments. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/8/2021. </remarks>
        ///
        /// <param name="packageCachePath"> Full pathname of the package cache file. </param>
        /// <param name="argumentsKind">    The arguments kind. </param>
        /// <param name="workingDirectory"> (Optional) Pathname of the working directory. </param>
        ///
        /// <returns>   The handler arguments. </returns>

        public Dictionary<string, object> GetHandlerArguments(string packageCachePath, string argumentsKind, string workingDirectory = null)
        {
            Dictionary<string, object> arguments;
            ConfigObject config;
            var directory = new DirectoryInfo(workingDirectory);
            var hydraJsonFile = directory.GetFiles("hydra.json", SearchOption.AllDirectories).FirstOrDefault();
            string key;

            if (hydraJsonFile == null)
            {
                directory = directory.Parent;

                if (directory.Name != "ApplicationGenerator")
                {
                    hydraJsonFile = directory.GetFiles("hydra.json", SearchOption.AllDirectories).FirstOrDefault();
                }
            }

            if (hydraJsonFile != null)
            {
                key = hydraJsonFile.GenerateKey();

                if (cachedArguments.ContainsKey(key))
                {
                    arguments = cachedArguments[key];
                }
                else
                {
                    config = ConfigObject.Load(hydraJsonFile.FullName);

                    arguments = config.GetPublicPropertyValues(true).ToDictionary();

                    cachedArguments.Add(key, arguments);
                }

                if (argumentsKind == null && (arguments.ContainsKey("ArgumentsKind") && arguments["ArgumentsKind"] != null))
                {
                    argumentsKind = (string) arguments["ArgumentsKind"];
                }

                if (argumentsKind == GeneratorArgumentsKind.GenerateAppFromHydraCLI)
                {
                    arguments = new Dictionary<string, object>();

                    AugmentArguments(arguments, GeneratorArgumentsKind.GenerateAppCore, workingDirectory);

                    arguments.Add("ArgumentsKind", GeneratorArgumentsKind.GenerateAppCore);
                    arguments.Remove("GeneratorOptions");
                    arguments.Remove("GeneratorKind");
                    arguments.Remove("GeneratorMode");
                }
                else if (argumentsKind == GeneratorArgumentsKind.GenerateWorkspaceFromHydraCLI)
                {
                    arguments = new Dictionary<string, object>();

                    AugmentArguments(arguments, GeneratorArgumentsKind.GenerateWorkspace, workingDirectory);

                    arguments.Add("ArgumentsKind", GeneratorArgumentsKind.GenerateWorkspace);
                    arguments.Remove("GeneratorOptions");
                }
                else
                {
                    AugmentArguments(arguments, argumentsKind, workingDirectory);
                }
            }
            else if (argumentsKind == GeneratorArgumentsKind.GenerateWorkspaceFromHydraCLI)
            {
                arguments = new Dictionary<string, object>();

                AugmentArguments(arguments, GeneratorArgumentsKind.GenerateWorkspace, workingDirectory);

                arguments.Add("ArgumentsKind", GeneratorArgumentsKind.GenerateWorkspace);
                arguments.Remove("GeneratorOptions");
            }
            else
            {
                arguments = null;
            }

            return arguments;
        }

        private void AugmentArguments(Dictionary<string, object> arguments, string argumentsKind, string workingDirectory = null)
        {
            if (argumentsKind == GeneratorArgumentsKind.GenerateApp)
            {
                var additions = new Dictionary<string, object>
                {
                    { "GeneratorKind", GeneratorKind.App },
                    { "GeneratorOptions", new DefaultGeneratorOptions(PrintMode.PrintUIHierarchyPathAndModuleAssembliesStackOnly) }
                };

                arguments.AddOrUpdateRange(additions);
            }
            else if (argumentsKind == GeneratorArgumentsKind.GenerateAppCore)
            {
                var additions = new Dictionary<string, object>
                {
                    { "GeneratorKind", GeneratorKind.App },
                    { "GeneratorOptions", new DefaultGeneratorOptions(PrintMode.PrintUIHierarchyPathAndModuleAssembliesStackOnly) }
                };

                arguments.AddOrUpdateRange(additions);
            }
            else if (argumentsKind == GeneratorArgumentsKind.GenerateStarterAppFrontend)
            {
                var additions = new Dictionary<string, object>
                {
                    { "GeneratorKind", GeneratorKind.Utility },
                    { "GeneratorOptions", new DefaultGeneratorOptions(PrintMode.PrintUIHierarchyPathAndModuleAssembliesStackOnly) }
                };

                arguments.AddOrUpdateRange(additions);
            }
            else if (argumentsKind == GeneratorArgumentsKind.GenerateWorkspace)
            {
                var additions = new Dictionary<string, object>
                {
                    { "GeneratorKind", GeneratorKind.Workspace },
                    { "GeneratorOptions", new DefaultGeneratorOptions(PrintMode.PrintUIHierarchyPathAndModuleAssembliesStackOnly) },
                    { "AdditionalOptions", new Dictionary<string, object>
                        {
                            { "SupportedTokens", new List<string>()
                                {
                                    "GENERATOR_TOKEN_USES_DATABASE"
                                }
                            }
                        }
                    }
                };

                arguments.AddOrUpdateRange(additions);
            }
            else if (argumentsKind == GeneratorArgumentsKind.GenerateBusinessModelDefault)
            {
                var assemblyPath = Assembly.GetEntryAssembly().Location;
                var templateFile = Path.Combine(assemblyPath, @"GeneratorTemplates\Default\businessModel.template");

                var additions = new Dictionary<string, object>
                {
                    { "GeneratorKind", GeneratorKind.BusinessModel },
                    { "GeneratorOptions", new DefaultGeneratorOptions(PrintMode.All) },
                    { "TemplateFile", templateFile },
                };

                arguments.AddOrUpdateRange(additions);
            }
            else if (argumentsKind == GeneratorArgumentsKind.GenerateEntitiesFromTemplateDefault)
            {
                var assemblyPath = Assembly.GetEntryAssembly().Location;
                var templateFile = Path.Combine(assemblyPath, @"GeneratorTemplates\Default\entityDomainModel.template");
                var businessModelFile = Path.Combine(workingDirectory, @"businessModel.json");

                var additions = new Dictionary<string, object>
                {
                    { "GeneratorKind", GeneratorKind.Entities },
                    { "GeneratorOptions", new DefaultGeneratorOptions(PrintMode.All) },
                    { "TemplateFile", templateFile },
                    { "BusinessModelFile", businessModelFile },
                    { "GeneratorOptions", new DefaultGeneratorOptions(PrintMode.All) }
                };

                arguments.AddOrUpdateRange(additions);
            }
            else if (argumentsKind == GeneratorArgumentsKind.GenerateBusinessModelBlank)
            {
                var templateFile = "Blank";

                var additions = new Dictionary<string, object>
                {
                    { "GeneratorKind", GeneratorKind.BusinessModel },
                    { "GeneratorOptions", new DefaultGeneratorOptions(PrintMode.All) },
                    { "TemplateFile", templateFile }
                };

                arguments.AddOrUpdateRange(additions);
            }
            else if (argumentsKind == GeneratorArgumentsKind.GenerateEntitiesFromTemplateBlank)
            {
                var templateFile = "Blank";
                var businessModelFile = Path.Combine(workingDirectory, @"businessModel.json");

                var additions = new Dictionary<string, object>
                {
                    { "GeneratorKind", GeneratorKind.Entities },
                    { "GeneratorOptions", new DefaultGeneratorOptions(PrintMode.All) },
                    { "BusinessModelFile", businessModelFile },
                    { "TemplateFile", templateFile }
                };

                arguments.AddOrUpdateRange(additions);
            }
            else if (argumentsKind == GeneratorArgumentsKind.GenerateEntitiesFromJson)
            {
                var businessModelFile = Path.Combine(workingDirectory, @"businessModel.json");
                var jsonFile = Path.Combine(workingDirectory, @"entities.json");

                var additions = new Dictionary<string, object>
                {
                    { "GeneratorKind", GeneratorKind.Entities },
                    { "GeneratorOptions", new DefaultGeneratorOptions(PrintMode.All) },
                    { "BusinessModelFile", businessModelFile },
                    { "JsonFile", jsonFile }
                };

                arguments.AddOrUpdateRange(additions);
            }
            else if (argumentsKind == GeneratorArgumentsKind.GenerateAllCoreBlank)
            {
                var type = typeof(GeneratorArgumentsKind);
                var constant = type.GetConstants().Where(c => ((string)c.GetRawConstantValue()) == argumentsKind).Single();
                var generatorArgumentsAttribute = constant.GetCustomAttribute<GeneratorArgumentsAttribute>();

                var additions = new Dictionary<string, object>
                {
                    { "GeneratorKinds", generatorArgumentsAttribute.GeneratorKinds },
                };

                arguments.AddOrUpdateRange(additions);
            }
            else if (argumentsKind == GeneratorArgumentsKind.GenerateStarterAppToCompletion)
            {
                var type = typeof(GeneratorArgumentsKind);
                var constant = type.GetConstants().Where(c => ((string)c.GetRawConstantValue()) == argumentsKind).Single();
                var generatorArgumentsAttribute = constant.GetCustomAttribute<GeneratorArgumentsAttribute>();

                var additions = new Dictionary<string, object>
                {
                    { "GeneratorKinds", generatorArgumentsAttribute.GeneratorKinds },
                };

                arguments.AddOrUpdateRange(additions);
            }
            else if (argumentsKind == GeneratorArgumentsKind.GenerateAllNoFrontendBlank)
            {
                var additions = new Dictionary<string, object>
                {
                    { "GeneratorKinds", $"{ GeneratorArgumentsKind.GenerateWorkspace }, { GeneratorArgumentsKind.GenerateBusinessModelBlank }, { GeneratorArgumentsKind.GenerateEntitiesFromTemplateBlank }, { GeneratorArgumentsKind.GenerateEntitiesFromJson }" },
                };

                arguments.AddOrUpdateRange(additions);
            }
            else if (argumentsKind != GeneratorArgumentsKind.GenerateFromConfig)
            {
                throw new ArgumentException($"Invalid argments kind for GeneratorOverrides '{ argumentsKind }'");
            }
        }

        /// <summary>   Gets a namespace. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/8/2021. </remarks>
        ///
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        /// <param name="argumentsKind">            The arguments kind. </param>
        ///
        /// <returns>   The namespace. </returns>

        public string GetNamespace(IGeneratorConfiguration generatorConfiguration, string argumentsKind)
        {
            throw new NotImplementedException();
        }

        /// <summary>   Gets override identifier. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/8/2021. </remarks>
        ///
        /// <param name="baseObject">   The base object. </param>
        /// <param name="predicate">    The predicate. </param>
        /// <param name="generatedId">  Identifier for the generated. </param>
        ///
        /// <returns>   The override identifier. </returns>

        public string GetOverrideId(IBase baseObject, string predicate, string generatedId)
        {
            return generatedId;
        }

        /// <summary>   Skip process. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/8/2021. </remarks>
        ///
        /// <param name="facetHandler">             The facet handler. </param>
        /// <param name="baseObject">               The base object. </param>
        /// <param name="facet">                    The facet. </param>
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public bool SkipProcess(IFacetHandler facetHandler, IBase baseObject, Facet facet, IGeneratorConfiguration generatorConfiguration)
        {
            return false;
        }
    }
}
