using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace AddinExpress.Installer.WiXDesigner
{
	public class FormNewCode : Form
	{
		private bool generateMergeModuleSignature;

		private IContainer components;

		private Panel panelControls;

		private Panel panelButtons;

		private Button buttonCancel;

		private Button buttonOK;

		private Button buttonNewCode;

		private TextBox textBoxNewCode;

		private Label labelFileName;

		private ImageList imageListIcons;

		private OpenFileDialog openFileDialog;

		public string ButtonText
		{
			get
			{
				return this.buttonNewCode.Text;
			}
			set
			{
				this.buttonNewCode.Text = value;
			}
		}

		public string Code
		{
			get
			{
				return this.textBoxNewCode.Text;
			}
			set
			{
				this.textBoxNewCode.Text = value;
			}
		}

		public bool GenerateMergeModuleSignature
		{
			get
			{
				return this.generateMergeModuleSignature;
			}
			set
			{
				this.generateMergeModuleSignature = value;
			}
		}

		public FormNewCode()
		{
			this.InitializeComponent();
			System.Drawing.Font environmentFont = VsShellUtilities.GetEnvironmentFont(VsPackage.CurrentInstance);
			if (environmentFont != null)
			{
				this.Font = environmentFont;
			}
			base.ActiveControl = this.textBoxNewCode;
		}

		private void buttonNewCode_Click(object sender, EventArgs e)
		{
			if (this.generateMergeModuleSignature)
			{
				this.textBoxNewCode.Text = Common.GenerateGuid().Replace("-", string.Empty);
				return;
			}
			this.textBoxNewCode.Text = string.Concat("{", Common.GenerateGuid(), "}");
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
			this.buttonNewCode = new Button();
			this.textBoxNewCode = new TextBox();
			this.labelFileName = new Label();
			this.imageListIcons = new ImageList(this.components);
			this.panelButtons = new Panel();
			this.buttonCancel = new Button();
			this.buttonOK = new Button();
			this.openFileDialog = new OpenFileDialog();
			this.panelControls.SuspendLayout();
			this.panelButtons.SuspendLayout();
			base.SuspendLayout();
			this.panelControls.Controls.Add(this.buttonNewCode);
			this.panelControls.Controls.Add(this.textBoxNewCode);
			this.panelControls.Controls.Add(this.labelFileName);
			this.panelControls.Dock = DockStyle.Fill;
			this.panelControls.Location = new Point(8, 8);
			this.panelControls.Name = "panelControls";
			this.panelControls.Size = new System.Drawing.Size(418, 94);
			this.panelControls.TabIndex = 0;
			this.buttonNewCode.Location = new Point(248, 45);
			this.buttonNewCode.Name = "buttonNewCode";
			this.buttonNewCode.Size = new System.Drawing.Size(170, 26);
			this.buttonNewCode.TabIndex = 2;
			this.buttonNewCode.Text = "&New Code";
			this.buttonNewCode.UseVisualStyleBackColor = true;
			this.buttonNewCode.Click += new EventHandler(this.buttonNewCode_Click);
			this.textBoxNewCode.Location = new Point(96, 9);
			this.textBoxNewCode.Multiline = true;
			this.textBoxNewCode.Name = "textBoxNewCode";
			this.textBoxNewCode.Size = new System.Drawing.Size(319, 25);
			this.textBoxNewCode.TabIndex = 1;
			this.textBoxNewCode.WordWrap = false;
			this.textBoxNewCode.TextChanged += new EventHandler(this.textBoxNewCode_TextChanged);
			this.labelFileName.AutoSize = true;
			this.labelFileName.Location = new Point(3, 12);
			this.labelFileName.Name = "labelFileName";
			this.labelFileName.Size = new System.Drawing.Size(50, 13);
			this.labelFileName.TabIndex = 0;
			this.labelFileName.Text = "&Identifier:";
			this.imageListIcons.ColorDepth = ColorDepth.Depth32Bit;
			this.imageListIcons.ImageSize = new System.Drawing.Size(32, 32);
			this.imageListIcons.TransparentColor = Color.Transparent;
			this.panelButtons.Controls.Add(this.buttonCancel);
			this.panelButtons.Controls.Add(this.buttonOK);
			this.panelButtons.Dock = DockStyle.Bottom;
			this.panelButtons.Location = new Point(8, 102);
			this.panelButtons.Name = "panelButtons";
			this.panelButtons.Size = new System.Drawing.Size(418, 41);
			this.panelButtons.TabIndex = 1;
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new Point(325, 9);
			this.buttonCancel.Margin = new System.Windows.Forms.Padding(2);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(90, 26);
			this.buttonCancel.TabIndex = 3;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Enabled = false;
			this.buttonOK.Location = new Point(231, 9);
			this.buttonOK.Margin = new System.Windows.Forms.Padding(2);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(90, 26);
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
			base.ClientSize = new System.Drawing.Size(434, 151);
			base.Controls.Add(this.panelControls);
			base.Controls.Add(this.panelButtons);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "FormNewCode";
			base.Padding = new System.Windows.Forms.Padding(8);
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "New Code";
			this.panelControls.ResumeLayout(false);
			this.panelControls.PerformLayout();
			this.panelButtons.ResumeLayout(false);
			base.ResumeLayout(false);
		}

		private void textBoxNewCode_TextChanged(object sender, EventArgs e)
		{
			this.buttonOK.Enabled = !string.IsNullOrEmpty(this.textBoxNewCode.Text);
		}
	}
}