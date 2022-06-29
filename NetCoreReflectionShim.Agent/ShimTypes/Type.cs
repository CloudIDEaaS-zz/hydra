using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Security.Claims;
using System.Threading.Tasks;
using Utils;
using CoreShim.Reflection.JsonTypes;
using System.Reflection;
using System.IO;
using System.Globalization;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using CustomAttributeData = System.Reflection.CustomAttributeData;
using System.Security.Policy;
using System.Security;
using NetCoreReflectionShim.Agent;
using NetCoreReflectionShim.Agent.ShimTypes;

namespace CoreShim.Reflection
{
    public class TypeShim : Type
    {
        private TypeJson type;
        private INetCoreReflectionAgent agent;
        private string parentIdentifier;
        private Attribute[] customAttributes;
        private PropertyInfo[] properties;
        private ModuleShim module;

        public TypeShim(TypeJson type, string parentIdentifier, INetCoreReflectionAgent agent)
        {
            this.type = type;
            this.agent = agent;
            this.parentIdentifier = parentIdentifier;
        } 

        public override System.Reflection.MemberTypes MemberType
        { 
            get
            {
                return EnumUtils.GetValue<System.Reflection.MemberTypes>(type.MemberTypeEnum);
            }
        }

        public override Type DeclaringType 
        { 
            get
            {
                return null;
            }
        }

        public override MethodBase DeclaringMethod 
        { 
            get
            {
                throw new NotImplementedException();
            }
        }

        public override Type ReflectedType 
        { 
            get
            {
                throw new NotImplementedException();
            }
        }

        public override StructLayoutAttribute StructLayoutAttribute 
        { 
            get
            {
                throw new NotImplementedException();
            }
        }

        public override Guid GUID 
        { 
            get
            {
                return type.GUID;
            }
        }

        public override Module Module 
        { 
            get
            {
                if (module == null)
                {
                    module = new ModuleShim(parentIdentifier, agent);
                }

                return module;
            }
        }

        public override Assembly Assembly 
        { 
            get
            {
                throw new NotImplementedException();
            }
        }

