using AbstraX.Angular;
using AbstraX.DataAnnotations;
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

namespace AbstraX.Generators.Pages.AboutPage
{
    [GeneratorTemplate("Page", @"Generators\Pages\AboutPage\AboutPageTemplate.tt")]
    [GeneratorTemplate("Class", @"Generators\Pages\AboutPage\AboutClassTemplate.tt")]
    [GeneratorTemplate("Sass", @"Generators\Pages\AboutPage\SassStyleSheetTemplate.tt")]
    public static class AboutPageGenerator
    {
        public static void GeneratePage(IBase baseObject, string pagesFolderPath, string pageName, IGeneratorConfiguration generatorConfiguration, IModuleAssembly module, IEnumerable<ModuleImportDeclaration> imports)
        {
            var host = generatorConfiguration.GetTemplateEngineHost();
            var pass = generatorConfiguration.CurrentPass;
            var moduleAssemblyProperties = new AngularModuleAssemblyProperties(baseObject, imports);
            var usesAboutThisImage = false;
            Dictionary<string, object> sessionVariables;
            ResourceDataClone resourceData;
            ObjectPropertiesDictionary objectProperties;
            FileInfo fileInfo;
            ImageInfo imageInfo;
            HtmlNode htmlImage;
            HtmlNode htmlAnchor;
            string fileLocation;
            string filePath;
            string output;
            string aboutImageUrl;

            try
            {
                // About page

                sessionVariables = new Dictionary<string, object>();

                resourceData = (ResourceDataClone)generatorConfiguration.KeyValuePairs["ResourceData"];
                resourceData.GeneratorPass = generatorConfiguration.CurrentPass;

                resourceData.CurrentImageFilePattern = string.Format("{0}-{1}-{{0}}-{{1}}", resourceData.OrganizationName.NullToEmpty().Replace(" ", "-").ToLower(), resourceData.AppName.NullToEmpty().Replace(" ", "-").ToLower());
                imageInfo = resourceData.CreateImage("AboutBannerImage", resourceData.AboutBanner, "about", generatorConfiguration);

                if (resourceData.ObjectProperties.ContainsKey("AboutBannerObjectProperties"))
                {
                    objectProperties = resourceData.ObjectProperties["AboutBannerObjectProperties"];
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
                    sessionVariables.Add("AnchorHref", (string) htmlAnchor.Attributes["href"].Value);
                    sessionVariables.Add("AnchorTitle", (string)htmlAnchor.Attributes["title"].Value);
                }

                aboutImageUrl = htmlImage.Attributes["src"].Value;

                fileLocation = PathCombine(pagesFolderPath, pageName.ToLower());
                filePath = PathCombine(fileLocation, pageName.ToLower() + ".html");
                fileInfo = new FileInfo(filePath);

                host.SetGenerator(typeof(AboutPageGenerator), "Page");

                do
                {
                    output = host.Generate<AboutPageTemplate>(sessionVariables, false);

                    generatorConfiguration.CreateFile(fileInfo, output, FileKind.Project, () => generatorConfiguration.GenerateInfo(sessionVariables, "About Page"));
                }
                while (host.PostProcess() == PostProcessResult.RedoGenerate);

                // About class

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("PageName", pageName);
                sessionVariables.Add("UIKind", UIKind.AboutPage);
                sessionVariables.Add("UILoadKind", UILoadKind.Default);
                sessionVariables.Add("Authorize", generatorConfiguration.AuthorizedRoles);
                sessionVariables.AddModuleAssemblyProperties(moduleAssemblyProperties);

                fileLocation = PathCombine(pagesFolderPath, pageName.ToLower());
                filePath = PathCombine(fileLocation, pageName.ToLower() + ".ts");
                fileInfo = new FileInfo(filePath);

                host.SetGenerator(typeof(AboutPageGenerator), "Class");

                do
                {
                    output = host.Generate<AboutClassTemplate>(sessionVariables, false);

                    module.ExportedComponents = sessionVariables.GetExportedComponents();
                    module.ForChildFile = generatorConfiguration.CreateFile(moduleAssemblyProperties, fileInfo, output, FileKind.Project, () => generatorConfiguration.GenerateInfo(sessionVariables, "About Page Class"));
                }
                while (host.PostProcess() == PostProcessResult.RedoGenerate);

                // sass

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("PageName", pageName);
                sessionVariables.Add("AboutImageUrl", aboutImageUrl);

                fileLocation = PathCombine(pagesFolderPath, pageName.ToLower());
                filePath = PathCombine(fileLocation, pageName.ToLower() + ".scss");
                fileInfo = new FileInfo(filePath);

                host.SetGenerator(typeof(AboutPageGenerator), "Sass");

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
