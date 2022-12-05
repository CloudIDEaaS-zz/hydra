using AbstraX.Angular;
using AbstraX.Generators.Client.Internationalization;
using AbstraX.ServerInterfaces;
using EntityProvider.Web.Entities;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using static AbstraX.FolderStructure.FileSystemObject;

namespace AbstraX.Generators.Client.Config
{
    [GeneratorTemplate("PackageWidgetXML", @"Generators\Client\Config\PackageWidgetXMLTemplate.tt")]
    [GeneratorTemplate("WebAppManifest", @"Generators\Client\Config\WebAppManifestTemplate.tt")]
    [GeneratorTemplate("LaunchJson", @"Generators\Client\Config\LaunchJsonTemplate.tt")]
    [GeneratorTemplate("TasksJson", @"Generators\Client\Config\TasksJsonTemplate.tt")]
    public static class ConfigGenerator
    {
        public static void GenerateConfig(string projectRootFolder, IGeneratorConfiguration generatorConfiguration)
        {
            var host = generatorConfiguration.GetTemplateEngineHost();
            var pass = generatorConfiguration.CurrentPass;
            var appName = generatorConfiguration.AppName;
            var appDescription = generatorConfiguration.AppDescription;
            var portNumber = NumberExtensions.GetRandomIntWithinRange(8089, 8997);
            IResourceDataProcessed<ImageInfo, LinkInfo> resourceDataProcessed;
            Dictionary<string, object> sessionVariables;
            FolderStructure.File file;
            string fileLocation;
            string filePath;
            string output;
            string teamName;
            string authorUrl;
            string authorEmail;
            string hostName;
            dynamic packageJsonObject;

            try
            {
                resourceDataProcessed = (IResourceDataProcessed<ImageInfo, LinkInfo>)generatorConfiguration.KeyValuePairs["ResourceData"];

                teamName = appName + " Team";
                authorEmail = resourceDataProcessed.SupportEmailAddress.AsDisplayText();
                authorUrl = resourceDataProcessed.SupportUrl.AsDisplayText();

                if (resourceDataProcessed.SupportUrl != null)
                {
                    var uri = new Uri(resourceDataProcessed.SupportUrl);

                    hostName = uri.Host;
                }
                else
                {
                    hostName = resourceDataProcessed.OrganizationName.ToLower().RemoveNonWordCharacters().RemoveSpaces();
                }

                filePath = PathCombine(projectRootFolder, "package.json");

                if (generatorConfiguration.CurrentPass == GeneratorPass.Files)
                {
                    using (var reader = new StreamReader(filePath))
                    {
                        packageJsonObject = JsonExtensions.ReadJson<dynamic>(reader);
                        packageJsonObject.name = appName;
                        packageJsonObject.description = appDescription;
                        packageJsonObject.author.name = teamName;
                        packageJsonObject.author.email = authorEmail;
                        packageJsonObject.repository.url = string.Empty;
                        packageJsonObject.readme = string.Format("# {0}\r\n\r\n{1}", appName, appDescription);
                        packageJsonObject.gitHead = string.Empty;
                        packageJsonObject.bugs.url = authorEmail;
                        packageJsonObject.homepage = string.Format("https://www.{0}.com", appName);
                        packageJsonObject._id = string.Format("{0}-app@0.0.0", appName);
                    }

                    using (var writer = new StreamWriter(filePath))
                    {
                        var json = JsonExtensions.ToJsonText(packageJsonObject, Newtonsoft.Json.Formatting.Indented, new CamelCaseNamingStrategy());

                        JsonExtensions.WriteJson(writer, json);
                    }
                }

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("AppName", appName);
                sessionVariables.Add("AppDescription", appDescription);
                sessionVariables.Add("HostName", hostName);
                sessionVariables.Add("AuthorEmail", authorEmail);
                sessionVariables.Add("AuthorUrl", authorUrl);
                sessionVariables.Add("TeamName", teamName);

                fileLocation = projectRootFolder;

                do
                {
                    filePath = PathCombine(projectRootFolder, "config.xml");

                    host.SetGenerator(typeof(PackageWidgetXMLTemplate), "PackageWidgetXML");
                    output = host.Generate<PackageWidgetXMLTemplate>(sessionVariables, false);

                    if (pass == GeneratorPass.Files)
                    {
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }

                        if (!Directory.Exists(fileLocation))
                        {
                            generatorConfiguration.CreateDirectory(fileLocation);
                        }

                        using (var fileStream = generatorConfiguration.CreateFile(filePath))
                        {
                            fileStream.Write(output);
                            generatorConfiguration.FileSystem.DeleteFile(filePath);

                            file = generatorConfiguration.FileSystem.AddSystemLocalProjectFile(new FileInfo(filePath));
                        }
                    }
                    else if (pass == GeneratorPass.StructureOnly)
                    {
                        file = generatorConfiguration.FileSystem.AddSystemLocalProjectFile(new FileInfo(filePath), generatorConfiguration.GenerateInfo(sessionVariables, "{\r\n\t\"Test\": \"Test\"\r\n}"));
                    }
                }
                while (host.PostProcess() == PostProcessResult.RedoGenerate);

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("AppName", appName);
                sessionVariables.Add("BackgroundColor", resourceDataProcessed.BackgroundColor);
                sessionVariables.Add("ThemeColor", resourceDataProcessed.PrimaryColor);

                fileLocation = PathCombine(projectRootFolder, @"src");

                do
                {
                    filePath = PathCombine(fileLocation, "manifest.json");

                    host.SetGenerator(typeof(WebAppManifestTemplate), "WebAppManifest");
                    output = host.Generate<WebAppManifestTemplate>(sessionVariables, false);

                    if (pass == GeneratorPass.Files)
                    {
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }

                        if (!Directory.Exists(fileLocation))
                        {
                            generatorConfiguration.CreateDirectory(fileLocation);
                        }

                        using (var fileStream = generatorConfiguration.CreateFile(filePath))
                        {
                            fileStream.Write(output);
                            generatorConfiguration.FileSystem.DeleteFile(filePath);

                            file = generatorConfiguration.FileSystem.AddSystemLocalProjectFile(new FileInfo(filePath));
                        }
                    }
                    else if (pass == GeneratorPass.StructureOnly)
                    {
                        file = generatorConfiguration.FileSystem.AddSystemLocalProjectFile(new FileInfo(filePath), generatorConfiguration.GenerateInfo(sessionVariables, "{\r\n\t\"Test\": \"Test\"\r\n}"));
                    }
                }
                while (host.PostProcess() == PostProcessResult.RedoGenerate);

