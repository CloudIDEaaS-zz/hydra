using System;
using System.Collections.Generic;
using System.Management;
using System.Text;
using System.Linq;

namespace Utils
{
    public static class ServerUtils
    {
        public static DateTime GetLastBootTime()
        {
            var systemUpTime = SystemInfo.SystemUpTime;
            var upTimeSpan = TimeSpan.FromSeconds(systemUpTime);

            return DateTime.Now.Subtract(upTimeSpan);
        }
    }
}
