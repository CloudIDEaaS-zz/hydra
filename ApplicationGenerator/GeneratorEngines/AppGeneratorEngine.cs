using EntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VisualStudioProvider;
using Utils;
using System.ComponentModel.DataAnnotations;
using AbstraX.DataAnnotations;
using AbstraX;
using AbstraX.ServerInterfaces;
using AbstraX.FolderStructure;
using AbstraX.Angular;
using System.Data.Entity;
using RestEntityProvider.Web.Entities;
using AbstraX.Models.Interfaces;
using System.IO;
using AbstraX.Handlers.FacetHandlers;
using AbstraX.Generators.Server.WebAPIController;
using Utils.Hierarchies;
using System.Xml;
using AbstraX.Handlers.ExpressionHandlers;
using System.Configuration;

namespace AbstraX.GeneratorEngines
{
    public class AppGeneratorEngine : IAppGeneratorEngine
    {
        private VSProject entitiesProject;
        private VSProject servicesProject;
        private Assembly entitiesAssembly;
        private Dictionary<string, EntityCount> recursionEntitiesList;
        private string nameSpace;
        private dynamic depsJsonObject;
        private List<Type> allTypes;
        private Queue<ContainerAction> containerActionQueue;
        public GeneratorConfiguration GeneratorConfiguration { get; private set; }
        private GeneratorMode generatorMode;
        private GeneratorOptions generatorOptions;
        private string projectFolderRoot;
        private List<KeyValuePair<string, IGeneratorOverrides>> generatorOverridePairs;
        private Guid projectType;
        private bool mscorlibLoaded;
        private Dictionary<string, IBase> baseObjectDictionary;
        public string CurrentUIHierarchyPath { get; private set; }

        public AppGeneratorEngine(Guid projectType, string projectFolderRoot, string entitiesProjectPath, string servicesProjectPath, string packageCachePath, Dictionary<string, object> additionalOptions, GeneratorMode generatorMode = GeneratorMode.Console, GeneratorOptions generatorOptions = null)
        {
            IGeneratorOverrides namespaceOverride;
            string depsJsonFilePath;
            KeyValuePair<string, IGeneratorOverrides> namespaceOverridePair;
            string argumentsKind;

            this.generatorOverridePairs = this.GetOverrides().ToList();
            this.projectType = projectType;
            this.projectFolderRoot = projectFolderRoot;
            this.entitiesProject = new VSProject(entitiesProjectPath);
            this.servicesProject = new VSProject(servicesProjectPath);
            this.entitiesAssembly = Assembly.LoadFrom(entitiesProject.OutputFile);
            this.recursionEntitiesList = new Dictionary<string, EntityCount>();

            namespaceOverridePair = generatorOverridePairs.Where(o => o.Value.OverridesNamespace).Last();

            argumentsKind = namespaceOverridePair.Key;
            namespaceOverride = namespaceOverridePair.Value;

            if (namespaceOverride == null)
            {
                this.nameSpace = entitiesProject.Properties.Single(p => p.Name == "RootNamespace").Value;
            }
            else
            {
                var originalNamespace = entitiesProject.Properties.Single(p => p.Name == "RootNamespace").Value;

                namespaceOverride.OriginalNamespace = originalNamespace;
                this.nameSpace = namespaceOverride.GetNamespace(this.GeneratorConfiguration, argumentsKind);
            }

            depsJsonFilePath = Path.Combine(entitiesProject.OutputPath, entitiesProject.Name + ".deps.json");
            baseObjectDictionary = new Dictionary<string, IBase>();

            if (System.IO.File.Exists(depsJsonFilePath))
            {
                using (var reader = new StreamReader(depsJsonFilePath))
                {
                    var depsJson = reader.ReadToEnd();

                    this.depsJsonObject = JsonExtensions.ReadJson<object>(depsJson);
                }

                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += (sender, args) => AssemblyResolve(entitiesAssembly, this.entitiesProject, args);
            }

            this.allTypes = entitiesAssembly.GetAllTypes().ToList();
            this.containerActionQueue = new Queue<ContainerAction>();

            this.GeneratorConfiguration = new GeneratorConfiguration(projectType, projectFolderRoot, servicesProject, packageCachePath, additionalOptions, generatorOptions, this, allTypes);
            this.generatorMode = generatorMode;
            this.generatorOptions = generatorOptions;

            QueryInfo.GetEntityPropertiesForQuery += (sender, e) =>
            {
                var queryInfo = e.Value.QueryInfo;
                var entityProperties = new List<Generators.EntityProperty>();

                foreach (var attribute in queryInfo.QueryEntity.Attributes)
                {
                    entityProperties.Add(new Generators.EntityProperty(attribute, this.GeneratorConfiguration));
                }

                e.Value.EntityProperties = entityProperties;
            };
        }

