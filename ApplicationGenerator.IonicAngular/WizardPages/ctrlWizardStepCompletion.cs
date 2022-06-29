// file:	ctrlLayoutDesigner.cs
//
// summary:	Implements the control layout designer class

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WizardBase;
using Utils;
using System.Drawing.Drawing2D;
using static Utils.ControlExtensions;
using System.Threading;
using AbstraX.Builds;
using AbstraX.Handlers.CommandHandlers;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Net.Http.Formatting;
using Moq;
using System.Runtime.InteropServices;
using static AbstraX.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Logging.Memory;
using Utils.InProcessTransactions;
using System.IO;
using System.Net.Http.Headers;
using AppStoreInterfaces;

namespace AbstraX.WizardPages
{
    /// <summary>   Designer for Control layout. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/22/2021. </remarks>

    [WizardPage("Completion")]

    public partial class ctrlWizardStepCompletion : UserControl, IWizardStepFinal, IWizardStepStatus, ILogWriter
    {
        private Process process;
        private WizardSettingsBase settings;
        private IAppFolderStructureSurveyor appFolderStructureSurveyor;
        private IResourceData resourceData;
        private string generatorHandlerType;
        private WizardControl wizardControl;
        private IHydraAppsAdminServicesClientConfig hydraAppsAdminServicesClientConfig;
        private IAppTargetsBuilder appsTargetsBuilder;
        private bool errorMode;
        private ManualResetEvent manualResetEvent;
        private Dictionary<LinkLabel, bool> defaultLinkState;
        private AppDomain appDomain;
        private ServiceProvider serviceProvider;
        private ILogger logger;
        private IDesktopForm desktopForm;
        private string currentProcessName;

        public ctrlUserProcessStatus UserProcessStatus => throw new NotImplementedException();
        public IList<string> CommandLog { get; set; }
        public bool Shown { get; set; }
        /// <summary>   Event queue for all listeners interested in DisableNext events. </summary>
        public event EventHandler DisableNext;
        /// <summary>   Event queue for all listeners interested in EnableNext events. </summary>
        public event EventHandler EnableNext;
        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/22/2021. </remarks>

        public ctrlWizardStepCompletion()
        {
            appDomain = AppDomain.CurrentDomain;
            serviceProvider = (ServiceProvider)AppDomain.CurrentDomain.GetData("serviceProvider");
            logger = serviceProvider.GetService<ILogger>();

            manualResetEvent = new ManualResetEvent(false);

            InitializeComponent();

            panelPreviewAppTile.AddRoundedCorners(3);
            panelPreviewAppTileShadow.AddRoundedCorners(3);
        }

        public int ProgressBarFileSizeValue 
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public string LabelFileSizeValue
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public string RootDirectory 
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public IProcessHandler ProcessHandler
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public Stopwatch Stopwatch
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        public void StopTimers()
        {
            throw new NotImplementedException();
        }

