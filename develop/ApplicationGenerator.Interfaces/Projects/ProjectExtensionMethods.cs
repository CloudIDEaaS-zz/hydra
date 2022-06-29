// file:	Projects\ProjectExtensionMethods.cs
//
// summary:	Implements the project extension methods class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Projects
{
    /// <summary>   A project extension methods. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/11/2022. </remarks>

    public static class ProjectExtensionMethods
    {
        /// <summary>   Searches for the first item by file name. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/11/2022. </remarks>
        ///
        /// <param name="project">  The project. </param>
        /// <param name="fileName"> Filename of the file. </param>
        ///
        /// <returns>   The found item by file name. </returns>

        public static ProjectItem FindItemByFileName(this Project project, string fileName)
        {
            return project.FindProjectItem(i => i.Document.FileName.AsCaseless() == fileName);
        }

        /// <summary>   Searches for the first item by folder name. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/11/2022. </remarks>
        ///
        /// <param name="project">  The project. </param>
        /// <param name="folder">   Pathname of the folder. </param>
        ///
        /// <returns>   The found item by folder name. </returns>

        public static ProjectItem FindItemByFolderName(this Project project, string folder)
        {
            return project.FindProjectItem(i => i.Properties.ContainsKey("FullName") && ((string) i.Properties["FullName"]).AsCaseless() == folder);
        }

        /// <summary>   Searches for the first item by item identifier. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/11/2022. </remarks>
        ///
        /// <param name="project">  The project. </param>
        /// <param name="itemId">   Identifier for the item. </param>
        ///
        /// <returns>   The found item by item identifier. </returns>

        public static ProjectItem FindItemByItemId(this Project project, uint itemId)
        {
            throw new NotImplementedException();
        }

        /// <summary>   A Project extension method that gets all project items. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/11/2022. </remarks>
        ///
        /// <param name="project">  The project. </param>
        ///
        /// <returns>   all project items. </returns>

        public static List<ProjectItem> GetAllProjectItems(this Project project)
        {
            var projectItems = project.ProjectItems;
            Action<ProjectItems> recurse = null;
            var list = new List<ProjectItem>();

            recurse = (i) =>
            {
                foreach (var projectItem in i)
                {
                    list.Add(projectItem);

                    recurse(projectItem.ProjectItems);
                }
            };

            recurse(project.ProjectItems);

            return list;
        }

        /// <summary>   A Project extension method that searches for the first project item. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/11/2022. </remarks>
        ///
        /// <param name="project">  The project. </param>
        /// <param name="filter">   Specifies the filter. </param>
        ///
        /// <returns>   The found project item. </returns>

        public static ProjectItem FindProjectItem(this Project project, Func<ProjectItem, bool> filter)
        {
            var projectItems = project.ProjectItems;
            Action<ProjectItems> recurse = null;
            ProjectItem found = null;

            recurse = (i) =>
            {
                foreach (var projectItem in i)
                {
                    if (filter(projectItem))
                    {
                        found = projectItem;
                        return;
                    }

                    recurse(projectItem.ProjectItems);

                    if (found != null)
                    {
                        return;
                    }
                }
            };

            recurse(project.ProjectItems);

            return found;
        }
    }
}
