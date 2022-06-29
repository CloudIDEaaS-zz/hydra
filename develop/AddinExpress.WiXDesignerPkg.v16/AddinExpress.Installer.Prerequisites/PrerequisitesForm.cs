using AddinExpress.Installer.Prerequisites.Manifests;
using AddinExpress.Installer.WiXDesigner.MyPrerequisites;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace AddinExpress.Installer.Prerequisites
{
	internal class PrerequisitesForm : Form, IProject
	{
		private static List<WeakReference> __ENCList;

		private ToolStripMenuItem addNewFileMenuItem;

		private ToolStripMenuItem cloneFileMenuItem;

		private System.Windows.Forms.ContextMenuStrip ViewPackagesCtx;

		private ImageList imageList16;

		private OpenFileDialog OpenFileDialog1;

		private ToolStripMenuItem removeMenuItem;

		private SaveFileDialog SaveFileDialog1;

		private TreeView ViewPackages;

		internal PrerequisitesForm.AddFileInfo AddFileResults;

		private IContainer components;

		private const string defaultBuildOutputDirectoryName = "Temporary Build Output";

		private const string internetExplorerExecutable = "ie.exe";

		private bool m_Dirty;

		private int m_errors;

		private string m_FileFilter;

		private string m_LoadedFromFile;

		private FileInfo m_ProjectFile;

		private AddinExpress.Installer.Prerequisites.ProjectProperties m_ProjectProperties;

		private int m_warnings;

		private string m_projectDir = string.Empty;

		private string m_projectName = string.Empty;

		private string dteVersion;

		private string sdkDir = string.Empty;

		private bool uiLocked;

		private Panel pnlTop;

		private Panel pnlItem;

		private Button btnCancel;

		private Panel panel3;

		private MyToolStrip toolStrip;

		private ToolStripButton btnAddFile;

		private ToolStripButton btnCloneFile;

		private ToolStripButton btnRemove;

		private ToolStripSeparator toolStripSeparator1;

		private ToolStripButton btnBuildPkg;

		private Panel pnlBottom;

		private ToolStripButton btnAddPkg;

		private ToolStripSeparator toolStripSeparator2;

		private Button btnOK;

		private ToolStripMenuItem addNewPackageMenuItem;

		private ToolStripMenuItem buildPackageMenuItem;

		private ToolStripSeparator toolStripMenuItem1;

		private ToolStripSeparator toolStripMenuItem2;

		internal static PrerequisitesForm Instance;

		internal static string DefaultFileExtension;

		private AddinExpress.Installer.Prerequisites.BuildResultsControl BuildResultsControl;

		private System.Windows.Forms.StatusBar StatusBar;

		private ToolTip toolTip1;

		private ImageList imageList20;

		private ImageList imageList24;

		private ImageList imageList32;

		internal string RootProjectElementName;

		string AddinExpress.Installer.Prerequisites.IProject.OpenProjectFilter
		{
			get
			{
				return this.m_FileFilter;
			}
		}

		FileInfo AddinExpress.Installer.Prerequisites.IProject.PathToProjectFile
		{
			get
			{
				return this.m_ProjectFile;
			}
		}

		string AddinExpress.Installer.Prerequisites.IProject.ProjectFileRootXML
		{
			get
			{
				return this.RootProjectElementName;
			}
		}

		string AddinExpress.Installer.Prerequisites.IProject.ProjectName
		{
			get
			{
				return this.m_ProjectProperties.ProjectName;
			}
		}

		string AddinExpress.Installer.Prerequisites.IProject.ProjectTypeDescription
		{
			get
			{
				return "Creates Bootstrapper Manifest Files that allow an Installer or Component to be added and built in a Bootstrapper.";
			}
		}

		string AddinExpress.Installer.Prerequisites.IProject.ProjectTypeName
		{
			get
			{
				return "Package Manifest";
			}
		}

		ToolStrip AddinExpress.Installer.Prerequisites.IProject.ToolstripToMerge
		{
			get
			{
				return this.toolStrip;
			}
		}

		public string DTEVersion
		{
			get
			{
				return this.dteVersion;
			}
		}

		public bool IsDirty
		{
			get
			{
				bool dirtyState;
				IEnumerator enumerator = null;
				if (this.m_Dirty)
				{
					return true;
				}
				try
				{
					if (this.ViewPackages.TopNode.Nodes.Count <= 0)
					{
						return this.m_Dirty;
					}
					else
					{
						enumerator = this.ViewPackages.TopNode.Nodes.GetEnumerator();
						dirtyState = this.GetDirtyState(enumerator);
					}
				}
				finally
				{
					if (enumerator is IDisposable)
					{
						(enumerator as IDisposable).Dispose();
					}
				}
				return dirtyState;
			}
			set
			{
				this.m_Dirty = value;
			}
		}

		internal string OpenProjectFilter
		{
			get
			{
				return this.m_FileFilter;
			}
		}

		public FileInfo PathToProjectFile
		{
			get
			{
				return this.m_ProjectFile;
			}
		}

		public string ProjectFileRootXML
		{
			get
			{
				return this.RootProjectElementName;
			}
		}

		internal string ProjectName
		{
			get
			{
				return this.m_ProjectProperties.ProjectName;
			}
		}

		public AddinExpress.Installer.Prerequisites.ProjectProperties ProjectProperties
		{
			get
			{
				return this.m_ProjectProperties;
			}
		}

		public string ProjectTypeDescription
		{
			get
			{
				return "Creates Bootstrapper Manifest Files that allow an Installer or Component to be added and built in a Bootstrapper.";
			}
		}

		public string ProjectTypeName
		{
			get
			{
				return "Package Manifest";
			}
		}

		public ToolStrip ToolstripToMerge
		{
			get
			{
				return this.toolStrip;
			}
		}

		static PrerequisitesForm()
		{
			PrerequisitesForm.__ENCList = new List<WeakReference>();
			PrerequisitesForm.Instance = null;
			PrerequisitesForm.DefaultFileExtension = "myprerequisites";
		}

		public PrerequisitesForm()
		{
			lock (PrerequisitesForm.__ENCList)
			{
				PrerequisitesForm.__ENCList.Add(new WeakReference(this));
			}
			PrerequisitesForm.DefaultFileExtension = "myprerequisites";
			this.RootProjectElementName = PrerequisitesForm.DefaultFileExtension;
			this.m_FileFilter = string.Concat("Manifest Generator Files (*.", PrerequisitesForm.DefaultFileExtension, ")|*.", PrerequisitesForm.DefaultFileExtension);
			this.InitializeComponent();
		}

		public PrerequisitesForm(string dteVersion, string projectName, string projectDir, string sdkDir)
		{
			PrerequisitesForm.Instance = this;
			Paths.Init(dteVersion);
			this.m_projectDir = projectDir;
			this.m_projectName = projectName;
			this.dteVersion = dteVersion;
			this.sdkDir = sdkDir;
			base.Load += new EventHandler(this.PrerequisitesForm_Load);
			lock (PrerequisitesForm.__ENCList)
			{
				PrerequisitesForm.__ENCList.Add(new WeakReference(this));
			}
			PrerequisitesForm.DefaultFileExtension = "myprerequisites";
			this.RootProjectElementName = PrerequisitesForm.DefaultFileExtension;
			this.m_FileFilter = string.Concat("Manifest Generator Files (*.", PrerequisitesForm.DefaultFileExtension, ")|*.", PrerequisitesForm.DefaultFileExtension);
			this.InitializeComponent();
		}

		void AddinExpress.Installer.Prerequisites.IProject.NewProject()
		{
		}

		void AddinExpress.Installer.Prerequisites.IProject.OpenProject(FileInfo openProjectFile)
		{
		}

		private UIInstallFile AddNewInstallFile(bool askUserForFile)
		{
			UIInstallFile uIInstallFile;
			CultureInfo langauge;
			TreeNode treeNode = new TreeNode();
			if (!askUserForFile)
			{
				uIInstallFile = new UIInstallFile(treeNode, (UIPackage)this.ViewPackages.SelectedNode.Tag);
			}
			else
			{
				if ((new AddFile(this)).ShowDialog(this) != System.Windows.Forms.DialogResult.OK)
				{
					return null;
				}
				if (!this.AddFileResults.Langauge.Equals(CultureInfo.InvariantCulture))
				{
					langauge = this.AddFileResults.Langauge;
				}
				else
				{
					langauge = CultureInfo.InvariantCulture;
					if (!langauge.IsNeutralCulture)
					{
						langauge = langauge.Parent;
					}
				}
				uIInstallFile = new UIInstallFile(this.AddFileResults.FilePath, langauge, treeNode, (UIPackage)this.ViewPackages.SelectedNode.Tag);
				uIInstallFile.AutoDetectAndFillFileSecurity();
			}
			treeNode.ImageIndex = 2;
			treeNode.SelectedImageIndex = 2;
			treeNode.Tag = uIInstallFile;
			this.ViewPackages.SelectedNode.Nodes.Add(treeNode);
			this.ViewPackages.SelectedNode.Expand();
			if (askUserForFile)
			{
				this.ViewPackages.SelectedNode = treeNode;
				this.ViewPackages.Select();
			}
			this.m_Dirty = true;
			return uIInstallFile;
		}

		private UIPackage AddNewPackage()
		{
			TreeNode treeNode = new TreeNode();
			UIPackage uIPackage = new UIPackage(treeNode, this);
			treeNode.Text = "<Unknown Product>";
			treeNode.ImageIndex = 1;
			treeNode.SelectedImageIndex = 1;
			treeNode.Tag = uIPackage;
			this.ViewPackages.TopNode.Nodes.Add(treeNode);
			this.ViewPackages.TopNode.Expand();
			this.ViewPackages.SelectedNode = treeNode;
			this.ViewPackages.Select();
			this.m_Dirty = true;
			return uIPackage;
		}

		private void btnAddFile_Click(object sender, EventArgs e)
		{
			this.AddNewInstallFile(true);
		}

		private void btnAddPkg_Click(object sender, EventArgs e)
		{
			this.AddNewPackage();
		}

		private void btnBuildPkg_Click(object sender, EventArgs e)
		{
			this.Build(true);
		}

		private void btnCloneFile_Click(object sender, EventArgs e)
		{
			if (this.ViewPackages.SelectedNode.Tag is UIInstallFile)
			{
				XmlElement xmlElement = (new XmlDocument()).CreateElement("CopyNode");
				((UIInstallFile)this.ViewPackages.SelectedNode.Tag).SaveToFile(xmlElement);
				TreeNode treeNode = new TreeNode();
				UIInstallFile uIInstallFile = new UIInstallFile(treeNode, (UIPackage)this.ViewPackages.SelectedNode.Parent.Tag);
				treeNode.ImageIndex = 2;
				treeNode.SelectedImageIndex = 2;
				treeNode.Tag = uIInstallFile;
				this.ViewPackages.SelectedNode.Parent.Nodes.Add(treeNode);
				this.ViewPackages.SelectedNode.Parent.Expand();
				uIInstallFile.LoadFromFile((XmlElement)xmlElement.SelectSingleNode("UIInstallFile"));
				this.m_Dirty = true;
			}
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.IsDirty)
				{
					this.SaveProject();
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				MessageBox.Show(this, exception.Message, Helpers.GetProductName(), MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}

		private void btnRemove_Click(object sender, EventArgs e)
		{
			if (!this.ViewPackages.SelectedNode.Equals(this.ViewPackages.TopNode))
			{
				this.ViewPackages.SelectedNode.Remove();
				this.m_Dirty = true;
			}
		}

		public void Build(bool showBuildForm)
		{
			string str;
			BuildMessageEventHandler buildMessageEventHandler;
			TreeNode selectedNode = this.ViewPackages.SelectedNode;
			if (selectedNode == null)
			{
				return;
			}
			if (selectedNode.Tag is UIInstallFile)
			{
				selectedNode = selectedNode.Parent;
			}
			try
			{
				IEnumerator enumerator = null;
				IEnumerator enumerator1 = null;
				this.m_errors = 0;
				this.m_warnings = 0;
				if (this.IsDirty)
				{
					this.SaveProject();
				}
				Helpers.ResetLocStrings();
				this.BuildResultsControl.StartBuild();
				bool flag = false;
				try
				{
					enumerator = selectedNode.Nodes.GetEnumerator();
					while (enumerator.MoveNext())
					{
						if (!(((TreeNode)enumerator.Current).Tag is UIInstallFile))
						{
							continue;
						}
						flag = true;
						goto Label0;
					}
				}
				finally
				{
					if (enumerator is IDisposable)
					{
						(enumerator as IDisposable).Dispose();
					}
				}
			Label0:
				if (!flag)
				{
					throw new Exception("You must include at least one Install File to run.");
				}
				BootstrapperProduct bootstrapperProduct = new BootstrapperProduct();
				buildMessageEventHandler = this.BuildMessage;
				if (buildMessageEventHandler != null)
				{
					buildMessageEventHandler("Configuring the Product", 0);
				}
				string pCode = ((UIPackage)selectedNode.Tag).PCode;
				string pName = ((UIPackage)selectedNode.Tag).PName;
				((UIPackage)selectedNode.Tag).Build(bootstrapperProduct, selectedNode);
				try
				{
					enumerator1 = selectedNode.Nodes.GetEnumerator();
					while (enumerator1.MoveNext())
					{
						TreeNode current = (TreeNode)enumerator1.Current;
						buildMessageEventHandler = this.BuildMessage;
						if (buildMessageEventHandler != null)
						{
							buildMessageEventHandler(string.Concat("Configuring the Package: ", current.Text), 0);
						}
						((UIInstallFile)current.Tag).Build(bootstrapperProduct, this.m_ProjectProperties);
					}
				}
				finally
				{
					if (enumerator1 is IDisposable)
					{
						(enumerator1 as IDisposable).Dispose();
					}
				}
				string fullName = this.m_ProjectProperties.BootstrapperBuildDirectory.FullName;
				fullName = Path.Combine(fullName, pName);
				buildMessageEventHandler = this.BuildMessage;
				if (buildMessageEventHandler != null)
				{
					buildMessageEventHandler("Building output files.", 0);
				}
				BootstrapperProduct.BuildMessage += new BootstrapperProduct.BuildMessageEventHandler(this.HandleManifestBuildMessage);
				bootstrapperProduct.primaryProduct.ProductCode = pCode;
				bootstrapperProduct.Build(new DirectoryInfo(fullName), true);
				this.BuildResultsControl.SetBuildLocation(new DirectoryInfo(fullName));
				this.BuildResultsControl.EndBuild(true, null);
			}
			catch (Exceptions.BuildErrorsException buildErrorsException1)
			{
				Exceptions.BuildErrorsException buildErrorsException = buildErrorsException1;
				this.BuildResultsControl.EndBuild(false, buildErrorsException.ToString());
				this.m_errors++;
				buildMessageEventHandler = this.BuildMessage;
				if (buildMessageEventHandler != null)
				{
					buildMessageEventHandler(buildErrorsException.Message, 2);
				}
			}
			catch (Exceptions.BuildWarningsException buildWarningsException1)
			{
				Exceptions.BuildWarningsException buildWarningsException = buildWarningsException1;
				this.BuildResultsControl.EndBuild(true, buildWarningsException.ToString());
				this.m_warnings++;
				buildMessageEventHandler = this.BuildMessage;
				if (buildMessageEventHandler != null)
				{
					buildMessageEventHandler(buildWarningsException.Message, 1);
				}
			}
			catch (Exception exception2)
			{
				Exception exception = exception2;
				try
				{
					this.BuildResultsControl.EndBuild(false, exception.ToString());
				}
				catch (Exception exception1)
				{
				}
				this.m_errors++;
				buildMessageEventHandler = this.BuildMessage;
				if (buildMessageEventHandler != null)
				{
					buildMessageEventHandler(string.Concat("An unexpected build failure occured: \r\n", exception.Message), 2);
				}
			}
			str = (this.m_errors <= 0 ? "Build Suceeded. " : "Build Failed. ");
			buildMessageEventHandler = this.BuildMessage;
			if (buildMessageEventHandler != null)
			{
				buildMessageEventHandler("------------------------------------------------", 0);
			}
			buildMessageEventHandler = this.BuildMessage;
			if (buildMessageEventHandler != null)
			{
				buildMessageEventHandler(string.Concat(new string[] { str, "\tWarnings: ", this.m_warnings.ToString(), ", Errors: ", this.m_errors.ToString() }), 0);
			}
			BootstrapperProduct.BuildMessage -= new BootstrapperProduct.BuildMessageEventHandler(this.HandleManifestBuildMessage);
		}

		public bool CloseAndExit()
		{
			bool flag;
			try
			{
				if (!this.HandleClose())
				{
					return false;
				}
				else
				{
					base.Close();
					flag = true;
				}
			}
			catch (Exception exception)
			{
				throw new ProjectExceptions.CloseAndExitProjectException(this.ProjectName, exception.Message);
			}
			return flag;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void GeneratePackageInstallerToolStripMenuItem_Click(object sender, EventArgs e)
		{
		}

		private bool GetDirtyState(IEnumerator items)
		{
			bool flag;
			while (items.MoveNext())
			{
				TreeNode current = (TreeNode)items.Current;
				if (!(current.Tag is UIInstallFile))
				{
					if (!(current.Tag is UIPackage))
					{
						continue;
					}
					if (((UIPackage)current.Tag).IsDirty)
					{
						return true;
					}
					if (current.Nodes.Count <= 0)
					{
						continue;
					}
					IEnumerator enumerator = null;
					try
					{
						enumerator = current.Nodes.GetEnumerator();
						if (!this.GetDirtyState(enumerator))
						{
							continue;
						}
						else
						{
							flag = true;
						}
					}
					finally
					{
						if (enumerator is IDisposable)
						{
							(enumerator as IDisposable).Dispose();
						}
					}
					return flag;
				}
				else
				{
					if (!((UIInstallFile)current.Tag).IsDirty)
					{
						continue;
					}
					return true;
				}
			}
			return false;
		}

		public static List<string> GetPrerequisites(string projectName, string projectDir, string dteVersion)
		{
			Paths.Init(dteVersion);
			FileInfo fileInfo = new FileInfo(Path.Combine((new AddinExpress.Installer.Prerequisites.ProjectProperties(projectName, projectDir, string.Empty)).ProjectDirectory.FullName, string.Concat(projectName, ".", PrerequisitesForm.DefaultFileExtension)));
			List<string> strs = new List<string>();
			if (fileInfo.Exists)
			{
				IEnumerator enumerator = null;
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(fileInfo.FullName);
				XmlElement xmlElement = (XmlElement)xmlDocument.SelectSingleNode(PrerequisitesForm.DefaultFileExtension);
				try
				{
					enumerator = xmlElement.ChildNodes.GetEnumerator();
					while (enumerator.MoveNext())
					{
						XmlElement current = (XmlElement)enumerator.Current;
						string name = current.Name;
						if (name == null || !(name == "UIPackage"))
						{
							continue;
						}
						XmlAttribute itemOf = current.Attributes["ProductName"];
						if (itemOf == null)
						{
							continue;
						}
						strs.Add(itemOf.Value);
					}
				}
				finally
				{
					if (enumerator is IDisposable)
					{
						(enumerator as IDisposable).Dispose();
					}
				}
			}
			return strs;
		}

		public void HandleBuildMessage(string message, BuildErrorLevel ErrorLevel)
		{
			this.BuildResultsControl.AddResults(message);
		}

		private bool HandleClose()
		{
			if (this.IsDirty)
			{
				int num = (int)MessageBox.Show(this, "You have some unsaved settings. Do you want to save settings?", "Close Dialog", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
				if (num == 2)
				{
					return false;
				}
				if (num == 6)
				{
					if (this.SaveProject() == System.Windows.Forms.DialogResult.Cancel)
					{
						return false;
					}
				}
			}
			return true;
		}

		public void HandleManifestBuildMessage(string message, BootstrapperProduct.BuildErrorLevel level)
		{
			BuildErrorLevel buildErrorLevel = BuildErrorLevel.None;
			switch (level)
			{
				case BootstrapperProduct.BuildErrorLevel.None:
				{
					buildErrorLevel = BuildErrorLevel.None;
					break;
				}
				case BootstrapperProduct.BuildErrorLevel.Warning:
				{
					buildErrorLevel = BuildErrorLevel.Warning;
					this.m_warnings++;
					break;
				}
				case BootstrapperProduct.BuildErrorLevel.BuildError:
				{
					buildErrorLevel = BuildErrorLevel.BuildError;
					this.m_errors++;
					break;
				}
			}
			BuildMessageEventHandler buildMessageEventHandler = this.BuildMessage;
			if (buildMessageEventHandler != null)
			{
				buildMessageEventHandler(message, buildErrorLevel);
			}
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(PrerequisitesForm));
			this.imageList16 = new ImageList(this.components);
			this.OpenFileDialog1 = new OpenFileDialog();
			this.SaveFileDialog1 = new SaveFileDialog();
			this.ViewPackagesCtx = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.addNewPackageMenuItem = new ToolStripMenuItem();
			this.buildPackageMenuItem = new ToolStripMenuItem();
			this.toolStripMenuItem1 = new ToolStripSeparator();
			this.addNewFileMenuItem = new ToolStripMenuItem();
			this.cloneFileMenuItem = new ToolStripMenuItem();
			this.toolStripMenuItem2 = new ToolStripSeparator();
			this.removeMenuItem = new ToolStripMenuItem();
			this.ViewPackages = new TreeView();
			this.pnlTop = new Panel();
			this.pnlItem = new Panel();
			this.panel3 = new Panel();
			this.toolStrip = new MyToolStrip();
			this.btnAddPkg = new ToolStripButton();
			this.btnBuildPkg = new ToolStripButton();
			this.toolStripSeparator2 = new ToolStripSeparator();
			this.btnAddFile = new ToolStripButton();
			this.btnCloneFile = new ToolStripButton();
			this.toolStripSeparator1 = new ToolStripSeparator();
			this.btnRemove = new ToolStripButton();
			this.btnCancel = new Button();
			this.pnlBottom = new Panel();
			this.StatusBar = new System.Windows.Forms.StatusBar();
			this.BuildResultsControl = new AddinExpress.Installer.Prerequisites.BuildResultsControl();
			this.btnOK = new Button();
			this.toolTip1 = new ToolTip(this.components);
			this.imageList20 = new ImageList(this.components);
			this.imageList24 = new ImageList(this.components);
			this.imageList32 = new ImageList(this.components);
			this.ViewPackagesCtx.SuspendLayout();
			this.pnlTop.SuspendLayout();
			this.panel3.SuspendLayout();
			this.toolStrip.SuspendLayout();
			this.pnlBottom.SuspendLayout();
			base.SuspendLayout();
			this.imageList16.ImageStream = (ImageListStreamer)componentResourceManager.GetObject("imageList16.ImageStream");
			this.imageList16.TransparentColor = Color.Transparent;
			this.imageList16.Images.SetKeyName(0, "packages-16.ico");
			this.imageList16.Images.SetKeyName(1, "package_tools-16.ico");
			this.imageList16.Images.SetKeyName(2, "file_tools-16.ico");
			this.ViewPackagesCtx.AllowDrop = true;
			this.ViewPackagesCtx.BackColor = SystemColors.Menu;
			this.ViewPackagesCtx.ImageList = this.imageList16;
			this.ViewPackagesCtx.Items.AddRange(new ToolStripItem[] { this.addNewPackageMenuItem, this.buildPackageMenuItem, this.toolStripMenuItem1, this.addNewFileMenuItem, this.cloneFileMenuItem, this.toolStripMenuItem2, this.removeMenuItem });
			this.ViewPackagesCtx.Name = "ContextMenuStrip1";
			this.ViewPackagesCtx.Size = new System.Drawing.Size(161, 126);
			this.ViewPackagesCtx.Opening += new CancelEventHandler(this.ViewPackagesCtx_Opening);
			this.addNewPackageMenuItem.Name = "addNewPackageMenuItem";
			this.addNewPackageMenuItem.Size = new System.Drawing.Size(160, 22);
			this.addNewPackageMenuItem.Text = "Add Package";
			this.addNewPackageMenuItem.Click += new EventHandler(this.btnAddPkg_Click);
			this.buildPackageMenuItem.Name = "buildPackageMenuItem";
			this.buildPackageMenuItem.Size = new System.Drawing.Size(160, 22);
			this.buildPackageMenuItem.Text = "Build Package";
			this.buildPackageMenuItem.Click += new EventHandler(this.btnBuildPkg_Click);
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(157, 6);
			this.addNewFileMenuItem.Name = "addNewFileMenuItem";
			this.addNewFileMenuItem.Size = new System.Drawing.Size(160, 22);
			this.addNewFileMenuItem.Text = "Add Install File";
			this.addNewFileMenuItem.Click += new EventHandler(this.btnAddFile_Click);
			this.cloneFileMenuItem.Name = "cloneFileMenuItem";
			this.cloneFileMenuItem.Size = new System.Drawing.Size(160, 22);
			this.cloneFileMenuItem.Text = "Clone Install File";
			this.cloneFileMenuItem.Click += new EventHandler(this.btnCloneFile_Click);
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(157, 6);
			this.removeMenuItem.Name = "removeMenuItem";
			this.removeMenuItem.Size = new System.Drawing.Size(160, 22);
			this.removeMenuItem.Text = "Remove";
			this.removeMenuItem.Click += new EventHandler(this.btnRemove_Click);
			this.ViewPackages.BorderStyle = BorderStyle.FixedSingle;
			this.ViewPackages.ContextMenuStrip = this.ViewPackagesCtx;
			this.ViewPackages.Dock = DockStyle.Fill;
			this.ViewPackages.HideSelection = false;
			this.ViewPackages.ImageIndex = 0;
			this.ViewPackages.ImageList = this.imageList16;
			this.ViewPackages.Location = new Point(5, 25);
			this.ViewPackages.Name = "ViewPackages";
			this.ViewPackages.SelectedImageIndex = 0;
			this.ViewPackages.ShowRootLines = false;
			this.ViewPackages.Size = new System.Drawing.Size(201, 363);
			this.ViewPackages.TabIndex = 1;
			this.toolTip1.SetToolTip(this.ViewPackages, "List of packages to be included with the bootstrapper.\r\nThis list is created from the packages installed on the machine.");
			this.ViewPackages.AfterSelect += new TreeViewEventHandler(this.ViewPackages_AfterSelect);
			this.ViewPackages.KeyDown += new KeyEventHandler(this.ViewPackages_KeyDown);
			this.pnlTop.Controls.Add(this.pnlItem);
			this.pnlTop.Controls.Add(this.panel3);
			this.pnlTop.Dock = DockStyle.Fill;
			this.pnlTop.Location = new Point(0, 0);
			this.pnlTop.Name = "pnlTop";
			this.pnlTop.Size = new System.Drawing.Size(649, 388);
			this.pnlTop.TabIndex = 0;
			this.pnlItem.Dock = DockStyle.Fill;
			this.pnlItem.Location = new Point(206, 0);
			this.pnlItem.Name = "pnlItem";
			this.pnlItem.Size = new System.Drawing.Size(443, 388);
			this.pnlItem.TabIndex = 2;
			this.panel3.Controls.Add(this.ViewPackages);
			this.panel3.Controls.Add(this.toolStrip);
			this.panel3.Dock = DockStyle.Left;
			this.panel3.Location = new Point(0, 0);
			this.panel3.Name = "panel3";
			this.panel3.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
			this.panel3.Size = new System.Drawing.Size(206, 388);
			this.panel3.TabIndex = 1;
			this.toolStrip.GripStyle = ToolStripGripStyle.Hidden;
			this.toolStrip.Items.AddRange(new ToolStripItem[] { this.btnAddPkg, this.btnBuildPkg, this.toolStripSeparator2, this.btnAddFile, this.btnCloneFile, this.toolStripSeparator1, this.btnRemove });
			this.toolStrip.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
			this.toolStrip.Location = new Point(5, 0);
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.RenderMode = ToolStripRenderMode.System;
			this.toolStrip.Size = new System.Drawing.Size(201, 25);
			this.toolStrip.TabIndex = 0;
			this.toolStrip.Text = "toolStrip1";
			this.btnAddPkg.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.btnAddPkg.Image = (Image)componentResourceManager.GetObject("btnAddPkg.Image");
			this.btnAddPkg.ImageTransparentColor = Color.Magenta;
			this.btnAddPkg.Name = "btnAddPkg";
			this.btnAddPkg.Size = new System.Drawing.Size(23, 22);
			this.btnAddPkg.Text = "Add New Package";
			this.btnAddPkg.Click += new EventHandler(this.btnAddPkg_Click);
			this.btnBuildPkg.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.btnBuildPkg.Image = (Image)componentResourceManager.GetObject("btnBuildPkg.Image");
			this.btnBuildPkg.ImageTransparentColor = Color.Magenta;
			this.btnBuildPkg.Name = "btnBuildPkg";
			this.btnBuildPkg.Size = new System.Drawing.Size(23, 22);
			this.btnBuildPkg.Text = "Build Package";
			this.btnBuildPkg.Click += new EventHandler(this.btnBuildPkg_Click);
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
			this.btnAddFile.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.btnAddFile.Image = (Image)componentResourceManager.GetObject("btnAddFile.Image");
			this.btnAddFile.ImageTransparentColor = Color.Magenta;
			this.btnAddFile.Name = "btnAddFile";
			this.btnAddFile.Size = new System.Drawing.Size(23, 22);
			this.btnAddFile.Text = "Add New File";
			this.btnAddFile.Click += new EventHandler(this.btnAddFile_Click);
			this.btnCloneFile.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.btnCloneFile.Image = (Image)componentResourceManager.GetObject("btnCloneFile.Image");
			this.btnCloneFile.ImageTransparentColor = Color.Magenta;
			this.btnCloneFile.Name = "btnCloneFile";
			this.btnCloneFile.Size = new System.Drawing.Size(23, 22);
			this.btnCloneFile.Text = "Clone File";
			this.btnCloneFile.Click += new EventHandler(this.btnCloneFile_Click);
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			this.btnRemove.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.btnRemove.Image = (Image)componentResourceManager.GetObject("btnRemove.Image");
			this.btnRemove.ImageTransparentColor = Color.Magenta;
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.Size = new System.Drawing.Size(23, 22);
			this.btnRemove.Text = "Remove";
			this.btnRemove.Click += new EventHandler(this.btnRemove_Click);
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new Point(551, 191);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(87, 25);
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.pnlBottom.Controls.Add(this.StatusBar);
			this.pnlBottom.Controls.Add(this.BuildResultsControl);
			this.pnlBottom.Controls.Add(this.btnOK);
			this.pnlBottom.Controls.Add(this.btnCancel);
			this.pnlBottom.Dock = DockStyle.Bottom;
			this.pnlBottom.Location = new Point(0, 388);
			this.pnlBottom.Name = "pnlBottom";
			this.pnlBottom.Padding = new System.Windows.Forms.Padding(6, 10, 12, 0);
			this.pnlBottom.Size = new System.Drawing.Size(649, 248);
			this.pnlBottom.TabIndex = 3;
			this.StatusBar.Location = new Point(6, 226);
			this.StatusBar.Name = "StatusBar";
			this.StatusBar.Size = new System.Drawing.Size(631, 22);
			this.StatusBar.SizingGrip = false;
			this.StatusBar.TabIndex = 9;
			this.BuildResultsControl.Dock = DockStyle.Top;
			this.BuildResultsControl.Font = new System.Drawing.Font("Segoe UI", 9f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.BuildResultsControl.Location = new Point(6, 10);
			this.BuildResultsControl.Margin = new System.Windows.Forms.Padding(58, 22, 58, 22);
			this.BuildResultsControl.Name = "BuildResultsControl";
			this.BuildResultsControl.Size = new System.Drawing.Size(631, 172);
			this.BuildResultsControl.StatusBar = null;
			this.BuildResultsControl.TabIndex = 0;
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new Point(458, 191);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(87, 25);
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new EventHandler(this.btnOK_Click);
			this.toolTip1.AutoPopDelay = 5000;
			this.toolTip1.InitialDelay = 500;
			this.toolTip1.ReshowDelay = 100;
			this.toolTip1.ShowAlways = true;
			this.imageList20.ImageStream = (ImageListStreamer)componentResourceManager.GetObject("imageList20.ImageStream");
			this.imageList20.TransparentColor = Color.Transparent;
			this.imageList20.Images.SetKeyName(0, "packages-20.ico");
			this.imageList20.Images.SetKeyName(1, "package_tools-20.ico");
			this.imageList20.Images.SetKeyName(2, "file_tools-20.ico");
			this.imageList24.ImageStream = (ImageListStreamer)componentResourceManager.GetObject("imageList24.ImageStream");
			this.imageList24.TransparentColor = Color.Transparent;
			this.imageList24.Images.SetKeyName(0, "packages-24.ico");
			this.imageList24.Images.SetKeyName(1, "package_tools-24.ico");
			this.imageList24.Images.SetKeyName(2, "file_tools-24.ico");
			this.imageList32.ImageStream = (ImageListStreamer)componentResourceManager.GetObject("imageList32.ImageStream");
			this.imageList32.TransparentColor = Color.Transparent;
			this.imageList32.Images.SetKeyName(0, "packages-32.ico");
			this.imageList32.Images.SetKeyName(1, "package_tools-32.ico");
			this.imageList32.Images.SetKeyName(2, "file_tools-32.ico");
			base.AutoScaleDimensions = new SizeF(96f, 96f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			base.ClientSize = new System.Drawing.Size(649, 636);
			base.Controls.Add(this.pnlTop);
			base.Controls.Add(this.pnlBottom);
			this.Font = new System.Drawing.Font("Segoe UI", 9f, FontStyle.Regular, GraphicsUnit.Point, 0);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "PrerequisitesForm";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "My Prerequisites";
			this.ViewPackagesCtx.ResumeLayout(false);
			this.pnlTop.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			this.panel3.PerformLayout();
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			this.pnlBottom.ResumeLayout(false);
			base.ResumeLayout(false);
		}

		private TreeNode LoadFreshTreeView()
		{
			this.ViewPackages.Nodes.Clear();
			Application.DoEvents();
			TreeNode treeNode = new TreeNode()
			{
				Text = "Packages",
				ImageIndex = 0,
				SelectedImageIndex = 0
			};
			this.ViewPackages.Nodes.Add(treeNode);
			return treeNode;
		}

		internal void LoadProjectFile(FileInfo file)
		{
			TreeNode treeNode = this.LoadFreshTreeView();
			try
			{
				XmlDocument xmlDocument = new XmlDocument();
				if (file.Exists)
				{
					IEnumerator enumerator = null;
					xmlDocument.Load(file.FullName);
					XmlElement xmlElement = (XmlElement)xmlDocument.SelectSingleNode(this.RootProjectElementName);
					try
					{
						enumerator = xmlElement.ChildNodes.GetEnumerator();
						while (enumerator.MoveNext())
						{
							XmlElement current = (XmlElement)enumerator.Current;
							string name = current.Name;
							if (name == null)
							{
								continue;
							}
							if (name == "UIInstallFile")
							{
								this.AddNewInstallFile(false).LoadFromFile(current);
							}
							else if (name == "UIPackage")
							{
								this.AddNewPackage().LoadFromFile(current);
							}
							else if (name == "ProjectProperties")
							{
								this.m_ProjectProperties.LoadFromFile(current);
							}
						}
					}
					finally
					{
						if (enumerator is IDisposable)
						{
							(enumerator as IDisposable).Dispose();
						}
					}
					this.m_LoadedFromFile = file.FullName;
				}
			}
			finally
			{
				UIPackages uIPackage = new UIPackages(this);
				uIPackage.OnLinkLabelClicked += new EventHandler(this.packages_OnLinkLabelClicked);
				treeNode.Tag = uIPackage;
				this.ViewPackages.SelectedNode = this.ViewPackages.TopNode;
				this.ViewPackages.Select();
			}
			this.m_Dirty = false;
		}

		private void packages_OnLinkLabelClicked(object sender, EventArgs e)
		{
			this.AddNewPackage();
		}

		private void PrerequisitesForm_Load(object sender, EventArgs e)
		{
			Application.EnableVisualStyles();
			this.BuildResultsControl.StatusBar = this.StatusBar;
			this.BuildResultsControl.BuildOutputLink.Visible = false;
			this.BuildResultsControl.BuildFailureDetailsLink.Visible = false;
			this.BuildMessage += new BuildMessageEventHandler(this.HandleBuildMessage);
			Helpers.Init();
			this.toolStrip.AutoSize = false;
			this.btnAddPkg.Image = Helpers.GetNormalizedImage(MyResources.package_tools, false);
			this.btnBuildPkg.Image = Helpers.GetNormalizedImage(MyResources.build_tools, false);
			this.btnAddFile.Image = Helpers.GetNormalizedImage(MyResources.file_tools, false);
			this.btnCloneFile.Image = Helpers.GetNormalizedImage(MyResources.clone_tools, false);
			this.btnRemove.Image = Helpers.GetNormalizedImage(MyResources.remove_tools, false);
			this.btnAddPkg.ToolTipText = "Add Package\nThis is a bootstrapper package to install on the target PC.";
			this.btnBuildPkg.ToolTipText = "Build Package\nBuilds the selected package to the Visual Studio Packages directory.";
			this.btnAddFile.ToolTipText = "Add Install File\nThis is a file to run to install the package for a specific language (or for all languages).";
			this.btnCloneFile.ToolTipText = "Clone Install File\nMakes a copy of the selected install file.";
			this.btnRemove.ToolTipText = "Remove\nRemoves the selected item.";
			this.addNewPackageMenuItem.Image = Helpers.GetNormalizedImage(MyResources.package_tools, false);
			this.buildPackageMenuItem.Image = Helpers.GetNormalizedImage(MyResources.build_tools, false);
			this.addNewFileMenuItem.Image = Helpers.GetNormalizedImage(MyResources.file_tools, false);
			this.cloneFileMenuItem.Image = Helpers.GetNormalizedImage(MyResources.clone_tools, false);
			this.removeMenuItem.Image = Helpers.GetNormalizedImage(MyResources.remove_tools, false);
			if (Helpers.DPI >= 192f)
			{
				this.toolStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
				this.ViewPackages.ImageList = this.imageList32;
			}
			else if (Helpers.DPI >= 144f)
			{
				this.toolStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
				this.ViewPackages.ImageList = this.imageList24;
			}
			else if (Helpers.DPI < 120f)
			{
				this.toolStrip.ImageScalingSize = new System.Drawing.Size(16, 16);
				this.ViewPackages.ImageList = this.imageList16;
			}
			else
			{
				this.toolStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
				this.ViewPackages.ImageList = this.imageList20;
			}
			this.toolStrip.AutoSize = true;
			try
			{
				this.m_ProjectProperties = new AddinExpress.Installer.Prerequisites.ProjectProperties(this.m_projectName, this.m_projectDir, this.sdkDir);
				this.m_LoadedFromFile = Path.Combine(this.m_ProjectProperties.ProjectDirectory.FullName, string.Concat(this.m_projectName, ".", PrerequisitesForm.DefaultFileExtension));
				this.m_ProjectFile = new FileInfo(this.m_LoadedFromFile);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				MessageBox.Show(this, exception.Message, Helpers.GetProductName(), MessageBoxButtons.OK, MessageBoxIcon.Hand);
				base.Close();
				return;
			}
			this.uiLocked = true;
			this.ViewPackages.Select();
			try
			{
				try
				{
					this.LoadProjectFile(this.m_ProjectFile);
				}
				catch (Exception exception3)
				{
					Exception exception2 = exception3;
					MessageBox.Show(this, exception2.Message, Helpers.GetProductName(), MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}
			}
			finally
			{
				this.uiLocked = false;
				this.SetUIState();
			}
		}

		private System.Windows.Forms.DialogResult Save()
		{
			if (string.IsNullOrEmpty(this.m_LoadedFromFile))
			{
				return this.SaveAs();
			}
			this.SaveFile(this.m_LoadedFromFile);
			return System.Windows.Forms.DialogResult.None;
		}

		private System.Windows.Forms.DialogResult SaveAs()
		{
			this.SaveFileDialog1.AddExtension = true;
			this.SaveFileDialog1.DefaultExt = PrerequisitesForm.DefaultFileExtension;
			this.SaveFileDialog1.FileName = this.m_ProjectProperties.ProjectName;
			this.SaveFileDialog1.Filter = string.Concat(this.m_FileFilter, "|All Files(*.*)|*.*");
			System.Windows.Forms.DialogResult dialogResult = this.SaveFileDialog1.ShowDialog();
			if (dialogResult == System.Windows.Forms.DialogResult.OK)
			{
				this.SaveFile(this.SaveFileDialog1.FileName);
			}
			return dialogResult;
		}

		private void SaveFile(string userFilePath)
		{
			IEnumerator enumerator = null;
			string str = userFilePath;
			FileInfo fileInfo = new FileInfo(userFilePath);
			if (string.IsNullOrEmpty(fileInfo.Extension))
			{
				str = string.Concat(str, PrerequisitesForm.DefaultFileExtension);
			}
			if (!fileInfo.Exists && !fileInfo.Directory.Exists)
			{
				fileInfo.Directory.Create();
			}
			XmlDocument xmlDocument = new XmlDocument();
			XmlElement xmlElement = xmlDocument.CreateElement(this.RootProjectElementName);
			xmlDocument.AppendChild(xmlElement);
			this.m_ProjectProperties.SaveToFile(xmlElement);
			foreach (TreeNode node in this.ViewPackages.TopNode.Nodes)
			{
				((UIPackage)node.Tag).SaveToFile(xmlElement);
				try
				{
					enumerator = node.Nodes.GetEnumerator();
					while (enumerator.MoveNext())
					{
						((UIInstallFile)((TreeNode)enumerator.Current).Tag).SaveToFile(xmlElement);
					}
				}
				finally
				{
					if (enumerator is IDisposable)
					{
						(enumerator as IDisposable).Dispose();
					}
				}
			}
			xmlDocument.Save(str);
			this.m_ProjectFile = new FileInfo(str);
			this.m_LoadedFromFile = str;
			this.m_Dirty = false;
		}

		public System.Windows.Forms.DialogResult SaveProject()
		{
			System.Windows.Forms.DialogResult dialogResult;
			try
			{
				dialogResult = this.Save();
			}
			catch (Exception exception)
			{
				throw new ProjectExceptions.SaveProjectException(this.ProjectName, exception.Message);
			}
			return dialogResult;
		}

		public void SaveProjectAs()
		{
			try
			{
				this.SaveAs();
			}
			catch (Exception exception)
			{
				throw new ProjectExceptions.SaveProjectAsException(this.ProjectName, exception.Message);
			}
		}

		private void SetUIState()
		{
			if (!this.uiLocked && this.ViewPackages.SelectedNode != null)
			{
				object tag = this.ViewPackages.SelectedNode.Tag;
				if (tag is UIInstallFile)
				{
					this.btnAddPkg.Enabled = false;
					this.btnBuildPkg.Enabled = true;
					this.btnAddFile.Enabled = false;
					this.btnCloneFile.Enabled = true;
					this.btnRemove.Enabled = true;
					this.addNewPackageMenuItem.Enabled = false;
					this.buildPackageMenuItem.Enabled = true;
					this.addNewFileMenuItem.Enabled = false;
					this.cloneFileMenuItem.Enabled = true;
					this.removeMenuItem.Enabled = true;
					return;
				}
				if (tag is UIPackage)
				{
					this.btnAddPkg.Enabled = false;
					this.btnBuildPkg.Enabled = true;
					this.btnAddFile.Enabled = true;
					this.btnCloneFile.Enabled = false;
					this.btnRemove.Enabled = true;
					this.addNewPackageMenuItem.Enabled = false;
					this.buildPackageMenuItem.Enabled = true;
					this.addNewFileMenuItem.Enabled = true;
					this.cloneFileMenuItem.Enabled = false;
					this.removeMenuItem.Enabled = true;
					return;
				}
				if (tag is UIPackages)
				{
					this.btnAddPkg.Enabled = true;
					this.btnBuildPkg.Enabled = false;
					this.btnAddFile.Enabled = false;
					this.btnCloneFile.Enabled = false;
					this.btnRemove.Enabled = false;
					this.addNewPackageMenuItem.Enabled = true;
					this.buildPackageMenuItem.Enabled = false;
					this.addNewFileMenuItem.Enabled = false;
					this.cloneFileMenuItem.Enabled = false;
					this.removeMenuItem.Enabled = false;
				}
			}
		}

		private void ViewPackages_AfterSelect(object sender, TreeViewEventArgs e)
		{
			try
			{
				try
				{
					this.pnlItem.Controls.Clear();
					Control tag = (Control)this.ViewPackages.SelectedNode.Tag;
					this.pnlItem.Controls.Add(tag);
					tag.Dock = DockStyle.Fill;
				}
				catch (Exception exception)
				{
				}
			}
			finally
			{
				this.SetUIState();
			}
		}

		private void ViewPackages_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete && e.Modifiers == Keys.None)
			{
				e.Handled = true;
				if (this.btnRemove.Enabled)
				{
					this.btnRemove.PerformClick();
				}
			}
		}

		private void ViewPackagesCtx_Opening(object sender, CancelEventArgs e)
		{
			this.SetUIState();
		}

		public event BuildMessageEventHandler BuildMessage;

		internal class AddFileInfo
		{
			public FileInfo FilePath;

			public CultureInfo Langauge;

			public AddFileInfo()
			{
			}
		}
	}
}