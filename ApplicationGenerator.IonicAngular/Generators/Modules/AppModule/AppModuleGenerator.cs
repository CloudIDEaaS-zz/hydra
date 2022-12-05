using AbstraX.Angular;
using AbstraX.Angular.Routes;
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
    [GeneratorTemplate("Module", @"Generators\Modules\AppModule\AppModuleClassTemplate.tt")]
    [GeneratorTemplate("Blank", @"Generators\Modules\AppModule\AppModuleClassTemplate.tt")]
    [GeneratorTemplate("Routing", @"Generators\Modules\RoutingModule\RoutingModuleClassTemplate.tt")]
    public static class AppModuleGenerator
    {
        public static void GenerateModule(IBase baseObject, AngularModule angularModule, string moduleFolderPath, string moduleName, IGeneratorConfiguration generatorConfiguration, IEnumerable<ModuleImportDeclaration> imports)
        {
            var host = generatorConfiguration.GetTemplateEngineHost();
            Dictionary<string, object> sessionVariables;
            FileInfo fileInfo;
            string fileLocation;
            string filePath;
            string output;
            List<ModuleImportDeclaration> routingImports = null;

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

                do
                {
                    if (!generatorConfiguration.NoFileCreation)
                    {
                        host.SetGenerator(typeof(AppModuleGenerator), "Module");
                        output = host.Generate<AppModuleClassTemplate>(sessionVariables, false);
                    }
                    else
                    {
                        host.SetGenerator(typeof(AppModuleGenerator), "Blank");
                        output = host.Generate<BlankAppModuleClassTemplate>(sessionVariables, false);
                    }

                    generatorConfiguration.CreateFile(fileInfo, output, FileKind.Project, angularModule);
                }
                while (host.PostProcess() == PostProcessResult.RedoGenerate);

                // Routing module class

                sessionVariables = new Dictionary<string, object>();

                angularModule.AddImportsAndRoutes(imports);

                sessionVariables.Add("ModuleName", string.Format(angularModule.RoutingNameExpression, string.Empty));
                sessionVariables.Add("IsRoot", true);
                sessionVariables.Add("Routes", angularModule.Routes);

                fileLocation = moduleFolderPath;

                filePath = PathCombine(fileLocation, "app-routing.module.ts");
                fileInfo = new FileInfo(filePath);
                routingImports = new List<ModuleImportDeclaration>();

                foreach (var canLoadComponent in angularModule.Routes.SelectMany(r => r.CanLoad))
                {
                    var fileRelative = canLoadComponent.File.RelativeFullName;
                    var baseRelative = angularModule.File.Folder.RelativeFullName;
                    var importPath = "." + fileRelative.RemoveStart(baseRelative).RemoveEndIfMatches(".ts");
                    var importDeclaration = new ModuleImportDeclaration(importPath, canLoadComponent.Name);

                    routingImports.Add(importDeclaration);
                }

                sessionVariables.Add("Imports", routingImports);

                do
                {
                    host.SetGenerator(typeof(AppModuleGenerator), "Routing");
                    output = host.Generate<RoutingModuleClassTemplate>(sessionVariables, false);

                    generatorConfiguration.CreateFile(fileInfo, output, FileKind.Project);
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
