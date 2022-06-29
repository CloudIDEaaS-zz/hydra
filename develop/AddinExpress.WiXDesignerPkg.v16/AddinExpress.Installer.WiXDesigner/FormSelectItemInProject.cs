using Microsoft.VisualStudio.Shell;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class FormSelectItemInProject : Form
	{
		private VSFolders _fileSystem;

		private ListViewColumnSorter _listViewSorter;

		private string _activeFilter = "*.*";

		private int _imageIndexOffset;

		private List<VSBinary> binaryList;

		private bool blockBuildView;

		private IContainer components;

		private Panel panelControls;

		private Panel panelButtons;

		private Button buttonCancel;

		private Button buttonOK;

		private Label labelLookIn;

		private ComboTreeBox comboBoxFileSystem;

		private ImageList imageList;

		private ListView listFileSystemEditor;

		private Button buttonUp;

		private TextBox textBoxSourcePath;

		private Label labelSourcePath;

		private ComboBox comboBoxFileTypes;

		private Label labelFileTypes;

		internal VSBaseFile SelectedFile
		{
			get
			{
				if (this.listFileSystemEditor.SelectedItems.Count <= 0 || this.listFileSystemEditor.SelectedItems[0].Tag == null)
				{
					return null;
				}
				return this.listFileSystemEditor.SelectedItems[0].Tag as VSBaseFile;
			}
		}

		internal VSBaseFolder SelectedFolder
		{
			get
			{
				if (this.comboBoxFileSystem.SelectedNode == null || this.comboBoxFileSystem.SelectedNode.Tag == null)
				{
					return null;
				}
				return this.comboBoxFileSystem.SelectedNode.Tag as VSBaseFolder;
			}
		}

		internal string SelectedItem
		{
			get
			{
				string str = string.Concat("[", (this.comboBoxFileSystem.SelectedNode.Tag as VSBaseFolder).WiXElement.Id, "]");
				if (this.listFileSystemEditor.SelectedItems[0].Tag is VSProjectOutputVDProj)
				{
					return string.Concat(str, (this.listFileSystemEditor.SelectedItems[0].Tag as VSProjectOutputVDProj).KeyOutput.TargetName);
				}
				if (this.listFileSystemEditor.SelectedItems[0].Tag is VSProjectOutputFile)
				{
					return string.Concat(str, (this.listFileSystemEditor.SelectedItems[0].Tag as VSProjectOutputFile).TargetName);
				}
				if (this.listFileSystemEditor.SelectedItems[0].Tag is VSFile)
				{
					return string.Concat(str, (this.listFileSystemEditor.SelectedItems[0].Tag as VSFile).TargetName);
				}
				if (this.listFileSystemEditor.SelectedItems[0].Tag is VSAssembly)
				{
					return string.Concat(str, (this.listFileSystemEditor.SelectedItems[0].Tag as VSAssembly).Name);
				}
				if (!(this.listFileSystemEditor.SelectedItems[0].Tag is VSBinary))
				{
					return string.Empty;
				}
				return string.Concat(str, Path.GetFileName((this.listFileSystemEditor.SelectedItems[0].Tag as VSBinary).SourcePath));
			}
		}

		public FormSelectItemInProject()
		{
			this.InitializeComponent();
			if (!base.DesignMode)
			{
				System.Drawing.Font environmentFont = VsShellUtilities.GetEnvironmentFont(VsPackage.CurrentInstance);
				if (environmentFont != null)
				{
					this.Font = environmentFont;
				}
				VsShellUtilities.ApplyListViewThemeStyles(this.listFileSystemEditor);
				this._listViewSorter = new ListViewColumnSorter();
				this.listFileSystemEditor.ListViewItemSorter = this._listViewSorter;
				if (VsPackage.CurrentInstance.GetVSVersion() >= 11)
				{
					this._imageIndexOffset = 12;
				}
				this.buttonUp.ImageIndex = 11 + this._imageIndexOffset;
			}
		}

		private void BuildTree(VSFolders folders, ComboTreeNodeCollection nodes)
		{
			for (int i = 0; i < folders.Count; i++)
			{
				ComboTreeNode item = null;
				item = (folders[i] is VSSpecialFolder || folders[i] is VSApplicationFolder ? new ComboTreeNode()
				{
					Text = folders[i].Name,
					ImageIndex = 1 + this._imageIndexOffset,
					ExpandedImageIndex = 2 + this._imageIndexOffset
				} : new ComboTreeNode()
				{
					Text = folders[i].Name,
					ImageIndex = 3 + this._imageIndexOffset,
					ExpandedImageIndex = 4 + this._imageIndexOffset
				});
				item.Name = string.Concat("Node", folders[i].WiXElement.Id);
				item.Tag = folders[i];
				nodes.Add(item);
				if (folders[i].Folders.Count > 0)
				{
					this.BuildTree(folders[i].Folders, item.Nodes);
				}
			}
		}

		private void BuildView(ComboTreeNode selectedNode)
		{
			ListViewItem item;
			if (this.blockBuildView)
			{
				return;
			}
			this.listFileSystemEditor.BeginUpdate();
			try
			{
				this.listFileSystemEditor.Items.Clear();
				if (selectedNode != null)
				{
					VSBaseFolder tag = selectedNode.Tag as VSBaseFolder;
					VSFolders vSFolder = null;
					vSFolder = (tag != null ? tag.Folders : this._fileSystem);
					for (int i = 0; i < vSFolder.Count; i++)
					{
						item = (vSFolder[i] is VSSpecialFolder || vSFolder[i] is VSApplicationFolder ? this.listFileSystemEditor.Items.Add(string.Concat("Item", vSFolder[i].WiXElement.Id), vSFolder[i].Name, 1 + this._imageIndexOffset) : this.listFileSystemEditor.Items.Add(string.Concat("Item", vSFolder[i].WiXElement.Id), vSFolder[i].Name, 3 + this._imageIndexOffset));
						item.Tag = vSFolder[i];
					}
					if (tag != null)
					{
						foreach (VSComponent component in tag.Components)
						{
							foreach (VSBaseFile file in component.Files)
							{
								if (file is VSShortcut)
								{
									continue;
								}
								ListViewItem listViewItem = null;
								if (file is VSProjectOutputFile)
								{
									VsWiXProject.ReferenceDescriptor referenceDescriptor = (file as VSProjectOutputFile).ReferenceDescriptor;
									if (referenceDescriptor != null && referenceDescriptor.ReferencedProject != null && this.CheckFileTypes(referenceDescriptor.ReferencedProject.KeyOutput.TargetName))
									{
										listViewItem = this.listFileSystemEditor.Items.Add(string.Concat("Item", (file as VSProjectOutputFile).WiXElement.Id), (file as VSProjectOutputFile).Name, 7 + this._imageIndexOffset);
									}
								}
								else if (file is VSAssembly)
								{
									if (this.CheckFileTypes((file as VSAssembly).TargetName))
									{
										listViewItem = this.listFileSystemEditor.Items.Add(string.Concat("Item", (file as VSAssembly).WiXElement.Id), (file as VSAssembly).TargetName, 9 + this._imageIndexOffset);
									}
								}
								else if (file is VSFile && this.CheckFileTypes((file as VSFile).TargetName))
								{
									listViewItem = this.listFileSystemEditor.Items.Add(string.Concat("Item", (file as VSFile).WiXElement.Id), (file as VSFile).TargetName, 8 + this._imageIndexOffset);
								}
								if (listViewItem == null)
								{
									continue;
								}
								listViewItem.Tag = file;
							}
						}
						foreach (VSProjectOutputUnknown projectOutput in tag.ProjectOutputs)
						{
							if (!(projectOutput is VSProjectOutputVDProj) || (projectOutput as VSProjectOutputVDProj).Group != OutputGroup.Binaries || !this.CheckFileTypes((projectOutput as VSProjectOutputVDProj).KeyOutput.TargetName))
							{
								continue;
							}
							this.listFileSystemEditor.Items.Add(string.Concat("Item", (projectOutput as VSProjectOutputVDProj).WiXElement.Id), (projectOutput as VSProjectOutputVDProj).Name, 7 + this._imageIndexOffset).Tag = projectOutput;
						}
						if (this.binaryList != null && tag is VSApplicationFolder)
						{
							foreach (VSBinary vSBinary in this.binaryList)
							{
								string fileName = Path.GetFileName(vSBinary.SourcePath);
								if (!this.CheckFileTypes(fileName) || !(vSBinary.WiXElement.Id != "DefBannerBitmap"))
								{
									continue;
								}
								this.listFileSystemEditor.Items.Add(string.Concat("Item", vSBinary.WiXElement.Id), fileName, 7 + this._imageIndexOffset).Tag = vSBinary;
							}
						}
					}
					this.listFileSystemEditor.Sort();
				}
			}
			finally
			{
				this.listFileSystemEditor.EndUpdate();
			}
		}

		private void buttonUp_Click(object sender, EventArgs e)
		{
			this.comboBoxFileSystem.SelectedNode = this.comboBoxFileSystem.SelectedNode.Parent;
			this.buttonUp.Enabled = this.comboBoxFileSystem.SelectedNode.Parent != null;
			this.BuildView(this.comboBoxFileSystem.SelectedNode);
			this.listFileSystemEditor.Focus();
			this.textBoxSourcePath.Text = string.Empty;
			this.buttonOK.Enabled = false;
		}

		private bool CheckFileTypes(string fileName)
		{
			if (fileName == null)
			{
				return false;
			}
			if (this._activeFilter == "*.*")
			{
				return true;
			}
			if (!this._activeFilter.Contains(";"))
			{
				string lower = this._activeFilter.Substring(this._activeFilter.LastIndexOf(".")).ToLower();
				return fileName.ToLower().EndsWith(lower);
			}
			string[] strArrays = this._activeFilter.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
			if (strArrays == null || strArrays.Length == 0)
			{
				return false;
			}
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				strArrays[i] = strArrays[i].Substring(strArrays[i].LastIndexOf(".")).ToLower();
			}
			return Array.Exists<string>(strArrays, (string x) => fileName.ToLower().EndsWith(x));
		}

		private void comboBoxFileTypes_SelectedIndexChanged(object sender, EventArgs e)
		{
			this._activeFilter = this.comboBoxFileTypes.Text.Substring(this.comboBoxFileTypes.Text.IndexOf("(") + 1, this.comboBoxFileTypes.Text.IndexOf(")") - this.comboBoxFileTypes.Text.IndexOf("(") - 1);
			this.BuildView(this.comboBoxFileSystem.SelectedNode);
			this.textBoxSourcePath.Text = string.Empty;
			this.buttonOK.Enabled = false;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private ComboTreeNode FindNode(ComboTreeNodeCollection nodes, string key)
		{
			ComboTreeNode comboTreeNode;
			ComboTreeNode item = nodes[key];
			if (item != null)
			{
				return item;
			}
			using (IEnumerator<ComboTreeNode> enumerator = nodes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					item = this.FindNode(enumerator.Current.Nodes, key);
					if (item == null)
					{
						continue;
					}
					comboTreeNode = item;
					return comboTreeNode;
				}
				return null;
			}
			return comboTreeNode;
		}

		internal void Initialize(VSBaseFolder folder, string fileName, VSFolders fileSystem, List<VSBinary> binaryList, string[] filters)
		{
			this.binaryList = binaryList;
			this.Initialize(folder, fileName, fileSystem, filters);
		}

		internal void Initialize(VSBaseFolder folder, string fileName, VSFolders fileSystem, string[] filters)
		{
			this._fileSystem = fileSystem;
			if (filters.Length == 0)
			{
				this.comboBoxFileTypes.Items.Add("All Files (*.*)");
			}
			else
			{
				for (int i = 0; i < (int)filters.Length; i++)
				{
					this.comboBoxFileTypes.Items.Add(filters[i]);
				}
			}
			ComboTreeNode comboTreeNode = new ComboTreeNode()
			{
				Text = "File System on Target Machine",
				Name = "NodeRoot",
				ImageIndex = this._imageIndexOffset,
				ExpandedImageIndex = this._imageIndexOffset
			};
			this.comboBoxFileSystem.Nodes.Add(comboTreeNode);
			this.BuildTree(fileSystem, comboTreeNode.Nodes);
			this.comboBoxFileSystem.Images = this.imageList;
			this.comboBoxFileSystem.Sort();
			this.comboBoxFileSystem.ExpandAll();
			this.comboBoxFileSystem.SelectedNodeChanged += new EventHandler(this.TreeView_NodeMouseClick);
			this.comboBoxFileSystem.KeyDown += new KeyEventHandler(this.TreeView_KeyDown);
			this.blockBuildView = true;
			if (folder == null)
			{
				this.buttonUp.Enabled = false;
				this.comboBoxFileSystem.SelectedNode = comboTreeNode;
			}
			else
			{
				this.SelectNode(folder);
			}
			this.blockBuildView = false;
			this.comboBoxFileTypes.SelectedIndex = 0;
			if (this.listFileSystemEditor.Items.Count > 0 && !string.IsNullOrEmpty(fileName))
			{
				foreach (ListViewItem item in this.listFileSystemEditor.Items)
				{
					if (item.Text != fileName)
					{
						continue;
					}
					item.Selected = true;
					this.UpdateFilePath(item);
					return;
				}
			}
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(FormSelectItemInProject));
			this.panelControls = new Panel();
			this.comboBoxFileTypes = new ComboBox();
			this.labelFileTypes = new Label();
			this.textBoxSourcePath = new TextBox();
			this.labelSourcePath = new Label();
			this.listFileSystemEditor = new ListView();
			this.imageList = new ImageList(this.components);
			this.buttonUp = new Button();
			this.comboBoxFileSystem = new ComboTreeBox();
			this.labelLookIn = new Label();
			this.panelButtons = new Panel();
			this.buttonCancel = new Button();
			this.buttonOK = new Button();
			this.panelControls.SuspendLayout();
			this.panelButtons.SuspendLayout();
			base.SuspendLayout();
			this.panelControls.Controls.Add(this.comboBoxFileTypes);
			this.panelControls.Controls.Add(this.labelFileTypes);
			this.panelControls.Controls.Add(this.textBoxSourcePath);
			this.panelControls.Controls.Add(this.labelSourcePath);
			this.panelControls.Controls.Add(this.listFileSystemEditor);
			this.panelControls.Controls.Add(this.buttonUp);
			this.panelControls.Controls.Add(this.comboBoxFileSystem);
			this.panelControls.Controls.Add(this.labelLookIn);
			this.panelControls.Dock = DockStyle.Fill;
			this.panelControls.Location = new Point(8, 8);
			this.panelControls.Name = "panelControls";
			this.panelControls.Size = new System.Drawing.Size(423, 270);
			this.panelControls.TabIndex = 0;
			this.comboBoxFileTypes.DropDownStyle = ComboBoxStyle.DropDownList;
			this.comboBoxFileTypes.FormattingEnabled = true;
			this.comboBoxFileTypes.Location = new Point(90, 241);
			this.comboBoxFileTypes.Name = "comboBoxFileTypes";
			this.comboBoxFileTypes.Size = new System.Drawing.Size(329, 21);
			this.comboBoxFileTypes.TabIndex = 7;
			this.comboBoxFileTypes.SelectedIndexChanged += new EventHandler(this.comboBoxFileTypes_SelectedIndexChanged);
			this.labelFileTypes.AutoSize = true;
			this.labelFileTypes.Location = new Point(3, 244);
			this.labelFileTypes.Name = "labelFileTypes";
			this.labelFileTypes.Size = new System.Drawing.Size(66, 13);
			this.labelFileTypes.TabIndex = 6;
			this.labelFileTypes.Text = "Files of &type:";
			this.textBoxSourcePath.Location = new Point(90, 215);
			this.textBoxSourcePath.Name = "textBoxSourcePath";
			this.textBoxSourcePath.ReadOnly = true;
			this.textBoxSourcePath.Size = new System.Drawing.Size(329, 20);
			this.textBoxSourcePath.TabIndex = 5;
			this.labelSourcePath.AutoSize = true;
			this.labelSourcePath.Location = new Point(3, 218);
			this.labelSourcePath.Name = "labelSourcePath";
			this.labelSourcePath.Size = new System.Drawing.Size(68, 13);
			this.labelSourcePath.TabIndex = 4;
			this.labelSourcePath.Text = "&Source path:";
			this.listFileSystemEditor.HideSelection = false;
			this.listFileSystemEditor.Location = new Point(0, 50);
			this.listFileSystemEditor.MultiSelect = false;
			this.listFileSystemEditor.Name = "listFileSystemEditor";
			this.listFileSystemEditor.ShowGroups = false;
			this.listFileSystemEditor.Size = new System.Drawing.Size(419, 153);
			this.listFileSystemEditor.SmallImageList = this.imageList;
			this.listFileSystemEditor.TabIndex = 3;
			this.listFileSystemEditor.UseCompatibleStateImageBehavior = false;
			this.listFileSystemEditor.View = View.List;
			this.listFileSystemEditor.SelectedIndexChanged += new EventHandler(this.listFileSystemEditor_SelectedIndexChanged);
			this.listFileSystemEditor.DoubleClick += new EventHandler(this.listFileSystemEditor_DoubleClick);
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
			this.imageList.Images.SetKeyName(11, "folder-up.gif");
			this.imageList.Images.SetKeyName(12, "file-root-13.png");
			this.imageList.Images.SetKeyName(13, "file-spec-folder-12.png");
			this.imageList.Images.SetKeyName(14, "file-spec-folder1-12.png");
			this.imageList.Images.SetKeyName(15, "file-folder-12.png");
			this.imageList.Images.SetKeyName(16, "file-folder1-12.png");
			this.imageList.Images.SetKeyName(17, "delete.gif");
			this.imageList.Images.SetKeyName(18, "properties-13.png");
			this.imageList.Images.SetKeyName(19, "file-project-output-13.png");
			this.imageList.Images.SetKeyName(20, "file-file-13.png");
			this.imageList.Images.SetKeyName(21, "file-assembly-13.png");
			this.imageList.Images.SetKeyName(22, "file-shortcut-13.png");
			this.imageList.Images.SetKeyName(23, "folder-up-12.png");
			this.buttonUp.ImageIndex = 11;
			this.buttonUp.ImageList = this.imageList;
			this.buttonUp.Location = new Point(393, 11);
			this.buttonUp.Name = "buttonUp";
			this.buttonUp.Size = new System.Drawing.Size(26, 23);
			this.buttonUp.TabIndex = 2;
			this.buttonUp.UseVisualStyleBackColor = true;
			this.buttonUp.Click += new EventHandler(this.buttonUp_Click);
			this.comboBoxFileSystem.DropDownHeight = 200;
			this.comboBoxFileSystem.Location = new Point(90, 12);
			this.comboBoxFileSystem.Name = "comboBoxFileSystem";
			this.comboBoxFileSystem.Size = new System.Drawing.Size(285, 21);
			this.comboBoxFileSystem.TabIndex = 1;
			this.labelLookIn.AutoSize = true;
			this.labelLookIn.Location = new Point(3, 15);
			this.labelLookIn.Name = "labelLookIn";
			this.labelLookIn.Size = new System.Drawing.Size(45, 13);
			this.labelLookIn.TabIndex = 0;
			this.labelLookIn.Text = "Look &in:";
			this.panelButtons.Controls.Add(this.buttonCancel);
			this.panelButtons.Controls.Add(this.buttonOK);
			this.panelButtons.Dock = DockStyle.Bottom;
			this.panelButtons.Location = new Point(8, 278);
			this.panelButtons.Name = "panelButtons";
			this.panelButtons.Size = new System.Drawing.Size(423, 41);
			this.panelButtons.TabIndex = 4;
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new Point(337, 9);
			this.buttonCancel.Margin = new System.Windows.Forms.Padding(2);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(86, 24);
			this.buttonCancel.TabIndex = 3;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Enabled = false;
			this.buttonOK.Location = new Point(247, 9);
			this.buttonOK.Margin = new System.Windows.Forms.Padding(2);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(86, 24);
			this.buttonOK.TabIndex = 2;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			base.AcceptButton = this.buttonOK;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.buttonCancel;
			base.ClientSize = new System.Drawing.Size(439, 327);
			base.Controls.Add(this.panelControls);
			base.Controls.Add(this.panelButtons);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "FormSelectItemInProject";
			base.Padding = new System.Windows.Forms.Padding(8);
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Select Item in Project";
			this.panelControls.ResumeLayout(false);
			this.panelControls.PerformLayout();
			this.panelButtons.ResumeLayout(false);
			base.ResumeLayout(false);
		}

		private void listFileSystemEditor_DoubleClick(object sender, EventArgs e)
		{
			if (this.listFileSystemEditor.FocusedItem != null && this.listFileSystemEditor.FocusedItem.Tag != null)
			{
				if (this.listFileSystemEditor.FocusedItem.Tag is VSBaseFolder)
				{
					this.SelectNode(this.listFileSystemEditor.FocusedItem.Tag as VSBaseFolder);
					this.BuildView(this.comboBoxFileSystem.SelectedNode);
					this.buttonUp.Enabled = this.comboBoxFileSystem.SelectedNode.Parent != null;
					return;
				}
				if (this.listFileSystemEditor.FocusedItem.Tag is VSProjectOutputVDProj)
				{
					if ((this.listFileSystemEditor.FocusedItem.Tag as VSProjectOutputVDProj).KeyOutput != null)
					{
						this.textBoxSourcePath.Text = (this.listFileSystemEditor.FocusedItem.Tag as VSProjectOutputVDProj).KeyOutput.SourcePath;
					}
					this.buttonOK.PerformClick();
					return;
				}
				if (this.listFileSystemEditor.FocusedItem.Tag is VSProjectOutputFile)
				{
					this.textBoxSourcePath.Text = (this.listFileSystemEditor.FocusedItem.Tag as VSProjectOutputFile).SourcePath;
					this.buttonOK.PerformClick();
					return;
				}
				if (this.listFileSystemEditor.FocusedItem.Tag is VSFile)
				{
					this.textBoxSourcePath.Text = (this.listFileSystemEditor.FocusedItem.Tag as VSFile).SourcePath;
					this.buttonOK.PerformClick();
					return;
				}
				if (this.listFileSystemEditor.FocusedItem.Tag is VSAssembly)
				{
					this.textBoxSourcePath.Text = (this.listFileSystemEditor.FocusedItem.Tag as VSAssembly).Name;
					this.buttonOK.PerformClick();
					return;
				}
				if (this.listFileSystemEditor.FocusedItem.Tag is VSBinary)
				{
					this.textBoxSourcePath.Text = (this.listFileSystemEditor.FocusedItem.Tag as VSBinary).GetFullPath();
					this.buttonOK.PerformClick();
				}
			}
		}

		private void listFileSystemEditor_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.textBoxSourcePath.Text = string.Empty;
			this.buttonOK.Enabled = false;
			if (this.listFileSystemEditor.SelectedItems.Count == 0)
			{
				return;
			}
			this.UpdateFilePath(this.listFileSystemEditor.FocusedItem);
		}

		private void SelectNode(VSBaseFolder currentFolder)
		{
			ComboTreeNode comboTreeNode = this.FindNode(this.comboBoxFileSystem.Nodes, string.Concat("Node", currentFolder.WiXElement.Id));
			if (comboTreeNode != null)
			{
				this.comboBoxFileSystem.SelectedNode = comboTreeNode;
			}
		}

		private void TreeView_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Alt || e.Control || e.Shift)
			{
				return;
			}
			if (e.KeyCode == Keys.Return)
			{
				e.Handled = true;
				this.buttonUp.Enabled = this.comboBoxFileSystem.SelectedNode.Parent != null;
				this.BuildView(this.comboBoxFileSystem.SelectedNode);
				base.Activate();
				this.listFileSystemEditor.Focus();
				this.textBoxSourcePath.Text = string.Empty;
				this.buttonOK.Enabled = false;
			}
		}

		private void TreeView_NodeMouseClick(object sender, EventArgs e)
		{
			this.buttonUp.Enabled = this.comboBoxFileSystem.SelectedNode.Parent != null;
			this.BuildView(this.comboBoxFileSystem.SelectedNode);
			base.Activate();
			this.listFileSystemEditor.Focus();
			this.textBoxSourcePath.Text = string.Empty;
			this.buttonOK.Enabled = false;
		}

		private void UpdateFilePath(ListViewItem listItem)
		{
			if (listItem != null && listItem.Tag != null)
			{
				if (listItem.Tag is VSProjectOutputVDProj)
				{
					if ((listItem.Tag as VSProjectOutputVDProj).KeyOutput != null)
					{
						this.textBoxSourcePath.Text = (listItem.Tag as VSProjectOutputVDProj).KeyOutput.SourcePath;
					}
				}
				else if (listItem.Tag is VSProjectOutputFile)
				{
					this.textBoxSourcePath.Text = (listItem.Tag as VSProjectOutputFile).SourcePath;
					VsWiXProject.ReferenceDescriptor referenceDescriptor = (listItem.Tag as VSProjectOutputFile).ReferenceDescriptor;
					if (referenceDescriptor != null && referenceDescriptor.ReferencedProject != null)
					{
						this.textBoxSourcePath.Text = referenceDescriptor.ReferencedProject.KeyOutput.SourcePath;
					}
				}
				else if (listItem.Tag is VSFile)
				{
					this.textBoxSourcePath.Text = (listItem.Tag as VSFile).SourcePath;
				}
				else if (listItem.Tag is VSAssembly)
				{
					this.textBoxSourcePath.Text = (listItem.Tag as VSAssembly).Name;
				}
				else if (listItem.Tag is VSBinary)
				{
					this.textBoxSourcePath.Text = (listItem.Tag as VSBinary).GetFullPath();
				}
				this.buttonOK.Enabled = !string.IsNullOrEmpty(this.textBoxSourcePath.Text);
			}
		}
	}
}