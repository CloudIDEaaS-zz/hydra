// file:	IWorkspaceFileTypeHandler.cs
//
// summary:	Declares the IWorkspaceFileTypeHandler interface

using AbstraX.ServerInterfaces;
using CodeInterfaces;
using EntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AbstraX
{
    /// <summary>   Interface for workspace file type handler. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/24/2021. </remarks>

    public interface IWorkspaceFileTypeHandler : IHandler
    {
        /// <summary>   Gets the file name expressions. </summary>
        ///
        /// <value> The file name expressions. </value>

        string[] FileNameExpressions { get; }

        /// <summary>   Gets the tokens to process. </summary>
        ///
        /// <value> The tokens to process. </value>

        string[] TokensToProcess { get; }

        /// <summary>   Gets the output content. </summary>
        ///
        /// <value> The output content. </value>

        string OutputContent { get; }

        /// <summary>   Pre process. </summary>
        ///
        /// <param name="projectType">              Type of the project. </param>
        /// <param name="appName">                  Name of the application. </param>
        /// <param name="rawFileRelativePath">      Full pathname of the raw file relative file. </param>
        /// <param name="outputFileName">           Filename of the output file. </param>
        /// <param name="supportedTokens">          The supported tokens. </param>
        /// <param name="content">                  The content. </param>
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        bool PreProcess(Guid projectType, string appName, string rawFileRelativePath, string outputFileName, string[] supportedTokens, string content, IGeneratorConfiguration generatorConfiguration);

        /// <summary>   Process this.  </summary>
        ///
        /// <param name="tokenContentHandlers">     The token content handlers. </param>
        /// <param name="workspaceTemplate">        The workspace template. </param>
        /// <param name="projectType">              Type of the project. </param>
        /// <param name="appName">                  Name of the application. </param>
        /// <param name="rawFileRelativePath">      Full pathname of the raw file relative file. </param>
        /// <param name="outputFileName">           Filename of the output file. </param>
        /// <param name="content">                  The content. </param>
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        bool Process(Dictionary<string, IWorkspaceTokenContentHandler> tokenContentHandlers, IWorkspaceTemplate workspaceTemplate, Guid projectType, string appName, string rawFileRelativePath, string outputFileName, string content, IGeneratorConfiguration generatorConfiguration);

        /// <summary>   Posts the process. </summary>
        ///
        /// <param name="appFolderStructureSurveyor">    The application layout surveyor. </param>

        void PostProcess(IAppFolderStructureSurveyor appFolderStructureSurveyor);
    }
}
