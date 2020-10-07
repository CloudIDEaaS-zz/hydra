// file:	GeneratorConfiguration.cs
//
// summary:	Implements the generator configuration class

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Schema;
using AbstraX.DataAnnotations;
using AbstraX.FolderStructure;
using AbstraX.Models.Interfaces;
using AbstraX.PackageCache;
using AbstraX.ServerInterfaces;
using AbstraX.Validation;
using CodeInterfaces;
using EntityProvider.Web.Entities;
using GraphQL.Types;
using Unity;
using Utils;
using System.Reflection;
using IBase = AbstraX.ServerInterfaces.IBase;
using AbstraX.TemplateObjects;
using System.Reflection.Emit;
using TypeExtensions = Utils.TypeExtensions;
using System.Runtime.CompilerServices;

namespace AbstraX
{
    /// <summary>   A generator configuration. </summary>
    ///
    /// <remarks>   Ken, 10/1/2020. </remarks>

    public class GeneratorConfiguration : IGeneratorConfiguration
    {
        /// <summary>   Gets the type of the project. </summary>
        ///
        /// <value> The type of the project. </value>

        public Guid ProjectType { get; }

        /// <summary>   Gets the project folder root. </summary>
        ///
        /// <value> The project folder root. </value>

        public string ProjectFolderRoot { get; }

        /// <summary>   Gets the package path cache. </summary>
        ///
        /// <value> The package path cache. </value>

        public string PackagePathCache { get; }

        /// <summary>   Gets or sets the configured pass. </summary>
        ///
        /// <value> The configured pass. </value>

        public GeneratorPass ConfiguredPass { get; set; }

        /// <summary>   Gets or sets the current pass. </summary>
        ///
        /// <value> The current pass. </value>

        public GeneratorPass CurrentPass { get; set; }

        /// <summary>   Gets or sets the recursion mode. </summary>
        ///
        /// <value> The recursion mode. </value>

        public RecursionMode RecursionMode { get; private set; }

        /// <summary>   Gets the services project. </summary>
        ///
        /// <value> The services project. </value>

        public IVSProject ServicesProject { get; }

        /// <summary>   Gets options for controlling the additional. </summary>
        ///
        /// <value> Options that control the additional. </value>

        public Dictionary<string, object> AdditionalOptions { get; }

        /// <summary>   Gets options for controlling the generator. </summary>
        ///
        /// <value> Options that control the generator. </value>

        public GeneratorOptions GeneratorOptions { get; }

        /// <summary>   Gets or sets for life stack items. </summary>
        ///
        /// <value> for life stack items. </value>

        public List<SingleHandler> ForLifeStackItems { get; set; }

        /// <summary>   Gets or sets a stack of handlers. </summary>
        ///
        /// <value> A stack of handlers. </value>

        public Stack<HandlerStackItem> HandlerStack { get; set; }

        /// <summary>   Gets or sets a stack of hierarchies. </summary>
        ///
        /// <value> A stack of hierarchies. </value>

        public Stack<HierarchyStackItem> HierarchyStack { get; set; }

        /// <summary>   Gets or sets the indent level. </summary>
        ///
        /// <value> The indent level. </value>

        public int IndentLevel { get; set; }

        /// <summary>   Gets or sets a stack of elements. </summary>
        ///
        /// <value> A stack of elements. </value>

        public Stack<XElement> ElementStack { get; set; }

        /// <summary>   Gets or sets the key value pairs. </summary>
        ///
        /// <value> The key value pairs. </value>

        public Dictionary<string, object> KeyValuePairs { get; set; }

        /// <summary>   Gets or sets a dictionary of languages. </summary>
        ///
        /// <value> A dictionary of languages. </value>

        public LanguageDictionary LanguageDictionary { get; set; }

        /// <summary>   Gets or sets the custom handlers. </summary>
        ///
        /// <value> The custom handlers. </value>

        public List<ICustomHandler> CustomHandlers { get; set; }

        /// <summary>   Gets or sets the other handlers. </summary>
        ///
        /// <value> The other handlers. </value>

        public List<IFacetHandler> OtherHandlers { get; set; }

        /// <summary>   Gets or sets the application generator engine. </summary>
        ///
        /// <value> The application generator engine. </value>

        public IAppGeneratorEngine AppGeneratorEngine { get; private set; }

        /// <summary>   Gets or sets the workspace generator engine. </summary>
        ///
        /// <value> The workspace generator engine. </value>

        public IWorkspaceGeneratorEngine WorkspaceGeneratorEngine { get; private set; }

        /// <summary>   Gets the input files. </summary>
        ///
        /// <value> The input files. </value>

        public Dictionary<string, string> InputFiles { get; }

        /// <summary>   Gets or sets a value indicating whether the suppress debug output. </summary>
        ///
        /// <value> True if suppress debug output, false if not. </value>

        public bool SuppressDebugOutput { get; set; }

        /// <summary>   Gets or sets the name of the application. </summary>
        ///
        /// <value> The name of the application. </value>

        public string AppName { get; set; }

        /// <summary>   Gets or sets information describing the application. </summary>
        ///
        /// <value> Information describing the application. </value>

        public string AppDescription { get; set; }

        /// <summary>   Gets or sets the identity provider. </summary>
        ///
        /// <value> The identity provider. </value>

        public string IdentityProvider { get; set; }

        /// <summary>   Gets the application folder hierarchy. </summary>
        ///
        /// <value> The application folder hierarchy. </value>

        public ApplicationFolderHierarchy ApplicationFolderHierarchy { get; }

        /// <summary>   Gets or sets a value indicating whether the no file creation. </summary>
        ///
        /// <value> True if no file creation, false if not. </value>

        public bool NoFileCreation { get; private set; }

        /// <summary>   Gets or sets the identity. </summary>
        ///
        /// <value> The identity entity. </value>

        public IEntityWithFacets IdentityEntity { get; private set; }

        /// <summary>   Gets or sets the parts alias resolver. </summary>
        ///
        /// <value> The parts alias resolver. </value>

        public PartsAliasResolver PartsAliasResolver { get; private set; }

        /// <summary>   Gets or sets the identifier of the client. </summary>
        ///
        /// <value> The identifier of the client. </value>

        public string ClientId { get; set; }

        /// <summary>   Gets or sets the client secret. </summary>
        ///
        /// <value> The client secret. </value>

        public string ClientSecret { get; set; }

        /// <summary>   Gets or sets the roles. </summary>
        ///
        /// <value> The roles. </value>

        public Dictionary<Guid, string> Roles { get; private set; }

        /// <summary>   Gets or sets the view projects. </summary>
        ///
        /// <value> The view projects. </value>

        public Dictionary<string, IViewProject> ViewProjects { get; private set; }

        /// <summary>   Gets or sets the custom queries. </summary>
        ///
        /// <value> The custom queries. </value>

        public QueryDictionary CustomQueries { get; private set; }

        /// <summary>   Gets the authorized roles. </summary>
        ///
        /// <value> The authorized roles. </value>

        private Dictionary<HandlerStackItem, string[]> authorizedRoles { get; }
        /// <summary>   Manager for package cache. </summary>
        private PackageCacheManager packageCacheManager;
        /// <summary>   The import handlers. </summary>
        private Dictionary<ulong, IImportHandler> importHandlers;
        /// <summary>   The types. </summary>
        private List<Type> types;
        /// <summary>   The built in modules. </summary>
        private List<Module> builtInModules;
        /// <summary>   True to disposing. </summary>
        private bool disposing;
        /// <summary>   The registry settings. </summary>
        private RegistrySettings registrySettings;
        /// <summary>   True to skip dispose. </summary>
        private bool skipDispose;
        /// <summary>   The module assemblies. </summary>
        private Stack<IModuleAssembly> moduleAssemblies;
        /// <summary>   The resources handler. </summary>
        private ResourcesHandler resourcesHandler;
        /// <summary>   The unity container. </summary>
        private UnityContainer unityContainer;
        /// <summary>   The workspace file type handlers. </summary>
        private List<IWorkspaceFileTypeHandler> workspaceFileTypeHandlers;
        /// <summary>   The workspace token content handlers. </summary>
        private List<IWorkspaceTokenContentHandler> workspaceTokenContentHandlers;
        private List<IDataAnnotationTypeHandler> dataAnnotationTypeHandlers;

        /// <summary>   Gets or sets the abstra x coordinate schema documents. </summary>
        ///
        /// <value> The abstra x coordinate schema documents. </value>

        public Dictionary<string, XDocument> AbstraXSchemaDocuments { get; private set; }

        /// <summary>   Identifier for the application import handler. </summary>
        private const ulong APPLICATION_IMPORT_HANDLER_ID = ulong.MinValue;

        /// <summary>   Gets the unity container. </summary>
        ///
        /// <value> The unity container. </value>

        public IUnityContainer UnityContainer
        {
            get
            {
                if (unityContainer == null)
                {
                    unityContainer = (UnityContainer) AppDomain.CurrentDomain.GetData("UnityContainer");
                }

                return unityContainer;
            }
        }

        /// <summary>   Gets or sets the resources handler. </summary>
        ///
        /// <value> The resources handler. </value>

        public ResourcesHandler ResourcesHandler
        {
            get
            {
                return resourcesHandler;
            }

            set
            {
                resourcesHandler = value;
                resourcesHandler.GeneratorConfiguration = this;
            }
        }

        /// <summary>   Gets the file system. </summary>
        ///
        /// <value> The file system. </value>

        public FileSystem FileSystem
        {
            get
            {
                return this.ApplicationFolderHierarchy.FileSystem;
            }
        }

        /// <summary>   Gets the full pathname of the hierarchy file. </summary>
        ///
        /// <value> The full pathname of the hierarchy file. </value>

        public string HierarchyPath
        {
            get
            {
                return this.HierarchyStack.Reverse().ToDelimitedList("/");
            }
        }

        /// <summary>   Gets the authorized roles. </summary>
        ///
        /// <value> The authorized roles. </value>

