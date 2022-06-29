// file:	Models\Entities\EntitySet.cs
//
// summary:	Implements the entity set class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using AbstraX.ServerInterfaces;
using System.Diagnostics;
using AbstraX.XPathBuilder;
using System.Runtime.Serialization;
using AbstraX;
using AbstraX.TypeMappings;
using AbstraX.Models.Interfaces;

namespace EntityProvider.Web.Entities
{
    /// <summary>   An entity set. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/21/2020. </remarks>

    [DebuggerDisplay(" { Name } ")]
    public class Entity_Set : EntityBase, IParentBase, IRelationProperty, IEntityObjectWithFacets, IEntitySet
    {
        /// <summary>   The name. </summary>
        protected string name;
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
        /// <summary>   The design comments. </summary>
        protected string designComments;
        /// <summary>   The child ordinal. </summary>
        protected float childOrdinal;
        /// <summary>   Information describing the debug. </summary>
        protected string debugInfo;
        /// <summary>   The identifier. </summary>
        protected string id;

        /// <summary>   Gets this multiplicity. </summary>
        ///
        /// <value> this multiplicity. </value>

        public string ThisMultiplicity { get; }

        /// <summary>   Gets the parent multiplicity. </summary>
        ///
        /// <value> The parent multiplicity. </value>

        public string ParentMultiplicity { get; }

        /// <summary>   Gets the name of this property reference. </summary>
        ///
        /// <value> The name of this property reference. </value>

        public string ThisPropertyRefName { get; }

        /// <summary>   Gets the name of the parent property reference. </summary>
        ///
        /// <value> The name of the parent property reference. </value>

        public string ParentPropertyRefName { get; }

        /// <summary>   Gets this property reference. </summary>
        ///
        /// <value> this property reference. </value>

        public IAttribute ThisPropertyRef => null;

        /// <summary>   Gets the parent property reference. </summary>
        ///
        /// <value> The parent property reference. </value>

        public IAttribute ParentPropertyRef => null;

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/21/2020. </remarks>
        ///
        /// <param name="edmxDocument">     The edmx document. </param>
        /// <param name="namespaceManager"> Manager for namespace. </param>
        /// <param name="name">             The name. </param>
        /// <param name="node">             The node. </param>
        /// <param name="parent">           The parent. </param>

        public Entity_Set(XmlDocument edmxDocument, XmlNamespaceManager namespaceManager, string name, XmlNode node, Entity_Container parent) : base(parent)
        {
            this.EdmxDocument = edmxDocument;
            this.NamespaceManager = namespaceManager;
            this.name = name;
            this.node = node;
            this.parent = parent;

            id = this.MakeID("EntitySet='" + name + "'");

            debugInfo = this.GetDebugInfo();

            this.ParentMultiplicity = "static";
            this.ThisMultiplicity = "*";
            this.ThisPropertyRefName = null;
            this.ParentPropertyRefName = null;
        }

        /// <summary>   Gets the attributes. </summary>
        ///
        /// <value> The attributes. </value>

        public IEnumerable<IAttribute> Attributes
        {
            get { return null; }
        }

        /// <summary>   Gets the entities. </summary>
        ///
        /// <value> The entities. </value>

        public List<IEntityType> Entities
        {
            get
            {

                var entities = new List<EntityType>();

                try
                {
                    if (node != null)
                    {
                        var entityType = node.Attributes["EntityType"].Value;
                        var regex = new Regex(@"(?<namespace>\w+?)\.(?<type>\w+)");
                        var match = regex.Match(entityType);
                        var nameSpace = match.Groups["namespace"].Value;
                        var type = match.Groups["type"].Value;

                        if (queryWhereProperty != null && queryWhereProperty == "Type" && node != null)
                        {
                            var entity = this.EdmxDocument.SelectSingleNode("/edmx:Edmx/edmx:Runtime/edmx:ConceptualModels/e:Schema/e:EntityType[@Name='" + type + "']", this.NamespaceManager);
                            var name = entity.Attributes["Name"].Value;

                            Debug.Assert(entity != null);

                            entities.Add(new EntityType(this.EdmxDocument, this.NamespaceManager, name, entity, this));
                        }
                        else
                        {
                            if (node != null)
                            {
                                foreach (XmlNode childNode in this.EdmxDocument.SelectNodes("/edmx:Edmx/edmx:Runtime/edmx:ConceptualModels/e:Schema/e:EntityType[@Name='" + type + "']", this.NamespaceManager))
                                {
                                    var name = childNode.Attributes["Name"].Value;

                                    entities.Add(new EntityType(this.EdmxDocument, this.NamespaceManager, name, childNode, this));
                                }
                            }
                        }
                    }
                }
                catch
                {
                }

                return entities.ToList<IEntityType>();

            }
        }

        /// <summary>   Gets the child elements. </summary>
        ///
        /// <value> The child elements. </value>

        public IEnumerable<IElement> ChildElements
        {
            get
            {
                var elements = this.Entities.AsQueryable().Cast<IElement>().Select(e => e);

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
                var nodes = this.Entities.AsQueryable().Cast<IBase>();

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

        /// <summary>   Gets URL of the image. </summary>
        ///
        /// <value> The image URL. </value>

        public string ImageURL
        {
            get
            {
                return string.Empty;
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
                return null;
            }
        }

        /// <summary>   Executes the where operation. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/21/2020. </remarks>
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
        /// <remarks>   CloudIDEaaS, 11/21/2020. </remarks>
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
        /// <remarks>   CloudIDEaaS, 11/21/2020. </remarks>
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
        /// <remarks>   CloudIDEaaS, 11/21/2020. </remarks>

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
                return DefinitionKind.ComplexSetProperty;
            }
        }

        /// <summary>   Gets a value indicating whether this  has children. </summary>
        ///
        /// <value> True if this  has children, false if not. </value>

        public override bool HasChildren
        {
            get
            {
                return this.Entities.Any();
            }
        }

        /// <summary>   Gets the facets. </summary>
        ///
        /// <value> The facets. </value>

        public override Facet[] Facets
        {
            get
            {
                var args = new GetAttributesEventArgs(this, ((Entity_Container) this.parent).GetOriginalDataType(), this.Name);
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
