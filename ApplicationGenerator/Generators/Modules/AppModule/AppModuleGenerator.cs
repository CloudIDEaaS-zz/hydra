using AbstraX.Angular;
using AbstraX.Generators.Client.ClientModel;
using AbstraX.Generators.Modules.RoutingModule;
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

namespace AbstraX.Generators.Modules.AppModule
{
    public static class AppModuleGenerator
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
                sessionVariables.Add("AppName", generatorConfiguration.AppName);
                sessionVariables.Add("Imports", imports);

                fileLocation = moduleFolderPath;

                filePath = PathCombine(fileLocation, "app.module.ts");
                fileInfo = new FileInfo(filePath);

                if (!generatorConfiguration.NoFileCreation)
                {
                    output = host.Generate<AppModuleClassTemplate>(sessionVariables, false);
                }
                else
                {
                    output = host.Generate<BlankAppModuleClassTemplate>(sessionVariables, false);
                }

                generatorConfiguration.CreateFile(fileInfo, output, FileKind.Project, angularModule);

                // Routing module class

                sessionVariables = new Dictionary<string, object>();

                angularModule.AddImportsAndRoutes(imports);

                sessionVariables.Add("ModuleName", angularModule.RoutingName);
                sessionVariables.Add("IsRoot", angularModule.UILoadKind == DataAnnotations.UILoadKind.RootPage);
                sessionVariables.Add("Routes", angularModule.Routes);

                fileLocation = moduleFolderPath;

                filePath = PathCombine(fileLocation, "app-routing.module.ts");
                fileInfo = new FileInfo(filePath);

                output = host.Generate<RoutingModuleClassTemplate>(sessionVariables, false);

                generatorConfiguration.CreateFile(fileInfo, output, FileKind.Project, angularModule);


            }
            catch (Exception e)
            {
                generatorConfiguration.AppGeneratorEngine.WriteError(e.ToString());
            }
        }
    }
}