        public void Process()
        {
            var config = this.GeneratorConfiguration;
            var assembly = new AssemblyProvider.Web.Entities.Assembly(this.entitiesAssembly, t => t.BaseType == typeof(DbContext));  // todo for code-first

            WriteLine("\r\n**** CurrentPass: {0} {1}\r\n", PrintMode.Any, config.CurrentPass, "*".Repeat(25));

            foreach (var restModelItem in entitiesProject.RestModels)
            {
                var model = new RestModel(new System.IO.FileInfo(restModelItem.FileName), config.AdditionalOptions, nameSpace);

                model.OnGetAttributes += Model_OnGetAttributes;
                model.OnGetAddIns += Model_OnGetAddIns;
                model.OnGetOverrides += Model_OnGetOverrides;

                foreach (var container in model.Containers)
                {
                    if (container is RestEntityContainer)
                    {
                        container.RegisterFacetHandlerType<WebAPIRestConfigJsonFacetHandler>();
                        container.RegisterFacetHandlerType<WebAPIRestProviderDataContextFacetHandler>();
                        container.RegisterFacetHandlerType<WebAPIRestServiceProviderFacetHandler>();
                    }

                    this.QueueContainerAction(container, () =>
                    {
                        using (this.GeneratorConfiguration.BeginChild(container))
                        {
                            ProcessContainer(container);
                        }
                    });
                }
            }

            foreach (var modelItem in entitiesProject.EDMXModels)
            {
                var model = new Model(new System.IO.FileInfo(modelItem.FileName), nameSpace);

                model.OnGetAttributes += Model_OnGetAttributes;
                model.OnGetAddIns += Model_OnGetAddIns;
                model.OnGetOverrides += Model_OnGetOverrides;

                foreach (var container in model.Containers)
                {
                    this.QueueContainerAction(container, () =>
                    {
                        using (this.GeneratorConfiguration.BeginChild(container))
                        {
                            ProcessContainer(container);
                        }
                    });
                }
            }

            while (containerActionQueue.Count > 0)
            {
                var containerProcess = containerActionQueue.Dequeue();
                var action = containerProcess.Action;

                action();
            }

            config.Terminate();

            if (generatorOptions.GeneratorPass != GeneratorPass.All || config.CurrentPass == GeneratorPassCommon.Last)
            {
                if (generatorMode == GeneratorMode.Console)
                {
                    var packageInstalls = config.PackageInstalls;

                    WriteLine("\r\nHit any key to exit", PrintMode.Any);
                }

                ReadKey();
            }
        }

        private void Model_OnGetOverrides(object sender, GetOverridesEventArgs e)
        {
            e.Overrides = this.generatorOverridePairs;
        }

