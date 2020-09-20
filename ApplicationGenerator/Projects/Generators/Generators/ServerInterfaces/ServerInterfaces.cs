using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Runtime.Serialization;
using System.Net;
using System.Linq.Expressions;
using AbstraX.XPathBuilder;
using AbstraX.TypeMappings;
using AbstraX;
using System.Reflection;
using Utils;
using System.Configuration;
using AbstraX.AssemblyInterfaces;

namespace AbstraX.ServerInterfaces
{
    public static class DNSHelper
    {
        public static string MakeURL(string path)
        {
            return "AbstraX://" + DNSHelper.Host + "/" + path;
        }

        public static string Host
        {
            get
            {
                return Dns.GetHostName();
            }
        }
    }

    public interface IAbstraXService
    {
    }

    public interface IAbstraXProviderServiceChannel
    {
        string GetRootID();
    }

    public interface IAbstraXProviderService : IAbstraXProviderServiceChannel, IAbstraXService
    {
        byte[] GetImageForFolder(string folderKey);
        byte[] GetImageForItemType(string itemTypeName);
        byte[] GetImageForUrl(string url);
        void LogGenerateByID(string id, MethodInfo method);
        void PostLogGenerateByID();
        string GetQueryMethodForID(string id);
        IBase GenerateByID(string id);
        ContainerType GetAllowableContainerTypes(string id);
        ConstructType GetAllowableConstructTypes(string id);
        void ClearCache();
    }

    public delegate void ChildrenLoadedHandler(IBase parent);

    public enum ProviderType : int
    {
        BusinessModel,
        ViewModel,
        EntityMap,
        AssemblyMap,
        ParsedCode,
        ResourceMap,
        WebService,
        CodeModel,
        ServerModel,
        ODATA,
        JSON,
        ASPX,
        XAML,
        URLDocument,
        HTML,
        XML,
        SQL,
        Custom
    }

    [Flags]
    public enum Modifiers : int
    {
        Unknown = 0,
        CanRead = 1,
        CanWrite = 2,
        IsLocal = 4,
        NotNavigable = 8,
        NotApplicable = 4096
    }

    public enum DefinitionKind : int 
    {
        Unknown = 0,
        Class = 1,
        Delegate = 2,
        Enumeration = 3,
        Interface = 4,
        Structure = 5,
        Constructor = -1,
        Event = -2,
        Field = -3,
        Method = -4,
        Operator = -5,
        Property = -6,
        NotApplicable = 100
    }

    public enum NodeType : int
    {
        Root,
        Element,
        Attribute,
        Operation
    }

    public enum OperationDirection : int
    {
        Incoming,
        Outgoing
    }

    public interface IPathQueryable
    {
        IQueryable ExecuteWhere(string property, object value);
        IQueryable ExecuteWhere(Expression expression);
        IQueryable ExecuteWhere(XPathAxisElement element);
        void ClearPredicates();
    }

    public interface IRoot : IBase, IPathQueryable, IDisposable
    {
        string ParentFieldName { get; }
        ProviderType ProviderType { get; }
        BaseType DataType { get; }
        bool GetItemsOfType<T>(out bool doStandardTraversal, out int traversalDepthLimit, out IEnumerable<T> elements);
        IEnumerable<IElement> RootElements { get; }
        void ExecuteGlobalWhere(XPathAxisElement element);
    }

    public class ItemGeneratedEventArgs<T> : EventArgs where T : IBase
    {
        public T Item { get; set; }

        public ItemGeneratedEventArgs()
        {
        }

        public ItemGeneratedEventArgs(T item)
        {
            this.Item = item;
        }
    }

    public class ExceptionEventArgs : EventArgs
    {
        public Exception Exception { get; set; }

        public ExceptionEventArgs()
        {
        }

        public ExceptionEventArgs(Exception ex)
        {
            this.Exception = ex;
        }
    }

    public delegate void ItemGeneratedHandler<T>(object sender, ItemGeneratedEventArgs<T> e) where T : IBase;
    public delegate void ExceptionHandler(object sender, ExceptionEventArgs e);

    public class ItemGenerationOperation<T> where T : IBase
    {
        public event ItemGeneratedHandler<T> Generated;
        public event ExceptionHandler ExceptionOccured;
        public Exception Exception { get; set; }

        public ItemGenerationOperation()
        {
        }

        public ItemGenerationOperation(Exception ex)
        {
            this.Exception = ex;
        }

        public bool HasException
        {
            get
            {
                return this.Exception != null;
            }
        }

        public void SetGenerated(object sender, T item)
        {
            Generated(sender, new ItemGeneratedEventArgs<T>(item));
        }

        public void SetException(object sender, Exception ex)
        {
            this.Exception = ex;
            ExceptionOccured(sender, new ExceptionEventArgs(ex));
        }
    }


