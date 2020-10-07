// file:	IGeneratorConfiguration.cs
//
// summary:	Declares the IGeneratorConfiguration interface

using AbstraX.DataAnnotations;
using AbstraX.FolderStructure;
using AbstraX.Models.Interfaces;
using AbstraX.PackageCache;
using AbstraX.ServerInterfaces;
using AbstraX.TemplateObjects;
using AbstraX.Validation;
using CodeInterfaces;
using EntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using System.Xml.Linq;
using Unity;

namespace AbstraX
{
    /// <summary>   Interface for generator configuration. </summary>
    ///
    /// <remarks>   Ken, 10/4/2020. </remarks>

    public interface IGeneratorConfiguration : IPackageCacheStatusProvider
    {
        /// <summary>   Gets the name of the application. </summary>
        ///
        /// <value> The name of the application. </value>

        string AppName { get; }

        /// <summary>   Gets or sets the indent level. </summary>
        ///
        /// <value> The indent level. </value>

        int IndentLevel { get; set; }

        /// <summary>   Gets information describing the application. </summary>
        ///
        /// <value> Information describing the application. </value>

        string AppDescription { get; }

        /// <summary>   Gets or sets the identity provider. </summary>
        ///
        /// <value> The identity provider. </value>

        string IdentityProvider { get; set; }

        /// <summary>   Gets the package path cache. </summary>
        ///
        /// <value> The package path cache. </value>

        string PackagePathCache { get; }

        /// <summary>   Gets a list of types of the graph qls. </summary>
        ///
        /// <value> A list of types of the graph qls. </value>

        List<string> GraphQLTypes { get; }

        /// <summary>   Gets options for controlling the additional. </summary>
        ///
        /// <value> Options that control the additional. </value>

        Dictionary<string, object> AdditionalOptions { get; }

        /// <summary>   Gets the authorized roles. </summary>
        ///
        /// <value> The authorized roles. </value>

        string AuthorizedRoles { get; }

        /// <summary>   Gets the abstra x coordinate schema documents. </summary>
        ///
        /// <value> The abstra x coordinate schema documents. </value>

        Dictionary<string, XDocument> AbstraXSchemaDocuments { get; }

        /// <summary>   Searches for the first data annotation type. </summary>
        ///
        /// <param name="propertyName"> Name of the property. </param>
        ///
        /// <returns>   The found data annotation type. </returns>

        Type FindDataAnnotationType(string propertyName);

        /// <summary>   Handler, called when the get data annotation type. </summary>
        ///
        /// <param name="propertyName"> Name of the property. </param>
        /// <param name="type">         The type. </param>
        ///
        /// <returns>   The data annotation type handler. </returns>

        IDataAnnotationTypeHandler GetDataAnnotationTypeHandler(string propertyName, Type type);

        /// <summary>   Gets the unity container. </summary>
        ///
        /// <value> The unity container. </value>

        IUnityContainer UnityContainer { get; }

        /// <summary>   Gets the resources handler. </summary>
        ///
        /// <value> The resources handler. </value>

        ResourcesHandler ResourcesHandler { get; }

        /// <summary>   Gets the roles. </summary>
        ///
        /// <value> The roles. </value>

        Dictionary<Guid, string> Roles { get; }

        /// <summary>   Gets the custom queries. </summary>
        ///
        /// <value> The custom queries. </value>

        QueryDictionary CustomQueries { get; }

        /// <summary>   Gets the application generator engine. </summary>
        ///
        /// <value> The application generator engine. </value>

        IAppGeneratorEngine AppGeneratorEngine { get; }

        /// <summary>   Gets the workspace generator engine. </summary>
        ///
        /// <value> The workspace generator engine. </value>

        IWorkspaceGeneratorEngine WorkspaceGeneratorEngine { get; }

        /// <summary>   Gets the input files. </summary>
        ///
        /// <value> The input files. </value>

        Dictionary<string, string> InputFiles { get; }

        /// <summary>   Gets or sets the configured pass. </summary>
        ///
        /// <value> The configured pass. </value>

