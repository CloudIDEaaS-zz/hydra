using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstraX.FolderStructure;

namespace AbstraX
{
    public class SurrogateModuleKindHandler : IModuleKindHandler
    {
        public float Priority => throw new NotImplementedException();

        public event ProcessFacetsHandler ProcessFacets;

        public void AddRange(IGeneratorConfiguration generatorConfiguration, IModuleAssembly moduleAssembly, List<IModuleOrAssembly> modulesOrAssemblies, IEnumerable<IModuleOrAssembly> addModulesOrAssemblies, ModuleAddType moduleAddType)
        {
            modulesOrAssemblies.AddRange(addModulesOrAssemblies);
        }

        public bool Process(Enum moduleKind, IModuleObject moduleObject, Folder folder, IGeneratorConfiguration generatorConfiguration)
        {
            throw new NotImplementedException();
        }

        public static IModuleKindHandler GetInstance()
        {
            return new SurrogateModuleKindHandler();
        }
    }
}