        private void Model_OnGetAddIns(object sender, GetAddInEntitiesEventArgs e)
        {
            var type = allTypes.SingleOrDefault(t => t.FullName == e.Type.FullyQualifiedName);
            var baseObject = e.BaseObject;

            if (type == null)
            {
                e.NoMetadata = true;
            }
            else
            {
                if (e.PropertyName != null)
                {
                    if (type.HasCustomAttribute<MetadataTypeAttribute>())
                    {
                        var metadataTypeAttribute = type.GetCustomAttribute<MetadataTypeAttribute>();
                        var metadataClassType = metadataTypeAttribute.MetadataClassType;
                    }
                    else
                    {
                        e.NoMetadata = true;
                    }
                }
                else if (type.HasCustomAttribute<MetadataTypeAttribute>())
                {
                    if (type.HasCustomAttribute<MetadataTypeAttribute>())
                    {
                        var metadataTypeAttribute = type.GetCustomAttribute<MetadataTypeAttribute>();
                        var metadataClassType = metadataTypeAttribute.MetadataClassType;
                        var handlerTypes = allTypes.Where(t => t.HasCustomAttribute<AddInHandlerAttribute>());

                        foreach (var handlerType in handlerTypes)
                        {
                            var handlerAttribute = handlerType.GetCustomAttribute<AddInHandlerAttribute>();
                            var providerAttribute = baseObject.Root.GetType().GetCustomAttribute<AbstraXProviderAttribute>();

                            if (handlerAttribute.AbstraXProviderGuid == providerAttribute.Guid)
                            {
                                if (handlerAttribute.AddInHandlerKind == AddInHandlerKind.MetadataReflection)
                                {
                                    var handler = handlerType.CreateInstance<IAddInMetadataReflectionHandler>();

                                    handler.Handle(e, metadataClassType);
                                }
                            }
                        }

                        switch (e.DefinitionKind)
                        {
                            case DefinitionKind.ComplexSetProperty:
                                break;
                            default:
                                DebugUtils.Break();
                                break;
                        }
                    }
                    else
                    {
                        e.NoMetadata = true;
                    }
                }
            }
        }

        private void Model_OnGetAttributes(object sender, GetAttributesEventArgs e)
        {
            var type = allTypes.SingleOrDefault(t => t.FullName == e.Type.FullyQualifiedName);

            if (type == null)
            {
                if (e.BaseObject.Root is RestModel)
                {
                    var jsonObject = ((RestEntityBase)(e.BaseObject)).JsonObject;
                    var expressionHandler = this.GeneratorConfiguration.GetExpressionHandler(Guid.Parse(AbstraXProviderGuids.RestService));
                    var attributes = expressionHandler.Handle<Attribute[]>(e.BaseObject, ExpressionResultLocation.Any, ExpressionType.UnknownJsonObject, ExpressionReturnType.Attributes, jsonObject);

                    if (attributes.Length > 0)
                    {
                        e.Attributes = attributes;
                        return;
                    }
                }

                e.NoMetadata = true;
            }
            else
            {
                if (e.PropertyName != null)
                {
                    if (type.HasCustomAttribute<MetadataTypeAttribute>())
                    {
                        var metadataTypeAttribute = type.GetCustomAttribute<MetadataTypeAttribute>();
                        var metadataClassType = metadataTypeAttribute.MetadataClassType;
                        var property = metadataClassType.GetProperty(e.PropertyName);

                        if (property != null)
                        {
                            var attributes = property.GetCustomAttributes();

                            e.Attributes = attributes;
                        }
                    }
                    else
                    {
                        e.NoMetadata = true;
                    }
                }
                else if (type.HasCustomAttribute<MetadataTypeAttribute>())
                {
                    if (type.HasCustomAttribute<MetadataTypeAttribute>())
                    {
                        var metadataTypeAttribute = type.GetCustomAttribute<MetadataTypeAttribute>();
                        var metadataClassType = metadataTypeAttribute.MetadataClassType;
                        var attributes = metadataClassType.GetCustomAttributes();

                        e.Attributes = attributes;
                    }
                    else
                    {
                        e.NoMetadata = true;
                    }
                }
            }
        }

        private Assembly AssemblyResolve(Assembly targetAssembly, VSProject relatedProject, ResolveEventArgs args)
        {
            var targetFramework = relatedProject.Properties.SingleOrDefault(p => p.Name == "TargetFramework").Value;
            var runtimeTarget = this.depsJsonObject.runtimeTarget.name;
            var targets = (object)this.depsJsonObject.targets;
            var target = targets.GetDynamicMemberValue<object>((string)runtimeTarget);
            var targetPairs = target.GetDynamicMemberNameValueDictionary();
            var parts = AssemblyExtensions.GetNameParts(args.Name);
            Assembly assembly = null;

            if (!mscorlibLoaded)
            {
                var mscorlib = targetAssembly.FindCoreAssembly(@"microsoft.netcore.app\2.1.0\ref\netcoreapp2.1\mscorlib.dll");
                var systemCore = targetAssembly.FindCoreAssembly(@"microsoft.netcore.app\2.1.0\ref\netcoreapp2.1\System.Core.dll");

                mscorlibLoaded = true;
            }

            foreach (var targetPair in targetPairs)
            {
                var targetValue = targetPair.Value;
                var compile = targetValue.GetDynamicMemberValue<object>("compile");

                if (compile != null)
                {
                    var compilePairs = compile.GetDynamicMemberNameValueDictionary();

                    foreach (var compilePair in compilePairs)
                    {
                        var key = compilePair.Key;
                        var assemblyNameFile = key.Split("/").LastOrDefault();
                        var assemblyName = Path.GetFileNameWithoutExtension(assemblyNameFile);

                        if (assemblyName == parts.AssemblyName)
                        {
                            var partialPath = targetPair.Key.BackSlashes() + @"\" + key.BackSlashes();
                            assembly = targetAssembly.FindCoreAssembly(partialPath);

                            if (assembly != null)
                            {
                                return assembly;
                            }
                        }

                    }
                }
            }

            return null;
        }

