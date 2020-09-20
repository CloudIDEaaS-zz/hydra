using AbstraX.FolderStructure;
using AbstraX.ServerInterfaces;
using EntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AbstraX
{
    public interface IModuleKindHandler
    {
        event ProcessFacetsHandler ProcessFacets;
        float Priority { get; }
        bool Process(Enum moduleKind, IModuleObject moduleObject, Folder folder, IGeneratorConfiguration generatorConfiguration);
        void AddRange(IGeneratorConfiguration generatorConfiguration, IModuleAssembly moduleAssembly, List<IModuleOrAssembly> modulesOrAssemblies, IEnumerable<IModuleOrAssembly> addModulesOrAssemblies, ModuleAddType moduleAddType);
    }
}