        public override RuntimeTypeHandle TypeHandle 
        { 
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string FullName 
        { 
            get
            {
                return type.FullName;
            }
        }

        public override string Namespace 
        { 
            get
            {
                return type.Namespace;
            }
        }

        public override string AssemblyQualifiedName 
        { 
            get
            {
                return type.AssemblyQualifiedName;
            }
        }

        public override Type BaseType 
        { 
            get
            {
                if (type.BaseType == null)
                {
                    return typeof(object);
                }
                else
                {
                    return agent.MapType(new TypeShim(type.BaseType, parentIdentifier, agent));
                }
            }
        }

        public new ConstructorInfo TypeInitializer 
        { 
            get
            {
                throw new NotImplementedException();
            }
        }

        public new bool IsNested 
        { 
            get
            {
                return type.IsNested;
            }
        }

        public new System.Reflection.TypeAttributes Attributes
        { 
            get
            {
                return EnumUtils.GetValue<System.Reflection.TypeAttributes>(type.AttributesEnum);
            }
        }

        public override System.Reflection.GenericParameterAttributes GenericParameterAttributes
        { 
            get
            {
                return EnumUtils.GetValue<System.Reflection.GenericParameterAttributes>(type.GenericParameterAttributesEnum);
            }
        }

        public new bool IsVisible 
        { 
            get
            {
                return type.IsVisible;
            }
        }

        public new bool IsNotPublic 
        { 
            get
            {
                return type.IsNotPublic;
            }
        }

        public new bool IsPublic 
        { 
            get
            {
                return type.IsPublic;
            }
        }

        public new bool IsNestedPublic 
        { 
            get
            {
                return type.IsNestedPublic;
            }
        }

        public new bool IsNestedPrivate 
        { 
            get
            {
                return type.IsNestedPrivate;
            }
        }

        public new bool IsNestedFamily 
        { 
            get
            {
                return type.IsNestedFamily;
            }
        }

        public new bool IsNestedAssembly 
        { 
            get
            {
                return type.IsNestedAssembly;
            }
        }

        public new bool IsNestedFamANDAssem 
        { 
            get
            {
                return type.IsNestedFamANDAssem;
            }
        }

        public new bool IsNestedFamORAssem 
        { 
            get
            {
                return type.IsNestedFamORAssem;
            }
        }

        public new bool IsAutoLayout 
        { 
            get
            {
                return type.IsAutoLayout;
            }
        }

        public new bool IsLayoutSequential 
        { 
            get
            {
                return type.IsLayoutSequential;
            }
        }

        public new bool IsExplicitLayout 
        { 
            get
            {
                return type.IsExplicitLayout;
            }
        }

        public new bool IsClass 
        { 
            get
            {
                return type.IsClass;
            }
        }

        public new bool IsInterface 
        { 
            get
            {
                return type.IsInterface;
            }
        }

        public new bool IsValueType 
        { 
            get
            {
                return type.IsValueType;
            }
        }

        public new bool IsAbstract 
        { 
            get
            {
                return type.IsAbstract;
            }
        }

        public new bool IsSealed 
        { 
            get
            {
                return type.IsSealed;
            }
        }

        public override bool IsEnum 
        { 
            get
            {
                return type.IsEnum;
            }
        }

        public new bool IsSpecialName 
        { 
            get
            {
                return type.IsSpecialName;
            }
        }

        public new bool IsImport 
        { 
            get
            {
                return type.IsImport;
            }
        }

        public override bool IsSerializable 
        { 
            get
            {
                return type.IsSerializable;
            }
        }

        public new bool IsAnsiClass 
        { 
            get
            {
                return type.IsAnsiClass;
            }
        }

        public new bool IsUnicodeClass 
        { 
            get
            {
                return type.IsUnicodeClass;
            }
        }

        public new bool IsAutoClass 
        { 
            get
            {
                return type.IsAutoClass;
            }
        }

        public new bool IsArray 
        { 
            get
            {
                return type.IsArray;
            }
        }

        public override bool IsGenericType 
        { 
            get
            {
                return type.IsGenericType;
            }
        }

        public override bool IsGenericTypeDefinition 
        { 
            get
            {
                return type.IsGenericTypeDefinition;
            }
        }

        public override bool IsConstructedGenericType 
        { 
            get
            {
                return type.IsConstructedGenericType;
            }
        }

        public override bool IsGenericParameter 
        { 
            get
            {
                return type.IsGenericParameter;
            }
        }

        public override int GenericParameterPosition 
        { 
            get
            {
                return type.GenericParameterPosition;
            }
        }

        public override bool ContainsGenericParameters 
        { 
            get
            {
                return type.ContainsGenericParameters;
            }
        }

        public new bool IsByRef 
        { 
            get
            {
                return type.IsByRef;
            }
        }

        public new bool IsPointer 
        { 
            get
            {
                return type.IsPointer;
            }
        }

        public new bool IsPrimitive 
        { 
            get
            {
                return type.IsPrimitive;
            }
        }

        public new bool IsCOMObject 
        { 
            get
            {
                return type.IsCOMObject;
            }
        }

        public new bool HasElementType 
        { 
            get
            {
                return type.HasElementType;
            }
        }

        public new bool IsContextful 
        { 
            get
            {
                return type.IsContextful;
            }
        }

        public new bool IsMarshalByRef 
        { 
            get
            {
                return type.IsMarshalByRef;
            }
        }

        public override Type[] GenericTypeArguments 
        { 
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool IsSecurityCritical 
        { 
            get
            {
                return type.IsSecurityCritical;
            }
        }

        public override bool IsSecuritySafeCritical 
        { 
            get
            {
                return type.IsSecuritySafeCritical;
            }
        }

        public override bool IsSecurityTransparent 
        { 
            get
            {
                return type.IsSecurityTransparent;
            }
        }

        public override Type UnderlyingSystemType 
        { 
            get
            {
                return typeof(object);
            }
        }

        public override string Name 
        { 
            get
            {
                return type.Name;
            }
        }

        public override int MetadataToken 
        { 
            get
            {
                return type.MetadataToken;
            }
        }
        public override Type MakePointerType()
        {
            throw new NotImplementedException();
        }
        public override Type MakeByRefType()
        {
            throw new NotImplementedException();
        }
        public override Type MakeArrayType()
        {
            throw new NotImplementedException();
        }
        public override Type MakeArrayType(int rank)
        {
            throw new NotImplementedException();
        }
        public new object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, Object[] args, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        public new object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, Object[] args)
        {
            throw new NotImplementedException();
        }
        public override int GetArrayRank()
        {
            throw new NotImplementedException();
        }
        public new ConstructorInfo GetConstructor(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            throw new NotImplementedException();
        }
        public new ConstructorInfo GetConstructor(BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers)
        {
            throw new NotImplementedException();
        }
        public new ConstructorInfo GetConstructor(Type[] types)
        {
            throw new NotImplementedException();
        }
        public new ConstructorInfo[] GetConstructors()
        {
            throw new NotImplementedException();
        }
        public new MethodInfo GetMethod(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            throw new NotImplementedException();
        }
        public new MethodInfo GetMethod(string name, BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers)
        {
            throw new NotImplementedException();
        }
        public new MethodInfo GetMethod(string name, Type[] types, ParameterModifier[] modifiers)
        {
            throw new NotImplementedException();
        }
        public new MethodInfo GetMethod(string name, Type[] types)
        {
            throw new NotImplementedException();
        }
        public new MethodInfo GetMethod(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }
        public new MethodInfo GetMethod(string name)
        {
            throw new NotImplementedException();
        }
        public new MethodInfo[] GetMethods()
        {
            throw new NotImplementedException();
        }
        public new FieldInfo GetField(string name)
        {
            throw new NotImplementedException();
        }
        public new FieldInfo[] GetFields()
        {
            throw new NotImplementedException();
        }
        public new Type GetInterface(string name)
        {
            throw new NotImplementedException();
        }
        public override Type[] FindInterfaces(TypeFilter filter, object filterCriteria)
        {
            throw new NotImplementedException();
        }
        public new EventInfo GetEvent(string name)
        {
            throw new NotImplementedException();
        }
        public override EventInfo[] GetEvents()
        {
            throw new NotImplementedException();
        }
        public new PropertyInfo GetProperty(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
        {
            throw new NotImplementedException();
        }
        public new PropertyInfo GetProperty(string name, Type returnType, Type[] types, ParameterModifier[] modifiers)
        {
            throw new NotImplementedException();
        }
        public new PropertyInfo GetProperty(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }
        public new PropertyInfo GetProperty(string name, Type returnType, Type[] types)
        {
            throw new NotImplementedException();
        }
        public new PropertyInfo GetProperty(string name, Type[] types)
        {
            throw new NotImplementedException();
        }
        public new PropertyInfo GetProperty(string name, Type returnType)
        {
            throw new NotImplementedException();
        }
        public new PropertyInfo GetProperty(string name)
        {
            return this.GetProperties().SingleOrDefault(p => p.Name == name);
        }

        public new PropertyInfo[] GetProperties()
        {
            if (properties == null)
            {
                properties = agent.Type_GetProperties(string.Format("{0}[@MetadataToken={1}]", parentIdentifier, type.MetadataToken.ToString()));
            }

            return properties;
        }

        public new Type[] GetNestedTypes()
        {
            throw new NotImplementedException();
        }
        public new Type GetNestedType(string name)
        {
            throw new NotImplementedException();
        }
        public new MemberInfo[] GetMember(string name)
        {
            throw new NotImplementedException();
        }
        public override MemberInfo[] GetMember(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }
        public override MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }
        public new MemberInfo[] GetMembers()
        {
            throw new NotImplementedException();
        }
        public override MemberInfo[] GetDefaultMembers()
        {
            throw new NotImplementedException();
        }
        public override MemberInfo[] FindMembers(MemberTypes memberType, BindingFlags bindingAttr, MemberFilter filter, object filterCriteria)
        {
            throw new NotImplementedException();
        }
        public override Type[] GetGenericParameterConstraints()
        {
            throw new NotImplementedException();
        }
        public override Type MakeGenericType(Type[] typeArguments)
        {
            throw new NotImplementedException();
        }

        public override Type[] GetGenericArguments()
        {
            Type[] args;

            if (type.GetGenericArguments != null)
            {
                args = type.GetGenericArguments.ToTypes(agent, parentIdentifier);
            }
            else
            {
                args = new Type[0];
            }

            return args;
        }

        public override Type GetGenericTypeDefinition()
        {
            if (type.GetGenericTypeDefinition != null)
            {
                var typeShim = new TypeShim(type.GetGenericTypeDefinition, parentIdentifier, agent);
                var mappedType = agent.MapType(typeShim);

                return mappedType;
            }

            return null;
        }

        public override String[] GetEnumNames()
        {
            throw new NotImplementedException();
        }
        public override Array GetEnumValues()
        {
            throw new NotImplementedException();
        }
        public override Type GetEnumUnderlyingType()
        {
            throw new NotImplementedException();
        }
        public override bool IsEnumDefined(object value)
        {
            throw new NotImplementedException();
        }
        public override string GetEnumName(object value)
        {
            throw new NotImplementedException();
        }
        public override bool IsSubclassOf(Type c)
        {
            throw new NotImplementedException();
        }
        public override bool IsInstanceOfType(object o)
        {
            throw new NotImplementedException();
        }
        public override bool IsAssignableFrom(Type c)
        {
            throw new NotImplementedException();
        }
        public override bool IsEquivalentTo(Type other)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object o)
        {
            return o.GetHashCode() == this.GetHashCode();
        }

