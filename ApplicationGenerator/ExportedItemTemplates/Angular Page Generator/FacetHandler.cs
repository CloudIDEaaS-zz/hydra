using AbstraX.Angular;
using AbstraX.DataAnnotations;
using AbstraX.FolderStructure;
using AbstraX.Generators.TabPage;
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
    [FacetHandler(typeof(UIAttribute), UIKindGuids.$basename$Page, ModuleImports.IONIC_ANGULAR_BASIC_PAGE_IMPORTS)]
    public class $basename$PageFacetHandler : BasePageFacetHandler
    {
        public override bool Process(IBase baseObject, Facet facet, IGeneratorConfiguration generatorConfiguration)
        {
            var uiAttribute = (UIAttribute)facet.Attribute;
            var name = baseObject.GetNavigationName();
            var parentObject = (IParentBase)baseObject;
            var pagesPath = generatorConfiguration.ApplicationFolderHierarchy[IonicFileSystemType.Pages];
            var pagesFolder = (Folder) generatorConfiguration.FileSystem[pagesPath];
            var imports = generatorConfiguration.CreateImports(this, baseObject, pagesFolder, false, 1);
            var inputObjects = new List<object>();  // TODO - change, preferably to a typed List, to match your needs
            IModuleAssembly module;

            foreach (var childObject in parentObject.GetFollowingChildren(generatorConfiguration.PartsAliasResolver))
            {
                // TODO - add code to capture children elements

                inputObjects.Add(new object());
            }

            module = generatorConfiguration.PushModuleAssembly<AngularModule>(name);
            module.UILoadKind = uiAttribute.UILoadKind;

            $basename$PageGenerator.GeneratePage(baseObject, pagesPath, name, generatorConfiguration, module, imports, module /* , .. TODO - add additional parameters */);

            return true;
        }
    }
}
