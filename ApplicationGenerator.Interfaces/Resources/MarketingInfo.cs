// file:	Resources\MarketingInfo.cs
//
// summary:	Implements the marketing information class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Resources
{
    /// <summary>   Information about the marketing. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/7/2021. </remarks>

    public class MarketingInfo
    {
        /// <summary>   Gets or sets a list of social medias. </summary>
        ///
        /// <value> A list of social medias. </value>

        public SocialMediaList SocialMediaList { get; set; }

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/7/2021. </remarks>

        public MarketingInfo()
        {
            this.SocialMediaList = new SocialMediaList();
        }
    }
}
