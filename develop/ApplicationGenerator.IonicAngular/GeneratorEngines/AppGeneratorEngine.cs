// file:	GeneratorEngines\AppGeneratorEngine.cs
//
// summary:	Implements the application generator engine class

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
using NetCoreReflectionShim.Agent;
using AssemblyModelEntityProvider.Web.Entities;
using CoreShim.Reflection;
using MailSlot;

namespace AbstraX.GeneratorEngines
{
    /// <summary>   An application generator engine. </summary>
    ///
    /// <remarks>   Ken, 10/25/2020. </remarks>

    public class AppGeneratorEngine : IAppGeneratorEngine
    {
        /// <summary>   The entities project. </summary>
        private VSProject entitiesProject;
        /// <summary>   The services project. </summary>
        private VSProject servicesProject;
        /// <summary>   The entities assembly. </summary>
        private Assembly entitiesAssembly;
        /// <summary>   List of recursion entities. </summary>
        private Dictionary<string, EntityCount> recursionEntitiesList;
        /// <summary>   The name space. </summary>
        private string nameSpace;
        /// <summary>   The deps JSON object. </summary>
        private dynamic depsJsonObject;
        /// <summary>   List of types of the alls. </summary>
        private List<Type> allTypes;
        /// <summary>   Queue of container actions. </summary>
        private Queue<ContainerAction> containerActionQueue;

        /// <summary>   Gets or sets the generator configuration. </summary>
        ///
        /// <value> The generator configuration. </value>

        public GeneratorConfiguration GeneratorConfiguration { get; private set; }
        /// <summary>   The generator mode. </summary>
        /// <summary>   Options for controlling the generator. </summary>
        private GeneratorOptions generatorOptions;
        /// <summary>   The project folder root. </summary>
        private string projectFolderRoot;
        /// <summary>   The generator override pairs. </summary>
        private List<KeyValuePair<string, IGeneratorOverrides>> generatorOverridePairs;
        /// <summary>   Type of the project. </summary>
        private Guid projectType;

        /// <summary>   True if mscorlib loaded. </summary>
        private bool mscorlibLoaded;
        /// <summary>   Dictionary of base objects. </summary>
        private Dictionary<string, IBase> baseObjectDictionary;
        private TypeCache typeCache;
        private TypeCache initiallyLoadedTypeCache;
        private string currentUIHierarchyPath;
        private string callbackName = CallbackNames.CodeGeneration;
        private IAppFolderStructureSurveyor appFolderStructureSurveyor;

        public GeneratorMode GeneratorMode { get; }

        /// <summary>   Gets or sets the net core reflection agent. </summary>
        ///
        /// <value> The net core reflection agent. </value>

        public NetCoreReflectionAgent NetCoreReflectionAgent { get; private set; }
        public MailslotClient MailslotClient { get; private set; }
        public Queue<string> LogMessageQueue { get; }
        public int ParentProcessId { get; }
        public string GenerationName => "App";
        public int PercentComplete { get; private set; }
        private const float WIND_PERCENTAGE = .18f;
        private const float UNWIND_PERCENTAGE = .18f;

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/25/2020. </remarks>
        ///
        /// <param name="projectType">          Type of the project. </param>
        /// <param name="projectFolderRoot">    The project folder root. </param>
        /// <param name="entitiesProjectPath">  Full pathname of the entities project file. </param>
        /// <param name="servicesProjectPath">  Full pathname of the services project file. </param>
        /// <param name="packageCachePath">     Full pathname of the package cache file. </param>
        /// <param name="additionalOptions">    Options for controlling the additional. </param>
        /// <param name="generatorMode">        (Optional) The generator mode. </param>
        /// <param name="generatorOptions">     (Optional) Options for controlling the generator. </param>

