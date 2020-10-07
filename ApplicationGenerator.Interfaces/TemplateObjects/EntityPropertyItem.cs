// file:	TemplateObjects\EntityPropertyItem.cs
//
// summary:	Implements the entity property item class

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.TemplateObjects
{
    /// <summary>   An entity property item. </summary>
    ///
    /// <remarks>   Ken, 10/3/2020. </remarks>

    public class EntityPropertyItem
    {
        /// <summary>   Gets or sets the name of the property. </summary>
        ///
        /// <value> The name of the property. </value>

        [JsonProperty(Order = 0)]
        public string PropertyName { get; set; }

        /// <summary>   Gets or sets the property value. </summary>
        ///
        /// <value> The property value. </value>

        [JsonProperty(Order = 1)]
        public string PropertyValue { get; set; }
        /// <summary>   . </summary>
        [JsonProperty(Order = 2), TsvParserChildRecursion(ItemType = typeof(EntityPropertyItem), ChildRecursionParameters = new object[] { "--RecurseColumns", "--RecursionChildProperty", "ChildProperties" })]
        public List<EntityPropertyItem> ChildProperties { get; set; }

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
    }
}
