// file:	Resources\IEmailLink.cs
//
// summary:	Declares the IEmailLink interface

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Resources
{
    /// <summary>   Interface for email link. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/13/2021. </remarks>

    public interface IEmailLink : IMarketingObject
    {
        /// <summary>   Gets or sets URL of the document. </summary>
        ///
        /// <value> The URL. </value>

        string Url { get; set; }
    }
}
