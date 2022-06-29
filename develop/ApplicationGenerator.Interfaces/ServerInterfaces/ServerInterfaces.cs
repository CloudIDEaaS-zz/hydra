// file:	ServerInterfaces\ServerInterfaces.cs
//
// summary:	Implements the server interfaces class

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
using System.Diagnostics;

namespace AbstraX.ServerInterfaces
{
    /// <summary>   The DNS helper. </summary>
    ///
    /// <remarks>   Ken, 10/8/2020. </remarks>

    public static class DNSHelper
    {
        /// <summary>   Makes an URL. </summary>
        ///
        /// <remarks>   Ken, 10/8/2020. </remarks>
        ///
        /// <param name="path"> Full pathname of the file. </param>
        ///
        /// <returns>   A string. </returns>

        public static string MakeURL(string path)
        {
            return "AbstraX://" + DNSHelper.Host + "/" + path;
        }

        /// <summary>   Gets the host. </summary>
        ///
        /// <value> The host. </value>

        public static string Host
        {
            get
            {
                return Dns.GetHostName();
            }
        }
    }

    /// <summary>   Interface for abstra x coordinate provider service channel. </summary>
    ///
    /// <remarks>   Ken, 10/8/2020. </remarks>

    public interface IAbstraXProviderServiceChannel
    {
        /// <summary>   Gets root identifier. </summary>
        ///
        /// <returns>   The root identifier. </returns>

        string GetRootID();
    }

    /// <summary>   Interface for abstra x coordinate provider service. </summary>
    ///
    /// <remarks>   Ken, 10/8/2020. </remarks>

    public interface IAbstraXProviderService : IAbstraXProviderServiceChannel
    {
        /// <summary>   Gets image for folder. </summary>
        ///
        /// <param name="folderKey">    The folder key. </param>
        ///
        /// <returns>   An array of byte. </returns>

        byte[] GetImageForFolder(string folderKey);

        /// <summary>   Gets image for item type. </summary>
        ///
        /// <param name="itemTypeName"> Name of the item type. </param>
        ///
        /// <returns>   An array of byte. </returns>

        byte[] GetImageForItemType(string itemTypeName);

        /// <summary>   Gets image for URL. </summary>
        ///
        /// <param name="url">  URL of the resource. </param>
        ///
        /// <returns>   An array of byte. </returns>

        byte[] GetImageForUrl(string url);

        /// <summary>   Logs generate by identifier. </summary>
        ///
        /// <param name="id">       The identifier. </param>
        /// <param name="method">   The method. </param>

        void LogGenerateByID(string id, MethodInfo method);
        /// <summary>   Posts the log generate by identifier. </summary>
        void PostLogGenerateByID();

        /// <summary>   Gets query method for identifier. </summary>
        ///
        /// <param name="id">   The identifier. </param>
        ///
        /// <returns>   The query method for identifier. </returns>

        string GetQueryMethodForID(string id);

        /// <summary>   Generates a by identifier. </summary>
        ///
        /// <param name="id">   The identifier. </param>
        ///
        /// <returns>   The by identifier. </returns>

        IBase GenerateByID(string id);

        /// <summary>   Gets allowable container types. </summary>
        ///
        /// <param name="id">   The identifier. </param>
        ///
        /// <returns>   The allowable container types. </returns>

        ContainerType GetAllowableContainerTypes(string id);

        /// <summary>   Gets allowable construct types. </summary>
        ///
        /// <param name="id">   The identifier. </param>
        ///
        /// <returns>   The allowable construct types. </returns>

        ConstructType GetAllowableConstructTypes(string id);
        /// <summary>   Clears the cache. </summary>
        void ClearCache();
    }

    /// <summary>   Handler, called when the children loaded. </summary>
    ///
    /// <remarks>   Ken, 10/8/2020. </remarks>
    ///
    /// <param name="parent">   The parent. </param>

    public delegate void ChildrenLoadedHandler(IBase parent);

    /// <summary>   Values that represent provider types. </summary>
    ///
    /// <remarks>   Ken, 10/8/2020. </remarks>

