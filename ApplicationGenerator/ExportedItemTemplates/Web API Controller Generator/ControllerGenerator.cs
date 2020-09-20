using AbstraX.Angular;
using $rootnamespace$;
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

namespace $rootnamespace$
{
    public static class $safeitemname$PageGenerator
    {
        public static void GenerateController(IBase baseObject, string controllersFolderPath, string controllerName, IGeneratorConfiguration generatorConfiguration, List<RelatedEntity> relatedEntities)
        {
            var host = new TemplateEngineHost();
            var pass = generatorConfiguration.CurrentPass;
            var exports = new List<ESModule>();
            var declarations = new List<IDeclarable>();
            Dictionary<string, object> sessionVariables;
            FolderStructure.File file;
            string fileLocation;
            string filePath;
            string output;

            try
            {
                // $safeitemname$ controller

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("ControllerName", controllerName);
                sessionVariables.Add("EntityName", baseObject.Name);
                sessionVariables.Add("RelatedEntities", relatedEntities);

                filePath = PathCombine(controllersFolderPath, controllerName + ".cs");

                output = host.Generate<$safeitemname$ControllerTemplate>(sessionVariables, false);

                if (pass == GeneratorPass.Files)
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }

                    if (!Directory.Exists(fileLocation))
                    {
                        Directory.CreateDirectory(fileLocation);
                    }

                    using (FileStream fileStream = File.Create(filePath))
                    {
                        fileStream.Write(output);
                        generatorConfiguration.FileSystem.DeleteFile(filePath);

                        generatorConfiguration.FileSystem.AddSystemLocalFile(new FileInfo(filePath));
                    }
                }
                else if (pass == GeneratorPass.HierarchyOnly)
                {
                    generatorConfiguration.FileSystem.AddSystemLocalFile(new FileInfo(filePath), generatorConfiguration.GenerateInfo(sessionVariables, "$safeitemname$ Page"));
                }
            }
            catch (Exception e)
            {
                generatorConfiguration.Engine.WriteError(e.ToString());
            }
        }
    }
}
