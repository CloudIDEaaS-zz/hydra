using AbstraX.Angular;
using AbstraX.Models.Interfaces;
using AbstraX.ServerInterfaces;
using ApplicationGenerator.Data;
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

namespace AbstraX.Generators.Server.WebAPIRestServiceProvider
{
    [GeneratorTemplate("Class", @"Generators\Server\WebAPIRestServiceProvider\WebAPIRestServiceProviderClassTemplate.tt")]
    public static class WebAPIRestServiceProviderGenerator
    {
        public static void GenerateProvider(IBase baseObject, string providerFolderPath, string title, string providerName, string configPathString, NamingConvention namingConvention, IGeneratorConfiguration generatorConfiguration, List<ServiceMethod> serviceMethods)
        {
            var host = generatorConfiguration.GetTemplateEngineHost();  
            var container = (RestEntityContainer)baseObject;
            var rootObject = (object)container.JsonRootObject;
            var expressionHandler = generatorConfiguration.GetExpressionHandler(Guid.Parse(AbstraXProviderGuids.RestService));
            Dictionary<string, object> sessionVariables;
            FileInfo fileInfo;
            string fileLocation;
            string filePath;
            string output;

            try
            {
                // WebAPI Rest provider class

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("Title", title);
                sessionVariables.Add("ProviderName", providerName);
                sessionVariables.Add("ConfigPathString", configPathString);
                sessionVariables.Add("ServiceMethods", serviceMethods);
                sessionVariables.Add("NamingConvention", namingConvention);
                sessionVariables.Add("RootNamespace", generatorConfiguration.AppName);

                if (baseObject is IEntityWithPrefix)
                {
                    fileLocation = PathCombine(providerFolderPath, baseObject.CastTo<IEntityWithPrefix>().PathPrefix);
                }
                else
                {
                    fileLocation = providerFolderPath;
                }

                filePath = PathCombine(fileLocation, providerName + ".cs");
                fileInfo = new FileInfo(filePath);

                host.SetGenerator(typeof(WebAPIRestServiceProviderGenerator), "Class");
                output = host.Generate<WebAPIRestServiceProviderClassTemplate>(sessionVariables, false);

                generatorConfiguration.CreateFile(fileInfo, output, FileKind.Services, () => generatorConfiguration.GenerateInfo(sessionVariables, "WebAPIRestProvider Class"));
            }
            catch (Exception e)
            {
                generatorConfiguration.AppGeneratorEngine.WriteError(e.ToString());
            }
        }
    }
}