        GeneratorPass ConfiguredPass { get; set; }

        /// <summary>   Gets or sets the current pass. </summary>
        ///
        /// <value> The current pass. </value>

        GeneratorPass CurrentPass { get; set; }

        /// <summary>   Gets the recursion mode. </summary>
        ///
        /// <value> The recursion mode. </value>

        RecursionMode RecursionMode { get; }

        /// <summary>   Gets the type of the project. </summary>
        ///
        /// <value> The type of the project. </value>

        Guid ProjectType { get; }

        /// <summary>   Gets the project folder root. </summary>
        ///
        /// <value> The project folder root. </value>

        string ProjectFolderRoot { get; }

        /// <summary>   Gets the services project. </summary>
        ///
        /// <value> The services project. </value>

        IVSProject ServicesProject { get; }
        /// <summary>   Indents this.  </summary>
        void Indent();
        /// <summary>   Dedents this.  </summary>
        void Dedent();

        /// <summary>   Gets a stack of handlers. </summary>
        ///
        /// <value> A stack of handlers. </value>

        Stack<HandlerStackItem> HandlerStack { get; }

        /// <summary>   Gets the view projects. </summary>
        ///
        /// <value> The view projects. </value>

        Dictionary<string, IViewProject> ViewProjects { get; }

        /// <summary>   Gets a stack of hierarchies. </summary>
        ///
        /// <value> A stack of hierarchies. </value>

        Stack<HierarchyStackItem> HierarchyStack { get; }

        /// <summary>   Gets the full pathname of the hierarchy file. </summary>
        ///
        /// <value> The full pathname of the hierarchy file. </value>

        string HierarchyPath { get; }

        /// <summary>   Gets the key value pairs. </summary>
        ///
        /// <value> The key value pairs. </value>

        Dictionary<string, object> KeyValuePairs { get; }

        /// <summary>   Gets the custom handlers. </summary>
        ///
        /// <value> The custom handlers. </value>

        List<ICustomHandler> CustomHandlers { get; }

        /// <summary>   Gets the other handlers. </summary>
        ///
        /// <value> The other handlers. </value>

        List<IFacetHandler> OtherHandlers { get; }

        /// <summary>   Gets the package installs. </summary>
        ///
        /// <value> The package installs. </value>

        IEnumerable<string> PackageInstalls { get; }

        /// <summary>   Gets the built in modules. </summary>
        ///
        /// <value> The built in modules. </value>

        IEnumerable<Module> BuiltInModules { get; }

        /// <summary>   Gets the identifier of the client. </summary>
        ///
        /// <value> The identifier of the client. </value>

        string ClientId { get; }

        /// <summary>   Gets the client secret. </summary>
        ///
        /// <value> The client secret. </value>

        string ClientSecret { get; }

        /// <summary>   Gets the application folder hierarchy. </summary>
        ///
        /// <value> The application folder hierarchy. </value>

        ApplicationFolderHierarchy ApplicationFolderHierarchy { get; }

        /// <summary>   Gets a value indicating whether the no file creation. </summary>
        ///
        /// <value> True if no file creation, false if not. </value>

        bool NoFileCreation { get; }

        /// <summary>   Gets the identity. </summary>
        ///
        /// <value> The identity entity. </value>

        IEntityWithFacets IdentityEntity { get; }

        /// <summary>   Adds a translation. </summary>
        ///
        /// <param name="baseObject">   The base object. </param>
        /// <param name="key">          The key. </param>
        /// <param name="value">        The value. </param>
        /// <param name="skipIfSame">   True to skip if same. </param>
        ///
        /// <returns>   A string. </returns>

        string AddTranslation(ServerInterfaces.IBase baseObject, string key, string value, bool skipIfSame);

        /// <summary>   Adds a built in module. </summary>
        ///
        /// <param name="module">   A variable-length parameters list containing module. </param>

        void AddBuiltInModule(params Module[] module);