    public enum ProviderType : int
    {
        /// <summary>   An enum constant representing the business model option. </summary>
        BusinessModel,
        /// <summary>   An enum constant representing the view model option. </summary>
        ViewModel,
        /// <summary>   An enum constant representing the entity map option. </summary>
        EntityMap,
        /// <summary>   An enum constant representing the assembly map option. </summary>
        AssemblyMap,
        /// <summary>   An enum constant representing the parsed code option. </summary>
        ParsedCode,
        /// <summary>   An enum constant representing the resource map option. </summary>
        ResourceMap,
        /// <summary>   An enum constant representing the web service option. </summary>
        WebService,
        /// <summary>   An enum constant representing the code model option. </summary>
        CodeModel,
        /// <summary>   An enum constant representing the server model option. </summary>
        ServerModel,
        /// <summary>   An enum constant representing the odata option. </summary>
        ODATA,
        /// <summary>   An enum constant representing the JSON option. </summary>
        JSON,
        /// <summary>   An enum constant representing the aspx option. </summary>
        ASPX,
        /// <summary>   An enum constant representing the XAML option. </summary>
        XAML,
        /// <summary>   An enum constant representing the URL document option. </summary>
        URLDocument,
        /// <summary>   An enum constant representing the HTML option. </summary>
        HTML,
        /// <summary>   An enum constant representing the XML option. </summary>
        XML,
        /// <summary>   An enum constant representing the SQL option. </summary>
        SQL,
        /// <summary>   An enum constant representing the custom option. </summary>
        Custom
    }

    /// <summary>   A bit-field of flags for specifying modifiers. </summary>
    ///
    /// <remarks>   Ken, 10/8/2020. </remarks>

    [Flags]
    public enum Modifiers : int
    {
        /// <summary>   A binary constant representing the unknown flag. </summary>
        Unknown = 0,
        /// <summary>   A binary constant representing the can read flag. </summary>
        CanRead = 1,
        /// <summary>   A binary constant representing the can write flag. </summary>
        CanWrite = 2,
        /// <summary>   A binary constant representing the is local flag. </summary>
        IsLocal = 4,
        /// <summary>   A binary constant representing the not navigable flag. </summary>
        NotNavigable = 8,
        /// <summary>   A binary constant representing the not applicable flag. </summary>
        NotApplicable = 4096
    }

    /// <summary>   Values that represent definition kinds. </summary>
    ///
    /// <remarks>   Ken, 10/8/2020. </remarks>

    public enum DefinitionKind : int 
    {
/// <summary>   . </summary>
        Unknown = 0,
/// <summary>   . </summary>
        Class = 1,
/// <summary>   . </summary>
        Delegate = 2,
/// <summary>   . </summary>
        Enumeration = 3,
/// <summary>   . </summary>
        Interface = 4,
/// <summary>   . </summary>
        Structure = 5,
/// <summary>   . </summary>
        Constructor = -1,
        /// <summary>   Event queue for all listeners interested in Event events. </summary>
        Event = -2,
        /// <summary>   An enum constant representing the field option. </summary>
        Field = -3,
        /// <summary>   An enum constant representing the method option. </summary>
        Method = -4,
        /// <summary>   An enum constant representing the operator option. </summary>
        Operator = -5,
        /// <summary>   An enum constant representing the simple property option. </summary>
        SimpleProperty = -6,
        /// <summary>   An enum constant representing the complex property option. </summary>
        ComplexProperty = -7,
        /// <summary>   An enum constant representing the complex set property option. </summary>
        ComplexSetProperty = -8,
        /// <summary>   An enum constant representing the static container option. </summary>
        StaticContainer = 100,
        /// <summary>   An enum constant representing the model option. </summary>
        Model = 101,
        /// <summary>   An enum constant representing the not applicable option. </summary>
        NotApplicable = 102
    }

    /// <summary>   Values that represent node types. </summary>
    ///
    /// <remarks>   Ken, 10/8/2020. </remarks>

    public enum NodeType : int
    {
        /// <summary>   An enum constant representing the root option. </summary>
        Root,
        /// <summary>   An enum constant representing the element option. </summary>
        Element,
        /// <summary>   An enum constant representing the attribute option. </summary>
        Attribute,
        /// <summary>   An enum constant representing the operation option. </summary>
        Operation
    }

    /// <summary>   Values that represent operation directions. </summary>
    ///
    /// <remarks>   Ken, 10/8/2020. </remarks>

    public enum OperationDirection : int
    {
        /// <summary>   An enum constant representing the incoming option. </summary>
        Incoming,
        /// <summary>   An enum constant representing the outgoing option. </summary>
        Outgoing
    }

    /// <summary>   Interface for path queryable. </summary>
    ///
    /// <remarks>   Ken, 10/8/2020. </remarks>

    public interface IPathQueryable
    {
        /// <summary>   Executes the where operation. </summary>
        ///
        /// <param name="property"> The property. </param>
        /// <param name="value">    The value. </param>
        ///
        /// <returns>   An IQueryable. </returns>

        IQueryable ExecuteWhere(string property, object value);

        /// <summary>   Executes the where operation. </summary>
        ///
        /// <param name="expression">   The expression. </param>
        ///
        /// <returns>   An IQueryable. </returns>

