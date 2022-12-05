using AbstraX.Angular;
using AbstraX.Generators.Server.WebAPIContainerController;
using AbstraX.Models.Interfaces;
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

namespace AbstraX.Generators.Server.WebAPIController
{
    [GeneratorTemplate("Class", @"Generators\Server\WebAPIContainerController\WebAPIContainerControllerClassTemplate.tt")]
    public static class WebAPIContainerControllerGenerator
    {
        public static void GenerateController(IBase baseObject, string controllersFolderPath, string controllerName, IGeneratorConfiguration generatorConfiguration)
        {
            var host = generatorConfiguration.GetTemplateEngineHost();
            var pass = generatorConfiguration.CurrentPass;
            var element = (IElement)baseObject;
            Dictionary<string, object> sessionVariables;
            FileInfo fileInfo;
            string fileLocation;
            string filePath;
            string output;

            try
            {
                // WebAPI controller class

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("ControllerName", controllerName);
                sessionVariables.Add("ContainerName", baseObject.Name);
                sessionVariables.Add("AppName", generatorConfiguration.AppName.ToLower());
                sessionVariables.Add("RootNamespace", generatorConfiguration.AppName);

                fileLocation = PathCombine(controllersFolderPath, controllerName);

                filePath = PathCombine(fileLocation, controllerName + "Controller.cs");
                fileInfo = new FileInfo(filePath);

                host.SetGenerator(typeof(WebAPIContainerControllerGenerator), "Class");

                do
                {
                    output = host.Generate<WebAPIContainerControllerClassTemplate>(sessionVariables, false);

                    if (generatorConfiguration.FileSystem.Contains(fileInfo.FullName))
                    {
                        if (pass != GeneratorPass.StructureOnly)
                        {
                            var file = (AbstraX.FolderStructure.File)generatorConfiguration.FileSystem[fileInfo.FullName];

                            if (file.Hash != output.GetHashCode())
                            {
                                // DebugUtils.Break();
                            }
                        }
                    }
                    else
                    {
                        generatorConfiguration.CreateFile(fileInfo, output, FileKind.Services, () => generatorConfiguration.GenerateInfo(sessionVariables, "WebAPIContainerController Class"));
                    }
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
