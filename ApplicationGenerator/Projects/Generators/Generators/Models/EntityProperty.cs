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

namespace EntityProvider.Web.Entities
{
    public class EntityProperty : EntitiesBase, IAttribute
    {
        private IAbstraXService service;
        protected string name;
        protected XmlNode node;
        protected IParentBase parent;
        protected string typeAttribute;
        private string imageURL;
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

            dataType = new ScalarType(type, this);
            debugInfo = this.GetDebugInfo();
        }

        public string ID
        {
            get
            {
                return id;
            }
        }

        public string Name
        {
            get 
            {
                return name;
            }
        }

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

        public string DesignComments
        {
            get
            {
                return designComments;
            }
        }

        public string Documentation
        {
            get
            {
                return documentation;
            }
        }

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

        public float ChildOrdinal
        {
            get
            {
                return childOrdinal;
            }
        }

        public string DebugInfo
        {
            get
            {
                return debugInfo;
            }
        }

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

        public string FolderKeyPair { get; set; }

        public DefinitionKind Kind
        {
            get
            {
                return DefinitionKind.Property;
            }
        }

        public bool HasChildren
        {
            get
            {
                return false;
            }
        }

        public Facet[] Facets
        {
            get
            {
                return null;
            }
        }

        public Modifiers Modifiers
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