        public AppGeneratorEngine(Guid projectType, string projectFolderRoot, string entitiesProjectPath, string servicesProjectPath, string packageCachePath, Dictionary<string, object> additionalOptions, GeneratorMode generatorMode = GeneratorMode.Console, GeneratorOptions generatorOptions = null)
        {
            var dbSetType = typeof(DbSet<object>);
            var thisAssembly = Assembly.GetExecutingAssembly();
            IGeneratorOverrides namespaceOverride;
            string depsJsonFilePath;
            KeyValuePair<string, IGeneratorOverrides> namespaceOverridePair;
            string argumentsKind;
            string framework;
            this.appFolderStructureSurveyor = generatorOptions.AppFolderStructureSurveyor;
            this.initiallyLoadedTypeCache = new TypeCache(false);
            this.typeCache = new TypeCache(true);
            this.generatorOverridePairs = this.GetOverrides(generatorOptions.UseOverrides, projectFolderRoot).ToList();
            this.projectType = projectType;
            this.projectFolderRoot = projectFolderRoot;
            this.entitiesProject = new VSProject(entitiesProjectPath);
            this.servicesProject = new VSProject(servicesProjectPath);
            this.entitiesAssembly = Assembly.LoadFrom(entitiesProject.OutputFile);
            this.recursionEntitiesList = new Dictionary<string, EntityCount>();
            this.NetCoreReflectionAgent = new NetCoreReflectionAgent(typeCache, generatorOptions.DebugShimService, generatorOptions.RunAsAutomated);
            this.MailslotClient = generatorOptions.MailslotClient;
            this.ParentProcessId = generatorOptions.ParentProcessId;
            this.LogMessageQueue = new Queue<string>();
            this.generatorOptions = generatorOptions;
            this.GeneratorMode = generatorMode;
            this.PercentComplete = 0;

            this.SendCallbackStatus(callbackName, "Initializing app generation engine", this.PercentComplete);

            this.NetCoreReflectionAgent.GetTypeEvent += (sender, e) =>
            {
                if (initiallyLoadedTypeCache.ContainsKey(e.TypeFullName))
                {
                    e.Type = initiallyLoadedTypeCache[e.TypeFullName];
                }
                else
                {
                    Type type = null;

                    if (initiallyLoadedTypeCache.IsStale)
                    {
                        initiallyLoadedTypeCache.BuildIndex(allTypes);
                    }

                    if (initiallyLoadedTypeCache.ContainsKey(e.TypeFullName))
                    {
                        e.Type = initiallyLoadedTypeCache[e.TypeFullName];
                    }
                    else
                    {
                        type = Type.GetType(e.TypeFullName);
                    }

                    e.Type = type;
                }
            };

            this.NetCoreReflectionAgent.GetTypeProxyEvent += (sender, e) =>
            {
                var typeProxy = allTypes.FindProxyType(e.TypeToProxy);

                e.ProxyType = typeProxy;
            };

            try
            {
                framework = this.entitiesAssembly.GetFramework();

                if (framework == null)
                {
                    this.WriteLine("Detected entities assembly as .NET Core, launching Core Reflection Agent in order to load.");

                    this.entitiesAssembly = this.NetCoreReflectionAgent.LoadCoreAssembly(this.entitiesAssembly, new Dictionary<string, string>()
                    {
                        { "AbstraX.DataAnnotations", "AbstraX.DataAnnotations" },
                        { "Microsoft.EntityFrameworkCore", "System.Data.Entity" },
                        { "System", "System" },
                    });

                    TypeShimExtensions.GetTypeShimActivatorEvent += (sender, e) => GetTypeShimActivator(this.NetCoreReflectionAgent, e);
                }

            }
            catch (Exception ex)
            {
                this.WriteLine("Detected entities assembly as .NET Core, launching Core Reflection Agent in order to load.");

                this.entitiesAssembly = this.NetCoreReflectionAgent.LoadCoreAssembly(this.entitiesAssembly, new Dictionary<string, string>() 
                {
                    { "AbstraX.DataAnnotations", "AbstraX.DataAnnotations" },
                    { "Microsoft.EntityFrameworkCore", "System.Data.Entity" },
                    { "System", "System" },
                });

                TypeShimExtensions.GetTypeShimActivatorEvent += (sender, e) => GetTypeShimActivator(this.NetCoreReflectionAgent, e);
            }

            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                this.NetCoreReflectionAgent.Dispose();
            };

            namespaceOverridePair = generatorOverridePairs.Where(o => o.Value.OverridesNamespace).LastOrDefault();

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

            this.PercentComplete = 5;
            this.SendCallbackStatus(callbackName, "Loading types", this.PercentComplete);
            this.WriteLine("Loading types");

            this.allTypes = thisAssembly.GetAllTypes(this.NetCoreReflectionAgent, entitiesAssembly).Distinct().OrderBy(t => t.FullName).ToList();
            this.containerActionQueue = new Queue<ContainerAction>();

