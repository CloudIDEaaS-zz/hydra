using AbstraX.Angular;
using AbstraX.DataAnnotations;
using AbstraX.FolderStructure;
using AbstraX.Generators.Client.Config;
using AbstraX.Generators.Client.Theme;
using AbstraX.Generators.Modules.AppModule;
using AbstraX.ServerInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Handlers.FacetHandlers
{
    [AngularModuleKindHandler(ModuleKind.AngularModule, DefinitionKind.StaticContainer, UIFeatureKind.Custom)]
    public class ThemeModuleKindHandler : IModuleKindHandler
    {
        public float Priority => 1.0f;
        public event ProcessFacetsHandler ProcessFacets;

        public ThemeModuleKindHandler()
        {
        }

        public bool Process(Enum moduleKind, IModuleObject moduleObject, Folder folder, IGeneratorConfiguration generatorConfiguration)
        {
            var theme = generatorConfiguration.ApplicationFolderHierarchy[IonicFileSystemType.Theme];
            var src = generatorConfiguration.ApplicationFolderHierarchy[IonicFileSystemType.Src];
            var assetImgs = generatorConfiguration.ApplicationFolderHierarchy[IonicFileSystemType.AssetsImgs];
            var assetData = generatorConfiguration.ApplicationFolderHierarchy[IonicFileSystemType.AssetsData];
            var themeFolder = (Folder)generatorConfiguration.FileSystem[theme];
            var srcFolder = (Folder)generatorConfiguration.FileSystem[src];
            var assetImgsFolder = (Folder)generatorConfiguration.FileSystem[assetImgs];
            var assetDataFolder = (Folder)generatorConfiguration.FileSystem[assetData];

            ThemeGenerator.GenerateTheme(srcFolder.fullPath, themeFolder.fullPath, assetImgsFolder.fullPath, assetDataFolder.fullPath, generatorConfiguration);

            return true;
        }

        public void AddRange(IGeneratorConfiguration generatorConfiguration, IModuleAssembly moduleAssembly, List<IModuleOrAssembly> modulesOrAssemblies, IEnumerable<IModuleOrAssembly> addModulesOrAssemblies, ModuleAddType moduleAddType)
        {
            modulesOrAssemblies.AddRange(addModulesOrAssemblies);
        }
    }
}
