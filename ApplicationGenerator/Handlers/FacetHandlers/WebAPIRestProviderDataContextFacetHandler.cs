using AbstraX.Angular;
using AbstraX.DataAnnotations;
using AbstraX.FolderStructure;
using AbstraX.Generators;
using AbstraX.Generators.Server.WebAPIRestProviderDataContext;
using AbstraX.ServerInterfaces;
using EntityProvider.Web.Entities;
using RestEntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Handlers.FacetHandlers
{
    public class WebAPIRestProviderDataContextFacetHandler : IFacetHandler
    {
        public float Priority => 3.0f;
        public bool ForLife => false;
        public event ProcessFacetsHandler ProcessFacets;
        public List<Module> RelatedModules { get; }
        public FacetHandlerLayer FacetHandlerLayer => FacetHandlerLayer.WebService;

        public WebAPIRestProviderDataContextFacetHandler()
        {
            this.RelatedModules = new List<Module>();
        }

        public bool Process(IBase baseObject, Facet facet, IGeneratorConfiguration generatorConfiguration)
        {
            var name = baseObject.Name;
            var container = (RestEntityContainer)baseObject;
            var modelsPath = generatorConfiguration.ApplicationFolderHierarchy[IonicFileSystemType.WebAPIModels];
            var modelsFolder = (Folder)generatorConfiguration.FileSystem[modelsPath];
            var title = (string)container.Variables["title"];

            WebAPIRestProviderDataContextGenerator.GenerateDataContext(baseObject, modelsPath, title, generatorConfiguration);

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
