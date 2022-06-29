// file:	UILoadKindAugmenterAttribute.cs
//
// summary:	Implements the load kind augmenter attribute class

using AbstraX.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    /// <summary>   Attribute for load kind augmenter. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 2/27/2021. </remarks>

    public class UILoadKindAugmenterAttribute : Attribute
    {
        /// <summary>   Gets the load kind. </summary>
        ///
        /// <value> The user interface load kind. </value>

        public UILoadKind UILoadKind { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 2/27/2021. </remarks>
        ///
        /// <param name="uILoadKind">   The i load kind. </param>

        public UILoadKindAugmenterAttribute(UILoadKind uILoadKind)
        {
            this.UILoadKind = uILoadKind;
        }
    }
}
