
namespace HydraCrashAnalyzer
{
    partial class frmAnalyzer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAnalyzer));
            this.lblMessage = new System.Windows.Forms.Label();
            this.txtDetails = new System.Windows.Forms.RichTextBox();
            this.cmdClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtError = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtApplication = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cmdOpenLog = new System.Windows.Forms.LinkLabel();
            this.cmdOpenDumpFile = new System.Windows.Forms.LinkLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.cmdViewPolicy = new System.Windows.Forms.LinkLabel();
            this.label6 = new System.Windows.Forms.Label();
            this.txtSteps = new System.Windows.Forms.RichTextBox();
            this.cmdSubmit = new System.Windows.Forms.LinkLabel();
            this.label7 = new System.Windows.Forms.Label();
            this.txtCaseId = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMessage.ForeColor = System.Drawing.Color.Firebrick;
            this.lblMessage.Location = new System.Drawing.Point(14, 15);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(125, 13);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "Performing Analysis..";
            // 
            // txtDetails
            // 
            this.txtDetails.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDetails.Location = new System.Drawing.Point(12, 386);
            this.txtDetails.Name = "txtDetails";
            this.txtDetails.ReadOnly = true;
            this.txtDetails.Size = new System.Drawing.Size(775, 151);
            this.txtDetails.TabIndex = 1;
            this.txtDetails.Text = "";
            // 
            // cmdClose
            // 
            this.cmdClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdClose.Enabled = false;
            this.cmdClose.Location = new System.Drawing.Point(712, 549);
            this.cmdClose.Name = "cmdClose";
            this.cmdClose.Size = new System.Drawing.Size(75, 23);
            this.cmdClose.TabIndex = 2;
            this.cmdClose.Text = "Close";
            this.cmdClose.UseVisualStyleBackColor = true;
            this.cmdClose.Click += new System.EventHandler(this.cmdClose_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 370);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Details";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Error";
            // 
            // txtError
            // 
            this.txtError.BackColor = System.Drawing.SystemColors.Window;
            this.txtError.Location = new System.Drawing.Point(74, 64);
            this.txtError.Name = "txtError";
            this.txtError.ReadOnly = true;
            this.txtError.Size = new System.Drawing.Size(567, 20);
            this.txtError.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Application";
            // 
            // txtApplication
            // 
            this.txtApplication.BackColor = System.Drawing.SystemColors.Window;
            this.txtApplication.Location = new System.Drawing.Point(74, 38);
            this.txtApplication.Name = "txtApplication";
            this.txtApplication.ReadOnly = true;
            this.txtApplication.Size = new System.Drawing.Size(567, 20);
            this.txtApplication.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 89);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(25, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Log";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 115);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(54, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Dump File";
            // 
            // cmdOpenLog
            // 
            this.cmdOpenLog.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(161)))), ((int)(((byte)(255)))));
            this.cmdOpenLog.AutoEllipsis = true;
            this.cmdOpenLog.BackColor = System.Drawing.Color.Transparent;
            this.cmdOpenLog.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(107)))), ((int)(((byte)(174)))));
            this.cmdOpenLog.Location = new System.Drawing.Point(74, 89);
            this.cmdOpenLog.Name = "cmdOpenLog";
            this.cmdOpenLog.Size = new System.Drawing.Size(567, 20);
            this.cmdOpenLog.TabIndex = 4;
            this.cmdOpenLog.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.cmdOpenLog_LinkClicked);
            // 
            // cmdOpenDumpFile
            // 
            this.cmdOpenDumpFile.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(161)))), ((int)(((byte)(255)))));
            this.cmdOpenDumpFile.AutoEllipsis = true;
            this.cmdOpenDumpFile.BackColor = System.Drawing.Color.Transparent;
            this.cmdOpenDumpFile.Enabled = false;
            this.cmdOpenDumpFile.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(107)))), ((int)(((byte)(174)))));
            this.cmdOpenDumpFile.Location = new System.Drawing.Point(74, 115);
            this.cmdOpenDumpFile.Name = "cmdOpenDumpFile";
            this.cmdOpenDumpFile.Size = new System.Drawing.Size(567, 20);
            this.cmdOpenDumpFile.TabIndex = 4;
            this.cmdOpenDumpFile.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.cmdOpenDumpFile_LinkClicked);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(662, 15);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(126, 174);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // cmdViewPolicy
            // 
            this.cmdViewPolicy.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(161)))), ((int)(((byte)(255)))));
            this.cmdViewPolicy.AutoSize = true;
            this.cmdViewPolicy.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(107)))), ((int)(((byte)(174)))));
            this.cmdViewPolicy.Location = new System.Drawing.Point(14, 135);
            this.cmdViewPolicy.Name = "cmdViewPolicy";
            this.cmdViewPolicy.Size = new System.Drawing.Size(150, 13);
            this.cmdViewPolicy.TabIndex = 6;
            this.cmdViewPolicy.TabStop = true;
            this.cmdViewPolicy.Text = "View our data collection policy";
            this.cmdViewPolicy.Visible = false;
            this.cmdViewPolicy.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.cmdViewPolicy_LinkClicked);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 195);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(461, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "You are not to blame, but please help us out by describing the steps you took lea" +
    "ding to the error";
            // 
            // txtSteps
            // 
            this.txtSteps.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSteps.BackColor = System.Drawing.SystemColors.Window;
            this.txtSteps.Location = new System.Drawing.Point(12, 211);
            this.txtSteps.Name = "txtSteps";
            this.txtSteps.Size = new System.Drawing.Size(775, 135);
            this.txtSteps.TabIndex = 0;
            this.txtSteps.Text = "";
            // 
            // cmdSubmit
            // 
            this.cmdSubmit.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(161)))), ((int)(((byte)(255)))));
            this.cmdSubmit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdSubmit.AutoSize = true;
            this.cmdSubmit.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(107)))), ((int)(((byte)(174)))));
            this.cmdSubmit.Location = new System.Drawing.Point(748, 349);
            this.cmdSubmit.Name = "cmdSubmit";
            this.cmdSubmit.Size = new System.Drawing.Size(39, 13);
            this.cmdSubmit.TabIndex = 6;
            this.cmdSubmit.TabStop = true;
            this.cmdSubmit.Text = "Submit";
            this.cmdSubmit.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.cmdSubmit_LinkClicked);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(14, 164);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(43, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Case Id";
            // 
            // txtCaseId
            // 
            this.txtCaseId.BackColor = System.Drawing.SystemColors.Window;
            this.txtCaseId.Location = new System.Drawing.Point(74, 161);
            this.txtCaseId.Name = "txtCaseId";
            this.txtCaseId.ReadOnly = true;
            this.txtCaseId.Size = new System.Drawing.Size(207, 20);
            this.txtCaseId.TabIndex = 3;
            // 
            // frmAnalyzer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(800, 584);
            this.Controls.Add(this.cmdSubmit);
            this.Controls.Add(this.cmdViewPolicy);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.cmdOpenDumpFile);
            this.Controls.Add(this.cmdOpenLog);
            this.Controls.Add(this.txtApplication);
            this.Controls.Add(this.txtCaseId);
            this.Controls.Add(this.txtError);
            this.Controls.Add(this.cmdClose);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtSteps);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtDetails);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblMessage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAnalyzer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Hydra Crash Analyzer";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmAnalyzer_FormClosing);
            this.Load += new System.EventHandler(this.frmAnalyzer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.RichTextBox txtDetails;
        private System.Windows.Forms.Button cmdClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtError;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtApplication;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.LinkLabel cmdOpenLog;
        private System.Windows.Forms.LinkLabel cmdOpenDumpFile;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.LinkLabel cmdViewPolicy;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RichTextBox txtSteps;
        private System.Windows.Forms.LinkLabel cmdSubmit;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtCaseId;
    }
}