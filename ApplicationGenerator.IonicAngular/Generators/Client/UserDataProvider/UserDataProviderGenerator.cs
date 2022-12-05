using AbstraX.Angular;
using AbstraX.Angular.Routes;
using AbstraX.DataAnnotations;
using AbstraX.Generators.Client.EntityProvider;
using AbstraX.Generators.Client.RouteGuardProvider;
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

namespace AbstraX.Generators.Client.UserDataProvider
{
    [GeneratorTemplate("Class", @"Generators\Client\UserDataProvider\UserDataProviderClassTemplate.tt")]
    public static class UserDataProviderGenerator
    {
        public static void GenerateProvider(IBase baseObject, string providersFolderPath, IGeneratorConfiguration generatorConfiguration)
        {
            var host = generatorConfiguration.GetTemplateEngineHost();
            var pass = generatorConfiguration.CurrentPass;
            var uiNavigationNameAttributes = baseObject.GetFacetAttributes<UINavigationNameAttribute>();
            var uiPathTree = generatorConfiguration.GetUIPathTree();
            Dictionary<string, object> sessionVariables;
            FileInfo fileInfo;
            string fileLocation;
            string filePath;
            string output;

            sessionVariables = new Dictionary<string, object>();

            sessionVariables.Add("IdentityProvider", generatorConfiguration.IdentityProvider);

            fileLocation = providersFolderPath;
            filePath = PathCombine(fileLocation, "userdata.provider.ts");
            fileInfo = new FileInfo(filePath);

            host.SetGenerator(typeof(UserDataProviderGenerator), "Class");

            do
            {
                output = host.Generate<UserDataProviderClassTemplate>(sessionVariables, false);

                generatorConfiguration.CreateFile(fileInfo, output, FileKind.Project, () => generatorConfiguration.GenerateInfo(sessionVariables, "User Data Provider Class"));
            }
            while (host.PostProcess() == PostProcessResult.RedoGenerate);
        }
    }
}
