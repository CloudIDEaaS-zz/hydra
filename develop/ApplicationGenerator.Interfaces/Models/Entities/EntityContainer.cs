// file:	Models\Entities\EntityContainer.cs
//
// summary:	Implements the entity container class

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

namespace EntityProvider.Web.Entities
{
    /// <summary>   Container for entities. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/4/2020. </remarks>

    [DebuggerDisplay(" { Name } ")]
    public class Entity_Container : EntityBase, IElement, IEntityObjectWithFacets, IEntityObjectWithDataType, IEntityContainer
    {
        /// <summary>   Name of the container. </summary>
        private string containerName;
        /// <summary>   The query where property. </summary>
        private string queryWhereProperty;
        /// <summary>   The query where value. </summary>
        private object queryWhereValue;
        /// <summary>   The parent. </summary>
        private Model parent;
        /// <summary>   The identifier. </summary>
        protected string id;
        /// <summary>   Type of the data. </summary>
        protected BaseType dataType;

        /// <summary>   Gets a value indicating whether the prevent recursion. </summary>
        ///
        /// <value> True if prevent recursion, false if not. </value>

        public bool PreventRecursion { get => false; }

        /// <summary>   Gets the namespace. </summary>
        ///
        /// <value> The namespace. </value>

        public string Namespace { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/4/2020. </remarks>
        ///
        /// <param name="edmxDocument">     The edmx document. </param>
        /// <param name="namespaceManager"> Manager for namespace. </param>
        /// <param name="containerName">    Name of the container. </param>
        /// <param name="node">             The node. </param>
        /// <param name="parent">           The parent. </param>

        public Entity_Container(XmlDocument edmxDocument, XmlNamespaceManager namespaceManager, string containerName, XmlNode node, Model parent) : base(parent)
        {
            string _namespace = null;
            var ancestor = (IBase)parent;

            this.EdmxDocument = edmxDocument;
            this.NamespaceManager = namespaceManager;
            this.containerName = containerName;
            this.node = node;
            this.parent = parent;

            id = this.MakeID("Container='" + containerName + "'");

            while (ancestor != null)
            {
                if (ancestor is Model)
                {
                    _namespace = ((Model)ancestor).Namespace;

                    break;
                }

                ancestor = ancestor.Parent;
            }

            this.Namespace = _namespace;

            dataType = new BaseType
            {
                FullyQualifiedName = _namespace + "." + containerName,
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
                var entitySets = new List<Entity_Set>();
                var args = new GetAddInEntitiesEventArgs(this, DefinitionKind.ComplexSetProperty, this.GetOriginalDataType(), new EntityFactory<string, XmlNode>((name, node) =>
                {
                    return new Entity_Set(this.EdmxDocument, this.NamespaceManager, name, node, this);

                }), () => entitySets.Cast<IBase>());

                try
                {
                    if (queryWhereProperty != null && queryWhereProperty == "EntitySet")
                    {
                        var entitySet = node.SelectSingleNode("e:EntitySet[@Name='" + (string)queryWhereValue + "']", this.NamespaceManager);

                        if (entitySet != null)
                        {
                            var name = entitySet.Attributes["Name"].Value;
                            Debug.Assert(entitySet != null);
                            entitySets.Add(new Entity_Set(this.EdmxDocument, this.NamespaceManager, name, entitySet, this));
                        }
                    }
                    else
                    {
                        foreach (XmlNode childNode in node.SelectNodes("e:EntitySet", this.NamespaceManager))
                        {
                            var name = childNode.Attributes["Name"].Value;

                            entitySets.Add(new Entity_Set(this.EdmxDocument, this.NamespaceManager, name, childNode, this));
                        }
                    }

                    foreach (var handler in this.getAddInsEventHandlers)
                    {
                        handler(this, args);

                        foreach (var addInEntitySet in args.GetEntities<Entity_Set>())
                        {
                            entitySets.Add(addInEntitySet);
                        }
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
        /// <remarks>   CloudIDEaaS, 11/4/2020. </remarks>
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
        /// <remarks>   CloudIDEaaS, 11/4/2020. </remarks>
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
        /// <remarks>   CloudIDEaaS, 11/4/2020. </remarks>
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
        /// <remarks>   CloudIDEaaS, 11/4/2020. </remarks>

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
                var args = new GetAttributesEventArgs(this, this.GetOriginalDataType());
                var facets = new List<Facet>();

                foreach (var handler in this.getAttributesEventHandlers)
                {
                    handler(this, args);

                    foreach (var attribute in args.Attributes)
                    {
                        var facet = new Facet(attribute);

                        facets.Add(facet);
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
