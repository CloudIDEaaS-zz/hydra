// file:	IAppGeneratorEngine.cs
//
// summary:	Declares the IAppGeneratorEngine interface

using AbstraX.FolderStructure;
using AbstraX.Models.Interfaces;
using EntityProvider.Web.Entities;
using System.Collections.Generic;

namespace AbstraX
{
    /// <summary>   Interface for application generator engine. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/21/2020. </remarks>

    public interface IAppGeneratorEngine : IGeneratorEngine, IAnalyticsReporter
    {
        /// <summary>   Process the container described by container. </summary>
        ///
        /// <param name="container">    The container. </param>

        void ProcessContainer(IEntityContainer container);

        /// <summary>   Process the entity. </summary>
        ///
        /// <param name="entity">           The entity. </param>
        /// <param name="isIdentityEntity"> (Optional) True if is identity, false if not. </param>

        void ProcessEntity(IEntityType entity, bool isIdentityEntity = false);

        /// <summary>   Process the entity set. </summary>
        ///
        /// <param name="entitySet">        Set the entity belongs to. </param>
        /// <param name="isIdentitySet">    (Optional) True if is identity set, false if not. </param>

        void ProcessEntitySet(IEntitySet entitySet, bool isIdentitySet = false);

        /// <summary>   Process the navigation property described by property. </summary>
        ///
        /// <param name="property"> The property. </param>

        void ProcessNavigationProperty(INavigationProperty property);

        /// <summary>   Handles the module assembly. </summary>
        ///
        /// <param name="moduleAssembly">   The module assembly. </param>
        /// <param name="folder">           Pathname of the folder. </param>

        void HandleModuleAssembly(IModuleAssembly moduleAssembly, Folder folder);

        /// <summary>   Process the property described by property. </summary>
        ///
        /// <param name="property"> The property. </param>
        ///
        /// <returns>   A HandlerStackItem. </returns>

        HandlerStackItem ProcessProperty(IEntityProperty property);

        /// <summary>   Gets the full pathname of the current user interface hierarchy file. </summary>
        ///
        /// <value> The full pathname of the current user interface hierarchy file. </value>

        string CurrentUIHierarchyPath { get; }

        /// <summary>   Gets all models. </summary>
        ///
        /// <value> all models. </value>

        IEnumerable<IModel> AllModels { get; }

        /// <summary>   Gets the identifier of the parent process. </summary>
        ///
        /// <value> The identifier of the parent process. </value>

        int ParentProcessId { get; }

        /// <summary>   Gets the generator mode. </summary>
        ///
        /// <value> The generator mode. </value>

        GeneratorMode GeneratorMode { get; }
    }
}