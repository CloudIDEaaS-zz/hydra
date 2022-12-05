using AbstraX.Angular;
using AbstraX.Angular.Routes;
using AbstraX.DataAnnotations;
using AbstraX.Generators.Client.EntityProvider;
using AbstraX.Generators.Client.RouteGuardProvider;
using AbstraX.ObjectProperties;
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

namespace AbstraX.Generators.Client.AppDataProvider
{
    [GeneratorTemplate("Class", @"Generators\Client\AppDataProvider\AppDataProviderClassTemplate.tt")]
    public static class AppDataProviderGenerator
    {
        public static void GenerateProvider(IBase baseObject, string providersFolderPath, IGeneratorConfiguration generatorConfiguration)
        {
            var host = generatorConfiguration.GetTemplateEngineHost();
            var pass = generatorConfiguration.CurrentPass;
            var uiPathTree = generatorConfiguration.GetUIPathTree();
            Dictionary<string, object> sessionVariables;
            FileInfo fileInfo;
            string fileLocation;
            string filePath;
            string output;

            try
            {
                sessionVariables = new Dictionary<string, object>();

                fileLocation = providersFolderPath;
                filePath = PathCombine(fileLocation, "appdata.provider.ts");
                fileInfo = new FileInfo(filePath);

                host.SetGenerator(typeof(AppDataProviderGenerator), "Class");

                do
                {
                    FolderStructure.File file;

                    output = host.Generate<AppDataProviderClassTemplate>(sessionVariables, false);

                    file = generatorConfiguration.CreateFile(fileInfo, output, FileKind.Project, () => generatorConfiguration.GenerateInfo(sessionVariables, "AppData Provider Class"));
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
