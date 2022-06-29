// file:	Resources\IMarketingObject.cs
//
// summary:	Declares the IMarketingObject interface

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Resources
{
    /// <summary>   Interface for marketing object. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/13/2021. </remarks>

    public interface IMarketingObject
    {
        /// <summary>   Gets or sets the name. </summary>
        ///
        /// <value> The name. </value>

        string Name { get; set; }

        /// <summary>   Gets or sets the pathname of the working directory. </summary>
        ///
        /// <value> The pathname of the working directory. </value>

        string WorkingDirectory { get; set; }

        /// <summary>   Gets the provider. </summary>
        ///
        /// <value> The provider. </value>

        IMarketingObjectAttributeProvider Provider { get; }
    }
}
