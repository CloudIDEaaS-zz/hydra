using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;

namespace VisualStudioProvider.Configuration.Services
{
    public class VsLocalRegistry : ILocalRegistry, ILocalRegistry2
    {
        public int CreateInstance(Guid clsid, object punkOuter, ref Guid riid, uint dwFlags, out IntPtr ppvObj)
        {
            throw new NotImplementedException();
        }

        public int GetClassObjectOfClsid(ref Guid clsid, uint dwFlags, IntPtr lpReserved, ref Guid riid, out IntPtr ppvClassObject)
        {
            throw new NotImplementedException();
        }

        public int GetTypeLibOfClsid(Guid clsid, out Microsoft.VisualStudio.OLE.Interop.ITypeLib pptLib)
        {
            throw new NotImplementedException();
        }

        public int GetClassObjectOfClsid(ref Guid clsid, uint dwFlags, IntPtr lpReserved, ref Guid riid, IntPtr ppvClassObject)
        {
            throw new NotImplementedException();
        }

        public int GetLocalRegistryRoot(out string pbstrRoot)
        {
            throw new NotImplementedException();
        }
    }
}
