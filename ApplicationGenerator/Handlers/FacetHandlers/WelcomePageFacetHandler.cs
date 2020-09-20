using AbstraX.Angular;
using AbstraX.DataAnnotations;
using AbstraX.FolderStructure;
using AbstraX.Generators.Pages.WelcomePage;
using AbstraX.Handlers.FacetHandlers;
using AbstraX.ServerInterfaces;
using EntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.FacetHandlers
{
    [FacetHandler(typeof(UIAttribute), UIKindGuids.WelcomePage, ModuleImports.IONIC_ANGULAR_BASIC_PAGE_IMPORTS)]
    public class WelcomePageFacetHandler : BasePageFacetHandler
    {
        public override bool Process(IBase baseObject, Facet facet, IGeneratorConfiguration generatorConfiguration)
        {
            var uiAttribute = (UIAttribute)facet.Attribute;
            var name = baseObject.GetNavigationName(UIKind.WelcomePage);
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

            WelcomePageGenerator.GeneratePage(baseObject, pagesPath, name, generatorConfiguration, module, imports, loadKind);

            return true;
        }
    }
}
