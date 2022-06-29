// file:	Models\AssemblyModels\AssemblyModelEntityType.cs
//
// summary:	Implements the assembly model entity type class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using AbstraX.ServerInterfaces;
using AbstraX.XPathBuilder;
using System.Runtime.Serialization;
using System.Diagnostics;
using AbstraX;
using AbstraX.TypeMappings;
using AbstraX.Models.Interfaces;
using Utils;
using AbstraX.Models;
using AssemblyProvider.Web.Entities;

namespace AssemblyModelEntityProvider.Web.Entities
{
    /// <summary>   An assembly model entity type. </summary>
    ///
    /// <remarks>   Ken, 11/1/2020. </remarks>

    [DebuggerDisplay(" { Name } ")]
    public class AssemblyModelEntityType : AssemblyModelEntityBase, IElementWithKeyAttribute, IElementWithSurrogateTemplateType, IEntityWithOptionalFacets, IEntityType
    {
        /// <summary>   The name. </summary>
        protected string name;
        private AssemblyType entityType;

        /// <summary>   The query where property. </summary>
        protected string queryWhereProperty;
        /// <summary>   The query where value. </summary>
        protected object queryWhereValue;
        /// <summary>   The parent. </summary>
        protected IParentBase parent;
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
        protected BaseType dataType;
        /// <summary>   The design comments. </summary>
        protected string designComments;
        /// <summary>   The identifier. </summary>
        protected string id;
        /// <summary>   Identifier for the temporary. </summary>
        protected string tempID;

        /// <summary>   Gets a value indicating whether the follow without. </summary>
        ///
        /// <value> True if follow without, false if not. </value>

        public bool FollowWithout => false;

        /// <summary>
        /// Gets or sets a value indicating whether the no user interface or configuration.
        /// </summary>
        ///
        /// <value> True if no user interface or configuration, false if not. </value>

        public bool NoUIOrConfig { get; set; }
        /// <summary>   The registered surrogate templates. </summary>
        private Dictionary<Type, Type> registeredSurrogateTemplates;

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <param name="entityType">       Type of the entity. </param>
        /// <param name="assembly">         The assembly. </param>
        /// <param name="typesProvider">    The types provider. </param>
        /// <param name="parent">           The parent. </param>

        public AssemblyModelEntityType(AssemblyType entityType, AssemblyProvider.Web.Entities.Assembly assembly, ITypesProvider typesProvider, BaseObject parent) : base(parent)
        {
            string _namespace = null;
            var ancestor = (IBase)parent;
            var AssemblyModelEntityParent = (AssemblyModelEntityBase)parent;

            this.NoUIOrConfig = ((IEntityWithOptionalFacets)parent).NoUIOrConfig;

            this.parent = (IParentBase) parent;
            this.name = entityType.Name;
            this.typesProvider = typesProvider;
            this.assembly = assembly;
            this.entityType = entityType;
            this.registeredSurrogateTemplates = new Dictionary<Type, Type>();

            id = this.MakeID("Entity='" + name + "'");
            _namespace = entityType.Namespace;

            dataType = new BaseType
            {
                FullyQualifiedName = _namespace + "." + name,
                Name = name,
                ID = name,
                ParentID = this.ID
            };

            debugInfo = this.GetDebugInfo();
        }

        /// <summary>   Gets the properties. </summary>
        ///
        /// <value> The properties. </value>

        public List<IEntityProperty> Properties
        {
            get
            {
                var properties = new List<AssemblyModelEntityProperty>();

                foreach (var property in this.entityType.PropertyAttributes)
                {
                    properties.Add(new AssemblyModelEntityProperty(property, assembly, typesProvider, this));
                }

                return properties.ToList<IEntityProperty>();
            }
        }

        /// <summary>   Gets the attributes. </summary>
        ///
        /// <value> The attributes. </value>

        public IEnumerable<IAttribute> Attributes
        {
            get 
            {
                var attributes = this.Properties.AsQueryable().Cast<IAttribute>().Select(e => e);

                return attributes;
            }
        }

        /// <summary>   Gets the navigation properties. </summary>
        ///
        /// <value> The navigation properties. </value>

        public List<INavigationProperty> NavigationProperties
        {
            get
            {
                var properties = new List<AssemblyModelNavigationProperty>();

                foreach (var property in this.entityType.PropertyElements)
                {
                    properties.Add(new AssemblyModelNavigationProperty(property, assembly, typesProvider, this));
                }

                return properties.ToList<INavigationProperty>();
            }
        }

        /// <summary>   Gets the child elements. </summary>
        ///
        /// <value> The child elements. </value>

        public IEnumerable<IElement> ChildElements
        {
            get 
            {
                var elements = this.NavigationProperties.AsQueryable().Cast<IElement>().Select(e => e);
                return elements;
            }
        }

        /// <summary>   Gets the child nodes. </summary>
        ///
        /// <value> The child nodes. </value>

