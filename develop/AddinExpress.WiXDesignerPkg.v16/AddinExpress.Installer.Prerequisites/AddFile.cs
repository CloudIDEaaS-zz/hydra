using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace AddinExpress.Installer.Prerequisites
{
	internal class AddFile : Form
	{
		private static List<WeakReference> __ENCList;

		private RadioButton AllLangsRadioButton;

		private Button BrowseButton;

		private Button CancelerButton;

		private TextBox FilenameTextbox;

		private Label Label1;

		private ComboBox LanguageCombo;

		private Button OKButton;

		private OpenFileDialog OpenFileDialog1;

		private RadioButton SingleLanguageRadio;

		private IContainer components = new System.ComponentModel.Container();

		private PrerequisitesForm m_ownerForm;

		static AddFile()
		{
			AddFile.__ENCList = new List<WeakReference>();
		}

		public AddFile()
		{
			base.Closing += new CancelEventHandler(this.AddFile_Closing);
			base.Load += new EventHandler(this.AddFile_Load);
			lock (AddFile.__ENCList)
			{
				AddFile.__ENCList.Add(new WeakReference(this));
			}
			this.InitializeComponent();
		}

		public AddFile(PrerequisitesForm ownerForm) : this()
		{
			this.m_ownerForm = ownerForm;
		}

		private void AddFile_Closing(object sender, CancelEventArgs e)
		{
			PrerequisitesForm.AddFileInfo addFileInfo = new PrerequisitesForm.AddFileInfo();
			if (!this.AllLangsRadioButton.Checked)
			{
				addFileInfo.Langauge = (CultureInfo)this.LanguageCombo.SelectedItem;
			}
			else
			{
				addFileInfo.Langauge = CultureInfo.InvariantCulture;
			}
			if (!string.IsNullOrEmpty(this.FilenameTextbox.Text))
			{
				addFileInfo.FilePath = new FileInfo(this.FilenameTextbox.Text);
				this.m_ownerForm.AddFileResults = addFileInfo;
			}
		}

		private void AddFile_Load(object sender, EventArgs e)
		{
			this.LanguageCombo.Items.Clear();
			CultureInfo currentCulture = Application.CurrentCulture;
			if (!currentCulture.IsNeutralCulture)
			{
				currentCulture = currentCulture.Parent;
			}
			CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
			for (int i = 0; i < (int)cultures.Length; i++)
			{
				CultureInfo cultureInfo = cultures[i];
				if (cultureInfo.IsNeutralCulture)
				{
					this.LanguageCombo.Items.Add(cultureInfo);
				}
				if (cultureInfo.Equals(currentCulture))
				{
					this.LanguageCombo.SelectedItem = cultureInfo;
				}
			}
			this.LanguageCombo.DisplayMember = "DisplayName";
			this.LanguageCombo.SelectedItem = currentCulture;
		}

		private void AllLangsRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			this.SetEnableds();
		}

		private void BrowseButton_Click(object sender, EventArgs e)
		{
			if (this.OpenFileDialog1.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{
				this.FilenameTextbox.Text = this.OpenFileDialog1.FileName;
			}
		}

		private void CancelButton_Click(object sender, EventArgs e)
		{
			base.Hide();
			base.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void FileNameTextbox_TextChanged(object sender, EventArgs e)
		{
			this.SetEnableds();
		}

		private void InitializeComponent()
		{
			this.CancelerButton = new Button();
			this.OKButton = new Button();
			this.BrowseButton = new Button();
			this.FilenameTextbox = new TextBox();
			this.Label1 = new Label();
			this.LanguageCombo = new ComboBox();
			this.SingleLanguageRadio = new RadioButton();
			this.AllLangsRadioButton = new RadioButton();
			this.OpenFileDialog1 = new OpenFileDialog();
			base.SuspendLayout();
			this.CancelerButton.Location = new Point(298, 152);
			this.CancelerButton.Name = "CancelerButton";
			this.CancelerButton.Size = new System.Drawing.Size(87, 25);
			this.CancelerButton.TabIndex = 7;
			this.CancelerButton.Text = "Cancel";
			this.CancelerButton.Click += new EventHandler(this.CancelButton_Click);
			this.OKButton.Enabled = false;
			this.OKButton.Location = new Point(205, 152);
			this.OKButton.Name = "OKButton";
			this.OKButton.Size = new System.Drawing.Size(87, 25);
			this.OKButton.TabIndex = 6;
			this.OKButton.Text = "OK";
			this.OKButton.Click += new EventHandler(this.OKButton_Click);
			this.BrowseButton.Location = new Point(298, 100);
			this.BrowseButton.Name = "BrowseButton";
			this.BrowseButton.Size = new System.Drawing.Size(87, 25);
			this.BrowseButton.TabIndex = 5;
			this.BrowseButton.Text = "Browse...";
			this.BrowseButton.Click += new EventHandler(this.BrowseButton_Click);
			this.FilenameTextbox.Location = new Point(13, 101);
			this.FilenameTextbox.Name = "FilenameTextbox";
			this.FilenameTextbox.Size = new System.Drawing.Size(284, 23);
			this.FilenameTextbox.TabIndex = 4;
			this.FilenameTextbox.TextChanged += new EventHandler(this.FileNameTextbox_TextChanged);
			this.Label1.AutoSize = true;
			this.Label1.Location = new Point(13, 81);
			this.Label1.Name = "Label1";
			this.Label1.Size = new System.Drawing.Size(61, 15);
			this.Label1.TabIndex = 3;
			this.Label1.Text = "File name:";
			this.LanguageCombo.FormattingEnabled = true;
			this.LanguageCombo.Location = new Point(134, 40);
			this.LanguageCombo.Name = "LanguageCombo";
			this.LanguageCombo.Size = new System.Drawing.Size(251, 23);
			this.LanguageCombo.TabIndex = 2;
			this.SingleLanguageRadio.AutoSize = true;
			this.SingleLanguageRadio.Location = new Point(13, 41);
			this.SingleLanguageRadio.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
			this.SingleLanguageRadio.Name = "SingleLanguageRadio";
			this.SingleLanguageRadio.Size = new System.Drawing.Size(112, 19);
			this.SingleLanguageRadio.TabIndex = 1;
			this.SingleLanguageRadio.Text = "Single language:";
			this.SingleLanguageRadio.CheckedChanged += new EventHandler(this.SingleLanguageRadio_CheckedChanged);
			this.AllLangsRadioButton.AutoSize = true;
			this.AllLangsRadioButton.Checked = true;
			this.AllLangsRadioButton.Location = new Point(14, 16);
			this.AllLangsRadioButton.Margin = new System.Windows.Forms.Padding(3, 3, 3, 1);
			this.AllLangsRadioButton.Name = "AllLangsRadioButton";
			this.AllLangsRadioButton.Size = new System.Drawing.Size(96, 19);
			this.AllLangsRadioButton.TabIndex = 0;
			this.AllLangsRadioButton.TabStop = true;
			this.AllLangsRadioButton.Text = "All languages";
			this.AllLangsRadioButton.CheckedChanged += new EventHandler(this.AllLangsRadioButton_CheckedChanged);
			this.OpenFileDialog1.Filter = "Installer Files (*.msi;*.exe)|*.msi;*.exe|All Files (*.*)|*.*";
			base.AcceptButton = this.OKButton;
			base.AutoScaleDimensions = new SizeF(96f, 96f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			base.ClientSize = new System.Drawing.Size(397, 189);
			base.Controls.Add(this.CancelerButton);
			base.Controls.Add(this.OKButton);
			base.Controls.Add(this.BrowseButton);
			base.Controls.Add(this.FilenameTextbox);
			base.Controls.Add(this.Label1);
			base.Controls.Add(this.LanguageCombo);
			base.Controls.Add(this.SingleLanguageRadio);
			base.Controls.Add(this.AllLangsRadioButton);
			this.Font = new System.Drawing.Font("Segoe UI", 9f, FontStyle.Regular, GraphicsUnit.Point, 0);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "AddFile";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Add File";
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void OKButton_Click(object sender, EventArgs e)
		{
			base.Hide();
			base.DialogResult = System.Windows.Forms.DialogResult.OK;
		}

		private void SetEnableds()
		{
			this.LanguageCombo.Enabled = this.SingleLanguageRadio.Checked;
			this.OKButton.Enabled = this.FilenameTextbox.Text.Length > 0;
		}

		private void SingleLanguageRadio_CheckedChanged(object sender, EventArgs e)
		{
			this.SetEnableds();
		}
	}
}