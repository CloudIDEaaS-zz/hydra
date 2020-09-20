using Serilog.Core;
using Serilog.Events;
using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using SuperSocket.ProtoBase;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;

namespace Utils
{
    [DataContract]
    public class TraceLogEventSinkConfigArgs
    {
        [DataMember(IsRequired=false)]
        public string RestrictedToMinimumLevel { get; set; }
        [DataMember(IsRequired = false)]
        public List<string> ExcludeSources { get; set; }
        [DataMember(IsRequired = false)]
        public List<string> IncludeOnlySources { get; set; }
    }
}
