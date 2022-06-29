// file:	ConfigObject.cs
//
// summary:	Implements the configuration object class

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using Utils;

namespace AbstraX
{
    /// <summary>   A configuration object. </summary>
    ///
    /// <remarks>   Ken, 10/1/2020. </remarks>

    public class ConfigObject
    {
        /// <summary>   Gets or sets the name of the application. </summary>
        ///
        /// <value> The name of the application. </value>

        public string AppName { get; set; }

        /// <summary>   Gets or sets the name of the organization. </summary>
        ///
        /// <value> The name of the organization. </value>

        public string AppDescription { get; set; }

        /// <summary>   Gets or sets the name of the organization. </summary>
        ///
        /// <value> The name of the organization. </value>

        public string OrganizationName { get; set; }

        /// <summary>   Gets or sets the full pathname of the solution file. </summary>
        ///
        /// <value> The full pathname of the solution file. </value>

        public string WorkspacePath { get; set; }

        /// <summary>   Gets or sets the full pathname of the services project file. </summary>
        ///
        /// <value> The full pathname of the services project file. </value>

        public string ServicesProjectPath { get; set; }

        /// <summary>   Gets or sets the full pathname of the entities project file. </summary>
        ///
        /// <value> The full pathname of the entities project file. </value>

        public string EntitiesProjectPath { get; set; }

        /// <summary>   Gets or sets the full pathname of the web project file. </summary>
        ///
        /// <value> The full pathname of the web project file. </value>

        public string WebProjectPath { get; set; }

        /// <summary>   Gets or sets the full pathname of the web front end root file. </summary>
        ///
        /// <value> The full pathname of the web front end root file. </value>

        public string WebFrontEndRootPath { get; set; }

        /// <summary>   Gets or sets the generator mode. </summary>
        ///
        /// <value> The generator mode. </value>

        public GeneratorMode GeneratorMode { get; set; }

        /// <summary>   Gets or sets the generator kind. </summary>
        ///
        /// <value> The generator kind. </value>

        public GeneratorKind GeneratorKind { get; set; }

        /// <summary>   Gets or sets the generator kinds. </summary>
        ///
        /// <value> The generator kinds. </value>

        public string GeneratorKinds { get; set; }

        /// <summary>   Gets or sets the type of the generator handler. </summary>
        ///
        /// <value> The type of the generator handler. </value>

        public string GeneratorHandlerType { get; set; }

        /// <summary>   Gets or sets options for controlling the generator. </summary>
        ///
        /// <value> Options that control the generator. </value>

        public dynamic GeneratorOptions { get; set; }

        /// <summary>   Gets or sets the full pathname of the package cache file. </summary>
        ///
        /// <value> The full pathname of the package cache file. </value>

        public string PackageCachePath { get; set; }

        /// <summary>   Gets or sets the arguments kind. </summary>
        ///
        /// <value> The arguments kind. </value>

        public string ArgumentsKind { get; set; }

        /// <summary>   Gets or sets information describing the application. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/15/2021. </remarks>
        ///
        /// <param name="directory">    Pathname of the directory. </param>
        ///
        /// ### <returns>   Information describing the application. </returns>

        public void Normalize(System.IO.DirectoryInfo directory)
        {
            if (this.WorkspacePath != null && !Path.IsPathRooted(this.WorkspacePath))
            {
                this.WorkspacePath = Path.GetFullPath(Path.Combine(directory.FullName, this.WorkspacePath));
            }

            if (this.ServicesProjectPath != null && !Path.IsPathRooted(this.ServicesProjectPath))
            {
                this.ServicesProjectPath = Path.GetFullPath(Path.Combine(directory.FullName, this.ServicesProjectPath));
            }

            if (this.EntitiesProjectPath != null && !Path.IsPathRooted(this.EntitiesProjectPath))
            {
                this.EntitiesProjectPath = Path.GetFullPath(Path.Combine(directory.FullName, this.EntitiesProjectPath));
            }

            if (this.WebProjectPath != null && !Path.IsPathRooted(this.WebProjectPath))
            {
                this.WebProjectPath = Path.GetFullPath(Path.Combine(directory.FullName, this.WebProjectPath));
            }

            if (this.WebFrontEndRootPath != null && !Path.IsPathRooted(this.WebFrontEndRootPath))
            {
                this.WebFrontEndRootPath = Path.GetFullPath(Path.Combine(directory.FullName, this.WebFrontEndRootPath));
            }

            if (this.PackageCachePath != null)
            {
                this.PackageCachePath = Path.GetFullPath(Environment.ExpandEnvironmentVariables(this.PackageCachePath));
            }
        }

