using AbstraX.FolderStructure;
using AbstraX.Models.Interfaces;
using AbstraX.PackageCache;
using AbstraX.ServerInterfaces;
using AbstraX.Validation;
using CodeInterfaces;
using EntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Unity;

namespace AbstraX
{
    public interface IGeneratorConfiguration : IPackageCacheStatusProvider
    {
        string AppName { get; }
        int IndentLevel { get; set; }
        string AppDescription { get; }
        string IdentityProvider { get; set; }
        string PackagePathCache { get; }
        List<string> GraphQLTypes { get; }
        Dictionary<string, object> AdditionalOptions { get; }
        string AuthorizedRoles { get; }
        Dictionary<string, XDocument> AbstraXSchemaDocuments { get; }
        IUnityContainer UnityContainer { get; }
        ResourcesHandler ResourcesHandler { get; }
        Dictionary<Guid, string> Roles { get; }
        QueryDictionary CustomQueries { get; }
        IAppGeneratorEngine AppGeneratorEngine { get; }
        IWorkspaceGeneratorEngine WorkspaceGeneratorEngine { get; }
        Dictionary<string, string> InputFiles { get; }
        GeneratorPass ConfiguredPass { get; set; }
        GeneratorPass CurrentPass { get; set; }
        RecursionMode RecursionMode { get; }
        Guid ProjectType { get; }
        string ProjectFolderRoot { get; }
        IVSProject ServicesProject { get; }
        void Indent();
        void Dedent();
        Stack<HandlerStackItem> HandlerStack { get; }
        Dictionary<string, IViewProject> ViewProjects { get; }
        Stack<HierarchyStackItem> HierarchyStack { get; }
        string HierarchyPath { get; }
        Dictionary<string, object> KeyValuePairs { get; }
        List<ICustomHandler> CustomHandlers { get; }
        List<IFacetHandler> OtherHandlers { get; }
        IEnumerable<string> PackageInstalls { get; }
        IEnumerable<Module> BuiltInModules { get; }
        string ClientId { get; }
        string ClientSecret { get; }
        ApplicationFolderHierarchy ApplicationFolderHierarchy { get; }
        bool NoFileCreation { get; }
        IEntityWithFacets IdentityEntity { get; }
        string AddTranslation(ServerInterfaces.IBase baseObject, string key, string value, bool skipIfSame);
        void AddBuiltInModule(params Module[] module);
        HandlerStackItem HandleFacets(IEntityWithFacets entityWithFacets, bool isIdentityEntity = false);
        bool IsIdentityEntity(ServerInterfaces.IBase entityWithFacets);
        bool HandleModuleKind(IModuleObject moduleObject, Folder folder, Enum moduleKind);
        bool HandleViews(IViewProject project, ServerInterfaces.IBase baseObject, ServerInterfaces.Facet facet);
        IValidationSet BuildValidationSet(ServerInterfaces.IBase baseObject);
        void AddPackageInstalls(IFacetHandler facetHandler);
        IEnumerable<ModuleImportDeclaration> CreateImports(IModuleHandler baseHandler, ServerInterfaces.IBase baseObject, Folder folder, bool includeSelf = false, int subFolderCount = 0);
        IEnumerable<ModuleImportDeclaration> CreateImports(IModuleKindHandler moduleKindHandler, IModuleAssembly moduleAssembly, Folder folder, bool includeSelf = false, int subFolderCount = 0);
        IEnumerable<ModuleImportDeclaration> CreateImports(IModuleKindHandler moduleKindHandler, IModuleAssembly moduleAssembly, List<IModuleOrAssembly> modulesOrAssemblies, Folder folder, bool includeSelf = false, int subFolderCount = 0);
        IDictionary<string, IEnumerable<ModuleImportDeclaration>> CreateImportGroups(IFacetHandler facetHandler, ServerInterfaces.IBase baseObject, Folder folder, bool includeSelf = false);
        IEnumerable<IExpressionHandler> GetExpressionHandlers(Guid providerGuid);
        IExpressionHandler GetExpressionHandler(Guid providerGuid);
        IModuleAssembly PushModuleAssembly<T>(string namePrefix) where T : IModuleAssembly, new();
        IDisposable PushContainer(IEntityContainer container);
        IDisposable PushEntitySet(IEntitySet entitySet);
        IDisposable PushEntityType(IEntityType entity);
        void AddNavigationProperty(INavigationProperty property);
        void AddProperty(ServerInterfaces.IAttribute attribute);
        void AddFacets(IEntityWithFacets entityWithFacets);
        void SetModuleAssemblyProperties(IModuleAssemblyProperties properties);
        void SetModuleAssemblyFolder(Folder folder);
        System.IO.Stream CreateFile(string filePath);
        System.IO.DirectoryInfo CreateDirectory(string filePath);
        File CreateFile(IModuleAssemblyProperties moduleAssemblyProperties, System.IO.FileInfo fileInfo, string output, FileKind fileKind, Func<StringBuilder> hierarchyGenerator);
        File CreateFile(System.IO.FileInfo fileInfo, string output, FileKind fileKind, Func<StringBuilder> hierarchyGenerator);
        File CreateFile(System.IO.FileInfo fileInfo, IEnumerable<Module> nonAssemblyModules, string output, FileKind fileKind, Func<StringBuilder> hierarchyGenerator);
        File CreateFile(System.IO.FileInfo fileInfo, string output, FileKind fileKind, IModuleAssembly moduleAssembly);
        FileSystem FileSystem { get; }
        LanguageDictionary LanguageDictionary { get; }
        PartsAliasResolver PartsAliasResolver { get; }
        void Terminate();
    }
}
