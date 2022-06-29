// file:	FileProperties\BaseObjectProperties.cs
//
// summary:	Implements the base object properties class

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.ObjectProperties
{
    /// <summary>   A base object properties. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/4/2021. </remarks>

    public class BaseObjectProperties
    {
        /// <summary>   Gets or sets the identifier. </summary>
        ///
        /// <value> The identifier. </value>
        [Category("Identification")]
        [ReadOnly(true)]

        public string ID { get; set; }

        /// <summary>   Gets or sets the name. </summary>
        ///
        /// <value> The name. </value>

        [Category("Identification")]
        public string Name { get; set; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/4/2021. </remarks>
        ///
        /// <param name="id">   The identifier. </param>

        public BaseObjectProperties(string id)
        {
            this.ID = id;
        }

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/20/2021. </remarks>

        public BaseObjectProperties()
        {
        }
    }
}
