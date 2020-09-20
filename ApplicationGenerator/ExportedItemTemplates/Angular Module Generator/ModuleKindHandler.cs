using AbstraX.Angular;
using AbstraX.DataAnnotations;
using AbstraX.FolderStructure;
using AbstraX.Generators.$basename$Module;
using AbstraX.ServerInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.FacetHandlers
{
    // TODO - change DefinitionKind
    [AngularModuleKindHandler(ModuleKind.AngularModule, DefinitionKind.Unknown)]
    public class $basename$ModuleKindHandler : IModuleKindHandler
    {
        public float Priority => 1.0f;
        public event ProcessFacetsHandler ProcessFacets;

        public $basename$ModuleKindHandler()
        {
        }

        public bool Process(Enum moduleKind, IModuleObject moduleObject, Folder folder, IGeneratorConfiguration generatorConfiguration)
        {
            var angularModule = (AngularModule)moduleObject;
            var baseObject = moduleObject.BaseObject;
            var imports = generatorConfiguration.CreateImports(angularModule, folder, true);
            var name = baseObject.GetNavigationName();
            var parentObject = (IParentBase)baseObject;

            $basename$ModuleGenerator.GenerateModule(baseObject, angularModule, folder.fullPath, name, generatorConfiguration, imports);

            return true;
        }
    }
}
