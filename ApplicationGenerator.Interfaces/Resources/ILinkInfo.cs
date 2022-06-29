// file:	Resources\ILinkInfo.cs
//
// summary:	Declares the ILinkInfo interface

using HtmlAgilityPack;

namespace AbstraX.Resources
{
    /// <summary>   Interface for link information. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/19/2021. </remarks>

    public interface ILinkInfo
    {
        /// <summary>   Gets the HTML link. </summary>
        ///
        /// <value> The HTML link. </value>

        HtmlNode HtmlLink { get; }

        /// <summary>   Gets the name of the resource. </summary>
        ///
        /// <value> The name of the resource. </value>

        string ResourceName { get; }

        /// <summary>   Gets the full pathname of the URL file. </summary>
        ///
        /// <value> The full pathname of the URL file. </value>

        string UrlPath { get; }
    }
}