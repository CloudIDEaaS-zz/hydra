// file:	FileProperties\BaseFileProperties.cs
//
// summary:	Implements the base file properties class

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.ObjectProperties
{
    /// <summary>   A base file properties. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/4/2021. </remarks>

    public class BaseFileProperties : BaseObjectProperties
    {
        /// <summary>   Gets or sets the filename of the source file. </summary>
        ///
        /// <value> The filename of the source file. </value>
        [Category("Identification")]
        [DisplayName("Source File Name")]
        [ReadOnly(true)]
        public string SourceFileName { get; set; }

        /// <summary>   Gets or sets the filename of the target file. </summary>
        ///
        /// <value> The filename of the target file. </value>

        [Category("Identification")]
        [DisplayName("Target File Name")]
        [ReadOnly(true)]
        public string TargetFileName { get; set; }

        /// <summary>   Gets or sets the author. </summary>
        ///
        /// <value> The author. </value>

        [Category("Search Engine Optimization File")]
        public string Author { get; set; }

        /// <summary>   Gets or sets the category. </summary>
        ///
        /// <value> The category. </value>

        [Category("Search Engine Optimization File")]
        public string[] Category { get; set; }

        /// <summary>   Gets or sets the kind. </summary>
        ///
        /// <value> The kind. </value>

        [Category("Search Engine Optimization File")]
        public string[] Kind { get; set; }

        /// <summary>   Gets or sets the keywords. </summary>
        ///
        /// <value> The keywords. </value>

        [Category("Search Engine Optimization File")]
        public string[] Keywords { get; set; }

        /// <summary>   Gets or sets the comment. </summary>
        ///
        /// <value> The comment. </value>

        [Category("Search Engine Optimization File")]
        public string Comment { get; set; }

        /// <summary>   Gets or sets the company. </summary>
        ///
        /// <value> The company. </value>

        [Category("Search Engine Optimization File")]
        public string Company { get; set; }

        /// <summary>   Gets or sets the project. </summary>
        ///
        /// <value> The project. </value>

        [Category("Search Engine Optimization File")]
        public string Project { get; set; }

        /// <summary>   Gets or sets URL of the item. </summary>
        ///
        /// <value> The item URL. </value>

        [Category("Search Engine Optimization File")]
        public string ItemUrl { get; set; }

        /// <summary>   Gets or sets the rating. </summary>
        ///
        /// <value> The rating. </value>

        [Category("Search Engine Optimization File")]
        public int Rating { get; set; }

        /// <summary>   Gets or sets the rating text. </summary>
        ///
        /// <value> The rating text. </value>

        [Category("Search Engine Optimization File")]
        [DisplayName("Rating Text")]
        public string RatingText { get; set; }

        /// <summary>   Gets or sets the title. </summary>
        ///
        /// <value> The title. </value>

        [Category("Search Engine Optimization File")]
        public string Title { get; set; }

        /// <summary>   Gets or sets the information tip text. </summary>
        ///
        /// <value> The information tip text. </value>

        [Category("Search Engine Optimization File")]
        [DisplayName("InfoTip Text")]
        public string InfoTipText { get; set; }

        /// <summary>   Gets or sets the importance text. </summary>
        ///
        /// <value> The importance text. </value>

        [Category("Search Engine Optimization File")]
        [DisplayName("Importance Text")]
        public string ImportanceText { get; set; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/4/2021. </remarks>
        ///
        /// <param name="id">   The identifier. </param>

        public BaseFileProperties(string id) : base(id)
        {
        }
    }
}
