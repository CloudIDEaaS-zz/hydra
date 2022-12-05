// file:	Handlers\FacetHandlers\WebAPIRestConfigJsonFacetHandler.cs
//
// summary:	Implements the web a pi REST configuration JSON facet handler class

using AbstraX.Angular;
using AbstraX.DataAnnotations;
using AbstraX.FolderStructure;
using AbstraX.Generators;
using AbstraX.Generators.Server.WebAPIRestConfigJson;
using AbstraX.ServerInterfaces;
using EntityProvider.Web.Entities;
using RestEntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Handlers.FacetHandlers
{
    /// <summary>   A WebAPI REST configuration JSON facet handler. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/3/2020. </remarks>

    public class WebAPIRestConfigJsonFacetHandler : IFacetHandler
    {
        /// <summary>   Gets the priority. </summary>
        ///
        /// <value> The priority. </value>

        public float Priority => 3.0f;

        /// <summary>   Gets a value indicating whether for life. </summary>
        ///
        /// <value> True if for life, false if not. </value>

        public bool ForLife => false;
        /// <summary>   Event queue for all listeners interested in ProcessFacets events. </summary>
        public event ProcessFacetsHandler ProcessFacets;

        /// <summary>   Gets the related modules. </summary>
        ///
        /// <value> The related modules. </value>

        public List<Module> RelatedModules { get; }

        /// <summary>   Gets the facet handler layer. </summary>
        ///
        /// <value> The facet handler layer. </value>

        public FacetHandlerLayer FacetHandlerLayer => FacetHandlerLayer.WebService;

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/3/2020. </remarks>

        public WebAPIRestConfigJsonFacetHandler()
        {
            this.RelatedModules = new List<Module>();
        }

        /// <summary>   Process this.  </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/3/2020. </remarks>
        ///
        /// <param name="baseObject">               The base object. </param>
        /// <param name="facet">                    The facet. </param>
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public bool Process(IBase baseObject, Facet facet, IGeneratorConfiguration generatorConfiguration)
        {
            var name = baseObject.Name;
            var container = (RestEntityContainer)baseObject;
            var variables = container.Variables;
            var servicesPath = generatorConfiguration.ApplicationFolderHierarchy[IonicFileSystemType.WebAPIServicesRoot];
            var configPath = FileSystemObject.PathCombine(servicesPath, "wwwroot", (string) container.Variables["title"] + "Config");
            var parentPath = (string)variables["parentPath"];
            var parentObject = baseObject.EntityDictionary.Single(p => p.Value.GetCondensedID() == parentPath).Value;
            var rootObject = (object)container.JsonRootObject;

            rootObject.SetDynamicMember("idBase", parentObject.GetCondensedID().ToBase64());

            WebAPIRestConfigJsonGenerator.GenerateJson(baseObject, configPath, generatorConfiguration);

            return true;
        }

        /// <summary>   Adds a collection of objects to the end of this collection. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/3/2020. </remarks>
        ///
        /// <param name="baseObject">               The base object. </param>
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        /// <param name="modules">                  The modules. </param>
        /// <param name="addModules">               The add modules. </param>
        /// <param name="moduleAddType">            Type of the module add. </param>

        public void AddRange(IBase baseObject, IGeneratorConfiguration generatorConfiguration, List<Module> modules, IEnumerable<Module> addModules, ModuleAddType moduleAddType)
        {
            modules.AddRange(addModules);
        }

        /// <summary>   Pre process. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/3/2020. </remarks>
        ///
        /// <param name="baseObject">               The base object. </param>
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        /// <param name="currentHandler">           The current handler. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public bool PreProcess(IBase baseObject, IGeneratorConfiguration generatorConfiguration, IFacetHandler currentHandler)
        {
            return true;
        }

        /// <summary>   Posts the process. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/3/2020. </remarks>
        ///
        /// <param name="baseObject">               The base object. </param>
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        /// <param name="currentHandler">           The current handler. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public bool PostProcess(IBase baseObject, IGeneratorConfiguration generatorConfiguration, IFacetHandler currentHandler)
        {
            return true;
        }
    }
}
