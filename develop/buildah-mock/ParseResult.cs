using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace buildah
{
    public class ParseResult : ParseResultBase
    {
        public List<string> NonswitchArgs { get; internal set; }
        public string Tag { get; set; }
        public string Creds { get; set; }
        
        public string PrimaryCommand
        {
            get
            {
                return this.NonswitchArgs.First();
            }
        }

        public ParseResult()
        {
            this.NonswitchArgs = new List<string>();
        }
    }

    public class SwitchCommands
    {
        public const string TAG = "t";
        public const string CREDS = "creds";
    }
}
