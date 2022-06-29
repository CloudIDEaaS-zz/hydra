using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace AddinExpress.Installer.WiXDesigner
{
	public class FormSelectIcon : Form
	{
		private VSShortcut _instanceShortcut;

		private VSFileType _instanceFileType;

		private string _selectedIcon = string.Empty;

		private string _listViewTag = "NEW_ICON";

		private bool directSelection;

		private string projectDir = string.Empty;

		private IContainer components;

		private Panel panelControls;

		private Panel panelButtons;

		private Button buttonCancel;

		private Button buttonOK;

		private Button buttonBrowse;

		private TextBox textBoxFileName;

		private Label labelFileName;

		private ListView listViewIcons;

		private ImageList imageListIcons;

		private Label labelIcons;

		private OpenFileDialog openFileDialog;

		public string SelectedIcon
		{
			get
			{
				return this._selectedIcon;
			}
			set
			{
				this._selectedIcon = value;
			}
		}

		public FormSelectIcon()
		{
			this.InitializeComponent();
			System.Drawing.Font environmentFont = VsShellUtilities.GetEnvironmentFont(VsPackage.CurrentInstance);
			if (environmentFont != null)
			{
				this.Font = environmentFont;
			}
			VsShellUtilities.ApplyListViewThemeStyles(this.listViewIcons);
		}

		internal FormSelectIcon(VSShortcut instance, string selectedIcon) : this()
		{
			int count;
			this._instanceShortcut = instance;
			this._selectedIcon = selectedIcon;
			for (int i = 0; i < this._instanceShortcut.Project.Icons.Count; i++)
			{
				WiXIcon item = this._instanceShortcut.Project.Icons[i];
				string empty = string.Empty;
				if (Common.FileExists(item.SourceFile, item.Owner as WiXProjectItem, out empty))
				{
					System.Drawing.Icon icon = new System.Drawing.Icon(empty);
					this.imageListIcons.Images.Add(item.Id, icon);
					count = i + 1;
					this.listViewIcons.Items.Add(item.Id, count.ToString(), i);
				}
			}
			if (!string.IsNullOrEmpty(selectedIcon))
			{
				string str = string.Empty;
				if (!Common.FileExists(selectedIcon, instance.WiXElement.Owner as WiXProjectItem, out str))
				{
					ListViewItem[] listViewItemArray = this.listViewIcons.Items.Find(selectedIcon, false);
					if (listViewItemArray != null && listViewItemArray.Length != 0)
					{
						listViewItemArray[0].Selected = true;
						listViewItemArray[0].Focused = true;
						this.buttonOK.Enabled = true;
					}
				}
				else
				{
					this.textBoxFileName.Text = selectedIcon;
					System.Drawing.Icon icon1 = new System.Drawing.Icon(str);
					this.imageListIcons.Images.Add(this._listViewTag, icon1);
					count = this.listViewIcons.Items.Count + 1;
					ListViewItem listViewItem = this.listViewIcons.Items.Add(this._listViewTag, count.ToString(), this.imageListIcons.Images.Count - 1);
					listViewItem.Tag = selectedIcon;
					listViewItem.Selected = true;
					listViewItem.Focused = true;
					this.buttonOK.Enabled = true;
				}
			}
			base.ActiveControl = this.listViewIcons;
		}

		internal FormSelectIcon(VSFileType instance, string selectedIcon) : this()
		{
			this._instanceFileType = instance;
			this._selectedIcon = selectedIcon;
			for (int i = 0; i < this._instanceFileType.Project.Icons.Count; i++)
			{
				WiXIcon item = this._instanceFileType.Project.Icons[i];
				string empty = string.Empty;
				if (Common.FileExists(item.SourceFile, item.Owner as WiXProjectItem, out empty))
				{
					System.Drawing.Icon icon = new System.Drawing.Icon(empty);
					this.imageListIcons.Images.Add(item.Id, icon);
					int num = i + 1;
					this.listViewIcons.Items.Add(item.Id, num.ToString(), i);
				}
			}
			if (!string.IsNullOrEmpty(selectedIcon))
			{
				ListViewItem[] listViewItemArray = this.listViewIcons.Items.Find(selectedIcon, false);
				if (listViewItemArray != null && listViewItemArray.Length != 0)
				{
					listViewItemArray[0].Selected = true;
					listViewItemArray[0].Focused = true;
					this.buttonOK.Enabled = true;
				}
			}
			base.ActiveControl = this.listViewIcons;
		}

		internal FormSelectIcon(string projectDir, string selectedIcon) : this()
		{
			this.directSelection = true;
			this.projectDir = projectDir;
			this._selectedIcon = selectedIcon;
			if (!string.IsNullOrEmpty(selectedIcon))
			{
				try
				{
					if (!Path.IsPathRooted(selectedIcon))
					{
						selectedIcon = Path.GetFullPath(Path.Combine(projectDir, selectedIcon));
					}
				}
				catch (Exception exception)
				{
				}
				if (File.Exists(selectedIcon))
				{
					System.Drawing.Icon icon = new System.Drawing.Icon(selectedIcon);
					this.imageListIcons.Images.Add("0", icon);
					this.listViewIcons.Items.Add("0", "1", 0);
					this.listViewIcons.Items[0].Tag = selectedIcon;
					this.listViewIcons.Items[0].Selected = true;
					this.listViewIcons.Items[0].Focused = true;
					this.listViewIcons_Click(this.listViewIcons, EventArgs.Empty);
				}
			}
			base.ActiveControl = this.listViewIcons;
		}

		private void buttonBrowse_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(this.SelectedIcon))
			{
				try
				{
					if (this.directSelection)
					{
						if (Path.IsPathRooted(this.SelectedIcon))
						{
							this.openFileDialog.InitialDirectory = Path.GetDirectoryName(this.SelectedIcon);
						}
						else
						{
							this.openFileDialog.InitialDirectory = Path.GetDirectoryName(Path.GetFullPath(Path.Combine(this.projectDir, this.SelectedIcon)));
						}
					}
				}
				catch (Exception exception)
				{
				}
			}
			if (this.openFileDialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK && File.Exists(this.openFileDialog.FileName))
			{
				if (this.directSelection)
				{
					this.textBoxFileName.Text = this.openFileDialog.FileName;
					this.imageListIcons.Images.Clear();
					this.listViewIcons.Items.Clear();
					this.imageListIcons.Images.Add("0", new System.Drawing.Icon(this.openFileDialog.FileName));
					ListViewItem text = this.listViewIcons.Items.Add("0", "1", 0);
					text.Tag = this.textBoxFileName.Text;
					text.Selected = true;
					text.Focused = true;
					this.listViewIcons_Click(this.listViewIcons, EventArgs.Empty);
					return;
				}
				if (this._instanceShortcut != null)
				{
					this.textBoxFileName.Text = Common.MakeRelativePath((this._instanceShortcut.WiXElement.Owner as WiXProjectItem).SourceFile, this.openFileDialog.FileName);
				}
				if (this._instanceFileType != null)
				{
					if (this._instanceFileType.WiXElement == null)
					{
						this.textBoxFileName.Text = Common.MakeRelativePath((this._instanceFileType.ParentOutput.WiXElement.Owner as WiXProjectItem).SourceFile, this.openFileDialog.FileName);
					}
					else
					{
						this.textBoxFileName.Text = Common.MakeRelativePath((this._instanceFileType.WiXElement.Owner as WiXProjectItem).SourceFile, this.openFileDialog.FileName);
					}
				}
				this.imageListIcons.Images.RemoveByKey(this._listViewTag);
				ListViewItem[] listViewItemArray = this.listViewIcons.Items.Find(this._listViewTag, false);
				if (listViewItemArray != null && listViewItemArray.Length != 0)
				{
					listViewItemArray[0].Remove();
				}
				WiXProjectParser project = null;
				if (this._instanceShortcut != null)
				{
					project = this._instanceShortcut.Project;
				}
				if (this._instanceFileType != null)
				{
					project = this._instanceFileType.Project;
				}
				if (project != null)
				{
					WiXIcon wiXIcon = project.Icons.Find((WiXIcon e1) => e1.SourceFile == this.textBoxFileName.Text);
					if (wiXIcon != null)
					{
						listViewItemArray = this.listViewIcons.Items.Find(wiXIcon.Id, false);
						if (listViewItemArray != null && listViewItemArray.Length != 0)
						{
							listViewItemArray[0].Selected = true;
							listViewItemArray[0].Focused = true;
							this.listViewIcons_Click(this.listViewIcons, EventArgs.Empty);
							return;
						}
					}
				}
				this.imageListIcons.Images.Add(this._listViewTag, new System.Drawing.Icon(this.openFileDialog.FileName));
				int count = this.listViewIcons.Items.Count + 1;
				ListViewItem listViewItem = this.listViewIcons.Items.Add(this._listViewTag, count.ToString(), this.imageListIcons.Images.Count - 1);
				listViewItem.Tag = this.textBoxFileName.Text;
				listViewItem.Selected = true;
				listViewItem.Focused = true;
				this.listViewIcons_Click(this.listViewIcons, EventArgs.Empty);
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

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.panelControls = new Panel();
			this.listViewIcons = new ListView();
			this.imageListIcons = new ImageList(this.components);
			this.labelIcons = new Label();
			this.buttonBrowse = new Button();
			this.textBoxFileName = new TextBox();
			this.labelFileName = new Label();
			this.panelButtons = new Panel();
			this.buttonCancel = new Button();
			this.buttonOK = new Button();
			this.openFileDialog = new OpenFileDialog();
			this.panelControls.SuspendLayout();
			this.panelButtons.SuspendLayout();
			base.SuspendLayout();
			this.panelControls.Controls.Add(this.listViewIcons);
			this.panelControls.Controls.Add(this.labelIcons);
			this.panelControls.Controls.Add(this.buttonBrowse);
			this.panelControls.Controls.Add(this.textBoxFileName);
			this.panelControls.Controls.Add(this.labelFileName);
			this.panelControls.Dock = DockStyle.Fill;
			this.panelControls.Location = new Point(8, 8);
			this.panelControls.Name = "panelControls";
			this.panelControls.Size = new System.Drawing.Size(393, 235);
			this.panelControls.TabIndex = 0;
			this.listViewIcons.LargeImageList = this.imageListIcons;
			this.listViewIcons.Location = new Point(6, 85);
			this.listViewIcons.MultiSelect = false;
			this.listViewIcons.Name = "listViewIcons";
			this.listViewIcons.Size = new System.Drawing.Size(387, 144);
			this.listViewIcons.TabIndex = 4;
			this.listViewIcons.UseCompatibleStateImageBehavior = false;
			this.listViewIcons.Click += new EventHandler(this.listViewIcons_Click);
			this.listViewIcons.DoubleClick += new EventHandler(this.listViewIcons_DoubleClick);
			this.listViewIcons.MouseDown += new MouseEventHandler(this.listViewIcons_MouseDown);
			this.imageListIcons.ColorDepth = ColorDepth.Depth32Bit;
			this.imageListIcons.ImageSize = new System.Drawing.Size(32, 32);
			this.imageListIcons.TransparentColor = Color.Transparent;
			this.labelIcons.AutoSize = true;
			this.labelIcons.Location = new Point(3, 65);
			this.labelIcons.Name = "labelIcons";
			this.labelIcons.Size = new System.Drawing.Size(81, 13);
			this.labelIcons.TabIndex = 3;
			this.labelIcons.Text = "&Available icons:";
			this.buttonBrowse.Location = new Point(308, 23);
			this.buttonBrowse.Name = "buttonBrowse";
			this.buttonBrowse.Size = new System.Drawing.Size(86, 24);
			this.buttonBrowse.TabIndex = 2;
			this.buttonBrowse.Text = "&Browse...";
			this.buttonBrowse.UseVisualStyleBackColor = true;
			this.buttonBrowse.Click += new EventHandler(this.buttonBrowse_Click);
			this.textBoxFileName.Location = new Point(6, 27);
			this.textBoxFileName.Name = "textBoxFileName";
			this.textBoxFileName.ReadOnly = true;
			this.textBoxFileName.Size = new System.Drawing.Size(290, 20);
			this.textBoxFileName.TabIndex = 1;
			this.labelFileName.AutoSize = true;
			this.labelFileName.Location = new Point(3, 7);
			this.labelFileName.Name = "labelFileName";
			this.labelFileName.Size = new System.Drawing.Size(55, 13);
			this.labelFileName.TabIndex = 0;
			this.labelFileName.Text = "&File name:";
			this.panelButtons.Controls.Add(this.buttonCancel);
			this.panelButtons.Controls.Add(this.buttonOK);
			this.panelButtons.Dock = DockStyle.Bottom;
			this.panelButtons.Location = new Point(8, 243);
			this.panelButtons.Name = "panelButtons";
			this.panelButtons.Size = new System.Drawing.Size(393, 41);
			this.panelButtons.TabIndex = 1;
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new Point(308, 9);
			this.buttonCancel.Margin = new System.Windows.Forms.Padding(2);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(86, 24);
			this.buttonCancel.TabIndex = 3;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Enabled = false;
			this.buttonOK.Location = new Point(218, 9);
			this.buttonOK.Margin = new System.Windows.Forms.Padding(2);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(86, 24);
			this.buttonOK.TabIndex = 2;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.openFileDialog.Filter = "Icon Files (*.ico)|*.ico";
			this.openFileDialog.SupportMultiDottedExtensions = true;
			this.openFileDialog.Title = "Select Icon";
			base.AcceptButton = this.buttonOK;
			base.AutoScaleDimensions = new SizeF(96f, 96f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			base.CancelButton = this.buttonCancel;
			base.ClientSize = new System.Drawing.Size(409, 292);
			base.Controls.Add(this.panelControls);
			base.Controls.Add(this.panelButtons);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "FormSelectIcon";
			base.Padding = new System.Windows.Forms.Padding(8);
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Icon";
			this.panelControls.ResumeLayout(false);
			this.panelControls.PerformLayout();
			this.panelButtons.ResumeLayout(false);
			base.ResumeLayout(false);
		}

		private void listViewIcons_Click(object sender, EventArgs e)
		{
			this.textBoxFileName.Text = string.Empty;
			this.buttonOK.Enabled = this.listViewIcons.FocusedItem != null;
			this.SelectedIcon = string.Empty;
			if (this.listViewIcons.FocusedItem != null)
			{
				if (!this.directSelection)
				{
					if (this.listViewIcons.FocusedItem.Name == this._listViewTag)
					{
						this.SelectedIcon = this.listViewIcons.FocusedItem.Tag.ToString();
						this.textBoxFileName.Text = this.SelectedIcon;
						return;
					}
					this.SelectedIcon = this.listViewIcons.FocusedItem.Name;
					WiXProjectParser project = null;
					if (this._instanceShortcut != null)
					{
						project = this._instanceShortcut.Project;
					}
					if (this._instanceFileType != null)
					{
						project = this._instanceFileType.Project;
					}
					if (project != null)
					{
						WiXIcon wiXIcon = project.Icons.Find((WiXIcon e1) => e1.Id == this.SelectedIcon);
						if (wiXIcon != null)
						{
							this.textBoxFileName.Text = wiXIcon.SourceFile;
						}
					}
				}
				else
				{
					this.textBoxFileName.Text = (string)this.listViewIcons.FocusedItem.Tag;
					if (!string.IsNullOrEmpty(this.textBoxFileName.Text))
					{
						this.SelectedIcon = Path.Combine(CommonUtilities.RelativizePathIfPossible(Path.GetDirectoryName(this.textBoxFileName.Text), this.projectDir), Path.GetFileName(this.textBoxFileName.Text));
						return;
					}
				}
			}
		}

		private void listViewIcons_DoubleClick(object sender, EventArgs e)
		{
			if (this.listViewIcons.FocusedItem != null && this.buttonOK.Enabled)
			{
				this.buttonOK.PerformClick();
			}
		}

		private void listViewIcons_MouseDown(object sender, MouseEventArgs e)
		{
			ListViewItem itemAt = (sender as ListView).GetItemAt(e.X, e.Y);
			(sender as ListView).FocusedItem = itemAt;
			if (itemAt == null)
			{
				this.buttonOK.Enabled = false;
				this.SelectedIcon = string.Empty;
			}
		}
	}
}