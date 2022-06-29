
using System.Windows.Forms;

namespace AbstraX
{
    partial class frmAppDesigner
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAppDesigner));

            this.SuspendLayout();
            // 
            // panel1
            // 
            this.ctrlAppDesigner.Location = new System.Drawing.Point(0, 0);
            this.ctrlAppDesigner.Name = "panel1";
            this.ctrlAppDesigner.Size = new System.Drawing.Size(800, 450);
            this.ctrlAppDesigner.TabIndex = 0;
            this.ctrlAppDesigner.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // frmAppDesigner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ctrlAppDesigner);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmAppDesigner";
            this.Text = "App Designer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmAppDesigner_FormClosing);
            this.Load += new System.EventHandler(this.frmAppDesigner_Load);
            this.ResumeLayout(false);   
            this.PerformLayout();
        }

        #endregion

        private ctrlAppDesigner ctrlAppDesigner;
    }
}