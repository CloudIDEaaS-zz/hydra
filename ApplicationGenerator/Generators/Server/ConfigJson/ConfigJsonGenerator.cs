using AbstraX.Angular;
using AbstraX.Generators.Server.WebAPIModel;
using AbstraX.ServerInterfaces;
using EntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using static AbstraX.FolderStructure.FileSystemObject;

namespace AbstraX.Generators.Server.ConfigJson
{
    public static class ConfigJsonGenerator
    {
        public static void GenerateJson(string configPath, IGeneratorConfiguration generatorConfiguration)
        {
            var host = new TemplateEngineHost();
            Dictionary<string, object> sessionVariables;
            FileInfo fileInfo;
            string fileLocation;
            string filePath;
            string output;

            try
            {
                // config.json

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("ClientId", generatorConfiguration.ClientId);
                sessionVariables.Add("ClientSecret", generatorConfiguration.ClientSecret);
                sessionVariables.Add("Roles", generatorConfiguration.Roles);

                fileLocation = configPath;
                filePath = PathCombine(fileLocation, "Config.json");
                fileInfo = new FileInfo(filePath);

                output = host.Generate<ConfigJsonTemplate>(sessionVariables, false);

                generatorConfiguration.CreateFile(fileInfo, output, FileKind.Services, () => generatorConfiguration.GenerateInfo(sessionVariables, "Config.json"));
            }
            catch (Exception e)
            {
                generatorConfiguration.AppGeneratorEngine.WriteError(e.ToString());
            }
        }
    }
}