            this.GeneratorConfiguration = new GeneratorConfiguration(projectType, projectFolderRoot, servicesProject, packageCachePath, additionalOptions, generatorOptions, this, allTypes);

            appFolderStructureSurveyor.DetermineLayout(projectFolderRoot);

            if (appFolderStructureSurveyor.ConfigObject != null)
            {
                var configObject = appFolderStructureSurveyor.ConfigObject;

                this.GeneratorConfiguration.OrganizationName = configObject.OrganizationName;
            }

            this.GeneratorConfiguration.AddPredictiveAnalytics(entitiesAssembly, this);

            this.GeneratorConfiguration.CreateInstance += (sender, e) =>
            {
                if (e.Type.Name == "AppResources")
                {
                    var appResourcesType = e.Type;
                    var appResources = this.NetCoreReflectionAgent.CreateInstance<IAppResources>((TypeShim)appResourcesType);

                    e.Instance = appResources;
                }
                else
                {
                    DebugUtils.Break();
                }
            };

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

            this.PercentComplete = 10;
            this.SendCallbackStatus(callbackName, "Finished initializing code generation", this.PercentComplete);
        }

        /// <summary>   Process this. </summary>
        ///
        /// <remarks>   Ken, 10/25/2020. </remarks>

        public void Process()
        {
            var config = this.GeneratorConfiguration;
            var assembly = new AssemblyProvider.Web.Entities.Assembly(this.entitiesAssembly, t => t.BaseType != null && t.BaseType.Name == typeof(DbContext).Name);
            var edmxContainers = new List<IEntityContainer>();

            WriteLine("\r\n**** CurrentPass: {0} {1}\r\n", PrintMode.Any, config.CurrentPass, "*".Repeat(25));

            if (config.CurrentPass == GeneratorPass.StructureOnly)
            {
                this.PercentComplete = 15;
            }
            else
            {
                this.PercentComplete = 60;
            }

            this.SendCallbackStatus(callbackName, string.Format("Processing for CurrentPass: {0}", config.CurrentPass), this.PercentComplete);

            foreach (var modelItem in entitiesProject.EDMXModels)
            {
                var model = new Model(new System.IO.FileInfo(modelItem.FileName), nameSpace);
                var disposable = this.GeneratorConfiguration.ReportObject(model);

                model.AppGeneratorEngine = this;

                model.OnGetAttributes += Model_OnGetAttributes;
                model.OnGetAddIns += Model_OnGetAddIns;
                model.OnGetOverrides += Model_OnGetOverrides;

                foreach (var container in model.Containers)
                {
                    edmxContainers.Add(container);

                    this.QueueContainerAction(container, () =>
                    {
                        using (this.GeneratorConfiguration.BeginChild(container, true))
                        {
                            ProcessContainer(container);
                        }

                        disposable.Dispose();
                    });
                }
            }

            foreach (var model in assembly.GetModels(this.GeneratorConfiguration))
            {
                model.OnGetAttributes += Model_OnGetAttributes;
                model.OnGetAddIns += Model_OnGetAddIns;
                model.OnGetOverrides += Model_OnGetOverrides;

                model.AppGeneratorEngine = this;

                foreach (var container in model.Containers.Where(c => !edmxContainers.Any(c2 => c2.GetFullName() == c.GetFullName())))
                {
                    this.QueueContainerAction(container, () =>
                    {
                        using (this.GeneratorConfiguration.BeginChild(container, true))
                        {
                            ProcessContainer(container);
                        }
                    });
                }
            }

            foreach (var restModelItem in entitiesProject.RestModels)
            {
                var model = new RestModel(new System.IO.FileInfo(restModelItem.FileName), config.AdditionalOptions, nameSpace);

                model.OnGetAttributes += Model_OnGetAttributes;
                model.OnGetAddIns += Model_OnGetAddIns;
                model.OnGetOverrides += Model_OnGetOverrides;

                model.AppGeneratorEngine = this;

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
                        using (this.GeneratorConfiguration.BeginChild(container, true))
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
                if (GeneratorMode == GeneratorMode.Console)
                {
                    var packageInstalls = config.PackageInstalls;

                    WriteLine("\r\nHit any key to exit", PrintMode.Any);
                }

                this.SendCallbackStatus(callbackName, string.Format("App code generation processing complete: {0}", config.CurrentPass), 100);
                WriteLine("App code generation processing complete");

                if (this.ParentProcessId != 0)
                {
                    ReadKey();
                }
            }
        }

