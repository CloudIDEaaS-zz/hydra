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

namespace AbstraX
{
    public class GeneratorConfiguration : IGeneratorConfiguration
    {
        public Guid ProjectType { get; }
        public string ProjectFolderRoot { get; }
        public string PackagePathCache { get; }
        public GeneratorPass ConfiguredPass { get; set; }
        public GeneratorPass CurrentPass { get; set; }
        public RecursionMode RecursionMode { get; private set; }
        public IVSProject ServicesProject { get; }
        public Dictionary<string, object> AdditionalOptions { get; }
        public GeneratorOptions GeneratorOptions { get; }
        public List<SingleHandler> ForLifeStackItems { get; set; }
        public Stack<HandlerStackItem> HandlerStack { get; set; }
        public Stack<HierarchyStackItem> HierarchyStack { get; set; }
        public int IndentLevel { get; set; }
        public Stack<XElement> ElementStack { get; set; }
        public Dictionary<string, object> KeyValuePairs { get; set; }
        public LanguageDictionary LanguageDictionary { get; set; }
        public List<ICustomHandler> CustomHandlers { get; set; }
        public List<IFacetHandler> OtherHandlers { get; set; }
        public IAppGeneratorEngine AppGeneratorEngine { get; private set; }
        public IWorkspaceGeneratorEngine WorkspaceGeneratorEngine { get; private set; }
        public Dictionary<string, string> InputFiles { get; }
        public bool SuppressDebugOutput { get; set; }
        public string AppName { get; set; }
        public string AppDescription { get; set; }
        public string IdentityProvider { get; set; }
        public ApplicationFolderHierarchy ApplicationFolderHierarchy { get; }
        public bool NoFileCreation { get; private set; }
        public IEntityWithFacets IdentityEntity { get; private set; }
        public PartsAliasResolver PartsAliasResolver { get; private set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public Dictionary<Guid, string> Roles { get; private set; }
        public Dictionary<string, IViewProject> ViewProjects { get; private set; }
        public QueryDictionary CustomQueries { get; private set; }
        private Dictionary<HandlerStackItem, string[]> authorizedRoles { get; }
        private PackageCacheManager packageCacheManager;
        private Dictionary<ulong, IImportHandler> importHandlers;
        private List<Type> types;
        private List<Module> builtInModules;
        private bool disposing;
        private RegistrySettings registrySettings;
        private bool skipDispose;
        private Stack<IModuleAssembly> moduleAssemblies;
        private ResourcesHandler resourcesHandler;
        private UnityContainer unityContainer;
        private List<IWorkspaceFileTypeHandler> workspaceFileTypeHandlers;
        private List<IWorkspaceTokenContentHandler> workspaceTokenContentHandlers;
        public Dictionary<string, XDocument> AbstraXSchemaDocuments { get; private set; }

        private const ulong APPLICATION_IMPORT_HANDLER_ID = ulong.MinValue;

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

        public FileSystem FileSystem
        {
            get
            {
                return this.ApplicationFolderHierarchy.FileSystem;
            }
        }

        public string HierarchyPath
        {
            get
            {
                return this.HierarchyStack.Reverse().ToDelimitedList("/");
            }
        }

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

        public GeneratorConfiguration(Guid projectType, string projectFolderRoot, string appName, Dictionary<string, object> additionalOptions, GeneratorOptions generatorOptions, IWorkspaceGeneratorEngine workspaceGeneratorEngine, List<Type> types)
        {
            this.ProjectType = projectType;
            this.ProjectFolderRoot = projectFolderRoot;
            this.AdditionalOptions = additionalOptions;
            this.GeneratorOptions = generatorOptions;
            this.WorkspaceGeneratorEngine = workspaceGeneratorEngine;
            this.AppName = appName;
            this.types = types;

            workspaceFileTypeHandlers = new List<IWorkspaceFileTypeHandler>();
            workspaceTokenContentHandlers = new List<IWorkspaceTokenContentHandler>();
        }

        public GeneratorConfiguration(Guid projectType, string projectFolderRoot, Dictionary<string, string> inputFiles, Dictionary<string, object> additionalOptions, GeneratorOptions generatorOptions, IWorkspaceGeneratorEngine workspaceGeneratorEngine, List<Type> types)
        {
            this.ProjectType = projectType;
            this.ProjectFolderRoot = projectFolderRoot;
            this.AdditionalOptions = additionalOptions;
            this.GeneratorOptions = generatorOptions;
            this.WorkspaceGeneratorEngine = workspaceGeneratorEngine;
            this.InputFiles = inputFiles;
            this.types = types;

            workspaceFileTypeHandlers = new List<IWorkspaceFileTypeHandler>();
            workspaceTokenContentHandlers = new List<IWorkspaceTokenContentHandler>();
        }

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

        public IEnumerable<Module> BuiltInModules
        {
            get
            {
                return builtInModules.OrderBy(m => m.Name);
            }
        }

        public void AddBuiltInModule(params Module[] modules)
        {
            builtInModules.AddRange(modules);
        }

        public void StopServices()
        {
            if (packageCacheManager != null)
            {
                packageCacheManager.Dispose();
            }
        }

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

        public PackageCacheStatusInfo GetCacheStatus(string mode, bool setAsReported = false)
        {
            return packageCacheManager.GetCacheStatus(mode, setAsReported);
        }

        public PackageInstallsFromCacheStatus GetInstallFromCacheStatus(string mode)
        {
            return packageCacheManager.GetInstallFromCacheStatus(mode);
        }

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
        }

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

