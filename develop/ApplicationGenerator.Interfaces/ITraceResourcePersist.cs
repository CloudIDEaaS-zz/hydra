// file:	ITraceResourcePersist.cs
//
// summary:	Declares the ITraceResourcePersist interface

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    /// <summary>   Interface for trace resource persist. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/13/2021. </remarks>

    public interface ITraceResourcePersist
    {
        /// <summary>   Gets or sets the trace resource last hash. </summary>
        ///
        /// <value> The trace resource last hash. </value>

        string TraceResourceLastHash { get; set; }

        /// <summary>   Gets or sets the trace resource. </summary>
        ///
        /// <value> The trace resource. </value>

        string TraceResourceDocument { get; set; }
        /// <summary>   Saves this. </summary>
        void Save();
    }
}
