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
        /// <summary>   Event queue for all listeners interested in ProcessFacets events. </summary>
        public event ProcessFacetsHandler ProcessFacets;

        public EditModuleKindHandler()
        {
        }

        public bool Process(Enum moduleKind, IModuleObject moduleObject, Folder folder, IGeneratorConfiguration generatorConfiguration)
        {
            var angularModule = (AngularModule)moduleObject;
            var baseObject = moduleObject.BaseObject;
            var names = baseObject.GetNavigationNames(UIKind.EditPage, UIKind.LoginPage, UIKind.RegisterPage);
            var imports = generatorConfiguration.CreateImports(this, angularModule, folder, true);
            var parentObject = (IParentBase)baseObject;
            var errors = new List<string>();

            foreach (var name in names)
            {
                if (moduleObject.ValidateModuleName(name, out string error))
                {
                    angularModule.AddImportsAndRoutes(imports);

                    EditModuleGenerator.GenerateModule(baseObject, angularModule, folder.fullPath, name, generatorConfiguration, imports, false, angularModule.UILoadKind, angularModule.UIKind);

                    return true;
                }
                else
                {
                    errors.Add(error);
                }
            }

            throw new AggregateException("Module kind handler exception", errors.Select(e => new Exception(e)));
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
