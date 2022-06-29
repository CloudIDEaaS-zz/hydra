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
using AbstraX.Models.Interfaces;
using AbstraX.Models;

namespace EntityProvider.Web.Entities
{
    [DebuggerDisplay(" { Name } ")]
    public class EntityType : EntityBase, IEntityType, IElement, IEntityObjectWithFacets, IEntityObjectWithDataType
    {
        protected string name;
        protected string queryWhereProperty;
        protected object queryWhereValue;
        protected IParentBase parent;
        protected bool hasDocumentation;
        protected string documentationSummary;
        protected string documentation;
        protected float childOrdinal;
        protected string debugInfo;
        protected BaseType dataType;
        protected string designComments;
        protected string id;
        protected string tempID;

        public string Namespace { get; }

        public EntityType(XmlDocument edmxDocument, XmlNamespaceManager namespaceManager, string name, XmlNode node, BaseObject parent) : base(parent)
        {
            string _namespace = null;
            var ancestor = (IBase)parent;

            this.EdmxDocument = edmxDocument;
            this.NamespaceManager = namespaceManager;
            this.name = name;
            this.node = node;
            this.parent = (IParentBase) parent;

            id = this.MakeID("Entity='" + name + "'");

            while (ancestor != null)
            {
                if (ancestor is Model)
                {
                    _namespace = ((Model)ancestor).Namespace;

                    break;
                }

                ancestor = ancestor.Parent;
            }

            this.Namespace = _namespace;

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

        public List<IEntityProperty> Properties
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

                return properties.ToList<IEntityProperty>();
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

        public List<INavigationProperty> NavigationProperties
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
                
                return properties.ToList<INavigationProperty>();
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

        public IEnumerable<IBase> ChildNodes
        {
            get
            {
                var nodes = this.NavigationProperties.AsQueryable().Cast<IBase>().Concat(
                    this.Properties.AsQueryable().Cast<IBase>()
                );

                return nodes;
            }
        }

        public IEnumerable<IOperation> Operations
        {
            get { return null; }
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
                return parent.ID;
            }           

        }

        public override string Name
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
            if (value is XPathElement && (property == "ID" || property == "ParentID"))
            {
                var predicate = ((XPathElement)value).Predicates.First();

                queryWhereProperty = predicate.Left.ToString();
                queryWhereValue = predicate.Right.ToString();
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

        public IQueryable ExecuteWhere(XPathElement element)
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

        public override string DesignComments
        {
            get
            {
                return string.Format("Type: {0}, Kind {1}, ID:'{2}'", this.GetType().Name, this.Kind, this.ID);
            }
        }

        public override string Documentation
        {
            get { return documentation; }
        }

        public override bool HasDocumentation
        {
            get { return hasDocumentation; }
        }

        public override string DocumentationSummary
        {
            get { return documentationSummary; }
        }

        public override float ChildOrdinal
        {
            get
            {
                return 0;
            }
        }

        public override string DebugInfo
        {
            get
            {
                return debugInfo;
            }
        }

        public override IBase Parent
        {
            get
            {
                return this.parent;
            }
        }

        public override string FolderKeyPair { get; set; }

        public override DefinitionKind Kind
        {
            get
            {
                return DefinitionKind.Class;
            }
        }

        public override bool HasChildren
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

        public override Facet[] Facets
        {
            get
            {
                var args = new GetAttributesEventArgs(this, this.GetOriginalDataType());
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

        public GetOverridesEventHandler OverrideEventHandler
        {
            get
            {
                return this.getOverrideEventHandlers.LastOrDefault();
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
    }
}
