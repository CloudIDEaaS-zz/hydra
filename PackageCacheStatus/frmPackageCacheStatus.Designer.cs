namespace PackageCacheStatus
{
    partial class frmPackageCacheStatus
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPackageCacheStatus));
            this.splitHorzContainer = new System.Windows.Forms.SplitContainer();
            this.richTextBoxStatus = new System.Windows.Forms.RichTextBox();
            this.richTextBoxSweep = new System.Windows.Forms.RichTextBox();
            this.splitVertContainer = new System.Windows.Forms.SplitContainer();
            this.propertyGridStatus = new Utils.TitlePropertyGrid();
            this.listViewFolders = new Utils.VirtualListView();
            this.columnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderDateModified = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panelSweeps = new System.Windows.Forms.Panel();
            this.listBoxSweeps = new System.Windows.Forms.ListBox();
            this.lblCaption = new System.Windows.Forms.Label();
            this.timerStatus = new System.Windows.Forms.Timer(this.components);
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearCacheToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearWorkingFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteWorkingFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteSrcAndNodeModulesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearConfigPackagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteLogFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearSweepsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemPause = new System.Windows.Forms.ToolStripMenuItem();
            this.clearWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sweepsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.killNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.killBuildToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripAlert1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripAlert2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.splitHorzContainer)).BeginInit();
            this.splitHorzContainer.Panel1.SuspendLayout();
            this.splitHorzContainer.Panel2.SuspendLayout();
            this.splitHorzContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitVertContainer)).BeginInit();
            this.splitVertContainer.Panel1.SuspendLayout();
            this.splitVertContainer.Panel2.SuspendLayout();
            this.splitVertContainer.SuspendLayout();
            this.panelSweeps.SuspendLayout();
            this.contextMenu.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitHorzContainer
            // 
            this.splitHorzContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitHorzContainer.Location = new System.Drawing.Point(0, 24);
            this.splitHorzContainer.Name = "splitHorzContainer";
            // 
            // splitHorzContainer.Panel1
            // 
            this.splitHorzContainer.Panel1.Controls.Add(this.richTextBoxStatus);
            this.splitHorzContainer.Panel1.Controls.Add(this.richTextBoxSweep);
            // 
            // splitHorzContainer.Panel2
            // 
            this.splitHorzContainer.Panel2.Controls.Add(this.splitVertContainer);
            this.splitHorzContainer.Size = new System.Drawing.Size(983, 678);
            this.splitHorzContainer.SplitterDistance = 661;
            this.splitHorzContainer.TabIndex = 0;
            // 
            // richTextBoxStatus
            // 
            this.richTextBoxStatus.BackColor = System.Drawing.SystemColors.Window;
            this.richTextBoxStatus.Cursor = System.Windows.Forms.Cursors.Default;
            this.richTextBoxStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxStatus.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxStatus.Name = "richTextBoxStatus";
            this.richTextBoxStatus.ReadOnly = true;
            this.richTextBoxStatus.Size = new System.Drawing.Size(661, 678);
            this.richTextBoxStatus.TabIndex = 0;
            this.richTextBoxStatus.Text = "";
            this.richTextBoxStatus.WordWrap = false;
            // 
            // richTextBoxSweep
            // 
            this.richTextBoxSweep.BackColor = System.Drawing.Color.OldLace;
            this.richTextBoxSweep.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxSweep.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxSweep.Name = "richTextBoxSweep";
            this.richTextBoxSweep.Size = new System.Drawing.Size(661, 678);
            this.richTextBoxSweep.TabIndex = 1;
            this.richTextBoxSweep.Text = "";
            this.richTextBoxSweep.Visible = false;
            this.richTextBoxSweep.WordWrap = false;
            // 
            // splitVertContainer
            // 
            this.splitVertContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitVertContainer.Location = new System.Drawing.Point(0, 0);
            this.splitVertContainer.Name = "splitVertContainer";
            this.splitVertContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitVertContainer.Panel1
            // 
            this.splitVertContainer.Panel1.Controls.Add(this.propertyGridStatus);
            // 
            // splitVertContainer.Panel2
            // 
            this.splitVertContainer.Panel2.Controls.Add(this.listViewFolders);
            this.splitVertContainer.Panel2.Controls.Add(this.panelSweeps);
            this.splitVertContainer.Size = new System.Drawing.Size(318, 678);
            this.splitVertContainer.SplitterDistance = 389;
            this.splitVertContainer.TabIndex = 0;
            // 
            // propertyGridStatus
            // 
            this.propertyGridStatus.Title = "Cache Status";
            this.propertyGridStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGridStatus.Location = new System.Drawing.Point(0, 0);
            this.propertyGridStatus.Name = "propertyGridStatus";
            this.propertyGridStatus.PropertySort = System.Windows.Forms.PropertySort.CategorizedAlphabetical;
            this.propertyGridStatus.SelectedObject = null;
            this.propertyGridStatus.Size = new System.Drawing.Size(318, 389);
            this.propertyGridStatus.TabIndex = 3;
            this.propertyGridStatus.ToolbarVisible = true;
            // 
            // listViewFolders
            // 
            this.listViewFolders.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderName,
            this.columnHeaderDateModified,
            this.columnHeaderType});
            this.listViewFolders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewFolders.HideSelection = false;
            this.listViewFolders.Location = new System.Drawing.Point(0, 0);
            this.listViewFolders.Name = "listViewFolders";
            this.listViewFolders.Scrollable = false;
            this.listViewFolders.Size = new System.Drawing.Size(318, 168);
            this.listViewFolders.TabIndex = 0;
            this.listViewFolders.UseCompatibleStateImageBehavior = false;
            this.listViewFolders.View = System.Windows.Forms.View.Details;
            this.listViewFolders.DoubleClick += new System.EventHandler(this.listViewFolders_DoubleClick);
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "Name";
            this.columnHeaderName.Width = 103;
            // 
            // columnHeaderDateModified
            // 
            this.columnHeaderDateModified.Text = "Date modified";
            this.columnHeaderDateModified.Width = 123;
            // 
            // columnHeaderType
            // 
            this.columnHeaderType.Text = "Type";
            this.columnHeaderType.Width = 82;
            // 
            // panelSweeps
            // 
            this.panelSweeps.Controls.Add(this.listBoxSweeps);
            this.panelSweeps.Controls.Add(this.lblCaption);
            this.panelSweeps.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelSweeps.Location = new System.Drawing.Point(0, 168);
            this.panelSweeps.Name = "panelSweeps";
            this.panelSweeps.Size = new System.Drawing.Size(318, 117);
            this.panelSweeps.TabIndex = 1;
            this.panelSweeps.Visible = false;
            // 
            // listBoxSweeps
            // 
            this.listBoxSweeps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxSweeps.FormattingEnabled = true;
            this.listBoxSweeps.Location = new System.Drawing.Point(0, 21);
            this.listBoxSweeps.Name = "listBoxSweeps";
            this.listBoxSweeps.Size = new System.Drawing.Size(318, 96);
            this.listBoxSweeps.TabIndex = 2;
            this.listBoxSweeps.SelectedIndexChanged += new System.EventHandler(this.listBoxSweeps_SelectedIndexChanged);
            this.listBoxSweeps.Leave += new System.EventHandler(this.listBoxSweeps_Leave);
            // 
            // lblCaption
            // 
            this.lblCaption.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.lblCaption.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCaption.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCaption.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.lblCaption.Location = new System.Drawing.Point(0, 0);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.Size = new System.Drawing.Size(318, 21);
            this.lblCaption.TabIndex = 1;
            this.lblCaption.Text = "Sweeps";
            this.lblCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // timerStatus
            // 
            this.timerStatus.Tick += new System.EventHandler(this.timerStatus_Tick);
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.contextMenu;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "Package Cache Status";
            this.notifyIcon.Visible = true;
            this.notifyIcon.DoubleClick += new System.EventHandler(this.notifyIcon_DoubleClick);
            this.notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseClick);
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(104, 54);
            // 
            // showToolStripMenuItem
            // 
            this.showToolStripMenuItem.Name = "showToolStripMenuItem";
            this.showToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.showToolStripMenuItem.Text = "&Show";
            this.showToolStripMenuItem.Click += new System.EventHandler(this.showToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(100, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // menuStrip
            // 
            this.menuStrip.BackColor = System.Drawing.Color.Transparent;
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(983, 24);
            this.menuStrip.TabIndex = 2;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearCacheToolStripMenuItem,
            this.clearWorkingFolderToolStripMenuItem,
            this.deleteWorkingFolderToolStripMenuItem,
            this.deleteSrcAndNodeModulesToolStripMenuItem,
            this.clearConfigPackagesToolStripMenuItem,
            this.deleteLogFileToolStripMenuItem,
            this.clearSweepsToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem1});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // clearCacheToolStripMenuItem
            // 
            this.clearCacheToolStripMenuItem.Enabled = false;
            this.clearCacheToolStripMenuItem.Name = "clearCacheToolStripMenuItem";
            this.clearCacheToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.clearCacheToolStripMenuItem.Text = "&Clear Cache";
            this.clearCacheToolStripMenuItem.Click += new System.EventHandler(this.clearCacheToolStripMenuItem_Click);
            // 
            // clearWorkingFolderToolStripMenuItem
            // 
            this.clearWorkingFolderToolStripMenuItem.Enabled = false;
            this.clearWorkingFolderToolStripMenuItem.Name = "clearWorkingFolderToolStripMenuItem";
            this.clearWorkingFolderToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.clearWorkingFolderToolStripMenuItem.Text = "Clear &Working Folder";
            this.clearWorkingFolderToolStripMenuItem.Click += new System.EventHandler(this.clearWorkingFolderToolStripMenuItem_Click);
            // 
            // deleteWorkingFolderToolStripMenuItem
            // 
            this.deleteWorkingFolderToolStripMenuItem.Enabled = false;
            this.deleteWorkingFolderToolStripMenuItem.Name = "deleteWorkingFolderToolStripMenuItem";
            this.deleteWorkingFolderToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.deleteWorkingFolderToolStripMenuItem.Text = "&Delete Working Folder";
            this.deleteWorkingFolderToolStripMenuItem.Click += new System.EventHandler(this.deleteWorkingFolderToolStripMenuItem_Click);
            // 
            // deleteSrcAndNodeModulesToolStripMenuItem
            // 
            this.deleteSrcAndNodeModulesToolStripMenuItem.Enabled = false;
            this.deleteSrcAndNodeModulesToolStripMenuItem.Name = "deleteSrcAndNodeModulesToolStripMenuItem";
            this.deleteSrcAndNodeModulesToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.deleteSrcAndNodeModulesToolStripMenuItem.Text = "Delete &src and node_modules";
            this.deleteSrcAndNodeModulesToolStripMenuItem.Click += new System.EventHandler(this.deleteSrcAndNodeModulesToolStripMenuItem_Click);
            // 
            // clearConfigPackagesToolStripMenuItem
            // 
            this.clearConfigPackagesToolStripMenuItem.Enabled = false;
            this.clearConfigPackagesToolStripMenuItem.Name = "clearConfigPackagesToolStripMenuItem";
            this.clearConfigPackagesToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.clearConfigPackagesToolStripMenuItem.Text = "Clear Config &Packages";
            this.clearConfigPackagesToolStripMenuItem.Click += new System.EventHandler(this.clearConfigPackagesToolStripMenuItem_Click);
            // 
            // deleteLogFileToolStripMenuItem
            // 
            this.deleteLogFileToolStripMenuItem.Name = "deleteLogFileToolStripMenuItem";
            this.deleteLogFileToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.deleteLogFileToolStripMenuItem.Text = "Delete &Log File";
            this.deleteLogFileToolStripMenuItem.Click += new System.EventHandler(this.deleteLogFileToolStripMenuItem_Click);
            // 
            // clearSweepsToolStripMenuItem
            // 
            this.clearSweepsToolStripMenuItem.Name = "clearSweepsToolStripMenuItem";
            this.clearSweepsToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.clearSweepsToolStripMenuItem.Text = "Clear Sw&eeps";
            this.clearSweepsToolStripMenuItem.Click += new System.EventHandler(this.clearSweepsToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(226, 6);
            // 
            // exitToolStripMenuItem1
            // 
            this.exitToolStripMenuItem1.Name = "exitToolStripMenuItem1";
            this.exitToolStripMenuItem1.Size = new System.Drawing.Size(229, 22);
            this.exitToolStripMenuItem1.Text = "E&xit";
            this.exitToolStripMenuItem1.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemPause,
            this.clearWindowToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // toolStripMenuItemPause
            // 
            this.toolStripMenuItemPause.CheckOnClick = true;
            this.toolStripMenuItemPause.Name = "toolStripMenuItemPause";
            this.toolStripMenuItemPause.Size = new System.Drawing.Size(148, 22);
            this.toolStripMenuItemPause.Text = "&Pause";
            this.toolStripMenuItemPause.Click += new System.EventHandler(this.toolStripMenuItemPause_Click);
            // 
            // clearWindowToolStripMenuItem
            // 
            this.clearWindowToolStripMenuItem.Name = "clearWindowToolStripMenuItem";
            this.clearWindowToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.clearWindowToolStripMenuItem.Text = "&Clear Window";
            this.clearWindowToolStripMenuItem.Click += new System.EventHandler(this.clearWindowToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sweepsToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // sweepsToolStripMenuItem
            // 
            this.sweepsToolStripMenuItem.Name = "sweepsToolStripMenuItem";
            this.sweepsToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
            this.sweepsToolStripMenuItem.Text = "&Sweeps";
            this.sweepsToolStripMenuItem.Click += new System.EventHandler(this.sweepsToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.killNodeToolStripMenuItem,
            this.killBuildToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // killNodeToolStripMenuItem
            // 
            this.killNodeToolStripMenuItem.Name = "killNodeToolStripMenuItem";
            this.killNodeToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.killNodeToolStripMenuItem.Text = "&Kill Node";
            this.killNodeToolStripMenuItem.Click += new System.EventHandler(this.killNodeToolStripMenuItem_Click);
            // 
            // killBuildToolStripMenuItem
            // 
            this.killBuildToolStripMenuItem.Name = "killBuildToolStripMenuItem";
            this.killBuildToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.K)));
            this.killBuildToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.killBuildToolStripMenuItem.Text = "Kill &Build";
            this.killBuildToolStripMenuItem.Click += new System.EventHandler(this.killBuildToolStripMenuItem_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.BackColor = System.Drawing.Color.Transparent;
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatus,
            this.toolStripAlert1,
            this.toolStripAlert2,
            this.progressBar});
            this.statusStrip.Location = new System.Drawing.Point(0, 702);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(983, 22);
            this.statusStrip.TabIndex = 3;
            this.statusStrip.Text = "statusStrip";
            // 
            // toolStripStatus
            // 
            this.toolStripStatus.Name = "toolStripStatus";
            this.toolStripStatus.Size = new System.Drawing.Size(960, 17);
            this.toolStripStatus.Spring = true;
            this.toolStripStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripAlert1
            // 
            this.toolStripAlert1.BackColor = System.Drawing.Color.LightCoral;
            this.toolStripAlert1.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.toolStripAlert1.Name = "toolStripAlert1";
            this.toolStripAlert1.Size = new System.Drawing.Size(4, 17);
            // 
            // toolStripAlert2
            // 
            this.toolStripAlert2.BackColor = System.Drawing.Color.LightCoral;
            this.toolStripAlert2.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.toolStripAlert2.Name = "toolStripAlert2";
            this.toolStripAlert2.Size = new System.Drawing.Size(4, 17);
            // 
            // progressBar
            // 
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(100, 16);
            this.progressBar.Visible = false;
            // 
            // frmPackageCacheStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(983, 724);
            this.Controls.Add(this.splitHorzContainer);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmPackageCacheStatus";
            this.Text = "Package Cache Status";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmPackageCacheStatus_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmPackageCacheStatus_FormClosed);
            this.Load += new System.EventHandler(this.frmPackageCacheStatus_Load);
            this.splitHorzContainer.Panel1.ResumeLayout(false);
            this.splitHorzContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitHorzContainer)).EndInit();
            this.splitHorzContainer.ResumeLayout(false);
            this.splitVertContainer.Panel1.ResumeLayout(false);
            this.splitVertContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitVertContainer)).EndInit();
            this.splitVertContainer.ResumeLayout(false);
            this.panelSweeps.ResumeLayout(false);
            this.contextMenu.ResumeLayout(false);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitHorzContainer;
        private System.Windows.Forms.RichTextBox richTextBoxStatus;
        private System.Windows.Forms.Timer timerStatus;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearCacheToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteWorkingFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteSrcAndNodeModulesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatus;
        private System.Windows.Forms.SplitContainer splitVertContainer;
        private Utils.TitlePropertyGrid propertyGridStatus;
        private Utils.VirtualListView listViewFolders;
        public System.Windows.Forms.ColumnHeader columnHeaderName;
        public System.Windows.Forms.ColumnHeader columnHeaderDateModified;
        private System.Windows.Forms.ColumnHeader columnHeaderType;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemPause;
        private System.Windows.Forms.ToolStripMenuItem clearWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearWorkingFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem killNodeToolStripMenuItem;
        private System.Windows.Forms.Panel panelSweeps;
        private System.Windows.Forms.Label lblCaption;
        private System.Windows.Forms.ListBox listBoxSweeps;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sweepsToolStripMenuItem;
        private System.Windows.Forms.RichTextBox richTextBoxSweep;
        private System.Windows.Forms.ToolStripMenuItem killBuildToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteLogFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearSweepsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearConfigPackagesToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripAlert1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripAlert2;
    }
}

