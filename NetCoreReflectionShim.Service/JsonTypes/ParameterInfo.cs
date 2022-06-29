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
    public class ParameterInfoJson
    {
        public string ParameterType { get; set; }
        public string Name { get; set; }
        public bool HasDefaultValue { get; set; }
        public string DefaultValueObject { get; set; }
        public string DefaultValueObjectType { get; set; }
        public string RawDefaultValueObject { get; set; }
        public string RawDefaultValueObjectType { get; set; }
        public int Position { get; set; }
        public string AttributesEnum { get; set; }
        public bool IsIn { get; set; }
        public bool IsOut { get; set; }
        public bool IsLcid { get; set; }
        public bool IsRetval { get; set; }
        public bool IsOptional { get; set; }
        public int MetadataToken { get; set; }
        public string ToStringMember { get; set; }
        public int GetHashCodeMember { get; set; }
    }
}
