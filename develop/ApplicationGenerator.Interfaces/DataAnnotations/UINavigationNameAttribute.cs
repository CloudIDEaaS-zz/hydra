// file:	DataAnnotations\UINavigationNameAttribute.cs
//
// summary:	Implements the navigation name attribute class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.DataAnnotations
{
    /// <summary>   Attribute for navigation name. </summary>
    ///
    /// <remarks>   Ken, 10/10/2020. </remarks>

    [AttributeUsage(AttributeTargets.Class, AllowMultiple =true)]
    public class UINavigationNameAttribute : Attribute
    {
        /// <summary>   Gets or sets the name. </summary>
        ///
        /// <value> The name. </value>

        public string Name { get; set; }

        /// <summary>   Gets the kind. </summary>
        ///
        /// <value> The user interface kind. </value>

        public UIKind UIKind { get; }

        /// <summary>   Gets the load kind. </summary>
        ///
        /// <value> The user interface load kind. </value>

        public UILoadKind UILoadKind { get; }

        /// <summary>   Gets or sets the full pathname of the hierarchy file. </summary>
        ///
        /// <value> The full pathname of the hierarchy file. </value>

        public string UIHierarchyPath { get; set; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="name"> The name. </param>

        public UINavigationNameAttribute(string name)
        {
            this.Name = name;
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="name">     The name. </param>
        /// <param name="uiKind">   The kind. </param>

        public UINavigationNameAttribute(string name, UIKind uiKind)
        {
            this.Name = name;
            this.UIKind = uiKind;
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="name">         The name. </param>
        /// <param name="uiKind">       The kind. </param>
        /// <param name="uiLoadKind">   The load kind. </param>

        public UINavigationNameAttribute(string name, UIKind uiKind, UILoadKind uiLoadKind)
        {
            this.Name = name;
            this.UIKind = uiKind;
            this.UILoadKind = uiLoadKind;
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="uiHierarchyPath">  Full pathname of the hierarchy file. </param>
        /// <param name="name">             The name. </param>

        public UINavigationNameAttribute(string uiHierarchyPath, string name)
        {
            this.UIHierarchyPath = uiHierarchyPath;
            this.Name = name;
        }
    }
}
