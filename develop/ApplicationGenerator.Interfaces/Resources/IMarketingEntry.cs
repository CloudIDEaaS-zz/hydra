// file:	Resources\SocialMediaEntry.cs
//
// summary:	Implements the social media entry class

namespace AbstraX.Resources
{
    /// <summary>   Interface for marketing entry. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/7/2021. </remarks>

    public interface IMarketingEntry : IMarketingObject
    {
        /// <summary>   Gets or sets URL of the account. </summary>
        ///
        /// <value> The account URL. </value>

        string AccountUrl { get; set; }

        /// <summary>   Gets or sets the large logo. </summary>
        ///
        /// <value> The large logo. </value>

        string LargeLogo { get; set; }

        /// <summary>   Gets or sets the small logo. </summary>
        ///
        /// <value> The small logo. </value>

        string SmallLogo { get; set; }

        /// <summary>   Gets or sets the tiny logo. </summary>
        ///
        /// <value> The tiny logo. </value>

        string TinyLogo { get; set; }
    }
}