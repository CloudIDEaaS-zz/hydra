// file:	Resources\SocialMediaEntry.cs
//
// summary:	Implements the social media entry class

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Resources
{
    /// <summary>   A social media entry. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/7/2021. </remarks>

    public class SocialMediaEntry : IMarketingEntry
    {
        private IMarketingObjectAttributeProvider provider;
        private string workingDirectory;

        /// <summary>   Gets or sets the name. </summary>
        ///
        /// <value> The name. </value>

        public string Name { get; set; }

        /// <summary>   Gets or sets the small logo. </summary>
        ///
        /// <value> The small logo. </value>

        public string SmallLogo { get; set; }

        /// <summary>   Gets or sets the large logo. </summary>
        ///
        /// <value> The large logo. </value>

        public string LargeLogo { get; set; }

        /// <summary>   Gets or sets the tiny logo. </summary>
        ///
        /// <value> The tiny logo. </value>

        public string TinyLogo { get; set; }

        /// <summary>   Gets or sets URL of the account. </summary>
        ///
        /// <value> The account URL. </value>

        public string AccountUrl { get; set; }

        /// <summary>   Gets or sets URL of the ahare. </summary>
        ///
        /// <value> The ahare URL. </value>

        public string ShareUrl { get; set; }

        /// <summary>   Gets or sets the visit call to action. </summary>
        ///
        /// <value> The visit call to action. </value>

        public string VisitCallToAction { get; set; }

        /// <summary>   Gets or sets the share call to action. </summary>
        ///
        /// <value> The share call to action. </value>

        public string ShareCallToAction { get; set; }

        /// <summary>   Gets or sets URL of the site. </summary>
        ///
        /// <value> The site URL. </value>

        public string SiteUrl { get; set; }

        /// <summary>   Gets or sets a value indicating whether this  is enabled. </summary>
        ///
        /// <value> True if enable, false if not. </value>

        public bool Enable { get; set; }

        /// <summary>   Gets or sets the pathname of the working directory. </summary>
        ///
        /// <value> The pathname of the working directory. </value>

        [JsonIgnore]
        public string WorkingDirectory 
        {
            get
            {
                return workingDirectory;
            }

            set
            {
                workingDirectory = value;
            }
        }

        /// <summary>   Gets the provider. </summary>
        ///
        /// <value> The provider. </value>

        [JsonIgnore]
        public IMarketingObjectAttributeProvider Provider
        {
            get
            {
                if (provider == null)
                {
                    provider = this.GetMarketingObjectAttributeProvider();
                }

                return provider;
            }
        }

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/15/2021. </remarks>

        public SocialMediaEntry()
        {
        }
    }
}
