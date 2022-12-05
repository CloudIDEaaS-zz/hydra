using AbstraX.Angular;
using AbstraX.DataAnnotations;
using AbstraX.Generators.Pages.WelcomePage;
using AbstraX.ServerInterfaces;
using EntityProvider.Web.Entities;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using static AbstraX.FolderStructure.FileSystemObject;

namespace AbstraX.Generators.Pages.WelcomePage
{
    [GeneratorTemplate("Page", @"Generators\Pages\WelcomePage\WelcomePageTemplate.tt")]
    [GeneratorTemplate("Class", @"Generators\Pages\WelcomePage\WelcomeClassTemplate.tt")]
    [GeneratorTemplate("Sass", @"Generators\Pages\WelcomePage\SassStyleSheetTemplate.tt")]
    public static class WelcomePageGenerator 
    {
        public static void GeneratePage(IBase baseObject, string pagesFolderPath, string pageName, string appName, string appDescription, IGeneratorConfiguration generatorConfiguration, IModuleAssembly module, IEnumerable<ModuleImportDeclaration> imports, UILoadKind loadKind, UIKind uiKind)
        {
            var host = generatorConfiguration.GetTemplateEngineHost();
            var pass = generatorConfiguration.CurrentPass;
            var moduleAssemblyProperties = new AngularModuleAssemblyProperties(baseObject, imports);
            var usesAboutThisImage = false;
            Dictionary<string, object> sessionVariables;
            FileInfo fileInfo;
            string fileLocation;
            string filePath;
            string output;
            ResourceDataClone resourceData;
            ObjectPropertiesDictionary objectProperties;
            ImageInfo imageInfo;
            HtmlNode htmlImage;
            HtmlNode htmlAnchor;
            string slideImageUrl;

            try
            {
                // Welcome page

                sessionVariables = new Dictionary<string, object>();

                resourceData = (ResourceDataClone)generatorConfiguration.KeyValuePairs["ResourceData"];
                resourceData.GeneratorPass = generatorConfiguration.CurrentPass;

                resourceData.CurrentImageFilePattern = string.Format("{0}-{1}-{{0}}-{{1}}", resourceData.OrganizationName.NullToEmpty().Replace(" ", "-").ToLower(), resourceData.AppName.NullToEmpty().Replace(" ", "-").ToLower());
                imageInfo = resourceData.CreateImage("SplashScreenImage", resourceData.SplashScreen, "splash", generatorConfiguration);

                if (resourceData.ObjectProperties.ContainsKey("SplashScreenObjectProperties"))
                {
                    objectProperties = resourceData.ObjectProperties["SplashScreenObjectProperties"];
                }
                else
                {
                    objectProperties = new ObjectPropertiesDictionary();
                }

                resourceData.ProcessImage(objectProperties, imageInfo, imageInfo.ResourceName, imageInfo.HtmlImage);
                htmlImage = imageInfo.HtmlImage;

                if (htmlImage.ParentNode.Name == "a")
                {
                    htmlAnchor = htmlImage.ParentNode;
                    usesAboutThisImage = true;

                    sessionVariables.Add("UsesAboutThisImage", usesAboutThisImage);
                    sessionVariables.Add("AnchorHref", (string)htmlAnchor.Attributes["href"].Value);
                    sessionVariables.Add("AnchorTitle", (string)htmlAnchor.Attributes["title"].Value);
                }

                slideImageUrl = htmlImage.Attributes["src"].Value;

                sessionVariables.Add("PageName", pageName);
                sessionVariables.Add("AppName", appName);
                sessionVariables.Add("AppDescription", appDescription);
                sessionVariables.Add("SlideImageUrl", slideImageUrl);

                fileLocation = PathCombine(pagesFolderPath, pageName.ToLower());
                filePath = PathCombine(fileLocation, pageName.ToLower() + ".html");
                fileInfo = new FileInfo(filePath);

                host.SetGenerator(typeof(WelcomePageGenerator), "Page");

                do
                {
                    output = host.Generate<WelcomePageTemplate>(sessionVariables, false);

                    generatorConfiguration.CreateFile(fileInfo, output, FileKind.Project, () => generatorConfiguration.GenerateInfo(sessionVariables, "Welcome Page"));
                }
                while (host.PostProcess() == PostProcessResult.RedoGenerate);

                // Welcome class

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("PageName", pageName);
                sessionVariables.Add("UILoadKind", loadKind);
                sessionVariables.Add("UIKind", uiKind);
                sessionVariables.AddModuleAssemblyProperties(moduleAssemblyProperties);

                fileLocation = PathCombine(pagesFolderPath, pageName.ToLower());
                filePath = PathCombine(fileLocation, pageName.ToLower() + ".ts");
                fileInfo = new FileInfo(filePath);

                host.SetGenerator(typeof(WelcomePageGenerator), "Class");

                do
                {
                    output = host.Generate<WelcomeClassTemplate>(sessionVariables, false);

                    module.ExportedComponents = sessionVariables.GetExportedComponents();
                    module.ForChildFile = generatorConfiguration.CreateFile(moduleAssemblyProperties, fileInfo, output, FileKind.Project, () => generatorConfiguration.GenerateInfo(sessionVariables, "Welcome Page Class"));
                }
                while (host.PostProcess() == PostProcessResult.RedoGenerate);

                // sass

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("PageName", pageName);

                fileLocation = PathCombine(pagesFolderPath, pageName.ToLower());
                filePath = PathCombine(fileLocation, pageName.ToLower() + ".scss");
                fileInfo = new FileInfo(filePath);

                host.SetGenerator(typeof(WelcomePageGenerator), "Sass");

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