        public void Reset()
        {
            baseObjectDictionary.Clear();

            this.GeneratorConfiguration.Reset();
        }

        public void QueueContainerAction(IEntityContainer container, Action action)
        {
            var tabs = this.GeneratorConfiguration.HierarchyStack.Count;
            var tabText = this.CurrentTabText;
            var parentModel = container.Parent;

            if (container.Facets.Length > 0)
            {
                if (container.HasFacetAttribute<AppNameAttribute>())
                {
                    var appNameAttribute = container.GetFacetAttribute<AppNameAttribute>();

                    if (this.HasRegisteredAppConfig())
                    {
                        var config = this.GeneratorConfiguration;

                        if (config.CurrentPass != GeneratorPass.Files)
                        {
                            throw new InvalidOperationException("Cannot have more than one container with app specific attributes");
                        }
                    }
                    else
                    {
                        KeyValuePair<string, IGeneratorOverrides> appNameOverridePair;
                        string argumentsKind;
                        IGeneratorOverrides appNameOverride;

                        appNameOverridePair = generatorOverridePairs.Where(o => o.Value.OverridesAppName).Last();

                        argumentsKind = appNameOverridePair.Key;
                        appNameOverride = appNameOverridePair.Value;

                        if (appNameOverride == null)
                        {
                            this.GeneratorConfiguration.AppName = appNameAttribute.Name;
                        }
                        else
                        {
                            this.GeneratorConfiguration.AppName = appNameOverride.GetAppName(this.GeneratorConfiguration, argumentsKind);
                        }

                        this.GeneratorConfiguration.AppDescription = appNameAttribute.Description;
                        this.GeneratorConfiguration.ClientId = appNameAttribute.Name;
                        this.GeneratorConfiguration.ClientSecret = Guid.NewGuid().ToString();
                    }
                }

                baseObjectDictionary.Add(parentModel.ID, parentModel);
                baseObjectDictionary.Add(container.ID, container);

                container.EntityDictionary = baseObjectDictionary;

                foreach (var baseObject in container.GetFollowingDescendants(this.GeneratorConfiguration.PartsAliasResolver))
                {
                    baseObjectDictionary.Add(baseObject.ID, baseObject);
                    baseObject.EntityDictionary = baseObjectDictionary;
                }
            }
            else
            {
                baseObjectDictionary.Add(container.ID, container);
                baseObjectDictionary.Add(parentModel.ID, parentModel);

                container.EntityDictionary = baseObjectDictionary;

                if (container.CanFollowWithout())
                {
                    foreach (var baseObject in container.GetFollowingDescendants(this.GeneratorConfiguration.PartsAliasResolver))
                    {
                        if (container.PreventRecursion)
                        {
                            if (!baseObjectDictionary.ContainsKey(baseObject.ID))
                            {
                                baseObjectDictionary.Add(baseObject.ID, baseObject);
                            }
                        }
                        else
                        {
                            baseObjectDictionary.Add(baseObject.ID, baseObject);
                        }

                        baseObject.EntityDictionary = baseObjectDictionary;
                    }
                }
            }

            this.containerActionQueue.Enqueue(new ContainerAction(container, action));          
        }

