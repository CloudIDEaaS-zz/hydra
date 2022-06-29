// file:	Models\Assemblies\AssemblyType.cs
//
// summary:	Implements the assembly type class

using System;
using System.Net;
using AbstraX.ServerInterfaces;
using System.Reflection;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using AbstraX.XPathBuilder;
using System.Linq;
using System.Diagnostics;
using AbstraX.TypeMappings;
using System.Collections.Generic;
using System.Linq.Expressions;
using AbstraX;
using AbstraX.AssemblyInterfaces;
using CodeInterfaces.XPathBuilder;
using AbstraX.Models;

namespace AssemblyProvider.Web.Entities
{
    /// <summary>   An assembly type. </summary>
    ///
    /// <remarks>   Ken, 10/30/2020. </remarks>

    public class AssemblyType : BaseObject, IElement, IAssemblyType
    {
        /// <summary>   Type of the base. </summary>
        private System.Type baseType;
        /// <summary>   The parent. </summary>
        private IBase parent;
        /// <summary>   The query where property. </summary>
        private string queryWhereProperty;
        /// <summary>   The query where value. </summary>
        private object queryWhereValue;
        /// <summary>   URL of the image. </summary>
        private string imageURL;
        /// <summary>   The child ordinal. </summary>
        private float childOrdinal;
        /// <summary>   The kind. </summary>
        private DefinitionKind kind;
        /// <summary>   The description. </summary>
        private string description;
        /// <summary>   True if not navigable. </summary>
        private bool notNavigable;

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/30/2020. </remarks>
        ///
        /// <param name="baseType">         Type of the base. </param>
        /// <param name="parent">           The parent. </param>
        /// <param name="genericParent">    (Optional) True to generic parent. </param>

