// file:	ProjectItemTemplates\IProjectItemTypeWizard.cs
//
// summary:	Declares the IProjectItemTypeWizard interface

using Microsoft.VisualStudio.TemplateWizard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Projects
{
    /// <summary>   Interface for project item type wizard. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/10/2022. </remarks>

    public interface IProjectItemTypeWizard
    {

        /// <summary>
        /// Summary:
        ///     Runs custom wizard logic before opening an item in the template.
        /// 
        /// Parameters:
        ///   projectItem:
        ///     The project item that will be opened.
        /// </summary>
        ///
        /// <param name="projectItem">  The project item that will be opened. </param>

        void BeforeOpeningFile(ProjectItem projectItem);

        /// <summary>
        /// Summary:
        ///     Runs custom wizard logic when a project has finished generating.
        /// 
        /// Parameters:
        ///   project:
        ///     The project that finished generating.
        /// </summary>
        ///
        /// <param name="project">  The project that finished generating. </param>

        void ProjectFinishedGenerating(Project project);

        /// <summary>
        /// Summary:
        ///     Runs custom wizard logic when a project item has finished generating.
        /// 
        /// Parameters:
        ///   projectItem:
        ///     The project item that finished generating.
        /// </summary>
        ///
        /// <param name="projectItem">  The project item that finished generating. </param>

        void ProjectItemFinishedGenerating(ProjectItem projectItem);

        /// <summary>
        /// Summary:
        ///     Runs custom wizard logic when the wizard has completed all tasks.
        /// </summary>

        void RunFinished();

        /// <summary>
        /// Summary:
        ///     Indicates whether the specified project item should be added to the project.
        /// 
        /// Parameters:
        ///   filePath:
        ///     The path to the project item.
        /// 
        /// Returns:
        ///     true if the project item should be added to the project; otherwise, false.
        /// </summary>
        ///
        /// <param name="filePath"> The path to the project item. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        bool ShouldAddProjectItem(string filePath);

        /// <summary>
        /// Summary:
        ///     Runs custom wizard logic at the beginning of a template wizard run.
        /// 
        /// Parameters:
        ///   automationObject:
        ///     The automation object being used by the template wizard.
        /// 
        ///   replacementsDictionary:
        ///     The list of standard parameters to be replaced.
        /// 
        ///   runKind:
        ///     A Microsoft.VisualStudio.TemplateWizard.WizardRunKind indicating the type of wizard run.
        /// 
        ///   customParams:
        ///     The custom parameters with which to perform parameter replacement in the project.
        /// </summary>
        ///
        /// <param name="automationObject">         The automation object being used by the template
        ///                                         wizard. </param>
        /// <param name="replacementsDictionary">   The list of standard parameters to be replaced. </param>
        /// <param name="runKind">                  A <see cref="T:Microsoft.VisualStudio.TemplateWizard.Wi
        ///                                         zardRunKind" /> indicating the type of wizard run. </param>
        /// <param name="customParams">             The custom parameters with which to perform parameter
        ///                                         replacement in the project. </param>
        /// <param name="project">                  The project that finished generating. </param>
        /// <param name="parentItemId">             Identifier for the parent item. </param>

        void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams, Project project, uint parentItemId);
    }
}
