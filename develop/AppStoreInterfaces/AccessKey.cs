using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppStoreInterfaces
{
    public class AccessKey
    {
        public string UserName { get; set; }
        public Guid Key { get; set; }
        public Dictionary<string, string> Cookies { get; }
        public object Tag { get; set; }

        public AccessKey()
        {
            this.Cookies = new Dictionary<string, string>();
        }
    }
}
