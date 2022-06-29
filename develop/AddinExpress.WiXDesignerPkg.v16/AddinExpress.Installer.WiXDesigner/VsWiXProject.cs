using AddinExpress.Installer.Prerequisites;
using AddinExpress.Installer.WiXDesigner.DesignTime;
using EnvDTE;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	[ComVisible(false)]
	internal class VsWiXProject : IDisposable
	{
		private bool disposed;

		private int projectIdBase;

		private Guid projectGuid = Guid.Empty;

		private IVsHierarchy hierarchy;

		private EnvDTE.Project vsProject;

		private HierarchyListener projectListener;

		private SortedList<string, VsPaneBase> panes = new SortedList<string, VsPaneBase>();

		private bool hasActiveDesigners;

		private VsPaneBase initiallyActivePane;

		private bool solutionLoadedCompletely;

		private bool buildStarted;

		private IOleUndoManager undoManager;

		private ServiceProvider _serviceProvider;

		private WiXProjectParser projectParser;

		private Timer delayTimer;

		private bool lockChanges;

		private IVsHierarchy lastSolutionRenamedProject;

		private Dictionary<string, string> projectVariables = new Dictionary<string, string>();

		private List<ProjectDescriptor> solutionProjectList = new List<ProjectDescriptor>();

		private VsWiXProject.ProjectPropertiesObject projectProperties;

		private object _xmlLanguageService;

		private bool gettingCheckoutStatus;

		private bool multiLangSupport;

		private bool checkSupportedLanguages;

		private bool multiSelectMode;

		private readonly int FileSystemPaneID;

		private readonly int RegistryPaneID;

		private readonly int FileTypesPaneID;

		private readonly int UserInterfacePaneID;

		private readonly int CustomActionsPaneID;

		private readonly int LaunchConditionsPaneID;

		private readonly int AdvancedViewPaneID;

		public const string FileSystemPaneName = "File System";

		public const string RegistryPaneName = "Registry";

		public const string FileTypesPaneName = "File Types";

		public const string UserInterfacePaneName = "User Interface";

		public const string CustomActionsPaneName = "Custom Actions";

		public const string LaunchConditionsPaneName = "Launch Conditions";

		public const string AdvancedViewPaneName = "Advanced View";

		public static string WixNamespaceUri_V2;

		public static string WixNamespaceUri_V3;

		public static string WixNamespaceUri_V4;

		public static string WixLocalizationNamespaceUri;

		private VsWiXProject.DelayActionData delayActionEventData;

		private SortedList<uint, VsWiXProject.ReferenceDescriptor> referenceList = new SortedList<uint, VsWiXProject.ReferenceDescriptor>();

		private Dictionary<uint, VsWiXProject.WiXFileDescriptor> allWiXFiles = new Dictionary<uint, VsWiXProject.WiXFileDescriptor>();

		private bool parseDoneCheckShown;

		private string lastRemovedFilePath = string.Empty;

		public VsPaneBase ActiveDesigner
		{
			get
			{
				VsPaneBase vsPaneBase;
				if (this.panes.Count > 0)
				{
					using (IEnumerator<VsPaneBase> enumerator = this.Panes.Values.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							VsPaneBase current = enumerator.Current;
							if (!current.Focused)
							{
								continue;
							}
							vsPaneBase = current;
							return vsPaneBase;
						}
						return null;
					}
					return vsPaneBase;
				}
				return null;
			}
		}

		public Dictionary<uint, VsWiXProject.WiXFileDescriptor> AllWiXFiles
		{
			get
			{
				return this.allWiXFiles;
			}
		}

		public IVsHierarchy Hierarchy
		{
			get
			{
				return this.hierarchy;
			}
		}

		public bool IsMultiLangSupported
		{
			get
			{
				return this.multiLangSupport;
			}
		}

		public SortedList<string, VsPaneBase> Panes
		{
			get
			{
				return this.panes;
			}
		}

		public Guid ProjectGuid
		{
			get
			{
				return this.projectGuid;
			}
		}

		public string ProjectGuidString
		{
			get
			{
				return this.projectGuid.ToString("B").ToUpper();
			}
		}

		public VsWiXProject.ProjectPropertiesObject ProjectProperties
		{
			get
			{
				return this.projectProperties;
			}
		}

		public SortedList<uint, VsWiXProject.ReferenceDescriptor> References
		{
			get
			{
				return this.referenceList;
			}
		}

		public string RootDirectory
		{
			get
			{
				object obj;
				this.hierarchy.GetProperty(-2, -2021, out obj);
				return (string)obj;
			}
		}

		public IOleUndoManager UndoManager
		{
			get
			{
				return this.undoManager;
			}
		}

		public string UniqueName
		{
			get
			{
				return this.vsProject.UniqueName;
			}
		}

		public DTE VsDTE
		{
			get
			{
				if (this.vsProject == null)
				{
					return null;
				}
				return this.vsProject.DTE;
			}
		}

		public EnvDTE.Project VsProject
		{
			get
			{
				return this.vsProject;
			}
		}

		public string WixLocUINamespaceUri
		{
			get
			{
				return VsWiXProject.WixLocalizationNamespaceUri;
			}
		}

		public WiXProjectParser WiXModel
		{
			get
			{
				return this.projectParser;
			}
		}

		public string WixNamespaceUri
		{
			get
			{
				if (WixSettings.Instance.IsUsingWix4())
				{
					return VsWiXProject.WixNamespaceUri_V4;
				}
				if (WixSettings.Instance.IsUsingWix3())
				{
					return VsWiXProject.WixNamespaceUri_V3;
				}
				if (WixSettings.Instance.IsUsingWix2())
				{
					return VsWiXProject.WixNamespaceUri_V2;
				}
				return VsWiXProject.WixNamespaceUri_V3;
			}
		}

		static VsWiXProject()
		{
			VsWiXProject.WixNamespaceUri_V2 = "http://schemas.microsoft.com/wix/2003/01/wi";
			VsWiXProject.WixNamespaceUri_V3 = "http://schemas.microsoft.com/wix/2006/wi";
			VsWiXProject.WixNamespaceUri_V4 = "http://wixtoolset.org/schemas/v4/wxs";
			VsWiXProject.WixLocalizationNamespaceUri = "http://schemas.microsoft.com/wix/2006/localization";
		}

		public VsWiXProject(EnvDTE.Project vsProject, IVsHierarchy hierarchy, Guid projectGuid)
		{
			this.vsProject = vsProject;
			this.hierarchy = hierarchy;
			this.projectGuid = projectGuid;
			this.projectParser = new WiXProjectParser(this);
			this.projectParser.AfterParse += new AfterParseEventHandler(this.projectParser_AfterParse);
			this.projectProperties = new VsWiXProject.ProjectPropertiesObject(this);
			this.delayTimer = new Timer()
			{
				Interval = 500
			};
			this.delayTimer.Tick += new EventHandler(this.delayTimer_Tick);
			this.projectIdBase = this.GetHashCode();
			byte[] byteArray = projectGuid.ToByteArray();
			for (int i = 0; i < (int)byteArray.Length; i++)
			{
				this.projectIdBase += byteArray[i];
			}
			this.FileSystemPaneID = this.projectIdBase + 1;
			this.RegistryPaneID = this.projectIdBase + 2;
			this.FileTypesPaneID = this.projectIdBase + 3;
			this.UserInterfacePaneID = this.projectIdBase + 4;
			this.CustomActionsPaneID = this.projectIdBase + 5;
			this.LaunchConditionsPaneID = this.projectIdBase + 6;
			this.AdvancedViewPaneID = this.projectIdBase + 7;
			this.projectListener = new HierarchyListener(hierarchy);
			this.projectListener.OnAddItem += new EventHandler<HierarchyEventArgs>(this.projectListener_OnAddProjectItem);
			this.projectListener.OnDeleteItem += new EventHandler<HierarchyEventArgs>(this.projectListener_OnDeleteProjectItem);
			this.projectListener.OnChangedItem += new EventHandler<HierarchyEventArgs>(this.projectListener_OnChangeProjectItem);
			Microsoft.VisualStudio.OLE.Interop.IServiceProvider globalService = Package.GetGlobalService(typeof(Microsoft.VisualStudio.OLE.Interop.IServiceProvider)) as Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
			if (globalService != null)
			{
				this._serviceProvider = new ServiceProvider(globalService);
			}
		}

		internal void ActivateWiXDesignerIfAny()
		{
			if (this.initiallyActivePane != null)
			{
				try
				{
					IVsWindowFrame frame = (IVsWindowFrame)this.initiallyActivePane.get_Frame();
					if (frame != null)
					{
						frame.Show();
					}
				}
				finally
				{
					this.initiallyActivePane = null;
				}
			}
		}

		internal void AddProjectOutput(AddinExpress.Installer.WiXDesigner.OutputGroup outputGroup, ProjectDescriptor projectDesc)
		{
			if (projectDesc == null)
			{
				return;
			}
			if (projectDesc.VsProject == null)
			{
				return;
			}
			try
			{
				bool flag = false;
				object obj = this.VsProject.Object;
				if (obj != null && projectDesc.VsProject.Object != null)
				{
					string str = ProjectUtilities.NormalizeProjectOutputName(Path.GetFileNameWithoutExtension(projectDesc.VsProject.FullName));
					this.AddReferenceXML(str);
					PropertyInfo property = obj.GetType().GetProperty("BuildProject", BindingFlags.Instance | BindingFlags.NonPublic);
					if (property != null)
					{
						object value = property.GetValue(obj, null);
						if (value is Microsoft.Build.Evaluation.Project)
						{
							Microsoft.Build.Evaluation.ProjectItem projectItem = null;
							Microsoft.Build.Evaluation.Project project = (Microsoft.Build.Evaluation.Project)value;
							List<Microsoft.Build.Evaluation.ProjectItem> projectItems = new List<Microsoft.Build.Evaluation.ProjectItem>();
							MethodInfo method = obj.GetType().GetMethod("ProcessReferences", BindingFlags.Instance | BindingFlags.NonPublic);
							if (method != null)
							{
								method.Invoke(obj, null);
							}
							foreach (Microsoft.Build.Evaluation.ProjectItem item in project.Items)
							{
								if (!(item.ItemType == "ProjectReference") || item.DirectMetadataCount <= 0)
								{
									continue;
								}
								foreach (ProjectMetadata directMetadatum in item.DirectMetadata)
								{
									if (!(directMetadatum.Name == "Name") || string.IsNullOrEmpty(directMetadatum.EvaluatedValue))
									{
										continue;
									}
									if (!directMetadatum.EvaluatedValue.Equals(str, StringComparison.InvariantCultureIgnoreCase))
									{
										if (!directMetadatum.EvaluatedValue.StartsWith(string.Concat(str, "_"), StringComparison.InvariantCultureIgnoreCase) || !item.HasMetadata("ImportedFromVDProj"))
										{
											continue;
										}
										projectItems.Add(item);
									}
									else
									{
										projectItem = item;
									}
								}
							}
							if (projectItem == null)
							{
								if (method != null)
								{
									IList<Microsoft.Build.Evaluation.ProjectItem> projectItems1 = project.AddItem("ProjectReference", Path.Combine(CommonUtilities.RelativizePathIfPossible(Path.GetDirectoryName(projectDesc.VsProject.FullName), this.RootDirectory), Path.GetFileName(projectDesc.VsProject.FullName)));
									if (projectItems1 != null && projectItems1.Count > 0)
									{
										Microsoft.Build.Evaluation.ProjectItem item1 = projectItems1[0];
										item1.SetMetadataValue("Name", str);
										Guid guid = projectDesc.Guid;
										item1.SetMetadataValue("Project", guid.ToString("B").ToUpper());
										item1.SetMetadataValue("Private", "True");
										item1.SetMetadataValue("DoNotHarvest", string.Empty);
										item1.SetMetadataValue("RefProjectOutputGroups", outputGroup.ToString());
										item1.SetMetadataValue("RefTargetDir", "INSTALLDIR");
										if (projectItems.Count > 0)
										{
											foreach (Microsoft.Build.Evaluation.ProjectItem projectItem1 in projectItems)
											{
												project.RemoveItem(projectItem1);
											}
											foreach (Microsoft.Build.Evaluation.ProjectItem projectItem2 in projectItems)
											{
												projectItems1 = project.AddItem(projectItem2.ItemType, projectItem2.UnevaluatedInclude);
												if (projectItems1 == null || projectItems1.Count <= 0)
												{
													continue;
												}
												Microsoft.Build.Evaluation.ProjectItem item2 = projectItems1[0];
												foreach (ProjectMetadata projectMetadatum in projectItem2.DirectMetadata)
												{
													item2.SetMetadataValue(projectMetadatum.Name, projectMetadatum.EvaluatedValue);
												}
											}
										}
										method.Invoke(obj, null);
										if (this.References.Count > 0)
										{
											foreach (VsWiXProject.ReferenceDescriptor referenceDescriptor in this.References.Values)
											{
												if (!referenceDescriptor.Caption.Equals(str, StringComparison.InvariantCultureIgnoreCase))
												{
													continue;
												}
												referenceDescriptor.ProjectOutputGroupsText = outputGroup.ToString();
												break;
											}
										}
										flag = true;
									}
								}
							}
							else if (this.References.Count > 0 && outputGroup != AddinExpress.Installer.WiXDesigner.OutputGroup.None)
							{
								foreach (VsWiXProject.ReferenceDescriptor array in this.References.Values)
								{
									if (!array.Caption.Equals(str, StringComparison.InvariantCultureIgnoreCase))
									{
										continue;
									}
									List<AddinExpress.Installer.WiXDesigner.OutputGroup> outputGroups = new List<AddinExpress.Installer.WiXDesigner.OutputGroup>(array.ProjectOutputGroups);
									if (!outputGroups.Contains(outputGroup))
									{
										outputGroups.Add(outputGroup);
										array.ProjectOutputGroups = outputGroups.ToArray();
									}
									if (!array.Harvest)
									{
										array.Harvest = true;
									}
									flag = true;
									break;
								}
							}
							ProjectTargetElement beforeBuildTarget = this.GetBeforeBuildTarget(project);
							if (beforeBuildTarget != null && !this.IsBeforeBuildTasksAdded(beforeBuildTarget))
							{
								beforeBuildTarget.AddTask("MakeDir").SetParameter("Directories", "$(IntermediateOutputPath)Harvested XML");
								beforeBuildTarget.AddTask("MakeDir").SetParameter("Directories", "$(IntermediateOutputPath)Harvested Output");
								ProjectTaskElement projectTaskElement = beforeBuildTarget.AddTask("XslTransformation");
								projectTaskElement.SetParameter("XmlInputPaths", "XSLT\\RegisterForCOM.xml");
								projectTaskElement.SetParameter("XslInputPath", "XSLT\\XslRegisterForCOM.xslt");
								projectTaskElement.SetParameter("OutputPaths", "$(IntermediateOutputPath)_COMFiles.txt");
								projectTaskElement.SetParameter("Parameters", "<Parameter Name='operationType' Value='HeatFiles'/><Parameter Name='intermediateDir' Value='$(IntermediateOutputPath)'/>");
								projectTaskElement.Condition = "$(MSBuildToolsVersion) <= 12";
								ProjectTaskElement projectTaskElement1 = beforeBuildTarget.AddTask("XslTransformation");
								projectTaskElement1.SetParameter("XmlInputPaths", "XSLT\\RegisterForCOM.xml");
								projectTaskElement1.SetParameter("XslInputPath", "XSLT\\XslRegisterForCOM.xslt");
								projectTaskElement1.SetParameter("UseTrustedSettings", "true");
								projectTaskElement1.SetParameter("OutputPaths", "$(IntermediateOutputPath)_COMFiles.txt");
								projectTaskElement1.SetParameter("Parameters", "<Parameter Name='operationType' Value='HeatFiles'/><Parameter Name='intermediateDir' Value='$(IntermediateOutputPath)'/>");
								projectTaskElement1.Condition = "$(MSBuildToolsVersion) >= 14";
								ProjectTaskElement projectTaskElement2 = beforeBuildTarget.AddTask("ReadLinesFromFile");
								projectTaskElement2.SetParameter("File", "$(IntermediateOutputPath)_COMFiles.txt");
								projectTaskElement2.AddOutputItem("Lines", "COMFilelist");
								ProjectTaskElement projectTaskElement3 = beforeBuildTarget.AddTask("ConvertToAbsolutePath");
								projectTaskElement3.SetParameter("Paths", "@(COMFilelist)");
								projectTaskElement3.AddOutputItem("AbsolutePaths", "ResolvedCOMFilelist");
								ProjectTaskElement projectTaskElement4 = beforeBuildTarget.AddTask("Exec");
								projectTaskElement4.SetParameter("Command", "\"$(Wix)Bin\\heat.exe\" file \"%(ResolvedCOMFilelist.Identity)\" -sw -gg -sfrag -nologo -srd -out \"$(IntermediateOutputPath)Harvested XML\\_%(Filename).com.xml\"");
								projectTaskElement4.SetParameter("IgnoreExitCode", "false");
								projectTaskElement4.SetParameter("WorkingDirectory", "$(MSBuildProjectDirectory)");
								projectTaskElement4.Condition = "'%(ResolvedCOMFilelist.Identity)'!=''";
								ProjectTaskElement projectTaskElement5 = beforeBuildTarget.AddTask("XslTransformation");
								projectTaskElement5.SetParameter("XmlInputPaths", "XSLT\\RegisterForCOM.xml");
								projectTaskElement5.SetParameter("XslInputPath", "XSLT\\XslRegisterForCOM.xslt");
								projectTaskElement5.SetParameter("OutputPaths", "$(IntermediateOutputPath)_COMFiles.txt");
								projectTaskElement5.SetParameter("Parameters", "<Parameter Name='operationType' Value='TransformFiles'/><Parameter Name='intermediateDir' Value='$(IntermediateOutputPath)Harvested XML\\'/>");
								projectTaskElement5.Condition = "$(MSBuildToolsVersion) <= 12";
								ProjectTaskElement projectTaskElement6 = beforeBuildTarget.AddTask("XslTransformation");
								projectTaskElement6.SetParameter("XmlInputPaths", "XSLT\\RegisterForCOM.xml");
								projectTaskElement6.SetParameter("XslInputPath", "XSLT\\XslRegisterForCOM.xslt");
								projectTaskElement6.SetParameter("UseTrustedSettings", "true");
								projectTaskElement6.SetParameter("OutputPaths", "$(IntermediateOutputPath)_COMFiles.txt");
								projectTaskElement6.SetParameter("Parameters", "<Parameter Name='operationType' Value='TransformFiles'/><Parameter Name='intermediateDir' Value='$(IntermediateOutputPath)Harvested XML\\'/>");
								projectTaskElement6.Condition = "$(MSBuildToolsVersion) >= 14";
								ProjectTaskElement projectTaskElement7 = beforeBuildTarget.AddTask("ReadLinesFromFile");
								projectTaskElement7.SetParameter("File", "$(IntermediateOutputPath)_COMFiles.txt");
								projectTaskElement7.AddOutputItem("Lines", "XMLFileList");
								ProjectTaskElement projectTaskElement8 = beforeBuildTarget.AddTask("XslTransformation");
								projectTaskElement8.SetParameter("XmlInputPaths", "%(XMLFileList.Identity)");
								projectTaskElement8.SetParameter("XslInputPath", "XSLT\\XslFile.xslt");
								projectTaskElement8.SetParameter("OutputPaths", "$(IntermediateOutputPath)Harvested Output\\%(Filename).wsx");
								projectTaskElement8.SetParameter("Parameters", "<Parameter Name='sourceFilePath' Value='%(XMLFileList.Identity)'/>");
								projectTaskElement8.Condition = "'%(XMLFileList.Identity)'!='' And $(MSBuildToolsVersion) <= 12";
								ProjectTaskElement projectTaskElement9 = beforeBuildTarget.AddTask("XslTransformation");
								projectTaskElement9.SetParameter("XmlInputPaths", "%(XMLFileList.Identity)");
								projectTaskElement9.SetParameter("XslInputPath", "XSLT\\XslFile.xslt");
								projectTaskElement9.SetParameter("UseTrustedSettings", "true");
								projectTaskElement9.SetParameter("OutputPaths", "$(IntermediateOutputPath)Harvested Output\\%(Filename).wsx");
								projectTaskElement9.SetParameter("Parameters", "<Parameter Name='sourceFilePath' Value='%(XMLFileList.Identity)'/>");
								projectTaskElement9.Condition = "'%(XMLFileList.Identity)'!='' And $(MSBuildToolsVersion) >= 14";
								ProjectTaskElement projectTaskElement10 = beforeBuildTarget.AddTask("Exec");
								projectTaskElement10.SetParameter("Command", "\"$(Wix)Bin\\heat.exe\" project \"%(ProjectReference.FullPath)\" -projectname \"%(ProjectReference.Name)\" -pog %(ProjectReference.RefProjectOutputGroups) -gg -sfrag -nologo -out \"$(IntermediateOutputPath)Harvested XML\\_%(ProjectReference.Name).xml\"");
								projectTaskElement10.SetParameter("IgnoreExitCode", "false");
								projectTaskElement10.SetParameter("WorkingDirectory", "$(MSBuildProjectDirectory)");
								projectTaskElement10.Condition = "'%(ProjectReference.FullPath)'!='' And '%(ProjectReference.DoNotHarvest)'!='True' And '%(ProjectReference.ImportedFromVDProj)'=='True'";
								ProjectTaskElement projectTaskElement11 = beforeBuildTarget.AddTask("HeatProject");
								projectTaskElement11.SetParameter("Project", "%(ProjectReference.FullPath)");
								projectTaskElement11.SetParameter("ProjectName", "%(ProjectReference.Name)");
								projectTaskElement11.SetParameter("OutputFile", "$(IntermediateOutputPath)Harvested XML\\_%(ProjectReference.Name).xml");
								projectTaskElement11.SetParameter("ProjectOutputGroups", "%(ProjectReference.RefProjectOutputGroups)");
								projectTaskElement11.SetParameter("ToolPath", "$(Wix)Bin\\");
								projectTaskElement11.SetParameter("SuppressAllWarnings", "true");
								projectTaskElement11.SetParameter("AutogenerateGuids", "false");
								projectTaskElement11.SetParameter("GenerateGuidsNow", "true");
								projectTaskElement11.SetParameter("SuppressFragments", "true");
								projectTaskElement11.SetParameter("SuppressUniqueIds", "false");
								projectTaskElement11.Condition = "'%(ProjectReference.FullPath)'!='' And '%(ProjectReference.DoNotHarvest)'!='True' And '%(ProjectReference.ImportedFromVDProj)'!='True'";
								ProjectTaskElement projectTaskElement12 = beforeBuildTarget.AddTask("XslTransformation");
								projectTaskElement12.SetParameter("XmlInputPaths", "$(IntermediateOutputPath)Harvested XML\\_%(ProjectReference.Name).xml");
								projectTaskElement12.SetParameter("XslInputPath", "XSLT\\XslProjectOutput.xslt");
								projectTaskElement12.SetParameter("OutputPaths", "$(IntermediateOutputPath)Harvested Output\\_%(ProjectReference.Name).wxs");
								projectTaskElement12.SetParameter("Parameters", "<Parameter Name='projectName' Value='%(ProjectReference.Name)'/><Parameter Name='projectFilePath' Value='%(ProjectReference.FullPath)'/><Parameter Name='intermediateDir' Value='$(IntermediateOutputPath)Harvested Output\\'/>");
								projectTaskElement12.Condition = "'%(ProjectReference.FullPath)'!='' And '%(ProjectReference.DoNotHarvest)'!='True' And Exists('$(IntermediateOutputPath)Harvested XML\\_%(ProjectReference.Name).xml') And $(MSBuildToolsVersion) <= 12";
								ProjectTaskElement projectTaskElement13 = beforeBuildTarget.AddTask("XslTransformation");
								projectTaskElement13.SetParameter("XmlInputPaths", "$(IntermediateOutputPath)Harvested XML\\_%(ProjectReference.Name).xml");
								projectTaskElement13.SetParameter("XslInputPath", "XSLT\\XslProjectOutput.xslt");
								projectTaskElement13.SetParameter("UseTrustedSettings", "true");
								projectTaskElement13.SetParameter("OutputPaths", "$(IntermediateOutputPath)Harvested Output\\_%(ProjectReference.Name).wxs");
								projectTaskElement13.SetParameter("Parameters", "<Parameter Name='projectName' Value='%(ProjectReference.Name)'/><Parameter Name='projectFilePath' Value='%(ProjectReference.FullPath)'/><Parameter Name='intermediateDir' Value='$(IntermediateOutputPath)Harvested Output\\'/>");
								projectTaskElement13.Condition = "'%(ProjectReference.FullPath)'!='' And '%(ProjectReference.DoNotHarvest)'!='True' And Exists('$(IntermediateOutputPath)Harvested XML\\_%(ProjectReference.Name).xml') And $(MSBuildToolsVersion) >= 14";
								ProjectTaskElement projectTaskElement14 = beforeBuildTarget.AddTask("XslTransformation");
								projectTaskElement14.SetParameter("XmlInputPaths", "XSLT\\RegisterForCOM.xml");
								projectTaskElement14.SetParameter("XslInputPath", "XSLT\\XslRegisterForCOM.xslt");
								projectTaskElement14.SetParameter("OutputPaths", "$(IntermediateOutputPath)_COMFiles.txt");
								projectTaskElement14.SetParameter("Parameters", "<Parameter Name='operationType' Value='CompileFiles'/><Parameter Name='intermediateDir' Value='$(IntermediateOutputPath)Harvested Output\\'/>");
								projectTaskElement14.Condition = "$(MSBuildToolsVersion) <= 12";
								ProjectTaskElement projectTaskElement15 = beforeBuildTarget.AddTask("XslTransformation");
								projectTaskElement15.SetParameter("XmlInputPaths", "XSLT\\RegisterForCOM.xml");
								projectTaskElement15.SetParameter("XslInputPath", "XSLT\\XslRegisterForCOM.xslt");
								projectTaskElement15.SetParameter("UseTrustedSettings", "true");
								projectTaskElement15.SetParameter("OutputPaths", "$(IntermediateOutputPath)_COMFiles.txt");
								projectTaskElement15.SetParameter("Parameters", "<Parameter Name='operationType' Value='CompileFiles'/><Parameter Name='intermediateDir' Value='$(IntermediateOutputPath)Harvested Output\\'/>");
								projectTaskElement15.Condition = "$(MSBuildToolsVersion) >= 14";
								ProjectTaskElement projectTaskElement16 = beforeBuildTarget.AddTask("ReadLinesFromFile");
								projectTaskElement16.SetParameter("File", "$(IntermediateOutputPath)_COMFiles.txt");
								projectTaskElement16.AddOutputItem("Lines", "WSXFileList");
								ProjectTaskElement projectTaskElement17 = beforeBuildTarget.AddTask("CreateItem");
								projectTaskElement17.SetParameter("Include", "$(IntermediateOutputPath)Harvested Output\\_%(ProjectReference.Name).wxs");
								projectTaskElement17.Condition = "'%(ProjectReference.FullPath)'!='' And '%(ProjectReference.DoNotHarvest)'!='True' And Exists('$(IntermediateOutputPath)Harvested Output\\_%(ProjectReference.Name).wxs')";
								projectTaskElement17.AddOutputItem("Include", "Compile");
								ProjectTaskElement projectTaskElement18 = beforeBuildTarget.AddTask("CreateItem");
								projectTaskElement18.SetParameter("Include", "@(WSXFileList)");
								projectTaskElement18.Condition = "Exists('%(WSXFileList.Identity)')";
								projectTaskElement18.AddOutputItem("Include", "Compile");
								flag = true;
							}
						}
						if (flag)
						{
							this.VsProject.Save("");
						}
					}
				}
			}
			catch (Exception exception)
			{
				DTEHelperObject.ShowErrorDialog(this, exception);
			}
		}

		private EnvDTE.ProjectItem AddReferenceXML(string referenceName)
		{
			bool flag = false;
			string str = Path.Combine(Path.GetDirectoryName(this.VsProject.FullName), "XSLT");
			EnvDTE.ProjectItem xSLTItem = this.GetXSLTItem(this.VsProject);
			if (xSLTItem == null)
			{
				flag = true;
				if (!Directory.Exists(str))
				{
					Directory.CreateDirectory(str);
				}
				xSLTItem = this.VsProject.ProjectItems.AddFolder("XSLT", "{6BB5F8EF-4483-11D3-8BCF-00C04F8EC28C}");
			}
			if (xSLTItem != null)
			{
				string str1 = Path.Combine(str, "XslProjectOutput.xslt");
				string str2 = Path.Combine(str, "XslFile.xslt");
				string str3 = Path.Combine(str, "XslRegisterForCOM.xslt");
				string str4 = Path.Combine(str, "RegisterForCOM.xml");
				string str5 = Path.Combine(str, "readme.txt");
				if (flag || !File.Exists(str1))
				{
					this.CreateFileFromTemplate(str1, Resources.XslProjectOutput.Replace("$wix_namespace$", this.WixNamespaceUri));
				}
				if (flag || !File.Exists(str2))
				{
					this.CreateFileFromTemplate(str2, Resources.XslFile.Replace("$wix_namespace$", this.WixNamespaceUri));
				}
				if (flag || !File.Exists(str3))
				{
					this.CreateFileFromTemplate(str3, Resources.XslRegisterForCOM);
				}
				if (flag || !File.Exists(str4))
				{
					this.CreateFileFromTemplate(str4, Resources.RegisterForCOM);
				}
				if (flag || !File.Exists(str5))
				{
					this.CreateFileFromTemplate(str5, Resources.readme);
				}
				if (File.Exists(str1) && this.GetXMLFileItem("XslProjectOutput.xslt", xSLTItem.ProjectItems) == null)
				{
					xSLTItem.ProjectItems.AddFromFile(str1);
				}
				if (File.Exists(str2) && this.GetXMLFileItem("XslFile.xslt", xSLTItem.ProjectItems) == null)
				{
					xSLTItem.ProjectItems.AddFromFile(str2);
				}
				if (File.Exists(str3) && this.GetXMLFileItem("XslRegisterForCOM.xslt", xSLTItem.ProjectItems) == null)
				{
					xSLTItem.ProjectItems.AddFromFile(str3);
				}
				if (File.Exists(str4) && this.GetXMLFileItem("RegisterForCOM.xml", xSLTItem.ProjectItems) == null)
				{
					xSLTItem.ProjectItems.AddFromFile(str4);
				}
				if (File.Exists(str5) && this.GetXMLFileItem("readme.txt", xSLTItem.ProjectItems) == null)
				{
					xSLTItem.ProjectItems.AddFromFile(str5);
				}
				string str6 = referenceName;
				string str7 = string.Concat("_", referenceName, ".xml");
				string str8 = Path.Combine(Path.GetDirectoryName(this.VsProject.FullName), "XSLT");
				if (this.GetXMLFileItem(str7, xSLTItem.ProjectItems) == null)
				{
					string str9 = Path.Combine(str8, str7);
					if (this.CreateProjectXmlFile(str9, referenceName, str6))
					{
						xSLTItem.ProjectItems.AddFromFile(str9);
					}
				}
			}
			return xSLTItem;
		}

		internal void AddWiXExtensionReference(string extensionName, bool isWix_dir)
		{
			try
			{
				object obj = this.VsProject.Object;
				PropertyInfo property = obj.GetType().GetProperty("BuildProject", BindingFlags.Instance | BindingFlags.NonPublic);
				if (property != null)
				{
					object value = property.GetValue(obj, null);
					if (value is Microsoft.Build.Evaluation.Project)
					{
						string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(extensionName);
						Microsoft.Build.Evaluation.ProjectItem projectItem = null;
						Microsoft.Build.Evaluation.Project project = (Microsoft.Build.Evaluation.Project)value;
						MethodInfo method = obj.GetType().GetMethod("ProcessReferences", BindingFlags.Instance | BindingFlags.NonPublic);
						if (method != null)
						{
							method.Invoke(obj, null);
							foreach (Microsoft.Build.Evaluation.ProjectItem item in project.Items)
							{
								if (!(item.ItemType == "WixExtension") || item.DirectMetadataCount <= 0)
								{
									continue;
								}
								foreach (ProjectMetadata directMetadatum in item.DirectMetadata)
								{
									if (!(directMetadatum.Name == "Name") || string.IsNullOrEmpty(directMetadatum.EvaluatedValue) || !directMetadatum.EvaluatedValue.Equals(fileNameWithoutExtension, StringComparison.InvariantCultureIgnoreCase))
									{
										continue;
									}
									projectItem = item;
								}
							}
							if (projectItem == null)
							{
								if (!isWix_dir)
								{
									string empty = string.Empty;
									if (!VsPackage.CurrentInstance.IsVSIXPackage)
									{
										empty = VsPackage.CurrentInstance.GetLocation();
										empty = (!WixSettings.Instance.IsUsingWix3() ? Path.GetFullPath(Path.Combine(empty, "..\\WiX Extensions\\v4.x")) : Path.GetFullPath(Path.Combine(empty, "..\\WiX Extensions\\v3.x")));
									}
									else
									{
										empty = VsPackage.CurrentInstance.GetLocation();
										empty = (!WixSettings.Instance.IsUsingWix3() ? Path.Combine(empty, "WiX Extensions\\v4.x") : Path.Combine(empty, "WiX Extensions\\v3.x"));
									}
									empty = Path.Combine(empty, string.Concat(fileNameWithoutExtension, ".dll"));
									if (File.Exists(empty))
									{
										IList<Microsoft.Build.Evaluation.ProjectItem> projectItems = project.AddItem("WixExtension", empty);
										if (projectItems != null && projectItems.Count > 0)
										{
											Microsoft.Build.Evaluation.ProjectItem item1 = projectItems[0];
											item1.UnevaluatedInclude = fileNameWithoutExtension;
											item1.SetMetadataValue("HintPath", empty);
											item1.SetMetadataValue("Name", fileNameWithoutExtension);
											method.Invoke(obj, null);
											project.MarkDirty();
											project.ReevaluateIfNecessary();
											if (!this.MakeProjectDirty())
											{
												this.VsProject.Save("");
											}
										}
									}
								}
								else
								{
									string binDirectory = WixSettings.Instance.WixBinariesDirectory.BinDirectory;
									if (Directory.Exists(binDirectory))
									{
										string str = Path.Combine(binDirectory, string.Concat(fileNameWithoutExtension, ".dll"));
										if (File.Exists(str))
										{
											IList<Microsoft.Build.Evaluation.ProjectItem> projectItems1 = project.AddItem("WixExtension", str);
											if (projectItems1 != null && projectItems1.Count > 0)
											{
												Microsoft.Build.Evaluation.ProjectItem projectItem1 = projectItems1[0];
												projectItem1.UnevaluatedInclude = fileNameWithoutExtension;
												projectItem1.SetMetadataValue("HintPath", string.Concat("$(WixExtDir)\\", fileNameWithoutExtension, ".dll"));
												projectItem1.SetMetadataValue("Name", fileNameWithoutExtension);
												method.Invoke(obj, null);
												project.MarkDirty();
												project.ReevaluateIfNecessary();
												if (!this.MakeProjectDirty())
												{
													this.VsProject.Save("");
												}
											}
										}
									}
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
		}

		public bool CanCloseProject()
		{
			return true;
		}

		private bool CanEditFile(string filePath)
		{
			uint num;
			uint num1;
			bool flag;
			if (this.gettingCheckoutStatus)
			{
				return false;
			}
			try
			{
				this.gettingCheckoutStatus = true;
				IVsQueryEditQuerySave2 service = (IVsQueryEditQuerySave2)this.GetService(typeof(SVsQueryEditQuerySave));
				if (service != null)
				{
					if (ErrorHandler.Succeeded(service.QueryEditFiles(0, 1, new string[] { filePath }, null, null, out num, out num1)) && num == 0)
					{
						flag = true;
						return flag;
					}
				}
				return false;
			}
			finally
			{
				this.gettingCheckoutStatus = false;
			}
			return flag;
		}

		internal void CheckParseDone()
		{
			if (!this.parseDoneCheckShown && this.allWiXFiles.Count > 0)
			{
				foreach (KeyValuePair<uint, VsWiXProject.WiXFileDescriptor> allWiXFile in this.allWiXFiles)
				{
					if (!allWiXFile.Value.IsInternalBufferChanged)
					{
						continue;
					}
					this.parseDoneCheckShown = true;
					MessageBox.Show(VsPackage.CurrentInstance, string.Format("The XML model of the '{0}' is outdated. Please try to open or activate any visual editor of the '{0}' to synchronize XML code.", "Designer for WiX Setup Projects"), "Designer for Visual Studio WiX Setup Projects", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return;
				}
			}
		}

		private void ClearReferences()
		{
			while (this.referenceList.Count > 0)
			{
				VsWiXProject.ReferenceDescriptor item = this.referenceList.Values[0];
				this.referenceList.Remove(item.ID);
				item.Dispose();
			}
		}

		private void ClearSolutionProjectList()
		{
			while (this.solutionProjectList.Count > 0)
			{
				ProjectDescriptor item = this.solutionProjectList[0];
				this.solutionProjectList.Remove(item);
				item.Dispose();
			}
		}

		internal void CloseWiXFiles(int designerId)
		{
			if (this.allWiXFiles.Count > 0)
			{
				foreach (KeyValuePair<uint, VsWiXProject.WiXFileDescriptor> allWiXFile in this.allWiXFiles)
				{
					allWiXFile.Value.Close(designerId);
				}
			}
		}

		public void CommitXmlChanges()
		{
			if (this.WiXModel == null)
			{
				return;
			}
			Dictionary<string, WiXProjectItem> strs = new Dictionary<string, WiXProjectItem>();
			foreach (WiXProjectItem projectItem in this.WiXModel.ProjectItems)
			{
				if (!projectItem.IsDirty)
				{
					continue;
				}
				strs.Add(projectItem.SourceFile, projectItem);
			}
			if (strs.Count > 0)
			{
				object xmlLanguageService = this.GetXmlLanguageService();
				try
				{
					try
					{
						if (xmlLanguageService != null)
						{
							xmlLanguageService.GetType().InvokeMember("IsParsing", BindingFlags.SetProperty, null, xmlLanguageService, new object[] { true });
						}
						foreach (KeyValuePair<string, WiXProjectItem> str in strs)
						{
							if (!this.CanEditFile(str.Key))
							{
								continue;
							}
							this.ModifyWiXFile(0, str.Key, str.Value.SourceDocument);
							str.Value.IsDirty = false;
						}
					}
					catch (Exception exception)
					{
						DTEHelperObject.ShowErrorDialog(this, exception);
					}
				}
				finally
				{
					if (xmlLanguageService != null)
					{
						xmlLanguageService.GetType().InvokeMember("IsParsing", BindingFlags.SetProperty, null, xmlLanguageService, new object[] { false });
					}
					strs.Clear();
				}
			}
		}

		public bool Connect()
		{
			bool flag = false;
			try
			{
				if (this.projectListener != null)
				{
					this.projectListener.StartListening(false);
					if (this.projectListener.IsListening)
					{
						this.PopulateReferences();
						flag = true;
					}
				}
				if (flag)
				{
					object obj = this.VsProject.Object;
					if (obj != null)
					{
						PropertyInfo property = obj.GetType().GetProperty("BuildProject", BindingFlags.Instance | BindingFlags.NonPublic);
						if (property != null)
						{
							object value = property.GetValue(obj, null);
							if (value is Microsoft.Build.Evaluation.Project)
							{
								ProjectProperty projectProperty = ((Microsoft.Build.Evaluation.Project)value).GetProperty("MakeProjectLocalizable");
								if (projectProperty != null)
								{
									this.multiLangSupport = (projectProperty.EvaluatedValue == "True" ? true : false);
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
			return flag;
		}

		private void CreateFileFromTemplate(string filePath, string templateString)
		{
			using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
			{
				using (StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
				{
					streamWriter.Write(templateString);
					streamWriter.Flush();
				}
			}
		}

		private void CreateFileFromTemplate(string filePath, byte[] templateData)
		{
			using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
			{
				fileStream.Write(templateData, 0, (int)templateData.Length);
				fileStream.Flush();
			}
		}

		private string CreateNewGuid()
		{
			return Guid.NewGuid().ToString("D").ToUpperInvariant();
		}

		private void CreateProjectGroupSection(string groupName, XmlTextWriter txtWriter)
		{
			txtWriter.WriteStartElement(groupName);
			txtWriter.WriteStartElement("DiskId");
			txtWriter.WriteString("1");
			txtWriter.WriteEndElement();
			txtWriter.WriteStartElement("Compressed");
			txtWriter.WriteString(string.Empty);
			txtWriter.WriteEndElement();
			txtWriter.WriteStartElement("Hidden");
			txtWriter.WriteString("no");
			txtWriter.WriteEndElement();
			txtWriter.WriteStartElement("KeyPath");
			txtWriter.WriteString("yes");
			txtWriter.WriteEndElement();
			txtWriter.WriteStartElement("ReadOnly");
			txtWriter.WriteString("no");
			txtWriter.WriteEndElement();
			txtWriter.WriteStartElement("System");
			txtWriter.WriteString("no");
			txtWriter.WriteEndElement();
			txtWriter.WriteStartElement("Vital");
			txtWriter.WriteString("yes");
			txtWriter.WriteEndElement();
			txtWriter.WriteStartElement("Condition");
			txtWriter.WriteCData(string.Empty);
			txtWriter.WriteEndElement();
			txtWriter.WriteStartElement("SelfRegCost");
			txtWriter.WriteString(string.Empty);
			txtWriter.WriteEndElement();
			txtWriter.WriteStartElement("TrueType");
			txtWriter.WriteString("no");
			txtWriter.WriteEndElement();
			txtWriter.WriteStartElement("Permanent");
			txtWriter.WriteString("no");
			txtWriter.WriteEndElement();
			txtWriter.WriteStartElement("SharedDllRefCount");
			txtWriter.WriteString("no");
			txtWriter.WriteEndElement();
			txtWriter.WriteStartElement("Transitive");
			txtWriter.WriteString("no");
			txtWriter.WriteEndElement();
			txtWriter.WriteEndElement();
		}

		private bool CreateProjectXmlFile(string filePath, string referenceName, string variableName)
		{
			Path.GetFileName(filePath);
			using (XmlTextWriter xmlTextWriter = new XmlTextWriter(filePath, Encoding.UTF8))
			{
				xmlTextWriter.Formatting = Formatting.Indented;
				xmlTextWriter.WriteStartDocument();
				xmlTextWriter.WriteStartElement("ProjectOutput");
				xmlTextWriter.WriteAttributeString("Name", referenceName);
				xmlTextWriter.WriteAttributeString("ProjectDirVar", string.Concat("$(var.", variableName, ".ProjectDir)"));
				xmlTextWriter.WriteAttributeString("TargetDirVar", string.Concat("$(var.", variableName, ".TargetDir)"));
				xmlTextWriter.WriteAttributeString("SharedFileId", string.Concat("_", this.CreateNewGuid().Replace("-", "_")));
				this.CreateProjectGroupSection("Binaries", xmlTextWriter);
				this.CreateProjectGroupSection("Symbols", xmlTextWriter);
				this.CreateProjectGroupSection("Sources", xmlTextWriter);
				this.CreateProjectGroupSection("Content", xmlTextWriter);
				this.CreateProjectGroupSection("Satellites", xmlTextWriter);
				this.CreateProjectGroupSection("Documents", xmlTextWriter);
				xmlTextWriter.WriteEndElement();
				xmlTextWriter.Flush();
			}
			return File.Exists(filePath);
		}

		private VsWiXProject.ReferenceDescriptor CreateReferenceFromId(uint id)
		{
			object obj = null;
			VsWiXProject.ReferenceDescriptor referenceDescriptor = null;
			try
			{
				MethodInfo method = this.vsProject.Object.GetType().GetMethod("NodeFromItemId", BindingFlags.Instance | BindingFlags.Public);
				if (method != null)
				{
					obj = method.Invoke(this.vsProject.Object, new object[] { id });
				}
				if (obj != null)
				{
					uint value = 0;
					string empty = string.Empty;
					IVsHierarchy hierarchy = null;
					PropertyInfo property = obj.GetType().GetProperty("Harvest", BindingFlags.Instance | BindingFlags.Public);
					if (property != null)
					{
						property = obj.GetType().GetProperty("Caption", BindingFlags.Instance | BindingFlags.Public);
						if (property != null)
						{
							empty = (string)property.GetValue(obj, null);
						}
						property = obj.GetType().GetProperty("ID", BindingFlags.Instance | BindingFlags.Public);
						if (property != null)
						{
							value = (uint)property.GetValue(obj, null);
						}
						property = obj.GetType().GetProperty("Url", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
						if (property != null)
						{
							string str = (string)property.GetValue(obj, null);
							if (!string.IsNullOrEmpty(str))
							{
								hierarchy = ProjectUtilities.GetHierarchy(str);
							}
							if (hierarchy == null)
							{
								property = obj.GetType().GetProperty("ReferencedProjectObject", BindingFlags.Instance | BindingFlags.NonPublic);
								if (property != null)
								{
									try
									{
										hierarchy = ProjectUtilities.GetHierarchyOfProject((EnvDTE.Project)property.GetValue(obj, null));
									}
									catch (Exception exception)
									{
									}
								}
							}
						}
						if (!string.IsNullOrEmpty(empty) && value > 0)
						{
							referenceDescriptor = new VsWiXProject.ReferenceDescriptor(this, (IVsProject)hierarchy, empty, value);
						}
					}
				}
			}
			catch (Exception exception1)
			{
				DTEHelperObject.ShowErrorDialog(this, exception1);
			}
			return referenceDescriptor;
		}

		private void delayTimer_Tick(object sender, EventArgs e)
		{
			this.delayTimer.Stop();
			try
			{
				try
				{
					switch (this.delayActionEventData.ActionReason)
					{
						case VsWiXProject.RefRefreshReason.ReferenceRenamed:
						{
							if (this.delayActionEventData.Reference == null || string.IsNullOrEmpty(this.delayActionEventData.OldReferenceName) || string.IsNullOrEmpty(this.delayActionEventData.NewReferenceName))
							{
								goto case VsWiXProject.RefRefreshReason.XmlFileRemoved;
							}
							string oldReferenceName = this.delayActionEventData.OldReferenceName;
							IVsHierarchy referenceNode = this.delayActionEventData.Reference.GetReferenceNode() as IVsHierarchy;
							if (referenceNode == null)
							{
								goto case VsWiXProject.RefRefreshReason.XmlFileRemoved;
							}
							PropertyInfo property = referenceNode.GetType().GetProperty("ReferencedProjectName", BindingFlags.Instance | BindingFlags.NonPublic);
							if (property == null)
							{
								goto case VsWiXProject.RefRefreshReason.XmlFileRemoved;
							}
							this.lockChanges = true;
							try
							{
								property.SetValue(referenceNode, oldReferenceName, null);
								goto case VsWiXProject.RefRefreshReason.XmlFileRemoved;
							}
							finally
							{
								this.lockChanges = false;
								this.RefreshReferences(oldReferenceName, VsWiXProject.RefRefreshReason.ReferenceRenamed);
							}
							break;
						}
						case VsWiXProject.RefRefreshReason.XmlFileAdded:
						case VsWiXProject.RefRefreshReason.XmlFileRemoved:
						{
							break;
						}
						case VsWiXProject.RefRefreshReason.ReferencedProjectRenamed:
						{
							if (this.delayActionEventData.Reference == null)
							{
								goto case VsWiXProject.RefRefreshReason.XmlFileRemoved;
							}
							string str = ProjectUtilities.NormalizeProjectOutputName(this.delayActionEventData.Reference.Caption);
							if (!str.Equals(this.delayActionEventData.Reference.Caption))
							{
								IVsHierarchy vsHierarchy = this.delayActionEventData.Reference.GetReferenceNode() as IVsHierarchy;
								if (vsHierarchy != null)
								{
									PropertyInfo propertyInfo = vsHierarchy.GetType().GetProperty("ReferencedProjectName", BindingFlags.Instance | BindingFlags.NonPublic);
									if (propertyInfo != null)
									{
										this.lockChanges = true;
										try
										{
											propertyInfo.SetValue(vsHierarchy, str, null);
										}
										finally
										{
											this.lockChanges = false;
										}
									}
								}
							}
							if (string.IsNullOrEmpty(this.delayActionEventData.XMLFileName))
							{
								goto case VsWiXProject.RefRefreshReason.XmlFileRemoved;
							}
							bool flag = true;
							EnvDTE.ProjectItem xSLTItem = null;
							string str1 = string.Concat("_", str, ".xml");
							if (!this.delayActionEventData.XMLFileName.Equals(str1))
							{
								xSLTItem = this.GetXSLTItem(this.VsProject);
								if (xSLTItem != null)
								{
									EnvDTE.ProjectItem xMLFileItem = this.GetXMLFileItem(this.delayActionEventData.XMLFileName, xSLTItem.ProjectItems);
									if (xMLFileItem != null)
									{
										xMLFileItem.Name = str1;
										flag = false;
									}
								}
							}
							if (!flag)
							{
								goto case VsWiXProject.RefRefreshReason.XmlFileRemoved;
							}
							this.LoadReferenceProperties(xSLTItem, this.delayActionEventData.Reference, VsWiXProject.RefRefreshReason.ReferenceRenamed);
							goto case VsWiXProject.RefRefreshReason.XmlFileRemoved;
						}
						case VsWiXProject.RefRefreshReason.MultiSelectFilesRemoved:
						{
							this.multiSelectMode = false;
							this.ParseXML(false);
							goto case VsWiXProject.RefRefreshReason.XmlFileRemoved;
						}
						default:
						{
							goto case VsWiXProject.RefRefreshReason.XmlFileRemoved;
						}
					}
				}
				catch (Exception exception)
				{
				}
			}
			finally
			{
				this.delayActionEventData.ActionReason = VsWiXProject.RefRefreshReason.Unknown;
				this.delayActionEventData.Reference = null;
				this.delayActionEventData.XMLFileName = null;
				this.delayActionEventData.OldReferenceName = null;
				this.delayActionEventData.NewReferenceName = null;
			}
		}

		private void Diconnect()
		{
			if (this.projectListener != null && this.projectListener.IsListening)
			{
				this.projectListener.StopListening();
			}
		}

		public void Dispose()
		{
			if (!this.disposed)
			{
				this.disposed = true;
				if (this.delayTimer != null)
				{
					this.delayTimer.Stop();
					this.delayTimer.Dispose();
					this.delayTimer = null;
				}
				this.Diconnect();
				if (this.projectListener != null)
				{
					this.projectListener.Dispose();
				}
				this.ClearReferences();
				if (this.panes.Count > 0)
				{
					foreach (VsPaneBase value in this.panes.Values)
					{
						value.OnProjectUnload();
						if (!(value.get_Frame() is IVsWindowFrame))
						{
							continue;
						}
						((IVsWindowFrame)value.get_Frame()).Hide();
					}
					this.panes.Clear();
				}
				if (this.allWiXFiles.Count > 0)
				{
					foreach (KeyValuePair<uint, VsWiXProject.WiXFileDescriptor> allWiXFile in this.allWiXFiles)
					{
						allWiXFile.Value.Dispose();
					}
					this.allWiXFiles.Clear();
				}
				this.projectParser = null;
				if (this.projectProperties != null)
				{
					this.projectProperties.Dispose();
					this.projectProperties = null;
				}
				this.ClearSolutionProjectList();
			}
			GC.SuppressFinalize(this);
		}

		~VsWiXProject()
		{
			this.Dispose();
		}

		private int GetActivePaneID()
		{
			int d;
			if (this.VsDTE != null)
			{
				Window activeWindow = this.VsDTE.ActiveWindow;
				if (activeWindow != null)
				{
					VsPaneBase obj = activeWindow.Object as VsPaneBase;
					if (obj != null)
					{
						return obj.ID;
					}
					if (this.panes.Count > 0)
					{
						using (IEnumerator<VsPaneBase> enumerator = this.panes.Values.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								VsPaneBase current = enumerator.Current;
								Window vsWindow = current.VsWindow;
								if (vsWindow == null || !vsWindow.Equals(activeWindow))
								{
									continue;
								}
								d = current.ID;
								return d;
							}
							return 0;
						}
						return d;
					}
				}
			}
			return 0;
		}

		private ProjectTargetElement GetAfterBuildTarget(Microsoft.Build.Evaluation.Project vsBuildProject)
		{
			ProjectTargetElement projectTargetElement = null;
			if (vsBuildProject != null)
			{
				foreach (ProjectTargetElement target in vsBuildProject.Xml.Targets)
				{
					if (string.IsNullOrEmpty(target.Name) || !target.Name.Equals("AfterBuild", StringComparison.InvariantCultureIgnoreCase) || !string.IsNullOrEmpty(target.Condition))
					{
						continue;
					}
					projectTargetElement = target;
					goto Label0;
				}
			Label0:
				if (projectTargetElement == null)
				{
					projectTargetElement = vsBuildProject.Xml.AddTarget("AfterBuild");
				}
			}
			return projectTargetElement;
		}

		private ProjectTargetElement GetBeforeBuildTarget(Microsoft.Build.Evaluation.Project vsBuildProject)
		{
			ProjectTargetElement projectTargetElement = null;
			if (vsBuildProject != null)
			{
				foreach (ProjectTargetElement target in vsBuildProject.Xml.Targets)
				{
					if (string.IsNullOrEmpty(target.Name) || !target.Name.Equals("BeforeBuild", StringComparison.InvariantCultureIgnoreCase))
					{
						continue;
					}
					projectTargetElement = target;
					goto Label0;
				}
			Label0:
				if (projectTargetElement == null)
				{
					projectTargetElement = vsBuildProject.Xml.AddTarget("BeforeBuild");
				}
			}
			return projectTargetElement;
		}

		internal string GetBuildProjectProperty(string propertyName)
		{
			object obj = this.VsProject.Object;
			PropertyInfo property = obj.GetType().GetProperty("BuildProject", BindingFlags.Instance | BindingFlags.NonPublic);
			if (property != null)
			{
				object value = property.GetValue(obj, null);
				if (value is Microsoft.Build.Evaluation.Project)
				{
					return ((Microsoft.Build.Evaluation.Project)value).GetPropertyValue(propertyName);
				}
			}
			return string.Empty;
		}

		public static string GetEditorNameByButtonID(uint buttonID)
		{
			switch (buttonID)
			{
				case 257:
				{
					return "File System";
				}
				case 258:
				{
					return "Registry";
				}
				case 259:
				{
					return "File Types";
				}
				case 260:
				{
					return "User Interface";
				}
				case 261:
				{
					return "Custom Actions";
				}
				case 262:
				{
					return "Launch Conditions";
				}
			}
			return string.Empty;
		}

		private ProjectPropertyGroupElement GetProjectPropertyGroup(Microsoft.Build.Evaluation.Project vsBuildProject)
		{
			ProjectPropertyGroupElement projectPropertyGroupElement = null;
			if (vsBuildProject != null)
			{
				foreach (ProjectPropertyGroupElement propertyGroup in vsBuildProject.Xml.PropertyGroups)
				{
					if (string.IsNullOrEmpty(propertyGroup.Label) || !propertyGroup.Label.Equals("WiXDesignerProperties", StringComparison.InvariantCultureIgnoreCase))
					{
						continue;
					}
					projectPropertyGroupElement = propertyGroup;
					goto Label0;
				}
			Label0:
				if (projectPropertyGroupElement == null)
				{
					projectPropertyGroupElement = vsBuildProject.Xml.AddPropertyGroup();
					projectPropertyGroupElement.Label = "WiXDesignerProperties";
				}
			}
			return projectPropertyGroupElement;
		}

		public string GetReferencedProjectVariable(string variableName)
		{
			string empty = string.Empty;
			string str = string.Empty;
			string empty1 = string.Empty;
			string str1 = string.Empty;
			int num = variableName.IndexOf('(');
			if (num != -1)
			{
				variableName = variableName.Substring(num + 1);
			}
			int num1 = variableName.IndexOf(')');
			if (num1 != -1)
			{
				variableName = variableName.Substring(0, num1);
			}
			int num2 = variableName.LastIndexOf('.');
			if (num2 != -1 && num2 < variableName.Length)
			{
				empty1 = variableName.Substring(num2 + 1);
				int num3 = variableName.IndexOf('.');
				if (num3 != -1 && num3 != num2)
				{
					str = variableName.Substring(num3 + 1, num2 - num3 - 1);
					num2 = str.LastIndexOf('.');
					if (num2 != -1)
					{
						string str2 = str.Substring(num2 + 1);
						if (!string.IsNullOrEmpty(str2))
						{
							try
							{
								if (ProjectUtilities.GetCultureInfo(str2) != null)
								{
									str1 = str2;
								}
							}
							catch (Exception exception)
							{
							}
							str = str.Substring(0, num2);
						}
					}
				}
			}
			if (!string.IsNullOrEmpty(empty1))
			{
				if (string.IsNullOrEmpty(str))
				{
					object obj = this.VsProject.Object;
					if (obj != null)
					{
						PropertyInfo property = obj.GetType().GetProperty("BuildProject", BindingFlags.Instance | BindingFlags.NonPublic);
						if (property != null)
						{
							object value = property.GetValue(obj, null);
							if (value is Microsoft.Build.Evaluation.Project)
							{
								return ((Microsoft.Build.Evaluation.Project)value).GetPropertyValue(empty1);
							}
						}
					}
				}
				else
				{
					foreach (VsWiXProject.ReferenceDescriptor referenceDescriptor in this.References.Values)
					{
						if (!referenceDescriptor.Caption.Equals(str))
						{
							continue;
						}
						empty = referenceDescriptor.GetProperty(empty1, str1);
						return empty;
					}
				}
			}
			return empty;
		}

		private object GetRefProperty(object wixItem, string propName)
		{
			object obj;
			if (wixItem != null)
			{
				try
				{
					obj = wixItem.GetType().InvokeMember(propName, BindingFlags.GetProperty, null, wixItem, null);
				}
				catch (Exception exception)
				{
					return null;
				}
				return obj;
			}
			return null;
		}

		internal string GetRegisterForCOMData()
		{
			EnvDTE.ProjectItem xSLTItem = this.GetXSLTItem(this.VsProject);
			if (xSLTItem != null)
			{
				EnvDTE.ProjectItem projectItem = xSLTItem.ProjectItems.Item("RegisterForCOM.xml");
				if (projectItem != null && projectItem.Document != null)
				{
					TextDocument textDocument = projectItem.Document.Object("TextDocument") as TextDocument;
					return textDocument.CreateEditPoint(textDocument.StartPoint).GetText(textDocument.EndPoint);
				}
			}
			return string.Empty;
		}

		public object GetService(Type serviceType)
		{
			if (this._serviceProvider == null)
			{
				return null;
			}
			return this._serviceProvider.GetService(serviceType);
		}

		public object GetService(Guid serviceGuid)
		{
			if (this._serviceProvider == null)
			{
				return null;
			}
			return this._serviceProvider.GetService(serviceGuid);
		}

		public List<ProjectDescriptor> GetSolutionProjects(bool refreshList)
		{
			if (refreshList)
			{
				if (this.solutionProjectList.Count > 0)
				{
					this.ClearSolutionProjectList();
				}
				foreach (IVsProject loadedProject in ProjectUtilities.LoadedProjects)
				{
					this.solutionProjectList.Add(new ProjectDescriptor(loadedProject, this));
				}
			}
			return this.solutionProjectList;
		}

		internal int GetUnsavedFilesCount(int designerId)
		{
			int num = 0;
			if (this.allWiXFiles.Count > 0)
			{
				foreach (KeyValuePair<uint, VsWiXProject.WiXFileDescriptor> allWiXFile in this.allWiXFiles)
				{
					if (!allWiXFile.Value.Opened || !allWiXFile.Value.IsDirty || !allWiXFile.Value.RegisteredViews.ContainsKey(designerId))
					{
						continue;
					}
					num++;
				}
			}
			return num;
		}

		public VsViewManager GetViewManagerByDesignerID(int designerId)
		{
			VsViewManager viewManager;
			if (this.panes.Count > 0)
			{
				using (IEnumerator<VsPaneBase> enumerator = this.Panes.Values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						VsPaneBase current = enumerator.Current;
						if (current.ID != designerId)
						{
							continue;
						}
						viewManager = current.ViewManager;
						return viewManager;
					}
					return null;
				}
				return viewManager;
			}
			return null;
		}

		public Color GetVsColor(VsSysColors vsColor)
		{
			uint num;
			IVsUIShell2 service = this._serviceProvider.GetService(typeof(SVsUIShell)) as IVsUIShell2;
			if (service == null || service.GetVSSysColorEx((int)vsColor, out num) != 0)
			{
				return Color.Empty;
			}
			return ColorTranslator.FromWin32((int)num);
		}

		private VsWiXProject.WiXFileDescriptor GetWiXFileByDocCookie(uint cookie)
		{
			VsWiXProject.WiXFileDescriptor value = null;
			foreach (KeyValuePair<uint, VsWiXProject.WiXFileDescriptor> allWiXFile in this.AllWiXFiles)
			{
				if (allWiXFile.Value.DocCookie != cookie)
				{
					continue;
				}
				value = allWiXFile.Value;
				return value;
			}
			return value;
		}

		private EnvDTE.ProjectItem GetXMLFileItem(string fileName, ProjectItems items)
		{
			for (int i = 1; i <= items.Count; i++)
			{
				EnvDTE.ProjectItem projectItem = items.Item(i);
				if (projectItem != null && projectItem.Name.Equals(fileName, StringComparison.InvariantCultureIgnoreCase))
				{
					return projectItem;
				}
			}
			return null;
		}

		private object GetXmlLanguageService()
		{
			IntPtr intPtr;
			if (this._xmlLanguageService == null)
			{
				Microsoft.VisualStudio.OLE.Interop.IServiceProvider service = this.GetService(typeof(Microsoft.VisualStudio.OLE.Interop.IServiceProvider)) as Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
				Guid guid = new Guid("f6819a78-a205-47b5-be1c-675b3c7f0b8e");
				Guid guid1 = new Guid("00000000-0000-0000-C000-000000000046");
				if (ErrorHandler.Succeeded(service.QueryService(ref guid, ref guid1, out intPtr)))
				{
					try
					{
						this._xmlLanguageService = Marshal.GetObjectForIUnknown(intPtr);
					}
					finally
					{
						Marshal.Release(intPtr);
					}
				}
			}
			return this._xmlLanguageService;
		}

		private EnvDTE.ProjectItem GetXSLTItem(EnvDTE.Project project)
		{
			for (int i = 1; i <= project.ProjectItems.Count; i++)
			{
				EnvDTE.ProjectItem projectItem = project.ProjectItems.Item(i);
				if (projectItem != null && !string.IsNullOrEmpty(projectItem.Kind) && !string.IsNullOrEmpty(projectItem.Name) && projectItem.Kind.Equals("{6BB5F8EF-4483-11D3-8BCF-00C04F8EC28C}", StringComparison.InvariantCultureIgnoreCase) && projectItem.Name.Equals("XSLT", StringComparison.InvariantCultureIgnoreCase))
				{
					return projectItem;
				}
			}
			return null;
		}

		private bool IsBeforeBuildTasksAdded(ProjectTargetInstance beforeBuildTarget)
		{
			bool flag;
			if (beforeBuildTarget != null)
			{
				bool flag1 = false;
				bool flag2 = false;
				using (IEnumerator<ProjectTaskInstance> enumerator = beforeBuildTarget.Tasks.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ProjectTaskInstance current = enumerator.Current;
						if (!string.IsNullOrEmpty(current.Name) && current.Name.Equals("MakeDir", StringComparison.InvariantCultureIgnoreCase) && current.Parameters.ContainsKey("Directories"))
						{
							if (current.Parameters["Directories"] == "$(IntermediateOutputPath)Harvested XML")
							{
								flag2 = true;
							}
							else if (current.Parameters["Directories"] == "$(IntermediateOutputPath)Harvested Output")
							{
								flag1 = true;
							}
						}
						if (!(flag1 & flag2))
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

		private bool IsBeforeBuildTasksAdded(ProjectTargetElement beforeBuildTarget)
		{
			bool flag;
			if (beforeBuildTarget != null)
			{
				bool flag1 = false;
				bool flag2 = false;
				using (IEnumerator<ProjectTaskElement> enumerator = beforeBuildTarget.Tasks.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ProjectTaskElement current = enumerator.Current;
						if (!string.IsNullOrEmpty(current.Name) && current.Name.Equals("MakeDir", StringComparison.InvariantCultureIgnoreCase) && current.Parameters.ContainsKey("Directories"))
						{
							if (current.Parameters["Directories"] == "$(IntermediateOutputPath)Harvested XML")
							{
								flag2 = true;
							}
							else if (current.Parameters["Directories"] == "$(IntermediateOutputPath)Harvested Output")
							{
								flag1 = true;
							}
						}
						if (!(flag1 & flag2))
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

		private bool IsIntermediateFilePath(string filePath)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				return false;
			}
			return Path.GetDirectoryName(filePath).StartsWith(Path.Combine(this.RootDirectory, "obj"), StringComparison.InvariantCultureIgnoreCase);
		}

		internal bool IsWiXFileDirty(string filePath)
		{
			bool flag;
			if (this.allWiXFiles.Count > 0)
			{
				Dictionary<uint, VsWiXProject.WiXFileDescriptor>.Enumerator enumerator = this.allWiXFiles.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<uint, VsWiXProject.WiXFileDescriptor> current = enumerator.Current;
						if (!current.Value.FilePath.Equals(filePath, StringComparison.InvariantCultureIgnoreCase))
						{
							continue;
						}
						flag = (!current.Value.Opened ? false : current.Value.IsDirty);
						return flag;
					}
					return false;
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				return flag;
			}
			return false;
		}

		private void LoadPaneSettings(string paneName, Hashtable paneData)
		{
			if (paneData != null)
			{
				try
				{
					VsPaneBase vsPaneBase = this.ShowWindow(paneName, false, false);
					if (vsPaneBase != null && paneData.Count > 0)
					{
						try
						{
							if ((bool)paneData["Active"])
							{
								this.initiallyActivePane = vsPaneBase;
							}
						}
						catch (Exception exception)
						{
						}
						if (vsPaneBase.ViewManager != null)
						{
							vsPaneBase.ViewManager.OnLoadUserSettings(paneData);
						}
					}
				}
				catch (Exception exception1)
				{
					DTEHelperObject.ShowErrorDialog(this, exception1);
				}
			}
		}

		private bool LoadReferenceProperties(EnvDTE.ProjectItem xsltItem, VsWiXProject.ReferenceDescriptor refDesc, VsWiXProject.RefRefreshReason reason)
		{
			bool flag = false;
			if (reason != VsWiXProject.RefRefreshReason.XmlFileRemoved)
			{
				EnvDTE.ProjectItem xMLFileItem = null;
				if (xsltItem == null)
				{
					xsltItem = this.GetXSLTItem(this.VsProject);
				}
				if (xsltItem != null)
				{
					string str = string.Concat("_", refDesc.Caption, ".xml");
					xMLFileItem = this.GetXMLFileItem(str, xsltItem.ProjectItems);
					if (xMLFileItem != null)
					{
						if (!refDesc.IsXSLTOutputsSupported)
						{
							refDesc.IsXSLTOutputsSupported = true;
							flag = true;
						}
						if (flag)
						{
							refDesc.ProjectOutputXMLFilePath = string.Empty;
							if (refDesc.ProjectOutputXMLDocument.DocumentElement != null)
							{
								refDesc.ProjectOutputXMLDocument.RemoveAll();
							}
							try
							{
								refDesc.ProjectOutputXMLDocument.Load(xMLFileItem[1]);
								refDesc.ProjectOutputXMLFilePath = xMLFileItem[1];
							}
							catch (Exception exception)
							{
								refDesc.IsXSLTOutputsSupported = false;
							}
						}
					}
				}
				if (xMLFileItem == null && refDesc.IsXSLTOutputsSupported)
				{
					refDesc.IsXSLTOutputsSupported = false;
					flag = true;
				}
			}
			else if (refDesc.IsXSLTOutputsSupported)
			{
				refDesc.IsXSLTOutputsSupported = false;
				flag = true;
			}
			return flag;
		}

		public void LoadSolutionSettings(IPropertyBag pPropBag)
		{
		}

		public void LoadUserSettings(Hashtable userData)
		{
			if (userData.ContainsKey(this.ProjectGuidString))
			{
				Hashtable item = userData[this.ProjectGuidString] as Hashtable;
				if (item != null && item.Count > 0)
				{
					if (item.ContainsKey("File System"))
					{
						this.LoadPaneSettings("File System", item["File System"] as Hashtable);
					}
					if (item.ContainsKey("Registry"))
					{
						this.LoadPaneSettings("Registry", item["Registry"] as Hashtable);
					}
					if (item.ContainsKey("File Types"))
					{
						this.LoadPaneSettings("File Types", item["File Types"] as Hashtable);
					}
					if (item.ContainsKey("User Interface"))
					{
						this.LoadPaneSettings("User Interface", item["User Interface"] as Hashtable);
					}
					if (item.ContainsKey("Custom Actions"))
					{
						this.LoadPaneSettings("Custom Actions", item["Custom Actions"] as Hashtable);
					}
					if (item.ContainsKey("Launch Conditions"))
					{
						this.LoadPaneSettings("Launch Conditions", item["Launch Conditions"] as Hashtable);
					}
				}
			}
		}

		internal bool MakeProjectDirty()
		{
			if (this.VsProject != null && this.VsProject.Object != null)
			{
				object obj = this.VsProject.Object;
				MethodInfo method = obj.GetType().GetMethod("SetProjectFileDirty", BindingFlags.Instance | BindingFlags.Public);
				if (method != null)
				{
					method.Invoke(obj, new object[] { true });
					return true;
				}
			}
			return false;
		}

		internal bool MakeProjectLocalizable()
		{
			this.multiLangSupport = false;
			try
			{
				object obj = this.VsProject.Object;
				if (obj != null)
				{
					PropertyInfo property = obj.GetType().GetProperty("BuildProject", BindingFlags.Instance | BindingFlags.NonPublic);
					if (property != null)
					{
						object value = property.GetValue(obj, null);
						if (value is Microsoft.Build.Evaluation.Project)
						{
							Microsoft.Build.Evaluation.Project project = (Microsoft.Build.Evaluation.Project)value;
							ProjectProperty projectProperty = project.GetProperty("MakeProjectLocalizable");
							if (projectProperty != null)
							{
								if (projectProperty.EvaluatedValue != "True")
								{
									projectProperty.UnevaluatedValue = "True";
									if (!this.MakeProjectDirty())
									{
										this.VsProject.Save("");
									}
								}
								this.multiLangSupport = true;
							}
							if (!this.multiLangSupport)
							{
								ProjectPropertyGroupElement projectPropertyGroup = this.GetProjectPropertyGroup(project);
								if (projectPropertyGroup != null)
								{
									projectPropertyGroup.SetProperty("MakeProjectLocalizable", "True");
									project.MarkDirty();
									project.ReevaluateIfNecessary();
									if (!this.MakeProjectDirty())
									{
										this.VsProject.Save("");
									}
									this.multiLangSupport = true;
								}
							}
						}
					}
					if (this.multiLangSupport && this.ProjectProperties.WiXPropertiesObject != null)
					{
						System.ComponentModel.TypeDescriptor.Refresh(this.ProjectProperties.WiXPropertiesObject);
					}
				}
			}
			catch (Exception exception)
			{
				DTEHelperObject.ShowErrorDialog(this, exception);
			}
			return this.multiLangSupport;
		}

		internal void ModifyWiXFile(int designerId, string filePath, string textLines)
		{
			if (this.allWiXFiles.Count > 0)
			{
				foreach (KeyValuePair<uint, VsWiXProject.WiXFileDescriptor> allWiXFile in this.allWiXFiles)
				{
					if (!allWiXFile.Value.FilePath.Equals(filePath, StringComparison.InvariantCultureIgnoreCase))
					{
						continue;
					}
					allWiXFile.Value.Modify(designerId, textLines);
				}
			}
		}

		internal void ModifyWiXFile(int designerId, string filePath, XmlDocument document)
		{
			if (this.allWiXFiles.Count > 0)
			{
				foreach (KeyValuePair<uint, VsWiXProject.WiXFileDescriptor> allWiXFile in this.allWiXFiles)
				{
					if (!allWiXFile.Value.FilePath.Equals(filePath, StringComparison.InvariantCultureIgnoreCase))
					{
						continue;
					}
					allWiXFile.Value.Modify(designerId, document);
				}
			}
		}

		private void NotifyDesigners(VsWiXProject.ChangeContextType contextType, params object[] contextObject)
		{
			foreach (VsPaneBase value in this.panes.Values)
			{
				value.DoSomethingChanged(contextType, contextObject);
			}
		}

		public void OnAddReference(VsWiXProject.ReferenceDescriptor desc)
		{
			this.NotifyDesigners(VsWiXProject.ChangeContextType.ReferenceAdded, new object[] { desc });
			this.WiXModel.OnReferenceAdded(desc);
		}

		public void OnAfterSave(uint docCookie)
		{
			if (docCookie > 0 && this.allWiXFiles.Count > 0)
			{
				foreach (KeyValuePair<uint, VsWiXProject.WiXFileDescriptor> allWiXFile in this.allWiXFiles)
				{
					if (!allWiXFile.Value.Opened || allWiXFile.Value.DocCookie != docCookie)
					{
						continue;
					}
					allWiXFile.Value.NotifyBufferSaved();
					return;
				}
			}
		}

		public void OnBeforeBuildStarted()
		{
		}

		public void OnBuildStarted()
		{
			this.buildStarted = true;
			this.UpdateCOMReferences();
			this.NotifyDesigners(VsWiXProject.ChangeContextType.BuildStarted, null);
		}

		public void OnBuildStopped()
		{
			this.buildStarted = false;
			this.NotifyDesigners(VsWiXProject.ChangeContextType.BuildStopped, null);
		}

		public void OnFileAdded(string filePath, uint id)
		{
			if (this.IsIntermediateFilePath(filePath))
			{
				return;
			}
			try
			{
				if (!CommonUtilities.IsFileExtensionSupported(filePath))
				{
					string extension = Path.GetExtension(filePath);
					string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
					if (!string.IsNullOrEmpty(extension) && extension.Equals(".xml", StringComparison.InvariantCultureIgnoreCase) && fileNameWithoutExtension.StartsWith("_"))
					{
						string directoryName = Path.GetDirectoryName(filePath);
						if (Path.Combine(this.RootDirectory, "XSLT").Equals(directoryName, StringComparison.InvariantCultureIgnoreCase))
						{
							this.RefreshReferences(fileNameWithoutExtension.Substring(1), VsWiXProject.RefRefreshReason.XmlFileAdded);
						}
					}
				}
				else
				{
					if (!this.allWiXFiles.ContainsKey(id))
					{
						VsWiXProject.WiXFileDescriptor wiXFileDescriptor = new VsWiXProject.WiXFileDescriptor(filePath, id, this, true)
						{
							FileStatus = VsWiXProject.WiXFileDescriptor.Status.New
						};
						this.allWiXFiles.Add(id, wiXFileDescriptor);
						this.NotifyDesigners(VsWiXProject.ChangeContextType.FileAdded, new object[] { filePath });
						this.WiXModel.OnFileAdded(filePath);
					}
					string str = Path.GetExtension(filePath);
					string fileNameWithoutExtension1 = Path.GetFileNameWithoutExtension(filePath);
					if (!string.IsNullOrEmpty(str) && str.Equals(".xml", StringComparison.InvariantCultureIgnoreCase) && fileNameWithoutExtension1.StartsWith("_"))
					{
						string directoryName1 = Path.GetDirectoryName(filePath);
						if (Path.Combine(this.RootDirectory, "XSLT").Equals(directoryName1, StringComparison.InvariantCultureIgnoreCase))
						{
							this.RefreshReferences(fileNameWithoutExtension1.Substring(1), VsWiXProject.RefRefreshReason.XmlFileAdded);
						}
					}
				}
			}
			catch (Exception exception)
			{
				DTEHelperObject.ShowErrorDialog(this, exception);
			}
		}

		public void OnFileRemoved(string filePath, uint id)
		{
			if (this.IsIntermediateFilePath(filePath))
			{
				return;
			}
			try
			{
				if (this.allWiXFiles.Count > 0)
				{
					if (id <= 0)
					{
						if (!string.IsNullOrEmpty(filePath) && CommonUtilities.IsFileExtensionSupported(filePath))
						{
							uint key = 0;
							foreach (KeyValuePair<uint, VsWiXProject.WiXFileDescriptor> allWiXFile in this.allWiXFiles)
							{
								if (!allWiXFile.Value.FilePath.Equals(filePath, StringComparison.InvariantCultureIgnoreCase))
								{
									continue;
								}
								key = allWiXFile.Key;
								break;
							}
							if (key <= 0)
							{
								filePath.Equals(this.lastRemovedFilePath, StringComparison.InvariantCultureIgnoreCase);
							}
							else
							{
								VsWiXProject.WiXFileDescriptor item = this.allWiXFiles[key];
								item.Close(0);
								item.FileStatus = VsWiXProject.WiXFileDescriptor.Status.Removed;
								if (!item.IsInSaveMode)
								{
									this.allWiXFiles.Remove(key);
									item.Dispose();
									this.NotifyDesigners(VsWiXProject.ChangeContextType.FileRemoved, new object[] { filePath });
									this.WiXModel.OnFileRemoved(filePath);
								}
								this.lastRemovedFilePath = filePath;
								return;
							}
						}
					}
					else if (this.allWiXFiles.ContainsKey(id))
					{
						VsWiXProject.WiXFileDescriptor wiXFileDescriptor = this.allWiXFiles[id];
						wiXFileDescriptor.Close(0);
						wiXFileDescriptor.FileStatus = VsWiXProject.WiXFileDescriptor.Status.Removed;
						if (!wiXFileDescriptor.IsInSaveMode)
						{
							this.allWiXFiles.Remove(id);
							string str = filePath;
							if (!string.IsNullOrEmpty(filePath))
							{
								this.NotifyDesigners(VsWiXProject.ChangeContextType.FileRemoved, new object[] { filePath });
								this.WiXModel.OnFileRemoved(filePath);
							}
							else
							{
								str = wiXFileDescriptor.FilePath;
								this.NotifyDesigners(VsWiXProject.ChangeContextType.FileRemoved, new object[] { wiXFileDescriptor.FilePath });
								this.WiXModel.OnFileRemoved(wiXFileDescriptor.FilePath);
							}
							wiXFileDescriptor.Dispose();
							if (this.IsMultiLangSupported && this.WiXModel != null && this.WiXModel.ProjectType == WiXProjectType.Product && Path.GetExtension(str).Equals(".wxl"))
							{
								this.checkSupportedLanguages = true;
								if (this.multiSelectMode)
								{
									this.delayTimer.Stop();
									this.delayActionEventData.ActionReason = VsWiXProject.RefRefreshReason.MultiSelectFilesRemoved;
									this.delayTimer.Start();
								}
							}
						}
					}
					else if (string.IsNullOrEmpty(filePath))
					{
						this.RefreshReferences(null, VsWiXProject.RefRefreshReason.Unknown);
					}
					else
					{
						string extension = Path.GetExtension(filePath);
						string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
						if (!string.IsNullOrEmpty(extension) && extension.Equals(".xml", StringComparison.InvariantCultureIgnoreCase) && fileNameWithoutExtension.StartsWith("_"))
						{
							string directoryName = Path.GetDirectoryName(filePath);
							if (Path.Combine(this.RootDirectory, "XSLT").Equals(directoryName, StringComparison.InvariantCultureIgnoreCase))
							{
								this.RefreshReferences(fileNameWithoutExtension.Substring(1), VsWiXProject.RefRefreshReason.XmlFileRemoved);
							}
						}
					}
				}
				this.lastRemovedFilePath = string.Empty;
			}
			catch (Exception exception)
			{
				DTEHelperObject.ShowErrorDialog(this, exception);
			}
		}

		public void OnFileRenamed(string oldFilePath, string newFilePath, uint id)
		{
			if (this.IsIntermediateFilePath(newFilePath))
			{
				return;
			}
			try
			{
				if (this.allWiXFiles.Count > 0)
				{
					bool flag = false;
					if (id <= 0 && (CommonUtilities.IsFileExtensionSupported(oldFilePath) || CommonUtilities.IsFileExtensionSupported(newFilePath)))
					{
						uint key = 0;
						foreach (KeyValuePair<uint, VsWiXProject.WiXFileDescriptor> allWiXFile in this.allWiXFiles)
						{
							if (!allWiXFile.Value.FilePath.Equals(oldFilePath, StringComparison.InvariantCultureIgnoreCase))
							{
								continue;
							}
							key = allWiXFile.Key;
							break;
						}
						if (key > 0)
						{
							flag = true;
							VsWiXProject.WiXFileDescriptor item = this.allWiXFiles[key];
							item.Close(0);
							item.FileStatus = VsWiXProject.WiXFileDescriptor.Status.Removed;
							this.allWiXFiles.Remove(key);
							item.Dispose();
						}
					}
					if (CommonUtilities.IsFileExtensionSupported(newFilePath) && id > 0)
					{
						flag = true;
						VsWiXProject.WiXFileDescriptor wiXFileDescriptor = new VsWiXProject.WiXFileDescriptor(newFilePath, id, this, true)
						{
							FileStatus = VsWiXProject.WiXFileDescriptor.Status.New
						};
						this.allWiXFiles.Add(id, wiXFileDescriptor);
					}
					if (flag)
					{
						this.NotifyDesigners(VsWiXProject.ChangeContextType.FileRenamed, new object[] { oldFilePath, newFilePath });
					}
				}
			}
			catch (Exception exception)
			{
				DTEHelperObject.ShowErrorDialog(this, exception);
			}
		}

		public void OnProjectClosing()
		{
			this.NotifyDesigners(VsWiXProject.ChangeContextType.ProjectClosing, null);
		}

		public void OnProjectOpened(bool added)
		{
			Dictionary<uint, string> nums = ProjectUtilities.AllItemsInProject(this.hierarchy as IVsProject, true);
			if (nums.Count > 0)
			{
				foreach (KeyValuePair<uint, string> keyValuePair in nums)
				{
					VsWiXProject.WiXFileDescriptor wiXFileDescriptor = new VsWiXProject.WiXFileDescriptor(keyValuePair.Value, keyValuePair.Key, this, true)
					{
						FileStatus = VsWiXProject.WiXFileDescriptor.Status.New
					};
					this.allWiXFiles.Add(keyValuePair.Key, wiXFileDescriptor);
				}
			}
			if (added)
			{
				this.solutionLoadedCompletely = true;
				this.RefreshReferencedProjects();
				this.NotifyDesigners(VsWiXProject.ChangeContextType.ProjectAdded, null);
			}
		}

		public void OnProjectParentChanged()
		{
			this.NotifyDesigners(VsWiXProject.ChangeContextType.ProjectParentChanged, null);
		}

		public void OnProjectPropertiesSelected()
		{
			if (this.WiXModel != null)
			{
				this.WiXModel.ViewManager = null;
			}
			this.NotifyDesigners(VsWiXProject.ChangeContextType.ProjectPropertiesSelected, null);
		}

		public void OnProjectRenamed()
		{
			this.NotifyDesigners(VsWiXProject.ChangeContextType.ProjectRenamed, null);
		}

		public void OnRemoveReference(VsWiXProject.ReferenceDescriptor desc)
		{
			this.NotifyDesigners(VsWiXProject.ChangeContextType.ReferenceRemoved, new object[] { desc });
			this.WiXModel.OnReferenceRemoved(desc);
		}

		public void OnRenameReference(VsWiXProject.ReferenceDescriptor oldRefDesc, VsWiXProject.ReferenceDescriptor newRefDesc)
		{
			this.NotifyDesigners(VsWiXProject.ChangeContextType.ReferenceRenamed, new object[] { oldRefDesc, newRefDesc });
			this.WiXModel.OnReferenceRenamed(oldRefDesc, newRefDesc);
		}

		public void OnSolutionClosing()
		{
			this.solutionLoadedCompletely = false;
		}

		public void OnSolutionOpened()
		{
			this.solutionLoadedCompletely = true;
			this.RefreshReferencedProjects();
			this.NotifyDesigners(VsWiXProject.ChangeContextType.SolutionLoaded, null);
			this.ActivateWiXDesignerIfAny();
		}

		public void OnSolutionProjectRenamed(IVsHierarchy project)
		{
			this.lastSolutionRenamedProject = project;
		}

		public void OnThemeChanged()
		{
			if (this.panes.Count > 0)
			{
				foreach (VsPaneBase value in this.panes.Values)
				{
					value.OnThemeChanged();
				}
			}
			this.WiXModel.OnThemeChanged();
		}

		internal void OpenWiXFile(string filePath, int designerId, out XmlDocument document)
		{
			document = null;
			if (this.allWiXFiles.Count > 0)
			{
				foreach (KeyValuePair<uint, VsWiXProject.WiXFileDescriptor> allWiXFile in this.allWiXFiles)
				{
					if (!allWiXFile.Value.FilePath.Equals(filePath, StringComparison.OrdinalIgnoreCase))
					{
						continue;
					}
					allWiXFile.Value.Open(designerId);
					if (!allWiXFile.Value.Opened)
					{
						break;
					}
					document = allWiXFile.Value.Document;
					return;
				}
			}
		}

		internal void OpenWiXFiles(int designerId, out Dictionary<string, XmlDocument> documents)
		{
			documents = null;
			if (this.allWiXFiles.Count > 0)
			{
				documents = new Dictionary<string, XmlDocument>();
				foreach (KeyValuePair<uint, VsWiXProject.WiXFileDescriptor> allWiXFile in this.allWiXFiles)
				{
					allWiXFile.Value.Open(designerId);
					if (!allWiXFile.Value.Opened)
					{
						continue;
					}
					documents.Add(allWiXFile.Value.FilePath, allWiXFile.Value.Document);
				}
			}
		}

		internal void OpenWiXFiles(int designerId, out Dictionary<string, string> textLines)
		{
			textLines = null;
			if (this.allWiXFiles.Count > 0)
			{
				textLines = new Dictionary<string, string>();
				foreach (KeyValuePair<uint, VsWiXProject.WiXFileDescriptor> allWiXFile in this.allWiXFiles)
				{
					allWiXFile.Value.Open(designerId);
					if (!allWiXFile.Value.Opened)
					{
						continue;
					}
					textLines.Add(allWiXFile.Value.FilePath, allWiXFile.Value.DocumentText);
				}
			}
		}

		internal void ParseXML(bool forceXmlDirty)
		{
			this.parseDoneCheckShown = false;
			if (this.checkSupportedLanguages && this.multiSelectMode)
			{
				return;
			}
			if (this.WiXModel != null)
			{
				if (forceXmlDirty)
				{
					this.WiXModel.IsDirty = true;
				}
				if (this.WiXModel.IsDirty)
				{
					this.ProjectProperties.ResetReadFlags();
					if (this.allWiXFiles.Count > 0)
					{
						this.WiXModel.Clear();
						foreach (KeyValuePair<uint, VsWiXProject.WiXFileDescriptor> allWiXFile in this.allWiXFiles)
						{
							allWiXFile.Value.Open(0);
							if (!allWiXFile.Value.Opened)
							{
								continue;
							}
							this.WiXModel.AddWiXSourceFile(allWiXFile.Value.FilePath, allWiXFile.Value.Document);
						}
						if (this.WiXModel.ProjectItems.Count > 0)
						{
							this.WiXModel.Parse();
						}
					}
				}
			}
		}

		private void PopulateReferences()
		{
			this.ClearReferences();
			object obj = null;
			try
			{
				MethodInfo method = this.vsProject.Object.GetType().GetMethod("GetReferenceContainer", BindingFlags.Instance | BindingFlags.Public);
				if (method != null)
				{
					obj = method.Invoke(this.vsProject.Object, null);
				}
				if (obj != null)
				{
					IEnumerable enumerable = null;
					method = obj.GetType().GetMethod("EnumReferences", BindingFlags.Instance | BindingFlags.Public);
					if (method != null)
					{
						enumerable = method.Invoke(obj, null) as IEnumerable;
					}
					if (enumerable != null)
					{
						uint value = 0;
						string empty = string.Empty;
						foreach (object obj1 in enumerable)
						{
							PropertyInfo property = obj1.GetType().GetProperty("Harvest", BindingFlags.Instance | BindingFlags.Public);
							if (property == null)
							{
								continue;
							}
							property = obj1.GetType().GetProperty("Caption", BindingFlags.Instance | BindingFlags.Public);
							if (property != null)
							{
								empty = (string)property.GetValue(obj1, null);
							}
							property = obj1.GetType().GetProperty("ID", BindingFlags.Instance | BindingFlags.Public);
							if (property != null)
							{
								value = (uint)property.GetValue(obj1, null);
							}
							if (string.IsNullOrEmpty(empty) || value <= 0)
							{
								continue;
							}
							VsWiXProject.ReferenceDescriptor referenceDescriptor = new VsWiXProject.ReferenceDescriptor(this, null, empty, value);
							this.referenceList.Add(value, referenceDescriptor);
							this.LoadReferenceProperties(null, referenceDescriptor, VsWiXProject.RefRefreshReason.ReferenceAdded);
						}
					}
				}
			}
			catch (Exception exception)
			{
				DTEHelperObject.ShowErrorDialog(this, exception);
			}
		}

		private void projectListener_OnAddProjectItem(object sender, HierarchyEventArgs e)
		{
			try
			{
				VsWiXProject.ReferenceDescriptor referenceDescriptor = this.CreateReferenceFromId(e.ItemID);
				if (referenceDescriptor == null)
				{
					this.OnFileAdded(ProjectUtilities.GetProjectItemFilePath(this.hierarchy as IVsProject, e.ItemID), e.ItemID);
				}
				else
				{
					this.referenceList.Add(e.ItemID, referenceDescriptor);
					if (this.delayActionEventData.ActionReason != VsWiXProject.RefRefreshReason.ReferencedProjectRenamed)
					{
						this.LoadReferenceProperties(null, referenceDescriptor, VsWiXProject.RefRefreshReason.ReferenceAdded);
					}
					else
					{
						this.delayActionEventData.Reference = referenceDescriptor;
						this.delayTimer.Start();
					}
					this.OnAddReference(referenceDescriptor);
				}
			}
			finally
			{
				this.lastSolutionRenamedProject = null;
			}
		}

		private void projectListener_OnChangeProjectItem(object sender, HierarchyEventArgs e)
		{
			try
			{
				if (e.PropertyID == -2003 && this.referenceList.ContainsKey(e.ItemID))
				{
					VsWiXProject.ReferenceDescriptor item = this.referenceList[e.ItemID];
					VsWiXProject.ReferenceDescriptor referenceDescriptor = this.CreateReferenceFromId(e.ItemID);
					if (referenceDescriptor != null && item != null)
					{
						try
						{
							this.OnRenameReference(item, referenceDescriptor);
							return;
						}
						finally
						{
							this.referenceList.Remove(e.ItemID);
							this.referenceList.Add(e.ItemID, referenceDescriptor);
							if (!this.lockChanges && item.IsXSLTOutputsSupported)
							{
								this.delayActionEventData.ActionReason = VsWiXProject.RefRefreshReason.ReferenceRenamed;
								this.delayActionEventData.OldReferenceName = item.Caption;
								this.delayActionEventData.NewReferenceName = referenceDescriptor.Caption;
								this.delayActionEventData.Reference = referenceDescriptor;
								this.delayTimer.Start();
							}
							item.Dispose();
						}
					}
				}
			}
			finally
			{
				this.lastSolutionRenamedProject = null;
			}
		}

		private void projectListener_OnDeleteProjectItem(object sender, HierarchyEventArgs e)
		{
			try
			{
				if (!this.referenceList.ContainsKey(e.ItemID))
				{
					this.OnFileRemoved(ProjectUtilities.GetProjectItemFilePath(this.hierarchy as IVsProject, e.ItemID), e.ItemID);
				}
				else
				{
					VsWiXProject.ReferenceDescriptor item = this.referenceList[e.ItemID];
					if (item != null)
					{
						try
						{
							this.OnRemoveReference(item);
							bool flag = true;
							string str = string.Concat("_", item.Caption, ".xml");
							if (item.IsXSLTOutputsSupported && item.ReferencedProject != null && this.lastSolutionRenamedProject != null && item.ReferencedProject.Guid.Equals(ProjectUtilities.GetProjectGuid(this.lastSolutionRenamedProject)))
							{
								flag = false;
								this.delayActionEventData.ActionReason = VsWiXProject.RefRefreshReason.ReferencedProjectRenamed;
								this.delayActionEventData.XMLFileName = str;
							}
							if (flag)
							{
								EnvDTE.ProjectItem xSLTItem = this.GetXSLTItem(this.VsProject);
								if (xSLTItem != null)
								{
									EnvDTE.ProjectItem xMLFileItem = this.GetXMLFileItem(str, xSLTItem.ProjectItems);
									if (xMLFileItem != null)
									{
										xMLFileItem.Remove();
									}
								}
							}
							return;
						}
						finally
						{
							this.referenceList.Remove(e.ItemID);
							item.Dispose();
						}
					}
				}
			}
			finally
			{
				this.lastSolutionRenamedProject = null;
			}
		}

		private void projectParser_AfterParse()
		{
			this.UpdateSupportedLanguages();
		}

		private void RefreshReferencedProjects()
		{
			if (this.References.Count > 0)
			{
				foreach (VsWiXProject.ReferenceDescriptor value in this.References.Values)
				{
					object referenceNode = value.GetReferenceNode();
					if (referenceNode == null || value.ReferencedProject.InternalProject != null)
					{
						continue;
					}
					try
					{
						EnvDTE.Project project = null;
						PropertyInfo property = referenceNode.GetType().GetProperty("ReferencedProjectObject", BindingFlags.Instance | BindingFlags.NonPublic);
						if (property != null)
						{
							project = property.GetValue(referenceNode, null) as EnvDTE.Project;
							if (project != null)
							{
								value.ReferencedProject.InternalProject = ProjectUtilities.GetHierarchyOfProject(project) as IVsProject;
								if (value.ReferencedProject.InternalProject != null)
								{
									this.NotifyDesigners(VsWiXProject.ChangeContextType.ReferenceRefreshed, new object[] { value });
								}
							}
						}
					}
					catch (Exception exception)
					{
					}
				}
			}
		}

		private void RefreshReferences(string refName, VsWiXProject.RefRefreshReason reason)
		{
			EnvDTE.ProjectItem xSLTItem = this.GetXSLTItem(this.VsProject);
			if (xSLTItem == null)
			{
				foreach (VsWiXProject.ReferenceDescriptor value in this.referenceList.Values)
				{
					if (!value.IsXSLTOutputsSupported)
					{
						continue;
					}
					value.IsXSLTOutputsSupported = false;
					this.NotifyDesigners(VsWiXProject.ChangeContextType.ReferenceRefreshed, new object[] { value });
					this.WiXModel.OnReferenceRefreshed(value);
				}
			}
			else if (!string.IsNullOrEmpty(refName))
			{
				foreach (VsWiXProject.ReferenceDescriptor referenceDescriptor in this.referenceList.Values)
				{
					if (!referenceDescriptor.Caption.Equals(refName, StringComparison.InvariantCultureIgnoreCase))
					{
						continue;
					}
					if (!this.LoadReferenceProperties(xSLTItem, referenceDescriptor, reason))
					{
						break;
					}
					this.NotifyDesigners(VsWiXProject.ChangeContextType.ReferenceRefreshed, new object[] { referenceDescriptor });
					this.WiXModel.OnReferenceRefreshed(referenceDescriptor);
					return;
				}
			}
			else
			{
				foreach (VsWiXProject.ReferenceDescriptor value1 in this.referenceList.Values)
				{
					if (!this.LoadReferenceProperties(xSLTItem, value1, reason))
					{
						continue;
					}
					this.NotifyDesigners(VsWiXProject.ChangeContextType.ReferenceRefreshed, new object[] { value1 });
					this.WiXModel.OnReferenceRefreshed(value1);
				}
			}
		}

		internal void RegisterDynamicProperties(object wixPropertiesObject)
		{
			Type type = typeof(VsWiXProject.ProjectPropertiesObject);
			List<AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor> propertyDescriptors = new List<AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor>();
			if (this.WiXModel.ProjectType == WiXProjectType.Product)
			{
				propertyDescriptors.Add(new ProxyPropertyDescriptor(type, "AddRemoveProgramsIconDummy", typeof(string), null, this.ProjectProperties, null));
				propertyDescriptors.Add(new ProxyPropertyDescriptor(type, "Author", typeof(string), string.Empty, this.ProjectProperties, null));
				propertyDescriptors.Add(new ProxyPropertyDescriptor(type, "Description", typeof(string), string.Empty, this.ProjectProperties, null));
				propertyDescriptors.Add(new ProxyPropertyDescriptor(type, "DetectNewerInstalledVersion", typeof(bool), string.Empty, this.ProjectProperties, null));
				propertyDescriptors.Add(new ProxyPropertyDescriptor(type, "InstallAllUsers", typeof(bool), string.Empty, this.ProjectProperties, null));
				propertyDescriptors.Add(new ProxyPropertyDescriptor(type, "Keywords", typeof(string), string.Empty, this.ProjectProperties, null));
				propertyDescriptors.Add(new ProxyPropertyDescriptor(type, "Localization", typeof(string), string.Empty, this.ProjectProperties, null));
				propertyDescriptors.Add(new ProxyPropertyDescriptor(type, "Manufacturer", typeof(string), string.Empty, this.ProjectProperties, null));
				propertyDescriptors.Add(new ProxyPropertyDescriptor(type, "ManufacturerUrl", typeof(string), string.Empty, this.ProjectProperties, null));
				propertyDescriptors.Add(new ProxyPropertyDescriptor(type, "ProductCode", typeof(string), string.Empty, this.ProjectProperties, null));
				propertyDescriptors.Add(new ProxyPropertyDescriptor(type, "ProductName", typeof(string), string.Empty, this.ProjectProperties, null));
				propertyDescriptors.Add(new ProxyPropertyDescriptor(type, "RemovePreviousVersions", typeof(bool), string.Empty, this.ProjectProperties, null));
				propertyDescriptors.Add(new ProxyPropertyDescriptor(type, "Subject", typeof(string), string.Empty, this.ProjectProperties, null));
				propertyDescriptors.Add(new ProxyPropertyDescriptor(type, "SupportPhone", typeof(string), string.Empty, this.ProjectProperties, null));
				propertyDescriptors.Add(new ProxyPropertyDescriptor(type, "SupportUrl", typeof(string), string.Empty, this.ProjectProperties, null));
				propertyDescriptors.Add(new ProxyPropertyDescriptor(type, "Title", typeof(string), string.Empty, this.ProjectProperties, null));
				propertyDescriptors.Add(new ProxyPropertyDescriptor(type, "UpdateUrl", typeof(string), string.Empty, this.ProjectProperties, null));
				propertyDescriptors.Add(new ProxyPropertyDescriptor(type, "UpgradeCode", typeof(string), string.Empty, this.ProjectProperties, null));
				propertyDescriptors.Add(new ProxyPropertyDescriptor(type, "Version", typeof(string), string.Empty, this.ProjectProperties, null));
				propertyDescriptors.Add(new ProxyPropertyDescriptor(type, "AllowModification", typeof(bool), string.Empty, this.ProjectProperties, null));
				propertyDescriptors.Add(new ProxyPropertyDescriptor(type, "AllowRemoving", typeof(bool), string.Empty, this.ProjectProperties, null));
				propertyDescriptors.Add(new ProxyPropertyDescriptor(type, "AllowRepair", typeof(bool), string.Empty, this.ProjectProperties, null));
				propertyDescriptors.Add(new ProxyPropertyDescriptor(type, "PrerequisitesDummy", typeof(string), string.Empty, this.ProjectProperties, null));
				propertyDescriptors.Add(new ProxyPropertyDescriptor(type, "MyPrerequisitesDummy", typeof(string), string.Empty, this.ProjectProperties, null));
				ProxyPropertyDescriptor proxyPropertyDescriptor = new ProxyPropertyDescriptor(type, "LanguagesDummy", typeof(string), string.Empty, this.ProjectProperties, null)
				{
					Browsable = this.IsMultiLangSupported
				};
				propertyDescriptors.Add(proxyPropertyDescriptor);
			}
			else if (this.WiXModel.ProjectType == WiXProjectType.Module)
			{
				propertyDescriptors.Add(new ProxyPropertyDescriptor(type, "Author", typeof(string), string.Empty, this.ProjectProperties, null));
				propertyDescriptors.Add(new ProxyPropertyDescriptor(type, "Description", typeof(string), string.Empty, this.ProjectProperties, null));
				propertyDescriptors.Add(new ProxyPropertyDescriptor(type, "Keywords", typeof(string), string.Empty, this.ProjectProperties, null));
				propertyDescriptors.Add(new ProxyPropertyDescriptor(type, "Localization", typeof(string), string.Empty, this.ProjectProperties, null));
				propertyDescriptors.Add(new ProxyPropertyDescriptor(type, "Subject", typeof(string), string.Empty, this.ProjectProperties, null));
				propertyDescriptors.Add(new ProxyPropertyDescriptor(type, "Title", typeof(string), string.Empty, this.ProjectProperties, null));
				propertyDescriptors.Add(new ProxyPropertyDescriptor(type, "Version", typeof(string), string.Empty, this.ProjectProperties, null));
				propertyDescriptors.Add(new ProxyPropertyDescriptor(type, "ModuleSignature", typeof(string), string.Empty, this.ProjectProperties, null));
				ProxyPropertyDescriptor proxyPropertyDescriptor1 = new ProxyPropertyDescriptor(type, "LanguagesDummy", typeof(string), string.Empty, this.ProjectProperties, null)
				{
					Browsable = this.IsMultiLangSupported
				};
				propertyDescriptors.Add(proxyPropertyDescriptor1);
			}
			this.ProjectProperties.WiXPropertiesObject = wixPropertiesObject;
			AddinExpress.Installer.WiXDesigner.DesignTime.TypeDescriptor.AddDynamicProperties(wixPropertiesObject, propertyDescriptors);
			this.ProjectProperties.ResetReadFlags();
		}

		private void RemoveProjectItem(EnvDTE.ProjectItem item)
		{
			Document document = null;
			try
			{
				document = item.Document;
			}
			catch (Exception exception)
			{
			}
			if (document != null)
			{
				Window activeWindow = null;
				try
				{
					activeWindow = document.ActiveWindow;
				}
				catch (Exception exception1)
				{
				}
				if (activeWindow == null)
				{
					document.Close(vsSaveChanges.vsSaveChangesNo);
				}
				else
				{
					activeWindow.Close(vsSaveChanges.vsSaveChangesNo);
				}
			}
			item.Remove();
		}

		internal void RemoveProjectOutput(AddinExpress.Installer.WiXDesigner.OutputGroup outputGroup, ProjectDescriptor projectDesc)
		{
			if (projectDesc == null)
			{
				return;
			}
			VsWiXProject.ReferenceDescriptor referenceDescriptor = projectDesc.ReferenceDescriptor;
			if (referenceDescriptor == null)
			{
				foreach (VsWiXProject.ReferenceDescriptor value in this.References.Values)
				{
					if (value.ReferencedProject == null || !value.ReferencedProject.Guid.Equals(projectDesc.Guid))
					{
						continue;
					}
					referenceDescriptor = value;
					goto Label0;
				}
			}
		Label0:
			if (referenceDescriptor != null)
			{
				List<AddinExpress.Installer.WiXDesigner.OutputGroup> outputGroups = new List<AddinExpress.Installer.WiXDesigner.OutputGroup>(referenceDescriptor.ProjectOutputGroups);
				if (outputGroups.Contains(outputGroup))
				{
					outputGroups.Remove(outputGroup);
					if (outputGroups.Count > 0)
					{
						referenceDescriptor.ProjectOutputGroups = outputGroups.ToArray();
						return;
					}
					object referenceNode = referenceDescriptor.GetReferenceNode();
					if (referenceNode != null)
					{
						MethodInfo method = referenceNode.GetType().GetMethod("Remove", BindingFlags.Instance | BindingFlags.Public);
						if (method != null)
						{
							try
							{
								method.Invoke(referenceNode, new object[] { true });
							}
							catch (Exception exception)
							{
							}
						}
					}
				}
			}
		}

		public void SaveSolutionSettings(IPropertyBag pPropBag)
		{
		}

		public void SaveUserSettings(Hashtable userData)
		{
			Hashtable hashtables = new Hashtable();
			if (this.panes.Count > 0)
			{
				int activePaneID = this.GetActivePaneID();
				foreach (VsPaneBase value in this.panes.Values)
				{
					Hashtable hashtables1 = new Hashtable();
					hashtables1["Active"] = (activePaneID <= 0 || value.ID != activePaneID ? false : true);
					if (value.ViewManager != null)
					{
						value.ViewManager.OnSaveUserSettings(hashtables1);
					}
					hashtables[value.GetPaneName()] = hashtables1;
				}
			}
			userData[this.ProjectGuidString] = hashtables;
		}

		internal void SaveWiXFiles(int designerId, bool promptSave, bool closeAfterSave, bool checkEditLocks, out bool cancel)
		{
			cancel = false;
			if (this.allWiXFiles.Count > 0)
			{
				List<uint> nums = new List<uint>();
				VsWiXProject.WiXFileDescriptor[] wiXFileDescriptorArray = new VsWiXProject.WiXFileDescriptor[this.allWiXFiles.Count];
				this.allWiXFiles.Values.CopyTo(wiXFileDescriptorArray, 0);
				VsWiXProject.WiXFileDescriptor[] wiXFileDescriptorArray1 = wiXFileDescriptorArray;
				for (int i = 0; i < (int)wiXFileDescriptorArray1.Length; i++)
				{
					VsWiXProject.WiXFileDescriptor wiXFileDescriptor = wiXFileDescriptorArray1[i];
					if (wiXFileDescriptor.Opened)
					{
						if (!promptSave)
						{
							wiXFileDescriptor.Save(designerId, checkEditLocks, closeAfterSave, out cancel);
						}
						else
						{
							wiXFileDescriptor.SaveWithPrompt(designerId, checkEditLocks, closeAfterSave, out cancel);
						}
						if (wiXFileDescriptor.FileStatus == VsWiXProject.WiXFileDescriptor.Status.Removed)
						{
							nums.Add(wiXFileDescriptor.ItemID);
						}
						if (cancel)
						{
							break;
						}
					}
				}
				if (nums.Count > 0)
				{
					foreach (uint num in nums)
					{
						VsWiXProject.WiXFileDescriptor item = this.allWiXFiles[num];
						if (item == null)
						{
							continue;
						}
						item.Dispose();
						this.allWiXFiles.Remove(num);
					}
				}
			}
		}

		internal void SaveWiXFiles(int designerId)
		{
			bool flag;
			if (this.allWiXFiles.Count > 0)
			{
				List<uint> nums = new List<uint>();
				VsWiXProject.WiXFileDescriptor[] wiXFileDescriptorArray = new VsWiXProject.WiXFileDescriptor[this.allWiXFiles.Count];
				this.allWiXFiles.Values.CopyTo(wiXFileDescriptorArray, 0);
				VsWiXProject.WiXFileDescriptor[] wiXFileDescriptorArray1 = wiXFileDescriptorArray;
				for (int i = 0; i < (int)wiXFileDescriptorArray1.Length; i++)
				{
					VsWiXProject.WiXFileDescriptor wiXFileDescriptor = wiXFileDescriptorArray1[i];
					if (wiXFileDescriptor.Opened)
					{
						wiXFileDescriptor.Save(designerId, false, false, out flag);
						if (wiXFileDescriptor.FileStatus == VsWiXProject.WiXFileDescriptor.Status.Removed)
						{
							nums.Add(wiXFileDescriptor.ItemID);
						}
						if (flag)
						{
							break;
						}
					}
				}
				if (nums.Count > 0)
				{
					foreach (uint num in nums)
					{
						VsWiXProject.WiXFileDescriptor item = this.allWiXFiles[num];
						if (item == null)
						{
							continue;
						}
						item.Dispose();
						this.allWiXFiles.Remove(num);
					}
				}
			}
		}

		internal void SetCurrentLocalization(string lcid)
		{
			try
			{
				object obj = this.VsProject.Object;
				if (obj != null)
				{
					PropertyInfo property = obj.GetType().GetProperty("BuildProject", BindingFlags.Instance | BindingFlags.NonPublic);
					if (property != null)
					{
						object value = property.GetValue(obj, null);
						if (value is Microsoft.Build.Evaluation.Project)
						{
							Microsoft.Build.Evaluation.Project project = (Microsoft.Build.Evaluation.Project)value;
							ProjectProperty projectProperty = project.GetProperty("CurrentUILanguage");
							if (projectProperty == null)
							{
								ProjectPropertyGroupElement projectPropertyGroup = this.GetProjectPropertyGroup(project);
								if (projectPropertyGroup != null)
								{
									projectPropertyGroup.SetProperty("CurrentUILanguage", lcid);
									project.MarkDirty();
									project.ReevaluateIfNecessary();
									if (!this.MakeProjectDirty())
									{
										this.VsProject.Save("");
									}
								}
							}
							else
							{
								if (projectProperty.EvaluatedValue != lcid)
								{
									projectProperty.UnevaluatedValue = lcid;
									if (!this.MakeProjectDirty())
									{
										this.VsProject.Save("");
									}
								}
								return;
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

		internal void SetDynamicPropertyVisible(string propName, bool state)
		{
			if (this.ProjectProperties.WiXPropertiesObject != null)
			{
				AddinExpress.Installer.WiXDesigner.DesignTime.TypeDescriptor typeDescriptor = AddinExpress.Installer.WiXDesigner.DesignTime.TypeDescriptor.GetTypeDescriptor(this.ProjectProperties.WiXPropertiesObject);
				if (typeDescriptor != null)
				{
					System.ComponentModel.PropertyDescriptor item = typeDescriptor.GetProperties()[propName];
					if (item is ProxyPropertyDescriptor)
					{
						(item as ProxyPropertyDescriptor).Browsable = state;
					}
				}
				System.ComponentModel.TypeDescriptor.Refresh(this.ProjectProperties.WiXPropertiesObject);
			}
		}

		internal bool SetMultiLangPostBuildEvent(string currentCulture, List<string> langList)
		{
			int num;
			try
			{
				object obj = this.VsProject.Object;
				if (obj != null)
				{
					PropertyInfo property = obj.GetType().GetProperty("BuildProject", BindingFlags.Instance | BindingFlags.NonPublic);
					if (property != null)
					{
						object value = property.GetValue(obj, null);
						if (value is Microsoft.Build.Evaluation.Project)
						{
							bool flag = false;
							string str = ".msi";
							string empty = string.Empty;
							Microsoft.Build.Evaluation.Project project = (Microsoft.Build.Evaluation.Project)value;
							ProjectTargetElement afterBuildTarget = this.GetAfterBuildTarget(project);
							if (afterBuildTarget != null)
							{
								List<ProjectElement> projectElements = new List<ProjectElement>();
								foreach (ProjectElement child in afterBuildTarget.Children)
								{
									if (!(child is ProjectTaskElement))
									{
										continue;
									}
									ProjectTaskElement projectTaskElement = child as ProjectTaskElement;
									if (projectTaskElement.Name == "Exec")
									{
										string item = projectTaskElement.Parameters["Command"];
										if (!item.Contains("\\vdwtool.exe") && (!item.Contains("\\torch.exe") || !item.Contains("-o \"$(IntermediateOutputPath)Harvested MST\\")))
										{
											continue;
										}
										projectElements.Add(projectTaskElement);
									}
									else if (projectTaskElement.Name != "MakeDir")
									{
										if (!(projectTaskElement.Name == "RemoveDir") || !(projectTaskElement.Parameters["Directories"] == "$(IntermediateOutputPath)Harvested MST"))
										{
											continue;
										}
										projectElements.Add(projectTaskElement);
									}
									else
									{
										if (projectTaskElement.Parameters["Directories"] != "$(IntermediateOutputPath)Harvested MST")
										{
											continue;
										}
										projectElements.Add(projectTaskElement);
									}
								}
								if (projectElements.Count > 0)
								{
									foreach (ProjectElement projectElement in projectElements)
									{
										afterBuildTarget.RemoveChild(projectElement);
									}
									flag = true;
								}
								if (string.IsNullOrEmpty(currentCulture))
								{
									currentCulture = "0";
								}
								List<string> strs = new List<string>(langList);
								if (!strs.Contains(currentCulture))
								{
									strs.Insert(0, currentCulture);
								}
								bool flag1 = false;
								ProjectTaskElement projectTaskElement1 = null;
								if (this.WiXModel != null && this.WiXModel.ProjectType == WiXProjectType.Module)
								{
									str = ".msm";
								}
								foreach (string str1 in strs)
								{
									if (string.IsNullOrEmpty(str1) || !int.TryParse(str1, out num))
									{
										continue;
									}
									try
									{
										if (!ProjectUtilities.IsNeutralLCID(num))
										{
											CultureInfo cultureInfo = ProjectUtilities.GetCultureInfo(num);
											if (!flag1)
											{
												flag1 = true;
												projectTaskElement1 = afterBuildTarget.AddTask("RemoveDir");
												projectTaskElement1.SetParameter("Directories", "$(IntermediateOutputPath)Harvested MST");
												projectTaskElement1.Condition = "'$(OutputType)'=='Package' OR '$(OutputType)'=='Module'";
												projectTaskElement1 = afterBuildTarget.AddTask("MakeDir");
												projectTaskElement1.SetParameter("Directories", "$(IntermediateOutputPath)Harvested MST");
												projectTaskElement1.Condition = "'$(OutputType)'=='Package' OR '$(OutputType)'=='Module'";
											}
											projectTaskElement1 = afterBuildTarget.AddTask("Exec");
											projectTaskElement1.SetParameter("Command", string.Concat(new string[] { "\"$(Wix)Bin\\torch.exe\" -nologo -serr f \"$(OutputPath)$(OutputName)", str, "\" \"$(OutputPath)", cultureInfo.Name, "\\$(OutputName)", str, "\" -o \"$(IntermediateOutputPath)Harvested MST\\", cultureInfo.Name, ".mst\"" }));
											projectTaskElement1.SetParameter("IgnoreExitCode", "false");
											projectTaskElement1.SetParameter("WorkingDirectory", "$(MSBuildProjectDirectory)");
											projectTaskElement1.Condition = string.Concat(new string[] { "('$(OutputType)'=='Package' OR '$(OutputType)'=='Module') AND Exists('$(OutputPath)$(OutputName)", str, "') AND Exists('$(OutputPath)", cultureInfo.Name, "\\$(OutputName)", str, "')" });
											projectTaskElement1 = afterBuildTarget.AddTask("Exec");
											projectTaskElement1.SetParameter("Command", string.Concat(new string[] { "\"$(MSBuildProjectDirectory)\\Resources\\vdwtool.exe\" -nologo -noconsole \"$(OutputPath)$(OutputName)", str, "\" \"$(IntermediateOutputPath)Harvested MST\\", cultureInfo.Name, ".mst\"" }));
											projectTaskElement1.SetParameter("IgnoreExitCode", "false");
											projectTaskElement1.SetParameter("WorkingDirectory", "$(MSBuildProjectDirectory)");
											projectTaskElement1.Condition = string.Concat(new string[] { "('$(OutputType)'=='Package' OR '$(OutputType)'=='Module') AND Exists('$(OutputPath)$(OutputName)", str, "') AND Exists('$(IntermediateOutputPath)Harvested MST\\", cultureInfo.Name, ".mst')" });
										}
									}
									catch (Exception exception)
									{
									}
								}
								if (projectTaskElement1 != null)
								{
									flag = true;
								}
								if (flag)
								{
									project.MarkDirty();
									project.ReevaluateIfNecessary();
									this.VsProject.Save("");
								}
								return true;
							}
						}
					}
				}
			}
			catch (Exception exception1)
			{
				DTEHelperObject.ShowErrorDialog(this, exception1);
			}
			return false;
		}

		internal void SetMultiSelectMode()
		{
			this.multiSelectMode = true;
		}

		internal void SetSupportedLanguages(string langList)
		{
			try
			{
				object obj = this.VsProject.Object;
				if (obj != null)
				{
					PropertyInfo property = obj.GetType().GetProperty("BuildProject", BindingFlags.Instance | BindingFlags.NonPublic);
					if (property != null)
					{
						object value = property.GetValue(obj, null);
						if (value is Microsoft.Build.Evaluation.Project)
						{
							Microsoft.Build.Evaluation.Project project = (Microsoft.Build.Evaluation.Project)value;
							ProjectProperty projectProperty = project.GetProperty("SupportedUILanguages");
							if (projectProperty == null)
							{
								ProjectPropertyGroupElement projectPropertyGroup = this.GetProjectPropertyGroup(project);
								if (projectPropertyGroup != null)
								{
									projectPropertyGroup.SetProperty("SupportedUILanguages", langList);
									project.MarkDirty();
									project.ReevaluateIfNecessary();
									if (!this.MakeProjectDirty())
									{
										this.VsProject.Save("");
									}
								}
							}
							else
							{
								if (projectProperty.EvaluatedValue != langList)
								{
									projectProperty.UnevaluatedValue = langList;
									if (!this.MakeProjectDirty())
									{
										this.VsProject.Save("");
									}
								}
								return;
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

		public VsPaneBase ShowWindow(string paneName, bool activate, bool userAction)
		{
			if (string.IsNullOrEmpty(paneName))
			{
				return null;
			}
			int fileSystemPaneID = 0;
			VsPaneBase item = null;
			if (!this.panes.ContainsKey(paneName))
			{
				if (paneName != null)
				{
					if (paneName == "File System")
					{
						fileSystemPaneID = this.FileSystemPaneID;
						item = VsPackage.CurrentInstance.CreateEditorPane(typeof(FileSystemPane), this.FileSystemPaneID) as VsPaneBase;
					}
					else if (paneName == "Registry")
					{
						fileSystemPaneID = this.RegistryPaneID;
						item = VsPackage.CurrentInstance.CreateEditorPane(typeof(RegistryPane), this.RegistryPaneID) as VsPaneBase;
					}
					else if (paneName == "File Types")
					{
						fileSystemPaneID = this.FileTypesPaneID;
						item = VsPackage.CurrentInstance.CreateEditorPane(typeof(FileTypesPane), this.FileTypesPaneID) as VsPaneBase;
					}
					else if (paneName == "User Interface")
					{
						fileSystemPaneID = this.UserInterfacePaneID;
						item = VsPackage.CurrentInstance.CreateEditorPane(typeof(UserInterfacePane), this.UserInterfacePaneID) as VsPaneBase;
					}
					else if (paneName == "Custom Actions")
					{
						fileSystemPaneID = this.CustomActionsPaneID;
						item = VsPackage.CurrentInstance.CreateEditorPane(typeof(CustomActionsPane), this.CustomActionsPaneID) as VsPaneBase;
					}
					else if (paneName == "Launch Conditions")
					{
						fileSystemPaneID = this.LaunchConditionsPaneID;
						item = VsPackage.CurrentInstance.CreateEditorPane(typeof(LaunchConditionsPane), this.LaunchConditionsPaneID) as VsPaneBase;
					}
				}
				if (item == null || item.get_Frame() == null)
				{
					throw new NotSupportedException(Resources.CanNotCreateWindow);
				}
				this.panes[paneName] = item;
				item.Initialize(this, fileSystemPaneID, this.solutionLoadedCompletely, this.buildStarted);
				IVsWindowFrame frame = (IVsWindowFrame)item.get_Frame();
				if (!activate)
				{
					ErrorHandler.ThrowOnFailure(frame.ShowNoActivate());
				}
				else
				{
					ErrorHandler.ThrowOnFailure(frame.Show());
				}
			}
			else
			{
				item = this.panes[paneName];
				IVsWindowFrame vsWindowFrame = (IVsWindowFrame)item.get_Frame();
				if (!activate)
				{
					ErrorHandler.ThrowOnFailure(vsWindowFrame.ShowNoActivate());
				}
				else
				{
					ErrorHandler.ThrowOnFailure(vsWindowFrame.Show());
				}
			}
			return item;
		}

		internal void UnregisterPane(string paneName)
		{
			if (this.panes.ContainsKey(paneName))
			{
				this.CloseWiXFiles(this.panes[paneName].ID);
				this.panes.Remove(paneName);
			}
		}

		private void UpdateCOMReferences()
		{
			for (int i = 0; i < this.referenceList.Count; i++)
			{
				VsWiXProject.ReferenceDescriptor item = this.referenceList.Values[i];
				if (item.IsXSLTOutputsSupported)
				{
					object projectOutputProperty = item.GetProjectOutputProperty(AddinExpress.Installer.WiXDesigner.OutputGroup.Binaries, OutputGroupProperties.RegisterForCOM);
					if (projectOutputProperty is bool && (bool)projectOutputProperty)
					{
						item.SetProjectOutputProperty(AddinExpress.Installer.WiXDesigner.OutputGroup.Binaries, OutputGroupProperties.RegisterForCOM, true);
					}
				}
			}
		}

		internal void UpdateLocUIFiles(string currentLCID, bool forceParse)
		{
			List<string> strs = new List<string>()
			{
				this.ProjectProperties.LCID
			};
			foreach (string language in this.ProjectProperties.Languages)
			{
				strs.Add(language);
			}
			this.UpdateLocUIFiles(null, currentLCID, strs, forceParse);
		}

		internal void UpdateLocUIFiles(CultureInfo newCulture, string currentLCID, List<string> languages, bool forceParse)
		{
			if (this.IsMultiLangSupported && this.WiXModel.ProjectType == WiXProjectType.Product)
			{
				bool flag = true;
				List<string> strs = new List<string>();
				List<string> strs1 = new List<string>();
				if (newCulture == null)
				{
					foreach (string language in languages)
					{
						strs1.Add(language);
					}
					List<WiXLocalization> wiXLocalizations = this.WiXModel.Languages.FindAll((WiXLocalization e) => e != null);
					if (wiXLocalizations.Count > 0)
					{
						foreach (WiXLocalization wiXLocalization in wiXLocalizations)
						{
							string id = wiXLocalization.Id;
							if (languages.Contains(id) || id.Equals(currentLCID))
							{
								strs1.Remove(id);
							}
							else
							{
								if (ProjectUtilities.IsNeutralLCID(id))
								{
									continue;
								}
								if (!strs.Contains(id))
								{
									strs.Add(id);
								}
								flag = true;
							}
						}
					}
				}
				bool flag1 = false;
				WiXEntity wiXEntity = null;
				if (flag)
				{
					try
					{
					Label0:
						foreach (string str in strs)
						{
							wiXEntity = this.WiXModel.Languages.Find((WiXLocalization e) => {
								if (e == null)
								{
									return false;
								}
								return e.GetAttributeValue("Language") == str;
							});
							if (wiXEntity == null || wiXEntity.Owner == null)
							{
								continue;
							}
							string sourceFile = (wiXEntity.Owner as WiXProjectItem).SourceFile;
							foreach (VsWiXProject.WiXFileDescriptor value in this.AllWiXFiles.Values)
							{
								if (!value.FilePath.Equals(sourceFile, StringComparison.OrdinalIgnoreCase))
								{
									continue;
								}
								EnvDTE.ProjectItem projectItemByPath = ProjectUtilities.GetProjectItemByPath(sourceFile, this.VsProject.ProjectItems);
								if (projectItemByPath == null)
								{
									break;
								}
								projectItemByPath.Delete();
								flag1 = true;
								goto Label0;
							}
						}
					}
					finally
					{
						this.checkSupportedLanguages = false;
					}
				}
				foreach (string str1 in strs1)
				{
					wiXEntity = this.WiXModel.Languages.Find((WiXLocalization e) => {
						if (e == null)
						{
							return false;
						}
						return e.GetAttributeValue("Language") == str1;
					});
					if (wiXEntity != null)
					{
						continue;
					}
					string cultureByLCID = VSDialogBase.GetCultureByLCID(str1);
					string str2 = string.Concat("StandardUI_", cultureByLCID, ".wxl");
					if (VSDialogBase.AddStandardUILocProjectItem(this.WiXModel, this.WiXModel.Namespace, str2, VSDialogBase.GetCultureInfoByLCID(str1), VSDialogBase.IsLCIDSupported(str1)) == null)
					{
						continue;
					}
					flag1 = true;
				}
				wiXEntity = this.WiXModel.Languages.Find((WiXLocalization e) => {
					if (e == null)
					{
						return false;
					}
					return ProjectUtilities.IsNeutralLCID(e.GetAttributeValue("Language"));
				});
				if (wiXEntity == null)
				{
					string str3 = string.Concat("StandardUI_", "neutral", ".wxl");
					if (VSDialogBase.AddStandardUILocProjectItem(this.WiXModel, this.WiXModel.Namespace, str3, VSDialogBase.GetCultureInfoByLCID("0"), false) != null)
					{
						flag1 = true;
					}
				}
				if (flag1)
				{
					this.ParseXML(forceParse);
				}
			}
		}

		internal void UpdateProjectTree()
		{
			EnvDTE.ProjectItem projectItem = null;
			try
			{
				int num = 1;
				while (num <= this.VsProject.ProjectItems.Count)
				{
					EnvDTE.ProjectItem projectItem1 = this.VsProject.ProjectItems.Item(num);
					if (projectItem1 == null || string.IsNullOrEmpty(projectItem1.Kind) || string.IsNullOrEmpty(projectItem1.Name) || !projectItem1.Kind.Equals("{6BB5F8EF-4483-11D3-8BCF-00C04F8EC28C}", StringComparison.InvariantCultureIgnoreCase) || !projectItem1.Name.Equals("Resources", StringComparison.InvariantCultureIgnoreCase))
					{
						num++;
					}
					else
					{
						projectItem = projectItem1;
						break;
					}
				}
				string str = Path.Combine(Path.GetDirectoryName(this.VsProject.FullName), "Resources");
				if (projectItem != null)
				{
					this.UpdateResourceFolder(str, projectItem);
				}
				else if (Directory.Exists(str) && Directory.GetFiles(str, "*", SearchOption.AllDirectories).Length != 0)
				{
					projectItem = this.VsProject.ProjectItems.AddFolder(str, "{6BB5F8EF-4483-11D3-8BCF-00C04F8EC28C}");
					this.UpdateResourceFolder(str, projectItem);
				}
			}
			catch (Exception exception)
			{
			}
		}

		private void UpdateResourceFolder(string rootDirPath, EnvDTE.ProjectItem resFolderItem)
		{
			EnvDTE.ProjectItem projectItem;
			string[] strArrays;
			int i;
			if (resFolderItem != null)
			{
				string[] files = Directory.GetFiles(rootDirPath, "*", SearchOption.TopDirectoryOnly);
				if (files.Length != 0)
				{
					strArrays = files;
					for (i = 0; i < (int)strArrays.Length; i++)
					{
						string str = strArrays[i];
						projectItem = resFolderItem.ProjectItems.Item(Path.GetFileName(str));
						if (projectItem == null)
						{
							resFolderItem.ProjectItems.AddFromFile(str);
						}
					}
				}
				string[] directories = Directory.GetDirectories(rootDirPath, "*", SearchOption.TopDirectoryOnly);
				if (directories.Length != 0)
				{
					strArrays = directories;
					for (i = 0; i < (int)strArrays.Length; i++)
					{
						string str1 = strArrays[i];
						string[] strArrays1 = str1.Split(new char[] { Path.DirectorySeparatorChar });
						string str2 = strArrays1[(int)strArrays1.Length - 1];
						projectItem = resFolderItem.ProjectItems.Item(str2) ?? resFolderItem.ProjectItems.AddFolder(str2, "{6BB5F8EF-4483-11D3-8BCF-00C04F8EC28C}");
						this.UpdateResourceFolder(str1, projectItem);
					}
				}
			}
		}

		private void UpdateSupportedLanguages()
		{
			if (this.checkSupportedLanguages)
			{
				this.multiSelectMode = false;
				this.checkSupportedLanguages = false;
				List<string> strs = new List<string>(this.ProjectProperties.Languages);
				List<string> strs1 = new List<string>();
				List<WiXLocalization> wiXLocalizations = this.WiXModel.Languages.FindAll((WiXLocalization e) => e != null);
				if (wiXLocalizations.Count > 0)
				{
					foreach (WiXLocalization wiXLocalization in wiXLocalizations)
					{
						string id = wiXLocalization.Id;
						if (ProjectUtilities.IsNeutralLCID(id))
						{
							continue;
						}
						if (strs.Contains(id))
						{
							strs.Remove(id);
						}
						if (strs1.Contains(id))
						{
							continue;
						}
						strs1.Add(id);
					}
					if (strs.Count != 0)
					{
						this.ProjectProperties.Languages = strs1;
						this.ProjectProperties.LanguagesDummy = "dummy text";
						if (this.ProjectProperties.WiXPropertiesObject != null)
						{
							System.ComponentModel.TypeDescriptor.Refresh(this.ProjectProperties.WiXPropertiesObject);
						}
					}
				}
			}
		}

		private void Wait_a_little(object sender, EventArgs e)
		{
			Application.Idle -= new EventHandler(this.Wait_a_little);
		}

		public enum ChangeContextType
		{
			SolutionLoaded = 1,
			ProjectClosing = 2,
			ProjectParentChanged = 3,
			ProjectRenamed = 4,
			ProjectAdded = 5,
			FileAdded = 6,
			FileRemoved = 7,
			FileRenamed = 8,
			ReferenceAdded = 9,
			ReferenceRemoved = 10,
			ReferenceRenamed = 11,
			BuildStarted = 12,
			BuildStopped = 13,
			ReferenceRefreshed = 14,
			ProjectPropertiesSelected = 15
		}

		private struct DelayActionData
		{
			public VsWiXProject.RefRefreshReason ActionReason;

			public string OldReferenceName;

			public string NewReferenceName;

			public string XMLFileName;

			public VsWiXProject.ReferenceDescriptor Reference;
		}

		public class LanguageItem : Component
		{
			private bool disposed;

			private string name;

			private string lcid;

			public string LCID
			{
				get
				{
					return this.lcid;
				}
			}

			public string Name
			{
				get
				{
					return this.name;
				}
				set
				{
					this.name = value;
				}
			}

			public LanguageItem()
			{
			}

			protected override void Dispose(bool disposing)
			{
				if (disposing && !this.disposed)
				{
					this.disposed = true;
				}
				base.Dispose(disposing);
			}
		}

		public class ProjectPropertiesObject : Component
		{
			private object wixPropertiesObject;

			private string addRemoveProgramsIcon;

			private bool addRemoveProgramsIconRead;

			private string author;

			private bool authorRead;

			private string description;

			private bool descriptionRead;

			private bool detectNewerInstalledVersion;

			private bool detectNewerInstalledVersionRead;

			private bool installAllUsers;

			private bool installAllUsersRead;

			private string keywords;

			private bool keywordsRead;

			private string localization;

			private bool localizationRead;

			private string lcid;

			private bool lcidRead;

			private List<string> languages;

			private bool languagesRead;

			private string manufacturer;

			private bool manufacturerRead;

			private string manufacturerUrl;

			private bool manufacturerUrlRead;

			private string postBuildEvent;

			private string preBuildEvent;

			private string runPostBuildEvent;

			private string productCode;

			private bool productCodeRead;

			private string productName;

			private bool productNameRead;

			private bool removePreviousVersions;

			private bool removePreviousVersionsRead;

			private string subject;

			private bool subjectRead;

			private string supportPhone;

			private bool supportPhoneRead;

			private string supportUrl;

			private bool supportUrlRead;

			private string comments;

			private bool commentsRead;

			private string updateUrl;

			private bool updateUrlRead;

			private WiXSupportedPlatforms targetPlatform;

			private bool targetPlatformRead;

			private string upgradeCode;

			private bool upgradeCodeRead;

			private string version;

			private bool versionRead;

			private bool allowModification;

			private bool allowModificationRead;

			private bool allowRemoving;

			private bool allowRemovingRead;

			private bool allowRepair;

			private bool allowRepairRead;

			private string moduleSignature;

			private bool moduleSignatureRead;

			private Dictionary<string, Dictionary<string, string>> prerequisites;

			private bool prerequisitesRead;

			private string prerequisitesConfigName;

			private List<string> myPrerequisites;

			private bool myPrerequisitesRead;

			private VsWiXProject project;

			private const string CategotyName = "Properties";

			public string AddRemoveProgramsIcon
			{
				get
				{
					this.Project.CheckParseDone();
					if (!this.addRemoveProgramsIconRead)
					{
						string empty = string.Empty;
						this.addRemoveProgramsIconRead = true;
						WiXEntity wiXEntity = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => {
							if (!(b is WiXProperty))
							{
								return false;
							}
							return b.GetAttributeValue("Id") == "ARPPRODUCTICON";
						});
						if (wiXEntity != null && wiXEntity.FirstChild != null)
						{
							XmlNode xmlNode = ((WiXEntity)wiXEntity.FirstChild).XmlNode;
							if (xmlNode.NodeType == XmlNodeType.Text)
							{
								empty = ((XmlText)xmlNode).Value;
							}
							else if (xmlNode.NodeType == XmlNodeType.CDATA)
							{
								empty = ((XmlCDataSection)xmlNode).Value;
							}
						}
						if (!string.IsNullOrEmpty(empty))
						{
							WiXEntity wiXEntity1 = this.Project.WiXModel.Icons.Find((WiXIcon b) => {
								if (b == null)
								{
									return false;
								}
								return b.GetAttributeValue("Id") == empty;
							});
							if (wiXEntity1 != null)
							{
								this.addRemoveProgramsIcon = wiXEntity1.GetAttributeValue("SourceFile");
							}
						}
					}
					return this.addRemoveProgramsIcon;
				}
				set
				{
					if (this.addRemoveProgramsIcon != value)
					{
						bool flag = false;
						WiXEntity wiXProperty = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => {
							if (!(b is WiXProperty))
							{
								return false;
							}
							return b.GetAttributeValue("Id") == "ARPPRODUCTICON";
						});
						if (wiXProperty != null)
						{
							string empty = string.Empty;
							if (wiXProperty.FirstChild != null)
							{
								XmlNode xmlNode = ((WiXEntity)wiXProperty.FirstChild).XmlNode;
								if (xmlNode.NodeType == XmlNodeType.Text)
								{
									empty = ((XmlText)xmlNode).Value;
								}
								else if (xmlNode.NodeType == XmlNodeType.CDATA)
								{
									empty = ((XmlCDataSection)xmlNode).Value;
								}
							}
							this.Project.WiXModel.SupportedEntities.Remove(wiXProperty);
							wiXProperty.Delete();
							flag = true;
							if (!string.IsNullOrEmpty(empty))
							{
								WiXIcon wiXIcon = this.Project.WiXModel.Icons.Find((WiXIcon b) => {
									if (b == null)
									{
										return false;
									}
									return b.GetAttributeValue("Id") == empty;
								});
								if (wiXIcon != null)
								{
									this.Project.WiXModel.Icons.Remove(wiXIcon);
									wiXIcon.Delete();
								}
							}
						}
						if (!string.IsNullOrEmpty(value))
						{
							WiXPackage wiXPackage = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXPackage) as WiXPackage;
							if (wiXPackage != null)
							{
								string str = Common.GenerateIconId();
								WiXEntity parent = wiXPackage.Parent as WiXEntity;
								XmlElement xmlElement = Common.CreateXmlElementWithAttributes(parent.XmlNode.OwnerDocument, "Property", parent.XmlNode.NamespaceURI, new string[] { "Id" }, new string[] { "ARPPRODUCTICON" }, "", false);
								XmlText xmlText = xmlElement.OwnerDocument.CreateTextNode(str);
								xmlElement.AppendChild(xmlText);
								if (!parent.HasChildEntities)
								{
									parent.XmlNode.AppendChild(xmlElement);
								}
								else
								{
									parent.XmlNode.InsertAfter(xmlElement, wiXPackage.XmlNode);
								}
								wiXProperty = new WiXProperty(this.Project.WiXModel, parent.Owner, parent, xmlElement);
								WiXEntity wiXEntity = new WiXEntity(this.Project.WiXModel, wiXProperty.Owner, wiXProperty, xmlText);
								this.Project.WiXModel.SupportedEntities.Add(wiXProperty);
								xmlElement = Common.CreateXmlElementWithAttributes(parent.XmlNode.OwnerDocument, "Icon", parent.XmlNode.NamespaceURI, new string[] { "Id", "SourceFile" }, new string[] { str, value }, "", false);
								if (!parent.HasChildEntities)
								{
									parent.XmlNode.AppendChild(xmlElement);
								}
								else
								{
									parent.XmlNode.InsertAfter(xmlElement, wiXProperty.XmlNode);
								}
								parent.SetDirty();
								this.Project.WiXModel.Icons.Add(new WiXIcon(this.Project.WiXModel, parent.Owner, parent, xmlElement));
								flag = true;
							}
						}
						if (flag)
						{
							this.Project.CommitXmlChanges();
							this.addRemoveProgramsIcon = value;
						}
					}
				}
			}

			[Category("Properties")]
			[DefaultValue(null)]
			[Description("Specifies an icon to be displayed in the Add/Remove Programs dialog box on the target computer.")]
			[Editor(typeof(InstallerIconEditor), typeof(UITypeEditor))]
			[LocDisplayName("AddRemoveProgramsIcon")]
			public string AddRemoveProgramsIconDummy
			{
				get
				{
					string addRemoveProgramsIcon = this.AddRemoveProgramsIcon;
					if (addRemoveProgramsIcon == null)
					{
						return "(None)";
					}
					if (addRemoveProgramsIcon != string.Empty)
					{
						return "(Icon)";
					}
					return string.Empty;
				}
				set
				{
					if (value != null && value.Equals("(None)", StringComparison.OrdinalIgnoreCase) || value == string.Empty)
					{
						this.AddRemoveProgramsIcon = null;
					}
				}
			}

			[Category("Properties")]
			[Description("Enables Add or Remove Programs functionality in Control Panel that modifies the product.")]
			[LocDisplayName("AllowModification")]
			public bool AllowModification
			{
				get
				{
					this.Project.CheckParseDone();
					if (!this.allowModificationRead)
					{
						this.allowModificationRead = true;
						WiXEntity wiXEntity = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => {
							if (!(b is WiXProperty))
							{
								return false;
							}
							return b.GetAttributeValue("Id") == "ARPNOMODIFY";
						});
						if (wiXEntity != null)
						{
							this.allowModification = wiXEntity.GetAttributeValue("Value") == "no";
						}
					}
					return this.allowModification;
				}
				set
				{
					if (this.allowModification != value)
					{
						bool flag = false;
						WiXEntity wiXProperty = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => {
							if (!(b is WiXProperty))
							{
								return false;
							}
							return b.GetAttributeValue("Id") == "ARPNOMODIFY";
						});
						if (wiXProperty == null)
						{
							WiXPackage wiXPackage = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXPackage) as WiXPackage;
							if (wiXPackage != null)
							{
								WiXEntity parent = wiXPackage.Parent as WiXEntity;
								XmlDocument ownerDocument = parent.XmlNode.OwnerDocument;
								string namespaceURI = parent.XmlNode.NamespaceURI;
								string[] strArrays = new string[] { "Id", "Value", "Secure" };
								string[] strArrays1 = new string[] { "ARPNOMODIFY", null, null };
								strArrays1[1] = (value ? "no" : "yes");
								strArrays1[2] = "yes";
								XmlElement xmlElement = Common.CreateXmlElementWithAttributes(ownerDocument, "Property", namespaceURI, strArrays, strArrays1, "", false);
								if (!parent.HasChildEntities)
								{
									parent.XmlNode.AppendChild(xmlElement);
								}
								else
								{
									parent.XmlNode.InsertAfter(xmlElement, wiXPackage.XmlNode);
								}
								parent.SetDirty();
								wiXProperty = new WiXProperty(this.Project.WiXModel, parent.Owner, parent, xmlElement);
								this.Project.WiXModel.SupportedEntities.Add(wiXProperty);
								flag = true;
							}
						}
						else
						{
							if (value)
							{
								wiXProperty.SetAttributeValue("Value", "no");
							}
							else
							{
								wiXProperty.SetAttributeValue("Value", "yes");
							}
							wiXProperty.SetDirty();
							flag = true;
						}
						if (flag)
						{
							this.Project.CommitXmlChanges();
							this.allowModification = value;
						}
					}
				}
			}

			[Category("Properties")]
			[Description("Enables the Add or Remove Programs functionality in Control Panel that removes the product.")]
			[LocDisplayName("AllowRemoving")]
			public bool AllowRemoving
			{
				get
				{
					this.Project.CheckParseDone();
					if (!this.allowRemovingRead)
					{
						this.allowRemovingRead = true;
						WiXEntity wiXEntity = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => {
							if (!(b is WiXProperty))
							{
								return false;
							}
							return b.GetAttributeValue("Id") == "ARPNOREMOVE";
						});
						if (wiXEntity != null)
						{
							this.allowRemoving = wiXEntity.GetAttributeValue("Value") == "no";
						}
					}
					return this.allowRemoving;
				}
				set
				{
					if (this.allowRemoving != value)
					{
						bool flag = false;
						WiXEntity wiXProperty = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => {
							if (!(b is WiXProperty))
							{
								return false;
							}
							return b.GetAttributeValue("Id") == "ARPNOREMOVE";
						});
						if (wiXProperty == null)
						{
							WiXPackage wiXPackage = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXPackage) as WiXPackage;
							if (wiXPackage != null)
							{
								WiXEntity parent = wiXPackage.Parent as WiXEntity;
								XmlDocument ownerDocument = parent.XmlNode.OwnerDocument;
								string namespaceURI = parent.XmlNode.NamespaceURI;
								string[] strArrays = new string[] { "Id", "Value", "Secure" };
								string[] strArrays1 = new string[] { "ARPNOREMOVE", null, null };
								strArrays1[1] = (value ? "no" : "yes");
								strArrays1[2] = "yes";
								XmlElement xmlElement = Common.CreateXmlElementWithAttributes(ownerDocument, "Property", namespaceURI, strArrays, strArrays1, "", false);
								if (!parent.HasChildEntities)
								{
									parent.XmlNode.AppendChild(xmlElement);
								}
								else
								{
									parent.XmlNode.InsertAfter(xmlElement, wiXPackage.XmlNode);
								}
								parent.SetDirty();
								wiXProperty = new WiXProperty(this.Project.WiXModel, parent.Owner, parent, xmlElement);
								this.Project.WiXModel.SupportedEntities.Add(wiXProperty);
								flag = true;
							}
						}
						else
						{
							if (value)
							{
								wiXProperty.SetAttributeValue("Value", "no");
							}
							else
							{
								wiXProperty.SetAttributeValue("Value", "yes");
							}
							wiXProperty.SetDirty();
							flag = true;
						}
						if (flag)
						{
							this.Project.CommitXmlChanges();
							this.allowRemoving = value;
						}
					}
				}
			}

			[Category("Properties")]
			[Description("Enables the Repair button in the Programs Wizard.")]
			[LocDisplayName("AllowRepair")]
			public bool AllowRepair
			{
				get
				{
					this.Project.CheckParseDone();
					if (!this.allowRepairRead)
					{
						this.allowRepairRead = true;
						WiXEntity wiXEntity = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => {
							if (!(b is WiXProperty))
							{
								return false;
							}
							return b.GetAttributeValue("Id") == "ARPNOREPAIR";
						});
						if (wiXEntity != null)
						{
							this.allowRepair = wiXEntity.GetAttributeValue("Value") == "no";
						}
					}
					return this.allowRepair;
				}
				set
				{
					if (this.allowRepair != value)
					{
						bool flag = false;
						WiXEntity wiXProperty = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => {
							if (!(b is WiXProperty))
							{
								return false;
							}
							return b.GetAttributeValue("Id") == "ARPNOREPAIR";
						});
						if (wiXProperty == null)
						{
							WiXPackage wiXPackage = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXPackage) as WiXPackage;
							if (wiXPackage != null)
							{
								WiXEntity parent = wiXPackage.Parent as WiXEntity;
								XmlDocument ownerDocument = parent.XmlNode.OwnerDocument;
								string namespaceURI = parent.XmlNode.NamespaceURI;
								string[] strArrays = new string[] { "Id", "Value", "Secure" };
								string[] strArrays1 = new string[] { "ARPNOREPAIR", null, null };
								strArrays1[1] = (value ? "no" : "yes");
								strArrays1[2] = "yes";
								XmlElement xmlElement = Common.CreateXmlElementWithAttributes(ownerDocument, "Property", namespaceURI, strArrays, strArrays1, "", false);
								if (!parent.HasChildEntities)
								{
									parent.XmlNode.AppendChild(xmlElement);
								}
								else
								{
									parent.XmlNode.InsertAfter(xmlElement, wiXPackage.XmlNode);
								}
								parent.SetDirty();
								wiXProperty = new WiXProperty(this.Project.WiXModel, parent.Owner, parent, xmlElement);
								this.Project.WiXModel.SupportedEntities.Add(wiXProperty);
								flag = true;
							}
						}
						else
						{
							if (value)
							{
								wiXProperty.SetAttributeValue("Value", "no");
							}
							else
							{
								wiXProperty.SetAttributeValue("Value", "yes");
							}
							wiXProperty.SetDirty();
							flag = true;
						}
						if (flag)
						{
							this.Project.CommitXmlChanges();
							this.allowRepair = value;
						}
					}
				}
			}

			[Category("Properties")]
			[Description("Specifies the name of the author of an application or component.")]
			[LocDisplayName("Author")]
			public string Author
			{
				get
				{
					this.Project.CheckParseDone();
					if (!this.authorRead)
					{
						this.authorRead = true;
						WiXPackage wiXPackage = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXPackage) as WiXPackage;
						if (wiXPackage != null)
						{
							this.author = wiXPackage.Manufacturer;
						}
					}
					return this.author;
				}
				set
				{
					if (!string.IsNullOrEmpty(value) && this.author != value)
					{
						bool flag = false;
						WiXPackage wiXPackage = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXPackage) as WiXPackage;
						if (wiXPackage != null)
						{
							wiXPackage.Manufacturer = value;
							flag = true;
							WiXEntity wiXProperty = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => {
								if (!(b is WiXProperty))
								{
									return false;
								}
								return b.GetAttributeValue("Id") == "ARPCONTACT";
							});
							if (wiXProperty != null)
							{
								if (string.IsNullOrEmpty(value))
								{
									this.Project.WiXModel.SupportedEntities.Remove(wiXProperty);
									wiXProperty.Delete();
								}
								else
								{
									wiXProperty.SetAttributeValue("Value", value);
									wiXProperty.SetDirty();
								}
							}
							else if (!string.IsNullOrEmpty(value))
							{
								WiXEntity parent = wiXPackage.Parent as WiXEntity;
								XmlElement xmlElement = Common.CreateXmlElementWithAttributes(parent.XmlNode.OwnerDocument, "Property", parent.XmlNode.NamespaceURI, new string[] { "Id", "Value" }, new string[] { "ARPCONTACT", value }, "", false);
								if (!parent.HasChildEntities)
								{
									parent.XmlNode.AppendChild(xmlElement);
								}
								else
								{
									parent.XmlNode.InsertAfter(xmlElement, wiXPackage.XmlNode);
								}
								parent.SetDirty();
								wiXProperty = new WiXProperty(this.Project.WiXModel, parent.Owner, parent, xmlElement);
								this.Project.WiXModel.SupportedEntities.Add(wiXProperty);
							}
						}
						if (flag)
						{
							this.Project.CommitXmlChanges();
							this.author = value;
						}
					}
				}
			}

			[Category("Properties")]
			[Description("Specifies the free-form comments for an installer.")]
			[LocDisplayName("Description")]
			public string Description
			{
				get
				{
					this.Project.CheckParseDone();
					if (!this.commentsRead)
					{
						this.commentsRead = true;
						WiXEntity wiXEntity = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => {
							if (!(b is WiXProperty))
							{
								return false;
							}
							return b.GetAttributeValue("Id") == "ARPCOMMENTS";
						});
						if (wiXEntity != null)
						{
							this.comments = wiXEntity.GetAttributeValue("Value");
						}
					}
					return this.comments;
				}
				set
				{
					if (this.comments != value)
					{
						bool flag = false;
						WiXPackage wiXPackage = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXPackage) as WiXPackage;
						if (wiXPackage != null)
						{
							wiXPackage.Comments = value;
							flag = true;
						}
						WiXEntity wiXProperty = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => {
							if (!(b is WiXProperty))
							{
								return false;
							}
							return b.GetAttributeValue("Id") == "ARPCOMMENTS";
						});
						if (wiXProperty != null)
						{
							if (string.IsNullOrEmpty(value))
							{
								this.Project.WiXModel.SupportedEntities.Remove(wiXProperty);
								wiXProperty.Delete();
							}
							else
							{
								wiXProperty.SetAttributeValue("Value", value);
								wiXProperty.SetDirty();
							}
							flag = true;
						}
						else if (!string.IsNullOrEmpty(value) && wiXPackage != null)
						{
							WiXEntity parent = wiXPackage.Parent as WiXEntity;
							XmlElement xmlElement = Common.CreateXmlElementWithAttributes(parent.XmlNode.OwnerDocument, "Property", parent.XmlNode.NamespaceURI, new string[] { "Id", "Value" }, new string[] { "ARPCOMMENTS", value }, "", false);
							if (!parent.HasChildEntities)
							{
								parent.XmlNode.AppendChild(xmlElement);
							}
							else
							{
								parent.XmlNode.InsertAfter(xmlElement, wiXPackage.XmlNode);
							}
							parent.SetDirty();
							wiXProperty = new WiXProperty(this.Project.WiXModel, parent.Owner, parent, xmlElement);
							this.Project.WiXModel.SupportedEntities.Add(wiXProperty);
							flag = true;
						}
						if (flag)
						{
							this.Project.CommitXmlChanges();
							this.comments = value;
						}
					}
				}
			}

			[Category("Properties")]
			[Description("Specifies whether to check for newer versions of an application during installation.")]
			[LocDisplayName("DetectNewerInstalledVersion")]
			public bool DetectNewerInstalledVersion
			{
				get
				{
					this.Project.CheckParseDone();
					if (!this.detectNewerInstalledVersionRead)
					{
						this.detectNewerInstalledVersionRead = true;
						this.detectNewerInstalledVersion = this.GetDetectNewerUpgradeVersionElement() != null;
					}
					return this.detectNewerInstalledVersion;
				}
				set
				{
					List<WiXEntity> detectNewerCustomElements;
					XmlElement xmlElement;
					string attributeValue;
					if (this.detectNewerInstalledVersion != value)
					{
						bool flag = false;
						if (!value)
						{
							WiXEntity detectNewerUpgradeVersionElement = this.GetDetectNewerUpgradeVersionElement();
							if (detectNewerUpgradeVersionElement != null)
							{
								string str = detectNewerUpgradeVersionElement.GetAttributeValue("Property");
								detectNewerUpgradeVersionElement.Delete();
								flag = true;
								if (!string.IsNullOrEmpty(str))
								{
									detectNewerCustomElements = this.GetDetectNewerCustomElements(str);
									if (detectNewerCustomElements.Count > 0)
									{
										attributeValue = detectNewerCustomElements[0].GetAttributeValue("Action");
										foreach (WiXEntity detectNewerCustomElement in detectNewerCustomElements)
										{
											this.Project.WiXModel.SupportedEntities.Remove(detectNewerCustomElement);
											detectNewerCustomElement.Delete();
										}
										WiXEntity wiXEntity = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => {
											if (!(b is WiXCustomAction))
											{
												return false;
											}
											return b.GetAttributeValue("Id") == attributeValue;
										});
										if (wiXEntity != null)
										{
											this.Project.WiXModel.SupportedEntities.Remove(wiXEntity);
											wiXEntity.Delete();
										}
									}
								}
							}
						}
						else
						{
							WiXEntity wiXEntity1 = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXPackage);
							WiXProduct wiXProduct = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXProduct) as WiXProduct;
							if (wiXProduct != null)
							{
								WiXEntity parent = null;
								string str1 = "NEWPRODUCTFOUND";
								WiXEntity detectNewerUpgradeVersionElement1 = this.GetDetectNewerUpgradeVersionElement();
								if (detectNewerUpgradeVersionElement1 != null)
								{
									detectNewerUpgradeVersionElement1.SetAttributeValue("Minimum", wiXProduct.Version);
									detectNewerUpgradeVersionElement1.SetAttributeValue("OnlyDetect", "yes");
									detectNewerUpgradeVersionElement1.SetAttributeValue("IncludeMinimum", "no");
									detectNewerUpgradeVersionElement1.SetAttributeValue("Language", wiXProduct.Language);
									detectNewerUpgradeVersionElement1.SetDirty();
									parent = detectNewerUpgradeVersionElement1.Parent as WiXEntity;
								}
								else
								{
									parent = wiXProduct.ChildEntities.Find((WiXEntity b) => b is WiXUpgrade);
									if (parent == null)
									{
										xmlElement = Common.CreateXmlElementWithAttributes(wiXProduct.XmlNode.OwnerDocument, "Upgrade", wiXProduct.XmlNode.NamespaceURI, new string[] { "Id" }, new string[] { wiXProduct.UpgradeCode }, "", false);
										if (!wiXProduct.HasChildEntities || wiXEntity1 == null)
										{
											wiXProduct.XmlNode.AppendChild(xmlElement);
										}
										else
										{
											wiXProduct.XmlNode.InsertAfter(xmlElement, wiXEntity1.XmlNode);
										}
										wiXProduct.SetDirty();
										parent = new WiXUpgrade(this.Project.WiXModel, wiXProduct.Owner, wiXProduct, xmlElement);
										this.Project.WiXModel.SupportedEntities.Add(parent);
									}
									xmlElement = Common.CreateXmlElementWithAttributes(parent.XmlNode.OwnerDocument, "UpgradeVersion", parent.XmlNode.NamespaceURI, new string[] { "Minimum", "Property", "OnlyDetect", "IncludeMinimum", "Language" }, new string[] { wiXProduct.Version, str1, "yes", "no", wiXProduct.Language }, "", false);
									parent.XmlNode.AppendChild(xmlElement);
									parent.SetDirty();
									detectNewerUpgradeVersionElement1 = new WiXEntity(this.Project.WiXModel, parent.Owner, parent, xmlElement);
								}
								flag = true;
								if (detectNewerUpgradeVersionElement1 != null)
								{
									attributeValue = "PreventDowngrading";
									List<WiXEntity> wiXEntities = new List<WiXEntity>();
									WiXEntity wiXCustom = null;
									WiXEntity wiXCustom1 = null;
									WiXEntity wiXInstallUISequence = null;
									WiXEntity wiXInstallExecuteSequence = null;
									detectNewerCustomElements = this.GetDetectNewerCustomElements(str1);
									if (detectNewerCustomElements.Count > 0)
									{
										attributeValue = detectNewerCustomElements[0].GetAttributeValue("Action");
										foreach (WiXEntity detectNewerCustomElement1 in detectNewerCustomElements)
										{
											if (!(detectNewerCustomElement1.Parent is WiXInstallUISequence))
											{
												if (!(detectNewerCustomElement1.Parent is WiXInstallExecuteSequence))
												{
													continue;
												}
												wiXInstallExecuteSequence = detectNewerCustomElement1.Parent as WiXEntity;
												wiXCustom1 = detectNewerCustomElement1;
											}
											else
											{
												wiXInstallUISequence = detectNewerCustomElement1.Parent as WiXEntity;
												wiXCustom = detectNewerCustomElement1;
											}
										}
									}
									if (wiXInstallExecuteSequence == null)
									{
										wiXInstallExecuteSequence = wiXProduct.ChildEntities.Find((WiXEntity b) => b is WiXInstallExecuteSequence);
										if (wiXInstallExecuteSequence == null)
										{
											xmlElement = Common.CreateXmlElementWithAttributes(wiXProduct.XmlNode.OwnerDocument, "InstallExecuteSequence", wiXProduct.XmlNode.NamespaceURI, null, null, "", false);
											if (!wiXProduct.HasChildEntities || wiXEntity1 == null)
											{
												wiXProduct.XmlNode.AppendChild(xmlElement);
											}
											else
											{
												wiXProduct.XmlNode.InsertAfter(xmlElement, wiXEntity1.XmlNode);
											}
											wiXProduct.SetDirty();
											wiXInstallExecuteSequence = new WiXInstallExecuteSequence(this.Project.WiXModel, wiXProduct.Owner, wiXProduct, xmlElement);
											this.Project.WiXModel.SupportedEntities.Add(wiXInstallExecuteSequence);
											wiXEntities.Add(wiXInstallExecuteSequence);
										}
										xmlElement = Common.CreateXmlElementWithAttributes(wiXInstallExecuteSequence.XmlNode.OwnerDocument, "Custom", wiXInstallExecuteSequence.XmlNode.NamespaceURI, new string[] { "Action", "After" }, new string[] { attributeValue, "FindRelatedProducts" }, "", false);
										XmlCDataSection xmlCDataSection = xmlElement.OwnerDocument.CreateCDataSection(str1);
										xmlElement.AppendChild(xmlCDataSection);
										wiXInstallExecuteSequence.XmlNode.AppendChild(xmlElement);
										wiXInstallExecuteSequence.SetDirty();
										wiXCustom1 = new WiXCustom(this.Project.WiXModel, wiXInstallExecuteSequence.Owner, wiXInstallExecuteSequence, xmlElement);
										WiXEntity wiXEntity2 = new WiXEntity(this.Project.WiXModel, wiXCustom1.Owner, wiXCustom1, xmlCDataSection);
										this.Project.WiXModel.SupportedEntities.Add(wiXCustom1);
									}
									if (wiXInstallUISequence == null)
									{
										wiXInstallUISequence = wiXProduct.ChildEntities.Find((WiXEntity b) => b is WiXInstallUISequence);
										if (wiXInstallUISequence == null)
										{
											xmlElement = Common.CreateXmlElementWithAttributes(wiXProduct.XmlNode.OwnerDocument, "InstallUISequence", wiXProduct.XmlNode.NamespaceURI, null, null, "", false);
											if (!wiXProduct.HasChildEntities || wiXEntity1 == null)
											{
												wiXProduct.XmlNode.AppendChild(xmlElement);
											}
											else
											{
												wiXProduct.XmlNode.InsertAfter(xmlElement, wiXEntity1.XmlNode);
											}
											wiXProduct.SetDirty();
											wiXInstallUISequence = new WiXInstallUISequence(this.Project.WiXModel, wiXProduct.Owner, wiXProduct, xmlElement);
											this.Project.WiXModel.SupportedEntities.Add(wiXInstallUISequence);
											wiXEntities.Add(wiXInstallUISequence);
										}
										xmlElement = Common.CreateXmlElementWithAttributes(wiXInstallUISequence.XmlNode.OwnerDocument, "Custom", wiXInstallUISequence.XmlNode.NamespaceURI, new string[] { "Action", "After" }, new string[] { attributeValue, "FindRelatedProducts" }, "", false);
										XmlCDataSection xmlCDataSection1 = xmlElement.OwnerDocument.CreateCDataSection(str1);
										xmlElement.AppendChild(xmlCDataSection1);
										wiXInstallUISequence.XmlNode.AppendChild(xmlElement);
										wiXInstallUISequence.SetDirty();
										wiXCustom = new WiXCustom(this.Project.WiXModel, wiXInstallUISequence.Owner, wiXInstallUISequence, xmlElement);
										WiXEntity wiXEntity3 = new WiXEntity(this.Project.WiXModel, wiXCustom.Owner, wiXCustom, xmlCDataSection1);
										this.Project.WiXModel.SupportedEntities.Add(wiXCustom);
									}
									if (wiXInstallUISequence != null && this.FindRelatedProductsElem() == null)
									{
										xmlElement = Common.CreateXmlElementWithAttributes(wiXInstallUISequence.XmlNode.OwnerDocument, "FindRelatedProducts", wiXInstallUISequence.XmlNode.NamespaceURI, new string[] { "Sequence" }, new string[] { "200" }, "", false);
										wiXInstallUISequence.XmlNode.AppendChild(xmlElement);
										WiXEntity wiXEntity4 = new WiXEntity(this.Project.WiXModel, wiXInstallUISequence.Owner, wiXInstallUISequence, xmlElement);
									}
									WiXEntity wiXCustomAction = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => {
										if (!(b is WiXCustomAction))
										{
											return false;
										}
										return b.GetAttributeValue("Id") == attributeValue;
									});
									if (wiXCustomAction == null)
									{
										xmlElement = Common.CreateXmlElementWithAttributes(wiXProduct.XmlNode.OwnerDocument, "CustomAction", wiXProduct.XmlNode.NamespaceURI, new string[] { "Id", "Error" }, new string[] { attributeValue, "Newer version already installed." }, "", false);
										if (!wiXProduct.HasChildEntities || wiXEntity1 == null)
										{
											wiXProduct.XmlNode.AppendChild(xmlElement);
										}
										else
										{
											wiXProduct.XmlNode.InsertAfter(xmlElement, wiXEntity1.XmlNode);
										}
										wiXProduct.SetDirty();
										wiXCustomAction = new WiXCustomAction(this.Project.WiXModel, wiXProduct.Owner, wiXProduct, xmlElement);
										this.Project.WiXModel.SupportedEntities.Add(wiXCustomAction);
									}
									else if (string.IsNullOrEmpty(wiXCustomAction.GetAttributeValue("Error")))
									{
										wiXCustomAction.SetAttributeValue("Error", "Newer version already installed.");
										wiXCustomAction.SetDirty();
									}
								}
							}
						}
						if (flag)
						{
							this.Project.CommitXmlChanges();
							this.detectNewerInstalledVersion = value;
						}
					}
				}
			}

			[Category("Properties")]
			[Description("Specifies whether the package is installed for all users or only the installing user.")]
			[LocDisplayName("InstallAllUsers")]
			public bool InstallAllUsers
			{
				get
				{
					this.Project.CheckParseDone();
					if (!this.installAllUsersRead)
					{
						string empty = string.Empty;
						this.installAllUsersRead = true;
						if (!this.HasFolderDialog())
						{
							WiXEntity wiXEntity = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => {
								if (!(b is WiXCustomAction))
								{
									return false;
								}
								return b.GetAttributeValue("Id") == "VSDCA_AllUsers";
							});
							if (wiXEntity != null)
							{
								this.installAllUsers = wiXEntity.GetAttributeValue("Value") == "2";
							}
						}
						else
						{
							WiXEntity wiXEntity1 = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => {
								if (!(b is WiXCustomAction))
								{
									return false;
								}
								return b.GetAttributeValue("Id") == "VSDCA_FolderForm_AllUsers";
							});
							if (wiXEntity1 != null)
							{
								this.installAllUsers = wiXEntity1.GetAttributeValue("Value") == "ALL";
							}
						}
					}
					return this.installAllUsers;
				}
				set
				{
					XmlElement xmlElement;
					XmlElement xmlElement1;
					XmlElement xmlElement2;
					if (this.installAllUsers != value)
					{
						bool flag = false;
						WiXEntity wiXEntity = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXPackage);
						WiXProduct wiXProduct = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXProduct) as WiXProduct;
						if (!this.HasFolderDialog())
						{
							WiXEntity wiXCustomAction = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => {
								if (!(b is WiXCustomAction))
								{
									return false;
								}
								return b.GetAttributeValue("Id") == "VSDCA_AllUsers";
							});
							if (wiXCustomAction == null)
							{
								XmlDocument ownerDocument = wiXProduct.XmlNode.OwnerDocument;
								string namespaceURI = wiXProduct.XmlNode.NamespaceURI;
								string[] strArrays = new string[] { "Id", "Property", "Value", "Execute" };
								string[] strArrays1 = new string[] { "VSDCA_AllUsers", "ALLUSERS", null, null };
								strArrays1[2] = (value ? "2" : string.Empty);
								strArrays1[3] = "firstSequence";
								XmlElement xmlElement3 = Common.CreateXmlElementWithAttributes(ownerDocument, "CustomAction", namespaceURI, strArrays, strArrays1, "", false);
								if (!wiXProduct.HasChildEntities || wiXEntity == null)
								{
									wiXProduct.XmlNode.AppendChild(xmlElement3);
								}
								else
								{
									wiXProduct.XmlNode.InsertAfter(xmlElement3, wiXEntity.XmlNode);
								}
								wiXProduct.SetDirty();
								wiXCustomAction = new WiXCustomAction(this.Project.WiXModel, wiXProduct.Owner, wiXProduct, xmlElement3);
								this.Project.WiXModel.SupportedEntities.Add(wiXCustomAction);
								flag = true;
							}
							else
							{
								wiXCustomAction.SetAttributeValue("Value", (value ? "2" : string.Empty), true);
								wiXCustomAction.SetDirty();
								flag = true;
							}
							List<WiXEntity> wiXEntities = new List<WiXEntity>();
							WiXEntity wiXCustom = null;
							WiXEntity wiXCustom1 = null;
							WiXEntity parent = null;
							WiXEntity wiXInstallExecuteSequence = null;
							List<WiXEntity> wiXEntities1 = this.Project.WiXModel.SupportedEntities.FindAll((WiXEntity b) => {
								if (!(b is WiXCustom))
								{
									return false;
								}
								return b.GetAttributeValue("Action") == "VSDCA_AllUsers";
							});
							if (wiXEntities1.Count > 0)
							{
								foreach (WiXEntity wiXEntity1 in wiXEntities1)
								{
									if (!(wiXEntity1.Parent is WiXInstallUISequence))
									{
										if (!(wiXEntity1.Parent is WiXInstallExecuteSequence))
										{
											continue;
										}
										wiXInstallExecuteSequence = wiXEntity1.Parent as WiXEntity;
										wiXCustom1 = wiXEntity1;
									}
									else
									{
										parent = wiXEntity1.Parent as WiXEntity;
										wiXCustom = wiXEntity1;
									}
								}
							}
							if (wiXCustom1 == null)
							{
								wiXInstallExecuteSequence = wiXProduct.ChildEntities.Find((WiXEntity b) => b is WiXInstallExecuteSequence);
								if (wiXInstallExecuteSequence == null)
								{
									xmlElement1 = Common.CreateXmlElementWithAttributes(wiXProduct.XmlNode.OwnerDocument, "InstallExecuteSequence", wiXProduct.XmlNode.NamespaceURI, null, null, "", false);
									if (!wiXProduct.HasChildEntities || wiXEntity == null)
									{
										wiXProduct.XmlNode.AppendChild(xmlElement1);
									}
									else
									{
										wiXProduct.XmlNode.InsertAfter(xmlElement1, wiXEntity.XmlNode);
									}
									wiXProduct.SetDirty();
									wiXInstallExecuteSequence = new WiXInstallExecuteSequence(this.Project.WiXModel, wiXProduct.Owner, wiXProduct, xmlElement1);
									this.Project.WiXModel.SupportedEntities.Add(wiXInstallExecuteSequence);
								}
								xmlElement1 = Common.CreateXmlElementWithAttributes(wiXInstallExecuteSequence.XmlNode.OwnerDocument, "Custom", wiXInstallExecuteSequence.XmlNode.NamespaceURI, new string[] { "Action", "After" }, new string[] { "VSDCA_AllUsers", "CostInitialize" }, "", false);
								XmlCDataSection xmlCDataSection = xmlElement1.OwnerDocument.CreateCDataSection("Installed=\"\" AND NOT RESUME AND ALLUSERS=1");
								xmlElement1.AppendChild(xmlCDataSection);
								wiXInstallExecuteSequence.XmlNode.AppendChild(xmlElement1);
								wiXInstallExecuteSequence.SetDirty();
								wiXCustom1 = new WiXCustom(this.Project.WiXModel, wiXInstallExecuteSequence.Owner, wiXInstallExecuteSequence, xmlElement1);
								WiXEntity wiXEntity2 = new WiXEntity(this.Project.WiXModel, wiXCustom1.Owner, wiXCustom1, xmlCDataSection);
								this.Project.WiXModel.SupportedEntities.Add(wiXCustom1);
								flag = true;
							}
							if (wiXCustom == null)
							{
								parent = wiXProduct.ChildEntities.Find((WiXEntity b) => b is WiXInstallUISequence);
								if (parent == null)
								{
									xmlElement2 = Common.CreateXmlElementWithAttributes(wiXProduct.XmlNode.OwnerDocument, "InstallUISequence", wiXProduct.XmlNode.NamespaceURI, null, null, "", false);
									if (!wiXProduct.HasChildEntities || wiXEntity == null)
									{
										wiXProduct.XmlNode.AppendChild(xmlElement2);
									}
									else
									{
										wiXProduct.XmlNode.InsertAfter(xmlElement2, wiXEntity.XmlNode);
									}
									wiXProduct.SetDirty();
									parent = new WiXInstallUISequence(this.Project.WiXModel, wiXProduct.Owner, wiXProduct, xmlElement2);
									this.Project.WiXModel.SupportedEntities.Add(parent);
								}
								xmlElement2 = Common.CreateXmlElementWithAttributes(parent.XmlNode.OwnerDocument, "Custom", parent.XmlNode.NamespaceURI, new string[] { "Action", "After" }, new string[] { "VSDCA_AllUsers", "CostInitialize" }, "", false);
								XmlCDataSection xmlCDataSection1 = xmlElement2.OwnerDocument.CreateCDataSection("Installed=\"\" AND NOT RESUME AND ALLUSERS=1");
								xmlElement2.AppendChild(xmlCDataSection1);
								parent.XmlNode.AppendChild(xmlElement2);
								parent.SetDirty();
								wiXCustom = new WiXCustom(this.Project.WiXModel, parent.Owner, parent, xmlElement2);
								WiXEntity wiXEntity3 = new WiXEntity(this.Project.WiXModel, wiXCustom.Owner, wiXCustom, xmlCDataSection1);
								this.Project.WiXModel.SupportedEntities.Add(wiXCustom);
								flag = true;
							}
						}
						else
						{
							WiXEntity wiXCustomAction1 = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => {
								if (!(b is WiXCustomAction))
								{
									return false;
								}
								return b.GetAttributeValue("Id") == "VSDCA_FolderForm_AllUsers";
							});
							if (wiXCustomAction1 != null)
							{
								wiXCustomAction1.SetAttributeValue("Value", (value ? "ALL" : "ME"));
								wiXCustomAction1.SetDirty();
								flag = true;
							}
							else if (wiXProduct != null)
							{
								XmlDocument xmlDocument = wiXProduct.XmlNode.OwnerDocument;
								string str = wiXProduct.XmlNode.NamespaceURI;
								string[] strArrays2 = new string[] { "Id", "Property", "Value" };
								string[] strArrays3 = new string[] { "VSDCA_FolderForm_AllUsers", "FolderForm_AllUsers", null };
								strArrays3[2] = (value ? "ALL" : "ME");
								XmlElement xmlElement4 = Common.CreateXmlElementWithAttributes(xmlDocument, "CustomAction", str, strArrays2, strArrays3, "", false);
								if (!wiXProduct.HasChildEntities || wiXEntity == null)
								{
									wiXProduct.XmlNode.AppendChild(xmlElement4);
								}
								else
								{
									wiXProduct.XmlNode.InsertAfter(xmlElement4, wiXEntity.XmlNode);
								}
								wiXProduct.SetDirty();
								wiXCustomAction1 = new WiXCustomAction(this.Project.WiXModel, wiXProduct.Owner, wiXProduct, xmlElement4);
								this.Project.WiXModel.SupportedEntities.Add(wiXCustomAction1);
								flag = true;
							}
							WiXEntity wiXCustom2 = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => {
								if (!(b is WiXCustom))
								{
									return false;
								}
								return b.GetAttributeValue("Action") == "VSDCA_FolderForm_AllUsers";
							});
							if (wiXCustom2 == null)
							{
								WiXEntity wiXInstallUISequence = wiXProduct.ChildEntities.Find((WiXEntity b) => b is WiXInstallUISequence);
								if (wiXInstallUISequence == null)
								{
									xmlElement = Common.CreateXmlElementWithAttributes(wiXProduct.XmlNode.OwnerDocument, "InstallUISequence", wiXProduct.XmlNode.NamespaceURI, null, null, "", false);
									if (!wiXProduct.HasChildEntities || wiXEntity == null)
									{
										wiXProduct.XmlNode.AppendChild(xmlElement);
									}
									else
									{
										wiXProduct.XmlNode.InsertAfter(xmlElement, wiXEntity.XmlNode);
									}
									wiXProduct.SetDirty();
									wiXInstallUISequence = new WiXInstallUISequence(this.Project.WiXModel, wiXProduct.Owner, wiXProduct, xmlElement);
									this.Project.WiXModel.SupportedEntities.Add(wiXInstallUISequence);
								}
								xmlElement = Common.CreateXmlElementWithAttributes(wiXInstallUISequence.XmlNode.OwnerDocument, "Custom", wiXInstallUISequence.XmlNode.NamespaceURI, new string[] { "Action", "Sequence" }, new string[] { "VSDCA_FolderForm_AllUsers", "997" }, "", false);
								XmlCDataSection xmlCDataSection2 = xmlElement.OwnerDocument.CreateCDataSection("Installed=\"\" AND NOT RESUME AND ALLUSERS=1");
								xmlElement.AppendChild(xmlCDataSection2);
								wiXInstallUISequence.XmlNode.AppendChild(xmlElement);
								wiXInstallUISequence.SetDirty();
								wiXCustom2 = new WiXCustom(this.Project.WiXModel, wiXInstallUISequence.Owner, wiXInstallUISequence, xmlElement);
								WiXEntity wiXEntity4 = new WiXEntity(this.Project.WiXModel, wiXCustom2.Owner, wiXCustom2, xmlCDataSection2);
								this.Project.WiXModel.SupportedEntities.Add(wiXCustom2);
								flag = true;
							}
							wiXCustomAction1 = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => {
								if (!(b is WiXCustomAction))
								{
									return false;
								}
								return b.GetAttributeValue("Id") == "VSDCA_AllUsers";
							});
							if (wiXCustomAction1 != null)
							{
								this.Project.WiXModel.SupportedEntities.Remove(wiXCustomAction1);
								wiXCustomAction1.Delete();
							}
							List<WiXEntity> wiXEntities2 = this.Project.WiXModel.SupportedEntities.FindAll((WiXEntity b) => {
								if (!(b is WiXCustom))
								{
									return false;
								}
								return b.GetAttributeValue("Action") == "VSDCA_AllUsers";
							});
							if (wiXEntities2.Count > 0)
							{
								foreach (WiXEntity wiXEntity5 in wiXEntities2)
								{
									this.Project.WiXModel.SupportedEntities.Remove(wiXEntity5);
									wiXEntity5.Delete();
								}
							}
						}
						if (wiXProduct != null)
						{
							if (wiXProduct.HasChildEntities && wiXEntity != null)
							{
								if (!value)
								{
									wiXEntity.SetAttributeValue("InstallScope", "perUser");
								}
								else
								{
									wiXEntity.SetAttributeValue("InstallScope", "perMachine");
								}
								wiXEntity.SetDirty();
							}
							flag = true;
						}
						if (flag)
						{
							this.Project.CommitXmlChanges();
							this.installAllUsers = value;
							this.SetBootstrapperProperty("ApplicationRequiresElevation", (value ? "True" : "False"));
						}
					}
				}
			}

			[Category("Properties")]
			[Description("Specifies keywords used to search for an installer.")]
			[LocDisplayName("Keywords")]
			public string Keywords
			{
				get
				{
					this.Project.CheckParseDone();
					if (!this.keywordsRead)
					{
						this.keywordsRead = true;
						WiXPackage wiXPackage = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXPackage) as WiXPackage;
						if (wiXPackage != null)
						{
							this.keywords = wiXPackage.GetAttributeValue("Keywords");
						}
					}
					return this.keywords;
				}
				set
				{
					if (this.keywords != value)
					{
						bool flag = false;
						WiXPackage wiXPackage = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXPackage) as WiXPackage;
						if (wiXPackage != null)
						{
							wiXPackage.SetAttributeValue("Keywords", value);
							wiXPackage.SetDirty();
							flag = true;
						}
						if (flag)
						{
							this.Project.CommitXmlChanges();
							this.keywords = value;
						}
					}
				}
			}

			public List<string> Languages
			{
				get
				{
					string[] strArrays;
					int i;
					if (!this.languagesRead)
					{
						this.languagesRead = true;
						this.languages.Clear();
						if (!this.project.IsMultiLangSupported)
						{
							WiXPackage wiXPackage = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXPackage) as WiXPackage;
							if (wiXPackage != null)
							{
								string attributeValue = wiXPackage.GetAttributeValue("Languages");
								if (!string.IsNullOrEmpty(attributeValue))
								{
									string[] strArrays1 = attributeValue.Split(new char[] { ';', ',' });
									if (strArrays1.Length != 0)
									{
										strArrays = strArrays1;
										for (i = 0; i < (int)strArrays.Length; i++)
										{
											string str = strArrays[i];
											this.languages.Add(str);
										}
									}
								}
							}
						}
						else
						{
							string buildProjectProperty = this.project.GetBuildProjectProperty("SupportedUILanguages");
							if (!string.IsNullOrEmpty(buildProjectProperty))
							{
								string[] strArrays2 = buildProjectProperty.Split(new char[] { ';', ',' });
								if (strArrays2.Length != 0)
								{
									strArrays = strArrays2;
									for (i = 0; i < (int)strArrays.Length; i++)
									{
										string str1 = strArrays[i];
										this.languages.Add(str1);
									}
								}
							}
						}
					}
					return this.languages;
				}
				set
				{
					int num;
					if (this.languages != value)
					{
						this.languages.Clear();
						CultureInfo currentCulture = CultureInfo.CurrentCulture;
						foreach (string str in value)
						{
							if (!int.TryParse(str, out num))
							{
								continue;
							}
							if (num != currentCulture.LCID || this.languages.Count <= 0)
							{
								this.languages.Add(str);
							}
							else
							{
								this.languages.Insert(0, str);
							}
						}
						if (this.project.IsMultiLangSupported)
						{
							bool flag = false;
							bool flag1 = true;
							string empty = string.Empty;
							string buildProjectProperty = this.Project.GetBuildProjectProperty("SupportedUILanguages");
							string lCID = this.LCID;
							if (ProjectUtilities.IsNeutralLCID(lCID))
							{
								flag1 = false;
							}
							foreach (string language in this.languages)
							{
								if (!string.IsNullOrEmpty(empty))
								{
									empty = string.Concat(empty, ",");
								}
								empty = string.Concat(empty, language);
								if (!language.Equals(lCID))
								{
									continue;
								}
								flag1 = false;
							}
							if (buildProjectProperty != empty)
							{
								this.Project.SetSupportedLanguages(empty);
								WiXPackage wiXPackage = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXPackage) as WiXPackage;
								if (wiXPackage != null)
								{
									if (!ProjectUtilities.IsNeutralLCID(lCID) && !this.languages.Contains(lCID) && !flag1)
									{
										if (!string.IsNullOrEmpty(empty))
										{
											empty = string.Concat(empty, ",");
										}
										empty = string.Concat(empty, lCID);
									}
									if (empty.Contains("1033") && !empty.StartsWith("1033"))
									{
										empty = string.Concat(empty.Substring(0, empty.IndexOf("1033")), empty.Substring(empty.IndexOf("1033") + 4));
										empty = string.Concat("1033,", empty);
										empty = empty.Replace(",,", ",");
									}
									empty = (string.IsNullOrEmpty(empty) ? "0".ToString() : string.Concat("0".ToString(), ",", empty));
									wiXPackage.SetAttributeValue("Languages", empty);
									wiXPackage.SetDirty();
								}
								flag = true;
							}
							if (flag)
							{
								this.Project.CommitXmlChanges();
								this.UpdateLangFiles(null, this.LCID);
								if (flag1)
								{
									this.lcidRead = false;
									this.localizationRead = false;
									this.Localization = "0";
								}
							}
						}
					}
				}
			}

			[Category("Properties")]
			[DefaultValue(null)]
			[Description("Specifies the list of languages supported in the package.")]
			[Editor(typeof(InstallerLanguagesEditor), typeof(UITypeEditor))]
			[LocDisplayName("Languages")]
			[RefreshProperties(RefreshProperties.Repaint)]
			public string LanguagesDummy
			{
				get
				{
					this.Project.CheckParseDone();
					if (this.Languages.Count > 0)
					{
						return "(Languages)";
					}
					return "(None)";
				}
				set
				{
					this.languagesRead = false;
				}
			}

			internal string LCID
			{
				get
				{
					if (!this.lcidRead)
					{
						this.lcidRead = true;
						if (!this.project.IsMultiLangSupported)
						{
							WiXPackage wiXPackage = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXPackage) as WiXPackage;
							if (wiXPackage != null)
							{
								WiXEntity parent = wiXPackage.Parent as WiXEntity;
								if (parent != null)
								{
									this.lcid = parent.GetAttributeValue("Language");
								}
							}
						}
						else
						{
							this.lcid = this.project.GetBuildProjectProperty("CurrentUILanguage");
						}
					}
					return this.lcid;
				}
			}

			[Category("Properties")]
			[Description("Specifies the localization for string resources and the run-time user interface.")]
			[Editor(typeof(InstallerLocalizationEditor), typeof(UITypeEditor))]
			[LocDisplayName("Localization")]
			[RefreshProperties(RefreshProperties.Repaint)]
			public string Localization
			{
				get
				{
					int num;
					this.Project.CheckParseDone();
					if (!this.localizationRead)
					{
						this.localizationRead = true;
						string attributeValue = "0";
						if (!this.project.IsMultiLangSupported)
						{
							WiXPackage wiXPackage = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXPackage) as WiXPackage;
							if (wiXPackage != null)
							{
								WiXEntity parent = wiXPackage.Parent as WiXEntity;
								if (parent != null)
								{
									attributeValue = parent.GetAttributeValue("Language");
								}
							}
						}
						else
						{
							attributeValue = this.project.GetBuildProjectProperty("CurrentUILanguage");
						}
						if (string.IsNullOrEmpty(attributeValue))
						{
							this.localization = "Neutral";
						}
						else if (int.TryParse(attributeValue, out num))
						{
							try
							{
								if (!ProjectUtilities.IsNeutralLCID(num))
								{
									this.localization = ProjectUtilities.GetCultureInfo(num).EnglishName;
								}
								else
								{
									this.localization = "Neutral";
								}
							}
							catch (Exception exception)
							{
							}
						}
					}
					return this.localization;
				}
				set
				{
					int num;
					string str;
					string str1;
					if (!string.IsNullOrEmpty(value))
					{
						string englishName = value;
						CultureInfo cultureInfo = null;
						if (!int.TryParse(englishName, out num))
						{
							try
							{
								if (!value.Equals("neutral", StringComparison.OrdinalIgnoreCase))
								{
									cultureInfo = ProjectUtilities.GetCultureInfo(value);
									englishName = cultureInfo.EnglishName;
								}
								else
								{
									cultureInfo = CultureInfo.InvariantCulture;
									englishName = "Neutral";
								}
							}
							catch (Exception exception)
							{
								englishName = this.localization;
							}
						}
						else
						{
							try
							{
								if (!ProjectUtilities.IsNeutralLCID(num))
								{
									cultureInfo = ProjectUtilities.GetCultureInfo(num);
									englishName = (cultureInfo.LCID != CultureInfo.InvariantCulture.LCID ? cultureInfo.EnglishName : "Neutral");
								}
								else
								{
									cultureInfo = CultureInfo.InvariantCulture;
									englishName = "Neutral";
								}
							}
							catch (Exception exception1)
							{
								englishName = this.localization;
							}
						}
						if (this.localization != englishName)
						{
							bool flag = false;
							string lCID = this.LCID;
							if (!this.project.IsMultiLangSupported)
							{
								WiXPackage wiXPackage = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXPackage) as WiXPackage;
								if (wiXPackage != null)
								{
									WiXEntity parent = wiXPackage.Parent as WiXEntity;
									if (parent != null && cultureInfo != null)
									{
										if (cultureInfo.LCID != CultureInfo.InvariantCulture.LCID)
										{
											str1 = cultureInfo.LCID.ToString();
											parent.SetAttributeValue("Language", str1);
										}
										else
										{
											str1 = "0";
											parent.SetAttributeValue("Language", "0");
										}
										parent.SetAttributeValue("Codepage", cultureInfo.TextInfo.ANSICodePage.ToString());
										parent.SetDirty();
										foreach (WiXEntity upgradeVersionElement in this.GetUpgradeVersionElements())
										{
											upgradeVersionElement.SetAttributeValue("Language", str1);
											upgradeVersionElement.SetDirty();
										}
										flag = true;
									}
								}
							}
							else
							{
								str = (cultureInfo.LCID != CultureInfo.InvariantCulture.LCID ? cultureInfo.LCID.ToString() : "0");
								this.project.SetCurrentLocalization(str);
								flag = true;
								List<string> strs = new List<string>(this.Languages);
								if (str == "0")
								{
									this.UpdateLangFiles(null, "0");
								}
								else if (!strs.Contains(str))
								{
									strs.Add(str);
									this.lcidRead = false;
									this.languagesRead = false;
									this.Languages = strs;
								}
								WiXPackage wiXPackage1 = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXPackage) as WiXPackage;
								if (wiXPackage1 != null)
								{
									WiXEntity wiXEntity = wiXPackage1.Parent as WiXEntity;
									if (wiXEntity != null && cultureInfo != null)
									{
										if (wiXEntity.GetAttributeValue("Language") != "!(loc.ID)")
										{
											wiXEntity.SetAttributeValue("Language", "!(loc.ID)");
											wiXEntity.SetDirty();
										}
										if (!string.IsNullOrEmpty(wiXEntity.GetAttributeValue("Codepage")))
										{
											wiXEntity.SetAttributeValue("Codepage", null);
											wiXEntity.SetDirty();
										}
										foreach (WiXEntity upgradeVersionElement1 in this.GetUpgradeVersionElements())
										{
											if (upgradeVersionElement1.GetAttributeValue("Language") == "!(loc.ID)")
											{
												continue;
											}
											upgradeVersionElement1.SetAttributeValue("Language", "!(loc.ID)");
											wiXEntity.SetDirty();
										}
									}
								}
							}
							if (flag)
							{
								this.Project.CommitXmlChanges();
								this.localization = englishName;
								this.lcidRead = false;
								if (cultureInfo != null)
								{
									if (cultureInfo.LCID == CultureInfo.InvariantCulture.LCID)
									{
										this.SetBootstrapperProperty("Culture", "en-US");
										return;
									}
									this.SetBootstrapperProperty("Culture", cultureInfo.Name);
								}
							}
						}
					}
				}
			}

			[Category("Properties")]
			[Description("Specifies the name of the manufacturer of an application or component.")]
			[LocDisplayName("Manufacturer")]
			public string Manufacturer
			{
				get
				{
					this.Project.CheckParseDone();
					if (!this.manufacturerRead)
					{
						this.manufacturerRead = true;
						WiXProduct wiXProduct = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXProduct) as WiXProduct;
						if (wiXProduct != null)
						{
							this.manufacturer = wiXProduct.Manufacturer;
						}
					}
					return this.manufacturer;
				}
				set
				{
					if (!string.IsNullOrEmpty(value) && this.manufacturer != value)
					{
						bool flag = false;
						WiXProduct wiXProduct = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXProduct) as WiXProduct;
						if (wiXProduct != null)
						{
							wiXProduct.Manufacturer = value;
							flag = true;
						}
						if (flag)
						{
							this.Project.CommitXmlChanges();
							this.manufacturer = value;
						}
					}
				}
			}

			[Category("Properties")]
			[Description("Specifies a URL for a Web site containing information about the manufacturer of an application or component.")]
			[LocDisplayName("ManufacturerUrl")]
			public string ManufacturerUrl
			{
				get
				{
					this.Project.CheckParseDone();
					if (!this.manufacturerUrlRead)
					{
						this.manufacturerUrlRead = true;
						WiXEntity wiXEntity = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => {
							if (!(b is WiXProperty))
							{
								return false;
							}
							return b.GetAttributeValue("Id") == "ARPHELPLINK";
						});
						if (wiXEntity != null)
						{
							this.manufacturerUrl = wiXEntity.GetAttributeValue("Value");
						}
					}
					return this.manufacturerUrl;
				}
				set
				{
					if (this.manufacturerUrl != value)
					{
						bool flag = false;
						WiXEntity wiXProperty = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => {
							if (!(b is WiXProperty))
							{
								return false;
							}
							return b.GetAttributeValue("Id") == "ARPHELPLINK";
						});
						if (wiXProperty != null)
						{
							if (string.IsNullOrEmpty(value))
							{
								this.Project.WiXModel.SupportedEntities.Remove(wiXProperty);
								wiXProperty.Delete();
							}
							else
							{
								wiXProperty.SetAttributeValue("Value", value);
								wiXProperty.SetDirty();
							}
							flag = true;
						}
						else if (!string.IsNullOrEmpty(value))
						{
							WiXPackage wiXPackage = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXPackage) as WiXPackage;
							if (wiXPackage != null)
							{
								WiXEntity parent = wiXPackage.Parent as WiXEntity;
								XmlElement xmlElement = Common.CreateXmlElementWithAttributes(parent.XmlNode.OwnerDocument, "Property", parent.XmlNode.NamespaceURI, new string[] { "Id", "Value" }, new string[] { "ARPHELPLINK", value }, "", false);
								if (!parent.HasChildEntities)
								{
									parent.XmlNode.AppendChild(xmlElement);
								}
								else
								{
									parent.XmlNode.InsertAfter(xmlElement, wiXPackage.XmlNode);
								}
								parent.SetDirty();
								wiXProperty = new WiXProperty(this.Project.WiXModel, parent.Owner, parent, xmlElement);
								this.Project.WiXModel.SupportedEntities.Add(wiXProperty);
								flag = true;
							}
						}
						if (flag)
						{
							this.Project.CommitXmlChanges();
							this.manufacturerUrl = value;
						}
					}
				}
			}

			[Category("Properties")]
			[Description("Specifies a unique identifier for a merge module.")]
			[Editor(typeof(InstallerNewCodeEditor), typeof(UITypeEditor))]
			[LocDisplayName("ModuleSignature")]
			public string ModuleSignature
			{
				get
				{
					this.Project.CheckParseDone();
					if (!this.moduleSignatureRead)
					{
						this.moduleSignatureRead = true;
						WiXPackage wiXPackage = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXPackage) as WiXPackage;
						if (wiXPackage != null)
						{
							WiXEntity parent = wiXPackage.Parent as WiXEntity;
							if (parent != null)
							{
								this.moduleSignature = parent.GetAttributeValue("Id");
							}
						}
					}
					return this.moduleSignature;
				}
				set
				{
					if (!string.IsNullOrEmpty(value) && this.moduleSignature != value)
					{
						bool flag = false;
						WiXPackage wiXPackage = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXPackage) as WiXPackage;
						if (wiXPackage != null)
						{
							WiXEntity parent = wiXPackage.Parent as WiXEntity;
							if (parent != null)
							{
								parent.SetAttributeValue("Id", value);
								parent.SetDirty();
								flag = true;
							}
						}
						if (flag)
						{
							this.Project.CommitXmlChanges();
							this.moduleSignature = value;
						}
					}
				}
			}

			public List<string> MyPrerequisites
			{
				get
				{
					if (!this.myPrerequisitesRead)
					{
						this.myPrerequisitesRead = true;
						this.myPrerequisites.Clear();
						try
						{
							this.myPrerequisites = PrerequisitesForm.GetPrerequisites(Path.GetFileNameWithoutExtension(this.Project.VsProject.FullName), this.Project.RootDirectory, this.Project.VsDTE.Version);
						}
						catch (Exception exception)
						{
						}
					}
					return this.myPrerequisites;
				}
				set
				{
					if (value != null && !value.Equals(this.myPrerequisites))
					{
						try
						{
							try
							{
								this.myPrerequisites = value;
							}
							catch (Exception exception)
							{
							}
						}
						finally
						{
							this.myPrerequisitesRead = false;
						}
					}
				}
			}

			[Category("Properties")]
			[DefaultValue(null)]
			[Description("Opens the My Prerequisites Dialog Box, where you develop your own prerequisite components.")]
			[Editor(typeof(InstallerMyPrerequisitesEditor), typeof(UITypeEditor))]
			[LocDisplayName("MyPrerequisites")]
			public string MyPrerequisitesDummy
			{
				get
				{
					if (this.MyPrerequisites.Count > 0)
					{
						return "(MyPrerequisites)";
					}
					return "(None)";
				}
				set
				{
				}
			}

			[Category("Properties")]
			[Description("Specifies any commands to execute after the build ends.")]
			[LocDisplayName("PostBuildEvent")]
			public string PostBuildEvent
			{
				get
				{
					return this.postBuildEvent;
				}
				set
				{
					this.postBuildEvent = value;
				}
			}

			[Category("Properties")]
			[Description("Specifies any commands to execute before the build starts.")]
			[LocDisplayName("PreBuildEvent")]
			public string PreBuildEvent
			{
				get
				{
					return this.preBuildEvent;
				}
				set
				{
					this.preBuildEvent = value;
				}
			}

			public Dictionary<string, Dictionary<string, string>> Prerequisites
			{
				get
				{
					string configurationName = this.Project.VsProject.ConfigurationManager.ActiveConfiguration.ConfigurationName;
					configurationName = string.Concat(configurationName, "|", this.Project.VsProject.ConfigurationManager.ActiveConfiguration.PlatformName);
					if (this.prerequisitesConfigName != configurationName)
					{
						this.prerequisitesConfigName = configurationName;
						this.prerequisitesRead = false;
					}
					if (!this.prerequisitesRead)
					{
						this.prerequisitesRead = true;
						this.prerequisites.Clear();
						try
						{
							object obj = this.Project.VsProject.Object;
							PropertyInfo property = obj.GetType().GetProperty("BuildProject", BindingFlags.Instance | BindingFlags.NonPublic);
							if (property != null)
							{
								object value = property.GetValue(obj, null);
								if (value is Microsoft.Build.Evaluation.Project)
								{
									ProjectTargetInstance item = ((Microsoft.Build.Evaluation.Project)value).Targets["AfterBuild"];
									if (item != null)
									{
										foreach (ProjectTargetInstanceChild child in item.Children)
										{
											if (this.ExtractConfiguration(child.Condition) != configurationName)
											{
												continue;
											}
											if (!(child is ProjectItemGroupTaskInstance))
											{
												if (!(child is ProjectTaskInstance))
												{
													continue;
												}
												ProjectTaskInstance projectTaskInstance = child as ProjectTaskInstance;
												if (projectTaskInstance.Name != "GenerateBootstrapper")
												{
													continue;
												}
												Dictionary<string, string> strs = new Dictionary<string, string>(projectTaskInstance.Parameters);
												this.prerequisites["GenerateBootstrapper"] = strs;
											}
											else
											{
												foreach (ProjectItemGroupTaskItemInstance projectItemGroupTaskItemInstance in (child as ProjectItemGroupTaskInstance).Items)
												{
													if (projectItemGroupTaskItemInstance.ItemType != "BootstrapperFile")
													{
														continue;
													}
													Dictionary<string, string> strs1 = new Dictionary<string, string>();
													foreach (ProjectItemGroupTaskMetadataInstance metadatum in projectItemGroupTaskItemInstance.Metadata)
													{
														strs1[metadatum.Name] = metadatum.Value;
													}
													this.prerequisites[projectItemGroupTaskItemInstance.Include] = strs1;
												}
											}
										}
									}
								}
							}
						}
						catch (Exception exception)
						{
						}
					}
					return this.prerequisites;
				}
				set
				{
					ProjectTaskElement projectTaskElement;
					if (value != null && !value.Equals(this.prerequisites))
					{
						bool flag = false;
						Microsoft.Build.Evaluation.Project project = null;
						try
						{
							object obj = this.Project.VsProject.Object;
							string configurationName = this.Project.VsProject.ConfigurationManager.ActiveConfiguration.ConfigurationName;
							configurationName = string.Concat(configurationName, "|", this.Project.VsProject.ConfigurationManager.ActiveConfiguration.PlatformName);
							PropertyInfo property = obj.GetType().GetProperty("BuildProject", BindingFlags.Instance | BindingFlags.NonPublic);
							if (property != null)
							{
								object obj1 = property.GetValue(obj, null);
								if (obj1 is Microsoft.Build.Evaluation.Project)
								{
									project = (Microsoft.Build.Evaluation.Project)obj1;
									ProjectTargetElement afterBuildTarget = this.GetAfterBuildTarget(project);
									if (afterBuildTarget != null)
									{
										List<ProjectElement> projectElements = new List<ProjectElement>();
										foreach (ProjectElement child in afterBuildTarget.Children)
										{
											if (this.ExtractConfiguration(child.Condition) != configurationName)
											{
												if (!string.IsNullOrEmpty(child.Condition) || !(child is ProjectTaskElement))
												{
													continue;
												}
												projectTaskElement = child as ProjectTaskElement;
												if (projectTaskElement.Name != "GenerateBootstrapper")
												{
													continue;
												}
												projectElements.Add(projectTaskElement);
											}
											else if (!(child is ProjectItemGroupElement))
											{
												if (!(child is ProjectTaskElement))
												{
													continue;
												}
												projectTaskElement = child as ProjectTaskElement;
												if (projectTaskElement.Name != "GenerateBootstrapper")
												{
													continue;
												}
												projectElements.Add(projectTaskElement);
											}
											else
											{
												ProjectItemGroupElement projectItemGroupElement = child as ProjectItemGroupElement;
												List<ProjectItemElement> projectItemElements = new List<ProjectItemElement>();
												foreach (ProjectItemElement item in projectItemGroupElement.Items)
												{
													if (item.ItemType != "BootstrapperFile")
													{
														continue;
													}
													projectItemElements.Add(item);
												}
												foreach (ProjectItemElement projectItemElement in projectItemElements)
												{
													projectItemGroupElement.RemoveChild(projectItemElement);
													flag = true;
												}
												projectElements.Add(projectItemGroupElement);
											}
										}
										ProjectItemGroupElement projectItemGroupElement1 = null;
										if (projectElements.Count > 0)
										{
											foreach (ProjectElement projectElement in projectElements)
											{
												if (!(projectElement is ProjectItemGroupElement))
												{
													if (!(projectElement is ProjectTaskElement))
													{
														continue;
													}
													afterBuildTarget.RemoveChild(projectElement);
													flag = true;
												}
												else
												{
													projectItemGroupElement1 = projectElement as ProjectItemGroupElement;
												}
											}
										}
										if (value.Count > 0)
										{
											flag = true;
											if (projectItemGroupElement1 == null)
											{
												projectItemGroupElement1 = afterBuildTarget.AddItemGroup();
												projectItemGroupElement1.Condition = string.Concat(" '$(Configuration)|$(Platform)' == '", configurationName, "' ");
												flag = true;
											}
											foreach (KeyValuePair<string, Dictionary<string, string>> keyValuePair in value)
											{
												if (keyValuePair.Key != "GenerateBootstrapper")
												{
													projectItemGroupElement1.AddItem("BootstrapperFile", keyValuePair.Key, keyValuePair.Value);
												}
												else
												{
													projectTaskElement = afterBuildTarget.AddTask("GenerateBootstrapper");
													foreach (KeyValuePair<string, string> keyValuePair1 in keyValuePair.Value)
													{
														if (keyValuePair1.Key != "Condition")
														{
															projectTaskElement.SetParameter(keyValuePair1.Key, keyValuePair1.Value);
														}
														else
														{
															projectTaskElement.Condition = keyValuePair1.Value;
														}
													}
												}
											}
										}
									}
								}
							}
						}
						catch (Exception exception)
						{
							flag = false;
						}
						if (flag)
						{
							try
							{
								if (project != null)
								{
									project.MarkDirty();
									project.ReevaluateIfNecessary();
									if (!this.Project.MakeProjectDirty())
									{
										this.Project.VsProject.Save("");
									}
								}
							}
							finally
							{
								this.prerequisitesRead = false;
							}
						}
					}
				}
			}

			[Category("Properties")]
			[DefaultValue(null)]
			[Description("Opens the Prerequisites Dialog Box, where you specify which prerequisite components to install, and specify the locations from which your application and the prerequisite components will be installed.")]
			[Editor(typeof(InstallerPrerequisitesEditor), typeof(UITypeEditor))]
			[LocDisplayName("Prerequisites")]
			public string PrerequisitesDummy
			{
				get
				{
					if (this.Prerequisites.Count > 0)
					{
						return "(Prerequisites)";
					}
					return "(None)";
				}
				set
				{
				}
			}

			[Category("Properties")]
			[Description("Specifies a unique identifier for an application.")]
			[Editor(typeof(InstallerNewCodeEditor), typeof(UITypeEditor))]
			[LocDisplayName("ProductCode")]
			public string ProductCode
			{
				get
				{
					this.Project.CheckParseDone();
					if (!this.productCodeRead)
					{
						this.productCodeRead = true;
						WiXProduct wiXProduct = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXProduct) as WiXProduct;
						if (wiXProduct != null)
						{
							this.productCode = wiXProduct.Id;
						}
					}
					return this.productCode;
				}
				set
				{
					if (!string.IsNullOrEmpty(value) && this.productCode != value)
					{
						bool flag = false;
						WiXProduct wiXProduct = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXProduct) as WiXProduct;
						if (wiXProduct != null)
						{
							wiXProduct.Id = value;
							flag = true;
						}
						if (flag)
						{
							this.Project.CommitXmlChanges();
							this.productCode = value;
						}
					}
				}
			}

			[Category("Properties")]
			[Description("Specifies a public name that describes an application or component.")]
			[LocDisplayName("ProductName")]
			public string ProductName
			{
				get
				{
					this.Project.CheckParseDone();
					if (!this.productNameRead)
					{
						this.productNameRead = true;
						WiXProduct wiXProduct = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXProduct) as WiXProduct;
						if (wiXProduct != null)
						{
							this.productName = wiXProduct.Name;
						}
					}
					return this.productName;
				}
				set
				{
					if (!string.IsNullOrEmpty(value) && this.productName != value)
					{
						bool flag = false;
						WiXProduct wiXProduct = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXProduct) as WiXProduct;
						if (wiXProduct != null)
						{
							wiXProduct.Name = value;
							flag = true;
						}
						if (flag)
						{
							this.Project.CommitXmlChanges();
							this.productName = value;
							this.SetBootstrapperProperty("ApplicationName", value);
						}
					}
				}
			}

			internal VsWiXProject Project
			{
				get
				{
					return this.project;
				}
			}

			[Category("Properties")]
			[Description("Specifies whether an installer will remove previous versions of an application during installation.")]
			[LocDisplayName("RemovePreviousVersions")]
			public bool RemovePreviousVersions
			{
				get
				{
					this.Project.CheckParseDone();
					if (!this.removePreviousVersionsRead)
					{
						this.removePreviousVersionsRead = true;
						this.removePreviousVersions = this.GetRemovePreviousVersionsElement() != null;
					}
					return this.removePreviousVersions;
				}
				set
				{
					XmlElement xmlElement;
					if (this.removePreviousVersions != value)
					{
						bool flag = false;
						if (!value)
						{
							WiXEntity removePreviousVersionsElement = this.GetRemovePreviousVersionsElement();
							if (removePreviousVersionsElement != null)
							{
								removePreviousVersionsElement.Delete();
								flag = true;
								WiXEntity removeExistingProductsElement = this.GetRemoveExistingProductsElement();
								if (removeExistingProductsElement != null)
								{
									removeExistingProductsElement.Delete();
								}
							}
						}
						else
						{
							WiXEntity wiXEntity = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXPackage);
							WiXProduct wiXProduct = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXProduct) as WiXProduct;
							if (wiXProduct != null)
							{
								WiXEntity parent = null;
								string str = "OLDPRODUCTFOUND";
								WiXEntity removePreviousVersionsElement1 = this.GetRemovePreviousVersionsElement();
								if (removePreviousVersionsElement1 != null)
								{
									removePreviousVersionsElement1.SetAttributeValue("Maximum", wiXProduct.Version);
									removePreviousVersionsElement1.SetAttributeValue("OnlyDetect", "no");
									removePreviousVersionsElement1.SetAttributeValue("IncludeMinimum", "yes");
									removePreviousVersionsElement1.SetAttributeValue("IncludeMaximum", "no");
									removePreviousVersionsElement1.SetAttributeValue("Language", wiXProduct.Language);
									removePreviousVersionsElement1.SetDirty();
									parent = removePreviousVersionsElement1.Parent as WiXEntity;
								}
								else
								{
									parent = wiXProduct.ChildEntities.Find((WiXEntity b) => b is WiXUpgrade);
									if (parent == null)
									{
										xmlElement = Common.CreateXmlElementWithAttributes(wiXProduct.XmlNode.OwnerDocument, "Upgrade", wiXProduct.XmlNode.NamespaceURI, new string[] { "Id" }, new string[] { wiXProduct.UpgradeCode }, "", false);
										if (!wiXProduct.HasChildEntities || wiXEntity == null)
										{
											wiXProduct.XmlNode.AppendChild(xmlElement);
										}
										else
										{
											wiXProduct.XmlNode.InsertAfter(xmlElement, wiXEntity.XmlNode);
										}
										wiXProduct.SetDirty();
										parent = new WiXUpgrade(this.Project.WiXModel, wiXProduct.Owner, wiXProduct, xmlElement);
										this.Project.WiXModel.SupportedEntities.Add(parent);
									}
									xmlElement = Common.CreateXmlElementWithAttributes(parent.XmlNode.OwnerDocument, "UpgradeVersion", parent.XmlNode.NamespaceURI, new string[] { "Maximum", "Property", "OnlyDetect", "IncludeMinimum", "IncludeMaximum", "Language" }, new string[] { wiXProduct.Version, str, "no", "yes", "no", wiXProduct.Language }, "", false);
									parent.XmlNode.AppendChild(xmlElement);
									parent.SetDirty();
									removePreviousVersionsElement1 = new WiXEntity(this.Project.WiXModel, parent.Owner, parent, xmlElement);
								}
								flag = true;
								if (removePreviousVersionsElement1 != null && this.GetRemoveExistingProductsElement() == null && this.GetMajorUpgradeElement(wiXProduct) == null)
								{
									WiXEntity wiXInstallExecuteSequence = wiXProduct.ChildEntities.Find((WiXEntity b) => b is WiXInstallExecuteSequence);
									if (wiXInstallExecuteSequence == null)
									{
										xmlElement = Common.CreateXmlElementWithAttributes(wiXProduct.XmlNode.OwnerDocument, "InstallExecuteSequence", wiXProduct.XmlNode.NamespaceURI, null, null, "", false);
										if (!wiXProduct.HasChildEntities || wiXEntity == null)
										{
											wiXProduct.XmlNode.AppendChild(xmlElement);
										}
										else
										{
											wiXProduct.XmlNode.InsertAfter(xmlElement, wiXEntity.XmlNode);
										}
										wiXProduct.SetDirty();
										wiXInstallExecuteSequence = new WiXInstallExecuteSequence(this.Project.WiXModel, wiXProduct.Owner, wiXProduct, xmlElement);
										this.Project.WiXModel.SupportedEntities.Add(wiXInstallExecuteSequence);
									}
									xmlElement = Common.CreateXmlElementWithAttributes(wiXInstallExecuteSequence.XmlNode.OwnerDocument, "RemoveExistingProducts", wiXInstallExecuteSequence.XmlNode.NamespaceURI, new string[] { "Before" }, new string[] { "InstallInitialize" }, "", false);
									wiXInstallExecuteSequence.XmlNode.AppendChild(xmlElement);
									wiXInstallExecuteSequence.SetDirty();
									WiXCustom wiXCustom = new WiXCustom(this.Project.WiXModel, wiXInstallExecuteSequence.Owner, wiXInstallExecuteSequence, xmlElement);
								}
							}
						}
						if (flag)
						{
							this.Project.CommitXmlChanges();
							this.removePreviousVersions = value;
						}
					}
					this.removePreviousVersions = value;
				}
			}

			public string RootBootstrapperDir
			{
				get
				{
					string empty = string.Empty;
					string configurationName = this.Project.VsProject.ConfigurationManager.ActiveConfiguration.ConfigurationName;
					configurationName = string.Concat(configurationName, "|", this.Project.VsProject.ConfigurationManager.ActiveConfiguration.PlatformName);
					try
					{
						object obj = this.Project.VsProject.Object;
						PropertyInfo property = obj.GetType().GetProperty("BuildProject", BindingFlags.Instance | BindingFlags.NonPublic);
						if (property != null)
						{
							object value = property.GetValue(obj, null);
							if (value is Microsoft.Build.Evaluation.Project)
							{
								ProjectTargetInstance item = ((Microsoft.Build.Evaluation.Project)value).Targets["AfterBuild"];
								if (item != null)
								{
									foreach (ProjectTargetInstanceChild child in item.Children)
									{
										if (!(this.ExtractConfiguration(child.Condition) == configurationName) || !(child is ProjectTaskInstance))
										{
											continue;
										}
										ProjectTaskInstance projectTaskInstance = child as ProjectTaskInstance;
										if (projectTaskInstance.Name != "GenerateBootstrapper")
										{
											continue;
										}
										Dictionary<string, string> strs = new Dictionary<string, string>(projectTaskInstance.Parameters);
										if (!strs.ContainsKey("Path"))
										{
											continue;
										}
										empty = strs["Path"];
									}
								}
							}
						}
					}
					catch (Exception exception)
					{
					}
					return empty;
				}
			}

			[Category("Properties")]
			[Description("Specifies the condition under which the post-build event runs.")]
			[LocDisplayName("RunPostBuildEvent")]
			public string RunPostBuildEvent
			{
				get
				{
					return this.runPostBuildEvent;
				}
				set
				{
					this.runPostBuildEvent = value;
				}
			}

			[Category("Properties")]
			[Description("Specifies additional information describing an application or component.")]
			[LocDisplayName("Subject")]
			public string Subject
			{
				get
				{
					this.Project.CheckParseDone();
					if (!this.subjectRead)
					{
						this.subjectRead = true;
						WiXEntity wiXEntity = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => {
							if (!(b is WiXProperty))
							{
								return false;
							}
							return b.GetAttributeValue("Id") == "ARPREADME";
						});
						if (wiXEntity != null)
						{
							this.subject = wiXEntity.GetAttributeValue("Value");
						}
					}
					return this.subject;
				}
				set
				{
					if (this.subject != value)
					{
						bool flag = false;
						WiXEntity wiXProperty = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => {
							if (!(b is WiXProperty))
							{
								return false;
							}
							return b.GetAttributeValue("Id") == "ARPREADME";
						});
						if (wiXProperty != null)
						{
							if (string.IsNullOrEmpty(value))
							{
								this.Project.WiXModel.SupportedEntities.Remove(wiXProperty);
								wiXProperty.Delete();
							}
							else
							{
								wiXProperty.SetAttributeValue("Value", value);
								wiXProperty.SetDirty();
							}
							flag = true;
						}
						else if (!string.IsNullOrEmpty(value))
						{
							WiXPackage wiXPackage = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXPackage) as WiXPackage;
							if (wiXPackage != null)
							{
								WiXEntity parent = wiXPackage.Parent as WiXEntity;
								XmlElement xmlElement = Common.CreateXmlElementWithAttributes(parent.XmlNode.OwnerDocument, "Property", parent.XmlNode.NamespaceURI, new string[] { "Id", "Value" }, new string[] { "ARPREADME", value }, "", false);
								if (!parent.HasChildEntities)
								{
									parent.XmlNode.AppendChild(xmlElement);
								}
								else
								{
									parent.XmlNode.InsertAfter(xmlElement, wiXPackage.XmlNode);
								}
								parent.SetDirty();
								wiXProperty = new WiXProperty(this.Project.WiXModel, parent.Owner, parent, xmlElement);
								this.Project.WiXModel.SupportedEntities.Add(wiXProperty);
								flag = true;
							}
						}
						if (flag)
						{
							this.Project.CommitXmlChanges();
							this.subject = value;
						}
					}
				}
			}

			[Category("Properties")]
			[Description("Specifies a phone number for support information for an application or component.")]
			[LocDisplayName("SupportPhone")]
			public string SupportPhone
			{
				get
				{
					this.Project.CheckParseDone();
					if (!this.supportPhoneRead)
					{
						this.supportPhoneRead = true;
						WiXEntity wiXEntity = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => {
							if (!(b is WiXProperty))
							{
								return false;
							}
							return b.GetAttributeValue("Id") == "ARPHELPTELEPHONE";
						});
						if (wiXEntity != null)
						{
							this.supportPhone = wiXEntity.GetAttributeValue("Value");
						}
					}
					return this.supportPhone;
				}
				set
				{
					if (this.supportPhone != value)
					{
						bool flag = false;
						WiXEntity wiXProperty = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => {
							if (!(b is WiXProperty))
							{
								return false;
							}
							return b.GetAttributeValue("Id") == "ARPHELPTELEPHONE";
						});
						if (wiXProperty != null)
						{
							if (string.IsNullOrEmpty(value))
							{
								this.Project.WiXModel.SupportedEntities.Remove(wiXProperty);
								wiXProperty.Delete();
							}
							else
							{
								wiXProperty.SetAttributeValue("Value", value);
								wiXProperty.SetDirty();
							}
							flag = true;
						}
						else if (!string.IsNullOrEmpty(value))
						{
							WiXPackage wiXPackage = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXPackage) as WiXPackage;
							if (wiXPackage != null)
							{
								WiXEntity parent = wiXPackage.Parent as WiXEntity;
								XmlElement xmlElement = Common.CreateXmlElementWithAttributes(parent.XmlNode.OwnerDocument, "Property", parent.XmlNode.NamespaceURI, new string[] { "Id", "Value" }, new string[] { "ARPHELPTELEPHONE", value }, "", false);
								if (!parent.HasChildEntities)
								{
									parent.XmlNode.AppendChild(xmlElement);
								}
								else
								{
									parent.XmlNode.InsertAfter(xmlElement, wiXPackage.XmlNode);
								}
								parent.SetDirty();
								wiXProperty = new WiXProperty(this.Project.WiXModel, parent.Owner, parent, xmlElement);
								this.Project.WiXModel.SupportedEntities.Add(wiXProperty);
								flag = true;
							}
						}
						if (flag)
						{
							this.Project.CommitXmlChanges();
							this.supportPhone = value;
						}
					}
				}
			}

			[Category("Properties")]
			[Description("Specifies a URL for a Web site containing support information for an application or component.")]
			[LocDisplayName("SupportUrl")]
			public string SupportUrl
			{
				get
				{
					this.Project.CheckParseDone();
					if (!this.supportUrlRead)
					{
						this.supportUrlRead = true;
						WiXEntity wiXEntity = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => {
							if (!(b is WiXProperty))
							{
								return false;
							}
							return b.GetAttributeValue("Id") == "ARPURLINFOABOUT";
						});
						if (wiXEntity != null)
						{
							this.supportUrl = wiXEntity.GetAttributeValue("Value");
						}
					}
					return this.supportUrl;
				}
				set
				{
					if (this.supportUrl != value)
					{
						bool flag = false;
						WiXEntity wiXProperty = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => {
							if (!(b is WiXProperty))
							{
								return false;
							}
							return b.GetAttributeValue("Id") == "ARPURLINFOABOUT";
						});
						if (wiXProperty != null)
						{
							if (string.IsNullOrEmpty(value))
							{
								this.Project.WiXModel.SupportedEntities.Remove(wiXProperty);
								wiXProperty.Delete();
							}
							else
							{
								wiXProperty.SetAttributeValue("Value", value);
								wiXProperty.SetDirty();
							}
							flag = true;
						}
						else if (!string.IsNullOrEmpty(value))
						{
							WiXPackage wiXPackage = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXPackage) as WiXPackage;
							if (wiXPackage != null)
							{
								WiXEntity parent = wiXPackage.Parent as WiXEntity;
								XmlElement xmlElement = Common.CreateXmlElementWithAttributes(parent.XmlNode.OwnerDocument, "Property", parent.XmlNode.NamespaceURI, new string[] { "Id", "Value" }, new string[] { "ARPURLINFOABOUT", value }, "", false);
								if (!parent.HasChildEntities)
								{
									parent.XmlNode.AppendChild(xmlElement);
								}
								else
								{
									parent.XmlNode.InsertAfter(xmlElement, wiXPackage.XmlNode);
								}
								parent.SetDirty();
								wiXProperty = new WiXProperty(this.Project.WiXModel, parent.Owner, parent, xmlElement);
								this.Project.WiXModel.SupportedEntities.Add(wiXProperty);
								flag = true;
							}
						}
						if (flag)
						{
							this.Project.CommitXmlChanges();
							this.supportUrl = value;
						}
					}
				}
			}

			[Category("Properties")]
			[Description("Specifies the target platform of the installer.")]
			[LocDisplayName("TargetPlatform")]
			public WiXSupportedPlatforms TargetPlatform
			{
				get
				{
					this.Project.CheckParseDone();
					if (!this.targetPlatformRead)
					{
						this.targetPlatformRead = true;
						WiXPackage wiXPackage = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXPackage) as WiXPackage;
						if (wiXPackage != null)
						{
							string platforms = wiXPackage.Platforms;
							if (platforms != null)
							{
								if (platforms == "x86")
								{
									this.targetPlatform = WiXSupportedPlatforms.x86;
								}
								else if (platforms == "ia64")
								{
									this.targetPlatform = WiXSupportedPlatforms.ia64;
								}
								else if (platforms == "x64")
								{
									this.targetPlatform = WiXSupportedPlatforms.x64;
								}
								else if (platforms == "arm")
								{
									this.targetPlatform = WiXSupportedPlatforms.arm;
								}
								else if (platforms == "intel")
								{
									this.targetPlatform = WiXSupportedPlatforms.intel;
								}
								else if (platforms == "intel64")
								{
									this.targetPlatform = WiXSupportedPlatforms.intel64;
								}
							}
						}
					}
					return this.targetPlatform;
				}
				set
				{
					if (this.targetPlatform != value)
					{
						bool flag = false;
						WiXPackage wiXPackage = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXPackage) as WiXPackage;
						if (wiXPackage != null)
						{
							switch (value)
							{
								case WiXSupportedPlatforms.x86:
								{
									wiXPackage.Platforms = "x86";
									break;
								}
								case WiXSupportedPlatforms.ia64:
								{
									wiXPackage.Platforms = "ia64";
									break;
								}
								case WiXSupportedPlatforms.x64:
								{
									wiXPackage.Platforms = "x64";
									break;
								}
								case WiXSupportedPlatforms.arm:
								{
									wiXPackage.Platforms = "arm";
									break;
								}
								case WiXSupportedPlatforms.intel:
								{
									wiXPackage.Platforms = "intel";
									break;
								}
								case WiXSupportedPlatforms.intel64:
								{
									wiXPackage.Platforms = "intel64";
									break;
								}
							}
							flag = true;
						}
						if (flag)
						{
							this.Project.CommitXmlChanges();
							this.targetPlatform = value;
						}
					}
					this.targetPlatform = value;
				}
			}

			[Category("Properties")]
			[Description("Specifies the free-form description for an installer.")]
			[LocDisplayName("Title")]
			public string Title
			{
				get
				{
					this.Project.CheckParseDone();
					if (!this.descriptionRead)
					{
						this.descriptionRead = true;
						WiXPackage wiXPackage = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXPackage) as WiXPackage;
						if (wiXPackage != null)
						{
							this.description = wiXPackage.GetAttributeValue("Description");
						}
					}
					return this.description;
				}
				set
				{
					if (this.description != value)
					{
						bool flag = false;
						WiXPackage wiXPackage = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXPackage) as WiXPackage;
						if (wiXPackage != null)
						{
							wiXPackage.SetAttributeValue("Description", value);
							wiXPackage.SetDirty();
							flag = true;
						}
						if (flag)
						{
							this.Project.CommitXmlChanges();
							this.description = value;
						}
					}
				}
			}

			[Category("Properties")]
			[Description("Specifies a URL for the link used to update information on the application or component.")]
			[LocDisplayName("UpdateUrl")]
			public string UpdateUrl
			{
				get
				{
					this.Project.CheckParseDone();
					if (!this.updateUrlRead)
					{
						this.updateUrlRead = true;
						WiXEntity wiXEntity = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => {
							if (!(b is WiXProperty))
							{
								return false;
							}
							return b.GetAttributeValue("Id") == "ARPURLUPDATEINFO";
						});
						if (wiXEntity != null)
						{
							this.updateUrl = wiXEntity.GetAttributeValue("Value");
						}
					}
					return this.updateUrl;
				}
				set
				{
					if (this.updateUrl != value)
					{
						bool flag = false;
						WiXEntity wiXProperty = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => {
							if (!(b is WiXProperty))
							{
								return false;
							}
							return b.GetAttributeValue("Id") == "ARPURLUPDATEINFO";
						});
						if (wiXProperty != null)
						{
							if (string.IsNullOrEmpty(value))
							{
								this.Project.WiXModel.SupportedEntities.Remove(wiXProperty);
								wiXProperty.Delete();
							}
							else
							{
								wiXProperty.SetAttributeValue("Value", value);
								wiXProperty.SetDirty();
							}
							flag = true;
						}
						else if (!string.IsNullOrEmpty(value))
						{
							WiXPackage wiXPackage = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXPackage) as WiXPackage;
							if (wiXPackage != null)
							{
								WiXEntity parent = wiXPackage.Parent as WiXEntity;
								XmlElement xmlElement = Common.CreateXmlElementWithAttributes(parent.XmlNode.OwnerDocument, "Property", parent.XmlNode.NamespaceURI, new string[] { "Id", "Value" }, new string[] { "ARPURLUPDATEINFO", value }, "", false);
								if (!parent.HasChildEntities)
								{
									parent.XmlNode.AppendChild(xmlElement);
								}
								else
								{
									parent.XmlNode.InsertAfter(xmlElement, wiXPackage.XmlNode);
								}
								parent.SetDirty();
								wiXProperty = new WiXProperty(this.Project.WiXModel, parent.Owner, parent, xmlElement);
								this.Project.WiXModel.SupportedEntities.Add(wiXProperty);
								flag = true;
							}
						}
						if (flag)
						{
							this.Project.CommitXmlChanges();
							this.updateUrl = value;
						}
					}
				}
			}

			[Category("Properties")]
			[Description("Specifies a shared identifier that represents multiple versions of an application.")]
			[Editor(typeof(InstallerNewCodeEditor), typeof(UITypeEditor))]
			[LocDisplayName("UpgradeCode")]
			public string UpgradeCode
			{
				get
				{
					this.Project.CheckParseDone();
					if (!this.upgradeCodeRead)
					{
						this.upgradeCodeRead = true;
						WiXProduct wiXProduct = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXProduct) as WiXProduct;
						if (wiXProduct != null)
						{
							this.upgradeCode = wiXProduct.UpgradeCode;
						}
					}
					return this.upgradeCode;
				}
				set
				{
					if (!string.IsNullOrEmpty(value) && this.upgradeCode != value)
					{
						bool flag = false;
						WiXProduct wiXProduct = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXProduct) as WiXProduct;
						if (wiXProduct != null)
						{
							wiXProduct.UpgradeCode = value;
							flag = true;
						}
						if (!string.IsNullOrEmpty(this.upgradeCode))
						{
							List<WiXEntity> wiXEntities = this.Project.WiXModel.SupportedEntities.FindAll((WiXEntity b) => {
								if (!(b is WiXUpgrade))
								{
									return false;
								}
								return b.GetAttributeValue("Id") == this.upgradeCode;
							});
							if (wiXEntities.Count > 0)
							{
								foreach (WiXEntity wiXEntity in wiXEntities)
								{
									wiXEntity.SetAttributeValue("Id", value);
									wiXEntity.SetDirty();
								}
								flag = true;
							}
						}
						if (flag)
						{
							this.Project.CommitXmlChanges();
							this.upgradeCode = value;
						}
					}
				}
			}

			[Category("Properties")]
			[Description("Specifies the version number of an installer, merge module, or .cab file.")]
			[LocDisplayName("Version")]
			public string Version
			{
				get
				{
					this.Project.CheckParseDone();
					if (!this.versionRead)
					{
						this.versionRead = true;
						WiXPackage wiXPackage = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXPackage) as WiXPackage;
						if (wiXPackage != null)
						{
							WiXEntity parent = wiXPackage.Parent as WiXEntity;
							if (parent != null)
							{
								this.version = parent.GetAttributeValue("Version");
							}
						}
					}
					return this.version;
				}
				set
				{
					if (!string.IsNullOrEmpty(value) && this.version != value)
					{
						bool flag = false;
						WiXEntity detectNewerUpgradeVersionElement = this.GetDetectNewerUpgradeVersionElement();
						if (detectNewerUpgradeVersionElement != null)
						{
							detectNewerUpgradeVersionElement.SetAttributeValue("Minimum", value);
							detectNewerUpgradeVersionElement.SetDirty();
							flag = true;
						}
						WiXEntity removePreviousVersionsElement = this.GetRemovePreviousVersionsElement();
						if (removePreviousVersionsElement != null)
						{
							removePreviousVersionsElement.SetAttributeValue("Maximum", value);
							removePreviousVersionsElement.SetDirty();
							flag = true;
						}
						WiXPackage wiXPackage = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXPackage) as WiXPackage;
						if (wiXPackage != null)
						{
							WiXEntity parent = wiXPackage.Parent as WiXEntity;
							if (parent != null)
							{
								parent.SetAttributeValue("Version", value);
								parent.SetDirty();
								flag = true;
							}
						}
						if (flag)
						{
							this.Project.CommitXmlChanges();
							this.version = value;
						}
					}
				}
			}

			internal object WiXPropertiesObject
			{
				get
				{
					if (this.wixPropertiesObject == null)
					{
						if (this.Project.Hierarchy != null)
						{
							this.Project.Hierarchy.GetProperty(-2, -2018, out this.wixPropertiesObject);
						}
						if (this.wixPropertiesObject == null && this.Project.VsProject != null)
						{
							this.wixPropertiesObject = this.Project.VsProject.Object.GetType().InvokeMember("NodeProperties", BindingFlags.GetProperty, null, this.Project.VsProject.Object, null);
						}
					}
					return this.wixPropertiesObject;
				}
				set
				{
					this.wixPropertiesObject = value;
				}
			}

			internal ProjectPropertiesObject(VsWiXProject project)
			{
				this.project = project;
			}

			protected override void Dispose(bool disposing)
			{
				base.Dispose(disposing);
			}

			private string ExtractConfiguration(string conditionText)
			{
				string empty = string.Empty;
				if (!string.IsNullOrEmpty(conditionText))
				{
					int num = conditionText.IndexOf("==");
					if (num != -1)
					{
						empty = conditionText.Substring(num + 2);
						empty = empty.Replace("'", string.Empty).Trim();
					}
				}
				return empty;
			}

			private WiXEntity FindRelatedProductsElem()
			{
				WiXEntity wiXEntity;
				List<WiXEntity>.Enumerator enumerator = this.Project.WiXModel.SupportedEntities.FindAll((WiXEntity b) => b is WiXInstallUISequence).GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						WiXEntity current = enumerator.Current;
						if (!current.HasChildEntities)
						{
							continue;
						}
						WiXEntity wiXEntity1 = current.ChildEntities.Find((WiXEntity e) => e.Name == "FindRelatedProducts");
						if (wiXEntity1 == null)
						{
							continue;
						}
						wiXEntity = wiXEntity1;
						return wiXEntity;
					}
					return null;
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				return wiXEntity;
			}

			private ProjectTargetElement GetAfterBuildTarget(Microsoft.Build.Evaluation.Project vsBuildProject)
			{
				ProjectTargetElement projectTargetElement = null;
				if (vsBuildProject != null)
				{
					foreach (ProjectTargetElement target in vsBuildProject.Xml.Targets)
					{
						if (string.IsNullOrEmpty(target.Name) || !target.Name.Equals("AfterBuild", StringComparison.InvariantCultureIgnoreCase) || !string.IsNullOrEmpty(target.Condition))
						{
							continue;
						}
						projectTargetElement = target;
						goto Label0;
					}
				Label0:
					if (projectTargetElement == null)
					{
						projectTargetElement = vsBuildProject.Xml.AddTarget("AfterBuild");
					}
				}
				return projectTargetElement;
			}

			internal string GetCulture()
			{
				int num;
				string name;
				string lCID = this.LCID;
				if (!string.IsNullOrEmpty(lCID) && int.TryParse(lCID, out num))
				{
					try
					{
						if (ProjectUtilities.IsNeutralLCID(num))
						{
							return null;
						}
						else
						{
							name = ProjectUtilities.GetCultureInfo(num).Name;
						}
					}
					catch (Exception exception)
					{
						return null;
					}
					return name;
				}
				return null;
			}

			private List<WiXEntity> GetDetectNewerCustomElements(string propName)
			{
				string empty = string.Empty;
				List<WiXEntity> wiXEntities = new List<WiXEntity>();
				foreach (WiXEntity wiXEntity in this.Project.WiXModel.SupportedEntities.FindAll((WiXEntity b) => {
					if (!(b is WiXCustom))
					{
						return false;
					}
					if (b.Parent is WiXInstallExecuteSequence)
					{
						return true;
					}
					return b.Parent is WiXInstallUISequence;
				}))
				{
					empty = null;
					if (wiXEntity.FirstChild != null)
					{
						XmlNode xmlNode = ((WiXEntity)wiXEntity.FirstChild).XmlNode;
						if (xmlNode.NodeType == XmlNodeType.Text)
						{
							empty = ((XmlText)xmlNode).Value;
						}
						else if (xmlNode.NodeType == XmlNodeType.CDATA)
						{
							empty = ((XmlCDataSection)xmlNode).Value;
						}
					}
					if (string.IsNullOrEmpty(empty) || !empty.Contains(propName))
					{
						continue;
					}
					wiXEntities.Add(wiXEntity);
				}
				return wiXEntities;
			}

			private WiXEntity GetDetectNewerUpgradeVersionElement()
			{
				WiXEntity wiXEntity = null;
				WiXProduct wiXProduct = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXProduct) as WiXProduct;
				if (wiXProduct != null)
				{
					foreach (WiXEntity wiXEntity1 in this.Project.WiXModel.SupportedEntities.FindAll((WiXEntity b) => b is WiXUpgrade))
					{
						if (!wiXEntity1.HasChildEntities || !(wiXEntity1.GetAttributeValue("Id") == wiXProduct.UpgradeCode))
						{
							continue;
						}
						foreach (WiXEntity childEntity in wiXEntity1.ChildEntities)
						{
							if (!(childEntity.Name == "UpgradeVersion") || !(childEntity.GetAttributeValue("Property") == "NEWPRODUCTFOUND"))
							{
								continue;
							}
							wiXEntity = childEntity;
							if (wiXEntity == null)
							{
								continue;
							}
							return wiXEntity;
						}
						if (wiXEntity == null)
						{
							continue;
						}
						return wiXEntity;
					}
				}
				return wiXEntity;
			}

			private WiXEntity GetMajorUpgradeElement(WiXEntity productElem)
			{
				WiXEntity wiXEntity;
				List<WiXEntity>.Enumerator enumerator = productElem.ChildEntities.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						WiXEntity current = enumerator.Current;
						if (current.Name != "MajorUpgrade")
						{
							continue;
						}
						wiXEntity = current;
						return wiXEntity;
					}
					return null;
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				return wiXEntity;
			}

			private string GetPackageLanguages(WiXPackage packageElem, string oldLang, string newLang)
			{
				string languages = packageElem.Languages;
				if (string.IsNullOrEmpty(languages))
				{
					if (newLang == string.Empty)
					{
						return null;
					}
					return newLang;
				}
				string str = ",";
				if (languages.Contains(";"))
				{
					str = ";";
				}
				List<string> strs = new List<string>(languages.Split(new char[] { ';', ',' }));
				if (!string.IsNullOrEmpty(oldLang))
				{
					int num = strs.IndexOf(oldLang);
					if (num != -1)
					{
						strs.RemoveAt(num);
						string str1 = null;
						foreach (string str2 in strs)
						{
							if (!string.IsNullOrEmpty(str1))
							{
								str1 = string.Concat(str1, str);
							}
							str1 = string.Concat(str1, str2);
						}
						languages = str1;
					}
				}
				if (!string.IsNullOrEmpty(newLang))
				{
					languages = (!string.IsNullOrEmpty(languages) ? string.Concat(languages, str, newLang) : newLang);
				}
				return languages;
			}

			private WiXEntity GetRemoveExistingProductsElement()
			{
				WiXEntity wiXEntity;
				List<WiXEntity>.Enumerator enumerator = this.Project.WiXModel.SupportedEntities.FindAll((WiXEntity b) => b is WiXInstallExecuteSequence).GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						WiXEntity current = enumerator.Current;
						if (!current.HasChildEntities)
						{
							continue;
						}
						List<WiXEntity>.Enumerator enumerator1 = current.ChildEntities.GetEnumerator();
						try
						{
							while (enumerator1.MoveNext())
							{
								WiXEntity current1 = enumerator1.Current;
								if (current1.Name != "RemoveExistingProducts")
								{
									continue;
								}
								wiXEntity = current1;
								return wiXEntity;
							}
						}
						finally
						{
							((IDisposable)enumerator1).Dispose();
						}
					}
					return null;
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				return wiXEntity;
			}

			private WiXEntity GetRemovePreviousVersionsElement()
			{
				WiXEntity wiXEntity = null;
				WiXProduct wiXProduct = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => b is WiXProduct) as WiXProduct;
				if (wiXProduct != null)
				{
					foreach (WiXEntity wiXEntity1 in this.Project.WiXModel.SupportedEntities.FindAll((WiXEntity b) => b is WiXUpgrade))
					{
						if (!wiXEntity1.HasChildEntities || !(wiXEntity1.GetAttributeValue("Id") == wiXProduct.UpgradeCode))
						{
							continue;
						}
						foreach (WiXEntity childEntity in wiXEntity1.ChildEntities)
						{
							if (!(childEntity.Name == "UpgradeVersion") || !(childEntity.GetAttributeValue("Property") == "OLDPRODUCTFOUND"))
							{
								continue;
							}
							wiXEntity = childEntity;
							if (wiXEntity == null)
							{
								continue;
							}
							return wiXEntity;
						}
						if (wiXEntity == null)
						{
							continue;
						}
						return wiXEntity;
					}
				}
				return wiXEntity;
			}

			private List<WiXEntity> GetUpgradeVersionElements()
			{
				List<WiXEntity> wiXEntities = new List<WiXEntity>();
				foreach (WiXEntity wiXEntity in this.Project.WiXModel.SupportedEntities.FindAll((WiXEntity b) => b is WiXUpgrade))
				{
					if (!wiXEntity.HasChildEntities)
					{
						continue;
					}
					foreach (WiXEntity childEntity in wiXEntity.ChildEntities)
					{
						string attributeValue = childEntity.GetAttributeValue("Property");
						if (!(childEntity.Name == "UpgradeVersion") || !(attributeValue == "NEWPRODUCTFOUND") && !(attributeValue == "OLDPRODUCTFOUND"))
						{
							continue;
						}
						wiXEntities.Add(childEntity);
					}
				}
				return wiXEntities;
			}

			private bool HasFolderDialog()
			{
				WiXEntity wiXEntity = this.Project.WiXModel.SupportedEntities.Find((WiXEntity b) => {
					if (!(b is WiXDialog))
					{
						return false;
					}
					return b.GetAttributeValue("Id") == "FolderForm";
				});
				if (wiXEntity != null)
				{
					List<WiXEntity> wiXEntities = wiXEntity.ChildEntities.FindAll((WiXEntity e) => {
						string attributeValue = e.GetAttributeValue("Id");
						if (e.Name != "Control")
						{
							return false;
						}
						if (attributeValue == "BannerBmp")
						{
							return true;
						}
						return attributeValue == "AllUsersText";
					});
					if (wiXEntities != null && wiXEntities.Count == 2)
					{
						wiXEntities = wiXEntity.Parent.ChildEntities.FindAll((WiXEntity e) => {
							string attributeValue = e.GetAttributeValue("Id");
							string str = e.GetAttributeValue("Property");
							if (!(e.Name == "Property") && !(e.Name == "RadioButtonGroup"))
							{
								return false;
							}
							if (attributeValue == "FolderForm_AllUsers" || attributeValue == "FolderForm_AllUsersVisible")
							{
								return true;
							}
							return str == "FolderForm_AllUsers";
						});
						if (wiXEntities != null && wiXEntities.Count == 3)
						{
							WiXControl nextButton = VSDialogBase.GetNextButton(wiXEntity.ChildEntities);
							WiXControl prevButton = VSDialogBase.GetPrevButton(wiXEntity.ChildEntities);
							if (nextButton != null && prevButton != null)
							{
								return true;
							}
						}
					}
				}
				return false;
			}

			internal void ResetReadFlags()
			{
				this.addRemoveProgramsIconRead = false;
				this.authorRead = false;
				this.descriptionRead = false;
				this.detectNewerInstalledVersionRead = false;
				this.installAllUsersRead = false;
				this.keywordsRead = false;
				this.localizationRead = false;
				this.lcidRead = false;
				this.languagesRead = false;
				this.manufacturerRead = false;
				this.manufacturerUrlRead = false;
				this.productCodeRead = false;
				this.productNameRead = false;
				this.removePreviousVersionsRead = false;
				this.subjectRead = false;
				this.supportPhoneRead = false;
				this.supportUrlRead = false;
				this.commentsRead = false;
				this.updateUrlRead = false;
				this.targetPlatformRead = false;
				this.upgradeCodeRead = false;
				this.versionRead = false;
				this.allowModificationRead = false;
				this.allowRemovingRead = false;
				this.allowRepairRead = false;
				this.moduleSignatureRead = false;
				this.addRemoveProgramsIcon = null;
				this.author = string.Empty;
				this.description = string.Empty;
				this.detectNewerInstalledVersion = false;
				this.installAllUsers = false;
				this.keywords = string.Empty;
				this.localization = string.Empty;
				this.languages.Clear();
				this.lcid = string.Empty;
				this.manufacturer = string.Empty;
				this.manufacturerUrl = string.Empty;
				this.productCode = string.Empty;
				this.productName = string.Empty;
				this.removePreviousVersions = false;
				this.subject = string.Empty;
				this.supportPhone = string.Empty;
				this.supportUrl = string.Empty;
				this.comments = string.Empty;
				this.updateUrl = string.Empty;
				this.targetPlatform = WiXSupportedPlatforms.x86;
				this.upgradeCode = string.Empty;
				this.version = string.Empty;
				this.allowModification = true;
				this.allowRemoving = true;
				this.allowRepair = true;
				this.moduleSignature = string.Empty;
			}

			private void SetBootstrapperProperty(string propName, string propValue)
			{
				bool flag = false;
				Microsoft.Build.Evaluation.Project project = null;
				try
				{
					object obj = this.Project.VsProject.Object;
					PropertyInfo property = obj.GetType().GetProperty("BuildProject", BindingFlags.Instance | BindingFlags.NonPublic);
					if (property != null)
					{
						object value = property.GetValue(obj, null);
						if (value is Microsoft.Build.Evaluation.Project)
						{
							project = (Microsoft.Build.Evaluation.Project)value;
							ProjectTargetElement afterBuildTarget = this.GetAfterBuildTarget(project);
							if (afterBuildTarget != null)
							{
								List<ProjectElement> projectElements = new List<ProjectElement>();
								foreach (ProjectElement child in afterBuildTarget.Children)
								{
									if (!(child is ProjectTaskElement))
									{
										continue;
									}
									ProjectTaskElement projectTaskElement = child as ProjectTaskElement;
									if (projectTaskElement.Name != "GenerateBootstrapper")
									{
										continue;
									}
									projectTaskElement.SetParameter(propName, propValue);
									flag = true;
								}
							}
						}
					}
				}
				catch (Exception exception)
				{
					flag = false;
				}
				if (flag && project != null)
				{
					project.MarkDirty();
					project.ReevaluateIfNecessary();
					if (!this.Project.MakeProjectDirty())
					{
						this.Project.VsProject.Save("");
					}
				}
			}

			internal void SetLocalization(string langID)
			{
				this.lcidRead = false;
				this.localizationRead = false;
				this.localization = string.Empty;
				this.Localization = langID;
			}

			private void UpdateLangFiles(CultureInfo newCulture, string currentLCID)
			{
				string str;
				if (this.Project.IsMultiLangSupported)
				{
					string str1 = Path.Combine(this.Project.RootDirectory, "Resources");
					if (!Directory.Exists(str1))
					{
						try
						{
							Directory.CreateDirectory(str1);
						}
						catch (Exception exception)
						{
						}
					}
					str = (!VsPackage.CurrentInstance.IsVSIXPackage ? Path.GetFullPath(Path.Combine(VsPackage.CurrentInstance.GetLocation(), "..\\..\\Redistributables")) : Path.Combine(VsPackage.CurrentInstance.GetLocation(), "Redistributables"));
					if (Directory.Exists(str))
					{
						string str2 = Path.Combine(str, "vdwtool.exe");
						string str3 = Path.Combine(str1, "vdwtool.exe");
						if (File.Exists(str2) && !File.Exists(str3))
						{
							try
							{
								File.Copy(str2, str3);
							}
							catch (Exception exception1)
							{
							}
						}
					}
					if (this.Project.SetMultiLangPostBuildEvent(this.LCID, this.languages))
					{
						this.Project.UpdateLocUIFiles(newCulture, currentLCID, this.languages, true);
					}
				}
			}
		}

		[ComVisible(false)]
		public class ReferenceDescriptor : IDisposable
		{
			private bool disposed;

			private bool xsltOutputsSupported;

			internal VsWiXProject Parent;

			public ProjectDescriptor ReferencedProject;

			public string Caption;

			public uint ID;

			public string ProjectOutputXMLFilePath;

			public XmlDocument ProjectOutputXMLDocument;

			public const string WiXReferenceContainerPropName = "GetReferenceContainer";

			public const string WiXReferenceEnumReferencesMethodName = "EnumReferences";

			public const string WiXReferenceHarvestPropName = "Harvest";

			public const string WiXReferenceCaptionPropName = "Caption";

			public const string WiXReferenceIdPropName = "ID";

			public const string WiXReferenceUrlPropName = "Url";

			public const string WiXNodeFromItemIdMethodName = "NodeFromItemId";

			public const string WiXReferenceOutputGroupsPropName = "RefProjectOutputGroups";

			public const string WiXReferenceProjectObjectPropName = "ReferencedProjectObject";

			public string DirectoryIDPrefix
			{
				get
				{
					return string.Concat(this.Caption, ".");
				}
			}

			public bool Harvest
			{
				get
				{
					object referenceNode = this.GetReferenceNode();
					if (referenceNode != null)
					{
						PropertyInfo property = referenceNode.GetType().GetProperty("Harvest", BindingFlags.Instance | BindingFlags.Public);
						if (property != null)
						{
							return (bool)property.GetValue(referenceNode, null);
						}
					}
					return false;
				}
				set
				{
					object referenceNode = this.GetReferenceNode();
					if (referenceNode != null)
					{
						PropertyInfo property = referenceNode.GetType().GetProperty("Harvest", BindingFlags.Instance | BindingFlags.Public);
						if (property != null)
						{
							property.SetValue(referenceNode, value, null);
						}
					}
				}
			}

			public bool IsXSLTOutputsSupported
			{
				get
				{
					return this.xsltOutputsSupported;
				}
				set
				{
					this.xsltOutputsSupported = value;
					if (!this.xsltOutputsSupported)
					{
						this.ProjectOutputXMLFilePath = string.Empty;
						if (this.ProjectOutputXMLDocument.DocumentElement != null)
						{
							this.ProjectOutputXMLDocument.RemoveAll();
						}
					}
				}
			}

			public AddinExpress.Installer.WiXDesigner.OutputGroup[] ProjectOutputGroups
			{
				get
				{
					return this.StringToOutputGroups(this.ProjectOutputGroupsText);
				}
				set
				{
					this.ProjectOutputGroupsText = this.OutputGroupsToString(value);
				}
			}

			public string ProjectOutputGroupsText
			{
				get
				{
					object referenceNode = this.GetReferenceNode();
					if (referenceNode != null)
					{
						PropertyInfo property = referenceNode.GetType().GetProperty("RefProjectOutputGroups", BindingFlags.Instance | BindingFlags.Public);
						if (property != null)
						{
							return (string)property.GetValue(referenceNode, null);
						}
					}
					return null;
				}
				set
				{
					object referenceNode = this.GetReferenceNode();
					if (referenceNode != null)
					{
						PropertyInfo property = referenceNode.GetType().GetProperty("RefProjectOutputGroups", BindingFlags.Instance | BindingFlags.Public);
						if (property != null)
						{
							property.SetValue(referenceNode, value, null);
						}
					}
				}
			}

			public string SharedFileId
			{
				get
				{
					if (this.xsltOutputsSupported && this.ProjectOutputXMLDocument.DocumentElement != null)
					{
						XmlAttribute itemOf = this.ProjectOutputXMLDocument.DocumentElement.Attributes["SharedFileId"];
						if (itemOf != null)
						{
							return itemOf.Value;
						}
					}
					return string.Empty;
				}
			}

			public string VariableName
			{
				get
				{
					if (this.xsltOutputsSupported && this.ProjectOutputXMLDocument.DocumentElement != null)
					{
						XmlAttribute itemOf = this.ProjectOutputXMLDocument.DocumentElement.Attributes["ProjectDirVar"] ?? this.ProjectOutputXMLDocument.DocumentElement.Attributes["TargetDirVar"];
						if (itemOf != null && !string.IsNullOrEmpty(itemOf.Value))
						{
							string[] strArrays = itemOf.Value.Split(new char[] { '.' });
							if ((int)strArrays.Length > 1)
							{
								return strArrays[1];
							}
						}
					}
					return string.Empty;
				}
			}

			public ReferenceDescriptor(VsWiXProject parent, IVsProject referencedProject, string caption, uint id)
			{
				this.Parent = parent;
				this.ReferencedProject = new ProjectDescriptor(referencedProject, parent, this);
				this.Caption = caption;
				this.ID = id;
			}

			private XmlElement CreateXmlElement(XmlElement parentElem, string name, bool createIfNotExists, bool forceCreate)
			{
				XmlElement item = parentElem[name];
				if (item == null & createIfNotExists | forceCreate)
				{
					item = this.ProjectOutputXMLDocument.CreateElement(name);
					parentElem.AppendChild(item);
				}
				return item;
			}

			public void Dispose()
			{
				if (!this.disposed)
				{
					this.disposed = true;
					if (this.ReferencedProject != null)
					{
						this.ReferencedProject.Dispose();
						this.ReferencedProject = null;
					}
				}
				GC.SuppressFinalize(this);
			}

			~ReferenceDescriptor()
			{
				this.Dispose();
			}

			private string GetChromeOutputPath(string fullPath, string configuration)
			{
				bool flag = false;
				XmlTextReader xmlTextReader = null;
				string empty = string.Empty;
				try
				{
					try
					{
						xmlTextReader = new XmlTextReader(fullPath);
						xmlTextReader.MoveToContent();
						xmlTextReader.Read();
						while (!xmlTextReader.EOF)
						{
							xmlTextReader.Read();
							xmlTextReader.MoveToContent();
							if (xmlTextReader.NodeType != XmlNodeType.Element)
							{
								continue;
							}
							if (!xmlTextReader.Name.Equals("PropertyGroup", StringComparison.InvariantCultureIgnoreCase))
							{
								if (!flag || !xmlTextReader.Name.Equals("OutputPath", StringComparison.InvariantCultureIgnoreCase) || xmlTextReader.IsEmptyElement)
								{
									continue;
								}
								return xmlTextReader.ReadElementString();
							}
							else
							{
								flag = false;
								if (!xmlTextReader.HasAttributes || xmlTextReader.GetAttribute("Condition").IndexOf(configuration, StringComparison.InvariantCultureIgnoreCase) <= 0)
								{
									continue;
								}
								flag = true;
							}
						}
					}
					catch
					{
					}
				}
				finally
				{
					if (xmlTextReader != null)
					{
						xmlTextReader.Close();
					}
				}
				return string.Empty;
			}

			public string GetPrimaryOutput()
			{
				string str;
				string empty = string.Empty;
				object referenceNodeById = this.ReferencedProject.GetReferenceNodeById(this.ID);
				if (referenceNodeById != null)
				{
					try
					{
						PropertyInfo property = referenceNodeById.GetType().GetProperty("ReferencedProjectOutputPath", BindingFlags.Instance | BindingFlags.NonPublic);
						if (property != null)
						{
							string value = (string)property.GetValue(referenceNodeById, null);
							if (!string.IsNullOrEmpty(value))
							{
								str = value;
								return str;
							}
						}
						goto Label0;
					}
					catch (Exception exception)
					{
						goto Label0;
					}
					return str;
				}
			Label0:
				if (this.ReferencedProject != null && this.ReferencedProject.VsProject != null)
				{
					EnvDTE.Project vsProject = this.ReferencedProject.VsProject;
					SolutionBuild solutionBuild = vsProject.DTE.Solution.SolutionBuild;
					string activeConfiguration = this.ReferencedProject.ActiveConfiguration;
					string kind = vsProject.Kind;
					if (kind != null)
					{
						if (kind == "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}" || kind == "{F184B08F-C81C-45F6-A57F-5ABD9991F28F}" || kind == "{E6FDF86B-F3D1-11D4-8576-0002A516ECE8}")
						{
							Property property1 = null;
							string empty1 = string.Empty;
							string value1 = string.Empty;
							for (int i = 1; i <= vsProject.Properties.Count; i++)
							{
								property1 = vsProject.Properties.Item(i);
								if (!string.IsNullOrEmpty(property1.Name))
								{
									if (property1.Name.Equals("FullPath", StringComparison.InvariantCultureIgnoreCase))
									{
										empty1 = (string)property1.Value;
									}
									else if (property1.Name.Equals("OutputFileName", StringComparison.InvariantCultureIgnoreCase))
									{
										value1 = (string)property1.Value;
									}
								}
							}
							string str1 = Convert.ToString(vsProject.ConfigurationManager.ActiveConfiguration.Properties.Item("OutputPath").Value);
							if (str1 == string.Empty)
							{
								empty = Path.Combine(empty1, value1);
							}
							else if (!Path.IsPathRooted(str1))
							{
								empty = Path.GetFullPath(Path.Combine(empty1, str1));
								empty = Path.Combine(empty, value1);
							}
							else
							{
								empty = Path.Combine(str1, value1);
							}
						}
						else if (kind == "{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}")
						{
							object obj = vsProject.Object;
							object obj1 = obj.GetType().InvokeMember("Configurations", BindingFlags.GetProperty, null, obj, null);
							int num = Convert.ToInt32(obj1.GetType().InvokeMember("Count", BindingFlags.GetProperty, null, obj1, null));
							int num1 = 1;
							while (num1 <= num)
							{
								object obj2 = obj1.GetType().InvokeMember("Item", BindingFlags.InvokeMethod, null, obj1, new object[] { num1 });
								if (Convert.ToString(obj2.GetType().InvokeMember("Name", BindingFlags.GetProperty, null, obj2, null)).IndexOf(activeConfiguration, StringComparison.InvariantCultureIgnoreCase) < 0)
								{
									num1++;
								}
								else
								{
									empty = Convert.ToString(obj2.GetType().InvokeMember("PrimaryOutput", BindingFlags.GetProperty, null, obj2, null));
									return empty;
								}
							}
						}
						else if (kind == "{656346D9-4656-40DA-A068-22D5425D4639}")
						{
							this.SaveSolution(vsProject);
							Property property2 = null;
							string empty2 = string.Empty;
							string value2 = string.Empty;
							for (int j = 1; j <= vsProject.Properties.Count; j++)
							{
								property2 = vsProject.Properties.Item(j);
								if (!string.IsNullOrEmpty(property2.Name))
								{
									if (property2.Name.Equals("FullPath", StringComparison.InvariantCultureIgnoreCase))
									{
										empty2 = (string)property2.Value;
									}
									else if (property2.Name.Equals("AssemblyName", StringComparison.InvariantCultureIgnoreCase))
									{
										value2 = (string)property2.Value;
									}
								}
							}
							Configuration configuration = null;
							try
							{
								configuration = vsProject.ConfigurationManager.ActiveConfiguration;
							}
							catch
							{
							}
							string str2 = string.Empty;
							str2 = (configuration != null ? this.GetChromeOutputPath(vsProject.FullName, configuration.ConfigurationName) : this.GetChromeOutputPath(vsProject.FullName, vsProject.DTE.Solution.SolutionBuild.ActiveConfiguration.Name));
							if (str2 == string.Empty)
							{
								empty = Path.Combine(empty2, value2);
							}
							else if (!Path.IsPathRooted(str2))
							{
								empty = Path.GetFullPath(Path.Combine(empty2, str2));
								empty = Path.Combine(empty, string.Concat(Path.GetFileNameWithoutExtension(value2), ".dll"));
							}
							else
							{
								empty = Path.Combine(str2, string.Concat(Path.GetFileNameWithoutExtension(value2), ".dll"));
							}
						}
					}
				}
				return empty;
			}

			public object GetProjectOutputProperty(AddinExpress.Installer.WiXDesigner.OutputGroup outputGroup, OutputGroupProperties property)
			{
				IEnumerator enumerator;
				object obj;
				if (!this.xsltOutputsSupported)
				{
					return null;
				}
				if (this.ProjectOutputXMLDocument == null)
				{
					return null;
				}
				if (this.ProjectOutputXMLDocument.DocumentElement == null)
				{
					return null;
				}
				if (outputGroup == AddinExpress.Installer.WiXDesigner.OutputGroup.None)
				{
					return null;
				}
				XmlElement item = this.ProjectOutputXMLDocument.DocumentElement[outputGroup.ToString()];
				if (item != null)
				{
					switch (property)
					{
						case OutputGroupProperties.DiskId:
						{
							XmlElement xmlElement = item["DiskId"];
							if (xmlElement == null || xmlElement.FirstChild == null || string.IsNullOrEmpty(xmlElement.FirstChild.Value))
							{
								break;
							}
							return xmlElement.FirstChild.Value;
						}
						case OutputGroupProperties.Compressed:
						case OutputGroupProperties.Hidden:
						case OutputGroupProperties.KeyPath:
						case OutputGroupProperties.ReadOnly:
						case OutputGroupProperties.System:
						case OutputGroupProperties.Vital:
						case OutputGroupProperties.TrueType:
						case OutputGroupProperties.Permanent:
						case OutputGroupProperties.Transitive:
						{
							XmlElement item1 = item[property.ToString()];
							if (item1 == null || item1.FirstChild == null || string.IsNullOrEmpty(item1.FirstChild.Value))
							{
								break;
							}
							return item1.FirstChild.Value.Equals("yes", StringComparison.InvariantCultureIgnoreCase);
						}
						case OutputGroupProperties.Condition:
						case OutputGroupProperties.HKCUKey:
						{
							XmlElement xmlElement1 = item[property.ToString()];
							if (xmlElement1 == null || xmlElement1.FirstChild == null || string.IsNullOrEmpty(xmlElement1.FirstChild.Value))
							{
								break;
							}
							return xmlElement1.FirstChild.Value;
						}
						case OutputGroupProperties.Register:
						{
							XmlElement item2 = item["SelfRegCost"];
							if (item2 == null || item2.FirstChild == null || string.IsNullOrEmpty(item2.FirstChild.Value))
							{
								break;
							}
							return item2.FirstChild.Value.Equals("1", StringComparison.InvariantCultureIgnoreCase);
						}
						case OutputGroupProperties.SharedLegacy:
						{
							XmlElement xmlElement2 = item["SharedDllRefCount"];
							if (xmlElement2 == null || xmlElement2.FirstChild == null || string.IsNullOrEmpty(xmlElement2.FirstChild.Value))
							{
								break;
							}
							return xmlElement2.FirstChild.Value.Equals("yes", StringComparison.InvariantCultureIgnoreCase);
						}
						case OutputGroupProperties.IsInGAC:
						{
							XmlElement item3 = item["Assembly"];
							if (item3 == null || item3.FirstChild == null || string.IsNullOrEmpty(item3.FirstChild.Value))
							{
								break;
							}
							return item3.FirstChild.Value.Equals(".net", StringComparison.InvariantCultureIgnoreCase);
						}
						case OutputGroupProperties.FileTypes:
						{
							XmlElement xmlElement3 = item["FileTypes"];
							if (xmlElement3 == null || !xmlElement3.HasChildNodes)
							{
								break;
							}
							List<VsWiXProject.ReferenceDescriptor.FileType> fileTypes = new List<VsWiXProject.ReferenceDescriptor.FileType>();
							foreach (XmlElement childNode in xmlElement3.ChildNodes)
							{
								VsWiXProject.ReferenceDescriptor.FileType value = new VsWiXProject.ReferenceDescriptor.FileType()
								{
									Verbs = new List<VsWiXProject.ReferenceDescriptor.FileTypeVerb>()
								};
								XmlAttribute itemOf = childNode.Attributes["Id"];
								if (itemOf != null)
								{
									value.Name = itemOf.Value;
								}
								itemOf = childNode.Attributes["Description"];
								if (itemOf != null)
								{
									value.Description = itemOf.Value;
								}
								itemOf = childNode.Attributes["Icon"];
								if (itemOf != null)
								{
									value.Icon = itemOf.Value;
								}
								if (childNode.HasChildNodes)
								{
									string empty = string.Empty;
									foreach (XmlElement childNode1 in childNode.ChildNodes)
									{
										if (string.IsNullOrEmpty(value.MIME))
										{
											itemOf = childNode1.Attributes["ContentType"];
											if (itemOf != null)
											{
												value.MIME = itemOf.Value;
											}
										}
										itemOf = childNode1.Attributes["Id"];
										if (itemOf != null)
										{
											if (!string.IsNullOrEmpty(empty))
											{
												empty = string.Concat(empty, ";");
											}
											empty = string.Concat(empty, itemOf.Value);
										}
										if (!childNode1.HasChildNodes || value.Verbs.Count != 0)
										{
											continue;
										}
										foreach (XmlElement childNode2 in childNode1.ChildNodes)
										{
											VsWiXProject.ReferenceDescriptor.FileTypeVerb fileTypeVerb = new VsWiXProject.ReferenceDescriptor.FileTypeVerb();
											itemOf = childNode2.Attributes["Id"];
											if (itemOf != null)
											{
												fileTypeVerb.Verb = itemOf.Value;
											}
											itemOf = childNode2.Attributes["Command"];
											if (itemOf != null)
											{
												fileTypeVerb.Name = itemOf.Value;
											}
											itemOf = childNode2.Attributes["Argument"];
											if (itemOf != null)
											{
												fileTypeVerb.Arguments = itemOf.Value;
											}
											itemOf = childNode2.Attributes["Sequence"];
											if (itemOf != null)
											{
												fileTypeVerb.Sequence = itemOf.Value;
											}
											value.Verbs.Add(fileTypeVerb);
										}
									}
									value.Extensions = empty;
								}
								fileTypes.Add(value);
							}
							return fileTypes;
						}
						case OutputGroupProperties.DirectoryID:
						{
							return string.Concat(this.Caption, ".", outputGroup.ToString());
						}
						case OutputGroupProperties.RegisterForCOM:
						{
							string str = Path.Combine(Path.GetDirectoryName(this.ProjectOutputXMLFilePath), "RegisterForCOM.xml");
							if (File.Exists(str))
							{
								XmlDocument xmlDocument = new XmlDocument();
								xmlDocument.Load(str);
								if (xmlDocument.DocumentElement != null)
								{
									enumerator = xmlDocument.DocumentElement.ChildNodes.GetEnumerator();
									try
									{
										while (enumerator.MoveNext())
										{
											XmlNode current = (XmlNode)enumerator.Current;
											XmlAttribute xmlAttribute = current.Attributes["Type"];
											if (xmlAttribute == null || string.IsNullOrEmpty(xmlAttribute.Value) || !xmlAttribute.Value.Equals("ProjectOutput", StringComparison.OrdinalIgnoreCase))
											{
												continue;
											}
											xmlAttribute = current.Attributes["Name"];
											if (xmlAttribute == null || string.IsNullOrEmpty(xmlAttribute.Value) || !xmlAttribute.Value.Equals(this.Caption, StringComparison.OrdinalIgnoreCase))
											{
												continue;
											}
											obj = true;
											return obj;
										}
										return false;
									}
									finally
									{
										IDisposable disposable = enumerator as IDisposable;
										if (disposable != null)
										{
											disposable.Dispose();
										}
									}
									return obj;
								}
							}
							return false;
						}
					}
				}
				return null;
			}

			public string GetProperty(string propertyName, string culture)
			{
				char directorySeparatorChar;
				string str;
				string str1;
				string str2;
				string str3;
				string str4;
				string str5;
				string str6;
				string str7;
				string str8;
				string str9;
				string str10;
				string str11;
				string str12;
				string str13;
				string str14;
				if (propertyName != null)
				{
					switch (propertyName)
					{
						case "Configuration":
						{
							if (this.ReferencedProject != null)
							{
								if (this.ReferencedProject.ActiveConfiguration.IndexOf('|') == -1)
								{
									return this.ReferencedProject.ActiveConfiguration;
								}
								return this.ReferencedProject.ActiveConfiguration.Split(new char[] { '|' })[0];
							}
							break;
						}
						case "FullConfiguration":
						{
							if (this.ReferencedProject != null)
							{
								return this.ReferencedProject.ActiveConfiguration;
							}
							break;
						}
						case "Platform":
						{
							if (this.ReferencedProject != null)
							{
								if (this.ReferencedProject.ActiveConfiguration.IndexOf('|') != -1)
								{
									return this.ReferencedProject.ActiveConfiguration.Split(new char[] { '|' })[1];
								}
								if (this.ReferencedProject.VsProject != null)
								{
									return this.ReferencedProject.VsProject.ConfigurationManager.ActiveConfiguration.PlatformName;
								}
							}
							break;
						}
						case "ProjectDir":
						{
							if (this.ReferencedProject != null && this.ReferencedProject.VsProject != null)
							{
								string directoryName = Path.GetDirectoryName(this.ReferencedProject.VsProject.FullName);
								directorySeparatorChar = Path.DirectorySeparatorChar;
								return string.Concat(directoryName, directorySeparatorChar.ToString());
							}
							break;
						}
						case "ProjectExt":
						{
							if (this.ReferencedProject != null && this.ReferencedProject.VsProject != null)
							{
								return Path.GetExtension(this.ReferencedProject.VsProject.FullName);
							}
							break;
						}
						case "ProjectFileName":
						{
							if (this.ReferencedProject != null && this.ReferencedProject.VsProject != null)
							{
								return Path.GetFileName(this.ReferencedProject.VsProject.FullName);
							}
							break;
						}
						case "ProjectName":
						{
							if (this.ReferencedProject != null && this.ReferencedProject.VsProject != null)
							{
								return Path.GetFileName(this.ReferencedProject.VsProject.Name);
							}
							break;
						}
						case "ProjectPath":
						{
							if (this.ReferencedProject != null && this.ReferencedProject.VsProject != null)
							{
								return this.ReferencedProject.VsProject.FullName;
							}
							break;
						}
						case "TargetDir":
						{
							string primaryOutput = this.GetPrimaryOutput();
							if (!string.IsNullOrEmpty(primaryOutput))
							{
								string directoryName1 = Path.GetDirectoryName(primaryOutput);
								directorySeparatorChar = Path.DirectorySeparatorChar;
								return string.Concat(directoryName1, directorySeparatorChar.ToString());
							}
							break;
						}
						case "TargetExt":
						{
							string primaryOutput1 = this.GetPrimaryOutput();
							if (!string.IsNullOrEmpty(primaryOutput1))
							{
								return Path.GetExtension(primaryOutput1);
							}
							break;
						}
						case "TargetFileName":
						{
							string primaryOutput2 = this.GetPrimaryOutput();
							if (!string.IsNullOrEmpty(primaryOutput2))
							{
								return Path.GetFileName(primaryOutput2);
							}
							break;
						}
						case "TargetName":
						{
							string primaryOutput3 = this.GetPrimaryOutput();
							if (!string.IsNullOrEmpty(primaryOutput3))
							{
								return Path.GetFileNameWithoutExtension(primaryOutput3);
							}
							break;
						}
						default:
						{
							if (propertyName == "TargetPath")
							{
								string primaryOutput4 = this.GetPrimaryOutput();
								if (string.IsNullOrEmpty(culture))
								{
									return primaryOutput4;
								}
								return Path.Combine(Path.Combine(Path.GetDirectoryName(primaryOutput4), culture), Path.GetFileName(primaryOutput4));
							}
							switch (propertyName)
							{
								case "SolutionDir":
								{
									IVsSolution vsSolution = ProjectUtilities.GetVsSolution();
									if (vsSolution != null && ErrorHandler.Succeeded(vsSolution.GetSolutionInfo(out str, out str1, out str2)))
									{
										return str;
									}
									if (this.ReferencedProject != null && this.ReferencedProject.VsProject != null)
									{
										string directoryName2 = Path.GetDirectoryName(this.ReferencedProject.VsProject.DTE.Solution.FullName);
										directorySeparatorChar = Path.DirectorySeparatorChar;
										return string.Concat(directoryName2, directorySeparatorChar.ToString());
									}
									break;
								}
								case "SolutionExt":
								{
									IVsSolution vsSolution1 = ProjectUtilities.GetVsSolution();
									if (vsSolution1 != null && ErrorHandler.Succeeded(vsSolution1.GetSolutionInfo(out str3, out str4, out str5)))
									{
										return Path.GetExtension(str4);
									}
									if (this.ReferencedProject != null && this.ReferencedProject.VsProject != null)
									{
										return Path.GetExtension(this.ReferencedProject.VsProject.DTE.Solution.FullName);
									}
									break;
								}
								case "SolutionFileName":
								{
									IVsSolution vsSolution2 = ProjectUtilities.GetVsSolution();
									if (vsSolution2 != null && ErrorHandler.Succeeded(vsSolution2.GetSolutionInfo(out str6, out str7, out str8)))
									{
										return Path.GetFileName(str7);
									}
									if (this.ReferencedProject != null && this.ReferencedProject.VsProject != null)
									{
										return Path.GetFileName(this.ReferencedProject.VsProject.DTE.Solution.FullName);
									}
									break;
								}
								case "SolutionName":
								{
									IVsSolution vsSolution3 = ProjectUtilities.GetVsSolution();
									if (vsSolution3 != null && ErrorHandler.Succeeded(vsSolution3.GetSolutionInfo(out str9, out str10, out str11)))
									{
										return Path.GetFileNameWithoutExtension(str10);
									}
									if (this.ReferencedProject != null && this.ReferencedProject.VsProject != null)
									{
										return Path.GetFileNameWithoutExtension(this.ReferencedProject.VsProject.DTE.Solution.FullName);
									}
									break;
								}
								case "SolutionPath":
								{
									IVsSolution vsSolution4 = ProjectUtilities.GetVsSolution();
									if (vsSolution4 != null && ErrorHandler.Succeeded(vsSolution4.GetSolutionInfo(out str12, out str13, out str14)))
									{
										return str13;
									}
									if (this.ReferencedProject != null && this.ReferencedProject.VsProject != null)
									{
										return this.ReferencedProject.VsProject.DTE.Solution.FullName;
									}
									break;
								}
							}
							break;
						}
					}
				}
				return string.Empty;
			}

			public object GetReferenceNode()
			{
				return this.ReferencedProject.GetReferenceNodeById(this.ID);
			}

			private string OutputGroupsToString(AddinExpress.Installer.WiXDesigner.OutputGroup[] groups)
			{
				string empty = string.Empty;
				if (groups != null && groups.Length != 0)
				{
					AddinExpress.Installer.WiXDesigner.OutputGroup[] outputGroupArray = groups;
					for (int i = 0; i < (int)outputGroupArray.Length; i++)
					{
						AddinExpress.Installer.WiXDesigner.OutputGroup outputGroup = outputGroupArray[i];
						if (outputGroup != AddinExpress.Installer.WiXDesigner.OutputGroup.None)
						{
							if (!string.IsNullOrEmpty(empty))
							{
								empty = string.Concat(empty, ";");
							}
							empty = string.Concat(empty, outputGroup.ToString());
						}
					}
				}
				return empty;
			}

			private void SaveSolution(EnvDTE.Project project)
			{
				EnvDTE.Command command = null;
				try
				{
					command = project.DTE.Commands.Item("File.SaveAll", -1);
				}
				catch
				{
				}
				if (command != null && command.IsAvailable)
				{
					project.DTE.ExecuteCommand("File.SaveAll", "");
				}
			}

			public void SetProjectOutputProperty(AddinExpress.Installer.WiXDesigner.OutputGroup outputGroup, OutputGroupProperties property, object value)
			{
				XmlAttribute itemOf;
				if (!this.xsltOutputsSupported)
				{
					return;
				}
				if (this.ProjectOutputXMLDocument == null)
				{
					return;
				}
				if (this.ProjectOutputXMLDocument.DocumentElement == null)
				{
					return;
				}
				bool flag = true;
				XmlElement xmlElement = this.CreateXmlElement(this.ProjectOutputXMLDocument.DocumentElement, outputGroup.ToString(), true, false);
				if (xmlElement != null)
				{
					switch (property)
					{
						case OutputGroupProperties.DiskId:
						{
							XmlElement xmlElement1 = this.CreateXmlElement(xmlElement, "DiskId", true, false);
							if (xmlElement1 == null)
							{
								goto case OutputGroupProperties.DirectoryID;
							}
							if (xmlElement1.FirstChild != null)
							{
								xmlElement1.RemoveChild(xmlElement1.FirstChild);
							}
							xmlElement1.AppendChild(this.ProjectOutputXMLDocument.CreateTextNode((string)value));
							goto case OutputGroupProperties.DirectoryID;
						}
						case OutputGroupProperties.Compressed:
						case OutputGroupProperties.Hidden:
						case OutputGroupProperties.KeyPath:
						case OutputGroupProperties.ReadOnly:
						case OutputGroupProperties.System:
						case OutputGroupProperties.Vital:
						case OutputGroupProperties.TrueType:
						case OutputGroupProperties.Permanent:
						case OutputGroupProperties.Transitive:
						{
							XmlElement xmlElement2 = this.CreateXmlElement(xmlElement, property.ToString(), true, false);
							if (xmlElement2 == null)
							{
								goto case OutputGroupProperties.DirectoryID;
							}
							if (xmlElement2.FirstChild != null)
							{
								xmlElement2.RemoveChild(xmlElement2.FirstChild);
							}
							if (!(bool)value)
							{
								xmlElement2.AppendChild(this.ProjectOutputXMLDocument.CreateTextNode("no"));
								goto case OutputGroupProperties.DirectoryID;
							}
							else
							{
								xmlElement2.AppendChild(this.ProjectOutputXMLDocument.CreateTextNode("yes"));
								goto case OutputGroupProperties.DirectoryID;
							}
						}
						case OutputGroupProperties.Condition:
						case OutputGroupProperties.HKCUKey:
						{
							XmlElement xmlElement3 = this.CreateXmlElement(xmlElement, property.ToString(), !string.IsNullOrEmpty((string)value), false);
							if (xmlElement3 == null)
							{
								goto case OutputGroupProperties.DirectoryID;
							}
							if (string.IsNullOrEmpty((string)value))
							{
								xmlElement.RemoveChild(xmlElement3);
								goto case OutputGroupProperties.DirectoryID;
							}
							else
							{
								if (xmlElement3.FirstChild != null)
								{
									xmlElement3.RemoveChild(xmlElement3.FirstChild);
								}
								xmlElement3.AppendChild(this.ProjectOutputXMLDocument.CreateCDataSection((string)value));
								goto case OutputGroupProperties.DirectoryID;
							}
						}
						case OutputGroupProperties.Register:
						{
							XmlElement xmlElement4 = this.CreateXmlElement(xmlElement, "SelfRegCost", true, false);
							if (xmlElement4 == null)
							{
								goto case OutputGroupProperties.DirectoryID;
							}
							if (xmlElement4.FirstChild != null)
							{
								xmlElement4.RemoveChild(xmlElement4.FirstChild);
							}
							if (!(bool)value)
							{
								xmlElement4.AppendChild(this.ProjectOutputXMLDocument.CreateTextNode("0"));
								goto case OutputGroupProperties.DirectoryID;
							}
							else
							{
								xmlElement4.AppendChild(this.ProjectOutputXMLDocument.CreateTextNode("1"));
								goto case OutputGroupProperties.DirectoryID;
							}
						}
						case OutputGroupProperties.SharedLegacy:
						{
							XmlElement xmlElement5 = this.CreateXmlElement(xmlElement, "SharedDllRefCount", true, false);
							if (xmlElement5 == null)
							{
								goto case OutputGroupProperties.DirectoryID;
							}
							if (xmlElement5.FirstChild != null)
							{
								xmlElement5.RemoveChild(xmlElement5.FirstChild);
							}
							if (!(bool)value)
							{
								xmlElement5.AppendChild(this.ProjectOutputXMLDocument.CreateTextNode("no"));
								goto case OutputGroupProperties.DirectoryID;
							}
							else
							{
								xmlElement5.AppendChild(this.ProjectOutputXMLDocument.CreateTextNode("yes"));
								goto case OutputGroupProperties.DirectoryID;
							}
						}
						case OutputGroupProperties.IsInGAC:
						{
							XmlElement xmlElement6 = this.CreateXmlElement(xmlElement, "Assembly", (bool)value, false);
							if (xmlElement6 == null)
							{
								goto case OutputGroupProperties.DirectoryID;
							}
							if (!(bool)value)
							{
								xmlElement.RemoveChild(xmlElement6);
								goto case OutputGroupProperties.DirectoryID;
							}
							else
							{
								if (xmlElement6.FirstChild != null)
								{
									xmlElement6.RemoveChild(xmlElement6.FirstChild);
								}
								xmlElement6.AppendChild(this.ProjectOutputXMLDocument.CreateTextNode(".net"));
								goto case OutputGroupProperties.DirectoryID;
							}
						}
						case OutputGroupProperties.FileTypes:
						{
							if (value != null)
							{
								if (!(value is List<VsWiXProject.ReferenceDescriptor.FileType>))
								{
									goto case OutputGroupProperties.DirectoryID;
								}
								XmlElement xmlElement7 = this.CreateXmlElement(xmlElement, "FileTypes", true, false);
								if (xmlElement7 == null)
								{
									goto case OutputGroupProperties.DirectoryID;
								}
								if (xmlElement7.HasChildNodes)
								{
									xmlElement7.RemoveAll();
								}
								List<VsWiXProject.ReferenceDescriptor.FileType> fileTypes = (List<VsWiXProject.ReferenceDescriptor.FileType>)value;
								if (fileTypes.Count <= 0)
								{
									goto case OutputGroupProperties.DirectoryID;
								}
								List<VsWiXProject.ReferenceDescriptor.FileType>.Enumerator enumerator = fileTypes.GetEnumerator();
								try
								{
									while (enumerator.MoveNext())
									{
										VsWiXProject.ReferenceDescriptor.FileType current = enumerator.Current;
										XmlElement xmlElement8 = this.CreateXmlElement(xmlElement7, "ProgId", true, true);
										if (xmlElement8 == null)
										{
											continue;
										}
										XmlAttribute wixNamespaceUri = this.ProjectOutputXMLDocument.CreateAttribute("Id");
										wixNamespaceUri.Value = current.Name.Trim();
										xmlElement8.Attributes.Append(wixNamespaceUri);
										if (!string.IsNullOrEmpty(current.Description))
										{
											wixNamespaceUri = this.ProjectOutputXMLDocument.CreateAttribute("Description");
											wixNamespaceUri.Value = current.Description.Trim();
											xmlElement8.Attributes.Append(wixNamespaceUri);
										}
										wixNamespaceUri = this.ProjectOutputXMLDocument.CreateAttribute("Advertise");
										wixNamespaceUri.Value = "yes";
										xmlElement8.Attributes.Append(wixNamespaceUri);
										if (!string.IsNullOrEmpty(current.Icon))
										{
											wixNamespaceUri = this.ProjectOutputXMLDocument.CreateAttribute("Icon");
											wixNamespaceUri.Value = current.Icon.Trim();
											xmlElement8.Attributes.Append(wixNamespaceUri);
										}
										wixNamespaceUri = this.ProjectOutputXMLDocument.CreateAttribute("xmlns");
										wixNamespaceUri.Value = this.Parent.WixNamespaceUri;
										xmlElement8.Attributes.Append(wixNamespaceUri);
										string[] strArrays = current.Extensions.Split(new char[] { ';' });
										for (int i = 0; i < (int)strArrays.Length; i++)
										{
											string str = strArrays[i].Trim();
											if (!string.IsNullOrEmpty(str))
											{
												XmlElement xmlElement9 = this.CreateXmlElement(xmlElement8, "Extension", true, true);
												if (xmlElement9 != null)
												{
													wixNamespaceUri = this.ProjectOutputXMLDocument.CreateAttribute("Id");
													wixNamespaceUri.Value = str;
													xmlElement9.Attributes.Append(wixNamespaceUri);
													if (!string.IsNullOrEmpty(current.MIME))
													{
														wixNamespaceUri = this.ProjectOutputXMLDocument.CreateAttribute("ContentType");
														wixNamespaceUri.Value = current.MIME.Trim();
														xmlElement9.Attributes.Append(wixNamespaceUri);
													}
													wixNamespaceUri = this.ProjectOutputXMLDocument.CreateAttribute("xmlns");
													wixNamespaceUri.Value = this.Parent.WixNamespaceUri;
													xmlElement9.Attributes.Append(wixNamespaceUri);
													List<VsWiXProject.ReferenceDescriptor.FileTypeVerb>.Enumerator enumerator1 = current.Verbs.GetEnumerator();
													try
													{
														while (enumerator1.MoveNext())
														{
															VsWiXProject.ReferenceDescriptor.FileTypeVerb fileTypeVerb = enumerator1.Current;
															XmlElement xmlElement10 = this.CreateXmlElement(xmlElement9, "Verb", true, true);
															if (xmlElement10 == null)
															{
																continue;
															}
															wixNamespaceUri = this.ProjectOutputXMLDocument.CreateAttribute("Id");
															wixNamespaceUri.Value = fileTypeVerb.Verb.Trim();
															xmlElement10.Attributes.Append(wixNamespaceUri);
															wixNamespaceUri = this.ProjectOutputXMLDocument.CreateAttribute("Command");
															wixNamespaceUri.Value = fileTypeVerb.Name.Trim();
															xmlElement10.Attributes.Append(wixNamespaceUri);
															if (!string.IsNullOrEmpty(fileTypeVerb.Arguments))
															{
																wixNamespaceUri = this.ProjectOutputXMLDocument.CreateAttribute("Argument");
																wixNamespaceUri.Value = fileTypeVerb.Arguments.Trim();
																xmlElement10.Attributes.Append(wixNamespaceUri);
															}
															wixNamespaceUri = this.ProjectOutputXMLDocument.CreateAttribute("Sequence");
															wixNamespaceUri.Value = fileTypeVerb.Sequence;
															xmlElement10.Attributes.Append(wixNamespaceUri);
															wixNamespaceUri = this.ProjectOutputXMLDocument.CreateAttribute("xmlns");
															wixNamespaceUri.Value = this.Parent.WixNamespaceUri;
															xmlElement10.Attributes.Append(wixNamespaceUri);
														}
													}
													finally
													{
														((IDisposable)enumerator1).Dispose();
													}
												}
											}
										}
									}
									goto case OutputGroupProperties.DirectoryID;
								}
								finally
								{
									((IDisposable)enumerator).Dispose();
								}
							}
							else
							{
								XmlElement xmlElement11 = this.CreateXmlElement(xmlElement, "FileTypes", false, false);
								if (xmlElement11 == null)
								{
									goto case OutputGroupProperties.DirectoryID;
								}
								xmlElement.RemoveChild(xmlElement11);
								goto case OutputGroupProperties.DirectoryID;
							}
							break;
						}
						case OutputGroupProperties.DirectoryID:
						{
							if (!flag)
							{
								break;
							}
							this.UpdateXMLDocument();
							break;
						}
						case OutputGroupProperties.RegisterForCOM:
						{
							flag = false;
							string str1 = Path.Combine(Path.GetDirectoryName(this.ProjectOutputXMLFilePath), "RegisterForCOM.xml");
							if (!File.Exists(str1))
							{
								goto case OutputGroupProperties.DirectoryID;
							}
							XmlDocument xmlDocument = new XmlDocument();
							xmlDocument.Load(str1);
							if (xmlDocument.DocumentElement == null)
							{
								goto case OutputGroupProperties.DirectoryID;
							}
							XmlNode xmlNodes = null;
							foreach (XmlNode childNode in xmlDocument.DocumentElement.ChildNodes)
							{
								itemOf = childNode.Attributes["Type"];
								if (itemOf == null || string.IsNullOrEmpty(itemOf.Value) || !itemOf.Value.Equals("ProjectOutput", StringComparison.OrdinalIgnoreCase))
								{
									continue;
								}
								itemOf = childNode.Attributes["Name"];
								if (itemOf == null || string.IsNullOrEmpty(itemOf.Value) || !itemOf.Value.Equals(this.Caption, StringComparison.OrdinalIgnoreCase))
								{
									continue;
								}
								xmlNodes = childNode;
								goto Label1;
							}
						Label1:
							if (xmlNodes != null)
							{
								xmlDocument.DocumentElement.RemoveChild(xmlNodes);
							}
							if ((bool)value)
							{
								XmlElement xmlElement12 = xmlDocument.CreateElement("COMEntry");
								itemOf = xmlDocument.CreateAttribute("Type");
								itemOf.Value = "ProjectOutput";
								xmlElement12.Attributes.Append(itemOf);
								itemOf = xmlDocument.CreateAttribute("Name");
								itemOf.Value = this.Caption;
								xmlElement12.Attributes.Append(itemOf);
								string primaryOutput = this.GetPrimaryOutput();
								itemOf = xmlDocument.CreateAttribute("Id");
								itemOf.Value = string.Concat("_", Path.GetFileNameWithoutExtension(primaryOutput));
								xmlElement12.Attributes.Append(itemOf);
								itemOf = xmlDocument.CreateAttribute("SharedFileId");
								itemOf.Value = this.SharedFileId;
								xmlElement12.Attributes.Append(itemOf);
								itemOf = xmlDocument.CreateAttribute("DirectoryId");
								itemOf.Value = string.Concat(this.Caption, ".Binaries");
								xmlElement12.Attributes.Append(itemOf);
								itemOf = xmlDocument.CreateAttribute("Source");
								itemOf.Value = Path.Combine(CommonUtilities.RelativizePathIfPossible(Path.GetDirectoryName(primaryOutput), this.Parent.RootDirectory), Path.GetFileName(primaryOutput));
								xmlElement12.Attributes.Append(itemOf);
								xmlDocument.DocumentElement.AppendChild(xmlElement12);
							}
							using (XmlTextWriter xmlTextWriter = new XmlTextWriter(str1, Encoding.UTF8))
							{
								xmlTextWriter.Formatting = Formatting.Indented;
								xmlDocument.WriteTo(xmlTextWriter);
								xmlTextWriter.Flush();
								goto case OutputGroupProperties.DirectoryID;
							}
							break;
						}
						default:
						{
							goto case OutputGroupProperties.DirectoryID;
						}
					}
				}
			}

			private AddinExpress.Installer.WiXDesigner.OutputGroup[] StringToOutputGroups(string groupNames)
			{
				AddinExpress.Installer.WiXDesigner.OutputGroup outputGroup;
				AddinExpress.Installer.WiXDesigner.OutputGroup[] outputGroupArray = null;
				if (string.IsNullOrEmpty(groupNames))
				{
					return new AddinExpress.Installer.WiXDesigner.OutputGroup[1];
				}
				string[] strArrays = groupNames.Split(new char[] { ';' });
				outputGroupArray = new AddinExpress.Installer.WiXDesigner.OutputGroup[(int)strArrays.Length];
				for (int i = 0; i < (int)strArrays.Length; i++)
				{
					if (Enum.TryParse<AddinExpress.Installer.WiXDesigner.OutputGroup>(strArrays[i].Trim(), out outputGroup))
					{
						outputGroupArray[i] = outputGroup;
					}
				}
				return outputGroupArray;
			}

			private void UpdateXMLDocument()
			{
				if (!string.IsNullOrEmpty(this.ProjectOutputXMLFilePath) && File.Exists(this.ProjectOutputXMLFilePath))
				{
					using (XmlTextWriter xmlTextWriter = new XmlTextWriter(this.ProjectOutputXMLFilePath, Encoding.UTF8))
					{
						xmlTextWriter.Formatting = Formatting.Indented;
						this.ProjectOutputXMLDocument.WriteTo(xmlTextWriter);
						xmlTextWriter.Flush();
					}
				}
			}

			public struct FileType
			{
				public string Name;

				public string Description;

				public string Extensions;

				public string Icon;

				public string MIME;

				public List<VsWiXProject.ReferenceDescriptor.FileTypeVerb> Verbs;

				public FileType(string name, string description, string extensions, string iconID, string mime)
				{
					this.Name = name;
					this.Description = description;
					this.Extensions = extensions;
					this.Icon = iconID;
					this.MIME = mime;
					this.Verbs = new List<VsWiXProject.ReferenceDescriptor.FileTypeVerb>();
				}

				public void AddVerb(VsWiXProject.ReferenceDescriptor.FileTypeVerb verbItem)
				{
					if (this.Verbs == null)
					{
						this.Verbs = new List<VsWiXProject.ReferenceDescriptor.FileTypeVerb>();
					}
					this.Verbs.Add(verbItem);
				}
			}

			public class FileTypeVerb
			{
				public string Name;

				public string Arguments;

				public string Verb;

				public string Sequence;

				public FileTypeVerb()
				{
				}

				public FileTypeVerb(string name, string arguments, string verb, string sequence)
				{
					this.Name = name;
					this.Arguments = arguments;
					this.Verb = verb;
					this.Sequence = sequence;
				}
			}
		}

		private enum RefRefreshReason
		{
			Unknown,
			ReferenceAdded,
			ReferenceRemoved,
			ReferenceRenamed,
			XmlFileAdded,
			XmlFileRemoved,
			ReferencedProjectRenamed,
			MultiSelectFilesRemoved
		}

		[ComVisible(false)]
		public class WiXFileDescriptor : IVsTextLinesEvents, IDisposable
		{
			private bool disposed;

			private uint itemid;

			private bool isInSaveMode;

			private bool isDocUpToDate;

			private IntPtr docData;

			private IVsTextLines textLines;

			private IVsHierarchy hierarchy;

			private string filePath;

			private VsWiXProject.WiXFileDescriptor.Status fileStatus;

			private VsWiXProject projectManager;

			private Hashtable viewList;

			private uint docCookie;

			private uint textLinesCookie;

			private XmlDocument document;

			private string documentText;

			private bool updatingOnUserEdit;

			private object xmlLanguageService;

			private int activeDesignerID;

			private Timer onTextChangedTimer;

			private bool parseXmlTree;

			private bool internalBufferChanged;

			public uint DocCookie
			{
				get
				{
					return this.docCookie;
				}
			}

			public XmlDocument Document
			{
				get
				{
					return this.document;
				}
			}

			public string DocumentText
			{
				get
				{
					return this.documentText;
				}
			}

			public string FilePath
			{
				get
				{
					return this.filePath;
				}
			}

			public VsWiXProject.WiXFileDescriptor.Status FileStatus
			{
				get
				{
					return this.fileStatus;
				}
				internal set
				{
					this.fileStatus = value;
				}
			}

			public IVsHierarchy Hierarchy
			{
				get
				{
					return this.hierarchy;
				}
			}

			public bool IsDirty
			{
				get
				{
					int num;
					if (this.docCookie <= 0 || this.textLines == null)
					{
						return !this.isDocUpToDate;
					}
					(this.textLines as IVsPersistDocData).IsDocDataDirty(out num);
					return num > 0;
				}
			}

			public bool IsInSaveMode
			{
				get
				{
					return this.isInSaveMode;
				}
			}

			public bool IsInternalBufferChanged
			{
				get
				{
					return this.internalBufferChanged;
				}
			}

			public uint ItemID
			{
				get
				{
					return this.itemid;
				}
			}

			public bool Opened
			{
				get
				{
					return this.docCookie != 0;
				}
			}

			public Hashtable RegisteredViews
			{
				get
				{
					return this.viewList;
				}
			}

			public IVsTextLines TextLines
			{
				get
				{
					return this.textLines;
				}
			}

			public WiXFileDescriptor(string filePath, uint id, VsWiXProject projectManager, bool parseXmlTree)
			{
				this.filePath = filePath;
				this.itemid = id;
				this.projectManager = projectManager;
				this.parseXmlTree = parseXmlTree;
				this.onTextChangedTimer = new Timer()
				{
					Interval = 1000
				};
				this.onTextChangedTimer.Tick += new EventHandler(this.onTextChangedTimer_Tick);
			}

			private void AdviseTextLinesEvents(bool subscribe)
			{
				IConnectionPoint connectionPoint;
				if (this.textLines != null)
				{
					IConnectionPointContainer connectionPointContainer = (IConnectionPointContainer)this.textLines;
					if (connectionPointContainer != null)
					{
						Guid gUID = typeof(IVsTextLinesEvents).GUID;
						connectionPointContainer.FindConnectionPoint(ref gUID, out connectionPoint);
						if (subscribe)
						{
							if (this.textLinesCookie == 0)
							{
								connectionPoint.Advise(this, out this.textLinesCookie);
								return;
							}
						}
						else if (this.textLinesCookie != 0)
						{
							connectionPoint.Unadvise(this.textLinesCookie);
							this.textLinesCookie = 0;
						}
					}
				}
			}

			internal void Close(int designerId)
			{
				IntPtr intPtr;
				string str;
				IVsHierarchy vsHierarchy;
				uint num;
				uint num1;
				uint num2;
				this.UnregisterViewManager(designerId);
				if (this.docCookie > 0 && designerId <= 0)
				{
					try
					{
						IVsRunningDocumentTable service = this.projectManager.GetService(typeof(SVsRunningDocumentTable)) as IVsRunningDocumentTable;
						if (service != null)
						{
							uint num3 = 0;
							service.GetDocumentInfo(this.docCookie, out num, out num1, out num3, out str, out vsHierarchy, out num2, out intPtr);
							if (num3 > 0)
							{
								service.UnlockDocument(2, this.docCookie);
								num3 = 0;
								service.GetDocumentInfo(this.docCookie, out num, out num1, out num3, out str, out vsHierarchy, out num2, out intPtr);
							}
							if (num3 == 0 && this.docData != IntPtr.Zero)
							{
								if (this.textLines is IVsPersistDocData)
								{
									(this.textLines as IVsPersistDocData).Close();
								}
								Marshal.Release(this.docData);
							}
						}
					}
					finally
					{
						this.AdviseTextLinesEvents(false);
						this.textLines = null;
						this.document = null;
						this.hierarchy = null;
						this.docData = IntPtr.Zero;
						this.docCookie = 0;
					}
				}
			}

			public void Dispose()
			{
				if (!this.disposed)
				{
					this.disposed = true;
					if (this.onTextChangedTimer != null)
					{
						this.onTextChangedTimer.Enabled = false;
						this.onTextChangedTimer.Dispose();
					}
					this.Close(-1);
					if (this.viewList != null)
					{
						this.viewList.Clear();
					}
				}
				GC.SuppressFinalize(this);
			}

			~WiXFileDescriptor()
			{
				this.Dispose();
			}

			private void FormatBuffer(object src)
			{
				string str;
				int vSVersion = VsPackage.CurrentInstance.GetVSVersion();
				if (vSVersion == 10)
				{
					str = "Microsoft.VisualStudio.Package.EditArray, Microsoft.VisualStudio.Package.LanguageService.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
				}
				else if (vSVersion != 11)
				{
					str = (vSVersion != 12 ? "Microsoft.VisualStudio.Package.EditArray, Microsoft.VisualStudio.Package.LanguageService.14.0, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" : "Microsoft.VisualStudio.Package.EditArray, Microsoft.VisualStudio.Package.LanguageService.12.0, Version=12.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
				}
				else
				{
					str = "Microsoft.VisualStudio.Package.EditArray, Microsoft.VisualStudio.Package.LanguageService.11.0, Version=11.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
				}
				Type type = Type.GetType(str, false);
				if (type != null)
				{
					object obj = null;
					try
					{
						try
						{
							obj = Activator.CreateInstance(type, new object[] { src, null, false, Resources.ReformatBuffer });
							if (obj != null)
							{
								TextSpan textSpan = (TextSpan)src.GetType().InvokeMember("GetDocumentSpan", BindingFlags.InvokeMethod, null, src, null);
								src.GetType().InvokeMember("ReformatSpan", BindingFlags.InvokeMethod, null, src, new object[] { obj, textSpan });
							}
						}
						catch (Exception exception)
						{
						}
					}
					finally
					{
						if (obj != null)
						{
							(obj as IDisposable).Dispose();
						}
					}
				}
			}

			private object GetSource()
			{
				object xmlLanguageService = this.GetXmlLanguageService();
				if (xmlLanguageService == null)
				{
					return null;
				}
				return xmlLanguageService.GetType().InvokeMember("GetSource", BindingFlags.InvokeMethod, null, xmlLanguageService, new object[] { this.textLines });
			}

			private string GetText()
			{
				string str;
				IVsRunningDocumentTable service = this.projectManager.GetService(typeof(SVsRunningDocumentTable)) as IVsRunningDocumentTable;
				if (service == null)
				{
					return null;
				}
				service.LockDocument(1, this.docCookie);
				try
				{
					int num = 0;
					int num1 = 0;
					string str1 = null;
					this.textLines.GetLastLineIndex(out num, out num1);
					this.textLines.GetLineText(0, 0, num, num1, out str1);
					str = str1;
				}
				finally
				{
					service.UnlockDocument(1, this.docCookie);
				}
				return str;
			}

			private object GetXmlLanguageService()
			{
				IntPtr intPtr;
				if (this.xmlLanguageService == null)
				{
					Microsoft.VisualStudio.OLE.Interop.IServiceProvider service = this.projectManager.GetService(typeof(Microsoft.VisualStudio.OLE.Interop.IServiceProvider)) as Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
					if (service != null)
					{
						Guid guid = new Guid("f6819a78-a205-47b5-be1c-675b3c7f0b8e");
						Guid guid1 = new Guid("00000000-0000-0000-C000-000000000046");
						if (ErrorHandler.Succeeded(service.QueryService(ref guid, ref guid1, out intPtr)))
						{
							try
							{
								this.xmlLanguageService = Marshal.GetObjectForIUnknown(intPtr);
							}
							finally
							{
								Marshal.Release(intPtr);
							}
						}
					}
				}
				return this.xmlLanguageService;
			}

			void Microsoft.VisualStudio.TextManager.Interop.IVsTextLinesEvents.OnChangeLineAttributes(int iFirstLine, int iLastLine)
			{
				if (!this.updatingOnUserEdit)
				{
					this.isDocUpToDate = false;
					if (this.viewList.Count > 0)
					{
						foreach (VsViewManager value in this.viewList.Values)
						{
							value.OnChangeLineAttributes(this, iFirstLine, iLastLine);
							value.OnFileBufferStatusChanged(this);
						}
					}
				}
				else if (this.activeDesignerID != -1 && this.viewList.Count > 0)
				{
					foreach (VsViewManager vsViewManager in this.viewList.Values)
					{
						if (vsViewManager.Pane.ID == this.activeDesignerID)
						{
							continue;
						}
						vsViewManager.OnChangeLineAttributes(this, iFirstLine, iLastLine);
						vsViewManager.OnFileBufferStatusChanged(this);
					}
				}
			}

			void Microsoft.VisualStudio.TextManager.Interop.IVsTextLinesEvents.OnChangeLineText(TextLineChange[] pTextLineChange, int fLast)
			{
				if (!this.updatingOnUserEdit)
				{
					this.isDocUpToDate = false;
					this.internalBufferChanged = false;
					if (this.projectManager.WiXModel != null)
					{
						this.projectManager.WiXModel.IsDirty = true;
					}
					if (this.viewList.Count > 0)
					{
						foreach (VsViewManager value in this.viewList.Values)
						{
							value.OnChangeLineText(this, pTextLineChange, fLast);
							value.OnFileBufferStatusChanged(this);
						}
					}
					if (this.onTextChangedTimer.Enabled)
					{
						this.onTextChangedTimer.Stop();
					}
					this.onTextChangedTimer.Start();
				}
				else
				{
					if (this.projectManager.WiXModel != null)
					{
						this.projectManager.WiXModel.IsDirty = true;
					}
					if (this.activeDesignerID != -1 && this.viewList.Count > 0)
					{
						foreach (VsViewManager vsViewManager in this.viewList.Values)
						{
							if (vsViewManager.Pane.ID == this.activeDesignerID)
							{
								continue;
							}
							vsViewManager.OnChangeLineText(this, pTextLineChange, fLast);
							vsViewManager.OnFileBufferStatusChanged(this);
						}
					}
				}
			}

			internal void Modify(int designerId, XmlDocument document)
			{
				if (document.DocumentElement == null)
				{
					this.Modify(designerId, string.Empty, false, false);
				}
				else
				{
					using (StringWriter stringWriter = new StringWriter())
					{
						using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
						{
							xmlTextWriter.Formatting = Formatting.Indented;
							xmlTextWriter.Indentation = 2;
							document.WriteTo(xmlTextWriter);
							xmlTextWriter.Flush();
							this.Modify(designerId, stringWriter.GetStringBuilder().ToString(), false, false);
						}
					}
				}
			}

			internal void Modify(int designerId, string textLines)
			{
				this.Modify(designerId, textLines, true, false);
			}

			internal void Modify(int designerId, string textLines, bool performFormatting, bool enableVsFormatting)
			{
				if (this.docCookie > 0)
				{
					object obj = null;
					bool flag = false;
					if (performFormatting)
					{
						if (!enableVsFormatting)
						{
							using (StringWriter stringWriter = new StringWriter())
							{
								using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
								{
									xmlTextWriter.Formatting = Formatting.Indented;
									xmlTextWriter.Indentation = 2;
									XmlDocument xmlDocument = new XmlDocument();
									xmlDocument.LoadXml(textLines);
									xmlDocument.WriteTo(xmlTextWriter);
									xmlTextWriter.Flush();
									textLines = stringWriter.GetStringBuilder().ToString();
								}
							}
						}
						else
						{
							try
							{
								object xmlLanguageService = this.GetXmlLanguageService();
								if (xmlLanguageService != null)
								{
									object obj1 = xmlLanguageService.GetType().InvokeMember("Preferences", BindingFlags.GetProperty, null, xmlLanguageService, null);
									if (obj1 != null)
									{
										obj1.GetType().InvokeMember("EnableFormatSelection", BindingFlags.SetProperty, null, obj1, new object[] { true });
									}
									obj = xmlLanguageService.GetType().InvokeMember("GetSource", BindingFlags.InvokeMethod, null, xmlLanguageService, new object[] { this.textLines });
									if (obj == null)
									{
										flag = true;
										obj = xmlLanguageService.GetType().InvokeMember("CreateSource", BindingFlags.InvokeMethod, null, xmlLanguageService, new object[] { this.textLines });
									}
									if (obj != null)
									{
										MethodInfo method = xmlLanguageService.GetType().GetMethod("GetParseTree");
										int num = 0;
										int num1 = 0;
										method.Invoke(xmlLanguageService, new object[] { obj, null, num, num1, 5 });
									}
								}
							}
							catch (Exception exception)
							{
							}
						}
					}
					this.updatingOnUserEdit = true;
					this.activeDesignerID = designerId;
					try
					{
						this.SetText(textLines);
						this.fileStatus = VsWiXProject.WiXFileDescriptor.Status.Modified;
						if (obj != null & performFormatting)
						{
							this.FormatBuffer(obj);
						}
						this.isDocUpToDate = true;
					}
					finally
					{
						if (obj != null & flag)
						{
							obj.GetType().InvokeMember("Close", BindingFlags.InvokeMethod, null, obj, null);
							(obj as IDisposable).Dispose();
						}
						this.updatingOnUserEdit = false;
						this.activeDesignerID = -1;
					}
				}
			}

			internal void NotifyBufferSaved()
			{
				if (this.viewList.Count > 0)
				{
					this.fileStatus = VsWiXProject.WiXFileDescriptor.Status.Default;
					foreach (object value in this.viewList.Values)
					{
						((VsViewManager)value).OnFileBufferStatusChanged(this);
					}
				}
			}

			private void onTextChangedTimer_Tick(object sender, EventArgs e)
			{
				this.onTextChangedTimer.Stop();
				try
				{
					this.RefreshDocument();
				}
				catch (Exception exception)
				{
				}
			}

			internal void Open(int designerId)
			{
				uint num;
				uint num1;
				uint num2;
				uint num3;
				if (this.fileStatus == VsWiXProject.WiXFileDescriptor.Status.Removed)
				{
					return;
				}
				try
				{
					try
					{
						if (this.docCookie == 0)
						{
							bool flag = false;
							IVsRunningDocumentTable service = this.projectManager.GetService(typeof(SVsRunningDocumentTable)) as IVsRunningDocumentTable;
							if (service != null && ProjectUtilities.OpenFileInternal(this.filePath, this.projectManager.Hierarchy, this.itemid, out this.docCookie, out flag))
							{
								if (flag)
								{
									service.LockDocument(2, this.docCookie);
								}
								service.GetDocumentInfo(this.docCookie, out num, out num1, out num2, out this.filePath, out this.hierarchy, out num3, out this.docData);
								this.textLines = Marshal.GetObjectForIUnknown(this.docData) as IVsTextLines;
								this.AdviseTextLinesEvents(true);
							}
						}
						if (this.docCookie > 0)
						{
							this.fileStatus = VsWiXProject.WiXFileDescriptor.Status.Default;
							if (this.parseXmlTree && this.document == null)
							{
								this.isDocUpToDate = false;
							}
							this.RefreshDocument();
						}
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						if (exception is XmlException)
						{
							XmlException xmlException = new XmlException(exception.Message, exception);
							xmlException.Data["SourceFile"] = this.FilePath;
							xmlException.Data["LineNumber"] = (exception as XmlException).LineNumber;
							xmlException.Data["LinePosition"] = (exception as XmlException).LinePosition;
							throw xmlException;
						}
						throw;
					}
				}
				finally
				{
					this.internalBufferChanged = false;
					this.RegisterViewManager(designerId);
				}
			}

			private bool RefreshDocument()
			{
				if (!this.isDocUpToDate)
				{
					this.document = null;
					this.documentText = string.Empty;
					if (this.textLines != null)
					{
						object obj = null;
						bool flag = false;
						try
						{
							try
							{
								object xmlLanguageService = this.GetXmlLanguageService();
								if (xmlLanguageService != null)
								{
									obj = xmlLanguageService.GetType().InvokeMember("GetSource", BindingFlags.InvokeMethod, null, xmlLanguageService, new object[] { this.textLines });
									if (obj == null)
									{
										flag = true;
										obj = xmlLanguageService.GetType().InvokeMember("CreateSource", BindingFlags.InvokeMethod, null, xmlLanguageService, new object[] { this.textLines });
									}
									if (obj != null)
									{
										MethodInfo method = xmlLanguageService.GetType().GetMethod("GetParseTree");
										int num = 0;
										int num1 = 0;
										method.Invoke(xmlLanguageService, new object[] { obj, null, num, num1, 5 });
									}
								}
							}
							catch (Exception exception)
							{
							}
						}
						finally
						{
							if (obj != null & flag)
							{
								obj.GetType().InvokeMember("Close", BindingFlags.InvokeMethod, null, obj, null);
								(obj as IDisposable).Dispose();
							}
						}
						this.documentText = this.GetText();
						if (this.parseXmlTree)
						{
							this.document = new XmlDocument();
							if (!string.IsNullOrEmpty(this.documentText))
							{
								this.document.LoadXml(this.documentText);
							}
						}
						this.isDocUpToDate = true;
						return true;
					}
				}
				return false;
			}

			private void RegisterViewManager(int designerId)
			{
				if (designerId > 0)
				{
					if (!this.viewList.ContainsKey(designerId))
					{
						VsViewManager viewManagerByDesignerID = this.projectManager.GetViewManagerByDesignerID(designerId);
						if (viewManagerByDesignerID != null)
						{
							viewManagerByDesignerID.OnWiXFileOpened(this);
							this.viewList.Add(designerId, viewManagerByDesignerID);
							return;
						}
					}
				}
				else if (designerId == 0)
				{
					foreach (VsPaneBase value in this.projectManager.Panes.Values)
					{
						if (this.viewList.ContainsKey(value.ID))
						{
							continue;
						}
						VsViewManager viewManager = value.ViewManager;
						if (viewManager == null)
						{
							continue;
						}
						viewManager.OnWiXFileOpened(this);
						this.viewList.Add(value.ID, viewManager);
					}
				}
			}

			internal bool Save(int designerId, bool checkEditLocks, bool closeAfterSave, out bool saveCanceled)
			{
				IntPtr intPtr;
				string str;
				IVsHierarchy vsHierarchy;
				uint num;
				uint num1;
				uint num2;
				bool flag;
				saveCanceled = false;
				if (this.IsDirty && this.viewList.Count <= 1 && this.viewList.ContainsKey(designerId))
				{
					IVsRunningDocumentTable service = this.projectManager.GetService(typeof(SVsRunningDocumentTable)) as IVsRunningDocumentTable;
					if (service != null)
					{
						if (checkEditLocks)
						{
							uint num3 = 0;
							service.GetDocumentInfo(this.docCookie, out num, out num1, out num3, out str, out vsHierarchy, out num2, out intPtr);
							if (num3 > 1)
							{
								return false;
							}
						}
						try
						{
							try
							{
								this.isInSaveMode = true;
								ErrorHandler.ThrowOnFailure(service.SaveDocuments(0, this.hierarchy, this.itemid, this.docCookie));
								return false;
							}
							catch (COMException cOMException)
							{
								if (cOMException.ErrorCode != -2147467260)
								{
									throw;
								}
								else
								{
									saveCanceled = true;
									flag = false;
								}
							}
							catch (ArgumentNullException argumentNullException)
							{
								return false;
							}
							catch (NullReferenceException nullReferenceException)
							{
								return false;
							}
						}
						finally
						{
							this.isInSaveMode = false;
						}
						return flag;
					}
				}
				return false;
			}

			internal bool SaveBuffer(int designerId, bool closeAfterSave, out bool saveCanceled)
			{
				int num;
				string str;
				bool flag;
				saveCanceled = false;
				if (this.IsDirty && this.viewList.Count <= 1 && this.viewList.ContainsKey(designerId))
				{
					IVsPersistDocData vsPersistDocDatum = (IVsPersistDocData)this.textLines;
					try
					{
						this.isInSaveMode = true;
						if (vsPersistDocDatum.SaveDocData(VSSAVEFLAGS.VSSAVE_Save, out str, out num) != 0)
						{
							return false;
						}
						else
						{
							saveCanceled = num > 0;
							if (!saveCanceled & closeAfterSave)
							{
								this.Close(designerId);
							}
							flag = true;
						}
					}
					finally
					{
						this.isInSaveMode = false;
					}
					return flag;
				}
				return false;
			}

			internal bool SaveWithPrompt(int designerId, bool checkEditLocks, bool closeAfterSave, out bool saveCanceled)
			{
				IntPtr intPtr;
				string str;
				IVsHierarchy vsHierarchy;
				uint num;
				uint num1;
				uint num2;
				bool flag;
				saveCanceled = false;
				if (this.IsDirty && this.viewList.Count <= 1 && this.viewList.ContainsKey(designerId))
				{
					IVsRunningDocumentTable service = this.projectManager.GetService(typeof(SVsRunningDocumentTable)) as IVsRunningDocumentTable;
					if (service != null)
					{
						if (checkEditLocks)
						{
							uint num3 = 0;
							service.GetDocumentInfo(this.docCookie, out num, out num1, out num3, out str, out vsHierarchy, out num2, out intPtr);
							if (num3 > 1)
							{
								return false;
							}
						}
						try
						{
							try
							{
								this.isInSaveMode = true;
								try
								{
									if (!closeAfterSave)
									{
										ErrorHandler.ThrowOnFailure(service.SaveDocuments(1, this.hierarchy, this.itemid, this.docCookie));
									}
									else
									{
										ErrorHandler.ThrowOnFailure(service.SaveDocuments(65537, this.hierarchy, this.itemid, this.docCookie));
									}
								}
								catch (ArgumentNullException argumentNullException)
								{
								}
								catch (NullReferenceException nullReferenceException)
								{
								}
								if (closeAfterSave)
								{
									this.Close(designerId);
								}
								flag = true;
							}
							catch (COMException cOMException)
							{
								if (cOMException.ErrorCode != -2147467260)
								{
									throw;
								}
								else
								{
									saveCanceled = true;
									flag = false;
								}
							}
						}
						finally
						{
							this.isInSaveMode = false;
						}
						return flag;
					}
				}
				return false;
			}

			private void SetText(string newText)
			{
				int num;
				int num1;
				ErrorHandler.ThrowOnFailure(this.textLines.GetLastLineIndex(out num, out num1));
				int num2 = (newText == null ? 0 : newText.Length);
				IntPtr coTaskMemAuto = Marshal.StringToCoTaskMemAuto(newText);
				try
				{
					ErrorHandler.ThrowOnFailure(this.textLines.ReplaceLines(0, 0, num, num1, coTaskMemAuto, num2, null));
				}
				finally
				{
					Marshal.FreeCoTaskMem(coTaskMemAuto);
				}
			}

			private void UnregisterViewManager(int designerId)
			{
				if (designerId > 0)
				{
					if (this.viewList.ContainsKey(designerId))
					{
						VsViewManager viewManagerByDesignerID = this.projectManager.GetViewManagerByDesignerID(designerId);
						if (viewManagerByDesignerID != null)
						{
							viewManagerByDesignerID.OnWiXFileClosed(this);
							this.viewList.Remove(designerId);
							return;
						}
					}
				}
				else if (designerId == 0)
				{
					foreach (VsPaneBase value in this.projectManager.Panes.Values)
					{
						if (!this.viewList.ContainsKey(value.ID))
						{
							continue;
						}
						VsViewManager viewManager = value.ViewManager;
						if (viewManager == null)
						{
							continue;
						}
						viewManager.OnWiXFileClosed(this);
						this.viewList.Remove(value.ID);
					}
				}
			}

			private enum ParseReason
			{
				None,
				MemberSelect,
				HighlightBraces,
				MemberSelectAndHighlightBraces,
				MatchBraces,
				Check,
				CompleteWord,
				DisplayMemberList,
				QuickInfo,
				MethodTip,
				Autos,
				CodeSpan,
				Goto
			}

			public enum Status
			{
				Default,
				New,
				Modified,
				Removed
			}
		}
	}
}