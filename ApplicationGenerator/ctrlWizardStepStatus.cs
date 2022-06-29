// file:	ctrlLayoutDesigner.cs
//
// summary:	Implements the control layout designer class

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;
using WizardBase;

namespace AbstraX
{
    /// <summary>   Designer for Control layout. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/22/2021. </remarks>

    public partial class ctrlWizardStepStatus : UserControl, IWizardSubPage, IWizardStepStatus
    {
        private Process process;
        /// <summary>   Gets or sets a value indicating whether this  is shown. </summary>
        ///
        /// <value> True if shown, false if not. </value>
        public bool Shown { get; set; }
        /// <summary>   Event queue for all listeners interested in DisableNext events. </summary>
        public event EventHandler DisableNext;
        /// <summary>   Event queue for all listeners interested in EnableNext events. </summary>
        public event EventHandler EnableNext;
        /// <summary>   Gets the user process status. </summary>
        ///
        /// <value> The user process status. </value>

        public ctrlUserProcessStatus UserProcessStatus
        {
            get
            {
                return ctrlUserProcessStatus;
            }
        }

        /// <summary>   Gets or sets the progress bar file size value. </summary>
        ///
        /// <value> The progress bar file size value. </value>

        public int ProgressBarFileSizeValue
        {
            get
            {
                return ctrlUserProcessStatus.ProgressBarFileSizeValue;
            }

            set
            {
                ctrlUserProcessStatus.ProgressBarFileSizeValue = value;
            }
        }

        /// <summary>   Gets or sets the actual file size value. </summary>
        ///
        /// <value> The actual file size value. </value>

        public string LabelFileSizeValue
        {
            get
            {
                return ctrlUserProcessStatus.LabelFileSizeValue;
            }

            set
            {
                ctrlUserProcessStatus.LabelFileSizeValue = value;
            }
        }

        /// <summary>   Gets or sets the pathname of the root directory. </summary>
        ///
        /// <value> The pathname of the root directory. </value>

        public string RootDirectory
        {
            get
            {
                return ctrlUserProcessStatus.RootDirectory;
            }

            set
            {
                ctrlUserProcessStatus.RootDirectory = value;
            }
        }

        /// <summary>   Gets or sets the process handler. </summary>
        ///
        /// <value> The process handler. </value>

        public IProcessHandler ProcessHandler
        {
            get
            {
                return ctrlUserProcessStatus.ProcessHandler;
            }

            set
            {
                ctrlUserProcessStatus.ProcessHandler = value;
            }
        }

        /// <summary>   Gets or sets the stopwatch. </summary>
        ///
        /// <value> The stopwatch. </value>

        public Stopwatch Stopwatch 
        {
            get
            {
                return ctrlUserProcessStatus.Stopwatch;
            }

            set
            {
                ctrlUserProcessStatus.Stopwatch = value;
            }
        }

        /// <summary>   Stops the timers. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/7/2021. </remarks>

        public void StopTimers()
        {
            ctrlUserProcessStatus.StopTimers();
        }

        /// <summary>   Gets or sets the caption. </summary>
        ///
        /// <value> The caption. </value>

        public string Caption
        {
            get
            {
                if (labelCaption != null)
                {
                    return labelCaption.Text;
                }
                else
                {
                    return string.Empty;
                }
            }

            set
            {
                labelCaption.Text = value;
            }
        }


        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/22/2021. </remarks>
        public ctrlWizardStepStatus()
        {
            InitializeComponent();
        }

        /// <summary>   Initializes this.  </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/22/2021. </remarks>
        ///
        /// <param name="wizardSettingsBase">   The wizard settings base. </param>

        public void Initialize(WizardSettingsBase wizardSettingsBase)
        {
        }

        /// <summary>   Saves. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/22/2021. </remarks>
        ///
        /// <param name="wizardSettingsBase">   The wizard settings base. </param>
        /// <param name="isNext">               True if is next, false if not. </param>

        public void Save(WizardSettingsBase wizardSettingsBase, bool isNext)
        {
        }

        /// <summary>   Sets the process. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/25/2021. </remarks>
        ///
        /// <param name="process">  The process. </param>

        public void SetProcess(Process process)
        {
            this.process = process;

            ctrlUserProcessStatus.Process = process;
        }

        /// <summary>   Reports the status. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/25/2021. </remarks>
        ///
        /// <param name="currentStep">      The current step. </param>
        /// <param name="status">           The status. </param>
        /// <param name="percentComplete">  The percent complete. </param>

        public void ReportStatus(string currentStep, string status, int percentComplete)
        {
            ctrlUserProcessStatus.ReportStatus(currentStep, status, percentComplete);
        }

        private void labelCaption_Paint(object sender, PaintEventArgs e)
        {
            labelCaption.DrawGradient(e);
        }
    }
}
