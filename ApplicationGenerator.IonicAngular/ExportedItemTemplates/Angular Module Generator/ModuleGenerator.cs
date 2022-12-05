using AbstraX.Angular;
using AbstraX.Generators.ClientModel;
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
    public static class $basename$ModuleGenerator
    {
        public static void GenerateModule(IBase baseObject, AngularModule angularModule, string moduleFolderPath, string moduleName, IGeneratorConfiguration generatorConfiguration, IEnumerable<ModuleImportDeclaration> imports)
        {
            var host = generatorConfiguration.GetTemplateEngineHost();
            Dictionary<string, object> sessionVariables;
            FileInfo fileInfo;
            string fileLocation;
            string filePath;
            string output;

            try
            {
                // Module class

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("ModuleName", angularModule.Name);
                sessionVariables.Add("AngularModule", angularModule);
                sessionVariables.Add("Imports", imports);

                fileLocation = moduleFolderPath;

                // TODO - change file name if needed
                filePath = PathCombine(fileLocation, moduleName.ToLower() + ".module.ts");
                fileInfo = new FileInfo(filePath);

                output = host.Generate<$basename$ModuleClassTemplate > (sessionVariables, false);

                sessionVariables.Add("Output", output);

                generatorConfiguration.CreateFile(fileInfo, output, FileKind.Project, angularModule);
            }
            catch (Exception e)
            {
                generatorConfiguration.Engine.WriteError(e.ToString());
            }
        }
    }
}
