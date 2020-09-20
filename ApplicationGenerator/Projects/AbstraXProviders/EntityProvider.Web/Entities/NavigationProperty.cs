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
using System.ServiceModel.DomainServices.Server;
using System.ComponentModel.DataAnnotations;
using AbstraX.XPathBuilder;
using AbstraX.Templates;
using AbstraX;
using AbstraX.TypeMappings;
using AbstraX.Contracts;
using Utils;
using System.Collections;
using EntityProvider.Web.Entities.DatabaseEntities;

namespace EntityProvider.Web.Entities
{
    [KnownType(typeof(DbNavigationProperty))]
    [DataContract, NodeImage("EntityProvider.Web.Images.NavigationProperty.png"), DebuggerDisplay("{ DebugInfo }"), ClientCodeGeneration(typeof(AbstraXClientInterfaceGenerator))]
    public class NavigationProperty : EntitiesBase, IEntityParent, IElement
    {
        private IAbstraXService service;
        private XmlNode node;
        protected EntityType parent;
        private bool isCollectionType;
        private XmlNode childNode;
        private XmlNode foreignRoleNode;
        protected string name;
        private string queryWhereProperty;
        private object queryWhereValue;
        protected float childOrdinal;
        protected ScalarType dataType;
        private XmlNode associationNode;
        protected IProviderEntityService providerEntityService;
        protected bool hasDocumentation;
        protected string documentationSummary;
        protected string documentation;   
        protected string debugInfo;      
        protected string designComments;
        protected string id;

        public NavigationProperty()
        {
            hasDocumentation = false;
            childOrdinal = 0;
            debugInfo = this.GetDebugInfo();
        }

        public NavigationProperty(IAbstraXService service)
        {
            this.service = service;            
        }

        public NavigationProperty(XmlDocument edmxDocument, XmlNamespaceManager namespaceManager, string name, XmlNode node, EntityType parentEntity)
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
            var associationSetNode = this.EdmxDocument.SelectSingleNode("/edmx:Edmx/edmx:Runtime/edmx:ConceptualModels/e:Schema[@Namespace='" + nameSpace + "']/e:EntityContainer/e:AssociationSet[@Name='" + relationship + "']", this.NamespaceManager);
            var association = associationSetNode.Attributes["Association"].Value;

            this.name = name;

            id = this.MakeID("Property='" + this.name + "'");

            regex = new Regex(@"(?<namespace>\w+?)\.(?<key>\w+)");
            match = regex.Match(association);
            nameSpace = match.Groups["namespace"].Value;

            var key = match.Groups["key"].Value;
            
            associationNode = this.EdmxDocument.SelectSingleNode("/edmx:Edmx/edmx:Runtime/edmx:ConceptualModels/e:Schema[@Namespace='" + nameSpace + "']/e:Association[@Name='" + key + "']", this.NamespaceManager);
            
            this.foreignRoleNode = associationNode.SelectSingleNode("e:End[@Role!='" + parentEntity.Name + "']", this.NamespaceManager);
            var foreignEntityRole = foreignRoleNode.Attributes["Role"].Value;
            
            this.childNode = this.EdmxDocument.SelectSingleNode("/edmx:Edmx/edmx:Runtime/edmx:ConceptualModels/e:Schema[@Namespace='" + nameSpace + "']/e:EntityType[@Name='" + foreignEntityRole + "']", this.NamespaceManager);

            if (this.childNode == null)
            {
                regex = new Regex(@"(?<entity>\w*?)\d+");

                while (this.childNode == null && regex.IsMatch(foreignEntityRole))
                {
                    foreignEntityRole = foreignEntityRole.RemoveEnd(1);
                    this.childNode = this.EdmxDocument.SelectSingleNode("/edmx:Edmx/edmx:Runtime/edmx:ConceptualModels/e:Schema[@Namespace='" + nameSpace + "']/e:EntityType[@Name='" + foreignEntityRole + "']", this.NamespaceManager);
                }
            }

            childOrdinal = 2;

            providerEntityService = ((IBase)parentEntity).ProviderEntityService;

            var childEntities = this.ChildElements;

            if (isCollectionType)
            {
                dataType = new ScalarType(typeof(IEnumerable), this);
            }
            else
            {
                dataType = new ScalarType(childEntities.Single().GetType(), this);
            }
        }


