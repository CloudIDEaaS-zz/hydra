using Leadtools;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace BingWebSearchTest
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            var license = @"d:\LEADTOOLS22\Support\Common\License\LEADTOOLS.LIC";
            var key = File.ReadAllText(@"d:\LEADTOOLS22\Support\Common\License\LEADTOOLS.LIC.key");
            var runUnitTests = bool.Parse(ConfigurationManager.AppSettings["RunUnitTests"]);

            RasterSupport.SetLicense(license, key);

            if (RasterSupport.KernelExpired)
            {
                Console.WriteLine("License file invalid or expired.");
                DebugUtils.Break();
            }
            else
            {
                Console.WriteLine("License file set successfully");
            }

            if (runUnitTests)
            {
                UnitTests.RunUnitTests();
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmTest());
        }
    }
}
