
namespace HydraDebugAssistant
{
    partial class HydraDebugToolWindowControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HydraDebugToolWindowControl));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btnSetBreakpoint = new System.Windows.Forms.ToolStripButton();
            this.btnContinue = new System.Windows.Forms.ToolStripButton();
            this.btnRetry = new System.Windows.Forms.ToolStripButton();
            this.btnHideButDontClose = new System.Windows.Forms.ToolStripButton();
            this.txtDirectory = new System.Windows.Forms.TextBox();
            this.treeView = new System.Windows.Forms.TreeView();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.setBreakpointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.continueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.retryAndBreakToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.toolStrip.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSetBreakpoint,
            this.btnContinue,
            this.btnRetry,
            this.btnHideButDontClose});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(460, 25);
            this.toolStrip.TabIndex = 0;
            this.toolStrip.Text = "toolStrip1";
            // 
            // btnSetBreakpoint
            // 
            this.btnSetBreakpoint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSetBreakpoint.Enabled = false;
            this.btnSetBreakpoint.Image = ((System.Drawing.Image)(resources.GetObject("btnSetBreakpoint.Image")));
            this.btnSetBreakpoint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSetBreakpoint.Name = "btnSetBreakpoint";
            this.btnSetBreakpoint.Size = new System.Drawing.Size(23, 22);
            this.btnSetBreakpoint.Text = "Set Breakpoint";
            this.btnSetBreakpoint.ToolTipText = "Set breakpoint on selected T4 file";
            this.btnSetBreakpoint.Click += new System.EventHandler(this.btnSetBreakpoint_Click);
            // 
            // btnContinue
            // 
            this.btnContinue.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnContinue.Enabled = false;
            this.btnContinue.Image = ((System.Drawing.Image)(resources.GetObject("btnContinue.Image")));
            this.btnContinue.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnContinue.Name = "btnContinue";
            this.btnContinue.Size = new System.Drawing.Size(23, 22);
            this.btnContinue.Text = "Continue";
            this.btnContinue.Click += new System.EventHandler(this.btnContinue_Click);
            // 
            // btnRetry
            // 
            this.btnRetry.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRetry.Enabled = false;
            this.btnRetry.Image = ((System.Drawing.Image)(resources.GetObject("btnRetry.Image")));
            this.btnRetry.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRetry.Name = "btnRetry";
            this.btnRetry.Size = new System.Drawing.Size(23, 22);
            this.btnRetry.Text = "Retry and Break";
            this.btnRetry.ToolTipText = "Retry and break";
            this.btnRetry.Click += new System.EventHandler(this.btnRetry_Click);
            // 
            // btnHideButDontClose
            // 
            this.btnHideButDontClose.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnHideButDontClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnHideButDontClose.Image = ((System.Drawing.Image)(resources.GetObject("btnHideButDontClose.Image")));
            this.btnHideButDontClose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnHideButDontClose.Name = "btnHideButDontClose";
            this.btnHideButDontClose.Size = new System.Drawing.Size(23, 22);
            this.btnHideButDontClose.Text = "Hide but don\'t close";
            this.btnHideButDontClose.Visible = false;
            this.btnHideButDontClose.Click += new System.EventHandler(this.btnHideButDontClose_Click);
            // 
            // txtDirectory
            // 
            this.txtDirectory.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtDirectory.Location = new System.Drawing.Point(0, 25);
            this.txtDirectory.Name = "txtDirectory";
            this.txtDirectory.Size = new System.Drawing.Size(460, 20);
            this.txtDirectory.TabIndex = 1;
            // 
            // treeView
            // 
            this.treeView.ContextMenuStrip = this.contextMenuStrip;
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.ImageIndex = 0;
            this.treeView.ImageList = this.imageList;
            this.treeView.Indent = 19;
            this.treeView.Location = new System.Drawing.Point(0, 45);
            this.treeView.Name = "treeView";
            this.treeView.SelectedImageIndex = 0;
            this.treeView.ShowRootLines = false;
            this.treeView.Size = new System.Drawing.Size(460, 549);
            this.treeView.TabIndex = 2;
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setBreakpointToolStripMenuItem,
            this.continueToolStripMenuItem,
            this.retryAndBreakToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(157, 70);
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            // 
            // setBreakpointToolStripMenuItem
            // 
            this.setBreakpointToolStripMenuItem.Image = global::HydraDebugAssistant.Properties.Resources.SetBreakpoint;
            this.setBreakpointToolStripMenuItem.Name = "setBreakpointToolStripMenuItem";
            this.setBreakpointToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.setBreakpointToolStripMenuItem.Text = "&Set Breakpoint";
            this.setBreakpointToolStripMenuItem.Click += new System.EventHandler(this.btnSetBreakpoint_Click);
            // 
            // continueToolStripMenuItem
            // 
            this.continueToolStripMenuItem.Image = global::HydraDebugAssistant.Properties.Resources.Continue;
            this.continueToolStripMenuItem.Name = "continueToolStripMenuItem";
            this.continueToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.continueToolStripMenuItem.Text = "&Continue";
            this.continueToolStripMenuItem.Click += new System.EventHandler(this.btnContinue_Click);
            // 
            // retryAndBreakToolStripMenuItem
            // 
            this.retryAndBreakToolStripMenuItem.Image = global::HydraDebugAssistant.Properties.Resources.RetryAndBreak;
            this.retryAndBreakToolStripMenuItem.Name = "retryAndBreakToolStripMenuItem";
            this.retryAndBreakToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.retryAndBreakToolStripMenuItem.Text = "&Retry and Break";
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "OpenFolderIcon.png");
            this.imageList.Images.SetKeyName(1, "ClosedFolderIcon.png");
            this.imageList.Images.SetKeyName(2, "T4Icon.png");
            // 
            // HydraDebugToolWindowControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeView);
            this.Controls.Add(this.txtDirectory);
            this.Controls.Add(this.toolStrip);
            this.Name = "HydraDebugToolWindowControl";
            this.Size = new System.Drawing.Size(460, 594);
            this.Load += new System.EventHandler(this.HydraDebugToolWindowControl_Load);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton btnSetBreakpoint;
        private System.Windows.Forms.TextBox txtDirectory;
        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ToolStripButton btnContinue;
        private System.Windows.Forms.ToolStripButton btnRetry;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem setBreakpointToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem continueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem retryAndBreakToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton btnHideButDontClose;
    }
}
