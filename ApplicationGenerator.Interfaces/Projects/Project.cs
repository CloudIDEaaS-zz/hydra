// file:	ProjectItemTemplates\IProjectItemTypeWizard.cs
//
// summary:	Declares the IProjectItemTypeWizard interface

using AbstraX.FolderStructure;
using System.Collections.Generic;

namespace AbstraX.Projects
{
    /// <summary>   A project. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/11/2022. </remarks>

    public class Project
    {
        /// <summary>   Gets or sets the website folder hierarchy. </summary>
        ///
        /// <value> The website folder hierarchy. </value>

        public WebsiteFolderHierarchy WebsiteFolderHierarchy { get; set; }

        /// <summary>   Gets the pathname of the root folder. </summary>
        ///
        /// <value> The pathname of the root folder. </value>

        public Folder RootFolder { get; }

        /// <summary>   Gets or sets the project items. </summary>
        ///
        /// <value> The project items. </value>

        public ProjectItems ProjectItems { get; private set; }

        /// <summary>   Gets the name. </summary>
        ///
        /// <value> The name. </value>

        public string Name { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/11/2022. </remarks>
        ///
        /// <param name="name">                     The name. </param>
        /// <param name="websiteFolderHierarchy">   The website folder hierarchy. </param>
        /// <param name="rootFolder">               The pathname of the root folder. </param>

        public Project(string name, WebsiteFolderHierarchy websiteFolderHierarchy, Folder rootFolder)
        {
            this.Name = name;
            this.WebsiteFolderHierarchy = websiteFolderHierarchy;
            this.RootFolder = rootFolder;
            this.ProjectItems = new ProjectItems(this);
        }
    }
}