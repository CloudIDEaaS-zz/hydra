using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;
using Utils.ProcessHelpers;
using System.Diagnostics;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;

namespace BingWebSearchTest
{
    public static class UnitTests
    {
        private static string testName = string.Empty;
        private static bool testWritingToConsole;

        private static string TestName
        {
            get
            {
                return testName;
            }

            set
            {
                Console.WriteLine("");
                Console.Write("Running {0}", value);

                testWritingToConsole = false;

                testName = value;
            }
        }

        private static string WriteToConsoleTestName
        {
            get
            {
                return testName;
            }

            set
            {
                Console.WriteLine("");
                Console.WriteLine("\tRunning {0}", value);

                testWritingToConsole = true;

                testName = value;
            }
        }

        /// <summary>   A string extension method that starts. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 6/18/2021. </remarks>
        ///
        /// <param name="testName">         The testName to act on. </param>
        /// <param name="writesToConsole">  (Optional) True to writes to console. </param>
        ///
        /// <returns>   An IDisposable. </returns>

        public static IDisposable Start(this string testName, bool writesToConsole = false)
        {
            if (writesToConsole)
            {
                WriteToConsoleTestName = testName;
                return testName.CreateDisposable(() => Console.WriteLine("\t{0} Completed", testName));
            }
            else
            {
                TestName = testName;
                return testName.CreateDisposable(() => Console.Write("\t... completed"));
            }
        }

        internal static void RunUnitTests()
        {
            try
            {
                ControlExtensions.ShowConsoleInSecondaryMonitor(FormWindowState.Normal);

                Console.WriteLine("Starting Unit Tests"); 

                // TestShortcuts 
                {
                    using ("Test Color Change".Start(true))
                    {
                        TestColorChange();
                    }
                }
            }
            catch (Exception ex)
            {
                var hwndConsole = ControlExtensions.GetConsoleWindow();

                Console.WriteLine("UnitTest: '{0}' failed.  Exception: {1}", TestName, ex.Message);
                ControlExtensions.Flash(hwndConsole, FlashWindowFlags.FLASHW_ALL | FlashWindowFlags.FLASHW_TIMERNOFG, 0, 1000);
                return;
            }

            Console.WriteLine("\r\nUnit Tests Completed Successfully!\r\n");
        }

        private static void TestColorChange()
        {
            var image = (Bitmap) Image.FromFile(@"D:\MC\CloudIDEaaS\develop\BingWebSearchTest\TestData\62378f98-02ef-4132-886b-4153618c189a.jpeg");
            var colorMap = image.GetColorMap().ToList();
        }
    }
}
