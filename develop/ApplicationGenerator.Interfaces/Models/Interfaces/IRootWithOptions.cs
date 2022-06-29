// file:	Models\Interfaces\IRootWithOptions.cs
//
// summary:	Declares the IRootWithOptions interface

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using AbstraX.ServerInterfaces;

namespace AbstraX.Models.Interfaces
{
    /// <summary>   Interface for root with options. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/21/2020. </remarks>

    public interface IRootWithOptions : IRoot
    {
        /// <summary>   Gets a value indicating whether this  is generated model. </summary>
        ///
        /// <value> True if this  is generated model, false if not. </value>

        bool IsGeneratedModel { get; }
    }
}
