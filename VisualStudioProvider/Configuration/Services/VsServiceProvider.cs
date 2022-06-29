using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using VisualStudioProvider.Configuration;
using System.Runtime.InteropServices;
using System.Reflection;
using Interop = Microsoft.VisualStudio.OLE.Interop;
using System.Diagnostics;

namespace VisualStudioProvider.Configuration.Services
{
    public class VsServiceProvider : Interop.IServiceProvider
    {
        private Dictionary<Guid, Type> serviceLookupTable;
        private bool corePackagesLoaded;

        public VsServiceProvider()
        {
            CreateLookupTable();
        }

        private void LoadCorePackages()
        {
            var list = new List<Guid>();
            var guidAdd = Guid.Empty;
            var environmentService = VSConfigProvider.EnvironmentService;

            //Undo Package
            guidAdd = new Guid("{1D76B2E0-F11B-11D2-AFC3-00105A9991EF}");
            list.Add(guidAdd);
 
            //Visual Studio Environment Package
            guidAdd = new Guid("{DA9FB551-C724-11d0-AE1F-00A0C90FFFC3}");
            list.Add(guidAdd);
 
            //Visual Studio Commands Definition Package
            guidAdd = new Guid("{44E07B02-29A5-11D3-B882-00C04F79F802}");
            list.Add(guidAdd);
 
            //Visual Studio Directory Listing Package
            guidAdd = new Guid("{5010C52F-44AB-4051-8CE1-D36C20D989B4}");
            list.Add(guidAdd);
 
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
                if (environmentService.OtherPackages.ContainsKey(guid))
                {
                    var href = 0;
                    var pUnk = IntPtr.Zero;
                    var package = VSConfigProvider.Packages[guid];
                    var serviceProvider = package.ClassName;
                    var objUnknown = Marshal.GetIUnknownForObject(serviceProvider);
                    var vsPackage = (IVsPackage)null;

                    href = Marshal.QueryInterface(objUnknown, ref VSPackage.IID_IVsPackage, out pUnk);

                    if (href == 0)
                    {
                        vsPackage = (IVsPackage)Marshal.GetObjectForIUnknown(pUnk);

                        vsPackage.SetSite(this);
                    }
                }
                else if (VSConfigProvider.Services.ContainsKey(guid))
                {
                    var href = 0;
                    var pUnk = IntPtr.Zero;
                    var service = VSConfigProvider.Services[guid];
                    var serviceProvider = service.ServiceProvider;
                    var objUnknown = Marshal.GetIUnknownForObject(serviceProvider);
                    var vsPackage = (IVsPackage)null;

                    href = Marshal.QueryInterface(objUnknown, ref VSPackage.IID_IVsPackage, out pUnk);

                    if (href == 0)
                    {
                        vsPackage = (IVsPackage)Marshal.GetObjectForIUnknown(pUnk);

                        vsPackage.SetSite(this);
                    }
                }
                else if (VSConfigProvider.Packages.ContainsKey(guid))
                {
                    var href = 0;
                    var pUnk = IntPtr.Zero;
                    var package = VSConfigProvider.Packages[guid];
                    var className = package.ClassName;
                }
            }

            corePackagesLoaded = true;
        }

        private void CreateLookupTable()
        {
            var name = Assembly.GetExecutingAssembly().GetReferencedAssemblies().Single(a => a.Name == "Microsoft.VisualStudio.Shell.Interop");
            var shellAssembly = Assembly.Load(name);

            serviceLookupTable = new Dictionary<Guid, Type>();

            shellAssembly.GetTypes().ToList().ForEach(t => 
            {
                if (t.IsInterface)
                {
                    var attributes = t.GetCustomAttributes(true).Cast<Attribute>().Where(a => a is GuidAttribute);

                    if (attributes.Count() > 0)
                    {
                        var guidAttribute = (GuidAttribute) attributes.First();
                        var guid = Guid.Parse(guidAttribute.Value);

                        if (serviceLookupTable.ContainsKey(guid))
                        {
                            var currentType = serviceLookupTable[guid];

                            if (t.Name.StartsWith("S") && !currentType.Name.StartsWith("S"))
                            {
                                serviceLookupTable.Remove(guid);
                                serviceLookupTable.Add(guid, t);
                            }
                        }
                        else
                        {
                            serviceLookupTable.Add(guid, t);
                        }
                    }
                }
            });
        }

        public Dictionary<Guid, Type> ServiceLookupTable
        {
            get
            {
                return serviceLookupTable;
            }
        }

        public int QueryService(ref Guid guidService, ref Guid riid, out IntPtr ppvObject)
        {
            if (guidService == typeof(SProfferService).GUID)
            {
                ppvObject = Marshal.GetIUnknownForObject(new VsProfferService());
            }
            else if (guidService == typeof(SUIHostLocale).GUID)
            {
                ppvObject = Marshal.GetIUnknownForObject(new VsUIHostLocale());
            }
            else if (guidService == typeof(SVsSolution).GUID)
            {
                ppvObject = Marshal.GetIUnknownForObject(new VsSolution());
            }
            else if (guidService == typeof(SVsRegisterProjectTypes).GUID)
            {
                ppvObject = Marshal.GetIUnknownForObject(new VsRegisterProjectTypes());
            }
            else if (VSConfigProvider.Services.ContainsKey(guidService))
            {
                var service = VSConfigProvider.Services[guidService];

                ppvObject = Marshal.GetIUnknownForObject(service.ServiceProvider);
            }
            else
            {
                if (serviceLookupTable.ContainsKey(guidService))
                {
                    Debug.WriteLine(serviceLookupTable[guidService].Name);
                }

                if (!corePackagesLoaded)
                {
                    LoadCorePackages();
                }

                ppvObject = IntPtr.Zero;

                return 1;
            }

            return 0;
        }
    }
}
