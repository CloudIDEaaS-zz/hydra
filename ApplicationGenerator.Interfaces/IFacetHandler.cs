using AbstraX.ServerInterfaces;
using EntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AbstraX
{
    public interface IFacetHandler : IModuleHandler
    {
        bool ForLife { get; }
        FacetHandlerLayer FacetHandlerLayer { get; }
        event ProcessFacetsHandler ProcessFacets;
        List<Module> RelatedModules { get; }
        bool Process(IBase baseObject, Facet facet, IGeneratorConfiguration generatorConfiguration);
        bool PreProcess(IBase baseObject, IGeneratorConfiguration generatorConfiguration, IFacetHandler currentHandler);
        bool PostProcess(IBase baseObject, IGeneratorConfiguration generatorConfiguration, IFacetHandler currentHandler);
    }
}
