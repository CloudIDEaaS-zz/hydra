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
    [DebuggerDisplay(" { Name } ")]
    public class NavigationProperty : EntityBase, IParentBase, IRelationProperty, IEntityWithFacets, INavigationProperty, IEntityWithDataType
    {
        protected EntityType parent;
        private bool isCollectionType;
        private XmlNode childNode;
        private XmlNode foreignRoleNode;
        private XmlNode parentRoleNode;
        protected string name;
        private string queryWhereProperty;
        private object queryWhereValue;
        protected float childOrdinal;
        protected ScalarType dataType;
        private XmlElement associationNode;
        protected bool hasDocumentation;
        protected string documentationSummary;
        protected string documentation;   
        protected string debugInfo;      
        protected string designComments;
        protected string id;
        public string ThisMultiplicity { get; }
        public string ParentMultiplicity { get; }
        public string ThisPropertyRefName { get; }
        public string ParentPropertyRefName { get; }
        public string Namespace { get; }

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

        public IAttribute ThisPropertyRef
        {
            get
            {
                return this.ChildElements.Single().Attributes.Single(a => a.Name == this.ThisPropertyRefName);
            }
        }

        public IAttribute ParentPropertyRef
        {
            get
            {
                return parent.Attributes.Single(a => a.Name == this.ParentPropertyRefName);
            }
        }

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


        public IEnumerable<IBase> ChildNodes
        {
            get
            {
                var nodes = this.ChildEntities.AsQueryable().Cast<IBase>();

                return nodes;
            }
        }

        public bool IsContainer
        {
            get
            {
                return true;
            }
        }

        public ScalarType DataType
        {
            get
            {
                return dataType;
            }
        }
        BaseType IEntityWithDataType.DataType
        {
            get
            {
                return this.dataType;
            }
        }

        BaseType IElement.DataType
        {
            get
            {
                return this.dataType;
            }
        }

        public IEnumerable<IAttribute> Attributes
        {
            get 
            {
                return new List<IAttribute>(); 
            }
        }

        public IEnumerable<IOperation> Operations
        {
            get { return null; }
        }

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

        public IQueryable ExecuteWhere(System.Linq.Expressions.Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable ExecuteWhere(AbstraX.XPathBuilder.XPathElement element)
        {
            var predicate = element.Predicates.First();

            queryWhereProperty = predicate.Left.ToString();
            queryWhereValue = predicate.Right.ToString();

            return this.ChildElements.AsQueryable();
        }

        public void ClearPredicates()
        {
            queryWhereProperty = null;
            queryWhereValue = null;
        }

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

        public override string Name
        {
            get
            {
                return name;
            }
        }

        public override string DesignComments
        {
            get
            {
                return string.Format("Type: {0}, Kind {1}, ID:'{2}'", this.GetType().Name, this.Kind, this.ID);
            }
        }

        public override string Documentation
        {
            get
            {
                return string.Empty;
            }
        }

        public override bool HasDocumentation
        {
            get
            {
                return false;
            }
        }

        public override string DocumentationSummary
        {
            get
            {
                return string.Empty;
            }
        }

        public override float ChildOrdinal
        {
            get
            {
                return childOrdinal;
            }
        }

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

        public override IBase Parent
        {
            get
            {
                return parent;
            }
        }

        public override string FolderKeyPair { get; set; }

        public override DefinitionKind Kind
        {
            get
            {
                return DefinitionKind.ComplexProperty;
            }
        }

        public override bool HasChildren
        {
            get
            {
                return this.ChildEntities.Count > 0;
            }
        }

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

        public override string ParentID
        {
            get
            {
                return this.Parent.ID;
            }
        }

        public ContainerType AllowableContainerTypes
        {
            get { throw new NotImplementedException(); }
        }

        public ConstructType AllowableConstructTypes
        {
            get { throw new NotImplementedException(); }
        }

        public ContainerType DefaultContainerType
        {
            get { throw new NotImplementedException(); }
        }

        public ConstructType DefaultConstructType
        {
            get { throw new NotImplementedException(); }
        }

        public override Modifiers Modifiers
        {
            get
            {
                return Modifiers.Unknown;
            }
        }

        public string DefaultValue
        {
            get
            {
                return null;
            }
        }
    }
}
