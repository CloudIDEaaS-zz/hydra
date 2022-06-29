// file:	CustomAttributeArgs.cs
//
// summary:	Implements the custom attribute arguments class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    /// <summary>   Arguments for custom attribute. </summary>
    ///
    /// <remarks>   Ken, 10/10/2020. </remarks>

    public class CustomAttributeArgs
    {
        /// <summary>   Gets the argument values. </summary>
        ///
        /// <value> The argument values. </value>

        public List<object> ArgValues { get; }

        /// <summary>   Gets the named fields. </summary>
        ///
        /// <value> The named fields. </value>

        public List<FieldInfo> NamedFields { get; }

        /// <summary>   Gets the field values. </summary>
        ///
        /// <value> The field values. </value>

        public List<object> FieldValues { get; }

        /// <summary>   Gets the named properties. </summary>
        ///
        /// <value> The named properties. </value>

        public List<PropertyInfo> NamedProperties { get; }

        /// <summary>   Gets the property values. </summary>
        ///
        /// <value> The property values. </value>

        public List<object> PropertyValues { get; }

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>

        public CustomAttributeArgs()
        {
            ArgValues = new List<object>();
            NamedFields = new List<FieldInfo>();
            FieldValues = new List<object>();
            NamedProperties = new List<PropertyInfo>();
            PropertyValues = new List<object>();
        }
    }
}
