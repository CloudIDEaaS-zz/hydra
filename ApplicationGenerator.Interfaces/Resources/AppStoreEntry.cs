// file:	Resources\AppStoreEntry.cs
//
// summary:	Implements the application store entry class

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Resources
{
    /// <summary>   An application store entry. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/7/2021. </remarks>

    public class AppStoreEntry : IMarketingEntry, IComparable<AppStoreEntry>
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

        /// <summary>   Gets or sets the device ratings operating system. </summary>
        ///
        /// <value> The device ratings operating system. </value>

        public string DeviceRatingsOs { get; set; }

        /// <summary>   Gets or sets the allow rating. </summary>
        ///
        /// <value> The allow rating. </value>

        public bool AllowRating { get; set; }

        /// <summary>   Gets or sets the call to action. </summary>
        ///
        /// <value> The call to action. </value>

        public string CallToAction { get; set; }

        /// <summary>   Gets or sets URL of the site. </summary>
        ///
        /// <value> The site URL. </value>

        public string SiteUrl { get; set; }

        /// <summary>   Gets or sets URL of the submit application. </summary>
        ///
        /// <value> The submit application URL. </value>

        public string SubmitAppUrl { get; set; }

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

        /// <summary>   Returns a string that represents the current object. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/8/2021. </remarks>
        ///
        /// <returns>   A string that represents the current object. </returns>

        public override string ToString()
        {
            return this.Name;
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer
        /// that indicates whether the current instance precedes, follows, or occurs in the same position
        /// in the sort order as the other object.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/19/2021. </remarks>
        ///
        /// <param name="other">    An object to compare with this instance. </param>
        ///
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has
        /// these meanings: Value Meaning Less than zero This instance precedes <paramref name="other" />
        /// in the sort order.  Zero This instance occurs in the same position in the sort order as
        /// <paramref name="other" />. Greater than zero This instance follows <paramref name="other" />
        /// in the sort order.
        /// </returns>

        public int CompareTo(AppStoreEntry other)
        {
            return this.SiteUrl.CompareTo(other.SiteUrl);
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/15/2021. </remarks>
        ///

        public AppStoreEntry()
        {
        }
    }
}
