using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using AbstraX.ServerInterfaces;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using AbstraX.XPathBuilder;
using System.Runtime.Serialization;
using System.ServiceModel.DomainServices.Server;
using AbstraX.Templates;
using AbstraX;
using AbstraX.TypeMappings;
using AbstraX.Contracts;
using EntityProvider.Web.Entities.DatabaseEntities;


namespace EntityProvider.Web.Entities
{
    [DataContract, NodeImage("EntityProvider.Web.Images.EntitySet.png"), DebuggerDisplay("{ DebugInfo }"), ClientCodeGeneration(typeof(AbstraXClientInterfaceGenerator))]
    public class Entity_Set : EntitiesBase, IEntityParent, IElement
    {
        protected IAbstraXService service;
        protected string name;
        private XmlNode node;
        protected string queryWhereProperty;
        protected object queryWhereValue;
        protected IParentBase parent;
        protected IProviderEntityService providerEntityService;
        protected bool hasDocumentation;
        protected string documentationSummary;
        protected string documentation;
        protected string designComments;
        protected float childOrdinal;
        protected string debugInfo;
        protected string id;

        public Entity_Set()
        {
            hasDocumentation = false;
            childOrdinal = 0;
            debugInfo = this.GetDebugInfo();
        }

        public Entity_Set(IAbstraXService service)
            : this()
        {
            this.service = service;
            //providerEntityService = (IProviderEntityService)((IAbstraXProviderService)service).DomainServiceHostApplication;
        }

        public Entity_Set(XmlDocument edmxDocument, XmlNamespaceManager namespaceManager, string name, XmlNode node, Entity_Container parent)
            : this()
        {
            this.EdmxDocument = edmxDocument;
            this.NamespaceManager = namespaceManager;
            this.name = name;
            this.node = node;
            this.parent = parent;
            id = this.MakeID("EntitySet='" + name + "'");

            providerEntityService = ((IBase)parent).ProviderEntityService;

            debugInfo = this.GetDebugInfo();
        }

        [Exclude]
        IProviderEntityService IBase.ProviderEntityService
        {
            get
            {
                return providerEntityService;
            }
        }

        [AbstraXExtensionAttribute("IEntityProviderExtension", "EntityProviderExtension")]
        public IAbstraXExtension LoadExtension()
        {
            return null;
        }

        public IEnumerable<IAttribute> Attributes
        {
            get { return null; }
        }

        [Association("EntitySet_Entities", "ID", "ParentID")]
        public List<EntityType> Entities
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


                        else if (queryWhereProperty != null && queryWhereProperty == "DbEntity" && node != null)
                        {
                            ((IBase)this).ProviderEntityService.DatabaseProxy.GetChildElements(this.ID).ToList<IDbElement>().ForEach(e =>
                            {
                                entities.Add(new DbEntityType(this, e, service));
                            });
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
                            else
                            {
                                ((IBase)this).ProviderEntityService.DatabaseProxy.GetChildElements(this.ID).ToList<IDbElement>().ForEach(e =>
                                {
                                    entities.Add(new DbEntityType(this, e, service));
                                });
                            }
                        }
                    }
                    else
                    {
                        ((IBase)this).ProviderEntityService.DatabaseProxy.GetChildElements(this.ID).ToList<IDbElement>().ForEach(e =>
                        {
                            entities.Add(new DbEntityType(this, e, service));
                        });
                    }

                }
                catch
                {
                }

                return entities;

            }
        }

        public IEnumerable<IElement> ChildElements
        {
            get
            {
                var elements = this.Entities.AsQueryable().Cast<IElement>().Select(e => e);

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
            get { return string.Empty; }
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
                return this.Entities.Any(); ;
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
