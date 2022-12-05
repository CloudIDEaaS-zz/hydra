using AbstraX.Angular;
using AbstraX.DataAnnotations;
using AbstraX.FolderStructure;
using AbstraX.Generators.Modules.NavigationGridModule;
using AbstraX.ServerInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Handlers.FacetHandlers
{
    [AngularModuleKindHandler(ModuleKind.AngularModule, UIKindGuids.NavigationGridPageWithEdits, UIFeatureKind.Custom)]
    public class NavigationGridModuleKindHandler : IModuleKindHandler
    {
        public float Priority => 1.0f;
        public event ProcessFacetsHandler ProcessFacets;

        public NavigationGridModuleKindHandler()
        {
        }

        public bool Process(Enum moduleKind, IModuleObject moduleObject, Folder folder, IGeneratorConfiguration generatorConfiguration)
        {
            var angularModule = (AngularModule)moduleObject;
            var baseObject = moduleObject.BaseObject;
            var imports = generatorConfiguration.CreateImports(this, angularModule, folder, true);
            var name = baseObject.GetNavigationName();
            var parentObject = (IParentBase)baseObject;

            if (!moduleObject.ValidateModuleName(name, out string error))
            {
                DebugUtils.Break(error);
            }

            angularModule.AddImportsAndRoutes(imports);

            NavigationGridModuleGenerator.GenerateModule(baseObject, angularModule, folder.fullPath, name, generatorConfiguration, imports, angularModule.UILoadKind, angularModule.UIKind);

            return true;
        }

        public void AddRange(IGeneratorConfiguration generatorConfiguration, IModuleAssembly moduleAssembly, List<IModuleOrAssembly> modulesOrAssemblies, IEnumerable<IModuleOrAssembly> addModulesOrAssemblies, ModuleAddType moduleAddType)
        {
            modulesOrAssemblies.AddRange(addModulesOrAssemblies);
        }
    }
}
