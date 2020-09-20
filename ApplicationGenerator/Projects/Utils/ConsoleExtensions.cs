using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;

namespace Utils
{
    public static class ConsoleExtensions
    {
        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern bool FreeConsole();
        private static bool isHidden;

        static ConsoleExtensions()
        {
            Application.ApplicationExit += (sender, e) =>
            {
                FreeConsole();
            };
        }

        public static void ShowConsole(this Form form)
        {
            ShowConsole();
        }

        public static bool AttachConsole(this Process process)
        {
            return AttachConsole(process.Id);
        }

        public static bool FreeConsole(this Process process)
        {
            return FreeConsole();
        }

        public static void ShowConsole()
        {
            if (isHidden)
            {
                var hwndConsole = ControlExtensions.GetConsoleWindow();

                ControlExtensions.ShowWindowAsync(hwndConsole, ControlExtensions.ShowWindowCommands.Show);

                isHidden = true;
            }
            else
            {
                if (!AllocConsole())
                {
                    DebugUtils.Break();
                }
            }
        }

        public static void HideConsole(this Form form)
        {
            HideConsole();
        }

        public static void HideConsole()
        {
            var hwndConsole = ControlExtensions.GetConsoleWindow();

            ControlExtensions.ShowWindowAsync(hwndConsole, ControlExtensions.ShowWindowCommands.Hide);

            isHidden = true;
        }
    }
}
