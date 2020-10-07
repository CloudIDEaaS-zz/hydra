// file:	TemplateObjects\EntityBaseObject.cs
//
// summary:	Implements the entity base object class

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.TemplateObjects
{
    /// <summary>   An entity base object. </summary>
    ///
    /// <remarks>   Ken, 10/3/2020. </remarks>

    public abstract class EntityBaseObject
    {
        /// <summary>   Gets or sets the name. </summary>
        ///
        /// <value> The name. </value>

        [JsonProperty(Order = 0)]
        public string Name { get; set; }

        /// <summary>   Gets or sets the properties. </summary>
        ///
        /// <value> The properties. </value>

        [JsonProperty(Order = 99)] 
        public List<EntityPropertyItem> Properties { get; set; }

        /// <summary>   Specialized default constructor for use only by derived class. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>

        protected EntityBaseObject()
        {
        }

        /// <summary>   Specialized constructor for use only by derived class. </summary>
        ///
        /// <remarks>   Ken, 10/3/2020. </remarks>
        ///
        /// <param name="name"> The name. </param>

        protected EntityBaseObject(string name)
        {
            Name = name;
            this.Properties = new List<EntityPropertyItem>();
        }
    }
}
