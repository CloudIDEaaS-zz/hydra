using AddinExpress.Installer.WiXDesigner.DesignTime;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Setup.Configuration;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	[ADXProvideSolutionProps("DesignerVSWiXSolutionOptions")]
	[Description("Designer for Visual Studio WiX Setup Projects")]
	[Guid("5A3C4C1E-8648-4005-2019-09DC4A2AFC21")]
	[InstalledProductRegistration("#110", "#112", "2.0", IconResourceID=400, LanguageIndependentName="Designer for Visual Studio WiX Setup Projects")]
	[PackageRegistration(UseManagedResourcesOnly=true, AllowsBackgroundLoading=true)]
	[ProvideAutoLoad(,)]    // JustDecompile was unable to locate the assembly where attribute parameters types are defined. Generating parameters values is impossible.
	[ProvideAutoLoad(,)]    // JustDecompile was unable to locate the assembly where attribute parameters types are defined. Generating parameters values is impossible.
	[ProvideAutoLoad(,)]    // JustDecompile was unable to locate the assembly where attribute parameters types are defined. Generating parameters values is impossible.
	[ProvideAutoLoad(,)]    // JustDecompile was unable to locate the assembly where attribute parameters types are defined. Generating parameters values is impossible.
	[ProvideMenuResource("Menus.ctmenu", 1)]
	[ProvideToolWindow(,)]    // JustDecompile was unable to locate the assembly where attribute parameters types are defined. Generating parameters values is impossible.
	[ProvideToolWindow(,)]    // JustDecompile was unable to locate the assembly where attribute parameters types are defined. Generating parameters values is impossible.
	[ProvideToolWindow(,)]    // JustDecompile was unable to locate the assembly where attribute parameters types are defined. Generating parameters values is impossible.
	[ProvideToolWindow(,)]    // JustDecompile was unable to locate the assembly where attribute parameters types are defined. Generating parameters values is impossible.
	[ProvideToolWindow(,)]    // JustDecompile was unable to locate the assembly where attribute parameters types are defined. Generating parameters values is impossible.
	[ProvideToolWindow(,)]    // JustDecompile was unable to locate the assembly where attribute parameters types are defined. Generating parameters values is impossible.
	public sealed class VsPackage : AsyncPackage, IWin32Window, IVsTrackProjectDocumentsEvents2, IVsSolutionEvents3, IVsSolutionEvents2, IVsSolutionEvents, IVsSolutionEvents4, IOleCommandTarget, IVsPersistSolutionProps, IVsPersistSolutionOpts, IVsRunningDocTableEvents, IVsRunningDocTableEvents2, IVsRunningDocTableEvents3, IVsRunningDocTableEvents4
	{
		private bool disposed;

		private IVsSolution solution;

		private bool prevVisibilityState;

		private _DTE application;

		private AddinExpress.Installer.WiXDesigner.ErrorList errorListProvider;

		private RunningDocumentTable runningDocumentTable;

		private IVsTrackProjectDocuments2 projectDocTracker;

		private IVsRegisterPriorityCommandTarget priorityCommands;

		private uint solutionEventsCookie;

		private uint priorityCommandsCookie;

		private uint docTrackerEventsCookie;

		private uint runningDocumentTableCookie;

		private SortedList<string, VsWiXProject> connectedProjectList = new SortedList<string, VsWiXProject>();

		private Hashtable userOptionsCache = new Hashtable();

		private System.Windows.Forms.Timer userOptionsTimer;

		private bool allowLoadUserOptions;

		private bool allowCheckForUpdates = true;

		private bool vsixPackage;

		private SelectionEvents _selectionEvents;

		internal static VsPackage CurrentInstance;

		private static volatile object Mutex;

		private int count;

		private string reloadedProjectGuid = string.Empty;

		private bool projectsWereOpened;

		internal SortedList<string, VsWiXProject> ConnectedProjectList
		{
			get
			{
				return this.connectedProjectList;
			}
		}

		public IntPtr Handle
		{
			get
			{
				if (this.application == null)
				{
					return IntPtr.Zero;
				}
				return new IntPtr(this.application.MainWindow.HWnd);
			}
		}

		internal bool IsVSIXPackage
		{
			get
			{
				return this.vsixPackage;
			}
		}

		public IVsSolution Solution
		{
			get
			{
				return this.solution;
			}
		}

		public string VSVersion
		{
			get
			{
				if (this.application == null)
				{
					return string.Empty;
				}
				return Path.GetFileName(this.application.RegistryRoot.Replace("Exp", string.Empty));
			}
		}

		static VsPackage()
		{
			VsPackage.CurrentInstance = null;
			VsPackage.Mutex = new object();
		}

		public VsPackage()
		{
			VsPackage.CurrentInstance = this;
			this.userOptionsTimer = new System.Windows.Forms.Timer()
			{
				Interval = 300
			};
			this.userOptionsTimer.Tick += new EventHandler(this.userOptionsTimer_Tick);
		}

		private void BeforeBuildStarted(vsBuildScope scope)
		{
		}

		private void BuildManager_BuildProjConfigBegin(string Project, string ProjectConfig, string Platform, string SolutionConfig)
		{
			if (this.connectedProjectList.Count > 0)
			{
				IVsHierarchy hierarchy = this.GetHierarchy(Project);
				if (hierarchy != null)
				{
					foreach (VsWiXProject value in this.connectedProjectList.Values)
					{
						if (value.Hierarchy == null || !hierarchy.Equals(value.Hierarchy))
						{
							continue;
						}
						value.OnBuildStarted();
						return;
					}
				}
			}
		}

		private void BuildManager_BuildProjConfigDone(string Project, string ProjectConfig, string Platform, string SolutionConfig, bool Success)
		{
			if (this.connectedProjectList.Count > 0)
			{
				IVsHierarchy hierarchy = this.GetHierarchy(Project);
				if (hierarchy != null)
				{
					foreach (VsWiXProject value in this.connectedProjectList.Values)
					{
						if (value.Hierarchy == null || !hierarchy.Equals(value.Hierarchy))
						{
							continue;
						}
						value.OnBuildStopped();
						return;
					}
				}
			}
		}

		private void BuildManager_BuildStarted()
		{
		}

		private void BuildManager_BuildStopped()
		{
		}

		private void CheckForUpdates(string url)
		{
			BackgroundWorker backgroundWorker = new BackgroundWorker();
			backgroundWorker.DoWork += new DoWorkEventHandler(this.WorkerDoWork);
			backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.WorkerRunWorkerCompleted);
			backgroundWorker.RunWorkerAsync(url);
		}

		internal static bool CheckInstalledVSVersion2(int version, out List<string> installDirs)
		{
			bool flag;
			installDirs = null;
			if (version >= 15)
			{
				try
				{
					List<string> vSInstallDir = VsPackage.GetVSInstallDir(version);
					if (vSInstallDir.Count <= 0)
					{
						return false;
					}
					else
					{
						installDirs = vSInstallDir;
						flag = true;
					}
				}
				catch
				{
					return false;
				}
				return flag;
			}
			return false;
		}

		private void CompareVersions(string versionInfo)
		{
			if (string.IsNullOrEmpty(versionInfo))
			{
				return;
			}
			Version version = new Version(versionInfo);
			if (base.GetType().Assembly.GetName().Version < version)
			{
				MessageBoxEx handle = null;
				DialogResult dialogResult = DialogResult.Yes;
				try
				{
					handle = MessageBoxExManager.CreateMessageBox("CheckForUpdatesMessageBox");
					handle.Caption = "Designer for Visual Studio WiX Setup Projects";
					handle.Text = "A newer version of 'Designer for Visual Studio WiX Setup Projects' is available.\r\nDo you want to download it now?";
					handle.StartPosition = FormStartPosition.Manual;
					handle.MainWindowHandle = VsPackage.CurrentInstance.Handle;
					MessageBoxExButton messageBoxExButton = new MessageBoxExButton()
					{
						Text = "Yes",
						Value = "Yes",
						HelpText = ""
					};
					MessageBoxExButton messageBoxExButton1 = new MessageBoxExButton()
					{
						Text = "Remind me later",
						Value = "No",
						HelpText = "",
						IsCancelButton = true
					};
					handle.AddButton(messageBoxExButton);
					handle.AddButton(messageBoxExButton1);
					handle.Icon = MessageBoxExIcon.Question;
					handle.SaveResponseText = "Don’t show this dialog";
					handle.AllowSaveResponse = true;
					handle.UseSavedResponse = false;
					handle.Font = SystemFonts.MessageBoxFont;
					if (handle.Show(this) == "No")
					{
						dialogResult = DialogResult.No;
					}
					if (handle.SaveResponse)
					{
						using (RegistryKey registryKey = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Add-in Express\\User Settings\\Designer for Visual Studio WiX Setup Projects\\"))
						{
							if (registryKey != null)
							{
								registryKey.SetValue("CheckForUpdates", 0);
							}
						}
					}
				}
				finally
				{
					if (handle != null)
					{
						MessageBoxExManager.DeleteMessageBox("DeleteComponent");
					}
				}
				if (dialogResult == DialogResult.Yes)
				{
					using (System.Diagnostics.Process process = null)
					{
						try
						{
							process = new System.Diagnostics.Process();
							process.StartInfo.FileName = "http://www.add-in-express.com/downloads/wix-designer.php";
							process.Start();
						}
						catch (Exception exception)
						{
						}
					}
				}
			}
		}

		internal WindowPane CreateEditorPane(Type type, int id)
		{
			return this.CreateToolWindow(type, id);
		}

		private void CustomActionsEditor_Callback(object sender, EventArgs e)
		{
			this.ProcessButtonClick(261);
		}

		protected override void Dispose(bool disposing)
		{
			lock (VsPackage.Mutex)
			{
				if (!this.disposed)
				{
					this.disposed = true;
					if (this.userOptionsTimer != null)
					{
						this.userOptionsTimer.Enabled = false;
						this.userOptionsTimer.Dispose();
						this.userOptionsTimer = null;
					}
					SystemEvents.UserPreferenceChanged -= new UserPreferenceChangedEventHandler(this.OnUserPreferenceChanged);
					if (this._selectionEvents != null)
					{
						this._selectionEvents.OnChange -= new _dispSelectionEvents_OnChangeEventHandler(this.selectionEvents_OnChange);
					}
					if (this.projectDocTracker != null && this.docTrackerEventsCookie != 0)
					{
						this.projectDocTracker.UnadviseTrackProjectDocumentsEvents(this.docTrackerEventsCookie);
						this.docTrackerEventsCookie = 0;
					}
					if (this.solutionEventsCookie != 0 && this.solution != null)
					{
						this.solution.UnadviseSolutionEvents(this.solutionEventsCookie);
						this.solutionEventsCookie = 0;
					}
					if (this.runningDocumentTable != null && this.runningDocumentTableCookie != 0)
					{
						this.runningDocumentTable.Unadvise(this.runningDocumentTableCookie);
						this.runningDocumentTableCookie = 0;
					}
					if (this.priorityCommands != null && this.priorityCommandsCookie != 0)
					{
						this.priorityCommands.UnregisterPriorityCommandTarget(this.priorityCommandsCookie);
						this.priorityCommandsCookie = 0;
					}
					if (this.connectedProjectList.Count > 0)
					{
						while (this.connectedProjectList.Count > 0)
						{
							VsWiXProject item = this.connectedProjectList.Values[0];
							this.connectedProjectList.Remove(item.UniqueName);
							item.Dispose();
						}
					}
					if (this.errorListProvider != null)
					{
						this.errorListProvider.Dispose();
					}
					DesignerFactory.GetBuildManager().IsListeningToBuildEvents = false;
					DesignerFactory.CleanupFactory();
					GC.SuppressFinalize(this);
				}
			}
			base.Dispose(disposing);
		}

		public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
		{
			int num = -2147221244;
			if (pguidCmdGroup == GuidList.guidWiXDesignerPkgCmdSet || pguidCmdGroup == VSConstants.GUID_VSStandardCommandSet97)
			{
				if (nCmdID > 883)
				{
					if (nCmdID > 893)
					{
						if (nCmdID - 1603 <= 1)
						{
							this.BeforeBuildStarted(vsBuildScope.vsBuildScopeProject);
							return num;
						}
						if (nCmdID != 23056)
						{
							return num;
						}
						else
						{
							goto Label1;
						}
					}
					else
					{
						if (nCmdID - 886 <= 1 || nCmdID - 892 <= 1)
						{
							this.BeforeBuildStarted(vsBuildScope.vsBuildScopeProject);
							return num;
						}
						return num;
					}
					this.BeforeBuildStarted(vsBuildScope.vsBuildScopeProject);
					return num;
				}
				else if (nCmdID <= 289)
				{
					if (nCmdID - 257 <= 5)
					{
						goto Label1;
					}
					if (nCmdID != 289)
					{
						return num;
					}
					else if (this.HasActiveDesigner())
					{
						return 0;
					}
					else
					{
						return num;
					}
				}
				else if (nCmdID != 331)
				{
					if (nCmdID - 882 <= 1)
					{
						this.BeforeBuildStarted(vsBuildScope.vsBuildScopeSolution);
						return num;
					}
					else
					{
						return num;
					}
				}
				else if (this.ExecuteContextAction(nCmdID))
				{
					return 0;
				}
				else
				{
					return num;
				}
			Label1:
				if (pguidCmdGroup == GuidList.guidWiXDesignerPkgCmdSet)
				{
					this.ProcessButtonClick(nCmdID);
					num = 0;
				}
			}
			return num;
		}

		private bool ExecuteContextAction(VSConstants.VSStd97CmdID command)
		{
			bool flag = false;
			if (this.connectedProjectList.Count > 0)
			{
				try
				{
					foreach (VsWiXProject value in this.connectedProjectList.Values)
					{
						if (value.ActiveDesigner == null)
						{
							continue;
						}
						if (command <= 16)
						{
							if (command == 15)
							{
								flag = false;
								break;
							}
							else if (command == 16)
							{
								flag = false;
								break;
							}
							else
							{
								break;
							}
						}
						else if (command == 26)
						{
							flag = false;
							break;
						}
						else if (command == 331)
						{
							try
							{
								value.SaveWiXFiles(value.ActiveDesigner.ID);
								break;
							}
							finally
							{
								flag = true;
							}
						}
						else
						{
							break;
						}
					}
				}
				catch (Exception exception)
				{
					DTEHelperObject.ShowErrorDialog(this, exception);
				}
			}
			return flag;
		}

		private void FileSystemEditor_Callback(object sender, EventArgs e)
		{
			this.ProcessButtonClick(257);
		}

		private void FileTypesEditor_Callback(object sender, EventArgs e)
		{
			this.ProcessButtonClick(259);
		}

		protected override void Finalize()
		{
			try
			{
				this.Dispose(true);
			}
			finally
			{
				base.Finalize();
			}
		}

		private IVsHierarchy GetHierarchy(Project project)
		{
			if (project == null)
			{
				return null;
			}
			IVsHierarchy vsHierarchy = null;
			try
			{
				(this.GetService(typeof(SVsSolution)) as IVsSolution).GetProjectOfUniqueName(project.FullName, out vsHierarchy);
			}
			catch (Exception exception)
			{
			}
			return vsHierarchy;
		}

		private IVsHierarchy GetHierarchy(string projectPath)
		{
			if (string.IsNullOrEmpty(projectPath))
			{
				return null;
			}
			IVsHierarchy vsHierarchy = null;
			try
			{
				(this.GetService(typeof(SVsSolution)) as IVsSolution).GetProjectOfUniqueName(projectPath, out vsHierarchy);
			}
			catch (Exception exception)
			{
			}
			return vsHierarchy;
		}

		internal string GetLocation()
		{
			return Path.GetDirectoryName((new Uri(base.GetType().Assembly.Location)).LocalPath);
		}

		internal static string GetProductInstallationBinDirectory()
		{
			string value;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\.NETFramework\\AssemblyFolders\\Designer for Visual Studio WiX Setup Projects", false))
			{
				if (registryKey == null)
				{
					return string.Empty;
				}
				else
				{
					value = (string)registryKey.GetValue(string.Empty, string.Empty);
				}
			}
			return value;
		}

		private Project GetProject(IVsHierarchy hierarchy, bool throwOnError)
		{
			object obj;
			if (!throwOnError)
			{
				hierarchy.GetProperty(-2, -2027, out obj);
			}
			else
			{
				ErrorHandler.ThrowOnFailure(hierarchy.GetProperty(-2, -2027, out obj));
			}
			return obj as Project;
		}

		private string GetProjectGuid(string filePath)
		{
			string value;
			try
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(filePath);
				XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("ProjectGuid");
				if (elementsByTagName.Count <= 0)
				{
					elementsByTagName = xmlDocument.GetElementsByTagName("VisualStudioProject");
					if (elementsByTagName != null && elementsByTagName.Count > 0)
					{
						XmlAttribute itemOf = elementsByTagName[0].Attributes["ProjectGUID"];
						if (itemOf != null)
						{
							value = itemOf.Value;
							return value;
						}
					}
					return string.Empty;
				}
				else
				{
					value = elementsByTagName[0].FirstChild.Value;
				}
			}
			catch (Exception exception)
			{
				return string.Empty;
			}
			return value;
		}

		private Guid GetProjectGuid(IVsHierarchy hierarchy)
		{
			Guid guid;
			hierarchy.GetGuidProperty(-2, -2059, out guid);
			return guid;
		}

		private string GetProjectGuid_String(IVsHierarchy hierarchy)
		{
			Guid guid;
			hierarchy.GetGuidProperty(-2, -2059, out guid);
			return guid.ToString("B").ToUpper();
		}

		private string GetProjectUniqueName(IVsHierarchy hierarchy)
		{
			if (hierarchy != null)
			{
				Project project = this.GetProject(hierarchy, false);
				if (project != null && !string.IsNullOrEmpty(project.UniqueName))
				{
					return project.UniqueName;
				}
			}
			return string.Empty;
		}

		public object GetServiceObject(Type serviceType)
		{
			return this.GetService(serviceType);
		}

		public ITrackSelection GetTrackSelection()
		{
			IVsWindowFrame vsWindowFrame;
			object obj;
			Guid guid = new Guid("{EEFA5220-E298-11D0-8F78-00A0C9110057}");
			IVsUIShell service = (IVsUIShell)this.GetService(typeof(SVsUIShell));
			if (service == null || service.FindToolWindow(524288, ref guid, out vsWindowFrame) != 0)
			{
				return null;
			}
			vsWindowFrame.GetProperty(-3002, out obj);
			return (new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)obj)).GetService(typeof(STrackSelection)) as ITrackSelection;
		}

		internal static List<string> GetVSInstallDir(int vsVersion)
		{
			int num;
			List<string> strs = new List<string>();
			try
			{
				IEnumSetupInstances variable = ((SetupConfiguration)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("177F0C4A-1CD3-4DE7-A32C-71DBBB9FA36D")))).EnumInstances();
				variable.Reset();
				ISetupInstance[] variable1 = new ISetupInstance[1];
				int num1 = 1;
				while (num1 == 1)
				{
					variable.Next(1, variable1, out num1);
					if (variable1.Length == 0 || variable1[0] == null)
					{
						continue;
					}
					string installationPath = variable1[0].GetInstallationPath();
					string[] strArrays = variable1[0].GetInstallationVersion().Split(new char[] { '.' });
					if (strArrays.Length != 0 && int.TryParse(strArrays[0], out num) && vsVersion == num)
					{
						if (!installationPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
						{
							strs.Add(installationPath);
						}
						else
						{
							strs.Add(installationPath.TrimEnd(new char[] { Path.DirectorySeparatorChar }));
						}
					}
					Marshal.ReleaseComObject(variable1[0]);
					variable1[0] = null;
				}
			}
			catch (Exception exception)
			{
			}
			return strs;
		}

		internal static List<string> GetVSInstanceID(int vsVersion)
		{
			List<string> strs = new List<string>();
			if (vsVersion >= 15)
			{
				try
				{
					IEnumSetupInstances variable = ((SetupConfiguration)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("177F0C4A-1CD3-4DE7-A32C-71DBBB9FA36D")))).EnumInstances();
					variable.Reset();
					ISetupInstance[] variable1 = new ISetupInstance[1];
					int num = 1;
					while (num == 1)
					{
						variable.Next(1, variable1, out num);
						if (variable1.Length == 0 || variable1[0] == null)
						{
							continue;
						}
						strs.Add(variable1[0].GetInstanceId());
						Marshal.ReleaseComObject(variable1[0]);
						variable1[0] = null;
					}
				}
				catch (Exception exception)
				{
				}
			}
			return strs;
		}

		internal static bool GetVSRegistryFile(int version, out List<string> files, out List<string> instances)
		{
			bool flag;
			files = null;
			instances = null;
			if (version >= 15)
			{
				try
				{
					string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
					List<string> vSInstanceID = VsPackage.GetVSInstanceID(15);
					if (vSInstanceID.Count <= 0)
					{
						return false;
					}
					else
					{
						files = new List<string>();
						instances = new List<string>();
						foreach (string str in vSInstanceID)
						{
							string str1 = Path.Combine(folderPath, string.Format("Microsoft\\VisualStudio\\{0}.0_{1}\\privateregistry.bin", version.ToString(), str));
							if (!File.Exists(str1))
							{
								continue;
							}
							files.Add(str1);
							instances.Add(str);
						}
						flag = true;
					}
				}
				catch
				{
					return false;
				}
				return flag;
			}
			return false;
		}

		public int GetVSVersion()
		{
			int num;
			if (this.application != null)
			{
				string[] strArrays = this.VSVersion.Split(new char[] { '.' });
				if (strArrays.Length != 0 && int.TryParse(strArrays[0], out num))
				{
					return num;
				}
			}
			return 0;
		}

		private bool HasActiveDesigner()
		{
			bool flag;
			if (this.connectedProjectList.Count > 0)
			{
				using (IEnumerator<VsWiXProject> enumerator = this.connectedProjectList.Values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.ActiveDesigner == null)
						{
							continue;
						}
						flag = true;
						return flag;
					}
					return false;
				}
				return flag;
			}
			return false;
		}

		protected override System.Threading.Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
		{
			this.InitPackage();
			return base.InitializeAsync(cancellationToken, progress);
		}

		private async Task<object> InitPackage()
		{
			await base.get_JoinableTaskFactory().SwitchToMainThreadAsync(new CancellationToken());
			this.vsixPackage = false;
			using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Add-in Express\\User Settings\\Designer for Visual Studio WiX Setup Projects\\", false))
			{
				if (registryKey != null)
				{
					this.allowCheckForUpdates = (int)registryKey.GetValue("CheckForUpdates", 1) > 0;
				}
			}
			this.application = this.GetService(typeof(_DTE)) as _DTE;
			if (this.application != null)
			{
				DesignerFactory.ServiceProvider = this;
				ProjectUtilities.ServiceProvider = this;
				DTEHelperObject.DTEInstance = this.application as DTE2;
				try
				{
					this.errorListProvider = new AddinExpress.Installer.WiXDesigner.ErrorList();
					DTEHelperObject.ErrorToolWindow = this.errorListProvider;
				}
				catch (Exception exception)
				{
				}
				this._selectionEvents = this.application.Events.SelectionEvents;
				this._selectionEvents.OnChange += new _dispSelectionEvents_OnChangeEventHandler(this.selectionEvents_OnChange);
				this.solution = this.GetService(typeof(SVsSolution)) as IVsSolution;
				if (this.solution == null)
				{
					DTEHelperObject.ReportError("Unable to get the SVsSolution service.");
				}
				else
				{
					this.solution.AdviseSolutionEvents(this, out this.solutionEventsCookie);
				}
				this.runningDocumentTable = new RunningDocumentTable(this);
				this.runningDocumentTableCookie = this.runningDocumentTable.Advise(this);
				this.priorityCommands = this.GetService(typeof(SVsRegisterPriorityCommandTarget)) as IVsRegisterPriorityCommandTarget;
				if (this.priorityCommands != null)
				{
					this.priorityCommands.RegisterPriorityCommandTarget(0, this, out this.priorityCommandsCookie);
				}
				OleMenuCommandService service = this.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
				if (service != null)
				{
					CommandID commandID = new CommandID(GuidList.guidWiXDesignerPkgCmdSet, 257);
					OleMenuCommand oleMenuCommand = new OleMenuCommand(new EventHandler(this.FileSystemEditor_Callback), commandID)
					{
						Visible = false
					};
					oleMenuCommand.add_BeforeQueryStatus(new EventHandler(this.SolutionToolbarCommands_QueryStatus));
					service.AddCommand(oleMenuCommand);
					commandID = new CommandID(GuidList.guidWiXDesignerPkgCmdSet, 258);
					oleMenuCommand = new OleMenuCommand(new EventHandler(this.RegistryEditor_Callback), commandID)
					{
						Visible = false
					};
					service.AddCommand(oleMenuCommand);
					commandID = new CommandID(GuidList.guidWiXDesignerPkgCmdSet, 259);
					oleMenuCommand = new OleMenuCommand(new EventHandler(this.FileTypesEditor_Callback), commandID)
					{
						Visible = false
					};
					service.AddCommand(oleMenuCommand);
					commandID = new CommandID(GuidList.guidWiXDesignerPkgCmdSet, 260);
					oleMenuCommand = new OleMenuCommand(new EventHandler(this.UserInterfaceEditor_Callback), commandID)
					{
						Visible = false
					};
					service.AddCommand(oleMenuCommand);
					commandID = new CommandID(GuidList.guidWiXDesignerPkgCmdSet, 261);
					oleMenuCommand = new OleMenuCommand(new EventHandler(this.CustomActionsEditor_Callback), commandID)
					{
						Visible = false
					};
					service.AddCommand(oleMenuCommand);
					commandID = new CommandID(GuidList.guidWiXDesignerPkgCmdSet, 262);
					oleMenuCommand = new OleMenuCommand(new EventHandler(this.LaunchConditionsEditor_Callback), commandID)
					{
						Visible = false
					};
					service.AddCommand(oleMenuCommand);
				}
				DesignerFactory.GetBuildManager().IsListeningToBuildEvents = true;
				DesignerFactory.GetBuildManager().BuildStarted += new EmptyEvent(this.BuildManager_BuildStarted);
				DesignerFactory.GetBuildManager().BuildStopped += new EmptyEvent(this.BuildManager_BuildStopped);
				DesignerFactory.GetBuildManager().BuildProjConfigBegin += new BuildProjConfigBegin_EventHandler(this.BuildManager_BuildProjConfigBegin);
				DesignerFactory.GetBuildManager().BuildProjConfigDone += new BuildProjConfigDone_EventHandler(this.BuildManager_BuildProjConfigDone);
				SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(this.OnUserPreferenceChanged);
			}
			return null;
		}

		private bool IsMergeModuleProject(Project vsProject)
		{
			bool flag;
			if (vsProject != null)
			{
				try
				{
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.Load(vsProject.FullName);
					XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("OutputType");
					if (elementsByTagName.Count > 0)
					{
						XmlNode itemOf = elementsByTagName[0];
						if (itemOf.FirstChild != null)
						{
							flag = itemOf.FirstChild.Value.Equals("Module", StringComparison.OrdinalIgnoreCase);
							return flag;
						}
					}
					return false;
				}
				catch (Exception exception)
				{
					return false;
				}
				return flag;
			}
			return false;
		}

		private bool IsMiscellaneousProject(IVsHierarchy hierarchy)
		{
			if (hierarchy != null)
			{
				Project project = this.GetProject(hierarchy, false);
				if (project != null && (project.Kind.Equals("{66A26720-8FB5-11D2-AA7E-00C04F688DDE}", StringComparison.InvariantCultureIgnoreCase) || project.Kind.Equals("{66A2671D-8FB5-11D2-AA7E-00C04F688DDE}", StringComparison.InvariantCultureIgnoreCase)))
				{
					return true;
				}
			}
			return false;
		}

		private bool IsSupportedProject(IVsHierarchy hierarchy)
		{
			if (hierarchy != null)
			{
				Project project = this.GetProject(hierarchy, false);
				if (project != null && project.Kind.Equals("{930C7802-8A8C-48F9-8165-68863BCCD9DD}", StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		private bool IsSupportedProject(IVsHierarchy hierarchy, out Project vsProject)
		{
			vsProject = null;
			if (hierarchy != null)
			{
				vsProject = this.GetProject(hierarchy, false);
				if (vsProject != null && vsProject.Kind.Equals("{930C7802-8A8C-48F9-8165-68863BCCD9DD}", StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		private bool IsSupportedProject(Project vsProject)
		{
			if (vsProject != null && vsProject.Kind.Equals("{930C7802-8A8C-48F9-8165-68863BCCD9DD}", StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}
			return false;
		}

		private void LaunchConditionsEditor_Callback(object sender, EventArgs e)
		{
			this.ProcessButtonClick(262);
		}

		public int LoadUserOptions([In] IVsSolutionPersistence pPersistence, [In] uint grfLoadOpts)
		{
			try
			{
				pPersistence.LoadPackageUserOpts(this, "DesignerVSWiXUserOptions");
			}
			catch (Exception exception)
			{
				DTEHelperObject.ShowErrorDialog(this, exception);
			}
			return 0;
		}

		public int OnAfterAddDirectoriesEx(int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSADDDIRECTORYFLAGS[] rgFlags)
		{
			return -2147467263;
		}

		public int OnAfterAddFilesEx(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSADDFILEFLAGS[] rgFlags)
		{
			Project project;
			try
			{
				if (cProjects > 0 && rgpProjects != null && rgpszMkDocuments != null)
				{
					for (int i = 0; i < (int)rgpProjects.Length; i++)
					{
						IVsHierarchy vsHierarchy = rgpProjects[i] as IVsHierarchy;
						if (vsHierarchy != null && this.IsSupportedProject(vsHierarchy, out project))
						{
							string projectGuidString = this.GetProjectGuid_String(vsHierarchy);
							if (this.connectedProjectList.ContainsKey(projectGuidString))
							{
								VsWiXProject item = this.connectedProjectList[projectGuidString];
								int num = ((int)rgFirstIndices.Length > i + 1 ? rgFirstIndices[i + 1] : (int)rgpszMkDocuments.Length);
								for (int j = rgFirstIndices[i]; j < num; j++)
								{
									item.OnFileAdded(rgpszMkDocuments[j], 0);
								}
							}
						}
					}
				}
			}
			catch (Exception exception)
			{
				DTEHelperObject.ShowErrorDialog(this, exception);
			}
			return 0;
		}

		public int OnAfterAsynchOpenProject(IVsHierarchy hierarchy, int added)
		{
			return -2147467263;
		}

		public int OnAfterAttributeChange(uint docCookie, uint grfAttribs)
		{
			return -2147467263;
		}

		public int OnAfterAttributeChangeEx(uint docCookie, uint grfAttribs, IVsHierarchy pHierOld, uint itemidOld, string pszMkDocumentOld, IVsHierarchy pHierNew, uint itemidNew, string pszMkDocumentNew)
		{
			return -2147467263;
		}

		public int OnAfterChangeProjectParent(IVsHierarchy hierarchy)
		{
			Project project;
			try
			{
				if (this.IsSupportedProject(hierarchy, out project))
				{
					string projectGuidString = this.GetProjectGuid_String(hierarchy);
					if (this.connectedProjectList.ContainsKey(projectGuidString))
					{
						this.connectedProjectList[projectGuidString].OnProjectParentChanged();
					}
				}
			}
			catch (Exception exception)
			{
				DTEHelperObject.ShowErrorDialog(this, exception);
			}
			return 0;
		}

		public int OnAfterCloseSolution(object reserved)
		{
			this.projectsWereOpened = false;
			return 0;
		}

		public int OnAfterClosingChildren(IVsHierarchy hierarchy)
		{
			return -2147467263;
		}

		public int OnAfterDocumentWindowHide(uint docCookie, IVsWindowFrame pFrame)
		{
			return -2147467263;
		}

		public int OnAfterFirstDocumentLock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
		{
			return -2147467263;
		}

		public int OnAfterLastDocumentUnlock(IVsHierarchy pHier, uint itemid, string pszMkDocument, int fClosedWithoutSaving)
		{
			return -2147467263;
		}

		public int OnAfterLoadProject(IVsHierarchy stubHierarchy, IVsHierarchy realHierarchy)
		{
			return -2147467263;
		}

		public int OnAfterMergeSolution(object pUnkReserved)
		{
			return -2147467263;
		}

		public int OnAfterOpeningChildren(IVsHierarchy hierarchy)
		{
			return -2147467263;
		}

		public int OnAfterOpenProject(IVsHierarchy hierarchy, int added)
		{
			Project project;
			if (!this.IsMiscellaneousProject(hierarchy))
			{
				this.projectsWereOpened = true;
			}
			try
			{
				if (this.IsSupportedProject(hierarchy, out project))
				{
					string projectGuidString = this.GetProjectGuid_String(hierarchy);
					if (!this.connectedProjectList.ContainsKey(projectGuidString))
					{
						VsWiXProject vsWiXProject = new VsWiXProject(project, hierarchy, new Guid(projectGuidString));
						if (!vsWiXProject.Connect())
						{
							vsWiXProject.Dispose();
						}
						else
						{
							this.connectedProjectList.Add(projectGuidString, vsWiXProject);
							vsWiXProject.OnProjectOpened(added > 0);
							if (!string.IsNullOrEmpty(this.reloadedProjectGuid) && this.reloadedProjectGuid.Equals(projectGuidString, StringComparison.OrdinalIgnoreCase))
							{
								this.selectionEvents_OnChange();
							}
						}
					}
				}
			}
			catch (Exception exception)
			{
				DTEHelperObject.ShowErrorDialog(this, exception);
			}
			return 0;
		}

		public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
		{
			try
			{
				try
				{
					if (!this.projectsWereOpened)
					{
						foreach (IVsProject loadedProject in ProjectUtilities.LoadedProjects)
						{
							if (loadedProject == null)
							{
								continue;
							}
							this.OnAfterOpenProject(loadedProject as IVsHierarchy, 0);
						}
					}
					if (this.connectedProjectList.Count > 0)
					{
						if (this.allowCheckForUpdates)
						{
							this.allowCheckForUpdates = false;
							this.CheckForUpdates("http://www.add-in-express.com/files/adxversions/wix-designer.txt");
						}
						foreach (VsWiXProject value in this.connectedProjectList.Values)
						{
							value.OnSolutionOpened();
						}
					}
				}
				catch (Exception exception)
				{
					DTEHelperObject.ShowErrorDialog(this, exception);
				}
			}
			finally
			{
				this.allowLoadUserOptions = true;
			}
			return 0;
		}

		public int OnAfterRemoveDirectories(int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEDIRECTORYFLAGS[] rgFlags)
		{
			return -2147467263;
		}

		public int OnAfterRemoveFiles(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEFILEFLAGS[] rgFlags)
		{
			Project project;
			try
			{
				if (cProjects > 0 && rgpProjects != null && rgpszMkDocuments != null)
				{
					for (int i = 0; i < (int)rgpProjects.Length; i++)
					{
						IVsHierarchy vsHierarchy = rgpProjects[i] as IVsHierarchy;
						if (vsHierarchy != null && this.IsSupportedProject(vsHierarchy, out project))
						{
							string projectGuidString = this.GetProjectGuid_String(vsHierarchy);
							if (this.connectedProjectList.ContainsKey(projectGuidString))
							{
								VsWiXProject item = this.connectedProjectList[projectGuidString];
								int num = ((int)rgFirstIndices.Length > i + 1 ? rgFirstIndices[i + 1] : (int)rgpszMkDocuments.Length);
								for (int j = rgFirstIndices[i]; j < num; j++)
								{
									item.OnFileRemoved(rgpszMkDocuments[j], 0);
								}
							}
						}
					}
				}
			}
			catch (Exception exception)
			{
				DTEHelperObject.ShowErrorDialog(this, exception);
			}
			return 0;
		}

		public int OnAfterRenameDirectories(int cProjects, int cDirs, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEDIRECTORYFLAGS[] rgFlags)
		{
			return -2147467263;
		}

		public int OnAfterRenameFiles(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEFILEFLAGS[] rgFlags)
		{
			Project project;
			try
			{
				if (cProjects > 0 && rgpProjects != null && rgszMkOldNames != null && rgszMkNewNames != null && (int)rgszMkOldNames.Length == (int)rgszMkNewNames.Length)
				{
					for (int i = 0; i < (int)rgpProjects.Length; i++)
					{
						IVsHierarchy vsHierarchy = rgpProjects[i] as IVsHierarchy;
						if (vsHierarchy != null && this.IsSupportedProject(vsHierarchy, out project))
						{
							string projectGuidString = this.GetProjectGuid_String(vsHierarchy);
							if (this.connectedProjectList.ContainsKey(projectGuidString))
							{
								VsWiXProject item = this.connectedProjectList[projectGuidString];
								int num = ((int)rgFirstIndices.Length > i + 1 ? rgFirstIndices[i + 1] : (int)rgszMkOldNames.Length);
								for (int j = rgFirstIndices[i]; j < num; j++)
								{
									item.OnFileRenamed(rgszMkOldNames[j], rgszMkNewNames[j], 0);
								}
							}
						}
					}
				}
			}
			catch (Exception exception)
			{
				DTEHelperObject.ShowErrorDialog(this, exception);
			}
			return 0;
		}

		public int OnAfterRenameProject(IVsHierarchy hierarchy)
		{
			Project project;
			try
			{
				if (this.IsSupportedProject(hierarchy, out project))
				{
					string projectGuidString = this.GetProjectGuid_String(hierarchy);
					if (this.connectedProjectList.ContainsKey(projectGuidString))
					{
						this.connectedProjectList[projectGuidString].OnProjectRenamed();
					}
				}
				else if (this.connectedProjectList.Count > 0)
				{
					foreach (VsWiXProject value in this.connectedProjectList.Values)
					{
						value.OnSolutionProjectRenamed(hierarchy);
					}
				}
			}
			catch (Exception exception)
			{
				DTEHelperObject.ShowErrorDialog(this, exception);
			}
			return 0;
		}

		public int OnAfterSave(uint docCookie)
		{
			try
			{
				if (this.connectedProjectList.Count > 0)
				{
					foreach (VsWiXProject value in this.connectedProjectList.Values)
					{
						value.OnAfterSave(docCookie);
					}
				}
			}
			catch (Exception exception)
			{
				DTEHelperObject.ShowErrorDialog(this, exception);
			}
			return 0;
		}

		public int OnAfterSaveAll()
		{
			return -2147467263;
		}

		public int OnAfterSccStatusChanged(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, uint[] rgdwSccStatus)
		{
			return -2147467263;
		}

		public int OnBeforeCloseProject(IVsHierarchy hierarchy, int removed)
		{
			Project project;
			try
			{
				if (this.IsSupportedProject(hierarchy, out project))
				{
					string projectGuidString = this.GetProjectGuid_String(hierarchy);
					if (this.connectedProjectList.ContainsKey(projectGuidString))
					{
						VsWiXProject item = this.connectedProjectList[projectGuidString];
						item.OnProjectClosing();
						this.connectedProjectList.Remove(projectGuidString);
						item.Dispose();
					}
				}
			}
			catch (Exception exception)
			{
				DTEHelperObject.ShowErrorDialog(this, exception);
			}
			return 0;
		}

		public int OnBeforeCloseSolution(object pUnkReserved)
		{
			try
			{
				if (this.connectedProjectList.Count > 0)
				{
					foreach (VsWiXProject value in this.connectedProjectList.Values)
					{
						value.OnSolutionClosing();
					}
				}
			}
			catch (Exception exception)
			{
				DTEHelperObject.ShowErrorDialog(this, exception);
			}
			return 0;
		}

		public int OnBeforeClosingChildren(IVsHierarchy hierarchy)
		{
			return -2147467263;
		}

		public int OnBeforeDocumentWindowShow(uint docCookie, int fFirstShow, IVsWindowFrame pFrame)
		{
			return -2147467263;
		}

		public int OnBeforeFirstDocumentLock(IVsHierarchy pHier, uint itemid, string pszMkDocument)
		{
			return -2147467263;
		}

		public int OnBeforeLastDocumentUnlock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
		{
			return -2147467263;
		}

		public int OnBeforeOpeningChildren(IVsHierarchy hierarchy)
		{
			return -2147467263;
		}

		public int OnBeforeSave(uint docCookie)
		{
			return -2147467263;
		}

		public int OnBeforeUnloadProject(IVsHierarchy realHierarchy, IVsHierarchy rtubHierarchy)
		{
			return -2147467263;
		}

		public int OnProjectLoadFailure([In] IVsHierarchy pStubHierarchy, [In] string pszProjectName, [In] string pszProjectMk, [In] string pszKey)
		{
			return -2147467263;
		}

		public int OnQueryAddDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYADDDIRECTORYFLAGS[] rgFlags, VSQUERYADDDIRECTORYRESULTS[] pSummaryResult, VSQUERYADDDIRECTORYRESULTS[] rgResults)
		{
			return -2147467263;
		}

		public int OnQueryAddFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYADDFILEFLAGS[] rgFlags, VSQUERYADDFILERESULTS[] pSummaryResult, VSQUERYADDFILERESULTS[] rgResults)
		{
			return -2147467263;
		}

		public int OnQueryChangeProjectParent(IVsHierarchy hierarchy, IVsHierarchy newParentHier, ref int cancel)
		{
			return -2147467263;
		}

		public int OnQueryCloseProject(IVsHierarchy hierarchy, int removing, ref int cancel)
		{
			bool flag = true;
			try
			{
				try
				{
					if (this.connectedProjectList.Count > 0)
					{
						using (IEnumerator<VsWiXProject> enumerator = this.connectedProjectList.Values.GetEnumerator())
						{
							do
							{
								if (!enumerator.MoveNext())
								{
									break;
								}
								flag = enumerator.Current.CanCloseProject();
							}
							while (flag);
						}
					}
				}
				catch (Exception exception)
				{
				}
			}
			finally
			{
				cancel = (flag ? 0 : 1);
			}
			return 0;
		}

		public int OnQueryCloseSolution(object pUnkReserved, ref int cancel)
		{
			return -2147467263;
		}

		public int OnQueryRemoveDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYREMOVEDIRECTORYFLAGS[] rgFlags, VSQUERYREMOVEDIRECTORYRESULTS[] pSummaryResult, VSQUERYREMOVEDIRECTORYRESULTS[] rgResults)
		{
			return -2147467263;
		}

		public int OnQueryRemoveFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYREMOVEFILEFLAGS[] rgFlags, VSQUERYREMOVEFILERESULTS[] pSummaryResult, VSQUERYREMOVEFILERESULTS[] rgResults)
		{
			return -2147467263;
		}

		public int OnQueryRenameDirectories(IVsProject pProject, int cDirs, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEDIRECTORYFLAGS[] rgFlags, VSQUERYRENAMEDIRECTORYRESULTS[] pSummaryResult, VSQUERYRENAMEDIRECTORYRESULTS[] rgResults)
		{
			return -2147467263;
		}

		public int OnQueryRenameFiles(IVsProject pProject, int cFiles, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEFILEFLAGS[] rgFlags, VSQUERYRENAMEFILERESULTS[] pSummaryResult, VSQUERYRENAMEFILERESULTS[] rgResults)
		{
			return -2147467263;
		}

		public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int cancel)
		{
			return -2147467263;
		}

		private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
		{
			if ((e.Category == UserPreferenceCategory.Color || e.Category == UserPreferenceCategory.VisualStyle) && this.connectedProjectList.Count > 0)
			{
				foreach (VsWiXProject value in this.connectedProjectList.Values)
				{
					value.OnThemeChanged();
				}
			}
		}

		[ComRegisterFunction]
		public static void PackageRegister(Type t)
		{
			List<string> strs;
			if (VsPackage.CheckInstalledVSVersion2(16, out strs))
			{
				for (int i = 0; i < strs.Count; i++)
				{
					if (!strs[i].EndsWith("BuildTools", StringComparison.OrdinalIgnoreCase))
					{
						string str = Path.Combine(Path.Combine(strs[i], "Common7\\IDE\\Extensions"), "Add-in Express\\Designer for Visual Studio WiX Setup Projects\\Wizard");
						if (!Directory.Exists(str))
						{
							Directory.CreateDirectory(str);
						}
						string str1 = Resources.extension;
						str1 = str1.Replace("$Version$", typeof(VsPackage).Assembly.GetName().Version.ToString(4));
						str1 = str1.Replace("$VsVersion$", "16.0");
						VsPackage.SaveItem(Path.Combine(str, "extension.vsixmanifest"), str1, Encoding.UTF8);
						string str2 = Resources.pkgdef;
						str2 = str2.Replace("$Version1$", typeof(VsPackage).Assembly.GetName().Version.ToString(3));
						str2 = str2.Replace("$Version2$", "2019");
						str2 = str2.Replace("$Version3$", "v16");
						str2 = str2.Replace("$ProductInstallationBinDir$", VsPackage.GetProductInstallationBinDirectory());
						VsPackage.SaveItem(Path.Combine(str, "AddinExpress.WiXDesignerPkg.pkgdef"), str2, Encoding.UTF8);
					}
				}
			}
		}

		[ComUnregisterFunction]
		public static void PackageUnregister(Type t)
		{
			List<string> strs;
			if (VsPackage.CheckInstalledVSVersion2(16, out strs))
			{
				for (int i = 0; i < strs.Count; i++)
				{
					if (!strs[i].EndsWith("BuildTools", StringComparison.OrdinalIgnoreCase))
					{
						string str = Path.Combine(strs[i], "Common7\\IDE\\Extensions");
						string str1 = Path.Combine(str, "Add-in Express\\Designer for Visual Studio WiX Setup Projects");
						if (Directory.Exists(str1))
						{
							Directory.Delete(str1, true);
						}
						str1 = Path.Combine(str, "Add-in Express");
						if (Directory.Exists(str1) && Directory.GetFiles(str1, "*", SearchOption.AllDirectories).Length == 0)
						{
							Directory.Delete(str1, true);
						}
					}
				}
			}
		}

		private void ProcessButtonClick(uint button)
		{
			Project project;
			try
			{
				IList<IVsProject> projectsOfCurrentSelections = ProjectUtilities.GetProjectsOfCurrentSelections();
				if (projectsOfCurrentSelections.Count == 1)
				{
					IVsHierarchy item = projectsOfCurrentSelections[0] as IVsHierarchy;
					if (this.IsSupportedProject(item, out project))
					{
						string projectGuidString = this.GetProjectGuid_String(item);
						if (!this.connectedProjectList.ContainsKey(projectGuidString))
						{
							this.projectsWereOpened = false;
							this.OnAfterOpenSolution(null, 0);
						}
						if (this.connectedProjectList.ContainsKey(projectGuidString))
						{
							Cursor.Current = Cursors.WaitCursor;
							try
							{
								this.connectedProjectList[projectGuidString].ShowWindow(VsWiXProject.GetEditorNameByButtonID(button), true, true);
							}
							finally
							{
								Cursor.Current = Cursors.Default;
							}
						}
					}
				}
			}
			catch (Exception exception)
			{
				DTEHelperObject.ShowErrorDialog(this, exception);
			}
		}

		public int QuerySaveSolutionProps([In] IVsHierarchy pHierarchy, [Out] VSQUERYSAVESLNPROPS[] pqsspSave)
		{
			return -2147467263;
		}

		public int QueryStatus(ref Guid guidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
		{
			if (prgCmds == null)
			{
				return -2147024809;
			}
			if (guidCmdGroup != GuidList.guidWiXDesignerPkgCmdSet && guidCmdGroup != VSConstants.GUID_VSStandardCommandSet97)
			{
				return -2147221248;
			}
			OLECMDF oLECMDF = (OLECMDF)0;
			uint num = prgCmds[0].cmdID;
			if (num <= 262)
			{
				if (num - 15 > 1 && num != 26)
				{
					goto Label3;
				}
				if (this.HasActiveDesigner())
				{
					oLECMDF = OLECMDF.OLECMDF_SUPPORTED;
					prgCmds[0].cmdf = (uint)oLECMDF;
					return 0;
				}
				else
				{
					prgCmds[0].cmdf = (uint)oLECMDF;
					return 0;
				}
			}
			else if (num != 289)
			{
				if (num != 331)
				{
					goto Label2;
				}
				if (this.HasActiveDesigner())
				{
					oLECMDF = OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED;
					prgCmds[0].cmdf = (uint)oLECMDF;
					return 0;
				}
				else
				{
					prgCmds[0].cmdf = (uint)oLECMDF;
					return 0;
				}
			}
			else if (this.HasActiveDesigner())
			{
				oLECMDF = OLECMDF.OLECMDF_SUPPORTED;
				prgCmds[0].cmdf = (uint)oLECMDF;
				return 0;
			}
			else
			{
				prgCmds[0].cmdf = (uint)oLECMDF;
				return 0;
			}
			return -2147221248;
			prgCmds[0].cmdf = (uint)oLECMDF;
			return 0;
		Label1:
			if (guidCmdGroup != GuidList.guidWiXDesignerPkgCmdSet)
			{
				return -2147221248;
			}
			Array activeSolutionProjects = null;
			this.prevVisibilityState = false;
			oLECMDF = OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_INVISIBLE;
			try
			{
				activeSolutionProjects = this.application.ActiveSolutionProjects as Array;
			}
			catch
			{
			}
			if (activeSolutionProjects != null && activeSolutionProjects.Length == 1)
			{
				Project value = (Project)activeSolutionProjects.GetValue(0);
				if (this.IsSupportedProject(this.GetHierarchy(value)))
				{
					oLECMDF = OLECMDF.OLECMDF_SUPPORTED;
					if (!this.IsMergeModuleProject(value) || prgCmds[0].cmdID != 260 && prgCmds[0].cmdID != 262)
					{
						this.prevVisibilityState = true;
						oLECMDF |= OLECMDF.OLECMDF_ENABLED;
						prgCmds[0].cmdf = (uint)oLECMDF;
						return 0;
					}
					else
					{
						oLECMDF |= OLECMDF.OLECMDF_INVISIBLE;
						prgCmds[0].cmdf = (uint)oLECMDF;
						return 0;
					}
				}
				else
				{
					prgCmds[0].cmdf = (uint)oLECMDF;
					return 0;
				}
			}
			else
			{
				prgCmds[0].cmdf = (uint)oLECMDF;
				return 0;
			}
		Label2:
			if (num == 23056)
			{
				goto Label1;
			}
			return -2147221248;
		Label3:
			if (num - 257 <= 5)
			{
				goto Label1;
			}
			return -2147221248;
		}

		public int ReadSolutionProps([In] IVsHierarchy pHierarchy, [In] string pszProjectName, [In] string pszProjectMk, [In] string pszKey, [In] int fPreLoad, [In] IPropertyBag pPropBag)
		{
			try
			{
				if ("DesignerVSWiXSolutionOptions".CompareTo(pszKey) == 0 && pHierarchy != null)
				{
					string projectGuidString = this.GetProjectGuid_String(pHierarchy);
					if (this.connectedProjectList.ContainsKey(projectGuidString))
					{
						this.connectedProjectList[projectGuidString].LoadSolutionSettings(pPropBag);
					}
				}
			}
			catch (Exception exception)
			{
				DTEHelperObject.ShowErrorDialog(this, exception);
			}
			return 0;
		}

		public int ReadUserOptions([In] IStream pOptionsStream, [In] string pszKey)
		{
			try
			{
				if (pOptionsStream != null && "DesignerVSWiXUserOptions".Equals(pszKey))
				{
					AddinExpress.Installer.WiXDesigner.DataStreamFromComStream dataStreamFromComStream = new AddinExpress.Installer.WiXDesigner.DataStreamFromComStream(pOptionsStream);
					if (dataStreamFromComStream.Length > (long)0)
					{
						Hashtable hashtables = (new BinaryFormatter()).Deserialize(dataStreamFromComStream) as Hashtable;
						if ((int)hashtables["WiXProjectsCount"] > 0)
						{
							this.userOptionsCache = hashtables;
							this.userOptionsTimer.Enabled = true;
						}
					}
				}
			}
			catch (Exception exception)
			{
				DTEHelperObject.ShowErrorDialog(this, exception);
			}
			return 0;
		}

		private void RegistryEditor_Callback(object sender, EventArgs e)
		{
			this.ProcessButtonClick(258);
		}

		public static void SaveItem(string fileName, string text, Encoding encoding)
		{
			FileStream fileStream = null;
			StreamWriter streamWriter = null;
			try
			{
				fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
				streamWriter = new StreamWriter(fileStream, encoding);
				streamWriter.Write(text);
			}
			finally
			{
				if (streamWriter != null)
				{
					streamWriter.Close();
				}
				if (fileStream != null)
				{
					fileStream.Close();
				}
			}
		}

		public int SaveSolutionProps([In] IVsHierarchy pHierarchy, [In] IVsSolutionPersistence pPersistence)
		{
			try
			{
				if (this.IsSupportedProject(pHierarchy))
				{
					string projectGuidString = this.GetProjectGuid_String(pHierarchy);
					if (this.connectedProjectList.ContainsKey(projectGuidString))
					{
						pPersistence.SavePackageSolutionProps(0, pHierarchy, this, "DesignerVSWiXSolutionOptions");
					}
				}
			}
			catch (Exception exception)
			{
				DTEHelperObject.ShowErrorDialog(this, exception);
			}
			return 0;
		}

		public int SaveUserOptions([In] IVsSolutionPersistence pPersistence)
		{
			try
			{
				pPersistence.SavePackageUserOpts(this, "DesignerVSWiXUserOptions");
			}
			catch (Exception exception)
			{
				DTEHelperObject.ShowErrorDialog(this, exception);
			}
			return 0;
		}

		private void selectionEvents_OnChange()
		{
			bool flag;
			object obj;
			IVsHierarchy vsHierarchy;
			Project project;
			Project project1;
			try
			{
				IVsHierarchy solutionWindowSelectedProject = ProjectUtilities.GetSolutionWindowSelectedProject(out obj, out vsHierarchy, out flag);
				if (solutionWindowSelectedProject != null)
				{
					if (this.IsSupportedProject(solutionWindowSelectedProject, out project))
					{
						string projectGuidString = this.GetProjectGuid_String(solutionWindowSelectedProject);
						if (!this.connectedProjectList.ContainsKey(projectGuidString))
						{
							this.reloadedProjectGuid = projectGuidString;
						}
						else
						{
							if (obj == null)
							{
								obj = project.Object.GetType().InvokeMember("NodeProperties", BindingFlags.GetProperty, null, project.Object, null);
							}
							if (obj != null)
							{
								VsWiXProject item = this.connectedProjectList[projectGuidString];
								item.OnProjectPropertiesSelected();
								item.ParseXML(false);
								if (AddinExpress.Installer.WiXDesigner.DesignTime.TypeDescriptor.InstallTypeDescriptor(obj))
								{
									item.RegisterDynamicProperties(obj);
								}
							}
						}
					}
				}
				else if (flag && this.IsSupportedProject(vsHierarchy, out project1))
				{
					string str = this.GetProjectGuid_String(vsHierarchy);
					if (this.connectedProjectList.ContainsKey(str))
					{
						this.connectedProjectList[str].SetMultiSelectMode();
					}
				}
			}
			catch (Exception exception)
			{
			}
		}

		private void SolutionToolbarCommands_QueryStatus(object sender, EventArgs e)
		{
			Project project;
			bool flag = false;
			bool flag1 = false;
			try
			{
				IList<IVsProject> projectsOfCurrentSelections = ProjectUtilities.GetProjectsOfCurrentSelections();
				if (projectsOfCurrentSelections.Count == 1 && this.IsSupportedProject(projectsOfCurrentSelections[0] as IVsHierarchy, out project))
				{
					flag = true;
					flag1 = this.IsMergeModuleProject(project);
				}
			}
			catch (Exception exception)
			{
			}
			OleMenuCommandService service = this.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
			service.FindCommand(new CommandID(GuidList.guidWiXDesignerPkgCmdSet, 257)).Visible = flag;
			service.FindCommand(new CommandID(GuidList.guidWiXDesignerPkgCmdSet, 258)).Visible = flag;
			service.FindCommand(new CommandID(GuidList.guidWiXDesignerPkgCmdSet, 259)).Visible = flag;
			service.FindCommand(new CommandID(GuidList.guidWiXDesignerPkgCmdSet, 260)).Visible = (!flag ? false : !flag1);
			service.FindCommand(new CommandID(GuidList.guidWiXDesignerPkgCmdSet, 261)).Visible = flag;
			service.FindCommand(new CommandID(GuidList.guidWiXDesignerPkgCmdSet, 262)).Visible = (!flag ? false : !flag1);
		}

		private void UserInterfaceEditor_Callback(object sender, EventArgs e)
		{
			this.ProcessButtonClick(260);
		}

		private void userOptionsTimer_Tick(object sender, EventArgs e)
		{
			if (this.allowLoadUserOptions)
			{
				this.userOptionsTimer.Enabled = false;
				if (this.connectedProjectList.Count > 0)
				{
					foreach (VsWiXProject value in this.connectedProjectList.Values)
					{
						value.LoadUserSettings(this.userOptionsCache);
					}
					foreach (VsWiXProject vsWiXProject in this.connectedProjectList.Values)
					{
						vsWiXProject.ActivateWiXDesignerIfAny();
					}
				}
			}
		}

		private void WorkerDoWork(object sender, DoWorkEventArgs e)
		{
			e.Result = string.Empty;
			WebClient webClient = new WebClient();
			try
			{
				try
				{
					System.Threading.Thread.Sleep(1000);
					e.Result = webClient.DownloadString(new Uri((string)e.Argument));
				}
				catch
				{
				}
			}
			finally
			{
				webClient.Dispose();
			}
		}

		private void WorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (!e.Cancelled && e.Error == null)
			{
				this.CompareVersions((string)e.Result);
			}
		}

		public int WriteSolutionProps([In] IVsHierarchy pHierarchy, [In] string pszKey, [In] IPropertyBag pPropBag)
		{
			try
			{
				if ("DesignerVSWiXSolutionOptions".CompareTo(pszKey) == 0 && pHierarchy != null)
				{
					string projectGuidString = this.GetProjectGuid_String(pHierarchy);
					if (this.connectedProjectList.ContainsKey(projectGuidString))
					{
						this.connectedProjectList[projectGuidString].SaveSolutionSettings(pPropBag);
					}
				}
			}
			catch (Exception exception)
			{
				DTEHelperObject.ShowErrorDialog(this, exception);
			}
			return 0;
		}

		public int WriteUserOptions([In] IStream pOptionsStream, [In] string pszKey)
		{
			try
			{
				if (pOptionsStream != null && "DesignerVSWiXUserOptions".Equals(pszKey))
				{
					Hashtable hashtables = new Hashtable();
					hashtables["WiXProjectsCount"] = this.connectedProjectList.Count;
					AddinExpress.Installer.WiXDesigner.DataStreamFromComStream dataStreamFromComStream = new AddinExpress.Installer.WiXDesigner.DataStreamFromComStream(pOptionsStream);
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					if (this.connectedProjectList.Count > 0)
					{
						foreach (VsWiXProject value in this.connectedProjectList.Values)
						{
							value.SaveUserSettings(hashtables);
						}
					}
					binaryFormatter.Serialize(dataStreamFromComStream, hashtables);
				}
			}
			catch (Exception exception)
			{
				DTEHelperObject.ShowErrorDialog(this, exception);
			}
			return 0;
		}
	}
}