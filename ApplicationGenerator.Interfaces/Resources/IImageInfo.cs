// file:	Resources\IImageInfo.cs
//
// summary:	Declares the IImageInfo interface

using HtmlAgilityPack;

namespace AbstraX.Resources
{
    /// <summary>   Interface for image information. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/19/2021. </remarks>

    public interface IImageInfo
    {
        /// <summary>   Gets or sets the alternate. </summary>
        ///
        /// <value> The alternate. </value>

        string Alt { get; set; }

        /// <summary>   Gets the full pathname of the copy to file. </summary>
        ///
        /// <value> The full pathname of the copy to file. </value>

        string CopyToFilePath { get; }

        /// <summary>   Gets or sets the description. </summary>
        ///
        /// <value> The description. </value>

        string Description { get; set; }

        /// <summary>   Gets or sets the description HTML. </summary>
        ///
        /// <value> The description HTML. </value>

        string DescriptionHtml { get; set; }

        /// <summary>   Gets or sets the name of the description page. </summary>
        ///
        /// <value> The name of the description page. </value>

        string DescriptionPageName { get; set; }

        /// <summary>   Gets the HTML image. </summary>
        ///
        /// <value> The HTML image. </value>

        HtmlNode HtmlImage { get; }

        /// <summary>   Gets or sets the object properties. </summary>
        ///
        /// <value> The object properties. </value>

        ObjectPropertiesDictionary ObjectProperties { get; set; }

        /// <summary>   Gets the full pathname of the original file. </summary>
        ///
        /// <value> The full pathname of the original file. </value>

        string OriginalPath { get; }

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