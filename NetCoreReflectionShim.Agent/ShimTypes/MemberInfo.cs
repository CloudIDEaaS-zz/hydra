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
    public abstract class MemberInfoShim : MemberInfo
    {
        private MemberInfoJson memberInfo;
        private INetCoreReflectionAgent agent;
        private string parentIdentifier;

        public MemberInfoShim(MemberInfoJson memberInfo, string parentIdentifier, INetCoreReflectionAgent agent)
        {
            this.memberInfo = memberInfo;
            this.agent = agent;
            this.parentIdentifier = parentIdentifier;
        } 

        public override System.Reflection.MemberTypes MemberType
        { 
            get
            {
                return EnumUtils.GetValue<System.Reflection.MemberTypes>(memberInfo.MemberTypeEnum);
            }
        }

        public override string Name 
        { 
            get
            {
                return memberInfo.Name;
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
                return memberInfo.MetadataToken;
            }
        }

        public override Module Module 
        { 
            get
            {
                throw new NotImplementedException();
            }
        }
        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            return obj.GetHashCode() == this.GetHashCode();
        }
        public override int GetHashCode()
        {
            return memberInfo.GetHashCodeMember;
        }
        public override Object[] GetCustomAttributes(bool inherit)
        {
            return agent.MemberInfo_GetCustomAttributes(string.Format("{0}[@MetadataToken={1}]", parentIdentifier, memberInfo.MetadataToken.ToString()), inherit);
        }
        public override Object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return agent.MemberInfo_GetCustomAttributes(string.Format("{0}[@MetadataToken={1}]", parentIdentifier, memberInfo.MetadataToken.ToString()), attributeType, inherit);
        }
        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }
        public override string ToString()
        {
            return memberInfo.ToStringMember;
        }
    }
}