        public void ProcessContainer(IEntityContainer container)
        {
            var tabs = this.GeneratorConfiguration.HierarchyStack.Count;
            var tabText = this.CurrentTabText;
            string identitySet = null;
            HandlerStackItem handlerStackItem;

            if (container.Facets.Length > 0)
            {
                var facetList = container.Facets.Select(f => string.Format("{0}[{1}]", tabText, f.AttributeCode)).ToMultiLineList();
                var uiHierarchyPath = container.Facets.GetUIHierarchyPathList(generatorOptions.PrintMode);

                this.CurrentUIHierarchyPath = uiHierarchyPath;

                WriteLine(facetList, PrintMode.PrintFacetsOnly);
                WriteLine(uiHierarchyPath, PrintMode.PrintUIHierarchyPathOnly);

                if (container.HasFacetAttribute<IdentityAttribute>())
                {
                    var identityAttribute = container.GetFacetAttribute<IdentityAttribute>();

                    identitySet = identityAttribute.IdentitySet;
                }

                if (container.HasFacetAttribute<ResourcesAttribute>())
                {
                    var resourcesAttribute = container.GetFacetAttribute<ResourcesAttribute>();

                    this.GeneratorConfiguration.ResourcesHandler = resourcesAttribute.CreateHandler();
                }
            }

            if (container.NoUIConfigOrFollow())
            {
                this.GeneratorConfiguration.HandleFacets(container);
            }
            else
            {
                handlerStackItem = this.GeneratorConfiguration.HandleFacets(container);
                this.GeneratorConfiguration.Push(handlerStackItem);
            }

            WriteLine("{0}{1}", PrintMode.All, tabText, container.Name);

            using (this.GeneratorConfiguration.PushContainer(container))
            {
                if (identitySet != null)
                {
                    var membershipSet = container.EntitySets.SingleOrDefault(e => e.Name == identitySet);

                    if (membershipSet != null)
                    {
                        ProcessEntitySet(membershipSet, true);
                    }
                }

                foreach (var entitySet in container.EntitySets)
                {
                    if (entitySet.CanFollowWithout())
                    {
                        ProcessEntitySet(entitySet);
                    }
                }
            }
        }

        private bool HasRegisteredAppConfig()
        {
            var appName = this.GeneratorConfiguration.AppName;
            var appDescription = this.GeneratorConfiguration.AppDescription;
            var clientId = this.GeneratorConfiguration.ClientId;
            var clientSecret = this.GeneratorConfiguration.ClientSecret;

            return CompareExtensions.AnyAreNotNull(appName, appDescription, clientId, clientSecret);
        }

        public void ProcessEntitySet(IEntitySet entitySet, bool identitySet = false)
        {
            if (entitySet.HasUI())
            {
                ProcessUIEntitySet(entitySet);
            }

            using (this.GeneratorConfiguration.PushEntitySet(entitySet))
            {
                foreach (var entity in entitySet.Entities)
                {
                    if (entity.HasNavigationOrUI() || entity.CanFollowWithout() || identitySet)
                    {
                        using (this.GeneratorConfiguration.BeginChild(entity))
                        {
                            ProcessEntity(entity, identitySet);
                        }
                    }
                    else
                    {
                        DocumentEntity(entity, identitySet);
                    }
                }
            }
        }

        private void DocumentEntity(IEntityType entity, bool identitySet)
        {
            using (this.GeneratorConfiguration.PushEntityType(entity))
            {
                foreach (var property in entity.Properties)
                {
                    this.GeneratorConfiguration.AddProperty(property);
                }

                foreach (var property in entity.NavigationProperties)
                {
                    this.GeneratorConfiguration.AddNavigationProperty(property);
                }
            }
        }

        public void ProcessUIEntitySet(IEntitySet entitySet)
        {
            var tabCount = this.GeneratorConfiguration.HierarchyStack.Count;
            var tabText = this.CurrentTabText;
            HandlerStackItem handlerStackItem;

            if (entitySet.Facets.Length > 0)
            {
                var facetList = entitySet.Facets.Select(f => string.Format("{0}[{1}]", tabText, f.AttributeCode)).ToMultiLineList();
                var uiHierarchyPath = entitySet.Facets.GetUIHierarchyPathList(generatorOptions.PrintMode);

                this.CurrentUIHierarchyPath = uiHierarchyPath;

                WriteLine(facetList, PrintMode.PrintFacetsOnly);
                WriteLine(uiHierarchyPath, PrintMode.PrintUIHierarchyPathOnly);
            }

            handlerStackItem = this.GeneratorConfiguration.HandleFacets(entitySet);

            this.GeneratorConfiguration.Push(handlerStackItem);
        }

