using AbstraX.Angular;
using AbstraX.DataAnnotations;
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

namespace AbstraX.Generators.Modules.EditModule
{
    [GeneratorTemplate("Class", @"Generators\Modules\EditModule\EditModuleClassTemplate.tt")]
    public static class EditModuleGenerator
    {
        public static void GenerateModule(IBase baseObject, AngularModule angularModule, string moduleFolderPath, string moduleName, IGeneratorConfiguration generatorConfiguration, IEnumerable<ModuleImportDeclaration> imports, bool isPopup, UILoadKind loadKind, UIKind uiKind)
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
                sessionVariables.Add("UILoadKind", loadKind);
                sessionVariables.Add("UIKind", uiKind);

                fileLocation = moduleFolderPath;

                if (isPopup)
                {
                    filePath = PathCombine(fileLocation, "edit-" + moduleName.ToLower() + ".module.ts");
                }
                else
                {
                    filePath = PathCombine(fileLocation, moduleName.ToLower() + ".module.ts");
                }

                fileInfo = new FileInfo(filePath);

                host.SetGenerator(typeof(EditModuleGenerator), "Class");

                do
                {
                    output = host.Generate<EditModuleClassTemplate>(sessionVariables, false);

                    sessionVariables.Add("Output", output);

                    generatorConfiguration.CreateFile(fileInfo, output, FileKind.Project, angularModule);
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
