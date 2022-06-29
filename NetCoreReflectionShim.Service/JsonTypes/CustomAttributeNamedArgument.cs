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
    public class CustomAttributeNamedArgumentJson : ValueTypeJson
    {
        public string MemberInfo { get; set; }
        public string TypedValue { get; set; }
        public string MemberName { get; set; }
        public bool IsField { get; set; }
        public int GetHashCodeMember { get; set; }
        public string ToStringMember { get; set; }
    }
}
