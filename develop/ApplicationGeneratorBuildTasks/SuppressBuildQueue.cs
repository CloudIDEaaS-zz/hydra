using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Build.Framework;
using System.Diagnostics;

namespace BuildTasks
{
    public class KillCassini : ITask
    {
        public IBuildEngine BuildEngine { get; set; }
        public ITaskHost HostObject { get; set; }

        public bool Execute()
        {
            try
            {
                // Kill all current instances         

                var name = Path.GetFileNameWithoutExtension("WebDev.WebServer40.EXE");
                
                foreach (Process proc in Process.GetProcessesByName(name))         
                {
                    proc.Kill();
                }

                foreach (Process proc in Process.GetProcesses().Where(p => p.ProcessName.EndsWith("vshost")))
                {
                    if (!proc.ProcessName.StartsWith("Log2Console"))
                    {
                        proc.Kill();
                    }
                } 
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error killing Cassini instances: '{0}'", ex));
                return false;
            }

            return true;
        }
    }
}
