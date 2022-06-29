// file:	Builds\BuildResult.cs
//
// summary:	Implements the build result class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Builds
{
    /// <summary>   Handler, called when the build result. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 4/10/2021. </remarks>
    ///
    /// <param name="sender">   Source of the event. </param>
    /// <param name="e">        Build result event information. </param>

    public delegate void BuildResultHandler(object sender, BuildResultEventArgs e);

    /// <summary>   Encapsulates the result of a build. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 4/10/2021. </remarks>

    public class BuildResultEventArgs : EventArgs
    {
        /// <summary>   Gets the builder. </summary>
        ///
        /// <value> The builder. </value>

        public IPlatformBuilder Builder { get; }

        /// <summary>   Gets the name. </summary>
        ///
        /// <value> The name. </value>

        public string Name { get; }

        /// <summary>   Gets a value indicating whether the succeeded. </summary>
        ///
        /// <value> True if succeeded, false if not. </value>

        public bool Succeeded { get; }

        /// <summary>   Gets the exceptions. </summary>
        ///
        /// <value> The exceptions. </value>

        public Exception[] Exceptions { get; }

        /// <summary>   Gets the name of the percentage of. </summary>
        ///
        /// <value> The name of the percentage of. </value>

        public string PercentageOfName { get; }

        /// <summary>   Gets a list of names of the percentage ofs. </summary>
        ///
        /// <value> A list of names of the percentage ofs. </value>

        public string[] PercentageOfNames { get; }

        /// <summary>   Gets the percentage. </summary>
        ///
        /// <value> The percentage. </value>

        public int Percentage { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/10/2021. </remarks>
        ///
        /// <param name="builder">          The builder. </param>
        /// <param name="name">             The name. </param>
        /// <param name="succeeded">        True if succeeded, false if not. </param>
        /// <param name="exceptions">       The exceptions. </param>
        /// <param name="percentageOfName"> (Optional) Name of the percentage of. </param>
        /// <param name="percentage">       (Optional) The percentage. </param>

        public BuildResultEventArgs(IPlatformBuilder builder, string name, bool succeeded, Exception[] exceptions, string percentageOfName = null, int percentage = 0)
        {
            this.Builder = builder;
            this.Name = name;
            this.Succeeded = succeeded;
            this.Exceptions = exceptions;
            this.PercentageOfName = percentageOfName;
            this.Percentage = percentage;
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/14/2021. </remarks>
        ///
        /// <param name="builder">              The builder. </param>
        /// <param name="name">                 The name. </param>
        /// <param name="succeeded">            True if succeeded, false if not. </param>
        /// <param name="exceptions">           The exceptions. </param>
        /// <param name="percentageOfNames">    (Optional) List of names of the percentage ofs. </param>
        /// <param name="percentage">           (Optional) The percentage. </param>

        public BuildResultEventArgs(IPlatformBuilder builder, string name, bool succeeded, Exception[] exceptions, string[] percentageOfNames = null, int percentage = 0)
        {
            this.Builder = builder;
            this.Name = name;
            this.Succeeded = succeeded;
            this.Exceptions = exceptions;
            this.PercentageOfNames = percentageOfNames;
            this.Percentage = percentage;
        }
    }
}
