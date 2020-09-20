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

namespace EntityProvider.Web.Entities
{
    public class EntityType : EntitiesBase, IElement
    {
        protected IAbstraXService service;
        protected string name;
        private XmlNode node;
        protected string queryWhereProperty;
        protected object queryWhereValue;
        protected IEntityParent parent;
        protected bool hasDocumentation;
        protected string documentationSummary;
        protected string documentation;
        protected float childOrdinal;
        protected string debugInfo;
        protected BaseType dataType;
        protected string designComments;
        protected string id;
        protected string tempID;
        
        public EntityType()
        {
        }

        public EntityType(IAbstraXService service)
        {
            this.service = service;
            //providerEntityService = (IProviderEntityService)((IAbstraXProviderService)service).DomainServiceHostApplication;
        }
                
        public EntityType(XmlDocument edmxDocument, XmlNamespaceManager namespaceManager, string name, XmlNode node, IEntityParent parent)
        {
            this.EdmxDocument = edmxDocument;
            this.NamespaceManager = namespaceManager;
            this.name = name;
            this.node = node;
            this.parent = parent;
            id = this.MakeID("Entity='" + name + "'");

            string _namespace = null;
            var ancestor = (IBase) parent;

            while (ancestor != null)
            {
                if (ancestor is Model)
                {
                    break;
                }

                ancestor = ancestor.Parent;
            }

            dataType = new BaseType
            {
                FullyQualifiedName = _namespace + "." + name,
                Name = name,
                ID = name,
                ParentID = this.ID
            };

            debugInfo = this.GetDebugInfo();
            
        }

        public XmlNode Node
        {
            get 
            { 
                return node; 
            }
        }

        public List<EntityProperty> Properties
        {
            get
            {
                var properties = new List<EntityProperty>();

                if (queryWhereProperty != null && queryWhereProperty == "Property")
                {
                    if (node != null)
                    {
                        var attributeNode = node.SelectNodes("e:Property", this.NamespaceManager).Cast<XmlNode>().Where(n => n.Attributes.Cast<XmlAttribute>().Any(a => a.Name == "Name" && a.Value  == (string)queryWhereValue)).SingleOrDefault();

                        if (attributeNode != null)
                        {
                            var name = attributeNode.Attributes["Name"].Value;

                            properties.Add(new EntityProperty(this.EdmxDocument, this.NamespaceManager, name, attributeNode, this));
                        }
                    }
                }
                else
                {
                    if (node != null)
                    {
                        foreach (XmlNode attributeNode in node.SelectNodes("e:Property", this.NamespaceManager))
                        {
                            var name = attributeNode.Attributes["Name"].Value;

                            properties.Add(new EntityProperty(this.EdmxDocument, this.NamespaceManager, name, attributeNode, this));
                        }
                    }
                }

                return properties;
            }
        }

        public IEnumerable<IAttribute> Attributes
        {
            get 
            {
                var attributes = this.Properties.AsQueryable().Cast<IAttribute>().Select(e => e);

                return attributes;
            }
        }

        public List<NavigationProperty> NavigationProperties
        {
            get
            {
                var properties = new List<NavigationProperty>();

                if (queryWhereProperty != null && queryWhereProperty == "Property")
                {
                    if (node != null)
                    {
                        var property = node.SelectNodes("e:NavigationProperty[@Name='" + (string)queryWhereValue + "']", this.NamespaceManager).Cast<XmlNode>().SingleOrDefault();

                        if (property != null)
                        {
                            var name = property.Attributes["Name"].Value;

                            properties.Add(new NavigationProperty(this.EdmxDocument, this.NamespaceManager, name, property, this));
                        }
                    }
                }
                else
                {
                    if (node != null)
                    {
                        foreach (XmlNode childNode in node.SelectNodes("e:NavigationProperty", this.NamespaceManager))
                        {
                            var name = childNode.Attributes["Name"].Value;
                            properties.Add(new NavigationProperty(this.EdmxDocument, this.NamespaceManager, name, childNode, this));
                        }
                    }
                }
                
                return properties;
            }
        }

        public IEnumerable<IElement> ChildElements
        {
            get 
            {
                var elements = this.NavigationProperties.AsQueryable().Cast<IElement>().Select(e => e);
                return elements;
            }
        }

        public IEnumerable<IOperation> Operations
        {
            get { return null; }
        }

        public string ID
        {
            get
            {
                return id;
            }
        }
   
        public string ParentID
        {
            get
            {
                return parent.ID;
            }           

        }

        public string Name
        {
            get 
            {
                return name;
            }
        }

        public bool IsContainer
        {
            get
            {
                return false;
            }
        }

        public BaseType DataType
        {
            get
            {
                return dataType;
            }
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
                System.Diagnostics.Debugger.Break();

                queryWhereProperty = property;
                queryWhereValue = value;
            }

            return this.ChildElements.AsQueryable();
        }

        public IQueryable ExecuteWhere(System.Linq.Expressions.Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable ExecuteWhere(XPathAxisElement element)
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

        public string DesignComments
        {
            get 
            {
                return designComments;
            }
        }

        public string Documentation
        {
            get { return documentation; }
        }

        public bool HasDocumentation
        {
            get { return hasDocumentation; }
            set { }
        }

        public string DocumentationSummary
        {
            get { return documentationSummary; }
            set { }
        }

        public float ChildOrdinal
        {
            get
            {
                return 0;
            }
        }

        public string DebugInfo
        {
            get
            {
                return debugInfo;
            }
        }

        public IBase Parent
        {
            get
            {
                return this.parent;
            }
        }

        public string FolderKeyPair { get; set; }

        public DefinitionKind Kind
        {
            get
            {
                return DefinitionKind.NotApplicable;
            }
        }

        public bool HasChildren
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

        public Facet[] Facets
        {
            get
            {
                return null;
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

        public Modifiers Modifiers
        {
            get
            {
                return Modifiers.Unknown;
            }
        }
    }
}