        IQueryable ExecuteWhere(Expression expression);

        /// <summary>   Executes the where operation. </summary>
        ///
        /// <param name="element">  The element. </param>
        ///
        /// <returns>   An IQueryable. </returns>

        IQueryable ExecuteWhere(XPathElement element);
        /// <summary>   Clears the predicates. </summary>
        void ClearPredicates();
    }

    /// <summary>   Interface for root. </summary>
    ///
    /// <remarks>   Ken, 10/8/2020. </remarks>

    public interface IRoot : IBase, IPathQueryable, IDisposable
    {
        /// <summary>   Gets or sets the application generator engine. </summary>
        ///
        /// <value> The application generator engine. </value>

        IAppGeneratorEngine AppGeneratorEngine { get; set; }

        /// <summary>   Gets the name of the parent field. </summary>
        ///
        /// <value> The name of the parent field. </value>

        string ParentFieldName { get; }

        /// <summary>   Gets the type of the provider. </summary>
        ///
        /// <value> The type of the provider. </value>

        ProviderType ProviderType { get; }

        /// <summary>   Gets the type of the data. </summary>
        ///
        /// <value> The type of the data. </value>

        BaseType DataType { get; }

        /// <summary>   Gets items of type. </summary>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="doStandardTraversal">  [out] True to do standard traversal. </param>
        /// <param name="traversalDepthLimit">  [out] The traversal depth limit. </param>
        /// <param name="elements">             [out] The elements. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        bool GetItemsOfType<T>(out bool doStandardTraversal, out int traversalDepthLimit, out IEnumerable<T> elements);

        /// <summary>   Gets the root elements. </summary>
        ///
        /// <value> The root elements. </value>

        IEnumerable<IElement> RootElements { get; }

        /// <summary>   Executes the global where operation. </summary>
        ///
        /// <param name="element">  The element. </param>

        void ExecuteGlobalWhere(XPathElement element);
    }

    /// <summary>   Additional information for item generated events. </summary>
    ///
    /// <remarks>   Ken, 10/8/2020. </remarks>
    ///
    /// <typeparam name="T">    Generic type parameter. </typeparam>

    public class ItemGeneratedEventArgs<T> : EventArgs where T : IBase
    {
        /// <summary>   Gets or sets the item. </summary>
        ///
        /// <value> The item. </value>

        public T Item { get; set; }

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   Ken, 10/8/2020. </remarks>

        public ItemGeneratedEventArgs()
        {
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/8/2020. </remarks>
        ///
        /// <param name="item"> The item. </param>

        public ItemGeneratedEventArgs(T item)
        {
            this.Item = item;
        }
    }

    /// <summary>   Additional information for exception events. </summary>
    ///
    /// <remarks>   Ken, 10/8/2020. </remarks>

    public class ExceptionEventArgs : EventArgs
    {
        /// <summary>   Gets or sets the exception. </summary>
        ///
        /// <value> The exception. </value>

        public Exception Exception { get; set; }

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   Ken, 10/8/2020. </remarks>

        public ExceptionEventArgs()
        {
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/8/2020. </remarks>
        ///
        /// <param name="ex">   The exception. </param>

        public ExceptionEventArgs(Exception ex)
        {
            this.Exception = ex;
        }
    }

    /// <summary>   Handler, called when the item generated. </summary>
    ///
    /// <remarks>   Ken, 10/8/2020. </remarks>
    ///
    /// <typeparam name="T">    Generic type parameter. </typeparam>
    /// <param name="sender">   Source of the event. </param>
    /// <param name="e">        An ItemGeneratedEventArgs&lt;T&gt; to process. </param>

    public delegate void ItemGeneratedHandler<T>(object sender, ItemGeneratedEventArgs<T> e) where T : IBase;

    /// <summary>   Handler, called when the exception. </summary>
    ///
    /// <remarks>   Ken, 10/8/2020. </remarks>
    ///
    /// <param name="sender">   Source of the event. </param>
    /// <param name="e">        Exception event information. </param>

    public delegate void ExceptionHandler(object sender, ExceptionEventArgs e);

    /// <summary>   An item generation operation. </summary>
    ///
    /// <remarks>   Ken, 10/8/2020. </remarks>
    ///
    /// <typeparam name="T">    Generic type parameter. </typeparam>

    public class ItemGenerationOperation<T> where T : IBase
    {
        /// <summary>   Event queue for all listeners interested in Generated events. </summary>
        public event ItemGeneratedHandler<T> Generated;
        /// <summary>   Event queue for all listeners interested in ExceptionOccured events. </summary>
        public event ExceptionHandler ExceptionOccured;

