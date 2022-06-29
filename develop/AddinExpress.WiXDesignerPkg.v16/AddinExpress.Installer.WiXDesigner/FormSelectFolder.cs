using Microsoft.VisualStudio.Shell;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace AddinExpress.Installer.WiXDesigner
{
	public class FormSelectFolder : Form
	{
		private VSBaseFolder _selectedFolder;

		private int _imageIndexOffset;

		private IContainer components;

		private Panel panelTree;

		private Panel panelButtons;

		private TreeViewEditor treeFileSystemEditor;

		private ImageList imageList;

		private Button buttonCancel;

		private Button buttonOK;

		internal VSBaseFolder SelectedFolder
		{
			get
			{
				return this._selectedFolder;
			}
		}

		public FormSelectFolder()
		{
			this.InitializeComponent();
			System.Drawing.Font environmentFont = VsShellUtilities.GetEnvironmentFont(VsPackage.CurrentInstance);
			if (environmentFont != null)
			{
				this.Font = environmentFont;
			}
			VsShellUtilities.ApplyTreeViewThemeStyles(this.treeFileSystemEditor);
			if (VsPackage.CurrentInstance.GetVSVersion() >= 11)
			{
				this._imageIndexOffset = 11;
			}
		}

		private void BuildTree(VSFolders folders, TreeNodeCollection nodes)
		{
			for (int i = 0; i < folders.Count; i++)
			{
				TreeNode item = null;
				item = (folders[i] is VSSpecialFolder || folders[i] is VSApplicationFolder ? new TreeNode(folders[i].Name, 1 + this._imageIndexOffset, 2 + this._imageIndexOffset) : new TreeNode(folders[i].Name, 3 + this._imageIndexOffset, 4 + this._imageIndexOffset));
				item.Name = string.Concat("Node", folders[i].WiXElement.Id);
				item.Tag = folders[i];
				nodes.Add(item);
				if (folders[i].Folders.Count > 0)
				{
					this.BuildTree(folders[i].Folders, item.Nodes);
				}
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

		private TreeNode FindNode(TreeNodeCollection nodes, string key)
		{
			TreeNode treeNode;
			TreeNode item = nodes[key];
			if (item != null)
			{
				return item;
			}
			IEnumerator enumerator = nodes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					TreeNode current = (TreeNode)enumerator.Current;
					item = this.FindNode(current.Nodes, key);
					if (item == null)
					{
						continue;
					}
					treeNode = item;
					return treeNode;
				}
				return null;
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return treeNode;
		}

		internal void Initialize(VSFolders folders, VSBaseFolder currentFolder)
		{
			this.treeFileSystemEditor.StartLoadingXml();
			try
			{
				TreeNode treeNode = new TreeNode("File System on Target Machine", this._imageIndexOffset, this._imageIndexOffset)
				{
					Name = "NodeRoot"
				};
				this.treeFileSystemEditor.Nodes.Add(treeNode);
				this.BuildTree(folders, treeNode.Nodes);
				treeNode.Expand();
				this.treeFileSystemEditor.Sort();
				this.SelectNode(currentFolder);
			}
			finally
			{
				this.treeFileSystemEditor.EndLoadingXml();
			}
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(FormSelectFolder));
			this.panelTree = new Panel();
			this.treeFileSystemEditor = new TreeViewEditor();
			this.imageList = new ImageList(this.components);
			this.panelButtons = new Panel();
			this.buttonCancel = new Button();
			this.buttonOK = new Button();
			this.panelTree.SuspendLayout();
			this.panelButtons.SuspendLayout();
			base.SuspendLayout();
			this.panelTree.Controls.Add(this.treeFileSystemEditor);
			this.panelTree.Dock = DockStyle.Fill;
			this.panelTree.Location = new Point(8, 8);
			this.panelTree.Margin = new System.Windows.Forms.Padding(2);
			this.panelTree.Name = "panelTree";
			this.panelTree.Size = new System.Drawing.Size(348, 305);
			this.panelTree.TabIndex = 0;
			this.treeFileSystemEditor.Dock = DockStyle.Fill;
			this.treeFileSystemEditor.HideSelection = false;
			this.treeFileSystemEditor.HotTracking = true;
			this.treeFileSystemEditor.ImageIndex = 0;
			this.treeFileSystemEditor.ImageList = this.imageList;
			this.treeFileSystemEditor.LabelEdit = true;
			this.treeFileSystemEditor.Location = new Point(0, 0);
			this.treeFileSystemEditor.Name = "treeFileSystemEditor";
			this.treeFileSystemEditor.SelectedImageIndex = 0;
			this.treeFileSystemEditor.ShowLines = false;
			this.treeFileSystemEditor.ShowRootLines = false;
			this.treeFileSystemEditor.Size = new System.Drawing.Size(348, 305);
			this.treeFileSystemEditor.StatusString = null;
			this.treeFileSystemEditor.TabIndex = 3;
			this.treeFileSystemEditor.AfterSelect += new TreeViewEventHandler(this.treeFileSystemEditor_AfterSelect);
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
			this.panelButtons.Controls.Add(this.buttonCancel);
			this.panelButtons.Controls.Add(this.buttonOK);
			this.panelButtons.Dock = DockStyle.Bottom;
			this.panelButtons.Location = new Point(8, 313);
			this.panelButtons.Margin = new System.Windows.Forms.Padding(2);
			this.panelButtons.Name = "panelButtons";
			this.panelButtons.Size = new System.Drawing.Size(348, 41);
			this.panelButtons.TabIndex = 1;
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new Point(261, 10);
			this.buttonCancel.Margin = new System.Windows.Forms.Padding(2);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(86, 24);
			this.buttonCancel.TabIndex = 1;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new Point(171, 10);
			this.buttonOK.Margin = new System.Windows.Forms.Padding(2);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(86, 24);
			this.buttonOK.TabIndex = 0;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			base.AcceptButton = this.buttonOK;
			base.AutoScaleDimensions = new SizeF(96f, 96f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			base.CancelButton = this.buttonCancel;
			base.ClientSize = new System.Drawing.Size(364, 362);
			base.Controls.Add(this.panelTree);
			base.Controls.Add(this.panelButtons);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			base.Margin = new System.Windows.Forms.Padding(2);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "FormSelectFolder";
			base.Padding = new System.Windows.Forms.Padding(8);
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Select Folder";
			this.panelTree.ResumeLayout(false);
			this.panelButtons.ResumeLayout(false);
			base.ResumeLayout(false);
		}

		private void SelectNode(VSBaseFolder currentFolder)
		{
			TreeNode treeNode = this.FindNode(this.treeFileSystemEditor.Nodes, string.Concat("Node", currentFolder.WiXElement.Id));
			if (treeNode != null)
			{
				this.treeFileSystemEditor.SelectedNode = treeNode;
				this.treeFileSystemEditor.SelectedNode.EnsureVisible();
				this.treeFileSystemEditor_AfterSelect(this.treeFileSystemEditor, new TreeViewEventArgs(null, TreeViewAction.Unknown));
			}
		}

		private void treeFileSystemEditor_AfterSelect(object sender, TreeViewEventArgs e)
		{
			this.buttonOK.Enabled = (this.treeFileSystemEditor.SelectedNode == null ? false : this.treeFileSystemEditor.SelectedNode.Tag != null);
			if (this.treeFileSystemEditor.SelectedNode == null)
			{
				this._selectedFolder = null;
				return;
			}
			this._selectedFolder = this.treeFileSystemEditor.SelectedNode.Tag as VSBaseFolder;
		}
	}
}