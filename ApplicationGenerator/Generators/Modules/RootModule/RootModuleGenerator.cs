using AbstraX.Angular;
using AbstraX.Generators.Client.ClientModel;
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

namespace AbstraX.Generators.Modules.RootModule
{
    public static class RootModuleGenerator
    {
        public static void GenerateModule(IBase baseObject, AngularModule angularModule, string moduleFolderPath, string moduleName, IGeneratorConfiguration generatorConfiguration, IEnumerable<ModuleImportDeclaration> imports)
        {
            var host = new TemplateEngineHost();
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

                filePath = PathCombine(fileLocation, moduleName.ToLower() + ".module.ts");
                fileInfo = new FileInfo(filePath);

                output = host.Generate<RootModuleClassTemplate>(sessionVariables, false);

                sessionVariables.Add("Output", output);

                generatorConfiguration.CreateFile(fileInfo, output, FileKind.Project, angularModule);
            }
            catch (Exception e)
            {
                generatorConfiguration.AppGeneratorEngine.WriteError(e.ToString());
            }
        }
    }
}
