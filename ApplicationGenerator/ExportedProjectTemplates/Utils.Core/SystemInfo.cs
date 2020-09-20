using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Net;
using System.Reflection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace Utils
{
    public static class SystemInfo
    {
        private static Process thisProc;
        private static bool hasData = false;
        private static PerformanceCounter processTimeCounter;
        private static PerformanceCounter memoryPercentCounter;
        private static PerformanceCounter systemUpTimeCounter;

        public static float MaximumCpuUsageForCurrentProcess { private set; get; }
        public static float MaximumMemoryUsageForCurrentProcess { private set; get; }
        public static int MaximumIPConnectionsForCurrentProcess { private set; get; }

        private static void Init()
        {
            if (hasData)
            {
                return;
            }

            if (CheckForPerformanceCounterCategoryExist("Process"))
            {
                processTimeCounter = new PerformanceCounter();

                processTimeCounter.CategoryName = "Process";
                processTimeCounter.CounterName = "% Processor Time";
                processTimeCounter.InstanceName = FindInstanceName("Process");

                if (processTimeCounter.InstanceName.IsNullOrEmpty())
                {
                    Thread.Sleep(100);

                    processTimeCounter.InstanceName = FindInstanceName("Process");
                }

                processTimeCounter.NextValue();
            }

            if (CheckForPerformanceCounterCategoryExist("Memory"))
            {
                memoryPercentCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
            }

            if (CheckForPerformanceCounterCategoryExist("System"))
            {
                systemUpTimeCounter = new PerformanceCounter("System", "System Up Time");
            }

            MaximumCpuUsageForCurrentProcess = 0;
            MaximumMemoryUsageForCurrentProcess = 0;
            MaximumIPConnectionsForCurrentProcess = 0;

            hasData = true;
        }

        public static void AddLocalServerInfo(this IDiagnostics diagnostics, IHostEnvironment environment = null, DbContext dbContext = null)
        {
            var hostName = Dns.GetHostName();
            var thisAssembly = Assembly.GetEntryAssembly();
            var assemblyName = thisAssembly.GetName();
            var versionAttribute = thisAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

            if (environment != null)
            {
                diagnostics.Environment = environment.EnvironmentName;
            }

            if (dbContext != null)
            {
                diagnostics.Database = dbContext.GetServerName() + "." + dbContext.GetDatabaseName();
            }

            diagnostics.HostName = hostName;
            diagnostics.ServerName = Environment.MachineName;
            diagnostics.Version = $"{ assemblyName.Name } v{ versionAttribute.InformationalVersion }";
            diagnostics.MemoryUsageForCurrentProcess = SystemInfo.MemoryUsageForCurrentProcess;
            diagnostics.CpuUsageForCurrentProcess = SystemInfo.CpuUsageForCurrentProcess;
            diagnostics.IpConnectionsForCurrentProcess = SystemInfo.IPConnectionsForCurrentProcess;
            diagnostics.MaximumCpuUsageForCurrentProcess = SystemInfo.MaximumCpuUsageForCurrentProcess;
            diagnostics.MaximumMemoryUsageForCurrentProcess = SystemInfo.MaximumMemoryUsageForCurrentProcess;
            diagnostics.MaximumIpConnectionsForCurrentProcess = SystemInfo.MaximumIPConnectionsForCurrentProcess;
            diagnostics.NetworkUtilization = SystemInfo.NetworkUtilization;
            diagnostics.SystemUpTime = SystemInfo.SystemUpTime;
            diagnostics.ProcessStartTime = Process.GetCurrentProcess().StartTime.ToUniversalTime();
        }

        private static bool CheckForPerformanceCounterCategoryExist(string categoryName)
        {
            return PerformanceCounterCategory.Exists(categoryName);
        }

        public static string FindInstanceName(string categoryName)
        {
            var result = String.Empty;

            thisProc = Process.GetCurrentProcess();

            if (!ReferenceEquals(thisProc, null))
            {
                if (!String.IsNullOrEmpty(categoryName))
                {
                    if (CheckForPerformanceCounterCategoryExist(categoryName))
                    {
                        var category = new PerformanceCounterCategory(categoryName);
                        var instances = category.GetInstanceNames();
                        var processName = thisProc.ProcessName;

                        if (instances != null)
                        {
                            foreach (var instance in instances)
                            {
                                if (instance.ToLower().Equals(processName.ToLower()))
                                {
                                    result = instance;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        public static double NetworkUtilization
        {
            get
            {
                var category = new PerformanceCounterCategory("Network Interface");
                var instanceNames = category.GetInstanceNames().ToList();
                var count = instanceNames.Count;
                double total = 0;

                foreach (string name in instanceNames)
                {
                    total = GetNetworkUtilization(name);
                }

                return total / count;
            }
        }

        private static double GetNetworkUtilization(string networkCard)
        {
            const int numberOfIterations = 10;
            float sendSum = 0;
            float receiveSum = 0;
            float bandwidth;
            float dataSent;
            float dataReceived;
            double utilization;
            PerformanceCounter bandwidthCounter;
            PerformanceCounter dataSentCounter;
            PerformanceCounter dataReceivedCounter;

            bandwidthCounter = new PerformanceCounter("Network Interface", "Current Bandwidth", networkCard);
            bandwidth = bandwidthCounter.NextValue(); 

            dataSentCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", networkCard);

            dataReceivedCounter = new PerformanceCounter("Network Interface", "Bytes Received/sec", networkCard);

            for (var index = 0; index < numberOfIterations; index++)
            {
                sendSum += dataSentCounter.NextValue();
                receiveSum += dataReceivedCounter.NextValue();
            }

            dataSent = sendSum;
            dataReceived = receiveSum;

            utilization = (8 * (dataSent + dataReceived)) / (bandwidth * numberOfIterations) * 100;
            
            return utilization;
        }

        public static float SystemUpTime
        {
            get
            {
                Init();

                if (!ReferenceEquals(systemUpTimeCounter, null))
                {
                    var result = systemUpTimeCounter.NextValue();

                    return result;
                }

                return 0;
            }
        }

        public static float MemoryUsageForCurrentProcess
        {
            get
            {
                Init();

                if (!ReferenceEquals(memoryPercentCounter, null))
                {
                    var result = memoryPercentCounter.NextValue();

                    if (MaximumMemoryUsageForCurrentProcess < result)
                    {
                        MaximumMemoryUsageForCurrentProcess = result;
                    }

                    return result;
                }

                return 0;
            }
        }

        public static float CpuUsageForCurrentProcess
        {
            get
            {
                Init();

                if (!ReferenceEquals(processTimeCounter, null))
                {
                    var result = processTimeCounter.NextValue();
                    result /= Environment.ProcessorCount;

                    if (MaximumCpuUsageForCurrentProcess < result)
                    {
                        MaximumCpuUsageForCurrentProcess = result;
                    }

                    return result;
                }

                return 0;
            }
        }

        public static int IPConnectionsForCurrentProcess
        {
            get
            {
                Init();

                var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
                var result = ipGlobalProperties.GetActiveTcpConnections().Length;

                if (MaximumIPConnectionsForCurrentProcess < result)
                {
                    MaximumIPConnectionsForCurrentProcess = result;
                }

                return 0;
            }
        }
    }
}
