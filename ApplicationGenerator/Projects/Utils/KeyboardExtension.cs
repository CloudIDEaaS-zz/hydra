using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;
using System.Runtime.InteropServices;

namespace Utils
{
    public static class KeyboardExtension
    {
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(System.Windows.Forms.Keys vKey); 

        public static bool IsPressed(this Key key)
        {
            return Keyboard.IsKeyDown(key);
        }

        public static bool IsPressed(this Keys key)
        {
            var keyState = GetAsyncKeyState(key);

            return keyState < 1;
        }
    }
}
