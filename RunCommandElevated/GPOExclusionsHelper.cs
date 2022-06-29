using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RunCommandElevated
{
    public static class GPOExclusionsHelper
    {
        [DllImport("GPOExclusionsHelper.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AddLocation(Guid adminToolGuid, [MarshalAs(UnmanagedType.LPWStr)] string path);

        [DllImport("GPOExclusionsHelper.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int RemoveLocation(Guid adminToolGuid, [MarshalAs(UnmanagedType.LPWStr)] string path);

        [DllImport("GPOExclusionsHelper.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetPathExclusionsEnabled(Guid adminToolGuid, out bool enabled);

        [DllImport("GPOExclusionsHelper.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetPathExclusionsEnabled(Guid adminToolGuid, bool enabled);
    }
}
