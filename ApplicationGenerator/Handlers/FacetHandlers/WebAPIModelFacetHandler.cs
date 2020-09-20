using AbstraX.Angular;
using AbstraX.DataAnnotations;
using AbstraX.FolderStructure;
using AbstraX.Generators;
using AbstraX.Generators.Server.WebAPIModel;
using AbstraX.ServerInterfaces;
using EntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Handlers.FacetHandlers
{
    [FacetHandler(typeof(UIAttribute), UIKindGuids.Element)]
    public class WebAPIModelFacetHandler : IFacetHandler
    {
        public float Priority => 3.0f;
        public bool ForLife => false;
        public event ProcessFacetsHandler ProcessFacets;
        public List<Module> RelatedModules { get; }
        public FacetHandlerLayer FacetHandlerLayer => FacetHandlerLayer.WebService;

        public WebAPIModelFacetHandler()
        {
            this.RelatedModules = new List<Module>();
        }

        public bool Process(IBase baseObject, Facet facet, IGeneratorConfiguration generatorConfiguration)
        {
            var name = baseObject.Name;
            var parentObject = (IElement)baseObject;
            var modelsPath = generatorConfiguration.ApplicationFolderHierarchy[IonicFileSystemType.WebAPIModels];
            var modelsFolder = (Folder)generatorConfiguration.FileSystem[modelsPath];
            var entityProperties = new List<Generators.EntityProperty>();
            var element = (IElement)baseObject;

            if (baseObject.SkipProcess(this, facet, generatorConfiguration))
            {
                return true;
            }

            foreach (var attribute in element.Attributes)
            {
                entityProperties.Add(new Generators.EntityProperty(attribute, generatorConfiguration));
            }

            WebAPIModelGenerator.GenerateModel(baseObject, modelsPath, name, generatorConfiguration, entityProperties);

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