        /// <summary>   Handles the facets. </summary>
        ///
        /// <param name="entityWithFacets"> The entity with facets. </param>
        /// <param name="isIdentityEntity"> (Optional) True if is identity, false if not. </param>
        ///
        /// <returns>   A HandlerStackItem. </returns>

        HandlerStackItem HandleFacets(IEntityWithFacets entityWithFacets, bool isIdentityEntity = false);

        /// <summary>   Query if 'entityWithFacets' is identity. </summary>
        ///
        /// <param name="entityWithFacets"> The entity with facets. </param>
        ///
        /// <returns>   True if identity, false if not. </returns>

        bool IsIdentityEntity(ServerInterfaces.IBase entityWithFacets);

        /// <summary>   Handles the module kind. </summary>
        ///
        /// <param name="moduleObject"> The module object. </param>
        /// <param name="folder">       Pathname of the folder. </param>
        /// <param name="moduleKind">   An enum constant representing the module kind option. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        bool HandleModuleKind(IModuleObject moduleObject, Folder folder, Enum moduleKind);

        /// <summary>   Handles the views. </summary>
        ///
        /// <param name="project">      The project. </param>
        /// <param name="baseObject">   The base object. </param>
        /// <param name="facet">        The facet. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        bool HandleViews(IViewProject project, ServerInterfaces.IBase baseObject, ServerInterfaces.Facet facet);

        /// <summary>   Builds validation set. </summary>
        ///
        /// <param name="baseObject">   The base object. </param>
        ///
        /// <returns>   An IValidationSet. </returns>

        IValidationSet BuildValidationSet(ServerInterfaces.IBase baseObject);

        /// <summary>   Adds a package installs. </summary>
        ///
        /// <param name="facetHandler"> The facet handler. </param>

        void AddPackageInstalls(IFacetHandler facetHandler);

        /// <summary>   Enumerates create imports in this collection. </summary>
        ///
        /// <param name="baseHandler">      The base handler. </param>
        /// <param name="baseObject">       The base object. </param>
        /// <param name="folder">           Pathname of the folder. </param>
        /// <param name="includeSelf">      (Optional) True to include, false to exclude the self. </param>
        /// <param name="subFolderCount">   (Optional) Number of sub folders. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process create imports in this collection.
        /// </returns>

        IEnumerable<ModuleImportDeclaration> CreateImports(IModuleHandler baseHandler, ServerInterfaces.IBase baseObject, Folder folder, bool includeSelf = false, int subFolderCount = 0);

        /// <summary>   Enumerates create imports in this collection. </summary>
        ///
        /// <param name="moduleKindHandler">    The module kind handler. </param>
        /// <param name="moduleAssembly">       The module assembly. </param>
        /// <param name="folder">               Pathname of the folder. </param>
        /// <param name="includeSelf">          (Optional) True to include, false to exclude the self. </param>
        /// <param name="subFolderCount">       (Optional) Number of sub folders. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process create imports in this collection.
        /// </returns>

        IEnumerable<ModuleImportDeclaration> CreateImports(IModuleKindHandler moduleKindHandler, IModuleAssembly moduleAssembly, Folder folder, bool includeSelf = false, int subFolderCount = 0);

        /// <summary>   Enumerates create imports in this collection. </summary>
        ///
        /// <param name="moduleKindHandler">    The module kind handler. </param>
        /// <param name="moduleAssembly">       The module assembly. </param>
        /// <param name="modulesOrAssemblies">  The modules or assemblies. </param>
        /// <param name="folder">               Pathname of the folder. </param>
        /// <param name="includeSelf">          (Optional) True to include, false to exclude the self. </param>
        /// <param name="subFolderCount">       (Optional) Number of sub folders. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process create imports in this collection.
        /// </returns>

        IEnumerable<ModuleImportDeclaration> CreateImports(IModuleKindHandler moduleKindHandler, IModuleAssembly moduleAssembly, List<IModuleOrAssembly> modulesOrAssemblies, Folder folder, bool includeSelf = false, int subFolderCount = 0);

