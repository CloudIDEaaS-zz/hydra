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
    public abstract class MethodBaseShim : MethodBase
    {
        private MethodBaseJson methodBase;
        private INetCoreReflectionAgent agent;
        private string parentIdentifier;

        public MethodBaseShim(MethodBaseJson methodBase, string parentIdentifier, INetCoreReflectionAgent agent)
        {
            this.methodBase = methodBase;
            this.agent = agent;
            this.parentIdentifier = parentIdentifier;
        } 

        public override System.Reflection.MethodImplAttributes MethodImplementationFlags
        { 
            get
            {
                return EnumUtils.GetValue<System.Reflection.MethodImplAttributes>(methodBase.MethodImplementationFlagsEnum);
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
                return EnumUtils.GetValue<System.Reflection.MethodAttributes>(methodBase.AttributesEnum);
            }
        }

        public override System.Reflection.CallingConventions CallingConvention
        { 
            get
            {
                return EnumUtils.GetValue<System.Reflection.CallingConventions>(methodBase.CallingConventionEnum);
            }
        }

        public override bool IsGenericMethodDefinition 
        { 
            get
            {
                return methodBase.IsGenericMethodDefinition;
            }
        }

        public override bool ContainsGenericParameters 
        { 
            get
            {
                return methodBase.ContainsGenericParameters;
            }
        }

        public override bool IsGenericMethod 
        { 
            get
            {
                return methodBase.IsGenericMethod;
            }
        }

        public override bool IsSecurityCritical 
        { 
            get
            {
                return methodBase.IsSecurityCritical;
            }
        }

        public override bool IsSecuritySafeCritical 
        { 
            get
            {
                return methodBase.IsSecuritySafeCritical;
            }
        }

        public override bool IsSecurityTransparent 
        { 
            get
            {
                return methodBase.IsSecurityTransparent;
            }
        }

        public new bool IsPublic 
        { 
            get
            {
                return methodBase.IsPublic;
            }
        }

        public new bool IsPrivate 
        { 
            get
            {
                return methodBase.IsPrivate;
            }
        }

        public new bool IsFamily 
        { 
            get
            {
                return methodBase.IsFamily;
            }
        }

        public new bool IsAssembly 
        { 
            get
            {
                return methodBase.IsAssembly;
            }
        }

        public new bool IsFamilyAndAssembly 
        { 
            get
            {
                return methodBase.IsFamilyAndAssembly;
            }
        }

        public new bool IsFamilyOrAssembly 
        { 
            get
            {
                return methodBase.IsFamilyOrAssembly;
            }
        }

        public new bool IsStatic 
        { 
            get
            {
                return methodBase.IsStatic;
            }
        }

        public new bool IsFinal 
        { 
            get
            {
                return methodBase.IsFinal;
            }
        }

        public new bool IsVirtual 
        { 
            get
            {
                return methodBase.IsVirtual;
            }
        }

        public new bool IsHideBySig 
        { 
            get
            {
                return methodBase.IsHideBySig;
            }
        }

        public new bool IsAbstract 
        { 
            get
            {
                return methodBase.IsAbstract;
            }
        }

        public new bool IsSpecialName 
        { 
            get
            {
                return methodBase.IsSpecialName;
            }
        }

        public new bool IsConstructor 
        { 
            get
            {
                return methodBase.IsConstructor;
            }
        }

        public override System.Reflection.MemberTypes MemberType
        { 
            get
            {
                return EnumUtils.GetValue<System.Reflection.MemberTypes>(methodBase.MemberTypeEnum);
            }
        }

        public override string Name 
        { 
            get
            {
                return methodBase.Name;
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
                return methodBase.MetadataToken;
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
            return methodBase.GetHashCodeMember;
        }
        public override Type[] GetGenericArguments()
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
            return agent.MemberInfo_GetCustomAttributes(string.Format("{0}[@MetadataToken={1}]", parentIdentifier, methodBase.MetadataToken.ToString()), inherit);
        }
        public override Object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return agent.MemberInfo_GetCustomAttributes(string.Format("{0}[@MetadataToken={1}]", parentIdentifier, methodBase.MetadataToken.ToString()), attributeType, inherit);
        }
        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }
        public override string ToString()
        {
            return methodBase.ToStringMember;
        }
    }
}
