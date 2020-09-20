using AbstraX.Angular;
using AbstraX.DataAnnotations;
using AbstraX.FolderStructure;
using AbstraX.Generators.Modules.EditModule;
using AbstraX.ServerInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Handlers.FacetHandlers
{
    [AngularModuleKindHandler(ModuleKind.AngularModule, DefinitionKind.Class, UIFeatureKind.Edit)]
    public class EditModuleKindHandler : IModuleAddModuleKindHandler
    {
        public float Priority => 1.0f;
        public event ProcessFacetsHandler ProcessFacets;

        public EditModuleKindHandler()
        {
        }

        public bool Process(Enum moduleKind, IModuleObject moduleObject, Folder folder, IGeneratorConfiguration generatorConfiguration)
        {
            var angularModule = (AngularModule)moduleObject;
            var baseObject = moduleObject.BaseObject;
            var name = baseObject.GetNavigationName();
            var imports = generatorConfiguration.CreateImports(this, angularModule, folder, true);
            var parentObject = (IParentBase)baseObject;

            if (!moduleObject.ValidateModuleName(name, out string error))
            {
                throw new Exception(error);
            }

            angularModule.AddImportsAndRoutes(imports);

            EditModuleGenerator.GenerateModule(baseObject, angularModule, folder.fullPath, name, generatorConfiguration, imports, false);

            return true;
        }

        public void AddRange(IGeneratorConfiguration generatorConfiguration, IModuleAssembly moduleAssembly, List<IModuleOrAssembly> modulesOrAssemblies, IEnumerable<IModuleOrAssembly> addModulesOrAssemblies, ModuleAddType moduleAddType)
        {
            modulesOrAssemblies.AddRange(addModulesOrAssemblies);
        }

        public void AddRange(IGeneratorConfiguration generatorConfiguration, IModuleAssembly moduleAssembly, List<IModuleOrAssembly> modulesOrAssemblies, ModuleAddType moduleAddType, Func<IModuleOrAssembly, bool> filter)
        {
            var generatorModules = (IEnumerable<ESModule>)generatorConfiguration.KeyValuePairs["Providers"];
            var addModules = generatorModules.Where(m => m is Validator && filter(m));

            modulesOrAssemblies.AddRange(addModules);
        }
    }
}
