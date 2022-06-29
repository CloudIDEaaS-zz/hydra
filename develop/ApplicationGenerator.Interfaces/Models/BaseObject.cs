// file:	Models\BaseObject.cs
//
// summary:	Implements the base object class

using AbstraX.ServerInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Models
{
    /// <summary>   A base object. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/11/2020. </remarks>

    public abstract class BaseObject : IBase
    {
        /// <summary>   The get attributes event handlers. </summary>
        protected List<GetAttributesEventHandler> getAttributesEventHandlers;
        /// <summary>   The get add insert event handlers. </summary>
        protected List<GetAddInEntitiesEventHandler> getAddInsEventHandlers;
        /// <summary>   The get override event handlers. </summary>
        protected List<GetOverridesEventHandler> getOverrideEventHandlers;

        /// <summary>   Gets or sets a dictionary of entities. </summary>
        ///
        /// <value> A dictionary of entities. </value>

        public Dictionary<string, IBase> EntityDictionary { get; set; }

        /// <summary>   Gets or sets the identifier. </summary>
        ///
        /// <value> The identifier. </value>

        public abstract string ID { get; protected set; }

        /// <summary>   Gets the identifier of the parent. </summary>
        ///
        /// <value> The identifier of the parent. </value>

        public abstract string ParentID { get; }

        /// <summary>   Gets the name. </summary>
        ///
        /// <value> The name. </value>

        public abstract string Name { get; }

        /// <summary>   Gets the design comments. </summary>
        ///
        /// <value> The design comments. </value>

        public abstract string DesignComments { get; }

        /// <summary>   Gets the documentation. </summary>
        ///
        /// <value> The documentation. </value>

        public abstract string Documentation { get; }

        /// <summary>   Gets the documentation summary. </summary>
        ///
        /// <value> The documentation summary. </value>

        public abstract string DocumentationSummary { get; }

        /// <summary>   Gets a value indicating whether this  has documentation. </summary>
        ///
        /// <value> True if this  has documentation, false if not. </value>

        public abstract bool HasDocumentation { get; }

        /// <summary>   Gets the child ordinal. </summary>
        ///
        /// <value> The child ordinal. </value>

        public abstract float ChildOrdinal { get; }

        /// <summary>   Gets information describing the debug. </summary>
        ///
        /// <value> Information describing the debug. </value>

        public abstract string DebugInfo { get; }

        /// <summary>   Gets the parent. </summary>
        ///
        /// <value> The parent. </value>

        public abstract IBase Parent { get; }

        /// <summary>   Gets or sets the folder key pair. </summary>
        ///
        /// <value> The folder key pair. </value>

        public abstract string FolderKeyPair { get; set; }

        /// <summary>   Gets the kind. </summary>
        ///
        /// <value> The kind. </value>

        public abstract DefinitionKind Kind { get; }

        /// <summary>   Gets the modifiers. </summary>
        ///
        /// <value> The modifiers. </value>

        public abstract Modifiers Modifiers { get; }

        /// <summary>   Gets a value indicating whether this  has children. </summary>
        ///
        /// <value> True if this  has children, false if not. </value>

        public abstract bool HasChildren { get; }

        /// <summary>   Gets the facets. </summary>
        ///
        /// <value> The facets. </value>

        public abstract Facet[] Facets { get; }

        /// <summary>   Gets the root. </summary>
        ///
        /// <value> The root. </value>

        public abstract IRoot Root { get; }

        /// <summary>   Gets or sets a value indicating whether this  is root. </summary>
        ///
        /// <value> True if this  is root, false if not. </value>

        public bool IsRoot { get; private set; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/11/2020. </remarks>
        ///
        /// <param name="parent">   The parent. </param>

        public BaseObject(BaseObject parent)
        {
            this.getAttributesEventHandlers = parent.AttributesEventHandlers;
            this.getAddInsEventHandlers = parent.AddInsEventHandlers;
            this.getOverrideEventHandlers = parent.OverrideEventHandlers;
        }

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/11/2020. </remarks>

        public BaseObject()
        {
            getAttributesEventHandlers = new List<GetAttributesEventHandler>();
            getAddInsEventHandlers = new List<GetAddInEntitiesEventHandler>();
            getOverrideEventHandlers = new List<GetOverridesEventHandler>();

            this.IsRoot = true;
        }

        /// <summary>   Event queue for all listeners interested in OnGetAttributes events. </summary>
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

        /// <summary>   Event queue for all listeners interested in OnGetAddIns events. </summary>
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

        /// <summary>   Event queue for all listeners interested in OnGetOverrides events. </summary>
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

        /// <summary>   Gets the override event handlers. </summary>
        ///
        /// <value> The override event handlers. </value>

        internal List<GetOverridesEventHandler> OverrideEventHandlers
        {
            get
            {
                return getOverrideEventHandlers;
            }
        }

        /// <summary>   Gets the attributes event handlers. </summary>
        ///
        /// <value> The attributes event handlers. </value>

        internal List<GetAttributesEventHandler> AttributesEventHandlers
        {
            get
            {
                return getAttributesEventHandlers;
            }
        }

        /// <summary>   Gets the add insert event handlers. </summary>
        ///
        /// <value> The add insert event handlers. </value>

        internal List<GetAddInEntitiesEventHandler> AddInsEventHandlers
        {
            get
            {
                return getAddInsEventHandlers;
            }
        }

        /// <summary>   Gets the override event handler. </summary>
        ///
        /// <value> The override event handler. </value>

        public GetOverridesEventHandler OverrideEventHandler
        {
            get
            {
                return this.getOverrideEventHandlers.LastOrDefault();
            }
        }

        /// <summary>   Gets override identifier. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/11/2020. </remarks>
        ///
        /// <param name="predicate">    The predicate. </param>
        /// <param name="generatedId">  Identifier for the generated. </param>
        ///
        /// <returns>   The override identifier. </returns>

        public string GetOverrideId(string predicate, string generatedId)
        {
            if (this.OverrideEventHandler != null)
            {
                return this.OverrideEventHandler.GetOverrideId(this, predicate, generatedId);
            }
            else
            {
                return generatedId;
            }
        }

        /// <summary>   Skip process. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/11/2020. </remarks>
        ///
        /// <param name="facetHandler">             The facet handler. </param>
        /// <param name="facet">                    The facet. </param>
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public bool SkipProcess(IFacetHandler facetHandler, Facet facet, IGeneratorConfiguration generatorConfiguration)
        {
            if (this.OverrideEventHandler != null)
            {
                return this.OverrideEventHandler.SkipProcess(facetHandler, this, facet, generatorConfiguration);
            }
            else
            {
                return false;
            }
        }
    }
}
