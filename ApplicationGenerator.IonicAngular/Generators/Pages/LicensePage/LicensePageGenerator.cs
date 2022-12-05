using AbstraX.Angular;
using AbstraX.DataAnnotations;
using AbstraX.Generators.Pages.LicensePage;
using AbstraX.Generators.Pages.SassPage;
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

namespace AbstraX.Generators.Pages.LicensePage
{
    [GeneratorTemplate("Page", @"Generators\Pages\LicensePage\LicensePageTemplate.tt")]
    [GeneratorTemplate("Class", @"Generators\Pages\LicensePage\LicenseClassTemplate.tt")]
    [GeneratorTemplate("Sass", @"Generators\Pages\LicensePage\SassStyleSheetTemplate.tt")]
    public static class LicensePageGenerator
    {
        public static void GeneratePage(IBase baseObject, string pagesFolderPath, string pageName, IGeneratorConfiguration generatorConfiguration, IModuleAssembly module, IEnumerable<ModuleImportDeclaration> imports)
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
                // License page

                sessionVariables = new Dictionary<string, object>();

                fileLocation = PathCombine(pagesFolderPath, pageName.ToLower());
                filePath = PathCombine(fileLocation, pageName.ToLower() + ".html");
                fileInfo = new FileInfo(filePath);

                host.SetGenerator(typeof(LicensePageGenerator), "Page");

                do
                {
                    output = host.Generate<LicensePageTemplate>(sessionVariables, false);

                    generatorConfiguration.CreateFile(fileInfo, output, FileKind.Project, () => generatorConfiguration.GenerateInfo(sessionVariables, "License Page"));
                }
                while (host.PostProcess() == PostProcessResult.RedoGenerate);

                // License class

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("PageName", pageName);
                sessionVariables.Add("UIKind", UIKind.LicensePage);
                sessionVariables.Add("UILoadKind", UILoadKind.Default);
                sessionVariables.Add("Authorize", generatorConfiguration.AuthorizedRoles);
                sessionVariables.AddModuleAssemblyProperties(moduleAssemblyProperties);

                fileLocation = PathCombine(pagesFolderPath, pageName.ToLower());
                filePath = PathCombine(fileLocation, pageName.ToLower() + ".ts");
                fileInfo = new FileInfo(filePath);

                host.SetGenerator(typeof(LicensePageGenerator), "Class");

                do
                {
                    output = host.Generate<LicenseClassTemplate>(sessionVariables, false);

                    module.ExportedComponents = sessionVariables.GetExportedComponents();
                    module.ForChildFile = generatorConfiguration.CreateFile(moduleAssemblyProperties, fileInfo, output, FileKind.Project, () => generatorConfiguration.GenerateInfo(sessionVariables, "License Page Class"));
                }
                while (host.PostProcess() == PostProcessResult.RedoGenerate);

                // sass

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("PageName", pageName);

                fileLocation = PathCombine(pagesFolderPath, pageName.ToLower());
                filePath = PathCombine(fileLocation, pageName.ToLower() + ".scss");
                fileInfo = new FileInfo(filePath);

                host.SetGenerator(typeof(LicensePageGenerator), "Sass");

                do
                {
                    output = host.Generate<SassStyleSheetTemplate>(sessionVariables, false);

                    generatorConfiguration.CreateFile(fileInfo, output, FileKind.Project, () => generatorConfiguration.GenerateInfo(sessionVariables, "page \r\n{\r\n}"));
                }
                while (host.PostProcess() == PostProcessResult.RedoGenerate);
            }
            catch (Exception ex)
            {
                generatorConfiguration.AppGeneratorEngine.WriteError(ex.ToString());
            }
        }
    }
}
