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
using AbstraX.Models.Interfaces;
using Utils;

namespace RestEntityProvider.Web.Entities
{
    [DebuggerDisplay(" { Name } ")]
    public class RestEntitySet : RestEntityBase, IEntitySet, IRelationProperty, IEntityWithOptionalFacets
    {
        protected string name;
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
        public string ThisMultiplicity { get; }
        public string ParentMultiplicity { get; }
        public string ThisPropertyRefName { get; }
        public string ParentPropertyRefName { get; }
        public IAttribute ThisPropertyRef => null;
        public IAttribute ParentPropertyRef => null;
        public bool FollowWithout => true;
        public bool NoUIOrConfig { get; set; }

        public RestEntitySet(object jsonRootObject, object jsonObject, object jsonOriginalRootObject, object jsonOriginalObject, string name, RestEntityContainer parent) : base(parent)
        {
            this.JsonRootObject = jsonRootObject;
            this.JsonObject = jsonObject;
            this.JsonOriginalRootObject = jsonOriginalRootObject;
            this.JsonOriginalObject = jsonOriginalObject;

            this.name = name;
            this.parent = parent;

            this.PathPrefix = parent.PathPrefix;
            this.ConfigPrefix = parent.ConfigPrefix;
            this.ControllerNamePrefix = parent.ControllerNamePrefix;

            id = this.MakeID("EntitySet='" + name + "'");

            debugInfo = this.GetDebugInfo();

            this.ParentMultiplicity = "static";
            this.ThisMultiplicity = "*";
            this.ThisPropertyRefName = null;
            this.ParentPropertyRefName = null;
            this.NoUIOrConfig = parent.NoUIOrConfig;
        }

        public IEnumerable<IAttribute> Attributes
        {
            get { return null; }
        }

        public List<IEntityType> Entities
        {
            get
            {

                var entities = new List<RestEntityType>();

                try
                {
                    var items = (object)this.JsonObject.items;
                    var originalItems = (object)this.JsonOriginalObject.items;

                    if (items.IsRef())
                    {
                        string name;
                        string originalName;
                        var rootObject = (object) this.JsonRootObject;
                        var originalRootObject = (object)this.JsonOriginalRootObject;
                        var entityObject = rootObject.ResolveDefinition(items, out name);
                        var entityOriginalObject = originalRootObject.ResolveDefinition(originalItems, out originalName);

                        Debug.Assert(name == originalName);

                        entities.Add(new RestEntityType(this.JsonRootObject, entityObject, this.JsonOriginalRootObject, entityOriginalObject, name, this));
                    }
                    else
                    {
                        DebugUtils.Break();
                    }
                }
                catch (Exception ex)
                {
                }

                return entities.ToList<IEntityType>();

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

        public IEnumerable<IBase> ChildNodes
        {
            get
            {
                var nodes = this.Entities.AsQueryable().Cast<IBase>();

                return nodes;
            }
        }

        public IEnumerable<IOperation> Operations
        {
            get
            {
                return null;
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
                return this.parent;
            }
        }

        public override string FolderKeyPair { get; set; }

        public override DefinitionKind Kind
        {
            get
            {
                return DefinitionKind.ComplexSetProperty;
            }
        }

        public override bool HasChildren
        {
            get
            {
                return this.Entities.Any();
            }
        }

        public override Facet[] Facets
        {
            get
            {
                var args = new GetAttributesEventArgs(this, ((RestEntityContainer) this.parent).DataType, this.Name);
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
