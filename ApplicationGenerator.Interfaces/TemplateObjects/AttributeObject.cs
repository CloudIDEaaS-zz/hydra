// file:	TemplateObjects\AttributeObject.cs
//
// summary:	Implements the attribute object class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.TemplateObjects
{
    /// <summary>   An attribute object. </summary>
    ///
    /// <remarks>   Ken, 10/3/2020. </remarks>

    public class AttributeObject : EntityBaseObject
    {
        /// <summary>   Gets or sets the type of the attribute. </summary>
        ///
        /// <value> The type of the attribute. </value>

        public string AttributeType { get; set; }

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