        [AbstraXExtensionAttribute("IEntityProviderExtension", "EntityProviderExtension")]
        public IAbstraXExtension LoadExtension()
        {
            return null;
        }

        [Exclude]
        IProviderEntityService IBase.ProviderEntityService
        {
            get
            {
                return providerEntityService;
            }
        }

        [Association("NavigationProperty_ChildEntities", "ID", "ParentID")]
        public List<EntityType> ChildEntities
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
                else
                {
                    if (((DbNavigationProperty)this).RelatedID != null)
                    {
                        ((IBase)this).ProviderEntityService.DatabaseProxy.GetAllElements().Where(p=>p.DatabaseID ==(((DbNavigationProperty)this).RelatedID)).ToList<IDbElement>().ForEach(e =>
                        {
                            childEntities.Add(new DbEntityType(this, e, service, "null"));
                        });
                    }
                }

                return childEntities;
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

        [DataMember]
        public bool IsContainer
        {
            get
            {
                return true;
            }
        }

        [DataMember, Include, Association("Parent_BaseType", "ID", "ParentID")]
        public ScalarType DataType
        {
            get
            {
                return dataType;
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
            if (value is XPathAxisElement && (property == "ID" || property == "ParentID"))
            {
                var predicate = ((XPathAxisElement)value).Predicates.First();

                queryWhereProperty = predicate.Left;
                queryWhereValue = predicate.Right;
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

        public IQueryable ExecuteWhere(AbstraX.XPathBuilder.XPathAxisElement element)
        {
            var predicate = element.Predicates.First();

            queryWhereProperty = predicate.Left;
            queryWhereValue = predicate.Right;

            return this.ChildElements.AsQueryable();
        }

        public void ClearPredicates()
        {
            queryWhereProperty = null;
            queryWhereValue = null;
        }

        public IRoot Root
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


        [DataMember]
        public string Name
        {
            get
            {
                return name;
            }
        }

        [DataMember]
        public string DesignComments
        {
            get
            {
                return string.Empty;
            }
        }

        [DataMember]
        public string Documentation
        {
            get
            {
                return string.Empty;
            }
        }

        [DataMember]
        public bool HasDocumentation
        {
            get
            {
                return false;
            }
            set
            {

            }
        }

        [DataMember]
        public string DocumentationSummary
        {
            get
            {
                return string.Empty;
            }
            set
            {

            }
        }

        [DataMember]
        public float ChildOrdinal
        {
            get
            {
                return childOrdinal;
            }
        }

        [DataMember]
        public string DebugInfo
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
                    return debugInfo;
            }
        }

        [DataMember]
        public IBase Parent
        {
            get
            {
                return parent;
            }
        }

        [DataMember]
        public string FolderKeyPair { get; set; }

        [DataMember]
        public DefinitionKind Kind
        {
            get
            {
                return DefinitionKind.Property;
            }
        }

        [DataMember]
        public bool HasChildren
        {
            get
            {
                return this.ChildEntities.Count > 0;
            }
        }

        [DataMember, Key]
        public string ID
        {
            get
            {
                return id;
            }
        }

        [DataMember]
        public string ParentID
        {
            get
            {
                return this.Parent.ID;
            }
        }

        [DataMember]
        public string ImageURL
        {
            get
            {
                return string.Empty;
            }
        }

        [DataMember, Include, Association("Parent_Facet", "ID", "ParentID")]
        public Facet[] Facets
        {
            get
            {
                return null;
            }
        }

        [Exclude]
        public ContainerType AllowableContainerTypes
        {
            get { throw new NotImplementedException(); }
        }

        [Exclude]
        public ConstructType AllowableConstructTypes
        {
            get { throw new NotImplementedException(); }
        }

        [Exclude]
        public ContainerType DefaultContainerType
        {
            get { throw new NotImplementedException(); }
        }

        [Exclude]
        public ConstructType DefaultConstructType
        {
            get { throw new NotImplementedException(); }
        }

        [DataMember]
        public Modifiers Modifiers
        {
            get
            {
                return Modifiers.Unknown;
            }
        }

        [DataMember]
        public string DefaultValue
        {
            get
            {
                return null;
            }
        }

        #region IElement Members


        BaseType IElement.DataType
        {
            get { return dataType; }
        }

        #endregion
    }
}
