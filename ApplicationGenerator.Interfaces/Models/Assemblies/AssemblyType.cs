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
    public class AssemblyType : BaseObject, IElement, IAssemblyType
    {
        private System.Type baseType;
        private IBase parent;
        private string queryWhereProperty;
        private object queryWhereValue;
        private string imageURL;
        private float childOrdinal;
        private DefinitionKind kind;
        private string description;
        private bool notNavigable;

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

        public bool IsContainer
        {
            get 
            {
                return this.DataType.IsCollectionType;
            }
        }

        public BaseType DataType
        {
            get
            {
                return new BaseType(baseType, this);
            }
        }

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

        public IEnumerable<IAttribute> Attributes
        {
            get 
            {
                var attributes = Queryable.Union(this.PropertyAttributes.AsQueryable().Cast<IAttribute>().Select(a => a),
                    this.FieldAttributes.AsQueryable().Cast<IAttribute>().Select (a => a));

                return attributes;
            }
        }

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

        public IEnumerable<IElement> ChildElements
        {
            get 
            {
                var elements = Queryable.Union(this.PropertyElements.AsQueryable().Cast<IElement>(), 
                    this.TypeTypes.AsQueryable().Cast<IElement>());

                return elements;
            }
        }

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

        public IEnumerable<IOperation> Operations
        {
            get
            {
                var operations = this.MethodsConstructors.AsQueryable().Cast<IOperation>().Select(o => o);

                return operations;
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
                var typeName = baseType.GenerateTypeString();

                return typeName;
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

            if (queryWhereProperty == "Method")
            {
                return this.Operations.AsQueryable();
            }
            else
            {
                return this.ChildElements.AsQueryable();
            }
        }

        public IQueryable ExecuteWhere(Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable ExecuteWhere(XPathAxisElement element)
        {
            var predicate = element.Predicates.First();

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
                return kind;
            }
        }

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

        public override Facet[] Facets
        {
            get
            {
                return null;
            }
        }

        public Type SystemType
        {
            get 
            {
                return baseType;
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

        public IQueryable ExecuteWhere(XPathElement element)
        {
            throw new NotImplementedException();
        }

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

        public IEnumerable<IBase> ChildNodes => throw new NotImplementedException();
    }
}
