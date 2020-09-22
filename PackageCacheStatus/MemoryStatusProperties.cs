using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace PackageCacheStatus
{
    public class MemoryStatusProperties
    {
        private MemoryStatus memoryStatus;

        public MemoryStatusProperties(MemoryStatus memoryStatus)
        {
            this.memoryStatus = memoryStatus;
        }

        public string MinWorkingSetStarting
        {
            get
            {
                return memoryStatus.MinWorkingSetStarting.ToFileSize();
            }
        }

        public string MaxWorkingSetStarting
        {
            get
            {
                return memoryStatus.MaxWorkingSetStarting.ToFileSize();
            }
        }

        public string NonpagedSystemMemorySize64Starting
        {
            get
            {
                return memoryStatus.NonpagedSystemMemorySize64Starting.ToFileSize();
            }
        }

        public string PagedMemorySize64Starting
        {
            get
            {
                return memoryStatus.PagedMemorySize64Starting.ToFileSize();
            }
        }

        public string PagedSystemMemorySize64Starting
        {
            get
            {
                return memoryStatus.PagedSystemMemorySize64Starting.ToFileSize();
            }
        }

        public string PeakPagedMemorySize64Starting
        {
            get
            {
                return memoryStatus.PeakPagedMemorySize64Starting.ToFileSize();
            }
        }

        public string PeakVirtualMemorySize64Starting
        {
            get
            {
                return memoryStatus.PeakVirtualMemorySize64Starting.ToFileSize();
            }
        }

        public string PeakWorkingSet64Starting
        {
            get
            {
                return memoryStatus.PeakWorkingSet64Starting.ToFileSize();
            }
        }

        public string VirtualMemorySize64Starting
        {
            get
            {
                return memoryStatus.VirtualMemorySize64Starting.ToFileSize();
            }
        }

        public string WorkingSet64Starting
        {
            get
            {
                return memoryStatus.WorkingSet64Current.ToFileSize();
            }
        }

        public string MinWorkingSetCurrent
        {
            get
            {
                return memoryStatus.MinWorkingSetCurrent.ToFileSize();
            }
        }

        public string MaxWorkingSetCurrent
        {
            get
            {
                return memoryStatus.MaxWorkingSetCurrent.ToFileSize();
            }
        }

        public string NonpagedSystemMemorySize64Current
        {
            get
            {
                return memoryStatus.NonpagedSystemMemorySize64Current.ToFileSize();
            }
        }

        public string PagedMemorySize64Current
        {
            get
            {
                return memoryStatus.PagedMemorySize64Current.ToFileSize();
            }
        }

        public string PagedSystemMemorySize64Current
        {
            get
            {
                return memoryStatus.PagedSystemMemorySize64Current.ToFileSize();
            }
        }

        public string PeakPagedMemorySize64Current
        {
            get
            {
                return memoryStatus.PeakPagedMemorySize64Current.ToFileSize();
            }
        }

        public string PeakVirtualMemorySize64Current
        {
            get
            {
                return memoryStatus.PeakVirtualMemorySize64Current.ToFileSize();
            }
        }

        public string PeakWorkingSet64Current
        {
            get
            {
                return memoryStatus.PeakWorkingSet64Current.ToFileSize();
            }
        }

        public string VirtualMemorySize64Current
        {
            get
            {
                return memoryStatus.VirtualMemorySize64Current.ToFileSize();
            }
        }

        public string WorkingSet64Current
        {
            get
            {
                return memoryStatus.WorkingSet64Current.ToFileSize();
            }
        }

        public string MemoryUsageStarting
        {
            get
            {
                return Math.Round(memoryStatus.MemoryUsageStarting, 0) + "%";
            }
        }

        public string MemoryUsageCurrent
        {
            get
            {
                return Math.Round(memoryStatus.MemoryUsageCurrent, 0) + "%";
            }
        }
    }
}