        /// <summary>   Gets or sets the exception. </summary>
        ///
        /// <value> The exception. </value>

        public Exception Exception { get; set; }

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   Ken, 10/8/2020. </remarks>

        public ItemGenerationOperation()
        {
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/8/2020. </remarks>
        ///
        /// <param name="ex">   The exception. </param>

        public ItemGenerationOperation(Exception ex)
        {
            this.Exception = ex;
        }

        /// <summary>   Gets a value indicating whether this  has exception. </summary>
        ///
        /// <value> True if this  has exception, false if not. </value>

        public bool HasException
        {
            get
            {
                return this.Exception != null;
            }
        }

        /// <summary>   Sets a generated. </summary>
        ///
        /// <remarks>   Ken, 10/8/2020. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="item">     The item. </param>

        public void SetGenerated(object sender, T item)
        {
            Generated(sender, new ItemGeneratedEventArgs<T>(item));
        }

        /// <summary>   Sets an exception. </summary>
        ///
        /// <remarks>   Ken, 10/8/2020. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="ex">       The exception. </param>

        public void SetException(object sender, Exception ex)
        {
            this.Exception = ex;
            ExceptionOccured(sender, new ExceptionEventArgs(ex));
        }
    }

    /// <summary>   A scalar type. </summary>
    ///
    /// <remarks>   Ken, 10/8/2020. </remarks>

    public class ScalarType : BaseType
    {
        /// <summary>   Gets or sets the type code. </summary>
        ///
        /// <value> The type code. </value>

        public int TypeCode { get; set; }

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   Ken, 10/8/2020. </remarks>

