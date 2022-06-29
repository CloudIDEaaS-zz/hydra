using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace AddinExpress.Installer.WiXDesigner
{
	public class FormAssemblyFiles : Form
	{
		private int _imageIndexOffset;

		private IContainer components;

		private Panel panelTree;

		private Panel panelButtons;

		private ImageList imageList;

		private Button buttonOK;

		private Label labelText;

		private ListView listViewItems;

		private ColumnHeader columnHeader1;

		public FormAssemblyFiles()
		{
			this.InitializeComponent();
			System.Drawing.Font environmentFont = VsShellUtilities.GetEnvironmentFont(VsPackage.CurrentInstance);
			if (environmentFont != null)
			{
				this.Font = environmentFont;
			}
			VsShellUtilities.ApplyListViewThemeStyles(this.listViewItems);
			if (VsPackage.CurrentInstance.GetVSVersion() >= 11)
			{
				this._imageIndexOffset = 11;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		internal void Initialize(string text, List<string> files)
		{
			this.Text = "Files";
			this.labelText.Text = text;
			if (files != null)
			{
				foreach (string file in files)
				{
					this.listViewItems.Items.Add(file, 8 + this._imageIndexOffset);
				}
				this.listViewItems.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.None);
				this.listViewItems.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
				if (this.listViewItems.Columns[0].Width < this.listViewItems.Width - 22)
				{
					this.listViewItems.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.None);
					this.listViewItems.Columns[0].Width = this.listViewItems.Width - 22;
				}
			}
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(FormAssemblyFiles));
			this.panelTree = new Panel();
			this.listViewItems = new ListView();
			this.columnHeader1 = new ColumnHeader();
			this.imageList = new ImageList(this.components);
			this.labelText = new Label();
			this.panelButtons = new Panel();
			this.buttonOK = new Button();
			this.panelTree.SuspendLayout();
			this.panelButtons.SuspendLayout();
			base.SuspendLayout();
			this.panelTree.Controls.Add(this.listViewItems);
			this.panelTree.Controls.Add(this.labelText);
			this.panelTree.Dock = DockStyle.Fill;
			this.panelTree.Location = new Point(8, 8);
			this.panelTree.Margin = new System.Windows.Forms.Padding(2);
			this.panelTree.Name = "panelTree";
			this.panelTree.Size = new System.Drawing.Size(338, 193);
			this.panelTree.TabIndex = 0;
			this.listViewItems.Columns.AddRange(new ColumnHeader[] { this.columnHeader1 });
			this.listViewItems.Dock = DockStyle.Fill;
			this.listViewItems.HeaderStyle = ColumnHeaderStyle.None;
			this.listViewItems.Location = new Point(0, 36);
			this.listViewItems.MultiSelect = false;
			this.listViewItems.Name = "listViewItems";
			this.listViewItems.ShowGroups = false;
			this.listViewItems.Size = new System.Drawing.Size(338, 157);
			this.listViewItems.SmallImageList = this.imageList;
			this.listViewItems.Sorting = SortOrder.Ascending;
			this.listViewItems.TabIndex = 1;
			this.listViewItems.UseCompatibleStateImageBehavior = false;
			this.listViewItems.View = View.Details;
			this.columnHeader1.Width = 330;
			this.imageList.ImageStream = (ImageListStreamer)componentResourceManager.GetObject("imageList.ImageStream");
			this.imageList.TransparentColor = Color.White;
			this.imageList.Images.SetKeyName(0, "file-root.gif");
			this.imageList.Images.SetKeyName(1, "file-spec-folder.gif");
			this.imageList.Images.SetKeyName(2, "file-spec-folder1.gif");
			this.imageList.Images.SetKeyName(3, "file-folder.gif");
			this.imageList.Images.SetKeyName(4, "file-folder1.gif");
			this.imageList.Images.SetKeyName(5, "delete.gif");
			this.imageList.Images.SetKeyName(6, "properties.gif");
			this.imageList.Images.SetKeyName(7, "file-project-output.gif");
			this.imageList.Images.SetKeyName(8, "file-file.gif");
			this.imageList.Images.SetKeyName(9, "file-assembly.gif");
			this.imageList.Images.SetKeyName(10, "file-shortcut.gif");
			this.imageList.Images.SetKeyName(11, "file-root-13.png");
			this.imageList.Images.SetKeyName(12, "file-spec-folder-12.png");
			this.imageList.Images.SetKeyName(13, "file-spec-folder1-12.png");
			this.imageList.Images.SetKeyName(14, "file-folder-12.png");
			this.imageList.Images.SetKeyName(15, "file-folder1-12.png");
			this.imageList.Images.SetKeyName(16, "delete.gif");
			this.imageList.Images.SetKeyName(17, "properties-13.png");
			this.imageList.Images.SetKeyName(18, "file-project-output-13.png");
			this.imageList.Images.SetKeyName(19, "file-file-13.png");
			this.imageList.Images.SetKeyName(20, "file-assembly-13.png");
			this.imageList.Images.SetKeyName(21, "file-shortcut-13.png");
			this.labelText.Dock = DockStyle.Top;
			this.labelText.Location = new Point(0, 0);
			this.labelText.Name = "labelText";
			this.labelText.Size = new System.Drawing.Size(338, 36);
			this.labelText.TabIndex = 0;
			this.panelButtons.Controls.Add(this.buttonOK);
			this.panelButtons.Dock = DockStyle.Bottom;
			this.panelButtons.Location = new Point(8, 201);
			this.panelButtons.Margin = new System.Windows.Forms.Padding(2);
			this.panelButtons.Name = "panelButtons";
			this.panelButtons.Padding = new System.Windows.Forms.Padding(0, 7, 0, 2);
			this.panelButtons.Size = new System.Drawing.Size(338, 33);
			this.panelButtons.TabIndex = 1;
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Dock = DockStyle.Right;
			this.buttonOK.Location = new Point(252, 7);
			this.buttonOK.Margin = new System.Windows.Forms.Padding(2);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(86, 24);
			this.buttonOK.TabIndex = 0;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			base.AcceptButton = this.buttonOK;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.buttonOK;
			base.ClientSize = new System.Drawing.Size(354, 242);
			base.Controls.Add(this.panelTree);
			base.Controls.Add(this.panelButtons);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			base.Margin = new System.Windows.Forms.Padding(2);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "FormAssemblyFiles";
			base.Padding = new System.Windows.Forms.Padding(8);
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Files";
			this.panelTree.ResumeLayout(false);
			this.panelButtons.ResumeLayout(false);
			base.ResumeLayout(false);
		}
	}
}