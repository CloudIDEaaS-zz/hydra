using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Builds
{
    /// <summary>   Interface for application builder. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 4/9/2021. </remarks>

    public interface IAppTargetsBuilder
    {
        /// <summary>   Event queue for all listeners interested in OnBuildResults events. </summary>
        event BuildResultHandler OnBuildResults;
        /// <summary>   Event queue for all listeners interested in OnCommand events. </summary>
        event EventHandlerT<string> OnCommand;
        /// <summary>   Gets supported platforms. </summary>
        ///
        /// <returns>   An array of i platform builder. </returns>

        IPlatformBuilder[] GetSupportedPlatforms();

        /// <summary>   Builds. </summary>
        ///
        /// <param name="generatorHandlerType"> The generator handler. </param>
        /// <param name="appLayout">        The application layout. </param>
        /// <param name="platforms">        The platforms. </param>
        /// <param name="logWriter">        The log writer. </param>

        void Build(string generatorHandlerType, IAppFolderStructureSurveyor appLayout, IPlatformBuilder[] platforms, ILogWriter logWriter);

        /// <summary>   Reports a build. </summary>
        ///
        /// <param name="builder">      The builder. </param>
        /// <param name="name">         The name. </param>
        /// <param name="succeeded">    True if the operation was a success, false if it failed. </param>
        /// <param name="exceptions">   The exceptions. </param>
        /// <param name="percentageOfName"></param>
        /// <param name="percentage"></param>

        void ReportBuild(IPlatformBuilder builder, string name, bool succeeded, Exception[] exceptions, string percentageOfName = null, int percentage = 0);

        /// <summary>   Reports a build. </summary>
        ///
        /// <param name="builder">          The builder. </param>
        /// <param name="name">             The name. </param>
        /// <param name="succeeded">        True if the operation was a success, false if it failed. </param>
        /// <param name="exceptions">       The exceptions. </param>
        /// <param name="percentageOfName"> (Optional) </param>
        /// <param name="percentage">       (Optional) </param>

        void ReportBuild(IPlatformBuilder builder, string name, bool succeeded, Exception[] exceptions, int percentage, params string[] percentageOfName);

        /// <summary>   Reports a command. </summary>
        ///
        /// <param name="command">  The command. </param>

        void ReportCommand(string command);
    }
}
