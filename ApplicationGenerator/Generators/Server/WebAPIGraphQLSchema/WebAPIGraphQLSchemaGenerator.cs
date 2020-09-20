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
    public static class WebAPIGraphQLSchemaGenerator
    {
        public static void GenerateSchema(IBase baseObject, string graphQLPath, string name, string dataContext, string dataContextNamespace, IGeneratorConfiguration generatorConfiguration)
        {
            var host = new TemplateEngineHost();
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

                output = host.Generate<WebAPIGraphQLSchemaClassTemplate>(sessionVariables, false);

                generatorConfiguration.CreateFile(fileInfo, output, FileKind.Services, () => generatorConfiguration.GenerateInfo(sessionVariables, "GraphQL Schema class"));

                // GraphQL Query class

                sessionVariables.Add("EntitySets", container.EntitySets);
                sessionVariables.Add("DataContext", dataContext);
                sessionVariables.Add("DataContextNamespace", dataContextNamespace);
                sessionVariables.Add("GraphQLTypes", graphQLTypes);

                fileLocation = graphQLPath;
                filePath = PathCombine(fileLocation, name + "Query.cs");
                fileInfo = new FileInfo(filePath);

                output = host.Generate<WebAPIGraphQLQueryClassTemplate>(sessionVariables, false);

                generatorConfiguration.CreateFile(fileInfo, output, FileKind.Services, () => generatorConfiguration.GenerateInfo(sessionVariables, "GraphQL Query class"));
            }
            catch (Exception e)
            {
                generatorConfiguration.AppGeneratorEngine.WriteError(e.ToString());
            }
        }
    }
}
