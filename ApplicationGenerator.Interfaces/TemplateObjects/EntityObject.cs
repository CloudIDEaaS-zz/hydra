// file:	TemplateObjects\EntityObject.cs
//
// summary:	Implements the entity object class

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.TemplateObjects
{
    /// <summary>   An entity object. </summary>
    ///
    /// <remarks>   Ken, 10/3/2020. </remarks>

    public class EntityObject : EntityBaseObject
    {
        /// <summary>   Gets or sets the parent data item. </summary>
        ///
        /// <value> The parent data item. </value>

        [JsonProperty(Order = 1)]
        public int ParentDataItem { get; set; }

        /// <summary>   Gets or sets a value indicating whether this  is inherent data item. </summary>
        ///
        /// <value> True if this  is inherent data item, false if not. </value>

        [JsonProperty(Order = 2)] 
        public bool IsInherentDataItem { get; set; }

        /// <summary>   Gets or sets the attributes. </summary>
        ///
        /// <value> The attributes. </value>

        [JsonProperty(Order = 3)]
        public List<AttributeObject> Attributes { get; set; }

        /// <summary>   Gets or sets the shadow of entity. </summary>
        ///
        /// <value> The shadow of entity. </value>

        [JsonIgnore]
        public EntityObject ShadowOfEntity { get; set; }

        /// <summary>   Gets or sets the type of the memory entity. </summary>
        ///
        /// <value> The type of the memory entity. </value>

        [JsonIgnore]
        public Type MemoryEntityType { get; internal set; }

        /// <summary>   Gets or sets the type of the memory entity metadata. </summary>
        ///
        /// <value> The type of the memory entity metadata. </value>

        public Type MemoryEntityMetadataType { get; internal set; }

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>

        public EntityObject()
        {
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="name">                 The name. </param>
        /// <param name="parentDataItemText">   The parent data item text. </param>

        public EntityObject(string name, string parentDataItemText) : base(name)
        {
            int parentDataItem;

            if (int.TryParse(parentDataItemText, out parentDataItem))
            {
                this.ParentDataItem = parentDataItem;
            }
            else if (parentDataItemText == "*")
            {
                this.IsInherentDataItem = true;
            }
            else
            {
                DebugUtils.Break();
            }

            this.ParentDataItem = parentDataItem;
            this.Attributes = new List<AttributeObject>();
        }
    }
}
