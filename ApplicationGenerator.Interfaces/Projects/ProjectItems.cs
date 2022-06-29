// file:	Projects\ProjectItems.cs
//
// summary:	Implements the project items class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Projects
{
    /// <summary>   A project items. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/11/2022. </remarks>

    public class ProjectItems : BaseList<ProjectItem>
    {
        /// <summary>   Gets the parent project item. </summary>
        ///
        /// <value> The parent project item. </value>

        public ProjectItem ParentProjectItem { get; }

        /// <summary>   Gets the parent project. </summary>
        ///
        /// <value> The parent project. </value>

        public Project ParentProject { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/11/2022. </remarks>
        ///
        /// <param name="parentProjectItem">    The parent project item. </param>

        public ProjectItems(ProjectItem parentProjectItem)
        {
            this.ParentProjectItem = parentProjectItem;
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/11/2022. </remarks>
        ///
        /// <param name="parentProject">    The parent project. </param>

        public ProjectItems(Project parentProject)
        {
            this.ParentProject = parentProject;
        }
    }
}