        protected override void CreateHandle()
        {
            base.CreateHandle();
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

        public bool EnableLinks
        {
            set
            {
                this.Invoke(() =>
                {
                    if (defaultLinkState == null)
                    {
                        defaultLinkState = tableLayoutPanel.GetAllControls().OfType<LinkLabel>().ToDictionary(l => l, l => l.Enabled);
                    }

                    if (value)
                    {
                        defaultLinkState.ForEach(p => p.Key.Enabled = p.Value);
                    }
                    else
                    {
                        tableLayoutPanel.GetAllControls().OfType<LinkLabel>().ToList().ForEach(l => l.Enabled = value);
                    }
                });
            }
        }

        public bool UISubmitButtonEnabled 
        {
            get
            {
                return cmdSubmitToHydraStore.Enabled;
            }

            set
            {
                cmdSubmitToHydraStore.Enabled = value;
            }
        }


        /// <summary>   Initializes this.  </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/22/2021. </remarks>
        ///
        /// <param name="wizardSettingsBase">   The wizard settings base. </param>

        public void Initialize(WizardSettingsBase wizardSettingsBase)
        {
            settings = wizardSettingsBase;
            appFolderStructureSurveyor = (IAppFolderStructureSurveyor) wizardSettingsBase["AppLayoutSurveyor"];
            resourceData = (IResourceData)wizardSettingsBase["ResourceData"];
            generatorHandlerType = (string)wizardSettingsBase["GeneratorHandlerType"];
            wizardControl = (WizardControl)wizardSettingsBase["WizardControl"];
            hydraAppsAdminServicesClientConfig = (IHydraAppsAdminServicesClientConfig)wizardSettingsBase["HydraAppsAdminServicesClientConfig"];
            this.CommandLog = (IList<string>)wizardSettingsBase["CommandLog"];

            wizardControl.NextButtonEnabled = false;
            this.EnableLinks = false;
            
            Task.Run(() =>
            {
                appsTargetsBuilder = AbstraXExtensions.GetAppTargetsBuilder(generatorHandlerType);

                this.Invoke(() =>
                {
                    appFolderStructureSurveyor.Refresh();

                    UpdateAppTile();

                    wizardControl.NextButtonEnabled = true;
                    this.EnableLinks = true;

                    if (appFolderStructureSurveyor.Builds.ContainsKey("Web"))
                    {
                        cmdRunLocally.Enabled = true;
                        cmdSubmitToHydraStore.Enabled = true;

                        cmdSubmitToHydraStore.Focus();
                    }
                    else
                    {
                        wizardControl.NextButton.Focus();
                    }

                    appsTargetsBuilder.OnCommand += AppsTargetsBuilder_OnCommand;
                });

                manualResetEvent.Set();

                ctrlUserProcessStatus.StopSizeThread();
            });
        }

        private void AppsTargetsBuilder_OnCommand(object sender, EventArgs<string> e)
        {
            var command = e.Value;

            this.CommandLog.Add(command);
        }

        private void UpdateAppTile()
        {
            var logo = resourceData.Logo;
            var organizationName = resourceData.OrganizationName;
            var appName = resourceData.AppName;
            var resourceDefaults = AbstraXExtensions.GetResourceDefaults();

            if (resourceData.Logo.IsNullOrEmpty())
            {
                pictureBoxLogo.Image = resourceDefaults.Logo;
            }
            else
            {
                using (var imageOriginal = (Bitmap)Bitmap.FromFile(resourceData.Logo))
                {
                    pictureBoxLogo.Image = imageOriginal.Clone(new Rectangle(0, 0, imageOriginal.Width, imageOriginal.Height), imageOriginal.PixelFormat);
                }
            }

            pictureBoxLogo.Refresh();
            this.DoEvents();

            lblAppName.Text = appName;
            lblOrganizationName.Text = organizationName;

            lblViewAppInHydraStore.Text = string.Format(lblViewAppInHydraStore.Text, appName);

            ctrlImageOverlay.Refresh();
        }

        /// <summary>   Saves. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/22/2021. </remarks>
        ///
        /// <param name="wizardSettingsBase">   The wizard settings base. </param>
        /// <param name="isNext">               True if is next, false if not. </param>

        public void Save(WizardSettingsBase wizardSettingsBase, bool isNext)
        {
            Task.Run(() =>
            {
                manualResetEvent.WaitOne();

                BuildApp();
            });
        }

        public void SetProcess(Process process)
        {
            this.process = process;
        }

        public void ReportStatus(string currentStep, string status, int percentComplete)
        {
        }

        private void labelCaption_Paint(object sender, PaintEventArgs e)
        {
            labelCaption.DrawGradient(e);
        }

        private void cmdBuildApp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Task.Run(() =>
            {
                BuildApp();
            });
        }

        private void BuildApp()
        {
            var platforms = appsTargetsBuilder.GetSupportedPlatforms();

            this.Invoke(() =>
            {
                this.EnableLinks = false;
                pictBuildApp.ToInProcess();
            });

            appsTargetsBuilder.OnBuildResults += OnBuildResults;

            foreach (var platform in platforms)
            {
                var buildSelections = platform.BuildSelections;

                foreach (var key in platform.BuildSelections.Keys.ToList())
                {
                    if (platform.Name.IsOneOf("AndroidBuilder", "iOSBuilder"))
                    {
                        if (key.NotOneOf("Android", "iOS"))
                        {
                            buildSelections[key] = true;
                        }
                    }
                    else
                    {
                        buildSelections[key] = true;
                    }
                }
            }

            appsTargetsBuilder.Build(generatorHandlerType, appFolderStructureSurveyor, platforms, this);

            this.Invoke(() =>
            {
                var webPlatform = platforms.SingleOrDefault(p => p.Name == "WebBuilder");
                var parentForm = this.ParentForm;

                parentForm.ShowFloatingMessageBox("Build Complete", @"Images\RibbonCutting.png", 3000);

                this.EnableLinks = true;

                if (webPlatform.Builds["Web"].Succeeded)
                {
                    if (webPlatform.BuildSelections["SQLServer"] && webPlatform.Builds["SQLServer"].Succeeded)
                    {
                        cmdSubmitToHydraStore.Enabled = true;
                        cmdRunLocally.Enabled = true;
                        pictBuildApp.ToComplete();

                        this.WriteLine("\r\n{0} build complete".ToUpper(), appFolderStructureSurveyor.AppName.ToUpper());
                    }
                    else
                    {
                        cmdSubmitToHydraStore.Enabled = true;
                        cmdRunLocally.Enabled = true;
                        pictBuildApp.ToComplete();

                        this.WriteLine("\r\n{0} build complete".ToUpper(), appFolderStructureSurveyor.AppName.ToUpper());
                    }
                }
            });
        }

