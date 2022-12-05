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
using $rootnamespace$;

namespace AbstraX.FacetHandlers
{
    [FacetHandler(typeof(UIAttribute), UIKindGuids.Any, ModuleImports.IONIC_ANGULAR_BASIC_PAGE_IMPORTS)]
    public class $basename$ClientModelFacetHandler : IFacetHandler
    {
        public float Priority => 1.0f;
        public bool ForLife => false;
        public event ProcessFacetsHandler ProcessFacets;
        public List<Module> RelatedModules { get; }

        public $basename$ClientModelFacetHandler()
        {
            this.RelatedModules = new List<Module>();
        }

        public bool Process(IBase baseObject, Facet facet, IGeneratorConfiguration generatorConfiguration)
        {
            var uiAttribute = (UIAttribute)facet.Attribute;
            var name = baseObject.GetNavigationName();
            var parentObject = (IParentBase)baseObject;
            var pagesPath = generatorConfiguration.ApplicationFolderHierarchy[IonicFileSystemType.Pages];
            var pagesFolder = (Folder) generatorConfiguration.FileSystem[pagesPath];
            var imports = generatorConfiguration.CreateImports(this, baseObject, pagesFolder);
            var module = new AngularModule(name + "Module");
            var formFields = new List<FormField>();

            generatorConfiguration.ModuleAssemblies.Push(module);

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

            $basename$PageGenerator.GeneratePage(baseObject, pagesPath, name, generatorConfiguration, module, imports, module, formFields);

            return true;
        }

        public bool PreProcess(IBase baseObject, IGeneratorConfiguration generatorConfiguration, IFacetHandler currentHandler)
        {
            return true;
        }

        public bool PostProcess(IBase baseObject, IGeneratorConfiguration generatorConfiguration, IFacetHandler currentHandler)
        {
            return true;
        }
    }
}
