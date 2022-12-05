using AbstraX.Angular;
using AbstraX.DataAnnotations;
using AbstraX.FolderStructure;
using AbstraX.Generators;
using AbstraX.Generators.Client.EntityProvider;
using AbstraX.ServerInterfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Handlers.FacetHandlers
{
    [FacetHandler(typeof(UIAttribute), UIKindGuids.Element, ModuleImports.IONIC_ANGULAR_BASIC_PAGE_IMPORTS)]
    public class EntityProviderFacetHandler : IModuleAddFacetHandler
    {
        public float Priority => 2.0f;
        public bool ForLife => false;
        public event ProcessFacetsHandler ProcessFacets;
        public List<Module> RelatedModules { get; }
        public FacetHandlerLayer FacetHandlerLayer => FacetHandlerLayer.Client;

        public EntityProviderFacetHandler()
        {
            this.RelatedModules = new List<Module>();
        }

        public bool Process(IBase baseObject, Facet facet, IGeneratorConfiguration generatorConfiguration)
        {
            var name = baseObject.Name;
            var parentObject = (IElement)baseObject;
            var providersPath = generatorConfiguration.ApplicationFolderHierarchy[IonicFileSystemType.Providers];
            var providersFolder = (Folder)generatorConfiguration.FileSystem[providersPath];
            var imports = generatorConfiguration.CreateImports(this, baseObject, providersFolder, true);
            var relatedEntities = new List<RelatedEntity>();

            foreach (var childObject in parentObject.GetParentNavigationProperties(generatorConfiguration.PartsAliasResolver))
            {
                relatedEntities.Add(new RelatedEntity(childObject, generatorConfiguration));
            }

            EntityProviderGenerator.GenerateProvider(baseObject, providersPath, name, generatorConfiguration, imports, relatedEntities);

            return true;
        }

        public void AddRange(IBase baseObject, IGeneratorConfiguration generatorConfiguration, List<Module> modules, IEnumerable<Module> addModules, ModuleAddType moduleAddType)
        {
            modules.AddRange(addModules.Where(m => !m.Attributes.Any(a => a == "Component")));
        }

        public void AddRange(IBase baseObject, IGeneratorConfiguration generatorConfiguration, List<Module> modules, ModuleAddType moduleAddType, Func<Module, bool> filter)
        {
            var generatorModules = (IEnumerable<ESModule>)generatorConfiguration.KeyValuePairs["Modules"];
            var addModules = generatorModules.Where(m => filter(m));

            modules.AddRange(addModules);
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
