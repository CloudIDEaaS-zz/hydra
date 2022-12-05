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

namespace AbstraX.Generators.Server.WebAPIRestProviderDataContext
{
    [GeneratorTemplate("Class", @"Generators\Server\WebAPIRestProviderDataContext\WebAPIRestProviderDataContextClassTemplate.tt")]
    public static class WebAPIRestProviderDataContextGenerator
    {
        public static void GenerateDataContext(IBase baseObject, string providerModelsPath, string title, IGeneratorConfiguration generatorConfiguration)
        {
            var host = generatorConfiguration.GetTemplateEngineHost();
            var container = (RestEntityContainer)baseObject;
            var rootObject = (object)container.JsonRootObject;
            Dictionary<string, object> sessionVariables;
            FileInfo fileInfo;
            string name;
            string fileLocation;
            string filePath;
            string output;

            try
            {
                // WebAPI Rest Data context (DbContext)

                sessionVariables = new Dictionary<string, object>();
                name = (string)container.Variables["title"] + "DataContext";

                sessionVariables.Add("Name", name);
                sessionVariables.Add("Title", title);
                sessionVariables.Add("EntitySets", container.EntitySets);

                if (baseObject is IEntityWithPrefix)
                {
                    var entityWithPathPrefix = baseObject.CastTo<IEntityWithPrefix>();

                    fileLocation = PathCombine(providerModelsPath, baseObject.CastTo<IEntityWithPrefix>().PathPrefix);
                    sessionVariables.Add("RootNamespace", entityWithPathPrefix.Namespace);
                }
                else
                {
                    fileLocation = providerModelsPath;
                    sessionVariables.Add("RootNamespace", generatorConfiguration.AppName);
                }

                filePath = PathCombine(fileLocation, name + ".cs");
                fileInfo = new FileInfo(filePath);

                host.SetGenerator(typeof(WebAPIRestProviderDataContextGenerator), "Class");

                do
                {
                    output = host.Generate<WebAPIRestProviderDataContextClassTemplate>(sessionVariables, false);

                    generatorConfiguration.CreateFile(fileInfo, output, FileKind.Services, () => generatorConfiguration.GenerateInfo(sessionVariables, "WebAPI Rest Data Context"));
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
