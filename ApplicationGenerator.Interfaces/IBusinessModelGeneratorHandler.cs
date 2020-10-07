// file:	IBusinessModelGeneratorHandler.cs
//
// summary:	Declares the IBusinessModelGeneratorHandler interface

using AbstraX.ServerInterfaces;
using CodeInterfaces;
using EntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AbstraX
{
    /// <summary>   Interface for business model generator handler. </summary>
    ///
    /// <remarks>   Ken, 10/1/2020. </remarks>

    public interface IBusinessModelGeneratorHandler : IHandler
    {
        /// <summary>   Process this.  </summary>
        ///
        /// <param name="businessModel">            The business model. </param>
        /// <param name="projectType">              Type of the project. </param>
        /// <param name="projectFolderRoot">        The project folder root. </param>
        /// <param name="templateFile">             The template file. </param>
        /// <param name="outputFileName">           Filename of the output file. </param>
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        bool Process(BusinessModel businessModel, Guid projectType, string projectFolderRoot, string templateFile, string outputFileName, IGeneratorConfiguration generatorConfiguration);
    }
}
