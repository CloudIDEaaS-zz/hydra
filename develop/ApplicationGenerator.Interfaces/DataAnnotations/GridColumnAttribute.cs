// file:	DataAnnotations\GridColumnAttribute.cs
//
// summary:	Implements the grid column attribute class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.DataAnnotations
{
    /// <summary>   Attribute for grid column. </summary>
    ///
    /// <remarks>   Ken, 10/11/2020. </remarks>

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class GridColumnAttribute : NavigableAttribute
    {
        /// <summary>   Gets the grid column kind. </summary>
        ///
        /// <value> The grid column kind. </value>

        public GridColumnKind GridColumnKind { get; }

        /// <summary>   Gets a value indicating whether this  is text identifier. </summary>
        ///
        /// <value> True if this  is text identifier, false if not. </value>

        public bool IsTextIdentifier { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>
        ///
        /// <param name="uiHierarchyPath">  Full pathname of the hierarchy file. </param>
        /// <param name="kind">             (Optional) The kind. </param>

        public GridColumnAttribute(string uiHierarchyPath, GridColumnKind kind = GridColumnKind.Text) : base(uiHierarchyPath)
        {
            this.GridColumnKind = kind;
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>
        ///
        /// <param name="uiHierarchyPath">  Full pathname of the hierarchy file. </param>
        /// <param name="isTextIdentifier"> True if is text identifier, false if not. </param>
        /// <param name="kind">             (Optional) The kind. </param>

        public GridColumnAttribute(string uiHierarchyPath, bool isTextIdentifier, GridColumnKind kind = GridColumnKind.Text) : base(uiHierarchyPath)
        {
            this.GridColumnKind = kind;
            this.IsTextIdentifier = isTextIdentifier;
        }
    }
}
