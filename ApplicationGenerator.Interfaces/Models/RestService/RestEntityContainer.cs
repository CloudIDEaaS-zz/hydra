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
using Utils;
using AbstraX.Models.Interfaces;

namespace RestEntityProvider.Web.Entities
{
    [DebuggerDisplay(" { Name } ")]
    public class RestEntityContainer : RestEntityBase, IElement, IEntityContainer, IElementWithFacetHanderTypes, IEntityWithOptionalFacets
    {
        private string containerName;
        private string queryWhereProperty;
        private object queryWhereValue;
        private RestModel parent;
        protected string id;
        protected BaseType dataType;
        private List<Type> facetHandlerTypes;
        public Dictionary<string, object> Variables { get; }

        public bool FollowWithout => true;
        public bool NoUIOrConfig { get; set; }
        public bool PreventRecursion { get => true; }

        public RestEntityContainer(object jsonRootObject, object jsonObject, object jsonOriginalRootObject, object jsonOriginalObject, string name, RestModel parent) : base(parent)
        {
            string _namespace = null;
            var ancestor = (IBase)parent;

            this.Variables = parent.Variables;
            this.JsonRootObject = jsonRootObject;
            this.JsonObject = jsonObject;
            this.JsonOriginalRootObject = jsonOriginalRootObject;
            this.JsonOriginalObject = jsonOriginalObject;

            this.containerName = name;
            this.parent = parent;

            this.facetHandlerTypes =  new List<Type>();
            this.PathPrefix = parent.PathPrefix;
            this.ConfigPrefix = parent.ConfigPrefix;
            this.ControllerNamePrefix = parent.ControllerNamePrefix;

            id = this.MakeID("Container='" + containerName + "'");

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
                FullyQualifiedName = _namespace + "." + containerName,
                Name = containerName,
                ID = containerName,
                ParentID = this.ID
            };
        }

        public IEnumerable<IAttribute> Attributes
        {
            get { return null; }
        }

        public List<IEntitySet> EntitySets
        {
            get
            {
                var entitySets = new List<RestEntitySet>();

                try
                {
                    var properties = (object) this.JsonRootObject.properties;
                    var originalProperties = (object)this.JsonOriginalRootObject.properties;
                    var pairs = properties.GetDynamicMemberNameValueDictionary();
                    var originalPairs = originalProperties.GetDynamicMemberNameValueDictionary();
                    var x = 0;

                    foreach (var pair in pairs)
                    {
                        entitySets.Add(new RestEntitySet(this.JsonRootObject, pair.Value, this.JsonOriginalRootObject, originalPairs.ElementAt(x).Value, pair.Key, this));

                        x++;
                    }
                }
                catch
                {
                }

                return entitySets.ToList<IEntitySet>();
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

        public IEnumerable<IBase> ChildNodes
        {
            get
            {
                var nodes = this.EntitySets.AsQueryable().Cast<IBase>();

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

        public IEnumerable<Type> GetFacetHandlerTypes()
        {
            return this.facetHandlerTypes;
        }

        public void RegisterFacetHandlerType<T>()
        {
            this.facetHandlerTypes.Add(typeof(T));
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
            get { return string.Empty; }
        }

        public override bool HasDocumentation
        {
            get { return false; }
        }

        public override string DocumentationSummary
        {
            get { return string.Empty; }
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
                return this.GetDebugInfo();
            }
        }

        public override IBase Parent
        {
            get
            {
                return parent;
            }
        }

        public override string FolderKeyPair { get; set; }

        public override DefinitionKind Kind
        {
            get
            {
                return DefinitionKind.StaticContainer;
            }
        }

        public override bool HasChildren
        {
            get
            {
                return this.EntitySets.Any();
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

                    if (args.NoMetadata)
                    {
                        this.NoUIOrConfig = true;
                    }
                    else if (args.Attributes != null)
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
