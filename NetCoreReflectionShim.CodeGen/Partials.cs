using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Utils;

namespace NetCoreReflectionShim.CodeGen.Generators
{
    public partial class JsonReflectionClassTemplate : ITemplateEngineBasePartialClass
    {
        public System.EventHandler DebugCallback { get; set; }
        public NetCoreReflectionShim.CodeGen.ReflectMemberEventHandler ReflectMemberCallback { get; set; }
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
        public NetCoreReflectionShim.CodeGen.ReflectMemberEventHandler ReflectMemberCallback { get; set; }
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