                sessionVariables = new Dictionary<string, object>();

                sessionVariables.Add("AppName", appName);
                sessionVariables.Add("PortNumber", portNumber);

                fileLocation = PathCombine(projectRootFolder, @".vscode");

                do
                {
                    filePath = PathCombine(fileLocation, @"launch.json");

                    host.SetGenerator(typeof(ConfigGenerator), "LaunchJson");
                    output = host.Generate<LaunchJsonTemplate>(sessionVariables, false);

                    if (pass == GeneratorPass.Files)
                    {
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }

                        if (!Directory.Exists(fileLocation))
                        {
                            generatorConfiguration.CreateDirectory(fileLocation);
                        }

                        using (var fileStream = generatorConfiguration.CreateFile(filePath))
                        {
                            fileStream.Write(output);
                            generatorConfiguration.FileSystem.DeleteFile(filePath);

                            file = generatorConfiguration.FileSystem.AddSystemLocalProjectFile(new FileInfo(filePath));
                        }
                    }
                    else if (pass == GeneratorPass.StructureOnly)
                    {
                        file = generatorConfiguration.FileSystem.AddSystemLocalProjectFile(new FileInfo(filePath), generatorConfiguration.GenerateInfo(sessionVariables, "{\r\n\t\"Test\": \"Test\"\r\n}"));
                    }

                    filePath = PathCombine(fileLocation, @"tasks.json");

                    host.SetGenerator(typeof(ConfigGenerator), "TasksJson");
                    output = host.Generate<TasksJsonTemplate>(sessionVariables, false);

                    if (pass == GeneratorPass.Files)
                    {
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }

                        if (!Directory.Exists(fileLocation))
                        {
                            generatorConfiguration.CreateDirectory(fileLocation);
                        }

                        using (var fileStream = generatorConfiguration.CreateFile(filePath))
                        {
                            fileStream.Write(output);
                            generatorConfiguration.FileSystem.DeleteFile(filePath);

                            file = generatorConfiguration.FileSystem.AddSystemLocalProjectFile(new FileInfo(filePath));
                        }
                    }
                    else if (pass == GeneratorPass.StructureOnly)
                    {
                        file = generatorConfiguration.FileSystem.AddSystemLocalProjectFile(new FileInfo(filePath), generatorConfiguration.GenerateInfo(sessionVariables, "{\r\n\t\"Test\": \"Test\"\r\n}"));
                    }
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
