// file:	ITypeShimActivator.cs
//
// summary:	Declares the ITypeShimActivator interface

using CoreShim.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    /// <summary>   Interface for type shim activator. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/4/2020. </remarks>

    public interface ITypeShimActivator
    {
        /// <summary>   Creates an instance. </summary>
        ///
        /// <param name="type"> The type. </param>
        ///
        /// <returns>   The new instance. </returns>

        T CreateInstance<T>(TypeShim type) where T : class;
    }
}
