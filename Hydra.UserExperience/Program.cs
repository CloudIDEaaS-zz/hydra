using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace Hydra.UserExperience
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            MessageBox.Show("Hydra.UserExperience. Hey!");

            //var parentProcess = Process.GetCurrentProcess().GetParent();

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);

            //parentProcess.Exited += (sender, e) =>
            //{
            //    Application.Run(new frmUserExperience());
            //};
        }
    }
}
