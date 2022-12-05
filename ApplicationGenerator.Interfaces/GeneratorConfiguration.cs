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
using System.Text.RegularExpressions;
using System.Data.Entity;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Sinks.RollingFile;
using HtmlAgilityPack;
using AbstraX.CommandHandlers;
using VisualStudioProvider;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using DynamicTemplateEngine;

namespace AbstraX
{
    /// <summary>   A generator configuration. </summary>
    ///
    /// <remarks>   Ken, 10/1/2020. </remarks>

    public class GeneratorConfiguration : IGeneratorConfiguration
    {
        /// <summary>   Gets the predictive analytics. </summary>
        ///
        /// <value> The predictive analytics. </value>

        public Dictionary<MemberInfo, List<PredictiveAnalytic>> PredictiveAnalytics { get; }

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

        /// <summary>   Gets a value indicating whether the log package listing. </summary>
        ///
        /// <value> True if log package listing, false if not. </value>

        public bool LogPackageListing { get; }

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

        public CodeInterfaces.IVSProject ServicesProject { get; }

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

        /// <summary>   Gets or sets the name of the organization. </summary>
        ///
        /// <value> The name of the organization. </value>

        public string OrganizationName { get; set; }

        /// <summary>   Gets the output writer. </summary>
        ///
        /// <value> The output writer. </value>

        public StreamWriter OutputWriter { get; }

        /// <summary>   Gets the redirected writer. </summary>
        ///
        /// <value> The redirected writer. </value>

        public TextWriter RedirectedWriter { get; }

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

        public IEntityObjectWithFacets IdentityEntity { get; private set; }

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

        /// <summary>   Gets or sets the role defaults. </summary>
        ///
        /// <value> The role defaults. </value>

        public Dictionary<Guid, Dictionary<string, object>> RoleDefaults { get; private set; }

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
        private bool useDynamicTemplates;

        /// <summary>   Name of the generation. </summary>
        private string generationName;

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
        /// <summary>   The data annotation type handlers. </summary>
        private List<IDataAnnotationTypeHandler> dataAnnotationTypeHandlers;
        /// <summary>   The schema log file. </summary>
        private string schemaLogFile;
        /// <summary>   The stack log file. </summary>
        private string stackLogFile;
        private string statusLogFile;

        /// <summary>   The path tree log file. </summary>
        private string uiPathTreeLogFile;
        private string processingReportFile;
        private string installFromCacheReportFile;
        private FacetPartsModules activeFacetModules;
        /// <summary>   The handler events logger. </summary>
        private Logger handlerEventsLogger;
        private Logger statusLogger;
        private LogSet logSet;

        /// <summary>   List of handlers. </summary>
        private List<HandlerStackItem> handlerList;
        /// <summary>   List of hierarchies. </summary>
        private List<HierarchyStackItem> hierarchyList;
        /// <summary>   List of modules. </summary>
        private List<IModuleAssembly> modulesList;
        private HtmlDocument processingReportDocument;
        private HtmlNode processingReportContainerElement;
        private HtmlNode processingReportTabsElement;
        private HtmlNode processingCurrentContentElement;
        private DirectoryInfo packageLogDirectory;
        private NpmPackage existingNpmPackage;
        private NodeCommandHandler nodeCommandHandler;
        private Dictionary<string, List<PackageRecord>> loggedPackages;
        private LogCleaner logCleaner;
        private HtmlDocument installFromCacheReportDocument;

        /// <summary>   Gets or sets the abstra x coordinate schema documents. </summary>
        ///
        /// <value> The abstra x coordinate schema documents. </value>

        public Dictionary<string, XDocument> AbstraXSchemaDocuments { get; private set; }

        /// <summary>   Identifier for the application import handler. </summary>
        private const ulong APPLICATION_IMPORT_HANDLER_ID = ulong.MinValue;

        /// <summary>   Gets a value indicating whether the no file creation. </summary>
        public event FolderStructure.FileSystemEventHandler FileSystemEvent;
        /// <summary>   Event queue for all listeners interested in CreateInstance events. </summary>
        public event CreateInstanceEventHandler CreateInstance;
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
                var lastAuthorizedRolePair = this.authorizedRoles.LastOrDefault();
                var lastAuthorizedStackItem = lastAuthorizedRolePair.Key;

                if (lastAuthorizedStackItem == null)
                {
                    return string.Empty;
                }

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
        /// <remarks>   CloudIDEaaS, 1/4/2021. </remarks>
        ///
        /// <param name="types">    The types. </param>

