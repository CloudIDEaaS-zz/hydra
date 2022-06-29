using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace AddinExpress.Installer.WiXDesigner
{
	public class FormMappings : Form
	{
		private IServiceProvider _provider;

		private VSWebCustomFolder _folder;

		private List<VSWebApplicationExtension> _extensions;

		private IContainer components;

		private ListView listViewMappings;

		private Button buttonAdd;

		private Button buttonEdit;

		private Button buttonRemove;

		private Button buttonOK;

		private Button buttonCancel;

		private ColumnHeader columnHeaderExtension;

		private ColumnHeader columnHeaderPath;

		private ColumnHeader columnHeaderVerbs;

		internal List<VSWebApplicationExtension> Extensions
		{
			get
			{
				return this._extensions;
			}
		}

		public FormMappings()
		{
			this.InitializeComponent();
		}

		private void buttonAdd_Click(object sender, EventArgs e)
		{
			FormMapping formMapping = new FormMapping();
			formMapping.Initialize(this._provider, this._folder._project.FileSystem, null);
			if (this.ShowForm(this._provider, formMapping) == System.Windows.Forms.DialogResult.OK)
			{
				VSWebApplicationExtension vSWebApplicationExtension = new VSWebApplicationExtension(string.Concat("[#", formMapping.fileID, "]"), formMapping.textBoxExtension.Text, (formMapping.radioButtonAll.Checked ? "*" : formMapping.textBoxVerb.Text));
				this._extensions.Add(vSWebApplicationExtension);
				ListViewItem listViewItem = new ListViewItem(formMapping.textBoxExtension.Text);
				listViewItem.SubItems.Add(formMapping.textBoxExecutable.Text);
				listViewItem.SubItems.Add((formMapping.radioButtonAll.Checked ? "*" : formMapping.textBoxVerb.Text));
				listViewItem.Tag = vSWebApplicationExtension;
				this.listViewMappings.Items.Add(listViewItem);
				this.buttonOK.Enabled = true;
			}
			formMapping.Dispose();
		}

		private void buttonEdit_Click(object sender, EventArgs e)
		{
			FormMapping formMapping = new FormMapping();
			VSWebApplicationExtension tag = this.listViewMappings.SelectedItems[0].Tag as VSWebApplicationExtension;
			formMapping.Initialize(this._provider, this._folder._project.FileSystem, tag);
			if (this.ShowForm(this._provider, formMapping) == System.Windows.Forms.DialogResult.OK)
			{
				tag.Executable = string.Concat("[#", formMapping.fileID, "]");
				tag.Extension = formMapping.textBoxExtension.Text;
				tag.Verbs = (formMapping.radioButtonAll.Checked ? "*" : formMapping.textBoxVerb.Text);
				this.listViewMappings.SelectedItems[0].Text = formMapping.textBoxExtension.Text;
				this.listViewMappings.SelectedItems[0].SubItems[1].Text = formMapping.textBoxExecutable.Text;
				this.listViewMappings.SelectedItems[0].SubItems[2].Text = (formMapping.radioButtonAll.Checked ? "*" : formMapping.textBoxVerb.Text);
				this.buttonOK.Enabled = true;
			}
			formMapping.Dispose();
		}

		private void buttonRemove_Click(object sender, EventArgs e)
		{
			if (this.listViewMappings.SelectedItems.Count > 0)
			{
				VSWebApplicationExtension tag = this.listViewMappings.SelectedItems[0].Tag as VSWebApplicationExtension;
				if (tag != null)
				{
					this._extensions.Remove(tag);
					this.listViewMappings.SelectedItems[0].Remove();
					this.buttonOK.Enabled = true;
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

		internal void Initialize(IServiceProvider provider, VSWebCustomFolder webFolder, List<VSWebApplicationExtension> extensions)
		{
			this._provider = provider;
			this._folder = webFolder;
			this._extensions = extensions;
			this.listViewMappings.Items.Clear();
			foreach (VSWebApplicationExtension _extension in this._extensions)
			{
				ListViewItem listViewItem = new ListViewItem(_extension.Extension);
				if (string.IsNullOrEmpty(_extension.Executable) || !_extension.Executable.StartsWith("[#"))
				{
					listViewItem.SubItems.Add(string.Empty);
				}
				else
				{
					string str = _extension.Executable.Substring("[#".Length);
					str = str.Substring(0, str.Length - 1);
					VSBaseFile fileById = webFolder._project.FileSystem.GetFileById(str);
					if (fileById == null)
					{
						listViewItem.SubItems.Add(string.Empty);
					}
					else if (fileById is VSProjectOutputFile)
					{
						listViewItem.SubItems.Add(fileById.Name);
					}
					else if (fileById is VSBinary)
					{
						listViewItem.SubItems.Add((fileById as VSBinary).TargetName);
					}
					else if (fileById is VSFile)
					{
						listViewItem.SubItems.Add((fileById as VSFile).TargetName);
					}
					else if (fileById is VSAssembly)
					{
						listViewItem.SubItems.Add((fileById as VSAssembly).TargetName);
					}
					else if (fileById is VSProjectOutputUnknown)
					{
						listViewItem.SubItems.Add(fileById.Name);
					}
				}
				if (string.IsNullOrEmpty(_extension.Verbs))
				{
					listViewItem.SubItems.Add(string.Empty);
				}
				else
				{
					listViewItem.SubItems.Add(_extension.Verbs);
				}
				listViewItem.Tag = _extension;
				this.listViewMappings.Items.Add(listViewItem);
			}
		}

		private void InitializeComponent()
		{
			this.listViewMappings = new ListView();
			this.columnHeaderExtension = new ColumnHeader();
			this.columnHeaderPath = new ColumnHeader();
			this.columnHeaderVerbs = new ColumnHeader();
			this.buttonAdd = new Button();
			this.buttonEdit = new Button();
			this.buttonRemove = new Button();
			this.buttonOK = new Button();
			this.buttonCancel = new Button();
			base.SuspendLayout();
			this.listViewMappings.Columns.AddRange(new ColumnHeader[] { this.columnHeaderExtension, this.columnHeaderPath, this.columnHeaderVerbs });
			this.listViewMappings.HeaderStyle = ColumnHeaderStyle.Nonclickable;
			this.listViewMappings.Location = new Point(10, 10);
			this.listViewMappings.MultiSelect = false;
			this.listViewMappings.Name = "listViewMappings";
			this.listViewMappings.Size = new System.Drawing.Size(334, 200);
			this.listViewMappings.Sorting = SortOrder.Ascending;
			this.listViewMappings.TabIndex = 0;
			this.listViewMappings.UseCompatibleStateImageBehavior = false;
			this.listViewMappings.View = View.Details;
			this.listViewMappings.SelectedIndexChanged += new EventHandler(this.listViewMappings_SelectedIndexChanged);
			this.columnHeaderExtension.Text = "Extension";
			this.columnHeaderExtension.Width = 70;
			this.columnHeaderPath.Text = "Executable Path";
			this.columnHeaderPath.Width = 200;
			this.columnHeaderVerbs.Text = "Verbs";
			this.columnHeaderVerbs.Width = 80;
			this.buttonAdd.Location = new Point(350, 10);
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.Size = new System.Drawing.Size(95, 25);
			this.buttonAdd.TabIndex = 1;
			this.buttonAdd.Text = "&Add...";
			this.buttonAdd.UseVisualStyleBackColor = true;
			this.buttonAdd.Click += new EventHandler(this.buttonAdd_Click);
			this.buttonEdit.Enabled = false;
			this.buttonEdit.Location = new Point(350, 41);
			this.buttonEdit.Name = "buttonEdit";
			this.buttonEdit.Size = new System.Drawing.Size(95, 25);
			this.buttonEdit.TabIndex = 2;
			this.buttonEdit.Text = "&Edit";
			this.buttonEdit.UseVisualStyleBackColor = true;
			this.buttonEdit.Click += new EventHandler(this.buttonEdit_Click);
			this.buttonRemove.Enabled = false;
			this.buttonRemove.Location = new Point(350, 72);
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.Size = new System.Drawing.Size(95, 25);
			this.buttonRemove.TabIndex = 3;
			this.buttonRemove.Text = "&Remove";
			this.buttonRemove.UseVisualStyleBackColor = true;
			this.buttonRemove.Click += new EventHandler(this.buttonRemove_Click);
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Enabled = false;
			this.buttonOK.Location = new Point(249, 220);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(95, 25);
			this.buttonOK.TabIndex = 4;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new Point(350, 220);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(95, 25);
			this.buttonCancel.TabIndex = 5;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			base.AcceptButton = this.buttonOK;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.buttonCancel;
			base.ClientSize = new System.Drawing.Size(454, 252);
			base.Controls.Add(this.buttonCancel);
			base.Controls.Add(this.buttonOK);
			base.Controls.Add(this.buttonRemove);
			base.Controls.Add(this.buttonEdit);
			base.Controls.Add(this.buttonAdd);
			base.Controls.Add(this.listViewMappings);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "FormMappings";
			base.Padding = new System.Windows.Forms.Padding(10);
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Application Mappings";
			base.ResumeLayout(false);
		}

		private void listViewMappings_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.buttonEdit.Enabled = this.listViewMappings.SelectedItems.Count > 0;
			this.buttonRemove.Enabled = this.buttonEdit.Enabled;
		}

		private System.Windows.Forms.DialogResult ShowForm(IServiceProvider provider, Form form)
		{
			IUIService service = (IUIService)provider.GetService(typeof(IUIService));
			if (service == null)
			{
				return form.ShowDialog();
			}
			return service.ShowDialog(form);
		}
	}
}