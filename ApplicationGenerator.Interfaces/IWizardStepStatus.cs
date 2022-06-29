// file:	IWizardStepStatus.cs
//
// summary:	Declares the IWizardStepStatus interface

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBase;

namespace AbstraX
{
    /// <summary>   Interface for wizard step status. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/25/2021. </remarks>

    public interface IWizardStepStatus : IWizardSubPage
    {
        /// <summary>   Gets the user process status. </summary>
        ///
        /// <value> The user process status. </value>

        ctrlUserProcessStatus UserProcessStatus { get; }

        /// <summary>   Gets or sets the progress bar file size value. </summary>
        ///
        /// <value> The progress bar file size value. </value>

        int ProgressBarFileSizeValue { get; set; }

        /// <summary>   Gets or sets the actual file size value. </summary>
        ///
        /// <value> The actual file size value. </value>

        string LabelFileSizeValue { get; set; }

        /// <summary>   Gets or sets the pathname of the root directory. </summary>
        ///
        /// <value> The pathname of the root directory. </value>

        string RootDirectory { get; set; }

        /// <summary>   Gets or sets the stopwatch. </summary>
        ///
        /// <value> The stopwatch. </value>

        Stopwatch Stopwatch { get; set; }

        /// <summary>   Gets or sets the caption. </summary>
        ///
        /// <value> The caption. </value>

        string Caption { get; set; }

        /// <summary>   Stops the timers. </summary>
        /// 
        void StopTimers();

        /// <summary>   Sets the process. </summary>
        ///
        /// <param name="process">  The process. </param>

        void SetProcess(Process process);

        /// <summary>   Gets or sets the process handler. </summary>
        ///
        /// <value> The process handler. </value>

        IProcessHandler ProcessHandler { get; set; }

        /// <summary>   Reports the status. </summary>
        ///
        /// <param name="currentStep">      The current step. </param>
        /// <param name="status">           The status. </param>
        /// <param name="percentComplete">  The percent complete. </param>

        void ReportStatus(string currentStep, string status, int percentComplete);
    }
}
