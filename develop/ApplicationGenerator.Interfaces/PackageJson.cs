// file:	PackageJson.cs
//
// summary:	Implements the package JSON class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    /// <summary>   A package json. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>

    public class PackageJson
    {
        /// <summary>   Gets or sets the name. </summary>
        ///
        /// <value> The name. </value>

        public string Name { get; set; }

        /// <summary>   Gets or sets the version. </summary>
        ///
        /// <value> The version. </value>

        public string Version { get; set; }

        /// <summary>   Gets or sets the description. </summary>
        ///
        /// <value> The description. </value>

        public string Description { get; set; }

        /// <summary>   Gets or sets the name of the display. </summary>
        ///
        /// <value> The name of the display. </value>

        public string DisplayName { get; set; }

        /// <summary>   Gets or sets the contributors. </summary>
        ///
        /// <value> The contributors. </value>

        public object Contributors { get; set; }

        /// <summary>   Gets or sets the author. </summary>
        ///
        /// <value> The author. </value>

        public object Author { get; set; }

        /// <summary>   Gets or sets the module. </summary>
        ///
        /// <value> The module. </value>

        public string Module { get; set; }

        /// <summary>   Gets or sets the es 2015. </summary>
        ///
        /// <value> The es 2015. </value>

        public string Es2015 { get; set; }

        /// <summary>   Gets or sets the es 2017. </summary>
        ///
        /// <value> The es 2017. </value>

        public string Es2017 { get; set; }

        /// <summary>   Gets or sets the license. </summary>
        ///
        /// <value> The license. </value>

        public string License { get; set; }

        /// <summary>   Gets or sets the dependencies. </summary>
        ///
        /// <value> The dependencies. </value>

        public Dictionary<string, string> Dependencies { get; set; }

        /// <summary>   Gets or sets the development dependencies. </summary>
        ///
        /// <value> The development dependencies. </value>

        public Dictionary<string, string> DevDependencies { get; set; }

        /// <summary>   Gets or sets the peer dependencies. </summary>
        ///
        /// <value> The peer dependencies. </value>

        public Dictionary<string, string> PeerDependencies { get; set; }
    }
}
