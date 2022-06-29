using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSBaseFolder : VSComponentBase
	{
		internal WiXProjectParser _project;

		private WiXDirectory _wixElement;

		private VSBaseFolder _parent;

		private VSFolders _folders;

		private VSComponents _components;

		private string _name = string.Empty;

		private AddinExpress.Installer.WiXDesigner.WiXCustomAction _wixCustomAction;

		private AddinExpress.Installer.WiXDesigner.WiXProperty _wixProperty;

		private AddinExpress.Installer.WiXDesigner.WiXSetDirectory _wixSetDirectory;

		private AddinExpress.Installer.WiXDesigner.WiXSetProperty _wixSetProperty;

		private AddinExpress.Installer.WiXDesigner.WiXCreateFolder _wixCreateFolder;

		private AddinExpress.Installer.WiXDesigner.WiXRemoveFolder _wixRemoveFolder;

		private List<WiXDirectoryRef> _directoryRefs;

		private List<VSProjectOutputUnknown> _projectOutputs = new List<VSProjectOutputUnknown>();

		[Browsable(true)]
		[DefaultValue(false)]
		[Description("Specifies whether to create the selected folder as part of every installation, even if the folder is empty")]
		public virtual bool AlwaysCreate
		{
			get
			{
				if (this is VSSpecialFolder)
				{
					return this.WiXCreateFolder != null;
				}
				if (this.WiXCreateFolder == null)
				{
					return false;
				}
				return this.WiXRemoveFolder != null;
			}
			set
			{
				VSComponent directoryComponent = this.GetDirectoryComponent();
				if (!value)
				{
					if (this.WiXCreateFolder != null)
					{
						this.WiXCreateFolder.Delete();
						this.WiXCreateFolder = null;
					}
					if (this.WiXRemoveFolder != null && !VSSpecialFolder.CheckForUserFolder(this))
					{
						this.WiXRemoveFolder.Delete();
						this.WiXRemoveFolder = null;
					}
					this.CheckAndDeleteDirectoryComponent(directoryComponent);
					return;
				}
				if (directoryComponent == null)
				{
					directoryComponent = this.CreateDirectoryComponent();
				}
				if (this.WiXCreateFolder == null)
				{
					XmlElement xmlElement = Common.CreateXmlElementWithAttributes(directoryComponent.WiXElement.XmlNode.OwnerDocument, "CreateFolder", directoryComponent.WiXElement.XmlNode.NamespaceURI, new string[] { "Directory" }, new string[] { this.Property }, "", false);
					directoryComponent.WiXElement.XmlNode.AppendChild(xmlElement);
					this.WiXCreateFolder = new AddinExpress.Installer.WiXDesigner.WiXCreateFolder(this._project, directoryComponent.WiXElement.Owner, directoryComponent.WiXElement, xmlElement);
				}
				if (!(this is VSSpecialFolder) && this.WiXRemoveFolder == null)
				{
					XmlElement xmlElement1 = Common.CreateXmlElementWithAttributes(directoryComponent.WiXElement.XmlNode.OwnerDocument, "RemoveFolder", directoryComponent.WiXElement.XmlNode.NamespaceURI, new string[] { "Id", "On", "Directory" }, new string[] { string.Concat("id_", Common.GenerateId(this._project.ProjectType)), "uninstall", this.Property }, "", false);
					directoryComponent.WiXElement.XmlNode.AppendChild(xmlElement1);
					this.WiXRemoveFolder = new AddinExpress.Installer.WiXDesigner.WiXRemoveFolder(this._project, directoryComponent.WiXElement.Owner, directoryComponent.WiXElement, xmlElement1);
				}
				directoryComponent.WiXElement.SetDirty();
			}
		}

		internal virtual bool CanHaveSubFolders
		{
			get
			{
				return true;
			}
		}

		internal override bool CanRename
		{
			get
			{
				return true;
			}
		}

		internal VSComponents Components
		{
			get
			{
				return this._components;
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Specifies a Windows Installer condition that must be satisfied (evaluate to true) in order for the selected item to be installed at installation time")]
		public virtual string Condition
		{
			get
			{
				VSComponent directoryComponent = this.GetDirectoryComponent();
				if (directoryComponent == null || directoryComponent.WiXCondition == null)
				{
					return string.Empty;
				}
				return directoryComponent.WiXCondition.Condition;
			}
			set
			{
				VSComponent directoryComponent = this.GetDirectoryComponent();
				if (string.IsNullOrEmpty(value))
				{
					if (directoryComponent != null)
					{
						directoryComponent.DeleteWiXCondition();
						this.CheckAndDeleteDirectoryComponent(directoryComponent);
					}
					return;
				}
				if (directoryComponent == null)
				{
					directoryComponent = this.CreateDirectoryComponent();
				}
				if (directoryComponent.WiXCondition == null)
				{
					directoryComponent.CreateWiXCondition();
				}
				directoryComponent.WiXCondition.Condition = value;
			}
		}

		[Browsable(false)]
		[DefaultValue("")]
		[Description("Specifies the default location where a folder will be installed on the target computer")]
		public virtual string DefaultLocation
		{
			get
			{
				if (this._wixCustomAction != null)
				{
					return this._wixCustomAction.Value;
				}
				if (this._wixProperty != null)
				{
					return this._wixProperty.Value;
				}
				if (this._wixSetDirectory != null)
				{
					return this._wixSetDirectory.Value;
				}
				if (this._wixSetProperty == null)
				{
					return string.Empty;
				}
				return this._wixSetProperty.Value;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					if (this._wixCustomAction != null)
					{
						this._wixCustomAction.Value = value;
						return;
					}
					if (this._wixProperty != null)
					{
						this._wixProperty.Value = value;
						return;
					}
					if (this._wixSetDirectory != null)
					{
						this._wixSetDirectory.Value = value;
						return;
					}
					if (this._wixSetProperty != null)
					{
						this._wixSetProperty.Value = value;
						return;
					}
					WiXEntity wiXEntity = null;
					WiXEntity wiXEntity1 = this._project.SupportedEntities.Find((WiXEntity e) => e is WiXProduct) ?? this._project.SupportedEntities.Find((WiXEntity e) => e is WiXModule);
					if (wiXEntity1 != null)
					{
						wiXEntity = wiXEntity1.ChildEntities.Find((WiXEntity e) => e.Name == "Package");
					}
					if (wiXEntity1 != null)
					{
						this._wixCustomAction = this.CreateDircaTargetDirAction(wiXEntity1, wiXEntity, value);
					}
				}
			}
		}

		internal List<WiXDirectoryRef> DirectoryRefs
		{
			get
			{
				return this._directoryRefs;
			}
		}

		internal VSFiles Files
		{
			get
			{
				VSFiles vSFile = new VSFiles(this._project, this);
				foreach (VSComponent component in this.Components)
				{
					vSFile.AddRange(component.Files);
				}
				vSFile.AddRange(this._projectOutputs);
				return vSFile;
			}
		}

		internal VSFolders Folders
		{
			get
			{
				return this._folders;
			}
		}

		[Browsable(false)]
		[DefaultValue("")]
		[Description("Specifies the name of the selected folder. Read-only for special folders")]
		[ReadOnly(true)]
		public override string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					this._name = value;
					if (this.WiXElement != null)
					{
						this.WiXElement.Name = value;
					}
					this.DoPropertyChanged();
				}
			}
		}

		internal VSBaseFolder Parent
		{
			get
			{
				return this._parent;
			}
		}

		internal List<VSProjectOutputUnknown> ProjectOutputs
		{
			get
			{
				return this._projectOutputs;
			}
		}

		[Browsable(false)]
		[DefaultValue("")]
		[Description("Specifies a named property that can be accessed at installation time to override the path of a custom folder")]
		[ReadOnly(true)]
		public virtual string Property
		{
			get
			{
				return this.WiXElement.Id;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					this.WiXElement.Id = value;
					foreach (WiXDirectoryRef _directoryRef in this._directoryRefs)
					{
						_directoryRef.Id = value;
					}
					if (this._wixCustomAction != null)
					{
						this._wixCustomAction.Property = value;
					}
					if (this._wixProperty != null)
					{
						this._wixProperty.Id = value;
					}
					if (this._wixSetDirectory != null)
					{
						this._wixSetDirectory.Id = value;
					}
					if (this._wixSetProperty != null)
					{
						this._wixSetProperty.Id = value;
					}
					if (this.WiXCreateFolder != null)
					{
						this.WiXCreateFolder.Directory = value;
					}
					if (this.WiXRemoveFolder != null)
					{
						this.WiXRemoveFolder.Directory = value;
					}
				}
			}
		}

		[Browsable(true)]
		[DefaultValue(false)]
		[Description("Determines whether the installer will reevaluate the Condition property for the selected item when reinstalling on a target computer")]
		public virtual bool Transitive
		{
			get
			{
				VSComponent directoryComponent = this.GetDirectoryComponent();
				if (directoryComponent == null)
				{
					return false;
				}
				return directoryComponent.WiXElement.Transitive == "yes";
			}
			set
			{
				VSComponent directoryComponent = this.GetDirectoryComponent() ?? this.CreateDirectoryComponent();
				if (value)
				{
					directoryComponent.WiXElement.Transitive = "yes";
					return;
				}
				directoryComponent.WiXElement.Transitive = null;
				this.CheckAndDeleteDirectoryComponent(directoryComponent);
			}
		}

		internal AddinExpress.Installer.WiXDesigner.WiXCreateFolder WiXCreateFolder
		{
			get
			{
				return this._wixCreateFolder;
			}
			set
			{
				this._wixCreateFolder = value;
			}
		}

		internal AddinExpress.Installer.WiXDesigner.WiXCustomAction WiXCustomAction
		{
			get
			{
				return this._wixCustomAction;
			}
			set
			{
				this._wixCustomAction = value;
			}
		}

		internal WiXDirectory WiXElement
		{
			get
			{
				return this._wixElement;
			}
		}

		internal AddinExpress.Installer.WiXDesigner.WiXProperty WiXProperty
		{
			get
			{
				return this._wixProperty;
			}
			set
			{
				this._wixProperty = value;
			}
		}

		internal AddinExpress.Installer.WiXDesigner.WiXRemoveFolder WiXRemoveFolder
		{
			get
			{
				return this._wixRemoveFolder;
			}
			set
			{
				this._wixRemoveFolder = value;
			}
		}

		internal AddinExpress.Installer.WiXDesigner.WiXSetDirectory WiXSetDirectory
		{
			get
			{
				return this._wixSetDirectory;
			}
			set
			{
				this._wixSetDirectory = value;
			}
		}

		internal AddinExpress.Installer.WiXDesigner.WiXSetProperty WiXSetProperty
		{
			get
			{
				return this._wixSetProperty;
			}
			set
			{
				this._wixSetProperty = value;
			}
		}

		protected VSBaseFolder(WiXProjectParser project)
		{
			this._project = project;
			this._directoryRefs = new List<WiXDirectoryRef>();
			this._folders = new VSFolders(this._project, this);
			this._components = new VSComponents(this);
		}

		public VSBaseFolder(WiXProjectParser project, VSBaseFolder parent, WiXDirectory wixElement) : this(project)
		{
			this._parent = parent;
			this._wixElement = wixElement;
			if (wixElement.XmlNode.Attributes["Name"] != null)
			{
				this._name = wixElement.XmlNode.Attributes["Name"].Value;
				return;
			}
			if (wixElement.XmlNode.Attributes["LongName"] != null)
			{
				this._name = wixElement.XmlNode.Attributes["LongName"].Value;
				return;
			}
			if (wixElement.XmlNode.Attributes["ShortName"] != null)
			{
				this._name = wixElement.XmlNode.Attributes["ShortName"].Value;
			}
		}

		public VSAssembly AddAssembly(string assemblyPath)
		{
			return this.AddAssembly(assemblyPath, null);
		}

		public VSAssembly AddAssembly(string assemblyPath, VSComponent component)
		{
			if (!File.Exists(assemblyPath))
			{
				return null;
			}
			return new VSAssembly(this, assemblyPath, component);
		}

		public VSFile AddFile(string filePath)
		{
			return this.AddFile(filePath, null);
		}

		public VSFile AddFile(string filePath, VSComponent component)
		{
			if (!File.Exists(filePath))
			{
				return null;
			}
			if (this.IsAssembly(filePath))
			{
				return this.AddAssembly(filePath, component);
			}
			return new VSFile(this, filePath, component);
		}

		public VSFolder AddFolder()
		{
			return this.Folders.AddFolder();
		}

		public VSProjectOutputVDProj AddProjectOutput(string id, VsWiXProject.ReferenceDescriptor descriptor)
		{
			return new VSProjectOutputVDProj(this, id, descriptor);
		}

		public VSWebFolder AddWebFolder()
		{
			return this.Folders.AddWebFolder();
		}

		private void AdjustKeyPath()
		{
			VSComponent directoryComponent = null;
			if (!VSSpecialFolder.CheckForUserFolder(this))
			{
				directoryComponent = this.GetDirectoryComponent();
				if (directoryComponent != null)
				{
					directoryComponent.AdjustDirectoryKeyPath();
					this.CheckAndDeleteDirectoryComponent(directoryComponent);
				}
			}
			else
			{
				directoryComponent = this.GetDirectoryComponent() ?? this.CreateDirectoryComponent();
				directoryComponent.AdjustDirectoryKeyPath();
			}
			foreach (VSComponent component in this.Components)
			{
				if (component == directoryComponent)
				{
					continue;
				}
				component.AdjustKeyPath();
			}
			foreach (VSProjectOutputUnknown projectOutput in this.ProjectOutputs)
			{
				projectOutput.AdjustKeyPath();
			}
			foreach (VSBaseFolder _folder in this._folders)
			{
				_folder.AdjustKeyPath();
			}
		}

		private void CheckAndDeleteDirectoryComponent(VSComponent dirComponent)
		{
			if (dirComponent == null)
			{
				return;
			}
			if (!this.Transitive && this.Condition == string.Empty && dirComponent.Files.Count == 0 && this.WiXCreateFolder == null && this.WiXRemoveFolder == null)
			{
				this.Components.Remove(dirComponent);
				dirComponent.Delete();
				dirComponent.Dispose();
				this.WiXElement.SetDirty();
			}
		}

		private AddinExpress.Installer.WiXDesigner.WiXCustomAction CreateDircaTargetDirAction(WiXEntity parent, WiXEntity insertAfter, string value)
		{
			AddinExpress.Installer.WiXDesigner.WiXCustomAction wiXCustomAction = null;
			XmlElement xmlElement = Common.CreateXmlElementWithAttributes(this.WiXElement.XmlNode.OwnerDocument, "CustomAction", this.WiXElement.XmlNode.NamespaceURI, new string[] { "Id", "Property", "Value", "Execute" }, new string[] { "DIRCA_TARGETDIR", "TARGETDIR", value, "firstSequence" }, "", false);
			if (insertAfter != null)
			{
				parent.XmlNode.InsertAfter(xmlElement, insertAfter.XmlNode);
			}
			else
			{
				parent.XmlNode.AppendChild(xmlElement);
			}
			wiXCustomAction = new AddinExpress.Installer.WiXDesigner.WiXCustomAction(this._project, this.WiXElement.Owner, parent, xmlElement);
			WiXEntity wiXEntity = parent.ChildEntities.Find((WiXEntity e) => e.Name == "InstallExecuteSequence");
			if (wiXEntity == null)
			{
				xmlElement = Common.CreateXmlElementWithAttributes(this.WiXElement.XmlNode.OwnerDocument, "InstallExecuteSequence", this.WiXElement.XmlNode.NamespaceURI, new string[0], new string[0], "", false);
				parent.XmlNode.AppendChild(xmlElement);
				wiXEntity = new WiXEntity(this._project, this.WiXElement.Owner, parent, xmlElement);
			}
			if (wiXEntity != null)
			{
				if (!(wiXEntity.ChildEntities.Find((WiXEntity e) => {
					if (!(e is WiXCustom))
					{
						return false;
					}
					return (e as WiXCustom).Action == "DIRCA_TARGETDIR";
				}) is WiXCustom))
				{
					xmlElement = Common.CreateXmlElementWithAttributes(this.WiXElement.XmlNode.OwnerDocument, "Custom", this.WiXElement.XmlNode.NamespaceURI, new string[] { "Action", "Before" }, new string[] { "DIRCA_TARGETDIR", "CostInitialize" }, "", false);
					XmlCDataSection xmlCDataSection = xmlElement.OwnerDocument.CreateCDataSection("TARGETDIR=\"\"");
					xmlElement.AppendChild(xmlCDataSection);
					wiXEntity.XmlNode.AppendChild(xmlElement);
					WiXCustom wiXCustom = new WiXCustom(this._project, this.WiXElement.Owner, wiXEntity, xmlElement);
				}
			}
			WiXEntity wiXEntity1 = parent.ChildEntities.Find((WiXEntity e) => e.Name == "InstallUISequence");
			if (wiXEntity1 == null)
			{
				xmlElement = Common.CreateXmlElementWithAttributes(this.WiXElement.XmlNode.OwnerDocument, "InstallUISequence", this.WiXElement.XmlNode.NamespaceURI, new string[0], new string[0], "", false);
				parent.XmlNode.AppendChild(xmlElement);
				wiXEntity1 = new WiXEntity(this._project, this.WiXElement.Owner, parent, xmlElement);
			}
			if (wiXEntity1 != null)
			{
				if (!(wiXEntity1.ChildEntities.Find((WiXEntity e) => {
					if (!(e is WiXCustom))
					{
						return false;
					}
					return (e as WiXCustom).Action == "DIRCA_TARGETDIR";
				}) is WiXCustom))
				{
					xmlElement = Common.CreateXmlElementWithAttributes(this.WiXElement.XmlNode.OwnerDocument, "Custom", this.WiXElement.XmlNode.NamespaceURI, new string[] { "Action", "Before" }, new string[] { "DIRCA_TARGETDIR", "CostInitialize" }, "", false);
					XmlCDataSection xmlCDataSection1 = xmlElement.OwnerDocument.CreateCDataSection("TARGETDIR=\"\"");
					xmlElement.AppendChild(xmlCDataSection1);
					wiXEntity1.XmlNode.AppendChild(xmlElement);
					WiXCustom wiXCustom1 = new WiXCustom(this._project, this.WiXElement.Owner, wiXEntity1, xmlElement);
				}
			}
			WiXEntity wiXEntity2 = parent.ChildEntities.Find((WiXEntity e) => e.Name == "AdminUISequence");
			if (wiXEntity2 == null)
			{
				xmlElement = Common.CreateXmlElementWithAttributes(this.WiXElement.XmlNode.OwnerDocument, "AdminUISequence", this.WiXElement.XmlNode.NamespaceURI, new string[0], new string[0], "", false);
				parent.XmlNode.AppendChild(xmlElement);
				wiXEntity2 = new WiXEntity(this._project, this.WiXElement.Owner, parent, xmlElement);
			}
			if (wiXEntity2 != null)
			{
				if (!(wiXEntity2.ChildEntities.Find((WiXEntity e) => {
					if (!(e is WiXCustom))
					{
						return false;
					}
					return (e as WiXCustom).Action == "DIRCA_TARGETDIR";
				}) is WiXCustom))
				{
					xmlElement = Common.CreateXmlElementWithAttributes(this.WiXElement.XmlNode.OwnerDocument, "Custom", this.WiXElement.XmlNode.NamespaceURI, new string[] { "Action", "Before" }, new string[] { "DIRCA_TARGETDIR", "CostInitialize" }, "", false);
					XmlCDataSection xmlCDataSection2 = xmlElement.OwnerDocument.CreateCDataSection("TARGETDIR=\"\"");
					xmlElement.AppendChild(xmlCDataSection2);
					wiXEntity2.XmlNode.AppendChild(xmlElement);
					WiXCustom wiXCustom2 = new WiXCustom(this._project, this.WiXElement.Owner, wiXEntity2, xmlElement);
				}
			}
			parent.SetDirty();
			return wiXCustomAction;
		}

		internal VSComponent CreateDirectoryComponent()
		{
			VSComponent vSComponent = null;
			XmlElement xmlElement = Common.CreateXmlElementWithAttributes(this.WiXElement.XmlNode.OwnerDocument, "Component", this.WiXElement.XmlNode.NamespaceURI, new string[] { "Id", "Guid" }, new string[] { string.Concat("comp_", this.Property), Common.GenerateGuid() }, "", false);
			if (!this.WiXElement.XmlNode.HasChildNodes)
			{
				this.WiXElement.XmlNode.AppendChild(xmlElement);
			}
			else
			{
				this.WiXElement.XmlNode.InsertBefore(xmlElement, this.WiXElement.XmlNode.FirstChild);
			}
			WiXComponent wiXComponent = new WiXComponent(this._project, this.WiXElement.Owner, this.WiXElement, xmlElement);
			if (VSSpecialFolder.CheckFor64BitFolder(this))
			{
				wiXComponent.Win64 = "yes";
			}
			wiXComponent.Parent.SetDirty();
			WiXComponentRef wiXComponentRef = null;
			if (this._project.ProjectType == WiXProjectType.Product)
			{
				WiXEntity wiXElement = null;
				VSComponentGroup vSComponentGroup = this._project.ComponentGroups.Find((VSComponentGroup e) => e.WiXElement.Directory == this.Property);
				if (vSComponentGroup == null)
				{
					VSFeature vSFeature = this._project.Features.Find((VSFeature e) => e.WiXElement.Level == "1");
					if (vSFeature != null)
					{
						wiXElement = vSFeature.WiXElement;
					}
				}
				else
				{
					wiXElement = vSComponentGroup.WiXElement;
				}
				if (wiXElement != null)
				{
					XmlElement xmlElement1 = Common.CreateXmlElementWithAttributes(wiXElement.XmlNode.OwnerDocument, "ComponentRef", wiXElement.XmlNode.NamespaceURI, new string[] { "Id" }, new string[] { wiXComponent.Id }, "", false);
					wiXElement.XmlNode.AppendChild(xmlElement1);
					wiXComponentRef = new WiXComponentRef(this._project, wiXElement.Owner, wiXElement, xmlElement1);
					wiXComponentRef.Parent.SetDirty();
				}
			}
			vSComponent = new VSComponent(this, wiXComponent);
			if (!VSSpecialFolder.CheckForUserFolder(this))
			{
				vSComponent.WiXElement.KeyPath = "yes";
			}
			else
			{
				vSComponent.CreateWiXKeyPathElement();
			}
			if (wiXComponentRef != null)
			{
				vSComponent.ComponentRefs.Add(wiXComponentRef);
			}
			this.Components.Insert(0, vSComponent);
			return vSComponent;
		}

		public VSShortcut CreateShortcut()
		{
			VSShortcut vSShortcut;
			FormSelectItemInProject formSelectItemInProject = new FormSelectItemInProject();
			try
			{
				formSelectItemInProject.Initialize(this, string.Empty, this._project.FileSystem, new string[] { "All Files (*.*)" });
				if (formSelectItemInProject.ShowDialog() != DialogResult.OK)
				{
					vSShortcut = null;
				}
				else
				{
					vSShortcut = new VSShortcut(this, formSelectItemInProject.SelectedFile);
				}
			}
			finally
			{
				formSelectItemInProject.Dispose();
			}
			return vSShortcut;
		}

		public VSShortcut CreateShortcut(VSBaseFile target)
		{
			return new VSShortcut(this, target);
		}

		public VSShortcut CreateShortcut(VSBaseFolder target)
		{
			return new VSShortcut(this, target);
		}

		public override void Delete()
		{
			if (this._project.ProjectType == WiXProjectType.Product && this._project.MergeModules.Count > 0)
			{
				VSBaseFolder folderById = this._project.FileSystem.GetFolderById("TARGETDIR");
				if (folderById != null)
				{
					List<VSMergeModule> vSMergeModules = this._project.MergeModules.FindAll((VSMergeModule x) => x.ParentFolder == this);
					if (vSMergeModules != null && vSMergeModules.Count > 0)
					{
						for (int i = 0; i < vSMergeModules.Count; i++)
						{
							vSMergeModules[i].ParentFolder = folderById;
						}
					}
				}
			}
			string id = this._wixElement.Id;
			while (this._folders.Count > 0)
			{
				this._folders[0].Delete();
			}
			this._folders.Clear();
			while (this._components.Count > 0)
			{
				this._components[0].Delete();
			}
			this._components.Clear();
			if (this._parent != null)
			{
				this._parent.Folders.Remove(this);
			}
			if (this._parent == null)
			{
				this._project.FileSystem.Remove(this);
			}
			this._wixElement.Delete();
			foreach (WiXDirectoryRef _directoryRef in this._directoryRefs)
			{
				_directoryRef.Delete();
			}
			this._directoryRefs.Clear();
			if (this._wixCustomAction != null)
			{
				this._wixCustomAction.Delete();
				this._wixCustomAction = null;
			}
			if (this._wixProperty != null)
			{
				this._wixProperty.Delete();
				this._wixProperty = null;
			}
			if (this._wixSetDirectory != null)
			{
				this._wixSetDirectory.Delete();
				this._wixSetDirectory = null;
			}
			if (this._wixSetProperty != null)
			{
				this._wixSetProperty.Delete();
				this._wixSetProperty = null;
			}
			List<VSComponentGroup> vSComponentGroups = this._project.ComponentGroups.FindAll((VSComponentGroup e) => e.WiXElement.Directory == id);
			if (vSComponentGroups != null)
			{
				foreach (VSComponentGroup vSComponentGroup in vSComponentGroups)
				{
					if (vSComponentGroup.WiXElement.ChildEntities.Count == 0)
					{
						vSComponentGroup.Delete();
						this._project.ComponentGroups.Remove(vSComponentGroup);
					}
					if (vSComponentGroup.WiXElement.ChildEntities.Count <= 0)
					{
						continue;
					}
					List<WiXEntity> wiXEntities = vSComponentGroup.WiXElement.ChildEntities.FindAll((WiXEntity e) => e.XmlNode is XmlComment);
					if (wiXEntities == null || wiXEntities.Count != vSComponentGroup.WiXElement.ChildEntities.Count)
					{
						continue;
					}
					vSComponentGroup.Delete();
					this._project.ComponentGroups.Remove(vSComponentGroup);
				}
			}
			if (this._project.Features.Count > 0)
			{
				foreach (VSFeature feature in this._project.Features)
				{
					if (string.IsNullOrEmpty(feature.WiXElement.ConfigurableDirectory) || !(feature.WiXElement.ConfigurableDirectory == id))
					{
						continue;
					}
					feature.WiXElement.ConfigurableDirectory = string.Empty;
					feature.WiXElement.SetDirty();
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this._directoryRefs.Clear();
				this._folders.Clear();
				this._components.Clear();
				this._folders = null;
				this._directoryRefs = null;
				this._components = null;
			}
			base.Dispose(disposing);
		}

		internal VSComponent GetDirectoryComponent()
		{
			if (this.Components.Count == 0)
			{
				return null;
			}
			VSComponent item = this.Components[0];
			if (item.Files.Count > 0)
			{
				return null;
			}
			return item;
		}

		private bool IsAssembly(string filePath)
		{
			bool flag;
			try
			{
				AssemblyName.GetAssemblyName(filePath);
				flag = true;
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		internal virtual void MoveTo(VSBaseFolder destinationFolder)
		{
			if (this.Parent != destinationFolder)
			{
				this.Parent.Folders.Remove(this);
				destinationFolder.Folders.Add(this);
				this._parent = destinationFolder;
				this.WiXElement.Parent.ChildEntities.Remove(this.WiXElement);
				this.WiXElement.XmlNode.ParentNode.RemoveChild(this.WiXElement.XmlNode);
				this.WiXElement.Parent.SetDirty();
				if (this.WiXElement.Owner != destinationFolder.WiXElement.Owner)
				{
					this.WiXElement.RebuildXmlNodes(destinationFolder.WiXElement.XmlNode.OwnerDocument, destinationFolder.WiXElement.Owner);
				}
				destinationFolder.WiXElement.ChildEntities.Add(this.WiXElement);
				destinationFolder.WiXElement.XmlNode.AppendChild(this.WiXElement.XmlNode);
				this.WiXElement.Parent = destinationFolder.WiXElement;
				this.WiXElement.Parent.SetDirty();
				this.AdjustKeyPath();
			}
		}
	}
}