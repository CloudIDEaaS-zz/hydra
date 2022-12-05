using AbstraX.Angular;
using AbstraX.Angular.Routes;
using AbstraX.DataAnnotations;
using AbstraX.FolderStructure;
using AbstraX.Generators.Pages.TabPage;
using EntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using static AbstraX.FolderStructure.FileSystemObject;

namespace AbstraX.Generators.Client.App
{
    [GeneratorTemplate("Page", @"Generators\Client\App\AppComponentPageTemplate.tt")]
    [GeneratorTemplate("InitApp", @"Generators\Client\App\InitApp.tt")]
    [GeneratorTemplate("InitUser", @"Generators\Client\App\InitUser.tt")]
    [GeneratorTemplate("Class", @"Generators\Client\App\AppComponentClassTemplate.tt")]
    [GeneratorTemplate("PageIndex", @"Generators\Client\App\PageIndexTemplate.tt")]
    public static class AppGenerator
    {
        public static void GenerateApp(string appFolderPath, string appName, IGeneratorConfiguration generatorConfiguration, IEnumerable<ModuleImportDeclaration> imports, AngularModule appModule, IEnumerable<Page> pages)
        {
            var host = generatorConfiguration.GetTemplateEngineHost();
            var pass = generatorConfiguration.CurrentPass;
            var pagesPath = generatorConfiguration.ApplicationFolderHierarchy[IonicFileSystemType.Pages];
            var pagesFolder = (Folder)generatorConfiguration.FileSystem[pagesPath];
            var defaultRoutes = generatorConfiguration.RoleDefaults.ToDictionary(d => d.Key, d => d.Value.Where(v => v.Key == "DefaultRoute").Select(p => p.Value).Cast<Navigation>().SingleOrDefault()).ToList();
            Dictionary<string, object> sessionVariables;
            FileInfo fileInfo;
            string fileLocation;
            string filePath;
            string output;
            string initApp = null;
            string initUser = null;

            try
            {
                fileLocation = appFolderPath;
                filePath = PathCombine(fileLocation, appName.ToLower() + ".component.html");
                fileInfo = new FileInfo(filePath);

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("HasIdentityProvider", generatorConfiguration.IdentityProvider != null);

                host.SetGenerator(typeof(AppGenerator), "Page");

                do
                {
                    output = host.Generate<AppComponentPageTemplate>(sessionVariables, false);

                    generatorConfiguration.CreateFile(fileInfo, output, FileKind.Project, () => generatorConfiguration.GenerateInfo(sessionVariables, "App Component Page"));
                }
                while (host.PostProcess() == PostProcessResult.RedoGenerate);

                do
                { 
                    // todo - obfuscate

                    sessionVariables = new Dictionary<string, object>();

                    sessionVariables.Add("IdentityProvider", generatorConfiguration.IdentityProvider);

                    host.SetGenerator(typeof(AppGenerator), "InitApp");
                    initApp = host.Generate<InitApp>(sessionVariables, false);

                    sessionVariables = new Dictionary<string, object>();

                    sessionVariables.Add("IdentityProvider", generatorConfiguration.IdentityProvider);
                    sessionVariables.Add("DefaultRoutes", defaultRoutes);

                    host.SetGenerator(typeof(AppGenerator), "InitUser");
                    initUser = host.Generate<InitUser>(sessionVariables, false);

                    sessionVariables = new Dictionary<string, object>();

                    sessionVariables.Add("AppName", generatorConfiguration.AppName);
                    sessionVariables.Add("IdentityProvider", generatorConfiguration.IdentityProvider);
                    sessionVariables.Add("InitApp", initApp);
                    sessionVariables.Add("InitUser", initUser);
                    sessionVariables.Add("Imports", imports);
                    sessionVariables.Add("Pages", pages);

                    fileLocation = appFolderPath;
                    filePath = PathCombine(fileLocation, appName.ToLower() + ".component.ts");
                    fileInfo = new FileInfo(filePath);

                    host.SetGenerator(typeof(AppGenerator), "Class");
                    output = host.Generate<AppComponentClassTemplate>(sessionVariables, false);

                    generatorConfiguration.CreateFile(fileInfo, output, FileKind.Project, () => generatorConfiguration.GenerateInfo(sessionVariables, "App Component Class"));
                }
                while (host.PostProcess() == PostProcessResult.RedoGenerate);

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("HomePage", pages.SingleOrDefault(p => p.UILoadKind == UILoadKind.HomePage));
                sessionVariables.Add("MainPage", pages.SingleOrDefault(p => p.UILoadKind == UILoadKind.MainPage));

                fileLocation = pagesPath;
                filePath = PathCombine(fileLocation, "index.ts");
                fileInfo = new FileInfo(filePath);

                do
                {
                    host.SetGenerator(typeof(AppGenerator), "PageIndex");
                    output = host.Generate<PageIndexTemplate>(sessionVariables, false);

                    generatorConfiguration.CreateFile(fileInfo, output, FileKind.Project, () => generatorConfiguration.GenerateInfo(sessionVariables, "Page Index Class"));
                }
                while (host.PostProcess() == PostProcessResult.RedoGenerate);


                // sass

                sessionVariables = new Dictionary<string, object>();

                fileLocation = appFolderPath;
                filePath = PathCombine(fileLocation, appName.ToLower() + ".component.scss");
                fileInfo = new FileInfo(filePath);

                host.SetGenerator(typeof(AppGenerator), "Sass");

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
