using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstraX.ServerInterfaces;

namespace AbstraX
{
    public delegate void GetOverridesEventHandler(object sender, GetOverridesEventArgs e);

    public class GetOverridesEventArgs : EventArgs
    {
        public List<KeyValuePair<string, IGeneratorOverrides>> Overrides { get; set; }

        public GetOverridesEventArgs()
        {
        }
    }
}
