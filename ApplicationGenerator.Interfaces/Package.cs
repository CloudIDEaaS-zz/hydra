using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Utils;

namespace AbstraX
{
    public abstract class Package
    {
        public Dictionary<string, List<string>> PackageModules { get; protected set; }
        public virtual string[] PackageInstalls { get; protected set; }
        public virtual string[] PackageDevInstalls { get; protected set; }

        public Package()
        {
            this.PackageModules = new Dictionary<string, List<string>>();
        }

        public virtual void ReadPackage()
        {
            var packageAttribute = this.GetType().GetCustomAttribute<PackageAttribute>();
            var packageConfigPath = packageAttribute.PackageConfigPath;
            var appGenPackagePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), packageConfigPath);
            GenPackage genPackage;

            using (var reader = File.OpenText(appGenPackagePath))
            {
                genPackage = JsonExtensions.ReadJson<GenPackage>(reader);
            }

            this.PackageInstalls = genPackage.dependencies;
            this.PackageDevInstalls = genPackage.devDependencies;
        }

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

            using (var stream = File.OpenWrite(appGenPackagePath))
            {
                JsonExtensions.WriteJson(stream, genPackage.ToJson());
            }

            this.PackageInstalls = genPackage.dependencies;
            this.PackageDevInstalls = genPackage.devDependencies;
        }

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

        public IEnumerable<ModuleImportDeclaration> GetDeclarations()
        {
            foreach (var pair in this.PackageModules)
            {
                yield return new ModuleImportDeclaration(pair.Key, pair.Value.ToArray());
            }
        }
    }
}
