// file:	ctrlLayoutDesigner.cs
//
// summary:	Implements the control layout designer class

using AppStoreInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;
using WizardBase;
using static AbstraX.Extensions;

namespace AbstraX.WizardPages
{
    /// <summary>   Designer for Control layout. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/22/2021. </remarks>

    [WizardPage("Foundation")]
    public partial class ctrlWizardStepFoundation : UserControl, IWizardStepDesign, IWizardStepInitialGenerator
    {
        private Process process;
        public IResourceData ResourceData { get; set; }
        public string LocalThemeFolder { get; set; }
        public bool Shown { get; set; }

        /// <summary>   Event queue for all listeners interested in DisableNext events. </summary>
        public event EventHandler DisableNext;
        /// <summary>   Event queue for all listeners interested in EnableNext events. </summary>
        public event EventHandler EnableNext;
        public event EventHandler OnPageValid;
        public event EventHandlerT<Exception> OnPageInvalid;

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/22/2021. </remarks>

        public unsafe ctrlWizardStepFoundation()
        {
#if TEST_SCANNER_MESSAGES
            Task.Run(() =>
            {
                IntPtr* getKey;
                AccessKey accessKey = null;
                GetKeyDelegate getKeyDelegate;
                var pGetKey = IntPtr.Zero;
                var hydraAppsAdminServicesClient = new HydraAppsAdminServicesClient(null, null);

                getKey = &pGetKey;

                hydraAppsAdminServicesClient.Initialize(ref getKey);

                accessKey = new AccessKey { Key = Guid.NewGuid(), UserName = Environment.UserName };
                getKeyDelegate = new GetKeyDelegate(() => accessKey);

                pGetKey = Marshal.GetFunctionPointerForDelegate(getKeyDelegate);
                getKey = &pGetKey;

                while (true)
                {
                    Application.DoEvents();
                    Thread.Sleep(100);
                }
            });
#endif
            InitializeComponent();
        }

        private AccessKey GetKey(AccessKey accessKey)
        {
            return accessKey;
        }

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

        public ctrlUserProcessStatus UserProcessStatus
        {
            get
            {
                return ctrlUserProcessStatus;
            }
        }

        public void StopTimers()
        {
            ctrlUserProcessStatus.StopTimers();
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

        public void InitializeControl(string workingDirectory)
        {
        }

        public void ValidatePage()
        {
        }

        public void SetProcess(Process process)
        {
            this.process = process;

            ctrlUserProcessStatus.Process = process;
        }

        public void ReportStatus(string currentStep, string status, int percentComplete)
        {
            ctrlUserProcessStatus.ReportStatus(currentStep, status, percentComplete);
        }

        public void Start(Action action)
        {
            ctrlUserProcessStatus.StartIgnition(action);
        }

        private void labelCaption_Paint(object sender, PaintEventArgs e)
        {
            labelCaption.DrawGradient(e);
        }
    }
}