        public ScalarType()
        {
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/8/2020. </remarks>
        ///
        /// <param name="type"> The type. </param>

        public ScalarType(string type)
        {
            this.TypeCode = (int)TypeMapper.GetXMLType(type);
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/8/2020. </remarks>
        ///
        /// <param name="type"> The type. </param>

        public ScalarType(Type type) : base(type)
        {
            this.TypeCode = (int) type.GetXMLType();
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/8/2020. </remarks>
        ///
        /// <param name="typeName">     Name of the type. </param>
        /// <param name="baseParent">   The base parent. </param>

        public ScalarType(string typeName, IBase baseParent) : this(typeName)
        {
            
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/8/2020. </remarks>
        ///
        /// <param name="type">         The type. </param>
        /// <param name="baseParent">   The base parent. </param>

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

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/8/2020. </remarks>
        ///
        /// <param name="type">         The type. </param>
        /// <param name="baseParent">   The base parent. </param>

        public ScalarType(BaseType type, BaseType baseParent)
        {
            this.ID = baseParent.ID + "/" + type.FullyQualifiedName;
            this.ParentID = baseParent.ID;
            this.Name = type.Name;
            this.FullyQualifiedName = type.FullyQualifiedName;
            this.AssemblyQualifiedName = type.AssemblyQualifiedName;
            this.SourceParent = baseParent;
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/8/2020. </remarks>
        ///
        /// <param name="type">         The type. </param>
        /// <param name="baseParent">   The base parent. </param>

        public ScalarType(Type type, BaseType baseParent) : this(type)
        {
            this.ID = baseParent.ID + "/" + type.FullName;
            this.ParentID = baseParent.ID;
            this.SourceParent = baseParent;
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/8/2020. </remarks>
        ///
        /// <param name="type">         The type. </param>
        /// <param name="baseParent">   The base parent. </param>

        public ScalarType(Type type, IBase baseParent) : this(type)
        {
            this.ID = baseParent.ID + "/" + type.FullName;
            this.ParentID = baseParent.ID;
        }
    }

    /// <summary>   A base type. </summary>
    ///
    /// <remarks>   Ken, 10/8/2020. </remarks>

    public class BaseType
    {
        /// <summary>   Gets or sets the identifier. </summary>
        ///
        /// <value> The identifier. </value>

        public string ID { get; set; }

        /// <summary>   Gets or sets the identifier of the parent. </summary>
        ///
        /// <value> The identifier of the parent. </value>

        public string ParentID { get; set; }

        /// <summary>   Gets or sets the name. </summary>
        ///
        /// <value> The name. </value>

        public string Name { get; set;  }

        /// <summary>   Gets or sets the namespace. </summary>
        ///
        /// <value> The namespace. </value>

        public string Namespace { get; set; }

        /// <summary>   Gets or sets the name of the fully qualified. </summary>
        ///
        /// <value> The name of the fully qualified. </value>

        public string FullyQualifiedName { get; set; }

        /// <summary>   Gets or sets the name of the assembly qualified. </summary>
        ///
        /// <value> The name of the assembly qualified. </value>

        public string AssemblyQualifiedName { get; set; }

        /// <summary>   Gets or sets the name of the friendly. </summary>
        ///
        /// <value> The name of the friendly. </value>

        public string FriendlyName { get; set; }

        /// <summary>   Gets or sets a value indicating whether this  is collection type. </summary>
        ///
        /// <value> True if this  is collection type, false if not. </value>

        public bool IsCollectionType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this  is system collection type.
        /// </summary>
        ///
        /// <value> True if this  is system collection type, false if not. </value>

        public bool IsSystemCollectionType { get; set; }

        /// <summary>   Gets or sets the type of the collection. </summary>
        ///
        /// <value> The type of the collection. </value>

        public BaseType CollectionType { get; set; }

        /// <summary>   Gets or sets a value indicating whether this  is image type. </summary>
        ///
        /// <value> True if this  is image type, false if not. </value>

        public bool IsImageType { get; set; }

        /// <summary>   Gets or sets a value indicating whether this  is abstract. </summary>
        ///
        /// <value> True if this  is abstract, false if not. </value>

        public bool IsAbstract { get; set; }

        /// <summary>   Gets or sets a value indicating whether this  is interface. </summary>
        ///
        /// <value> True if this  is interface, false if not. </value>

        public bool IsInterface { get; set; }

        /// <summary>   Gets or sets a value indicating whether this  is enum. </summary>
        ///
        /// <value> True if this  is enum, false if not. </value>

        public bool IsEnum { get; set; }

        /// <summary>   Gets or sets the suggested namespace. </summary>
        ///
        /// <value> The suggested namespace. </value>

        public string SuggestedNamespace { get; set; }

        /// <summary>   Gets or sets the type of the base data. </summary>
        ///
        /// <value> The type of the base data. </value>

        public BaseType BaseDataType { get; set; }

        /// <summary>   Gets or sets the interfaces. </summary>
        ///
        /// <value> The interfaces. </value>

        public BaseType[] Interfaces { get; set; }

        /// <summary>   Gets or sets the generic arguments. </summary>
        ///
        /// <value> The generic arguments. </value>

        public BaseType[] GenericArguments { get; set; }

        /// <summary>   Gets or sets a value indicating whether this  is array. </summary>
        ///
        /// <value> True if this  is array, false if not. </value>

        public bool IsArray { get; set; }

        /// <summary>   Gets or sets a value indicating whether this  is dictionary generic. </summary>
        ///
        /// <value> True if this  is dictionary generic, false if not. </value>

        public bool IsDictionaryGeneric { get; set; }

        /// <summary>   Gets or sets a value indicating whether this  is scalar type. </summary>
        ///
        /// <value> True if this  is scalar type, false if not. </value>

        public bool IsScalarType { get; set; }

        /// <summary>   Gets or sets the type of the underlying. </summary>
        ///
        /// <value> The type of the underlying. </value>

        public Type UnderlyingType { get; set; }

        /// <summary>   Gets or sets source parent. </summary>
        ///
        /// <value> The source parent. </value>

        public BaseType SourceParent { get; set; }  // to prevent recursion

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   Ken, 10/8/2020. </remarks>

        public BaseType() 
        {
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/8/2020. </remarks>
        ///
        /// <param name="type">         The type. </param>
        /// <param name="baseParent">   The base parent. </param>

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

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/8/2020. </remarks>
        ///
        /// <param name="type">     The type. </param>
        /// <param name="parentID"> Identifier for the parent. </param>

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

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/8/2020. </remarks>
        ///
        /// <param name="type"> The type. </param>

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

            try
            {
                this.IsEnum = type.IsEnum;
            }
            catch (Exception ex)
            {
            }

            this.IsDictionaryGeneric = type.IsDictionaryGeneric();

            this.IsArray = type.IsArray;
            this.IsScalarType = type.IsPrimitive || AttributeType.Types.Contains(type);
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/8/2020. </remarks>
        ///
        /// <param name="type">         The type. </param>
        /// <param name="baseParent">   The base parent. </param>
        /// <param name="minimal">      (Optional) True to minimal. </param>

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

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/8/2020. </remarks>
        ///
        /// <param name="type">         The type. </param>
        /// <param name="baseParent">   The base parent. </param>
        /// <param name="minimal">      (Optional) True to minimal. </param>

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

    /// <summary>   An argument. </summary>
    ///
    /// <remarks>   Ken, 10/8/2020. </remarks>

    [DebuggerDisplay(" { DebugInfo } ")]
    public class Argument
    {
        /// <summary>   Gets or sets the identifier. </summary>
        ///
        /// <value> The identifier. </value>

        public string ID { get; set; }

        /// <summary>   Gets or sets the identifier of the parent. </summary>
        ///
        /// <value> The identifier of the parent. </value>

        public string ParentID { get; set; }

        /// <summary>   Gets or sets the name. </summary>
        ///
        /// <value> The name. </value>

        public string Name { get; set; }

        /// <summary>   Gets or sets the value. </summary>
        ///
        /// <value> The value. </value>

        public string Value { get; set; }

        /// <summary>   Gets or sets the type of the argument. </summary>
        ///
        /// <value> The type of the argument. </value>

        public BaseType ArgumentType { get; set; }

        /// <summary>   Gets or sets the raw value. </summary>
        ///
        /// <value> The raw value. </value>

        public object RawValue { get; set; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/8/2020. </remarks>
        ///
        /// <param name="name">     The name. </param>
        /// <param name="value">    The value. </param>

        public Argument(string name, object value)
        {
            this.Name = name;
            this.RawValue = value;

            if (value != null)
            {
                var valueType = value.GetType();

                this.Value = value.ToString();

                this.ArgumentType = new BaseType
                {
                    FullyQualifiedName = valueType.FullName,
                    Name = valueType.Name,
                    ID = valueType.Name,
                    IsEnum = valueType.IsEnum,
                    ParentID = this.ID
                };
            }
        }

        /// <summary>   Gets the argument code. </summary>
        ///
        /// <value> The argument code. </value>

        public string ArgumentCode
        {
            get
            {
                if (this.ArgumentType?.Name == "String" || this.ArgumentType?.Name == "Guid")
                {
                    return this.Value.AsDisplayText(QuoteTextType.DoubleQuote);
                }
                else if (this.ArgumentType?.Name == "Boolean")
                {
                    return this.Value.AsDisplayText().ToLower();
                }
                else if ((this.ArgumentType?.IsEnum).HasValue && (this.ArgumentType?.IsEnum).Value)
                {
                    return this.ArgumentType.Name + "." +  this.Value.AsDisplayText();
                }
                else
                {
                    return this.Value.AsDisplayText();
                }
            }
        }

        /// <summary>   Gets information describing the debug. </summary>
        ///
        /// <value> Information describing the debug. </value>

        public string DebugInfo
        {
            get
            {
                return this.Name + "=" + this.Value.AsDisplayText(QuoteTextType.DoubleQuote);
            }
        }
    }

    /// <summary>   A facet. </summary>
    ///
    /// <remarks>   Ken, 10/8/2020. </remarks>

    [DebuggerDisplay(" { AttributeCode } ")]
    public class Facet
    {
        /// <summary>   Gets or sets the attribute. </summary>
        ///
        /// <value> The attribute. </value>

        public Attribute Attribute { get; set; }  // TODO - violates marshallability

        /// <summary>   Gets or sets the identifier. </summary>
        ///
        /// <value> The identifier. </value>

        public string ID { get; set; }

        /// <summary>   Gets or sets the identifier of the parent. </summary>
        ///
        /// <value> The identifier of the parent. </value>

        public string ParentID { get; set; }

        /// <summary>   Gets or sets the type of the facet. </summary>
        ///
        /// <value> The type of the facet. </value>

        public BaseType FacetType { get; set; }

        /// <summary>   Gets or sets the arguments. </summary>
        ///
        /// <value> The arguments. </value>

        public Argument[] Arguments { get; set; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/8/2020. </remarks>
        ///
        /// <param name="attribute">    The attribute. </param>

        public Facet(Attribute attribute)
        {
            var attributeType = attribute.GetType();
            var arguments = new List<Argument>();
            string attributeCode;

            this.Attribute = attribute;

            this.FacetType = new BaseType
            {
                FullyQualifiedName = attributeType.FullName,
                Name = attributeType.Name,
                ID = attributeType.Name,
                ParentID = this.ID
            };

            foreach (var property in attributeType.GetProperties().Where(p => p.CanRead))
            {
                if (property.Name == "TypeId")
                {
                    this.ID = property.GetValue(attribute).ToString();
                }
                else
                {
                    var argument = new Argument(property.Name, property.GetValue(attribute));

                    arguments.Add(argument);
                }
            }

            this.Arguments = arguments.ToArray();

            attributeCode = this.AttributeCode;
        }

        /// <summary>   Gets the attribute code. </summary>
        ///
        /// <value> The attribute code. </value>

        public string AttributeCode
        {
            get
            {
                return FacetType.Name + "(" + Arguments.Select(a => a.ArgumentCode).ToCommaDelimitedList() + ")";
            }
        }
    }

    /// <summary>   Interface for parent base. </summary>
    ///
    /// <remarks>   Ken, 10/8/2020. </remarks>

    public interface IParentBase : IBase, IPathQueryable
    {
        /// <summary>   Gets the child elements. </summary>
        ///
        /// <value> The child elements. </value>

        IEnumerable<IElement> ChildElements { get; }

        /// <summary>   Gets the child nodes. </summary>
        ///
        /// <value> The child nodes. </value>

        IEnumerable<IBase> ChildNodes { get; }

        /// <summary>   Gets the root. </summary>
        ///
        /// <value> The root. </value>

        IRoot Root { get; } 
    }

    /// <summary>   Interface for base. </summary>
    ///
    /// <remarks>   Ken, 10/8/2020. </remarks>

    public interface IBase 
    {
        /// <summary>   Gets the identifier. </summary>
        ///
        /// <value> The identifier. </value>

        string ID { get; }

        /// <summary>   Gets the identifier of the parent. </summary>
        ///
        /// <value> The identifier of the parent. </value>

        string ParentID { get; }

        /// <summary>   Gets the name. </summary>
        ///
        /// <value> The name. </value>

        string Name { get; }

        /// <summary>   Gets the design comments. </summary>
        ///
        /// <value> The design comments. </value>

        string DesignComments { get; }

        /// <summary>   Gets the documentation. </summary>
        ///
        /// <value> The documentation. </value>

        string Documentation { get; }

        /// <summary>   Gets the documentation summary. </summary>
        ///
        /// <value> The documentation summary. </value>

        string DocumentationSummary { get; }

        /// <summary>   Gets a value indicating whether this  has documentation. </summary>
        ///
        /// <value> True if this  has documentation, false if not. </value>

        bool HasDocumentation { get; }

        /// <summary>   Gets the child ordinal. </summary>
        ///
        /// <value> The child ordinal. </value>

        float ChildOrdinal { get; }

        /// <summary>   Gets information describing the debug. </summary>
        ///
        /// <value> Information describing the debug. </value>

        string DebugInfo { get; }

        /// <summary>   Gets the parent. </summary>
        ///
        /// <value> The parent. </value>

        IBase Parent { get; }

        /// <summary>   Gets or sets the folder key pair. </summary>
        ///
        /// <value> The folder key pair. </value>

        string FolderKeyPair { get; set; }

        /// <summary>   Gets the kind. </summary>
        ///
        /// <value> The kind. </value>

        DefinitionKind Kind { get; }

        /// <summary>   Gets the modifiers. </summary>
        ///
        /// <value> The modifiers. </value>

        Modifiers Modifiers { get; }

        /// <summary>   Gets a value indicating whether this  has children. </summary>
        ///
        /// <value> True if this  has children, false if not. </value>

        bool HasChildren { get; }

        /// <summary>   Gets the facets. </summary>
        ///
        /// <value> The facets. </value>

        Facet[] Facets { get; }

        /// <summary>   Gets the root. </summary>
        ///
        /// <value> The root. </value>

        IRoot Root { get; }

        /// <summary>   Gets a value indicating whether this  is root. </summary>
        ///
        /// <value> True if this  is root, false if not. </value>

        bool IsRoot { get; }

        /// <summary>   Gets or sets a dictionary of entities. </summary>
        ///
        /// <value> A dictionary of entities. </value>

        Dictionary<string, IBase> EntityDictionary { set; get; }

        /// <summary>   Skip process. </summary>
        ///
        /// <param name="facetHandler">             The facet handler. </param>
        /// <param name="facet">                    The facet. </param>
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        bool SkipProcess(IFacetHandler facetHandler, Facet facet, IGeneratorConfiguration generatorConfiguration);
    }

    /// <summary>   Interface for attribute. </summary>
    ///
    /// <remarks>   Ken, 10/8/2020. </remarks>

    public interface IAttribute : IParentBase 
    {
        /// <summary>   Gets the type of the data. </summary>
        ///
        /// <value> The type of the data. </value>

        ScalarType DataType { get; }

        /// <summary>   Gets the default value. </summary>
        ///
        /// <value> The default value. </value>

        string DefaultValue { get; }

        /// <summary>   Gets a value indicating whether this  is nullable. </summary>
        ///
        /// <value> True if nullable, false if not. </value>

        bool Nullable { get; }
    }

    /// <summary>   Interface for database attribute. </summary>
    ///
    /// <remarks>   Ken, 10/8/2020. </remarks>

    public interface IDbAttribute : IAttribute
    {
        /// <summary>   Gets the identifier of the database. </summary>
        ///
        /// <value> The identifier of the database. </value>

        string DatabaseID { get; }

        /// <summary>   Gets the identifier of the related. </summary>
        ///
        /// <value> The identifier of the related. </value>

        string RelatedID { get; }
    }

    /// <summary>   Interface for database element. </summary>
    ///
    /// <remarks>   Ken, 10/8/2020. </remarks>

    public interface IDbElement : IElement
    {
        /// <summary>   Gets the identifier of the database. </summary>
        ///
        /// <value> The identifier of the database. </value>

        string DatabaseID { get; }

        /// <summary>   Gets the identifier of the related. </summary>
        ///
        /// <value> The identifier of the related. </value>

        string RelatedID { get; }
    }

    /// <summary>   Interface for database operation. </summary>
    ///
    /// <remarks>   Ken, 10/8/2020. </remarks>

    public interface IDbOperation : IOperation
    {
        /// <summary>   Gets the identifier of the database. </summary>
        ///
        /// <value> The identifier of the database. </value>

        string DatabaseID { get; }

        /// <summary>   Gets the identifier of the related. </summary>
        ///
        /// <value> The identifier of the related. </value>

        string RelatedID { get; }
    }

    /// <summary>   Interface for operation. </summary>
    ///
    /// <remarks>   Ken, 10/8/2020. </remarks>

    public interface IOperation : IParentBase
    {
        /// <summary>   Gets the direction. </summary>
        ///
        /// <value> The direction. </value>

        OperationDirection Direction { get; }

        /// <summary>   Gets the type of the return. </summary>
        ///
        /// <value> The type of the return. </value>

        BaseType ReturnType { get; }
    }

    /// <summary>   Interface for element. </summary>
    ///
    /// <remarks>   Ken, 10/8/2020. </remarks>

    public interface IElement : IParentBase 
    {
        /// <summary>   Gets a value indicating whether this  is container. </summary>
        ///
        /// <value> True if this  is container, false if not. </value>

        bool IsContainer { get; }

        /// <summary>   Gets the type of the data. </summary>
        ///
        /// <value> The type of the data. </value>

        BaseType DataType { get; }

        /// <summary>   Gets the attributes. </summary>
        ///
        /// <value> The attributes. </value>

        IEnumerable<IAttribute> Attributes { get; }

        /// <summary>   Gets the operations. </summary>
        ///
        /// <value> The operations. </value>

        IEnumerable<IOperation> Operations { get; }

        /// <summary>   Gets the default container type. </summary>
        ///
        /// <value> The default container type. </value>

        ContainerType DefaultContainerType { get; }

        /// <summary>   Gets the default construct type. </summary>
        ///
        /// <value> The default construct type. </value>

        ConstructType DefaultConstructType { get; }

        /// <summary>   Gets a list of types of the allowable containers. </summary>
        ///
        /// <value> A list of types of the allowable containers. </value>

        ContainerType AllowableContainerTypes { get; }

        /// <summary>   Gets a list of types of the allowable constructs. </summary>
        ///
        /// <value> A list of types of the allowable constructs. </value>

        ConstructType AllowableConstructTypes { get; }
    }

    /// <summary>   Interface for relation property. </summary>
    ///
    /// <remarks>   Ken, 10/8/2020. </remarks>

    public interface IRelationProperty : IElement
    {
        /// <summary>   Gets this multiplicity. </summary>
        ///
        /// <value> this multiplicity. </value>

        string ThisMultiplicity { get; }

        /// <summary>   Gets the parent multiplicity. </summary>
        ///
        /// <value> The parent multiplicity. </value>

        string ParentMultiplicity { get; }

        /// <summary>   Gets the name of this property reference. </summary>
        ///
        /// <value> The name of this property reference. </value>

        string ThisPropertyRefName { get; }

        /// <summary>   Gets the name of the parent property reference. </summary>
        ///
        /// <value> The name of the parent property reference. </value>

        string ParentPropertyRefName { get; }

        /// <summary>   Gets this property reference. </summary>
        ///
        /// <value> this property reference. </value>

        IAttribute ThisPropertyRef { get; }

        /// <summary>   Gets the parent property reference. </summary>
        ///
        /// <value> The parent property reference. </value>

        IAttribute ParentPropertyRef { get; }
    }

    /// <summary>   Interface for surrogate element. </summary>
    ///
    /// <remarks>   Ken, 10/8/2020. </remarks>

    public interface ISurrogateElement : IElement
    {
        /// <summary>   Gets the surrogate source. </summary>
        ///
        /// <value> The surrogate source. </value>

        IElement SurrogateSource { get; }

        /// <summary>   Gets the referenced from. </summary>
        ///
        /// <value> The referenced from. </value>

        IElement ReferencedFrom { get; }
    }
}
