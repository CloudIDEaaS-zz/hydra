using AbstraX.FolderStructure;
using AbstraX.ServerInterfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX
{
    public interface IModuleAssemblyProperties
    {
        IBase BaseObject { get; }
        IGeneratorConfiguration Configuration { get; set; }

        void UpdateModuleAssembly(IModuleAssembly moduleAssembly);
        void AddDefaultFile(FolderStructure.File file);
        void Clear();
    }
}
