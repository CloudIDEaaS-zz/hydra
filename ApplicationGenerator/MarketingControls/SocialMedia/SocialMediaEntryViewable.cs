// file:	MarketingControls\SocialMedia\SocialMediaEntryProcessed.cs
//
// summary:	Implements the social media entry processed class

using AbstraX.Resources;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.MarketingControls.SocialMedia
{
    /// <summary>   A social media entry processed. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/7/2021. </remarks>

    public class SocialMediaEntryViewable : SocialMediaEntry
    {
        /// <summary>   Gets or sets the small logo. </summary>
        ///
        /// <value> The small logo. </value>

        public Image SmallLogoImage { get; set; }
    }
}
