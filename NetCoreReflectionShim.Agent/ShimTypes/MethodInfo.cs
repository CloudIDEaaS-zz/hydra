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

namespace CoreShim.Reflection
{
    public class MethodInfoShim : MethodInfo
    {
        private MethodInfoJson methodInfo;
        private INetCoreReflectionAgent agent;
        private string parentIdentifier;

        public MethodInfoShim(MethodInfoJson methodInfo, string parentIdentifier, INetCoreReflectionAgent agent)
        {
            this.methodInfo = methodInfo;
            this.agent = agent;
            this.parentIdentifier = parentIdentifier;
        } 

        public override System.Reflection.MemberTypes MemberType
        { 
            get
            {
                return EnumUtils.GetValue<System.Reflection.MemberTypes>(methodInfo.MemberTypeEnum);
            }
        }

        public override Type ReturnType 
        { 
            get
            {
                throw new NotImplementedException();
            }
        }

        public override ParameterInfo ReturnParameter 
        { 
            get
            {
                throw new NotImplementedException();
            }
        }

        public override ICustomAttributeProvider ReturnTypeCustomAttributes 
        { 
            get
            {
                throw new NotImplementedException();
            }
        }

        public override System.Reflection.MethodImplAttributes MethodImplementationFlags
        { 
            get
            {
                return EnumUtils.GetValue<System.Reflection.MethodImplAttributes>(methodInfo.MethodImplementationFlagsEnum);
            }
        }

        public override RuntimeMethodHandle MethodHandle 
        { 
            get
            {
                throw new NotImplementedException();
            }
        }

        public override System.Reflection.MethodAttributes Attributes
        { 
            get
            {
                return EnumUtils.GetValue<System.Reflection.MethodAttributes>(methodInfo.AttributesEnum);
            }
        }

        public override System.Reflection.CallingConventions CallingConvention
        { 
            get
            {
                return EnumUtils.GetValue<System.Reflection.CallingConventions>(methodInfo.CallingConventionEnum);
            }
        }

        public override bool IsGenericMethodDefinition 
        { 
            get
            {
                return methodInfo.IsGenericMethodDefinition;
            }
        }

        public override bool ContainsGenericParameters 
        { 
            get
            {
                return methodInfo.ContainsGenericParameters;
            }
        }

        public override bool IsGenericMethod 
        { 
            get
            {
                return methodInfo.IsGenericMethod;
            }
        }

        public override bool IsSecurityCritical 
        { 
            get
            {
                return methodInfo.IsSecurityCritical;
            }
        }

        public override bool IsSecuritySafeCritical 
        { 
            get
            {
                return methodInfo.IsSecuritySafeCritical;
            }
        }

        public override bool IsSecurityTransparent 
        { 
            get
            {
                return methodInfo.IsSecurityTransparent;
            }
        }

        public new bool IsPublic 
        { 
            get
            {
                return methodInfo.IsPublic;
            }
        }

        public new bool IsPrivate 
        { 
            get
            {
                return methodInfo.IsPrivate;
            }
        }

        public new bool IsFamily 
        { 
            get
            {
                return methodInfo.IsFamily;
            }
        }

        public new bool IsAssembly 
        { 
            get
            {
                return methodInfo.IsAssembly;
            }
        }

        public new bool IsFamilyAndAssembly 
        { 
            get
            {
                return methodInfo.IsFamilyAndAssembly;
            }
        }

        public new bool IsFamilyOrAssembly 
        { 
            get
            {
                return methodInfo.IsFamilyOrAssembly;
            }
        }

        public new bool IsStatic 
        { 
            get
            {
                return methodInfo.IsStatic;
            }
        }

        public new bool IsFinal 
        { 
            get
            {
                return methodInfo.IsFinal;
            }
        }

        public new bool IsVirtual 
        { 
            get
            {
                return methodInfo.IsVirtual;
            }
        }

        public new bool IsHideBySig 
        { 
            get
            {
                return methodInfo.IsHideBySig;
            }
        }

        public new bool IsAbstract 
        { 
            get
            {
                return methodInfo.IsAbstract;
            }
        }

        public new bool IsSpecialName 
        { 
            get
            {
                return methodInfo.IsSpecialName;
            }
        }

        public new bool IsConstructor 
        { 
            get
            {
                return methodInfo.IsConstructor;
            }
        }

        public override string Name 
        { 
            get
            {
                return methodInfo.Name;
            }
        }

        public override Type DeclaringType 
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

        public override int MetadataToken 
        { 
            get
            {
                return methodInfo.MetadataToken;
            }
        }

        public override Module Module 
        { 
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool Equals(object obj)
        {
            return obj.GetHashCode() == this.GetHashCode();
        }
        public override int GetHashCode()
        {
            return methodInfo.GetHashCodeMember;
        }
        public override Type[] GetGenericArguments()
        {
            throw new NotImplementedException();
        }
        public override MethodInfo GetGenericMethodDefinition()
        {
            throw new NotImplementedException();
        }
        public override MethodInfo MakeGenericMethod(Type[] typeArguments)
        {
            throw new NotImplementedException();
        }
        public override Delegate CreateDelegate(Type delegateType)
        {
            throw new NotImplementedException();
        }
        public override Delegate CreateDelegate(Type delegateType, object target)
        {
            throw new NotImplementedException();
        }
        public override MethodInfo GetBaseDefinition()
        {
            throw new NotImplementedException();
        }
        public new object Invoke(object obj, Object[] parameters)
        {
            throw new NotImplementedException();
        }
        public override MethodBody GetMethodBody()
        {
            throw new NotImplementedException();
        }
        public override ParameterInfo[] GetParameters()
        {
            throw new NotImplementedException();
        }
        public override MethodImplAttributes GetMethodImplementationFlags()
        {
            throw new NotImplementedException();
        }
        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            throw new NotImplementedException();
        }
        public override Object[] GetCustomAttributes(bool inherit)
        {
            return agent.MemberInfo_GetCustomAttributes(string.Format("{0}[@MetadataToken={1}]", parentIdentifier, methodInfo.MetadataToken.ToString()), inherit);
        }
        public override Object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return agent.MemberInfo_GetCustomAttributes(string.Format("{0}[@MetadataToken={1}]", parentIdentifier, methodInfo.MetadataToken.ToString()), attributeType, inherit);
        }
        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }
        public override string ToString()
        {
            return methodInfo.ToStringMember;
        }
    }
}
