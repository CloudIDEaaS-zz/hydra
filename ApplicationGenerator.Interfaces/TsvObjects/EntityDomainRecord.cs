using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.TsvObjects
{
    public class EntityDomainRecord
    {
        [JsonProperty(Order = 0)]
        public string EntityName { get; set; }
        [JsonProperty(Order = 1)]
        public string ParentDataItem { get; set; }
        [JsonProperty(Order = 2)]
        public string AttributeName { get; set; }
        [JsonProperty(Order = 3)]
        public string AttributeType { get; set; }
        [JsonProperty(Order = 4, ItemConverterType = typeof(AttributePropertyItem), ItemConverterParameters = new object[] { "--IndentRecurseColumns", "--RecursionChildProperty", "ChildProperties" })]
        public List<AttributePropertyItem> AttributeProperties { get; set; }
    }
}
