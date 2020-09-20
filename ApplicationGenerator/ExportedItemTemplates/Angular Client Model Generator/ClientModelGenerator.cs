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
    public static class $basename$ClientModelGenerator
    {
        public static void GenerateModel(IBase baseObject, string modelsFolderPath, string modelName, IGeneratorConfiguration generatorConfiguration, IEnumerable<ModuleImportDeclaration> imports, AngularModule angularModule, List<object> inputObjects)
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
                // $basename$ class

                sessionVariables = new Dictionary<string, object>();

                // TODO - change to match above
                sessionVariables.Add("Input", inputObjects);
                sessionVariables.Add("ModelName", modelName);
                sessionVariables.Add("EntityName", baseObject.Name);

                fileLocation = PathCombine(modelsFolderPath, modelName);
                filePath = PathCombine(fileLocation, modelName + ".ts");

                output = host.Generate<$basename$ClassTemplate> (sessionVariables, false);

                foreach (var export in exports)
                {
                    angularModule.AddExport(baseObject, export);
                }

                angularModule.Declarations.AddRange(declarations);

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

                        file = generatorConfiguration.FileSystem.AddSystemLocalFile(new FileInfo(filePath));

                        file.Folder.AddAssembly(angularModule);
                    }
                }
                else if (pass == GeneratorPass.HierarchyOnly)
                {
                    file = generatorConfiguration.FileSystem.AddSystemLocalFile(new FileInfo(filePath), generatorConfiguration.GenerateInfo(sessionVariables, "$basename$ Model Class"));

                    file.Folder.AddAssembly(angularModule);
                }
            }
            catch (Exception e)
            {
                generatorConfiguration.Engine.WriteError(e.ToString());
            }
        }
    }
}
