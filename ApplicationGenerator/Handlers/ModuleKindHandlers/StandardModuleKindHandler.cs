using AbstraX.Angular;
using AbstraX.DataAnnotations;
using AbstraX.FolderStructure;
using AbstraX.Generators.Modules.StandardModule;
using AbstraX.ServerInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Handlers.FacetHandlers
{
    [AngularModuleKindHandler(ModuleKind.AngularModule, UIFeatureKind.Standard)]
    public class StandardModuleKindHandler : IModuleKindHandler
    {
        public float Priority => 1.0f;
        public event ProcessFacetsHandler ProcessFacets;

        public StandardModuleKindHandler()
        {
        }

        public bool Process(Enum moduleKind, IModuleObject moduleObject, Folder folder, IGeneratorConfiguration generatorConfiguration)
        {
            var angularModule = (AngularModule)moduleObject;
            var baseObject = moduleObject.BaseObject;
            var imports = generatorConfiguration.CreateImports(this, angularModule, folder, true);
            var name = baseObject.GetNavigationName(angularModule.UILoadKind);
            var parentObject = (IParentBase)baseObject;

            if (!moduleObject.ValidateModuleName(name, out string error))
            {
                throw new Exception(error);
            }

            angularModule.AddImportsAndRoutes(imports);

            StandardModuleGenerator.GenerateModule(baseObject, angularModule, folder.fullPath, name, generatorConfiguration, imports);

            return true;
        }

        public void AddRange(IGeneratorConfiguration generatorConfiguration, IModuleAssembly moduleAssembly, List<IModuleOrAssembly> modulesOrAssemblies, IEnumerable<IModuleOrAssembly> addModulesOrAssemblies, ModuleAddType moduleAddType)
        {
            modulesOrAssemblies.AddRange(addModulesOrAssemblies);
        }
    }
}
