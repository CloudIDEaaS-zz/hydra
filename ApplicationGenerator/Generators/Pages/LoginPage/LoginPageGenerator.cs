using AbstraX.Angular;
using AbstraX.Generators.Client.Validator;
using AbstraX.Generators.Pages.LoginPage;
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

namespace AbstraX.Generators.Pages.LoginPage
{
    public static class LoginPageGenerator
    {
        public static void GeneratePage(IBase baseObject, string pagesFolderPath, string pageName, IGeneratorConfiguration generatorConfiguration, IModuleAssembly module, IDictionary<string, IEnumerable<ModuleImportDeclaration>> importGroups, List<IdentityField> identityFields, string loginTitleTranslationKey, string loginButtonTranslationKey)
        {
            var host = new TemplateEngineHost();
            var pass = generatorConfiguration.CurrentPass;
            var moduleAssemblyProperties = new AngularModuleAssemblyProperties(baseObject);
            var resourcesHandler = generatorConfiguration.ResourcesHandler;
            IEnumerable<ModuleImportDeclaration> importDeclarations;
            Dictionary<string, object> sessionVariables;
            FileInfo fileInfo;
            string fileLocation;
            string filePath;
            string output;

            try
            {
                // Login page

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("IdentityFields", identityFields);
                sessionVariables.Add("PageName", pageName);
                sessionVariables.Add("EntityName", baseObject.Name);
                sessionVariables.Add("LoginTitleTranslationKey", loginTitleTranslationKey);
                sessionVariables.Add("LoginButtonTranslationKey", loginButtonTranslationKey);

                fileLocation = PathCombine(pagesFolderPath, pageName.ToLower());
                filePath = PathCombine(fileLocation, pageName.ToLower() + ".html");
                fileInfo = new FileInfo(filePath);

                output = host.Generate<LoginPageTemplate>(sessionVariables, false);

                generatorConfiguration.CreateFile(fileInfo, output, FileKind.Project, () => generatorConfiguration.GenerateInfo(sessionVariables, "Login Page"));

                // Login class

                sessionVariables = new Dictionary<string, object>();
                importDeclarations = importGroups["Page"];

                moduleAssemblyProperties.Imports = importDeclarations.Exclude(pageName + "Page");

                sessionVariables.Add("IdentityFields", identityFields);
                sessionVariables.Add("PageName", pageName);
                sessionVariables.Add("Authorize", generatorConfiguration.AuthorizedRoles);
                sessionVariables.Add("EntityName", baseObject.Name);
                sessionVariables.AddModuleAssemblyProperties(moduleAssemblyProperties);

                filePath = PathCombine(fileLocation, pageName.ToLower() + ".ts");
                fileInfo = new FileInfo(filePath);

                output = host.Generate<LoginClassTemplate>(sessionVariables, false);

                module.ExportedComponents = sessionVariables.GetExportedComponents();
                module.ForChildFile = generatorConfiguration.CreateFile(moduleAssemblyProperties, fileInfo, output, FileKind.Project, () => generatorConfiguration.GenerateInfo(sessionVariables, "Login Page Class"));

                // Validator class

                sessionVariables = new Dictionary<string, object>();
                importDeclarations = importGroups["Validator"];

                moduleAssemblyProperties.Clear();
                moduleAssemblyProperties.Imports = importDeclarations.Exclude(pageName + "Validator");

                sessionVariables.Add("FormFields", identityFields.Cast<FormField>().ToList());
                sessionVariables.Add("PageName", pageName);
                sessionVariables.Add("Authorize", generatorConfiguration.AuthorizedRoles);
                sessionVariables.Add("EntityName", baseObject.Name);
                sessionVariables.AddModuleAssemblyProperties(moduleAssemblyProperties);

                filePath = PathCombine(fileLocation, pageName.ToLower() + "-validator.ts");
                fileInfo = new FileInfo(filePath);

                output = host.Generate<ValidatorClassTemplate>(sessionVariables, false);

                generatorConfiguration.CreateFile(moduleAssemblyProperties, fileInfo, output, FileKind.Project, () => generatorConfiguration.GenerateInfo(sessionVariables, "Login Validator Class"));

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
