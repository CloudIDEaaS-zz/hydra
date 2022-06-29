// file:	Models\AssemblyModels\AssemblyModelEntityContainer.cs
//
// summary:	Implements the assembly model entity container class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using AbstraX.ServerInterfaces;
using System.Diagnostics;
using AbstraX.XPathBuilder;
using System.Runtime.Serialization;
using AbstraX;
using AbstraX.TypeMappings;
using Utils;
using AbstraX.Models.Interfaces;
using AssemblyProvider.Web.Entities;

namespace AssemblyModelEntityProvider.Web.Entities
{
    /// <summary>   Container for assembly model entities. </summary>
    ///
    /// <remarks>   Ken, 11/1/2020. </remarks>

    [DebuggerDisplay(" { Name } ")]
    public class AssemblyModelEntityContainer : AssemblyModelEntityBase, IElement, IEntityObjectWithFacets, IEntityObjectWithDataType, IEntityContainer
    {
        private AssemblyType containerType;

        /// <summary>   Name of the container. </summary>
        private string containerName;
        /// <summary>   The query where property. </summary>
        private string queryWhereProperty;
        /// <summary>   The query where value. </summary>
        private object queryWhereValue;
        /// <summary>   The parent. </summary>
        private AssemblyModel parent;
        /// <summary>   The identifier. </summary>
        protected string id;
        /// <summary>   Type of the data. </summary>
        protected BaseType dataType;
        /// <summary>   List of types of the facet handlers. </summary>

        /// <summary>   Gets the variables. </summary>
        ///
        /// <value> The variables. </value>

        public Dictionary<string, object> Variables { get; }

        /// <summary>   Gets a value indicating whether the follow without. </summary>
        ///
        /// <value> True if follow without, false if not. </value>

        public bool FollowWithout => true;

        /// <summary>
        /// Gets or sets a value indicating whether the no user interface or configuration.
        /// </summary>
        ///
        /// <value> True if no user interface or configuration, false if not. </value>

        public bool NoUIOrConfig { get; set; }

        /// <summary>   Gets a value indicating whether the prevent recursion. </summary>
        ///
        /// <value> True if prevent recursion, false if not. </value>

        public bool PreventRecursion { get => true; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <param name="containerType">    The name. </param>
        /// <param name="assembly">         The assembly. </param>
        /// <param name="typesProvider">    The types provider. </param>
        /// <param name="parent">           The parent. </param>

        public AssemblyModelEntityContainer(AssemblyType containerType, AssemblyProvider.Web.Entities.Assembly assembly, ITypesProvider typesProvider, AssemblyModel parent) : base(parent)
        {
            this.assembly = assembly;
            this.containerType = containerType;
            this.containerName = containerType.Name;
            this.parent = parent;
            this.typesProvider = typesProvider;

            id = this.MakeID("Container='" + containerName + "'");

            dataType = new BaseType
            {
                FullyQualifiedName = containerType.Namespace + "." + containerType.Name,
                Name = containerName,
                ID = containerName,
                ParentID = this.ID
            };
        }

        /// <summary>   Gets the attributes. </summary>
        ///
        /// <value> The attributes. </value>

        public IEnumerable<IAttribute> Attributes
        {
            get { return null; }
        }

        /// <summary>   Gets the sets the entity belongs to. </summary>
        ///
        /// <value> The entity sets. </value>

        public List<IEntitySet> EntitySets
        {
            get
            {
                var entitySets = new List<AssemblyModelEntitySet>();

                try
                {
                    foreach (var property in this.containerType.PropertyElements.Where(p => p.DataType.Name.StartsWith("DbSet")))
                    {
                        entitySets.Add(new AssemblyModelEntitySet(property, this.assembly, this.typesProvider, this));
                    }
                }
                catch
                {
                }

                return entitySets.ToList<IEntitySet>();
            }
        }

        /// <summary>   Gets the child elements. </summary>
        ///
        /// <value> The child elements. </value>

        public IEnumerable<IElement> ChildElements
        {
            get
            {
                var elements = this.EntitySets.AsQueryable().Cast<IElement>().Select(e => e);

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
                var nodes = this.EntitySets.AsQueryable().Cast<IBase>();

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
                return containerName;
            }
        }

        /// <summary>   Gets a value indicating whether this  is container. </summary>
        ///
        /// <value> True if this  is container, false if not. </value>

        public bool IsContainer
        {
            get
            {
                return true;
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
                Debugger.Break();

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
            get { return string.Empty; }
        }

        /// <summary>   Gets a value indicating whether this  has documentation. </summary>
        ///
        /// <value> True if this  has documentation, false if not. </value>

        public override bool HasDocumentation
        {
            get { return false; }
        }

        /// <summary>   Gets the documentation summary. </summary>
        ///
        /// <value> The documentation summary. </value>

        public override string DocumentationSummary
        {
            get { return string.Empty; }
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
                return this.GetDebugInfo();
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
                return DefinitionKind.StaticContainer;
            }
        }

        /// <summary>   Gets a value indicating whether this  has children. </summary>
        ///
        /// <value> True if this  has children, false if not. </value>

        public override bool HasChildren
        {
            get
            {
                return this.EntitySets.Any();
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

                    if (args.NoMetadata)
                    {
                        this.NoUIOrConfig = true;
                    }
                    else if (args.Attributes != null)
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
