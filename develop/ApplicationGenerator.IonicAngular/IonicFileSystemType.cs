// file:	IonicFileSystemType.cs
//
// summary:	Implements the ionic file system type class

using AbstraX.FolderStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    /// <summary>   Attribute for ionic file system type. </summary>
    ///
    /// <remarks>   Ken, 10/11/2020. </remarks>

    public class IonicFileSystemTypeAttribute : FileSystemTypeAttribute
    {
        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>
        ///
        /// <param name="type"> The type. </param>

        public IonicFileSystemTypeAttribute(IonicFileSystemType type)
        {
            this.Type = type;
        }
    }

    /// <summary>   Values that represent ionic file system types. </summary>
    ///
    /// <remarks>   Ken, 10/11/2020. </remarks>

    public enum IonicFileSystemType
    {
        /// <summary>   An enum constant representing the Application project root option. </summary>
        AppProjectRoot,
        /// <summary>   An enum constant representing the Application option. </summary>
        App,
        /// <summary>   An enum constant representing the 18n option. </summary>
        i18n,
        /// <summary>   An enum constant representing the pages option. </summary>
        Pages,
        /// <summary>   An enum constant representing the providers option. </summary>
        Providers,
        /// <summary>   An enum constant representing the models option. </summary>
        Models,
        /// <summary>   An enum constant representing the Application core option. </summary>
        AppCore,
        /// <summary>   An enum constant representing the Application core services option. </summary>
        AppCoreServices,
        /// <summary>   An enum constant representing the assets option. </summary>
        Assets,
        /// <summary>   An enum constant representing the assets imgs option. </summary>
        AssetsImgs,
        /// <summary>   An enum constant representing the assets data option. </summary>
        AssetsData,
        /// <summary>   An enum constant representing the jasmine option. </summary>
        Jasmine,
        /// <summary>   An enum constant representing the modules option. </summary>
        Modules,
        /// <summary>   An enum constant representing the modules components option. </summary>
        ModulesComponents,
        /// <summary>   An enum constant representing the web a pi controllers option. </summary>
        WebAPIControllers,
        /// <summary>   An enum constant representing the web a pi models option. </summary>
        WebAPIModels,
        /// <summary>   An enum constant representing the web a pi providers option. </summary>
        WebAPIProviders,
        /// <summary>   An enum constant representing the web a pi services root option. </summary>
        WebAPIServicesRoot,
        /// <summary>   An enum constant representing the entity models option. </summary>
        EntityModels,
        /// <summary>   An enum constant representing the entity metadata option. </summary>
        EntityMetadata,
        /// <summary>   An enum constant representing the entity data configuration option. </summary>
        EntityDataConfiguration,
        /// <summary>   An enum constant representing the Application resources option. </summary>
        AppResources,
        /// <summary>   An enum constant representing the theme option. </summary>
        Theme,
        /// <summary>   An enum constant representing the Source option. </summary>
        Src
    }
}
