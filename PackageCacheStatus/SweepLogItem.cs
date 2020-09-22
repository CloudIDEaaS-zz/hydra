using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageCacheStatus
{
    public class SweepLogItem
    {
        public string Name { get; }
        public FileInfo LogFile { get; }
        public DateTime DateTime { get; }

        public SweepLogItem(string name, DateTime dateTime, FileInfo sweepLogFile)
        {
            this.Name = name;
            this.LogFile = sweepLogFile;
            this.DateTime = dateTime;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
