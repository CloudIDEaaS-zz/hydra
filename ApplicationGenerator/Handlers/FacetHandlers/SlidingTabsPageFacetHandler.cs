using AbstraX.Angular;
using AbstraX.DataAnnotations;
using AbstraX.FolderStructure;
using AbstraX.Generators.Pages.SlidingTabPage;
using AbstraX.Handlers.FacetHandlers;
using AbstraX.ServerInterfaces;
using EntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.FacetHandlers
{
    [FacetHandler(typeof(UIAttribute), UIKindGuids.SlidingTabsPage, ModuleImports.IONIC_ANGULAR_SUPERTABS_PAGE_IMPORTS)]
    public class SlidingTabsPageFacetHandler : BasePageFacetHandler
    {
        public override bool Process(IBase baseObject, Facet facet, IGeneratorConfiguration generatorConfiguration)
        {
            var uiAttribute = (UIAttribute)facet.Attribute;
            var name = baseObject.GetNavigationName();
            var parentObject = (IParentBase)baseObject;
            var pagesPath = generatorConfiguration.ApplicationFolderHierarchy[IonicFileSystemType.Pages];
            var pagesFolder = (Folder)generatorConfiguration.FileSystem[pagesPath];
            var imports = generatorConfiguration.CreateImports(this, baseObject, pagesFolder, false, 1);
            var slidingTabs = new List<SlidingTab>();
            var featureKind = uiAttribute.GetFeatureKind();
            var isComponent = featureKind.IsComponent();
            IModuleAssembly module;

            foreach (var childObject in parentObject.GetFollowingChildren(generatorConfiguration.PartsAliasResolver, baseObject.Kind != DefinitionKind.StaticContainer))
            {
                slidingTabs.Add(new SlidingTab(childObject, generatorConfiguration));
            }

            module = generatorConfiguration.PushModuleAssembly<AngularModule>(name);
            module.UILoadKind = uiAttribute.UILoadKind;
            module.IsComponent = isComponent;

            SlidingTabsPageGenerator.GeneratePage(baseObject, pagesPath, name, generatorConfiguration, module, imports, slidingTabs, isComponent);

            return true;
        }
    }
}