        private void OnBuildResults(object sender, BuildResultEventArgs e)
        {
            this.Invoke(() =>
            {
                var buttons = toolStripPlatforms.Items.OfType<ToolStripButton>().ToList();
                var buttonCount = buttons.Count;
                var checkedButtons = buttons.Count(b => b.Checked);

                if (e.PercentageOfName == null)
                {
                    foreach (var percentageOfName in e.PercentageOfNames)
                    {
                        var button = buttons.SingleOrDefault(b => b.Text == percentageOfName);
                        var percent = (int)((1.ToPercentageOf(buttonCount).As<float>() / 100) * e.Percentage.As<float>());

                        toolStripBuildProgressBar.Value += Math.Min(100 - toolStripBuildProgressBar.Value, percent);

                        if (button != null)
                        {
                            button.Checked = true;
                        }

                        this.DoEvents();
                        Thread.Sleep(100);

                        checkedButtons++;
                    }
                }
                else
                {
                    var button = buttons.SingleOrDefault(b => b.Text == e.Name);

                    if (button == null)
                    {
                        var percent = (int)((1.ToPercentageOf(buttonCount).As<float>() / 100) * e.Percentage.As<float>());

                        button = buttons.SingleOrDefault(b => b.Text == e.PercentageOfName);

                        toolStripBuildProgressBar.Value += Math.Min(100 - toolStripBuildProgressBar.Value, percent);

                        if (button != null)
                        {
                            button.Checked = true;
                        }
                    }
                    else
                    {
                        button.Checked = true;
                        toolStripBuildProgressBar.Value = checkedButtons.ToPercentageOf(buttonCount);
                    }

                    this.DoEvents();
                }
            });
        }

        public void Write(string value)
        {
            this.Invoke(() =>
            {
                if (errorMode)
                {
                    txtOutput.AppendText(value);
                }
                else
                {
                    txtOutput.AppendText(value);
                }

                txtOutput.ScrollToEnd();

                this.DoEvents();
            });
        }

        public void Write(string format, params object[] args)
        {
            this.Invoke(() =>
            {
                if (errorMode)
                {
                    txtOutput.AppendText(Color.Maroon, string.Format(format, args));
                }
                else
                {
                    txtOutput.AppendText(string.Format(format, args));
                }

                txtOutput.ScrollToEnd();

                this.DoEvents();
            });
        }

        public void WriteLine(string value)
        {
            this.Invoke(() =>
            {
                if (errorMode)
                {
                    txtOutput.AppendText(Color.Maroon, value);
                }
                else
                {
                    txtOutput.AppendLine(value);
                }

                txtOutput.ScrollToEnd();

                this.DoEvents();
            });
        }

        public void WriteLine()
        {
            this.Invoke(() =>
            {
                if (errorMode)
                {
                    txtOutput.AppendLine(string.Empty);
                }
                else
                {
                    txtOutput.AppendLine(string.Empty);
                }

                txtOutput.ScrollToEnd();

                this.DoEvents();
            });
        }

        public void WriteLine(string format, params object[] args)
        {
            this.Invoke(() =>
            {
                if (errorMode)
                {
                    txtOutput.AppendLine(Color.Maroon, string.Format(format, args).FormatEscape());
                }
                else
                {
                    txtOutput.AppendLine(format, args);
                }

                txtOutput.ScrollToEnd();

                this.DoEvents();
            });
        }

        public IDisposable ErrorMode()
        {
            this.Invoke(() =>
            {
                errorMode = true;
            });

            return this.CreateDisposable(() => errorMode = false);
        }

        private void cmdClearLog_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            txtOutput.Text = string.Empty;
        }

        private void cmdRunLocally_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var hydraCommandHandler = new HydraCommandHandler();
            var builder = new StringBuilder();
            var firstCheck = false;

            pictRunLocally.ToInProcess();

            this.WriteLine("\r\nRunning {0} locally\r\n", appFolderStructureSurveyor.AppName);

            hydraCommandHandler.OutputWriteLine = (format, args) =>
            {
                builder.AppendLineFormat(format, args); 

                if (!firstCheck && builder.Contains("Development server running!"))
                {
                    firstCheck = true;
                }

                if (firstCheck && builder.Contains("Browser window opened to "))
                {
                    pictRunLocally.ToComplete();
                }

                this.WriteLine(format, args);
            };

            hydraCommandHandler.ErrorWriteLine = (format, args) =>
            {
                builder.AppendLineFormat(format, args);

                using (this.ErrorMode())
                {
                    this.WriteLine(format, args);
                }
            };

            hydraCommandHandler.Serve(appFolderStructureSurveyor.WebFrontEndRootPath);
        }

        public void InitializeControl(IDesktopForm desktopForm)
        {
            this.desktopForm = desktopForm;
            desktopForm.TextOutput = txtOutput;
        }

        private unsafe void cmdSubmitToHydraStore_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var appStore = AbstraXExtensions.GetAppStore("Hydra");

            appStore.SubmitToStore(hydraAppsAdminServicesClientConfig, appFolderStructureSurveyor, desktopForm);
        }
    }
}
