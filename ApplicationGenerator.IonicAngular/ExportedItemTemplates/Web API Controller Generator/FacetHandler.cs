using AbstraX.Angular;
using AbstraX.DataAnnotations;
using AbstraX.FolderStructure;
using AbstraX.Generators.TabController;
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
    [FacetHandler(typeof(UIAttribute), UIKindGuids.$basename$)]
    public class $basename$ControllerFacetHandler : IFacetHandler
    {
        public float Priority => 1.0f;
        public bool ForLife => false;
        public event ProcessFacetsHandler ProcessFacets;
        public List<Module> RelatedModules { get; }

        public $basename$ControllerFacetHandler()
        {
            this.RelatedModules = new List<Module>();
        }

        public bool Process(IBase baseObject, Facet facet, IGeneratorConfiguration generatorConfiguration)
        {
            var providerAttribute = (UIAttribute)facet.Attribute;
            var name = baseObject.GetNavigationName();
            var parentObject = (IElement)baseObject;
            var controllersPath = generatorConfiguration.ApplicationFolderHierarchy[IonicFileSystemType.Controllers];
            var controllersFolder = (Folder)generatorConfiguration.FileSystem[controllersPath];
            var relatedEntities = new List<RelatedEntity>();

            foreach (var childObject in parentObject.GetParentNavigationProperties(generatorConfiguration.PartsAliasResolver))
            {
                relatedEntities.Add(new RelatedEntity(childObject, generatorConfiguration));
            }

            $basename$ControllerGenerator.GenerateController(baseObject, pagesPath, name, generatorConfiguration, imports, module, relatedEntities);

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
