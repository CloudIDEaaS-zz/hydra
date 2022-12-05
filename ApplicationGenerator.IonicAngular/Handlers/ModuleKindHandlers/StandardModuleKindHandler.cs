using AbstraX.Angular;
using AbstraX.DataAnnotations;
using AbstraX.FolderStructure;
using AbstraX.Generators.Modules.StandardModule;
using AbstraX.ServerInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Handlers.FacetHandlers
{
    [AngularModuleKindHandler(ModuleKind.AngularModule, UIFeatureKind.Standard)]
    public class StandardModuleKindHandler : IModuleKindHandler
    {
        public float Priority => 1.0f;
        public event ProcessFacetsHandler ProcessFacets;

        public StandardModuleKindHandler()
        {
        }

        public bool Process(Enum moduleKind, IModuleObject moduleObject, Folder folder, IGeneratorConfiguration generatorConfiguration)
        {
            var angularModule = (AngularModule)moduleObject;
            var baseObject = moduleObject.BaseObject;
            var imports = generatorConfiguration.CreateImports(this, angularModule, folder, true);
            var name = baseObject.GetNavigationName(angularModule.UILoadKind);
            var parentObject = (IParentBase)baseObject;
            List<AngularModule> runtimeModules;

            if (moduleObject.RuntimeFacets.Count == 0)
            {
                if (!moduleObject.ValidateModuleName(name, out string error))
                {
                    throw new Exception(error);
                }

                if (generatorConfiguration.KeyValuePairs.ContainsKey("RuntimeModules") && generatorConfiguration.CurrentPass == GeneratorPass.Files)
                {
                    var runtimeImports = new List<ModuleImportDeclaration>();

                    runtimeModules = (List<AngularModule>)generatorConfiguration.KeyValuePairs["RuntimeModules"];

                    if (runtimeModules.Count > 0)
                    {
                        foreach (var module in runtimeModules)
                        {
                            var moduleAssembly = (IModuleAssembly)module;
                            var import = moduleAssembly.CreateImportDeclaration(folder, 0);

                            runtimeImports.Add(import);
                        }

                        imports = imports.Concat(runtimeImports);

                        generatorConfiguration.KeyValuePairs.Remove("RuntimeModules");
                        name = moduleObject.Name.RemoveEndIfMatches("Module");
                    }
                }
            }
            else
            {
                if (generatorConfiguration.CurrentPass == GeneratorPass.Files)
                {
                    if (generatorConfiguration.KeyValuePairs.ContainsKey("RuntimeModules"))
                    {
                        runtimeModules = (List<AngularModule>)generatorConfiguration.KeyValuePairs["RuntimeModules"];
                    }
                    else
                    {
                        runtimeModules = new List<AngularModule>();

                        generatorConfiguration.KeyValuePairs.Add("RuntimeModules", runtimeModules);
                    }

                    runtimeModules.Add(angularModule);
                }

                name = moduleObject.Name.RemoveEndIfMatches("Module");
            }

            StandardModuleGenerator.GenerateModule(baseObject, angularModule, folder.fullPath, name, generatorConfiguration, imports, angularModule.UILoadKind, angularModule.UIKind);

            return true;
        }

        public void AddRange(IGeneratorConfiguration generatorConfiguration, IModuleAssembly moduleAssembly, List<IModuleOrAssembly> modulesOrAssemblies, IEnumerable<IModuleOrAssembly> addModulesOrAssemblies, ModuleAddType moduleAddType)
        {
            modulesOrAssemblies.AddRange(addModulesOrAssemblies);
        }
    }
}
