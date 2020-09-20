using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace ModuleImportsHelper
{
    public class Helper
    {
        public List<Package> Packages { get; private set; }

        public Helper()
        {
            this.Packages = new List<Package>();
        }

        public void AddPackages(List<object> creationObjects)
        {
            var packageGuids = new Dictionary<object, Guid>();

            foreach (var obj in creationObjects)
            {
                packageGuids.Add(obj, Guid.NewGuid());
            }

            foreach (var pair in packageGuids)
            {
                var guid = pair.Value;
                var obj = pair.Key;
                var properties = obj.GetType().GetProperties();
                string addFrom;
                Package package;

                if (properties.Count() == 1)
                {
                    var property = properties.Single();
                    var packageName = property.Name;
                    var moduleImport = property.GetValue(obj);
                    var handlerIdString = string.Join("-", guid.ToString().Split("-").Take(3));
                    var importPath = moduleImport.GetAnonymousPropertyValue<string>("ImportPath");
                    var inheritsFrom = moduleImport.GetAnonymousPropertyValue<string>("InheritsFrom");
                    var combinedName = moduleImport.GetAnonymousPropertyValue<string>("CombineName");
                    var modules = new List<Module>();
                    List<string> moduleStrings;

                    package = new Package
                    {
                        PackageName = packageName,
                        ImportPath = importPath,
                        InheritsFrom = inheritsFrom,
                        HandlerIdString = handlerIdString,
                        CombinedName = combinedName
                    };

                    addFrom = moduleImport.GetAnonymousPropertyValue<string>("AddFrom");
                    moduleStrings = moduleImport.GetAnonymousPropertyValue<List<string>>("Modules");

                    if (moduleStrings == null)
                    {
                        var moduleGuid = Guid.NewGuid();
                        var moduleIdString = string.Join("-", moduleGuid.ToString().Split("-").Skip(3).Take(2));
                        var module = new Module
                        {
                            ModuleName = "[Default]",
                            ModuleIdString = moduleIdString,
                        };

                        module.Package = package;
                        modules.Add(module);
                    }
                    else
                    {
                        foreach (string moduleName in moduleStrings)
                        {
                            var moduleGuid = Guid.NewGuid();
                            var moduleIdString = string.Join("-", moduleGuid.ToString().Split("-").Skip(3).Take(2));
                            var module = new Module
                            {
                                ModuleName = moduleName,
                                ModuleIdString = moduleIdString,
                            };

                            module.Package = package;
                            modules.Add(module);
                        }
                    }

                    package.Modules = modules;
                }
                else
                {
                    addFrom = obj.GetAnonymousPropertyValue<string>("AddFrom");
                    var combinedName = obj.GetAnonymousPropertyValue<string>("CombineName");

                    package = new Package
                    {
                        CombinedName = combinedName
                    };
                }

                if (addFrom != null)
                {
                    var addFromPackagesNames = addFrom.Split(",").Select(n => n.Trim());
                    var addFromModules = new List<Module>();

                    foreach (var addFromPackageName in addFromPackagesNames)
                    {
                        var addFromPackage = this.Packages.SingleOrDefault(p => p.PackageName == addFromPackageName);

                        if (addFromPackage != null)
                        {
                            if (addFromPackage.AddFromModules != null)
                            {
                                foreach (var addFromModule in addFromPackage.Modules.Concat(addFromPackage.AddFromModules))
                                {
                                    addFromModules.Add(addFromModule);
                                }
                            }
                            else
                            {
                                foreach (var addFromModule in addFromPackage.Modules)
                                {
                                    addFromModules.Add(addFromModule);
                                }
                            }
                        }
                    }

                    package.AddFromModules = addFromModules;
                }

                this.Packages.Add(package);
            }
        }
    }
}
