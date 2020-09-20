using AbstraX.ServerInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Models
{
    public abstract class BaseObject : IBase
    {
        protected List<GetAttributesEventHandler> getAttributesEventHandlers;
        protected List<GetAddInEntitiesEventHandler> getAddInsEventHandlers;
        protected List<GetOverridesEventHandler> getOverrideEventHandlers;
        public Dictionary<string, IBase> EntityDictionary { get; set; }
        public abstract string ID { get; protected set; }
        public abstract string ParentID { get; }
        public abstract string Name { get; }
        public abstract string DesignComments { get; }
        public abstract string Documentation { get; }
        public abstract string DocumentationSummary { get; }
        public abstract bool HasDocumentation { get; }
        public abstract float ChildOrdinal { get; }
        public abstract string DebugInfo { get; }
        public abstract IBase Parent { get; }
        public abstract string FolderKeyPair { get; set; }
        public abstract DefinitionKind Kind { get; }
        public abstract Modifiers Modifiers { get; }
        public abstract bool HasChildren { get; }
        public abstract Facet[] Facets { get; }
        public abstract IRoot Root { get; }
        public bool IsRoot { get; private set; }

        public BaseObject(BaseObject parent)
        {
            this.getAttributesEventHandlers = parent.AttributesEventHandlers;
            this.getAddInsEventHandlers = parent.AddInsEventHandlers;
            this.getOverrideEventHandlers = parent.OverrideEventHandlers;
        }

        public BaseObject()
        {
            getAttributesEventHandlers = new List<GetAttributesEventHandler>();
            getAddInsEventHandlers = new List<GetAddInEntitiesEventHandler>();
            getOverrideEventHandlers = new List<GetOverridesEventHandler>();

            this.IsRoot = true;
        }

        public event GetAttributesEventHandler OnGetAttributes
        {
            add
            {
                getAttributesEventHandlers.Add(value);
            }

            remove
            {
                getAttributesEventHandlers.Remove(value);
            }
        }

        public event GetAddInEntitiesEventHandler OnGetAddIns
        {
            add
            {
                getAddInsEventHandlers.Add(value);
            }

            remove
            {
                getAddInsEventHandlers.Remove(value);
            }
        }

        public event GetOverridesEventHandler OnGetOverrides
        {
            add
            {
                getOverrideEventHandlers.Add(value);
            }

            remove
            {
                getOverrideEventHandlers.Remove(value);
            }
        }

        internal List<GetOverridesEventHandler> OverrideEventHandlers
        {
            get
            {
                return getOverrideEventHandlers;
            }
        }

        internal List<GetAttributesEventHandler> AttributesEventHandlers
        {
            get
            {
                return getAttributesEventHandlers;
            }
        }

        internal List<GetAddInEntitiesEventHandler> AddInsEventHandlers
        {
            get
            {
                return getAddInsEventHandlers;
            }
        }

        public GetOverridesEventHandler OverrideEventHandler
        {
            get
            {
                return this.getOverrideEventHandlers.LastOrDefault();
            }
        }

        public string GetOverrideId(string predicate, string generatedId)
        {
            return this.OverrideEventHandler.GetOverrideId(this, predicate, generatedId);
        }

        public bool SkipProcess(IFacetHandler facetHandler, Facet facet, IGeneratorConfiguration generatorConfiguration)
        {
            return this.OverrideEventHandler.SkipProcess(facetHandler, this, facet, generatorConfiguration);
        }
    }
}
