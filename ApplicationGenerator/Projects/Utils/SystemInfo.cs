using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using mscoree;

namespace Utils
{
    public static class SystemInfo
    {
        private static Process thisProc;
        private static bool hasData = false;
        private static PerformanceCounter processTimeCounter;
        public static int MaximumCpuUsageForCurrentProcess { private set; get; }

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

                processTimeCounter.NextValue();
            }

            MaximumCpuUsageForCurrentProcess = 0;
            hasData = true;
        }

        public static IList<AppDomain> GetAppDomains()
        {
            var list = new List<AppDomain>();
            var enumHandle = IntPtr.Zero;
            var host = new CorRuntimeHost();

            try
            {
                host.EnumDomains(out enumHandle);

                object domain = null;

                while (true)
                {
                    host.NextDomain(enumHandle, out domain);

                    if (domain == null)
                    {
                        break;
                    }

                    var appDomain = (AppDomain)domain;

                    list.Add(appDomain);
                }

                return list;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
            finally
            {
                host.CloseEnum(enumHandle);
                Marshal.ReleaseComObject(host);
            }
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

        public static int CpuUsageForCurrentProcess
        {
            get
            {
                Init();

                if (!ReferenceEquals(processTimeCounter, null))
                {
                    var result = (int)processTimeCounter.NextValue();
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
    }
}
