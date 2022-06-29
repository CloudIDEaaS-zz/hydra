// file:	Models\Entities\NavigationProperty.cs
//
// summary:	Implements the navigation property class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;
using AbstraX.ServerInterfaces;
using System.Runtime.Serialization;
using AbstraX.XPathBuilder;
using AbstraX;
using AbstraX.TypeMappings;
using Utils;
using System.Collections;
using AbstraX.Models.Interfaces;

namespace EntityProvider.Web.Entities
{
    /// <summary>   A navigation property. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/2/2020. </remarks>

    [DebuggerDisplay(" { Name } ")]
    public class NavigationProperty : EntityBase, IParentBase, IRelationProperty, IEntityObjectWithFacets, INavigationProperty, IEntityObjectWithDataType
    {
        /// <summary>   The parent. </summary>
        protected EntityType parent;
        /// <summary>   True if is collection type, false if not. </summary>
        private bool isCollectionType;
        /// <summary>   The child node. </summary>
        private XmlNode childNode;
        /// <summary>   The foreign role node. </summary>
        private XmlNode foreignRoleNode;
        /// <summary>   The parent role node. </summary>
        private XmlNode parentRoleNode;
        /// <summary>   The name. </summary>
        protected string name;
        /// <summary>   The query where property. </summary>
        private string queryWhereProperty;
        /// <summary>   The query where value. </summary>
        private object queryWhereValue;
        /// <summary>   The child ordinal. </summary>
        protected float childOrdinal;
        /// <summary>   Type of the data. </summary>
        protected ScalarType dataType;
        /// <summary>   Element describing the association node. </summary>
        private XmlElement associationNode;
        /// <summary>   True if has documentation, false if not. </summary>
        protected bool hasDocumentation;
        /// <summary>   The documentation summary. </summary>
        protected string documentationSummary;
        /// <summary>   The documentation. </summary>
        protected string documentation;   
        /// <summary>   Information describing the debug. </summary>
        protected string debugInfo;      
        /// <summary>   The design comments. </summary>
        protected string designComments;
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

        /// <summary>   Gets the namespace. </summary>
        ///
        /// <value> The namespace. </value>

