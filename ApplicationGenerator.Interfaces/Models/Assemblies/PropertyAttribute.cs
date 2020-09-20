using System;
using System.Net;
using AbstraX.ServerInterfaces;
using System.Reflection;
using AbstraX.XPathBuilder;
using System.Linq;
using System.Xml.Schema;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using AbstraX;
using AbstraX.AssemblyInterfaces;
using System.Collections.Generic;
using CodeInterfaces.XPathBuilder;
using AbstraX.Models;

namespace AssemblyProvider.Web.Entities
{
    public class PropertyAttribute : BaseObject, IAttribute, IPropertyAttribute
    {
        private PropertyInfo property;
        protected IBase parent;
        protected string queryWhereProperty;
        protected object queryWhereValue;
        protected float childOrdinal;
        protected bool hasDocumentation;
        protected string documentationSummary;
        protected string documentation;
        protected string debugInfo;
        protected ScalarType dataType;
        protected string name;
        protected Modifiers modifiers;
        protected string id;

        public PropertyAttribute(BaseObject parent) : base(parent)
        {
        }

        public PropertyAttribute(PropertyInfo property, BaseObject parent) : base(parent)
        {
            this.property = property;
            this.parent = parent;
            this.childOrdinal = 3;
            name = property.Name;

            var type = property.PropertyType;

            id = this.MakeID("Property='" + property.Name + "'");

            dataType = new ScalarType(type, this);

            modifiers = Modifiers.Unknown;

            if (property.CanRead)
            {
                modifiers |= Modifiers.CanRead;
            }

            if (property.CanWrite)
            {
                modifiers |= Modifiers.CanWrite;
            }

            if (property.DeclaringType.FullName == ((IElement)parent).DataType.FullyQualifiedName)
            {
                modifiers |= Modifiers.IsLocal;
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
                return this.GetDebugInfo();
            }
        }

        public ScalarType DataType
        {
            get
            {
                return dataType;
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
                return documentation;
            }
        }

        public override bool HasDocumentation
        {
            get
            {
                return hasDocumentation;
            }
        }

        public override string DocumentationSummary
        {
            get
            {
                return documentationSummary;
            }
        }

        public string ImageURL
        {
            get
            {
                return string.Empty;
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
            throw new NotImplementedException();
        }

        public void ClearPredicates()
        {
            queryWhereProperty = null;
            queryWhereValue = null;
        }

        public IQueryable ExecuteWhere(XPathElement element)
        {
            throw new NotImplementedException();
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


        public override DefinitionKind Kind
        {
            get 
            {
                return DefinitionKind.SimpleProperty;
            }
        }

        public System.Collections.Generic.IEnumerable<IElement> ChildElements
        {
            get { throw new NotImplementedException(); }
        }

        public override bool HasChildren
        {
            get 
            {
                return false;
            }
        }

        public override Facet[] Facets
        {
            get
            {
                return null;
            }
        }

        public override Modifiers Modifiers
        {
            get
            {
                return modifiers;
            }
        }

        public string DefaultValue
        {
            get
            {
                return null;
            }
        }

        public bool Nullable => throw new NotImplementedException();

        public IEnumerable<IBase> ChildNodes => throw new NotImplementedException();
    }
}
