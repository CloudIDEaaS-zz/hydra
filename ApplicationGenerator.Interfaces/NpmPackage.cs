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
    [DebuggerDisplay(" { Info }")]
    public class NpmPackage
    {
        public string PackagePath { get; }
        public string Name { get; private set; }
        public NpmVersion Version { get; private set; }
        public bool IsLoaded { get; private set; }
        public Dictionary<string, string> Dependencies { get; private set; }
        public Dictionary<string, string> DevDependencies { get; private set; }
        public Dictionary<string, string> PeerDependencies { get; private set; }

        public NpmPackage(string path)
        {
            var pathDirectory = new DirectoryInfo(path);

            Debug.Assert(pathDirectory.Exists);

            this.PackagePath = path;
        }

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
                }
            }

            this.IsLoaded = true;
        }

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
