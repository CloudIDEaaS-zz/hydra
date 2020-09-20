using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    public class PackageJson
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public Dictionary<string, string> Dependencies { get; set; }
        public Dictionary<string, string> DevDependencies { get; set; }
        public Dictionary<string, string> PeerDependencies { get; set; }
    }
}
