using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public static class NetworkExtensions
    {
        [System.Runtime.InteropServices.DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);

        public static bool CheckNetworkConnectivity()
        {
            int desc;
            return InternetGetConnectedState(out desc, 0);
        }
    }
}
