// file:	Resources\MarketingObjectAttribute.cs
//
// summary:	Implements the marketing object attribute class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Resources
{
    /// <summary>   Attribute for marketing object. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/13/2021. </remarks>

    public class MarketingObjectAttribute : Attribute
    {
        /// <summary>   Gets a list of names of the properties. </summary>
        ///
        /// <value> A list of names of the properties. </value>

        public string[] PropertyNames { get; }

        /// <summary>   Gets the name of the object. </summary>
        ///
        /// <value> The name of the object. </value>

        public string ObjectName { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/13/2021. </remarks>
        ///
        /// <param name="objectName">       Name of the object. </param>
        /// <param name="propertyNames">    Name of the property. </param>

        public MarketingObjectAttribute(string objectName, string[] propertyNames = null)
        {
            this.ObjectName = objectName;
            this.PropertyNames = propertyNames;
        }

    }
}
