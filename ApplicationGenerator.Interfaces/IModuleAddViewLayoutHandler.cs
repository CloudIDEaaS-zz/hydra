using AbstraX.ServerInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    public interface IModuleAddViewLayoutHandler : IViewLayoutHandler
    {
        void AddRange(IBase baseObject, IGeneratorConfiguration generatorConfiguration, List<Module> modules, ModuleAddType moduleAddType, Func<Module, bool> filter);
    }
}
