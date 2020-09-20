using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interop = Microsoft.VisualStudio.OLE.Interop;
using System.Runtime.InteropServices;

namespace VisualStudioProvider.Configuration
{
    public class VSService : IDisposable 
    {
        public Guid ServiceGuid { get; protected set; }
        public string Name { get; protected set; }
        public uint Cookie { get; set; }
        protected Interop.IServiceProvider serviceProvider;

        public VSService(Guid serviceGuid)
        {
            this.ServiceGuid = serviceGuid;
        }

        public int AddRef()
        {
            return Marshal.AddRef(Marshal.GetIUnknownForObject(serviceProvider));
        }

        public int Release()
        {
            return Marshal.Release(Marshal.GetIUnknownForObject(serviceProvider));
        }

        public VSService(Guid serviceGuid, Interop.IServiceProvider serviceProvider)
        {
            this.ServiceGuid = serviceGuid;
            this.serviceProvider = serviceProvider;
        }

        public virtual Interop.IServiceProvider ServiceProvider
        {
            get
            {
                return serviceProvider;
            }
        }

        public virtual void Dispose()
        {
            if (serviceProvider != null)
            {
                Release();
            }
        }
    }
}
