using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VisualStudioProvider.Configuration
{
    //
    // 00 01 00 00
    // 00 00 00 00
    // f0 42 f6 00		0xf642f0
    // 0a 00 00 00
    // 3b 21 f7 2f 	0x2ff7213b
    // 00 00 00 00
    // 30 6b f0 00 	0xf06b30
    // 00 00 00 00
    // d8 4a f0 00		0xf04ad8
    //
    
    [StructLayout(LayoutKind.Sequential)]
    public class MainParam
    {
        public int version;
        public int notused1;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string exeLocation;
        public int int1 = 10;
        public IntPtr ptr1;
        public int notused2;
        public IntPtr ptr2;
        public IntPtr ptr3;
        public IntPtr ptr4;
        public IntPtr ptr5;
        public IntPtr ptr6;
        public IntPtr ptr7;
        public IntPtr ptr8;
        public IntPtr ptr9;
        public IntPtr ptr10;
        public IntPtr ptr11;
        public IntPtr ptr12;
        public IntPtr ptr13;
        public IntPtr ptr14;
        public IntPtr ptr15;
        public IntPtr ptr16;
        public IntPtr ptr17;
        public IntPtr ptr18;
        public IntPtr ptr19;
        public IntPtr ptr20;
        public IntPtr ptr21;
        public IntPtr ptr22;
        public IntPtr ptr23;
        public IntPtr ptr24;
        public IntPtr ptr25;
        public IntPtr ptr26;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string registryRoot;
        public IntPtr ptr27;
        public IntPtr ptr28;
        public IntPtr ptr29;
        public IntPtr ptr30;
        public IntPtr ptr31;
        public IntPtr ptr32;
        public IntPtr ptr33;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string userDataFolder;
    }
}
