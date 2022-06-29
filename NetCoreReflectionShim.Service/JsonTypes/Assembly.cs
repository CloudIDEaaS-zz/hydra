using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Security.Claims;
using System.Threading.Tasks;
using Utils;
using CustomAttributeData = System.Reflection.CustomAttributeData;

namespace CoreShim.Reflection.JsonTypes
{
    public class AssemblyJson
    {
        public string CodeBase { get; set; }
        public string EscapedCodeBase { get; set; }
        public string FullName { get; set; }
        public bool IsFullyTrusted { get; set; }
        public string SecurityRuleSetEnum { get; set; }
        public bool ReflectionOnly { get; set; }
        public string Location { get; set; }
        public string ImageRuntimeVersion { get; set; }
        public bool GlobalAssemblyCache { get; set; }
        public long HostContext { get; set; }
        public bool IsDynamic { get; set; }
        public int GetHashCodeMember { get; set; }
        public string ToStringMember { get; set; }
    }
}
