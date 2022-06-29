using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace AddinExpress.Installer.WiXDesigner
{
	public class FormMapping : Form
	{
		private IServiceProvider _provider;

		private VSFolders _fileSystem;

		internal string fileID = string.Empty;

		private IContainer components;

		private Label labelExecutable;

		private Button buttonBrowse;

		private Label labelExtension;

		private Label labelVerbs;

		private RadioButton radioButtonLimit;

		private Button buttonCancel;

		private Button buttonOK;

		internal TextBox textBoxExecutable;

		internal TextBox textBoxExtension;

		internal RadioButton radioButtonAll;

		internal TextBox textBoxVerb;

		public FormMapping()
		{
			this.InitializeComponent();
		}

		private void buttonBrowse_Click(object sender, EventArgs e)
		{
			FormSelectItemInProject formSelectItemInProject = new FormSelectItemInProject();
			formSelectItemInProject.Initialize(null, string.Empty, this._fileSystem, new string[] { "Dynamic-link Libraries (*.dll)", "Executable Files (*.exe)", "All Files (*.*)" });
			if (this.ShowForm(this._provider, formSelectItemInProject) == System.Windows.Forms.DialogResult.OK)
			{
				if (formSelectItemInProject.SelectedFile != null)
				{
					if (formSelectItemInProject.SelectedFile is VSProjectOutputFile)
					{
						this.textBoxExecutable.Text = formSelectItemInProject.SelectedFile.Name;
						this.fileID = (formSelectItemInProject.SelectedFile as VSProjectOutputFile).WiXElement.Id;
					}
					else if (formSelectItemInProject.SelectedFile is VSBinary)
					{
						this.textBoxExecutable.Text = (formSelectItemInProject.SelectedFile as VSBinary).TargetName;
						this.fileID = (formSelectItemInProject.SelectedFile as VSBinary).WiXElement.Id;
					}
					else if (formSelectItemInProject.SelectedFile is VSFile)
					{
						this.textBoxExecutable.Text = (formSelectItemInProject.SelectedFile as VSFile).TargetName;
						this.fileID = (formSelectItemInProject.SelectedFile as VSFile).WiXElement.Id;
					}
					else if (formSelectItemInProject.SelectedFile is VSAssembly)
					{
						this.textBoxExecutable.Text = (formSelectItemInProject.SelectedFile as VSAssembly).TargetName;
						this.fileID = (formSelectItemInProject.SelectedFile as VSAssembly).WiXElement.Id;
					}
					else if (formSelectItemInProject.SelectedFile is VSProjectOutputUnknown)
					{
						this.textBoxExecutable.Text = formSelectItemInProject.SelectedFile.Name;
						this.fileID = (formSelectItemInProject.SelectedFile as VSProjectOutputUnknown).WiXElement.Id;
					}
				}
				this.buttonOK.Enabled = this.CheckControls();
			}
			formSelectItemInProject.Dispose();
		}

		private bool CheckControls()
		{
			bool text = this.textBoxExecutable.Text != "(Not set)";
			if (text)
			{
				text = this.textBoxExtension.Text != string.Empty;
			}
			if (text && this.radioButtonLimit.Checked)
			{
				text = this.textBoxVerb.Text != string.Empty;
			}
			return text;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		internal void Initialize(IServiceProvider provider, VSFolders fileSystem, VSWebApplicationExtension vsExtension = null)
		{
			this._provider = provider;
			this._fileSystem = fileSystem;
			if (vsExtension != null)
			{
				string str = vsExtension.Executable.Substring("[#".Length);
				str = str.Substring(0, str.Length - 1);
				VSBaseFile fileById = fileSystem.GetFileById(str);
				if (fileById != null)
				{
					if (fileById is VSProjectOutputFile)
					{
						this.textBoxExecutable.Text = fileById.Name;
					}
					else if (fileById is VSBinary)
					{
						this.textBoxExecutable.Text = (fileById as VSBinary).TargetName;
					}
					else if (fileById is VSFile)
					{
						this.textBoxExecutable.Text = (fileById as VSFile).TargetName;
					}
					else if (fileById is VSAssembly)
					{
						this.textBoxExecutable.Text = (fileById as VSAssembly).TargetName;
					}
					else if (fileById is VSProjectOutputUnknown)
					{
						this.textBoxExecutable.Text = fileById.Name;
					}
				}
				this.textBoxExtension.Text = vsExtension.Extension;
				if (!string.IsNullOrEmpty(vsExtension.Verbs) && vsExtension.Verbs != "*")
				{
					this.radioButtonLimit.Checked = true;
					this.textBoxVerb.Text = vsExtension.Verbs;
					this.textBoxVerb.Enabled = true;
				}
			}
		}

		private void InitializeComponent()
		{
			this.labelExecutable = new Label();
			this.textBoxExecutable = new TextBox();
			this.buttonBrowse = new Button();
			this.labelExtension = new Label();
			this.textBoxExtension = new TextBox();
			this.labelVerbs = new Label();
			this.radioButtonAll = new RadioButton();
			this.radioButtonLimit = new RadioButton();
			this.textBoxVerb = new TextBox();
			this.buttonCancel = new Button();
			this.buttonOK = new Button();
			base.SuspendLayout();
			this.labelExecutable.AutoSize = true;
			this.labelExecutable.Location = new Point(10, 18);
			this.labelExecutable.Name = "labelExecutable";
			this.labelExecutable.Size = new System.Drawing.Size(63, 13);
			this.labelExecutable.TabIndex = 0;
			this.labelExecutable.Text = "E&xecutable:";
			this.textBoxExecutable.Location = new Point(140, 13);
			this.textBoxExecutable.Name = "textBoxExecutable";
			this.textBoxExecutable.ReadOnly = true;
			this.textBoxExecutable.Size = new System.Drawing.Size(204, 20);
			this.textBoxExecutable.TabIndex = 1;
			this.textBoxExecutable.Text = "(Not set)";
			this.buttonBrowse.Location = new Point(350, 10);
			this.buttonBrowse.Name = "buttonBrowse";
			this.buttonBrowse.Size = new System.Drawing.Size(95, 25);
			this.buttonBrowse.TabIndex = 2;
			this.buttonBrowse.Text = "&Browse...";
			this.buttonBrowse.UseVisualStyleBackColor = true;
			this.buttonBrowse.Click += new EventHandler(this.buttonBrowse_Click);
			this.labelExtension.AutoSize = true;
			this.labelExtension.Location = new Point(10, 48);
			this.labelExtension.Name = "labelExtension";
			this.labelExtension.Size = new System.Drawing.Size(56, 13);
			this.labelExtension.TabIndex = 3;
			this.labelExtension.Text = "&Extension:";
			this.textBoxExtension.Location = new Point(140, 45);
			this.textBoxExtension.Name = "textBoxExtension";
			this.textBoxExtension.Size = new System.Drawing.Size(305, 20);
			this.textBoxExtension.TabIndex = 4;
			this.textBoxExtension.TextChanged += new EventHandler(this.textBoxExtension_TextChanged);
			this.labelVerbs.AutoSize = true;
			this.labelVerbs.Location = new Point(10, 78);
			this.labelVerbs.Name = "labelVerbs";
			this.labelVerbs.Size = new System.Drawing.Size(68, 13);
			this.labelVerbs.TabIndex = 5;
			this.labelVerbs.Text = "HTTP verbs:";
			this.radioButtonAll.AutoSize = true;
			this.radioButtonAll.Checked = true;
			this.radioButtonAll.Location = new Point(60, 97);
			this.radioButtonAll.Name = "radioButtonAll";
			this.radioButtonAll.Size = new System.Drawing.Size(65, 17);
			this.radioButtonAll.TabIndex = 6;
			this.radioButtonAll.TabStop = true;
			this.radioButtonAll.Text = "&All verbs";
			this.radioButtonAll.UseVisualStyleBackColor = true;
			this.radioButtonAll.CheckedChanged += new EventHandler(this.radioButtonAll_CheckedChanged);
			this.radioButtonLimit.AutoSize = true;
			this.radioButtonLimit.Location = new Point(60, 120);
			this.radioButtonLimit.Name = "radioButtonLimit";
			this.radioButtonLimit.Size = new System.Drawing.Size(61, 17);
			this.radioButtonLimit.TabIndex = 7;
			this.radioButtonLimit.Text = "&Limit to:";
			this.radioButtonLimit.UseVisualStyleBackColor = true;
			this.textBoxVerb.Enabled = false;
			this.textBoxVerb.Location = new Point(85, 143);
			this.textBoxVerb.Name = "textBoxVerb";
			this.textBoxVerb.Size = new System.Drawing.Size(360, 20);
			this.textBoxVerb.TabIndex = 8;
			this.textBoxVerb.TextChanged += new EventHandler(this.textBoxVerb_TextChanged);
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new Point(350, 175);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(95, 25);
			this.buttonCancel.TabIndex = 10;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Enabled = false;
			this.buttonOK.Location = new Point(249, 175);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(95, 25);
			this.buttonOK.TabIndex = 9;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			base.AcceptButton = this.buttonOK;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.buttonCancel;
			base.ClientSize = new System.Drawing.Size(454, 207);
			base.Controls.Add(this.buttonCancel);
			base.Controls.Add(this.buttonOK);
			base.Controls.Add(this.textBoxVerb);
			base.Controls.Add(this.radioButtonLimit);
			base.Controls.Add(this.radioButtonAll);
			base.Controls.Add(this.labelVerbs);
			base.Controls.Add(this.textBoxExtension);
			base.Controls.Add(this.labelExtension);
			base.Controls.Add(this.buttonBrowse);
			base.Controls.Add(this.textBoxExecutable);
			base.Controls.Add(this.labelExecutable);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "FormMapping";
			base.Padding = new System.Windows.Forms.Padding(10);
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Application Extension Mapping";
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void radioButtonAll_CheckedChanged(object sender, EventArgs e)
		{
			this.textBoxVerb.Enabled = this.radioButtonLimit.Checked;
			this.buttonOK.Enabled = this.CheckControls();
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

		private void textBoxExtension_TextChanged(object sender, EventArgs e)
		{
			this.buttonOK.Enabled = this.CheckControls();
		}

		private void textBoxVerb_TextChanged(object sender, EventArgs e)
		{
			this.buttonOK.Enabled = this.CheckControls();
		}
	}
}