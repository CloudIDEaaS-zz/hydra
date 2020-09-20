using System;
using System.Collections.Generic;
using System.Text;

namespace Utils
{
    public interface IDiagnostics
    {
        string Environment { get; set; }
        string HostName { get; set; }
        string ServerName { get; set; }
        string Version { get; set; }
        string Database { get; set; }
        float MemoryUsageForCurrentProcess { get; set; }
        float CpuUsageForCurrentProcess { get; set; }
        int IpConnectionsForCurrentProcess { get; set; }
        float MaximumCpuUsageForCurrentProcess { get; set; }
        float MaximumMemoryUsageForCurrentProcess { get; set; }
        int MaximumIpConnectionsForCurrentProcess { get; set; }
        double NetworkUtilization { get; set; }
        float SystemUpTime { get; set; }
        DateTime ProcessStartTime { get; set; }
    }
}
