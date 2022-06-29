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
    public class PropertyInfoJson : MemberInfoJson
    {
        public string MemberTypeEnum { get; set; }
        public string AttributesEnum { get; set; }
        public bool CanRead { get; set; }
        public bool CanWrite { get; set; }
        public bool IsSpecialName { get; set; }
        public int GetHashCodeMember { get; set; }
        public string ToStringMember { get; set; }
        public string PropertyType { get; set; }
        public string DeclaringType { get; set; }
    }
}
