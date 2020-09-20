using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using System.Diagnostics;

namespace VisualStudioProvider.Configuration.Services
{
    public class VsShell : IVsShell
    {
        public int AdviseBroadcastMessages(IVsBroadcastMessageEvents pSink, out uint pdwCookie)
        {
            Debugger.Break();
            pdwCookie = 0;

            return 0;
        }

        public int AdviseShellPropertyChanges(IVsShellPropertyEvents pSink, out uint pdwCookie)
        {
            Debugger.Break();
            pdwCookie = 0;

            return 0;
        }

        public int GetPackageEnum(out IEnumPackages ppenum)
        {
            Debugger.Break();
            ppenum = null;

            return 0;
        }

        public int GetProperty(int propid, out object pvar)
        {
            Debugger.Break();
            pvar = null;

            return 0;
        }

        public int IsPackageInstalled(ref Guid guidPackage, out int pfInstalled)
        {
            Debugger.Break();
            pfInstalled = 0;

            return 0;
        }

        public int IsPackageLoaded(ref Guid guidPackage, out IVsPackage ppPackage)
        {
            Debugger.Break();
            ppPackage = null;

            return 0;
        }

        public int LoadPackage(ref Guid guidPackage, out IVsPackage ppPackage)
        {
            Debugger.Break();
            ppPackage = null;

            return 0;
        }

        public int LoadPackageString(ref Guid guidPackage, uint resid, out string pbstrOut)
        {
            Debugger.Break();
            pbstrOut = null;

            return 0;
        }

        public int LoadUILibrary(ref Guid guidPackage, uint dwExFlags, out uint phinstOut)
        {
            Debugger.Break();
            phinstOut = 0;

            return 0;
        }

        public int SetProperty(int propid, object var)
        {
            Debugger.Break();

            return 0;
        }

        public int UnadviseBroadcastMessages(uint dwCookie)
        {
            Debugger.Break();

            return 0;
        }

        public int UnadviseShellPropertyChanges(uint dwCookie)
        {
            Debugger.Break();

            return 0;
        }
    }
}
