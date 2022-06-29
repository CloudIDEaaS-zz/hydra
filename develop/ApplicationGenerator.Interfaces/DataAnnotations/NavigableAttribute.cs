// file:	DataAnnotations\NavigableAttribute.cs
//
// summary:	Implements the navigable attribute class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.DataAnnotations
{
    /// <summary>   Attribute for navigable. </summary>
    ///
    /// <remarks>   Ken, 10/10/2020. </remarks>

    public abstract class NavigableAttribute : Attribute
    {
        /// <summary>   Gets or sets the full pathname of the hierarchy file. </summary>
        ///
        /// <value> The full pathname of the hierarchy file. </value>

        public string UIHierarchyPath { get; set; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="uiHierarchyPath">  Full pathname of the hierarchy file. </param>

        public NavigableAttribute(string uiHierarchyPath)
        {
            this.UIHierarchyPath = uiHierarchyPath;
        }
    }
}
