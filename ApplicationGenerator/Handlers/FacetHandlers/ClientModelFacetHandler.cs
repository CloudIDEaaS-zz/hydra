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
using AbstraX.Generators.Client.ClientModel;
using AbstraX.Generators;

namespace AbstraX.Handlers.FacetHandlers
{
    [FacetHandler(typeof(UIAttribute), UIKindGuids.Element, ModuleImports.IONIC_ANGULAR_BASIC_PAGE_IMPORTS)]
    public class ClientModelFacetHandler : BasePageFacetHandler
    {
        public override bool Process(IBase baseObject, Facet facet, IGeneratorConfiguration generatorConfiguration)
        {
            var uiAttribute = (UIAttribute)facet.Attribute;
            var name = baseObject.Name;
            var parentObject = (IElement)baseObject;
            var modelsPath = generatorConfiguration.ApplicationFolderHierarchy[IonicFileSystemType.Models];
            var modelsFolder = (Folder)generatorConfiguration.FileSystem[modelsPath];
            var imports = generatorConfiguration.CreateImports(this, baseObject, modelsFolder);
            var formFields = new List<FormField>();

            foreach (var childObject in parentObject.GetFormFields(generatorConfiguration.PartsAliasResolver))
            {
                if (childObject is IAttribute)
                {
                    formFields.Add(new FormField((IAttribute)childObject, generatorConfiguration));
                }
                else
                {

                }
            }

            ClientModelGenerator.GenerateModel(baseObject, modelsPath, name, generatorConfiguration, imports, formFields);

            return true;
        }
    }
}
