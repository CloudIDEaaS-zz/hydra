using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.PackageCache
{
    public delegate void AddInstallStatusEventHandler(object sender, AddInstallStatusEventArgs e);

    public class AddInstallStatusEventArgs : EventArgs
    {
        public PackageWorkingInstallFromCache InstallFromCache { get; }
        public StatusMode StatusMode { get; }
        public string Status { get; }
        public object[] Args { get; }

        public AddInstallStatusEventArgs(PackageWorkingInstallFromCache installFromCache, StatusMode mode, string status, params object[] args)
        {
            this.InstallFromCache = installFromCache;
            this.StatusMode = mode;
            this.Status = status;
            this.Args = args;
        }
    }
}
