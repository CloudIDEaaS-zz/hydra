// file:	IAppLayoutSurveyor.cs
//
// summary:	Declares the IAppLayoutSurveyor interface

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    /// <summary>   Interface for application layout surveyor. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 1/18/2021. </remarks>

    public interface IAppFolderStructureSurveyor
    {
        /// <summary>   Gets the builds. </summary>
        ///
        /// <value> The builds. </value>

        Dictionary<string, DirectoryInfo> Builds { get; }

        /// <summary>   Determine layout. </summary>
        ///
        /// <param name="path"> (Optional) Full pathname of the file. </param>

        void DetermineLayout(string path = null);

        /// <summary>   Determine layout. </summary>
        ///
        /// <param name="arguments">    The arguments. </param>

        void DetermineLayout(Dictionary<string, object> arguments);

        /// <summary>   Refreshes this.  </summary>
        void Refresh();

        /// <summary>   Gets the pathname of the top most folder. </summary>
        ///
        /// <value> The pathname of the top most folder. </value>

        string TopMostFolder { get; }

        /// <summary>   Gets the exceptions. </summary>
        ///
        /// <value> The exceptions. </value>

        List<Exception> Exceptions { get; }

        /// <summary>   Gets the name of the application. </summary>
        ///
        /// <value> The name of the application. </value>

        string AppName { get; set; }

        /// <summary>   Gets the hydra JSON file. </summary>
        ///
        /// <value> The hydra JSON file. </value>

        FileInfo HydraJsonFile { get; }

        /// <summary>   Gets the full pathname of the hydra JSON file. </summary>
        ///
        /// <value> The full pathname of the hydra JSON file. </value>

        string HydraJsonPath { get; }

        /// <summary>   Gets the configuration object. </summary>
        ///
        /// <value> The configuration object. </value>

        ConfigObject ConfigObject { get; }

        /// <summary>   Gets the full pathname of the web project file. </summary>
        ///
        /// <value> The full pathname of the web project file. </value>

        string WebProjectPath { get; }

        /// <summary>   Gets the full pathname of the web root file. </summary>
        ///
        /// <value> The full pathname of the web root file. </value>

        string WebFrontEndRootPath { get; }

        /// <summary>   Gets the business model file. </summary>
        ///
        /// <value> The business model file. </value>

        string BusinessModelFilePath { get; }

        /// <summary>   Gets the full pathname of the entities project file. </summary>
        ///
        /// <value> The full pathname of the entities project file. </value>

        string EntitiesProjectPath { get; }

        /// <summary>   Gets the full pathname of the services project file. </summary>
        ///
        /// <value> The full pathname of the services project file. </value>

        string ServicesProjectPath { get; }

        /// <summary>   Gets the project folder root. </summary>
        ///
        /// <value> The project folder root. </value>

        string ProjectFolderRoot { get; }

        /// <summary>   Gets the workspace file. </summary>
        ///
        /// <value> The workspace file. </value>

        FileInfo WorkspaceFile { get; }

        /// <summary>   Gets a value indicating whether the foundation generated. </summary>
        ///
        /// <value> True if foundation generated, false if not. </value>

        bool FoundationGenerated { get; }

        /// <summary>   Gets a value indicating whether the template generated. </summary>
        ///
        /// <value> True if template generated, false if not. </value>

        bool TemplateGenerated { get; }

        /// <summary>   Gets a value indicating whether the application generated. </summary>
        ///
        /// <value> True if application generated, false if not. </value>

        bool AppGenerated { get;  }

        /// <summary>   Creates the configuration. </summary>
        ///
        /// <returns>   The new configuration. </returns>

        ConfigObject CreateConfig(string generatorHandlerType);
    }
}
