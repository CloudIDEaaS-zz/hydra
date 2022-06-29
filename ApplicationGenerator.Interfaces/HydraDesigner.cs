using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX
{
    public class HydraDesigner : IRegistryKey
    {
        public string KeyName => nameof(HydraDesigner);
        public byte[] DockSiteXml { get; set; }

    }
}
