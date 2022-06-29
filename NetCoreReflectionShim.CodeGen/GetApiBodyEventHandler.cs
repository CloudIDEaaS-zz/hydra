using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NetCoreReflectionShim.CodeGen
{
    public delegate void GetApiBodyEventHandler(object sender, GetApiBodyEventArgs e);

    public class GetApiBodyEventArgs : EventArgs
    {
        public ApiMember Member { get; }
        public string Code { get; set; }

        public GetApiBodyEventArgs(ApiMember member)
        {
            this.Member = member;
        }
    }
}
