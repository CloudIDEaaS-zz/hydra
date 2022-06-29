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
    public class FieldAttribute : BaseObject, IAttribute, IFieldAttribute
    {
        private FieldInfo field;
        private IBase parent;
        private string queryWhereProperty;
        private object queryWhereValue;
        private float childOrdinal;
        private string imageURL;

        public FieldAttribute(FieldInfo field, BaseObject parent) : base(parent)
        {
            this.field = field;
            this.parent = parent;
            this.childOrdinal = 2;

            if (parent.Kind == DefinitionKind.Enumeration)
            {
                imageURL = "AssemblyProvider.Web.Images.EnumItem.png";
            }
            else
            {
                imageURL = "AssemblyProvider.Web.Images.Field.png";
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
                var type = typeof(AssemblyType);

                return new ScalarType(type, this);
            }
        }

        public override string ID
        {
            get 
            {
                return this.MakeID("Field='" + field.Name + "'");
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
                return field.Name;
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

        public string ImageURL
        {
            get
            {
                return imageURL;
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
                return DefinitionKind.Field;
            }
        }

        ScalarType IAttribute.DataType
        {
            get
            {
                var type = field.FieldType;

                return new ScalarType(type, this);
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
                var modifiers = Modifiers.Unknown;

                if (field.DeclaringType.FullName == ((IElement)parent).DataType.FullyQualifiedName)
                {
                    modifiers |= Modifiers.IsLocal;
                }

                if (field.IsInitOnly)
                {
                    modifiers |= Modifiers.CanRead;
                }
                else
                {
                    modifiers |= Modifiers.CanRead | Modifiers.CanWrite;
                }

                return modifiers;
            }
        }

        public string DefaultValue
        {
            get
            {
                if (parent.Kind == DefinitionKind.Enumeration && !field.FieldType.IsPrimitive)
                {
                    return field.GetRawConstantValue().ToString();
                }
                else
                {
                    return null;
                }
            }
        }

        public bool Nullable => throw new NotImplementedException();
        public IEnumerable<IBase> ChildNodes => throw new NotImplementedException();
    }
}
