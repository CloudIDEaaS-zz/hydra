using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;

namespace VisualStudioProvider.Configuration.Services
{
    public class VsProfferService : IProfferService
    {
        private static uint cookieCounter;

        public int ProfferService(ref Guid rguidService, Microsoft.VisualStudio.OLE.Interop.IServiceProvider psp, out uint pdwCookie)
        {
            cookieCounter++;
            pdwCookie = cookieCounter;

            if (VSConfigProvider.Services.ContainsKey(rguidService))
            {
                VSConfigProvider.Services.Remove(rguidService);
            }

            VSConfigProvider.Services.Add(rguidService, new VSService(rguidService, psp));

            return 0;
        }

        public int RevokeService(uint dwCookie)
        {
            var service = VSConfigProvider.Services.Values.SingleOrDefault(s => s.Cookie == dwCookie);

            if (service != null)
            {
                VSConfigProvider.Services.Remove(service.ServiceGuid);
                service.Dispose();
            }

            return 0;
        }
    }
}
