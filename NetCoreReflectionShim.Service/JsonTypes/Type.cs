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
    public class TypeJson : MemberInfoJson
    {
        public string MemberTypeEnum { get; set; }
        public Guid GUID { get; set; }
        public string FullName { get; set; }
        public string Namespace { get; set; }
        public string AssemblyQualifiedName { get; set; }
        public TypeJson BaseType { get; set; }
        public bool IsNested { get; set; }
        public string AttributesEnum { get; set; }
        public string GenericParameterAttributesEnum { get; set; }
        public bool IsVisible { get; set; }
        public bool IsNotPublic { get; set; }
        public bool IsPublic { get; set; }
        public bool IsNestedPublic { get; set; }
        public bool IsNestedPrivate { get; set; }
        public bool IsNestedFamily { get; set; }
        public bool IsNestedAssembly { get; set; }
        public bool IsNestedFamANDAssem { get; set; }
        public bool IsNestedFamORAssem { get; set; }
        public bool IsAutoLayout { get; set; }
        public bool IsLayoutSequential { get; set; }
        public bool IsExplicitLayout { get; set; }
        public bool IsClass { get; set; }
        public bool IsInterface { get; set; }
        public bool IsValueType { get; set; }
        public bool IsAbstract { get; set; }
        public bool IsSealed { get; set; }
        public bool IsEnum { get; set; }
        public bool IsSpecialName { get; set; }
        public bool IsImport { get; set; }
        public bool IsSerializable { get; set; }
        public bool IsAnsiClass { get; set; }
        public bool IsUnicodeClass { get; set; }
        public bool IsAutoClass { get; set; }
        public bool IsArray { get; set; }
        public bool IsGenericType { get; set; }
        public bool IsGenericTypeDefinition { get; set; }
        public bool IsConstructedGenericType { get; set; }
        public bool IsGenericParameter { get; set; }
        public int GenericParameterPosition { get; set; }
        public bool ContainsGenericParameters { get; set; }
        public bool IsByRef { get; set; }
        public bool IsPointer { get; set; }
        public bool IsPrimitive { get; set; }
        public bool IsCOMObject { get; set; }
        public bool HasElementType { get; set; }
        public bool IsContextful { get; set; }
        public bool IsMarshalByRef { get; set; }
        public bool IsSecurityCritical { get; set; }
        public bool IsSecuritySafeCritical { get; set; }
        public bool IsSecurityTransparent { get; set; }
        public bool IsValueTypeImplMember { get; set; }
        public int GetHashCodeMember { get; set; }
        public List<TypeJson> GetInterfaces { get; set; }
        public List<TypeJson> GetGenericArguments { get; set; }
        public TypeJson GetGenericTypeDefinition { get; set; }
        public int GetAttributeFlagsImplMemberEnum { get; set; }
        public string ToStringMember { get; set; }
    }
}
