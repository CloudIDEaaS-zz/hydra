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
    public class MethodBaseJson : MemberInfoJson
    {
        public string MethodImplementationFlagsEnum { get; set; }
        public List<ParameterInfoJson> MethodHandle { get; set; }
        public string AttributesEnum { get; set; }
        public string CallingConventionEnum { get; set; }
        public bool IsGenericMethodDefinition { get; set; }
        public bool ContainsGenericParameters { get; set; }
        public bool IsGenericMethod { get; set; }
        public bool IsSecurityCritical { get; set; }
        public bool IsSecuritySafeCritical { get; set; }
        public bool IsSecurityTransparent { get; set; }
        public bool IsPublic { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsFamily { get; set; }
        public bool IsAssembly { get; set; }
        public bool IsFamilyAndAssembly { get; set; }
        public bool IsFamilyOrAssembly { get; set; }
        public bool IsStatic { get; set; }
        public bool IsFinal { get; set; }
        public bool IsVirtual { get; set; }
        public bool IsHideBySig { get; set; }
        public bool IsAbstract { get; set; }
        public bool IsSpecialName { get; set; }
        public bool IsConstructor { get; set; }
        public int GetHashCodeMember { get; set; }
        public List<ParameterInfoJson> GetParameters { get; set; }
        public string ToStringMember { get; set; }
    }
}
