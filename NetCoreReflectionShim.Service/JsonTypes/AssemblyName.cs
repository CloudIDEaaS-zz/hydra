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
    public class AssemblyNameJson
    {
        public string Name { get; set; }
        public string CultureName { get; set; }
        public string CodeBase { get; set; }
        public string EscapedCodeBase { get; set; }
        public string ProcessorArchitectureEnum { get; set; }
        public string ContentTypeEnum { get; set; }
        public string FlagsEnum { get; set; }
        public string HashAlgorithmEnum { get; set; }
        public string VersionCompatibilityEnum { get; set; }
        public string FullName { get; set; }
        public string ToStringMember { get; set; }
        public int GetHashCodeMember { get; set; }
    }
}
