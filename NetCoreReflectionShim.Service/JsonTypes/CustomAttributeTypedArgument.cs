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
    public class CustomAttributeTypedArgumentJson : ValueTypeJson
    {
        public string ArgumentType { get; set; }
        public string ValueObject { get; set; }
        public string ValueObjectType { get; set; }
        public string ToStringMember { get; set; }
        public int GetHashCodeMember { get; set; }
    }
}
