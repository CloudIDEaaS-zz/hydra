using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CreateTest.NetCoreReflectionShim
{
    public enum ReflectKind
    {
        JsonTypes,
        ShimTypes
    }

    public delegate void ReflectMemberEventHandler(object sender, ReflectMemberEventArgs e);

    public class ReflectMemberEventArgs : EventArgs
    {
        public MemberInfo MemberInfo { get; }
        public ReflectKind ReflectKind { get; set; }

        public ReflectMemberEventArgs(MemberInfo memberInfo, ReflectKind reflectKind)
        {
            this.MemberInfo = memberInfo;
            this.ReflectKind = reflectKind;
        }
    }
}
