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

namespace AbstraX.Generators.Modules.NavigationGridModule
{
    [GeneratorTemplate("Class", @"Generators\Modules\NavigationGridModule\NavigationGridModuleClassTemplate.tt")]
    public static class NavigationGridModuleGenerator
    {
        public static void GenerateModule(IBase baseObject, AngularModule angularModule, string moduleFolderPath, string moduleName, IGeneratorConfiguration generatorConfiguration, IEnumerable<ModuleImportDeclaration> imports, UILoadKind loadKind, UIKind uiKind)
        {
            var host = generatorConfiguration.GetTemplateEngineHost();
            Dictionary<string, object> sessionVariables;
            FileInfo fileInfo;
            string fileLocation;
            string filePath;
            string output;

            try
            {
                // Client Model class

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("ModuleName", angularModule.Name);
                sessionVariables.Add("AngularModule", angularModule);
                sessionVariables.Add("Imports", imports);
                sessionVariables.Add("IsComponent", angularModule.IsComponent);
                sessionVariables.Add("UILoadKind", loadKind);
                sessionVariables.Add("UIKind", uiKind);

                fileLocation = moduleFolderPath;
                filePath = PathCombine(fileLocation, moduleName.ToLower() + ".module.ts");
                fileInfo = new FileInfo(filePath);

                host.SetGenerator(typeof(NavigationGridModuleGenerator), "Class");

                do
                {
                    output = host.Generate<NavigationGridModuleClassTemplate>(sessionVariables, false);

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