        /// <summary>   Creates import groups. </summary>
        ///
        /// <param name="facetHandler"> The facet handler. </param>
        /// <param name="baseObject">   The base object. </param>
        /// <param name="folder">       Pathname of the folder. </param>
        /// <param name="includeSelf">  (Optional) True to include, false to exclude the self. </param>
        ///
        /// <returns>   The new import groups. </returns>

        IDictionary<string, IEnumerable<ModuleImportDeclaration>> CreateImportGroups(IFacetHandler facetHandler, ServerInterfaces.IBase baseObject, Folder folder, bool includeSelf = false);

        /// <summary>   Gets the expression handlers in this collection. </summary>
        ///
        /// <param name="providerGuid"> Unique identifier for the provider. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the expression handlers in this
        /// collection.
        /// </returns>

        IEnumerable<IExpressionHandler> GetExpressionHandlers(Guid providerGuid);

        /// <summary>   Handler, called when the get expression. </summary>
        ///
        /// <param name="providerGuid"> Unique identifier for the provider. </param>
        ///
        /// <returns>   The expression handler. </returns>

        IExpressionHandler GetExpressionHandler(Guid providerGuid);

        /// <summary>   Handler, called when the get. </summary>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        ///
        /// <returns>   The handler. </returns>

        T GetHandler<T>() where T : IHandler;

        /// <summary>   Handler, called when the get. </summary>
        ///
        /// <typeparam name="T">            Generic type parameter. </typeparam>
        /// <typeparam name="TAttribute">   Type of the attribute. </typeparam>
        /// <param name="filter">   Specifies the filter. </param>
        ///
        /// <returns>   The handler. </returns>

        T GetHandler<T, TAttribute>(Func<TAttribute, bool> filter) where T : IHandler where TAttribute : Attribute;
        T GetHandler<T>(Func<T, bool> filter) where T : IHandler;

        /// <summary>   Handler, called when the get application settings kind. </summary>
        ///
        /// <param name="appSettingsKind">  The application settings kind. </param>
        ///
        /// <returns>   The application settings kind handler. </returns>

        IAppSettingsKindHandler GetAppSettingsKindHandler(AppSettingsKind appSettingsKind);


        /// <summary>   Gets a memory module builder. </summary>
        ///
        /// <remarks>   Ken, 10/4/2020. </remarks>
        ///
        /// <returns>   The memory module builder is a sandbox builder to allow runtime reflection-like creation to aid with code generation. </returns>

        IMemoryModuleBuilder GetMemoryModuleBuilder();


        /// <summary>   Pushes a module assembly. </summary>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="namePrefix">   The name prefix. </param>
        ///
        /// <returns>   An IModuleAssembly. </returns>
        /// 
        IModuleAssembly PushModuleAssembly<T>(string namePrefix) where T : IModuleAssembly, new();

        /// <summary>   Pushes a container. </summary>
        ///
        /// <param name="container">    The container. </param>
        ///
        /// <returns>   An IDisposable. </returns>

        IDisposable PushContainer(IEntityContainer container);

        /// <summary>   Pushes an entity set. </summary>
        ///
        /// <param name="entitySet">    Set the entity belongs to. </param>
        ///
        /// <returns>   An IDisposable. </returns>

        IDisposable PushEntitySet(IEntitySet entitySet);

        /// <summary>   Pushes an entity type. </summary>
        ///
        /// <param name="entity">   The entity. </param>
        ///
        /// <returns>   An IDisposable. </returns>

        IDisposable PushEntityType(IEntityType entity);

        /// <summary>   Adds a navigation property. </summary>
        ///
        /// <param name="property"> The property. </param>

        void AddNavigationProperty(INavigationProperty property);

        /// <summary>   Adds a property. </summary>
        ///
        /// <param name="attribute">    The attribute. </param>

        void AddProperty(ServerInterfaces.IAttribute attribute);

        /// <summary>   Adds the facets. </summary>
        ///
        /// <param name="entityWithFacets"> The entity with facets. </param>

        void AddFacets(IEntityWithFacets entityWithFacets);

