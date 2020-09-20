using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;

namespace VisualStudioProvider.Configuration.Services
{
    public class VsRegisterProjectTypes : IVsRegisterProjectTypes
    {
        public int RegisterProjectType(ref Guid rguidProjType, IVsProjectFactory pVsPF, out uint pdwCookie)
        {
            var iid = Guid.NewGuid();
            var project = IntPtr.Zero;
            var canceled = 0;
            var href = 0;
            var flags = (uint)__VSCREATEPROJFLAGS.CPF_CLONEFILE;

            pdwCookie = 1;

            href = pVsPF.CreateProject(null, @"C:\Users\Ken\Documents\Mind Chemistry\Hydra\VisualStudioProvider.Services\ProjectFolder\FSharpProject.fsproj", @"C:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\IDE\ProjectTemplates\FSharp\Windows\1033\ConsoleApplication.zip", flags, ref iid, out project, out canceled);

            return 0;
        }

        public int UnregisterProjectType(uint dwCookie)
        {
            return 0;
        }
    }
}
