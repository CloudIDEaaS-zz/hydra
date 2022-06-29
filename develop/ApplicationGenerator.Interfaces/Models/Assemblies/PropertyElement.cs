// file:	Models\Assemblies\PropertyElement.cs
//
// summary:	Implements the property element class

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
    /// <summary>   A property element. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/13/2020. </remarks>

    public class PropertyElement : BaseObject, IElement, IPropertyElement
    {
        /// <summary>   The property. </summary>
        private PropertyInfo property;
        /// <summary>   The query where property. </summary>
        protected string queryWhereProperty;
        /// <summary>   The query where value. </summary>
        protected object queryWhereValue;
        /// <summary>   The child ordinal. </summary>
        protected float childOrdinal;
        /// <summary>   True if has documentation, false if not. </summary>
        protected bool hasDocumentation;
        /// <summary>   The documentation summary. </summary>
        protected string documentationSummary;
        /// <summary>   The documentation. </summary>
        protected string documentation;
        /// <summary>   Information describing the debug. </summary>
        protected string debugInfo;
        /// <summary>   Type of the data. </summary>
        protected BaseType dataType;
        /// <summary>   The name. </summary>
        protected string name;
        /// <summary>   The modifiers. </summary>
        protected Modifiers modifiers;
        /// <summary>   The identifier. </summary>
        protected string id;
        private IBase parent;

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/13/2020. </remarks>
        ///
        /// <param name="parent">   The parent. </param>

        public PropertyElement(BaseObject parent) : base(parent)
        {
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/13/2020. </remarks>
        ///
        /// <param name="property"> The property. </param>
        /// <param name="parent">   The parent. </param>

        public PropertyElement(PropertyInfo property, BaseObject parent) : base(parent)
        {
            this.property = property;
            this.childOrdinal = 3;
            this.parent = parent;

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

        /// <summary>   Gets a value indicating whether this  is container. </summary>
        ///
        /// <value> True if this  is container, false if not. </value>

        public bool IsContainer
        {
            get 
            {
                return true;
            }
        }

        /// <summary>   Gets the type of the data. </summary>
        ///
        /// <value> The type of the data. </value>

        public BaseType DataType
        {
            get
            {
                return dataType;
            }
        }

        /// <summary>   Gets the attributes. </summary>
        ///
        /// <value> The attributes. </value>

        public IEnumerable<IAttribute> Attributes
        {
            get 
            {
                return new List<IAttribute>();
            }
        }

        /// <summary>   Gets the operations. </summary>
        ///
        /// <value> The operations. </value>

        public IEnumerable<IOperation> Operations
        {
            get 
            {
                return new List<IOperation>();
            }
        }

        /// <summary>   Gets the types. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/13/2020. </remarks>
        ///
        /// <returns>   The types. </returns>

        protected virtual List<AssemblyType> GetTypes()
        {
            var typeElement = new AssemblyType(property.PropertyType, this);

            return new List<AssemblyType>() { typeElement };
        }

        /// <summary>   Gets the types. </summary>
        ///
        /// <value> The types. </value>

        [Association("PropertyElement_PropertyTypes", "ID", "ParentID")]
        public List<AssemblyType> Types
        {
            get
            {
                return GetTypes();
            }
        }

        /// <summary>   Gets the child elements in this collection. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/13/2020. </remarks>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the child elements in this collection.
        /// </returns>

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

        /// <summary>   Gets the child elements. </summary>
        ///
        /// <value> The child elements. </value>

        public IEnumerable<IElement> ChildElements
        {
            get 
            {
                return GetChildElements();
            }
        }

        /// <summary>   Gets or sets the identifier. </summary>
        ///
        /// <value> The identifier. </value>

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

        /// <summary>   Gets the identifier of the parent. </summary>
        ///
        /// <value> The identifier of the parent. </value>

        public override string ParentID
        {
            get 
            {
                return this.Parent.ID;
            }
        }

        /// <summary>   Gets the name. </summary>
        ///
        /// <value> The name. </value>

        public override string Name
        {
            get 
            {
                return name;
            }
        }

        /// <summary>   Gets the design comments. </summary>
        ///
        /// <value> The design comments. </value>

        public override string DesignComments
        {
            get
            {
                return string.Format("Type: {0}, Kind {1}, ID:'{2}'", this.GetType().Name, this.Kind, this.ID);
            }
        }

        /// <summary>   Gets the documentation. </summary>
        ///
        /// <value> The documentation. </value>

        public override string Documentation
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>   Gets a value indicating whether this  has documentation. </summary>
        ///
        /// <value> True if this  has documentation, false if not. </value>

        public override bool HasDocumentation
        {
            get
            {
                return false;
            }
        }

        /// <summary>   Gets the documentation summary. </summary>
        ///
        /// <value> The documentation summary. </value>

        public override string DocumentationSummary
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>   Gets URL of the image. </summary>
        ///
        /// <value> The image URL. </value>

        public string ImageURL
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>   Gets or sets the folder key pair. </summary>
        ///
        /// <value> The folder key pair. </value>

        public override string FolderKeyPair { get; set; }

        /// <summary>   Executes the where operation. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/13/2020. </remarks>
        ///
        /// <param name="property"> The property. </param>
        /// <param name="value">    The value. </param>
        ///
        /// <returns>   An IQueryable. </returns>

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

        /// <summary>   Executes the where operation. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/13/2020. </remarks>
        ///
        /// <param name="expression">   The expression. </param>
        ///
        /// <returns>   An IQueryable. </returns>

        public IQueryable ExecuteWhere(Expression expression)
        {
            throw new NotImplementedException();
        }

        /// <summary>   Executes the where operation. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/13/2020. </remarks>
        ///
        /// <param name="element">  The element. </param>
        ///
        /// <returns>   An IQueryable. </returns>

        public IQueryable ExecuteWhere(XPathAxisElement element)
        {
            var predicate = element.Predicates.First();

            ClearPredicates();

            queryWhereProperty = predicate.Left;
            queryWhereValue = predicate.Right;

            return this.ChildElements.AsQueryable();
        }

        /// <summary>   Clears the predicates. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/13/2020. </remarks>

        public void ClearPredicates()
        {
            queryWhereProperty = null;
            queryWhereValue = null;
        }

        /// <summary>   Gets the root. </summary>
        ///
        /// <value> The root. </value>

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

        /// <summary>   Gets the kind. </summary>
        ///
        /// <value> The kind. </value>

        public override DefinitionKind Kind
        {
            get
            {
                return DefinitionKind.NotApplicable;
            }
        }

        /// <summary>   Gets a value indicating whether this  has children. </summary>
        ///
        /// <value> True if this  has children, false if not. </value>

        public override bool HasChildren
        {
            get 
            {
                return true;
            }
        }

        /// <summary>   Debugger display. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/13/2020. </remarks>
        ///
        /// <param name="p">    A string to process. </param>
        ///
        /// <returns>   An object. </returns>

        private static object DebuggerDisplay(string p)
        {
            throw new NotImplementedException();
        }

        /// <summary>   Gets the types. </summary>
        ///
        /// <value> The types. </value>

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

        /// <summary>   Gets the facets. </summary>
        ///
        /// <value> The facets. </value>

        public override Facet[] Facets
        {
            get
            {
                return null;
            }
        }

        /// <summary>   Gets a list of types of the allowable containers. </summary>
        ///
        /// <value> A list of types of the allowable containers. </value>

        public ContainerType AllowableContainerTypes
        {
            get
            {
                return ContainerType.NotSet;
            }
        }

        /// <summary>   Gets a list of types of the allowable constructs. </summary>
        ///
        /// <value> A list of types of the allowable constructs. </value>

        public ConstructType AllowableConstructTypes
        {
            get
            {
                return ConstructType.NotSet;
            }
        }

        /// <summary>   Gets the default container type. </summary>
        ///
        /// <value> The default container type. </value>

        public ContainerType DefaultContainerType
        {
            get
            {
                return ContainerType.NotSet;
            }
        }

        /// <summary>   Gets the default construct type. </summary>
        ///
        /// <value> The default construct type. </value>

        public ConstructType DefaultConstructType
        {
            get
            {
                return ConstructType.NotSet;
            }
        }

        /// <summary>   Executes the where operation. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/13/2020. </remarks>
        ///
        /// <param name="element">  The element. </param>
        ///
        /// <returns>   An IQueryable. </returns>

        public IQueryable ExecuteWhere(XPathElement element)
        {
            throw new NotImplementedException();
        }

        /// <summary>   Gets the modifiers. </summary>
        ///
        /// <value> The modifiers. </value>

        public override Modifiers Modifiers
        {
            get
            {
                return modifiers;
            }
        }

        /// <summary>   Gets the child nodes. </summary>
        ///
        /// <value> The child nodes. </value>

        public IEnumerable<IBase> ChildNodes => throw new NotImplementedException();

        /// <summary>   Gets the child ordinal. </summary>
        ///
        /// <value> The child ordinal. </value>

        public override float ChildOrdinal { get; }

        /// <summary>   Gets information describing the debug. </summary>
        ///
        /// <value> Information describing the debug. </value>

        public override string DebugInfo { get; }

        /// <summary>   Gets the parent. </summary>
        ///
        /// <value> The parent. </value>

        public override IBase Parent
        {
            get
            {
                return parent;
            }
        }
    }
}
