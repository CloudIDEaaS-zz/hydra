using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstraX.Angular;
using AbstraX.ServerInterfaces;

namespace AbstraX.Handlers.FacetHandlers
{
    public abstract class BaseNonPageFacetHandler : IFacetHandler
    {
        public virtual float Priority => 4.0f;
        public bool ForLife => false;
        public event ProcessFacetsHandler ProcessFacets;
        public List<Module> RelatedModules { get; }
        public abstract FacetHandlerLayer FacetHandlerLayer { get; }

        public BaseNonPageFacetHandler()
        {
            this.RelatedModules = new List<Module>();
        }

        public abstract bool Process(IBase baseObject, Facet facet, IGeneratorConfiguration generatorConfiguration);

        public virtual bool PostProcess(IBase baseObject, IGeneratorConfiguration generatorConfiguration, IFacetHandler currentHandler)
        {
            return true;
        }

        public virtual bool PreProcess(IBase baseObject, IGeneratorConfiguration generatorConfiguration, IFacetHandler currentHandler)
        {
            return true;
        }
        internal void Raise<T>()
        {
            this.ProcessFacets.Raise<T>(this);
        }

        public void AddRange(IBase baseObject, IGeneratorConfiguration generatorConfiguration, List<Module> modules, IEnumerable<Module> addModules, ModuleAddType moduleAddType)
        {
            throw new NotImplementedException();
        }
    }
}
