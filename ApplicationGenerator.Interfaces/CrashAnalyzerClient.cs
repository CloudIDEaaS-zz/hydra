// file:	CrashAnalyzerClient.cs
//
// summary:	Implements the crash analyzer client class

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX
{
    /// <summary>   A crash analyzer client. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 4/16/2021. </remarks>

    public class CrashAnalyzerClient : BaseWindowsCommandHandler
    {
        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/16/2021. </remarks>

        public CrashAnalyzerClient() : base(GetAnalyzerPath())
        {
            this.NoWait = true;
        }

        private static string GetAnalyzerPath()
        {
            var assemblyExeFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = Path.GetFullPath(Path.Combine(assemblyExeFolder, ConfigurationSettings.AppSettings["CrashAnalyzerExeLocation"]));

            if (!File.Exists(path))
            {
                DebugUtils.Break();
            }

            return path;
        }

        /// <summary>   Executes the analysis operation. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/16/2021. </remarks>
        ///
        /// <param name="processId">        Identifier for the process. </param>
        /// <param name="managedThreadId">  Identifier for the managed thread. </param>
        /// <param name="threadId">         Identifier for the thread. </param>
        /// <param name="dumpLogFile">      The dump log file. </param>
        /// <param name="runAsAutomated">     True if run automated. </param>

        public void RunAnalysis(int processId, int managedThreadId, uint threadId, FileInfo dumpLogFile, bool runAsAutomated)
        {
            base.RunCommand(string.Empty, Environment.CurrentDirectory, processId.ToString(), managedThreadId.ToString(), threadId.ToString(), dumpLogFile.FullName, runAsAutomated ? "-runAsAutomated" : string.Empty);
        }
    }
}
