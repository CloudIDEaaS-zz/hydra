// file:	Resources\IMarketingObjectAttributeProvider.cs
//
// summary:	Declares the IMarketingObjectAttributeProvider interface

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Resources
{
    /// <summary>   Interface for marketing object attribute provider. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/13/2021. </remarks>

    public interface IMarketingObjectAttributeProvider
    {
        /// <summary>   Gets or sets the supported replacements. </summary>
        ///
        /// <value> The supported replacements. </value>

        Dictionary<string, List<string>> SupportedVariables { get; set; }

        /// <summary>   Gets the attributes. </summary>
        ///
        /// <param name="property"> The property. </param>
        ///
        /// <returns>   An array of attribute. </returns>

        Attribute[] GetAttributes(string property);
    }
}
