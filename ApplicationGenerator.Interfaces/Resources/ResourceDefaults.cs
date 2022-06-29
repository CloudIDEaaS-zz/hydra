// file:	Resources\ResourceDefaults.cs
//
// summary:	Implements the resource defaults class

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Resources
{
    /// <summary>   A resource defaults. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 4/11/2021. </remarks>

    public class ResourceDefaults
    {
        /// <summary>   Gets or sets the dimensions. </summary>
        ///
        /// <value> The dimensions. </value>

        public Dimensions Dimensions { get; set; }

        /// <summary>   Gets or sets the about banner. </summary>
        ///
        /// <value> The about banner. </value>

        public Image AboutBanner { get; set; }

        /// <summary>   Gets or sets the logo. </summary>
        ///
        /// <value> The logo. </value>

        public Image Logo { get; set; }

        /// <summary>   Gets or sets the splash screen. </summary>
        ///
        /// <value> The splash screen. </value>

        public Image SplashScreen { get; set; }

        /// <summary>   Gets or sets the about banner file. </summary>
        ///
        /// <value> The about banner file. </value>

        public string AboutBannerFile { get; internal set; }

        /// <summary>   Gets or sets the logo file. </summary>
        ///
        /// <value> The logo file. </value>

        public string LogoFile { get; internal set; }

        /// <summary>   Gets or sets the splash screen file. </summary>
        ///
        /// <value> The splash screen file. </value>

        public string SplashScreenFile { get; internal set; }
    }
}
