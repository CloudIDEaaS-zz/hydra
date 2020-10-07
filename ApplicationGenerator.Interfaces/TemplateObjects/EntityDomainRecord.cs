// file:	TemplateObjects\EntityDomainRecord.cs
//
// summary:	Implements the entity domain record class

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.TemplateObjects
{
    /// <summary>   Information about the entity domain. </summary>
    ///
    /// <remarks>   Ken, 10/2/2020. </remarks>

    public class EntityDomainRecord
    {
        /// <summary>   Gets or sets the name of the entity. </summary>
        ///
        /// <value> The name of the entity. </value>

        [JsonProperty(Order = 0)]
        public string EntityName { get; set; }

        /// <summary>   Gets or sets the parent data item. </summary>
        ///
        /// <value> The parent data item. </value>

        [JsonProperty(Order = 1)]
        public string ParentDataItem { get; set; }

        /// <summary>   Gets or sets the name of the attribute. </summary>
        ///
        /// <value> The name of the attribute. </value>

        [JsonProperty(Order = 2)]
        public string AttributeName { get; set; }

        /// <summary>   Gets or sets the type of the attribute. </summary>
        ///
        /// <value> The type of the attribute. </value>

        [JsonProperty(Order = 3)]
        public string AttributeType { get; set; }

        /// <summary>   Gets or sets the properties. </summary>
        ///
        /// <value> The type of the attribute. </value>
        [JsonProperty(Order = 4), TsvParserChildRecursion(ItemType = typeof(EntityPropertyItem), ChildRecursionParameters = new object[] { "--IndentRecurseColumns", "--RecursionChildProperty", "ChildProperties" })]
        public List<EntityPropertyItem> Properties { get; set; }
    }
}
