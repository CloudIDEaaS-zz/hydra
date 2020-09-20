using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using AbstraX.ServerInterfaces;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.ServiceModel.DomainServices.Server;
using AbstraX.XPathBuilder;
using System.Runtime.Serialization;
using AbstraX.Templates;
using AbstraX;
using AbstraX.TypeMappings;
using AbstraX.Contracts;
using EntityProvider.Web.Entities.DatabaseEntities;


namespace EntityProvider.Web.Entities
{
    [DataContract, NodeImage("EntityProvider.Web.Images.EntityContainer.png"), DebuggerDisplay("{ DebugInfo }"), ClientCodeGeneration(typeof(AbstraXClientInterfaceGenerator))]
    public class Entity_Container : EntitiesBase, IElement
    {
        private IAbstraXService service;
        private string containerName;
        private XmlNode node;
        private string queryWhereProperty;
        private object queryWhereValue;
        private Model parent;
        protected IProviderEntityService providerEntityService;
        protected string id;

        public Entity_Container()
        {
        }

        public Entity_Container(IAbstraXService service)
        {
            this.service = service;
            providerEntityService = ((IBase)parent).ProviderEntityService;
        }

        public Entity_Container(XmlDocument edmxDocument, XmlNamespaceManager namespaceManager, string containerName, XmlNode node, Model parent)
        {
            this.EdmxDocument = edmxDocument;
            this.NamespaceManager = namespaceManager;
            this.containerName = containerName;
            this.node = node;
            this.parent = parent;

            providerEntityService = ((IBase)parent).ProviderEntityService;
            id = this.MakeID("Container='" + containerName + "'");
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

        public IEnumerable<IAttribute> Attributes
        {
            get { return null; }
        }

        [Association("EntityContainer_EntitySets", "ID", "ParentID")]
        public List<Entity_Set> EntitySets
        {
            get
            {
                var entitySets = new List<Entity_Set>();

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
                        else
                        {
                            var temp = ((IBase)this).ProviderEntityService.DatabaseProxy.GetChildElements(this.ID).ToList<IDbElement>().Single(e => e.Name == (string)queryWhereValue);
                            entitySets.Add(new DbEntitySet(this, temp, service));
                        }
                    }
                    else if (queryWhereProperty != null && queryWhereProperty == "DbEntitySet")
                    {
                        var temp = ((IBase)this).ProviderEntityService.DatabaseProxy.GetChildElements(this.ID).ToList<IDbElement>().Single(e => e.DatabaseID == (string)queryWhereValue);
                        entitySets.Add(new DbEntitySet(this, temp, service));
                    }
                    else
                    {
                        foreach (XmlNode childNode in node.SelectNodes("e:EntitySet", this.NamespaceManager))
                        {
                            var name = childNode.Attributes["Name"].Value;

                            entitySets.Add(new Entity_Set(this.EdmxDocument, this.NamespaceManager, name, childNode, this));
                        }

                        ((IBase)this).ProviderEntityService.DatabaseProxy.GetChildElements(this.ID).ToList<IDbElement>().ForEach(e =>
                        {
                            entitySets.Add(new DbEntitySet(this, e, service));
                        });
                    }

                }
                catch
                {
                }


                return entitySets;
            }
        }

        public IEnumerable<IElement> ChildElements
        {
            get
            {
                var elements = this.EntitySets.AsQueryable().Cast<IElement>().Select(e => e);

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
                return containerName;
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
                return true;
            }
        }

        [Include, Association("Parent_BaseType", "ID", "ParentID")]
        public BaseType DataType
        {
            get
            {
                return null;
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
            get { return string.Empty; }
        }

        [DataMember]
        public string Documentation
        {
            get { return string.Empty; }
        }

        [DataMember]
        public bool HasDocumentation
        {
            get { return false; }
            set { }
        }

        [DataMember]
        public string DocumentationSummary
        {
            get { return string.Empty; }
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
                return this.GetDebugInfo();
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
                return DefinitionKind.NotApplicable;
            }
        }

        [DataMember]
        public bool HasChildren
        {
            get
            {
                return this.EntitySets.Any();
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
