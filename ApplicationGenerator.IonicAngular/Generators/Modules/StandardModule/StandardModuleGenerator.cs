using AbstraX.Angular;
using AbstraX.DataAnnotations;
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

namespace AbstraX.Generators.Modules.StandardModule
{
    [GeneratorTemplate("Class", @"Generators\Modules\StandardModule\StandardModuleClassTemplate.tt")]
    [GeneratorTemplate("Routing", @"Generators\Modules\RoutingModule\RoutingModuleClassTemplate.tt")]
    public static class StandardModuleGenerator
    {
        public static void GenerateModule(IBase baseObject, AngularModule angularModule, string moduleFolderPath, string moduleName, IGeneratorConfiguration generatorConfiguration, IEnumerable<ModuleImportDeclaration> imports, UILoadKind loadKind, UIKind uiKind)
        {
            var host = generatorConfiguration.GetTemplateEngineHost();
            Dictionary<string, object> sessionVariables;
            FileInfo fileInfo;
            string fileLocation;
            string filePath;
            FileInfo routingFileInfo;
            string routingFileLocation;
            string routingFilePath;
            string output;
            string routingModuleName;
            List<ModuleImportDeclaration> routingImports = null;
            Angular.RoutingModule routingModule;
            ModuleImportDeclaration moduleImportDeclaration;
            Page page;

            try
            {
                // Module class

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("ModuleName", angularModule.Name);
                sessionVariables.Add("AngularModule", angularModule);
                sessionVariables.Add("UILoadKind", loadKind);
                sessionVariables.Add("UIKind", uiKind);

                fileLocation = moduleFolderPath;

                filePath = PathCombine(fileLocation, moduleName.ToLower() + ".module.ts");
                fileInfo = new FileInfo(filePath);

                routingFileLocation = moduleFolderPath;

                routingFilePath = PathCombine(routingFileLocation, moduleName.ToLower() + "-routing.module.ts");
                routingFileInfo = new FileInfo(routingFilePath);
                routingImports = new List<ModuleImportDeclaration>();

                routingModuleName = string.Format(angularModule.RoutingNameExpression, string.Empty);

                routingModule = new Angular.RoutingModule(routingModuleName);
                moduleImportDeclaration = new ModuleImportDeclaration("./" + routingFileInfo.Name.RemoveEnd(".ts"), routingModule, routingModuleName);

                imports = imports.Concat(new[] { moduleImportDeclaration });

                angularModule.AddImportsAndRoutes(imports);

                sessionVariables.Add("Imports", imports);

                host.SetGenerator(typeof(StandardModuleGenerator), "Class");

                do
                {
                    output = host.Generate<StandardModuleClassTemplate>(sessionVariables, false);

                    generatorConfiguration.CreateFile(fileInfo, output, FileKind.Project, angularModule);
                }
                while (host.PostProcess() == PostProcessResult.RedoGenerate);

                // Routing module class

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("ModuleName", routingModuleName);
                sessionVariables.Add("IsRoot", false);
                sessionVariables.Add("Routes", angularModule.Routes);

                foreach (var canLoadComponent in angularModule.Routes.SelectMany(r => r.CanLoad))
                {
                    var fileRelative = canLoadComponent.File.RelativeFullName;
                    var baseRelative = angularModule.File.Folder.RelativeFullName;
                    var importPath = "." + fileRelative.RemoveStart(baseRelative).RemoveEndIfMatches(".ts");
                    var importDeclaration = new ModuleImportDeclaration(importPath, canLoadComponent.Name);

                    routingImports.Add(importDeclaration);
                }

                page = angularModule.Declarations.OfType<Page>().Single();

                moduleImportDeclaration = new ModuleImportDeclaration("./" + page.File.Name.RemoveEnd(".ts"), page.Name);

                routingImports.Add(moduleImportDeclaration);

                sessionVariables.Add("Imports", routingImports);

                do
                {
                    host.SetGenerator(typeof(StandardModuleGenerator), "Routing");
                    output = host.Generate<RoutingModuleClassTemplate>(sessionVariables, false);

                    generatorConfiguration.CreateFile(routingFileInfo, output, FileKind.Project);
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
