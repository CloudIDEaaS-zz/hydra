using AbstraX.Angular;
using AbstraX.DataAnnotations;
using AbstraX.Generators.Pages.SassPage;
using AbstraX.Generators.Pages.TabPage;
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

namespace AbstraX.Generators.Pages.TabPage
{
    public static class TabPageGenerator
    {
        public static void GeneratePage(IBase baseObject, string pagesFolderPath, string pageName, IGeneratorConfiguration generatorConfiguration, IModuleAssembly module, IEnumerable<ModuleImportDeclaration> imports, List<Tab> tabs, UILoadKind loadKind)
        {
            var host = new TemplateEngineHost();
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

                sessionVariables.Add("PageName", pageName);
                sessionVariables.Add("Tabs", tabs);

                fileLocation = PathCombine(pagesFolderPath, pageName.ToLower());
                filePath = PathCombine(fileLocation, pageName.ToLower() + ".html"); 
                fileInfo = new FileInfo(filePath);

                output = host.Generate<TabPageTemplate>(sessionVariables, false);

                generatorConfiguration.CreateFile(fileInfo, output, FileKind.Project, () => generatorConfiguration.GenerateInfo(sessionVariables, "Tab Page"));

                // tab class

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("Tabs", tabs);
                sessionVariables.Add("PageName", pageName);
                sessionVariables.Add("Authorize", generatorConfiguration.AuthorizedRoles);
                sessionVariables.Add("UILoadKind", loadKind);
                sessionVariables.AddModuleAssemblyProperties(moduleAssemblyProperties);

                fileLocation = PathCombine(pagesFolderPath, pageName.ToLower());
                filePath = PathCombine(fileLocation, pageName.ToLower() + ".ts");
                fileInfo = new FileInfo(filePath);

                output = host.Generate<TabClassTemplate>(sessionVariables, false);

                module.ExportedComponents = sessionVariables.GetExportedComponents();
                module.ForChildFile = generatorConfiguration.CreateFile(moduleAssemblyProperties, fileInfo, output, FileKind.Project, () => generatorConfiguration.GenerateInfo(sessionVariables, "Tab Page Class"));

                // sass

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("PageName", pageName);
                 
                fileLocation = PathCombine(pagesFolderPath, pageName.ToLower());
                filePath = PathCombine(fileLocation, pageName.ToLower() + ".scss");
                fileInfo = new FileInfo(filePath);

                output = host.Generate<SassStyleSheetTemplate>(sessionVariables, false);

                generatorConfiguration.CreateFile(fileInfo, output, FileKind.Project, () => generatorConfiguration.GenerateInfo(sessionVariables, "page \r\n{\r\n}"));
            }
            catch (Exception e)
            {
                generatorConfiguration.AppGeneratorEngine.WriteError(e.ToString());
            }
        }
    }
}
