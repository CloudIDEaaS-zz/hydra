// file:	WorkspaceTemplateParameters.cs
//
// summary:	Implements the workspace template parameters class

using CodeInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    /// <summary>   A workspace template parameters. </summary>
    ///
    /// <remarks>   Ken, 10/3/2020. </remarks>

    public class WorkspaceTemplateParameters : ICodeTemplateParameters
    {
        /// <summary>   Gets or sets the name of the application. </summary>
        ///
        /// <value> The name of the application. </value>

        public string AppName { get; set; }

        /// <summary>   Gets or sets information describing the application. </summary>
        ///
        /// <value> Information describing the application. </value>

        public string AppDescription { get; set; }

        /// <summary>   Gets or sets the name of the project. </summary>
        ///
        /// <value> The name of the project. </value>

        public string ProjectName { get; set; }

        /// <summary>   Gets or sets the framework version. </summary>
        ///
        /// <value> The framework version. </value>

        public string FrameworkVersion { get; set; }

        /// <summary>   Gets or sets the registered organization. </summary>
        ///
        /// <value> The registered organization. </value>

        public string RegisteredOrganization { get; set; }

        /// <summary>   Gets or sets the name of the solution. </summary>
        ///
        /// <value> The name of the solution. </value>

        public string SolutionName { get; set; }

        /// <summary>   Gets or sets the copyright year. </summary>
        ///
        /// <value> The copyright year. </value>

        public string CopyrightYear { get; set; }

        public Dictionary<string, string> CustomParameters { get; }

        public WorkspaceTemplateParameters()
        {
            this.CustomParameters = new Dictionary<string, string>();
        }
    }
}