        /// <summary>   Loads. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="hydraJsonFile">    The hydra JSON file to load. </param>
        /// <param name="webFrontEndRoot">  (Optional) The web front end root. </param>
        ///
        /// <returns>   A ConfigObject. </returns>

        public static ConfigObject Load(string hydraJsonFile, string webFrontEndRoot = null)
        {
            using (var reader = System.IO.File.OpenText(hydraJsonFile))
            {
                var configObject = JsonExtensions.ReadJson<ConfigObject>(reader);
                var serializer = new JsonSerializer();
                var namingStrategy = new CamelCaseNamingStrategy();
                string normalizeRoot;

                serializer.Converters.Add(new Utils.KeyValuePairConverter());
                serializer.Converters.Add(new StringEnumConverter());
                serializer.NullValueHandling = NullValueHandling.Ignore;
                serializer.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                serializer.Formatting = Formatting.Indented;

                serializer.ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = namingStrategy
                };

                if (configObject.GeneratorOptions != null && configObject.GeneratorOptions is JObject jOptions)
                {
                    if (configObject.GeneratorOptions.defaultGeneratorOptions is JObject jDefaultOptions)
                    {
                        configObject.GeneratorOptions = jDefaultOptions.ToObject<DefaultGeneratorOptions>(serializer);
                    }
                }    

                if (webFrontEndRoot == null)
                {
                    normalizeRoot = Path.GetDirectoryName(hydraJsonFile);
                }
                else
                {
                    normalizeRoot = webFrontEndRoot;
                }

                if (configObject.AppName.IsNullOrEmpty())
                {
                    configObject.Normalize(new DirectoryInfo(Path.Combine(normalizeRoot, "Unnamed")));
                }
                else
                {
                    configObject.Normalize(new DirectoryInfo(Path.Combine(normalizeRoot, configObject.AppName)));
                }

                return configObject;
            }
        }

        /// <summary>   Saves this.  </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/15/2021. </remarks>

        public void Save(string hydraJsonFile)
        {
            ConfigObject configObject;
            var rootDirectory = Path.GetDirectoryName(hydraJsonFile + @"\" + this.AppName);

            if (File.Exists(hydraJsonFile))
            {
                using (var reader = System.IO.File.OpenText(hydraJsonFile))
                {
                    configObject = JsonExtensions.ReadJson<ConfigObject>(reader);
                }

                using (var stream = System.IO.File.OpenWrite(hydraJsonFile))
                {
                    stream.SetLength(0);

                    using (var writer = new StreamWriter(stream))
                    {
                        foreach (var pair in this.GetPublicPropertyValues())
                        {
                            var property = pair.Key;
                            var value = pair.Value;

                            if (value != null)
                            {
                                if (property == "PackageCachePath")
                                {
                                    var path = (string)value;
                                    var appData = Environment.ExpandEnvironmentVariables("%APPDATA%");

                                    if (path.StartsWith(appData))
                                    {
                                        value = path.Replace(appData, "%APPDATA%");
                                    }
                                }
                                else if (property.EndsWith("RootPath"))
                                {
                                    var directory = new DirectoryInfo((string)value);

                                    value = directory.GetRelativePath(rootDirectory);
                                }
                                else if (property.EndsWith("Path"))
                                {
                                    var file = new FileInfo((string)value);

                                    value = file.GetRelativePath(rootDirectory);
                                }

                                if (!property.IsOneOf("GeneratorOptions"))
                                {
                                    configObject.SetPropertyValue(property, value);
                                }
                            }
                        }

                        writer.WriteJson(JsonExtensions.ToJsonText(configObject, Newtonsoft.Json.Formatting.Indented, new CamelCaseNamingStrategy()));
                    }
                }
            }
            else
            {
                configObject = this;

                using (var stream = System.IO.File.OpenWrite(hydraJsonFile))
                {
                    stream.SetLength(0);

                    using (var writer = new StreamWriter(stream))
                    {
                        writer.WriteJson(JsonExtensions.ToJsonText(configObject, Newtonsoft.Json.Formatting.Indented, new CamelCaseNamingStrategy()));
                    }
                }
            }
        }
    }
}