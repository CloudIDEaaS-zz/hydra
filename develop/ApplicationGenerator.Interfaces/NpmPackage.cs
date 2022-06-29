// file:	NpmPackage.cs
//
// summary:	Implements the npm package class

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX
{
    /// <summary>   A npm package. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>

    [DebuggerDisplay(" { Info }")]
    public class NpmPackage
    {
        /// <summary>   Gets the full pathname of the package file. </summary>
        ///
        /// <value> The full pathname of the package file. </value>

        public string PackagePath { get; }

        /// <summary>   Gets or sets the name. </summary>
        ///
        /// <value> The name. </value>

        public string Name { get; private set; }

        /// <summary>   Gets or sets the version. </summary>
        ///
        /// <value> The version. </value>

        public NpmVersion Version { get; private set; }

        /// <summary>   Gets or sets a value indicating whether this  is loaded. </summary>
        ///
        /// <value> True if this  is loaded, false if not. </value>

        public bool IsLoaded { get; private set; }

        /// <summary>   Gets or sets the dependencies. </summary>
        ///
        /// <value> The dependencies. </value>

        public Dictionary<string, string> Dependencies { get; private set; }

        /// <summary>   Gets or sets the development dependencies. </summary>
        ///
        /// <value> The development dependencies. </value>

        public Dictionary<string, string> DevDependencies { get; private set; }

        /// <summary>   Gets or sets the peer dependencies. </summary>
        ///
        /// <value> The peer dependencies. </value>

        public Dictionary<string, string> PeerDependencies { get; private set; }

        /// <summary>   Gets or sets the description. </summary>
        ///
        /// <value> The description. </value>

        public string Description { get; set; }

        /// <summary>   Gets or sets the name of the display. </summary>
        ///
        /// <value> The name of the display. </value>

        public string DisplayName { get; set; }

        /// <summary>   Gets or sets the contributors. </summary>
        ///
        /// <value> The contributors. </value>

        public object Contributors { get; set; }

        /// <summary>   Gets or sets the author. </summary>
        ///
        /// <value> The author. </value>

        public object Author { get; set; }

        /// <summary>   Gets or sets the module. </summary>
        ///
        /// <value> The module. </value>

        public string Module { get; set; }

        /// <summary>   Gets or sets the es 2015. </summary>
        ///
        /// <value> The es 2015. </value>

        public string Es2015 { get; set; }

        /// <summary>   Gets or sets the es 2017. </summary>
        ///
        /// <value> The es 2017. </value>

        public string Es2017 { get; set; }

        /// <summary>   Gets or sets the license. </summary>
        ///
        /// <value> The license. </value>

        public string License { get; set; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="path"> Full pathname of the file. </param>

        public NpmPackage(string path)
        {
            var pathDirectory = new DirectoryInfo(path);

            Debug.Assert(pathDirectory.Exists);

            this.PackagePath = path;
        }

        /// <summary>   Query if 'path' has package. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="path"> Full pathname of the file. </param>
        ///
        /// <returns>   True if package, false if not. </returns>

        public static bool HasPackage(string path)
        {
            var pathDirectory = new DirectoryInfo(path);

            if (pathDirectory.Exists)
            {
                var fileInfoJson = new FileInfo(Path.Combine(path, "package.json"));

                return fileInfoJson.Exists;
            }

            return false;
        }

        /// <summary>   Loads this. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>

        public void Load()
        {
            var fileInfoJson = new FileInfo(Path.Combine(this.PackagePath, "package.json"));

            if (fileInfoJson.Exists)
            {
                using (var readerPackage = fileInfoJson.OpenText())
                {
                    var packageJson = JsonExtensions.ReadJson<PackageJson>(readerPackage);

                    this.Name = packageJson.Name;
                    this.Version = new NpmVersion(packageJson.Version);
                    this.Dependencies = packageJson.Dependencies;
                    this.DevDependencies = packageJson.DevDependencies;
                    this.PeerDependencies = packageJson.PeerDependencies;
                    this.Description = packageJson.Description;
                    this.DisplayName = packageJson.DisplayName;
                    this.Contributors = packageJson.Contributors;
                    this.Author = packageJson.Author;
                    this.Module = packageJson.Module;
                    this.Es2015 = packageJson.Es2015;
                    this.Es2017 = packageJson.Es2017;
                    this.License = packageJson.License;
                }
            }

            this.IsLoaded = true;
        }

        /// <summary>   Gets the information. </summary>
        ///
        /// <value> The information. </value>

        public string Info
        {
            get
            {
                if (this.IsLoaded)
                {
                    return string.Format("{0}@{1}",
                        this.Name,
                        this.Version.Version
                    );
                }
                else
                {
                    return string.Format("PackagePath: '{0}', IsLoaded: false", 
                        this.PackagePath
                    );
                }
            }
        }
    }
}
