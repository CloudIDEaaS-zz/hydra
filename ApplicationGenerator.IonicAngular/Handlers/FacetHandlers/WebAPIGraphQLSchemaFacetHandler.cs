using AbstraX.Angular;
using AbstraX.DataAnnotations;
using AbstraX.FolderStructure;
using AbstraX.Generators;
using AbstraX.Generators.Server.WebAPIGraphQLSchema;
using AbstraX.Generators.Server.WebAPIRestConfigJson;
using AbstraX.Models;
using AbstraX.Models.Interfaces;
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
    [FacetHandler(typeof(UIAttribute), UIKindGuids.StaticContainer)]
    public class WebAPIGraphQLSchemaFacetHandler : IFacetHandler
    {
        public float Priority => 3.0f;
        public bool ForLife => false;
        public event ProcessFacetsHandler ProcessFacets;
        public List<Module> RelatedModules { get; }
        public FacetHandlerLayer FacetHandlerLayer => FacetHandlerLayer.WebService;

        public WebAPIGraphQLSchemaFacetHandler()
        {
            this.RelatedModules = new List<Module>();
        }

        public bool Process(IBase baseObject, Facet facet, IGeneratorConfiguration generatorConfiguration)
        {
            var container = (IEntityContainer)baseObject;
            var servicesPath = generatorConfiguration.ApplicationFolderHierarchy[IonicFileSystemType.WebAPIServicesRoot];
            var graphQLPath = FileSystemObject.PathCombine(servicesPath, "GraphQL");
            string name;
            string dataContext;
            string dataContextNamespace;

            if (container is RestEntityContainer)
            {
                var entityWithPathPrefix = baseObject.CastTo<IEntityWithPrefix>();

                name = (string)((RestEntityContainer)container).Variables["title"];
                dataContext = "I" + name + "DataContext";
                dataContextNamespace = ((RestEntityContainer)container).Namespace + ".Models." + entityWithPathPrefix.PathPrefix;
            }
            else
            {
                name = baseObject.Name.RemoveEndIfMatches("Entities", "Context");
                dataContext = baseObject.Name;
                dataContextNamespace = ((IEntityContainer)container).Namespace;
            }

            WebAPIGraphQLSchemaGenerator.GenerateSchema(baseObject, graphQLPath, name, dataContext, dataContextNamespace, generatorConfiguration);

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
