using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using AbstraX.ServerInterfaces;
using System.Diagnostics;
using AbstraX.XPathBuilder;
using System.Runtime.Serialization;
using AbstraX;
using AbstraX.TypeMappings;


namespace EntityProvider.Web.Entities
{
    public class Entity_Container : EntitiesBase, IElement
    {
        private IAbstraXService service;
        private string containerName;
        private XmlNode node;
        private string queryWhereProperty;
        private object queryWhereValue;
        private Model parent;
        protected string id;

        public Entity_Container()
        {
        }

        public Entity_Container(IAbstraXService service)
        {
            this.service = service;
        }

        public Entity_Container(XmlDocument edmxDocument, XmlNamespaceManager namespaceManager, string containerName, XmlNode node, Model parent)
        {
            this.EdmxDocument = edmxDocument;
            this.NamespaceManager = namespaceManager;
            this.containerName = containerName;
            this.node = node;
            this.parent = parent;

            id = this.MakeID("Container='" + containerName + "'");
        }

        public IEnumerable<IAttribute> Attributes
        {
            get { return null; }
        }

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
                    }
                    else
                    {
                        foreach (XmlNode childNode in node.SelectNodes("e:EntitySet", this.NamespaceManager))
                        {
                            var name = childNode.Attributes["Name"].Value;

                            entitySets.Add(new Entity_Set(this.EdmxDocument, this.NamespaceManager, name, childNode, this));
                        }
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
                return containerName;
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


        public string DesignComments
        {
            get { return string.Empty; }
        }

        public string Documentation
        {
            get { return string.Empty; }
        }

        public bool HasDocumentation
        {
            get { return false; }
            set { }
        }

        public string DocumentationSummary
        {
            get { return string.Empty; }
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
                return this.EntitySets.Any();
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
