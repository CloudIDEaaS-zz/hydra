// file:	NpmNodeModules.cs
//
// summary:	Implements the npm node modules class

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
    /// <summary>   A npm node modules. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 1/19/2021. </remarks>

    public class NpmNodeModules
    {
        /// <summary>   Gets the full pathname of the node modules file. </summary>
        ///
        /// <value> The full pathname of the node modules file. </value>

        public string NodeModulesPath { get; }

        /// <summary>   Gets the packages. </summary>
        ///
        /// <value> The packages. </value>

        public List<NpmPackage> Packages { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/19/2021. </remarks>
        ///
        /// <param name="nodeModulesPath">  Full pathname of the node modules file. </param>

        public NpmNodeModules(string nodeModulesPath)
        {
            var pathDirectory = new DirectoryInfo(nodeModulesPath);

            if (!pathDirectory.Exists)
            {
                pathDirectory.Create();
            }

            this.NodeModulesPath = nodeModulesPath;
            this.Packages = new List<NpmPackage>();

            EnumerateDirectories(pathDirectory);
        }

        /// <summary>   Enumerate directories. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/19/2021. </remarks>
        ///
        /// <param name="pathDirectory">    Pathname of the path directory. </param>

        private void EnumerateDirectories(DirectoryInfo pathDirectory)
        {
            foreach (var subDirectory in pathDirectory.GetDirectories())
            {
                if (subDirectory.Name.StartsWith("@"))
                {
                    EnumerateDirectories(subDirectory);
                }

                if (NpmPackage.HasPackage(subDirectory.FullName))
                {
                    this.Packages.Add(new NpmPackage(subDirectory.FullName));
                }
            }
        }

        /// <summary>   Updates this.  </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/19/2021. </remarks>

        public void Update()
        {
            var pathDirectory = new DirectoryInfo(this.NodeModulesPath);

            foreach (var subDirectory in pathDirectory.GetDirectories().Where(d => !this.Packages.Any(p => p.PackagePath.AsCaseless() != d.FullName)))
            {
                if (subDirectory.Name.StartsWith("@"))
                {
                    EnumerateDirectories(subDirectory);
                }

                if (NpmPackage.HasPackage(subDirectory.FullName))
                {
                    this.Packages.Add(new NpmPackage(subDirectory.FullName));
                }
            }
        }

        /// <summary>   Gets the loaded packages. </summary>
        ///
        /// <value> The loaded packages. </value>

        public IEnumerable<NpmPackage> LoadedPackages
        {
            get
            {
                this.Update();

                foreach (var package in this.Packages)
                {
                    if (!package.IsLoaded)
                    {
                        package.Load();
                    }

                    yield return package;
                }
            }
        }
    }
}