        /// <summary>   Sets module assembly properties. </summary>
        ///
        /// <param name="properties">   The properties. </param>

        void SetModuleAssemblyProperties(IModuleAssemblyProperties properties);

        /// <summary>   Sets module assembly folder. </summary>
        ///
        /// <param name="folder">   Pathname of the folder. </param>

        void SetModuleAssemblyFolder(Folder folder);

        /// <summary>   Creates a file. </summary>
        ///
        /// <param name="filePath"> Full pathname of the file. </param>
        ///
        /// <returns>   The new file. </returns>

        System.IO.Stream CreateFile(string filePath);

        /// <summary>   Creates a directory. </summary>
        ///
        /// <param name="filePath"> Full pathname of the file. </param>
        ///
        /// <returns>   The new directory. </returns>

        System.IO.DirectoryInfo CreateDirectory(string filePath);

        /// <summary>   Creates a file. </summary>
        ///
        /// <param name="moduleAssemblyProperties"> The module assembly properties. </param>
        /// <param name="fileInfo">                 Information describing the file. </param>
        /// <param name="output">                   The output. </param>
        /// <param name="fileKind">                 The file kind. </param>
        /// <param name="hierarchyGenerator">       The hierarchy generator. </param>
        ///
        /// <returns>   The new file. </returns>

        File CreateFile(IModuleAssemblyProperties moduleAssemblyProperties, System.IO.FileInfo fileInfo, string output, FileKind fileKind, Func<StringBuilder> hierarchyGenerator);

        /// <summary>   Creates a file. </summary>
        ///
        /// <param name="fileInfo">             Information describing the file. </param>
        /// <param name="output">               The output. </param>
        /// <param name="fileKind">             The file kind. </param>
        /// <param name="hierarchyGenerator">   The hierarchy generator. </param>
        ///
        /// <returns>   The new file. </returns>

        File CreateFile(System.IO.FileInfo fileInfo, string output, FileKind fileKind, Func<StringBuilder> hierarchyGenerator);

        /// <summary>   Creates a file. </summary>
        ///
        /// <param name="fileInfo">             Information describing the file. </param>
        /// <param name="nonAssemblyModules">   The non assembly modules. </param>
        /// <param name="output">               The output. </param>
        /// <param name="fileKind">             The file kind. </param>
        /// <param name="hierarchyGenerator">   The hierarchy generator. </param>
        ///
        /// <returns>   The new file. </returns>

        File CreateFile(System.IO.FileInfo fileInfo, IEnumerable<Module> nonAssemblyModules, string output, FileKind fileKind, Func<StringBuilder> hierarchyGenerator);

        /// <summary>   Creates a file. </summary>
        ///
        /// <param name="fileInfo">         Information describing the file. </param>
        /// <param name="output">           The output. </param>
        /// <param name="fileKind">         The file kind. </param>
        /// <param name="moduleAssembly">   The module assembly. </param>
        ///
        /// <returns>   The new file. </returns>

        File CreateFile(System.IO.FileInfo fileInfo, string output, FileKind fileKind, IModuleAssembly moduleAssembly);

        /// <summary>   Gets the file system. </summary>
        ///
        /// <value> The file system. </value>

        FileSystem FileSystem { get; }

        /// <summary>   Gets a dictionary of languages. </summary>
        ///
        /// <value> A dictionary of languages. </value>

        LanguageDictionary LanguageDictionary { get; }

        /// <summary>   Gets the parts alias resolver. </summary>
        ///
        /// <value> The parts alias resolver. </value>

        PartsAliasResolver PartsAliasResolver { get; }

        /// <summary>   Creates type for entity. </summary>
        ///
        /// <param name="moduleBuilder">            The module builder. </param>
        /// <param name="entity">                   The entity. </param>
        /// <param name="appHierarchyNodeObject">   The application hierarchy node object. </param>

        void CreateTypeForEntity(ModuleBuilder moduleBuilder, EntityObject entity, UIHierarchyNodeObject appHierarchyNodeObject);
        /// <summary>   Terminates this.  </summary>
        void Terminate();
    }
}
