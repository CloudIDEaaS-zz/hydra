using AbstraX.Angular;
using AbstraX.Generators.Pages.SassPage;
using AbstraX.DataAnnotations;
using $rootnamespace$;
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

namespace $rootnamespace$
{
    [GeneratorTemplate("Page", @"Generators\Pages\$basename$Page\$basename$PageTemplate.tt")]
    [GeneratorTemplate("Class", @"Generators\Pages\$basename$Page\$basename$ClassTemplate.tt")]
    [GeneratorTemplate("Sass", @"Generators\Pages\$basename$Page\SassStyleSheetTemplate.tt")]
    public static class $basename$PageGenerator
    {
        public static void GeneratePage(IBase baseObject, string pagesFolderPath, string pageName, IGeneratorConfiguration generatorConfiguration, IModuleAssembly module, IEnumerable<ModuleImportDeclaration> imports, List<object> inputObjects)
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
                // $basename$ page

                sessionVariables = new Dictionary<string, object>();

                // TODO - change this and the input variable inputObjects, preferably typed, to match your needs
                sessionVariables.Add("Input", inputObjects);

                fileLocation = PathCombine(pagesFolderPath, pageName.ToLower());
                filePath = PathCombine(fileLocation, pageName.ToLower() + ".html");
                fileInfo = new FileInfo(filePath);

                host.SetGenerator(typeof($basename$PageGenerator), "Page");

                do
                {
                    output = host.Generate<$basename$PageTemplate>(sessionVariables, false);

                    generatorConfiguration.CreateFile(fileInfo, output, FileKind.Project, () => generatorConfiguration.GenerateInfo(sessionVariables, "$basename$ Page"));
                }
                while (host.PostProcess() == PostProcessResult.RedoGenerate);

                // $basename$ class

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("Input", inputObjects);
                sessionVariables.Add("PageName", pageName);
                sessionVariables.Add("UIKind", UIKind.$basename$Page);
                sessionVariables.Add("UILoadKind", UILoadKind.Default);
                sessionVariables.Add("Authorize", generatorConfiguration.AuthorizedRoles);
                sessionVariables.AddModuleAssemblyProperties(moduleAssemblyProperties);

                fileLocation = PathCombine(pagesFolderPath, pageName.ToLower());
                filePath = PathCombine(fileLocation, pageName.ToLower() + ".ts");
                fileInfo = new FileInfo(filePath);

                host.SetGenerator(typeof($basename$PageGenerator), "Class");

                do
                {
                    output = host.Generate<$basename$ClassTemplate> (sessionVariables, false);

                    module.ExportedComponents = sessionVariables.GetExportedComponents();
                    module.ForChildFile = generatorConfiguration.CreateFile(moduleAssemblyProperties, fileInfo, output, FileKind.Project, () => generatorConfiguration.GenerateInfo(sessionVariables, "$basename$ Page Class"));
                }
                while (host.PostProcess() == PostProcessResult.RedoGenerate);

                // sass

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("PageName", pageName);

                fileLocation = PathCombine(pagesFolderPath, pageName.ToLower());
                filePath = PathCombine(fileLocation, pageName.ToLower() + ".scss");
                fileInfo = new FileInfo(filePath);

                host.SetGenerator(typeof($basename$PageGenerator), "Sass");

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
