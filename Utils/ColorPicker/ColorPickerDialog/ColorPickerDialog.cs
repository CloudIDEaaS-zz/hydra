﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils.Controls.ColorPicker;

namespace Utils.Controls.ColorPicker
{
    public partial class ColorPickerDialog : Form
    {
        #region Fields

        private HslColor colorHsl = HslColor.FromAhsl(0xff);
        private ColorModes colorMode = ColorModes.Hue;
        private Color colorRgb = System.Drawing.Color.Empty;
        private bool lockUpdates = false;
        private Form m_InstanceRef = null;
        private bool closing;

        #endregion

        public ColorPickerDialog()
        {
            InitializeComponent();
            this.colorBox2D.ColorMode = this.colorMode;
            this.colorSlider.ColorMode = this.colorMode;
        }

        private void colorHexagon_ColorChanged(object sender, ColorChangedEventArgs args)
        {
            labelCurrentColor.BackColor = colorHexagon.SelectedColor;
            textboxHexColor.Text = ColorTranslator.ToHtml(colorHexagon.SelectedColor);
            this.colorRgb = colorHexagon.SelectedColor;
        }

        private void colorWheel_ColorChanged(object sender, EventArgs e)
        {
            labelCurrentColor.BackColor = colorWheel.Color;
            textboxHexColor.Text = ColorTranslator.ToHtml(colorWheel.Color);
            this.colorRgb = colorWheel.Color;
        }

        public Form InstanceRef
        {
            get
            {
                return m_InstanceRef;
            }
            set
            {
                m_InstanceRef = value;
            }
        }

        public Color? Color 
        { 
            get
            {
                return colorRgb;
            }

            set
            {
                colorRgb = value.Value;

                UpdateColorFields();
            }
        }

        private void colorSlider_ColorChanged(object sender, ColorChangedEventArgs args)
        {
            if (!this.lockUpdates)
            {
                HslColor colorHSL = this.colorSlider.ColorHSL;
                this.colorHsl = colorHSL;
                this.colorRgb = this.colorHsl.RgbValue;
                this.lockUpdates = true;
                this.colorBox2D.ColorHSL = this.colorHsl;
                this.lockUpdates = false;
                labelCurrentColor.BackColor = this.colorRgb;
                textboxHexColor.Text = ColorTranslator.ToHtml(this.colorRgb);
                UpdateColorFields();
            }  
        }

        private void colorBox2D_ColorChanged(object sender, ColorChangedEventArgs args)
        {
            if (!this.lockUpdates)
            {
                HslColor colorHSL = this.colorBox2D.ColorHSL;
                this.colorHsl = colorHSL;
                this.colorRgb = this.colorHsl.RgbValue;
                this.lockUpdates = true;
                this.colorSlider.ColorHSL = this.colorHsl;
                this.lockUpdates = false;
                labelCurrentColor.BackColor = this.colorRgb;
                textboxHexColor.Text = ColorTranslator.ToHtml(this.colorRgb);
                UpdateColorFields();
            }    
        }

        private void ColorModeChangedHandler(object sender, EventArgs e)
        {
            if (sender == this.radioRed)
            {
                this.colorMode = ColorModes.Red;
            }
            else if (sender == this.radioGreen)
            {
                this.colorMode = ColorModes.Green;
            }
            else if (sender == this.radioBlue)
            {
                this.colorMode = ColorModes.Blue;
            }
            else if (sender == this.radioHue)
            {
                this.colorMode = ColorModes.Hue;
            }
            else if (sender == this.radioSaturation)
            {
                this.colorMode = ColorModes.Saturation;
            }
            else if (sender == this.radioLuminance)
            {
                this.colorMode = ColorModes.Luminance;
            }
            this.colorSlider.ColorMode = this.colorMode;
            this.colorBox2D.ColorMode = this.colorMode;        
        }

        private void UpdateColorFields()
        {
            this.lockUpdates = true;
            this.numRed.Value = this.colorRgb.R;
            this.numGreen.Value = this.colorRgb.G;
            this.numBlue.Value = this.colorRgb.B;
            this.numHue.Value = (int)(((decimal)this.colorHsl.H) * 360M);
            this.numSaturation.Value = (int)(((decimal)this.colorHsl.S) * 100M);
            this.numLuminance.Value = (int)(((decimal)this.colorHsl.L) * 100M);
            this.lockUpdates = false;
        }

        private void UpdateRgbFields(Color newColor)
        {
            this.colorHsl = HslColor.FromColor(newColor);
            this.colorRgb = newColor;
            this.lockUpdates = true;
            this.numHue.Value = (int)(((decimal)this.colorHsl.H) * 360M);
            this.numSaturation.Value = (int)(((decimal)this.colorHsl.S) * 100M);
            this.numLuminance.Value = (int)(((decimal)this.colorHsl.L) * 100M);
            this.lockUpdates = false;
            this.colorSlider.ColorHSL = this.colorHsl;
            this.colorBox2D.ColorHSL = this.colorHsl;
        }

        private void UpdateHslFields(HslColor newColor)
        {
            this.colorHsl = newColor;
            this.colorRgb = newColor.RgbValue;
            this.lockUpdates = true;
            this.numRed.Value = this.colorRgb.R;
            this.numGreen.Value = this.colorRgb.G;
            this.numBlue.Value = this.colorRgb.B;
            this.lockUpdates = false;
            this.colorSlider.ColorHSL = this.colorHsl;
            this.colorBox2D.ColorHSL = this.colorHsl;
        }