    public class ScalarType : BaseType
    {
        public int TypeCode { get; set; }

        public ScalarType()
        {
        }

        public ScalarType(Type type) : base(type)
        {
            this.TypeCode = (int) type.GetXMLType();
        }

        public ScalarType(BaseType type, IBase baseParent)
        {
            this.ID = baseParent.ID + "/" + type.FullyQualifiedName;
            this.ParentID = baseParent.ID;
            this.FullyQualifiedName = type.FullyQualifiedName;
            this.AssemblyQualifiedName = type.AssemblyQualifiedName;
            this.FriendlyName = type.GenerateTypeString();
            this.Namespace = type.Namespace;
            this.Name = type.Name;
            this.UnderlyingType = type.UnderlyingType;
            this.BaseDataType = type.BaseDataType;
            this.IsInterface = type.IsInterface;
            this.IsEnum = type.IsEnum;
            this.IsArray = type.IsArray;
            this.IsDictionaryGeneric = type.IsDictionaryGeneric;
            this.IsScalarType = type.IsScalarType;
        }

        public ScalarType(BaseType type, BaseType baseParent)
        {
            this.ID = baseParent.ID + "/" + type.FullyQualifiedName;
            this.ParentID = baseParent.ID;
            this.Name = type.Name;
            this.FullyQualifiedName = type.FullyQualifiedName;
            this.AssemblyQualifiedName = type.AssemblyQualifiedName;
            this.SourceParent = baseParent;
        }

        public ScalarType(Type type, BaseType baseParent) : this(type)
        {
            this.ID = baseParent.ID + "/" + type.FullName;
            this.ParentID = baseParent.ID;
            this.SourceParent = baseParent;
        }

        public ScalarType(Type type, IBase baseParent) : this(type)
        {
            this.ID = baseParent.ID + "/" + type.FullName;
            this.ParentID = baseParent.ID;
        }
    }

    public class BaseType
    {
        public string ID { get; set; }
        public string ParentID { get; set; }
        public string Name { get; set;  }
        public string Namespace { get; set; }
        public string FullyQualifiedName { get; set; }
        public string AssemblyQualifiedName { get; set; }
        public string FriendlyName { get; set; }
        public bool IsCollectionType { get; set; }
        public bool IsSystemCollectionType { get; set; }
        public BaseType CollectionType { get; set; }
        public bool IsImageType { get; set; }
        public bool IsAbstract { get; set; }
        public bool IsInterface { get; set; }
        public bool IsEnum { get; set; }
        public string SuggestedNamespace { get; set; }
        public BaseType BaseDataType { get; set; }
        public BaseType[] Interfaces { get; set; }
        public BaseType[] GenericArguments { get; set; }
        public bool IsArray { get; set; }
        public bool IsDictionaryGeneric { get; set; }
        public bool IsScalarType { get; set; }
        public Type UnderlyingType { get; set; }
        public BaseType SourceParent { get; set; }  // to prevent recursion

        public BaseType() 
        {
        }

        public BaseType(BaseType type, IBase baseParent)
        {
            this.ID = baseParent.ID + "/" + type.FullyQualifiedName;
            this.ParentID = baseParent.ID;
            this.FullyQualifiedName = type.FullyQualifiedName;
            this.AssemblyQualifiedName = type.AssemblyQualifiedName;
            this.FriendlyName = type.GenerateTypeString();
            this.Namespace = type.Namespace;
            this.Name = type.Name;
            this.UnderlyingType = type.UnderlyingType;
            this.BaseDataType = type.BaseDataType;
            this.IsInterface = type.IsInterface;
            this.IsEnum = type.IsEnum;
            this.IsArray = type.IsArray;
            this.IsDictionaryGeneric = type.IsDictionaryGeneric;
            this.IsScalarType = type.IsScalarType;
        }

        public BaseType(Type type, string parentID)
        {
            this.ID = parentID + "/" + type.FullName;
            this.ParentID = parentID;
            this.FullyQualifiedName = type.FullName;
            this.AssemblyQualifiedName = type.AssemblyQualifiedName;
            this.FriendlyName = type.GenerateTypeString();
            this.Namespace = type.Namespace;
            this.Name = type.Name;
            this.UnderlyingType = type;
            this.BaseDataType = type.GetBaseDataType();
            this.IsInterface = type.IsInterface;
            this.IsEnum = type.IsEnum;
            this.IsArray = type.IsArray;
            this.IsDictionaryGeneric = type.IsDictionaryGeneric();
            this.IsScalarType = type.IsPrimitive || AttributeType.Types.Contains(type);
        }

