using AbstraX.Angular;
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
    public static class AppGenerator
    {
        public static void GenerateApp(string appFolderPath, string appName, IGeneratorConfiguration generatorConfiguration, IEnumerable<ModuleImportDeclaration> imports, IEnumerable<Page> pages)
        {
            var host = new TemplateEngineHost();
            var pass = generatorConfiguration.CurrentPass;
            var pagesPath = generatorConfiguration.ApplicationFolderHierarchy[IonicFileSystemType.Pages];
            var pagesFolder = (Folder)generatorConfiguration.FileSystem[pagesPath];
            Dictionary<string, object> sessionVariables;
            FileInfo fileInfo;
            string fileLocation;
            string filePath;
            string output;
            string initApp;

            try
            {
                fileLocation = appFolderPath;
                filePath = PathCombine(fileLocation, appName.ToLower() + ".component.html");
                fileInfo = new FileInfo(filePath);

                sessionVariables = new Dictionary<string, object>();

                output = host.Generate<AppComponentPageTemplate>(sessionVariables, false);

                generatorConfiguration.CreateFile(fileInfo, output, FileKind.Project, () => generatorConfiguration.GenerateInfo(sessionVariables, "App Component Page"));

                // todo - obfuscate

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("IdentityProvider", generatorConfiguration.IdentityProvider);

                initApp = host.Generate<InitApp>(sessionVariables, false);

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("AppName", generatorConfiguration.AppName);
                sessionVariables.Add("IdentityProvider", generatorConfiguration.IdentityProvider);
                sessionVariables.Add("InitApp", initApp);
                sessionVariables.Add("Imports", imports);
                sessionVariables.Add("Pages", pages);

                fileLocation = appFolderPath;
                filePath = PathCombine(fileLocation, appName.ToLower() + ".component.ts");
                fileInfo = new FileInfo(filePath);

                output = host.Generate<AppComponentClassTemplate>(sessionVariables, false);

                generatorConfiguration.CreateFile(fileInfo, output, FileKind.Project, () => generatorConfiguration.GenerateInfo(sessionVariables, "App Component Class"));

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("RootPage", pages.SingleOrDefault(p => p.UILoadKind == UILoadKind.RootPage));
                sessionVariables.Add("MainPage", pages.SingleOrDefault(p => p.UILoadKind == UILoadKind.MainPage));

                fileLocation = pagesPath;
                filePath = PathCombine(fileLocation, "index.ts");
                fileInfo = new FileInfo(filePath);

                output = host.Generate<PageIndexTemplate>(sessionVariables, false);

                generatorConfiguration.CreateFile(fileInfo, output, FileKind.Project, () => generatorConfiguration.GenerateInfo(sessionVariables, "Page Index Class"));
            }
            catch (Exception e)
            {
                generatorConfiguration.AppGeneratorEngine.WriteError(e.ToString());
            }
        }
    }
}
