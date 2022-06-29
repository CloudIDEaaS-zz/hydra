using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXProjectParser
	{
		private bool dirty = true;

		private string projectNamespace = string.Empty;

		private VsWiXProject projectManager;

		private WiXProjectType _projectType;

		private List<WiXProjectItem> _wixProjectItems;

		private List<WiXEntity> _supportedEntities;

		private List<WiXLocalization> _languages;

		private WiXCustomVariables _customVariables;

		private VSFolders _fileSystem;

		private List<VSFeature> _features;

		private List<VSFeatureGroup> _featureGroups;

		private VSComponentGroups _componentGroups;

		private VSLaunchConditions _launchConditions;

		private VSSearches _searches;

		private List<WiXIcon> _icons;

		private VSWebSetupParameters _webSetupParameters;

		private VSFileTypes _fileTypes;

		private List<VSBinary> _binaries;

		private VSCustomActions _customActions;

		private List<VSMergeModule> _mergeModules;

		private List<VSMergeModuleReference> _mergeModuleReferences;

		private VSRegistry _registry;

		private VSUserInterface _userInterface;

		internal IViewManager ViewManager;

		internal List<VSBinary> Binaries
		{
			get
			{
				if (this._binaries.Count == 0)
				{
					List<WiXBinary> wiXBinaries = this._supportedEntities.FindAll((WiXEntity e) => e is WiXBinary).ConvertAll<WiXBinary>((WiXEntity e) => e as WiXBinary);
					if (wiXBinaries != null && wiXBinaries.Count > 0)
					{
						foreach (WiXBinary wiXBinary in wiXBinaries)
						{
							if (!(wiXBinary.Parent is WiXProduct) && !(wiXBinary.Parent is WiXModule) && !(wiXBinary.Parent is WiXFragment) && !(wiXBinary.Parent is WiXUI) && !(wiXBinary.Parent is WiXControl))
							{
								continue;
							}
							VSBinary vSBinary = new VSBinary(this, wiXBinary);
							this._binaries.Add(vSBinary);
							this._supportedEntities.Remove(wiXBinary);
						}
					}
				}
				return this._binaries;
			}
		}

		internal VSComponentGroups ComponentGroups
		{
			get
			{
				return this._componentGroups;
			}
		}

		internal string CurrentLCID
		{
			get
			{
				if (this.ProjectManager == null || this.ProjectManager.ProjectProperties == null)
				{
					return "0";
				}
				return this.ProjectManager.ProjectProperties.LCID;
			}
		}

		internal WiXLocalization CurrentLocalization
		{
			get
			{
				WiXLocalization wiXLocalization = null;
				if (this.projectManager != null && this.projectManager.IsMultiLangSupported)
				{
					string lCID = this.projectManager.ProjectProperties.LCID;
					wiXLocalization = this.Languages.Find((WiXLocalization e) => e.Id == lCID);
				}
				return wiXLocalization;
			}
		}

		internal VSCustomActions CustomActions
		{
			get
			{
				return this._customActions;
			}
		}

		internal WiXCustomVariables CustomVariables
		{
			get
			{
				return this._customVariables;
			}
		}

		internal List<VSFeatureGroup> FeatureGroups
		{
			get
			{
				return this._featureGroups;
			}
		}

		internal List<VSFeature> Features
		{
			get
			{
				return this._features;
			}
		}

		internal VSFolders FileSystem
		{
			get
			{
				return this._fileSystem;
			}
		}

		internal VSFileTypes FileTypes
		{
			get
			{
				return this._fileTypes;
			}
		}

		internal List<WiXIcon> Icons
		{
			get
			{
				return this._icons;
			}
		}

		internal bool IsDirty
		{
			get
			{
				return this.dirty;
			}
			set
			{
				this.dirty = value;
			}
		}

		internal bool IsWebSetup
		{
			get
			{
				bool flag;
				WiXEntity wiXEntity = null;
				if (this._projectType == WiXProjectType.Product)
				{
					wiXEntity = this._supportedEntities.Find((WiXEntity e) => e is WiXProduct);
				}
				if (this._projectType == WiXProjectType.Module)
				{
					wiXEntity = this._supportedEntities.Find((WiXEntity e) => e is WiXModule);
				}
				if (wiXEntity != null)
				{
					try
					{
						foreach (object attribute in (wiXEntity.Parent as WiXEntity).XmlNode.Attributes)
						{
							if (((XmlAttribute)attribute).Value.ToLower() != IISSchema.TargetNamespace.ToLower())
							{
								continue;
							}
							flag = true;
							return flag;
						}
						if (this._webSetupParameters.WebSites.Count <= 0 && this._webSetupParameters.AppPools.Count <= 0)
						{
							return false;
						}
						return true;
					}
					catch
					{
						if (this._webSetupParameters.WebSites.Count <= 0 && this._webSetupParameters.AppPools.Count <= 0)
						{
							return false;
						}
						return true;
					}
					return flag;
				}
				if (this._webSetupParameters.WebSites.Count <= 0 && this._webSetupParameters.AppPools.Count <= 0)
				{
					return false;
				}
				return true;
			}
		}

		internal List<WiXLocalization> Languages
		{
			get
			{
				return this._languages;
			}
		}

		internal VSLaunchConditions LaunchConditions
		{
			get
			{
				return this._launchConditions;
			}
		}

		internal List<VSMergeModuleReference> MergeModuleReferences
		{
			get
			{
				return this._mergeModuleReferences;
			}
		}

		internal List<VSMergeModule> MergeModules
		{
			get
			{
				return this._mergeModules;
			}
		}

		internal string Namespace
		{
			get
			{
				return this.projectNamespace;
			}
			set
			{
				this.projectNamespace = value;
			}
		}

		internal WiXLocalization NeutralLocalization
		{
			get
			{
				WiXLocalization wiXLocalization = null;
				if (this.projectManager != null && this.projectManager.IsMultiLangSupported)
				{
					wiXLocalization = this.Languages.Find((WiXLocalization e) => ProjectUtilities.IsNeutralLCID(e.Id));
				}
				return wiXLocalization;
			}
		}

		internal List<WiXProjectItem> ProjectItems
		{
			get
			{
				return this._wixProjectItems;
			}
		}

		internal VsWiXProject ProjectManager
		{
			get
			{
				return this.projectManager;
			}
		}

		internal WiXProjectType ProjectType
		{
			get
			{
				return this._projectType;
			}
			set
			{
				this._projectType = value;
			}
		}

		internal VSRegistry Registry
		{
			get
			{
				return this._registry;
			}
		}

		internal VSSearches Searches
		{
			get
			{
				return this._searches;
			}
		}

		internal List<WiXEntity> SupportedEntities
		{
			get
			{
				return this._supportedEntities;
			}
		}

		internal VSUserInterface UserInterface
		{
			get
			{
				return this._userInterface;
			}
		}

		internal VSWebSetupParameters WebSetupParameters
		{
			get
			{
				return this._webSetupParameters;
			}
		}

		internal WiXProjectParser(VsWiXProject projectManager)
		{
			this.projectManager = projectManager;
			this._wixProjectItems = new List<WiXProjectItem>();
			this._supportedEntities = new List<WiXEntity>();
			this._languages = new List<WiXLocalization>();
			this._customVariables = new WiXCustomVariables(this);
			this._fileSystem = new VSFolders(this, null);
			this._features = new List<VSFeature>();
			this._featureGroups = new List<VSFeatureGroup>();
			this._componentGroups = new VSComponentGroups(this, null);
			this._launchConditions = new VSLaunchConditions(this);
			this._searches = new VSSearches(this);
			this._icons = new List<WiXIcon>();
			this._webSetupParameters = new VSWebSetupParameters();
			this._fileTypes = new VSFileTypes(this);
			this._binaries = new List<VSBinary>();
			this._customActions = new VSCustomActions(this);
			this._mergeModules = new List<VSMergeModule>();
			this._mergeModuleReferences = new List<VSMergeModuleReference>();
			this._registry = new VSRegistry(this);
			this._userInterface = new VSUserInterface(this);
		}

		private void AddDirectories(VSBaseFolder parent, WiXEntityList list)
		{
			if (list.Count == 0)
			{
				return;
			}
			List<WiXDirectory> wiXDirectories = list.FindAll((WiXEntity e) => e is WiXDirectory).ConvertAll<WiXDirectory>((WiXEntity e) => e as WiXDirectory);
			List<IISWebVirtualDir> iSWebVirtualDirs = this._supportedEntities.FindAll((WiXEntity e) => e is IISWebVirtualDir).ConvertAll<IISWebVirtualDir>((WiXEntity e) => e as IISWebVirtualDir);
			foreach (WiXDirectory wiXDirectory in wiXDirectories)
			{
				if (wiXDirectory.Id.Contains(".") && wiXDirectory.GetAttributeValue("Name") == null)
				{
					continue;
				}
				VSBaseFolder folderById = this.FileSystem.GetFolderById(wiXDirectory.Id);
				if (folderById == null)
				{
					if (!(parent is VSWebFolder))
					{
						IISWebVirtualDir iSWebVirtualDir = iSWebVirtualDirs.Find((IISWebVirtualDir e) => wiXDirectory.Id == e.Directory);
						if (iSWebVirtualDir == null)
						{
							folderById = new VSFolder(this, parent, wiXDirectory);
						}
						else
						{
							VSWebCustomFolder vSWebCustomFolder = new VSWebCustomFolder(this, null, wiXDirectory);
							parent.Folders.Add(vSWebCustomFolder);
							iSWebVirtualDirs.Remove(iSWebVirtualDir);
						}
					}
					else
					{
						folderById = new VSWebFolder(this, parent, wiXDirectory);
					}
					parent.Folders.Add(folderById);
				}
				this.AddDirectories(folderById, wiXDirectory.ChildEntities);
				this._supportedEntities.Remove(wiXDirectory);
			}
		}

		internal WiXIcon AddIcon(string filePath)
		{
			WiXIcon wiXIcon = this.Icons.Find((WiXIcon e) => e.SourceFile == filePath);
			if (wiXIcon != null)
			{
				return wiXIcon;
			}
			wiXIcon = this.Icons.Find((WiXIcon e) => e.Id == filePath);
			if (wiXIcon != null)
			{
				return wiXIcon;
			}
			if (this.Icons.Count <= 0)
			{
				WiXEntity wiXEntityById = null;
				VSBaseFolder folderById = this.FileSystem.GetFolderById("TARGETDIR");
				if (folderById == null)
				{
					foreach (WiXProjectItem _wixProjectItem in this._wixProjectItems)
					{
						wiXEntityById = _wixProjectItem.GetWiXEntityById("Directory", "TARGETDIR");
						if (wiXEntityById == null)
						{
							continue;
						}
						goto Label0;
					}
				}
				else
				{
					wiXEntityById = folderById.WiXElement;
				}
			Label0:
				if (wiXEntityById != null)
				{
					XmlNode xmlNodes = Common.CreateXmlElementWithAttributes(wiXEntityById.XmlNode.OwnerDocument, "Icon", wiXEntityById.XmlNode.NamespaceURI, new string[] { "Id", "SourceFile" }, new string[] { Common.GenerateIconId(), filePath }, "", false);
					(wiXEntityById.Parent as WiXEntity).XmlNode.InsertBefore(xmlNodes, wiXEntityById.XmlNode);
					wiXEntityById.Parent.SetDirty();
					wiXIcon = new WiXIcon(this, wiXEntityById.Owner, wiXEntityById.Parent, xmlNodes);
				}
			}
			else
			{
				WiXEntity item = this.Icons[this.Icons.Count - 1];
				XmlNode xmlNodes1 = Common.CreateXmlElementWithAttributes(item.XmlNode.OwnerDocument, "Icon", item.XmlNode.NamespaceURI, new string[] { "Id", "SourceFile" }, new string[] { Common.GenerateIconId(), filePath }, "", false);
				(item.Parent as WiXEntity).XmlNode.InsertAfter(xmlNodes1, item.XmlNode);
				item.Parent.SetDirty();
				wiXIcon = new WiXIcon(this, item.Owner, item.Parent, xmlNodes1);
			}
			if (wiXIcon != null)
			{
				this.Icons.Add(wiXIcon);
			}
			return wiXIcon;
		}

		internal VSMergeModule AddMergeModule(string modulePath)
		{
			VSFeature item;
			if (this.Features.Count > 0)
			{
				item = this.Features[0];
			}
			else
			{
				item = null;
			}
			VSFeature vSFeature = item;
			VSBaseFolder folderById = this.FileSystem.GetFolderById("TARGETDIR");
			if (vSFeature == null || folderById == null)
			{
				return null;
			}
			string str = string.Concat("merge_", Common.GenerateId(this.ProjectType));
			string empty = string.Empty;
			using (MsiHelper msiHelper = new MsiHelper())
			{
				empty = msiHelper.GetModuleSignatureInfo(modulePath, 2);
			}
			XmlElement xmlElement = Common.CreateXmlElementWithAttributes(vSFeature.WiXElement.XmlNode.OwnerDocument, "MergeRef", vSFeature.WiXElement.XmlNode.NamespaceURI, new string[] { "Id" }, new string[] { str }, "", false);
			WiXMergeRef wiXMergeRef = new WiXMergeRef(this, vSFeature.WiXElement.Owner, vSFeature.WiXElement, xmlElement);
			vSFeature.WiXElement.XmlNode.AppendChild(xmlElement);
			vSFeature.WiXElement.SetDirty();
			XmlDocument ownerDocument = folderById.WiXElement.XmlNode.OwnerDocument;
			string namespaceURI = folderById.WiXElement.XmlNode.NamespaceURI;
			string[] strArrays = new string[] { "Id", "Language", "DiskId", "SourceFile" };
			string[] strArrays1 = new string[] { str, null, null, null };
			strArrays1[1] = (string.IsNullOrEmpty(empty) ? "0" : empty);
			strArrays1[2] = "1";
			strArrays1[3] = modulePath;
			XmlElement xmlElement1 = Common.CreateXmlElementWithAttributes(ownerDocument, "Merge", namespaceURI, strArrays, strArrays1, "", false);
			WiXMerge wiXMerge = new WiXMerge(this, folderById.WiXElement.Owner, folderById.WiXElement, xmlElement1);
			folderById.WiXElement.XmlNode.AppendChild(xmlElement1);
			folderById.WiXElement.SetDirty();
			VSMergeModule vSMergeModule = new VSMergeModule(this, wiXMerge, wiXMergeRef, folderById);
			this.MergeModules.Add(vSMergeModule);
			return vSMergeModule;
		}

		internal VSMergeModuleReference AddMergeModuleReference(string modulePath)
		{
			string str;
			string str1;
			string str2;
			using (MsiHelper msiHelper = new MsiHelper())
			{
				msiHelper.GetModuleSignatureInfo(modulePath, out str, out str1, out str2);
			}
			if (!string.IsNullOrEmpty(str) && !string.IsNullOrEmpty(str1))
			{
				WiXPackage wiXPackage = this.SupportedEntities.Find((WiXEntity e) => e is WiXPackage) as WiXPackage;
				if (wiXPackage != null)
				{
					XmlElement xmlElement = Common.CreateXmlElementWithAttributes(wiXPackage.XmlNode.OwnerDocument, "Dependency", wiXPackage.XmlNode.NamespaceURI, new string[] { "RequiredId", "RequiredLanguage", "RequiredVersion" }, new string[] { str, str1, str2 }, "", false);
					WiXDependency wiXDependency = new WiXDependency(this, wiXPackage.Owner, wiXPackage.Parent, xmlElement);
					(wiXPackage.Parent as WiXEntity).XmlNode.InsertAfter(xmlElement, wiXPackage.XmlNode);
					wiXPackage.Parent.SetDirty();
					VSMergeModuleReference vSMergeModuleReference = new VSMergeModuleReference(this, wiXDependency, modulePath);
					this.MergeModuleReferences.Add(vSMergeModuleReference);
					return vSMergeModuleReference;
				}
			}
			return null;
		}

		internal WiXProjectItem AddWiXSourceFile(string filePath, XmlDocument doc)
		{
			WiXProjectItem wiXProjectItem = null;
			if (doc != null && doc.DocumentElement != null)
			{
				wiXProjectItem = new WiXProjectItem(this, filePath, doc);
				this._wixProjectItems.Add(wiXProjectItem);
			}
			return wiXProjectItem;
		}

		internal void Clear()
		{
			this._wixProjectItems.Clear();
			this._supportedEntities.Clear();
			this._languages.Clear();
			this._customVariables.Clear();
			this._fileSystem.Clear();
			this._features.Clear();
			this._featureGroups.Clear();
			this._componentGroups.Clear();
			this._launchConditions.Clear();
			this._searches.Clear();
			this._icons.Clear();
			this._webSetupParameters.Clear();
			this._fileTypes.Clear();
			this._binaries.Clear();
			this._customActions.Clear();
			this._mergeModules.Clear();
			this._mergeModuleReferences.Clear();
			this._registry.Clean();
			this._userInterface.Clean();
		}

		private void CreateVSDialogs(DialogScope scope, List<WiXEntity> sequenceItems, List<WiXDialog> dialogList, List<WiXProperty> propertyList)
		{
			Predicate<WiXDialog> predicate = null;
			foreach (WiXEntity sequenceItem in sequenceItems)
			{
				if (!(sequenceItem is WiXShow))
				{
					continue;
				}
				string attributeValue = sequenceItem.GetAttributeValue("Dialog");
				if (string.IsNullOrEmpty(attributeValue))
				{
					continue;
				}
				List<WiXDialog> wiXDialogs = dialogList;
				Predicate<WiXDialog> predicate1 = predicate;
				if (predicate1 == null)
				{
					Predicate<WiXDialog> id = (WiXDialog e) => e.Id == attributeValue;
					Predicate<WiXDialog> predicate2 = id;
					predicate = id;
					predicate1 = predicate2;
				}
				WiXDialog wiXDialog = wiXDialogs.Find(predicate1);
				if (wiXDialog == null || VSDialogBase.IsBasicDialog(wiXDialog))
				{
					continue;
				}
				int num = 0;
				DialogStage installStage = VSDialogBase.GetInstallStage((WiXShow)sequenceItem, sequenceItems, ref num);
				if (installStage == DialogStage.Undefined)
				{
					continue;
				}
				VSDialogBase.CreateDialog(this, sequenceItems, this.UserInterface, wiXDialog, scope, installStage, num, dialogList, propertyList);
			}
		}

		internal void DoError(string message)
		{
			Trace.WriteLine(message);
		}

		internal void DoError(Exception e)
		{
			this.DoError(e.Message);
			DTEHelperObject.ShowErrorDialog(this, e);
		}

		private int GetSequence(WiXEntity wixEntity)
		{
			int num;
			string attributeValue = wixEntity.GetAttributeValue("Sequence");
			if (!string.IsNullOrEmpty(attributeValue) && int.TryParse(attributeValue, out num))
			{
				return num;
			}
			return 0;
		}

		internal void OnBeforeShowError(Exception error, ref bool cancelErrorView)
		{
			if (this.BeforeShowError != null)
			{
				this.BeforeShowError(error, ref cancelErrorView);
			}
		}

		internal void OnFileAdded(string filePath)
		{
			this.IsDirty = true;
			if (this.FileAdded != null)
			{
				this.FileAdded(filePath);
			}
		}

		internal void OnFileRemoved(string filePath)
		{
			this.IsDirty = true;
			if (this.FileRemoved != null)
			{
				this.FileRemoved(filePath);
			}
		}

		internal void OnReferenceAdded(VsWiXProject.ReferenceDescriptor desc)
		{
			if (this.ReferenceAdded != null)
			{
				this.ReferenceAdded(desc);
			}
		}

		internal void OnReferenceRefreshed(VsWiXProject.ReferenceDescriptor reference)
		{
			if (this.ReferenceRefreshed != null)
			{
				this.ReferenceRefreshed(reference);
			}
		}

		internal void OnReferenceRemoved(VsWiXProject.ReferenceDescriptor desc)
		{
			if (this.ReferenceRemoved != null)
			{
				this.ReferenceRemoved(desc);
			}
		}

		internal void OnReferenceRenamed(VsWiXProject.ReferenceDescriptor oldRefDesc, VsWiXProject.ReferenceDescriptor newRefDesc)
		{
			if (this.ReferenceRenamed != null)
			{
				this.ReferenceRenamed(oldRefDesc, newRefDesc);
			}
		}

		internal void OnThemeChanged()
		{
			if (this.ThemeChanged != null)
			{
				this.ThemeChanged();
			}
		}

		internal void OnToolWindowActivate()
		{
			if (this.ToolWindowActivate != null)
			{
				this.ToolWindowActivate();
			}
		}

		internal void OnToolWindowBeforeClose(int paneId, bool userAction)
		{
			if (this.ToolWindowBeforeClose != null)
			{
				this.ToolWindowBeforeClose(paneId, userAction);
			}
		}

		internal void OnToolWindowCreated(int paneId, int x, int y, int cx, int cy)
		{
			if (this.ToolWindowCreated != null)
			{
				this.ToolWindowCreated(paneId, x, y, cx, cy);
			}
		}

		internal void OnToolWindowDeactivate()
		{
			if (this.ToolWindowDeactivate != null)
			{
				this.ToolWindowDeactivate();
			}
		}

		internal void OnViewTextModelChanged(int paneId)
		{
			Dictionary<string, string> strs;
			if (this.ViewTextModelChanged != null)
			{
				this.ProjectManager.OpenWiXFiles(paneId, out strs);
				this.ViewTextModelChanged(paneId, strs);
			}
		}

		internal void OnViewXmlModelChanged(int paneId)
		{
			Dictionary<string, XmlDocument> strs;
			if (this.ViewXmlModelChanged != null)
			{
				this.ProjectManager.OpenWiXFiles(paneId, out strs);
				this.ViewXmlModelChanged(paneId, strs);
			}
		}

		internal void Parse()
		{
			if (this.BeforeParse != null)
			{
				this.BeforeParse();
			}
			this.ParseCustomVariables();
			this.ParseWebSetupParameters();
			this.ParseFeatures();
			this.ParseRootFolders();
			this.ParseSubFolders();
			this.ParseProjectOutputs();
			this.ParseMergeModules();
			this.ParseMergeModuleReferences();
			this.ParseComponents();
			this.ParseComponentRefs();
			this.ParseComponentGroups();
			this.ParseComponentKeyPath();
			this.ParseCreateRemoveFolders();
			this.ParseSearches();
			this.ParseConditions();
			this.ParseFiles();
			this.ParseShortcuts();
			this.ParseIcons();
			this.ParseFileTypes();
			this.ParseCustomActions();
			this.ParseRegistry();
			this.ParseUserInterface();
			this.dirty = false;
			if (this.AfterParse != null)
			{
				this.AfterParse();
			}
		}

		private void ParseComponentGroups()
		{
			List<WiXComponentGroup> wiXComponentGroups = this._supportedEntities.FindAll((WiXEntity e) => e is WiXComponentGroup).ConvertAll<WiXComponentGroup>((WiXEntity e) => e as WiXComponentGroup);
			List<WiXComponentGroupRef> wiXComponentGroupRefs = this._supportedEntities.FindAll((WiXEntity e) => e is WiXComponentGroupRef).ConvertAll<WiXComponentGroupRef>((WiXEntity e) => e as WiXComponentGroupRef);
			if (wiXComponentGroups != null && wiXComponentGroupRefs != null)
			{
				foreach (WiXComponentGroup wiXComponentGroup in wiXComponentGroups)
				{
					bool flag = false;
					foreach (WiXComponentGroupRef wiXComponentGroupRef in wiXComponentGroupRefs)
					{
						if (wiXComponentGroup.Id != wiXComponentGroupRef.Id)
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
					this._supportedEntities.Remove(wiXComponentGroup);
				}
			}
			wiXComponentGroups = this._supportedEntities.FindAll((WiXEntity e) => e is WiXComponentGroup).ConvertAll<WiXComponentGroup>((WiXEntity e) => e as WiXComponentGroup);
			if (wiXComponentGroups != null)
			{
				foreach (WiXComponentGroup wiXComponentGroup1 in wiXComponentGroups)
				{
					this.ComponentGroups.Add(new VSComponentGroup(null, wiXComponentGroup1));
					this._supportedEntities.Remove(wiXComponentGroup1);
				}
			}
			wiXComponentGroupRefs = this._supportedEntities.FindAll((WiXEntity e) => e is WiXComponentGroupRef).ConvertAll<WiXComponentGroupRef>((WiXEntity e) => e as WiXComponentGroupRef);
			if (wiXComponentGroupRefs != null)
			{
				foreach (WiXComponentGroupRef wiXComponentGroupRef1 in wiXComponentGroupRefs)
				{
					VSComponentGroup vSComponentGroup = this.ComponentGroups.Find((VSComponentGroup e) => e.Id == wiXComponentGroupRef1.Id);
					if (vSComponentGroup == null)
					{
						continue;
					}
					vSComponentGroup.ComponentGroupRefs.Add(wiXComponentGroupRef1);
				}
			}
		}

		private void ParseComponentKeyPath()
		{
			List<WiXRegistryValue> wiXRegistryValues = this._supportedEntities.FindAll((WiXEntity e) => {
				if (!(e is WiXRegistryValue) || !((e as WiXRegistryValue).KeyPath == "yes") || (e as WiXRegistryValue).Parent == null)
				{
					return false;
				}
				return (e as WiXRegistryValue).Parent is WiXComponent;
			}).ConvertAll<WiXRegistryValue>((WiXEntity e) => e as WiXRegistryValue);
			if (wiXRegistryValues != null && wiXRegistryValues.Count > 0)
			{
				foreach (WiXRegistryValue wiXRegistryValue in wiXRegistryValues)
				{
					VSComponent componentById = this.FileSystem.GetComponentById((wiXRegistryValue.Parent as WiXComponent).Id);
					if (componentById == null)
					{
						continue;
					}
					componentById.WiXKeyPathElement = wiXRegistryValue;
					this._supportedEntities.Remove(wiXRegistryValue);
				}
			}
		}

		private void ParseComponentRefs()
		{
			List<WiXComponentRef> wiXComponentRefs = this._supportedEntities.FindAll((WiXEntity e) => e is WiXComponentRef).ConvertAll<WiXComponentRef>((WiXEntity e) => e as WiXComponentRef);
			if (wiXComponentRefs != null)
			{
				foreach (WiXComponentRef wiXComponentRef in wiXComponentRefs)
				{
					VSComponent componentById = this.FileSystem.GetComponentById(wiXComponentRef.Id);
					if (componentById == null)
					{
						continue;
					}
					componentById.ComponentRefs.Add(wiXComponentRef);
					this._supportedEntities.Remove(wiXComponentRef);
				}
			}
		}

		private void ParseComponents()
		{
			List<WiXComponent> wiXComponents = this._supportedEntities.FindAll((WiXEntity e) => {
				if (!(e is WiXComponent) || (e as WiXComponent).Parent == null)
				{
					return false;
				}
				return (e as WiXComponent).Parent is WiXDirectory;
			}).ConvertAll<WiXComponent>((WiXEntity e) => e as WiXComponent);
			if (wiXComponents != null)
			{
				foreach (WiXComponent wiXComponent in wiXComponents)
				{
					VSBaseFolder folderById = this._fileSystem.GetFolderById((wiXComponent.Parent as WiXDirectory).Id);
					if (folderById == null)
					{
						continue;
					}
					folderById.Components.Add(new VSComponent(folderById, wiXComponent));
					this._supportedEntities.Remove(wiXComponent);
				}
			}
			wiXComponents = this._supportedEntities.FindAll((WiXEntity e) => {
				if (!(e is WiXComponent) || (e as WiXComponent).Parent == null)
				{
					return false;
				}
				return (e as WiXComponent).Parent is WiXDirectoryRef;
			}).ConvertAll<WiXComponent>((WiXEntity e) => e as WiXComponent);
			if (wiXComponents != null)
			{
				foreach (WiXComponent wiXComponent1 in wiXComponents)
				{
					VSBaseFolder vSBaseFolder = this._fileSystem.GetFolderById((wiXComponent1.Parent as WiXDirectoryRef).Id);
					if (vSBaseFolder == null)
					{
						continue;
					}
					vSBaseFolder.Components.Add(new VSComponent(vSBaseFolder, wiXComponent1));
					this._supportedEntities.Remove(wiXComponent1);
				}
			}
			wiXComponents = this._supportedEntities.FindAll((WiXEntity e) => {
				if (!(e is WiXComponent))
				{
					return false;
				}
				return !string.IsNullOrEmpty((e as WiXComponent).Directory);
			}).ConvertAll<WiXComponent>((WiXEntity e) => e as WiXComponent);
			if (wiXComponents != null)
			{
				foreach (WiXComponent wiXComponent2 in wiXComponents)
				{
					VSBaseFolder folderById1 = this._fileSystem.GetFolderById(wiXComponent2.Directory);
					if (folderById1 == null)
					{
						continue;
					}
					folderById1.Components.Add(new VSComponent(folderById1, wiXComponent2));
					this._supportedEntities.Remove(wiXComponent2);
				}
			}
			wiXComponents = this._supportedEntities.FindAll((WiXEntity e) => {
				if (!(e is WiXComponent) || (e as WiXComponent).Parent == null || !((e as WiXComponent).Parent is WiXComponentGroup))
				{
					return false;
				}
				return !string.IsNullOrEmpty(((e as WiXComponent).Parent as WiXComponentGroup).Directory);
			}).ConvertAll<WiXComponent>((WiXEntity e) => e as WiXComponent);
			if (wiXComponents != null)
			{
				foreach (WiXComponent wiXComponent3 in wiXComponents)
				{
					VSBaseFolder vSBaseFolder1 = this._fileSystem.GetFolderById((wiXComponent3.Parent as WiXComponentGroup).Directory);
					if (vSBaseFolder1 == null)
					{
						continue;
					}
					vSBaseFolder1.Components.Add(new VSComponent(vSBaseFolder1, wiXComponent3));
					this._supportedEntities.Remove(wiXComponent3);
				}
			}
		}

		private void ParseConditions()
		{
			List<WiXCondition> wiXConditions = this._supportedEntities.FindAll((WiXEntity e) => e is WiXCondition).ConvertAll<WiXCondition>((WiXEntity e) => e as WiXCondition);
			if (wiXConditions != null)
			{
				foreach (WiXCondition wiXCondition in wiXConditions)
				{
					if (wiXCondition.Parent is WiXComponent)
					{
						VSComponent componentById = this.FileSystem.GetComponentById((wiXCondition.Parent as WiXComponent).Id);
						if (componentById != null)
						{
							componentById.WiXCondition = wiXCondition;
							this._supportedEntities.Remove(wiXCondition);
						}
					}
					if (!(wiXCondition.Parent is WiXFragment) && !(wiXCondition.Parent is WiXProduct))
					{
						continue;
					}
					VSLaunchCondition vSLaunchCondition = new VSLaunchCondition(this, wiXCondition, this._launchConditions);
					this._launchConditions.Add(vSLaunchCondition);
					this._supportedEntities.Remove(wiXCondition);
				}
			}
		}

		private void ParseCreateRemoveFolders()
		{
			List<WiXCreateFolder> wiXCreateFolders = this._supportedEntities.FindAll((WiXEntity e) => e is WiXCreateFolder).ConvertAll<WiXCreateFolder>((WiXEntity e) => e as WiXCreateFolder);
			if (wiXCreateFolders != null)
			{
				foreach (WiXCreateFolder wiXCreateFolder in wiXCreateFolders)
				{
					VSBaseFolder folderById = null;
					if (string.IsNullOrEmpty(wiXCreateFolder.Directory))
					{
						WiXComponent parent = wiXCreateFolder.Parent as WiXComponent;
						if (parent != null)
						{
							if (!string.IsNullOrEmpty(parent.Directory))
							{
								folderById = this.FileSystem.GetFolderById(parent.Directory);
							}
							else if (parent.Parent is WiXDirectory)
							{
								folderById = this.FileSystem.GetFolderById((parent.Parent as WiXDirectory).Id);
							}
							else if (parent.Parent is WiXDirectoryRef)
							{
								folderById = this.FileSystem.GetFolderById((parent.Parent as WiXDirectoryRef).Id);
							}
						}
					}
					else
					{
						folderById = this.FileSystem.GetFolderById(wiXCreateFolder.Directory);
					}
					if (folderById == null)
					{
						continue;
					}
					folderById.WiXCreateFolder = wiXCreateFolder;
					this._supportedEntities.Remove(wiXCreateFolder);
				}
			}
			List<WiXRemoveFolder> wiXRemoveFolders = this._supportedEntities.FindAll((WiXEntity e) => e is WiXRemoveFolder).ConvertAll<WiXRemoveFolder>((WiXEntity e) => e as WiXRemoveFolder);
			if (wiXRemoveFolders != null)
			{
				foreach (WiXRemoveFolder wiXRemoveFolder in wiXRemoveFolders)
				{
					VSBaseFolder vSBaseFolder = null;
					if (string.IsNullOrEmpty(wiXRemoveFolder.Directory))
					{
						WiXComponent wiXComponent = wiXRemoveFolder.Parent as WiXComponent;
						if (wiXComponent != null)
						{
							if (!string.IsNullOrEmpty(wiXComponent.Directory))
							{
								vSBaseFolder = this.FileSystem.GetFolderById(wiXComponent.Directory);
							}
							else if (wiXComponent.Parent is WiXDirectory)
							{
								vSBaseFolder = this.FileSystem.GetFolderById((wiXComponent.Parent as WiXDirectory).Id);
							}
							else if (wiXComponent.Parent is WiXDirectoryRef)
							{
								vSBaseFolder = this.FileSystem.GetFolderById((wiXComponent.Parent as WiXDirectoryRef).Id);
							}
						}
					}
					else
					{
						vSBaseFolder = this.FileSystem.GetFolderById(wiXRemoveFolder.Directory);
					}
					if (vSBaseFolder == null)
					{
						continue;
					}
					vSBaseFolder.WiXRemoveFolder = wiXRemoveFolder;
					this._supportedEntities.Remove(wiXRemoveFolder);
				}
			}
		}

		private void ParseCustomActions()
		{
			List<WiXCustomAction> wiXCustomActions = this._supportedEntities.FindAll((WiXEntity e) => e is WiXCustomAction).ConvertAll<WiXCustomAction>((WiXEntity e) => e as WiXCustomAction);
			if (wiXCustomActions == null || wiXCustomActions.Count == 0)
			{
				return;
			}
			List<WiXCustom> wiXCustoms = this._supportedEntities.FindAll((WiXEntity e) => e is WiXCustom).ConvertAll<WiXCustom>((WiXEntity e) => e as WiXCustom);
			if (wiXCustoms == null || wiXCustoms.Count == 0)
			{
				return;
			}
			foreach (WiXCustomAction wiXCustomAction in wiXCustomActions)
			{
				if (wiXCustomAction.Id.ToLower().Contains("ca_createconfig") || wiXCustomAction.Id.Contains(".SetProperty") || string.IsNullOrEmpty(wiXCustomAction.FileKey) && string.IsNullOrEmpty(wiXCustomAction.BinaryKey))
				{
					continue;
				}
				VSBaseFile fileById = null;
				if (!string.IsNullOrEmpty(wiXCustomAction.FileKey))
				{
					fileById = this.FileSystem.GetFileById(wiXCustomAction.FileKey);
				}
				if (!string.IsNullOrEmpty(wiXCustomAction.BinaryKey))
				{
					fileById = this.Binaries.Find((VSBinary e) => {
						if (e.WiXElement == null)
						{
							return false;
						}
						return e.WiXElement.Id == wiXCustomAction.BinaryKey;
					});
				}
				if (fileById == null)
				{
					continue;
				}
				string empty = string.Empty;
				if (fileById is VSFile)
				{
					empty = (fileById as VSFile).SourcePath;
				}
				if (fileById is VSBinary)
				{
					empty = (fileById as VSBinary).SourcePath;
				}
				if (fileById is VSProjectOutputVDProj && (fileById as VSProjectOutputVDProj).KeyOutput != null)
				{
					empty = (fileById as VSProjectOutputVDProj).KeyOutput.SourcePath;
				}
				if (string.IsNullOrEmpty(empty))
				{
					continue;
				}
				WiXCustomAction wiXCustomAction1 = wiXCustomActions.Find((WiXCustomAction e) => e.Id == string.Concat(wiXCustomAction.Id, ".SetProperty"));
				WiXCustom wiXCustom = wiXCustoms.Find((WiXCustom e) => e.Action == wiXCustomAction.Id);
				WiXCustom wiXCustom1 = wiXCustoms.Find((WiXCustom e) => e.Action == string.Concat(wiXCustomAction.Id, ".SetProperty"));
				VSCustomActionBase vSCustomActionEXE = null;
				if (empty.ToLower().EndsWith(".exe") || empty.ToLower().EndsWith(".bat"))
				{
					vSCustomActionEXE = new VSCustomActionEXE(this, this.CustomActions, wiXCustomAction, wiXCustomAction1, wiXCustom, wiXCustom1, fileById);
				}
				if (empty.ToLower().EndsWith(".dll"))
				{
					vSCustomActionEXE = new VSCustomActionDLL(this, this.CustomActions, wiXCustomAction, wiXCustomAction1, wiXCustom, wiXCustom1, fileById);
				}
				if (empty.ToLower().EndsWith(".vbs") || empty.ToLower().EndsWith(".js"))
				{
					vSCustomActionEXE = new VSCustomActionScript(this, this.CustomActions, wiXCustomAction, wiXCustomAction1, wiXCustom, wiXCustom1, fileById);
				}
				if (!(wiXCustomAction.BinaryKey == "InstallUtil") || !empty.EndsWith("InstallUtil") && !empty.EndsWith("InstallUtil64") || wiXCustomAction1 == null || string.IsNullOrEmpty(wiXCustomAction1.Value))
				{
					continue;
				}
				int length = wiXCustomAction1.Value.IndexOf("[#");
				if (length < 0)
				{
					continue;
				}
				length += "[#".Length;
				int num = wiXCustomAction1.Value.IndexOf("]", length);
				if (num < 0 || num <= length)
				{
					continue;
				}
				string str = wiXCustomAction1.Value.Substring(length, num - length);
				if (string.IsNullOrEmpty(str))
				{
					continue;
				}
				fileById = this.FileSystem.GetFileById(str);
				if (fileById == null)
				{
					fileById = this.Binaries.Find((VSBinary e) => {
						if (e.WiXElement == null)
						{
							return false;
						}
						return e.WiXElement.Id == str;
					});
				}
				if (fileById == null)
				{
					continue;
				}
				empty = string.Empty;
				if (fileById is VSFile)
				{
					empty = (fileById as VSFile).SourcePath;
				}
				if (fileById is VSBinary)
				{
					empty = (fileById as VSBinary).SourcePath;
				}
				if (fileById is VSProjectOutputVDProj && (fileById as VSProjectOutputVDProj).KeyOutput != null)
				{
					empty = (fileById as VSProjectOutputVDProj).KeyOutput.SourcePath;
				}
				if (string.IsNullOrEmpty(empty))
				{
					continue;
				}
				if (empty.ToLower().EndsWith(".exe"))
				{
					vSCustomActionEXE = new VSCustomActionEXE(this, this.CustomActions, wiXCustomAction, wiXCustomAction1, wiXCustom, wiXCustom1, fileById);
				}
				if (empty.ToLower().EndsWith(".dll"))
				{
					vSCustomActionEXE = new VSCustomActionDLL(this, this.CustomActions, wiXCustomAction, wiXCustomAction1, wiXCustom, wiXCustom1, fileById);
				}
				if (vSCustomActionEXE == null)
				{
					continue;
				}
				vSCustomActionEXE._isManaged = true;
			}
			this.CustomActions.SortCustomActions();
		}

		private void ParseCustomVariables()
		{
			List<WiXEntity> wiXEntities = new List<WiXEntity>();
			foreach (WiXProjectItem projectItem in this.ProjectItems)
			{
				wiXEntities = projectItem.GetWiXEntitiesByXmlTypeAndName(typeof(XmlProcessingInstruction), "define", wiXEntities);
			}
			this._customVariables.Parse(wiXEntities);
		}

		private void ParseFeatures()
		{
			List<WiXFeature> wiXFeatures = this._supportedEntities.FindAll((WiXEntity e) => e is WiXFeature).ConvertAll<WiXFeature>((WiXEntity e) => e as WiXFeature);
			if (wiXFeatures != null)
			{
				foreach (WiXFeature wiXFeature in wiXFeatures)
				{
					this.Features.Add(new VSFeature(this, wiXFeature));
					this._supportedEntities.Remove(wiXFeature);
				}
			}
			List<WiXFeatureRef> wiXFeatureRefs = this._supportedEntities.FindAll((WiXEntity e) => e is WiXFeatureRef).ConvertAll<WiXFeatureRef>((WiXEntity e) => e as WiXFeatureRef);
			if (wiXFeatureRefs != null)
			{
				foreach (WiXFeatureRef wiXFeatureRef in wiXFeatureRefs)
				{
					if (string.IsNullOrEmpty(wiXFeatureRef.Id))
					{
						continue;
					}
					VSFeature vSFeature = this.Features.Find((VSFeature e) => e.Id == wiXFeatureRef.Id);
					if (vSFeature == null)
					{
						continue;
					}
					vSFeature.FeatureRefs.Add(wiXFeatureRef);
				}
			}
			List<WiXFeatureGroup> wiXFeatureGroups = this._supportedEntities.FindAll((WiXEntity e) => e is WiXFeatureGroup).ConvertAll<WiXFeatureGroup>((WiXEntity e) => e as WiXFeatureGroup);
			if (wiXFeatureGroups != null)
			{
				foreach (WiXFeatureGroup wiXFeatureGroup in wiXFeatureGroups)
				{
					this.FeatureGroups.Add(new VSFeatureGroup(this, wiXFeatureGroup));
					this._supportedEntities.Remove(wiXFeatureGroup);
				}
			}
			List<WiXFeatureGroupRef> wiXFeatureGroupRefs = this._supportedEntities.FindAll((WiXEntity e) => e is WiXFeatureGroupRef).ConvertAll<WiXFeatureGroupRef>((WiXEntity e) => e as WiXFeatureGroupRef);
			if (wiXFeatureGroupRefs != null)
			{
				foreach (WiXFeatureGroupRef wiXFeatureGroupRef in wiXFeatureGroupRefs)
				{
					if (string.IsNullOrEmpty(wiXFeatureGroupRef.Id))
					{
						continue;
					}
					VSFeatureGroup vSFeatureGroup = this.FeatureGroups.Find((VSFeatureGroup e) => e.Id == wiXFeatureGroupRef.Id);
					if (vSFeatureGroup == null)
					{
						continue;
					}
					vSFeatureGroup.FeatureGroupRefs.Add(wiXFeatureGroupRef);
				}
			}
		}

		private void ParseFiles()
		{
			string str = "$(var.";
			List<WiXFile> wiXFiles = this._supportedEntities.FindAll((WiXEntity e) => e is WiXFile).ConvertAll<WiXFile>((WiXEntity e) => e as WiXFile);
			if (wiXFiles != null)
			{
				foreach (WiXFile wiXFile in wiXFiles)
				{
					if (!(wiXFile.Parent is WiXComponent))
					{
						continue;
					}
					VSComponent componentById = this.FileSystem.GetComponentById((wiXFile.Parent as WiXComponent).Id);
					if (componentById == null)
					{
						continue;
					}
					if (wiXFile.Name == null || !wiXFile.Name.ToLower().StartsWith(str) || wiXFile.Source == null || !wiXFile.Source.ToLower().StartsWith(str))
					{
						bool flag = false;
						string source = wiXFile.Source;
						if (!string.IsNullOrEmpty(source))
						{
							if (!Path.IsPathRooted(source))
							{
								source = Path.Combine(this.ProjectManager.RootDirectory, source);
							}
							flag = (!File.Exists(source) ? false : VSCustomActionBase.IsAssembly(source));
						}
						if (!flag)
						{
							componentById.Files.Add(new VSFile(this, componentById, wiXFile));
						}
						else
						{
							componentById.Files.Add(new VSAssembly(this, componentById, wiXFile));
						}
					}
					else
					{
						string empty = string.Empty;
						VsWiXProject.ReferenceDescriptor referenceDescriptor = null;
						if (wiXFile.Source != null && wiXFile.Source.ToLower().StartsWith(str))
						{
							empty = wiXFile.Source.Substring(str.Length);
						}
						else if (wiXFile.Name != null && wiXFile.Name.ToLower().StartsWith(str))
						{
							empty = wiXFile.Name.Substring(str.Length);
						}
						if (!string.IsNullOrEmpty(empty) && empty.Contains("."))
						{
							empty = empty.Substring(0, empty.IndexOf("."));
							foreach (VsWiXProject.ReferenceDescriptor value in this.ProjectManager.References.Values)
							{
								if (value.Caption != empty)
								{
									continue;
								}
								referenceDescriptor = value;
								goto Label0;
							}
						}
					Label0:
						if (referenceDescriptor == null)
						{
							bool flag1 = false;
							string source1 = wiXFile.Source;
							if (!string.IsNullOrEmpty(source1))
							{
								if (!Path.IsPathRooted(source1))
								{
									source1 = Path.Combine(this.ProjectManager.RootDirectory, source1);
								}
								flag1 = (!File.Exists(source1) ? false : VSCustomActionBase.IsAssembly(source1));
							}
							if (!flag1)
							{
								componentById.Files.Add(new VSFile(this, componentById, wiXFile));
							}
							else
							{
								componentById.Files.Add(new VSAssembly(this, componentById, wiXFile));
							}
						}
						else
						{
							componentById.Files.Add(new VSProjectOutputFile(this, componentById, wiXFile, referenceDescriptor));
						}
					}
					this._supportedEntities.Remove(wiXFile);
				}
			}
		}

		private void ParseFileTypes()
		{
			List<WiXProgId> wiXProgIds = this._supportedEntities.FindAll((WiXEntity e) => {
				if (!(e is WiXProgId) || (e as WiXProgId).Parent == null)
				{
					return false;
				}
				return (e as WiXProgId).Parent is WiXComponent;
			}).ConvertAll<WiXProgId>((WiXEntity e) => e as WiXProgId);
			if (wiXProgIds != null)
			{
				foreach (WiXProgId wiXProgId in wiXProgIds)
				{
					if (!(wiXProgId.Parent is WiXComponent))
					{
						continue;
					}
					VSComponent componentById = this.FileSystem.GetComponentById((wiXProgId.Parent as WiXComponent).Id);
					if (componentById == null)
					{
						continue;
					}
					this.FileTypes.Add(new VSFileType(this, componentById, wiXProgId));
					this._supportedEntities.Remove(wiXProgId);
				}
			}
		}

		private void ParseIcons()
		{
			List<WiXIcon> wiXIcons = this._supportedEntities.FindAll((WiXEntity e) => {
				if (!(e is WiXIcon) || (e as WiXIcon).Parent == null)
				{
					return false;
				}
				if ((e as WiXIcon).Parent is WiXFragment || (e as WiXIcon).Parent is WiXModule)
				{
					return true;
				}
				return (e as WiXIcon).Parent is WiXProduct;
			}).ConvertAll<WiXIcon>((WiXEntity e) => e as WiXIcon);
			if (wiXIcons != null)
			{
				foreach (WiXIcon wiXIcon in wiXIcons)
				{
					this._icons.Add(wiXIcon);
					this._supportedEntities.Remove(wiXIcon);
				}
			}
			wiXIcons = this._supportedEntities.FindAll((WiXEntity e) => {
				if (!(e is WiXIcon) || (e as WiXIcon).Parent == null)
				{
					return false;
				}
				return (e as WiXIcon).Parent is WiXShortcut;
			}).ConvertAll<WiXIcon>((WiXEntity e) => e as WiXIcon);
			if (wiXIcons != null)
			{
				foreach (WiXIcon wiXIcon1 in wiXIcons)
				{
					(wiXIcon1.Parent as WiXShortcut).IconElement = wiXIcon1;
					this._supportedEntities.Remove(wiXIcon1);
				}
			}
		}

		private void ParseMergeModuleReferences()
		{
			if (this._projectType == WiXProjectType.Module)
			{
				List<WiXDependency> wiXDependencies = this._supportedEntities.FindAll((WiXEntity e) => e is WiXDependency).ConvertAll<WiXDependency>((WiXEntity e) => e as WiXDependency);
				if (wiXDependencies != null && wiXDependencies.Count > 0)
				{
					foreach (WiXDependency wiXDependency in wiXDependencies)
					{
						VSMergeModuleReference vSMergeModuleReference = new VSMergeModuleReference(this, wiXDependency);
						this._mergeModuleReferences.Add(vSMergeModuleReference);
						this._supportedEntities.Remove(wiXDependency);
					}
				}
			}
		}

		private void ParseMergeModules()
		{
			if (this._projectType == WiXProjectType.Product)
			{
				List<WiXMerge> wiXMerges = this._supportedEntities.FindAll((WiXEntity e) => e is WiXMerge).ConvertAll<WiXMerge>((WiXEntity e) => e as WiXMerge);
				if (wiXMerges != null && wiXMerges.Count > 0)
				{
					foreach (WiXMerge wiXMerge in wiXMerges)
					{
						VSBaseFolder folderById = null;
						WiXMergeRef wiXMergeRef = this._supportedEntities.Find((WiXEntity e) => {
							if (!(e is WiXMergeRef))
							{
								return false;
							}
							return (e as WiXMergeRef).Id == wiXMerge.Id;
						}) as WiXMergeRef;
						if (wiXMerge.Parent is WiXDirectory)
						{
							folderById = this.FileSystem.GetFolderById((wiXMerge.Parent as WiXDirectory).Id);
						}
						else if (wiXMerge.Parent is WiXDirectoryRef)
						{
							folderById = this.FileSystem.GetFolderById((wiXMerge.Parent as WiXDirectoryRef).Id);
						}
						if (wiXMergeRef != null && folderById != null)
						{
							VSMergeModule vSMergeModule = new VSMergeModule(this, wiXMerge, wiXMergeRef, folderById);
							this._mergeModules.Add(vSMergeModule);
							this._supportedEntities.Remove(wiXMergeRef);
						}
						this._supportedEntities.Remove(wiXMerge);
					}
				}
			}
		}

		private void ParseProjectOutputs()
		{
			List<WiXDirectory> wiXDirectories = this._supportedEntities.FindAll((WiXEntity e) => e is WiXDirectory).ConvertAll<WiXDirectory>((WiXEntity e) => e as WiXDirectory);
			int num = 0;
			while (wiXDirectories.Count > 0)
			{
				WiXDirectory item = wiXDirectories[0];
				if (!item.Id.Contains(".") || item.GetAttributeValue("Name") != null)
				{
					num++;
					if (num <= 1000)
					{
						continue;
					}
					this.DoError(string.Concat("Error: Cannot find a parent for Directory with Id=", item.Id));
					return;
				}
				else if (item.Parent == null || !item.Parent.IsSupported || !(item.Parent.SupportedObject is WiXDirectory) && !(item.Parent.SupportedObject is WiXDirectoryRef))
				{
					num++;
					if (num <= 1000)
					{
						continue;
					}
					this.DoError(string.Concat("Error: Cannot find a parent for Directory with Id=", item.Id));
					return;
				}
				else
				{
					VSBaseFolder folderById = null;
					if (item.Parent.SupportedObject is WiXDirectory)
					{
						folderById = this.FileSystem.GetFolderById((item.Parent.SupportedObject as WiXDirectory).Id);
					}
					else if (item.Parent.SupportedObject is WiXDirectoryRef)
					{
						folderById = this.FileSystem.GetFolderById((item.Parent.SupportedObject as WiXDirectoryRef).Id);
					}
					if (folderById == null)
					{
						wiXDirectories.Remove(item);
					}
					else
					{
						VsWiXProject.ReferenceDescriptor referenceDescriptor = null;
						string str = item.Id.Remove(item.Id.LastIndexOf(".") + 1);
						foreach (VsWiXProject.ReferenceDescriptor value in this.ProjectManager.References.Values)
						{
							if (!(str == value.DirectoryIDPrefix) || !value.IsXSLTOutputsSupported)
							{
								continue;
							}
							referenceDescriptor = value;
							goto Label0;
						}
					Label0:
						if (referenceDescriptor == null)
						{
							folderById.ProjectOutputs.Add(new VSProjectOutputUnknown(this, folderById, item));
							this._supportedEntities.Remove(item);
							wiXDirectories.Remove(item);
						}
						else
						{
							WiXComponent wiXComponent = null;
							WiXComponentRef wiXComponentRef = null;
							if (item.ChildEntities != null && item.ChildEntities.Count > 0)
							{
								wiXComponent = this._supportedEntities.Find((WiXEntity e) => {
									if (!(e is WiXComponent))
									{
										return false;
									}
									return (e as WiXComponent).Parent == item;
								}) as WiXComponent;
								if (wiXComponent != null)
								{
									if (wiXComponent.HasChildEntities)
									{
										foreach (WiXEntity childEntity in wiXComponent.ChildEntities)
										{
											this._supportedEntities.Remove(childEntity);
										}
									}
									this._supportedEntities.Remove(wiXComponent);
									wiXComponentRef = this._supportedEntities.Find((WiXEntity e) => {
										if (!(e is WiXComponentRef))
										{
											return false;
										}
										return (e as WiXComponentRef).Id == wiXComponent.Id;
									}) as WiXComponentRef;
									if (wiXComponentRef != null)
									{
										this._supportedEntities.Remove(wiXComponentRef);
									}
								}
							}
							VSProjectOutputVDProj vSProjectOutputVDProj = new VSProjectOutputVDProj(this, folderById, item, referenceDescriptor, wiXComponent, wiXComponentRef);
							folderById.ProjectOutputs.Add(vSProjectOutputVDProj);
							this._supportedEntities.Remove(item);
							wiXDirectories.Remove(item);
							object projectOutputProperty = referenceDescriptor.GetProjectOutputProperty(vSProjectOutputVDProj.Group, OutputGroupProperties.FileTypes);
							if (projectOutputProperty == null)
							{
								continue;
							}
							foreach (VsWiXProject.ReferenceDescriptor.FileType fileType in projectOutputProperty as List<VsWiXProject.ReferenceDescriptor.FileType>)
							{
								this.FileTypes.Add(new VSFileType(this, vSProjectOutputVDProj, fileType));
							}
						}
					}
				}
			}
		}

		private void ParseRegistry()
		{
			string root;
			List<WiXRegistryKey> wiXRegistryKeys = this._supportedEntities.FindAll((WiXEntity e) => {
				if (!(e is WiXRegistryKey))
				{
					return false;
				}
				return (e as WiXRegistryKey).Parent is WiXComponent;
			}).ConvertAll<WiXRegistryKey>((WiXEntity e) => e as WiXRegistryKey);
			if (wiXRegistryKeys == null || wiXRegistryKeys.Count == 0)
			{
				return;
			}
			for (int i = wiXRegistryKeys.Count - 1; i >= 0; i--)
			{
				WiXRegistryKey item = wiXRegistryKeys[i];
				if (!string.IsNullOrEmpty(item.Root) && !string.IsNullOrEmpty(item.Key) && !item.Key.Contains("\\"))
				{
					VSComponent componentById = this.FileSystem.GetComponentById((item.Parent as WiXComponent).Id);
					if (componentById == null)
					{
						VSBaseFolder folderById = this.FileSystem.GetFolderById("MergeRedirectFolder") ?? this.FileSystem.GetFolderById("TARGETDIR") ?? this.FileSystem[0];
						if (folderById != null)
						{
							componentById = new VSComponent(folderById, item.Parent as WiXComponent);
							this._supportedEntities.Remove(item.Parent as WiXComponent);
							WiXCondition wiXCondition = this._supportedEntities.Find((WiXEntity e) => {
								if (!(e is WiXCondition))
								{
									return false;
								}
								return (e as WiXCondition).Parent == componentById.WiXElement;
							}) as WiXCondition;
							if (wiXCondition != null)
							{
								componentById.WiXCondition = wiXCondition;
								this._supportedEntities.Remove(wiXCondition);
							}
						}
					}
					root = item.Root;
					if (root != null)
					{
						if (root == "HKCR")
						{
							VSRegistryKey vSRegistryKey = new VSRegistryKey(this, item, componentById, this.Registry.HKCR);
						}
						else if (root == "HKCU")
						{
							VSRegistryKey vSRegistryKey1 = new VSRegistryKey(this, item, componentById, this.Registry.HKCU);
						}
						else if (root == "HKLM")
						{
							VSRegistryKey vSRegistryKey2 = new VSRegistryKey(this, item, componentById, this.Registry.HKLM);
						}
						else if (root == "HKU")
						{
							VSRegistryKey vSRegistryKey3 = new VSRegistryKey(this, item, componentById, this.Registry.HKU);
						}
						else if (root == "HKMU")
						{
							VSRegistryKey vSRegistryKey4 = new VSRegistryKey(this, item, componentById, this.Registry.HKMU);
						}
					}
					this._supportedEntities.Remove(item);
					wiXRegistryKeys.Remove(item);
				}
			}
			int num = 0;
			while (wiXRegistryKeys.Count > 0)
			{
				WiXRegistryKey wiXRegistryKey = wiXRegistryKeys[0];
				if (string.IsNullOrEmpty(wiXRegistryKey.Root) || string.IsNullOrEmpty(wiXRegistryKey.Key))
				{
					wiXRegistryKeys.Remove(wiXRegistryKey);
				}
				else
				{
					string str = string.Concat(wiXRegistryKey.Root, "\\", wiXRegistryKey.Key);
					str = str.Substring(0, str.LastIndexOf("\\"));
					VSRegistryKey vSRegistryKey5 = this.Registry.FindRegistryKey(wiXRegistryKey.Root, str);
					if (vSRegistryKey5 != null)
					{
						VSComponent vSComponent = this.FileSystem.GetComponentById((wiXRegistryKey.Parent as WiXComponent).Id);
						VSRegistryKey vSRegistryKey6 = new VSRegistryKey(this, wiXRegistryKey, vSComponent, vSRegistryKey5.Keys);
						wiXRegistryKeys.Remove(wiXRegistryKey);
						this._supportedEntities.Remove(wiXRegistryKey);
						num = 0;
					}
					else
					{
						wiXRegistryKeys.Remove(wiXRegistryKeys[0]);
						wiXRegistryKeys.Add(wiXRegistryKey);
						int num1 = num;
						num = num1 + 1;
						if (num1 <= 1000)
						{
							continue;
						}
						this.DoError(string.Concat("Error: Cannot find parent for RegistryKey with Key=", wiXRegistryKey.Key));
						break;
					}
				}
			}
			List<WiXRegistryValue> wiXRegistryValues = this._supportedEntities.FindAll((WiXEntity e) => {
				if (!(e is WiXRegistryValue))
				{
					return false;
				}
				return (e as WiXRegistryValue).Parent is WiXRegistryKey;
			}).ConvertAll<WiXRegistryValue>((WiXEntity e) => e as WiXRegistryValue);
			if (wiXRegistryValues != null && wiXRegistryValues.Count > 0)
			{
				foreach (WiXRegistryValue wiXRegistryValue in wiXRegistryValues)
				{
					if (string.IsNullOrEmpty(wiXRegistryValue.Type))
					{
						continue;
					}
					WiXRegistryKey parent = wiXRegistryValue.Parent as WiXRegistryKey;
					if (parent == null || string.IsNullOrEmpty(parent.Root) || string.IsNullOrEmpty(parent.Key))
					{
						continue;
					}
					VSRegistryKey vSRegistryKey7 = this.Registry.FindRegistryKey(parent.Root, string.Concat(parent.Root, "\\", parent.Key));
					if (vSRegistryKey7 == null)
					{
						continue;
					}
					VSRegistryValueBase vSRegistryValueString = null;
					root = wiXRegistryValue.Type;
					if (root != null)
					{
						if (root == "string")
						{
							vSRegistryValueString = new VSRegistryValueString(this, wiXRegistryValue, null, vSRegistryKey7.Values);
						}
						else if (root == "expandable")
						{
							vSRegistryValueString = new VSRegistryValueEnvString(this, wiXRegistryValue, null, vSRegistryKey7.Values);
						}
						else if (root == "binary")
						{
							vSRegistryValueString = new VSRegistryValueBinary(this, wiXRegistryValue, null, vSRegistryKey7.Values);
						}
						else if (root == "integer")
						{
							vSRegistryValueString = new VSRegistryValueInteger(this, wiXRegistryValue, null, vSRegistryKey7.Values);
						}
					}
					if (vSRegistryValueString == null)
					{
						continue;
					}
					this._supportedEntities.Remove(wiXRegistryValue);
				}
			}
			wiXRegistryValues = this._supportedEntities.FindAll((WiXEntity e) => {
				if (!(e is WiXRegistryValue))
				{
					return false;
				}
				return (e as WiXRegistryValue).Parent is WiXComponent;
			}).ConvertAll<WiXRegistryValue>((WiXEntity e) => e as WiXRegistryValue);
			if (wiXRegistryValues != null && wiXRegistryValues.Count > 0)
			{
				foreach (WiXRegistryValue wiXRegistryValue1 in wiXRegistryValues)
				{
					if (string.IsNullOrEmpty(wiXRegistryValue1.Type) || string.IsNullOrEmpty(wiXRegistryValue1.Root) || string.IsNullOrEmpty(wiXRegistryValue1.Key))
					{
						continue;
					}
					VSRegistryKey vSRegistryKey8 = this.Registry.FindRegistryKey(wiXRegistryValue1.Root, string.Concat(wiXRegistryValue1.Root, "\\", wiXRegistryValue1.Key));
					if (vSRegistryKey8 == null)
					{
						continue;
					}
					VSComponent componentById1 = this.FileSystem.GetComponentById((wiXRegistryValue1.Parent as WiXComponent).Id);
					VSRegistryValueBase vSRegistryValueEnvString = null;
					root = wiXRegistryValue1.Type;
					if (root != null)
					{
						if (root == "string")
						{
							vSRegistryValueEnvString = new VSRegistryValueString(this, wiXRegistryValue1, componentById1, vSRegistryKey8.Values);
						}
						else if (root == "expandable")
						{
							vSRegistryValueEnvString = new VSRegistryValueEnvString(this, wiXRegistryValue1, componentById1, vSRegistryKey8.Values);
						}
						else if (root == "binary")
						{
							vSRegistryValueEnvString = new VSRegistryValueBinary(this, wiXRegistryValue1, componentById1, vSRegistryKey8.Values);
						}
						else if (root == "integer")
						{
							vSRegistryValueEnvString = new VSRegistryValueInteger(this, wiXRegistryValue1, componentById1, vSRegistryKey8.Values);
						}
					}
					if (vSRegistryValueEnvString == null)
					{
						continue;
					}
					this._supportedEntities.Remove(wiXRegistryValue1);
				}
			}
		}

		private void ParseRootFolders()
		{
			this._fileSystem.Clear();
			WiXDirectory wiXDirectory = this._supportedEntities.Find((WiXEntity e) => {
				if (!(e is WiXDirectory) || !((e as WiXDirectory).Id == "TARGETDIR"))
				{
					return false;
				}
				return (e as WiXDirectory).Name == "SourceDir";
			}) as WiXDirectory;
			if (wiXDirectory != null)
			{
				if (this._projectType != WiXProjectType.Module)
				{
					VSBaseFolder vSWebApplicationFolder = null;
					if (this._webSetupParameters.WebSites.Count > 0)
					{
						if (this._supportedEntities.Find((WiXEntity e) => {
							if (!(e is IISWebVirtualDir))
							{
								return false;
							}
							return (e as IISWebVirtualDir).Directory == "TARGETDIR";
						}) is IISWebVirtualDir)
						{
							vSWebApplicationFolder = new VSWebApplicationFolder(this, null, wiXDirectory);
						}
					}
					if (vSWebApplicationFolder == null)
					{
						vSWebApplicationFolder = new VSApplicationFolder(this, null, wiXDirectory);
					}
					this._fileSystem.Add(vSWebApplicationFolder);
					this._supportedEntities.Remove(wiXDirectory);
					WiXCustomAction wiXCustomAction = this._supportedEntities.Find((WiXEntity e) => {
						if (!(e is WiXCustomAction))
						{
							return false;
						}
						return (e as WiXCustomAction).Property == vSWebApplicationFolder.Property;
					}) as WiXCustomAction;
					if (wiXCustomAction == null)
					{
						WiXProperty wiXProperty = this._supportedEntities.Find((WiXEntity e) => {
							if (!(e is WiXProperty))
							{
								return false;
							}
							return (e as WiXProperty).Id == vSWebApplicationFolder.Property;
						}) as WiXProperty;
						if (wiXProperty == null)
						{
							WiXSetDirectory wiXSetDirectory = this._supportedEntities.Find((WiXEntity e) => {
								if (!(e is WiXSetDirectory))
								{
									return false;
								}
								return (e as WiXSetDirectory).Id == vSWebApplicationFolder.Property;
							}) as WiXSetDirectory;
							if (wiXSetDirectory == null)
							{
								WiXSetProperty wiXSetProperty = this._supportedEntities.Find((WiXEntity e) => {
									if (!(e is WiXSetProperty))
									{
										return false;
									}
									return (e as WiXSetProperty).Id == vSWebApplicationFolder.Property;
								}) as WiXSetProperty;
								if (wiXSetProperty != null)
								{
									vSWebApplicationFolder.WiXSetProperty = wiXSetProperty;
									this._supportedEntities.Remove(wiXSetProperty);
								}
							}
							else
							{
								vSWebApplicationFolder.WiXSetDirectory = wiXSetDirectory;
								this._supportedEntities.Remove(wiXSetDirectory);
							}
						}
						else
						{
							vSWebApplicationFolder.WiXProperty = wiXProperty;
							this._supportedEntities.Remove(wiXProperty);
						}
					}
					else
					{
						vSWebApplicationFolder.WiXCustomAction = wiXCustomAction;
					}
				}
				else
				{
					this._supportedEntities.Remove(wiXDirectory);
				}
			}
			List<WiXEntity> wiXEntities = this._supportedEntities.FindAll((WiXEntity e) => {
				if (!(e is WiXDirectory))
				{
					return false;
				}
				return VSSpecialFolder.CheckForSpecialID((e as WiXDirectory).Id, this.CustomVariables);
			});
			if (wiXEntities != null)
			{
				for (int i = 0; i < wiXEntities.Count; i++)
				{
					this._fileSystem.Add(VSSpecialFolder.Create(this, null, wiXEntities[i] as WiXDirectory));
					this._supportedEntities.Remove(wiXEntities[i]);
				}
			}
			List<WiXEntity> wiXEntities1 = this._supportedEntities.FindAll((WiXEntity e) => e is WiXDirectory);
			List<WiXEntity> wiXEntities2 = this._supportedEntities.FindAll((WiXEntity e) => e is WiXProperty);
			if (wiXEntities1 != null && wiXEntities2 != null)
			{
			Label0:
				foreach (WiXProperty wiXProperty1 in wiXEntities2)
				{
					foreach (WiXDirectory wiXDirectory1 in wiXEntities1)
					{
						if (wiXDirectory1.Id != wiXProperty1.Id)
						{
							continue;
						}
						VSCustomFolder vSCustomFolder = new VSCustomFolder(this, null, wiXDirectory1)
						{
							WiXProperty = wiXProperty1
						};
						this._fileSystem.Add(vSCustomFolder);
						this._supportedEntities.Remove(wiXDirectory1);
						this._supportedEntities.Remove(wiXProperty1);
						goto Label0;
					}
				}
			}
			wiXEntities1 = this._supportedEntities.FindAll((WiXEntity e) => e is WiXDirectory);
			List<WiXEntity> wiXEntities3 = this._supportedEntities.FindAll((WiXEntity e) => e is WiXCustomAction);
			if (wiXEntities1 != null && wiXEntities3 != null)
			{
			Label1:
				foreach (WiXCustomAction wiXCustomAction1 in wiXEntities3)
				{
					foreach (WiXDirectory wiXDirectory2 in wiXEntities1)
					{
						if (wiXDirectory2.Id != wiXCustomAction1.Property)
						{
							continue;
						}
						VSCustomFolder vSCustomFolder1 = new VSCustomFolder(this, null, wiXDirectory2)
						{
							WiXCustomAction = wiXCustomAction1
						};
						this._fileSystem.Add(vSCustomFolder1);
						this._supportedEntities.Remove(wiXDirectory2);
						this._supportedEntities.Remove(wiXCustomAction1);
						goto Label1;
					}
				}
			}
			wiXEntities1 = this._supportedEntities.FindAll((WiXEntity e) => e is WiXDirectory);
			List<WiXEntity> wiXEntities4 = this._supportedEntities.FindAll((WiXEntity e) => e is WiXSetDirectory);
			if (wiXEntities1 != null && wiXEntities4 != null)
			{
				foreach (WiXSetDirectory wiXSetDirectory1 in wiXEntities4)
				{
					foreach (WiXDirectory wiXDirectory3 in wiXEntities1)
					{
						if (wiXDirectory3.Id != wiXSetDirectory1.Id)
						{
							continue;
						}
						VSCustomFolder vSCustomFolder2 = new VSCustomFolder(this, null, wiXDirectory3)
						{
							WiXSetDirectory = wiXSetDirectory1
						};
						this._fileSystem.Add(vSCustomFolder2);
						this._supportedEntities.Remove(wiXDirectory3);
						goto Label2;
					}
				Label2:
					this._supportedEntities.Remove(wiXSetDirectory1);
				}
			}
			wiXEntities1 = this._supportedEntities.FindAll((WiXEntity e) => e is WiXDirectory);
			List<WiXEntity> wiXEntities5 = this._supportedEntities.FindAll((WiXEntity e) => e is WiXSetProperty);
			if (wiXEntities1 != null && wiXEntities5 != null)
			{
				foreach (WiXSetProperty wiXSetProperty1 in wiXEntities5)
				{
					foreach (WiXDirectory wiXDirectory4 in wiXEntities1)
					{
						if (wiXDirectory4.Id != wiXSetProperty1.Id)
						{
							continue;
						}
						VSCustomFolder vSCustomFolder3 = new VSCustomFolder(this, null, wiXDirectory4)
						{
							WiXSetProperty = wiXSetProperty1
						};
						this._fileSystem.Add(vSCustomFolder3);
						this._supportedEntities.Remove(wiXDirectory4);
						goto Label3;
					}
				Label3:
					this._supportedEntities.Remove(wiXSetProperty1);
				}
			}
			wiXEntities1 = this._supportedEntities.FindAll((WiXEntity e) => e is WiXDirectory);
			List<IISWebVirtualDir> iSWebVirtualDirs = this._supportedEntities.FindAll((WiXEntity e) => e is IISWebVirtualDir).ConvertAll<IISWebVirtualDir>((WiXEntity e) => e as IISWebVirtualDir);
			if (wiXEntities1 != null && iSWebVirtualDirs != null)
			{
				foreach (WiXDirectory wiXDirectory5 in wiXEntities1)
				{
					if ((!(wiXDirectory5.Parent is WiXDirectory) || !((wiXDirectory5.Parent as WiXDirectory).Id == "TARGETDIR")) && (!(wiXDirectory5.Parent is WiXDirectoryRef) || !((wiXDirectory5.Parent as WiXDirectoryRef).Id == "TARGETDIR")) || iSWebVirtualDirs.Find((IISWebVirtualDir e) => wiXDirectory5.Id == e.Directory) == null)
					{
						continue;
					}
					VSWebCustomFolder vSWebCustomFolder = new VSWebCustomFolder(this, null, wiXDirectory5);
					this._fileSystem.Add(vSWebCustomFolder);
					this._supportedEntities.Remove(wiXDirectory5);
				}
			}
		}

		private void ParseSearches()
		{
			List<WiXComponentSearch> wiXComponentSearches = this._supportedEntities.FindAll((WiXEntity e) => {
				if (!(e is WiXComponentSearch) || (e as WiXComponentSearch).Parent == null)
				{
					return false;
				}
				return (e as WiXComponentSearch).Parent is WiXProperty;
			}).ConvertAll<WiXComponentSearch>((WiXEntity e) => e as WiXComponentSearch);
			if (wiXComponentSearches != null)
			{
				foreach (WiXComponentSearch wiXComponentSearch in wiXComponentSearches)
				{
					VSSearchComponent vSSearchComponent = new VSSearchComponent(this, wiXComponentSearch, wiXComponentSearch.Parent as WiXProperty, this._searches);
					this._supportedEntities.Remove(wiXComponentSearch);
					this._supportedEntities.Remove(wiXComponentSearch.Parent as WiXProperty);
				}
			}
			List<WiXRegistrySearch> wiXRegistrySearches = this._supportedEntities.FindAll((WiXEntity e) => {
				if (!(e is WiXRegistrySearch) || (e as WiXRegistrySearch).Parent == null)
				{
					return false;
				}
				return (e as WiXRegistrySearch).Parent is WiXProperty;
			}).ConvertAll<WiXRegistrySearch>((WiXEntity e) => e as WiXRegistrySearch);
			if (wiXRegistrySearches != null)
			{
				foreach (WiXRegistrySearch wiXRegistrySearch in wiXRegistrySearches)
				{
					List<WiXRegistrySearchRef> wiXRegistrySearchRefs = this._supportedEntities.FindAll((WiXEntity e) => {
						if (!(e is WiXRegistrySearchRef))
						{
							return false;
						}
						return (e as WiXRegistrySearchRef).Id == wiXRegistrySearch.Id;
					}).ConvertAll<WiXRegistrySearchRef>((WiXEntity e) => e as WiXRegistrySearchRef);
					if (wiXRegistrySearchRefs != null)
					{
						foreach (WiXRegistrySearchRef wiXRegistrySearchRef in wiXRegistrySearchRefs)
						{
							wiXRegistrySearch.RegistrySearchRefs.Add(wiXRegistrySearchRef);
							this._supportedEntities.Remove(wiXRegistrySearchRef);
						}
					}
					VSSearchRegistry vSSearchRegistry = new VSSearchRegistry(this, wiXRegistrySearch, wiXRegistrySearch.Parent as WiXProperty, this._searches);
					this._supportedEntities.Remove(wiXRegistrySearch);
					this._supportedEntities.Remove(wiXRegistrySearch.Parent as WiXProperty);
				}
			}
			List<WiXFileSearch> wiXFileSearches = this._supportedEntities.FindAll((WiXEntity e) => {
				if (!(e is WiXFileSearch) || (e as WiXFileSearch).Parent == null || !((e as WiXFileSearch).Parent is WiXDirectorySearch) || ((e as WiXFileSearch).Parent as WiXDirectorySearch).Parent == null)
				{
					return false;
				}
				return ((e as WiXFileSearch).Parent as WiXDirectorySearch).Parent is WiXProperty;
			}).ConvertAll<WiXFileSearch>((WiXEntity e) => e as WiXFileSearch);
			if (wiXFileSearches != null)
			{
				foreach (WiXFileSearch wiXFileSearch in wiXFileSearches)
				{
					WiXDirectorySearch parent = wiXFileSearch.Parent as WiXDirectorySearch;
					IWiXEntity wiXEntity = parent.Parent;
					List<WiXFileSearchRef> wiXFileSearchRefs = this._supportedEntities.FindAll((WiXEntity e) => {
						if (!(e is WiXFileSearchRef))
						{
							return false;
						}
						return (e as WiXFileSearchRef).Id == wiXFileSearch.Id;
					}).ConvertAll<WiXFileSearchRef>((WiXEntity e) => e as WiXFileSearchRef);
					if (wiXFileSearchRefs != null)
					{
						foreach (WiXFileSearchRef wiXFileSearchRef in wiXFileSearchRefs)
						{
							wiXFileSearch.FileSearchRefs.Add(wiXFileSearchRef);
							this._supportedEntities.Remove(wiXFileSearchRef);
						}
					}
					List<WiXDirectorySearchRef> wiXDirectorySearchRefs = this._supportedEntities.FindAll((WiXEntity e) => {
						if (!(e is WiXDirectorySearchRef))
						{
							return false;
						}
						return (e as WiXDirectorySearchRef).Id == parent.Id;
					}).ConvertAll<WiXDirectorySearchRef>((WiXEntity e) => e as WiXDirectorySearchRef);
					if (wiXDirectorySearchRefs != null)
					{
						foreach (WiXDirectorySearchRef wiXDirectorySearchRef in wiXDirectorySearchRefs)
						{
							parent.DirectorySearchRefs.Add(wiXDirectorySearchRef);
							this._supportedEntities.Remove(wiXDirectorySearchRef);
						}
					}
					VSSearchFile vSSearchFile = new VSSearchFile(this, wiXFileSearch, parent, parent.Parent as WiXProperty, this._searches);
					this._supportedEntities.Remove(wiXFileSearch);
					this._supportedEntities.Remove(parent);
				}
			}
		}

		private void ParseShortcuts()
		{
			List<WiXShortcut> wiXShortcuts = this._supportedEntities.FindAll((WiXEntity e) => {
				if (!(e is WiXShortcut) || (e as WiXShortcut).Parent == null)
				{
					return false;
				}
				return (e as WiXShortcut).Parent is WiXComponent;
			}).ConvertAll<WiXShortcut>((WiXEntity e) => e as WiXShortcut);
			if (wiXShortcuts != null)
			{
				foreach (WiXShortcut wiXShortcut in wiXShortcuts)
				{
					if (!(wiXShortcut.Parent is WiXComponent))
					{
						continue;
					}
					VSComponent componentById = this.FileSystem.GetComponentById((wiXShortcut.Parent as WiXComponent).Id);
					if (componentById == null)
					{
						continue;
					}
					componentById.Files.Add(new VSShortcut(this, componentById, wiXShortcut));
					this._supportedEntities.Remove(wiXShortcut);
				}
			}
		}

		private void ParseSubFolders()
		{
			int num = 0;
			List<WiXDirectory> wiXDirectories = this._supportedEntities.FindAll((WiXEntity e) => {
				if (!(e is WiXDirectory))
				{
					return false;
				}
				return !(e as WiXDirectory).InDirectoryRef();
			}).ConvertAll<WiXDirectory>((WiXEntity e) => e as WiXDirectory);
			List<IISWebVirtualDir> iSWebVirtualDirs = this._supportedEntities.FindAll((WiXEntity e) => e is IISWebVirtualDir).ConvertAll<IISWebVirtualDir>((WiXEntity e) => e as IISWebVirtualDir);
			while (wiXDirectories.Count > 0)
			{
				WiXDirectory item = wiXDirectories[0];
				if (item.Id.Contains(".") && item.GetAttributeValue("Name") == null && !item.Id.Contains("$(var."))
				{
					wiXDirectories.Remove(item);
				}
				else if (item.Parent == null || !item.Parent.IsSupported || !(item.Parent.SupportedObject is WiXDirectory))
				{
					wiXDirectories.Remove(wiXDirectories[0]);
					wiXDirectories.Add(item);
					num++;
					if (num <= 1000)
					{
						continue;
					}
					this.DoError(string.Concat("Error: Cannot find parent for Directory with Id=", item.Id));
					break;
				}
				else
				{
					VSBaseFolder folderById = this.FileSystem.GetFolderById((item.Parent.SupportedObject as WiXDirectory).Id);
					if (folderById == null)
					{
						wiXDirectories.Remove(wiXDirectories[0]);
						wiXDirectories.Add(item);
						num++;
						if (num <= 1000)
						{
							continue;
						}
						this.DoError(string.Concat("Error: Cannot find parent for Directory with Id=", item.Id));
						break;
					}
					else
					{
						if (!(folderById is VSWebFolder))
						{
							IISWebVirtualDir iSWebVirtualDir = iSWebVirtualDirs.Find((IISWebVirtualDir e) => item.Id == e.Directory);
							if (iSWebVirtualDir == null)
							{
								folderById.Folders.Add(new VSFolder(this, folderById, item));
							}
							else
							{
								VSWebCustomFolder vSWebCustomFolder = new VSWebCustomFolder(this, null, item);
								folderById.Folders.Add(vSWebCustomFolder);
								iSWebVirtualDirs.Remove(iSWebVirtualDir);
							}
						}
						else
						{
							folderById.Folders.Add(new VSWebFolder(this, folderById, item));
						}
						this._supportedEntities.Remove(item);
						wiXDirectories.Remove(item);
					}
				}
			}
			List<WiXDirectoryRef> wiXDirectoryRefs = this._supportedEntities.FindAll((WiXEntity e) => e is WiXDirectoryRef).ConvertAll<WiXDirectoryRef>((WiXEntity e) => e as WiXDirectoryRef);
			num = 0;
			while (wiXDirectoryRefs.Count > 0)
			{
				WiXDirectoryRef wiXDirectoryRef = wiXDirectoryRefs[0];
				VSBaseFolder vSBaseFolder = this.FileSystem.GetFolderById(wiXDirectoryRef.Id);
				if (vSBaseFolder == null)
				{
					wiXDirectoryRefs.Remove(wiXDirectoryRefs[0]);
					wiXDirectoryRefs.Add(wiXDirectoryRef);
					num++;
					if (num <= 1000)
					{
						continue;
					}
					this.DoError(string.Concat("Error: Cannot find DirectoryRef with Id=", wiXDirectoryRef.Id));
					return;
				}
				else
				{
					vSBaseFolder.DirectoryRefs.Add(wiXDirectoryRef);
					this.AddDirectories(vSBaseFolder, wiXDirectoryRef.ChildEntities);
					this._supportedEntities.Remove(wiXDirectoryRef);
					wiXDirectoryRefs.Remove(wiXDirectoryRef);
				}
			}
		}

		private void ParseUserInterface()
		{
			List<WiXDialog> wiXDialogs = new List<WiXDialog>();
			List<WiXEntity> wiXEntities = new List<WiXEntity>();
			List<WiXEntity> wiXEntities1 = new List<WiXEntity>();
			foreach (WiXEntity _supportedEntity in this._supportedEntities)
			{
				if (_supportedEntity is WiXInstallUISequence)
				{
					wiXEntities.Add(_supportedEntity as WiXInstallUISequence);
				}
				else if (!(_supportedEntity is WiXAdminUISequence))
				{
					if (!(_supportedEntity is WiXDialog))
					{
						continue;
					}
					wiXDialogs.Add(_supportedEntity as WiXDialog);
				}
				else
				{
					wiXEntities1.Add(_supportedEntity as WiXAdminUISequence);
				}
			}
			if (wiXEntities1.Count <= 0 & wiXEntities.Count <= 0)
			{
				return;
			}
			if (wiXDialogs.Count == 0)
			{
				return;
			}
			List<WiXEntity> wiXEntities2 = new List<WiXEntity>();
			List<WiXEntity> wiXEntities3 = new List<WiXEntity>();
			this.PopulateSequence(wiXEntities, wiXEntities2);
			this.PopulateSequence(wiXEntities1, wiXEntities3);
			this.SortSequence(wiXEntities2);
			this.SortSequence(wiXEntities3);
			List<WiXProperty> wiXProperties = this._supportedEntities.FindAll((WiXEntity e) => {
				if (e is WiXProperty)
				{
					string attributeValue = e.GetAttributeValue("Value");
					if (!string.IsNullOrEmpty(attributeValue))
					{
						return wiXDialogs.Find((WiXDialog d) => attributeValue == d.GetAttributeValue("Id")) != null;
					}
				}
				return false;
			}).ConvertAll<WiXProperty>((WiXEntity e) => e as WiXProperty);
			this.CreateVSDialogs(DialogScope.UserInstall, wiXEntities2, wiXDialogs, wiXProperties);
			this.CreateVSDialogs(DialogScope.AdministrativeInstall, wiXEntities3, wiXDialogs, wiXProperties);
		}

		private void ParseWebSetupParameters()
		{
			this._webSetupParameters.Parse(ref this._supportedEntities);
		}

		private void PopulateSequence(List<WiXEntity> parentSequence, List<WiXEntity> sequenceItems)
		{
			foreach (WiXEntity wiXEntity in parentSequence)
			{
				foreach (WiXEntity childEntity in wiXEntity.ChildEntities)
				{
					if (childEntity is WiXShow)
					{
						string attributeValue = childEntity.GetAttributeValue("Dialog");
						if (attributeValue == "FinishedForm")
						{
							if (wiXEntity is WiXAdminUISequence)
							{
								continue;
							}
						}
						else if (attributeValue == "AdminFinishedForm" && wiXEntity is WiXInstallUISequence)
						{
							continue;
						}
						WiXEntity wiXEntity1 = sequenceItems.Find((WiXEntity d) => {
							if (!(d is WiXShow))
							{
								return false;
							}
							return d.GetAttributeValue("Dialog") == attributeValue;
						});
						if (wiXEntity1 != null)
						{
							string str = wiXEntity1.GetAttributeValue("Overridable");
							string attributeValue1 = childEntity.GetAttributeValue("Overridable");
							if (!(str == "yes") || !(attributeValue1 != "yes"))
							{
								continue;
							}
							sequenceItems.Remove(wiXEntity1);
						}
					}
					sequenceItems.Add(childEntity);
				}
			}
		}

		private void SortSequence(List<WiXEntity> sequenceItems)
		{
			int sequence;
			Predicate<KeyValuePair<int, WiXEntity>> predicate = null;
			Predicate<WiXEntity> predicate1 = null;
			List<KeyValuePair<int, WiXEntity>> keyValuePairs = new List<KeyValuePair<int, WiXEntity>>();
			foreach (WiXEntity sequenceItem in sequenceItems)
			{
				if (sequenceItem.XmlNode is XmlComment)
				{
					continue;
				}
				sequence = this.GetSequence(sequenceItem);
				if (sequence == 0)
				{
					continue;
				}
				if (keyValuePairs.Count != 0)
				{
					bool flag = false;
					int num = 0;
					while (num < keyValuePairs.Count)
					{
						if (keyValuePairs[num].Key <= sequence)
						{
							num++;
						}
						else
						{
							flag = true;
							keyValuePairs.Insert(num, new KeyValuePair<int, WiXEntity>(sequence, sequenceItem));
							break;
						}
					}
					if (flag)
					{
						continue;
					}
					keyValuePairs.Add(new KeyValuePair<int, WiXEntity>(sequence, sequenceItem));
				}
				else
				{
					keyValuePairs.Add(new KeyValuePair<int, WiXEntity>(sequence, sequenceItem));
				}
			}
			if (keyValuePairs.Count > 0)
			{
				foreach (KeyValuePair<int, WiXEntity> keyValuePair in keyValuePairs)
				{
					sequenceItems.Remove(keyValuePair.Value);
				}
			}
			for (int i = 0; i < sequenceItems.Count; i++)
			{
				WiXEntity item = sequenceItems[i];
				if (item is WiXShow || item is WiXCustom)
				{
					string str = "Before";
					string attributeValue = item.GetAttributeValue("Before");
					if (string.IsNullOrEmpty(attributeValue))
					{
						str = "After";
						attributeValue = item.GetAttributeValue("After");
					}
					if (!string.IsNullOrEmpty(attributeValue))
					{
						List<KeyValuePair<int, WiXEntity>> keyValuePairs1 = keyValuePairs;
						Predicate<KeyValuePair<int, WiXEntity>> predicate2 = predicate;
						if (predicate2 == null)
						{
							Predicate<KeyValuePair<int, WiXEntity>> name = (KeyValuePair<int, WiXEntity> e) => {
								if (e.Value.Name == attributeValue || e.Value.GetAttributeValue("Action") == attributeValue)
								{
									return true;
								}
								return e.Value.GetAttributeValue("Dialog") == attributeValue;
							};
							Predicate<KeyValuePair<int, WiXEntity>> predicate3 = name;
							predicate = name;
							predicate2 = predicate3;
						}
						KeyValuePair<int, WiXEntity> keyValuePair1 = keyValuePairs1.Find(predicate2);
						if (keyValuePair1.Value == null)
						{
							sequence = 0;
							if (VSDialogBase.GetStandardActionStage(attributeValue, str, false, ref sequence) == DialogStage.Undefined)
							{
								List<WiXEntity> wiXEntities = sequenceItems;
								Predicate<WiXEntity> predicate4 = predicate1;
								if (predicate4 == null)
								{
									Predicate<WiXEntity> attributeValue1 = (WiXEntity e) => {
										if (e.GetAttributeValue("Action") == attributeValue)
										{
											return true;
										}
										return e.GetAttributeValue("Dialog") == attributeValue;
									};
									Predicate<WiXEntity> predicate5 = attributeValue1;
									predicate1 = attributeValue1;
									predicate4 = predicate5;
								}
								if (wiXEntities.Find(predicate4) == null)
								{
									sequenceItems.Remove(item);
									i = -1;
								}
							}
							else
							{
								bool flag1 = false;
								int num1 = 0;
								while (num1 < keyValuePairs.Count)
								{
									if (keyValuePairs[num1].Key <= sequence)
									{
										num1++;
									}
									else
									{
										flag1 = true;
										keyValuePairs.Insert(num1, new KeyValuePair<int, WiXEntity>(sequence, item));
										break;
									}
								}
								if (!flag1)
								{
									keyValuePairs.Add(new KeyValuePair<int, WiXEntity>(sequence, item));
								}
								sequenceItems.Remove(item);
								i = -1;
							}
						}
						else
						{
							int num2 = keyValuePairs.IndexOf(keyValuePair1);
							if (str == "Before")
							{
								keyValuePairs.Insert(num2, new KeyValuePair<int, WiXEntity>(keyValuePair1.Key, item));
							}
							else if (num2 >= keyValuePairs.Count - 1)
							{
								keyValuePairs.Add(new KeyValuePair<int, WiXEntity>(keyValuePair1.Key, item));
							}
							else
							{
								keyValuePairs.Insert(num2 + 1, new KeyValuePair<int, WiXEntity>(keyValuePair1.Key, item));
							}
							sequenceItems.Remove(item);
							i = -1;
						}
					}
				}
			}
			foreach (WiXEntity wiXEntity in sequenceItems)
			{
				if (wiXEntity.XmlNode is XmlComment || string.IsNullOrEmpty(wiXEntity.GetAttributeValue("OnExit")))
				{
					continue;
				}
				keyValuePairs.Add(new KeyValuePair<int, WiXEntity>(-1, wiXEntity));
			}
			sequenceItems.Clear();
			foreach (KeyValuePair<int, WiXEntity> keyValuePair2 in keyValuePairs)
			{
				sequenceItems.Add(keyValuePair2.Value);
			}
		}

		public event AfterParseEventHandler AfterParse;

		public event BeforeParseEventHandler BeforeParse;

		public event BeforeShowErrorEventHandler BeforeShowError;

		public event FileAddedEventHandler FileAdded;

		public event FileRemovedEventHandler FileRemoved;

		public event ReferenceAddedEventHandler ReferenceAdded;

		public event ReferenceRefreshedEventHandler ReferenceRefreshed;

		public event ReferenceRemovedEventHandler ReferenceRemoved;

		public event ReferenceRenamedEventHandler ReferenceRenamed;

		public event ThemeChangedEventHandler ThemeChanged;

		public event ToolWindowActivateEventHandler ToolWindowActivate;

		public event ToolWindowBeforeCloseEventHandler ToolWindowBeforeClose;

		public event ToolWindowCreatedEventHandler ToolWindowCreated;

		public event ToolWindowDeactivateEventHandler ToolWindowDeactivate;

		public event ViewTextModelChangedEventHandler ViewTextModelChanged;

		public event ViewXmlModelChangedEventHandler ViewXmlModelChanged;
	}
}