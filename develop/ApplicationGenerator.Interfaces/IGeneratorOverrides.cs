// file:	IGeneratorOverrides.cs
//
// summary:	Declares the IGeneratorOverrides interface

using AbstraX.FolderStructure;
using AbstraX.Models.Interfaces;
using AbstraX.ServerInterfaces;
using EntityProvider.Web.Entities;
using System.Collections.Generic;

namespace AbstraX
{
    /// <summary>   Interface for generator overrides. </summary>
    ///
    /// <remarks>   Ken, 10/5/2020. </remarks>

    public interface IGeneratorOverrides
    {
        /// <summary>   Gets a value indicating whether the overrides namespace. </summary>
        ///
        /// <value> True if overrides namespace, false if not. </value>

        bool OverridesNamespace { get; }

        /// <summary>   Gets a value indicating whether the overrides application name. </summary>
        ///
        /// <value> True if overrides application name, false if not. </value>

        bool OverridesAppName { get; }

        /// <summary>   Gets a value indicating whether the overrides application description. </summary>
        ///
        /// <value> True if overrides application description, false if not. </value>

        bool OverridesAppDescription { get; }

        /// <summary>   Gets a value indicating whether the copies to alternate location. </summary>
        ///
        /// <value> True if copies to alternate location, false if not. </value>

        bool CopiesToAlternateLocation { get; }

        /// <summary>   Gets or sets the original namespace. </summary>
        ///
        /// <value> The original namespace. </value>

        string OriginalNamespace { get; set; }

        /// <summary>   Gets a namespace. </summary>
        ///
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        /// <param name="argumentsKind">            The arguments kind. </param>
        ///
        /// <returns>   The namespace. </returns>

        string GetNamespace(IGeneratorConfiguration generatorConfiguration, string argumentsKind);

        /// <summary>   Gets application name. </summary>
        ///
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        /// <param name="argumentsKind">            The arguments kind. </param>
        ///
        /// <returns>   The application name. </returns>

        string GetAppName(IGeneratorConfiguration generatorConfiguration, string argumentsKind);

        /// <summary>   Gets application description. </summary>
        ///
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        /// <param name="argumentsKind">            The arguments kind. </param>
        ///
        /// <returns>   The application description. </returns>

        string GetAppDescription(IGeneratorConfiguration generatorConfiguration, string argumentsKind);

        /// <summary>   Copies the files. </summary>
        ///
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        /// <param name="argumentsKind">            The arguments kind. </param>

        void CopyFiles(IGeneratorConfiguration generatorConfiguration, string argumentsKind);

        /// <summary>   Gets handler arguments. </summary>
        ///
        /// <param name="packageCachePath"> Full pathname of the package cache file. </param>
        /// <param name="argumentsKind">    The arguments kind. </param>
        /// <param name="workingDirectory"> (Optional) Pathname of the working directory. </param>
        ///
        /// <returns>   The handler arguments. </returns>

        Dictionary<string, object> GetHandlerArguments(string packageCachePath, string argumentsKind, string workingDirectory = null);

        /// <summary>   Skip process. </summary>
        ///
        /// <param name="facetHandler">             The facet handler. </param>
        /// <param name="baseObject">               The base object. </param>
        /// <param name="facet">                    The facet. </param>
        /// <param name="generatorConfiguration">   The generator configuration. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        bool SkipProcess(IFacetHandler facetHandler, IBase baseObject, Facet facet, IGeneratorConfiguration generatorConfiguration);

        /// <summary>   Gets override identifier. </summary>
        ///
        /// <param name="baseObject">   The base object. </param>
        /// <param name="predicate">    The predicate. </param>
        /// <param name="generatedId">  Identifier for the generated. </param>
        ///
        /// <returns>   The override identifier. </returns>

        string GetOverrideId(IBase baseObject, string predicate, string generatedId);
    }
}