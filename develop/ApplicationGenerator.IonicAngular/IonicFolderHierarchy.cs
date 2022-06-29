// file:	IonicFolderHierarchy.cs
//
// summary:	Implements the ionic folder hierarchy class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstraX.Angular;
using AbstraX.FolderStructure;
using Utils;

namespace AbstraX
{
    /// <summary>   An ionic folder hierarchy. </summary>
    ///
    /// <remarks>   Ken, 10/11/2020. </remarks>

    public class IonicFolderHierarchy : ApplicationFolderHierarchy
    {
        /// <summary>   Gets or sets the source for the. </summary>
        ///
        /// <value> The source. </value>

        [IonicFileSystemType(IonicFileSystemType.Src)]
        public string Src { get; set; }

        /// <summary>   Gets or sets the application. </summary>
        ///
        /// <value> The application. </value>

        [IonicFileSystemType(IonicFileSystemType.App)]
        public string App { get; set; }

        /// <summary>   Gets or sets the pages. </summary>
        ///
        /// <value> The pages. </value>

        [IonicFileSystemType(IonicFileSystemType.Pages)]
        public string Pages { get; set; }

        /// <summary>   Gets the providers. </summary>
        ///
        /// <value> The providers. </value>

        [IonicFileSystemType(IonicFileSystemType.Providers)]
        public string Providers { get; }

        /// <summary>   Gets the web a pi controllers. </summary>
        ///
        /// <value> The web a pi controllers. </value>

        [IonicFileSystemType(IonicFileSystemType.WebAPIControllers)]
        public string WebAPIControllers { get; }

        /// <summary>   Gets the web a pi providers. </summary>
        ///
        /// <value> The web a pi providers. </value>

        [IonicFileSystemType(IonicFileSystemType.WebAPIProviders)]
        public string WebAPIProviders { get; }

        /// <summary>   Gets the web a pi models. </summary>
        ///
        /// <value> The web api models. </value>

        [IonicFileSystemType(IonicFileSystemType.WebAPIModels)]
        public string WebAPIModels { get; }

        /// <summary>   Gets the entity models. </summary>
        ///
        /// <value> The entity models. </value>

        [IonicFileSystemType(IonicFileSystemType.EntityModels)]
        public string EntityModels { get; }

        /// <summary>   Gets the entity metadata. </summary>
        ///
        /// <value> The entity metadata. </value>

        [IonicFileSystemType(IonicFileSystemType.EntityMetadata)]
        public string EntityMetadata { get; }

        /// <summary>   Gets the entity data configuration. </summary>
        ///
        /// <value> The entity data configuration. </value>

        [IonicFileSystemType(IonicFileSystemType.EntityDataConfiguration)]
        public string EntityDataConfiguration { get; }

        /// <summary>   Gets the application resources. </summary>
        ///
        /// <value> The application resources. </value>

        [IonicFileSystemType(IonicFileSystemType.AppResources)]
        public string AppResources { get; }

        /// <summary>   Gets the web a pi services root. </summary>
        ///
        /// <value> The web a pi services root. </value>

        [IonicFileSystemType(IonicFileSystemType.WebAPIServicesRoot)]
        public string WebAPIServicesRoot { get; }

        /// <summary>   Gets the models. </summary>
        ///
        /// <value> The models. </value>

        [IonicFileSystemType(IonicFileSystemType.Models)]
        public string Models { get; }

        /// <summary>   Gets the 18n. </summary>
        ///
        /// <value> The i 18n. </value>

        [IonicFileSystemType(IonicFileSystemType.i18n)]
        public string i18n { get; }

        /// <summary>   Gets the theme. </summary>
        ///
        /// <value> The theme. </value>

        [IonicFileSystemType(IonicFileSystemType.Theme)]
        public string Theme { get; }
        [IonicFileSystemType(IonicFileSystemType.Assets)]
        public string Assets { get; }
        [IonicFileSystemType(IonicFileSystemType.AssetsImgs)]
        public string AssetImgs { get; }
        [IonicFileSystemType(IonicFileSystemType.AssetsData)]
        public string AssetData { get; }

        /// <summary>   Gets the ionic project root. </summary>
        ///
        /// <value> The ionic project root. </value>

        [IonicFileSystemType(IonicFileSystemType.AppProjectRoot)]
        public string IonicProjectRoot
        {
            get
            {
                return this.ProjectRoot;
            }
        }

        /// <summary>   Indexer to get items within this collection using array index syntax. </summary>
        ///
        /// <param name="type"> The name. </param>
        ///
        /// <returns>   The indexed item. </returns>

        public override string this[Enum type]
        {
            get
            {
                return this[(IonicFileSystemType)type];
            }
        }

        /// <summary>   Indexer to get items within this collection using array index syntax. </summary>
        ///
        /// <param name="name"> The name. </param>
        ///
        /// <returns>   The indexed item. </returns>

        public override string this[string name]
        {
            get
            {
                return this[EnumUtils.GetValue<IonicFileSystemType>(name)];
            }
        }

        /// <summary>   Indexer to get items within this collection using array index syntax. </summary>
        ///
        /// <param name="type"> The type. </param>
        ///
        /// <returns>   The indexed item. </returns>

        public string this[IonicFileSystemType type]
        {
            get
            {
                return base.GetPropertyValue(type);
            }
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>
        ///
        /// <param name="folderRoot">           The folder root. </param>
        /// <param name="projectFolderRoot">    The project folder root. </param>
        /// <param name="servicesFolderRoot">   The services folder root. </param>
        /// <param name="entitiesFolderRoot">   The entities folder root. </param>

        public IonicFolderHierarchy(string folderRoot, string projectFolderRoot, string servicesFolderRoot, string entitiesFolderRoot) : base(folderRoot, projectFolderRoot, servicesFolderRoot, entitiesFolderRoot)
        {
            this.Src = base.CreateProjectPath("src", true);
            this.App = base.CreateProjectPath("src/app", true);
            this.Pages = base.CreateProjectPath("src/app/pages", true);
            this.Providers = base.CreateProjectPath("src/app/providers", true);
            this.Models = base.CreateProjectPath("src/app/models", true);
            this.i18n = base.CreateProjectPath("src/assets/i18n", true);
            this.Theme = base.CreateProjectPath("src/theme", true);
            this.Assets = base.CreateProjectPath("src/assets", true);
            this.AssetImgs = base.CreateProjectPath("src/assets/img", true);
            this.AssetData = base.CreateProjectPath("src/assets/data", true);
            this.WebAPIControllers = base.CreateServicesPath("Controllers", true);
            this.WebAPIProviders = base.CreateServicesPath("Providers", true);
            this.WebAPIModels = base.CreateServicesPath("Models", true);
            this.WebAPIServicesRoot = this.ServicesRoot;
            this.EntityModels = base.CreateEntitiesPath("Models", true);
            this.EntityMetadata = base.CreateEntitiesPath("Metadata", true);
            this.EntityDataConfiguration = base.CreateEntitiesPath("DataConfiguration", true);
            this.AppResources = base.CreateEntitiesPath("AppResources", true);
        }
    }
}
