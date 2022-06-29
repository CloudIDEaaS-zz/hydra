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
    public class AppResourcesJson
    {
        public int GetHashCodeMember { get; set; }
        public string ToStringMember { get; set; }
    }
}
