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
using Utils;
using AbstraX.Models;

namespace RestEntityProvider.Web.Entities
{
    [DebuggerDisplay(" { Name } ")]
    public class RestEntityType : RestEntityBase, IElementWithKeyAttribute, IElementWithSurrogateTemplateType, IEntityWithOptionalFacets, IEntityType
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
        public bool FollowWithout => true;
        public bool NoUIOrConfig { get; set; }
        private Dictionary<Type, Type> registeredSurrogateTemplates;
                
        public RestEntityType(object jsonRootObject, object jsonObject, object jsonOriginalRootObject, object jsonOriginalObject, string name, BaseObject parent) : base(parent)
        {
            string _namespace = null;
            var ancestor = (IBase)parent;
            var restEntityParent = (RestEntityBase)parent;

            this.JsonRootObject = jsonRootObject;
            this.JsonObject = jsonObject;
            this.JsonOriginalRootObject = jsonOriginalRootObject;
            this.JsonOriginalObject = jsonOriginalObject;

            this.NoUIOrConfig = ((IEntityWithOptionalFacets)parent).NoUIOrConfig;
            this.PathPrefix = restEntityParent.PathPrefix;
            this.ConfigPrefix = restEntityParent.ConfigPrefix;
            this.ControllerNamePrefix = restEntityParent.ControllerNamePrefix;

            this.parent = (IParentBase) parent;
            this.name = name;
            this.registeredSurrogateTemplates = new Dictionary<Type, Type>();

            id = this.MakeID("Entity='" + name + "'");

            while (ancestor != null)
            {
                if (ancestor is RestModel)
                {
                    _namespace = ((RestModel)ancestor).Namespace;

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

        public List<IEntityProperty> Properties
        {
            get
            {
                var properties = new List<RestEntityProperty>();
                var objectProperties = (object)this.JsonObject.properties;

                if (objectProperties != null)
                {
                    var pairs = objectProperties.GetDynamicMemberNameValueDictionary();

                    foreach (var pair in pairs)
                    {
                        var name = pair.Key;
                        var properyObject = pair.Value;
                        var jsonObject = (dynamic)properyObject;
                        var type = (string)jsonObject.type;

                        switch (type)
                        {
                            case "array":
                                break;
                            case "object":
                                break;
                            case null:
                                break;
                            default:
                                properties.Add(new RestEntityProperty(jsonObject, this.JsonRootObject, name, this));
                                break;
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
                var properties = new List<RestNavigationProperty>();
                var objectProperties = (object)this.JsonObject.properties;
                var originalProperties = (object)this.JsonObject.properties;

                if (objectProperties != null)
                {
                    var pairs = objectProperties.GetDynamicMemberNameValueDictionary();
                    var originalPairs = originalProperties.GetDynamicMemberNameValueDictionary();
                    var x = 0;

                    foreach (var pair in pairs)
                    {
                        var name = pair.Key;
                        var properyObject = pair.Value;
                        var jsonObject = (dynamic)properyObject;
                        var type = (string)jsonObject.type;

                        switch (type)
                        {
                            case "array":
                                properties.Add(new RestNavigationProperty(this.JsonRootObject, jsonObject, this.JsonOriginalRootObject, originalPairs.ElementAt(x).Value, name, this));
                                break;
                            case "object":
                                properties.Add(new RestNavigationProperty(this.JsonRootObject, jsonObject, this.JsonOriginalRootObject, originalPairs.ElementAt(x).Value, name, this));
                                break;
                            default:
                                break;
                        }

                        x++;
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

        public IAttribute GetKeyAttribute()
        {
            var keyAttribute = this.Properties.Cast<RestEntityProperty>().Single(p => ((object)p.JsonObject).HasDynamicMember("isKey"));

            return keyAttribute;
        }

        public Type GetSurrogateTemplateType<TSurrogateFor>()
        {
            return this.registeredSurrogateTemplates[typeof(TSurrogateFor)];
        }

        public bool HasSurrogateTemplateType<TSurrogateFor>()
        {
            return this.registeredSurrogateTemplates.ContainsKey(typeof(TSurrogateFor));
        }

        public void RegisterSurrogateTemplateType<T, TSurrogate>()
        {
            this.registeredSurrogateTemplates.Add(typeof(T), typeof(TSurrogate));
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
                var args = new GetAttributesEventArgs(this, this.dataType);
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
