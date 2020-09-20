using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Utils
{
    public delegate int WndProcHandler(IntPtr hWnd, uint msg, int wParam, IntPtr lParam);

    public class WindowsHook 
    {
        public event WndProcHandler OnMessage;
        private ControlExtensions.WndProcDelegate wndProc;

        public int DelegatePtr
        {
            get
            {
                wndProc = new Utils.ControlExtensions.WndProcDelegate(WndProc);

                return Marshal.GetFunctionPointerForDelegate(wndProc).ToInt32();
            }
        }

        public int WndProc(IntPtr hWnd, uint msg, int wParam, IntPtr lParam)
        {
            var result = OnMessage(hWnd, msg, wParam, lParam);

            return result;
        }
    }
}