        public AssemblyType(System.Type baseType, BaseObject parent, bool genericParent = false) : base(parent)
        {
            this.baseType = baseType;
            this.parent = parent;

            if (parent is MethodOperation)
            {
                if (genericParent)
                {
                    this.FolderKeyPair = "Return Type Generic Arguments;AssemblyProvider.Web.Images.Folder.png";
                    description = "Method Return Type Generic Argument";

                    notNavigable = true;
                }
                else
                {
                    this.FolderKeyPair = "Return Type;AssemblyProvider.Web.Images.Folder.png";
                    description = "Method Return Type";

                    notNavigable = true;
                }
            }
            else if (parent is Assembly)
            {
                if (baseType.Namespace != null && baseType.Namespace.Length > 0)
                {
                    this.FolderKeyPair = baseType.Namespace + ";AssemblyProvider.Web.Images.Namespace.png";
                }
            }
            else if (parent is AssemblyType)
            {
                if (genericParent)
                {
                    this.FolderKeyPair = "Generic Arguments;AssemblyProvider.Web.Images.Folder.png";
                    description = "Class Generic Argument";

                    notNavigable = true;
                }
                else
                {
                    this.FolderKeyPair = "Base Types;AssemblyProvider.Web.Images.Folder.png";
                    description = "Class Base Type";

                    notNavigable = true;
                }
            }
            else if (parent is FieldElement)
            {
                description = "Field Type";
            }
            else if (parent is GetSetPropertyElement)
            {
                description = "GetSet Property Type";
            }
            else if (parent is PropertyElement)
            {
                description = "Property Type";
            }

            if (baseType.BaseType != null && (baseType.BaseType.FullName == "System.MulticastDelegate" || baseType.BaseType.FullName == "System.Delegate"))
            {
                imageURL = "AssemblyProvider.Web.Images.Delegate.png";
                this.childOrdinal = 5.0f;

                kind = DefinitionKind.Delegate;
            }
            else if (baseType.IsEnum)
            {
                imageURL = "AssemblyProvider.Web.Images.Enum.png";
                this.childOrdinal = 4.0f;

                kind = DefinitionKind.Enumeration;
            }
            else if (baseType.IsInterface)
            {
                imageURL = "AssemblyProvider.Web.Images.Interface.png";
                this.childOrdinal = 3.0f;

                kind = DefinitionKind.Interface;
            }
            else if (baseType.IsClass)
            {
                imageURL = "AssemblyProvider.Web.Images.Struct.png";
                this.childOrdinal = 2.0f;

                kind = DefinitionKind.Class;
            }
            else
            {
                imageURL = string.Empty;
                this.childOrdinal = 1.0f;

                kind = DefinitionKind.Structure;
            }

        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/30/2020. </remarks>
        ///
        /// <param name="baseType"> Type of the base. </param>
        /// <param name="parent">   The parent. </param>

        public AssemblyType(BaseType baseType, BaseObject parent) : base(parent)
        {
            try
            {
                this.baseType = System.Type.GetType(baseType.AssemblyQualifiedName, true);
                this.parent = parent;

                if (parent is MethodOperation)
                {
                    this.FolderKeyPair = "Return Type;AssemblyProvider.Web.Images.Folder.png";
                    description = "Method Return Type";

                    notNavigable = true;
                }
                else if (parent is Assembly)
                {
                    if (baseType.Namespace != null && baseType.Namespace.Length > 0)
                    {
                        this.FolderKeyPair = baseType.Namespace + ";AssemblyProvider.Web.Images.Namespace.png";
                    }
                }
                else if (parent is AssemblyType)
                {
                    this.FolderKeyPair = "Base Types;AssemblyProvider.Web.Images.Folder.png";
                    description = "Class Base Type";

                    notNavigable = true;
                }
                else if (parent is FieldElement)
                {
                    description = "Field Type";
                }
                else if (parent is GetSetPropertyElement)
                {
                    description = "GetSet Property Type";
                }
                else if (parent is PropertyElement)
                {
                    description = "Property Type";
                }

                if (baseType.UnderlyingType.BaseType != null && (baseType.UnderlyingType.BaseType.FullName == "System.MulticastDelegate" || baseType.UnderlyingType.BaseType.FullName == "System.Delegate"))
                {
                    imageURL = "AssemblyProvider.Web.Images.Delegate.png";
                    this.childOrdinal = 5.0f;

                    kind = DefinitionKind.Delegate;
                }
                else if (baseType.UnderlyingType.IsEnum)
                {
                    imageURL = "AssemblyProvider.Web.Images.Enum.png";
                    this.childOrdinal = 4.0f;

                    kind = DefinitionKind.Enumeration;
                }
                else if (baseType.UnderlyingType.IsInterface)
                {
                    imageURL = "AssemblyProvider.Web.Images.Interface.png";
                    this.childOrdinal = 3.0f;

                    kind = DefinitionKind.Interface;
                }
                else if (!baseType.UnderlyingType.IsClass)
                {
                    imageURL = "AssemblyProvider.Web.Images.Struct.png";
                    this.childOrdinal = 2.0f;

                    kind = DefinitionKind.Class;
                }
                else
                {
                    imageURL = string.Empty;
                    this.childOrdinal = 1.0f;

                    kind = DefinitionKind.Structure;
                }
            }
            catch (Exception ex)
            {
                Debugger.Break();
            }

        }


        /// <summary>   Gets the namespace. </summary>
        ///
        /// <value> The namespace. </value>

        public string Namespace
        {
            get
            {
                return baseType.Namespace;
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

        /// <summary>   Gets or sets the identifier. </summary>
        ///
        /// <value> The identifier. </value>

        public override string ID
        {
            get
            {
                return this.MakeID("Type='" + this.baseType.FullName + "'");
            }

            protected set
            {
            }
        }

        /// <summary>   Gets a value indicating whether this  is container. </summary>
        ///
        /// <value> True if this  is container, false if not. </value>

        public bool IsContainer
        {
            get 
            {
                return this.DataType.IsCollectionType;
            }
        }

        /// <summary>   Gets the type of the data. </summary>
        ///
        /// <value> The type of the data. </value>

        public BaseType DataType
        {
            get
            {
                return new BaseType(baseType, this);
            }
        }

        /// <summary>   Gets the field attributes. </summary>
        ///
        /// <value> The field attributes. </value>

        [Association("Type_FieldAttributes", "ID", "ParentID")]
        public List<FieldAttribute> FieldAttributes
        {
            get
            {
                var attributes = new List<FieldAttribute>();

                if (queryWhereProperty != null && queryWhereProperty == "Field")
                {
                    var field = (FieldInfo)baseType.GetFields().Where(p => p.Name == (string)queryWhereValue && p.IsPublic).Single();

                    attributes.Add(new FieldAttribute(field, this));
                }
                else
                {
                    if (this.Kind == DefinitionKind.Enumeration)
                    {
                        foreach (FieldInfo field in baseType.GetFields())
                        {
                            attributes.Add(new FieldAttribute(field, this));
                        }
                    }
                    else
                    {
                        foreach (FieldInfo field in baseType.GetFields().Where(f => f.FieldType.IsPrimitive || AttributeType.Types.Contains(f.FieldType)))
                        {
                            attributes.Add(new FieldAttribute(field, this));
                        }
                    }
                }

                return attributes;
            }
        }

        /// <summary>   Gets the property attributes. </summary>
        ///
        /// <value> The property attributes. </value>

        [Association("Type_PropertyAttributes", "ID", "ParentID")]
        public List<PropertyAttribute> PropertyAttributes
        {
            get
            {
                var attributes = new List<PropertyAttribute>();

                if (queryWhereProperty != null && queryWhereProperty == "Property")
                {
                    var property = baseType.GetProperties().Where(p => p.Name == (string)queryWhereValue).SingleOrDefault();

                    attributes.Add(new PropertyAttribute(property, this));
                }
                else
                {
                    foreach (var property in baseType.GetProperties())
                    {
                        if (property.PropertyType.IsPrimitive || AttributeType.Types.Contains(property.PropertyType))
                        {
                            attributes.Add(new PropertyAttribute(property, this));
                        }
                    }
                }

                return attributes;
            }
        }

        /// <summary>   Gets the attributes. </summary>
        ///
        /// <value> The attributes. </value>

        public IEnumerable<IAttribute> Attributes
        {
            get 
            {
                var attributes = Queryable.Union(this.PropertyAttributes.AsQueryable().Cast<IAttribute>().Select(a => a),
                    this.FieldAttributes.AsQueryable().Cast<IAttribute>().Select (a => a));

                return attributes;
            }
        }

        /// <summary>   Gets a list of types of the types. </summary>
        ///
        /// <value> A list of types of the types. </value>

        public List<AssemblyType> TypeTypes
        {
            get
            {
                var types = new List<AssemblyType>();

                if (queryWhereProperty != null)
                {
                    if (queryWhereProperty == "Type")
                    {
                        if (baseType.BaseType != null && baseType.BaseType.FullName == (string)queryWhereValue)
                        {
                            types.Add(new AssemblyType(baseType.BaseType, this));
                        }

                        if (baseType.GetInterfaces().Any(t => t.FullName == (string)queryWhereValue))
                        {
                            baseType.GetInterfaces().Where(t => t.FullName == (string)queryWhereValue).ToList().ForEach(t => types.Add(new AssemblyType(t, this)));
                        }

                        if (baseType.GetGenericArguments().Any(t => t.FullName == (string)queryWhereValue))
                        {
                            baseType.GetGenericArguments().Where(t => t.FullName == (string)queryWhereValue).ToList().ForEach(t => types.Add(new AssemblyType(t, this, true)));
                        }
                    }
                }
                else
                {
                    if (baseType.BaseType != null)
                    {
                        types.Add(new AssemblyType(baseType.BaseType, this));
                    }

                    baseType.GetInterfaces().ToList().ForEach(t => types.Add(new AssemblyType(t, this)));
                    baseType.GetGenericArguments().ToList().ForEach(t => types.Add(new AssemblyType(t, this, true)));
                }

                return types;
            }
        }

        /// <summary>   Gets the property elements. </summary>
        ///
        /// <value> The property elements. </value>

        public List<PropertyElement> PropertyElements
        {
            get
            {
                var elements = new List<PropertyElement>();

                if (queryWhereProperty != null)
                {
                    if (queryWhereProperty == "Property")
                    {
                        var property = (PropertyInfo)baseType.GetProperties().Where(p => p.Name == (string)queryWhereValue).SingleOrDefault();

                        elements.Add(new PropertyElement(property, this));
                    }
                }
                else
                {
                    foreach (var property in baseType.GetProperties())
                    {
                        if (!property.PropertyType.IsPrimitive && !AttributeType.Types.Contains(property.PropertyType))
                        {
                            elements.Add(new PropertyElement(property, this));
                        }
                    }
                }

                return elements;
            }
        }

        /// <summary>   Gets the child elements. </summary>
        ///
        /// <value> The child elements. </value>

        public IEnumerable<IElement> ChildElements
        {
            get 
            {
                var elements = Queryable.Union(this.PropertyElements.AsQueryable().Cast<IElement>(), 
                    this.TypeTypes.AsQueryable().Cast<IElement>());

                return elements;
            }
        }

        /// <summary>   Gets the methods constructors. </summary>
        ///
        /// <value> The methods constructors. </value>

        public List<Operation> MethodsConstructors
        {
            get
            {
                var methodsConstructors = new List<Operation>();

                if (queryWhereProperty != null && queryWhereProperty == "Method")
                {
                    var method = baseType.GetMethods().Where(m => m.GenerateName() == (string)queryWhereValue).SingleOrDefault();

                    if (method == null)
                    {
                        var constructor = baseType.GetConstructors().Where(m => m.GenerateName() == (string)queryWhereValue).Single();

                        methodsConstructors.Add(new ConstructorOperation(constructor, this));
                    }
                    else
                    {
                        methodsConstructors.Add(new MethodOperation(method, this));
                    }
                }
                else
                {
                    foreach (MethodInfo method in baseType.GetMethods())
                    {
                        if (!method.Name.StartsWith("get_") && !method.Name.StartsWith("set_") && !method.Name.StartsWith("add_") && !method.Name.StartsWith("remove_"))
                        {
                            methodsConstructors.Add(new MethodOperation(method, this));
                        }
                    }

                    foreach (ConstructorInfo constructor in baseType.GetConstructors())
                    {
                        methodsConstructors.Add(new ConstructorOperation(constructor, this));
                    }
                }

                return methodsConstructors;
            }
        }

        /// <summary>   Gets the operations. </summary>
        ///
        /// <value> The operations. </value>

        public IEnumerable<IOperation> Operations
        {
            get
            {
                var operations = this.MethodsConstructors.AsQueryable().Cast<IOperation>().Select(o => o);

                return operations;
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
                var typeName = baseType.GenerateTypeString();

                return typeName;
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
                return imageURL;
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

            if (queryWhereProperty == "Method")
            {
                return this.Operations.AsQueryable();
            }
            else
            {
                return this.ChildElements.AsQueryable();
            }
        }

        /// <summary>   Executes the where operation. </summary>
        ///
        /// <remarks>   Ken, 10/30/2020. </remarks>
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
        /// <remarks>   Ken, 10/30/2020. </remarks>
        ///
        /// <param name="element">  The element. </param>
        ///
        /// <returns>   An IQueryable. </returns>

        public IQueryable ExecuteWhere(XPathAxisElement element)
        {
            var predicate = element.Predicates.First();

            queryWhereProperty = predicate.Left;
            queryWhereValue = predicate.Right;

            return this.ChildElements.AsQueryable();
        }

        /// <summary>   Clears the predicates. </summary>
        ///
        /// <remarks>   Ken, 10/30/2020. </remarks>

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
                return kind;
            }
        }

        /// <summary>   Gets a value indicating whether this  has children. </summary>
        ///
        /// <value> True if this  has children, false if not. </value>

        public override bool HasChildren
        {
            get 
            {
                if (baseType.GetProperties().Any())
                {
                    return true;
                }
                else if (baseType.GetMethods().Any())
                {
                    return true;
                }
                else if (baseType.GetConstructors().Any())
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>   Gets a list of types of the types. </summary>
        ///
        /// <value> A list of types of the types. </value>

        IEnumerable<IAssemblyType> IAssemblyType.TypeTypes
        {
            get
            {
                foreach (var type in this.TypeTypes)
                {
                    yield return type;
                }
            }
        }

        /// <summary>   Gets the field attributes. </summary>
        ///
        /// <value> The field attributes. </value>

        IEnumerable<IFieldAttribute> IAssemblyType.FieldAttributes
        {
            get
            {
                foreach (var attribute in this.FieldAttributes)
                {
                    yield return attribute;
                }
            }
        }

        /// <summary>   Gets the methods constructors. </summary>
        ///
        /// <value> The methods constructors. </value>

        IEnumerable<IOperation> IAssemblyType.MethodsConstructors
        {
            get
            {
                foreach (var methodConstructor in this.MethodsConstructors)
                {
                    yield return methodConstructor;
                }
            }
        }

        /// <summary>   Gets the property attributes. </summary>
        ///
        /// <value> The property attributes. </value>

        IEnumerable<IPropertyAttribute> IAssemblyType.PropertyAttributes
        {
            get
            {
                foreach (var attribute in this.PropertyAttributes)
                {
                    yield return attribute;
                }
            }
        }

        /// <summary>   Gets the property elements. </summary>
        ///
        /// <value> The property elements. </value>

        IEnumerable<IPropertyElement> IAssemblyType.PropertyElements
        {
            get
            {
                foreach (var element in this.PropertyElements)
                {
                    yield return element;
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

        /// <summary>   Gets the type of the system. </summary>
        ///
        /// <value> The type of the system. </value>

        public Type SystemType
        {
            get 
            {
                return baseType;
            }
        }

        /// <summary>   Gets a list of types of the allowable containers. </summary>
        ///
        /// <value> A list of types of the allowable containers. </value>

        public ContainerType AllowableContainerTypes
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>   Gets a list of types of the allowable constructs. </summary>
        ///
        /// <value> A list of types of the allowable constructs. </value>

        public ConstructType AllowableConstructTypes
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>   Gets the default container type. </summary>
        ///
        /// <value> The default container type. </value>

        public ContainerType DefaultContainerType
        {
            get 
            {
                if (baseType == null)
                {
                    return ContainerType.NotSet;
                }
                else
                {
                    return ContainerType.Construct;
                }
            }
        }

        /// <summary>   Gets the default construct type. </summary>
        ///
        /// <value> The default construct type. </value>

        public ConstructType DefaultConstructType
        {
            get 
            {
                if (baseType == null)
                {
                    return ConstructType.NotSet;
                }
                else if (baseType.IsEnum)
                {
                    return ConstructType.Enum;
                }
                else if (baseType.IsClass)
                {
                    return ConstructType.Class;
                }
                else if (baseType.IsInterface)
                {
                    return ConstructType.Interface;
                }
                else if (baseType.IsValueType)
                {
                    return ConstructType.Struct;
                }

                return ConstructType.NotSet;
            }
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

        /// <summary>   Gets the modifiers. </summary>
        ///
        /// <value> The modifiers. </value>

        public override Modifiers Modifiers
        {
            get
            {
                if (notNavigable)
                {
                    return Modifiers.NotNavigable;
                }
                else
                {
                    return Modifiers.Unknown;
                }
            }
        }

        /// <summary>   Gets a list of types of the bases. </summary>
        ///
        /// <value> A list of types of the bases. </value>

        IEnumerable<IAssemblyType> IAssemblyType.BaseTypes
        {
            get 
            {
                var types = new List<AssemblyType>();

                if (baseType.BaseType != null)
                {
                    types.Add(new AssemblyType(baseType.BaseType, this));
                }

                return types;
            }
        }

        /// <summary>   Gets the child nodes. </summary>
        ///
        /// <value> The child nodes. </value>

        public IEnumerable<IBase> ChildNodes => throw new NotImplementedException();
    }
}
