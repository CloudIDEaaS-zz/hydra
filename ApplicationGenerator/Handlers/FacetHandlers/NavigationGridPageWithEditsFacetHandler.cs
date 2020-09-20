using AbstraX.Angular;
using AbstraX.DataAnnotations;
using AbstraX.FolderStructure;
using AbstraX.Generators.Pages.NavigationGridPage;
using AbstraX.ServerInterfaces;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace AbstraX.Handlers.FacetHandlers
{
    [FacetHandler(typeof(UIAttribute), UIKindGuids.NavigationGridPageWithEdits, ModuleImports.IONIC_ANGULAR_GRID_PAGE_IMPORTS)]
    public class NavigationGridPageWithEditsFacetHandler : BasePageFacetHandler
    {
        public override bool Process(IBase baseObject, Facet facet, IGeneratorConfiguration generatorConfiguration)
        {
            var uiAttribute = (UIAttribute)facet.Attribute;
            var name = baseObject.GetNavigationName();
            var parentObject = (IParentBase)baseObject;
            var mainObject = parentObject.ChildElements.Single();
            var managedLists = new List<ManagedList>();
            var gridColumns = new List<GridColumn>();
            var pagesPath = generatorConfiguration.ApplicationFolderHierarchy[IonicFileSystemType.Pages];
            var pagesFolder = (Folder) generatorConfiguration.FileSystem[pagesPath];
            var featureKind = uiAttribute.GetFeatureKind();
            var isComponent = featureKind.IsComponent();
            IModuleAssembly module;
            IEnumerable<ModuleImportDeclaration> imports;

            if (mainObject is IParentBase)
            {
                foreach (var childObject in mainObject.GetFollowingNavigationChildren(generatorConfiguration.PartsAliasResolver))
                {
                    managedLists.Add(new ManagedList(childObject, generatorConfiguration));
                }
            }

            foreach (var childObject in mainObject.GetGridColumns(generatorConfiguration.PartsAliasResolver))
            {
                gridColumns.Add(new GridColumn(childObject, generatorConfiguration));
            }

            if (uiAttribute.UILoadKind == UILoadKind.RootPage)
            {
                this.Raise<ApplicationFacetHandler>();
            }

            imports = generatorConfiguration.CreateImports(this, baseObject, pagesFolder, false, 1);

            module = generatorConfiguration.PushModuleAssembly<AngularModule>(name);
            module.UILoadKind = uiAttribute.UILoadKind;
            module.IsComponent = isComponent;

            NavigationGridPageGenerator.GeneratePage(baseObject, pagesPath, name, generatorConfiguration, module, imports, module, managedLists, gridColumns, isComponent);

            return true;
        }
    }
}