        public GeneratorConfiguration(List<Type> types)
        {
            this.types = types;
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

        public GeneratorConfiguration(Guid projectType, string projectFolderRoot, CodeInterfaces.IVSProject servicesProject, string packageCachePath, Dictionary<string, object> additionalOptions, GeneratorOptions generatorOptions, IAppGeneratorEngine generatorEngine, List<Type> types)
        {
            this.ProjectType = projectType;
            this.ProjectFolderRoot = projectFolderRoot;
            this.ServicesProject = servicesProject;
            this.AdditionalOptions = additionalOptions;
            this.GeneratorOptions = generatorOptions;
            this.ConfiguredPass = generatorOptions.GeneratorPass;
            this.LogPackageListing = generatorOptions.LogPackageListing;
            this.DebugAssistantAddress = generatorOptions.DebugAssistantAddress;
            this.DebugPackageInstalls = generatorOptions.DebugPackageInstalls;
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
            this.RoleDefaults = new Dictionary<Guid, Dictionary<string, object>>();
            this.CustomQueries = new QueryDictionary();
            this.PackagePathCache = packageCachePath;
            this.ElementStack = new Stack<XElement>();
            this.AbstraXSchemaDocuments = new Dictionary<string, XDocument>();
            this.PredictiveAnalytics = new Dictionary<MemberInfo, List<PredictiveAnalytic>>();
            this.OutputWriter = generatorOptions.OutputWriter;
            this.RedirectedWriter = generatorOptions.RedirectedWriter;

            registrySettings = new RegistrySettings();

            registrySettings.CurrentWorkingDirectory = this.ProjectFolderRoot;
            registrySettings.PackagePathCache = this.PackagePathCache;

            registrySettings.Save();

            this.types = types;
            this.useDynamicTemplates = generatorOptions.UseDynamicTemplates;
            builtInModules = new List<Module>();
            moduleAssemblies = new Stack<IModuleAssembly>();
            importHandlers = new Dictionary<ulong, IImportHandler>();
            authorizedRoles = new Dictionary<HandlerStackItem, string[]>();
            this.generationName = generatorEngine.GenerationName;
            handlerList = new List<HandlerStackItem>();
            hierarchyList = new List<HierarchyStackItem>();
            modulesList = new List<IModuleAssembly>();

            this.RoleDefaults.Add(Guid.Empty, new Dictionary<string, object>());

            CreateLoggers();

            if (packageCachePath.IsNullOrEmpty())
            {
                var container = this.UnityContainer;
            }
            else
            {
                var container = this.UnityContainer;
                
                packageCacheManager = new PackageCacheManager(projectFolderRoot, packageCachePath, installFromCacheReportDocument, installFromCacheReportFile, this.DebugPackageInstalls);

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
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
        }

        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            if (nodeCommandHandler != null)
            {
                nodeCommandHandler.Dispose();
            }

            if (logCleaner != null)
            {
                logCleaner.Stop();
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ExceptionObject.ToString());
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="projectType">              Type of the project. </param>
        /// <param name="projectFolderRoot">        The project folder root. </param>
        /// <param name="appName">                  Name of the application. </param>
        /// <param name="appDescription">           . </param>
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
            this.OutputWriter = generatorOptions.OutputWriter;
            this.RedirectedWriter = generatorOptions.RedirectedWriter;
            this.types = types;
            this.generationName = workspaceGeneratorEngine.GenerationName;
            this.PredictiveAnalytics = new Dictionary<MemberInfo, List<PredictiveAnalytic>>();

            if (generatorOptions.ApplicationFolderHierarchy != null)
            {
                this.ApplicationFolderHierarchy = generatorOptions.ApplicationFolderHierarchy;
            }

            registrySettings = new RegistrySettings();

            registrySettings.CurrentWorkingDirectory = this.ProjectFolderRoot;
            registrySettings.PackagePathCache = this.PackagePathCache;

            registrySettings.Save();

            workspaceFileTypeHandlers = new List<IWorkspaceFileTypeHandler>();
            workspaceTokenContentHandlers = new List<IWorkspaceTokenContentHandler>();
            dataAnnotationTypeHandlers = new List<IDataAnnotationTypeHandler>();

            CreateLoggers();
        }

        /// <summary>   Logs user interface hierarchy. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/27/2020. </remarks>
        ///
        /// <param name="path"> Full pathname of the file. </param>
        
        public void LogUIHierarchyPath(string path)
        {
            logSet.CurrentName = "UI Hierarchy Path";
            logSet.CurrentStreamWriter = null;
            logSet.LogLine(path);

            processingReportDocument.Save(processingReportFile);
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
            this.generationName = workspaceGeneratorEngine.GenerationName;
            this.PredictiveAnalytics = new Dictionary<MemberInfo, List<PredictiveAnalytic>>();
            this.OutputWriter = generatorOptions.OutputWriter;
            this.RedirectedWriter = generatorOptions.RedirectedWriter;

            if (generatorOptions.ApplicationFolderHierarchy != null)
            {
                this.ApplicationFolderHierarchy = generatorOptions.ApplicationFolderHierarchy;
            }

            registrySettings = new RegistrySettings();

            registrySettings.CurrentWorkingDirectory = this.ProjectFolderRoot;
            registrySettings.PackagePathCache = this.PackagePathCache;

            registrySettings.Save();

            workspaceFileTypeHandlers = new List<IWorkspaceFileTypeHandler>();
            workspaceTokenContentHandlers = new List<IWorkspaceTokenContentHandler>();
            dataAnnotationTypeHandlers = new List<IDataAnnotationTypeHandler>();

            CreateLoggers();
        }

        /// <summary>   Creates the loggers. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/25/2020. </remarks>

        private void CreateLoggers()
        {
            string logPath;
            string serilogFile;
            var logFolder = DateTime.Now.ToSortableDateTimeText();
            var topMostFolder = this.GeneratorOptions.AppFolderStructureSurveyor.TopMostFolder;

            logCleaner = new LogCleaner(topMostFolder);

            logCleaner.Start();

            logPath = Path.GetFullPath(Path.Combine(this.ProjectFolderRoot , @"Logs", logFolder));
            serilogFile = Path.Combine(logPath, $"{ this.generationName }.Main.log");

            System.IO.Directory.CreateDirectory(logPath);

            this.Logger = new LoggerConfiguration()
                            .WriteTo.File(serilogFile, rollingInterval: RollingInterval.Day, fileSizeLimitBytes: NumberExtensions.MB, rollOnFileSizeLimit: true, outputTemplate: "{Message:lj}{NewLine}")
                            .CreateLogger();

            schemaLogFile = Path.Combine(logPath, $"{ this.generationName }.Schema.log");
            stackLogFile = Path.Combine(logPath, $"{ this.generationName }.Stack.log");
            statusLogFile = Path.Combine(logPath, "Status.log");
            uiPathTreeLogFile = Path.Combine(logPath, $"{ this.generationName }.UIPathTree.log");
            processingReportFile = Path.Combine(logPath, "Processing", $"{ this.generationName }.ProcessingReport.html");
            installFromCacheReportFile = Path.Combine(logPath, "InstallFromCache", $"{ this.generationName }.InstallFromCacheReport.html");

            CreateProcessingReportDocument();
            CreateInstallFromCacheReportDocument();

            serilogFile = Path.Combine(logPath, $"{ this.generationName }.HandlerEvents.log");

            this.handlerEventsLogger = new LoggerConfiguration()
                            .WriteTo.File(serilogFile, rollingInterval: RollingInterval.Day, fileSizeLimitBytes: NumberExtensions.MB, rollOnFileSizeLimit: true, shared: true, outputTemplate: "{Message:lj}{NewLine}")
                            .CreateLogger();

            this.statusLogger = new LoggerConfiguration()
                            .WriteTo.File(statusLogFile, rollingInterval: RollingInterval.Day, fileSizeLimitBytes: NumberExtensions.MB, rollOnFileSizeLimit: true, shared: true)
                            .CreateLogger();

            this.logSet = new LogSet();

            if (this.LogPackageListing)
            {
                Package.PackageReadEvent += Package_PackageReadEvent;
                packageLogDirectory = AbstraX.LoggingExtensions.CreateLogFolder("Packages");
                existingNpmPackage = this.ReadExistingPackage();
                loggedPackages = new Dictionary<string, List<PackageRecord>>();

                packageLogDirectory.WriteToLog("Listing.tsv", PackageRecord.GetColumns());
            }
        }

        /// <summary>   Logs status message. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/6/2021. </remarks>
        ///
        /// <param name="message">  The message. </param>

        public void LogStatusInformation(string message)
        {
            this.statusLogger.Information(message);
        }

        /// <summary>   Logs status error. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/6/2021. </remarks>
        ///
        /// <param name="message">  The message. </param>

        public void LogStatusError(string message)
        {
            this.statusLogger.Error(message);
        }

        private void Package_PackageReadEvent(object sender, PackageReadEventArgs e)
        {
            if (this.CurrentPass == GeneratorPass.StructureOnly)
            {
                var packageType = e.Package.GetType().Name;

                if (!loggedPackages.ContainsKey(packageType))
                {
                    List<PackageRecord> records;

                    this.Indent();
                    Console.WriteLine("{0}Logging packages for type '{1}'", " ".Repeat(this.IndentLevel * 2), packageType);
                    this.Dedent();

                    records = this.GetPackageRecords(existingNpmPackage, e);
                    loggedPackages.Add(packageType, records);

                    foreach (var record in records)
                    {
                        packageLogDirectory.AppendToLog("Listing.tsv", record.ToString());
                    }
                }
            }
        }

        private void AddObjectElements(IBase baseObject, string tabName, char prefix)
        {
            processingReportTabsElement.AppendChild(HtmlNode.CreateNode($"<button name=\"{ tabName }\" class=\"tablinks\" onclick=\"openTab(event, '{ tabName }')\">{ prefix }{ baseObject.Name }</button>"));
            processingCurrentContentElement = HtmlNode.CreateNode($"<div id=\"{ tabName }\" class=\"tabcontent\">");

            processingReportContainerElement.AppendChild(processingCurrentContentElement);

            processingCurrentContentElement.CreateBox("UI Hierarchy Path");
            processingCurrentContentElement.CreateBox("Base Object");
            processingCurrentContentElement.CreateBox("Handler Stack");
            processingCurrentContentElement.CreateBox("Hierarchy Stack");
            processingCurrentContentElement.CreateBox("Module Assembly Stack");
            processingCurrentContentElement.CreateBox("File System");

            logSet.CurrentHtmlContentNode = processingCurrentContentElement;
            logSet.CurrentBaseObject = baseObject;
            logSet.CurrentName = "Base Object";

            logSet.LogBaseObject(baseObject);

            LogStack();
            LogFileSystem();
        }

        /// <summary>   Handler, called when the get grunt command. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 9/24/2022. </remarks>
        ///
        /// <returns>   The grunt command handler. </returns>

        public GruntCommandHandler GetGruntCommandHandler()
        {
            return new GruntCommandHandler();
        }

        /// <summary>   Handler, called when the get git command. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 9/24/2022. </remarks>
        ///
        /// <returns>   The git command handler. </returns>

        public GitCommandHandler GetGitCommandHandler()
        {
            return new GitCommandHandler();
        }

        private void CreateProcessingReportDocument()
        {
            var type = typeof(GeneratorConfiguration);
            var htmlContents = type.ReadResource<string>(@"ProcessingReportTemplate\Report.html");
            var cssContents = type.ReadResource<string>(@"ProcessingReportTemplate\Report.css");
            var jsContents = type.ReadResource<string>(@"ProcessingReportTemplate\Report.js");
            var iconContents = type.ReadResource<byte[]>(@"ProcessingReportTemplate\Hydra40x40.png");
            var waveContents = type.ReadResource<byte[]>(@"ProcessingReportTemplate\Wave.png");
            var reportDirectoryName = Path.GetDirectoryName(processingReportFile);
            var reportDirectory = new DirectoryInfo(reportDirectoryName);

            if (!reportDirectory.Exists)
            {
                reportDirectory.Create();
            }

            System.IO.File.WriteAllText(processingReportFile, htmlContents);
            System.IO.File.WriteAllText(Path.Combine(reportDirectoryName, "Report.css"), cssContents);
            System.IO.File.WriteAllText(Path.Combine(reportDirectoryName, "Report.js"), jsContents);
            System.IO.File.WriteAllBytes(Path.Combine(reportDirectoryName, "Hydra40x40.png"), iconContents);
            System.IO.File.WriteAllBytes(Path.Combine(reportDirectoryName, "Wave.png"), waveContents);

            processingReportDocument = new HtmlDocument();

            processingReportDocument.Load(processingReportFile);

            processingReportContainerElement = processingReportDocument.DocumentNode.SelectSingleNode("//div[@class='container']");
            processingReportTabsElement = processingReportContainerElement.SelectSingleNode("//div[@class='tab']");
        }

        private void CreateInstallFromCacheReportDocument()
        {
            var type = typeof(GeneratorConfiguration);
            var htmlContents = type.ReadResource<string>(@"InstallFromCacheReportTemplate\Report.html");
            var cssContents = type.ReadResource<string>(@"InstallFromCacheReportTemplate\Report.css");
            var jsContents = type.ReadResource<string>(@"InstallFromCacheReportTemplate\Report.js");
            var iconContents = type.ReadResource<byte[]>(@"InstallFromCacheReportTemplate\Hydra40x40.png");
            var waveContents = type.ReadResource<byte[]>(@"InstallFromCacheReportTemplate\Wave.png");
            var reportDirectoryName = Path.GetDirectoryName(installFromCacheReportFile);
            var reportDirectory = new DirectoryInfo(reportDirectoryName);

            if (!reportDirectory.Exists)
            {
                reportDirectory.Create();
            }

            System.IO.File.WriteAllText(installFromCacheReportFile, htmlContents);
            System.IO.File.WriteAllText(Path.Combine(reportDirectoryName, "Report.css"), cssContents);
            System.IO.File.WriteAllText(Path.Combine(reportDirectoryName, "Report.js"), jsContents);
            System.IO.File.WriteAllBytes(Path.Combine(reportDirectoryName, "Hydra40x40.png"), iconContents);
            System.IO.File.WriteAllBytes(Path.Combine(reportDirectoryName, "Wave.png"), waveContents);

            installFromCacheReportDocument = new HtmlDocument();

            installFromCacheReportDocument.Load(installFromCacheReportFile);
        }

        /// <summary>   Creates parts alias resolver instance. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/23/2020. </remarks>
        ///
        /// <returns>   The new parts alias resolver instance. </returns>

        public PartsAliasResolver CreatePartsAliasResolverInstance()
        {
            return new PartsAliasResolver();
        }

        /// <summary>   Event handler. Called by CurrentDomain for first chance exception events. </summary>
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

            if (nodeCommandHandler != null)
            {
                nodeCommandHandler.Dispose();
            }

            if (logCleaner != null)
            {
                logCleaner.Stop();
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
                var requestHandled = new List<string>();

                foreach (var importHandler in importHandlers.Values)
                {
                    foreach (var package in importHandler.Packages.Values)
                    {
                        foreach (var install in package.PackageInstalls)
                        {
                            if (this.DebugPackageInstalls != null && this.DebugPackageInstalls.Any(i => install.StartsWith(i)))
                            {
                                AbstraXExtensions.DebugAttach(true);
                            }

                            if (packageCacheManager != null)
                            {
                                if (!requestHandled.Contains(install))
                                {
                                    if (packageCacheManager.Handled("installs", install))
                                    {
                                        allInstalls.Add(install);
                                        requestHandled.Add(install);
                                    }
                                    else
                                    {
                                        installs.Add(install);
                                        requestHandled.Add(install);
                                    }
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
                var requestHandled = new List<string>();

                foreach (var importHandler in importHandlers.Values)
                {
                    foreach (var package in importHandler.Packages.Values)
                    {
                        try
                        {
                            foreach (var install in package.PackageDevInstalls)
                            {
                                if (packageCacheManager != null)
                                {
                                    if (this.DebugPackageInstalls != null && this.DebugPackageInstalls.Any(i => install.StartsWith(i)))
                                    {
                                        Debugger.Launch();
                                    }

                                    if (!requestHandled.Contains(install))
                                    {
                                        if (packageCacheManager.Handled("devInstalls", install))
                                        {
                                            allInstalls.Add(install);
                                            requestHandled.Add(install);
                                        }
                                        else
                                        {
                                            installs.Add(install);
                                            requestHandled.Add(install);
                                        }
                                    }
                                }
                                else
                                {
                                    installs.Add(install);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
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
            var entityWithFacets = (IEntityObjectWithFacets)baseObject;
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
                        var partialEntity = (IEntityObjectWithFacets) objectGraph.Last();

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

        /// <summary>   Resets this. </summary>
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
            this.RoleDefaults = new Dictionary<Guid, Dictionary<string, object>>();
            this.CustomQueries = new QueryDictionary();
            this.AbstraXSchemaDocuments = new Dictionary<string, XDocument>();

            registrySettings.CurrentWorkingDirectory = this.ProjectFolderRoot;
            registrySettings.PackagePathCache = this.PackagePathCache;

            registrySettings.Save();

            moduleAssemblies = new Stack<IModuleAssembly>();
            importHandlers = new Dictionary<ulong, IImportHandler>();
            handlerList = new List<HandlerStackItem>();
            hierarchyList = new List<HierarchyStackItem>();
            modulesList = new List<IModuleAssembly>();

            if (this.ApplicationFolderHierarchy != null)
            {
                this.FileSystem.ClearEmptyFiles();
            }

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

            handlerStackItem.LogEvent(HandlerStackEvent.Push, this.AppGeneratorEngine.CurrentUIHierarchyPath, this.CurrentPass, this.handlerEventsLogger);
            handlerStackItem.Indentation = this.HandlerStack.Count;

            this.HandlerStack.Push(handlerStackItem);

            handlerList.Add(handlerStackItem);

            if (this.HierarchyStack.Count == 0)
            {
                HierarchyStackItem stackItem = string.Empty;

                stackItem.Indentation = this.HierarchyStack.Count;

                this.HierarchyStack.Push(stackItem);
                hierarchyList.Add(stackItem);

                stackItem = handlerStackItem.EntityObjectName;
                stackItem.Indentation = this.HierarchyStack.Count;

                this.HierarchyStack.Push(stackItem);
                hierarchyList.Add(stackItem);
            }
            else
            {
                if (handlerStackItem.DoNotFollow)
                {
                    HierarchyStackItem stackItem = string.Empty;

                    stackItem.Indentation = this.HierarchyStack.Count;

                    this.HierarchyStack.Push(stackItem);
                    hierarchyList.Add(stackItem);

                }
                else
                {
                    HierarchyStackItem stackItem = handlerStackItem.EntityObjectName;

                    stackItem.Indentation = this.HierarchyStack.Count;

                    this.HierarchyStack.Push(stackItem);
                    hierarchyList.Add(stackItem);
                }
            }

            LogStack();
            LogFileSystem();
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

            LogSchema();

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

            var disposable = container.CreateDisposable(() =>
            {
                this.ElementStack.Pop(2);
            });

            rootElement.Add(element);
            this.ElementStack.Push(element);

            LogSchema();

            return disposable;
        }

        /// <summary>   Logs user interface path tree. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/24/2020. </remarks>
        ///
        /// <param name="uiPathTree">   The path tree. </param>

        public void LogUIPathTree(ObjectTree<UIPathItem> uiPathTree)
        {
            using (logSet.CreateWriter("UI Path Tree", uiPathTreeLogFile))
            {
                logSet.LogUIPathItem(uiPathTree);
            }
        }

        /// <summary>   Logs the schema. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/25/2020. </remarks>

        private void LogSchema()
        {
            using (logSet.CreateWriter("Schema", schemaLogFile))
            {
                foreach (var pair in this.AbstraXSchemaDocuments)
                {
                    var name = pair.Key;
                    var document = pair.Value;

                    logSet.LogLine("\r\n\r\n{0} {1}\r\n\r\n", name, "-".Repeat(25));

                    logSet.LogLine(document.ToString());
                }
            }
        }

        /// <summary>   Logs the stack. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/25/2020. </remarks>

        private void LogStack()
        {
            using (var writer = logSet.CreateWriter(stackLogFile))
            {
                writer.WriteLine(string.Format("{0} {1}\r\n", "Handler Stack", "-".Repeat(25)));
                logSet.CurrentName = "Handler Stack";

                foreach (var handlerStackItem in this.handlerList)
                {
                    string output;

                    if (handlerStackItem.Popped)
                    {
                        output = string.Format("{0}[{1} ({2}), {3}]", " ".Repeat(handlerStackItem.Indentation * 4), handlerStackItem.BaseObject.Name, handlerStackItem.EntityObjectName, handlerStackItem.DebugInfo);
                    }
                    else
                    {
                        output = string.Format("{0}{1} ({2}), {3}", " ".Repeat(handlerStackItem.Indentation * 4), handlerStackItem.BaseObject.Name, handlerStackItem.EntityObjectName, handlerStackItem.DebugInfo);
                    }

                    logSet.LogLine(output);
                }

                writer.WriteLine(string.Format("\r\n\r\n{0} {1}\r\n", "Hierarchy Stack", "-".Repeat(25)));
                logSet.CurrentName = "Hierarchy Stack";

                foreach (var hierarchyStackItem in hierarchyList)
                {
                    string output;
                    var itemText = hierarchyStackItem.ToString();

                    if (itemText.IsNullOrEmpty())
                    {
                        itemText = "Root";
                    }

                    if (hierarchyStackItem.Popped)
                    {
                        output = string.Format("{0}[{1}]", " ".Repeat(hierarchyStackItem.Indentation * 4), itemText);
                    }
                    else
                    {
                        output = string.Format("{0}{1}", " ".Repeat(hierarchyStackItem.Indentation * 4), itemText);
                    }

                    logSet.LogLine(output);
                }

                writer.WriteLine(string.Format("\r\n\r\n{0} {1}\r\n", "Module Assembly Stack", "-".Repeat(25)));
                logSet.CurrentName = "Module Assembly Stack";

                foreach (var moduleAssembly in modulesList)
                {
                    string output;
                    var itemText = moduleAssembly.DebugInfo;

                    if (itemText.IsNullOrEmpty())
                    {
                        itemText = "Root";
                    }

                    if (moduleAssembly.Popped)
                    {
                        output = string.Format("{0}[{1}]", " ".Repeat(moduleAssembly.Indentation * 4), itemText);
                    }
                    else
                    {
                        output = string.Format("{0}{1}", " ".Repeat(moduleAssembly.Indentation * 4), itemText);
                    }

                    logSet.LogLine(output);
                }

                processingReportDocument.Save(processingReportFile);
            }
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

            var disposable = entitySet.CreateDisposable(() =>
            {
                this.ElementStack.Pop();
            });

            parentElement.Add(element);
            this.ElementStack.Push(element);

            LogSchema();

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

                disposable = entity.CreateDisposable(() =>
                {
                    this.ElementStack.Pop();
                });

                return disposable;
            }

            element = new XElement(xs + "element",
                            new XAttribute("name", entity.Name));

            disposable = entity.CreateDisposable(() =>
            {
                this.ElementStack.Pop();
            });

            parentElement.Add(element);
            this.ElementStack.Push(element);

            LogSchema();

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

            LogSchema();
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

            LogSchema();
        }

        /// <summary>   Adds the facets. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="entityWithFacets"> The entity with facets. </param>

        public void AddFacets(IEntityObjectWithFacets entityWithFacets)
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


        /// <summary>   Begins a child. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="baseObject">   The base object. </param>
        /// <param name="reportObject"> (Optional) True to report object. </param>
        ///
        /// <returns>   An IDisposable. </returns>

        public IDisposable BeginChild(IBase baseObject, bool reportObject = false)
        {
            if (this.RecursionMode != RecursionMode.Winding)
            {
                if (this.RecursionMode == RecursionMode.Unwinding)
                {
                    this.AppGeneratorEngine.WriteLine("\r\n------------------------\r\n", "-".Repeat(10));
                }

                this.RecursionMode = RecursionMode.Winding;
            }

            if (reportObject)
            {
                this.ReportObject(baseObject);
            }

            var popAction = new Action(() =>
            {
                if (!skipDispose)
                {
                    if (reportObject)
                    {
                        EndObject(baseObject);
                    }

                    EndChild(baseObject);
                }
            });

            return this.CreateDisposable(popAction);
        }

        /// <summary>   Reports an object. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/25/2020. </remarks>
        ///
        /// <param name="baseObject">   The base object. </param>
        ///
        /// <returns>   An IDisposable. </returns>

        public IDisposable ReportObject(IBase baseObject)
        {
            var popAction = new Action(() =>
            {
                if (!skipDispose)
                {
                    EndObject(baseObject);
                }
            });

            if (this.CurrentPass == GeneratorPass.StructureOnly)
            {
                var tabName = baseObject.ID.Replace("'", "`").HtmlEncode();

                AddObjectElements(baseObject, tabName, '+');

                processingReportDocument.Save(processingReportFile);
            }

            return this.CreateDisposable(popAction);
        }

        /// <summary>   Ends an object. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/25/2020. </remarks>
        ///
        /// <param name="baseObject">   The base object. </param>

        private void EndObject(IBase baseObject)
        {
            if (this.CurrentPass == GeneratorPass.StructureOnly)
            {
                var tabName = "End:" + baseObject.ID.Replace("'", "`").HtmlEncode();

                AddObjectElements(baseObject, tabName, '-');

                processingReportDocument.Save(processingReportFile);
            }
        }
        private void LogFileSystem()
        {
            Folder root;

            logSet.CurrentStreamWriter = null;
            logSet.CurrentName = "File System";

            root = (Folder) this.FileSystem["/"];

            logSet.LogFileSystem(root);

            processingReportDocument.Save(processingReportFile);
        }

        /// <summary>   Ends a child. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="baseObject">   The base object. </param>

        private void EndChild(IBase baseObject)
        {
            disposing = true;

            if (this.HandlerException != null)
            {
                throw this.HandlerException;
            }

            if (!baseObject.NoUIConfigOrFollow())
            {
                ReduceStack(baseObject);
            }

            if (this.HierarchyStack.Count == 1 && this.HierarchyStack.Single().Name == string.Empty)
            {
                var hierarchyStackItem = this.HierarchyStack.Pop();
                hierarchyStackItem.Popped = true;

                this.RecursionMode = RecursionMode.None;

                LogStack();
            }

            disposing = false;
        }

        private void ReduceStack(IBase baseObject)
        {
            HandlerStackItem handlerStackItem;
            HierarchyStackItem hierarchyStackItem;
            int moduleCount = 0;

            handlerStackItem = this.HandlerStack.Peek();
            moduleCount = handlerStackItem.ModuleCount;

            if (this.authorizedRoles.ContainsKey(handlerStackItem))
            {
                this.authorizedRoles.Remove(handlerStackItem);
            }

            handlerStackItem = this.HandlerStack.Pop();
            handlerStackItem.Popped = true;

            hierarchyStackItem = this.HierarchyStack.Pop();
            hierarchyStackItem.Popped = true;

            handlerStackItem.LogEvent(HandlerStackEvent.Push, this.AppGeneratorEngine.CurrentUIHierarchyPath, this.CurrentPass, this.handlerEventsLogger);

            if (!handlerStackItem.NoModules && !handlerStackItem.DoNotFollow)
            {
                for (var x = 0; x < moduleCount; x++)
                {
                    HandleModuleAssemblies(baseObject, handlerStackItem);
                }

                if (baseObject.Kind == ServerInterfaces.DefinitionKind.StaticContainer)
                {
                    var parent = baseObject.Parent;

                    while (moduleAssemblies.Count > 0)
                    {
                        HandleModuleAssemblies(parent, handlerStackItem);
                    }
                }
            }

            LogStack();
        }

        /// <summary>   Handles the module assemblies described by baseObject. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="baseObject">       The base object. </param>
        /// <param name="handlerStackItem"> The handler stack item to push. </param>

        private void HandleModuleAssemblies(IBase baseObject, HandlerStackItem handlerStackItem)
        {
            IModuleAssembly moduleAssembly;
            Folder folder;
            List<PredictiveAnalytic> analytics = null;
            IAnalyticsReporter analyticsReporter = null;
            MemberInfo memberInfo = null;

            if (baseObject is IEntityObjectWithFacets entityWithFacets)
            {
                if (this.PredictiveAnalytics.Any(p => p.Key.Name.RemoveEndIfMatches("Metadata") == entityWithFacets.Name && p.Value.Any(a => a.DefinitionKind == entityWithFacets.Kind)))
                {
                    var pair = this.PredictiveAnalytics.Single(p => p.Key.Name.RemoveEndIfMatches("Metadata") == entityWithFacets.Name && p.Value.Any(a => a.DefinitionKind == entityWithFacets.Kind));

                    memberInfo = pair.Key;
                    analytics = pair.Value;

                    analyticsReporter = analytics.First().Reporter;
                }
            }

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
            moduleAssembly.Popped = true;

            LogStack();

            folder = this.GetAllFolders().Single(f => f.ModuleAssemblies.Any(a => a.Name == moduleAssembly.Name));

            this.AppGeneratorEngine.HandleModuleAssembly(moduleAssembly, folder);

            if (analytics != null)
            {
                var matchingAnalytic = analytics.SingleOrDefault(a => moduleAssembly.Matches(a.UIAttribute));

                if (matchingAnalytic != null)
                {
                    matchingAnalytic.IsUnwinding = true;

                    analyticsReporter.ReportObservation(matchingAnalytic);
                }
            }
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
        /// <param name="appDescription">   . </param>
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
        /// <param name="appDescription">   . </param>
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
        /// <returns>
        /// The memory module builder is a sandbox builder to allow runtime reflection-like creation to
        /// aid with code generation.
        /// </returns>

        public IDynamicModuleBuilder GetDynamicModuleBuilder()
        {
            var builders = new List<IDynamicModuleBuilder>();

            foreach (var type in types.Where(t => !t.IsInterface && t.Implements<IDynamicModuleBuilder>()))
            {
                var handler = (IDynamicModuleBuilder)Activator.CreateInstance(type);

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

        public T GetHandler<T>(params object[] args) where T : IHandler 
        {
            var handlers = new List<T>();

            foreach (var type in types.Where(t => !t.IsInterface && t.Implements<T>()))
            {
                var handler = (T)Activator.CreateInstance(type, args);

                handlers.Add(handler);
            }

            return handlers.OrderBy(h => h.Priority).FirstOrDefault();
        }

        /// <summary>   Handler, called when the get. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/5/2021. </remarks>
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

        /// <summary>   Handler, called when the get node command. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <returns>   The node command handler. </returns>

        public NodeCommandHandler GetNodeCommandHandler(bool interactive = false, string workingDirectory = null)
        {
            if (interactive)
            {
                if (this.nodeCommandHandler == null)
                {
                    var nodeCommandHandler = new NodeCommandHandler();

                    this.nodeCommandHandler = nodeCommandHandler;

                    if (workingDirectory == null)
                    {
                        nodeCommandHandler.RunInteractively();
                    }
                    else
                    {
                        nodeCommandHandler.RunInteractively(workingDirectory);
                    }
                }

                return this.nodeCommandHandler;
            }
            else
            {
                return new NodeCommandHandler();
            }
        }

        /// <summary>   Handler, called when the get npm command. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <returns>   The npm command handler. </returns>

        public NpmCommandHandler GetNpmCommandHandler()
        {
            return new NpmCommandHandler();
        }

        /// <summary>   Handles the facets. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="entityWithFacets"> The entity with facets. </param>
        /// <param name="isIdentityEntity"> (Optional) True if is identity, false if not. </param>
        ///
        /// <returns>   A HandlerStackItem. </returns>

        public HandlerStackItem HandleFacets(IEntityObjectWithFacets entityWithFacets, bool isIdentityEntity = false)
        {
            var handlersList = new List<HandlerFacet>();
            var facets = entityWithFacets.Facets.ToList();
            var authorizationState = AuthorizationState.Authorize;
            var setAuthorization = false;
            var reportedAttributes = new List<UIAttribute>();
            List<HandlerFacet> sortedList;
            string[] roles = null;
            HandlerStackItem handlerStackItem;
            List<PredictiveAnalytic> analytics = null;
            IAnalyticsReporter analyticsReporter = null;
            MemberInfo memberInfo = null;

            if (this.PredictiveAnalytics.Any(p => p.Key.Name.RemoveEndIfMatches("Metadata") == entityWithFacets.Name && p.Value.Any(a => a.DefinitionKind == entityWithFacets.Kind)))
            {
                var pair = this.PredictiveAnalytics.Single(p => p.Key.Name.RemoveEndIfMatches("Metadata") == entityWithFacets.Name && p.Value.Any(a => a.DefinitionKind == entityWithFacets.Kind));

                memberInfo = pair.Key;
                analytics = pair.Value;

                analyticsReporter = analytics.First().Reporter;
            }

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
                if (isIdentityEntity)
                {
                    var handlerTypes = types.Where(t => t.IsFacetHandlerType(entityWithFacets.Kind));

                    foreach (var handlerType in handlerTypes)
                    {
                        IFacetHandler handler;
                        HandlerFacet handlerFacet;

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

                        handlerFacet = new HandlerFacet(handler, null);

                        if (!this.SuppressDebugOutput)
                        {
                            Debug.WriteLine(handlerFacet.DebugInfo);
                        }

                        handlersList.Add(handlerFacet);
                    }
                }

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
                    else if (handlersList.Count == 0)
                    {
                        return new DoNotFollowHandler(entityWithFacets, "Facet count is zero. IEntityWithOptionalFacets.FollowWithout = false").LogCreate(this.AppGeneratorEngine.CurrentUIHierarchyPath, this.CurrentPass, this.handlerEventsLogger);
                    }
                }
                else if (handlersList.Count == 0)
                {
                    return new DoNotFollowHandler(entityWithFacets, "Facet count is zero. Not an IEntityWithOptionalFacets").LogCreate(this.AppGeneratorEngine.CurrentUIHierarchyPath, this.CurrentPass, this.handlerEventsLogger);
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
                else if (attribute is UIAttribute)
                {
                    var uiAttribute = (UIAttribute)attribute;

                    if (uiAttribute.PathRootAlias != null)
                    {
                        this.PartsAliasResolver.Add(uiAttribute);
                    }

                    if (uiAttribute.UIHierarchyPath != null)
                    {
                        if (!uiAttribute.UIKind.IsOneOf(UIKind.LoginPage, UIKind.RegisterPage))
                        {
                            var queue = uiAttribute.ParseHierarchyPath(this.PartsAliasResolver);
                            var parts = queue.SplitElementParts(entityWithFacets).ToList();
                            var count = parts.Count;

                            if (this.StackCount != count)
                            {
                                return new DoNotFollowHandler(entityWithFacets, $"For UIHierarchyPath '{ uiAttribute.UIHierarchyPath }', part count { count } does not equal Hierarchy Stack count of { this.StackCount }").LogCreate(this.AppGeneratorEngine.CurrentUIHierarchyPath, this.CurrentPass, this.handlerEventsLogger);
                            }
                        }
                    }
                }
                else if (attribute is ScaffoldColumnAttribute)
                {
                    var scaffoldColumnAttribute = (ScaffoldColumnAttribute)attribute;

                    if (!scaffoldColumnAttribute.Scaffold)
                    {
                        return new DoNotFollowHandler(entityWithFacets, "scaffoldColumnAttribute.Scaffold = false").LogCreate(this.AppGeneratorEngine.CurrentUIHierarchyPath, this.CurrentPass, this.handlerEventsLogger);
                    }
                }
                else if (attribute is QueryPathAttribute)
                {
                    var queryPathAttribute = (QueryPathAttribute)attribute;
                    var queryPathQueue = queryPathAttribute.GetQueryPathQueue(entityWithFacets);
                    var controllerMethod = queryPathQueue.SuggestedControllerMethodName;
                    var codeExpression = queryPathQueue.QueryCode;
                    var code = codeExpression.ReplaceTokens(new Dictionary<string, string> { { "containerVariable", "entities" }, { "identityNameVariable", "userName" } });

                    // not ready, not reasonable?

                    DebugUtils.Break();
                }
                else if (attribute is CustomQueryAttribute)
                {
                    var customQueryAttribute = (CustomQueryAttribute)attribute;
                    var controllerMethod = customQueryAttribute.ControllerMethodName;
                    var resourcesType = customQueryAttribute.ResourcesType;
                    var resources = CreateInstance.Invoke<IAppResources>(resourcesType);
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

            sortedList = handlersList.OrderBy(h => h.Handler.Priority).DistinctBy(h => h.Handler.GetType().FullName).ToList();

            handlerStackItem = HandlerStackItem.Create(entityWithFacets, roles, sortedList.Select(h => (IFacetHandler) h.Handler).ToArray()).LogCreate(this.AppGeneratorEngine.CurrentUIHierarchyPath, this.CurrentPass, this.handlerEventsLogger);

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
                    this.RoleDefaults.AddRange(this.Roles.Where(r => !this.RoleDefaults.Keys.Any(r2 => r.Key == r2)).ToDictionary(r => r.Key, r => new Dictionary<string, object>()));

                    this.authorizedRoles.Add(handlerStackItem, handlerStackItem.Roles);
                }
            }

            if (sortedList.Count > 0)
            {
                var moduleBeforeCount = this.moduleAssemblies.Count;
                int moduleAfterCount;

                while (sortedList.Count > 0)
                {
                    var handlerFacet = sortedList.First();
                    var handler = handlerFacet.Handler;
                    var facet = handlerFacet.Facet;
                    var attribute = facet.Attribute;
                    var handleModulesImmediately = false;

                    if (attribute is UIAttribute)
                    {
                        var uiAttribute = (UIAttribute)attribute;

                        if (uiAttribute.UIHierarchyPath != null)
                        {
                            if (uiAttribute.UIKind.IsOneOf(UIKind.LoginPage, UIKind.RegisterPage))
                            {
                                handleModulesImmediately = true;
                            }
                        }
                    }

                    activeFacetModules = new FacetPartsModules(facet, entityWithFacets, this.PartsAliasResolver);
                    handlerStackItem.FacetPartsModules.Add(activeFacetModules);

                    if (this.HandleFacet(entityWithFacets, facet, (IFacetHandler) handler))
                    {
                        if (handleModulesImmediately)
                        {
                            HandleModuleAssemblies(entityWithFacets, handlerStackItem);
                        }
                    }
                    else
                    {
                        return new DoNotFollowHandler(entityWithFacets, $"HandleFacet for { handler.GetType().Name } returned false").LogCreate(this.AppGeneratorEngine.CurrentUIHierarchyPath, this.CurrentPass, this.handlerEventsLogger);
                    }

                    if (attribute is UIAttribute)
                    {
                        var uiAttribute = (UIAttribute)attribute;

                        if (analytics != null)
                        {
                            if (!reportedAttributes.Contains(uiAttribute))
                            {
                                var matchingAnalytic = analytics.SingleOrDefault(a => uiAttribute.Matches(a.UIAttribute));

                                matchingAnalytic.PassObserved = this.CurrentPass;

                                analyticsReporter.ReportObservation(matchingAnalytic);

                                reportedAttributes.Add(uiAttribute);
                            }
                        }
                    }

                    sortedList.Remove(handlerFacet);
                }

                moduleAfterCount = this.moduleAssemblies.Count;
                handlerStackItem.ModuleCount = moduleAfterCount - moduleBeforeCount;
            }
            else
            {
                if (entityWithFacets is IEntityWithOptionalFacets)
                {
                    var entityWithOptionalFacets = (IEntityWithOptionalFacets)entityWithFacets;

                    if (entityWithOptionalFacets.FollowWithout)
                    {
                        return new FollowHandler(entityWithFacets, "IEntityWithOptionalFacets.FollowWithout == true").LogCreate(this.AppGeneratorEngine.CurrentUIHierarchyPath, this.CurrentPass, this.handlerEventsLogger);
                    }
                }

                return new DoNotFollowHandler(entityWithFacets, $"Handler count is zero").LogCreate(this.AppGeneratorEngine.CurrentUIHierarchyPath, this.CurrentPass, this.handlerEventsLogger);
            }

            return handlerStackItem;
        }

        private HandlerStackItem HandleFacets(IEntityObjectWithFacets entityWithFacets, List<Facet> facets, bool handleModulesImmediately)
        {
            var handlersList = new List<HandlerFacet>();
            var authorizationState = AuthorizationState.Authorize;
            var setAuthorization = false;
            List<HandlerFacet> sortedList;
            string[] roles = null;
            HandlerStackItem handlerStackItem;

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
                else if (attribute is UIAttribute)
                {
                    var componentAttribute = (UIAttribute)attribute;

                    if (componentAttribute.PathRootAlias != null)
                    {
                        this.PartsAliasResolver.Add(componentAttribute);
                    }
                }
                else if (attribute is ScaffoldColumnAttribute)
                {
                    var scaffoldColumnAttribute = (ScaffoldColumnAttribute)attribute;

                    if (!scaffoldColumnAttribute.Scaffold)
                    {
                        return new DoNotFollowHandler(entityWithFacets, "scaffoldColumnAttribute.Scaffold = false").LogCreate(this.AppGeneratorEngine.CurrentUIHierarchyPath, this.CurrentPass, this.handlerEventsLogger);
                    }
                }
                else if (attribute is QueryPathAttribute)
                {
                    var queryPathAttribute = (QueryPathAttribute)attribute;
                    var queryPathQueue = queryPathAttribute.GetQueryPathQueue(entityWithFacets);
                    var controllerMethod = queryPathQueue.SuggestedControllerMethodName;
                    var codeExpression = queryPathQueue.QueryCode;
                    var code = codeExpression.ReplaceTokens(new Dictionary<string, string> { { "containerVariable", "entities" }, { "identityNameVariable", "userName" } });

                    // not ready, not reasonable?

                    DebugUtils.Break();
                }
                else if (attribute is CustomQueryAttribute)
                {
                    var customQueryAttribute = (CustomQueryAttribute)attribute;
                    var controllerMethod = customQueryAttribute.ControllerMethodName;
                    var resourcesType = customQueryAttribute.ResourcesType;
                    var resources = CreateInstance.Invoke<IAppResources>(resourcesType);
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
                    var handlerTypes = types.Where(t => t.IsFacetHandlerType(attributeType, kind));

                    foreach (var handlerType in handlerTypes)
                    {
                        IFacetHandler handler;
                        HandlerFacet handlerFacet;

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

                        handlerFacet = new HandlerFacet(handler, facet);

                        if (!this.SuppressDebugOutput)
                        {
                            Debug.WriteLine(handlerFacet.DebugInfo);
                        }

                        handlersList.Add(handlerFacet);
                    }
                }
            }

            sortedList = handlersList.OrderBy(h => h.Handler.Priority).DistinctBy(h => h.Handler.GetType().FullName).ToList();

            handlerStackItem = HandlerStackItem.Create(entityWithFacets, roles, sortedList.Select(h => (IFacetHandler)h.Handler).ToArray()).LogCreate(this.AppGeneratorEngine.CurrentUIHierarchyPath, this.CurrentPass, this.handlerEventsLogger);

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
                    this.RoleDefaults.AddRange(this.Roles.Where(r => !this.RoleDefaults.Keys.Any(r2 => r.Key == r2)).ToDictionary(r => r.Key, r => new Dictionary<string, object>()));

                    this.authorizedRoles.Add(handlerStackItem, handlerStackItem.Roles);
                }
            }

            if (sortedList.Count > 0)
            {
                var moduleBeforeCount = this.moduleAssemblies.Count;
                int moduleAfterCount;

                while (sortedList.Count > 0)
                {
                    var handlerFacet = sortedList.First();
                    var handler = handlerFacet.Handler;
                    var facet = handlerFacet.Facet;
                    var attribute = facet.Attribute;

                    activeFacetModules = new FacetPartsModules(facet, entityWithFacets, this.PartsAliasResolver);
                    handlerStackItem.FacetPartsModules.Add(activeFacetModules);

                    if (this.HandleFacet(entityWithFacets, facet, (IFacetHandler)handler))
                    {
                        if (handleModulesImmediately)
                        {
                            HandleModuleAssemblies(entityWithFacets, handlerStackItem);
                        }
                    }
                    else
                    {
                        return new DoNotFollowHandler(entityWithFacets, $"HandleFacet for { handler.GetType().Name } returned false").LogCreate(this.AppGeneratorEngine.CurrentUIHierarchyPath, this.CurrentPass, this.handlerEventsLogger);
                    }

                    sortedList.Remove(handlerFacet);
                }

                moduleAfterCount = this.moduleAssemblies.Count;
                handlerStackItem.ModuleCount = moduleAfterCount - moduleBeforeCount;
            }
            else
            {
                return new DoNotFollowHandler(entityWithFacets, $"Handler count is zero").LogCreate(this.AppGeneratorEngine.CurrentUIHierarchyPath, this.CurrentPass, this.handlerEventsLogger);
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

        private bool HandleFacet(IEntityObjectWithFacets entityWithFacets, ServerInterfaces.Facet facet, IView view, IViewLayoutHandler handler)
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

        private bool HandleFacet(IEntityObjectWithFacets entityWithFacets, ServerInterfaces.Facet facet, IFacetHandler handler)
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
                            this.ForLifeStackItems.Add(new SingleHandler(entityWithFacets, null, handler).LogCreate<SingleHandler>(this.AppGeneratorEngine.CurrentUIHierarchyPath, this.CurrentPass, this.handlerEventsLogger));
                        }
                    }
                    else
                    {
                        this.ForLifeStackItems.Add(new SingleHandler(entityWithFacets, null, handler).LogCreate<SingleHandler>(this.AppGeneratorEngine.CurrentUIHierarchyPath, this.CurrentPass, this.handlerEventsLogger));
                    }
                }

                try
                {
                    if (!handler.Process(entityWithFacets, facet, this))
                    {
                        return false;
                    }

                    if (!this.StackItems.PostProcess(entityWithFacets, this, handler))
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    this.LogStatusError(ex.ToString());
                    this.HandlerException = ex;

                    throw;
                }

                if (facet.Attribute is UIAttribute uIAttribute)
                {
                    var augmenterHandler = GetHandler<IUILoadKindAugmenterHandler, UILoadKindAugmenterAttribute>(a => a.UILoadKind == uIAttribute.UILoadKind);

                    if (augmenterHandler != null)
                    {
                        augmenterHandler.Process(entityWithFacets, facet, this);

                        this.HandleFacets(entityWithFacets, augmenterHandler.AdditionalFacets, true);
                    }
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
            moduleAssembly.RoutingNameExpression = namePrefix + "{0}RoutingModule";
            moduleAssembly.Indentation = moduleAssemblies.Count;

            moduleAssemblies.Push(moduleAssembly);

            modulesList.Add(moduleAssembly);
            activeFacetModules.ModuleAssemblies.Add(moduleAssembly);

            LogStack();

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
                    else if (fileKind == FileKind.Entities)
                    {
                        file = this.FileSystem.AddSystemLocalEntitiesFile(new FileInfo(filePath));
                    }
                    else
                    {
                        file = null;
                        DebugUtils.Break();
                    }

                    file.Length = output.Length;
                    file.Hash = output.GetHashCode();
                }

                FileSystemEvent?.Invoke(this, new FolderStructure.FileSystemEventArgs(new FileInfo(filePath), file));

            }
            else if (pass == GeneratorPass.StructureOnly)
            {
                if (fileKind == FileKind.Project)
                {
                    file = this.FileSystem.AddSystemLocalProjectFile(new FileInfo(filePath), hierarchyGenerator());
                }
                else if (fileKind == FileKind.Services)
                {
                    file = this.FileSystem.AddSystemLocalServicesFile(new FileInfo(filePath), hierarchyGenerator());
                }
                else if (fileKind == FileKind.Entities)
                {
                    file = this.FileSystem.AddSystemLocalEntitiesFile(new FileInfo(filePath), hierarchyGenerator());
                }
                else
                {
                    file = null;
                    DebugUtils.Break();
                }
            }
            else
            {
                file = null;
                DebugUtils.Break();
            }

            return file;
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

        public FolderStructure.File CreateFile(FileInfo fileInfo, string output, FileKind fileKind, IModuleAssembly moduleAssembly = null)
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

                if (moduleAssembly != null)
                {
                    moduleAssembly.File = file;
                }

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
            else if (pass == GeneratorPass.StructureOnly)
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

        /// <summary>   Gets or sets the logger. </summary>
        ///
        /// <value> The logger. </value>

        public Serilog.ILogger Logger { get; private set; }

        /// <summary>   Gets or sets the hydra debug assistant address. </summary>
        ///
        /// <value> The hydra debug assistant address. </value>

        public string DebugAssistantAddress { get; private set; }

        /// <summary>   Gets the debug package installs. </summary>
        ///
        /// <value> The debug package installs. </value>

        public List<string> DebugPackageInstalls { get; }

        /// <summary>   Gets a value indicating whether the file mode should be written. </summary>
        ///
        /// <value> True if write file mode, false if not. </value>

        public bool WriteFileMode => this.CurrentPass == GeneratorPass.Files;

        /// <summary>   Gets or sets the handler exception. </summary>
        ///
        /// <value> The handler exception. </value>

        public Exception HandlerException { get; private set; }

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
                moduleKindHandler.AddRange(this, moduleAssembly, modulesOrAssemblies, folders.SelectMany(f => f.ModuleAssemblies).Where(m => m.HasExports(baseObject, moduleAssembly)).SelectMany(m => m.GetExports(baseObject)), ModuleAddType.Self);
            }

            if (baseObject.Kind == ServerInterfaces.DefinitionKind.Model && moduleAssembly.File != null)
            {
                moduleKindHandler.AddRange(this, moduleAssembly, modulesOrAssemblies, folders.SelectMany(f => f.ModuleAssemblies).SelectMany(m => m.GetExports("Page")), ModuleAddType.Surrogate);
            }
            else
            {
                foreach (var childObject in baseObject.GetFollowingChildren(PartsAliasResolver))
                {
                    moduleKindHandler.AddRange(this, moduleAssembly, modulesOrAssemblies, folders.SelectMany(f => f.ModuleAssemblies).Where(m => m.HasExports(childObject, moduleAssembly)), ModuleAddType.Children);
                }
            }

            if (baseObject is NavigationProperty)
            {
                var element = (ServerInterfaces.IParentBase)baseObject;
                var elementChild = element.ChildElements.Single();

                foreach (var childObject in elementChild.GetFollowingChildren(PartsAliasResolver))
                {
                    moduleKindHandler.AddRange(this, moduleAssembly, modulesOrAssemblies, folders.SelectMany(f => f.ModuleAssemblies).Where(m => m.HasExports(childObject, moduleAssembly)), ModuleAddType.Grandchildren);
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
                    var package = packageType.CreateInstance<Package>(applicationImportHandler);

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

        /// <summary>   Terminates this. </summary>
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

        /// <summary>   Indents this. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>

        public void Indent()
        {
            this.IndentLevel++;
        }

        /// <summary>   Dedents this. </summary>
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

        /// <summary>   Handler, called when the get entity container. </summary>
        ///
        /// <remarks>   Ken, 10/8/2020. </remarks>
        ///
        /// <returns>   The entity container handler. </returns>

        public IEntityContainerHandler GetEntityContainerHandler()
        {
            return this.GetHandler<IEntityContainerHandler>();
        }

        /// <summary>   Handler, called when the get utility. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/4/2021. </remarks>
        ///
        /// <returns>   The utility handler. </returns>

        public IUtilityHandler GetUtilityHandler(Dictionary<string, object> arguments)
        {
            return this.GetHandler<IUtilityHandler>(arguments);
        }

        /// <summary>   Handler, called when the get data annotation type. </summary>
        ///
        /// <remarks>   Ken, 10/12/2020. </remarks>
        ///
        /// <returns>   The data annotation type handler. </returns>

        public IModelTypeBuilderHandler GetModelTypeBuilderHandler()
        {
            return this.GetHandler<IModelTypeBuilderHandler>();
        }

        /// <summary>   Searches for the first type. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <param name="fullName"> Name of the full. </param>
        ///
        /// <returns>   The found type. </returns>

        public Type FindType(string fullName)
        {
            var type = this.types.SingleOrDefault(t => t.FullName == fullName);

            return type;
        }

        /// <summary>   Gets template engine host. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 2/6/2021. </remarks>
        ///
        /// <returns>   The template engine host. </returns>

        public ITemplateEngineHost GetTemplateEngineHost()
        {
            if (useDynamicTemplates)
            {
                var host = new DynamicTemplateEngineHost(this);

                return host;
            }
            else
            {
                return new TemplateEngineHost();
            }
        }

        /// <summary>   Reports a generate. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 2/7/2021. </remarks>
        ///
        /// <param name="generatorType">    Type of the generator. </param>
        /// <param name="templateFile">     The template file. </param>

        public void ReportGenerate(Type generatorType, string templateFile)
        {
            if (this.useDynamicTemplates)
            {
                var hydraSolutionPath = Path.GetFullPath(Environment.ExpandEnvironmentVariables("%HYDRASOLUTIONPATH%"));
                var projectPath = Path.Combine(hydraSolutionPath, @"ApplicationGenerator.IonicAngular\ApplicationGenerator.IonicAngular.csproj");

                if (System.IO.File.Exists(projectPath))
                {
                    var project = new VSProject(projectPath);
                    var templateClasses = project.CompileItems.Where(i => i.FileName.EndsWith("Template.cs")).ToList();
                    var directory = new DirectoryInfo(Path.GetDirectoryName(project.FileName));

                    foreach (var templateClass in templateClasses)
                    {
                        var contents = templateClass.GetFileContents<string>();
                        var syntaxTree = CSharpSyntaxTree.ParseText(contents);
                        var root = syntaxTree.GetCompilationUnitRoot();
                        var generatorClass = root.DescendantNodes().OfType<ClassDeclarationSyntax>().Single();
                        var generateMethod = generatorClass.Members.OfType<MethodDeclarationSyntax>().Single(m => m.Identifier.Text.StartsWith("TransformText"));

                        if (generatorClass.Identifier.Text == generatorType.Name)
                        {
                            var sourceTemplateFile = Path.Combine(Path.GetDirectoryName(templateClass.FileName), Path.GetFileNameWithoutExtension(templateClass.FileName) + ".tt");
                            var destinationCopyFile = new FileInfo(sourceTemplateFile);
                            var sourceCopyFile = new FileInfo(templateFile);

                            if (destinationCopyFile.Exists)
                            {
                                if (destinationCopyFile.LastWriteTime < sourceCopyFile.LastWriteTime)
                                {
                                    if (System.Windows.Forms.MessageBox.Show($"Update { destinationCopyFile.Name } from changes made?", "Update template", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                                    {
                                        destinationCopyFile.MakeWritable();

                                        sourceCopyFile.CopyTo(destinationCopyFile.FullName, true);
                                    }

                                }
                            }

                            break;
                        }
                    }
                }
            }
        }
    }
}
