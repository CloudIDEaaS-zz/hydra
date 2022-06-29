// file:	ProjectItemTemplates\IProjectItemTypeWizard.cs
//
// summary:	Declares the IProjectItemTypeWizard interface

using System.Collections.Generic;
using Utils;

namespace AbstraX.Projects
{
    /// <summary>   A project item. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/10/2022. </remarks>

    public class ProjectItem
    {
        /// <summary>   Gets the identifier of the item. </summary>
        ///
        /// <value> The identifier of the item. </value>

        public uint ItemId { get; }

        /// <summary>   Gets or sets the project. </summary>
        ///
        /// <value> The project. </value>

        public Project ContainingProject { get; set; }

        /// <summary>   Gets the object. </summary>
        ///
        /// <value> The object. </value>

        public object Object { get; }

        /// <summary>   Gets or sets the name. </summary>
        ///
        /// <value> The name. </value>

        public string Name { get; set; }

        /// <summary>   Gets or sets the type of the item. </summary>
        ///
        /// <value> The type of the item. </value>

        public string ItemType { get; set; }

        /// <summary>   Gets or sets a value indicating whether the user created. </summary>
        ///
        /// <value> True if user created, false if not. </value>

        public bool UserCreated { get; set; }

        /// <summary>   Gets or sets the project items. </summary>
        ///
        /// <value> The project items. </value>

        public ProjectItems ProjectItems { get; private set; }

        /// <summary>   Gets or sets the navigation project items. </summary>
        ///
        /// <value> The navigation project items. </value>

        public ProjectItems NavigationProjectItems { get; private set; }

        /// <summary>   Gets the document. </summary>
        ///
        /// <value> The document. </value>

        public Document Document { get; set; }

        /// <summary>   Gets the properties. </summary>
        ///
        /// <value> The properties. </value>

        public Dictionary<string, object> Properties { get; }

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/11/2022. </remarks>

        public ProjectItem(Project containingProject, string name, object obj)
        {
            this.ProjectItems = new ProjectItems(this);
            this.NavigationProjectItems = new ProjectItems(this);
            this.ContainingProject = containingProject;
            this.Name = name;
            this.Object = obj;

            this.Properties = new Dictionary<string, object>();
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/11/2022. </remarks>
        ///
        /// <param name="containingProject">    The project. </param>
        /// <param name="name">                 The name. </param>
        /// <param name="document">             The document. </param>

        public ProjectItem(Project containingProject, string name, Document document)
        {
            this.ProjectItems = new ProjectItems(this);
            this.NavigationProjectItems = new ProjectItems(this);
            this.ContainingProject = containingProject;
            this.Name = name;
            this.Document = document;

            this.Properties = new Dictionary<string, object>();
        }
    }
}