using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.TsvObjects
{
    public class AttributePropertyItem
    {
        [JsonProperty(Order = 0)]
        public string PropertyName { get; set; }
        [JsonProperty(Order = 1)]
        public string PropertyValue { get; set; }
        [JsonProperty(Order = 4, ItemConverterType = typeof(AttributePropertyItem), ItemConverterParameters = new object[] { "--RecurseColumns", "--RecursionChildProperty", "ChildProperties" })]
        public List<AttributePropertyItem> ChildProperties { get; set; }
    }
}
