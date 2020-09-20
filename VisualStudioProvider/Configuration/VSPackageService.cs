using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using Utils;
using Interop = Microsoft.VisualStudio.OLE.Interop;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using VisualStudioProvider.Configuration.Services.Interfaces;
using System.Diagnostics;
using System.IO;

namespace VisualStudioProvider.Configuration
{
    public class VSPackageService : VSService
    {
        public Guid PackageGuid { get; private set; }
        public VSPackage Package { get; set; }
        private IntPtr hModule;
        private delegate int GetClassObjectHandler([In, MarshalAs(UnmanagedType.LPStruct)] Guid rclsid, [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object pUnk);
        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern IntPtr LoadLibrary(string lpFileName);
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FreeLibrary(IntPtr hModule);

        public VSPackageService(Guid serviceGuid, RegistryKey key) : base(serviceGuid)
        {
            var serviceKey = key.ToIndexable();
            var packageGuid = (string)serviceKey.Default;
            var name = (string)serviceKey["Name"];

            if (packageGuid != null)
            {
                this.PackageGuid = Guid.Parse(packageGuid);
            }

            if (!string.IsNullOrEmpty(packageGuid))
            {
                this.PackageGuid = Guid.Parse(packageGuid);
            }

            this.Name = name;
        }

        public override Interop.IServiceProvider ServiceProvider
        {
            get
            {
                if (serviceProvider == null)
                {
                    if (this.Package != null && this.Package.ClassName != null)
                    {
                        Debugger.Break();
                    }
                    else
                    {
                        var dll = this.Package.PackageDll.FullName;

                        hModule = LoadLibrary(dll);

                        if (hModule != IntPtr.Zero)
                        {
                            var proc = GetProcAddress(hModule, "DllGetClassObject");

                            if (proc != IntPtr.Zero)
                            {
                                var getClassObject = (GetClassObjectHandler)Marshal.GetDelegateForFunctionPointer(proc, typeof(GetClassObjectHandler));
                                var IID_ClassFactory = typeof(IClassFactory).GUID;
                                var IID_ServiceProvider = typeof(Interop.IServiceProvider).GUID;
                                var objUnknown = (object)null;
                                var classFactory = (IClassFactory)null;
                                var serviceGuid = this.ServiceGuid;

                                var hr = getClassObject(serviceGuid, IID_ClassFactory, out objUnknown);

                                if (hr != 0)
                                {
                                    return null;
                                }

                                classFactory = (IClassFactory)objUnknown;

                                hr = classFactory.CreateInstance(null, ref IID_ServiceProvider, out objUnknown);

                                if (hr != 0)
                                {
                                    return null;
                                }

                                serviceProvider = (Interop.IServiceProvider)objUnknown;

                                Marshal.ReleaseComObject(classFactory);

                                AddRef();
                            }
                        }
                    }
                }

                return serviceProvider;
            }
        }

        public override void Dispose()
        {
            if (serviceProvider != null)
            {
                Release();
            }

            if (hModule != IntPtr.Zero)
            {
                FreeLibrary(hModule);
            }

            base.Dispose();
        }
    }
}
