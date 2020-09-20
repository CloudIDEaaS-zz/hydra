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

namespace AbstraX.Generators.Server.WebAPIRestConfigJson
{
    public static class WebAPIRestConfigJsonGenerator
    {
        public static void GenerateJson(IBase baseObject, string configPath, IGeneratorConfiguration generatorConfiguration)
        {
            var host = new TemplateEngineHost();
            var container = (RestEntityContainer)baseObject;
            var rootObject = (object)container.JsonRootObject;
            Dictionary<string, object> sessionVariables;
            Dictionary<string, string> nameValueDictionary;
            FileInfo fileInfo;
            string fileLocation;
            string filePath;
            string output;

            try
            {
                // WebAPI Rest config json

                sessionVariables = new Dictionary<string, object>();

                nameValueDictionary = rootObject.GetDynamicMemberNameValueDictionary().Where(p => !p.Key.IsOneOf("$schema", "title", "type", "clientControllerRouteBase") && p.Value is string).ToDictionary(p => p.Key, p => (string) p.Value);

                sessionVariables.Add("NameValues", nameValueDictionary);

                fileLocation = configPath;
                filePath = PathCombine(fileLocation, "Config.json");
                fileInfo = new FileInfo(filePath);

                output = host.Generate<WebAPIRestConfigJsonTemplate>(sessionVariables, false);

                generatorConfiguration.CreateFile(fileInfo, output, FileKind.Services, () => generatorConfiguration.GenerateInfo(sessionVariables, "Config.json"));
            }
            catch (Exception e)
            {
                generatorConfiguration.AppGeneratorEngine.WriteError(e.ToString());
            }
        }
    }
}
