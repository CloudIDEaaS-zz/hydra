using AbstraX.Angular;
using AbstraX.DataAnnotations;
using AbstraX.FolderStructure;
using AbstraX.Generators.Pages.TabPage;
using AbstraX.ServerInterfaces;
using EntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Handlers.FacetHandlers
{
    [FacetHandler(typeof(UIAttribute), UIKindGuids.TabsPage, ModuleImports.IONIC_ANGULAR_BASIC_PAGE_IMPORTS)]
    public class TabsPageFacetHandler : BasePageFacetHandler
    {
        public override bool Process(IBase baseObject, Facet facet, IGeneratorConfiguration generatorConfiguration)
        {
            var uiAttribute = (UIAttribute)facet.Attribute;
            var name = baseObject.GetNavigationName(UIKind.TabsPage);
            var parentObject = (IParentBase)baseObject;
            var tabs = new List<Tab>();
            var pagesPath = generatorConfiguration.ApplicationFolderHierarchy[IonicFileSystemType.Pages];
            var pagesFolder = (Folder) generatorConfiguration.FileSystem[pagesPath];
            var loadKind = uiAttribute.UILoadKind;
            var kind = uiAttribute.UIKind;
            IModuleAssembly module;
            IEnumerable<ModuleImportDeclaration> imports;

            foreach (var childObject in parentObject.GetFollowingChildren(generatorConfiguration.PartsAliasResolver))
            {
                tabs.Add(new Tab(childObject, generatorConfiguration));
            }

            if (uiAttribute.UILoadKind == UILoadKind.HomePage)
            {
                this.Raise<ApplicationFacetHandler>();
            }

            imports = generatorConfiguration.CreateImports(this, baseObject, pagesFolder, false, 1);

            module = generatorConfiguration.PushModuleAssembly<AngularModule>(name);

            module.UILoadKind = loadKind;
            module.UIKind = kind;
            module.UIHierarchyPath = uiAttribute.UIHierarchyPath;

            if (loadKind == UILoadKind.MainPage)
            {
                module.BaseRoute = "/app/main";
            }

            TabPageGenerator.GeneratePage(baseObject, pagesPath, name, generatorConfiguration, module, imports, tabs, loadKind, kind);

            return true;
        }
    }
}
