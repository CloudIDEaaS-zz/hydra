using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Builds
{
    /// <summary>   Interface for platform builder. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 4/9/2021. </remarks>

    public interface IPlatformBuilder
    {
        /// <summary>   Gets the priority. </summary>
        ///
        /// <value> The priority. </value>

        float Priority { get; }

        /// <summary>   Gets the name. </summary>
        ///
        /// <value> The name. </value>
        string Name { get; }

        /// <summary>   Gets the names. </summary>
        ///
        /// <value> The names. </value>

        string[] Names { get; }

        /// <summary>   Gets a value indicating whether the build succeeded. </summary>
        ///
        /// <value> True if build succeeded, false if not. </value>

        bool AllBuildsSucceeded { get; }

        /// <summary>   Gets the icon images. </summary>
        ///
        /// <value> The icon images. </value>

        Dictionary<string, Image> IconImages { get; }

        /// <summary>   Gets the build selections. </summary>
        ///
        /// <value> The build selections. </value>

        Dictionary<string, bool> BuildSelections { get; }

        /// <summary>   Gets the builds. </summary>
        ///
        /// <value> The builds. </value>

        Dictionary<string, BuildStats> Builds { get; }

        /// <summary>   Builds. </summary>
        ///
        /// <param name="generatorHandler"> The generator handler. </param>
        /// <param name="appLayout">        The application layout. </param>
        /// <param name="logWriter">        The log writer. </param>

        bool Build(string generatorHandler, IAppFolderStructureSurveyor appLayout, ILogWriter logWriter);
    }
}
