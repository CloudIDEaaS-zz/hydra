using AbstraX.ServerInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstraX.FolderStructure;
using AbstraX.DataAnnotations;

namespace AbstraX
{
    public interface IModuleAssembly : IModuleObject, IModuleOrAssembly
    {
        File File { get; set; }
        File ForChildFile { get; set; }
        UILoadKind UILoadKind { get; set; }
        Dictionary<string, List<Module>> BaseExports { get; }
        List<Module> ExportedComponents { get; set; }
        Module FindExport(string name);
        IEnumerable<Module> GetExports(IBase baseObject);
        IEnumerable<Module> GetExports(string attribute);
        bool HasExports(IBase baseObject);
    }
}
