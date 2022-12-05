using AbstraX.Angular;
using AbstraX.DataAnnotations;
using AbstraX.FolderStructure;
using AbstraX.Generators.Modules.AppModule;
using AbstraX.ServerInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Handlers.FacetHandlers
{
    [AngularModuleKindHandler(ModuleKind.AngularModule, DefinitionKind.Model)]
    public class AppModuleKindHandler : IModuleKindHandler
    {
        public float Priority => 1.0f;
        public event ProcessFacetsHandler ProcessFacets;

        public AppModuleKindHandler()
        {
        }

        public bool Process(Enum moduleKind, IModuleObject moduleObject, Folder folder, IGeneratorConfiguration generatorConfiguration)
        {
            var angularModule = (AngularModule)moduleObject;
            var baseObject = moduleObject.BaseObject;
            IEnumerable<ModuleImportDeclaration> imports;
            var name = baseObject.GetNavigationName();
            var parentObject = (IParentBase)baseObject;
            var modulesOrAssemblies = new List<IModuleOrAssembly>();

            if (!moduleObject.ValidateModuleName(name, out string error))
            {
                throw new Exception(error);
            }

            if (generatorConfiguration.KeyValuePairs.ContainsKey("Providers"))
            {
                var providers = (List<Provider>)generatorConfiguration.KeyValuePairs["Providers"];

                foreach (var provider in providers)
                {
                    angularModule.Providers.Add(provider);
                }

                modulesOrAssemblies = providers.Cast<IModuleOrAssembly>().ToList();
                imports = generatorConfiguration.CreateImports(this, angularModule, modulesOrAssemblies, folder, true);
            }
            else
            {
                imports = generatorConfiguration.CreateImports(this, angularModule, folder, true);
            }

            AppModuleGenerator.GenerateModule(baseObject, angularModule, folder.fullPath, name, generatorConfiguration, imports);

            return true;
        }

        public void AddRange(IGeneratorConfiguration generatorConfiguration, IModuleAssembly moduleAssembly, List<IModuleOrAssembly> modulesOrAssemblies, IEnumerable<IModuleOrAssembly> addModulesOrAssemblies, ModuleAddType moduleAddType)
        {
            modulesOrAssemblies.AddRange(addModulesOrAssemblies);
        }
    }
}
