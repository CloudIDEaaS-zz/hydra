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
using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Server;
using AbstraX.Templates;
using AbstraX;
using AbstraX.Contracts;
using AbstraX.AssemblyInterfaces;

namespace EntityProvider.Web.Entities
{
    [DataContract, NodeImage("EntityProvider.Web.Images.EntityProperty.png"), DebuggerDisplay("{ DebugInfo }"), ClientCodeGeneration(typeof(AbstraXClientInterfaceGenerator))]
    public class EntityProperty : EntitiesBase, IAttribute
    {
        private IAbstraXService service;
        protected string name;
        protected XmlNode node;
        protected IParentBase parent;
        protected string typeAttribute;
        private string imageURL;
        protected IProviderEntityService providerEntityService;
        protected bool hasDocumentation;
        protected string documentationSummary;
        protected string documentation;
        protected float childOrdinal;
        protected string debugInfo;
        protected ScalarType dataType;
        protected string designComments;
        protected string id;

        public EntityProperty()
        {
            hasDocumentation = false;
            childOrdinal = 0;
            debugInfo = this.GetDebugInfo();
        }

        public EntityProperty(IAbstraXService service)
        {
            this.service = service;
        }

        public EntityProperty(XmlDocument edmxDocument, XmlNamespaceManager namespaceManager, string name, XmlNode node, EntityType parent)
        {
            this.EdmxDocument = edmxDocument;
            this.NamespaceManager = namespaceManager;
            this.name = name;
            this.node = node;
            this.parent = parent;
            id = this.MakeID("Property='" + this.name + "'");

            var keyNodes = parent.Node.SelectNodes("e:Key/e:PropertyRef[@Name='" + name + "']", namespaceManager);

            typeAttribute = node.Attributes["Type"].Value;

            var type = Type.GetType("System." + typeAttribute);

            if (keyNodes.Count > 0)
            {
                imageURL = "EntityProvider.Web.Images.EntityPropertyKey.png";
                childOrdinal = 0;
            }
            else
            {
                imageURL = "EntityProvider.Web.Images.EntityProperty.png";
                childOrdinal = 1;
            }

            providerEntityService = ((IBase)parent).ProviderEntityService;

            dataType = new ScalarType(type, this);
            debugInfo = this.GetDebugInfo();
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

        [DataMember, Key]
        public string ID
        {
            get
            {
                return id;
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
        public string ImageURL
        {
            get
            {
                return imageURL;
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

        public IEnumerable<IElement> ChildElements
        {
            get { return null; }
        }

        public IQueryable ExecuteWhere(string property, object value)
        {
            throw new NotImplementedException();
        }

        public IQueryable ExecuteWhere(System.Linq.Expressions.Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable ExecuteWhere(AbstraX.XPathBuilder.XPathAxisElement element)
        {
            throw new NotImplementedException();
        }

        public void ClearPredicates()
        {
            throw new NotImplementedException();
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
        public string DesignComments
        {
            get
            {
                return designComments;
            }
        }

        [DataMember]
        public string Documentation
        {
            get
            {
                return documentation;
            }
        }

        [DataMember]
        public bool HasDocumentation
        {
            get
            {
                return hasDocumentation;
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
                return documentationSummary;
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
                return debugInfo;
            }
        }

        [DataMember]
        public string ParentID
        {
            get 
            {
                return parent.ID;
            }
        }

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
                return false;
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
    }
}
