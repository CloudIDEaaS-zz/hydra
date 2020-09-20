using AbstraX.Angular;
using AbstraX.DataAnnotations;
using AbstraX.FolderStructure;
using AbstraX.Generators;
using AbstraX.Generators.Pages.LoginPage;
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
    [FacetHandler(typeof(UIAttribute), UIKindGuids.LoginPage, ModuleImports.IONIC_ANGULAR_BASIC_PAGE_IMPORTS)]
    [ImportGroup("Page", 1, ModuleImports.IONIC_ANGULAR_EDIT_PAGE_IMPORTS)]
    [ImportGroup("Validator", 1, ModuleImports.ANGULAR_VALIDATION_PAGE_IMPORTS)]
    public class LoginPageFacetHandler : BasePageFacetHandler
    {
        public override float Priority => 1.0f;
        public const string LOGIN_TITLE = "LOGIN_TITLE";
        public const string LOGIN_BUTTON = "LOGIN_BUTTON";

        public override bool Process(IBase baseObject, Facet facet, IGeneratorConfiguration generatorConfiguration)
        {
            var uiAttribute = (UIAttribute)facet.Attribute;
            var baseElement = (IElement)baseObject;
            var userFields = new List<IdentityField>();
            var parentObject = (IElement)baseObject.Parent;
            var name = baseElement.GetNavigationName(UIKind.LoginPage);
            var subFolderName = name.ToLower();
            var pagePath = FileSystemObject.PathCombine(generatorConfiguration.ApplicationFolderHierarchy[IonicFileSystemType.Pages], subFolderName);
            var resourcesHelper = generatorConfiguration.ResourcesHandler.CreateHelper(UIKind.LoginPage, baseObject);
            IDictionary<string, IEnumerable<ModuleImportDeclaration>> importGroups;
            IModuleAssembly module;
            Folder pageFolder = null;
            string loginTitleTranslationKey;
            string loginButtonTranslationKey;

            if (!generatorConfiguration.FileSystem.Exists(pagePath))
            {
                var newPath = generatorConfiguration.FileSystem.CreatePath(pagePath);

                pageFolder = (Folder)generatorConfiguration.FileSystem[newPath];
            }
            else
            {
                pageFolder = (Folder)generatorConfiguration.FileSystem[pagePath];
            }

            if (uiAttribute.UILoadKind == UILoadKind.RootPage)
            {
                this.Raise<ApplicationFacetHandler>();
            }

            importGroups = generatorConfiguration.CreateImportGroups(this, baseObject, pageFolder, true);
            importGroups.AddIndexImport("Page", "MainPage", 1);

            foreach (var childObject in baseElement.GetIdentityFields(IdentityFieldCategory.Login))
            {
                if (childObject is IAttribute)
                {
                    userFields.Add(new IdentityField((IAttribute)childObject, generatorConfiguration));
                }
                else
                {
                    DebugUtils.Break();
                }
            }

            module = generatorConfiguration.PushModuleAssembly<AngularModule>("Login");
            module.UILoadKind = uiAttribute.UILoadKind;
            loginTitleTranslationKey = resourcesHelper.Get(() => LOGIN_TITLE);
            loginButtonTranslationKey = resourcesHelper.Get(() => LOGIN_BUTTON);

            LoginPageGenerator.GeneratePage(baseObject, pagePath, name, generatorConfiguration, module, importGroups, userFields, loginTitleTranslationKey, loginButtonTranslationKey);

            return true;
        }
    }
}