        public void ReportObservation(PredictiveAnalytic analytic)
        {
            var percentAdd = (int)(WIND_PERCENTAGE * analytic.WeightPercentage * 100);

            this.PercentComplete += percentAdd;

            this.SendCallbackStatus(callbackName, analytic.Status, 100);
        }

        /// <summary>   Process this. </summary>
        ///
        /// <remarks>   Ken, 10/25/2020. </remarks>

        public void TestProcess()
        {
            var config = this.GeneratorConfiguration;
            var assembly = new AssemblyProvider.Web.Entities.Assembly(this.entitiesAssembly, t => t.BaseType != null && t.BaseType.Name == typeof(DbContext).Name);

            WriteLine("\r\n**** CurrentPass: {0} {1}\r\n", PrintMode.Any, config.CurrentPass, "*".Repeat(25));

            foreach (var modelItem in entitiesProject.EDMXModels)
            {
                var model = new Model(new System.IO.FileInfo(modelItem.FileName), nameSpace);

                model.OnGetAttributes += Model_OnGetAttributes;
                model.OnGetAddIns += Model_OnGetAddIns;
                model.OnGetOverrides += Model_OnGetOverrides;

                foreach (var container in model.Containers)
                {
                    foreach (var entitySet in container.EntitySets)
                    {
                        foreach (var facet in entitySet.Facets)
                        {
                        }

                        foreach (var entity in entitySet.Entities)
                        {
                            foreach (var facet in entity.Facets)
                            {
                            }

                            foreach (var property in entity.Properties)
                            {
                                foreach (var facet in property.Facets)
                                {
                                }
                            }

                            foreach (var property in entity.NavigationProperties)
                            {
                                foreach (var facet in property.Facets)
                                {
                                }
                            }
                        }
                    }
                }
            }

            foreach (var model in assembly.GetModels(this.GeneratorConfiguration).Where(m => !entitiesProject.EDMXModels.Any(m2 => m2.Name == m.Name)))
            {
                model.OnGetAttributes += Model_OnGetAttributes;
                model.OnGetAddIns += Model_OnGetAddIns;
                model.OnGetOverrides += Model_OnGetOverrides;

                foreach (var container in model.Containers)
                {
                    foreach (var entitySet in container.EntitySets)
                    {
                        foreach (var facet in entitySet.Facets)
                        {
                        }

                        foreach (var entity in entitySet.Entities)
                        {
                            foreach (var facet in entity.Facets)
                            {
                            }

                            foreach (var property in entity.Properties)
                            {
                                foreach (var facet in property.Facets)
                                {
                                }
                            }

                            foreach (var property in entity.NavigationProperties)
                            {
                                foreach (var facet in property.Facets)
                                {
                                }
                            }
                        }
                    }
                }
            }

            foreach (var restModelItem in entitiesProject.RestModels)
            {
                var model = new RestModel(new System.IO.FileInfo(restModelItem.FileName), config.AdditionalOptions, nameSpace);

                model.OnGetAttributes += Model_OnGetAttributes;
                model.OnGetAddIns += Model_OnGetAddIns;
                model.OnGetOverrides += Model_OnGetOverrides;

                foreach (var container in model.Containers)
                {
                    foreach (var facet in container.Facets)
                    {
                    }

                    foreach (var entitySet in container.EntitySets)
                    {
                        foreach (var facet in entitySet.Facets)
                        {
                        }

                        foreach (var entity in entitySet.Entities)
                        {
                            foreach (var facet in entity.Facets)
                            {
                            }

                            foreach (var property in entity.Properties)
                            {
                                foreach (var facet in property.Facets)
                                {
                                }
                            }

                            foreach (var property in entity.NavigationProperties)
                            {
                                foreach (var facet in property.Facets)
                                {
                                }
                            }
                        }
                    }
                }
            }

            config.Terminate();

            if (generatorOptions.GeneratorPass != GeneratorPass.All || config.CurrentPass == GeneratorPassCommon.Last)
            {
                if (GeneratorMode == GeneratorMode.Console)
                {
                    var packageInstalls = config.PackageInstalls;

                    WriteLine("\r\nHit any key to exit", PrintMode.Any);
                }

                ReadKey();
            }
        }

