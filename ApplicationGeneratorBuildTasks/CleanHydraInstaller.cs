using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Build.Framework;
using System.Diagnostics;
using System.Xml.XPath;
using System.Xml;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using EnvDTE;
using Process = System.Diagnostics.Process;
using Debugger = System.Diagnostics.Debugger;
using Microsoft.VisualStudio.ExtensionManager;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO.Packaging;
using Utils;
using System.Xml.Linq;

namespace BuildTasks
{
    public class CleanHydraInstaller : ITask
    {
        public IBuildEngine BuildEngine { get; set; }
        public ITaskHost HostObject { get; set; }
        [Required]
        public string Configuration { get; set; }

        public bool Execute()
        {
            try
            {
                if (Keys.ControlKey.IsPressed())
                {
                    if (MessageBox.Show("Debug?", "Debug CleanHydraInstaller", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        Debugger.Launch();
                    }
                }

                if (this.Configuration == "PreRelease")
                {
                    var solutionPath = Path.GetFullPath(Environment.ExpandEnvironmentVariables("%HYDRASOLUTIONPATH%"));
                    var productFilePath = Path.Combine(solutionPath, @"Hydra.InstallerStandalone\Product.wxs");
                    var document = XDocument.Load(productFilePath);
                    var namespaceManager = new XmlNamespaceManager(new NameTable());
                    XElement elementFeature;
                    XElement elementDirectory;

                    namespaceManager.AddNamespace("wi", "http://schemas.microsoft.com/wix/2006/wi");

                    elementFeature = document.XPathSelectElement("/wi:Wix/wi:Product/wi:Feature", namespaceManager);
                    elementDirectory = document.XPathSelectElement("/wi:Wix/wi:Fragment/wi:Directory", namespaceManager);

                    elementFeature.Elements().Remove();
                    elementDirectory.Elements().Remove();

                    document.Save(productFilePath, SaveOptions.DisableFormatting);
                }
            }
            catch (Exception ex)
            {
                var error = string.Format("Error installing Hydra Visual Studio templates. \r\nError: {0}", ex.ToString());

                var message = new BuildErrorEventArgs(string.Empty, string.Empty, string.Empty, 0, 0, 0, 0, error, "", "", DateTime.Now);
                this.BuildEngine.LogErrorEvent(message);

                Console.WriteLine(error);

                return false;
            }

            return true;
        }
    }
}
