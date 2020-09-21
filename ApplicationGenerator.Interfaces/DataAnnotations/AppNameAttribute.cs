// file:	DataAnnotations\AppNameAttribute.cs
//
// summary:	Implements the application name attribute class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.DataAnnotations
{
    /// <summary>   Attribute for application name. </summary>
    ///
    /// <remarks>   Ken, 9/20/2020. </remarks>

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AppNameAttribute : Attribute
    {
        /// <summary>   Gets or sets the name. </summary>
        ///
        /// <value> The name. </value>

        public string Name { get; set; }

        /// <summary>   Gets or sets the description. </summary>
        ///
        /// <value> The description. </value>

        public string Description { get; set; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 9/20/2020. </remarks>
        ///
        /// <param name="name">         The name. </param>
        /// <param name="description">  The description. </param>

        public AppNameAttribute(string name, string description)
        {
            this.Name = name;
            this.Description = description;
        }
    }
}