        public override bool Equals(Type o)
        {
            return o.GetHashCode() == this.GetHashCode();
        }
        public override int GetHashCode()
        {
            return type.GetHashCodeMember;
        }
        public override InterfaceMapping GetInterfaceMap(Type interfaceType)
        {
            throw new NotImplementedException();
        }
        public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, Object[] args, ParameterModifier[] modifiers, CultureInfo culture, String[] namedParameters)
        {
            throw new NotImplementedException();
        }
        public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }
        public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }
        public override FieldInfo GetField(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }
        public override FieldInfo[] GetFields(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }
        public override Type GetInterface(string name, bool ignoreCase)
        {
            throw new NotImplementedException();
        }
        public override Type[] GetInterfaces()
        {
            Type[] interfaces;

            if (type.GetInterfaces != null)
            {
                interfaces = type.GetInterfaces.ToTypes(agent, parentIdentifier);
            }   
            else
            {
                interfaces = new Type[0];
            }

            return interfaces;
        }
        public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }
        public override EventInfo[] GetEvents(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
        {
            if (properties == null)
            {
                properties = agent.Type_GetProperties(string.Format("{0}[@MetadataToken={1}]", parentIdentifier, type.MetadataToken.ToString()), bindingAttr);
            }

            return properties;
        }

        public override Type[] GetNestedTypes(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }
        public override Type GetNestedType(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }
        public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }
        public override Type GetElementType()
        {
            throw new NotImplementedException();
        }
        public override string ToString()
        {
            return type.ToStringMember;
        }
        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            throw new NotImplementedException();
        }
        public override Object[] GetCustomAttributes(bool inherit)
        {
            if (customAttributes == null)
            {
                customAttributes = (Attribute[]) agent.MemberInfo_GetCustomAttributes(string.Format("{0}[@MetadataToken={1}]", parentIdentifier, type.MetadataToken.ToString()), inherit);
            }

            return customAttributes;
        }

        public override Object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            if (customAttributes == null)
            {
                customAttributes = (Attribute[])agent.MemberInfo_GetCustomAttributes(string.Format("{0}[@MetadataToken={1}]", parentIdentifier, type.MetadataToken.ToString()), attributeType, inherit);
            }

            return customAttributes;
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        protected override TypeCode GetTypeCodeImpl()
        {
            throw new NotImplementedException();
        }

        protected override bool IsValueTypeImpl()
        {
            return type.IsValueTypeImplMember;
        }

        protected override bool IsContextfulImpl()
        {
            throw new NotImplementedException();
        }

        protected override bool IsMarshalByRefImpl()
        {
            throw new NotImplementedException();
        }

        protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            throw new NotImplementedException();
        }

        protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            throw new NotImplementedException();
        }

        protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
        {
            return this.GetProperties().Single(p => p.Name == name);
        }

        protected override TypeAttributes GetAttributeFlagsImpl()
        {
            return EnumUtils.GetValue<TypeAttributes>(type.GetAttributeFlagsImplMemberEnum);
        }

        protected override bool IsArrayImpl()
        {
            return type.IsArray;
        }

        protected override bool IsByRefImpl()
        {
            return type.IsByRef;
        }

        protected override bool IsPointerImpl()
        {
            return type.IsPointer;
        }

        protected override bool IsPrimitiveImpl()
        {
            return type.IsPrimitive;
        }

        protected override bool IsCOMObjectImpl()
        {
            return type.IsCOMObject;
        }

        protected override bool HasElementTypeImpl()
        {
            return type.HasElementType;
        }

        public static bool operator==(Type type, TypeShim typeShim)
        {
            return typeShim.CompareTo(type);
        }

        public static bool operator !=(Type type, TypeShim typeShim)
        {
            return !typeShim.CompareTo(type);
        }
    }
}
