using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.PackageCache
{
    public class InstallAction
    {
        public PackageWorkingInstallFromCache WorkingInstall { get; set; }
        public Action<DirectoryInfo, DirectoryInfo> Action { get; set; }
        public DirectoryInfo CacheDirectory { get; set; }
        public DirectoryInfo PackageDirectory { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime ExecutedTime { get; set; }
        public Exception Exception { get; set; }

        public InstallAction()
        {
            this.CreationTime = DateTime.Now;
        }
    }
}
