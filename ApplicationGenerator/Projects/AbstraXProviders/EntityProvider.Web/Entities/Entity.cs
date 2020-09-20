using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using AbstraX.ServerInterfaces;
using System.ServiceModel.DomainServices.Server;
using System.ComponentModel.DataAnnotations;
using AbstraX.XPathBuilder;
using System.Runtime.Serialization;
using System.Diagnostics;
using AbstraX.Templates;
using AbstraX;
using AbstraX.TypeMappings;
using AbstraX.Contracts;
using EntityProvider.Web.Entities.DatabaseEntities;

namespace EntityProvider.Web.Entities
{
    [KnownType(typeof(DbEntityType ))]
    [DataContract, NodeImage("EntityProvider.Web.Images.Entity.png"), DebuggerDisplay("{ DebugInfo }"), ClientCodeGeneration(typeof(AbstraXClientInterfaceGenerator))]    
    public class EntityType : EntitiesBase, IElement
    {
        protected IAbstraXService service;
        protected string name;
        private XmlNode node;
        protected string queryWhereProperty;
        protected object queryWhereValue;
        protected IEntityParent parent;
        protected IProviderEntityService providerEntityService;
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

            providerEntityService = ((IBase)parent).ProviderEntityService;

            string _namespace = null;
            var ancestor = (IBase) parent;

            while (ancestor != null)
            {
                if (ancestor is Project)
                {
                    var project = (Project)ancestor;
                    _namespace = project.DefaultNamespace;

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

        [Exclude()]
        public XmlNode Node
        {
            get 
            { 
                return node; 
            }
        }

        [Association("Entity_EntityProperties", "ID", "ParentID")]
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
                    else
                    {
                       
                        ((IBase)this).ProviderEntityService.DatabaseProxy.GetAttributes(tempID).ToList<IDbAttribute>().ForEach(e =>
                        {
                            properties.Add(new DbEntityProperty(this, e, service));
                        });

                    }
                }
                else if (queryWhereProperty != null && queryWhereProperty == "DbProperty")
                {
                    ((IBase)this).ProviderEntityService.DatabaseProxy.GetAttributes(tempID).Where(p => p.DatabaseID == (string)queryWhereValue).ToList<IDbAttribute>().ForEach(e =>
                    {
                        properties.Add(new DbEntityProperty(this, e, service));
                    });
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
                    else
                    {                        
                        ((IBase)this).ProviderEntityService.DatabaseProxy.GetAttributes(tempID).ToList<IDbAttribute>().ForEach(e =>
                        {
                            properties.Add(new DbEntityProperty(this, e, service));
                        });

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

        [Association("Entity_NavigationProperties", "ID", "ParentID")]
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
                    else
                    {
                        ((IBase)this).ProviderEntityService.DatabaseProxy.GetChildElements(tempID).ToList<IDbElement>().ForEach(e =>
                        {
                            properties.Add(new DbNavigationProperty(this, e, service));
                        });                        

                    }
                }
                else if (queryWhereProperty != null && queryWhereProperty == "DbProperty")
                {
                   ((IBase)this).ProviderEntityService.DatabaseProxy.GetChildElements(tempID).Where(p => p.DatabaseID == (string)queryWhereValue).ToList<IDbElement>().ForEach(e =>
                    {
                        properties.Add(new DbNavigationProperty(this, e, service));
                    });                  
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
                    else
                    {
                        ((IBase)this).ProviderEntityService.DatabaseProxy.GetChildElements(tempID).ToList<IDbElement>().ForEach(e =>
                        {
                            properties.Add(new DbNavigationProperty(this, e, service));
                        });

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
                return parent.ID;
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
                return string.Empty;
            }
        }

        [DataMember]
        public bool IsContainer
        {
            get
            {
                return false;
            }
        }

        [Include, Association("Parent_BaseType", "ID", "ParentID")]
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
            get { return documentation; }
        }

        [DataMember]
        public bool HasDocumentation
        {
            get { return hasDocumentation; }
            set { }
        }

        [DataMember]
        public string DocumentationSummary
        {
            get { return documentationSummary; }
            set { }
        }

        [DataMember]
        public float ChildOrdinal
        {
            get
            {
                return 0;
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

        public IBase Parent
        {
            get
            {
                return this.parent;
            }
        }

        [DataMember]
        public string FolderKeyPair { get; set; }

        [DataMember]
        public DefinitionKind Kind
        {
            get
            {
                return DefinitionKind.NotApplicable;
            }
        }

        [DataMember]
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
    }
}
