using System.Drawing;
using WizardBase;

namespace WizardDemo
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.wizardControl1 = new WizardBase.WizardControl();
            this.startStep1 = new WizardBase.StartStep();
            this.licenceStep1 = new WizardBase.LicenceStep();
            this.intermediateStep1 = new WizardBase.IntermediateStep();
            this.finishStep1 = new WizardBase.FinishStep();
            this.SuspendLayout();
            // 
            // wizardControl1
            // 
            this.wizardControl1.BackButtonEnabled = true;
            this.wizardControl1.BackButtonVisible = true;
            this.wizardControl1.CancelButtonEnabled = true;
            this.wizardControl1.CancelButtonVisible = true;
            this.wizardControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardControl1.EulaButtonEnabled = true;
            this.wizardControl1.EulaButtonText = "eula";
            this.wizardControl1.EulaButtonVisible = true;
            this.wizardControl1.HelpButtonEnabled = true;
            this.wizardControl1.HelpButtonVisible = true;
            this.wizardControl1.Location = new System.Drawing.Point(0, 0);
            this.wizardControl1.Name = "wizardControl1";
            this.wizardControl1.NextButtonEnabled = true;
            this.wizardControl1.NextButtonVisible = true;
            this.wizardControl1.Size = new System.Drawing.Size(532, 392);
            this.wizardControl1.WizardSteps.AddRange(new WizardBase.WizardStep[] {
            ((WizardBase.WizardStep)(this.startStep1)),
            ((WizardBase.WizardStep)(this.licenceStep1)),
            ((WizardBase.WizardStep)(this.intermediateStep1)),
            ((WizardBase.WizardStep)(this.finishStep1))});
            this.wizardControl1.CancelButtonClick += new System.EventHandler(this.demoWizard_CancelButtonClick);
            this.wizardControl1.FinishButtonClick += new System.EventHandler(this.demoWizard_FinishButtonClick);
            this.wizardControl1.EulaButtonClick += new System.EventHandler(this.wizardControl1_EulaButtonClick);
            // 
            // startStep1
            // 
            this.startStep1.BindingImage = null;
            this.startStep1.Icon = null;
            this.startStep1.IsStartStep = false;
            this.startStep1.LeftPair = ((WizardBase.ColorPair)(resources.GetObject("startStep1.LeftPair")));
            this.startStep1.Name = "startStep1";
            this.startStep1.SubtitleAppearence = ((WizardBase.TextAppearence)(resources.GetObject("startStep1.SubtitleAppearence")));
            this.startStep1.TitleAppearence = ((WizardBase.TextAppearence)(resources.GetObject("startStep1.TitleAppearence")));
            // 
            // licenceStep1
            // 
            this.licenceStep1.BindingImage = null;
            this.licenceStep1.HeaderPair = ((WizardBase.ColorPair)(resources.GetObject("licenceStep1.HeaderPair")));
            this.licenceStep1.IsStartStep = false;
            this.licenceStep1.LicenseFile = "";
            this.licenceStep1.Name = "licenceStep1";
            this.licenceStep1.SubtitleAppearence = ((WizardBase.TextAppearence)(resources.GetObject("licenceStep1.SubtitleAppearence")));
            this.licenceStep1.Title = "License Agreement.";
            this.licenceStep1.TitleAppearence = ((WizardBase.TextAppearence)(resources.GetObject("licenceStep1.TitleAppearence")));
            this.licenceStep1.Warning = "Please read the following license agreement. You must accept the terms  of this a" +
    "greement before continuing.";
            this.licenceStep1.WarningFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            // 
            // intermediateStep1
            // 
            this.intermediateStep1.BindingImage = null;
            this.intermediateStep1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.intermediateStep1.HeaderPair = ((WizardBase.ColorPair)(resources.GetObject("intermediateStep1.HeaderPair")));
            this.intermediateStep1.IsStartStep = false;
            this.intermediateStep1.Name = "intermediateStep1";
            this.intermediateStep1.SubtitleAppearence = ((WizardBase.TextAppearence)(resources.GetObject("intermediateStep1.SubtitleAppearence")));
            this.intermediateStep1.TitleAppearence = ((WizardBase.TextAppearence)(resources.GetObject("intermediateStep1.TitleAppearence")));
            // 
            // finishStep1
            // 
            this.finishStep1.BindingImage = null;
            this.finishStep1.IsStartStep = false;
            this.finishStep1.Name = "finishStep1";
            this.finishStep1.Pair = ((WizardBase.ColorPair)(resources.GetObject("finishStep1.Pair")));
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(532, 392);
            this.Controls.Add(this.wizardControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private WizardControl wizardControl1;
        private StartStep startStep1;
        private LicenceStep licenceStep1;
        private IntermediateStep intermediateStep1;
        private FinishStep finishStep1;







    }
}

