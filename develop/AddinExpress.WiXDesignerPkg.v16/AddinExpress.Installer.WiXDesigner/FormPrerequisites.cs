using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Resources;
using System.Windows.Forms;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class FormPrerequisites : Form
	{
		private bool dirty;

		private bool loaded;

		private SortedList outerList;

		private SortedList innerList = new SortedList();

		private string dteVersion;

		private string imageRuntimeVersion = string.Empty;

		private Dictionary<string, Dictionary<string, string>> collection;

		private string sdkRoot = string.Empty;

		private string applicationName = string.Empty;

		private bool requiresElevation;

		private string culture = string.Empty;

		private string configuration = string.Empty;

		private IContainer components;

		private Button btnOk;

		private Button btnCancel;

		private CheckBox chkCreateSetup;

		private Label label1;

		private Label label2;

		private RadioButton rbtnVendor;

		private RadioButton rbtnMyApp;

		private Panel panel1;

		private RadioButton rbtnMyLocation;

		private TextBox tbMyLocation;

		private Button btnMyLocation;

		private ListView listView;

		private ColumnHeader prerequisiteHeader;

		private ImageList imageList1;

		private Label label3;

		private TextBox tbPackagesPath;

		private Button btnBrowsePackages;

		public string ApplicationName
		{
			get
			{
				return this.applicationName;
			}
			set
			{
				this.applicationName = value;
			}
		}

		public string Configuration
		{
			get
			{
				return this.configuration;
			}
			set
			{
				this.configuration = value;
			}
		}

		public string Culture
		{
			get
			{
				return this.culture;
			}
			set
			{
				this.culture = value;
				if (string.IsNullOrEmpty(this.culture))
				{
					this.culture = "en";
				}
			}
		}

		public string DTEVersion
		{
			get
			{
				return this.dteVersion;
			}
			set
			{
				this.dteVersion = value;
			}
		}

		public string PackagesPath
		{
			get
			{
				return this.sdkRoot;
			}
		}

		public bool RequiresElevation
		{
			get
			{
				return this.requiresElevation;
			}
			set
			{
				this.requiresElevation = value;
			}
		}

		public FormPrerequisites()
		{
			this.InitializeComponent();
			System.Drawing.Font environmentFont = VsShellUtilities.GetEnvironmentFont(VsPackage.CurrentInstance);
			if (environmentFont != null)
			{
				this.Font = environmentFont;
			}
		}

		public FormPrerequisites(Dictionary<string, Dictionary<string, string>> collection)
		{
			this.InitializeComponent();
			this.collection = collection;
			System.Drawing.Font environmentFont = VsShellUtilities.GetEnvironmentFont(VsPackage.CurrentInstance);
			if (environmentFont != null)
			{
				this.Font = environmentFont;
			}
		}

		private void btnBrowsePackages_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()
			{
				ShowNewFolderButton = false,
				Description = "Select the location of available prerequisite packages.",
				RootFolder = Environment.SpecialFolder.ProgramFilesX86,
				SelectedPath = this.tbPackagesPath.Text
			};
			if (folderBrowserDialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK && !string.IsNullOrEmpty(folderBrowserDialog.SelectedPath))
			{
				bool flag = false;
				try
				{
					string str = Path.Combine(folderBrowserDialog.SelectedPath, "Engine");
					if (Directory.Exists(str) && File.Exists(Path.Combine(str, "setup.bin")) && Directory.Exists(Path.Combine(folderBrowserDialog.SelectedPath, "Packages")))
					{
						string str1 = Path.Combine(folderBrowserDialog.SelectedPath, "Schemas");
						if (Directory.Exists(str1) && File.Exists(Path.Combine(str1, "Package.xsd")))
						{
							flag = true;
						}
					}
				}
				catch (Exception exception)
				{
				}
				if (flag)
				{
					this.tbPackagesPath.Text = folderBrowserDialog.SelectedPath;
					return;
				}
				MessageBox.Show(this, "This folder doesn't match the structure of the Visual Studio Bootstrapper directory. Please select the correct directory.", "Designer for Visual Studio WiX Setup Projects", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		private void btnMyLocation_Click(object sender, EventArgs e)
		{
			Microsoft.VisualStudio.OLE.Interop.IServiceProvider serviceObject = VsPackage.CurrentInstance.GetServiceObject(typeof(Microsoft.VisualStudio.OLE.Interop.IServiceProvider)) as Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
			if (serviceObject != null)
			{
				using (ServiceProvider serviceProvider = new ServiceProvider(serviceObject))
				{
					IVsWebConnectionDlgSvc service = serviceProvider.GetService(new Guid("{E4A1263A-11CF-4c5f-B3D5-E41AFA0ADE59}")) as IVsWebConnectionDlgSvc;
					if (service != null)
					{
						int num = 1;
						int num1 = 0;
						object obj = null;
						string empty = string.Empty;
						service.WebConnectionDlg(8, out empty, out num1, out obj, out num);
						if (num == 0)
						{
							this.tbMyLocation.Text = empty;
						}
					}
				}
			}
		}

		private void chkCreateSetup_CheckedChanged(object sender, EventArgs e)
		{
			if (this.loaded)
			{
				this.dirty = true;
			}
			this.RefreshState();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private static string GetGenericBootstrapperDirectory(string vsVersion)
		{
			string str = "2.0";
			string empty = string.Empty;
			if (vsVersion != null)
			{
				if (vsVersion == "9.0")
				{
					str = "3.5";
				}
				else if (vsVersion == "10.0")
				{
					str = "4.0";
				}
				else if (vsVersion == "11.0")
				{
					str = "11.0";
				}
				else if (vsVersion == "12.0")
				{
					str = "12.0";
				}
				else if (vsVersion == "14.0")
				{
					str = "14.0";
				}
			}
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(string.Concat("Software\\Microsoft\\GenericBootstrapper\\", str), false))
			{
				if (registryKey != null)
				{
					empty = (string)registryKey.GetValue("Path", string.Empty);
					if (!string.IsNullOrEmpty(empty) && Directory.Exists(empty) && Directory.GetDirectories(empty).Length == 0)
					{
						empty = string.Empty;
					}
				}
			}
			return empty;
		}

		public Dictionary<string, Dictionary<string, string>> GetPrerequisites()
		{
			Dictionary<string, string> strs;
			if (this.dirty)
			{
				this.collection = new Dictionary<string, Dictionary<string, string>>();
				if (this.chkCreateSetup.Checked)
				{
					strs = new Dictionary<string, string>();
					strs["ApplicationFile"] = "$(TargetFileName)";
					if (!string.IsNullOrEmpty(this.ApplicationName))
					{
						strs["ApplicationName"] = this.ApplicationName;
					}
					strs["BootstrapperItems"] = "@(BootstrapperFile)";
					if (this.rbtnVendor.Checked)
					{
						strs["ComponentsLocation"] = "HomeSite";
						strs["CopyComponents"] = "False";
					}
					else if (this.rbtnMyApp.Checked)
					{
						strs["ComponentsLocation"] = "Relative";
						strs["CopyComponents"] = "True";
					}
					else if (this.rbtnMyLocation.Checked)
					{
						strs["ComponentsLocation"] = "Absolute";
						strs["CopyComponents"] = "False";
						string str = this.tbMyLocation.Text.Trim();
						if (!string.IsNullOrEmpty(str))
						{
							strs["ComponentsUrl"] = str;
						}
					}
					strs["OutputPath"] = "$(OutputPath)";
					if (!string.IsNullOrEmpty(this.sdkRoot))
					{
						strs["Path"] = this.sdkRoot;
					}
					strs["ApplicationRequiresElevation"] = (this.RequiresElevation ? "True" : "False");
					if (!string.IsNullOrEmpty(this.Culture))
					{
						strs["Culture"] = this.Culture;
					}
					strs["Condition"] = string.Concat(" '$(Configuration)|$(Platform)' == '", this.Configuration, "' ");
					this.collection["GenerateBootstrapper"] = strs;
				}
				for (int i = 0; i < this.listView.Items.Count; i++)
				{
					if (this.listView.Items[i].Checked)
					{
						strs = new Dictionary<string, string>();
						strs["ProductName"] = (string)this.innerList.GetValueList()[i];
						this.collection[(string)this.innerList.GetKeyList()[i]] = strs;
					}
				}
			}
			return this.collection;
		}

		private string GetProductCode(string fileName)
		{
			if (File.Exists(fileName))
			{
				XmlDocument xmlDocument = new XmlDocument();
				XmlTextReader xmlTextReader = null;
				try
				{
					xmlTextReader = new XmlTextReader(fileName);
					xmlDocument.Load(xmlTextReader);
				}
				finally
				{
					if (xmlTextReader != null)
					{
						xmlTextReader.Close();
					}
				}
				XmlElement documentElement = xmlDocument.DocumentElement;
				if (documentElement.HasAttributes)
				{
					XmlAttribute itemOf = documentElement.Attributes["ProductCode"];
					if (itemOf != null)
					{
						return itemOf.Value.Trim();
					}
				}
			}
			return string.Empty;
		}

		private string GetProductName(string fileName)
		{
			if (File.Exists(fileName))
			{
				XmlDocument xmlDocument = new XmlDocument();
				XmlTextReader xmlTextReader = null;
				try
				{
					xmlTextReader = new XmlTextReader(fileName);
					xmlDocument.Load(xmlTextReader);
				}
				finally
				{
					if (xmlTextReader != null)
					{
						xmlTextReader.Close();
					}
				}
				XmlElement documentElement = xmlDocument.DocumentElement;
				if (documentElement.HasAttributes)
				{
					XmlAttribute itemOf = documentElement.Attributes["Name"];
					if (itemOf != null)
					{
						string str = itemOf.Value.Trim();
						if (!string.IsNullOrEmpty(str))
						{
							XmlElement item = documentElement["Strings"];
							if (item != null && item.HasChildNodes)
							{
								XmlNodeList childNodes = item.ChildNodes;
								for (int i = 0; i < childNodes.Count; i++)
								{
									XmlNode xmlNodes = childNodes[i];
									if (xmlNodes.HasChildNodes)
									{
										itemOf = xmlNodes.Attributes["Name"];
										if (itemOf != null && str.Equals(itemOf.Value.Trim(), StringComparison.OrdinalIgnoreCase))
										{
											return xmlNodes.FirstChild.Value.Trim();
										}
									}
								}
							}
						}
					}
				}
			}
			return string.Empty;
		}

		public static string GetSDKRootDirectory(string sdkVersion, out bool found)
		{
			string empty = string.Empty;
			found = false;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(string.Concat("SOFTWARE\\Microsoft\\Microsoft SDKs\\Windows\\", sdkVersion), false))
			{
				if (registryKey != null)
				{
					empty = (string)registryKey.GetValue("InstallationFolder", string.Empty);
					if (!string.IsNullOrEmpty(empty))
					{
						if (!Directory.Exists(Path.Combine(empty, "BootStrapper")))
						{
							empty = string.Empty;
							goto Label0;
						}
						else if (Directory.GetDirectories(Path.Combine(empty, "BootStrapper")).Length == 0)
						{
							empty = string.Empty;
						}
					}
				}
			}
		Label0:
			if (!string.IsNullOrEmpty(empty))
			{
				found = true;
			}
			if (string.IsNullOrEmpty(empty))
			{
				using (RegistryKey registryKey1 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Microsoft SDKs\\Windows", false))
				{
					if (registryKey1 != null)
					{
						empty = (string)registryKey1.GetValue("CurrentInstallFolder", string.Empty);
						if (!string.IsNullOrEmpty(empty))
						{
							if (!Directory.Exists(Path.Combine(empty, "BootStrapper")))
							{
								empty = string.Empty;
								if (string.IsNullOrEmpty(empty))
								{
									using (RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\GenericBootstrapper\\3.5", false))
									{
										if (registryKey2 != null)
										{
											empty = (string)registryKey2.GetValue("Path", string.Empty);
											if (!string.IsNullOrEmpty(empty))
											{
												empty = Path.GetFullPath(Path.Combine(empty, "..\\"));
											}
										}
									}
								}
								return empty;
							}
							else if (Directory.GetDirectories(Path.Combine(empty, "BootStrapper")).Length == 0)
							{
								empty = string.Empty;
							}
						}
					}
				}
			}
			if (string.IsNullOrEmpty(empty))
			{
				using (registryKey2 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\GenericBootstrapper\\3.5", false))
				{
					if (registryKey2 != null)
					{
						empty = (string)registryKey2.GetValue("Path", string.Empty);
						if (!string.IsNullOrEmpty(empty))
						{
							empty = Path.GetFullPath(Path.Combine(empty, "..\\"));
						}
					}
				}
			}
			return empty;
		}

		private static string GetVSSDKRootDirectory(string vsVersion)
		{
			string empty = string.Empty;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(string.Concat("SOFTWARE\\Microsoft\\VisualStudio\\", vsVersion), false))
			{
				if (registryKey != null)
				{
					empty = (string)registryKey.GetValue("ShellFolder", string.Empty);
					if (string.IsNullOrEmpty(empty))
					{
						empty = (string)registryKey.GetValue("InstallDir", string.Empty);
						if (!string.IsNullOrEmpty(empty))
						{
							empty = Path.GetFullPath(Path.Combine(empty, "..\\..\\SDK"));
						}
					}
					else
					{
						empty = Path.Combine(empty, "SDK");
					}
					if (!string.IsNullOrEmpty(empty))
					{
						string str = Path.Combine(empty, "BootStrapper");
						if (!Directory.Exists(str))
						{
							empty = string.Empty;
							return empty;
						}
						else if (Directory.GetDirectories(str).Length == 0)
						{
							empty = string.Empty;
						}
					}
				}
			}
			return empty;
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(FormPrerequisites));
			this.btnOk = new Button();
			this.btnCancel = new Button();
			this.chkCreateSetup = new CheckBox();
			this.label1 = new Label();
			this.label2 = new Label();
			this.rbtnVendor = new RadioButton();
			this.rbtnMyApp = new RadioButton();
			this.panel1 = new Panel();
			this.rbtnMyLocation = new RadioButton();
			this.tbMyLocation = new TextBox();
			this.btnMyLocation = new Button();
			this.listView = new ListView();
			this.prerequisiteHeader = new ColumnHeader();
			this.imageList1 = new ImageList(this.components);
			this.label3 = new Label();
			this.tbPackagesPath = new TextBox();
			this.btnBrowsePackages = new Button();
			base.SuspendLayout();
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Location = new Point(383, 467);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(87, 25);
			this.btnOk.TabIndex = 10;
			this.btnOk.Text = "OK";
			this.btnOk.UseVisualStyleBackColor = true;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new Point(477, 467);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(87, 25);
			this.btnCancel.TabIndex = 11;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.chkCreateSetup.AutoSize = true;
			this.chkCreateSetup.Location = new Point(14, 13);
			this.chkCreateSetup.Name = "chkCreateSetup";
			this.chkCreateSetup.Size = new System.Drawing.Size(329, 19);
			this.chkCreateSetup.TabIndex = 0;
			this.chkCreateSetup.Text = "Create setup program to install prerequisites components";
			this.chkCreateSetup.UseVisualStyleBackColor = true;
			this.chkCreateSetup.CheckedChanged += new EventHandler(this.chkCreateSetup_CheckedChanged);
			this.label1.AutoSize = true;
			this.label1.Location = new Point(14, 103);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(203, 15);
			this.label1.TabIndex = 3;
			this.label1.Text = "Choose which prerequisites to install:";
			this.label2.AutoSize = true;
			this.label2.Location = new Point(14, 323);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(233, 15);
			this.label2.TabIndex = 5;
			this.label2.Text = "Specify the install location for prerequisites";
			this.rbtnVendor.AutoSize = true;
			this.rbtnVendor.Checked = true;
			this.rbtnVendor.Location = new Point(34, 349);
			this.rbtnVendor.Name = "rbtnVendor";
			this.rbtnVendor.Size = new System.Drawing.Size(357, 19);
			this.rbtnVendor.TabIndex = 5;
			this.rbtnVendor.TabStop = true;
			this.rbtnVendor.Text = "Download prerequisites from the component vendor's web site";
			this.rbtnVendor.UseVisualStyleBackColor = true;
			this.rbtnVendor.CheckedChanged += new EventHandler(this.rbtnVendor_CheckedChanged);
			this.rbtnMyApp.AutoSize = true;
			this.rbtnMyApp.Location = new Point(34, 373);
			this.rbtnMyApp.Name = "rbtnMyApp";
			this.rbtnMyApp.Size = new System.Drawing.Size(371, 19);
			this.rbtnMyApp.TabIndex = 6;
			this.rbtnMyApp.Text = "Download prerequisites from the same location as my application";
			this.rbtnMyApp.UseVisualStyleBackColor = true;
			this.rbtnMyApp.CheckedChanged += new EventHandler(this.rbtnMyApp_CheckedChanged);
			this.panel1.BackColor = SystemColors.ControlDarkDark;
			this.panel1.Location = new Point(252, 330);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(309, 1);
			this.panel1.TabIndex = 8;
			this.rbtnMyLocation.AutoSize = true;
			this.rbtnMyLocation.Location = new Point(34, 397);
			this.rbtnMyLocation.Name = "rbtnMyLocation";
			this.rbtnMyLocation.Size = new System.Drawing.Size(300, 19);
			this.rbtnMyLocation.TabIndex = 7;
			this.rbtnMyLocation.Text = "Download prerequisites from the following location:";
			this.rbtnMyLocation.UseVisualStyleBackColor = true;
			this.rbtnMyLocation.CheckedChanged += new EventHandler(this.rbtnMyLocation_CheckedChanged);
			this.tbMyLocation.Enabled = false;
			this.tbMyLocation.Location = new Point(52, 426);
			this.tbMyLocation.Name = "tbMyLocation";
			this.tbMyLocation.Size = new System.Drawing.Size(422, 23);
			this.tbMyLocation.TabIndex = 8;
			this.tbMyLocation.TextChanged += new EventHandler(this.tbMyLocation_TextChanged);
			this.btnMyLocation.Enabled = false;
			this.btnMyLocation.Location = new Point(477, 425);
			this.btnMyLocation.Name = "btnMyLocation";
			this.btnMyLocation.Size = new System.Drawing.Size(87, 25);
			this.btnMyLocation.TabIndex = 9;
			this.btnMyLocation.Text = "Browse...";
			this.btnMyLocation.UseVisualStyleBackColor = true;
			this.btnMyLocation.Click += new EventHandler(this.btnMyLocation_Click);
			this.listView.CheckBoxes = true;
			this.listView.Columns.AddRange(new ColumnHeader[] { this.prerequisiteHeader });
			this.listView.FullRowSelect = true;
			this.listView.HeaderStyle = ColumnHeaderStyle.None;
			this.listView.HideSelection = false;
			this.listView.LabelWrap = false;
			this.listView.Location = new Point(17, 122);
			this.listView.MultiSelect = false;
			this.listView.Name = "listView";
			this.listView.ShowGroups = false;
			this.listView.Size = new System.Drawing.Size(546, 185);
			this.listView.SmallImageList = this.imageList1;
			this.listView.TabIndex = 4;
			this.listView.UseCompatibleStateImageBehavior = false;
			this.listView.View = View.Details;
			this.listView.ItemChecked += new ItemCheckedEventHandler(this.listView_ItemChecked);
			this.prerequisiteHeader.Text = "Prerequisite Name";
			this.prerequisiteHeader.Width = 550;
			this.imageList1.ImageStream = (ImageListStreamer)componentResourceManager.GetObject("imageList1.ImageStream");
			this.imageList1.TransparentColor = Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "Prerequisite.ico");
			this.label3.AutoSize = true;
			this.label3.Location = new Point(14, 45);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(110, 15);
			this.label3.TabIndex = 9;
			this.label3.Text = "Packages Directory:";
			this.tbPackagesPath.BackColor = SystemColors.Window;
			this.tbPackagesPath.Location = new Point(17, 63);
			this.tbPackagesPath.Name = "tbPackagesPath";
			this.tbPackagesPath.ReadOnly = true;
			this.tbPackagesPath.Size = new System.Drawing.Size(457, 23);
			this.tbPackagesPath.TabIndex = 1;
			this.tbPackagesPath.TextChanged += new EventHandler(this.tbPackagesPath_TextChanged);
			this.btnBrowsePackages.Enabled = false;
			this.btnBrowsePackages.Location = new Point(477, 62);
			this.btnBrowsePackages.Name = "btnBrowsePackages";
			this.btnBrowsePackages.Size = new System.Drawing.Size(87, 25);
			this.btnBrowsePackages.TabIndex = 2;
			this.btnBrowsePackages.Text = "Browse...";
			this.btnBrowsePackages.UseVisualStyleBackColor = true;
			this.btnBrowsePackages.Click += new EventHandler(this.btnBrowsePackages_Click);
			base.AcceptButton = this.btnOk;
			base.AutoScaleDimensions = new SizeF(96f, 96f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			base.CancelButton = this.btnCancel;
			base.ClientSize = new System.Drawing.Size(579, 506);
			base.Controls.Add(this.btnBrowsePackages);
			base.Controls.Add(this.tbPackagesPath);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.listView);
			base.Controls.Add(this.btnMyLocation);
			base.Controls.Add(this.tbMyLocation);
			base.Controls.Add(this.rbtnMyLocation);
			base.Controls.Add(this.panel1);
			base.Controls.Add(this.rbtnMyApp);
			base.Controls.Add(this.rbtnVendor);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.chkCreateSetup);
			base.Controls.Add(this.btnCancel);
			base.Controls.Add(this.btnOk);
			this.Font = new System.Drawing.Font("Segoe UI", 9f, FontStyle.Regular, GraphicsUnit.Point, 204);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "FormPrerequisites";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Prerequisites";
			base.Load += new EventHandler(this.PrerequisitesForm_Load);
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void listBox_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			if (this.loaded)
			{
				this.dirty = true;
			}
		}

		private void listView_ItemChecked(object sender, ItemCheckedEventArgs e)
		{
			if (this.loaded)
			{
				this.dirty = true;
			}
		}

		public void ParsePrerequisites()
		{
			this.outerList = new SortedList();
			foreach (KeyValuePair<string, Dictionary<string, string>> item in this.collection)
			{
				if (item.Key != "GenerateBootstrapper")
				{
					if (!item.Value.ContainsKey("ProductName"))
					{
						continue;
					}
					this.outerList[item.Key] = item.Value["ProductName"];
				}
				else
				{
					if (item.Value.ContainsKey("Path"))
					{
						this.sdkRoot = item.Value["Path"];
					}
					this.tbPackagesPath.Text = this.sdkRoot;
					this.chkCreateSetup.Checked = true;
					string str = item.Value["ComponentsLocation"];
					if (str != null)
					{
						if (str == "Relative")
						{
							this.rbtnMyApp.Checked = true;
							goto Label0;
						}
						else
						{
							if (str != "Absolute")
							{
								goto Label2;
							}
							this.rbtnMyLocation.Checked = true;
							goto Label0;
						}
					}
				Label2:
					this.rbtnVendor.Checked = true;
				Label0:
					if (!item.Value.ContainsKey("ComponentsUrl"))
					{
						continue;
					}
					this.tbMyLocation.Text = item.Value["ComponentsUrl"];
				}
			}
			this.RefreshState();
		}

		private void PrerequisitesForm_Load(object sender, EventArgs e)
		{
			bool flag;
			bool flag1;
			bool flag2;
			bool flag3;
			try
			{
				try
				{
					this.innerList.Clear();
					if (string.IsNullOrEmpty(this.sdkRoot))
					{
						string fileName = Path.GetFileName(this.dteVersion);
						this.sdkRoot = FormPrerequisites.GetGenericBootstrapperDirectory(fileName);
						if (string.IsNullOrEmpty(this.sdkRoot))
						{
							this.sdkRoot = FormPrerequisites.GetVSSDKRootDirectory(fileName);
							if (string.IsNullOrEmpty(this.sdkRoot))
							{
								if (fileName == "10.0")
								{
									this.sdkRoot = FormPrerequisites.GetSDKRootDirectory("v7.0A", out flag);
								}
								else if (fileName == "11.0")
								{
									this.sdkRoot = FormPrerequisites.GetSDKRootDirectory("v8.0A", out flag1);
									if (!flag1)
									{
										this.sdkRoot = FormPrerequisites.GetSDKRootDirectory("v7.1A", out flag1);
										if (!flag1)
										{
											this.sdkRoot = FormPrerequisites.GetSDKRootDirectory("v7.0A", out flag1);
										}
									}
								}
								else if (fileName == "12.0")
								{
									this.sdkRoot = FormPrerequisites.GetSDKRootDirectory("v8.1A", out flag2);
									if (!flag2)
									{
										this.sdkRoot = FormPrerequisites.GetSDKRootDirectory("v8.0A", out flag2);
										if (!flag2)
										{
											this.sdkRoot = FormPrerequisites.GetSDKRootDirectory("v7.1A", out flag2);
											if (!flag2)
											{
												this.sdkRoot = FormPrerequisites.GetSDKRootDirectory("v7.0A", out flag2);
											}
										}
									}
								}
								else if (fileName != "14.0")
								{
									using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\.NETFramework", false))
									{
										if (registryKey != null)
										{
											this.sdkRoot = (string)registryKey.GetValue("sdkInstallRootv2.0", string.Empty);
										}
									}
								}
								else
								{
									this.sdkRoot = FormPrerequisites.GetSDKRootDirectory("v10.0A", out flag3);
									if (!flag3)
									{
										this.sdkRoot = FormPrerequisites.GetSDKRootDirectory("v8.1A", out flag3);
										if (!flag3)
										{
											this.sdkRoot = FormPrerequisites.GetSDKRootDirectory("v8.0A", out flag3);
											if (!flag3)
											{
												this.sdkRoot = FormPrerequisites.GetSDKRootDirectory("v7.1A", out flag3);
												if (!flag3)
												{
													this.sdkRoot = FormPrerequisites.GetSDKRootDirectory("v7.0A", out flag3);
												}
											}
										}
									}
								}
							}
							if (!string.IsNullOrEmpty(this.sdkRoot))
							{
								this.sdkRoot = Path.Combine(this.sdkRoot, "BootStrapper");
							}
						}
					}
					this.tbPackagesPath.Text = this.sdkRoot;
					this.UpdatePrerequisitesList();
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					MessageBox.Show(this, exception.Message, "Designer for Visual Studio WiX Setup Projects", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}
			}
			finally
			{
				this.loaded = true;
				this.chkCreateSetup.Select();
			}
		}

		private void rbtnMyApp_CheckedChanged(object sender, EventArgs e)
		{
			if (this.loaded)
			{
				this.dirty = true;
			}
		}

		private void rbtnMyLocation_CheckedChanged(object sender, EventArgs e)
		{
			if (this.loaded)
			{
				this.dirty = true;
			}
			this.btnMyLocation.Enabled = this.rbtnMyLocation.Checked;
			this.tbMyLocation.Enabled = this.rbtnMyLocation.Checked;
			if (this.btnMyLocation.Enabled)
			{
				this.btnMyLocation.Select();
			}
		}

		private void rbtnVendor_CheckedChanged(object sender, EventArgs e)
		{
			if (this.loaded)
			{
				this.dirty = true;
			}
		}

		private void RefreshState()
		{
			this.listView.Enabled = this.chkCreateSetup.Checked;
			this.rbtnVendor.Enabled = this.chkCreateSetup.Checked;
			this.rbtnMyApp.Enabled = this.chkCreateSetup.Checked;
			this.rbtnMyLocation.Enabled = this.chkCreateSetup.Checked;
			this.tbPackagesPath.Enabled = this.chkCreateSetup.Checked;
			this.btnBrowsePackages.Enabled = this.chkCreateSetup.Checked;
			if (!this.rbtnMyLocation.Enabled)
			{
				this.btnMyLocation.Enabled = false;
				this.tbMyLocation.Enabled = false;
			}
			else if (this.rbtnMyLocation.Checked)
			{
				this.btnMyLocation.Enabled = true;
				this.tbMyLocation.Enabled = true;
				return;
			}
		}

		private void tbMyLocation_TextChanged(object sender, EventArgs e)
		{
			if (this.loaded)
			{
				this.dirty = true;
			}
		}

		private void tbPackagesPath_TextChanged(object sender, EventArgs e)
		{
			if (this.loaded)
			{
				this.dirty = true;
				this.sdkRoot = this.tbPackagesPath.Text;
				this.UpdatePrerequisitesList();
			}
		}

		private void UpdatePrerequisitesList()
		{
			this.listView.Items.Clear();
			this.innerList.Clear();
			if (!string.IsNullOrEmpty(this.sdkRoot))
			{
				string[] files = Directory.GetFiles(this.sdkRoot, "product.xml", SearchOption.AllDirectories);
				if (files.Length != 0)
				{
					string empty = string.Empty;
					string[] strArrays = this.culture.Split(new char[] { '-' });
					if (strArrays.Length != 0)
					{
						empty = strArrays[0];
					}
					for (int i = 0; i < (int)files.Length; i++)
					{
						string[] files1 = Directory.GetFiles(Path.GetDirectoryName(files[i]), "package.xml", SearchOption.AllDirectories);
						if (files1.Length != 0)
						{
							int num = 0;
							int num1 = 0;
							bool flag = false;
							if (!string.IsNullOrEmpty(empty))
							{
								string str = string.Concat("\\", empty);
								int num2 = 0;
								while (num2 < (int)files1.Length)
								{
									string directoryName = Path.GetDirectoryName(files1[num2]);
									if (!directoryName.EndsWith(str, StringComparison.OrdinalIgnoreCase))
									{
										if (directoryName.EndsWith("\\en", StringComparison.OrdinalIgnoreCase))
										{
											num = num2;
										}
										num2++;
									}
									else
									{
										num1 = num2;
										flag = true;
										break;
									}
								}
							}
							if (!flag)
							{
								num1 = num;
							}
							this.innerList[this.GetProductCode(files[i])] = this.GetProductName(files1[num1]);
						}
					}
				}
			}
			if (this.innerList.Count > 0)
			{
				for (int j = 0; j < this.innerList.Count; j++)
				{
					ListViewItem listViewItem = new ListViewItem()
					{
						Text = (string)this.innerList.GetValueList()[j],
						ImageIndex = 0
					};
					this.listView.Items.Add(listViewItem);
				}
			}
			if (this.outerList != null)
			{
				for (int k = 0; k < this.outerList.Count; k++)
				{
					int num3 = this.innerList.IndexOfKey(this.outerList.GetKeyList()[k]);
					if (num3 >= 0)
					{
						this.listView.Items[num3].Checked = true;
					}
				}
			}
		}
	}
}