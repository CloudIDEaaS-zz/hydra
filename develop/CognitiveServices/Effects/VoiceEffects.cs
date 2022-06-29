using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CognitiveServices.Effects
{
    public class EffectsRole
    {
        public bool AnyAge { get; set; }
        public List<object> Effects { get; set; }
        public List<object> Attributes { get; set; }
        public int? AgeMin { get; set; }
        public int? AgeMax { get; set; }
    }

    public class RoleVoiceEffects
    {
        [JsonProperty("$schema")]
        public string Schema { get; set; }
        public List<EffectsRole> Roles { get; set; }
    }

}
