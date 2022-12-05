using AbstraX.Angular;
using AbstraX.DataAnnotations;
using AbstraX.FolderStructure;
using AbstraX.Generators.Pages.SupportPage;
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
    [FacetHandler(typeof(UIAttribute), UIKindGuids.SupportPage, ModuleImports.IONIC_ANGULAR_BASIC_PAGE_IMPORTS)]
    public class SupportPageFacetHandler : BasePageFacetHandler
    {
        public override bool Process(IBase baseObject, Facet facet, IGeneratorConfiguration generatorConfiguration)
        {
            var uiAttribute = (UIAttribute)facet.Attribute;
            var name = "Support";
            var parentObject = (IParentBase)baseObject;
            var pagesPath = generatorConfiguration.ApplicationFolderHierarchy[IonicFileSystemType.Pages];
            var pagesFolder = (Folder)generatorConfiguration.FileSystem[pagesPath];
            var imports = generatorConfiguration.CreateImports(this, baseObject, pagesFolder, false, 1);
            IModuleAssembly module;

            imports = generatorConfiguration.CreateImports(this, baseObject, pagesFolder, false, 1);

            module = generatorConfiguration.PushModuleAssembly<AngularModule>(name);

            module.UILoadKind = UILoadKind.Default;
            module.UIKind = UIKind.SupportPage;
            module.UIHierarchyPath = uiAttribute.UIHierarchyPath;
            module.RuntimeFacets.Add(facet);

            SupportPageGenerator.GeneratePage(baseObject, pagesPath, name, generatorConfiguration, module, imports);

            return true;
        }
    }
}
