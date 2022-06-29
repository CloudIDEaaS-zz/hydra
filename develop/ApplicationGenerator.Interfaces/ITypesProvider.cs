// file:	ITypesProvider.cs
//
// summary:	Declares the ITypesProvider interface

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    /// <summary>   Interface for types provider. </summary>
    ///
    /// <remarks>   Ken, 11/1/2020. </remarks>

    public interface ITypesProvider
    {
        /// <summary>   Searches for the first type. </summary>
        ///
        /// <param name="fullName"> Name of the full. </param>
        ///
        /// <returns>   The found type. </returns>

        Type FindType(string fullName);
    }
}
