using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;

namespace VisualStudioProvider.Configuration.Services
{
    public class VsUIHostLocale : IUIHostLocale
    {
        public int GetDialogFont(UIDLGLOGFONT[] pLOGFONT)
        {
            return 0;
        }

        public int GetUILocale(out uint plcid)
        {
            plcid = 1033;

            return 0;
        }
    }
}
