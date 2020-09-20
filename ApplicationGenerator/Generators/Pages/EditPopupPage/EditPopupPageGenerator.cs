using AbstraX.Angular;
using AbstraX.Generators.Pages.EditPopupPage;
using AbstraX.Generators.Client.Validator;
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
using AbstraX.Generators.Pages.SassPage;

namespace AbstraX.Generators.Pages.EditPopupPage
{
    public static class EditPopupPageGenerator
    {
        public static void GeneratePage(IBase baseObject, string pagesFolderPath, string pageName, IGeneratorConfiguration generatorConfiguration, IModuleAssembly module, IDictionary<string, IEnumerable<ModuleImportDeclaration>> importGroups, List<FormField> formFields)
        {
            var host = new TemplateEngineHost();
            var pass = generatorConfiguration.CurrentPass;
            var moduleAssemblyProperties = new AngularModuleAssemblyProperties(baseObject);
            Dictionary<string, object> sessionVariables;
            IEnumerable<ModuleImportDeclaration> importDeclarations;
            FileInfo fileInfo;
            string fileLocation;
            string filePath;
            string output;

            try
            {
                // EditPopup page

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("FormFields", formFields);
                sessionVariables.Add("PageName", pageName);
                sessionVariables.Add("EntityName", baseObject.Name);

                fileLocation = PathCombine(pagesFolderPath, "edit-" + pageName.ToLower());
                filePath = PathCombine(fileLocation, "edit-" + pageName.ToLower() + ".html");
                fileInfo = new FileInfo(filePath);

                output = host.Generate<EditPopupPageTemplate>(sessionVariables, false);

                generatorConfiguration.CreateFile(fileInfo, output, FileKind.Project, () => generatorConfiguration.GenerateInfo(sessionVariables, "EditPopup Page"));

                // EditPopup class

                sessionVariables = new Dictionary<string, object>();
                importDeclarations = importGroups["Page"];

                moduleAssemblyProperties.Imports = importDeclarations.Exclude("Edit" + pageName + "Page");

                sessionVariables.Add("FormFields", formFields);
                sessionVariables.Add("PageName", pageName);
                sessionVariables.Add("Authorize", generatorConfiguration.AuthorizedRoles);
                sessionVariables.Add("EntityName", baseObject.Name);
                sessionVariables.AddModuleAssemblyProperties(moduleAssemblyProperties);

                filePath = PathCombine(fileLocation, "edit-" + pageName.ToLower() + ".ts");
                fileInfo = new FileInfo(filePath);

                output = host.Generate<EditPopupClassTemplate>(sessionVariables, false);

                module.ExportedComponents = sessionVariables.GetExportedComponents();
                module.ForChildFile = generatorConfiguration.CreateFile(moduleAssemblyProperties, fileInfo, output, FileKind.Project, () => generatorConfiguration.GenerateInfo(sessionVariables, "EditPopup Page Class"));

                // Validator class

                sessionVariables = new Dictionary<string, object>();
                importDeclarations = importGroups["Validator"];

                moduleAssemblyProperties.Clear();
                moduleAssemblyProperties.Imports = importDeclarations.Exclude(pageName + "Validator");

                sessionVariables.Add("FormFields", formFields);
                sessionVariables.Add("PageName", pageName);
                sessionVariables.Add("Authorize", generatorConfiguration.AuthorizedRoles);
                sessionVariables.Add("EntityName", baseObject.Name);
                sessionVariables.AddModuleAssemblyProperties(moduleAssemblyProperties);

                filePath = PathCombine(fileLocation, pageName.ToLower() + "-validator.ts");
                fileInfo = new FileInfo(filePath);

                output = host.Generate<ValidatorClassTemplate>(sessionVariables, false);

                generatorConfiguration.CreateFile(moduleAssemblyProperties, fileInfo, output, FileKind.Project, () => generatorConfiguration.GenerateInfo(sessionVariables, "EditPopup Validator Class"));

                // sass

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("PageName", pageName);

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
