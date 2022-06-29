// file:	TemplateObjects\EntityObject.cs
//
// summary:	Implements the entity object class

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.TemplateObjects
{
    /// <summary>   An entity object. </summary>
    ///
    /// <remarks>   Ken, 10/3/2020. </remarks>

    [DebuggerDisplay(" { Name }")]
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

        /// <summary>   Gets or sets the dynamic entity type builder. </summary>
        ///
        /// <value> The dynamic entity type builder. </value>

        [JsonIgnore]
        public TypeBuilder DynamicEntityTypeBuilder { get; set; }

        /// <summary>   Gets or sets the dynamic entity metadata type builder. </summary>
        ///
        /// <value> The dynamic entity metadata type builder. </value>

        [JsonIgnore]
        public TypeBuilder DynamicEntityMetadataTypeBuilder { get; set; }

        /// <summary>   Gets or sets the custome queries. </summary>
        ///
        /// <value> The custome queries. </value>

        [JsonIgnore]
        public Dictionary<string, CustomQuery> CustomQueries { get; set; }


        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>

        public EntityObject()
        {
            this.CustomQueries = new Dictionary<string, CustomQuery>();
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

            if (parentDataItemText == "*")
            {
                this.IsInherentDataItem = true;
                parentDataItem = 0;
            }
            else if (int.TryParse(parentDataItemText, out parentDataItem))
            {
                this.ParentDataItem = parentDataItem;
            }
            else
            {
                DebugUtils.Break();
            }

            this.ParentDataItem = parentDataItem;
            this.Attributes = new List<AttributeObject>();
            this.CustomQueries = new Dictionary<string, CustomQuery>();
        }
    }
}
