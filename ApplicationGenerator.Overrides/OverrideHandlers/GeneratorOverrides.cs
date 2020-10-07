using AbstraX;
using AbstraX.ServerInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace ApplicationGenerator.Overrides.OverrideHandlers
{
    public class GeneratorOverrides : IGeneratorOverrides
    {
        public bool OverridesNamespace { get; set; }
        public bool OverridesAppName { get; set; }
        public string OriginalNamespace { get; set; }
        public bool OverridesAppDescription { get; set; }
        public bool CopiesToAlternateLocation { get; private set; }

        public void CopyFiles(IGeneratorConfiguration generatorConfiguration, string argumentsKind)
        {
            var hydraSolutionPath = Path.GetFullPath(Environment.ExpandEnvironmentVariables("%HYDRASOLUTIONPATH%"));
            var sourceDirectory = new DirectoryInfo(generatorConfiguration.ApplicationFolderHierarchy.ServicesRoot.BackSlashes());
            var targetDirectory = new DirectoryInfo(Path.Combine(hydraSolutionPath, @"HydraDevOps.Services"));

            BackupFiles(targetDirectory);

            sourceDirectory.CopyTo(targetDirectory.FullName, true, (f) =>
            {
                if (f.Name.StartsWith("Ripley"))
                {
                    return false;
                }
                else if (f is FileInfo && Path.GetDirectoryName(f.FullName).AsCaseless() == Path.Combine(hydraSolutionPath, @"ApplicationGenerator\TestOutput\Services\Models"))
                {
                    if (f.Name == "Project.cs" || f.Name == "WorkItemReference.cs")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                return true;
            });

            sourceDirectory = new DirectoryInfo(generatorConfiguration.ProjectFolderRoot);
            targetDirectory = new DirectoryInfo(Path.Combine(hydraSolutionPath, @"Ripley\TestOutput"));

            sourceDirectory.CopyTo(targetDirectory.FullName, true);
        }

        public void BackupFiles(DirectoryInfo directory)
        {
            string name;

            foreach (var file in directory.GetFiles("*.Backup.zip"))
            {
                try
                {
                    file.Delete();
                }
                catch (Exception ex)
                {
                }
            }

            name = DateTime.Now.ToSortableDateTimeText() + ".Backup.zip";

            directory.ToZipFile(Path.Combine(directory.FullName, name), (f) =>
            {
                if (f.Directory.FullName.RegexIsMatch(@"\\bin[\\$]"))
                {
                    return false;
                }
                else if (f.Directory.FullName.RegexIsMatch(@"\\obj[\\$]"))
                {
                    return false;
                }

                return true;
            });
        }

        public Dictionary<string, object> GetHandlerArguments(string packageCachePath, string argumentsKind)
        {
            if (argumentsKind == "RestServiceProvider")
            {
                var hydraSolutionPath = Path.GetFullPath(Environment.ExpandEnvironmentVariables("%HYDRASOLUTIONPATH%"));
                var entitiesProjectPath = Path.Combine(hydraSolutionPath, @"Ripley.Entities\Ripley.Entities.csproj");
                var servicesProjectPath = Path.Combine(hydraSolutionPath, @"Ripley.Services\Ripley.Services.csproj");

                this.OverridesAppName = true;
                this.OverridesNamespace = true;

                return new Dictionary<string, object>
                {
                    { "EntitiesProjectPath", entitiesProjectPath },
                    { "ServicesProjectPath", servicesProjectPath },
                    { "GeneratorMode", GeneratorMode.Console },
                    { "PackageCachePath", packageCachePath },
                    { "GeneratorOptions", new DefaultGeneratorOptions(PrintMode.PrintUIHierarchyPathAndModuleAssembliesStackOnly) },
                    { "AdditionalOptions", new Dictionary<string, object>
                        {
                            { "https://dev.azure.com/#accessToken", "cnb7itvcr3phsw34yemf264qt4d2z4jlgsvnsxt75txzddiekdta" },
                            { "https://dev.azure.com/#parentPath", "HydraDevOps/HydraDevOpsContext/Projects/Project" },
                            { "https://dev.azure.com/#organizationName", "{/ parent()[lower-case(translate(@OrganizationName, ' ', ''))] /}" },
                            { "https://dev.azure.com/#clientControllerRouteBase", "api/devops/devopsservice" },
                        }
                    }
                };
            }
            else if (argumentsKind == "GenerateAll")
            {
                return new Dictionary<string, object>
                {
                    { "GeneratorKinds", "GenerateWorkspace, GenerateBusinessModel, GenerateEntitiesFromTemplate, GenerateEntitiesFromJson" },
                };
            }
            else if (argumentsKind == "GenerateWorkspace")
            {
                return new Dictionary<string, object>
                {
                    { "AppName", "contoso" },
                    { "AppDescription", "The Contoso Application is an enterprise system that supports a multi-national business with headquarters in Paris, France. It provides features for manufacturing, sales, and support, and inventory of over 100,000 products." },
                    { "OrganizationName", "contoso" },
                    { "GeneratorKind", GeneratorKind.Workspace },
                    { "GeneratorMode", GeneratorMode.Console },
                    { "GeneratorOptions", new DefaultGeneratorOptions(PrintMode.All) },
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
            }
            else if (argumentsKind == "GenerateBusinessModel")
            {
                var hydraSolutionPath = Path.GetFullPath(Environment.ExpandEnvironmentVariables("%HYDRASOLUTIONPATH%"));
                var templateFile = Path.Combine(hydraSolutionPath, @"ApplicationGenerator\GeneratorTemplates\Default\businessModel.template");

                return new Dictionary<string, object>
                {
                    { "GeneratorKind", GeneratorKind.BusinessModel },
                    { "TemplateFile", templateFile },
                    { "GeneratorOptions", new DefaultGeneratorOptions(PrintMode.All) },
                    { "GeneratorMode", GeneratorMode.Console }
                };
            }
            else if (argumentsKind == "GenerateEntitiesFromTemplate")
            {
                var hydraSolutionPath = Path.GetFullPath(Environment.ExpandEnvironmentVariables("%HYDRASOLUTIONPATH%"));
                var templateFile = Path.Combine(hydraSolutionPath, @"ApplicationGenerator\GeneratorTemplates\Default\entityDomainModel.template");
                var businessModelFile = Path.Combine(hydraSolutionPath, @"ApplicationGenerator\TestOutput\businessModel.json");

                return new Dictionary<string, object>
                {
                    { "GeneratorKind", GeneratorKind.Entities },
                    { "TemplateFile", templateFile },
                    { "BusinessModelFile", businessModelFile },
                    { "GeneratorOptions", new DefaultGeneratorOptions(PrintMode.All) },
                    { "GeneratorMode", GeneratorMode.Console }
                };
            }
            else if (argumentsKind == "GenerateEntitiesFromJson")
            {
                var hydraSolutionPath = Path.GetFullPath(Environment.ExpandEnvironmentVariables("%HYDRASOLUTIONPATH%"));
                var businessModelFile = Path.Combine(hydraSolutionPath, @"ApplicationGenerator\TestOutput\businessModel.json");
                var jsonFile = Path.Combine(hydraSolutionPath, @"ApplicationGenerator\TestOutput\entities.json");
                var entitiesProjectPath = Path.Combine(hydraSolutionPath, @"ApplicationGenerator\TestOutput\contoso.Entities\contoso.Entities.csproj");

                return new Dictionary<string, object>
                {
                    { "GeneratorKind", GeneratorKind.Entities },
                    { "JsonFile", jsonFile },
                    { "BusinessModelFile", businessModelFile },
                    { "EntitiesProjectPath", entitiesProjectPath },
                    { "GeneratorOptions", new DefaultGeneratorOptions(PrintMode.All) },
                    { "GeneratorMode", GeneratorMode.Console }
                };
            }
            else
            {
                throw new ArgumentException("Invalid argments kind for GeneratorOverrides");
            }
        }

        public string GetNamespace(IGeneratorConfiguration generatorConfiguration, string argumentsKind)
        {
            return "HydraDevOps.Services";
        }

        public string GetAppName(IGeneratorConfiguration generatorConfiguration, string argumentsKind)
        {
            return "HydraDevOps";
        }

        public bool SkipProcess(IFacetHandler facetHandler, IBase baseObject, Facet facet, IGeneratorConfiguration generatorConfiguration)
        {
            if (facetHandler.GetType().Name == "WebAPIStartupGraphQLSchemasFacetHandler")
            {
                return baseObject.Name == "RipleyEntities";
            }
            else if (facetHandler.GetType().Name == "WebAPIModelFacetHandler")
            {
                return baseObject.ID == "/Model[@Model='HydraDevOps']/Entity_Container[@Container='HydraDevOpsContext']/Entity_Set[@EntitySet='Projects']/EntityType[@Entity='Project']";
            }

            return false;
        }

        public string GetOverrideId(IBase baseObject, string predicate, string generatedId)
        {
            var id = generatedId;

            if (baseObject.Name == "Ripley")
            {
                predicate = "Model='HydraDevOps'";
            }
            else if (baseObject.Name == "RipleyEntities")
            {
                predicate = "Container='HydraDevOpsContext'";
            }

            id = "/" + baseObject.GetType().Name + "[@" + predicate + "]";

            if (baseObject.Parent != null)
            {
                id = baseObject.Parent.ID + id;
            }

            return id;
        }

        public string GetAppDescription(IGeneratorConfiguration generatorConfiguration, string argumentsKind)
        {
            throw new NotImplementedException();
        }
    }
}
