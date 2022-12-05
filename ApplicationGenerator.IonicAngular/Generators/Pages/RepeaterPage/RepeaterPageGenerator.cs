using AbstraX.Angular;
using AbstraX.DataAnnotations;
using AbstraX.Generators.Pages.SassPage;
using AbstraX.Generators.Pages.TabPage;
using AbstraX.ServerInterfaces;
using AbstraX.ViewEngine;
using EntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using static AbstraX.FolderStructure.FileSystemObject;

namespace AbstraX.Generators.Pages.RepeaterPage
{
    [GeneratorTemplate("Page", @"Generators\Pages\RepeaterPage\RepeaterPageTemplate.tt")]
    [GeneratorTemplate("Class", @"Generators\Pages\RepeaterPage\RepeaterClassTemplate.tt")]
    [GeneratorTemplate("Sass", @"Generators\Pages\SassPage\SassStyleSheetTemplate.tt")]
    public static class RepeaterPageGenerator
    {
        public static void GeneratePage(IBase baseObject, string pagesFolderPath, string pageName, IGeneratorConfiguration generatorConfiguration, IModuleAssembly module, IEnumerable<ModuleImportDeclaration> imports, View view, RazorSemanticVisitor visitor, UILoadKind loadKind, UIKind uiKind)
        {
            var host = generatorConfiguration.GetTemplateEngineHost();
            var pass = generatorConfiguration.CurrentPass;
            var moduleAssemblyProperties = new AngularModuleAssemblyProperties(baseObject, imports);
            Dictionary<string, object> sessionVariables;
            FileInfo fileInfo;
            string fileLocation;
            string filePath;
            string output;

            try
            {
                // repeater page

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("RazorSemanticVisitor", visitor);
                sessionVariables.Add("View", view);

                fileLocation = PathCombine(pagesFolderPath, pageName.ToLower());
                filePath = PathCombine(fileLocation, pageName.ToLower() + ".html");
                fileInfo = new FileInfo(filePath);

                host.SetGenerator(typeof(RepeaterPageGenerator), "Page");

                do
                {
                    output = host.Generate<RepeaterPageTemplate>(sessionVariables, false);

                    generatorConfiguration.CreateFile(fileInfo, output, FileKind.Project, () => generatorConfiguration.GenerateInfo(sessionVariables, "Repeater Page"));
                }
                while (host.PostProcess() == PostProcessResult.RedoGenerate);

                // repeater class

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("RazorSemanticVisitor", visitor);
                sessionVariables.Add("PageName", pageName);
                sessionVariables.Add("Authorize", generatorConfiguration.AuthorizedRoles);
                sessionVariables.Add("UILoadKind", loadKind);
                sessionVariables.Add("UIKind", uiKind);

                sessionVariables.AddModuleAssemblyProperties(moduleAssemblyProperties);

                fileLocation = PathCombine(pagesFolderPath, pageName.ToLower());
                filePath = PathCombine(fileLocation, pageName.ToLower() + ".ts");
                fileInfo = new FileInfo(filePath);

                do
                {
                    host.SetGenerator(typeof(RepeaterPageGenerator), "Class");
                    output = host.Generate<RepeaterClassTemplate>(sessionVariables, false);

                    module.ExportedComponents = sessionVariables.GetExportedComponents();
                    module.ForChildFile = generatorConfiguration.CreateFile(moduleAssemblyProperties, fileInfo, output, FileKind.Project, () => generatorConfiguration.GenerateInfo(sessionVariables, "Repeater Page Class"));
                }
                while (host.PostProcess() == PostProcessResult.RedoGenerate);

                // sass

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("PageName", pageName);

                fileLocation = PathCombine(pagesFolderPath, pageName.ToLower());
                filePath = PathCombine(fileLocation, pageName.ToLower() + ".scss");
                fileInfo = new FileInfo(filePath);

                host.SetGenerator(typeof(RepeaterPageGenerator), "Sass");

                do
                {
                    output = host.Generate<SassStyleSheetTemplate>(sessionVariables, false);

                    generatorConfiguration.CreateFile(fileInfo, output, FileKind.Project, () => generatorConfiguration.GenerateInfo(sessionVariables, "page \r\n{\r\n}"));
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