        private void numRed_ValueChanged(object sender, EventArgs e)
        {
            if (!this.lockUpdates)
            {
                UpdateRgbFields(System.Drawing.Color.FromArgb((int)this.numRed.Value, (int)this.numGreen.Value, (int)this.numBlue.Value));
            }
        }

        private void numGreen_ValueChanged(object sender, EventArgs e)
        {
            if (!this.lockUpdates)
            {
                UpdateRgbFields(System.Drawing.Color.FromArgb((int)this.numRed.Value, (int)this.numGreen.Value, (int)this.numBlue.Value));
            }
        }

        private void numBlue_ValueChanged(object sender, EventArgs e)
        {
            if (!this.lockUpdates)
            {
                UpdateRgbFields(System.Drawing.Color.FromArgb((int)this.numRed.Value, (int)this.numGreen.Value, (int)this.numBlue.Value));
            }
        }

        private void numHue_ValueChanged(object sender, EventArgs e)
        {
            if (!this.lockUpdates)
            {
                HslColor newColor = HslColor.FromAhsl((double)(((float)((int)this.numHue.Value)) / 360f), this.colorHsl.S, this.colorHsl.L);
                this.UpdateHslFields(newColor);
            }
        }

        private void numSaturation_ValueChanged(object sender, EventArgs e)
        {
            if (!this.lockUpdates)
            {
                HslColor newColor = HslColor.FromAhsl(this.colorHsl.A, this.colorHsl.H, (double)(((float)((int)this.numSaturation.Value)) / 100f), this.colorHsl.L);
                this.UpdateHslFields(newColor);
            }
            
        }

        private void numLuminance_ValueChanged(object sender, EventArgs e)
        {
            if (!this.lockUpdates)
            {
                HslColor newColor = HslColor.FromAhsl(this.colorHsl.A, this.colorHsl.H, this.colorHsl.S, (double)(((float)((int)this.numLuminance.Value)) / 100f));
                this.UpdateHslFields(newColor);
            }
        }

        private void FormColorPickerDemo_Load(object sender, EventArgs e)
        {
            var colorHsl = HslColor.FromColor(colorRgb);

            labelCurrentColor.BackColor = colorRgb;
            textboxHexColor.Text = ColorTranslator.ToHtml(this.colorRgb);

            this.colorHsl = colorHsl;

            colorBox2D.ColorRGB = colorRgb;
            colorHexagon.SelectedColor = colorRgb;
            colorSlider.ColorRGB = colorRgb;
            colorSlider.ColorHSL = colorHsl;

            UpdateColorFields();
        }

        private void colorDropperIcon_Click(object sender, EventArgs e)
        {
            var hIcon = Utils.Properties.Resources.ColorSelector.GetHicon();
            var cursor = CursorExtensions.CreateCursor(hIcon, 0, 13);
            OverlayScreen overlayScreen;

            Cursor.Current = cursor;
            this.Hide();

            if (this.InstanceRef != null)
            {
                this.InstanceRef.Hide();
            }

            this.InstanceRef.HideConsole();

            overlayScreen = new OverlayScreen();

            overlayScreen.Cursor = cursor;
            overlayScreen.InstanceRef = this;

            overlayScreen.Show();

            while (overlayScreen.IsHandleCreated)
            {
                this.DoEventsSleep(100);
            }

            this.InstanceRef.ShowConsole();

            if (this.InstanceRef != null)
            {
                this.InstanceRef.Show();
            }

            if (overlayScreen.Color != null)
            {
                labelCurrentColor.BackColor = overlayScreen.Color.Value;
                textboxHexColor.Text = ColorTranslator.ToHtml(overlayScreen.Color.Value);
                colorRgb = overlayScreen.Color.Value;

                UpdateColorFields();
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Color = this.colorRgb;
            this.DialogResult = DialogResult.OK;
            this.closing = true;

            this.Close();
        }

        private bool IsStartOfHexEntry(string text)
        {
            var regex = new Regex("^#?[a-fA-F0-9]{0,6}$");

            return regex.IsMatch(text);
        }

        private void textboxHexColor_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!textboxHexColor.Text.IsNullOrEmpty())
                {
                    var color = ColorTranslator.FromHtml(textboxHexColor.Text);

                    if (labelCurrentColor.BackColor != color)
                    {
                        labelCurrentColor.BackColor = ColorTranslator.FromHtml(textboxHexColor.Text);
                        colorRgb = labelCurrentColor.BackColor;

                        UpdateColorFields();
                    }
                }
                else if (!IsStartOfHexEntry(textboxHexColor.Text))
                {
                    textboxHexColor.Text = ColorTranslator.ToHtml(colorRgb);

                    UpdateColorFields();
                }
            }
            catch
            {
                if (!IsStartOfHexEntry(textboxHexColor.Text))
                {
                    textboxHexColor.Text = ColorTranslator.ToHtml(colorRgb);

                    UpdateColorFields();
                }
            }
        }

        private void textboxHexColor_Leave(object sender, EventArgs e)
        {
            textboxHexColor.Text = ColorTranslator.ToHtml(colorRgb);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.closing = true;

            this.Close();
        }

        private void ColorPickerDialog_Deactivate(object sender, EventArgs e)
        {
            if (!this.closing)
            {
                this.Close();
            }
        }
    }
}
