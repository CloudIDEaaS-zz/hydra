using AbstraX.Angular;
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

namespace AbstraX.Generators.Server.WebAPIStartupGraphQLSchemas
{
    public static class WebAPIStartupGraphQLSchemasGenerator
    {
        public static void GenerateStartupSchemas(string graphQLPath, IEnumerable<GraphQLSchema> schemas, IGeneratorConfiguration generatorConfiguration)
        {
            var host = new TemplateEngineHost();
            Dictionary<string, object> sessionVariables;
            FileInfo fileInfo;
            string fileLocation;
            string filePath;
            string output;

            if (graphQLPath == null)
            {
                return;
            }

            try
            {
                // WebAPI StartupSchemas.cs

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("RootNamespace", generatorConfiguration.AppName);
                sessionVariables.Add("Schemas", schemas.Select(s => s.SchemaName).ToArray());

                fileLocation = graphQLPath;
                filePath = PathCombine(fileLocation, "StartupSchemas.cs");
                fileInfo = new FileInfo(filePath);

                output = host.Generate<WebAPIStartupGraphQLSchemasClassTemplate>(sessionVariables, false);

                generatorConfiguration.CreateFile(fileInfo, output, FileKind.Services, () => generatorConfiguration.GenerateInfo(sessionVariables, "StartupSchemas.cs"));
            }
            catch (Exception e)
            {
                generatorConfiguration.AppGeneratorEngine.WriteError(e.ToString());
            }
        }
    }
}
