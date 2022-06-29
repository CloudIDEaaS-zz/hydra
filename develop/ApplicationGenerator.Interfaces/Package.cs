// file:	Package.cs
//
// summary:	Implements the package class

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Utils;

namespace AbstraX
{
    /// <summary>   A package. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>

    public abstract class Package
    {
        /// <summary>   Gets or sets the package modules. </summary>
        ///
        /// <value> The package modules. </value>

        public Dictionary<string, List<string>> PackageModules { get; protected set; }

        /// <summary>   Gets or sets the package installs. </summary>
        ///
        /// <value> The package installs. </value>

        public virtual string[] PackageInstalls { get; protected set; }

        /// <summary>   Gets or sets the package development installs. </summary>
        ///
        /// <value> The package development installs. </value>

        public virtual string[] PackageDevInstalls { get; protected set; }
        /// <summary>   Event queue for all listeners interested in packageRead events. </summary>
        public static event PackageReadEventHandler PackageReadEvent;

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>

        public Package(IImportHandler importHandler)
        {
            this.PackageModules = new Dictionary<string, List<string>>();
        }

        /// <summary>   Reads the package. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>

        public virtual void ReadPackage(IImportHandler importHandler)
        {
            var packageAttribute = this.GetType().GetCustomAttribute<PackageAttribute>();
            var packageConfigPath = packageAttribute.PackageConfigPath;
            var appGenPackagePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), packageConfigPath);
            GenPackage genPackage;

            using (var reader = File.OpenText(appGenPackagePath))
            {
                genPackage = JsonExtensions.ReadJson<GenPackage>(reader);

                PackageReadEvent?.Invoke(this, new PackageReadEventArgs(this, genPackage, importHandler));
            }

            this.PackageInstalls = genPackage.dependencies;
            this.PackageDevInstalls = genPackage.devDependencies;
        }

        /// <summary>   Writes a package. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="rootPath"> Full pathname of the root file. </param>

        public virtual void WritePackage(string rootPath)
        {
            var packageAttribute = this.GetType().GetCustomAttribute<PackageAttribute>();
            var packageConfigPath = packageAttribute.PackageConfigPath;
            var appGenPackagePath = Path.Combine(rootPath, packageConfigPath);
            var genPackage = new
            {
                dependencies = this.PackageInstalls,
                devDependencies = this.PackageDevInstalls
            };

            using (var writer = new StreamWriter(File.OpenWrite(appGenPackagePath)))
            {
                JsonExtensions.WriteJson(writer, genPackage.ToJsonText());
            }

            this.PackageInstalls = genPackage.dependencies;
            this.PackageDevInstalls = genPackage.devDependencies;
        }

        /// <summary>   Adds a module. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="importModule"> An enum constant representing the import module option. </param>

        public virtual void AddModule(Enum importModule)
        {
            var importDeclarationAttribute = importModule.GetModuleImportDeclarationAttribute();

            if (importDeclarationAttribute.Module == null)
            {
                this.PackageModules.AddToDictionaryListCreateIfNotExist(importDeclarationAttribute.ImportPath);
            }
            else
            {
                this.PackageModules.AddToDictionaryListCreateIfNotExist(importDeclarationAttribute.ImportPath, importDeclarationAttribute.Module);
            }
        }

        /// <summary>   Gets the declarations in this collection. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the declarations in this collection.
        /// </returns>

        public IEnumerable<ModuleImportDeclaration> GetDeclarations()
        {
            foreach (var pair in this.PackageModules)
            {
                yield return new ModuleImportDeclaration(pair.Key, pair.Value.ToArray());
            }
        }
    }
}
