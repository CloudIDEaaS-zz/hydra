using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstraX.Angular;
using AbstraX.DataAnnotations;
using AbstraX.FolderStructure;
using AbstraX.Generators.Client.App;
using AbstraX.Generators.Client.Internationalization;
using AbstraX.Handlers.ImportHandlers.Packages;
using AbstraX.ServerInterfaces;
using EntityProvider.Web.Entities;
using Utils;

namespace AbstraX.Handlers.FacetHandlers
{
    [FacetHandler(typeof(IonicApplicationPackage))]
    public class ApplicationFacetHandler : IForLifeFacetHandler
    {
        public float Priority => 1.0f;
        public bool ForLife => true;
        public event ProcessFacetsHandler ProcessFacets;
        public List<Module> RelatedModules { get; }
        public FacetHandlerLayer FacetHandlerLayer => FacetHandlerLayer.Client;

        private AngularModule appModule;

        public ApplicationFacetHandler()
        {
            this.RelatedModules = new List<Module>();
        }

        public bool Process(IBase baseObject, Facet facet, IGeneratorConfiguration generatorConfiguration)
        {
            var projectRootPath = generatorConfiguration.ApplicationFolderHierarchy[IonicFileSystemType.AppProjectRoot];
            var appPath = generatorConfiguration.ApplicationFolderHierarchy[IonicFileSystemType.App];
            var module = generatorConfiguration.PushModuleAssembly<AngularModule>("App");
            var folder = (Folder)generatorConfiguration.FileSystem[appPath];

            generatorConfiguration.AddPackageInstalls(this);

            module.BaseObject = baseObject.Parent;
            appModule = (AngularModule) module;

            generatorConfiguration.SetModuleAssemblyFolder(folder);

            EmbeddedFilesGenerator.Generate(projectRootPath, generatorConfiguration);

            return true;
        }

        public void AddRange(IBase baseObject, IGeneratorConfiguration generatorConfiguration, List<Module> modules, IEnumerable<Module> addModules, ModuleAddType moduleAddType)
        {
            modules.AddRange(addModules);
        }

        public bool Terminate(IGeneratorConfiguration generatorConfiguration)
        {
            var appPath = generatorConfiguration.ApplicationFolderHierarchy[IonicFileSystemType.App];
            var folder = (Folder)generatorConfiguration.FileSystem[appPath];
            var imports = generatorConfiguration.CreateImports(SurrogateModuleKindHandler.GetInstance(), appModule, folder, true);
            var pages = new List<Page>();
            string i18nPath;

            foreach (var page in imports.Select(i => i.ModuleOrAssembly).Cast<Page>().OrderBy(c => c.UILoadKind == UILoadKind.RootPage))
            {
                pages.Add(page);
            }

            i18nPath = generatorConfiguration.ApplicationFolderHierarchy[IonicFileSystemType.i18n];

            AppGenerator.GenerateApp(appPath, "App", generatorConfiguration, imports, pages);
            LanguageTranslationGenerator.GenerateTranslations(i18nPath, generatorConfiguration);

            return true;
        }

        public bool PostProcess(IBase baseObject, IGeneratorConfiguration generatorConfiguration, IFacetHandler currentHandler)
        {
            return true;
        }

        public bool PreProcess(IBase baseObject, IGeneratorConfiguration generatorConfiguration, IFacetHandler currentHandler)
        {
            return true;
        }
    }
}
