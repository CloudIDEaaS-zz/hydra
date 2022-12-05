using AbstraX.Angular;
using AbstraX.DataAnnotations;
using AbstraX.FolderStructure;
using AbstraX.Generators;
using AbstraX.Generators.Server.WebAPIRestConfigJson;
using AbstraX.Generators.Server.WebAPIStartupGraphQLSchemas;
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
    public class WebAPIStartupGraphQLSchemasFacetHandler : ISingletonForLifeFacetHandler
    {
        public float Priority => 3.0f;
        public bool ForLife => true;
        public event ProcessFacetsHandler ProcessFacets;
        public List<Module> RelatedModules { get; }
        public FacetHandlerLayer FacetHandlerLayer => FacetHandlerLayer.WebService;
        private List<GraphQLSchema> schemas;
        private string graphQLPath;

        public WebAPIStartupGraphQLSchemasFacetHandler()
        {
            this.RelatedModules = new List<Module>();
            schemas = new List<GraphQLSchema>();
        }

        public bool Process(IBase baseObject, Facet facet, IGeneratorConfiguration generatorConfiguration)
        {
            var container = (IEntityContainer)baseObject;
            GraphQLSchema schema;
            string name;

            if (baseObject.SkipProcess(this, facet, generatorConfiguration))
            {
                return true;
            }

            if (graphQLPath == null)
            {
                var servicesPath = generatorConfiguration.ApplicationFolderHierarchy[IonicFileSystemType.WebAPIServicesRoot];
                
                graphQLPath = FileSystemObject.PathCombine(servicesPath, "GraphQL");
            }

            if (container is RestEntityContainer)
            {
                name = (string)((RestEntityContainer) container).Variables["title"];
            }
            else
            {
                name = baseObject.Name.RemoveEndIfMatches("Entities", "Context");
            }

            schema = new GraphQLSchema(baseObject, name, generatorConfiguration);

            if (generatorConfiguration.CurrentPass == GeneratorPass.Files)
            {
                this.schemas.Add(schema);
            }

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

        public bool Terminate(IGeneratorConfiguration generatorConfiguration)
        {
            WebAPIStartupGraphQLSchemasGenerator.GenerateStartupSchemas(graphQLPath, schemas, generatorConfiguration);

            return true;
        }
    }
}
