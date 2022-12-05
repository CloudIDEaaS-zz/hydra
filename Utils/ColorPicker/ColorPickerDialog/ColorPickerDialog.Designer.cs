﻿namespace Utils.Controls.ColorPicker
{
    partial class ColorPickerDialog
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelCurrent = new System.Windows.Forms.Label();
            this.labelCurrentColor = new System.Windows.Forms.Label();
            this.labelHex = new System.Windows.Forms.Label();
            this.textboxHexColor = new System.Windows.Forms.TextBox();
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabHexagon = new System.Windows.Forms.TabPage();
            this.tabWheel = new System.Windows.Forms.TabPage();
            this.tabColorBox = new System.Windows.Forms.TabPage();
            this.numLuminance = new System.Windows.Forms.NumericUpDown();
            this.radioLuminance = new System.Windows.Forms.RadioButton();
            this.numSaturation = new System.Windows.Forms.NumericUpDown();
            this.radioSaturation = new System.Windows.Forms.RadioButton();
            this.numHue = new System.Windows.Forms.NumericUpDown();
            this.radioHue = new System.Windows.Forms.RadioButton();
            this.numBlue = new System.Windows.Forms.NumericUpDown();
            this.radioBlue = new System.Windows.Forms.RadioButton();
            this.numGreen = new System.Windows.Forms.NumericUpDown();
            this.radioGreen = new System.Windows.Forms.RadioButton();
            this.numRed = new System.Windows.Forms.NumericUpDown();
            this.radioRed = new System.Windows.Forms.RadioButton();
            this.colorDropperIcon = new System.Windows.Forms.PictureBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.colorHexagon = new Utils.Controls.ColorPicker.ColorHexagon();
            this.colorWheel = new Utils.Controls.ColorPicker.ColorWheel();
            this.colorSlider = new Utils.Controls.ColorPicker.ColorSliderVertical();
            this.colorBox2D = new Utils.Controls.ColorPicker.ColorBox2D();
            this.tabControlMain.SuspendLayout();
            this.tabHexagon.SuspendLayout();
            this.tabWheel.SuspendLayout();
            this.tabColorBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLuminance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSaturation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBlue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGreen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorDropperIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // labelCurrent
            // 
            this.labelCurrent.AutoSize = true;
            this.labelCurrent.Location = new System.Drawing.Point(427, 22);
            this.labelCurrent.Name = "labelCurrent";
            this.labelCurrent.Size = new System.Drawing.Size(41, 13);
            this.labelCurrent.TabIndex = 1;
            this.labelCurrent.Text = "Current";
            // 
            // labelCurrentColor
            // 
            this.labelCurrentColor.BackColor = System.Drawing.Color.White;
            this.labelCurrentColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelCurrentColor.Location = new System.Drawing.Point(430, 39);
            this.labelCurrentColor.Name = "labelCurrentColor";
            this.labelCurrentColor.Size = new System.Drawing.Size(68, 32);
            this.labelCurrentColor.TabIndex = 2;
            // 
            // labelHex
            // 
            this.labelHex.AutoSize = true;
            this.labelHex.Location = new System.Drawing.Point(427, 98);
            this.labelHex.Name = "labelHex";
            this.labelHex.Size = new System.Drawing.Size(26, 13);
            this.labelHex.TabIndex = 3;
            this.labelHex.Text = "Hex";
            // 
            // textboxHexColor
            // 
            this.textboxHexColor.Location = new System.Drawing.Point(430, 114);
            this.textboxHexColor.Name = "textboxHexColor";
            this.textboxHexColor.Size = new System.Drawing.Size(68, 20);
            this.textboxHexColor.TabIndex = 4;
            this.textboxHexColor.Text = "FFFFFF";
            this.textboxHexColor.TextChanged += new System.EventHandler(this.textboxHexColor_TextChanged);
            this.textboxHexColor.Leave += new System.EventHandler(this.textboxHexColor_Leave);
            // 
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.tabHexagon);
            this.tabControlMain.Controls.Add(this.tabWheel);
            this.tabControlMain.Controls.Add(this.tabColorBox);
            this.tabControlMain.Location = new System.Drawing.Point(0, 0);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(424, 297);
            this.tabControlMain.TabIndex = 5;
            // 
            // tabHexagon
            // 
            this.tabHexagon.Controls.Add(this.colorHexagon);
            this.tabHexagon.Location = new System.Drawing.Point(4, 22);
            this.tabHexagon.Name = "tabHexagon";
            this.tabHexagon.Padding = new System.Windows.Forms.Padding(3);
            this.tabHexagon.Size = new System.Drawing.Size(416, 271);
            this.tabHexagon.TabIndex = 0;
            this.tabHexagon.Text = "Color Hexagon";
            this.tabHexagon.UseVisualStyleBackColor = true;
            // 
            // tabWheel
            // 
            this.tabWheel.Controls.Add(this.colorWheel);
            this.tabWheel.Location = new System.Drawing.Point(4, 22);
            this.tabWheel.Name = "tabWheel";
            this.tabWheel.Padding = new System.Windows.Forms.Padding(3);
            this.tabWheel.Size = new System.Drawing.Size(416, 271);
            this.tabWheel.TabIndex = 1;
            this.tabWheel.Text = "Color Wheel";
            this.tabWheel.UseVisualStyleBackColor = true;
            // 
            // tabColorBox
            // 
            this.tabColorBox.Controls.Add(this.numLuminance);
            this.tabColorBox.Controls.Add(this.radioLuminance);
            this.tabColorBox.Controls.Add(this.numSaturation);
            this.tabColorBox.Controls.Add(this.radioSaturation);
            this.tabColorBox.Controls.Add(this.numHue);
            this.tabColorBox.Controls.Add(this.radioHue);
            this.tabColorBox.Controls.Add(this.numBlue);
            this.tabColorBox.Controls.Add(this.radioBlue);
            this.tabColorBox.Controls.Add(this.numGreen);
            this.tabColorBox.Controls.Add(this.radioGreen);
            this.tabColorBox.Controls.Add(this.numRed);
            this.tabColorBox.Controls.Add(this.radioRed);
            this.tabColorBox.Controls.Add(this.colorSlider);
            this.tabColorBox.Controls.Add(this.colorBox2D);
            this.tabColorBox.Location = new System.Drawing.Point(4, 22);
            this.tabColorBox.Name = "tabColorBox";
            this.tabColorBox.Padding = new System.Windows.Forms.Padding(3);
            this.tabColorBox.Size = new System.Drawing.Size(416, 271);
            this.tabColorBox.TabIndex = 2;
            this.tabColorBox.Text = "Color Box & Slider";
            this.tabColorBox.UseVisualStyleBackColor = true;
            // 
            // numLuminance
            // 
            this.numLuminance.Location = new System.Drawing.Point(349, 165);
            this.numLuminance.Name = "numLuminance";
            this.numLuminance.Size = new System.Drawing.Size(54, 20);
            this.numLuminance.TabIndex = 13;
            this.numLuminance.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numLuminance.ValueChanged += new System.EventHandler(this.numLuminance_ValueChanged);
            // 
            // radioLuminance
            // 
            this.radioLuminance.AutoSize = true;
            this.radioLuminance.Location = new System.Drawing.Point(306, 165);
            this.radioLuminance.Name = "radioLuminance";
            this.radioLuminance.Size = new System.Drawing.Size(34, 17);
            this.radioLuminance.TabIndex = 12;
            this.radioLuminance.Text = "L:";
            this.radioLuminance.UseVisualStyleBackColor = true;
            this.radioLuminance.CheckedChanged += new System.EventHandler(this.ColorModeChangedHandler);
            // 
            // numSaturation
            // 
            this.numSaturation.Location = new System.Drawing.Point(349, 139);
            this.numSaturation.Name = "numSaturation";
            this.numSaturation.Size = new System.Drawing.Size(54, 20);
            this.numSaturation.TabIndex = 11;
            this.numSaturation.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numSaturation.ValueChanged += new System.EventHandler(this.numSaturation_ValueChanged);
            // 
            // radioSaturation
            // 
            this.radioSaturation.AutoSize = true;
            this.radioSaturation.Location = new System.Drawing.Point(306, 139);
            this.radioSaturation.Name = "radioSaturation";
            this.radioSaturation.Size = new System.Drawing.Size(35, 17);
            this.radioSaturation.TabIndex = 10;
            this.radioSaturation.Text = "S:";
            this.radioSaturation.UseVisualStyleBackColor = true;
            this.radioSaturation.CheckedChanged += new System.EventHandler(this.ColorModeChangedHandler);
            // 
            // numHue
            // 
            this.numHue.Location = new System.Drawing.Point(349, 113);
            this.numHue.Maximum = new decimal(new int[] {
            359,
            0,
            0,
            0});
            this.numHue.Name = "numHue";
            this.numHue.Size = new System.Drawing.Size(54, 20);
            this.numHue.TabIndex = 9;
            this.numHue.ValueChanged += new System.EventHandler(this.numHue_ValueChanged);
            // 
            // radioHue
            // 
            this.radioHue.AutoSize = true;
            this.radioHue.Checked = true;
            this.radioHue.Location = new System.Drawing.Point(306, 113);
            this.radioHue.Name = "radioHue";
            this.radioHue.Size = new System.Drawing.Size(36, 17);
            this.radioHue.TabIndex = 8;
            this.radioHue.TabStop = true;
            this.radioHue.Text = "H:";
            this.radioHue.UseVisualStyleBackColor = true;
            this.radioHue.CheckedChanged += new System.EventHandler(this.ColorModeChangedHandler);
            // 
            // numBlue
            // 
            this.numBlue.Location = new System.Drawing.Point(349, 69);
            this.numBlue.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numBlue.Name = "numBlue";
            this.numBlue.Size = new System.Drawing.Size(54, 20);
            this.numBlue.TabIndex = 7;
            this.numBlue.ValueChanged += new System.EventHandler(this.numBlue_ValueChanged);
            // 
            // radioBlue
            // 
            this.radioBlue.AutoSize = true;
            this.radioBlue.Location = new System.Drawing.Point(306, 69);
            this.radioBlue.Name = "radioBlue";
            this.radioBlue.Size = new System.Drawing.Size(35, 17);
            this.radioBlue.TabIndex = 6;
            this.radioBlue.Text = "B:";
            this.radioBlue.UseVisualStyleBackColor = true;
            this.radioBlue.CheckedChanged += new System.EventHandler(this.ColorModeChangedHandler);
            // 
            // numGreen
            // 
            this.numGreen.Location = new System.Drawing.Point(349, 43);
            this.numGreen.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numGreen.Name = "numGreen";
            this.numGreen.Size = new System.Drawing.Size(54, 20);
            this.numGreen.TabIndex = 5;
            this.numGreen.ValueChanged += new System.EventHandler(this.numGreen_ValueChanged);
            // 
            // radioGreen
            // 
            this.radioGreen.AutoSize = true;
            this.radioGreen.Location = new System.Drawing.Point(306, 43);
            this.radioGreen.Name = "radioGreen";
            this.radioGreen.Size = new System.Drawing.Size(36, 17);
            this.radioGreen.TabIndex = 4;
            this.radioGreen.Text = "G:";
            this.radioGreen.UseVisualStyleBackColor = true;
            this.radioGreen.CheckedChanged += new System.EventHandler(this.ColorModeChangedHandler);
            // 
            // numRed
            // 
            this.numRed.Location = new System.Drawing.Point(349, 17);
            this.numRed.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numRed.Name = "numRed";
            this.numRed.Size = new System.Drawing.Size(54, 20);
            this.numRed.TabIndex = 3;
            this.numRed.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numRed.ValueChanged += new System.EventHandler(this.numRed_ValueChanged);
            // 
            // radioRed
            // 
            this.radioRed.AutoSize = true;
            this.radioRed.Location = new System.Drawing.Point(306, 17);
            this.radioRed.Name = "radioRed";
            this.radioRed.Size = new System.Drawing.Size(36, 17);
            this.radioRed.TabIndex = 2;
            this.radioRed.Text = "R:";
            this.radioRed.UseVisualStyleBackColor = true;
            this.radioRed.CheckedChanged += new System.EventHandler(this.ColorModeChangedHandler);
            // 
            // colorDropperIcon
            // 
            this.colorDropperIcon.Image = global::Utils.Properties.Resources.ColorSelector;
            this.colorDropperIcon.Location = new System.Drawing.Point(430, 74);
            this.colorDropperIcon.Name = "colorDropperIcon";
            this.colorDropperIcon.Size = new System.Drawing.Size(30, 25);
            this.colorDropperIcon.TabIndex = 6;
            this.colorDropperIcon.TabStop = false;
            this.colorDropperIcon.Click += new System.EventHandler(this.colorDropperIcon_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(335, 306);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 7;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(416, 306);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // colorHexagon
            // 
            this.colorHexagon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.colorHexagon.Location = new System.Drawing.Point(3, 3);
            this.colorHexagon.Name = "colorHexagon";
            this.colorHexagon.SelectedColor = System.Drawing.Color.Empty;
            this.colorHexagon.Size = new System.Drawing.Size(410, 265);
            this.colorHexagon.TabIndex = 1;
            this.colorHexagon.ColorChanged += new Utils.Controls.ColorPicker.ColorHexagon.ColorChangedEventHandler(this.colorHexagon_ColorChanged);
            // 
            // colorWheel
            // 
            this.colorWheel.Color = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.colorWheel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.colorWheel.Location = new System.Drawing.Point(3, 3);
            this.colorWheel.Name = "colorWheel";
            this.colorWheel.Size = new System.Drawing.Size(410, 265);
            this.colorWheel.TabIndex = 0;
            this.colorWheel.ColorChanged += new System.EventHandler(this.colorWheel_ColorChanged);
            // 
            // colorSlider
            // 
            this.colorSlider.ColorMode = Utils.Controls.ColorPicker.ColorModes.Hue;
            this.colorSlider.ColorRGB = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.colorSlider.Location = new System.Drawing.Point(259, 8);
            this.colorSlider.Name = "colorSlider";
            this.colorSlider.NubColor = System.Drawing.Color.White;
            this.colorSlider.Position = 142;
            this.colorSlider.Size = new System.Drawing.Size(40, 252);
            this.colorSlider.TabIndex = 1;
            this.colorSlider.ColorChanged += new Utils.Controls.ColorPicker.ColorSliderVertical.ColorChangedEventHandler(this.colorSlider_ColorChanged);
            // 
            // colorBox2D
            // 
            this.colorBox2D.ColorMode = Utils.Controls.ColorPicker.ColorModes.Hue;
            this.colorBox2D.ColorRGB = System.Drawing.Color.Red;
            this.colorBox2D.Location = new System.Drawing.Point(8, 8);
            this.colorBox2D.Name = "colorBox2D";
            this.colorBox2D.Size = new System.Drawing.Size(245, 252);
            this.colorBox2D.TabIndex = 0;
            this.colorBox2D.ColorChanged += new Utils.Controls.ColorPicker.ColorBox2D.ColorChangedEventHandler(this.colorBox2D_ColorChanged);
            // 
            // ColorPickerDialog
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(503, 341);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.colorDropperIcon);
            this.Controls.Add(this.tabControlMain);
            this.Controls.Add(this.textboxHexColor);
            this.Controls.Add(this.labelHex);
            this.Controls.Add(this.labelCurrentColor);
            this.Controls.Add(this.labelCurrent);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ColorPickerDialog";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Color Selector";
            this.TopMost = true;
            this.Deactivate += new System.EventHandler(this.ColorPickerDialog_Deactivate);
            this.Load += new System.EventHandler(this.FormColorPickerDemo_Load);
            this.tabControlMain.ResumeLayout(false);
            this.tabHexagon.ResumeLayout(false);
            this.tabWheel.ResumeLayout(false);
            this.tabColorBox.ResumeLayout(false);
            this.tabColorBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLuminance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSaturation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBlue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGreen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.colorDropperIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelCurrent;
        private System.Windows.Forms.Label labelCurrentColor;
        private System.Windows.Forms.Label labelHex;
        private System.Windows.Forms.TextBox textboxHexColor;
        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabHexagon;
        private Utils.Controls.ColorPicker.ColorHexagon colorHexagon;
        private System.Windows.Forms.TabPage tabWheel;
        private Utils.Controls.ColorPicker.ColorWheel colorWheel;
        private System.Windows.Forms.TabPage tabColorBox;
        private Utils.Controls.ColorPicker.ColorBox2D colorBox2D;
        private Utils.Controls.ColorPicker.ColorSliderVertical colorSlider;
        private System.Windows.Forms.NumericUpDown numBlue;
        private System.Windows.Forms.RadioButton radioBlue;
        private System.Windows.Forms.NumericUpDown numGreen;
        private System.Windows.Forms.RadioButton radioGreen;
        private System.Windows.Forms.NumericUpDown numRed;
        private System.Windows.Forms.RadioButton radioRed;
        private System.Windows.Forms.NumericUpDown numLuminance;
        private System.Windows.Forms.RadioButton radioLuminance;
        private System.Windows.Forms.NumericUpDown numSaturation;
        private System.Windows.Forms.RadioButton radioSaturation;
        private System.Windows.Forms.NumericUpDown numHue;
        private System.Windows.Forms.RadioButton radioHue;
        private System.Windows.Forms.PictureBox colorDropperIcon;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
    }
}

