using AbstraX.Angular;
using AbstraX.DataAnnotations;
using AbstraX.FolderStructure;
using AbstraX.Generators;
using AbstraX.Generators.Server.WebAPIRestConfigJson;
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
    public class WebAPIRestConfigJsonFacetHandler : IFacetHandler
    {
        public float Priority => 3.0f;
        public bool ForLife => false;
        public event ProcessFacetsHandler ProcessFacets;
        public List<Module> RelatedModules { get; }
        public FacetHandlerLayer FacetHandlerLayer => FacetHandlerLayer.WebService;

        public WebAPIRestConfigJsonFacetHandler()
        {
            this.RelatedModules = new List<Module>();
        }

        public bool Process(IBase baseObject, Facet facet, IGeneratorConfiguration generatorConfiguration)
        {
            var name = baseObject.Name;
            var container = (RestEntityContainer)baseObject;
            var variables = container.Variables;
            var servicesPath = generatorConfiguration.ApplicationFolderHierarchy[IonicFileSystemType.WebAPIServicesRoot];
            var configPath = FileSystemObject.PathCombine(servicesPath, "wwwroot", (string) container.Variables["title"] + "Config");
            var parentPath = (string)variables["parentPath"];
            var parentObject = baseObject.EntityDictionary.Single(p => p.Value.GetCondensedID() == parentPath).Value;
            var rootObject = (object)container.JsonRootObject;

            rootObject.SetDynamicMember("idBase", parentObject.GetCondensedID().ToBase64());

            WebAPIRestConfigJsonGenerator.GenerateJson(baseObject, configPath, generatorConfiguration);

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