        public string AuthorizedRoles
        {
            get
            {
                var lastAuthorizedRolePair = this.authorizedRoles.Last();
                var lastAuthorizedStackItem = lastAuthorizedRolePair.Key;

                if (lastAuthorizedStackItem.AuthorizationState == AuthorizationState.Authorize)
                {
                    var roles = this.Roles.Where(r => authorizedRoles.Any(a => a.Value.Any(v => v == r.Value))).ToDictionary(r => r.Key, r => r.Value);

                    return roles.Keys.ToCommaDelimitedList();
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="projectType">          Type of the project. </param>
        /// <param name="projectFolderRoot">    The project folder root. </param>
        /// <param name="servicesProject">      The services project. </param>
        /// <param name="packageCachePath">     Full pathname of the package cache file. </param>
        /// <param name="additionalOptions">    Options for controlling the additional. </param>
        /// <param name="generatorOptions">     Options for controlling the generator. </param>
        /// <param name="generatorEngine">      The generator engine. </param>
        /// <param name="types">                The types. </param>

        public GeneratorConfiguration(Guid projectType, string projectFolderRoot, IVSProject servicesProject, string packageCachePath, Dictionary<string, object> additionalOptions, GeneratorOptions generatorOptions, IAppGeneratorEngine generatorEngine, List<Type> types)
        {
            this.ProjectType = projectType;
            this.ProjectFolderRoot = projectFolderRoot;
            this.ServicesProject = servicesProject;
            this.AdditionalOptions = additionalOptions;
            this.GeneratorOptions = generatorOptions;
            this.ConfiguredPass = generatorOptions.GeneratorPass;
            this.AppGeneratorEngine = generatorEngine;
            this.HandlerStack = new Stack<HandlerStackItem>();
            this.HierarchyStack = new Stack<HierarchyStackItem>();
            this.KeyValuePairs = new Dictionary<string, object>();
            this.LanguageDictionary = new LanguageDictionary();
            this.CustomHandlers = new List<ICustomHandler>();
            this.OtherHandlers = new List<IFacetHandler>();
            this.ForLifeStackItems = new List<SingleHandler>();
            this.ViewProjects = new Dictionary<string, IViewProject>();
            this.ApplicationFolderHierarchy = generatorOptions.ApplicationFolderHierarchy;
            this.NoFileCreation = generatorOptions.NoFileCreation;
            this.Roles = new Dictionary<Guid, string>();
            this.CustomQueries = new QueryDictionary();
            this.PackagePathCache = packageCachePath;
            this.ElementStack = new Stack<XElement>();
            this.AbstraXSchemaDocuments = new Dictionary<string, XDocument>();

            registrySettings = new RegistrySettings();

            registrySettings.CurrentWorkingDirectory = this.ProjectFolderRoot;
            registrySettings.PackagePathCache = this.PackagePathCache;

            registrySettings.Save();

            this.types = types;
            builtInModules = new List<Module>();
            moduleAssemblies = new Stack<IModuleAssembly>();
            importHandlers = new Dictionary<ulong, IImportHandler>();
            authorizedRoles = new Dictionary<HandlerStackItem, string[]>();

            if (packageCachePath.IsNullOrEmpty())
            {
                var container = this.UnityContainer;
            }
            else
            {
                var container = this.UnityContainer;
                
                packageCacheManager = new PackageCacheManager(projectFolderRoot, packageCachePath);

                if (container != null)
                {
                    if (container.IsRegistered<IPackageCacheStatusProvider>())
                    {
                        var packageCacheStatusProxy = (PackageCacheStatusProvider)container.Resolve<IPackageCacheStatusProvider>();

                        packageCacheStatusProxy.Source = this;
                    }
                }
            }

            PartsAliasResolver = new PartsAliasResolver();

            //AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="projectType">              Type of the project. </param>
        /// <param name="projectFolderRoot">        The project folder root. </param>
        /// <param name="appName">                  Name of the application. </param>
        /// <param name="additionalOptions">        Options for controlling the additional. </param>
        /// <param name="generatorOptions">         Options for controlling the generator. </param>
        /// <param name="workspaceGeneratorEngine"> The workspace generator engine. </param>
        /// <param name="types">                    The types. </param>

        public GeneratorConfiguration(Guid projectType, string projectFolderRoot, string appName, string appDescription, Dictionary<string, object> additionalOptions, GeneratorOptions generatorOptions, IWorkspaceGeneratorEngine workspaceGeneratorEngine, List<Type> types)
        {
            this.ProjectType = projectType;
            this.ProjectFolderRoot = projectFolderRoot;
            this.AdditionalOptions = additionalOptions;
            this.GeneratorOptions = generatorOptions;
            this.WorkspaceGeneratorEngine = workspaceGeneratorEngine;
            this.AppName = appName;
            this.AppDescription = appDescription;
            this.types = types;

            registrySettings = new RegistrySettings();

            registrySettings.CurrentWorkingDirectory = this.ProjectFolderRoot;
            registrySettings.PackagePathCache = this.PackagePathCache;

            registrySettings.Save();

            workspaceFileTypeHandlers = new List<IWorkspaceFileTypeHandler>();
            workspaceTokenContentHandlers = new List<IWorkspaceTokenContentHandler>();
            dataAnnotationTypeHandlers = new List<IDataAnnotationTypeHandler>();
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="projectType">              Type of the project. </param>
        /// <param name="projectFolderRoot">        The project folder root. </param>
        /// <param name="inputFiles">               The input files. </param>
        /// <param name="additionalOptions">        Options for controlling the additional. </param>
        /// <param name="generatorOptions">         Options for controlling the generator. </param>
        /// <param name="workspaceGeneratorEngine"> The workspace generator engine. </param>
        /// <param name="types">                    The types. </param>

        public GeneratorConfiguration(Guid projectType, string projectFolderRoot, Dictionary<string, string> inputFiles, Dictionary<string, object> additionalOptions, GeneratorOptions generatorOptions, IWorkspaceGeneratorEngine workspaceGeneratorEngine, List<Type> types)
        {
            this.ProjectType = projectType;
            this.ProjectFolderRoot = projectFolderRoot;
            this.AdditionalOptions = additionalOptions;
            this.GeneratorOptions = generatorOptions;
            this.WorkspaceGeneratorEngine = workspaceGeneratorEngine;
            this.InputFiles = inputFiles;
            this.types = types;

            registrySettings = new RegistrySettings();

            registrySettings.CurrentWorkingDirectory = this.ProjectFolderRoot;
            registrySettings.PackagePathCache = this.PackagePathCache;

            registrySettings.Save();

            workspaceFileTypeHandlers = new List<IWorkspaceFileTypeHandler>();
            workspaceTokenContentHandlers = new List<IWorkspaceTokenContentHandler>();
            dataAnnotationTypeHandlers = new List<IDataAnnotationTypeHandler>();
        }

        /// <summary>
        /// Event handler. Called by CurrentDomain for first chance exception events.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        First chance exception event information. </param>

        private void CurrentDomain_FirstChanceException(object sender, FirstChanceExceptionEventArgs e)
        {
            switch (e.Exception.Message)
            {

            }

            if (!disposing)
            {
                skipDispose = true;
            }
        }

        /// <summary>   Gets the built in modules. </summary>
        ///
        /// <value> The built in modules. </value>

        public IEnumerable<Module> BuiltInModules
        {
            get
            {
                return builtInModules.OrderBy(m => m.Name);
            }
        }

        /// <summary>   Adds a built in module. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="modules">  A variable-length parameters list containing modules. </param>

        public void AddBuiltInModule(params Module[] modules)
        {
            builtInModules.AddRange(modules);
        }

        /// <summary>   Stops the services. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>

        public void StopServices()
        {
            if (packageCacheManager != null)
            {
                packageCacheManager.Dispose();
            }
        }

        /// <summary>   Gets the package installs. </summary>
        ///
        /// <value> The package installs. </value>

        public IEnumerable<string> PackageInstalls
        {
            get
            {
                var installs = new List<string>();
                var root = this.ProjectFolderRoot;
                var allInstalls = new List<string>();

                foreach (var importHandler in importHandlers.Values)
                {
                    foreach (var package in importHandler.Packages.Values)
                    {
                        foreach (var install in package.PackageInstalls)
                        {
                            if (packageCacheManager != null)
                            {
                                if (packageCacheManager.Handled("installs", install))
                                {
                                    allInstalls.Add(install);
                                }
                                else
                                {
                                    installs.Add(install);
                                }
                            }
                            else
                            {
                                installs.Add(install);
                            }
                        }
                    }
                }

                installs = installs.Distinct().ToList();

                allInstalls.AddRange(installs);

                if (packageCacheManager != null)
                {
                    packageCacheManager.SetInstallCount(installs.Count);
                }

                return installs;
            }
        }

        /// <summary>   Gets the package development installs. </summary>
        ///
        /// <value> The package development installs. </value>

        public IEnumerable<string> PackageDevInstalls
        {
            get
            {
                var installs = new List<string>();
                var allInstalls = new List<string>();

                foreach (var importHandler in importHandlers.Values)
                {
                    foreach (var package in importHandler.Packages.Values)
                    {
                        foreach (var install in package.PackageDevInstalls)
                        {
                            if (packageCacheManager != null)
                            {
                                if (packageCacheManager.Handled("devInstalls", install))
                                {
                                    allInstalls.Add(install);
                                }
                                else
                                {
                                    installs.Add(install);
                                }
                            }
                            else
                            {
                                installs.Add(install);
                            }
                        }
                    }
                }

                installs = installs.Distinct().ToList();

                allInstalls.AddRange(installs);

                if (packageCacheManager != null)
                {
                    packageCacheManager.SetInstallCount(installs.Count);
                }

                return installs.Distinct();
            }
        }

        /// <summary>   Gets cache status. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="mode">             The mode. </param>
        /// <param name="setAsReported">    (Optional) True if set as reported. </param>
        ///
        /// <returns>   The cache status. </returns>

        public PackageCacheStatusInfo GetCacheStatus(string mode, bool setAsReported = false)
        {
            return packageCacheManager.GetCacheStatus(mode, setAsReported);
        }

        /// <summary>   Gets install from cache status. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="mode"> The mode. </param>
        ///
        /// <returns>   The install from cache status. </returns>

        public PackageInstallsFromCacheStatus GetInstallFromCacheStatus(string mode)
        {
            return packageCacheManager.GetInstallFromCacheStatus(mode);
        }

        /// <summary>   Sets install status. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="status">   The status. </param>
        ///
        /// <returns>   A string. </returns>

        public string SetInstallStatus(string status)
        {
            if (packageCacheManager == null)
            {
                return $"Install status set to { status }";
            }
            else
            {
                return packageCacheManager.SetInstallStatus(status);
            }
        }

        /// <summary>   Handles the views. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="project">      The project. </param>
        /// <param name="baseObject">   The base object. </param>
        /// <param name="facet">        The facet. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public bool HandleViews(IViewProject project, ServerInterfaces.IBase baseObject, ServerInterfaces.Facet facet)
        {
            var views = project.Views.Where(v => !v.Generated);
            var handlerFacetList = new List<HandlerFacet>();
            var handlerViewList = new List<HandlerView>();
            List<HandlerFacet> sortedList;
            var entityWithFacets = (IEntityWithFacets)baseObject;
            var facets = entityWithFacets.Facets.ToList();
            var currentHandlerStackItem = this.HandlerStack.Peek();
            var customComponentAttribute = (UICustomAttribute)facet.Attribute;

            foreach (var view in views)
            {
                var layout = view.GetLayout();
                var handlerTypes = types.Where(t => t.IsViewLayoutHandlerType(layout));

                foreach (var handlerType in handlerTypes)
                {
                    var handler = handlerType.CreateInstance<IViewLayoutHandler>();

                    if (view.Name == customComponentAttribute.ViewName)
                    {
                        handlerFacetList.Add(new HandlerFacet(handler, facet, view));
                    }

                    handlerViewList.Add(new HandlerView(handler, view));
                }
            }

            sortedList = handlerFacetList.Where(h => !((UICustomAttribute) h.Facet.Attribute).IsPartialOnly).OrderBy(h => h.Handler.Priority).ToList();

            if (sortedList.Count > 0)
            {
                while (sortedList.Count > 0)
                {
                    var handlerFacet = sortedList.First();
                    var sortedFacet = handlerFacet.Facet;
                    var sortedCustomComponentAttribute = (UICustomAttribute)sortedFacet.Attribute;
                    var handler = (IViewLayoutHandler) handlerFacet.Handler;
                    var view = handlerFacet.View;

                    sortedList.Remove(handlerFacet);

                    handler.OnChildView += (sender, e) =>
                    {
                        var partialHandlerView = handlerViewList.First(h => h.View.Name == e.PartialViewName);
                        var partialView = partialHandlerView.View;
                        var partialHandler = (IViewLayoutHandler)partialHandlerView.Handler;
                        var objectGraph = e.ModelObjectGraph;
                        var partialEntity = (IEntityWithFacets) objectGraph.Last();

                        partialHandler.ModelObjectGraph = objectGraph;
                        partialHandler.ViewData.AddRange(e.ViewData);

                        this.HandleFacet(partialEntity, null, partialView, partialHandler);

                        e.PartialViewComponentName = partialHandler.PartialComponentName;
                    };

                    if (!this.HandleFacet(entityWithFacets, facet, view, handler))
                    {
                        continue;
                    }

                    currentHandlerStackItem.ViewLayoutHandlers.Add(handler);
                }
            }

            return true;
        }

        /// <summary>   Resets this.  </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>

        public void Reset()
        {
            this.HandlerStack = new Stack<HandlerStackItem>();
            this.HierarchyStack = new Stack<HierarchyStackItem>();
            this.ElementStack = new Stack<XElement>();
            this.LanguageDictionary = new LanguageDictionary();
            this.CustomHandlers = new List<ICustomHandler>();
            this.OtherHandlers = new List<IFacetHandler>();
            this.ForLifeStackItems = new List<SingleHandler>();
            this.Roles = new Dictionary<Guid, string>();
            this.CustomQueries = new QueryDictionary();
            this.AbstraXSchemaDocuments = new Dictionary<string, XDocument>();

            registrySettings.CurrentWorkingDirectory = this.ProjectFolderRoot;
            registrySettings.PackagePathCache = this.PackagePathCache;

            registrySettings.Save();

            moduleAssemblies = new Stack<IModuleAssembly>();
            importHandlers = new Dictionary<ulong, IImportHandler>();

            this.PartsAliasResolver = new PartsAliasResolver();
            this.IndentLevel = 0;
        }

        /// <summary>   Pushes an object onto this stack. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="handlerStackItem"> The handler stack item to push. </param>

        public void Push(HandlerStackItem handlerStackItem)
        {
            if (!handlerStackItem.DoNotFollow)
            {
                this.OtherHandlers.AddRange(handlerStackItem.AsEnumerable());
            }

            handlerStackItem.LogEvent(HandlerStackEvent.Push, this.AppGeneratorEngine.CurrentUIHierarchyPath, this.CurrentPass);
            this.HandlerStack.Push(handlerStackItem);

            if (this.HierarchyStack.Count == 0)
            {
                this.HierarchyStack.Push(string.Empty);
                this.HierarchyStack.Push(handlerStackItem.HierarchyItemName);
            }
            else
            {
                if (handlerStackItem.DoNotFollow)
                {
                    this.HierarchyStack.Push(string.Empty);
                }
                else
                {
                    this.HierarchyStack.Push(handlerStackItem.HierarchyItemName);
                }
            }
        }

        /// <summary>   Creates schema document. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="container">    The container. </param>
        ///
        /// <returns>   The new schema document. </returns>

        private XElement CreateSchemaDocument(IEntityContainer container)
        {
            string name = container.Name;
            XNamespace target = "http://www.cloudideaas.com";
            XNamespace xmlns = "http://www.cloudideaas.com/Schemas";
            XNamespace xs = "http://www.w3.org/2001/XMLSchema";
            XNamespace hydra = "http://www.cloudideaas.com/Hydra/Schemas";
            XNamespace tns = "http://www.cloudideaas.com/Hydra/Schemas";

            var document = new XDocument(
                    new XElement(xs + "schema",
                        new XAttribute("id", "AbstraX"),
                        new XAttribute("elementFormDefault", "qualified"),
                        new XAttribute("targetNamespace", target.NamespaceName + "/" + this.AppName + "/" + container.Name),
                        new XAttribute("xmlns", xmlns.NamespaceName),
                        new XAttribute(XNamespace.Xmlns + "xs", xs.NamespaceName),
                        new XAttribute(XNamespace.Xmlns + "tns", tns.NamespaceName),
                        new XAttribute(XNamespace.Xmlns + "hydra", hydra.NamespaceName)));

            this.ElementStack.Push(document.Root);

            this.AbstraXSchemaDocuments.Add(name, document);

            return document.Root;
        }

        /// <summary>   Saves the schema documents. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>

        private void SaveSchemaDocuments()
        {
            var root = this.ApplicationFolderHierarchy.Root;

            foreach (var pair in this.AbstraXSchemaDocuments)
            {
                var name = pair.Key;
                var document = pair.Value;

                if (this.CurrentPass == GeneratorPass.Files)
                {
                    document.Save(Path.Combine(root.BackSlashes(), name + ".xsd"));
                }
            }
        }

        /// <summary>   Pushes a container. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="container">    The container. </param>
        ///
        /// <returns>   An IDisposable. </returns>

        public IDisposable PushContainer(IEntityContainer container)
        {
            XNamespace xs = "http://www.w3.org/2001/XMLSchema";
            XElement rootElement;
            var element = new XElement(xs + "element",
                new XAttribute("name", container.Name));

            DebugUtils.ThrowIf(this.ElementStack.Count > 0, () => new InvalidOperationException("Generator handler stack has remaining items."));

            rootElement = this.CreateSchemaDocument(container);

            var disposable = container.AsDisposable(() =>
            {
                this.ElementStack.Pop(2);
            });

            rootElement.Add(element);
            this.ElementStack.Push(element);

            return disposable;
        }

        /// <summary>   Pushes an entity set. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="entitySet">    Set the entity belongs to. </param>
        ///
        /// <returns>   An IDisposable. </returns>

        public IDisposable PushEntitySet(IEntitySet entitySet)
        {
            XNamespace xs = "http://www.w3.org/2001/XMLSchema";
            var parentElement = this.ElementStack.Peek().GetSequence();
            var element = new XElement(xs + "element",
                new XAttribute("name", entitySet.Name));
            var sequence = element.GetSequence();

            sequence.Add(new XElement(xs + "element",
                new XAttribute("ref", "tns:" + entitySet.Entities.Single().Name)));

            var disposable = entitySet.AsDisposable(() =>
            {
                this.ElementStack.Pop();
            });

            parentElement.Add(element);
            this.ElementStack.Push(element);

            return disposable;
        }

        /// <summary>   Pushes an entity type. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="entity">   The entity. </param>
        ///
        /// <returns>   An IDisposable. </returns>

        public IDisposable PushEntityType(IEntityType entity)
        {
            XNamespace xs = "http://www.w3.org/2001/XMLSchema";
            var parentElement = this.ElementStack.Last();
            XElement element = new XElement(xs + "element",
                new XAttribute("name", entity.Name));
            IDisposable disposable;

            element = parentElement.FindElement(entity);

            if (element != null)
            {
                this.ElementStack.Push(element);

                disposable = entity.AsDisposable(() =>
                {
                    this.ElementStack.Pop();
                });

                return disposable;
            }

            element = new XElement(xs + "element",
                            new XAttribute("name", entity.Name));

            disposable = entity.AsDisposable(() =>
            {
                this.ElementStack.Pop();
            });

            parentElement.Add(element);
            this.ElementStack.Push(element);

            return disposable;
        }

        /// <summary>   Adds a navigation property. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="property"> The property. </param>

        public void AddNavigationProperty(INavigationProperty property)
        {
            XNamespace xs = "http://www.w3.org/2001/XMLSchema";
            var parentElement = this.ElementStack.Peek().GetSequence();
            XElement element;
            XElement sequence;

            element = parentElement.FindElement(property);

            if (element != null)
            {
                return;
            }

            element = new XElement(xs + "element",
                new XAttribute("name", property.Name));
            sequence = element.GetSequence();
            
            sequence.Add(new XElement(xs + "element",
                new XAttribute("ref", "tns:" + property.ChildEntities.Single().Name)));

            parentElement.Add(element);
        }

        /// <summary>   Adds a property. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="attribute">    The attribute. </param>

        public void AddProperty(ServerInterfaces.IAttribute attribute)
        {
            var parentElement = this.ElementStack.Peek().GetComplexType();
            XNamespace xs = "http://www.w3.org/2001/XMLSchema";
            string typeCodeString;
            XmlTypeCode typeCode;
            XElement attributeElement;

            attributeElement = parentElement.FindAttribute(attribute);

            if (attributeElement != null)
            {
                return;
            }

            typeCode = (XmlTypeCode)attribute.DataType.TypeCode;

            if (typeCode == XmlTypeCode.Id)
            {
                typeCodeString = "ID";
            }
            else
            {
                typeCodeString = typeCode.ToString().ToLower();
            }

            attributeElement = new XElement(xs + "attribute",
                new XAttribute("name", attribute.Name),
                new XAttribute("type", "xs:" + typeCodeString));

            parentElement.Add(attributeElement);
        }

        /// <summary>   Adds the facets. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="entityWithFacets"> The entity with facets. </param>

        public void AddFacets(IEntityWithFacets entityWithFacets)
        {
        }

        /// <summary>   Adds a translation. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="baseObject">   The base object. </param>
        /// <param name="key">          The key. </param>
        /// <param name="value">        The value. </param>
        /// <param name="skipIfSame">   True to skip if same. </param>
        ///
        /// <returns>   A string. </returns>

        public string AddTranslation(ServerInterfaces.IBase baseObject, string key, string value, bool skipIfSame)
        {
            return this.LanguageDictionary.AddTranslation(baseObject, key, value, skipIfSame);
        }

        /// <summary>   Adds a hierarchy property. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="propertyName"> Name of the property. </param>

        public void AddHierarchyProperty(string propertyName)
        {
            var item = this.HierarchyStack.Peek();

            item.Name += string.Format("[@{0}]", propertyName);
        }

        /// <summary>   Begins a child. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="baseObject">   The base object. </param>
        ///
        /// <returns>   An IDisposable. </returns>

        public IDisposable BeginChild(IBase baseObject)
        {
            if (this.RecursionMode != RecursionMode.Winding)
            {
                if (this.RecursionMode == RecursionMode.Unwinding)
                {
                    this.AppGeneratorEngine.WriteLine("\r\n------------------------\r\n", "-".Repeat(10));
                }

                this.RecursionMode = RecursionMode.Winding;
            }

            var popAction = new Action(() =>
            {
                if (!skipDispose)
                {
                    EndChild(baseObject);
                }
            });

            return this.AsDisposable(popAction);
        }

        /// <summary>   Ends a child. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="baseObject">   The base object. </param>

        private void EndChild(IBase baseObject)
        {
            HandlerStackItem item;

            disposing = true;

            if (!baseObject.NoUIConfigOrFollow())
            {
                item = this.HandlerStack.Peek();

                if (this.authorizedRoles.ContainsKey(item))
                {
                    this.authorizedRoles.Remove(item);
                }

                item = this.HandlerStack.Pop();
                this.HierarchyStack.Pop();

                item.LogEvent(HandlerStackEvent.Push, this.AppGeneratorEngine.CurrentUIHierarchyPath, this.CurrentPass);

                if (!item.NoModules && !item.DoNotFollow)
                {
                    HandleModuleAssemblies(baseObject);

                    if (baseObject.Kind == ServerInterfaces.DefinitionKind.StaticContainer)
                    {
                        var parent = baseObject.Parent;

                        while (moduleAssemblies.Count > 0)
                        {
                            HandleModuleAssemblies(parent);
                        }
                    }
                }
            }

            if (this.HierarchyStack.Count == 1 && this.HierarchyStack.Single() == string.Empty)
            {
                this.HierarchyStack.Pop();
                this.RecursionMode = RecursionMode.None;
            }

            disposing = false;
        }

        /// <summary>   Handles the module assemblies described by baseObject. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="baseObject">   The base object. </param>

        private void HandleModuleAssemblies(IBase baseObject)
        {
            IModuleAssembly moduleAssembly;
            Folder folder;

            if (this.GeneratorOptions.PrintMode == PrintMode.PrintUIHierarchyPathAndModuleAssembliesStackOnly)
            {
                string stack;

                if (this.RecursionMode == RecursionMode.Winding)
                {
                    this.AppGeneratorEngine.WriteLine("\r\n---- Unwinding Stack {0}\r\n", "-".Repeat(10));
                    this.RecursionMode = RecursionMode.Unwinding;
                }

                stack = moduleAssemblies.Reverse().Select(a => a.Name).ToDelimitedList(" -> ");
                this.AppGeneratorEngine.WriteLine("\t{0}, Stack: {1}", baseObject.Name, stack);
            }

            moduleAssembly = moduleAssemblies.Pop();
            folder = this.GetAllFolders().Single(f => f.ModuleAssemblies.Any(a => a.Name == moduleAssembly.Name));

            this.AppGeneratorEngine.HandleModuleAssembly(moduleAssembly, folder);
        }

        /// <summary>   Gets the number of stacks. </summary>
        ///
        /// <value> The number of stacks. </value>

        public int StackCount
        {
            get
            {
                return Math.Max(this.HandlerStack.Count, this.HierarchyStack.Count);
            }
        }

        /// <summary>   Gets template parameters. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="content">  The content. </param>
        ///
        /// <returns>   The template parameters. </returns>

        public List<string> GetTemplateParameters(string content)
        {
            var matches = content.RegexGetMatches(@"(?<parameter>\$" + StringExtensions.REGEX_IDENTIFIER_MIDSTRING + @"\$)");

            if (matches != null)
            {
                return matches.Select(m => m.GetGroupValue("parameter")).ToList();
            }

            return new List<string>();
        }

        /// <summary>   Creates business model from template. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="templateFilePath"> Full pathname of the template file. </param>
        /// <param name="appName">          Name of the application. </param>
        /// <param name="organizationName"> Name of the organization. </param>
        ///
        /// <returns>   The new business model from template. </returns>

        public BusinessModel CreateBusinessModelFromTemplate(string templateFilePath, string appName, string appDescription, string organizationName)
        {
            var businessModel = new BusinessModel();
            var textReplacements = new Dictionary<string, string>();

            textReplacements.Add("[AppName]", appName);
            textReplacements.Add("[AppDescription]", appDescription);
            textReplacements.Add("[OrganizationName]", organizationName);

            businessModel.ParseFile(templateFilePath, textReplacements);

            return businessModel;
        }

        /// <summary>   Creates business model from JSON. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="jsonFilePath"> Full pathname of the JSON file. </param>
        ///
        /// <returns>   The new business model from JSON. </returns>

        public BusinessModel CreateBusinessModelFromJson(string jsonFilePath)
        {
            using (var reader = System.IO.File.OpenText(jsonFilePath))
            {
                var businessModel = new BusinessModel();
                var businessModelObject = JsonExtensions.ReadJson<BusinessModelObject>(reader);

                businessModel.TopLevelObject = businessModelObject;

                return businessModel;
            }
        }

        /// <summary>   Creates entity domain model from template. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="templateFilePath"> Full pathname of the template file. </param>
        /// <param name="appName">          Name of the application. </param>
        /// <param name="organizationName"> Name of the organization. </param>
        ///
        /// <returns>   The new entity domain model from template. </returns>

        public EntityDomainModel CreateEntityDomainModelFromTemplate(string templateFilePath, string appName, string appDescription, string organizationName)
        {
            var entityDomainModel = new EntityDomainModel();
            var textReplacements = new Dictionary<string, string>();

            textReplacements.Add("[AppName]", appName);
            textReplacements.Add("[AppDescription]", appDescription);
            textReplacements.Add("[OrganizationName]", organizationName);

            entityDomainModel.ParseFile(templateFilePath, textReplacements);

            return entityDomainModel;
        }

        /// <summary>   Creates entity domain model from JSON file. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="jsonFilePath"> The JSON file. </param>
        ///
        /// <returns>   The new entity domain model from JSON file. </returns>

        public EntityDomainModel CreateEntityDomainModelFromJsonFile(string jsonFilePath)
        {
            using (var reader = System.IO.File.OpenText(jsonFilePath))
            {
                var entityDomainModel = JsonExtensions.ReadJson<EntityDomainModel>(reader);

                return entityDomainModel;
            }
        }

        /// <summary>   Handler, called when the get business model generator. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <returns>   The business model generator handler. </returns>

        public IBusinessModelGeneratorHandler GetBusinessModelGeneratorHandler()
        {
            return this.GetHandler<IBusinessModelGeneratorHandler>();
        }

        /// <summary>   Handler, called when the get entities JSON generator. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <returns>   The entities JSON generator handler. </returns>

        public IEntitiesJsonGeneratorHandler GetEntitiesJsonGeneratorHandler()
        {
            return this.GetHandler<IEntitiesJsonGeneratorHandler>();
        }

        /// <summary>   Handler, called when the get entities model generator. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <returns>   The entities model generator handler. </returns>

        public IEntitiesModelGeneratorHandler GetEntitiesModelGeneratorHandler()
        {
            return this.GetHandler<IEntitiesModelGeneratorHandler>();
        }

        /// <summary>   Handler, called when the get application settings kind. </summary>
        ///
        /// <remarks>   Ken, 10/4/2020. </remarks>
        ///
        /// <param name="appSettingsKind">  The application settings kind. </param>
        ///
        /// <returns>   The application settings kind handler. </returns>

        public IAppSettingsKindHandler GetAppSettingsKindHandler(AppSettingsKind appSettingsKind)
        {
            return this.GetHandler<IAppSettingsKindHandler, AppSettingsKindHandlerAttribute>(a => a.AppSettingsKind == appSettingsKind);
        }

        /// <summary>   Handler, called when the get model augmentation. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <returns>   The model augmentation handler. </returns>

        public IModelAugmentationHandler GetModelAugmentationHandler()
        {
            return this.GetHandler<IModelAugmentationHandler>();
        }

        /// <summary>   Gets a memory module builder. </summary>
        ///
        /// <remarks>   Ken, 10/4/2020. </remarks>
        ///
        /// <returns>   The memory module builder is a sandbox builder to allow runtime reflection-like creation to aid with code generation. </returns>

        public IMemoryModuleBuilder GetMemoryModuleBuilder()
        {
            var builders = new List<IMemoryModuleBuilder>();

            foreach (var type in types.Where(t => !t.IsInterface && t.Implements<IMemoryModuleBuilder>()))
            {
                var handler = (IMemoryModuleBuilder)Activator.CreateInstance(type);

                builders.Add(handler);
            }

            return builders.OrderBy(h => h.Priority).FirstOrDefault();
        }

        /// <summary>   Handler, called when the get. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        ///
        /// <returns>   The handler. </returns>

        public T GetHandler<T>() where T : IHandler 
        {
            var handlers = new List<T>();

            foreach (var type in types.Where(t => !t.IsInterface && t.Implements<T>()))
            {
                var handler = (T)Activator.CreateInstance(type);

                handlers.Add(handler);
            }

            return handlers.OrderBy(h => h.Priority).FirstOrDefault();
        }

        /// <summary>   Handler, called when the get. </summary>
        ///
        /// <remarks>   Ken, 10/4/2020. </remarks>
        ///
        /// <typeparam name="T">            Generic type parameter. </typeparam>
        /// <typeparam name="TAttribute">   Type of the attribute. </typeparam>
        /// <param name="filter">   Specifies the filter. </param>
        ///
        /// <returns>   The handler. </returns>

        public T GetHandler<T, TAttribute>(Func<TAttribute, bool> filter) where T : IHandler where TAttribute : Attribute
        {
            var handlers = new List<T>();

            foreach (var type in types.Where(t => !t.IsInterface && t.Implements<T>() && t.HasCustomAttribute<TAttribute>() && filter(t.GetCustomAttribute<TAttribute>())))
            {
                var handler = (T)Activator.CreateInstance(type);

                handlers.Add(handler);
            }

            return handlers.OrderBy(h => h.Priority).FirstOrDefault();
        }

        /// <summary>   Handler, called when the get. </summary>
        ///
        /// <remarks>   Ken, 10/5/2020. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="filter">   Specifies the filter. </param>
        ///
        /// <returns>   The handler. </returns>

        public T GetHandler<T>(Func<T, bool> filter) where T : IHandler
        {
            var handlers = new List<T>();

            foreach (var type in types.Where(t => !t.IsInterface && t.Implements<T>()))
            {
                var handler = (T)Activator.CreateInstance(type);

                if (filter(handler))
                {
                    handlers.Add(handler);
                }
            }

            return handlers.OrderBy(h => h.Priority).FirstOrDefault();
        }

        /// <summary>   Handler, called when the get workspace file type. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="fileName"> Filename of the file. </param>
        ///
        /// <returns>   The workspace file type handler. </returns>

        public IWorkspaceFileTypeHandler GetWorkspaceFileTypeHandler(string fileName)
        {
            if (workspaceFileTypeHandlers.Count == 0)
            {
                foreach (var type in types.Where(t => !t.IsInterface && t.Implements<IWorkspaceFileTypeHandler>()))
                {
                    var fileTypeHandler = (IWorkspaceFileTypeHandler) Activator.CreateInstance(type);

                    workspaceFileTypeHandlers.Add(fileTypeHandler);
                }
            }

            return workspaceFileTypeHandlers.Where(h => h.FileNameExpressions.Any(e => fileName.RegexIsMatch(e))).OrderBy(h => h.Priority).FirstOrDefault();
        }

        /// <summary>   Handler, called when the get token content. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="token">    The token. </param>
        ///
        /// <returns>   The token content handler. </returns>

        public IWorkspaceTokenContentHandler GetTokenContentHandler(string token)
        {
            if (workspaceTokenContentHandlers.Count == 0)
            {
                foreach (var type in types.Where(t => !t.IsInterface && t.Implements<IWorkspaceTokenContentHandler>()))
                {
                    var tokenContentHandler = (IWorkspaceTokenContentHandler)Activator.CreateInstance(type);

                    workspaceTokenContentHandlers.Add(tokenContentHandler);
                }
            }

            return workspaceTokenContentHandlers.Where(h => h.Tokens.Any(t => t == token)).OrderBy(h => h.Priority).FirstOrDefault();
        }

        /// <summary>   Handles the facets. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="entityWithFacets"> The entity with facets. </param>
        /// <param name="isIdentityEntity"> (Optional) True if is identity, false if not. </param>
        ///
        /// <returns>   A HandlerStackItem. </returns>

        public HandlerStackItem HandleFacets(IEntityWithFacets entityWithFacets, bool isIdentityEntity = false)
        {
            var handlersList = new List<HandlerFacet>();
            var facets = entityWithFacets.Facets.ToList();
            var authorizationState = AuthorizationState.Authorize;
            var setAuthorization = false;
            List<HandlerFacet> sortedList;
            string[] roles = null;
            HandlerStackItem handlerStackItem;
            string name = null;

            if (entityWithFacets is IElementWithFacetHanderTypes)
            {
                var entityWithFacetHandlerTypes = (IElementWithFacetHanderTypes)entityWithFacets;
                var handlerTypes = entityWithFacetHandlerTypes.GetFacetHandlerTypes();

                foreach (var handlerType in handlerTypes)
                {
                    var handler = handlerType.CreateInstance<IFacetHandler>();
                    var handlerFacet = new HandlerFacet(handler, null);

                    if (!this.SuppressDebugOutput)
                    {
                        Debug.WriteLine(handlerFacet.DebugInfo);
                    }

                    handlersList.Add(handlerFacet);
                }
            }

            if (isIdentityEntity)
            {
                this.IdentityEntity = entityWithFacets;
            }

            if (facets.Count == 0)
            {
                if (entityWithFacets is IEntityWithOptionalFacets)
                {
                    var entityWithOptionalFacets = (IEntityWithOptionalFacets)entityWithFacets;

                    if (entityWithOptionalFacets.FollowWithout)
                    {
                        var attributeType = typeof(Attribute);
                        var handlerTypes = types.Where(t => t.IsFacetHandlerType(entityWithFacets.Kind));

                        foreach (var handlerType in handlerTypes)
                        {
                            IFacetHandler handler;

                            if (handlerType.Implements<ISingletonForLifeFacetHandler>())
                            {
                                if (unityContainer.IsRegistered(handlerType))
                                {
                                    handler = (IFacetHandler)unityContainer.Resolve(handlerType);
                                }
                                else
                                {
                                    handler = handlerType.CreateInstance<IFacetHandler>();
                                    unityContainer.RegisterInstance(handlerType, handler);
                                }
                            }
                            else
                            {
                                handler = handlerType.CreateInstance<IFacetHandler>();
                            }

                            if (CompareExtensions.AnyAreFalse(entityWithOptionalFacets.NoUIOrConfig, handler.FacetHandlerLayer.IsOneOf(FacetHandlerLayer.ServerConfig, FacetHandlerLayer.Client)))
                            {
                                var handlerFacet = new HandlerFacet(handler, null);

                                if (!this.SuppressDebugOutput)
                                {
                                    Debug.WriteLine(handlerFacet.DebugInfo);
                                }

                                handlersList.Add(handlerFacet);
                            }
                        }
                    }
                    else
                    {
                        return new DoNotFollowHandler(entityWithFacets, "Facet count is zero. IEntityWithOptionalFacets.FollowWithout = false").LogCreate(this.AppGeneratorEngine.CurrentUIHierarchyPath, this.CurrentPass);
                    }
                }
                else
                {
                    return new DoNotFollowHandler(entityWithFacets, "Facet count is zero. Not an IEntityWithOptionalFacets").LogCreate(this.AppGeneratorEngine.CurrentUIHierarchyPath, this.CurrentPass);
                }
            }

            foreach (var facet in facets)
            {
                var attribute = facet.Attribute;

                if (attribute is AuthorizeAttribute)
                {
                    var authorizeAttribute = (AuthorizeAttribute)attribute;

                    if (authorizeAttribute.State == AuthorizationState.Authorize)
                    {
                        roles = authorizeAttribute.Roles.Split(",");
                        setAuthorization = true;
                    }
                    else
                    {
                        authorizationState = authorizeAttribute.State;
                    }
                }
                else if (attribute is UINavigationNameAttribute)
                {
                    var navigationNameAttribute = (UINavigationNameAttribute)attribute;

                    if (navigationNameAttribute.UIHierarchyPath != null)
                    {
                        if (this.HierarchyPath == navigationNameAttribute.UIHierarchyPath)
                        {
                            name = navigationNameAttribute.Name;
                        }
                    }
                    else
                    {
                        name = navigationNameAttribute.Name;
                    }
                }
                else if (attribute is UIAttribute)
                {
                    var componentAttribute = (UIAttribute)attribute;

                    if (componentAttribute.PathRootAlias != null)
                    {
                        this.PartsAliasResolver.Add(componentAttribute);
                    }

                    if (componentAttribute.UIHierarchyPath != null)
                    {
                        if (!isIdentityEntity)
                        {
                            var queue = componentAttribute.ParseHierarchyPath(this.PartsAliasResolver);
                            var parts = queue.SplitElementParts().ToList();
                            var count = parts.Count;

                            if (this.StackCount == count)
                            {
                                var x = 1;

                                foreach (var part in parts.Take(count - 1))
                                {
                                    var stackElement = this.HierarchyStack.Reverse().ElementAt(x);

                                    if (stackElement.Name != part)
                                    {
                                        return new DoNotFollowHandler(entityWithFacets, $"For UIHierarchyPath '{ componentAttribute.UIHierarchyPath }', Hierarchy StackElement[{ x }].Name '{ stackElement.Name }' does not equal path part '{ part }'").LogCreate(this.AppGeneratorEngine.CurrentUIHierarchyPath, this.CurrentPass);
                                    }

                                    x++;
                                }
                            }
                            else
                            {
                                return new DoNotFollowHandler(entityWithFacets, $"For UIHierarchyPath '{ componentAttribute.UIHierarchyPath }', part count { count } does not equal Hierarchy Stack count of { this.StackCount }").LogCreate(this.AppGeneratorEngine.CurrentUIHierarchyPath, this.CurrentPass);
                            }
                        }
                    }
                }
                else if (attribute is ScaffoldColumnAttribute)
                {
                    var scaffoldColumnAttribute = (ScaffoldColumnAttribute)attribute;

                    if (!scaffoldColumnAttribute.Scaffold)
                    {
                        return new DoNotFollowHandler(entityWithFacets, "scaffoldColumnAttribute.Scaffold = false").LogCreate(this.AppGeneratorEngine.CurrentUIHierarchyPath, this.CurrentPass);
                    }
                }
                else if (attribute is QueryPathAttribute)
                {
                    var queryPathAttribute = (QueryPathAttribute)attribute;
                    var queryPathQueue = queryPathAttribute.GetQueryPathQueue(entityWithFacets);
                    var controllerMethod = queryPathQueue.SuggestedControllerMethodName;
                    var codeExpression = queryPathQueue.QueryCode;
                    var code = codeExpression.Replace(new Dictionary<string, string> { { "containerVariable", "entities" }, { "identityNameVariable", "userName" } });

                    // not ready, not reasonable?

                    DebugUtils.Break();
                }
                else if (attribute is CustomQueryAttribute)
                {
                    var customQueryAttribute = (CustomQueryAttribute)attribute;
                    var controllerMethod = customQueryAttribute.ControllerMethodName;
                    var resourcesType = customQueryAttribute.ResourcesType;
                    var resources = (IAppResources) Activator.CreateInstance(resourcesType);
                    var queries = resources.GetQueries();
                    var queryInfo = queries.Single(q => q.ServiceControllerMethodName == controllerMethod);

                    queryInfo.QueryKind = customQueryAttribute.QueryKind;
                    queryInfo.SourceEntity = entityWithFacets;

                    this.CustomQueries.AddToDictionaryListCreateIfNotExist(entityWithFacets, queryInfo);
                }

                if (attribute is IFacetHandlerKindAttribute)
                {
                    var attributeType = types.Single(t => t.FullName == facet.ID);
                    var kindAttribute = (IFacetHandlerKindAttribute)attribute;
                    var kind = kindAttribute.Kind;
                    var handlerTypes = types.Where(t => t.IsFacetHandlerType(attributeType, entityWithFacets.Kind, kind));

                    foreach (var handlerType in handlerTypes)
                    {
                        IFacetHandler handler;
                        HandlerFacet handlerFacet;

                        if (handlerType.Implements<ISingletonForLifeFacetHandler>())
                        {
                            if (unityContainer.IsRegistered(handlerType))
                            {
                                handler = (IFacetHandler) unityContainer.Resolve(handlerType);
                            }
                            else
                            {
                                handler = handlerType.CreateInstance<IFacetHandler>();
                                unityContainer.RegisterInstance(handlerType, handler);
                            }
                        }
                        else
                        {
                            handler = handlerType.CreateInstance<IFacetHandler>();
                        }

                        handlerFacet = new HandlerFacet(handler, facet);

                        if (!this.SuppressDebugOutput)
                        {
                            Debug.WriteLine(handlerFacet.DebugInfo);
                        }

                        handlersList.Add(handlerFacet);
                    }
                }
            }

            if (name == null)
            {
                name = entityWithFacets.Name;
            }

            sortedList = handlersList.OrderBy(h => h.Handler.Priority).DistinctBy(h => h.Handler.GetType().FullName).ToList();

            handlerStackItem = HandlerStackItem.Create(entityWithFacets, name, roles, sortedList.Select(h => (IFacetHandler) h.Handler).ToArray()).LogCreate(this.AppGeneratorEngine.CurrentUIHierarchyPath, this.CurrentPass);

            if (authorizationState != AuthorizationState.Authorize)
            {
                var lastAuthorizedRolePair = this.authorizedRoles.Last();
                var lastAuthorizedStackItem = lastAuthorizedRolePair.Key;

                if (authorizationState == AuthorizationState.RestoreAuthorization)
                {
                    lastAuthorizedStackItem.AuthorizationState = AuthorizationState.Authorize;
                    setAuthorization = true;
                }
                else
                {
                    lastAuthorizedStackItem.AuthorizationState = authorizationState;
                }
            }

            if (setAuthorization)
            {
                if (handlerStackItem.Roles != null)
                {
                    this.Roles.AddRange(handlerStackItem.Roles.Where(r => !this.Roles.Values.Any(r2 => r == r2)).ToDictionary(r => Guid.NewGuid(), r => r));
                    this.authorizedRoles.Add(handlerStackItem, handlerStackItem.Roles);
                }
            }

            if (sortedList.Count > 0)
            {
                while (sortedList.Count > 0)
                {
                    var handlerFacet = sortedList.First();
                    var handler = handlerFacet.Handler;
                    var facet = handlerFacet.Facet;

                    sortedList.Remove(handlerFacet);

                    if (!this.HandleFacet(entityWithFacets, facet, (IFacetHandler) handler))
                    {
                        return new DoNotFollowHandler(entityWithFacets, $"HandleFacet for { handler.GetType().Name } returned false").LogCreate(this.AppGeneratorEngine.CurrentUIHierarchyPath, this.CurrentPass);
                    }
                }
            }
            else
            {
                if (entityWithFacets is IEntityWithOptionalFacets)
                {
                    var entityWithOptionalFacets = (IEntityWithOptionalFacets)entityWithFacets;

                    if (entityWithOptionalFacets.FollowWithout)
                    {
                        return new FollowHandler(entityWithFacets, "IEntityWithOptionalFacets.FollowWithout == true").LogCreate(this.AppGeneratorEngine.CurrentUIHierarchyPath, this.CurrentPass);
                    }
                }

                return new DoNotFollowHandler(entityWithFacets, $"Handler count is zero").LogCreate(this.AppGeneratorEngine.CurrentUIHierarchyPath, this.CurrentPass);
            }

            return handlerStackItem;
        }

        /// <summary>   Handles the module kind. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="moduleObject"> The module object. </param>
        /// <param name="folder">       Pathname of the folder. </param>
        /// <param name="moduleKind">   An enum constant representing the module kind option. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public bool HandleModuleKind(IModuleObject moduleObject, Folder folder, Enum moduleKind)
        {
            var handlersList = new List<IModuleKindHandler>();
            var baseObject = moduleObject.BaseObject;
            var definitionKind = baseObject.Kind;
            var uiComponentAttribute = moduleObject.GetUIAttribute(folder);
            List<IModuleKindHandler> sortedList;
            IEnumerable<Type> handlerTypes;

            if (uiComponentAttribute != null)
            {
                var kindType = typeof(UIKind);
                var kindField = kindType.GetFields().Single(f => f.Name != "value__" && ((UIKind)f.GetRawConstantValue()) == uiComponentAttribute.UIKind);
                var kindGuidAttribute = kindField.GetCustomAttribute<KindGuidAttribute>();

                handlerTypes = types.Where(t => t.IsModuleKindType(moduleKind, definitionKind, uiComponentAttribute.UILoadKind, uiComponentAttribute.Kind, kindGuidAttribute.FeatureKind));
            }
            else
            {
                handlerTypes = types.Where(t => t.IsModuleKindType(moduleKind, definitionKind, UIFeatureKind.None));
            }

            foreach (var handlerType in handlerTypes)
            {
                var handler = handlerType.CreateInstance<IModuleKindHandler>();

                handlersList.Add(handler);
            }

            sortedList = handlersList.OrderBy(h => h.Priority).ToList();

            if (sortedList.Count == 0)
            {
                DebugUtils.Break();
            }

            while (sortedList.Count > 0)
            {
                var handler = sortedList.First();

                sortedList.Remove(handler);

                if (!handler.Process(moduleKind, moduleObject, folder, this))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>   Handles the facet. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="entityWithFacets"> The entity with facets. </param>
        /// <param name="facet">            The facet. </param>
        /// <param name="view">             The view. </param>
        /// <param name="handler">          The handler. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        private bool HandleFacet(IEntityWithFacets entityWithFacets, ServerInterfaces.Facet facet, IView view, IViewLayoutHandler handler)
        {
            if (this.StackItems.PreProcess(entityWithFacets, this, handler))
            {
                if (!handler.Process(entityWithFacets, facet, view, this))
                {
                    return false;
                }

                if (!this.StackItems.PostProcess(entityWithFacets, this, handler))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>   Handles the facet. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="entityWithFacets"> The entity with facets. </param>
        /// <param name="facet">            The facet. </param>
        /// <param name="handler">          The handler. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        private bool HandleFacet(IEntityWithFacets entityWithFacets, ServerInterfaces.Facet facet, IFacetHandler handler)
        {
            if (this.StackItems.PreProcess(entityWithFacets, this, handler))
            {
                handler.ProcessFacets += (sender, e) =>
                {
                    foreach (var type in e.Types)
                    {
                        var processHandler = type.CreateInstance<IFacetHandler>();

                        HandleFacet(entityWithFacets, facet, processHandler);
                    }
                };

                if (handler.ForLife)
                {
                    if (handler is ISingletonForLifeFacetHandler)
                    {
                        if (!this.ForLifeStackItems.Any(i => i.Handler == handler))
                        {
                            this.ForLifeStackItems.Add(new SingleHandler(entityWithFacets, "ForLife", null, handler).LogCreate<SingleHandler>(this.AppGeneratorEngine.CurrentUIHierarchyPath, this.CurrentPass));
                        }
                    }
                    else
                    {
                        this.ForLifeStackItems.Add(new SingleHandler(entityWithFacets, "ForLife", null, handler).LogCreate<SingleHandler>(this.AppGeneratorEngine.CurrentUIHierarchyPath, this.CurrentPass));
                    }
                }

                if (!handler.Process(entityWithFacets, facet, this))
                {
                    return false;
                }

                if (!this.StackItems.PostProcess(entityWithFacets, this, handler))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>   Pushes a module assembly. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="namePrefix">   The name prefix. </param>
        ///
        /// <returns>   An IModuleAssembly. </returns>

        public IModuleAssembly PushModuleAssembly<T>(string namePrefix) where T : IModuleAssembly, new()
        {
            var moduleAssembly = new T();

            moduleAssembly.Name = namePrefix + "Module";
            moduleAssembly.RoutingName = namePrefix + "RoutingModule";
            moduleAssemblies.Push(moduleAssembly);

            return moduleAssembly;
        }

        /// <summary>   Sets module assembly properties. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="properties">   The properties. </param>

        public void SetModuleAssemblyProperties(IModuleAssemblyProperties properties)
        {
            var moduleAssembly = moduleAssemblies.Peek();

            properties.Configuration = this;
            properties.UpdateModuleAssembly(moduleAssembly);
        }

        /// <summary>   Sets module assembly folder. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="folder">   Pathname of the folder. </param>

        public void SetModuleAssemblyFolder(Folder folder)
        {
            var moduleAssembly = moduleAssemblies.Peek();

            folder.AddAssembly(moduleAssembly);
        }

        /// <summary>   Creates a file. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="fileInfo">             Information describing the file. </param>
        /// <param name="nonAssemblyModules">   The non assembly modules. </param>
        /// <param name="output">               The output. </param>
        /// <param name="fileKind">             The file kind. </param>
        /// <param name="hierarchyGenerator">   The hierarchy generator. </param>
        ///
        /// <returns>   The new file. </returns>

        public FolderStructure.File CreateFile(System.IO.FileInfo fileInfo, IEnumerable<Module> nonAssemblyModules, string output, FileKind fileKind, Func<StringBuilder> hierarchyGenerator)
        {
            var file = this.CreateFile(fileInfo, output, fileKind, hierarchyGenerator);

            file.SetNonAssemblyModulesDefaultFile(nonAssemblyModules);

            return file;
        }

        /// <summary>   Creates a file. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="filePath"> Full pathname of the file. </param>
        ///
        /// <returns>   The new file. </returns>

        public Stream CreateFile(string filePath)
        {
            if (!this.NoFileCreation || filePath.EndsWith(@"\src\app\app.module.ts"))
            {
                return System.IO.File.Create(filePath);
            }
            else
            {
                return new MemoryStream();
            }
        }

        /// <summary>   Creates a directory. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="directoryPath">    Full pathname of the directory file. </param>
        ///
        /// <returns>   The new directory. </returns>

        public DirectoryInfo CreateDirectory(string directoryPath)
        {
            if (!this.NoFileCreation || directoryPath.EndsWith(@"\src\app"))
            {
                return Directory.CreateDirectory(directoryPath);
            }
            else
            {
                return null;
            }
        }

        /// <summary>   Creates a file. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="fileInfo">             Information describing the file. </param>
        /// <param name="output">               The output. </param>
        /// <param name="fileKind">             The file kind. </param>
        /// <param name="hierarchyGenerator">   The hierarchy generator. </param>
        ///
        /// <returns>   The new file. </returns>

        public FolderStructure.File CreateFile(System.IO.FileInfo fileInfo, string output, FileKind fileKind, Func<StringBuilder> hierarchyGenerator)
        {
            var pass = this.CurrentPass;
            var filePath = fileInfo.FullName;
            var fileLocation = fileInfo.DirectoryName;
            FolderStructure.File file;

            if (pass == GeneratorPass.Files)
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                if (!Directory.Exists(fileLocation))
                {
                    CreateDirectory(fileLocation);
                }

                using (var fileStream = this.CreateFile(filePath))
                {
                    fileStream.Write(output);
                    this.FileSystem.DeleteFile(filePath);

                    if (fileKind == FileKind.Project)
                    {
                        file = this.FileSystem.AddSystemLocalProjectFile(new FileInfo(filePath));
                    }
                    else if (fileKind == FileKind.Services)
                    {
                        file = this.FileSystem.AddSystemLocalServicesFile(new FileInfo(filePath));
                    }
                    else
                    {
                        file = null;
                        DebugUtils.Break();
                    }

                    file.Length = output.Length;
                    file.Hash = output.GetHashCode();

                    return file;
                }
            }
            else if (pass == GeneratorPass.HierarchyOnly)
            {
                if (fileKind == FileKind.Project)
                {
                    file = this.FileSystem.AddSystemLocalProjectFile(new FileInfo(filePath), hierarchyGenerator());
                }
                else if (fileKind == FileKind.Services)
                {
                    file = this.FileSystem.AddSystemLocalServicesFile(new FileInfo(filePath), hierarchyGenerator());
                }
                else
                {
                    file = null;
                    DebugUtils.Break();
                }

                return file;
            }
            else
            {
                file = null;
                DebugUtils.Break();

                return file;
            }
        }

        /// <summary>   Creates a file. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="fileInfo">         Information describing the file. </param>
        /// <param name="output">           The output. </param>
        /// <param name="fileKind">         The file kind. </param>
        /// <param name="moduleAssembly">   The module assembly. </param>
        ///
        /// <returns>   The new file. </returns>

        public FolderStructure.File CreateFile(FileInfo fileInfo, string output, FileKind fileKind, IModuleAssembly moduleAssembly)
        {
            var pass = this.CurrentPass;
            var filePath = fileInfo.FullName;
            var fileLocation = fileInfo.DirectoryName;
            FolderStructure.File file;

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            if (!Directory.Exists(fileLocation))
            {
                CreateDirectory(fileLocation);
            }

            using (var fileStream = this.CreateFile(filePath))
            {
                fileStream.Write(output);
                this.FileSystem.DeleteFile(filePath);

                file = this.FileSystem.AddSystemLocalProjectFile(new FileInfo(filePath));
                moduleAssembly.File = file;

                return file;
            }
        }

        /// <summary>   Creates a file. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="moduleAssemblyProperties"> The module assembly properties. </param>
        /// <param name="fileInfo">                 Information describing the file. </param>
        /// <param name="output">                   The output. </param>
        /// <param name="fileKind">                 The file kind. </param>
        /// <param name="hierarchyGenerator">       The hierarchy generator. </param>
        ///
        /// <returns>   The new file. </returns>

        public FolderStructure.File CreateFile(IModuleAssemblyProperties moduleAssemblyProperties, System.IO.FileInfo fileInfo, string output, FileKind fileKind, Func<StringBuilder> hierarchyGenerator)
        {
            var pass = this.CurrentPass;
            var filePath = fileInfo.FullName;
            var fileLocation = fileInfo.DirectoryName;
            FolderStructure.File file;

            this.SetModuleAssemblyProperties(moduleAssemblyProperties);

            if (pass == GeneratorPass.Files)
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                if (!Directory.Exists(fileLocation))
                {
                    CreateDirectory(fileLocation);
                }

                using (var fileStream = this.CreateFile(filePath))
                {
                    fileStream.Write(output);
                    this.FileSystem.DeleteFile(filePath);

                    file = this.FileSystem.AddSystemLocalProjectFile(new FileInfo(filePath));

                    if (!fileKind.HasFlag(FileKind.ProjectNoModule))
                    {
                        SetModuleAssemblyFolder(file.Folder);
                    }

                    moduleAssemblyProperties.AddDefaultFile(file);

                    return file;
                }
            }
            else if (pass == GeneratorPass.HierarchyOnly)
            {
                file = this.FileSystem.AddSystemLocalProjectFile(new FileInfo(filePath), hierarchyGenerator());

                if (!fileKind.HasFlag(FileKind.ProjectNoModule))
                {
                    SetModuleAssemblyFolder(file.Folder);
                }

                moduleAssemblyProperties.AddDefaultFile(file);

                return file;
            }
            else
            {
                file = null;
                DebugUtils.Break();

                return file;
            }
        }

        /// <summary>   Gets the stack items. </summary>
        ///
        /// <value> The stack items. </value>

        public IEnumerable<HandlerStackItem> StackItems
        {
            get
            {
                return this.ForLifeStackItems.Concat(this.HandlerStack);
            }
        }

        /// <summary>   Gets a list of types of the graph qls. </summary>
        ///
        /// <value> A list of types of the graph qls. </value>

        public List<string> GraphQLTypes
        {
            get
            {
                var graphQLTypes = typeof(GraphType).Assembly.GetTypes().Where(t => t.InheritsFrom(typeof(GraphType)));

                return graphQLTypes.Select(t => t.Name).ToList();
            }
        }

        /// <summary>   Enumerates create imports in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
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

        public IEnumerable<ModuleImportDeclaration> CreateImports(IModuleKindHandler moduleKindHandler, IModuleAssembly moduleAssembly, Folder folder, bool includeSelf = false, int subFolderCount = 0)
        {
            var modulesOrAssemblies = new List<IModuleOrAssembly>();

            return CreateImports(moduleKindHandler, moduleAssembly, modulesOrAssemblies, folder, includeSelf, subFolderCount);
        }

        /// <summary>   Enumerates create imports in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
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

        public IEnumerable<ModuleImportDeclaration> CreateImports(IModuleKindHandler moduleKindHandler, IModuleAssembly moduleAssembly, List<IModuleOrAssembly> modulesOrAssemblies, Folder folder, bool includeSelf = false, int subFolderCount = 0)
        {
            var baseObject = moduleAssembly.BaseObject;
            var folders = this.GetAllFolders();
            var declarations = new List<ModuleImportDeclaration>();

            if (includeSelf)
            {
                moduleKindHandler.AddRange(this, moduleAssembly, modulesOrAssemblies, folders.SelectMany(f => f.ModuleAssemblies).Where(m => m.HasExports(baseObject)).SelectMany(m => m.GetExports(baseObject)), ModuleAddType.Self);
            }

            if (baseObject.Kind == ServerInterfaces.DefinitionKind.Model && moduleAssembly.File != null)
            {
                moduleKindHandler.AddRange(this, moduleAssembly, modulesOrAssemblies, folders.SelectMany(f => f.ModuleAssemblies).SelectMany(m => m.GetExports("Page")), ModuleAddType.Surrogate);
            }
            else
            {
                foreach (var childObject in baseObject.GetFollowingChildren(PartsAliasResolver))
                {
                    moduleKindHandler.AddRange(this, moduleAssembly, modulesOrAssemblies, folders.SelectMany(f => f.ModuleAssemblies).Where(m => m.HasExports(childObject)), ModuleAddType.Children);
                }
            }

            if (baseObject is NavigationProperty)
            {
                var element = (ServerInterfaces.IParentBase)baseObject;
                var elementChild = element.ChildElements.Single();

                foreach (var childObject in elementChild.GetFollowingChildren(PartsAliasResolver))
                {
                    moduleKindHandler.AddRange(this, moduleAssembly, modulesOrAssemblies, folders.SelectMany(f => f.ModuleAssemblies).Where(m => m.HasExports(childObject)), ModuleAddType.Grandchildren);
                }
            }

            if (moduleKindHandler is IModuleAddModuleKindHandler)
            {
                var moduleAddModuleKindHandler = (IModuleAddModuleKindHandler)moduleKindHandler;

                moduleAddModuleKindHandler.AddRange(this, moduleAssembly, modulesOrAssemblies, ModuleAddType.Custom, (m) =>
                {
                    ServerInterfaces.IBase moduleOrAssemblyBaseObject;

                    if (m is Module)
                    {
                        moduleOrAssemblyBaseObject = ((Module) m).BaseObject;
                    }
                    else if (m is IModuleAssembly)
                    {
                        moduleOrAssemblyBaseObject = ((IModuleAssembly)m).BaseObject;
                    }
                    else
                    {
                        moduleOrAssemblyBaseObject = null;
                        DebugUtils.Break();
                    }

                    if (moduleOrAssemblyBaseObject != null)
                    {
                        if (moduleOrAssemblyBaseObject.Name == baseObject.Name)
                        {
                            return true;
                        }

                        if (baseObject is NavigationProperty)
                        {
                            var element = (ServerInterfaces.IParentBase)baseObject;
                            var elementChild = element.ChildElements.Single();

                            if (moduleOrAssemblyBaseObject.Name == elementChild.Name)
                            {
                                return true;
                            }

                            if (elementChild.Parent != null)
                            {
                                if (moduleOrAssemblyBaseObject.Name == elementChild.Parent.Name)
                                {
                                    return true;
                                }
                            }

                            if (moduleOrAssemblyBaseObject.Parent != null)
                            {
                                if (moduleOrAssemblyBaseObject.Parent.Name == elementChild.Name)
                                {
                                    return true;
                                }
                            }
                        }

                        if (baseObject.Parent != null)
                        {
                            if (moduleOrAssemblyBaseObject.Name == baseObject.Parent.Name)
                            {
                                return true;
                            }
                        }

                        if (moduleOrAssemblyBaseObject.Parent != null)
                        {
                            if (moduleOrAssemblyBaseObject.Parent.Name == baseObject.Name)
                            {
                                return true;
                            }
                        }
                    }

                    return false;
                });
            }

            foreach (var moduleOrAssembly in modulesOrAssemblies)
            {
                if (moduleOrAssembly is IModuleAssembly)
                {
                    var importModuleAssembly = (IModuleAssembly)moduleOrAssembly;
                    var declaration = importModuleAssembly.CreateImportDeclaration(folder, subFolderCount);

                    declarations.Add(declaration);
                }
                else if (moduleOrAssembly is Module)
                {
                    var importModule = (Module)moduleOrAssembly;
                    var declaration = importModule.CreateImportDeclaration(folder, subFolderCount);

                    declarations.Add(declaration);
                }
                else
                {
                    DebugUtils.Break();
                }
            }

            return declarations;
        }

        /// <summary>   Enumerates create imports in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
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

        public IEnumerable<ModuleImportDeclaration> CreateImports(IModuleHandler baseHandler, ServerInterfaces.IBase baseObject, Folder folder, bool includeSelf = false, int subFolderCount = 0)
        {
            var handlerAttribute = baseHandler.GetType().GetCustomAttribute<HandlerAttribute>();
            var importGuids = handlerAttribute.Imports.SplitImports();
            var handlers = new List<IImportHandler>();
            var declarations = new List<ModuleImportDeclaration>();
            var builtInImportHandlerList = new List<IBuiltInImportHandler>();
            IBuiltInImportHandler builtInImportHandler = null;

            foreach (var importGuid in importGuids)
            {
                var firstId = ulong.Parse(string.Join(string.Empty, importGuid.ToString().Split("-").Take(3)), System.Globalization.NumberStyles.AllowHexSpecifier);
                var lastId = ulong.Parse(string.Join(string.Empty, importGuid.ToString().Split("-").Skip(3).Take(2)), System.Globalization.NumberStyles.AllowHexSpecifier);
                var handlerTypes = types.Where(t => t.IsImportHandlerType(firstId));
                var handlerType = handlerTypes.Single();
                var handler = importHandlers.AddToDictionaryIfNotExist(firstId, () => handlerType.CreateInstance<IImportHandler>());

                if (handler is IBuiltInImportHandler)
                {
                    builtInImportHandler = (IBuiltInImportHandler)handler;

                    builtInImportHandler.AddBuiltInImport(lastId, builtInModules, folder, subFolderCount);
                    builtInImportHandlerList.Add(builtInImportHandler);
                }
                else
                {
                    handler.AddImport(lastId);
                }

                if (!handlers.Contains(handler))
                {
                    handlers.Add(handler);
                }
            }

            foreach (var handler in handlers)
            {
                foreach (var declaration in handler.GetDeclarations())
                {
                    if (!declarations.Any(d => d.DeclarationCode == declaration.DeclarationCode))
                    {
                        declarations.Add(declaration);
                    }
                }
            }

            if (this.CurrentPass == GeneratorPass.Files)
            {
                AddInternalImports(baseHandler, baseObject, folder, subFolderCount, declarations, includeSelf);
            }

            builtInImportHandlerList.ClearDeclarations();

            return declarations;
        }

        /// <summary>   Creates import groups. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <exception cref="HandlerNotFoundException"> Thrown when a Handler Not Found error condition
        ///                                             occurs. </exception>
        ///
        /// <param name="facetHandler"> The facet handler. </param>
        /// <param name="baseObject">   The base object. </param>
        /// <param name="folder">       Pathname of the folder. </param>
        /// <param name="includeSelf">  (Optional) True to include, false to exclude the self. </param>
        ///
        /// <returns>   The new import groups. </returns>

        public IDictionary<string, IEnumerable<ModuleImportDeclaration>> CreateImportGroups(IFacetHandler facetHandler, ServerInterfaces.IBase baseObject, Folder folder, bool includeSelf = false)
        {
            var facetHandlerAttribute = facetHandler.GetType().GetCustomAttribute<FacetHandlerAttribute>();
            var importGroupAttributes = facetHandler.GetType().GetCustomAttributes<ImportGroupAttribute>();
            var declarationGroups = new Dictionary<string, IEnumerable<ModuleImportDeclaration>>();

            foreach (var importGroup in importGroupAttributes)
            {
                var handlers = new List<IImportHandler>();
                var importGuids = importGroup.Imports.SplitImports();
                var groupName = importGroup.GroupName;
                List<ModuleImportDeclaration> declarations;
                var builtInImportHandlerList = new List<IBuiltInImportHandler>();

                if (declarationGroups.ContainsKey(groupName))
                {
                    declarations = (List<ModuleImportDeclaration>)declarationGroups[groupName];
                }
                else
                {
                    declarations = new List<ModuleImportDeclaration>();
                    declarationGroups.Add(groupName, declarations);
                }

                foreach (var importGuid in importGuids)
                {
                    var firstId = ulong.Parse(string.Join(string.Empty, importGuid.ToString().Split("-").Take(3)), System.Globalization.NumberStyles.AllowHexSpecifier);
                    var lastId = ulong.Parse(string.Join(string.Empty, importGuid.ToString().Split("-").Skip(3).Take(2)), System.Globalization.NumberStyles.AllowHexSpecifier);
                    var handlerTypes = types.Where(t => t.IsImportHandlerType(firstId));
                    var handlerType = handlerTypes.SingleOrDefault();
                    IBuiltInImportHandler builtInImportHandler = null;
                    IImportHandler handler;

                    if (handlerType == null)
                    {
                        var field = types.Where(t => t.HasCustomAttribute<ModuleImportsAttribute>()).Select(t => t.GetConstant<ulong>(l => l == firstId)).Single();
                        var name = field.Name;

                        throw new HandlerNotFoundException(name);
                    }

                    handler = importHandlers.AddToDictionaryIfNotExist(firstId, () => handlerType.CreateInstance<IImportHandler>());

                    if (handler is IBuiltInImportHandler)
                    {
                        builtInImportHandler = (IBuiltInImportHandler)handler;

                        builtInImportHandler.AddBuiltInImport(lastId, builtInModules, folder, importGroup.SubFolderCount);
                        builtInImportHandlerList.Add(builtInImportHandler);
                    }
                    else
                    {
                        handler.AddImport(lastId);
                    }

                    if (!handlers.Contains(handler))
                    {
                        handlers.Add(handler);
                    }
                }

                foreach (var handler in handlers)
                {
                    foreach (var declaration in handler.GetDeclarations())
                    {
                        if (!declarations.Any(d => d.DeclarationCode == declaration.DeclarationCode))
                        {
                            declarations.Add(declaration);
                        }
                    }
                }

                if (this.CurrentPass == GeneratorPass.Files)
                {
                    AddInternalImports(facetHandler, baseObject, folder, importGroup.SubFolderCount, declarations, includeSelf);
                }

                builtInImportHandlerList.ClearDeclarations();
            }

            return declarationGroups;
        }

        /// <summary>   Adds a package installs. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="facetHandler"> The facet handler. </param>

        public void AddPackageInstalls(IFacetHandler facetHandler)
        {
            var facetHandlerAttribute = facetHandler.GetType().GetCustomAttribute<FacetHandlerAttribute>();
            var importGuids = facetHandlerAttribute.Imports.SplitImports();
            var handlers = new List<IImportHandler>();
            var declarations = new List<ModuleImportDeclaration>();

            foreach (var importGuid in importGuids)
            {
                var firstId = ulong.Parse(string.Join(string.Empty, importGuid.ToString().Split("-").Take(3)), System.Globalization.NumberStyles.AllowHexSpecifier);
                var lastId = ulong.Parse(string.Join(string.Empty, importGuid.ToString().Split("-").Skip(3).Take(2)), System.Globalization.NumberStyles.AllowHexSpecifier);
                var handlerTypes = types.Where(t => t.IsImportHandlerType(firstId));
                var handlerType = handlerTypes.Single();
                var handler = importHandlers.AddToDictionaryIfNotExist(firstId, () => handlerType.CreateInstance<IImportHandler>());

                handler.AddImport(lastId);

                if (!handlers.Contains(handler))
                {
                    handlers.Add(handler);
                }
            }

            if (facetHandlerAttribute.PackageTypes.Length > 0)
            {
                var id = GeneratorConfiguration.APPLICATION_IMPORT_HANDLER_ID;
                var handlerTypes = types.Where(t => t.IsImportHandlerType(id));
                var handlerType = handlerTypes.Single();
                var applicationImportHandler = importHandlers.AddToDictionaryIfNotExist(id, () => handlerType.CreateInstance<IImportHandler>());

                foreach (var packageType in facetHandlerAttribute.PackageTypes)
                {
                    var package = packageType.CreateInstance<Package>();

                    applicationImportHandler.Packages.Add(packageType.Name, package);
                }
            }
        }

        /// <summary>   Adds an internal imports. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="baseHandler">      The base handler. </param>
        /// <param name="baseObject">       The base object. </param>
        /// <param name="folder">           Pathname of the folder. </param>
        /// <param name="subFolderCount">   Number of sub folders. </param>
        /// <param name="declarations">     The declarations. </param>
        /// <param name="includeSelf">      True to include, false to exclude the self. </param>

        private void AddInternalImports(IModuleHandler baseHandler, ServerInterfaces.IBase baseObject, Folder folder, int subFolderCount, List<ModuleImportDeclaration> declarations, bool includeSelf)
        {
            var folders = this.GetAllFolders();
            var modules = new List<Module>();

            if (includeSelf)
            {
                baseHandler.AddRange(baseObject, this, modules, folders.SelectMany(f => f.ModuleAssemblies).Where(m => m.HasExports(baseObject)).SelectMany(m => m.GetExports(baseObject)), ModuleAddType.Self);
            }

            if (baseObject.Parent != null)
            {
                baseHandler.AddRange(baseObject, this, modules, folders.SelectMany(f => f.ModuleAssemblies).Where(m => m.HasExports(baseObject.Parent)).SelectMany(m => m.GetExports(baseObject.Parent)), ModuleAddType.Parent);
            }

            foreach (var childObject in baseObject.GetFollowingChildren(PartsAliasResolver))
            {
                baseHandler.AddRange(baseObject, this, modules, folders.SelectMany(f => f.ModuleAssemblies).Where(m => m.HasExports(childObject)).SelectMany(m => m.GetExports(childObject)), ModuleAddType.Children);
            }

            if (baseObject is NavigationProperty)
            {
                var element = (ServerInterfaces.IParentBase) baseObject;
                var elementChild = element.ChildElements.Single();

                foreach (var childObject in elementChild.GetFollowingChildren(PartsAliasResolver))
                {
                    baseHandler.AddRange(baseObject, this, modules, folders.SelectMany(f => f.ModuleAssemblies).Where(m => m.HasExports(childObject)).SelectMany(m => m.GetExports(childObject)), ModuleAddType.Grandchildren);
                }
            }

            if (baseHandler is IModuleAddFacetHandler)
            {
                var moduleAddFacetHandler = (IModuleAddFacetHandler)baseHandler;

                moduleAddFacetHandler.AddRange(baseObject, this, modules, ModuleAddType.Custom, (m) =>
                {
                    if (m.BaseObject != null)
                    {
                        if (m.BaseObject.Name == baseObject.Name)
                        {
                            return includeSelf;
                        }

                        if (baseObject is NavigationProperty)
                        {
                            var element = (ServerInterfaces.IParentBase)baseObject;
                            var elementChild = element.ChildElements.Single();

                            if (m.BaseObject.Name == elementChild.Name)
                            {
                                return true;
                            }

                            if (elementChild.Parent != null)
                            {
                                if (m.BaseObject.Name == elementChild.Parent.Name)
                                {
                                    return true;
                                }
                            }

                            if (m.BaseObject.Parent != null)
                            {
                                if (m.BaseObject.Parent.Name == elementChild.Name)
                                {
                                    return true;
                                }
                            }
                        }

                        if (this.CustomQueries.ContainsKey(baseObject))
                        {
                            var queriesList = this.CustomQueries[baseObject];

                            if (queriesList.Any(q => q.QueryEntity.Name == m.BaseObject.Name))
                            {
                                return true;
                            }
                        }
                        else if (this.CustomQueries.ContainsNavigationKey(baseObject))
                        {
                            var queriesList = this.CustomQueries.GetNavigationValue(baseObject);

                            if (queriesList.Any(q => q.QueryEntity.Name == m.BaseObject.Name))
                            {
                                return true;
                            }
                        }

                        if (baseObject.Parent != null)
                        {
                            if (m.BaseObject.Name == baseObject.Parent.Name)
                            {
                                return true;
                            }
                        }

                        if (m.BaseObject.Parent != null)
                        {
                            if (m.BaseObject.Parent.Name == baseObject.Name)
                            {
                                return true;
                            }
                        }
                    }

                    return false;
                });
            }

            foreach (var module in modules)
            {
                var declaration = module.CreateImportDeclaration(folder, subFolderCount);

                declarations.Add(declaration);
            }
        }

        /// <summary>   Gets all folders. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <returns>   all folders. </returns>

        internal List<Folder> GetAllFolders()
        {
            return this.ApplicationFolderHierarchy.GetAllFolders();
        }

        /// <summary>   Terminates this.  </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>

        public void Terminate()
        {
            foreach (var stackItem in this.ForLifeStackItems.ToList())
            {
                var handler = (IForLifeFacetHandler)stackItem.Handler;

                handler.Terminate(this);

                this.ForLifeStackItems.Remove(stackItem);
            }

            this.SaveSchemaDocuments();

            DebugUtils.ThrowIf(this.HandlerStack.Count > 0, () => new InvalidOperationException("Generator handler stack has remaining items."));
            DebugUtils.ThrowIf(this.HierarchyStack.Count > 0, () => new InvalidOperationException("Generator hierarchy stack has remaining items."));
            DebugUtils.ThrowIf(moduleAssemblies.Count > 0, () => new InvalidOperationException("Module assemblies stack has remaining items."));
            DebugUtils.ThrowIf(this.ElementStack.Count > 0, () => new InvalidOperationException("Element stack has remaining items."));
        }

        /// <summary>   Builds validation set. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="baseObject">   The base object. </param>
        ///
        /// <returns>   An IValidationSet. </returns>

        public IValidationSet BuildValidationSet(ServerInterfaces.IBase baseObject)
        {
            var validationAttributes = baseObject.GetValidationAttributes();
            var validationSet = new ValidationSet(this);

            foreach (var validationAttribute in validationAttributes)
            {
                var type = validationAttribute.GetType();
                var handlerTypes = types.Where(t => t.IsValidationHandlerType(type));

                foreach (var handlerType in handlerTypes)
                {
                    var handler = handlerType.CreateInstance<IValidationHandler>();

                    handler.HandleValidations(baseObject, validationSet, validationAttributes);
                }
            }

            return validationSet;
        }

        /// <summary>   Query if 'entityWithFacets' is identity. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="entityWithFacets"> The entity with facets. </param>
        ///
        /// <returns>   True if identity, false if not. </returns>

        public bool IsIdentityEntity(IBase entityWithFacets)
        {
            return entityWithFacets == this.IdentityEntity;
        }

        /// <summary>   Handler, called when the get expression. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="providerGuid"> Unique identifier for the provider. </param>
        ///
        /// <returns>   The expression handler. </returns>

        public IExpressionHandler GetExpressionHandler(Guid providerGuid)
        {
            foreach (var type in this.types.Where(t => t.HasCustomAttribute<ExpressionHandlerAttribute>()))
            {
                var expressionHandlerAttribute = type.GetCustomAttribute<ExpressionHandlerAttribute>();

                if (expressionHandlerAttribute.AbstraXProviderGuid == providerGuid)
                {
                    return (IExpressionHandler)Activator.CreateInstance(type);
                }
            }

            return null;
        }

        /// <summary>   Gets the expression handlers in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="providerGuid"> Unique identifier for the provider. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the expression handlers in this
        /// collection.
        /// </returns>

        public IEnumerable<IExpressionHandler> GetExpressionHandlers(Guid providerGuid)
        {
            foreach (var type in this.types.Where(t => t.HasCustomAttribute<ExpressionHandlerAttribute>()))
            {
                var expressionHandlerAttribute = type.GetCustomAttribute<ExpressionHandlerAttribute>();

                if (expressionHandlerAttribute.AbstraXProviderGuid == providerGuid)
                {
                    yield return (IExpressionHandler) Activator.CreateInstance(type);
                }
            }
        }

        /// <summary>   Indents this.  </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>

        public void Indent()
        {
            this.IndentLevel++;
        }

        /// <summary>   Dedents this.  </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>

        public void Dedent()
        {
            this.IndentLevel--;
        }

        /// <summary>   Searches for the first data annotation type. </summary>
        ///
        /// <remarks>   Ken, 10/4/2020. </remarks>
        ///
        /// <param name="propertyName"> Name of the property. </param>
        ///
        /// <returns>   The found data annotation type. </returns>

        public Type FindDataAnnotationType(string propertyName)
        {
            var type = types.SingleOrDefault(t => t.Namespace.IsOneOf("System.ComponentModel", "System.ComponentModel.DataAnnotations", "AbstraX.DataAnnotations") && t.Name == propertyName);

            switch (propertyName)
            {
                case "MinLengthAttribute":
                    return typeof(MinLengthAttribute);
                case "MaxLengthAttribute":
                    return typeof(MaxLengthAttribute);
                case "DataTypeAttribute":
                    return typeof(DataTypeAttribute);
            }

            return type;
        }

        /// <summary>   Handler, called when the get data annotation type. </summary>
        ///
        /// <remarks>   Ken, 10/5/2020. </remarks>
        ///
        /// <param name="propertyName"> Name of the property. </param>
        /// <param name="type">         The type. </param>
        ///
        /// <returns>   The data annotation type handler. </returns>

        public IDataAnnotationTypeHandler GetDataAnnotationTypeHandler(string propertyName, Type type)
        {
            IDataAnnotationTypeHandler handler;

            if (dataAnnotationTypeHandlers.Any(h => h.CanHandle(propertyName, type)))
            {
                handler = dataAnnotationTypeHandlers.OrderBy(h => h.Priority).First(h => h.CanHandle(propertyName, type));
            }
            else
            {
                handler = this.GetHandler<IDataAnnotationTypeHandler>(h => h.CanHandle(propertyName, type));

                if (handler != null)
                {
                    dataAnnotationTypeHandlers.Add(handler);
                }
            }

            return handler;
        }

        /// <summary>   Creates a type for an entity. </summary>
        ///
        /// <remarks>   Ken, 10/6/2020. </remarks>
        ///
        /// <exception cref="Exception">                Thrown when an exception error condition occurs. </exception>
        /// <exception cref="HandlerNotFoundException"> Thrown when a Handler Not Found error condition
        ///                                             occurs. </exception>
        ///
        /// <param name="moduleBuilder">            The module builder. </param>
        /// <param name="entity">                   The entity. </param>
        /// <param name="appHierarchyNodeObject">   The application hierarchy node object. </param>

        public void CreateTypeForEntity(ModuleBuilder moduleBuilder, EntityObject entity, UIHierarchyNodeObject appHierarchyNodeObject)
        {
            var allEntities = appHierarchyNodeObject.AllEntities;
            var namespaceName = moduleBuilder.Assembly.GetName().Name;
            var typeBuilder = moduleBuilder.DefineType(namespaceName + "." + entity.Name, TypeAttributes.Public | TypeAttributes.Class, typeof(object));
            var metadataTypeBuilder = moduleBuilder.DefineType(namespaceName + "." + entity.Name + "Metadata", TypeAttributes.Public | TypeAttributes.Class, typeof(object));
            var attributeType = typeof(System.ComponentModel.DataAnnotations.MetadataTypeAttribute);
            var attributeTypeConstructor = attributeType.GetConstructor(new Type[] { typeof(Type) });
            Type entityType;
            Type metadataType;
            ILGenerator ilGenerator;
            string metadataCode;
            string entityTypeCode;

            // flush out the metadata

            foreach (var property in entity.Properties)
            {
                var annotationAtrributeType = this.FindDataAnnotationType(property.PropertyName + "Attribute");
                var handler = this.GetDataAnnotationTypeHandler(property.PropertyName, annotationAtrributeType);

                if (handler != null)
                {
                    if (!handler.Process(entity, property, annotationAtrributeType, metadataTypeBuilder, appHierarchyNodeObject, this))
                    {
                        throw new Exception($"Cannot process annotation type '{ annotationAtrributeType.AsDisplayText() }' and property '{ property.PropertyName }");
                    }
                }
                else
                {
                    throw new HandlerNotFoundException($"Cannot find annotation type handler for annotation type '{ annotationAtrributeType.AsDisplayText() }' and property '{ property.PropertyName }");
                }
            }

            foreach (var attribute in entity.Attributes)
            {
                var propertyName = attribute.Name.RemoveText(" ");
                var propertyTypeName = attribute.AttributeType;

                if (propertyTypeName == "related entity")
                {

                }
                else
                {
                    var propertyType = Type.GetType(TypeExtensions.GetPrimitiveTypeFullName(propertyTypeName));
                    var propertyBuilder = metadataTypeBuilder.DefineProperty(propertyName, PropertyAttributes.None, propertyType, null);
                    var getMethod = metadataTypeBuilder.DefineMethod("get" + propertyName, MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName, propertyType, null);
                    var setMethod = metadataTypeBuilder.DefineMethod("set" + propertyName, MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName, typeof(void), new Type[] { propertyType });

                    propertyBuilder.SetGetMethod(getMethod);
                    propertyBuilder.SetSetMethod(setMethod);

                    ilGenerator = getMethod.GetILGenerator();

                    ilGenerator.Emit(OpCodes.Ldnull);
                    ilGenerator.Emit(OpCodes.Ret);

                    ilGenerator = setMethod.GetILGenerator();

                    ilGenerator.Emit(OpCodes.Ret);

                    foreach (var property in attribute.Properties)
                    {
                        var annotationAtrributeType = this.FindDataAnnotationType(property.PropertyName + "Attribute");
                        var handler = this.GetDataAnnotationTypeHandler(property.PropertyName, annotationAtrributeType);

                        if (handler != null)
                        {
                            if (!handler.Process(entity, attribute, property, annotationAtrributeType, propertyBuilder, appHierarchyNodeObject, this))
                            {
                                throw new Exception($"Cannot process annotation type '{ annotationAtrributeType.AsDisplayText() }' and property '{ property.PropertyName }");
                            }
                        }
                        else
                        {
                            throw new HandlerNotFoundException($"annotation type '{ annotationAtrributeType.AsDisplayText() }' and property '{ property.PropertyName }");
                        }
                    }
                }
            }

            metadataType = metadataTypeBuilder.CreateType();

            typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(attributeTypeConstructor, new object[] { metadataType }));

            entityType = typeBuilder.CreateType();
            metadataCode = metadataType.GenerateCode(entityType);

            entity.MemoryEntityType = entityType;
            entity.MemoryEntityMetadataType = entityType;

            // flush out the entity type

            foreach (var attribute in entity.Attributes)
            {
                var propertyName = attribute.Name;
                var propertyTypeName = attribute.AttributeType;

                if (propertyTypeName == "related entity")
                {

                }
                else
                {
                    var propertyType = Type.GetType(TypeExtensions.GetPrimitiveTypeFullName(propertyTypeName));
                    var propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.None, propertyType, null);
                    var getMethod = typeBuilder.DefineMethod("get" + propertyTypeName, MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName, propertyType, null);
                    var setMethod = typeBuilder.DefineMethod("set" + propertyTypeName, MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName, typeof(void), new Type[] { propertyType });

                    propertyBuilder.SetGetMethod(getMethod);
                    propertyBuilder.SetSetMethod(setMethod);
                }
            }
        }
    }
}
