// file:	FileProperties\ImageProperties.cs
//
// summary:	Implements the image properties class

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.ObjectProperties
{
    /// <summary>   An image properties. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/4/2021. </remarks>

    public class ImageProperties : BaseFileProperties
    {
        /// <summary>   Gets or sets the identifier of the image. </summary>
        ///
        /// <value> The identifier of the image. </value>

        [Category("Search Engine Optimization Image")]
        [DisplayName("Image Id")]
        public string ImageId { get; set; }

        /// <summary>   Gets or sets the alternative text. </summary>
        ///
        /// <value> The alternative text. </value>

        [Category("Search Engine Optimization Html")]
        [DisplayName("Alternative Text")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string AlternativeText { get; set; }

        /// <summary>   Gets or sets information describing the long. </summary>
        ///
        /// <value> Information describing the long. </value>

        [Category("Search Engine Optimization Html")]
        [DisplayName("Long Description")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string LongDescription { get; set; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/4/2021. </remarks>
        ///
        /// <param name="id">   The identifier. </param>

        public ImageProperties(string id) : base(id)
        {
        }
    }
}
