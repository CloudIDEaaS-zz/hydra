// file:	FileProperties\ImageProperties.cs
//
// summary:	Implements the image properties class

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.ObjectProperties
{
    /// <summary>   Page properties. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/4/2021. </remarks>

    public class PageProperties : BaseObjectProperties
    {
        /// <summary>   Gets or sets the page title. </summary>
        ///
        /// <value> The page title. </value>

        [Category("Search Engine Optimization Html")]
        [DisplayName("Page Title")]
        public string PageTitle { get; set; }

        /// <summary>   Gets or sets information describing the page metadata. </summary>
        ///
        /// <value> Information describing the page metadata. </value>

        [Category("Search Engine Optimization Html")]
        [DisplayName("Page Metadata Description")]
        public string PageMetadataDescription { get; set; }

        /// <summary>   Gets or sets the page metadata key words. </summary>
        ///
        /// <value> The page metadata key words. </value>

        [Category("Search Engine Optimization Html")]
        [DisplayName("Page Metadata Key Words")]
        public string PageMetadataKeyWords { get; set; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/4/2021. </remarks>
        ///
        /// <param name="id">   The identifier. </param>

        public PageProperties(string id) : base(id)
        {
        }
    }
}
