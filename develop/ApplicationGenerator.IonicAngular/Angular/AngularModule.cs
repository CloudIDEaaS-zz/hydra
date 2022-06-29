using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstraX.Angular.Routes;
using AbstraX.DataAnnotations;
using AbstraX.FolderStructure;
using AbstraX.ServerInterfaces;
using Utils;

namespace AbstraX.Angular
{
    public class AngularModule : Module, IModuleAssembly
    {
        public override string Name { get; set; }
        public string RoutingNameExpression { get; set; }
        public List<IDeclarable> Declarations { get; private set; }
        public List<Module> Imports { get; private set; }
        public List<Route> Routes { get; private set; }
        public Dictionary<string, List<Module>> BaseExports { get; private set; }
        public List<Module> Exports { get; private set; }
        public List<ESModule> Providers { get; private set; }
        public ESModule EntryComponent { get; set; }
        public List<Module> ExportedComponents { get; set; }
        public List<IRouteGuard> RouteGuards { get; set; }
        public List<Facet> RuntimeFacets { get; }
        public int Indentation { get; set; }
        public bool Popped { get; set; }
        public bool IsComponent { get; set; }
        public bool IsApp { get; set; }
        public string BaseRoute { get; set; }

        private File forChildFile;
        private UILoadKind componentLoadKind;
        private UIKind componentUiKind;
        private string componentUiHierarchyPath;

        public AngularModule()
        {
            this.Declarations = new List<IDeclarable>();
            this.Imports = new List<Module>();
            this.BaseExports = new Dictionary<string, List<Module>>();
            this.Exports = new List<Module>();
            this.Providers = new List<ESModule>();
            this.Routes = new List<Route>();
            this.RouteGuards = new List<IRouteGuard>();
            this.RuntimeFacets = new List<Facet>();
        }

        public AngularModule(string name) : base(name)
        {
            this.Imports = new List<Module>();
            this.BaseExports = new Dictionary<string, List<Module>>();
            this.Exports = new List<Module>();
            this.Providers = new List<ESModule>();
            this.Routes = new List<Route>();
            this.RouteGuards = new List<IRouteGuard>();
            this.RuntimeFacets = new List<Facet>();
        }

        public override File File
        {
            get => base.File;
            set => base.File = value;
        }

        public File ForChildFile
        {
            get => forChildFile;
            set => forChildFile = value;
        }

        public UILoadKind UILoadKind
        {
            get => componentLoadKind;
            set => componentLoadKind = value;
        }

        public UIKind UIKind
        {
            get => componentUiKind;
            set => componentUiKind = value;
        }
      
        public string UIHierarchyPath
        {
            get => componentUiHierarchyPath;
            set => componentUiHierarchyPath = value;
        }

        public IEnumerable<Module> BaseExportValues
        {
            get
            {
                return this.BaseExports.Values.SelectMany(e => e);
            }
        }

        public string DebugInfo
        {
            get
            {
                return this.Name;
            }
        }

        public void AddExport(Module module)
        {
            this.Exports.Add(module);
        }

        public void AddBaseExport(IBase baseObject, Module module)
        {
            this.BaseExports.AddToDictionaryListCreateIfNotExist(baseObject.Name, module);
        }

        public Module FindExport(string name)
        {
            return this.Exports.SingleOrDefault(n => n.Name == name);
        }

        public IEnumerable<Module> GetExports(string attribute)
        {
            return this.Exports.Where(m => m.Attributes.Any(a => a == attribute));
        }

        public IEnumerable<Module> GetExports(IBase baseObject)
        {
            return this.BaseExports[baseObject.Name];
        }

        public bool HasExports(IBase baseObject, IModuleAssembly moduleAssembly = null)
        {
            if (this.RuntimeFacets.Count > 0)
            {
                if (moduleAssembly != null)
                {
                    if (moduleAssembly.BaseRoute != null)
                    {
                        var uiAttributes = this.RuntimeFacets.Select(f => f.Attribute).OfType<UIAttribute>();
                        var hasExports = uiAttributes.Any(a => a.UIHierarchyPath.StartsWith(moduleAssembly.BaseRoute));

                        return hasExports;
                    }
                    else if (moduleAssembly.Name == this.Name)
                    {
                        return this.BaseExports.ContainsKey(baseObject.Name);
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return this.BaseExports.ContainsKey(baseObject.Name);
                }
            }
            else
            {
                return this.BaseExports.ContainsKey(baseObject.Name);
            }
        }

        public UIAttribute GetUIAttribute(Folder folder)
        {
            var uiComponentAttributes = this.BaseObject.GetFacetAttributes<UIAttribute>();
            UIAttribute uiComponentAttribute;

            if (uiComponentAttributes.Count() > 1)
            {
                uiComponentAttribute = uiComponentAttributes.SingleOrDefault(a => a.UILoadKind == this.UILoadKind && a.UIHierarchyPath == this.UIHierarchyPath);
            }
            else
            {
                uiComponentAttribute = uiComponentAttributes.SingleOrDefault();
            }

            if (uiComponentAttribute == null)
            {
                uiComponentAttribute = this.RuntimeFacets.Select(f => f.Attribute).OfType<UIAttribute>().SingleOrDefault(a => a.UILoadKind == this.UILoadKind && a.UIHierarchyPath == this.UIHierarchyPath);
            }

            return uiComponentAttribute;
        }

        public bool ValidateModuleName(string name, out string error)
        {
            error = null;

            if (this.Name != "AppModule")
            {
                if (this.ExportedComponents == null || this.ExportedComponents.Count == 0)
                {
                    error = $"Module named { this.Name } has no exported components";
                    return false;
                }
                else
                {
                    if (!this.ExportedComponents.Any(c => c.Name.StartsWith(name, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        error = $"Module named '{ this.Name }' has no matching exported components matching name '{ name }'";
                        return false;
                    }
                }

                if (this.ForChildFile == null)
                {
                    error = $"Module named { this.Name } has no component file";
                    return false;
                }
                else
                {
                    var directory = this.ForChildFile.SystemLocalFile.Directory;
                    var directoryName = directory.Name.RemoveText("-");
                    var directoryNamePlural = directoryName.Pluralize();

                    if (name.AsCaseless() != directoryName && name.AsCaseless() != directoryNamePlural)
                    {
                        error = $"Module named '{ this.Name }' has directory '{ directory.FullName }' that does not match name { name }";
                        return false;
                    }
                    else if (!this.ExportedComponents.Any(c => c.Name.StartsWith(directoryName, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        error = $"Module named '{ this.Name }' has directory '{ directory.FullName }' with no matching exported components matching name";
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
