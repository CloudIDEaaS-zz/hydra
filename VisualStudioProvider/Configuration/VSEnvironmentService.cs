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
using System.Threading;
using Microsoft.VisualStudio;

namespace VisualStudioProvider.Configuration
{
    public class VSEnvironmentService : VSService
    {
        public Guid PackageGuid { get; private set; }
        public VSPackage Package { get; set; }
        private IntPtr hModule;
        private FileInfo serviceFile;
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int GetClassObjectHandler([In, MarshalAs(UnmanagedType.LPStruct)] Guid rclsid, [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object pUnk);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int VStudioMainHandler(ref int version);
        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern IntPtr LoadLibrary(string lpFileName);
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FreeLibrary(IntPtr hModule);

        public VSEnvironmentService(FileInfo serviceFile) : base(Guid.Empty)
        {
            this.serviceFile = serviceFile;
        }

        public static VSEnvironmentService LoadEnvironmentServices()
        {
            var configKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\VisualStudio\10.0_Config");
            var file = string.Empty;

            var serviceFile = new FileInfo((string)configKey.GetValue("MsEnvLocation"));

            Debug.Assert(serviceFile.Exists);

            return new VSEnvironmentService(serviceFile);
        }

        public override Interop.IServiceProvider ServiceProvider
        {
            get
            {
                if (serviceProvider == null)
                {
                    hModule = LoadLibrary(serviceFile.FullName);

                    if (hModule != IntPtr.Zero)
                    {
                        var proc = GetProcAddress(hModule, "DllGetClassObject");

                        if (proc != IntPtr.Zero)
                        {
                            var getClassObject = (GetClassObjectHandler)Marshal.GetDelegateForFunctionPointer(proc, typeof(GetClassObjectHandler));
                            var IID_ClassFactory = typeof(IClassFactory).GUID;
                            var IID_IUnknown = new Guid("00000000-0000-0000-C000-000000000046");
                            var IID_ServiceProvider = typeof(Interop.IServiceProvider).GUID;
                            var objUnknown = (object)null;
                            var classFactory = (IClassFactory)null;
                            var list = new List<Guid>();
                            var guidAdd = Guid.Empty;
 
                            //Visual Studio Common IDE Package
                            guidAdd = new Guid("{6E87CFAD-6C05-4ADF-9CD7-3B7943875B7C}");
                            list.Add(guidAdd);
 
                            //Visual Studio Environment Menu Package
                            guidAdd = new Guid("{715F10EB-9E99-11D2-BFC2-00C04F990235}");
                            list.Add(guidAdd);
 
                            //Visual Studio COM+ Library Manager Package
                            guidAdd = new Guid("{ED8979BC-B02F-4dA9-A667-D3256C36220A}");
                            list.Add(guidAdd);
 
                            //Visual Studio Source Control Integration Package
                            guidAdd = new Guid("{53544C4D-E3F8-4AA0-8195-8A8D16019423}");
                            list.Add(guidAdd);
 
                            //Visual Studio Solution Build Package
                            guidAdd = new Guid("{282BD676-8B5B-11D0-8A34-00A0C91E2ACD}");
                            list.Add(guidAdd);
 
                            //Text Management Package
                            guidAdd = new Guid("{F5E7E720-1401-11d1-883B-0000F87579D2}");
                            list.Add(guidAdd);
 
                            //Visual Studio VsSettings Package
                            guidAdd = new Guid("{F74C5077-D848-4630-80C9-B00E68A1CA0C}");
                            list.Add(guidAdd);

                            foreach (var guid in list)
                            {
                                if (serviceProvider == null)
                                {
                                    var hr = getClassObject(typeof(SVsShell).GUID, IID_ClassFactory, out objUnknown);

                                    if (hr != 0)
                                    {
                                        Debugger.Break();
                                    }

                                    classFactory = (IClassFactory)objUnknown;

                                    hr = classFactory.CreateInstance(null, ref IID_ServiceProvider, out objUnknown);

                                    if (hr != 0)
                                    {
                                        Debugger.Break();
                                    }

                                    serviceProvider = (Interop.IServiceProvider)objUnknown;
                                }
                                else
                                {
                                    var pUnk = IntPtr.Zero;

                                    var hr = serviceProvider.QueryService(guid, IID_IUnknown, out pUnk);

                                    if (hr == 0)
                                    {
                                        var service = Marshal.GetObjectForIUnknown(pUnk);
                                    }
                                }
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
