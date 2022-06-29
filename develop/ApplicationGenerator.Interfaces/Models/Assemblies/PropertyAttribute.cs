// file:	Models\Assemblies\PropertyAttribute.cs
//
// summary:	Implements the property attribute class

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
    /// <summary>   Attribute for property. </summary>
    ///
    /// <remarks>   Ken, 10/30/2020. </remarks>

    public class PropertyAttribute : BaseObject, IAttribute, IPropertyAttribute
    {
        /// <summary>   The property. </summary>
        private PropertyInfo property;
        /// <summary>   The parent. </summary>
        protected IBase parent;
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
        protected ScalarType dataType;
        /// <summary>   The name. </summary>
        protected string name;
        /// <summary>   The modifiers. </summary>
        protected Modifiers modifiers;
        /// <summary>   The identifier. </summary>
        protected string id;

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/30/2020. </remarks>
        ///
        /// <param name="parent">   The parent. </param>

        public PropertyAttribute(BaseObject parent) : base(parent)
        {
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/30/2020. </remarks>
        ///
        /// <param name="property"> The property. </param>
        /// <param name="parent">   The parent. </param>

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

        /// <summary>   Gets the child ordinal. </summary>
        ///
        /// <value> The child ordinal. </value>

        public override float ChildOrdinal
        {
            get
            {
                return childOrdinal;
            }
        }

        /// <summary>   Gets information describing the debug. </summary>
        ///
        /// <value> Information describing the debug. </value>

        public override string DebugInfo
        {
            get
            {
                return this.GetDebugInfo();
            }
        }

        /// <summary>   Gets the type of the data. </summary>
        ///
        /// <value> The type of the data. </value>

        public ScalarType DataType
        {
            get
            {
                return dataType;
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
                return documentation;
            }
        }

        /// <summary>   Gets a value indicating whether this  has documentation. </summary>
        ///
        /// <value> True if this  has documentation, false if not. </value>

        public override bool HasDocumentation
        {
            get
            {
                return hasDocumentation;
            }
        }

        /// <summary>   Gets the documentation summary. </summary>
        ///
        /// <value> The documentation summary. </value>

        public override string DocumentationSummary
        {
            get
            {
                return documentationSummary;
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

        /// <summary>   Gets or sets the folder key pair. </summary>
        ///
        /// <value> The folder key pair. </value>

        public override string FolderKeyPair { get; set; }

        /// <summary>   Executes the where operation. </summary>
        ///
        /// <remarks>   Ken, 10/30/2020. </remarks>
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
        /// <remarks>   Ken, 10/30/2020. </remarks>
        ///
        /// <param name="expression">   The expression. </param>
        ///
        /// <returns>   An IQueryable. </returns>

        public IQueryable ExecuteWhere(System.Linq.Expressions.Expression expression)
        {
            throw new NotImplementedException();
        }

        /// <summary>   Executes the where operation. </summary>
        ///
        /// <remarks>   Ken, 10/30/2020. </remarks>
        ///
        /// <param name="element">  The element. </param>
        ///
        /// <returns>   An IQueryable. </returns>

        public IQueryable ExecuteWhere(XPathAxisElement element)
        {
            throw new NotImplementedException();
        }

        /// <summary>   Clears the predicates. </summary>
        ///
        /// <remarks>   Ken, 10/30/2020. </remarks>

        public void ClearPredicates()
        {
            queryWhereProperty = null;
            queryWhereValue = null;
        }

        /// <summary>   Executes the where operation. </summary>
        ///
        /// <remarks>   Ken, 10/30/2020. </remarks>
        ///
        /// <param name="element">  The element. </param>
        ///
        /// <returns>   An IQueryable. </returns>

        public IQueryable ExecuteWhere(XPathElement element)
        {
            throw new NotImplementedException();
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
                return DefinitionKind.SimpleProperty;
            }
        }

        /// <summary>   Gets the child elements. </summary>
        ///
        /// <value> The child elements. </value>

        public System.Collections.Generic.IEnumerable<IElement> ChildElements
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>   Gets a value indicating whether this  has children. </summary>
        ///
        /// <value> True if this  has children, false if not. </value>

        public override bool HasChildren
        {
            get 
            {
                return false;
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

        /// <summary>   Gets the default value. </summary>
        ///
        /// <value> The default value. </value>

        public string DefaultValue
        {
            get
            {
                return null;
            }
        }

        /// <summary>   Gets a value indicating whether this  is nullable. </summary>
        ///
        /// <value> True if nullable, false if not. </value>

        public bool Nullable => throw new NotImplementedException();

        /// <summary>   Gets the child nodes. </summary>
        ///
        /// <value> The child nodes. </value>

        public IEnumerable<IBase> ChildNodes => throw new NotImplementedException();
    }
}
