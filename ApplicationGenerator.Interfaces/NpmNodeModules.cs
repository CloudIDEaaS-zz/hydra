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
    public class NpmNodeModules
    {
        public string NodeModulesPath { get; }
        public List<NpmPackage> Packages { get; }

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
