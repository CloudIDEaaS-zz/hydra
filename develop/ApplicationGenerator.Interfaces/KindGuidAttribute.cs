// file:	KindGuidAttribute.cs
//
// summary:	Implements the kind unique identifier attribute class

using AbstraX.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace AbstraX
{
    /// <summary>   Attribute for kind unique identifier. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 2/27/2021. </remarks>

    [AttributeUsage(AttributeTargets.Field)]
    public class KindGuidAttribute : Attribute
    {
        /// <summary>   Gets a unique identifier. </summary>
        ///
        /// <value> The identifier of the unique. </value>

        public Guid Guid { get; }

        /// <summary>   Gets the feature kind. </summary>
        ///
        /// <value> The feature kind. </value>

        public UIFeatureKind FeatureKind { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 2/27/2021. </remarks>
        ///
        /// <param name="guidValue">    The unique identifier value. </param>
        /// <param name="featureKind">  (Optional) The feature kind. </param>

        public KindGuidAttribute(string guidValue, UIFeatureKind featureKind = UIFeatureKind.Custom)
        {
            this.Guid = Guid.Parse(guidValue);
            this.FeatureKind = featureKind;
        }
    }
}
    