        public void ProcessEntity(IEntityType entity, bool isIdentityEntity = false)
        {
            HandlerStackItem handlerStackItem;
            var tabText = this.CurrentTabText;

            if (entity.Facets.Length > 0)
            {
                var facetList = entity.Facets.Select(f => string.Format("{0}[{1}]", tabText, f.AttributeCode)).ToMultiLineList();
                var uiHierarchyPath = entity.Facets.GetUIHierarchyPathList(generatorOptions.PrintMode);

                this.CurrentUIHierarchyPath = uiHierarchyPath;

                WriteLine(facetList, PrintMode.PrintFacetsOnly);
                WriteLine(uiHierarchyPath, PrintMode.PrintUIHierarchyPathOnly);
            }

            if (entity is RestEntityType)
            {
                var restEntityType = (RestEntityType) entity;

                restEntityType.RegisterSurrogateTemplateType<WebAPIControllerClassTemplate, WebAPIRestControllerClassTemplate>();
            }

            handlerStackItem = this.GeneratorConfiguration.HandleFacets(entity, isIdentityEntity);

            if (!entity.NoUIConfigOrFollow())
            {
                this.GeneratorConfiguration.Push(handlerStackItem);
            }

            WriteLine("{0}{1}", PrintMode.All, tabText, entity.Name);

            recursionEntitiesList.AddOrUpdateDictionary(entity.Name, new EntityCount(entity), t => t.Count++);

            if (recursionEntitiesList.Any(e => e.Value.Count > generatorOptions.RecursionStackLimit))
            {
                WriteError("Recursion limit of '{0}' hit.  Change this in GeneratorOptions.RecursionStackLimit", generatorOptions.RecursionStackLimit);
                return;
            }

            if (!handlerStackItem.DoNotFollow)
            {
                using (this.GeneratorConfiguration.PushEntityType(entity))
                {
                    foreach (var property in entity.Properties)
                    {
                        ProcessProperty(property);
                    }

                    foreach (var property in entity.NavigationProperties)
                    {
                        if (property.HasNavigationOrUI() || property.CanFollowWithout())
                        {
                            using (this.GeneratorConfiguration.BeginChild(property))
                            {
                                ProcessNavigationProperty(property);
                            }
                        }
                    }
                }
            }
            else
            {
                using (this.GeneratorConfiguration.PushEntityType(entity))
                {
                    foreach (var property in entity.Properties)
                    {
                        this.GeneratorConfiguration.AddProperty(property);
                    }

                    foreach (var property in entity.NavigationProperties)
                    {
                        this.GeneratorConfiguration.AddNavigationProperty(property);
                    }
                }
            }
        }

        public string CurrentTabText
        {
            get
            {
                var tabs = this.GeneratorConfiguration.HierarchyStack.Count;
                var tabText = new string(' ', tabs * 2);

                return tabText;
            }
        }

        public void ProcessNavigationProperty(INavigationProperty property)
        {
            var tabCount = this.GeneratorConfiguration.HierarchyStack.Count;
            var tabText = this.CurrentTabText;
            HandlerStackItem handlerStackItem;

            if (property.Facets.Length > 0)
            {
                var facetList = property.Facets.Select(f => string.Format("{0}[{1}]", tabText, f.AttributeCode)).ToMultiLineList();
                var uiHierarchyPath = property.Facets.GetUIHierarchyPathList(generatorOptions.PrintMode);

                this.CurrentUIHierarchyPath = uiHierarchyPath;

                WriteLine(facetList, PrintMode.PrintFacetsOnly);
                WriteLine(uiHierarchyPath, PrintMode.PrintUIHierarchyPathOnly);
            }

            handlerStackItem = this.GeneratorConfiguration.HandleFacets(property);

            WriteLine("{0}{1} ({2})", PrintMode.All, tabText, property.Name, property.DataType.Name);

            if (!handlerStackItem.DoNotFollow)
            {
                this.GeneratorConfiguration.Push(handlerStackItem);

                foreach (var childEntity in property.ChildEntities)
                {
                    recursionEntitiesList.AddOrUpdateDictionary(childEntity.Name, new EntityCount(childEntity), t => t.Count++);

                    if (recursionEntitiesList.Any(e => e.Value.Count > generatorOptions.RecursionStackLimit))
                    {
                        WriteError("Recursion limit of '{0}' hit.  Change this in GeneratorOptions.RecursionStackLimit", generatorOptions.RecursionStackLimit);
                        return;
                    }

                    using (this.GeneratorConfiguration.BeginChild(childEntity))
                    {
                        ProcessEntity(childEntity);
                    }
                }
            }

            this.GeneratorConfiguration.AddNavigationProperty(property);
        }

