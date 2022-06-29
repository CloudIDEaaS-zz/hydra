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
    public class CustomAttributeDataJson
    {
        public string AttributeType { get; set; }
        public ConstructorInfoJson Constructor { get; set; }
        public List<CustomAttributeTypedArgumentJson> ConstructorArguments { get; set; }
        public List<CustomAttributeNamedArgumentJson> NamedArguments { get; set; }
        public int GetHashCodeMember { get; set; }
        public string ToStringMember { get; set; }
    }
}
