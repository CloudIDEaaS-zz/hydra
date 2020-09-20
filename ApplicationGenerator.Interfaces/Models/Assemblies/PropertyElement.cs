using System;
using System.Net;
using AbstraX.ServerInterfaces;
using System.Reflection;
using AbstraX.XPathBuilder;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Linq.Expressions;
using AbstraX;
using AbstraX.AssemblyInterfaces;
using AbstraX.TypeMappings;
using CodeInterfaces.XPathBuilder;
using AbstraX.Models;

namespace AssemblyProvider.Web.Entities
{
    public class PropertyElement : BaseObject, IElement, IPropertyElement
    {
        private PropertyInfo property;
        protected string queryWhereProperty;
        protected object queryWhereValue;
        protected float childOrdinal;
        protected bool hasDocumentation;
        protected string documentationSummary;
        protected string documentation;
        protected string debugInfo;
        protected BaseType dataType;
        protected string name;
        protected Modifiers modifiers;
        protected string id;

        public PropertyElement(BaseObject parent) : base(parent)
        {
        }

        public PropertyElement(PropertyInfo property, BaseObject parent) : base(parent)
        {
            this.property = property;
            this.childOrdinal = 3;

            name = property.Name;

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

            id = this.MakeID("Property='" + property.Name + "'");
            dataType = new BaseType(property.PropertyType, this);
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

        public IEnumerable<IAttribute> Attributes
        {
            get 
            {
                return new List<IAttribute>();
            }
        }

        public IEnumerable<IOperation> Operations
        {
            get 
            {
                return new List<IOperation>();
            }
        }

        protected virtual List<AssemblyType> GetTypes()
        {
            var typeElement = new AssemblyType(property.PropertyType, this);

            return new List<AssemblyType>() { typeElement };
        }

        [Association("PropertyElement_PropertyTypes", "ID", "ParentID")]
        public List<AssemblyType> Types
        {
            get
            {
                return GetTypes();
            }
        }

        protected virtual IEnumerable<IElement> GetChildElements()
        {
            var typeElement = new AssemblyType(property.PropertyType, this);

            if (typeElement.DataType.IsCollectionType)
            {
                if (queryWhereProperty != null && queryWhereProperty == "Type")
                {
                    yield return new AssemblyType(typeElement.DataType.CollectionType, this);
                }
                else
                {
                    yield return new AssemblyType(typeElement.DataType.CollectionType, this);
                }
            }
            else
            {
                if (queryWhereProperty != null && queryWhereProperty == "Type")
                {
                    yield return typeElement;
                }
                else
                {
                    yield return typeElement;
                }
            }
        }

        public IEnumerable<IElement> ChildElements
        {
            get 
            {
                return GetChildElements();
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
                return string.Empty;
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

        public IQueryable ExecuteWhere(Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable ExecuteWhere(XPathAxisElement element)
        {
            var predicate = element.Predicates.First();

            ClearPredicates();

            queryWhereProperty = predicate.Left;
            queryWhereValue = predicate.Right;

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


        public override DefinitionKind Kind
        {
            get
            {
                return DefinitionKind.NotApplicable;
            }
        }

        public override bool HasChildren
        {
            get 
            {
                return true;
            }
        }

        private static object DebuggerDisplay(string p)
        {
            throw new NotImplementedException();
        }

        IEnumerable<IAssemblyType> IPropertyElement.Types
        {
            get
            {
                foreach (var type in this.Types)
                {
                    yield return type;
                }
            }
        }

        public override Facet[] Facets
        {
            get
            {
                return null;
            }
        }

        public ContainerType AllowableContainerTypes
        {
            get
            {
                return ContainerType.NotSet;
            }
        }

        public ConstructType AllowableConstructTypes
        {
            get
            {
                return ConstructType.NotSet;
            }
        }

        public ContainerType DefaultContainerType
        {
            get
            {
                return ContainerType.NotSet;
            }
        }

        public ConstructType DefaultConstructType
        {
            get
            {
                return ConstructType.NotSet;
            }
        }

        public IQueryable ExecuteWhere(XPathElement element)
        {
            throw new NotImplementedException();
        }

        public override Modifiers Modifiers
        {
            get
            {
                return modifiers;
            }
        }

        public IEnumerable<IBase> ChildNodes => throw new NotImplementedException();

        public override float ChildOrdinal { get; }
        public override string DebugInfo { get; }
        public override IBase Parent { get; }
    }
}
