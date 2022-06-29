// file:	IProcessHandler.cs
//
// summary:	Declares the IProcessHandler interface

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX
{
    /// <summary>   Delegate for handling Process events. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 4/6/2021. </remarks>
    ///
    /// <param name="sender">   Source of the event. </param>
    /// <param name="e">        Process event information. </param>

    public delegate void ProcessEventHandler(object sender, ProcessEventArgs e);

    /// <summary>   Additional information for process events. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 4/6/2021. </remarks>

    public class ProcessEventArgs : EventArgs
    {
        /// <summary>   Gets the process. </summary>
        ///
        /// <value> The process. </value>

        public Process Process { get; }

        /// <summary>   Gets the status. </summary>
        ///
        /// <value> The status. </value>

        public string Status { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/6/2021. </remarks>
        ///
        /// <param name="process">  The process. </param>
        /// <param name="status">   The status. </param>

        public ProcessEventArgs(Process process, string status)
        {
            this.Process = process;
            this.Status = status;
        }

    }

    /// <summary>   The process handler. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 4/5/2021. </remarks>

    public interface IProcessHandler
    {
        /// <summary>   Event queue for all listeners interested in process events. </summary>
        event ProcessEventHandler ProcessEvent;

        /// <summary>   Searches for the first match. </summary>
        ///
        /// <param name="workingDirectory"> Pathname of the working directory. </param>
        ///
        /// <returns>   A Process[]. </returns>

        Process[] Find(string workingDirectory);

        /// <summary>   Kills this. </summary>
        ///
        /// <param name="processes">    The processes. </param>

        void Kill(Process[] processes);

        /// <summary>   Reports the process. </summary>
        ///
        /// <param name="process">      The process. </param>
        /// <param name="logWriter">    The log writer. </param>

        void ReportProcess(Process process, ILogWriter logWriter);
    }
}
