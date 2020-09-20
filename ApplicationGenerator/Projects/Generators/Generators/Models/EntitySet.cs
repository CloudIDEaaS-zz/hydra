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


namespace EntityProvider.Web.Entities
{
    public class Entity_Set : EntitiesBase, IEntityParent, IElement
    {
        protected IAbstraXService service;
        protected string name;
        private XmlNode node;
        protected string queryWhereProperty;
        protected object queryWhereValue;
        protected IParentBase parent;
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

        public Entity_Set(IAbstraXService service) : this()
        {
            this.service = service;
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

            debugInfo = this.GetDebugInfo();
        }

        public IEnumerable<IAttribute> Attributes
        {
            get { return null; }
        }

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

        public string ImageURL
        {
            get
            {
                return string.Empty;
            }
        }

        public bool IsContainer
        {
            get
            {
                return true;
            }
        }

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

        public string DesignComments
        {
            get { return string.Empty; }
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
                return this.Entities.Any(); ;
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
