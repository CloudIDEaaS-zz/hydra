// file:	Models\AssemblyModels\AssemblyModelEntityProperty.cs
//
// summary:	Implements the assembly model entity property class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using AbstraX.ServerInterfaces;
using System.Runtime.Serialization;
using System.Diagnostics;
using AbstraX;
using AbstraX.Models.Interfaces;
using AbstraX.TypeMappings;
using System.Xml.Schema;
using AssemblyProvider.Web.Entities;
using Utils;

namespace AssemblyModelEntityProvider.Web.Entities
{
    /// <summary>   An assembly model entity property. </summary>
    ///
    /// <remarks>   Ken, 11/1/2020. </remarks>

    [DebuggerDisplay(" { Name } ")]
    public class AssemblyModelEntityProperty : AssemblyModelEntityBase, IEntityProperty, IAttribute, IEntityWithOptionalFacets
    {
        /// <summary>   The name. </summary>
        protected string name;
        /// <summary>   The node. </summary>
        protected XmlNode node;
        /// <summary>   The parent. </summary>
        protected AssemblyModelEntityType parent;
        /// <summary>   The type attribute. </summary>
        protected string typeAttribute;
        /// <summary>   True if has documentation, false if not. </summary>
        protected bool hasDocumentation;
        /// <summary>   The documentation summary. </summary>
        protected string documentationSummary;
        /// <summary>   The documentation. </summary>
        protected string documentation;
        /// <summary>   The child ordinal. </summary>
        protected float childOrdinal;
        /// <summary>   Information describing the debug. </summary>
        protected string debugInfo;
        /// <summary>   Type of the data. </summary>
        protected ScalarType dataType;
        /// <summary>   The identifier. </summary>
        protected string id;
        /// <summary>   True if nullable. </summary>
        private bool nullable;

        /// <summary>   Gets a value indicating whether the follow without. </summary>
        ///
        /// <value> True if follow without, false if not. </value>

        public bool FollowWithout => false;

        private PropertyAttribute propertyAttribute;

        /// <summary>
        /// Gets or sets a value indicating whether the no user interface or configuration.
        /// </summary>
        ///
        /// <value> True if no user interface or configuration, false if not. </value>

