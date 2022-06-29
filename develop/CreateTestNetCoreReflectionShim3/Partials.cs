using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Utils;

namespace CreateTest.NetCoreReflectionShim.Generators
{
    public partial class JsonReflectionClassTemplate : ITemplateEngineBasePartialClass
    {
        public System.EventHandler DebugCallback { get; set; }
        public ReflectMemberEventHandler ReflectMemberCallback { get; set; }
        public System.Type Type { get; set; }

        public void ReflectMember(MemberInfo memberInfo)
        {
            ReflectMemberCallback(this, new ReflectMemberEventArgs(memberInfo, ReflectKind.JsonTypes));
        }

        public void Initialize()
        {
            this.InitializePartial();
        }
    }
    public partial class ShimReflectionClassTemplate : ITemplateEngineBasePartialClass
    {
        public System.EventHandler DebugCallback { get; set; }
        public ReflectMemberEventHandler ReflectMemberCallback { get; set; }
        public System.Type Type { get; set; }

        public void ReflectMember(MemberInfo memberInfo)
        {
            ReflectMemberCallback(this, new ReflectMemberEventArgs(memberInfo, ReflectKind.ShimTypes));
        }

        public void Initialize()
        {
            this.InitializePartial();
        }
    }
}
