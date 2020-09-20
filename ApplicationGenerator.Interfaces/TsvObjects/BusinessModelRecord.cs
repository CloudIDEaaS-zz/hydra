using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AbstraX.TsvObjects
{
    public class BusinessModelRecord
    {
        [JsonProperty(Order = 0)]
        public int Id { get; set; }
        [JsonProperty(Order = 1)]
        public int ParentId { get; set; }
        [JsonProperty(Order = 2)]
        public string Name { get; set; }
        [JsonProperty(Order = 3)]
        public string SingularName { get; set; }
        [JsonProperty(Order = 4)]
        public string Level { get; set; }
        [JsonProperty(Order = 5)]
        public string ClassName { get; set; }
        [JsonProperty(Order = 6)]
        public string UserRoles { get; set; }
        [JsonProperty(Order = 7)]
        public string StakeholderKind { get; set; }
        [JsonProperty(Order = 8)]
        public bool IsSystemRole { get; set; }
        [JsonProperty(Order = 9)]
        public bool IsPseudoRole { get; set; }
        [JsonProperty(Order = 10)]
        public string PseudoRoles { get; set; }
        [JsonProperty(Order = 11)]
        public int ShadowItem { get; set; }
        [JsonProperty(Order = 12)]
        public bool IsApp { get; set; }
        [JsonProperty(Order = 13)]
        public bool IsDataItem { get; set; }
        [JsonProperty(Order = 14)]
        public string AppSettingsKind { get; set; }
        [JsonProperty(Order = 15)]
        public string IdentityKind { get; set; }
        [JsonProperty(Order = 16)]
        public bool IsSystemTask { get; set; }
        [JsonProperty(Order = 17)]
        public string TaskCapabilities { get; set; }
        [JsonProperty(Order = 18)]
        public bool ShowInUI { get; set; }
        [JsonProperty(Order = 19)]
        public string UIName { get; set; }
        [JsonProperty(Order = 20)]
        public string UIKind { get; set; }
        [JsonProperty(Order = 21)]
        public string UILoadKind { get; set; }
    }
}
