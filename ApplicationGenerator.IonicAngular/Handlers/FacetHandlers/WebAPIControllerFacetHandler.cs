using AbstraX.Angular;
using AbstraX.DataAnnotations;
using AbstraX.FolderStructure;
using AbstraX.Generators;
using AbstraX.Generators.Server.WebAPIController;
using AbstraX.ServerInterfaces;
using EntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Handlers.FacetHandlers
{
    [FacetHandler(typeof(UIAttribute), UIKindGuids.Element)]
    public class WebAPIControllerFacetHandler : IFacetHandler
    {
        public float Priority => 3.0f;
        public bool ForLife => false;
        public event ProcessFacetsHandler ProcessFacets;
        public List<Module> RelatedModules { get; }
        public FacetHandlerLayer FacetHandlerLayer => FacetHandlerLayer.WebService;

        public WebAPIControllerFacetHandler()
        {
            this.RelatedModules = new List<Module>();
        }

        public bool Process(IBase baseObject, Facet facet, IGeneratorConfiguration generatorConfiguration)
        {
            var name = baseObject.Name;
            var parentObject = (IElement)baseObject;
            var controllersPath = generatorConfiguration.ApplicationFolderHierarchy[IonicFileSystemType.WebAPIControllers];
            var controllersFolder = (Folder)generatorConfiguration.FileSystem[controllersPath];
            var relatedEntities = new List<RelatedEntity>();
            var entityProperties = new List<Generators.EntityProperty>();
            var element = (IElement)baseObject;

            foreach (var attribute in element.Attributes)
            {
                entityProperties.Add(new Generators.EntityProperty(attribute, generatorConfiguration));
            }

            foreach (var childObject in parentObject.GetParentNavigationProperties(generatorConfiguration.PartsAliasResolver))
            {
                relatedEntities.Add(new RelatedEntity(childObject, generatorConfiguration));
            }

            WebAPIControllerGenerator.GenerateController(baseObject, controllersPath, name, generatorConfiguration, relatedEntities, entityProperties);

            return true;
        }

        public void AddRange(IBase baseObject, IGeneratorConfiguration generatorConfiguration, List<Module> modules, IEnumerable<Module> addModules, ModuleAddType moduleAddType)
        {
            modules.AddRange(addModules);
        }

        public bool PreProcess(IBase baseObject, IGeneratorConfiguration generatorConfiguration, IFacetHandler currentHandler)
        {
            return true;
        }

        public bool PostProcess(IBase baseObject, IGeneratorConfiguration generatorConfiguration, IFacetHandler currentHandler)
        {
            return true;
        }
    }
}
