using NetCoreReflectionShim.CodeGen.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace NetCoreReflectionShim.CodeGen
{
    public static class Extensions
    {
        public static string GetName(this MemberInfo memberInfo, bool mangleName = false)
        {
            if (mangleName && memberInfo is MethodBase)
            {
                var methodBase = (MethodBase)memberInfo;
                var parmSignature = string.Join(string.Empty, methodBase.GetParameters().Select(p => p.ParameterType.GetShortNameOrNameOnly()[0].ToString()));

                return memberInfo.Name + "__" + parmSignature;
            }
            else
            {
                return memberInfo.Name;
            }
        }

        public static Type GetOwningType(this MemberInfo memberInfo, Type type = null)
        {
            if (type != null)
            {
                return type;
            }
            else if (memberInfo.DeclaringType == typeof(Object))
            {
                return memberInfo.ReflectedType;
            }
            else
            {
                return memberInfo.DeclaringType;
            }
        }

        public static ApiMember AddIfNotExists(this Dictionary<int, ApiMember> apiMembers, ReflectMemberEventArgs e, bool cacheResult = false, bool noShim = false)
        {
            if (!apiMembers.ContainsKey(e.MemberInfo.MetadataToken))
            {
                var owningType = e.MemberInfo.GetOwningType();
                var mangleName = false;
                ApiMember apiMember;

                if (owningType.GetMembers().Any(m => m != e.MemberInfo && m.Name == e.MemberInfo.Name))
                {
                    mangleName = true;
                }

                apiMember = new ApiMember(e.MemberInfo, cacheResult, noShim, mangleName);

                e.Code = apiMember.ClientCode;
                e.MangleName = apiMember.MangleName;

                apiMembers.Add(e.MemberInfo.MetadataToken, apiMember);

                return apiMember;
            }
            else
            {
                var owningType = e.MemberInfo.ReflectedType;
                var mangleName = false;
                ApiMember apiMember;

                if (owningType.GetMembers().Any(m => m != e.MemberInfo && m.Name == e.MemberInfo.Name))
                {
                    mangleName = true;
                }

                apiMember = new ApiMember(owningType, e.MemberInfo, cacheResult, noShim, mangleName);

                e.Code = apiMember.ClientCode;
                e.MangleName = apiMember.MangleName;

                return apiMembers[e.MemberInfo.MetadataToken];
            }
        }
    }
}