        public IEnumerable<IBase> ChildNodes
        {
            get
            {
                var nodes = this.NavigationProperties.AsQueryable().Cast<IBase>().Concat(
                    this.Properties.AsQueryable().Cast<IBase>()
                );

                return nodes;
            }
        }

        /// <summary>   Gets the operations. </summary>
        ///
        /// <value> The operations. </value>

        public IEnumerable<IOperation> Operations
        {
            get { return null; }
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

        /// <summary>   Gets a value indicating whether this  is container. </summary>
        ///
        /// <value> True if this  is container, false if not. </value>

        public bool IsContainer
        {
            get
            {
                return false;
            }
        }

        /// <summary>   Gets the type of the data. </summary>
        ///
        /// <value> The type of the data. </value>

        public BaseType DataType
        {
            get
            {
                return dataType;
            }
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
            if (value is XPathElement && (property == "ID" || property == "ParentID"))
            {
                var predicate = ((XPathElement)value).Predicates.First();

                queryWhereProperty = predicate.Left.ToString();
                queryWhereValue = predicate.Right.ToString();
            }
            else
            {
                System.Diagnostics.Debugger.Break();

                queryWhereProperty = property;
                queryWhereValue = value;
            }

            return this.ChildElements.AsQueryable();
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

        public IQueryable ExecuteWhere(XPathElement element)
        {
            var predicate = element.Predicates.First();

            queryWhereProperty = predicate.Left.ToString();
            queryWhereValue = predicate.Right.ToString();

            return this.ChildElements.AsQueryable();
        }

        /// <summary>   Clears the predicates. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>

        public void ClearPredicates()
        {
            queryWhereProperty = null;
            queryWhereValue = null;
        }

        /// <summary>   Gets key attribute. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <returns>   The key attribute. </returns>

        public IAttribute GetKeyAttribute()
        {
            var keyAttribute = this.Properties.Cast<AssemblyModelEntityProperty>().Single(p => p.Facets.Any(f => f.Attribute.GetType().Name == "KeyAttribute"));

            return keyAttribute;
        }

        /// <summary>   Gets surrogate template type. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <typeparam name="TSurrogateFor">    Type of the surrogate for. </typeparam>
        ///
        /// <returns>   The surrogate template type. </returns>

        public Type GetSurrogateTemplateType<TSurrogateFor>()
        {
            return this.registeredSurrogateTemplates[typeof(TSurrogateFor)];
        }

        /// <summary>   Query if this  has surrogate template type. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <typeparam name="TSurrogateFor">    Type of the surrogate for. </typeparam>
        ///
        /// <returns>   True if surrogate template type, false if not. </returns>

        public bool HasSurrogateTemplateType<TSurrogateFor>()
        {
            return this.registeredSurrogateTemplates.ContainsKey(typeof(TSurrogateFor));
        }

        /// <summary>   Registers the surrogate template type. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <typeparam name="T">            Generic type parameter. </typeparam>
        /// <typeparam name="TSurrogate">   Type of the surrogate. </typeparam>

        public void RegisterSurrogateTemplateType<T, TSurrogate>()
        {
            this.registeredSurrogateTemplates.Add(typeof(T), typeof(TSurrogate));
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
            get { return documentation; }
        }

        /// <summary>   Gets a value indicating whether this  has documentation. </summary>
        ///
        /// <value> True if this  has documentation, false if not. </value>

        public override bool HasDocumentation
        {
            get { return hasDocumentation; }
        }

        /// <summary>   Gets the documentation summary. </summary>
        ///
        /// <value> The documentation summary. </value>

        public override string DocumentationSummary
        {
            get { return documentationSummary; }
        }

        /// <summary>   Gets the child ordinal. </summary>
        ///
        /// <value> The child ordinal. </value>

        public override float ChildOrdinal
        {
            get
            {
                return 0;
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

        /// <summary>   Gets the parent. </summary>
        ///
        /// <value> The parent. </value>

        public override IBase Parent
        {
            get
            {
                return this.parent;
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
                return DefinitionKind.Class;
            }
        }

        /// <summary>   Gets a value indicating whether this  has children. </summary>
        ///
        /// <value> True if this  has children, false if not. </value>

        public override bool HasChildren
        {
            get
            {
                if (this.Properties.Any())
                {
                    return true;
                }
                else if (this.NavigationProperties.Any())
                {
                    return true;
                }

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
                var args = new GetAttributesEventArgs(this, this.dataType);
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

        /// <summary>   Gets a list of types of the allowable containers. </summary>
        ///
        /// <value> A list of types of the allowable containers. </value>

        public ContainerType AllowableContainerTypes
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>   Gets a list of types of the allowable constructs. </summary>
        ///
        /// <value> A list of types of the allowable constructs. </value>

        public ConstructType AllowableConstructTypes
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>   Gets the default container type. </summary>
        ///
        /// <value> The default container type. </value>

        public ContainerType DefaultContainerType
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>   Gets the default construct type. </summary>
        ///
        /// <value> The default construct type. </value>

        public ConstructType DefaultConstructType
        {
            get { throw new NotImplementedException(); }
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
    }
}
