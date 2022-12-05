using AbstraX.Angular;
using AbstraX.DataAnnotations;
using AbstraX.Generators.Pages.SassPage;
using AbstraX.Generators.Pages.SlidingTabPage;
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

namespace AbstraX.Generators.Pages.SlidingTabPage
{
    [GeneratorTemplate("Page", @"Generators\Pages\SlidingTabPage\SlidingTabPageTemplate.tt")]
    [GeneratorTemplate("Class", @"Generators\Pages\SlidingTabPage\SlidingTabClassTemplate.tt")]
    [GeneratorTemplate("Sass", @"Generators\Pages\SassPage\SassStyleSheetTemplate.tt")]
    public static class SlidingTabsPageGenerator
    {
        public static void GeneratePage(IBase baseObject, string pagesFolderPath, string pageName, IGeneratorConfiguration generatorConfiguration, IModuleAssembly module, IEnumerable<ModuleImportDeclaration> imports, List<SlidingTab> slidingTabs, bool isComponent, UILoadKind loadKind, UIKind uiKind)
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
                // tab page

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("SlidingTabs", slidingTabs);
                sessionVariables.Add("IsComponent", isComponent);

                fileLocation = PathCombine(pagesFolderPath, pageName.ToLower());
                filePath = PathCombine(fileLocation, pageName.ToLower() + ".html");
                fileInfo = new FileInfo(filePath);

                host.SetGenerator(typeof(SlidingTabsPageGenerator), "Page");

                do
                {
                    output = host.Generate<SlidingTabPageTemplate>(sessionVariables, false);

                    generatorConfiguration.CreateFile(fileInfo, output, FileKind.Project, () => generatorConfiguration.GenerateInfo(sessionVariables, "Sliding Tab Page"));
                }
                while (host.PostProcess() == PostProcessResult.RedoGenerate);

                // tab class

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("SlidingTabs", slidingTabs);
                sessionVariables.Add("PageName", pageName);
                sessionVariables.Add("Authorize", generatorConfiguration.AuthorizedRoles);
                sessionVariables.Add("IsComponent", isComponent);
                sessionVariables.Add("UILoadKind", loadKind);
                sessionVariables.Add("UIKind", uiKind);

                sessionVariables.AddModuleAssemblyProperties(moduleAssemblyProperties);

                fileLocation = PathCombine(pagesFolderPath, pageName.ToLower());
                filePath = PathCombine(fileLocation, pageName.ToLower() + ".ts");
                fileInfo = new FileInfo(filePath);

                host.SetGenerator(typeof(SlidingTabsPageGenerator), "Class");

                do
                {
                    output = host.Generate<SlidingTabClassTemplate>(sessionVariables, false);

                    module.ExportedComponents = sessionVariables.GetExportedComponents();
                    module.ForChildFile = generatorConfiguration.CreateFile(moduleAssemblyProperties, fileInfo, output, FileKind.Project, () => generatorConfiguration.GenerateInfo(sessionVariables, "Sliding Tab Page Class"));
                }
                while (host.PostProcess() == PostProcessResult.RedoGenerate);

                // sass

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("PageName", pageName);

                fileLocation = PathCombine(pagesFolderPath, pageName.ToLower());
                filePath = PathCombine(fileLocation, pageName.ToLower() + ".scss");
                fileInfo = new FileInfo(filePath);

                do
                {
                    host.SetGenerator(typeof(SlidingTabsPageGenerator), "Sass");
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