        /// <summary>
        /// Gets or sets the full pathname of the current user interface hierarchy file.
        /// </summary>
        ///
        /// <value> The full pathname of the current user interface hierarchy file. </value>

        public string CurrentUIHierarchyPath
        {
            get
            {
                return currentUIHierarchyPath;
            }

            private set
            {
                currentUIHierarchyPath = value;

                this.GeneratorConfiguration.LogUIHierarchyPath(currentUIHierarchyPath);
            }
        }

        private void GetTypeShimActivator(INetCoreReflectionAgent agent, GetTypeShimActivatorEventArgs e)
        {
            e.Activator = agent;
        }

        /// <summary>   Event handler. Called by Model for on get overrides events. </summary>
        ///
        /// <remarks>   Ken, 10/25/2020. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Get overrides event information. </param>

        private void Model_OnGetOverrides(object sender, GetOverridesEventArgs e)
        {
            e.Overrides = this.generatorOverridePairs;
        }

        /// <summary>   Event handler. Called by Model for on get add insert events. </summary>
        ///
        /// <remarks>   Ken, 10/25/2020. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Get add in entities event information. </param>

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
                                    var handler = handlerType.CreateInstance<IHandlerCache>();

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

        /// <summary>   Event handler. Called by Model for on get attributes events. </summary>
        ///
        /// <remarks>   Ken, 10/25/2020. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Get attributes event information. </param>

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

                        if (metadataClassType is TypeShim metadataClassTypeShim)
                        {
                            property = metadataClassTypeShim.GetProperty(e.PropertyName);
                        }

