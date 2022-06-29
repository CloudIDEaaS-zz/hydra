// file:	TemplateObjects\AttributeObject.cs
//
// summary:	Implements the attribute object class

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.TemplateObjects
{
    /// <summary>   An attribute object. </summary>
    ///
    /// <remarks>   Ken, 10/3/2020. </remarks>

    [DebuggerDisplay(" { Name } ")]
    public class AttributeObject : EntityBaseObject
    {
        /// <summary>   Gets or sets the type of the attribute. </summary>
        ///
        /// <value> The type of the attribute. </value>

        public string AttributeType { get; set; }

        /// <summary>   Gets or sets the name of the dynamic property. </summary>
        ///
        /// <value> The name of the dynamic property. </value>

        [JsonIgnore]
        public string DynamicPropertyName { get; set; }

        /// <summary>   Gets or sets the type of the dynamic property. </summary>
        ///
        /// <value> The type of the dynamic property. </value>

        [JsonIgnore]
        public Type DynamicPropertyType { get; set; }

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>

        public AttributeObject()
        {
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="name">             The name. </param>
        /// <param name="attributeType">    Type of the attribute. </param>

        public AttributeObject(string name, string attributeType) : base(name)
        {
            this.AttributeType = attributeType;
        }
    }
}
