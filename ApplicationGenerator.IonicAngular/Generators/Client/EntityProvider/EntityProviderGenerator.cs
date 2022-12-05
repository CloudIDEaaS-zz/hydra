using AbstraX.Angular;
using AbstraX.Generators.Client.EntityProvider;
using AbstraX.ServerInterfaces;
using EntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using static AbstraX.FolderStructure.FileSystemObject;

namespace AbstraX.Generators.Client.EntityProvider
{
    [GeneratorTemplate("Class", @"Generators\Client\EntityProvider\EntityProviderClassTemplate.tt")]
    public static class EntityProviderGenerator
    {
        public static void GenerateProvider(IBase baseObject, string providersFolderPath, string providerName, IGeneratorConfiguration generatorConfiguration, IEnumerable<ModuleImportDeclaration> imports, List<RelatedEntity> relatedEntities)
        {
            var host = generatorConfiguration.GetTemplateEngineHost();
            var pass = generatorConfiguration.CurrentPass;
            var moduleAssemblyProperties = new AngularModuleAssemblyProperties(baseObject, imports);
            var newProviders = new List<Provider>();
            var isIdentityEntity = generatorConfiguration.IsIdentityEntity(baseObject);
            List<Provider> providers = null;
            Dictionary<string, object> sessionVariables;
            FileInfo fileInfo;
            string fileLocation;
            string filePath;
            string output;

            try
            {
                // Entity provider

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("ProviderName", providerName);
                sessionVariables.Add("EntityName", baseObject.Name);
                sessionVariables.Add("RelatedEntities", relatedEntities);
                sessionVariables.Add("IsIdentityEntity", isIdentityEntity);
                sessionVariables.AddModuleAssemblyProperties(moduleAssemblyProperties);

                if (isIdentityEntity)
                {
                    var className = providerName + "Provider";

                    sessionVariables.Add("ClientId", generatorConfiguration.ClientId);
                    sessionVariables.Add("ClientSecret", generatorConfiguration.ClientSecret);

                    generatorConfiguration.IdentityProvider = className;
                }

                if (generatorConfiguration.CustomQueries.ContainsKey(baseObject))
                {
                    var queriesList = generatorConfiguration.CustomQueries[baseObject];

                    sessionVariables.Add("CustomQueries", queriesList);
                }
                else if (generatorConfiguration.CustomQueries.ContainsNavigationKey(baseObject))
                {
                    var queriesList = generatorConfiguration.CustomQueries.GetNavigationValue(baseObject);

                    sessionVariables.Add("CustomQueries", queriesList);
                }

                fileLocation = providersFolderPath;
                filePath = PathCombine(fileLocation, providerName.ToLower() + ".provider.ts");
                fileInfo = new FileInfo(filePath);

                host.SetGenerator(typeof(EntityProviderGenerator), "Class");

                do
                {
                    output = host.Generate<EntityProviderClassTemplate>(sessionVariables, false);

                    if (generatorConfiguration.KeyValuePairs.ContainsKey("Providers"))
                    {
                        providers = (List<Provider>)generatorConfiguration.KeyValuePairs["Providers"];
                    }
                    else
                    {
                        providers = new List<Provider>();

                        generatorConfiguration.KeyValuePairs.Add("Providers", providers);
                    }

                    foreach (var provider in moduleAssemblyProperties.Providers)
                    {
                        provider.BaseObject = baseObject;

                        if (!providers.Contains(provider))
                        {
                            providers.Add(provider);
                        }

                        newProviders.Add(provider);
                    }

                    generatorConfiguration.CreateFile(fileInfo, newProviders.Cast<Module>(), output, FileKind.Project, () => generatorConfiguration.GenerateInfo(sessionVariables, "Entity Provider Class"));
                }
                while (host.PostProcess() == PostProcessResult.RedoGenerate);
            }
            catch (Exception e)
            {
                generatorConfiguration.AppGeneratorEngine.WriteError(e.ToString());
            }
        }
    }
}
