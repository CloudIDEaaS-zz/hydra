// file:	Resources\Dimensions.cs
//
// summary:	Implements the dimensions class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Resources
{
    /// <summary>   A dimensions. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 4/11/2021. </remarks>

    public class Dimensions
    {
        /// <summary>   Gets or sets the splash screen. </summary>
        ///
        /// <value> The splash screen. </value>

        public Dimension SplashScreen { get; set; }

        /// <summary>   Gets or sets the about banner. </summary>
        ///
        /// <value> The about banner. </value>

        public Dimension AboutBanner { get; set; }

        /// <summary>   Gets or sets the logo. </summary>
        ///
        /// <value> The logo. </value>

        public Dimension Logo { get; set; }

        /// <summary>   Gets or sets the icon. </summary>
        ///
        /// <value> The icon. </value>

        public Dimension Icon { get; set; }
    }
}
