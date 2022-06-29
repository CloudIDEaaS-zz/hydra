using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydraResourceTracer
{
    public class DocumentInfo
    {
        public string Path { get; set; }
        public long Size { get; set; }
        public bool InZip { get; set; }
        public string CurrentState { get; set; }
        public DateTime LastReview { get; set; }
        public DateTime FirstReview { get; set; }
        public string Hash { get; internal set; }
    }
}
