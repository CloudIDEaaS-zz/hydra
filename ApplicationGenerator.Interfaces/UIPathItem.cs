// file:	UIPathItem.cs
//
// summary:	Implements the path item class

using AbstraX.ServerInterfaces;
using AbstraX.XPathBuilder;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    /// <summary>   A path item. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/21/2020. </remarks>

    [DebuggerDisplay(" { DebugInfo }")]
    public class UIPathItem : IComparable
    {
        /// <summary>   Gets or sets target base object. </summary>
        ///
        /// <value> The target base object. </value>

        public IBase TargetBaseObject { get; set; }

        /// <summary>   Gets or sets the full pathname of the file. </summary>
        ///
        /// <value> The full pathname of the file. </value>

        public string Path { get; set; }

        /// <summary>   Gets or sets the name. </summary>
        ///
        /// <value> The name. </value>

        public string Name { get; set; }

        /// <summary>   Gets or sets the predicates. </summary>
        ///
        /// <value> The predicates. </value>

        public List<XPathPredicate> Predicates { get; set; }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer
        /// that indicates whether the current instance precedes, follows, or occurs in the same position
        /// in the sort order as the other object.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/21/2020. </remarks>
        ///
        /// <param name="obj">  An object to compare with this instance. </param>
        ///
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has
        /// these meanings: Value Meaning Less than zero This instance precedes <paramref name="obj" />
        /// in the sort order. Zero This instance occurs in the same position in the sort order as
        /// <paramref name="obj" />. Greater than zero This instance follows <paramref name="obj" /> in
        /// the sort order.
        /// </returns>

        public int CompareTo(object obj)
        {
            return string.Compare(this.Path, ((UIPathItem)obj).Path);
        }

        /// <summary>   Gets information describing the debug. </summary>
        ///
        /// <value> Information describing the debug. </value>

        public string DebugInfo
        {
            get
            {
                return this.Path;
            }
        }
    }
}
