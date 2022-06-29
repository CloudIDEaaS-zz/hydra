using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Utils
{
    [DebuggerDisplay(" { DebugInfo } ")]
    public class CommandPacket
    {
        public string Type { get; set; }
        public string Command { get; set; }
        public KeyValuePair<string, object>[] Arguments { get; set; }
        public DateTime ReceivedTimestamp { get; set; }
        public DateTime SentTimestamp { get; set; }
        public object Response { get; set; }
        private string debugInfo;

        public CommandPacket()
        {
            this.Type = "request";
            this.SentTimestamp = DateTime.UtcNow;

            debugInfo = this.ToJsonText();
        }

        public CommandPacket(string command, Dictionary<string, object> arguments)
        {
            this.Type = "request";
            this.Command = command;
            this.Arguments = arguments.ToArray();
            this.SentTimestamp = DateTime.UtcNow;

            debugInfo = this.ToJsonText();
        }

        public CommandPacket(string command, params KeyValuePair<string, object>[] arguments)
        {
            this.Type = "request";
            this.Command = command;
            this.Arguments = arguments;
            this.SentTimestamp = DateTime.UtcNow;

            debugInfo = this.ToJsonText();
        }

        public CommandPacket(string command, DateTime receivedTimestamp, object response)
        {
            this.Type = "response";
            this.Command = command;
            this.Response = response;
            this.ReceivedTimestamp = receivedTimestamp;
            this.SentTimestamp = DateTime.UtcNow;

            debugInfo = this.ToJsonText();
        }

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
