using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;
using AbstraX.ServerInterfaces;
using System.Runtime.Serialization;
using AbstraX.XPathBuilder;
using AbstraX;
using AbstraX.TypeMappings;
using Utils;
using System.Collections;
using AbstraX.Models.Interfaces;

namespace RestEntityProvider.Web.Entities
{
    [DebuggerDisplay(" { Name } ")]
    public class RestNavigationProperty : RestEntityBase, IParentBase, INavigationProperty, IRelationProperty, IEntityWithOptionalFacets
    {
        protected RestEntityType parent;
        private bool isCollectionType;
        protected string name;
        private string queryWhereProperty;
        private object queryWhereValue;
        protected float childOrdinal;
        protected ScalarType dataType;
        protected bool hasDocumentation;
        protected string documentationSummary;
        protected string documentation;   
        protected string debugInfo;      
        protected string designComments;
        protected string id;
        public string ThisMultiplicity { get; }
        public string ParentMultiplicity { get; }
        public string ThisPropertyRefName { get; }
        public string ParentPropertyRefName { get; }
        public bool FollowWithout => true;
        public bool NoUIOrConfig { get; set; }

        public RestNavigationProperty(object jsonRootObject, object jsonObject, object jsonOriginalRootObject, object jsonOriginalObject, string name, RestEntityType parentEntity) : base(parentEntity)
        {
            string _namespace = null;
            string multiplicity;
            var ancestor = (IBase)parent;
            var restEntityParent = (RestEntityBase)parent;
            IEnumerable<IElement> childEntities;

            this.parent = parentEntity;

            this.JsonRootObject = jsonRootObject;
            this.JsonObject = jsonObject;
            this.JsonOriginalRootObject = jsonOriginalRootObject;
            this.JsonOriginalObject = jsonOriginalObject;

            this.NoUIOrConfig = parentEntity.NoUIOrConfig;
            this.PathPrefix = parentEntity.PathPrefix;
            this.ConfigPrefix = parent.ConfigPrefix;
            this.ControllerNamePrefix = parent.ControllerNamePrefix;

            this.name = name;

            id = this.MakeID("Property='" + this.name + "'");

            while (ancestor != null)
            {
                if (ancestor is RestModel)
                {
                    _namespace = ((RestModel)ancestor).Namespace;

                    break;
                }

                ancestor = ancestor.Parent;
            }

            childOrdinal = 2;
            childEntities = this.ChildElements;

            multiplicity = this.JsonObject.type == "array" ? "*" : "1..1";

            this.ParentMultiplicity = multiplicity == "*" ? "0..1" : "1..1";
            this.ThisMultiplicity = multiplicity;
            this.ThisPropertyRefName = "id";
            this.ParentPropertyRefName = "id";

            if (isCollectionType)
            {
                dataType = new ScalarType(typeof(IEnumerable), this);
                dataType.IsCollectionType = true;
            }
            else
            {
                dataType = new ScalarType(childEntities.Single().GetType(), this);
            }

            this.NoUIOrConfig = parent.NoUIOrConfig;
        }

        public IAttribute ThisPropertyRef
        {
            get
            {
                return this.ChildElements.Single().Attributes.Single(a => a.Name == this.ThisPropertyRefName);
            }
        }

        public IAttribute ParentPropertyRef
        {
            get
            {
                return parent.Attributes.Single(a => a.Name == this.ParentPropertyRefName);
            }
        }

        public override Facet[] Facets
        {
            get
            {
                var args = new GetAttributesEventArgs(this, this.parent.DataType, this.Name);
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

        public List<IEntityType> ChildEntities
        {
            get 
            {
                var childEntities = new List<RestEntityType>();
                var items = (object)this.JsonObject.items;
                var originalItems = (object)this.JsonOriginalObject.items;

                if (items.IsRef())
                {
                    string name;
                    string originalName;
                    var rootObject = (object)this.JsonRootObject;
                    var originalRootObject = (object)this.JsonOriginalRootObject;
                    var entityObject = rootObject.ResolveDefinition(items, out name);
                    var entityOriginalObject = originalRootObject.ResolveDefinition(originalItems, out originalName);
                    var multiplicity = this.JsonObject.type == "array" ? "*" : "1..1";

                    Debug.Assert(name == originalName);

                    childEntities.Add(new RestEntityType(this.JsonRootObject, entityObject, this.JsonOriginalRootObject, entityOriginalObject, name, this));

                    if (multiplicity == "*")
                    {
                        isCollectionType = true;
                    }
                    else
                    {
                        isCollectionType = false;
                    }

                }
                else
                {
                    DebugUtils.Break();
                }

                return childEntities.ToList<IEntityType>();
            }
        }

        public IEnumerable<IElement> ChildElements
        {
            get 
            {
                var childElements = new List<IElement>();

                if (this.ChildEntities != null)
                {
                    foreach (var entity in this.ChildEntities)
                    {
                        childElements.Add(entity);
                    }
                }

                return childElements;
            }
        }


        public IEnumerable<IBase> ChildNodes
        {
            get
            {
                var nodes = this.ChildEntities.AsQueryable().Cast<IBase>();

                return nodes;
            }
        }

        public bool IsContainer
        {
            get
            {
                return true;
            }
        }

        public ScalarType DataType
        {
            get
            {
                return dataType;
            }
        }

        public IEnumerable<IAttribute> Attributes
        {
            get 
            {
                return new List<IAttribute>(); 
            }
        }

        public IEnumerable<IOperation> Operations
        {
            get { return null; }
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

        public IQueryable ExecuteWhere(AbstraX.XPathBuilder.XPathElement element)
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

        public override string Name
        {
            get
            {
                return name;
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
            get
            {
                return string.Empty;
            }
        }

        public override bool HasDocumentation
        {
            get
            {
                return false;
            }
        }

        public override string DocumentationSummary
        {
            get
            {
                return string.Empty;
            }
        }

        public override float ChildOrdinal
        {
            get
            {
                return childOrdinal;
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
                return parent;
            }
        }

        public override string FolderKeyPair { get; set; }

        public override DefinitionKind Kind
        {
            get
            {
                return DefinitionKind.ComplexProperty;
            }
        }

        public override bool HasChildren
        {
            get
            {
                return this.ChildEntities.Count > 0;
            }
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
                return this.Parent.ID;
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

        public string DefaultValue
        {
            get
            {
                return null;
            }
        }

        #region IElement Members


        BaseType IElement.DataType
        {
            get { return dataType; }
        }

        #endregion
    }
}
