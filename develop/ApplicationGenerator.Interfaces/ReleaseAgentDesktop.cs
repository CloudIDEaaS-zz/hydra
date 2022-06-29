using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX
{
    public class ReleaseAgentDesktop : IRegistryKey
    {
        public string KeyName => nameof(ReleaseAgentDesktop);
        public string LastProjectFile { get; set; }
        public string LastWorkspaceRoot { get; set; }
    }
}
