using AbstraX.FolderStructure;
using AbstraX.ServerInterfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Angular
{
    public class AngularModuleAssemblyProperties : IModuleAssemblyProperties
    {
        public IEnumerable<ModuleImportDeclaration> Imports { get; set; }
        public List<ESModule> Exports { get; private set; }
        public List<IDeclarable> Declarations { get; private set; }
        public List<Provider> Providers { get; private set; }
        public List<ESModule> Modules { get; private set; }
        public IBase BaseObject { get; }
        public IGeneratorConfiguration Configuration { get; set; }

        public AngularModuleAssemblyProperties(IBase baseObject)
        {
            this.Exports = new List<ESModule>();
            this.Declarations = new List<IDeclarable>();
            this.Providers = new List<Provider>();
            this.Modules = new List<ESModule>();
            this.BaseObject = baseObject;
        }

        public AngularModuleAssemblyProperties(IBase baseObject, IEnumerable<ModuleImportDeclaration> imports) : this(baseObject)
        {
            this.Imports = imports;
        }

        public void UpdateModuleAssembly(IModuleAssembly moduleAssembly)
        {
            var angularModule = (AngularModule)moduleAssembly;
            var count = 0;

            if (this.BaseObject == null)
            {
                DebugUtils.Break();
            }

            angularModule.BaseObject = this.BaseObject;

            ObjectExtensions.MultiAct<IList>(l =>
            {
                count += l.Count;

            }, this.Exports, this.Providers, this.Declarations);

            if (count == 0)
            {
                DebugUtils.Break();
            }
            
            foreach (var export in this.Exports)
            {
                angularModule.AddBaseExport(this.BaseObject, export);
            }

            foreach (var provider in this.Providers)
            {
                List<Provider> providers = null;

                provider.BaseObject = this.BaseObject;

                if (this.Configuration.KeyValuePairs.ContainsKey("Providers"))
                {
                    providers = (List<Provider>) this.Configuration.KeyValuePairs["Providers"];
                }
                else
                {
                    providers = new List<Provider>();

                    this.Configuration.KeyValuePairs.Add("Providers", providers);
                }

                if (!providers.Contains(provider))
                {
                    providers.Add(provider);
                }
            }

            angularModule.Declarations.AddRange(this.Declarations);
            angularModule.Exports.AddRange(this.Exports);
        }

        public void AddDefaultFile(File file)
        {
            this.Exports.AddFile(file);
            this.Providers.AddFile(file);
        }

        public void Clear()
        {
            this.Imports = null;
            this.Exports = new List<ESModule>();
            this.Declarations = new List<IDeclarable>();
            this.Providers = new List<Provider>();
        }
    }
}
