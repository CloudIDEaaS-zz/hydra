using AbstraX.Angular;
using AbstraX.DataAnnotations;
using AbstraX.FolderStructure;
using AbstraX.Generators.Pages.BlankPage;
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
    [FacetHandler(typeof(UIAttribute), UIKindGuids.BlankPage, ModuleImports.IONIC_ANGULAR_BASIC_PAGE_IMPORTS)]
    public class BlankPageFacetHandler : BasePageFacetHandler
    {
        public override bool Process(IBase baseObject, Facet facet, IGeneratorConfiguration generatorConfiguration)
        {
            var uiAttribute = (UIAttribute)facet.Attribute;
            var name = baseObject.GetNavigationName(UIKind.BlankPage);
            var parentObject = (IParentBase)baseObject;
            var pagesPath = generatorConfiguration.ApplicationFolderHierarchy[IonicFileSystemType.Pages];
            var pagesFolder = (Folder)generatorConfiguration.FileSystem[pagesPath];
            var loadKind = uiAttribute.UILoadKind;
            IModuleAssembly module;
            IEnumerable<ModuleImportDeclaration> imports;

            if (uiAttribute.UILoadKind == UILoadKind.RootPage)
            {
                this.Raise<ApplicationFacetHandler>();
            }

            imports = generatorConfiguration.CreateImports(this, baseObject, pagesFolder, false, 1);

            module = generatorConfiguration.PushModuleAssembly<AngularModule>(name);
            module.UILoadKind = uiAttribute.UILoadKind;

            BlankPageGenerator.GeneratePage(baseObject, pagesPath, name, generatorConfiguration, module, imports, loadKind);

            return true;
        }
    }
}
