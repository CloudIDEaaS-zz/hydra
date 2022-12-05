using AbstraX.Angular;
using AbstraX.Generators.Client.Internationalization;
using AbstraX.ObjectProperties;
using AbstraX.ServerInterfaces;
using EntityProvider.Web.Entities;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using static AbstraX.FolderStructure.FileSystemObject;

namespace AbstraX.Generators.Client.Theme
{
    [GeneratorTemplate("VariablesScss", @"Generators\Client\Theme\VariablesScssTemplate.tt")]
    [GeneratorTemplate("DataJson", @"Generators\Client\Theme\DataJsonTemplate.tt")]
    public static class ThemeGenerator
    {
        public static void GenerateTheme(string srcFolder, string themeFolder, string assetsImagesFolder, string assetsDataFolder, IGeneratorConfiguration generatorConfiguration)
        {
            var host = generatorConfiguration.GetTemplateEngineHost();
            var pass = generatorConfiguration.CurrentPass;
            var imagesFolder = assetsImagesFolder.ReverseSlashes();
            var imagesRelative = imagesFolder.RemoveStart(srcFolder.ReverseSlashes());
            var imagesUrlPath = imagesRelative.ReverseSlashes().Append("/");
            var tempSrcFolder = Path.Combine(Path.GetTempPath(), generatorConfiguration.AppName.Replace(" ",  "-"));
            Dictionary<string, object> sessionVariables;
            List<ImageInfo> imageAssets;
            IResourceDataProcessed<ImageInfo, LinkInfo> resourceDataProcessed;
            Dictionary<string, List<ColorGroup>> appliedColorGroups;
            ObjectPropertiesDictionary objectProperties;
            ObjectPropertiesDictionary marketingProperties;
            MarketingObjectProperties marketingObjectProperties;
            FolderStructure.File file;
            TextWriter textWriter;
            FileInfo fileInfo;
            string fileLocation;
            string filePath;
            string output;

            try
            {
                sessionVariables = new Dictionary<string, object>();

                if (generatorConfiguration.AppGeneratorEngine.GeneratorMode != GeneratorMode.RedirectedConsole)
                {
                    textWriter = new StreamWriter(Console.OpenStandardOutput());
                }
                else
                {
                    textWriter = generatorConfiguration.RedirectedWriter;
                }

                resourceDataProcessed = (IResourceDataProcessed<ImageInfo, LinkInfo>)generatorConfiguration.KeyValuePairs["ResourceData"];
                appliedColorGroups = resourceDataProcessed.ApplyColorGroups();

                if (generatorConfiguration.KeyValuePairs.ContainsKey("ImageAssets"))
                {
                    imageAssets = (List<ImageInfo>) generatorConfiguration.KeyValuePairs["ImageAssets"];
                }
                else
                {
                    imageAssets = new List<ImageInfo>();

                    generatorConfiguration.KeyValuePairs.Add("ImageAssets", imageAssets);
                }

                sessionVariables.Add("ResourceData", resourceDataProcessed);
                sessionVariables.Add("AppName", generatorConfiguration.AppName);
                sessionVariables.Add("Description", generatorConfiguration.AppDescription);
                sessionVariables.Add("AuthorName", generatorConfiguration.OrganizationName);
                sessionVariables.Add("ThemeColor", resourceDataProcessed.PrimaryColor);

                // variables.scss

                fileLocation = srcFolder;
                filePath = PathCombine(fileLocation, @"index.html");
                fileInfo = new FileInfo(filePath);

                host.SetGenerator(typeof(ThemeGenerator), "HtmlIndexPage");

                do
                {
                    output = host.Generate<HtmlIndexTemplate>(sessionVariables, false);

                    file = generatorConfiguration.CreateFile(fileInfo, output, FileKind.Project, () => generatorConfiguration.GenerateInfo(sessionVariables, "index.html"));
                }
                while (host.PostProcess() == PostProcessResult.RedoGenerate);

                // variables.scss

                fileLocation = themeFolder;
                filePath = PathCombine(fileLocation, @"variables.scss");
                fileInfo = new FileInfo(filePath);

                host.SetGenerator(typeof(ThemeGenerator), "VariablesScss");

                do
                {
                    output = host.Generate<VariablesScssTemplate>(sessionVariables, false);

                    file = generatorConfiguration.CreateFile(fileInfo, output, FileKind.Project, () => generatorConfiguration.GenerateInfo(sessionVariables, "variables.scss"));
                }
                while (host.PostProcess() == PostProcessResult.RedoGenerate);

                // data.json

                objectProperties = resourceDataProcessed.ObjectProperties;

                if (objectProperties.ContainsKey("MarketingObjectProperties"))
                {
                    marketingProperties = objectProperties["MarketingObjectProperties"];
                    marketingObjectProperties = marketingProperties.ToObject<MarketingObjectProperties>();
                }
                else
                {
                    marketingObjectProperties = new MarketingObjectProperties();
                }

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("ResourceData", resourceDataProcessed);
                sessionVariables.Add("MarketingObjectProperties", marketingObjectProperties);

                fileLocation = assetsDataFolder;
                filePath = PathCombine(fileLocation, @"data.json");
                fileInfo = new FileInfo(filePath);

                host.SetGenerator(typeof(ThemeGenerator), "DataJson");

                if (generatorConfiguration.CurrentPass == GeneratorPass.Files)
                {
                    resourceDataProcessed.CurrentImagesPath = srcFolder;
                }
                else
                {
                    if (!Directory.Exists(tempSrcFolder))
                    {
                        Directory.CreateDirectory(tempSrcFolder);
                    }

                    resourceDataProcessed.CurrentImagesPath = tempSrcFolder;
                }

                resourceDataProcessed.CurrentImageFilePattern = string.Format("{0}{1}-{2}-{{0}}-{{1}}{{{{0}}}}{{{{1}}}}", imagesUrlPath, resourceDataProcessed.OrganizationName.NullToEmpty().Replace(" ", "-").ToLower(), resourceDataProcessed.AppName.NullToEmpty().Replace(" ", "-").ToLower());
                resourceDataProcessed.GeneratorPass = generatorConfiguration.CurrentPass;

                do
                {
                    output = host.Generate<DataJsonTemplate>(sessionVariables, false);

                    if (!JsonExtensions.IsValidJson(output))
                    {
                        var exception = JsonExtensions.GetJsonExceptions(output);

                        File.WriteAllText(Path.Combine(tempSrcFolder, "data.json"), output);
                    }

                    file = generatorConfiguration.CreateFile(fileInfo, output, FileKind.Project, () => generatorConfiguration.GenerateInfo(sessionVariables, "data.json"));
                }
                while (host.PostProcess() == PostProcessResult.RedoGenerate);

                if (generatorConfiguration.CurrentPass == GeneratorPass.Files)
                {
                    using (var resourceData = generatorConfiguration.GetResourceData())
                    {
                        // verify and copy images to directories

                        foreach (var imageInfo in resourceDataProcessed.ParsedImages)
                        {
                            var imageLocation = Path.Combine(srcFolder, imageInfo.UrlPath.RemoveStartIfMatches("/")).BackSlashes();
                            var fileInfoImage = new FileInfo(imageLocation);

                            if (imageInfo.CopyToFilePath != null)
                            {
                                var copyToFile = new FileInfo(imageInfo.CopyToFilePath);

                                if (imageInfo.OriginalPath != null)
                                {
                                    var copyFromFile = new FileInfo(imageInfo.OriginalPath);

                                    if (copyToFile.Extension != copyFromFile.Extension)
                                    {
                                        var bitmapImage = (Bitmap)Bitmap.FromFile(copyFromFile.FullName);

                                        switch (copyToFile.Extension)
                                        {
                                            case "jpg":
                                                bitmapImage.Save(copyToFile.FullName, ImageFormat.Jpeg);
                                                break;
                                            default:
                                                DebugUtils.Break();
                                                break;
                                        }

                                        fileInfoImage = new FileInfo(imageLocation);
                                    }
                                    else
                                    {
                                        if (!Directory.Exists(copyToFile.DirectoryName))
                                        {
                                            Directory.CreateDirectory(copyToFile.DirectoryName);
                                        }

                                        copyFromFile.CopyTo(copyToFile.FullName, true);

                                        fileInfoImage = new FileInfo(imageLocation);
                                    }

                                    if (!fileInfoImage.Exists)
                                    {
                                        DebugUtils.Break();
                                    }
                                }

                                resourceData.SetImageFileDetails(imageInfo, fileInfoImage, textWriter);
                            }
                        }
                    }
                }
                else
                {
                    var imagesDirectory = new DirectoryInfo(imagesFolder);

                    foreach (var imageInfo in resourceDataProcessed.ParsedImages)
                    {
                        if (imagesDirectory.Exists)
                        {
                            var pattern = imageInfo.CleanPattern;
                            var oldImageFiles = imagesDirectory.GetFiles("*.*", SearchOption.AllDirectories).Where(f => f.Name.RegexIsMatch(pattern));

                            foreach (var imageFile in oldImageFiles)
                            {
                                imageFile.MakeWritable();
                                imageFile.Delete();
                            }
                        }
                    }

                    try
                    {
                        resourceDataProcessed.ParsedImages.Clear();
                        Directory.Delete(tempSrcFolder, true);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            catch (Exception e)
            {
                generatorConfiguration.AppGeneratorEngine.WriteError(e.ToString());
            }
        }
    }
}
