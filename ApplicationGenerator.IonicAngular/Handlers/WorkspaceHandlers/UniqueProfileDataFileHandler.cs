using AbstraX.Handlers.WorkspaceHandlers.UniqueProfileData;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Utils;

namespace AbstraX.Handlers.WorkspaceHandlers
{
    public class UniqueProfileDataFileHandler : IUniqueProfileDataFileHandler
    {
        public float Priority => 1f;

        public bool Process(Guid projectType, IAppFolderStructureSurveyor appFolderStructureSurveyor, string organizationUniqueName, string appUniqueName, IGeneratorConfiguration generatorConfiguration)
        {
            var replacementsDictionary = GetFileReplacements(appFolderStructureSurveyor, organizationUniqueName, appUniqueName);

            foreach (var pair in replacementsDictionary)
            {
                var filePath = pair.Key;
                var replacements = pair.Value;

                foreach (var replacement in replacements)
                {
                    switch (replacement)
                    {
                        case XmlReplacment xmlReplacment:
                            {
                                var document = XDocument.Load(filePath);
                                var element = document.XPathSelectElement(xmlReplacment.Path);

                                xmlReplacment.Replace(element);

                                using (var writer = new XmlTextWriter(filePath, null))
                                {
                                    writer.Formatting = Formatting.Indented;
                                    document.Save(writer);
                                }
                            }

                            break;

                        case JsonReplacement jsonReplacement:
                            {
                                JObject jsonObject;

                                using (var reader = new StreamReader(filePath))
                                {
                                    jsonObject = reader.ReadJson<JObject>();
                                    var jToken = jsonObject.SelectToken(jsonReplacement.Path);

                                    jsonReplacement.Replace(jToken);
                                }

                                using (var writer = new StreamWriter(filePath))
                                {
                                    writer.WriteJson(JsonExtensions.ToJsonText((object)jsonObject, Newtonsoft.Json.Formatting.Indented, new CamelCaseNamingStrategy()));
                                }

                                break;
                            }
                    }
                }
            }

            return true;
        }

        private Dictionary<string, List<IReplacement>> GetFileReplacements(IAppFolderStructureSurveyor appFolderStructureSurveyor, string organizationUniqueName, string appUniqueName)
        {
            var replacements = new Dictionary<string, List<IReplacement>>();
            string filePath;
            string rootPath;

            // web project file

            rootPath = appFolderStructureSurveyor.WebProjectPath;
            filePath = rootPath;

            if (File.Exists(filePath))
            {
                replacements.Add(filePath, new List<IReplacement>
                {
                    new XmlReplacment
                    {
                        Path = "/Project/Target[@Name='PublishRunWebpack']/Exec[2]",
                        Replace = (XElement e) => e.Attribute("Command").Value = $"npm run build -- --configuration production --base-href=/{ organizationUniqueName }/{ appUniqueName }/web"
                    },
                    new XmlReplacment
                    {
                        Path = "/Project/Target[@Name='PublishRunWebpack']/Exec[3]",
                        Replace = (XElement e) => e.Attribute("Command").Value = $"npm run build:ssr -- --configuration production --base-href=/{ organizationUniqueName }/{ appUniqueName }/web"
                    }
                });
            }

            // appsettings.json

            rootPath = appFolderStructureSurveyor.WebProjectPath;
            filePath = Path.Combine(Path.GetDirectoryName(rootPath), @"appsettings.json");

            if (File.Exists(filePath))
            {
                replacements.Add(filePath, new List<IReplacement>
                {
                    new JsonReplacement
                    {
                        Path = "$.BaseUrl",
                        Replace = (JToken t) => t.ChangeTo($"/{ organizationUniqueName }/{ appUniqueName }/web")
                    }
                });
            }

            // launchSettings.json

            filePath = Path.Combine(Path.GetDirectoryName(rootPath), @"Properties\launchSettings.json");

            if (File.Exists(filePath))
            {
                replacements.Add(filePath, new List<IReplacement>
                {

                });
            }

            // angular.json

            rootPath = appFolderStructureSurveyor.WebFrontEndRootPath;
            filePath = Path.Combine(Path.GetDirectoryName(rootPath), @"angular.json");

            if (File.Exists(filePath))
            {
                replacements.Add(filePath, new List<IReplacement>
                {

                });
            }

            // package.json

            rootPath = appFolderStructureSurveyor.WebFrontEndRootPath;
            filePath = Path.Combine(Path.GetDirectoryName(rootPath), @"package.json");

            if (File.Exists(filePath))
            {
                replacements.Add(filePath, new List<IReplacement>
                {

                });
            }

            return replacements;
        }
    }
}
