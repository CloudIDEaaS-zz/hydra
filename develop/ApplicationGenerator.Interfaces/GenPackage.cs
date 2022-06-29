// file:	GenPackage.cs
//
// summary:	Implements the generate package class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    /// <summary>   (Serializable) a generate package. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>

    [Serializable]
    public class GenPackage
    {
        /// <summary>   Gets or sets the dependencies. </summary>
        ///
        /// <value> The dependencies. </value>

        public string[] dependencies { get; set; }

        /// <summary>   Gets or sets the development dependencies. </summary>
        ///
        /// <value> The development dependencies. </value>

        public string[] devDependencies { get; set; }
    }
}