        public bool NoUIOrConfig { get; set; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <param name="propertyAttribute">    The property attribute. </param>
        /// <param name="assembly">             The assembly. </param>
        /// <param name="typesProvider">        The types provider. </param>
        /// <param name="parent">               The parent. </param>

        public AssemblyModelEntityProperty(PropertyAttribute propertyAttribute, AssemblyProvider.Web.Entities.Assembly assembly, ITypesProvider typesProvider, AssemblyModelEntityType parent) : base(parent)
        {
            Type type;

            this.name = propertyAttribute.Name;
            this.parent = parent;
            this.assembly = assembly;
            this.typesProvider = typesProvider;
            this.propertyAttribute = propertyAttribute;

            this.NoUIOrConfig = parent.NoUIOrConfig;

            id = this.MakeID("Property='" + this.name + "'");

            type = Type.GetType("System." + propertyAttribute.DataType.Name);

            if (type == null)
            {
                DebugUtils.Break();
            }

            dataType = new ScalarType(type, this);
            debugInfo = this.GetDebugInfo();
        }

        /// <summary>   Gets or sets the identifier. </summary>
        ///
        /// <value> The identifier. </value>

        public override string ID
        {
            get
            {
                return id;
            }
            
            protected set
            {
            }
        }

        /// <summary>   Gets the name. </summary>
        ///
        /// <value> The name. </value>

        public override string Name
        {
            get 
            {
                return name;
            }
        }

        /// <summary>   Gets the type of the data. </summary>
        ///
        /// <value> The type of the data. </value>

        public ScalarType DataType
        {
            get
            {
                return dataType;
            }
        }

        /// <summary>   Gets the child elements. </summary>
        ///
        /// <value> The child elements. </value>

        public IEnumerable<IElement> ChildElements
        {
            get { return null; }
        }

        /// <summary>   Gets the child nodes. </summary>
        ///
        /// <value> The child nodes. </value>

        public IEnumerable<IBase> ChildNodes
        {
            get { return null; }
        }

        /// <summary>   Executes the where operation. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <param name="property"> The property. </param>
        /// <param name="value">    The value. </param>
        ///
        /// <returns>   An IQueryable. </returns>

        public IQueryable ExecuteWhere(string property, object value)
        {
            throw new NotImplementedException();
        }

        /// <summary>   Executes the where operation. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <param name="expression">   The expression. </param>
        ///
        /// <returns>   An IQueryable. </returns>

        public IQueryable ExecuteWhere(System.Linq.Expressions.Expression expression)
        {
            throw new NotImplementedException();
        }

        /// <summary>   Executes the where operation. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <param name="element">  The element. </param>
        ///
        /// <returns>   An IQueryable. </returns>

        public IQueryable ExecuteWhere(AbstraX.XPathBuilder.XPathElement element)
        {
            throw new NotImplementedException();
        }

        /// <summary>   Clears the predicates. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>

        public void ClearPredicates()
        {
            throw new NotImplementedException();
        }

        /// <summary>   Gets the root. </summary>
        ///
        /// <value> The root. </value>

        public override IRoot Root
        {
            get
            {
                IBase baseObject = this;

                while (baseObject != null)
                {
                    baseObject = baseObject.Parent;

                    if (baseObject is IRoot)
                    {
                        return (IRoot)baseObject;
                    }
                }

                return null;
            }
        }

        /// <summary>   Gets the design comments. </summary>
        ///
        /// <value> The design comments. </value>

        public override string DesignComments
        {
            get
            {
                return string.Format("Type: {0}, Kind {1}, ID:'{2}'", this.GetType().Name, this.Kind, this.ID);
            }
        }

        /// <summary>   Gets the documentation. </summary>
        ///
        /// <value> The documentation. </value>

        public override string Documentation
        {
            get
            {
                return documentation;
            }
        }

        /// <summary>   Gets a value indicating whether this  has documentation. </summary>
        ///
        /// <value> True if this  has documentation, false if not. </value>

        public override bool HasDocumentation
        {
            get
            {
                return hasDocumentation;
            }
        }

        /// <summary>   Gets the documentation summary. </summary>
        ///
        /// <value> The documentation summary. </value>

        public override string DocumentationSummary
        {
            get
            {
                return documentationSummary;
            }
        }

        /// <summary>   Gets the child ordinal. </summary>
        ///
        /// <value> The child ordinal. </value>

        public override float ChildOrdinal
        {
            get
            {
                return childOrdinal;
            }
        }

        /// <summary>   Gets information describing the debug. </summary>
        ///
        /// <value> Information describing the debug. </value>

        public override string DebugInfo
        {
            get
            {
                return debugInfo;
            }
        }

        /// <summary>   Gets the identifier of the parent. </summary>
        ///
        /// <value> The identifier of the parent. </value>

        public override string ParentID
        {
            get 
            {
                return parent.ID;
            }
        }

        /// <summary>   Gets the parent. </summary>
        ///
        /// <value> The parent. </value>

        public override IBase Parent
        {
            get
            {
                return parent;
            }
        }

        /// <summary>   Gets or sets the folder key pair. </summary>
        ///
        /// <value> The folder key pair. </value>

        public override string FolderKeyPair { get; set; }

        /// <summary>   Gets the kind. </summary>
        ///
        /// <value> The kind. </value>

        public override DefinitionKind Kind
        {
            get
            {
                return DefinitionKind.SimpleProperty;
            }
        }

        /// <summary>   Gets a value indicating whether this  has children. </summary>
        ///
        /// <value> True if this  has children, false if not. </value>

        public override bool HasChildren
        {
            get
            {
                return false;
            }
        }

        /// <summary>   Gets the facets. </summary>
        ///
        /// <value> The facets. </value>

        public override Facet[] Facets
        {
            get
            {
                var args = new GetAttributesEventArgs(this, this.parent.DataType, this.Name);
                var facets = new List<Facet>();

                foreach (var handler in this.getAttributesEventHandlers)
                {
                    handler(this, args);

                    if (args.Attributes != null)
                    {
                        foreach (var attribute in args.Attributes)
                        {
                            var facet = new Facet(attribute);

                            facets.Add(facet);
                        }
                    }
                }

                return facets.ToArray();
            }
        }

        /// <summary>   Gets the modifiers. </summary>
        ///
        /// <value> The modifiers. </value>

        public override Modifiers Modifiers
        {
            get
            {
                return Modifiers.Unknown;
            }
        }

        /// <summary>   Gets the default value. </summary>
        ///
        /// <value> The default value. </value>

        public string DefaultValue
        {
            get
            {
                return null;
            }
        }

        /// <summary>   Gets a value indicating whether this  is nullable. </summary>
        ///
        /// <value> True if nullable, false if not. </value>

        public bool Nullable
        {
            get
            {
                return nullable;
            }
        }
    }
}
