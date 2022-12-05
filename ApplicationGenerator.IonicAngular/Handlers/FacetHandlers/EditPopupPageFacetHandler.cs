using AbstraX.Angular;
using AbstraX.DataAnnotations;
using AbstraX.FolderStructure;
using AbstraX.Generators;
using AbstraX.Generators.Pages.EditPopupPage;
using AbstraX.Generators.Pages.TabPage;
using AbstraX.ServerInterfaces;
using EntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Handlers.FacetHandlers
{
    [FacetHandler(typeof(UIAttribute), UIKindGuids.EditPopup)]
    [ImportGroup("Page", 1, ModuleImports.IONIC_ANGULAR_EDIT_PAGE_IMPORTS)]
    [ImportGroup("Validator", 1, ModuleImports.ANGULAR_VALIDATION_PAGE_IMPORTS)]
    public class EditPopupPageFacetHandler : BasePageFacetHandler
    {
        public override float Priority => 3.0f;

        public override bool Process(IBase baseObject, Facet facet, IGeneratorConfiguration generatorConfiguration)
        {
            var uiAttribute = (UIAttribute)facet.Attribute;
            var baseElement = (IElement)baseObject;
            var name = baseElement.GetNavigationName(UIKind.EditPopup);
            var formFields = new List<FormField>();
            var parentObject = (IElement)baseObject.Parent;
            var subFolderName = name.ToLower();
            var pagePath = FileSystemObject.PathCombine(generatorConfiguration.ApplicationFolderHierarchy[IonicFileSystemType.Pages], subFolderName);
            var loadKind = uiAttribute.UILoadKind;
            var kind = uiAttribute.UIKind;
            IDictionary<string, IEnumerable<ModuleImportDeclaration>> importGroups;
            IModuleAssembly module;
            Folder pageFolder = null;

            if (!generatorConfiguration.FileSystem.Exists(pagePath))
            {
                var newPath = generatorConfiguration.FileSystem.CreatePath(pagePath);

                pageFolder = (Folder)generatorConfiguration.FileSystem[newPath];
            }
            else
            {
                pageFolder = (Folder)generatorConfiguration.FileSystem[pagePath];
            }

            if (uiAttribute.UILoadKind == UILoadKind.HomePage)
            {
                this.Raise<ApplicationFacetHandler>();
            }

            importGroups = generatorConfiguration.CreateImportGroups(this, baseObject, pageFolder, true);

            foreach (var childObject in baseElement.GetFormFields(generatorConfiguration.PartsAliasResolver))
            {
                if (childObject is IAttribute)
                {
                    formFields.Add(new FormField((IAttribute) childObject, facet, generatorConfiguration));
                }
                else
                {
                    DebugUtils.Break();
                }
            }

            module = generatorConfiguration.PushModuleAssembly<AngularModule>("Edit" + name);

            module.UILoadKind = loadKind;
            module.UIKind = kind;
            module.UIHierarchyPath = uiAttribute.UIHierarchyPath;

            EditPopupPageGenerator.GeneratePage(baseObject, pagePath, name, generatorConfiguration, module, importGroups, formFields, loadKind, kind);

            return true;
        }
    }
}
