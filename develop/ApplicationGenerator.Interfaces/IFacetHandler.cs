// file:	IFacetHandler.cs
//
// summary:	Declares the IFacetHandler interface

using AbstraX.ServerInterfaces;
using EntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AbstraX
{
    /// <summary>   Interface for facet handler. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 2/27/2021. </remarks>

    public interface IFacetHandler : IModuleHandler
    {
        /// <summary>   Gets a value indicating whether for life. </summary>
        ///
        /// <value> True if for life, false if not. </value>

        bool ForLife { get; }

        /// <summary>   Gets the facet handler layer. </summary>
        ///
        /// <value> The facet handler layer. </value>

        FacetHandlerLayer FacetHandlerLayer { get; }
        /// <summary>   Event queue for all listeners interested in ProcessFacets events. </summary>
        event ProcessFacetsHandler ProcessFacets;

        /// <summary>   Gets the related modules. </summary>
        ///
        /// <value> The related modules. </value>

        List<Module> RelatedModules { get; }

        /// <summary>   Process this.  </summary>
        ///
        /// <param name="baseObject">               The base object. </param>
        /// <param name="facet">                    The facet. </param>
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        bool Process(IBase baseObject, Facet facet, IGeneratorConfiguration generatorConfiguration);

        /// <summary>   Pre process. </summary>
        ///
        /// <param name="baseObject">               The base object. </param>
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        /// <param name="currentHandler">           The current handler. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        bool PreProcess(IBase baseObject, IGeneratorConfiguration generatorConfiguration, IFacetHandler currentHandler);

        /// <summary>   Posts the process. </summary>
        ///
        /// <param name="baseObject">               The base object. </param>
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        /// <param name="currentHandler">           The current handler. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        bool PostProcess(IBase baseObject, IGeneratorConfiguration generatorConfiguration, IFacetHandler currentHandler);
    }
}
