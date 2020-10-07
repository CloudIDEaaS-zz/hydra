// file:	ConfigObject.cs
//
// summary:	Implements the configuration object class

using System;
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

        public string OrganizationName { get; set; }

        /// <summary>   Gets or sets the full pathname of the solution file. </summary>
        ///
        /// <value> The full pathname of the solution file. </value>

        public string SolutionPath { get; set; }

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

        /// <summary>   Gets or sets the full pathname of the package cache file. </summary>
        ///
        /// <value> The full pathname of the package cache file. </value>

        public string PackageCachePath { get; set; }

        /// <summary>   Gets or sets information describing the application. </summary>
        ///
        /// <value> Information describing the application. </value>

        public string AppDescription { get; internal set; }

        /// <summary>   Normalizes the given directory. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="directory">    Pathname of the directory. </param>

        public void Normalize(System.IO.DirectoryInfo directory)
        {
            this.SolutionPath = Path.GetFullPath(Path.Combine(directory.FullName, this.SolutionPath));
            this.ServicesProjectPath = Path.GetFullPath(Path.Combine(directory.FullName, this.ServicesProjectPath));
            this.EntitiesProjectPath = Path.GetFullPath(Path.Combine(directory.FullName, this.EntitiesProjectPath));
            this.WebProjectPath = Path.GetFullPath(Path.Combine(directory.FullName, this.WebProjectPath));
            this.PackageCachePath = Path.GetFullPath(Environment.ExpandEnvironmentVariables(this.PackageCachePath));
        }

        /// <summary>   Loads. </summary>
        ///
        /// <remarks>   Ken, 10/1/2020. </remarks>
        ///
        /// <param name="hydraJsonFile">    The hydra JSON file to load. </param>
        ///
        /// <returns>   A ConfigObject. </returns>

        public static ConfigObject Load(string hydraJsonFile)
        {
            using (var reader = System.IO.File.OpenText(hydraJsonFile))
            {
                var configObject = JsonExtensions.ReadJson<ConfigObject>(reader);

                configObject.Normalize(new DirectoryInfo(Path.GetDirectoryName(hydraJsonFile)));

                return configObject;
            }
        }
    }
}