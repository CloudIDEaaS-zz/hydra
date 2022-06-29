
namespace AbstraX
{
    partial class ctrlWizardStepStatus
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
            this.labelCaption = new System.Windows.Forms.Label();
            this.panelUserProcessStatus = new System.Windows.Forms.Panel();
            this.ctrlUserProcessStatus = new AbstraX.ctrlUserProcessStatus();
            this.panelUserProcessStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelCaption
            // 
            this.labelCaption.BackColor = System.Drawing.Color.White;
            this.labelCaption.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelCaption.Font = new System.Drawing.Font("Century Gothic", 18F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(118)))), ((int)(((byte)(117)))));
            this.labelCaption.Location = new System.Drawing.Point(0, 0);
            this.labelCaption.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelCaption.Name = "labelCaption";
            this.labelCaption.Size = new System.Drawing.Size(1056, 38);
            this.labelCaption.TabIndex = 6;
            this.labelCaption.Text = "[Not Set]";
            this.labelCaption.Paint += new System.Windows.Forms.PaintEventHandler(this.labelCaption_Paint);
            // 
            // panelUserProcessStatus
            // 
            this.panelUserProcessStatus.Controls.Add(this.ctrlUserProcessStatus);
            this.panelUserProcessStatus.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelUserProcessStatus.Location = new System.Drawing.Point(397, 38);
            this.panelUserProcessStatus.Name = "panelUserProcessStatus";
            this.panelUserProcessStatus.Padding = new System.Windows.Forms.Padding(5);
            this.panelUserProcessStatus.Size = new System.Drawing.Size(659, 571);
            this.panelUserProcessStatus.TabIndex = 3;
            // 
            // ctrlUserProcessStatus
            // 
            this.ctrlUserProcessStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrlUserProcessStatus.LabelFileSizeValue = "0 KB";
            this.ctrlUserProcessStatus.Location = new System.Drawing.Point(15, 15);
            this.ctrlUserProcessStatus.Margin = new System.Windows.Forms.Padding(15);
            this.ctrlUserProcessStatus.Name = "ctrlUserProcessStatus";
            this.ctrlUserProcessStatus.Process = null;
            this.ctrlUserProcessStatus.ProcessHandler = null;
            this.ctrlUserProcessStatus.ProgressBarFileSizeValue = 0;
            this.ctrlUserProcessStatus.RootDirectory = null;
            this.ctrlUserProcessStatus.Size = new System.Drawing.Size(1026, 541);
            this.ctrlUserProcessStatus.Stopwatch = null;
            this.ctrlUserProcessStatus.TabIndex = 0;
            // 
            // ctrlWizardStepStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelUserProcessStatus);
            this.Controls.Add(this.labelCaption);
            this.Name = "ctrlWizardStepStatus";
            this.Size = new System.Drawing.Size(1056, 609);
            this.panelUserProcessStatus.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelCaption;
        private System.Windows.Forms.Panel panelUserProcessStatus;
        private ctrlUserProcessStatus ctrlUserProcessStatus;
    }
}