                        if (property != null)
                        {
                            var attributes = property.GetCustomAttributes(false);

                            e.Attributes = attributes.Cast<Attribute>();
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
                        var attributes = metadataClassType.GetCustomAttributes(false);

                        e.Attributes = attributes.Cast<Attribute>();
                    }
                    else
                    {
                        e.NoMetadata = true;
                    }
                }
            }
        }

        /// <summary>   Assembly resolve. </summary>
        ///
        /// <remarks>   Ken, 10/25/2020. </remarks>
        ///
        /// <param name="targetAssembly">   Target assembly. </param>
        /// <param name="relatedProject">   The related project. </param>
        /// <param name="args">             Resolve event information. </param>
        ///
        /// <returns>   An Assembly. </returns>

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

        /// <summary>   Resets this. </summary>
        ///
        /// <remarks>   Ken, 10/25/2020. </remarks>

        public void Reset()
        {
            baseObjectDictionary.Clear();

            this.GeneratorConfiguration.Reset();
        }

        /// <summary>   Queue container action. </summary>
        ///
        /// <remarks>   Ken, 10/25/2020. </remarks>
        ///
        /// <exception cref="InvalidOperationException">    Thrown when the requested operation is
        ///                                                 invalid. </exception>
        ///
        /// <param name="container">    The container. </param>
        /// <param name="action">       The action. </param>

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

                        appNameOverridePair = generatorOverridePairs.Where(o => o.Value.OverridesAppName).LastOrDefault();

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

        /// <summary>   Process the container described by container. </summary>
        ///
        /// <remarks>   Ken, 10/25/2020. </remarks>
        ///
        /// <param name="container">    The container. </param>

        public void ProcessContainer(IEntityContainer container)
        {
            var tabs = this.GeneratorConfiguration.HierarchyStack.Count;
            var tabText = this.CurrentTabText;
            string identitySetName = null;
            IEntitySet identitySet = null;
            HandlerStackItem handlerStackItem;

            if (container.Facets.Length > 0)
            {
                var facetList = container.Facets.Select(f => string.Format("{0}[{1}]", tabText, f.AttributeCode)).ToMultiLineList();
                var uiHierarchyPath = container.Facets.GetUIHierarchyPathList(generatorOptions.PrintMode);

                this.CurrentUIHierarchyPath = uiHierarchyPath;

                WriteLine(facetList, PrintMode.PrintFacets);
                WriteLine(uiHierarchyPath, PrintMode.PrintUIHierarchyPath);

                if (container.HasFacetAttribute<IdentityAttribute>())
                {
                    var identityAttribute = container.GetFacetAttribute<IdentityAttribute>();

                    identitySetName = identityAttribute.IdentitySet;
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
                if (identitySetName != null)
                {
                    identitySet = container.EntitySets.SingleOrDefault(e => e.Name == identitySetName);

                    if (identitySet != null)
                    {
                        using (this.GeneratorConfiguration.ReportObject(identitySet))
                        {
                            ProcessEntitySet(identitySet, true);
                        }
                    }
                }

                foreach (var entitySet in container.EntitySets.Where(s => s.ID != identitySet?.ID))
                {
                    using (this.GeneratorConfiguration.ReportObject(entitySet))
                    {
                        if (entitySet.CanFollowWithout())
                        {
                            ProcessEntitySet(entitySet);
                        }
                    }
                }
            }
        }

        /// <summary>   Query if this  has registered application configuration. </summary>
        ///
        /// <remarks>   Ken, 10/25/2020. </remarks>
        ///
        /// <returns>   True if registered application configuration, false if not. </returns>

        private bool HasRegisteredAppConfig()
        {
            var appName = this.GeneratorConfiguration.AppName;
            var appDescription = this.GeneratorConfiguration.AppDescription;
            var clientId = this.GeneratorConfiguration.ClientId;
            var clientSecret = this.GeneratorConfiguration.ClientSecret;

            return CompareExtensions.AnyAreNotNull(appName, appDescription, clientId, clientSecret);
        }

        /// <summary>   Process the entity set. </summary>
        ///
        /// <remarks>   Ken, 10/25/2020. </remarks>
        ///
        /// <param name="entitySet">    Set the entity belongs to. </param>
        /// <param name="identitySet">  (Optional) True to identity set. </param>

        public void ProcessEntitySet(IEntitySet entitySet, bool identitySet = false)
        {
            IDisposable disposable = null;

            if (entitySet.HasUI())
            {
                disposable = this.GeneratorConfiguration.BeginChild(entitySet, false);
                ProcessUIEntitySet(entitySet);
            }

            using (this.GeneratorConfiguration.PushEntitySet(entitySet))
            {
                foreach (var entity in entitySet.Entities)
                {
                    if (entity.HasNavigationOrUI() || entity.CanFollowWithout() || identitySet)
                    {
                        using (this.GeneratorConfiguration.BeginChild(entity, true))
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

            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        /// <summary>   Document entity. </summary>
        ///
        /// <remarks>   Ken, 10/25/2020. </remarks>
        ///
        /// <param name="entity">       The entity. </param>
        /// <param name="identitySet">  True to identity set. </param>

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

        /// <summary>   Process the user interface entity set described by entitySet. </summary>
        ///
        /// <remarks>   Ken, 10/25/2020. </remarks>
        ///
        /// <param name="entitySet">    Set the entity belongs to. </param>

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

                WriteLine(facetList, PrintMode.PrintFacets);
                WriteLine(uiHierarchyPath, PrintMode.PrintUIHierarchyPath);
            }

            handlerStackItem = this.GeneratorConfiguration.HandleFacets(entitySet);

            this.GeneratorConfiguration.Push(handlerStackItem);
        }

        /// <summary>   Process the entity. </summary>
        ///
        /// <remarks>   Ken, 10/25/2020. </remarks>
        ///
        /// <param name="entity">           The entity. </param>
        /// <param name="isIdentityEntity"> (Optional) True if is identity, false if not. </param>

        public void ProcessEntity(IEntityType entity, bool isIdentityEntity = false)
        {
            HandlerStackItem handlerStackItem;
            var tabText = this.CurrentTabText;

            if (entity.Facets.Length > 0)
            {
                var facetList = entity.Facets.Select(f => string.Format("{0}[{1}]", tabText, f.AttributeCode)).ToMultiLineList();
                var uiHierarchyPath = entity.Facets.GetUIHierarchyPathList(generatorOptions.PrintMode);

                this.CurrentUIHierarchyPath = uiHierarchyPath;

                WriteLine(facetList, PrintMode.PrintFacets);
                WriteLine(uiHierarchyPath, PrintMode.PrintUIHierarchyPath);
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
                            using (this.GeneratorConfiguration.BeginChild(property, true))
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

        /// <summary>   Gets the current tab text. </summary>
        ///
        /// <value> The current tab text. </value>

        public string CurrentTabText
        {
            get
            {
                var tabs = this.GeneratorConfiguration.HierarchyStack.Count;
                var tabText = new string(' ', tabs * 2);

                return tabText;
            }
        }

        public IEnumerable<IModel> AllModels
        {
            get
            {
                var config = this.GeneratorConfiguration;
                var assembly = new AssemblyProvider.Web.Entities.Assembly(this.entitiesAssembly, t => t.BaseType != null && t.BaseType.Name == typeof(DbContext).Name);
                var edmxContainers = new List<IEntityContainer>();

                foreach (var modelItem in entitiesProject.EDMXModels)
                {
                    var model = new Model(new System.IO.FileInfo(modelItem.FileName), nameSpace);

                    edmxContainers.AddRange(model.Containers);

                    model.OnGetAttributes += Model_OnGetAttributes;
                    model.OnGetAddIns += Model_OnGetAddIns;
                    model.OnGetOverrides += Model_OnGetOverrides;

                    yield return model;
                }

                foreach (var model in assembly.GetModels(this.GeneratorConfiguration).Where(m => !entitiesProject.EDMXModels.Any(m2 => m2.Name == m.Name)))
                {
                    if (model.Containers.Any(c => !edmxContainers.Any(c2 => c2.GetFullName() == c.GetFullName())))
                    {
                        model.OnGetAttributes += Model_OnGetAttributes;
                        model.OnGetAddIns += Model_OnGetAddIns;
                        model.OnGetOverrides += Model_OnGetOverrides;

                        yield return model;
                    }
                }

                foreach (var restModelItem in entitiesProject.RestModels)
                {
                    var model = new RestModel(new System.IO.FileInfo(restModelItem.FileName), config.AdditionalOptions, nameSpace);

                    model.OnGetAttributes += Model_OnGetAttributes;
                    model.OnGetAddIns += Model_OnGetAddIns;
                    model.OnGetOverrides += Model_OnGetOverrides;

                    yield return model;
                }
            }
        }

        /// <summary>   Process the navigation property described by property. </summary>
        ///
        /// <remarks>   Ken, 10/25/2020. </remarks>
        ///
        /// <param name="property"> The property. </param>

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

                WriteLine(facetList, PrintMode.PrintFacets);
                WriteLine(uiHierarchyPath, PrintMode.PrintUIHierarchyPath);
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

                    using (this.GeneratorConfiguration.BeginChild(childEntity, true))
                    {
                        ProcessEntity(childEntity);
                    }
                }
            }

            this.GeneratorConfiguration.AddNavigationProperty(property);
        }

        /// <summary>   Process the property described by property. </summary>
        ///
        /// <remarks>   Ken, 10/25/2020. </remarks>
        ///
        /// <param name="property"> The property. </param>
        ///
        /// <returns>   A HandlerStackItem. </returns>

        public HandlerStackItem ProcessProperty(IEntityProperty property)
        {
            var tabs = this.GeneratorConfiguration.HierarchyStack.Count;
            var tabText = this.CurrentTabText;
            HandlerStackItem handlerStackItem;

            if (property.Facets.Length > 0)
            {
                var facetList = property.Facets.Select(f => string.Format("{0}[{1}]", tabText, f.AttributeCode)).ToMultiLineList();
                var uiHierarchyPath = property.Facets.GetUIHierarchyPathList(generatorOptions.PrintMode);

                currentUIHierarchyPath = uiHierarchyPath;

                WriteLine(facetList, PrintMode.PrintFacets);
                WriteLine(uiHierarchyPath, PrintMode.PrintUIHierarchyPath);
            }

            handlerStackItem = this.GeneratorConfiguration.HandleFacets(property);
            
            WriteLine("{0}{1} ({2})", PrintMode.All, tabText, property.Name, property.DataType.Name);
            
            this.GeneratorConfiguration.AddProperty(property);

            return handlerStackItem;
        }

        /// <summary>   Reads the key. </summary>
        ///
        /// <remarks>   Ken, 10/25/2020. </remarks>

        public void ReadKey()
        {
            if (GeneratorMode == GeneratorMode.Console)
            {
                try
                {
                    Console.ReadKey();
                }
                catch (Exception ex)
                {
                    WriteLine(ex.Message);
                }

                this.GeneratorConfiguration.StopServices();
            }
        }

        /// <summary>   Writes a line. </summary>
        ///
        /// <remarks>   Ken, 10/25/2020. </remarks>
        ///
        /// <param name="format">       Describes the format to use. </param>
        /// <param name="printMode">    The print mode. </param>
        /// <param name="args">         A variable-length parameters list containing arguments. </param>

        public void WriteLine(string format, PrintMode printMode, params object[] args)
        {
            var output = string.Format(format, args);

            if (output.Trim().Length > 0)
            {
                if ((generatorOptions.PrintMode == PrintMode.All && (!printMode.HasFlag(PrintMode.ExcludeFromAll))) || printMode.HasAnyFlag(generatorOptions.PrintMode) || printMode == PrintMode.Any)
                {
                    if (GeneratorMode == GeneratorMode.Console)
                    {
                        generatorOptions.OutputWriter.WriteLine(output);
                    }
                    else if (GeneratorMode == GeneratorMode.RedirectedConsole)
                    {
                        generatorOptions.RedirectedWriter.WriteLine(output);
                        generatorOptions.RedirectedWriter.Flush();
                    }
                }

                if (this.GeneratorConfiguration?.Logger != null)
                {
                    var logger = this.GeneratorConfiguration.Logger;

                    logger.Information(output);
                }
            }
        }

        /// <summary>   Writes an error. </summary>
        ///
        /// <remarks>   Ken, 10/25/2020. </remarks>
        ///
        /// <param name="format">   Describes the format to use. </param>
        /// <param name="args">     A variable-length parameters list containing arguments. </param>

        public void WriteError(string format, params object[] args)
        {
            var output = string.Format(format, args);

            if (GeneratorMode == GeneratorMode.Console)
            {
                using (new ConsoleColorizer(ConsoleColor.Red))
                {
                    generatorOptions.OutputWriter.WriteLine(output);
                }
            }
            else if (GeneratorMode == GeneratorMode.RedirectedConsole)
            {
                generatorOptions.RedirectedWriter.WriteLine(output);
                generatorOptions.RedirectedWriter.Flush();
            }

            if (this.GeneratorConfiguration?.Logger != null)
            {
                var logger = this.GeneratorConfiguration.Logger;

                logger.Error(output);
            }
        }

        /// <summary>   Writes a line. </summary>
        ///
        /// <remarks>   Ken, 10/25/2020. </remarks>
        ///
        /// <param name="format">   Describes the format to use. </param>
        /// <param name="args">     A variable-length parameters list containing arguments. </param>

        public void WriteLine(string format, params object[] args)
        {
            var output = string.Format(format, args);

            if (GeneratorMode == GeneratorMode.Console)
            {
                generatorOptions.OutputWriter.WriteLine(output);
            }
            else if (GeneratorMode == GeneratorMode.RedirectedConsole)
            {
                generatorOptions.RedirectedWriter.WriteLine(output);
                generatorOptions.RedirectedWriter.Flush();
            }

            if (this.GeneratorConfiguration?.Logger != null)
            {
                var logger = this.GeneratorConfiguration.Logger;

                logger.Information(output);
            }
        }

        /// <summary>   Handles the module assembly. </summary>
        ///
        /// <remarks>   Ken, 10/25/2020. </remarks>
        ///
        /// <param name="moduleAssembly">   The module assembly. </param>
        /// <param name="folder">           Pathname of the folder. </param>

        public void HandleModuleAssembly(IModuleAssembly moduleAssembly, Folder folder)
        {
            var config = this.GeneratorConfiguration;
            var angularModule = (AngularModule)moduleAssembly;

            config.HandleModuleKind(moduleAssembly, folder, ModuleKind.AngularModule);
        }

        /// <summary>   Indents this. </summary>
        ///
        /// <remarks>   Ken, 10/5/2020.  Not used, uses hierarch stack. </remarks>

        public void Indent()
        {
            throw new NotImplementedException();
        }

        /// <summary>   Dedents this. </summary>
        ///
        /// <remarks>   Ken, 10/5/2020.  Not used, uses hierarch stack. </remarks>

        public void Dedent()
        {
            throw new NotImplementedException();
        }

        public void EndProcessing(IGeneratorConfiguration generatorConfiguration)
        {
            if (this.NetCoreReflectionAgent != null)
            {
                this.NetCoreReflectionAgent.Dispose();
            }

            this.SendCallbackStatus(generatorConfiguration.CurrentPass, "Completion", "Finished", 100);
        }
    }
}
