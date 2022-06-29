using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Utils
{
    [DebuggerDisplay(" { DebugInfo } ")]
    public class CommandPacket<T>
    {
        public string Type { get; set; }
        public string Command { get; set; }
        public DateTime ReceivedTimestamp { get; set; }
        public DateTime SentTimestamp { get; set; }
        public T Response { get; set; }
        private string debugInfo;

        public CommandPacket()
        {
            OneTimeTimer.Run(() =>
            {
                debugInfo = this.ToJsonText();

            }, 100);
        }

        public CommandPacket(string command, DateTime receivedTimestamp, T response)
        {
            this.Type = "response";
            this.Command = command;
            this.Response = response;
            this.ReceivedTimestamp = receivedTimestamp;
            this.SentTimestamp = DateTime.UtcNow;

            debugInfo = this.ToJsonText();
        }

        [ScriptIgnore]
        [JsonIgnore]
        public string DebugInfo
        {
            get
            {
                return debugInfo;
            }
        }
    }
}
