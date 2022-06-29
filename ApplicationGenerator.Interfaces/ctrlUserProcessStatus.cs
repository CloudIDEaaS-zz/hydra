// file:	ctrlUserProcessStatus.cs
//
// summary:	Implements the control user process status class

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace AbstraX
{
    /// <summary>   A control user status. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/25/2021. </remarks>

    public partial class ctrlUserProcessStatus : UserControl, ILogWriter
    {
        /// <summary>   Gets or sets the process. </summary>
        ///
        /// ### <returns>   The process. </returns>

        private Process process;
        /// <summary>   The steps. </summary>
        private List<string> steps;
        /// <summary>   The platform process. </summary>
        private PlatformProcess platformProcess;
        /// <summary>   The last tick. </summary>
        private int lastTick;
        /// <summary>   The previous memory total. </summary>
        private float previousMemoryTotal = int.MaxValue;
        /// <summary>   The previous CPU total. </summary>
        private float previousCPUTotal = int.MaxValue;
        /// <summary>   The previous network total. </summary>
        private float previousNetworkTotal = int.MaxValue;
        /// <summary>   The cruising animation. </summary>
        private Animation cruisingAnimation;
        /// <summary>   The ignition animation. </summary>
        private Animation ignitionAnimation;
        /// <summary>   Number of cruising frames. </summary>
        private int cruisingFrameCount = 12;
        /// <summary>   Number of ignition frames. </summary>
        private int ignitionFrameCount = 50;
        /// <summary>   The ignition current value. </summary>
        private int ignitionCurrentValue;
        /// <summary>   The hover level. </summary>
        private int hoverLevel = 20;
        /// <summary>   Zero-based index of the sine wave frame. </summary>
        private int sineWaveFrameIndex;
        /// <summary>   Buffer for sine wave data. </summary>
        private short[] sineWaveBuffer;
        /// <summary>   The cruising animation timer. </summary>
        private System.Threading.Timer cruisingAnimationTimer;
        /// <summary>   True if reported main process unloaded. </summary>
        private bool reportedMainProcessUnloaded;
        /// <summary>   The range color normal. </summary>
        private Color rangeColorNormal;
        /// <summary>   The range color warning. </summary>
        private Color rangeColorWarning;
        /// <summary>   The range color danger. </summary>
        private Color rangeColorDanger;
        /// <summary>   The clock lighted background color. </summary>
        private Color clockLightedBackgroundColor;
        /// <summary>   The clock dark color. </summary>
        private Color clockDarkColor;
        /// <summary>   The ignition multiplier. </summary>
        private int ignitionMultiplier;
        /// <summary>   The range color normal active. </summary>
        private Color rangeColorNormalActive;
        /// <summary>   The range color warning active. </summary>
        private Color rangeColorWarningActive;
        /// <summary>   The range color danger active. </summary>
        private Color rangeColorDangerActive;
        /// <summary>   The range inner radius. </summary>
        private int rangeInnerRadius;
        /// <summary>   The range outer radius. </summary>
        private int rangeOuterRadius;
        /// <summary>   The range inner radius active. </summary>
        private int rangeInnerRadiusActive;
        /// <summary>   The range outer radius active. </summary>
        private int rangeOuterRadiusActive;
        private Thread uiThread;

        /// <summary>   The last ignition frame. </summary>
        private int lastIgnitionFrame;
        /// <summary>   The ignition complete action. </summary>
        private Action ignitionCompleteAction;
        /// <summary>   The last gauge ranges. </summary>
        private AGaugeRange[] lastGaugeRanges;
        /// <summary>   True to enable, false to disable the debugger for node. </summary>
        /// <summary>   The stopwatch. </summary>
        private Stopwatch stopwatch;
        /// <summary>   Pathname of the root directory. </summary>
        private string rootDirectory;
        /// <summary>   The size thread. </summary>
        private static Thread sizeThread;
        /// <summary>   The current user proces status. </summary>
        private static ctrlUserProcessStatus currentUserProcesStatus;
        /// <summary>   The lock object. </summary>
        private static IManagedLockObject lockObject;
        /// <summary>   The size thread running reset event. </summary>
        private static ManualResetEvent sizeThreadRunningResetEvent;
        /// <summary>   True to size thread running. </summary>
        private static bool sizeThreadRunning;
        /// <summary>   The process handler. </summary>
        private IProcessHandler processHandler;
        private bool inAnimationFrame;

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/25/2021. </remarks>

        public ctrlUserProcessStatus()
        {
            int sampleRate = 8000;
            short[] buffer = new short[cruisingFrameCount * 2];
            double amplitude = 5;
            double frequency = 1000;

            rangeColorNormal = ColorTranslator.FromHtml("#a6bc9c");
            rangeColorWarning = ColorTranslator.FromHtml("#c7a2a1");
            rangeColorDanger = ColorTranslator.FromHtml("#c37473");
            rangeColorNormalActive = ColorTranslator.FromHtml("#00ff00");
            rangeColorWarningActive = ColorTranslator.FromHtml("#fa5742");
            rangeColorDangerActive = ColorTranslator.FromHtml("#fe3500");

            clockLightedBackgroundColor = ColorTranslator.FromHtml("#3c0000");
            clockDarkColor = ColorTranslator.FromHtml("#250000");

            rangeInnerRadius = 45;
            rangeOuterRadius = 50;
            rangeInnerRadiusActive = 44;
            rangeOuterRadiusActive = 53;

            ignitionMultiplier = (int)(100 / ignitionFrameCount.As<float>() * 2);
            sineWaveBuffer = new short[cruisingFrameCount * 2];

            for (int n = 0; n < buffer.Length; n++)
            {
                sineWaveBuffer[n] = (short)(amplitude * Math.Sin((2 * Math.PI * n * frequency) / sampleRate));
            }

            steps = new List<string>();

            InitializeComponent();

            foreach (var gaugeControl in ctrlDashPanelGauges.Controls.OfType<AGauge>())
            {
                gaugeControl.GaugeRanges.Add(new AGaugeRange(rangeColorNormal, -100, 100, rangeInnerRadius, rangeOuterRadius));
                gaugeControl.GaugeRanges.Add(new AGaugeRange(rangeColorWarning, 100, 300, rangeInnerRadius, rangeOuterRadius));
                gaugeControl.GaugeRanges.Add(new AGaugeRange(rangeColorDanger, 300, 400, rangeInnerRadius, rangeOuterRadius));
            }

            lastGaugeRanges = new AGaugeRange[3];
        }

        protected override void CreateHandle()
        {
            base.CreateHandle();

            uiThread = this.GetUIThread();
        }

        /// <summary>   Static constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/6/2021. </remarks>

        static ctrlUserProcessStatus()
        {
            lockObject = LockManager.CreateObject();
            sizeThreadRunningResetEvent = new ManualResetEvent(false);
        }

        /// <summary>   Gets or sets the label file size value. </summary>
        ///
        /// <value> The label file size value. </value>

        public string LabelFileSizeValue 
        {
            get
            {
                return lblSize.Text;
            }

            set
            {
                lblSize.Text = value;
            }
        }

        /// <summary>   Gets or sets the progress bar file size value. </summary>
        ///
        /// <value> The progress bar file size value. </value>

        public int ProgressBarFileSizeValue
        {
            get
            {
                return progBarSize.Value;
            }

            set
            {
                progBarSize.Value = value;
            }
        }

        /// <summary>   Gets or sets the stopwatch. </summary>
        ///
        /// <value> The stopwatch. </value>

        public Stopwatch Stopwatch
        {
            get
            {
                return stopwatch;
            }

            set
            {
                stopwatch = value;

                if (stopwatch == null)
                {
                    return;
                }
                else if (stopwatch.IsRunning)
                {
                    Run(false);
                }
            }
        }

        /// <summary>   Gets or sets the pathname of the root directory. </summary>
        ///
        /// <value> The pathname of the root directory. </value>

        public string RootDirectory
        {
            get
            {
                return rootDirectory;
            }

            set
            {
                rootDirectory = value;

                if (rootDirectory != null)
                {
                    using (lockObject.Lock())
                    {
                        currentUserProcesStatus = this;

                        if (sizeThread == null)
                        {
                            this.StartSizeThread();
                        }
                    }
                }
            }
        }

        /// <summary>   Gets or sets the process handler. </summary>
        ///
        /// <value> The process handler. </value>

        public IProcessHandler ProcessHandler
        {
            get
            {
                return processHandler;
            }

            set
            {
                processHandler = value;
            }
        }

        /// <summary>   Starts size thread. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/6/2021. </remarks>

        private void StartSizeThread()
        {
            lockObject = LockManager.CreateObject();

            sizeThread = new Thread(() => SizeThreadMethod(rootDirectory));

            sizeThread.Priority = ThreadPriority.Lowest;
            sizeThread.IsBackground = true;
            sizeThreadRunning = true;

            sizeThread.Start();
        }

        /// <summary>   Stops the timers. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/7/2021. </remarks>

        public void StopTimers()
        {
            timerProcess.Stop();
            timerClock.Stop();

            if (cruisingAnimationTimer != null)
            {
                StopCruisingAnimation();
            }
        }

        /// <summary>   Stops size thread. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/4/2021. </remarks>

        public static void StopSizeThread()
        {
            using (lockObject.Lock())
            {
                sizeThreadRunning = false;
            }

            if (!sizeThreadRunningResetEvent.WaitOne())
            {
            }
        }

        /// <summary>   Size thread method. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/6/2021. </remarks>
        ///
        /// <param name="rootDirectory">    The pathname of the root directory. </param>

        private static void SizeThreadMethod(string rootDirectory)
        {
            var running = false;
            var directory = new DirectoryInfo(rootDirectory);
            ctrlUserProcessStatus currentStatus;
            long lastSize = 0;

            using (lockObject.Lock())
            {
                running = sizeThreadRunning;
                currentStatus = currentUserProcesStatus;
            }

            while (running)
            {
                long size = 0;

                try
                {
                    size = directory.GetFiles("*.*", SearchOption.AllDirectories).Sum(f => f.Length);
                }
                catch
                {
                }

                if (size != lastSize)
                {
                    if (!currentStatus.IsDisposed)
                    {
                        currentStatus.Invoke(() =>
                        {
                            currentStatus.ReportSize(size);
                        });
                    }

                    lastSize = size;
                }

                using (lockObject.Lock())
                {
                    running = sizeThreadRunning;
                    currentStatus = currentUserProcesStatus;
                }

                Thread.Sleep(1000);
            }

            using (lockObject.Lock())
            {
                sizeThreadRunningResetEvent.Set();
            }
        }

        /// <summary>   Reports a size. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/6/2021. </remarks>
        ///
        /// <param name="size"> The size. </param>

        private void ReportSize(long size)
        {
            long gb2 = (long)(NumberExtensions.GB * 2L);

            if (size > NumberExtensions.GB)
            {
                lblSize.Text = ((int)(size / NumberExtensions.GB)).ToString() + "GB";
            }
            else if (size > NumberExtensions.MB)
            {
                lblSize.Text = ((int)(size / NumberExtensions.MB)).ToString() + "MB";
            }
            else
            {
                lblSize.Text = ((int)(size / NumberExtensions.KB)).ToString() + "KB";
            }

            progBarSize.Value = (int)((size.As<double>() / gb2.As<double>()) * 100);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.VisibleChanged" /> event when the
        /// <see cref="P:System.Windows.Forms.Control.Visible" /> property value of the control's
        /// container changes.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/26/2021. </remarks>
        ///
        /// <param name="e">    An <see cref="T:System.EventArgs" /> that contains the event data. </param>

        protected override void OnParentVisibleChanged(EventArgs e)
        {
            cruisingAnimation = new Utils.Animation(100, true);
            ignitionAnimation = new Utils.Animation(30);

            cruisingAnimation.OnFrame += CruisingAnimation_OnFrame;
            ignitionAnimation.OnFrame += IgnitionAnimation_OnFrame;

            base.OnParentVisibleChanged(e);
        }

        /// <summary>   Gets or sets the process. </summary>
        ///
        /// <value> The process. </value>

        public Process Process 
        {
            get
            {
                return process;
            }

            set
            {
                ctrlTaskIcon ctrlTaskIcon;
                var process = value;

                if (process != null)
                {
                    platformProcess = ProcessExtensions.GetAllProcesses().OrderBy(p => p.ProcessName).Where(p => !p.Path.IsNullOrEmpty() && p.Process.Id == process.Id).SingleOrDefault();

                    if (platformProcess != null)
                    {
                        ctrlTaskIcon = new ctrlTaskIcon(platformProcess);

                        panelProcesses.Controls.Add(ctrlTaskIcon);

                        this.process = process;

                        txtStatus.AppendLine("{0} loaded", platformProcess.ProcessName);
                        txtStatus.ScrollToEnd();

                        this.DoEvents();
                    }
                }

                if (process != null)
                {
                    timerProcess.Start();
                }
                else
                {
                    timerProcess.Stop();
                }
            }
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
            if (currentStep == "Final Lap")
            {
            }

            if (!steps.Contains(currentStep))
            {
                txtStatus.AppendLine("Entering step {0} {1}", currentStep, "*".Repeat(25));
                steps.Add(currentStep);
            }

            if (!status.IsNullOrEmpty())
            {
                txtStatus.AppendLine(status);
            }

            txtStatus.ScrollToEnd();

            this.DoEvents();
        }

        /// <summary>   Starts an ignition. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/3/2021. </remarks>
        ///
        /// <param name="action">   The action. </param>

        public void StartIgnition(Action action)
        {
            ignitionCompleteAction = action;

            this.StartIgnitionAnimation();
        }

        /// <summary>   Runs. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/6/2021. </remarks>
        ///
        /// <param name="gradualOn">    True to enable, false to disable the gradual. </param>

        private void Run(bool gradualOn)
        {
            digitalClock.ColorDark = clockDarkColor;
            digitalClock.ColorBackground = clockLightedBackgroundColor;
            digitalClock.Value = "00000";
            digitalClock.ColonShow = true;

            ctrlDashPanelClock.Lighted = true;

            if (gradualOn)
            {
                digitalClock.LightUpGradually(Color.Red, 100);
            }
            else
            {
                digitalClock.ColorLight = Color.Red;
            }
        }

        /// <summary>   Event handler. Called by timerProcess for tick events. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/6/2021. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>

        private void timerProcess_Tick(object sender, EventArgs e)
        {
            float newMemoryTotal;
            float newCPUTotal;
            float newNetworkTotal;
            int tick;
            int time;
            float rateOfChangeMemory;
            float rateOfChangeCPU;
            float rateOfChangeNetwork;
            float combinedRate;
            int level;

            if (platformProcess == null || platformProcess.Process.HasExited)
            {
                if (reportedMainProcessUnloaded)
                {
                    timerProcess.Enabled = false;

                    return;
                }
                else if (platformProcess != null && !reportedMainProcessUnloaded)
                {
                    this.gaugeControlMemory.Value = -100;
                    this.gaugeControlCPU.Value = -100;
                    this.gaugeControlNetwork.Value = -100;

                    ResetRanges();

                    txtStatus.AppendLine("{0} unloaded", platformProcess.ProcessName);
                    txtStatus.ScrollToEnd();

                    this.DoEvents();

                    panelProcesses.Controls.Clear();

                    reportedMainProcessUnloaded = true;

                    pictureBoxWait.Visible = false;

                    this.DoEvents();

                    return;
                }
            }
            else
            {
                var processes = platformProcess.DescendantProcesses.ToList();

                if (!pictureBoxWait.Visible)
                {
                    pictureBoxWait.Visible = true;
                }

                this.processHandler.ReportProcess(platformProcess.Process, this);

                foreach (var descendantProcess in processes)
                {
                    if (!panelProcesses.Controls.OfType<ctrlTaskIcon>().Any(t => t.ProcessName.AsCaseless() == descendantProcess.ProcessName))
                    {
                        var process = processes.FirstOrDefault(p => p.ProcessName.AsCaseless() == descendantProcess.ProcessName);

                        if (process != null)
                        {
                            var ctrlTaskIcon = new ctrlTaskIcon(process);

                            this.processHandler.ReportProcess(platformProcess.Process, this);

                            panelProcesses.Controls.Add(ctrlTaskIcon);

                            txtStatus.AppendLine("{0} loaded", process.ProcessName);
                            txtStatus.ScrollToEnd();

                            this.DoEvents();
                        }
                        else
                        {
                            var ctrlTaskIcon = panelProcesses.Controls.OfType<ctrlTaskIcon>().Single(t => t.ProcessName.AsCaseless() == descendantProcess.ProcessName);

                            ctrlTaskIcon.UpdateList();
                        }
                    }

                    if (panelProcesses.Controls.OfType<ctrlTaskIcon>().Any(t => !processes.Any(p => p.ProcessName.AsCaseless() == t.ProcessName)))
                    {
                        foreach (var ctrlTaskIcon in panelProcesses.Controls.OfType<ctrlTaskIcon>().Where(t => !processes.Any(p => p.ProcessName.AsCaseless() == t.ProcessName) && t.ProcessName != platformProcess.ProcessName))
                        {
                            panelProcesses.Controls.Remove(ctrlTaskIcon);
                            txtStatus.AppendLine("{0} unloaded", ctrlTaskIcon.ProcessName);
                            txtStatus.ScrollToEnd();

                            this.DoEvents();
                        }
                    }
                }

                processes.Add(platformProcess);

                newMemoryTotal = Math.Min(processes.Sum(p => p.MemoryUsage) * 4, 400);
                newCPUTotal = Math.Min(processes.Sum(p => p.CpuUsage), 400);
                newNetworkTotal = Math.Min(processes.Sum(p => p.NetworkUtilization) * 8, 400);

                if (newMemoryTotal == 0)
                {
                    try
                    {
                        var process = platformProcess.Process;

                        if (process.WorkingSet64 > 0)
                        {
                            newMemoryTotal = (process.WorkingSet64 / process.VirtualMemorySize64) * 400;
                        }
                    }
                    catch
                    {
                    }
                }

                tick = Environment.TickCount;
                time = tick - lastTick;

                rateOfChangeMemory = (previousMemoryTotal - newMemoryTotal) / time;
                rateOfChangeCPU = (previousCPUTotal - newCPUTotal) / time;
                rateOfChangeNetwork = (previousNetworkTotal - newNetworkTotal) / time;

                this.gaugeControlMemory.Value = newMemoryTotal;
                this.gaugeControlCPU.Value = newCPUTotal;
                this.gaugeControlNetwork.Value = newNetworkTotal;

                SetActiveRanges();

                combinedRate = Math.Max(rateOfChangeMemory, Math.Max(rateOfChangeCPU, newNetworkTotal));

                level = (int)(combinedRate * 10000);

                if (level < hoverLevel && (newMemoryTotal > 20 || newCPUTotal > 20))
                {
                    if (!cruisingAnimation.Started)
                    {
                        StartCruisingAnimation();
                    }

                    level = hoverLevel;
                }
                else if (cruisingAnimation.Started)
                {
                    StopCruisingAnimation();
                }

                vuMeter.Level = level;

                previousMemoryTotal = newMemoryTotal;
                previousCPUTotal = newCPUTotal;
                previousNetworkTotal = newNetworkTotal;

                lastTick = tick;
            }
        }

        /// <summary>   Resets the ranges. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/6/2021. </remarks>

        private void ResetRanges()
        {
            foreach (var gaugeControl in ctrlDashPanelGauges.Controls.OfType<AGauge>())
            {
                foreach (var range in gaugeControl.GaugeRanges.Cast<AGaugeRange>())
                {
                    range.InnerRadius = rangeInnerRadius;
                    range.OuterRadius = rangeOuterRadius;

                    switch (range.StartValue)
                    {
                        case -100:
                            range.Color = rangeColorNormal;
                            break;
                        case 100:
                            range.Color = rangeColorWarning;
                            break;
                        case 300:
                            range.Color = rangeColorDanger;
                            break;
                    }
                }
            }
        }

        /// <summary>   Sets active ranges. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/6/2021. </remarks>

        private void SetActiveRanges()
        {
            var x = 0;

            foreach (var gaugeControl in ctrlDashPanelGauges.Controls.OfType<AGauge>())
            {
                var activeRange = gaugeControl.GaugeRanges.Cast<AGaugeRange>().First(r => gaugeControl.Value.IsBetween(r.StartValue, r.EndValue, true));
                var lastActiveRange = lastGaugeRanges[x];

                if (activeRange != lastActiveRange)
                {
                    if (lastActiveRange != null)
                    {
                        lastActiveRange.InnerRadius = rangeInnerRadius;
                        lastActiveRange.OuterRadius = rangeOuterRadius;

                        switch (lastActiveRange.StartValue)
                        {
                            case -100:
                                lastActiveRange.Color = rangeColorNormal;
                                break;
                            case 100:
                                lastActiveRange.Color = rangeColorWarning;
                                break;
                            case 300:
                                lastActiveRange.Color = rangeColorDanger;
                                break;
                        }
                    }

                    activeRange.InnerRadius = rangeInnerRadiusActive;
                    activeRange.OuterRadius = rangeOuterRadiusActive;

                    switch (activeRange.StartValue)
                    {
                        case -100:
                            activeRange.Color = rangeColorNormalActive;
                            break;
                        case 100:
                            activeRange.Color = rangeColorWarningActive;
                            break;
                        case 300:
                            activeRange.Color = rangeColorDangerActive;
                            break;
                    }

                    lastGaugeRanges[x] = activeRange;
                    gaugeControl.Refresh();
                }

                x++;
            }
        }

        /// <summary>   Stops cruising animation. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/6/2021. </remarks>

        private void StopCruisingAnimation()
        {
            cruisingAnimationTimer.Dispose();
            cruisingAnimationTimer = null;

            cruisingAnimation.Stop();
        }

        /// <summary>   Starts cruising animation. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/6/2021. </remarks>

        private void StartCruisingAnimation()
        {
            cruisingAnimation.Start(cruisingFrameCount);

            cruisingAnimationTimer = new System.Threading.Timer((o) =>
            {
                cruisingAnimation.Tick();

            }, null, 0, 500);
        }

        /// <summary>   Cruising animation frame. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/6/2021. </remarks>
        ///
        /// <param name="frame">    The frame. </param>

        private void CruisingAnimation_OnFrame(int frame)
        {
            this.Invoke(() =>
            {
                try
                {
                    vuMeter.Level = 18 + sineWaveBuffer[sineWaveFrameIndex];
                }
                catch
                {
                }
            });

            sineWaveFrameIndex = (++sineWaveFrameIndex).ScopeRange((cruisingFrameCount * 2) - 1);
        }

        /// <summary>   Stops ignition animation. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/6/2021. </remarks>

        private void StopIgnitionAnimation()
        {
            ignitionAnimation.Stop();
            this.Run(true);

            if (ignitionCompleteAction != null)
            {
                this.DelayInvoke(100, () =>
                {
                    ignitionCompleteAction();

                    ignitionCompleteAction = null;
                });
            }
        }

        /// <summary>   Starts ignition animation. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/6/2021. </remarks>

        private void StartIgnitionAnimation()
        {
            foreach (var gaugeControl in ctrlDashPanelGauges.Controls.OfType<AGauge>())
            {
                gaugeControl.Value = -100;
            }

            ignitionCurrentValue = ignitionFrameCount;
            lastIgnitionFrame = 0;

            ignitionAnimation.Start(ignitionFrameCount);
        }

        /// <summary>   Ignition animation frame. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/6/2021. </remarks>
        ///
        /// <param name="frame">    The frame. </param>

        private void IgnitionAnimation_OnFrame(int frame)
        {
            bool revvingUp;
            var thread = Thread.CurrentThread;

            if (inAnimationFrame)
            {
                return;
            }

            inAnimationFrame = true;

            if (thread.ManagedThreadId != uiThread.ManagedThreadId)
            {
                DebugUtils.Break();
            }

            if (frame < lastIgnitionFrame)
            {
                this.StopIgnitionAnimation();

                foreach (var gaugeControl in ctrlDashPanelGauges.Controls.OfType<AGauge>())
                {
                    gaugeControl.Value = 0;
                }

                vuMeter.Level = 0;

                return;
            }

            lastIgnitionFrame = frame;

            if (frame > ignitionFrameCount / 2)
            {
                ignitionCurrentValue = ignitionFrameCount - frame;
                revvingUp = false;
            }
            else
            {
                ignitionCurrentValue = frame;
                revvingUp = true;
            }

            try
            {
                if (!revvingUp)
                {
                    var value = -100 + (ignitionMultiplier * (frame - (ignitionFrameCount / 2)));

                    foreach (var gaugeControl in ctrlDashPanelGauges.Controls.OfType<AGauge>())
                    {
                        gaugeControl.Value = value;
                    }
                }

                vuMeter.Level = (int)(ignitionCurrentValue * 2.5);
            }
            catch
            {
            }

            inAnimationFrame = false;
        }

        /// <summary>   Event handler. Called by vuMeter for click events. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/6/2021. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>

        private void vuMeter_Click(object sender, EventArgs e)
        {
            if (ignitionAnimation.Started)
            {
                StopIgnitionAnimation();

                vuMeter.Level = 0;
                vuMeter.Refresh();
            }
            else
            {
                StartIgnitionAnimation();
            }
        }

        /// <summary>   Event handler. Called by vuMeter for double click events. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/6/2021. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>

        private void vuMeter_DoubleClick(object sender, EventArgs e)
        {
            if (cruisingAnimation.Started)
            {
                StopCruisingAnimation();

                vuMeter.Level = 0;
                vuMeter.Refresh();
            }
            else
            {
                StartCruisingAnimation();
            }
        }

        /// <summary>   Event handler. Called by timerClock for tick events. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/6/2021. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>

        private void timerClock_Tick(object sender, EventArgs e)
        {
            if (stopwatch != null && stopwatch.IsRunning)
            {
                var timeSpan = new TimeSpan(stopwatch.ElapsedTicks);
                var value = timeSpan.ToString("hmmss");

                digitalClock.Value = value;
            }
        }

        /// <summary>   Writes. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/6/2021. </remarks>
        ///
        /// <param name="value">    The value. </param>

        public void Write(string value)
        {
            this.Invoke(() =>
            {
                txtStatus.AppendText(value);
                txtStatus.ScrollToEnd();

                this.DoEvents();
            });
        }

        /// <summary>   Writes. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/6/2021. </remarks>
        ///
        /// <param name="format">   Describes the format to use. </param>
        /// <param name="args">     A variable-length parameters list containing arguments. </param>

        public void Write(string format, params object[] args)
        {
            this.Invoke(() =>
            {
                txtStatus.AppendLine(string.Format(format, args));
                txtStatus.ScrollToEnd();

                this.DoEvents();
            });
        }

        /// <summary>   Writes a line. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/6/2021. </remarks>
        ///
        /// <param name="value">    The value. </param>

        public void WriteLine(string value)
        {
            this.Invoke(() =>
            {
                txtStatus.AppendLine(value);
                txtStatus.ScrollToEnd();

                this.DoEvents();
            });
        }

        /// <summary>   Writes a line. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/6/2021. </remarks>

        public void WriteLine()
        {
            this.Invoke(() =>
            {
                txtStatus.AppendLine(string.Empty);
                txtStatus.ScrollToEnd();

                this.DoEvents();
            });
        }

        /// <summary>   Writes a line. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/6/2021. </remarks>
        ///
        /// <param name="format">   Describes the format to use. </param>
        /// <param name="args">     A variable-length parameters list containing arguments. </param>

        public void WriteLine(string format, params object[] args)
        {
            this.Invoke(() =>
            {
                txtStatus.AppendLine(string.Format(format, args));
                txtStatus.ScrollToEnd();

                this.DoEvents();
            });
        }

        /// <summary>   Error mode. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/10/2021. </remarks>
        ///
        /// <returns>   An IDisposable. </returns>

        public IDisposable ErrorMode()
        {
            throw new NotImplementedException();
        }
    }
}
