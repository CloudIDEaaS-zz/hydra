using AbstraX.Angular;
using AbstraX.DataAnnotations;
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

namespace AbstraX.Generators.Modules.WelcomeModule
{
    [GeneratorTemplate("Class", @"Generators\Modules\WelcomeModule\WelcomeModuleClassTemplate.tt")]
    [GeneratorTemplate("Routing", @"Generators\Modules\RoutingModule\RoutingModuleClassTemplate.tt")]
    public static class WelcomeModuleGenerator
    {
        public static void GenerateModule(IBase baseObject, AngularModule angularModule, string moduleFolderPath, string moduleName, IGeneratorConfiguration generatorConfiguration, IEnumerable<ModuleImportDeclaration> imports, UILoadKind loadKind, UIKind uiKind)
        {
            var host = generatorConfiguration.GetTemplateEngineHost();
            Dictionary<string, object> sessionVariables;
            FileInfo fileInfo;
            string fileLocation;
            string filePath;
            string output;
            string pageComponentName;
            string routingModuleFile;
            string routingModuleName;
            Page page;
            List<ModuleImportDeclaration> routingImports = null;

            try
            {
                // Module class

                page = angularModule.ExportedComponents.OfType<Page>().Single();
                pageComponentName = page.Name.ToLower();
                routingModuleName = string.Format(angularModule.RoutingNameExpression, "Page");
                routingModuleFile = pageComponentName.RemoveEndIfMatches("page") + "-routing.module.ts";

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("ModuleName", angularModule.Name);
                sessionVariables.Add("RoutingModuleName", routingModuleName);
                sessionVariables.Add("AngularModule", angularModule);
                sessionVariables.Add("Imports", imports.Concat(new[] { new ModuleImportDeclaration("./" + routingModuleFile.RemoveEndIfMatches(".ts"), routingModuleName) }));
                sessionVariables.Add("UILoadKind", loadKind);
                sessionVariables.Add("UIKind", uiKind);

                fileLocation = moduleFolderPath;

                filePath = PathCombine(fileLocation, moduleName.ToLower() + ".module.ts");
                fileInfo = new FileInfo(filePath);

                host.SetGenerator(typeof(WelcomeModuleGenerator), "Class");

                do
                {
                    output = host.Generate<WelcomeModuleClassTemplate>(sessionVariables, false);

                    generatorConfiguration.CreateFile(fileInfo, output, FileKind.Project, angularModule);
                }
                while (host.PostProcess() == PostProcessResult.RedoGenerate);


                // Routing module class

                sessionVariables = new Dictionary<string, object>();

                angularModule.AddImportsAndRoutes(imports, page);
                angularModule.AddRouteGuards(generatorConfiguration, baseObject);

                sessionVariables.Add("ModuleName", routingModuleName);
                sessionVariables.Add("IsRoot", false);
                sessionVariables.Add("Routes", angularModule.Routes);

                fileLocation = moduleFolderPath;

                filePath = PathCombine(fileLocation, routingModuleFile);
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

                sessionVariables.Add("Imports", routingImports.Concat(imports));

                do
                {
                    host.SetGenerator(typeof(WelcomeModuleGenerator), "Routing");
                    output = host.Generate<RoutingModuleClassTemplate>(sessionVariables, false);

                    generatorConfiguration.CreateFile(fileInfo, output, FileKind.Project, () => generatorConfiguration.GenerateInfo(sessionVariables, "Welcome Routing Module"));
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
