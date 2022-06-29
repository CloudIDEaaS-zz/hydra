
namespace AbstraX
{
    partial class ctrlUserProcessStatus
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            ProgBar.cBlendItems cBlendItems1 = new ProgBar.cBlendItems();
            ProgBar.cFocalPoints cFocalPoints1 = new ProgBar.cFocalPoints();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ctrlUserProcessStatus));
            this.label1 = new System.Windows.Forms.Label();
            this.panelProcessStatus = new System.Windows.Forms.Panel();
            this.lblSize = new System.Windows.Forms.Label();
            this.pictureBoxWait = new System.Windows.Forms.PictureBox();
            this.panelProcesses = new System.Windows.Forms.FlowLayoutPanel();
            this.txtStatus = new System.Windows.Forms.RichTextBox();
            this.timerProcess = new System.Windows.Forms.Timer(this.components);
            this.timerClock = new System.Windows.Forms.Timer(this.components);
            this.ctrlDashPanel1 = new AbstraX.ctrlDashPanel();
            this.progBarSize = new ProgBar.ProgBarPlus();
            this.ctrlDashPanel2 = new AbstraX.ctrlDashPanel();
            this.vuMeter = new VU_MeterLibrary.VuMeter();
            this.ctrlDashPanelGauges = new AbstraX.ctrlDashPanel();
            this.gaugeControlMemory = new System.Windows.Forms.AGauge();
            this.label2 = new System.Windows.Forms.Label();
            this.gaugeControlCPU = new System.Windows.Forms.AGauge();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.gaugeControlNetwork = new System.Windows.Forms.AGauge();
            this.ctrlDashPanelClock = new AbstraX.ctrlDashPanel();
            this.digitalClock = new Utils.SevenSegmentClock();
            this.panelProcessStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWait)).BeginInit();
            this.ctrlDashPanel1.SuspendLayout();
            this.ctrlDashPanel2.SuspendLayout();
            this.ctrlDashPanelGauges.SuspendLayout();
            this.ctrlDashPanelClock.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(998, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Process Status";
            // 
            // panelProcessStatus
            // 
            this.panelProcessStatus.BackColor = System.Drawing.Color.Gray;
            this.panelProcessStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelProcessStatus.Controls.Add(this.lblSize);
            this.panelProcessStatus.Controls.Add(this.ctrlDashPanel1);
            this.panelProcessStatus.Controls.Add(this.ctrlDashPanel2);
            this.panelProcessStatus.Controls.Add(this.ctrlDashPanelGauges);
            this.panelProcessStatus.Controls.Add(this.ctrlDashPanelClock);
            this.panelProcessStatus.Controls.Add(this.pictureBoxWait);
            this.panelProcessStatus.Controls.Add(this.panelProcesses);
            this.panelProcessStatus.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelProcessStatus.Location = new System.Drawing.Point(775, 13);
            this.panelProcessStatus.Name = "panelProcessStatus";
            this.panelProcessStatus.Size = new System.Drawing.Size(223, 783);
            this.panelProcessStatus.TabIndex = 5;
            // 
            // lblSize
            // 
            this.lblSize.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblSize.Location = new System.Drawing.Point(0, 592);
            this.lblSize.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblSize.Name = "lblSize";
            this.lblSize.Size = new System.Drawing.Size(43, 17);
            this.lblSize.TabIndex = 9;
            this.lblSize.Text = "0 KB";
            this.lblSize.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBoxWait
            // 
            this.pictureBoxWait.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxWait.Image")));
            this.pictureBoxWait.Location = new System.Drawing.Point(98, 653);
            this.pictureBoxWait.Name = "pictureBoxWait";
            this.pictureBoxWait.Size = new System.Drawing.Size(32, 32);
            this.pictureBoxWait.TabIndex = 4;
            this.pictureBoxWait.TabStop = false;
            this.pictureBoxWait.Visible = false;
            // 
            // panelProcesses
            // 
            this.panelProcesses.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panelProcesses.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelProcesses.Location = new System.Drawing.Point(3, 3);
            this.panelProcesses.Name = "panelProcesses";
            this.panelProcesses.Size = new System.Drawing.Size(216, 45);
            this.panelProcesses.TabIndex = 1;
            // 
            // txtStatus
            // 
            this.txtStatus.BackColor = System.Drawing.Color.White;
            this.txtStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtStatus.Location = new System.Drawing.Point(0, 13);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.Size = new System.Drawing.Size(775, 783);
            this.txtStatus.TabIndex = 6;
            this.txtStatus.Text = "";
            // 
            // timerProcess
            // 
            this.timerProcess.Interval = 2000;
            this.timerProcess.Tick += new System.EventHandler(this.timerProcess_Tick);
            // 
            // timerClock
            // 
            this.timerClock.Enabled = true;
            this.timerClock.Interval = 1000;
            this.timerClock.Tick += new System.EventHandler(this.timerClock_Tick);
            // 
            // ctrlDashPanel1
            // 
            this.ctrlDashPanel1.BackColor = System.Drawing.Color.DimGray;
            this.ctrlDashPanel1.Controls.Add(this.progBarSize);
            this.ctrlDashPanel1.Lighted = false;
            this.ctrlDashPanel1.LitBackgroundColor = System.Drawing.Color.Empty;
            this.ctrlDashPanel1.Location = new System.Drawing.Point(7, 265);
            this.ctrlDashPanel1.Name = "ctrlDashPanel1";
            this.ctrlDashPanel1.Radius = 10;
            this.ctrlDashPanel1.Size = new System.Drawing.Size(28, 324);
            this.ctrlDashPanel1.TabIndex = 8;
            // 
            // progBarSize
            // 
            this.progBarSize.BarBackColor = System.Drawing.Color.Gray;
            cBlendItems1.iColor = new System.Drawing.Color[] {
        System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(196)))), ((int)(((byte)(222))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(130)))), ((int)(((byte)(180)))))};
            cBlendItems1.iPoint = new float[] {
        0F,
        1F};
            this.progBarSize.BarColorBlend = cBlendItems1;
            this.progBarSize.BarColorSolid = System.Drawing.Color.SteelBlue;
            this.progBarSize.BarColorSolidB = System.Drawing.Color.White;
            this.progBarSize.BarLengthValue = ((short)(100));
            this.progBarSize.BarPadding = new System.Windows.Forms.Padding(0);
            this.progBarSize.BarStyleFill = ProgBar.ProgBarPlus.eBarStyle.GradientLinear;
            this.progBarSize.BarStyleLinear = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.progBarSize.BarStyleTexture = null;
            this.progBarSize.BorderWidth = ((short)(1));
            this.progBarSize.Corners.All = ((short)(0));
            this.progBarSize.Corners.LowerLeft = ((short)(0));
            this.progBarSize.Corners.LowerRight = ((short)(0));
            this.progBarSize.Corners.UpperLeft = ((short)(0));
            this.progBarSize.Corners.UpperRight = ((short)(0));
            this.progBarSize.CylonInterval = ((short)(1));
            this.progBarSize.CylonMove = 5F;
            cFocalPoints1.CenterPoint = ((System.Drawing.PointF)(resources.GetObject("cFocalPoints1.CenterPoint")));
            cFocalPoints1.FocusScales = ((System.Drawing.PointF)(resources.GetObject("cFocalPoints1.FocusScales")));
            this.progBarSize.FocalPoints = cFocalPoints1;
            this.progBarSize.Location = new System.Drawing.Point(8, 5);
            this.progBarSize.Name = "progBarSize";
            this.progBarSize.Orientation = ProgBar.ProgBarPlus.eOrientation.Vertical;
            this.progBarSize.ShapeTextFont = new System.Drawing.Font("Arial Black", 30F);
            this.progBarSize.Size = new System.Drawing.Size(12, 310);
            this.progBarSize.TabIndex = 8;
            this.progBarSize.TextFormat = "Process {1}% Done";
            this.progBarSize.Value = 0;
            // 
            // ctrlDashPanel2
            // 
            this.ctrlDashPanel2.BackColor = System.Drawing.Color.DimGray;
            this.ctrlDashPanel2.Controls.Add(this.vuMeter);
            this.ctrlDashPanel2.Lighted = false;
            this.ctrlDashPanel2.LitBackgroundColor = System.Drawing.Color.Empty;
            this.ctrlDashPanel2.Location = new System.Drawing.Point(7, 55);
            this.ctrlDashPanel2.Name = "ctrlDashPanel2";
            this.ctrlDashPanel2.Radius = 10;
            this.ctrlDashPanel2.Size = new System.Drawing.Size(28, 204);
            this.ctrlDashPanel2.TabIndex = 7;
            // 
            // vuMeter
            // 
            this.vuMeter.AnalogMeter = false;
            this.vuMeter.DialBackground = System.Drawing.Color.White;
            this.vuMeter.DialTextNegative = System.Drawing.Color.Red;
            this.vuMeter.DialTextPositive = System.Drawing.Color.Black;
            this.vuMeter.DialTextZero = System.Drawing.Color.DarkGreen;
            this.vuMeter.Led1ColorOff = System.Drawing.Color.DarkGreen;
            this.vuMeter.Led1ColorOn = System.Drawing.Color.LimeGreen;
            this.vuMeter.Led1Count = 6;
            this.vuMeter.Led2ColorOff = System.Drawing.Color.Olive;
            this.vuMeter.Led2ColorOn = System.Drawing.Color.Yellow;
            this.vuMeter.Led2Count = 6;
            this.vuMeter.Led3ColorOff = System.Drawing.Color.Maroon;
            this.vuMeter.Led3ColorOn = System.Drawing.Color.Red;
            this.vuMeter.Led3Count = 4;
            this.vuMeter.LedSize = new System.Drawing.Size(15, 8);
            this.vuMeter.LedSpace = 3;
            this.vuMeter.Level = 0;
            this.vuMeter.LevelMax = 100;
            this.vuMeter.Location = new System.Drawing.Point(3, 14);
            this.vuMeter.MeterScale = VU_MeterLibrary.MeterScale.Log10;
            this.vuMeter.Name = "vuMeter";
            this.vuMeter.NeedleColor = System.Drawing.Color.Black;
            this.vuMeter.PeakHold = false;
            this.vuMeter.Peakms = 1000;
            this.vuMeter.PeakNeedleColor = System.Drawing.Color.Red;
            this.vuMeter.ShowDialOnly = false;
            this.vuMeter.ShowLedPeak = false;
            this.vuMeter.ShowTextInDial = false;
            this.vuMeter.Size = new System.Drawing.Size(21, 179);
            this.vuMeter.TabIndex = 3;
            this.vuMeter.TextInDial = new string[] {
        "-40",
        "-20",
        "-10",
        "-5",
        "0",
        "+6"};
            this.vuMeter.UseLedLight = true;
            this.vuMeter.VerticalBar = true;
            this.vuMeter.VuText = "VU";
            this.vuMeter.Click += new System.EventHandler(this.vuMeter_Click);
            this.vuMeter.DoubleClick += new System.EventHandler(this.vuMeter_DoubleClick);
            // 
            // ctrlDashPanelGauges
            // 
            this.ctrlDashPanelGauges.BackColor = System.Drawing.Color.DimGray;
            this.ctrlDashPanelGauges.Controls.Add(this.gaugeControlMemory);
            this.ctrlDashPanelGauges.Controls.Add(this.label2);
            this.ctrlDashPanelGauges.Controls.Add(this.gaugeControlCPU);
            this.ctrlDashPanelGauges.Controls.Add(this.label4);
            this.ctrlDashPanelGauges.Controls.Add(this.label3);
            this.ctrlDashPanelGauges.Controls.Add(this.gaugeControlNetwork);
            this.ctrlDashPanelGauges.Lighted = false;
            this.ctrlDashPanelGauges.LitBackgroundColor = System.Drawing.Color.Empty;
            this.ctrlDashPanelGauges.Location = new System.Drawing.Point(42, 55);
            this.ctrlDashPanelGauges.Name = "ctrlDashPanelGauges";
            this.ctrlDashPanelGauges.Radius = 10;
            this.ctrlDashPanelGauges.Size = new System.Drawing.Size(171, 534);
            this.ctrlDashPanelGauges.TabIndex = 7;
            // 
            // gaugeControlMemory
            // 
            this.gaugeControlMemory.BaseArcColor = System.Drawing.Color.Transparent;
            this.gaugeControlMemory.BaseArcRadius = 50;
            this.gaugeControlMemory.BaseArcStart = 135;
            this.gaugeControlMemory.BaseArcSweep = 270;
            this.gaugeControlMemory.BaseArcWidth = 2;
            this.gaugeControlMemory.Center = new System.Drawing.Point(80, 80);
            this.gaugeControlMemory.Location = new System.Drawing.Point(1, 24);
            this.gaugeControlMemory.MaxValue = 400F;
            this.gaugeControlMemory.MinValue = -100F;
            this.gaugeControlMemory.Name = "gaugeControlMemory";
            this.gaugeControlMemory.NeedleColor1 = System.Windows.Forms.AGaugeNeedleColor.Gray;
            this.gaugeControlMemory.NeedleColor2 = System.Drawing.Color.SaddleBrown;
            this.gaugeControlMemory.NeedleRadius = 50;
            this.gaugeControlMemory.NeedleType = System.Windows.Forms.NeedleType.Advance;
            this.gaugeControlMemory.NeedleWidth = 2;
            this.gaugeControlMemory.ScaleLinesInterColor = System.Drawing.Color.Silver;
            this.gaugeControlMemory.ScaleLinesInterInnerRadius = 58;
            this.gaugeControlMemory.ScaleLinesInterOuterRadius = 43;
            this.gaugeControlMemory.ScaleLinesInterWidth = 1;
            this.gaugeControlMemory.ScaleLinesMajorColor = System.Drawing.Color.Silver;
            this.gaugeControlMemory.ScaleLinesMajorInnerRadius = 58;
            this.gaugeControlMemory.ScaleLinesMajorOuterRadius = 48;
            this.gaugeControlMemory.ScaleLinesMajorStepValue = 50F;
            this.gaugeControlMemory.ScaleLinesMajorWidth = 2;
            this.gaugeControlMemory.ScaleLinesMinorColor = System.Drawing.Color.Silver;
            this.gaugeControlMemory.ScaleLinesMinorInnerRadius = 50;
            this.gaugeControlMemory.ScaleLinesMinorOuterRadius = 55;
            this.gaugeControlMemory.ScaleLinesMinorTicks = 9;
            this.gaugeControlMemory.ScaleLinesMinorWidth = 1;
            this.gaugeControlMemory.ScaleNumbersColor = System.Drawing.Color.Gainsboro;
            this.gaugeControlMemory.ScaleNumbersFormat = null;
            this.gaugeControlMemory.ScaleNumbersRadius = 70;
            this.gaugeControlMemory.ScaleNumbersRotation = 0;
            this.gaugeControlMemory.ScaleNumbersStartScaleLine = 0;
            this.gaugeControlMemory.ScaleNumbersStepScaleLines = 1;
            this.gaugeControlMemory.Size = new System.Drawing.Size(167, 145);
            this.gaugeControlMemory.TabIndex = 0;
            this.gaugeControlMemory.Text = "gauge";
            this.gaugeControlMemory.Value = -100F;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label2.Location = new System.Drawing.Point(52, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "Activity";
            // 
            // gaugeControlCPU
            // 
            this.gaugeControlCPU.BaseArcColor = System.Drawing.Color.Transparent;
            this.gaugeControlCPU.BaseArcRadius = 50;
            this.gaugeControlCPU.BaseArcStart = 135;
            this.gaugeControlCPU.BaseArcSweep = 270;
            this.gaugeControlCPU.BaseArcWidth = 2;
            this.gaugeControlCPU.Center = new System.Drawing.Point(80, 80);
            this.gaugeControlCPU.Location = new System.Drawing.Point(1, 192);
            this.gaugeControlCPU.MaxValue = 400F;
            this.gaugeControlCPU.MinValue = -100F;
            this.gaugeControlCPU.Name = "gaugeControlCPU";
            this.gaugeControlCPU.NeedleColor1 = System.Windows.Forms.AGaugeNeedleColor.Gray;
            this.gaugeControlCPU.NeedleColor2 = System.Drawing.Color.SaddleBrown;
            this.gaugeControlCPU.NeedleRadius = 50;
            this.gaugeControlCPU.NeedleType = System.Windows.Forms.NeedleType.Advance;
            this.gaugeControlCPU.NeedleWidth = 2;
            this.gaugeControlCPU.ScaleLinesInterColor = System.Drawing.Color.Silver;
            this.gaugeControlCPU.ScaleLinesInterInnerRadius = 58;
            this.gaugeControlCPU.ScaleLinesInterOuterRadius = 43;
            this.gaugeControlCPU.ScaleLinesInterWidth = 1;
            this.gaugeControlCPU.ScaleLinesMajorColor = System.Drawing.Color.Silver;
            this.gaugeControlCPU.ScaleLinesMajorInnerRadius = 58;
            this.gaugeControlCPU.ScaleLinesMajorOuterRadius = 48;
            this.gaugeControlCPU.ScaleLinesMajorStepValue = 50F;
            this.gaugeControlCPU.ScaleLinesMajorWidth = 2;
            this.gaugeControlCPU.ScaleLinesMinorColor = System.Drawing.Color.Silver;
            this.gaugeControlCPU.ScaleLinesMinorInnerRadius = 50;
            this.gaugeControlCPU.ScaleLinesMinorOuterRadius = 55;
            this.gaugeControlCPU.ScaleLinesMinorTicks = 9;
            this.gaugeControlCPU.ScaleLinesMinorWidth = 1;
            this.gaugeControlCPU.ScaleNumbersColor = System.Drawing.Color.Gainsboro;
            this.gaugeControlCPU.ScaleNumbersFormat = null;
            this.gaugeControlCPU.ScaleNumbersRadius = 70;
            this.gaugeControlCPU.ScaleNumbersRotation = 0;
            this.gaugeControlCPU.ScaleNumbersStartScaleLine = 0;
            this.gaugeControlCPU.ScaleNumbersStepScaleLines = 1;
            this.gaugeControlCPU.Size = new System.Drawing.Size(167, 145);
            this.gaugeControlCPU.TabIndex = 0;
            this.gaugeControlCPU.Text = "gauge";
            this.gaugeControlCPU.Value = -100F;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label4.Location = new System.Drawing.Point(50, 348);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 17);
            this.label4.TabIndex = 2;
            this.label4.Text = "Network";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label3.Location = new System.Drawing.Point(50, 172);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 17);
            this.label3.TabIndex = 2;
            this.label3.Text = "Memory";
            // 
            // gaugeControlNetwork
            // 
            this.gaugeControlNetwork.BaseArcColor = System.Drawing.Color.Transparent;
            this.gaugeControlNetwork.BaseArcRadius = 50;
            this.gaugeControlNetwork.BaseArcStart = 135;
            this.gaugeControlNetwork.BaseArcSweep = 270;
            this.gaugeControlNetwork.BaseArcWidth = 2;
            this.gaugeControlNetwork.Center = new System.Drawing.Point(80, 80);
            this.gaugeControlNetwork.Location = new System.Drawing.Point(1, 368);
            this.gaugeControlNetwork.MaxValue = 400F;
            this.gaugeControlNetwork.MinValue = -100F;
            this.gaugeControlNetwork.Name = "gaugeControlNetwork";
            this.gaugeControlNetwork.NeedleColor1 = System.Windows.Forms.AGaugeNeedleColor.Gray;
            this.gaugeControlNetwork.NeedleColor2 = System.Drawing.Color.SaddleBrown;
            this.gaugeControlNetwork.NeedleRadius = 50;
            this.gaugeControlNetwork.NeedleType = System.Windows.Forms.NeedleType.Advance;
            this.gaugeControlNetwork.NeedleWidth = 2;
            this.gaugeControlNetwork.ScaleLinesInterColor = System.Drawing.Color.Silver;
            this.gaugeControlNetwork.ScaleLinesInterInnerRadius = 58;
            this.gaugeControlNetwork.ScaleLinesInterOuterRadius = 43;
            this.gaugeControlNetwork.ScaleLinesInterWidth = 1;
            this.gaugeControlNetwork.ScaleLinesMajorColor = System.Drawing.Color.Silver;
            this.gaugeControlNetwork.ScaleLinesMajorInnerRadius = 58;
            this.gaugeControlNetwork.ScaleLinesMajorOuterRadius = 48;
            this.gaugeControlNetwork.ScaleLinesMajorStepValue = 50F;
            this.gaugeControlNetwork.ScaleLinesMajorWidth = 2;
            this.gaugeControlNetwork.ScaleLinesMinorColor = System.Drawing.Color.Silver;
            this.gaugeControlNetwork.ScaleLinesMinorInnerRadius = 50;
            this.gaugeControlNetwork.ScaleLinesMinorOuterRadius = 55;
            this.gaugeControlNetwork.ScaleLinesMinorTicks = 9;
            this.gaugeControlNetwork.ScaleLinesMinorWidth = 1;
            this.gaugeControlNetwork.ScaleNumbersColor = System.Drawing.Color.Gainsboro;
            this.gaugeControlNetwork.ScaleNumbersFormat = null;
            this.gaugeControlNetwork.ScaleNumbersRadius = 70;
            this.gaugeControlNetwork.ScaleNumbersRotation = 0;
            this.gaugeControlNetwork.ScaleNumbersStartScaleLine = 0;
            this.gaugeControlNetwork.ScaleNumbersStepScaleLines = 1;
            this.gaugeControlNetwork.Size = new System.Drawing.Size(167, 145);
            this.gaugeControlNetwork.TabIndex = 0;
            this.gaugeControlNetwork.Text = "gauge";
            this.gaugeControlNetwork.Value = -100F;
            // 
            // ctrlDashPanelClock
            // 
            this.ctrlDashPanelClock.BackColor = System.Drawing.Color.DimGray;
            this.ctrlDashPanelClock.Controls.Add(this.digitalClock);
            this.ctrlDashPanelClock.Lighted = false;
            this.ctrlDashPanelClock.LitBackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.ctrlDashPanelClock.Location = new System.Drawing.Point(56, 595);
            this.ctrlDashPanelClock.Name = "ctrlDashPanelClock";
            this.ctrlDashPanelClock.Radius = 10;
            this.ctrlDashPanelClock.Size = new System.Drawing.Size(123, 52);
            this.ctrlDashPanelClock.TabIndex = 6;
            this.ctrlDashPanelClock.Text = "ctrlDashPanel1";
            // 
            // digitalClock
            // 
            this.digitalClock.ArrayCount = 5;
            this.digitalClock.BackColor = System.Drawing.Color.Black;
            this.digitalClock.ColonShow = false;
            this.digitalClock.ColorBackground = System.Drawing.Color.DimGray;
            this.digitalClock.ColorDark = System.Drawing.Color.Gray;
            this.digitalClock.ColorLight = System.Drawing.Color.Gray;
            this.digitalClock.DecimalShow = true;
            this.digitalClock.ElementPadding = new System.Windows.Forms.Padding(1);
            this.digitalClock.ElementWidth = 10;
            this.digitalClock.ItalicFactor = 0F;
            this.digitalClock.Location = new System.Drawing.Point(11, 7);
            this.digitalClock.Name = "digitalClock";
            this.digitalClock.Size = new System.Drawing.Size(104, 32);
            this.digitalClock.TabIndex = 5;
            this.digitalClock.TabStop = false;
            this.digitalClock.Value = "0:00:00";
            // 
            // ctrlUserProcessStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.panelProcessStatus);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(15);
            this.Name = "ctrlUserProcessStatus";
            this.Size = new System.Drawing.Size(998, 796);
            this.panelProcessStatus.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWait)).EndInit();
            this.ctrlDashPanel1.ResumeLayout(false);
            this.ctrlDashPanel2.ResumeLayout(false);
            this.ctrlDashPanelGauges.ResumeLayout(false);
            this.ctrlDashPanelGauges.PerformLayout();
            this.ctrlDashPanelClock.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox txtStatus;
        private System.Windows.Forms.AGauge gaugeControlCPU;
        private System.Windows.Forms.Timer timerProcess;
        private System.Windows.Forms.AGauge gaugeControlMemory;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panelProcessStatus;
        private System.Windows.Forms.FlowLayoutPanel panelProcesses;
        private VU_MeterLibrary.VuMeter vuMeter;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.AGauge gaugeControlNetwork;
        private System.Windows.Forms.PictureBox pictureBoxWait;
        private Utils.SevenSegmentClock digitalClock;
        private System.Windows.Forms.Timer timerClock;
        private ctrlDashPanel ctrlDashPanelClock;
        private ctrlDashPanel ctrlDashPanel2;
        private ctrlDashPanel ctrlDashPanelGauges;
        private ctrlDashPanel ctrlDashPanel1;
        private ProgBar.ProgBarPlus progBarSize;
        private System.Windows.Forms.Label lblSize;
    }
}