        public BaseType(Type type)
        {
            this.FullyQualifiedName = type.FullName;
            this.AssemblyQualifiedName = type.AssemblyQualifiedName;
            this.FriendlyName = type.GenerateTypeString();
            this.Namespace = type.Namespace;
            this.Name = type.Name;
            this.UnderlyingType = type;
            this.BaseDataType = type.GetBaseDataType();
            this.IsInterface = type.IsInterface;
            this.IsEnum = type.IsEnum;
            this.IsArray = type.IsArray;
            this.IsDictionaryGeneric = type.IsDictionaryGeneric();
            this.IsScalarType = type.IsPrimitive || AttributeType.Types.Contains(type);
        }

        public BaseType(Type type, BaseType baseParent, bool minimal = false) : this(type)
        {
            this.ID = baseParent.ID + "/" + type.FullName;
            this.ParentID = baseParent.ID;
            this.SourceParent = baseParent;

            if (!minimal)
            {
                this.Interfaces = this.GetBaseInterfaces();
                this.GenericArguments = this.GetGenericArguments();
            }

            if (type.IsDictionaryGeneric())
            {
                IsCollectionType = true;
                CollectionType = new BaseType(type.GetDictionaryGenericType(), this);

                if (type.Namespace.StartsWith("System.Collections"))
                {
                    IsSystemCollectionType = true;
                }
            }
            else if (type.IsEnumerableGeneric())
            {
                IsCollectionType = true;
                CollectionType = new BaseType(type.GetEnumerableGenericType(), this);

                if (type.Namespace.StartsWith("System.Collections") || type.IsArray)
                {
                    IsSystemCollectionType = true;
                }
            }
        }

        public BaseType(Type type, IBase baseParent, bool minimal = false) : this(type)
        {
            this.ID = baseParent.ID + "/" + type.FullName;
            this.ParentID = baseParent.ID;

            if (!minimal)
            {
                this.Interfaces = this.GetBaseInterfaces();
                this.GenericArguments = this.GetGenericArguments();
            }

            if (type.IsDictionaryGeneric())
            {
                IsCollectionType = true;
                CollectionType = new BaseType(type.GetDictionaryGenericType(), this);

                if (type.Namespace.StartsWith("System.Collections"))
                {
                    IsSystemCollectionType = true;
                }
            }
            else if (type.IsEnumerableGeneric())
            {
                IsCollectionType = true;
                CollectionType = new BaseType(type.GetEnumerableGenericType(), this);

                if (type.Namespace.StartsWith("System.Collections") || type.IsArray)
                {
                    IsSystemCollectionType = true;
                }
            }
        }
    }

    public class Argument
    {
        public string ID { get; set; }
        public string ParentID { get; set; }
        public string Value { get; set; }
    }

    public class Facet
    {
        public string ID { get; set; }
        public string ParentID { get; set; }
        public BaseType FacetType { get; set; }
        public Argument[] Arguments { get; set; }
        public string AttributeCode
        {
            get
            {
                return FacetType.Name + "(" + Arguments.Select(a => a.Value).ToCommaDelimitedList() + ")";
            }
        }
    }

    public interface IParentBase : IBase, IPathQueryable
    {
        IEnumerable<IElement> ChildElements { get; }
        IRoot Root { get; }
    }

    public interface IBase 
    {
        string ID { get; }
        string ParentID { get; }
        string Name { get; }
        string DesignComments { get; }
        string Documentation { get; }
        string DocumentationSummary { get; }
        bool HasDocumentation { get; }
        float ChildOrdinal { get; }
        string DebugInfo { get; }
        IBase Parent { get; }
        string FolderKeyPair { get; set; }
        DefinitionKind Kind { get; }
        Modifiers Modifiers { get; }
        bool HasChildren { get; }
        Facet[] Facets { get; }
    }

    public interface IAttribute : IParentBase 
    {
        ScalarType DataType { get; }
        string DefaultValue { get; }
    }

    public interface IDbAttribute : IAttribute
    {
        string DatabaseID { get; }
        string RelatedID { get; }
    }

    public interface IDbElement : IElement
    {
        string DatabaseID { get; }
        string RelatedID { get; }
    }

    public interface IDbOperation : IOperation
    {
        string DatabaseID { get; }
        string RelatedID { get; }
    }

    public interface IOperation : IParentBase
    {
        OperationDirection Direction { get; }
        BaseType ReturnType { get; }
    }

    public interface IElement : IParentBase 
    {
        bool IsContainer { get; }
        BaseType DataType { get; }
        IEnumerable<IAttribute> Attributes { get; }
        IEnumerable<IOperation> Operations { get; }
        ContainerType DefaultContainerType { get; }
        ConstructType DefaultConstructType { get; }
        ContainerType AllowableContainerTypes { get; }
        ConstructType AllowableConstructTypes { get; }
    }

    public interface ISurrogateElement : IElement
    {
        IElement SurrogateSource { get; }
        IElement ReferencedFrom { get; }
    }
}
