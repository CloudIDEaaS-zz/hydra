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
    public class ParameterInfoShim : ParameterInfo
    {
        private ParameterInfoJson parameterInfo;
        private INetCoreReflectionAgent agent;
        private string parentIdentifier;

        public ParameterInfoShim(ParameterInfoJson parameterInfo, string parentIdentifier, INetCoreReflectionAgent agent)
        {
            this.parameterInfo = parameterInfo;
            this.agent = agent;
            this.parentIdentifier = parentIdentifier;
        } 

        public override Type ParameterType 
        { 
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string Name 
        { 
            get
            {
                return parameterInfo.Name;
            }
        }

        public override bool HasDefaultValue 
        { 
            get
            {
                return parameterInfo.HasDefaultValue;
            }
        }

        public override object DefaultValue
        { 
            get
            {
                return Convert.ChangeType(parameterInfo.DefaultValueObject, Type.GetType(parameterInfo.DefaultValueObjectType));
            }
        }

        public override object RawDefaultValue
        { 
            get
            {
                return Convert.ChangeType(parameterInfo.RawDefaultValueObject, Type.GetType(parameterInfo.RawDefaultValueObjectType));
            }
        }

        public override int Position 
        { 
            get
            {
                return parameterInfo.Position;
            }
        }

        public override System.Reflection.ParameterAttributes Attributes
        { 
            get
            {
                return EnumUtils.GetValue<System.Reflection.ParameterAttributes>(parameterInfo.AttributesEnum);
            }
        }

        public override MemberInfo Member 
        { 
            get
            {
                throw new NotImplementedException();
            }
        }

        public new bool IsIn 
        { 
            get
            {
                return parameterInfo.IsIn;
            }
        }

        public new bool IsOut 
        { 
            get
            {
                return parameterInfo.IsOut;
            }
        }

        public new bool IsLcid 
        { 
            get
            {
                return parameterInfo.IsLcid;
            }
        }

        public new bool IsRetval 
        { 
            get
            {
                return parameterInfo.IsRetval;
            }
        }

        public new bool IsOptional 
        { 
            get
            {
                return parameterInfo.IsOptional;
            }
        }

        public override int MetadataToken 
        { 
            get
            {
                return parameterInfo.MetadataToken;
            }
        }
        public override Type[] GetRequiredCustomModifiers()
        {
            throw new NotImplementedException();
        }
        public override Type[] GetOptionalCustomModifiers()
        {
            throw new NotImplementedException();
        }
        public override Object[] GetCustomAttributes(bool inherit)
        {
            return agent.ParameterInfo_GetCustomAttributes(string.Format("{0}[@MetadataToken={1}]", parentIdentifier, parameterInfo.MetadataToken.ToString()), inherit);
        }
        public override Object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return agent.ParameterInfo_GetCustomAttributes(string.Format("{0}[@MetadataToken={1}]", parentIdentifier, parameterInfo.MetadataToken.ToString()), attributeType, inherit);
        }
        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }
        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            throw new NotImplementedException();
        }
        public new object GetRealObject(StreamingContext context)
        {
            throw new NotImplementedException();
        }
        public override string ToString()
        {
            return parameterInfo.ToStringMember;
        }

        public override bool Equals(object obj)
        {
            return obj.GetHashCode() == this.GetHashCode();
        }
        public override int GetHashCode()
        {
            return parameterInfo.GetHashCodeMember;
        }
    }
}
