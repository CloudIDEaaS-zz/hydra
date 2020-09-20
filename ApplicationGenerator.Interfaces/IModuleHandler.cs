using AbstraX.ServerInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    public interface IModuleHandler : IHandler
    {
        void AddRange(IBase baseObject, IGeneratorConfiguration generatorConfiguration, List<Module> modules, IEnumerable<Module> addModules, ModuleAddType moduleAddType);
    }
}
