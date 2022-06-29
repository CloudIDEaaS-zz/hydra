// file:	ObjectProperties\MarketingObjectProperties.cs
//
// summary:	Implements the marketing object properties class

using AbstraX.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.ObjectProperties
{
    /// <summary>   An other. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/20/2021. </remarks>

    public class Other
    {
        /// <summary>   Gets or sets the tell others. </summary>
        ///
        /// <value> The tell others. </value>

        public TellOthers TellOthers { get; set; }

        /// <summary>   Gets or sets the email us link. </summary>
        ///
        /// <value> The email us link. </value>

        public string EmailUsLink { get; set; }

        /// <summary>   Gets or sets the advertising link. </summary>
        ///
        /// <value> The advertising link. </value>

        public string AdvertisingLink { get; set; }

        /// <summary>   Gets or sets the connect with us link. </summary>
        ///
        /// <value> The connect with us link. </value>

        public string ConnectWithUsLink { get; set; }
    }

    /// <summary>   A marketing object properties. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/20/2021. </remarks>

    public class MarketingObjectProperties
    {
        /// <summary>   Gets or sets the links. </summary>
        ///
        /// <value> The links. </value>

        public Dictionary<string, LinkProperties> Links { get; set; }

        /// <summary>   Gets or sets the social media. </summary>
        ///
        /// <value> The social media. </value>

        public SocialMediaList SocialMedia { get; set; }

        /// <summary>   Gets or sets the rate us. </summary>
        ///
        /// <value> The rate us. </value>

        public AppStoreList RateUs { get; set; }

        /// <summary>   Gets or sets the other links. </summary>
        ///
        /// <value> The other links. </value>

        public string[] OtherLinks { get; set; }

        /// <summary>   Gets or sets the other. </summary>
        ///
        /// <value> The other. </value>

        public Other Other { get; set; }
    }
}
