using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NetCoreReflectionShim.CodeGen
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
        public string Code { get; set; }
        public bool CacheResult { get; set; }
        public bool NoShim { get; set; }
        public string JsonPropertyType { get; set; }
        public bool MangleName { get; internal set; }

        public ReflectMemberEventArgs(MemberInfo memberInfo, ReflectKind reflectKind, bool noShim = false)
        {
            this.MemberInfo = memberInfo;
            this.ReflectKind = reflectKind;
            this.NoShim = noShim;
        }
    }
}
