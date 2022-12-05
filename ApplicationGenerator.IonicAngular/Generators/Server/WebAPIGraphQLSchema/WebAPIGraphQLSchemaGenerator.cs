using AbstraX.Angular;
using AbstraX.Generators.Server.WebAPIGraphQLSchema;
using AbstraX.Models.Interfaces;
using AbstraX.ServerInterfaces;
using EntityProvider.Web.Entities;
using RestEntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using static AbstraX.FolderStructure.FileSystemObject;

namespace AbstraX.Generators.Server.WebAPIGraphQLSchema
{
    [GeneratorTemplate("SchemaClass", @"Generators\Server\WebAPIGraphQLSchema\WebAPIGraphQLSchemaClassTemplate.tt")]
    [GeneratorTemplate("QueryClass", @"Generators\Server\WebAPIGraphQLSchema\WebAPIGraphQLQueryClassTemplate.tt")]
    public static class WebAPIGraphQLSchemaGenerator
    {
        public static void GenerateSchema(IBase baseObject, string graphQLPath, string name, string dataContext, string dataContextNamespace, IGeneratorConfiguration generatorConfiguration)
        {
            var host = generatorConfiguration.GetTemplateEngineHost();
            var container = (IEntityContainer)baseObject;
            var graphQLTypes = generatorConfiguration.GraphQLTypes;
            Dictionary<string, object> sessionVariables;
            FileInfo fileInfo;
            string fileLocation;
            string filePath;
            string output;

            try
            {
                // GraphQL Schema class

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("Name", name);
                sessionVariables.Add("RootNamespace", generatorConfiguration.AppName);

                fileLocation = graphQLPath;
                filePath = PathCombine(fileLocation, name + "Schema.cs");
                fileInfo = new FileInfo(filePath);

                host.SetGenerator(typeof(WebAPIGraphQLSchemaGenerator), "SchemaClass");

                do
                {
                    output = host.Generate<WebAPIGraphQLSchemaClassTemplate>(sessionVariables, false);

                    generatorConfiguration.CreateFile(fileInfo, output, FileKind.Services, () => generatorConfiguration.GenerateInfo(sessionVariables, "GraphQL Schema class"));
                }
                while (host.PostProcess() == PostProcessResult.RedoGenerate);

                // GraphQL Query class

                sessionVariables.Add("EntitySets", container.EntitySets);
                sessionVariables.Add("DataContext", dataContext);
                sessionVariables.Add("DataContextNamespace", dataContextNamespace);
                sessionVariables.Add("GraphQLTypes", graphQLTypes);

                fileLocation = graphQLPath;
                filePath = PathCombine(fileLocation, name + "Query.cs");
                fileInfo = new FileInfo(filePath);

                host.SetGenerator(typeof(WebAPIGraphQLSchemaGenerator), "QueryClass");

                do
                {
                    output = host.Generate<WebAPIGraphQLQueryClassTemplate>(sessionVariables, false);

                    generatorConfiguration.CreateFile(fileInfo, output, FileKind.Services, () => generatorConfiguration.GenerateInfo(sessionVariables, "GraphQL Query class"));
                }
                while (host.PostProcess() == PostProcessResult.RedoGenerate);
            }
            catch (Exception e)
            {
                generatorConfiguration.AppGeneratorEngine.WriteError(e.ToString());
            }
        }
    }
}
