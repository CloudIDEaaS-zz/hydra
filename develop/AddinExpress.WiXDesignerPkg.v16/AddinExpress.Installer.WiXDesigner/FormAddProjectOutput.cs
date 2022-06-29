using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace AddinExpress.Installer.WiXDesigner
{
	public class FormAddProjectOutput : Form
	{
		private List<ProjectDescriptor> _solutionProjects = new List<ProjectDescriptor>();

		private IContainer components;

		private Label labelProject;

		private ComboBox comboBoxProjects;

		private Panel panelControls;

		private Panel panelButtons;

		private Button buttonCancel;

		private Button buttonOK;

		private ListView listViewOutputType;

		private TextBox textBoxDescription;

		private Label labelDescription;

		private ComboBox comboBoxConfigurations;

		private Label labelConfiguration;

		private ColumnHeader columnHeader1;

		internal OutputGroup SelectedGroup
		{
			get
			{
				if (this.listViewOutputType.SelectedItems.Count > 0)
				{
					string str = this.listViewOutputType.SelectedItems[0].Tag.ToString();
					if (str != null)
					{
						if (str == ".Binaries")
						{
							return OutputGroup.Binaries;
						}
						if (str == ".Satellites")
						{
							return OutputGroup.Satellites;
						}
						if (str == ".Symbols")
						{
							return OutputGroup.Symbols;
						}
						if (str == ".Content")
						{
							return OutputGroup.Content;
						}
						if (str == ".Sources")
						{
							return OutputGroup.Sources;
						}
						if (str == ".Documents")
						{
							return OutputGroup.Documents;
						}
					}
				}
				return OutputGroup.None;
			}
		}

		internal string SelectedId
		{
			get
			{
				if (this.comboBoxProjects.SelectedIndex < 0 || this.listViewOutputType.SelectedItems.Count <= 0)
				{
					return string.Empty;
				}
				return string.Concat(this.comboBoxProjects.Text, this.listViewOutputType.SelectedItems[0].Tag.ToString());
			}
		}

		internal ProjectDescriptor SelectedProject
		{
			get
			{
				ProjectDescriptor projectDescriptor;
				if (this.comboBoxProjects.SelectedIndex >= 0)
				{
					List<ProjectDescriptor>.Enumerator enumerator = this._solutionProjects.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							ProjectDescriptor current = enumerator.Current;
							if (current.Name != this.comboBoxProjects.Text)
							{
								continue;
							}
							projectDescriptor = current;
							return projectDescriptor;
						}
						return null;
					}
					finally
					{
						((IDisposable)enumerator).Dispose();
					}
					return projectDescriptor;
				}
				return null;
			}
		}

		public FormAddProjectOutput()
		{
			this.InitializeComponent();
			System.Drawing.Font environmentFont = VsShellUtilities.GetEnvironmentFont(VsPackage.CurrentInstance);
			if (environmentFont != null)
			{
				this.Font = environmentFont;
			}
			VsShellUtilities.ApplyListViewThemeStyles(this.listViewOutputType);
		}

		private void comboBoxProjects_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.listViewOutputType.SelectedItems.Clear();
			this.listViewOutputType.Items[0].Selected = true;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		internal void Initialize(List<ProjectDescriptor> solutionProjects)
		{
			foreach (ProjectDescriptor solutionProject in solutionProjects)
			{
				if (solutionProject.Kind.Equals("{930C7802-8A8C-48F9-8165-68863BCCD9DD}", StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}
				this._solutionProjects.Add(solutionProject);
				this.comboBoxProjects.Items.Add(solutionProject.Name);
			}
			if (this.comboBoxProjects.Items.Count > 0)
			{
				this.comboBoxProjects.SelectedIndex = 0;
			}
			this.buttonOK.Enabled = this.comboBoxProjects.Items.Count > 0;
		}

		private void InitializeComponent()
		{
			ListViewItem listViewItem = new ListViewItem("Primary Output");
			ListViewItem listViewItem1 = new ListViewItem("Localized Resources");
			ListViewItem listViewItem2 = new ListViewItem("Debug Symbols");
			ListViewItem listViewItem3 = new ListViewItem("Content Files");
			ListViewItem listViewItem4 = new ListViewItem("Source Files");
			ListViewItem listViewItem5 = new ListViewItem("Documentation Files");
			this.labelProject = new Label();
			this.comboBoxProjects = new ComboBox();
			this.panelControls = new Panel();
			this.textBoxDescription = new TextBox();
			this.labelDescription = new Label();
			this.comboBoxConfigurations = new ComboBox();
			this.labelConfiguration = new Label();
			this.listViewOutputType = new ListView();
			this.columnHeader1 = new ColumnHeader();
			this.panelButtons = new Panel();
			this.buttonCancel = new Button();
			this.buttonOK = new Button();
			this.panelControls.SuspendLayout();
			this.panelButtons.SuspendLayout();
			base.SuspendLayout();
			this.labelProject.AutoSize = true;
			this.labelProject.Location = new Point(0, 12);
			this.labelProject.Name = "labelProject";
			this.labelProject.Size = new System.Drawing.Size(43, 13);
			this.labelProject.TabIndex = 0;
			this.labelProject.Text = "&Project:";
			this.comboBoxProjects.DropDownStyle = ComboBoxStyle.DropDownList;
			this.comboBoxProjects.FormattingEnabled = true;
			this.comboBoxProjects.Location = new Point(87, 9);
			this.comboBoxProjects.Name = "comboBoxProjects";
			this.comboBoxProjects.Size = new System.Drawing.Size(240, 21);
			this.comboBoxProjects.Sorted = true;
			this.comboBoxProjects.TabIndex = 1;
			this.comboBoxProjects.SelectedIndexChanged += new EventHandler(this.comboBoxProjects_SelectedIndexChanged);
			this.panelControls.Controls.Add(this.textBoxDescription);
			this.panelControls.Controls.Add(this.labelDescription);
			this.panelControls.Controls.Add(this.comboBoxConfigurations);
			this.panelControls.Controls.Add(this.labelConfiguration);
			this.panelControls.Controls.Add(this.listViewOutputType);
			this.panelControls.Controls.Add(this.comboBoxProjects);
			this.panelControls.Controls.Add(this.labelProject);
			this.panelControls.Dock = DockStyle.Fill;
			this.panelControls.Location = new Point(8, 8);
			this.panelControls.Name = "panelControls";
			this.panelControls.Size = new System.Drawing.Size(328, 295);
			this.panelControls.TabIndex = 2;
			this.textBoxDescription.Location = new Point(0, 215);
			this.textBoxDescription.Multiline = true;
			this.textBoxDescription.Name = "textBoxDescription";
			this.textBoxDescription.ReadOnly = true;
			this.textBoxDescription.ScrollBars = ScrollBars.Vertical;
			this.textBoxDescription.Size = new System.Drawing.Size(327, 74);
			this.textBoxDescription.TabIndex = 6;
			this.labelDescription.AutoSize = true;
			this.labelDescription.Location = new Point(0, 194);
			this.labelDescription.Name = "labelDescription";
			this.labelDescription.Size = new System.Drawing.Size(63, 13);
			this.labelDescription.TabIndex = 5;
			this.labelDescription.Text = "&Description:";
			this.comboBoxConfigurations.DropDownStyle = ComboBoxStyle.DropDownList;
			this.comboBoxConfigurations.FormattingEnabled = true;
			this.comboBoxConfigurations.Location = new Point(87, 165);
			this.comboBoxConfigurations.Name = "comboBoxConfigurations";
			this.comboBoxConfigurations.Size = new System.Drawing.Size(240, 21);
			this.comboBoxConfigurations.TabIndex = 4;
			this.comboBoxConfigurations.Visible = false;
			this.labelConfiguration.AutoSize = true;
			this.labelConfiguration.Location = new Point(0, 168);
			this.labelConfiguration.Name = "labelConfiguration";
			this.labelConfiguration.Size = new System.Drawing.Size(72, 13);
			this.labelConfiguration.TabIndex = 3;
			this.labelConfiguration.Text = "&Configuration:";
			this.labelConfiguration.Visible = false;
			this.listViewOutputType.Columns.AddRange(new ColumnHeader[] { this.columnHeader1 });
			this.listViewOutputType.HeaderStyle = ColumnHeaderStyle.None;
			this.listViewOutputType.HideSelection = false;
			listViewItem.StateImageIndex = 0;
			listViewItem.Tag = ".Binaries";
			listViewItem.ToolTipText = "Contains the DLL or EXE built by the project.";
			listViewItem1.StateImageIndex = 0;
			listViewItem1.Tag = ".Satellites";
			listViewItem1.ToolTipText = "Contains the satellite assemblies for each culture's resources.";
			listViewItem2.StateImageIndex = 0;
			listViewItem2.Tag = ".Symbols";
			listViewItem2.ToolTipText = "Contains the debugging files for the project.";
			listViewItem3.StateImageIndex = 0;
			listViewItem3.Tag = ".Content";
			listViewItem3.ToolTipText = "Contains all content files in the project.";
			listViewItem4.StateImageIndex = 0;
			listViewItem4.Tag = ".Sources";
			listViewItem4.ToolTipText = "Contains all source files in the project.";
			listViewItem5.StateImageIndex = 0;
			listViewItem5.Tag = ".Documents";
			listViewItem5.ToolTipText = "Contains the XML Documentation files for the project.";
			this.listViewOutputType.Items.AddRange(new ListViewItem[] { listViewItem, listViewItem1, listViewItem2, listViewItem3, listViewItem4, listViewItem5 });
			this.listViewOutputType.Location = new Point(0, 40);
			this.listViewOutputType.MultiSelect = false;
			this.listViewOutputType.Name = "listViewOutputType";
			this.listViewOutputType.Size = new System.Drawing.Size(327, 113);
			this.listViewOutputType.TabIndex = 2;
			this.listViewOutputType.TileSize = new System.Drawing.Size(160, 1);
			this.listViewOutputType.UseCompatibleStateImageBehavior = false;
			this.listViewOutputType.View = View.List;
			this.listViewOutputType.SelectedIndexChanged += new EventHandler(this.listViewOutputType_SelectedIndexChanged);
			this.listViewOutputType.DoubleClick += new EventHandler(this.listViewOutputType_DoubleClick);
			this.columnHeader1.Width = 327;
			this.panelButtons.Controls.Add(this.buttonCancel);
			this.panelButtons.Controls.Add(this.buttonOK);
			this.panelButtons.Dock = DockStyle.Bottom;
			this.panelButtons.Location = new Point(8, 303);
			this.panelButtons.Name = "panelButtons";
			this.panelButtons.Size = new System.Drawing.Size(328, 36);
			this.panelButtons.TabIndex = 5;
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new Point(242, 8);
			this.buttonCancel.Margin = new System.Windows.Forms.Padding(2);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(86, 24);
			this.buttonCancel.TabIndex = 3;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Enabled = false;
			this.buttonOK.Location = new Point(152, 8);
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
			base.ClientSize = new System.Drawing.Size(344, 347);
			base.Controls.Add(this.panelControls);
			base.Controls.Add(this.panelButtons);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "FormAddProjectOutput";
			base.Padding = new System.Windows.Forms.Padding(8);
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Add Project Output Group";
			this.panelControls.ResumeLayout(false);
			this.panelControls.PerformLayout();
			this.panelButtons.ResumeLayout(false);
			base.ResumeLayout(false);
		}

		private void listViewOutputType_DoubleClick(object sender, EventArgs e)
		{
			if (this.buttonOK.Enabled)
			{
				this.buttonOK.PerformClick();
			}
		}

		private void listViewOutputType_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.textBoxDescription.Text = string.Empty;
			if (this.listViewOutputType.SelectedItems.Count > 0)
			{
				this.textBoxDescription.Text = this.listViewOutputType.SelectedItems[0].ToolTipText;
			}
		}
	}
}