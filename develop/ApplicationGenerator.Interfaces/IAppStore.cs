using AbstraX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    public interface IAppStore
    {
        void SubmitToStore(IHydraAppsAdminServicesClientConfig hydraAppsAdminServicesClientConfig, IAppFolderStructureSurveyor appFolderStructureSurveyor, IDesktopForm desktopForm);
    }
}
