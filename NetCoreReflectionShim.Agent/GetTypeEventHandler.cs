using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreReflectionShim.Agent
{
    public delegate void GetTypeEventHandler(object sender, GetTypeEventArgs e);

    public class GetTypeEventArgs : EventArgs
    {
        public string TypeFullName { get; }
        public Type Type { get; set; }

        public GetTypeEventArgs(string typeFullName)
        {
            this.TypeFullName = typeFullName;
        }
    }
}
