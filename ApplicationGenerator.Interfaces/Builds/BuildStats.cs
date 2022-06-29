// file:	Builds\BuildStats.cs
//
// summary:	Implements the build statistics class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Builds
{
    /// <summary>   A build statistics. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 4/27/2021. </remarks>

    public class BuildStats
    {
        /// <summary>   Gets or sets a value indicating whether the succeeded. </summary>
        ///
        /// <value> True if succeeded, false if not. </value>

        public bool Succeeded { get; set; }

        /// <summary>   Gets or sets the exceptions. </summary>
        ///
        /// <value> The exceptions. </value>

        public Exception[] Exceptions { get; set; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/27/2021. </remarks>
        ///
        /// <param name="succeeded">    True if succeeded, false if not. </param>
        /// <param name="exceptions">   The exceptions. </param>

        public BuildStats(bool succeeded, Exception[] exceptions)
        {
            this.Succeeded = succeeded;
            this.Exceptions = exceptions;
        }
    }
}
