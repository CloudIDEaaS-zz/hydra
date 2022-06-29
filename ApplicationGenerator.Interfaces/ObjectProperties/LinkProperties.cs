// file:	FileProperties\LinkProperties.cs
//
// summary:	Implements the link properties class

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.ObjectProperties
{
    /// <summary>   A link properties. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/4/2021. </remarks>

    public class LinkProperties : BaseObjectProperties
    {
        /// <summary>   Gets or sets the link title. </summary>
        ///
        /// <value> The link title. </value>

        [Category("Search Engine Optimization Html")]
        [DisplayName("Call To Action")]
        public string LinkCallToAction { get; set; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/4/2021. </remarks>
        ///
        /// <param name="id">   The identifier. </param>

        public LinkProperties(string id) : base(id)
        {
        }

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/20/2021. </remarks>

        public LinkProperties() 
        {
        }
    }
}
