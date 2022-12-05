using AbstraX.Angular;
using AbstraX.DataAnnotations;
using AbstraX.FolderStructure;
using AbstraX.Generators;
using AbstraX.Generators.Server.ConfigJson;
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
    [FacetHandler(typeof(UIAttribute), UIKindGuids.StaticContainer)]
    public class ConfigJsonFacetHandler : IForLifeFacetHandler
    {
        public float Priority => 3.0f;
        public bool ForLife => true;
        public event ProcessFacetsHandler ProcessFacets;
        public List<Module> RelatedModules { get; }
        public FacetHandlerLayer FacetHandlerLayer => FacetHandlerLayer.ServerConfig;

        public ConfigJsonFacetHandler()
        {
            this.RelatedModules = new List<Module>();
        }

        public bool Process(IBase baseObject, Facet facet, IGeneratorConfiguration generatorConfiguration)
        {
            return true;
        }

        public bool Terminate(IGeneratorConfiguration generatorConfiguration)
        {
            var servicesPath = generatorConfiguration.ApplicationFolderHierarchy[IonicFileSystemType.WebAPIServicesRoot];
            var configPath = FileSystemObject.PathCombine(servicesPath, "wwwroot");

            ConfigJsonGenerator.GenerateJson(configPath, generatorConfiguration);

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
