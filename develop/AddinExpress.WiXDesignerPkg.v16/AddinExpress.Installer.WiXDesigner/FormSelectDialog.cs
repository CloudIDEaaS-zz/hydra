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
	public class FormSelectDialog : Form
	{
		private VSShortcut _instanceShortcut;

		private VSFileType _instanceFileType;

		private DialogType selectedDialog;

		private IContainer components;

		private Panel panelControls;

		private Panel panelButtons;

		private Button buttonCancel;

		private Button buttonOK;

		private TextBox textBoxFileName;

		private ListView listViewIcons;

		private ImageList imageListIcons;

		internal DialogType SelectedDialog
		{
			get
			{
				return this.selectedDialog;
			}
			set
			{
				this.selectedDialog = value;
			}
		}

		internal List<DialogType> SelectedDialogs
		{
			get
			{
				List<DialogType> dialogTypes = new List<DialogType>();
				if (this.listViewIcons.SelectedItems != null && this.listViewIcons.SelectedItems.Count > 0)
				{
					foreach (ListViewItem selectedItem in this.listViewIcons.SelectedItems)
					{
						dialogTypes.Add((DialogType)selectedItem.Tag);
					}
				}
				return dialogTypes;
			}
		}

		public FormSelectDialog()
		{
			this.InitializeComponent();
			if (!base.DesignMode)
			{
				System.Drawing.Font environmentFont = VsShellUtilities.GetEnvironmentFont(VsPackage.CurrentInstance);
				if (environmentFont != null)
				{
					this.Font = environmentFont;
				}
				VsShellUtilities.ApplyListViewThemeStyles(this.listViewIcons);
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

		private string GetDialogDescription(DialogType dialogType)
		{
			switch (dialogType)
			{
				case DialogType.ConfirmInstallation:
				{
					return "This dialog asks the customer to confirm the installation.";
				}
				case DialogType.RadioButtons2:
				{
					return "Provides a set of two RadioButtons that can be customized to control the installation.";
				}
				case DialogType.RadioButtons3:
				{
					return "Provides a set of three RadioButtons that can be customized to control the installation.";
				}
				case DialogType.RadioButtons4:
				{
					return "Provides a set of four RadioButtons that can be customized to control the installation.";
				}
				case DialogType.CheckBoxesA:
				{
					return "Provides CheckBoxes that can be customized to control the installation.";
				}
				case DialogType.CheckBoxesB:
				{
					return "Provides CheckBoxes that can be customized to control the installation.";
				}
				case DialogType.CheckBoxesC:
				{
					return "Provides CheckBoxes that can be customized to control the installation.";
				}
				case DialogType.CustomerInformation:
				{
					return "This dialog asks the customer for their name, company, and serial number.";
				}
				case DialogType.TextBoxesA:
				{
					return "Provides TextBoxes that can be customized to control the installation.";
				}
				case DialogType.TextBoxesB:
				{
					return "Provides TextBoxes that can be customized to control the installation.";
				}
				case DialogType.TextBoxesC:
				{
					return "Provides TextBoxes that can be customized to control the installation.";
				}
				case DialogType.Finished:
				{
					return "This dialog appears when the installation is finished.";
				}
				case DialogType.InstallationFolder:
				{
					return "This dialog asks the customer for an installation folder.";
				}
				case DialogType.LicenseAgreement:
				{
					return "This dialog shows the customer a license agreement.";
				}
				case DialogType.Progress:
				{
					return "This dialog shows the progress of the installation.";
				}
				case DialogType.ReadMe:
				{
					return "This dialog shows the customer a \"Read Me\" message.";
				}
				case DialogType.RegisterUser:
				{
					return "This dialog allows the user to register the application.";
				}
				case DialogType.Splash:
				{
					return "This dialog displays a bitmap before the installation.";
				}
				case DialogType.Welcome:
				{
					return "This dialog welcomes the customer to the installation.";
				}
				case DialogType.Empty:
				{
					return "Provides a template to create a new dialog.";
				}
				case DialogType.InstallationAddress:
				{
					return "This dialog asks the customer for an installation address.";
				}
			}
			return string.Empty;
		}

		internal static Dictionary<DialogType, string> GetDialogNames(DialogScope scope, DialogStage stage, bool isWeb)
		{
			Dictionary<DialogType, string> dialogTypes = new Dictionary<DialogType, string>();
			if (stage == DialogStage.Start || stage == DialogStage.End)
			{
				dialogTypes.Add(DialogType.ConfirmInstallation, "Confirm Installation");
				if (scope != DialogScope.AdministrativeInstall)
				{
					dialogTypes.Add(DialogType.RadioButtons2, "RadioButtons (2 buttons)");
					dialogTypes.Add(DialogType.RadioButtons3, "RadioButtons (3 buttons)");
					dialogTypes.Add(DialogType.RadioButtons4, "RadioButtons (4 buttons)");
					dialogTypes.Add(DialogType.CheckBoxesA, "Checkboxes (A)");
					dialogTypes.Add(DialogType.CheckBoxesB, "Checkboxes (B)");
					dialogTypes.Add(DialogType.CheckBoxesC, "Checkboxes (C)");
					dialogTypes.Add(DialogType.CustomerInformation, "Customer Information");
					dialogTypes.Add(DialogType.TextBoxesA, "Textboxes (A)");
					dialogTypes.Add(DialogType.TextBoxesB, "Textboxes (B)");
					dialogTypes.Add(DialogType.TextBoxesC, "Textboxes (C)");
				}
				if (stage == DialogStage.End)
				{
					dialogTypes.Add(DialogType.Finished, "Finished");
				}
				if (!isWeb)
				{
					dialogTypes.Add(DialogType.InstallationFolder, "Installation Folder");
				}
				dialogTypes.Add(DialogType.LicenseAgreement, "License Agreement");
				dialogTypes.Add(DialogType.ReadMe, "Read Me");
				if (scope != DialogScope.AdministrativeInstall)
				{
					dialogTypes.Add(DialogType.RegisterUser, "Register User");
				}
				dialogTypes.Add(DialogType.Splash, "Splash");
				if (isWeb)
				{
					dialogTypes.Add(DialogType.InstallationAddress, "Installation Address");
				}
				dialogTypes.Add(DialogType.Welcome, "Welcome");
			}
			else if (stage == DialogStage.Progress)
			{
				dialogTypes.Add(DialogType.Progress, "Progress");
			}
			return dialogTypes;
		}

		internal void Initialize(List<VSDialogBase> addedDialogList, DialogScope scope, DialogStage stage, bool isWeb)
		{
			foreach (KeyValuePair<DialogType, string> dialogName in FormSelectDialog.GetDialogNames(scope, stage, isWeb))
			{
				bool flag = false;
				foreach (VSDialogBase vSDialogBase in addedDialogList)
				{
					if (vSDialogBase.DisplayName != dialogName.Value)
					{
						continue;
					}
					flag = true;
					goto Label0;
				}
			Label0:
				if (flag)
				{
					continue;
				}
				this.listViewIcons.Items.Add(dialogName.Value, (VsPackage.CurrentInstance.GetVSVersion() >= 11 ? 1 : 0)).Tag = dialogName.Key;
			}
			base.ActiveControl = this.buttonCancel;
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(FormSelectDialog));
			this.panelControls = new Panel();
			this.listViewIcons = new ListView();
			this.imageListIcons = new ImageList(this.components);
			this.textBoxFileName = new TextBox();
			this.panelButtons = new Panel();
			this.buttonCancel = new Button();
			this.buttonOK = new Button();
			this.panelControls.SuspendLayout();
			this.panelButtons.SuspendLayout();
			base.SuspendLayout();
			this.panelControls.Controls.Add(this.listViewIcons);
			this.panelControls.Controls.Add(this.textBoxFileName);
			this.panelControls.Dock = DockStyle.Fill;
			this.panelControls.Location = new Point(8, 8);
			this.panelControls.Name = "panelControls";
			this.panelControls.Size = new System.Drawing.Size(556, 336);
			this.panelControls.TabIndex = 0;
			this.listViewIcons.LargeImageList = this.imageListIcons;
			this.listViewIcons.Location = new Point(0, 0);
			this.listViewIcons.Name = "listViewIcons";
			this.listViewIcons.Size = new System.Drawing.Size(550, 304);
			this.listViewIcons.TabIndex = 0;
			this.listViewIcons.UseCompatibleStateImageBehavior = false;
			this.listViewIcons.ItemSelectionChanged += new ListViewItemSelectionChangedEventHandler(this.listViewIcons_ItemSelectionChanged);
			this.listViewIcons.SelectedIndexChanged += new EventHandler(this.listViewIcons_SelectedIndexChanged);
			this.listViewIcons.Click += new EventHandler(this.listViewIcons_Click);
			this.listViewIcons.DoubleClick += new EventHandler(this.listViewIcons_DoubleClick);
			this.listViewIcons.Enter += new EventHandler(this.listViewIcons_Enter);
			this.imageListIcons.ImageStream = (ImageListStreamer)componentResourceManager.GetObject("imageListIcons.ImageStream");
			this.imageListIcons.TransparentColor = Color.Transparent;
			this.imageListIcons.Images.SetKeyName(0, "ui-add-dialog-item.ico");
			this.imageListIcons.Images.SetKeyName(1, "ui-add-dialog-item-12.ico");
			this.textBoxFileName.Location = new Point(0, 310);
			this.textBoxFileName.Name = "textBoxFileName";
			this.textBoxFileName.ReadOnly = true;
			this.textBoxFileName.Size = new System.Drawing.Size(550, 20);
			this.textBoxFileName.TabIndex = 1;
			this.textBoxFileName.TabStop = false;
			this.panelButtons.Controls.Add(this.buttonCancel);
			this.panelButtons.Controls.Add(this.buttonOK);
			this.panelButtons.Dock = DockStyle.Bottom;
			this.panelButtons.Location = new Point(8, 344);
			this.panelButtons.Name = "panelButtons";
			this.panelButtons.Size = new System.Drawing.Size(556, 41);
			this.panelButtons.TabIndex = 1;
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new Point(468, 8);
			this.buttonCancel.Margin = new System.Windows.Forms.Padding(2);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(86, 24);
			this.buttonCancel.TabIndex = 3;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Enabled = false;
			this.buttonOK.Location = new Point(378, 8);
			this.buttonOK.Margin = new System.Windows.Forms.Padding(2);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(86, 24);
			this.buttonOK.TabIndex = 2;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			base.AcceptButton = this.buttonOK;
			base.AutoScaleDimensions = new SizeF(96f, 96f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			base.CancelButton = this.buttonCancel;
			base.ClientSize = new System.Drawing.Size(572, 393);
			base.Controls.Add(this.panelControls);
			base.Controls.Add(this.panelButtons);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "FormSelectDialog";
			base.Padding = new System.Windows.Forms.Padding(8);
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Add Dialog";
			this.panelControls.ResumeLayout(false);
			this.panelControls.PerformLayout();
			this.panelButtons.ResumeLayout(false);
			base.ResumeLayout(false);
		}

		private void listViewIcons_Click(object sender, EventArgs e)
		{
		}

		private void listViewIcons_DoubleClick(object sender, EventArgs e)
		{
			if (this.listViewIcons.FocusedItem != null && this.buttonOK.Enabled)
			{
				this.buttonOK.PerformClick();
			}
		}

		private void listViewIcons_Enter(object sender, EventArgs e)
		{
			if (this.listViewIcons.FocusedItem == null && this.listViewIcons.Items.Count > 0)
			{
				this.listViewIcons.FocusedItem = this.listViewIcons.Items[0];
				this.listViewIcons.FocusedItem.Selected = true;
			}
		}

		private void listViewIcons_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			this.textBoxFileName.Text = string.Empty;
			this.buttonOK.Enabled = (this.listViewIcons.FocusedItem != null ? true : this.listViewIcons.SelectedItems.Count > 0);
			this.SelectedDialog = DialogType.Undefined;
			if (this.listViewIcons.FocusedItem != null)
			{
				this.SelectedDialog = (DialogType)this.listViewIcons.FocusedItem.Tag;
				this.textBoxFileName.Text = this.GetDialogDescription(this.SelectedDialog);
			}
		}

		private void listViewIcons_SelectedIndexChanged(object sender, EventArgs e)
		{
		}
	}
}