        public void AddFacets(IEntityWithFacets entityWithFacets)
        {
        }

        public string AddTranslation(ServerInterfaces.IBase baseObject, string key, string value, bool skipIfSame)
        {
            return this.LanguageDictionary.AddTranslation(baseObject, key, value, skipIfSame);
        }

        public void AddHierarchyProperty(string propertyName)
        {
            var item = this.HierarchyStack.Peek();

            item.Name += string.Format("[@{0}]", propertyName);
        }

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

        public int StackCount
        {
            get
            {
                return Math.Max(this.HandlerStack.Count, this.HierarchyStack.Count);
            }
        }

        public List<string> GetTemplateParameters(string content)
        {
            var matches = content.RegexGetMatches(@"(?<parameter>\$" + StringExtensions.REGEX_IDENTIFIER_MIDSTRING + @"\$)");

            if (matches != null)
            {
                return matches.Select(m => m.GetGroupValue("parameter")).ToList();
            }

            return new List<string>();
        }

        public BusinessModel CreateBusinessModelFromTemplate(string templateFilePath)
        {
            var businessModel = new BusinessModel();

            businessModel.ParseFile(templateFilePath);

            return businessModel;
        }

        public EntityDomainModel CreateEntityDomainModelFromTemplate(string templateFilePath)
        {
            var entityDomainModel = new EntityDomainModel();

            entityDomainModel.ParseFile(templateFilePath);

            return entityDomainModel;
        }

        public EntityDomainModel CreateEntityDomainModelFromJsonFile(string jsonFile)
        {
            var entityDomainModel = new EntityDomainModel();

            entityDomainModel.ParseJsonFile(jsonFile);

            return entityDomainModel;
        }

        public IBusinessModelGeneratorHandler GetBusinessModelGeneratorHandler()
        {
            return (IBusinessModelGeneratorHandler)this.GetHandler<IBusinessModelGeneratorHandler>();
        }

        public IEntitiesJsonGeneratorHandler GetEntitiesJsonGeneratorHandler()
        {
            return (IEntitiesJsonGeneratorHandler)this.GetHandler<IEntitiesJsonGeneratorHandler>();
        }

        public IEntitiesModelGeneratorHandler GetEntitiesModelGeneratorHandler()
        {
            return (IEntitiesModelGeneratorHandler)this.GetHandler<IEntitiesModelGeneratorHandler>();
        }

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

        public IModuleAssembly PushModuleAssembly<T>(string namePrefix) where T : IModuleAssembly, new()
        {
            var moduleAssembly = new T();

            moduleAssembly.Name = namePrefix + "Module";
            moduleAssembly.RoutingName = namePrefix + "RoutingModule";
            moduleAssemblies.Push(moduleAssembly);

            return moduleAssembly;
        }

        public void SetModuleAssemblyProperties(IModuleAssemblyProperties properties)
        {
            var moduleAssembly = moduleAssemblies.Peek();

            properties.Configuration = this;
            properties.UpdateModuleAssembly(moduleAssembly);
        }

        public void SetModuleAssemblyFolder(Folder folder)
        {
            var moduleAssembly = moduleAssemblies.Peek();

            folder.AddAssembly(moduleAssembly);
        }

        public FolderStructure.File CreateFile(System.IO.FileInfo fileInfo, IEnumerable<Module> nonAssemblyModules, string output, FileKind fileKind, Func<StringBuilder> hierarchyGenerator)
        {
            var file = this.CreateFile(fileInfo, output, fileKind, hierarchyGenerator);

            file.SetNonAssemblyModulesDefaultFile(nonAssemblyModules);

            return file;
        }

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

        public IEnumerable<HandlerStackItem> StackItems
        {
            get
            {
                return this.ForLifeStackItems.Concat(this.HandlerStack);
            }
        }

        public List<string> GraphQLTypes
        {
            get
            {
                var graphQLTypes = typeof(GraphType).Assembly.GetTypes().Where(t => t.InheritsFrom(typeof(GraphType)));

                return graphQLTypes.Select(t => t.Name).ToList();
            }
        }

        public IEnumerable<ModuleImportDeclaration> CreateImports(IModuleKindHandler moduleKindHandler, IModuleAssembly moduleAssembly, Folder folder, bool includeSelf = false, int subFolderCount = 0)
        {
            var modulesOrAssemblies = new List<IModuleOrAssembly>();

            return CreateImports(moduleKindHandler, moduleAssembly, modulesOrAssemblies, folder, includeSelf, subFolderCount);
        }

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

        internal List<Folder> GetAllFolders()
        {
            return this.ApplicationFolderHierarchy.GetAllFolders();
        }

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

        public bool IsIdentityEntity(IBase entityWithFacets)
        {
            return entityWithFacets == this.IdentityEntity;
        }

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

        public void Indent()
        {
            this.IndentLevel++;
        }

        public void Dedent()
        {
            this.IndentLevel--;
        }
    }
}