        public string Namespace { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/2/2020. </remarks>
        ///
        /// <param name="edmxDocument">     The edmx document. </param>
        /// <param name="namespaceManager"> Manager for namespace. </param>
        /// <param name="name">             The name. </param>
        /// <param name="node">             The node. </param>
        /// <param name="parentEntity">     The parent entity. </param>

        public NavigationProperty(XmlDocument edmxDocument, XmlNamespaceManager namespaceManager, string name, XmlNode node, EntityType parentEntity) : base(parentEntity)
        {
            this.EdmxDocument = edmxDocument;
            this.NamespaceManager = namespaceManager;
            this.node = node;
            this.parent = parentEntity;
            var regex = new Regex(@"(?<namespace>\w+?)\.(?<key>\w+)");
            var relationshipAttribute = node.Attributes["Relationship"];
            var match = regex.Match(relationshipAttribute.Value);
            var nameSpace = match.Groups["namespace"].Value;
            var relationship = match.Groups["key"].Value;
            var associationSetNode = this.EdmxDocument.SelectSingleNode("/edmx:Edmx/edmx:Runtime/edmx:ConceptualModels/e:Schema[@Namespace='" + nameSpace + "' or @Alias='" + nameSpace + "']/e:EntityContainer/e:AssociationSet[@Name='" + relationship + "']", this.NamespaceManager);
            var association = associationSetNode.Attributes["Association"].Value;
            string key;
            string foreignEntityRole;
            string parentEntityRole;
            IEnumerable<IElement> childEntities;
            XmlElement referencialConstraint;
            XmlElement parentPropertyRefNode;
            XmlElement thisPropertyRefNode;

            this.name = name;

            id = this.MakeID("Property='" + this.name + "'");

            regex = new Regex(@"(?<namespace>\w+?)\.(?<key>\w+)");
            match = regex.Match(association);
            nameSpace = match.Groups["namespace"].Value;
            key = match.Groups["key"].Value;
            
            associationNode = (XmlElement) this.EdmxDocument.SelectSingleNode("/edmx:Edmx/edmx:Runtime/edmx:ConceptualModels/e:Schema[@Namespace='" + nameSpace + "' or @Alias='" + nameSpace + "']/e:Association[@Name='" + key + "']", this.NamespaceManager);
            referencialConstraint = associationNode["ReferentialConstraint"];

            this.parentRoleNode = associationNode.SelectSingleNode("e:End[@Role='" + parentEntity.Name + "']", this.NamespaceManager);
            this.foreignRoleNode = associationNode.SelectSingleNode("e:End[@Role!='" + parentEntity.Name + "']", this.NamespaceManager);

            foreignEntityRole = foreignRoleNode.Attributes["Role"].Value;
            parentEntityRole = parentRoleNode.Attributes["Role"].Value;

            this.childNode = this.EdmxDocument.SelectSingleNode("/edmx:Edmx/edmx:Runtime/edmx:ConceptualModels/e:Schema[@Namespace='" + nameSpace + "' or @Alias='" + nameSpace + "']/e:EntityType[@Name='" + foreignEntityRole + "']", this.NamespaceManager);

            if (this.childNode == null)
            {
                regex = new Regex(@"(?<entity>\w*?)\d+");

                while (this.childNode == null && regex.IsMatch(foreignEntityRole))
                {
                    foreignEntityRole = foreignEntityRole.RemoveEnd(1);
                    this.childNode = this.EdmxDocument.SelectSingleNode("/edmx:Edmx/edmx:Runtime/edmx:ConceptualModels/e:Schema[@Namespace='" + nameSpace + "' or @Alias='" + nameSpace + "']/e:EntityType[@Name='" + foreignEntityRole + "']", this.NamespaceManager);
                }
            }

            childOrdinal = 2;
            childEntities = this.ChildElements;

            parentPropertyRefNode = (XmlElement) referencialConstraint.SelectSingleNode("e:Dependent[@Role='" + parentEntityRole + "']/e:PropertyRef | e:Principal[@Role='" + parentEntityRole + "']/e:PropertyRef", this.NamespaceManager);
            thisPropertyRefNode = (XmlElement)referencialConstraint.SelectSingleNode("e:Dependent[@Role='" + foreignEntityRole + "']/e:PropertyRef | e:Principal[@Role='" + foreignEntityRole + "']/e:PropertyRef", this.NamespaceManager);

            this.ParentMultiplicity = parentRoleNode.Attributes["Multiplicity"].Value;
            this.ThisMultiplicity = foreignRoleNode.Attributes["Multiplicity"].Value;
            this.ThisPropertyRefName = thisPropertyRefNode.Attributes["Name"].Value;
            this.ParentPropertyRefName = parentPropertyRefNode.Attributes["Name"].Value;

            this.Namespace = nameSpace;

            if (isCollectionType)
            {
                dataType = new ScalarType(typeof(IEnumerable), this);
                dataType.IsCollectionType = true;
            }
            else
            {
                dataType = new ScalarType(childEntities.Single().GetType(), this);
            }
        }

        /// <summary>   Gets this property reference. </summary>
        ///
        /// <value> this property reference. </value>

        public IAttribute ThisPropertyRef
        {
            get
            {
                return this.ChildElements.Single().Attributes.Single(a => a.Name == this.ThisPropertyRefName);
            }
        }

        /// <summary>   Gets the parent property reference. </summary>
        ///
        /// <value> The parent property reference. </value>

        public IAttribute ParentPropertyRef
        {
            get
            {
                return parent.Attributes.Single(a => a.Name == this.ParentPropertyRefName);
            }
        }

        /// <summary>   Gets the facets. </summary>
        ///
        /// <value> The facets. </value>

        public override Facet[] Facets
        {
            get
            {
                var args = new GetAttributesEventArgs(this, this.parent.GetOriginalDataType(), this.Name);
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

        /// <summary>   Gets the child entities. </summary>
        ///
        /// <value> The child entities. </value>

        public List<IEntityType> ChildEntities
        {
            get 
            {
                List<EntityType> childEntities = new List<EntityType>();

                if (childNode != null)
                {
                    var childName = childNode.Attributes["Name"].Value;
                    var multiplicity = foreignRoleNode.Attributes["Multiplicity"].Value;

                    childEntities.Add(new EntityType(this.EdmxDocument, this.NamespaceManager, childName, childNode, this));

                    if (multiplicity == "*")
                    {
                        isCollectionType = true;
                    }
                    else
                    {
                        isCollectionType = false;
                    }
                }

                return childEntities.ToList<IEntityType>();
            }
        }

        /// <summary>   Gets the child elements. </summary>
        ///
        /// <value> The child elements. </value>

        public IEnumerable<IElement> ChildElements
        {
            get 
            {
                var childElements = new List<IElement>();

                if (this.ChildEntities != null)
                {
                    foreach (var entity in this.ChildEntities)
                    {
                        childElements.Add(entity);
                    }
                }

                return childElements;
            }
        }

        /// <summary>   Gets the child nodes. </summary>
        ///
        /// <value> The child nodes. </value>

        public IEnumerable<IBase> ChildNodes
        {
            get
            {
                var nodes = this.ChildEntities.AsQueryable().Cast<IBase>();

                return nodes;
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

        public ScalarType DataType
        {
            get
            {
                return dataType;
            }
        }

        /// <summary>   Gets the type of the data. </summary>
        ///
        /// <value> The type of the data. </value>

        BaseType IEntityObjectWithDataType.DataType
        {
            get
            {
                return this.dataType;
            }
        }

        /// <summary>   Gets the type of the data. </summary>
        ///
        /// <value> The type of the data. </value>

        BaseType IElement.DataType
        {
            get
            {
                return this.dataType;
            }
        }

        /// <summary>   Gets the attributes. </summary>
        ///
        /// <value> The attributes. </value>

        public IEnumerable<IAttribute> Attributes
        {
            get 
            {
                return new List<IAttribute>(); 
            }
        }

        /// <summary>   Gets the operations. </summary>
        ///
        /// <value> The operations. </value>

        public IEnumerable<IOperation> Operations
        {
            get { return null; }
        }

        /// <summary>   Executes the where operation. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/2/2020. </remarks>
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
        /// <remarks>   CloudIDEaaS, 11/2/2020. </remarks>
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
        /// <remarks>   CloudIDEaaS, 11/2/2020. </remarks>
        ///
        /// <param name="element">  The element. </param>
        ///
        /// <returns>   An IQueryable. </returns>

        public IQueryable ExecuteWhere(AbstraX.XPathBuilder.XPathElement element)
        {
            var predicate = element.Predicates.First();

            queryWhereProperty = predicate.Left.ToString();
            queryWhereValue = predicate.Right.ToString();

            return this.ChildElements.AsQueryable();
        }

        /// <summary>   Clears the predicates. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/2/2020. </remarks>

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
                return string.Empty;
            }
        }

        /// <summary>   Gets a value indicating whether this  has documentation. </summary>
        ///
        /// <value> True if this  has documentation, false if not. </value>

        public override bool HasDocumentation
        {
            get
            {
                return false;
            }
        }

        /// <summary>   Gets the documentation summary. </summary>
        ///
        /// <value> The documentation summary. </value>

        public override string DocumentationSummary
        {
            get
            {
                return string.Empty;
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
                if (associationNode != null)
                {
                    var additionalInfo = new StringBuilder();

                    var principalNode = associationNode.SelectSingleNode("e:ReferentialConstraint/e:Principal", this.NamespaceManager);
                    var principalRole = principalNode.Attributes["Role"].Value;
                    var principalProperty = principalNode.SelectSingleNode("e:PropertyRef/@Name", this.NamespaceManager).Value;
                    var dependentNode = associationNode.SelectSingleNode("e:ReferentialConstraint/e:Dependent", this.NamespaceManager);
                    var dependentRole = dependentNode.Attributes["Role"].Value;
                    var dependentProperty = dependentNode.SelectSingleNode("e:PropertyRef/@Name", this.NamespaceManager).Value;

                    additionalInfo.AppendFormat("\r\nPrincipal Role: {0}", principalRole);
                    additionalInfo.AppendFormat("\r\nPrincipal Property: {0}", principalProperty);
                    additionalInfo.AppendFormat("\r\n\r\nDependent Role: {0}", dependentRole);
                    additionalInfo.AppendFormat("\r\nDependent Property: {0}", dependentProperty);

                    return this.GetDebugInfo(additionalInfo);
                }
                else
                {
                    return debugInfo;
                }
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
                return DefinitionKind.ComplexProperty;
            }
        }

        /// <summary>   Gets a value indicating whether this  has children. </summary>
        ///
        /// <value> True if this  has children, false if not. </value>

        public override bool HasChildren
        {
            get
            {
                return this.ChildEntities.Count > 0;
            }
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
                return this.Parent.ID;
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
    }
}