        public HandlerStackItem ProcessProperty(IEntityProperty property)
        {
            var tabs = this.GeneratorConfiguration.HierarchyStack.Count;
            var tabText = this.CurrentTabText;
            HandlerStackItem handlerStackItem;

            if (property.Facets.Length > 0)
            {
                var facetList = property.Facets.Select(f => string.Format("{0}[{1}]", tabText, f.AttributeCode)).ToMultiLineList();
                var uiHierarchyPath = property.Facets.GetUIHierarchyPathList(generatorOptions.PrintMode);

                this.CurrentUIHierarchyPath = uiHierarchyPath;

                WriteLine(facetList, PrintMode.PrintFacetsOnly);
                WriteLine(uiHierarchyPath, PrintMode.PrintUIHierarchyPathOnly);
            }

            handlerStackItem = this.GeneratorConfiguration.HandleFacets(property);
            
            WriteLine("{0}{1} ({2})", PrintMode.All, tabText, property.Name, property.DataType.Name);
            
            this.GeneratorConfiguration.AddProperty(property);

            return handlerStackItem;
        }

        public void ReadKey()
        {
            if (generatorMode == GeneratorMode.Console)
            {
                Console.ReadKey();
                this.GeneratorConfiguration.StopServices();
            }
            else if (generatorMode == GeneratorMode.RedirectedConsole)
            {
                generatorOptions.OutputWriter.WriteLine(Environment.NewLine);  // 3 total
                generatorOptions.OutputWriter.Flush();
            }
        }

        public void WriteLine(string format, PrintMode printMode, params object[] args)
        {
            var output = string.Format(format, args);

            if (output.Trim().Length > 0)
            {
                if ((generatorOptions.PrintMode == PrintMode.All && (!printMode.HasFlag(PrintMode.ExcludeFromAll))) || printMode.HasAnyFlag(generatorOptions.PrintMode) || printMode == PrintMode.Any)
                {
                    if (generatorMode == GeneratorMode.Console)
                    {
                        Console.WriteLine(output);
                    }
                    else if (generatorMode == GeneratorMode.RedirectedConsole)
                    {
                        generatorOptions.OutputWriter.WriteLine(output);
                        generatorOptions.OutputWriter.Flush();
                    }
                }
            }
        }

        public void WriteError(string format, params object[] args)
        {
            var output = string.Format(format, args);

            if (generatorMode == GeneratorMode.Console)
            {
                using (new ConsoleColorizer(ConsoleColor.Red))
                {
                    Console.WriteLine(output);
                }
            }
            else if (generatorMode == GeneratorMode.RedirectedConsole)
            {
                generatorOptions.ErrorWriter.WriteLine(output);
                generatorOptions.ErrorWriter.Flush();
            }
        }

        public void WriteLine(string format, params object[] args)
        {
            var output = string.Format(format, args);

            if (generatorMode == GeneratorMode.Console)
            {
                Console.WriteLine(output);
            }
            else if (generatorMode == GeneratorMode.RedirectedConsole)
            {
                generatorOptions.OutputWriter.WriteLine(output);
                generatorOptions.OutputWriter.Flush();
            }
        }

        public void HandleModuleAssembly(IModuleAssembly moduleAssembly, Folder folder)
        {
            var config = this.GeneratorConfiguration;
            var angularModule = (AngularModule)moduleAssembly;

            config.HandleModuleKind(moduleAssembly, folder, ModuleKind.AngularModule);
        }
    }
}
