using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    public interface IModuleAddModuleKindHandler : IModuleKindHandler
    {
        void AddRange(IGeneratorConfiguration generatorConfiguration, IModuleAssembly moduleAssembly, List<IModuleOrAssembly> modulesOrAssemblies, ModuleAddType moduleAddType, Func<IModuleOrAssembly, bool> filter);
    }
}
