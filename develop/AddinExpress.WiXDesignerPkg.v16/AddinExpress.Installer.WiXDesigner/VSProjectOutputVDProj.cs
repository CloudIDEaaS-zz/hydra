using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSProjectOutputVDProj : VSProjectOutputUnknown
	{
		private VsWiXProject.ReferenceDescriptor _referenceDescriptor;

		private KeyOutputDescriptorClass _keyOutputDescriptor;

		private OutputGroup _group;

		private string[] _outputs;

		private WiXComponent _directoryComponent;

		private WiXComponentRef _directoryComponentRef;

		internal override bool CanRename
		{
			get
			{
				return false;
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Specifies a Windows Installer condition that must be satisfied (evaluate to true) in order for the selected item to be installed at installation time")]
		public string Condition
		{
			get
			{
				if (this._referenceDescriptor != null)
				{
					object projectOutputProperty = this._referenceDescriptor.GetProjectOutputProperty(this.Group, OutputGroupProperties.Condition);
					if (projectOutputProperty != null)
					{
						return projectOutputProperty.ToString();
					}
				}
				return string.Empty;
			}
			set
			{
				if (this._referenceDescriptor != null)
				{
					this._referenceDescriptor.SetProjectOutputProperty(this.Group, OutputGroupProperties.Condition, value);
				}
			}
		}

		[Browsable(true)]
		[DefaultValue(null)]
		[Description("Displays the Dependencies dialog box that contains a list of dependent files")]
		[Editor(typeof(DependenciesPropertyEditor), typeof(UITypeEditor))]
		[MergableProperty(false)]
		[TypeConverter(typeof(DependenciesPropertyConverter))]
		public object Dependencies
		{
			get
			{
				if (this._referenceDescriptor == null || this._referenceDescriptor.ReferencedProject == null)
				{
					return string.Empty;
				}
				return this._referenceDescriptor.ReferencedProject.Dependencies(this.Group);
			}
		}

		internal string FileId
		{
			get
			{
				if (this._referenceDescriptor == null)
				{
					return string.Empty;
				}
				return this._referenceDescriptor.SharedFileId;
			}
		}

		internal OutputGroup Group
		{
			get
			{
				if (this._group == OutputGroup.None)
				{
					this._group = this.GetOutputGroupFromId();
				}
				return this._group;
			}
		}

		[Browsable(true)]
		[DefaultValue(false)]
		[Description("Specifies whether to install the files in a project output as a hidden files")]
		public bool Hidden
		{
			get
			{
				if (this._referenceDescriptor != null)
				{
					object projectOutputProperty = this._referenceDescriptor.GetProjectOutputProperty(this.Group, OutputGroupProperties.Hidden);
					if (projectOutputProperty != null)
					{
						return Convert.ToBoolean(projectOutputProperty);
					}
				}
				return false;
			}
			set
			{
				if (this._referenceDescriptor != null)
				{
					this._referenceDescriptor.SetProjectOutputProperty(this.Group, OutputGroupProperties.Hidden, value);
				}
			}
		}

		[Browsable(true)]
		[DefaultValue(null)]
		[Description("Expand this node to see properties for the key file in this output group")]
		[MergableProperty(false)]
		[TypeConverter(typeof(KeyOutputPropertyConverter))]
		public KeyOutputDescriptorClass KeyOutput
		{
			get
			{
				if (this.Group != OutputGroup.Binaries || this._referenceDescriptor == null || this._referenceDescriptor.ReferencedProject == null)
				{
					return null;
				}
				if (this._keyOutputDescriptor == null)
				{
					this._keyOutputDescriptor = new KeyOutputDescriptorClass(this._referenceDescriptor.ReferencedProject.KeyOutput);
				}
				return this._keyOutputDescriptor;
			}
		}

		[Browsable(false)]
		[DisplayName("(Name)")]
		[ReadOnly(true)]
		public override string Name
		{
			get
			{
				if (this._referenceDescriptor != null)
				{
					OutputGroup group = this.Group;
					if (group > OutputGroup.Content)
					{
						if (group == OutputGroup.Satellites)
						{
							return string.Concat("Localized Resources from ", this._referenceDescriptor.Caption);
						}
						if (group == OutputGroup.Documents)
						{
							return string.Concat("Documentation Files from ", this._referenceDescriptor.Caption);
						}
					}
					else
					{
						switch (group)
						{
							case OutputGroup.Binaries:
							{
								return string.Concat("Primary Output from ", this._referenceDescriptor.Caption);
							}
							case OutputGroup.Symbols:
							{
								return string.Concat("Debug Symbols from ", this._referenceDescriptor.Caption);
							}
							case OutputGroup.Binaries | OutputGroup.Symbols:
							{
								break;
							}
							case OutputGroup.Sources:
							{
								return string.Concat("Source Files from ", this._referenceDescriptor.Caption);
							}
							default:
							{
								if (group == OutputGroup.Content)
								{
									return string.Concat("Content Files from ", this._referenceDescriptor.Caption);
								}
								break;
							}
						}
					}
				}
				return base.Name;
			}
			set
			{
			}
		}

		[Browsable(true)]
		[DefaultValue(null)]
		[Description("Displays the Outputs dialog box that contains a list of files in the primary output group")]
		[Editor(typeof(OutputsPropertyEditor), typeof(UITypeEditor))]
		[MergableProperty(false)]
		[TypeConverter(typeof(OutputsPropertyConverter))]
		public object Outputs
		{
			get
			{
				if (this._referenceDescriptor == null || this._referenceDescriptor.ReferencedProject == null)
				{
					return string.Empty;
				}
				if (this._outputs == null)
				{
					this._outputs = this._referenceDescriptor.ReferencedProject.Outputs(this.Group, this._referenceDescriptor.ReferencedProject.ActiveConfiguration, false);
				}
				return this._outputs;
			}
		}

		[Browsable(true)]
		[DefaultValue(VSPackageAs.vsdpaDefault)]
		[Description("Specifies whether to override the default packaging behavior for a selected project output group")]
		public VSPackageAs PackageAs
		{
			get
			{
				if (this._referenceDescriptor != null)
				{
					object projectOutputProperty = this._referenceDescriptor.GetProjectOutputProperty(this.Group, OutputGroupProperties.Compressed);
					if (projectOutputProperty != null)
					{
						if (!Convert.ToBoolean(projectOutputProperty))
						{
							return VSPackageAs.vsdpaLoose;
						}
						return VSPackageAs.vsdpaDefault;
					}
				}
				return VSPackageAs.vsdpaDefault;
			}
			set
			{
				if (this._referenceDescriptor != null)
				{
					this._referenceDescriptor.SetProjectOutputProperty(this.Group, OutputGroupProperties.Compressed, value == VSPackageAs.vsdpaDefault);
				}
			}
		}

		[Browsable(true)]
		[DefaultValue(false)]
		[Description("Specifies whether files in a project output group should be removed when the application is uninstalled")]
		public bool Permanent
		{
			get
			{
				if (this._referenceDescriptor != null)
				{
					object projectOutputProperty = this._referenceDescriptor.GetProjectOutputProperty(this.Group, OutputGroupProperties.Permanent);
					if (projectOutputProperty != null)
					{
						return Convert.ToBoolean(projectOutputProperty);
					}
				}
				return false;
			}
			set
			{
				if (this._referenceDescriptor != null)
				{
					this._referenceDescriptor.SetProjectOutputProperty(this.Group, OutputGroupProperties.Permanent, value);
				}
			}
		}

		[Browsable(true)]
		[DefaultValue(false)]
		[Description("Specifies whether to install the files in a selected project output group as read-only files")]
		public bool ReadOnly
		{
			get
			{
				if (this._referenceDescriptor != null)
				{
					object projectOutputProperty = this._referenceDescriptor.GetProjectOutputProperty(this.Group, OutputGroupProperties.ReadOnly);
					if (projectOutputProperty != null)
					{
						return Convert.ToBoolean(projectOutputProperty);
					}
				}
				return false;
			}
			set
			{
				if (this._referenceDescriptor != null)
				{
					this._referenceDescriptor.SetProjectOutputProperty(this.Group, OutputGroupProperties.ReadOnly, value);
				}
			}
		}

		internal VsWiXProject.ReferenceDescriptor ReferenceDescriptor
		{
			get
			{
				return this._referenceDescriptor;
			}
		}

		[Browsable(true)]
		[DefaultValue(VSRegister.vsdrfDoNotRegister)]
		[Description("Specifies whether files in project output group should be registered during installation")]
		public VSRegister Register
		{
			get
			{
				if (this._referenceDescriptor != null)
				{
					object projectOutputProperty = this._referenceDescriptor.GetProjectOutputProperty(this.Group, OutputGroupProperties.Register);
					if (projectOutputProperty != null && Convert.ToBoolean(projectOutputProperty))
					{
						return VSRegister.vsdrfCOMSelfReg;
					}
					object obj = this._referenceDescriptor.GetProjectOutputProperty(this.Group, OutputGroupProperties.TrueType);
					if (obj != null && Convert.ToBoolean(obj))
					{
						return VSRegister.vsdrfFont;
					}
				}
				return VSRegister.vsdrfDoNotRegister;
			}
			set
			{
				if (this._referenceDescriptor != null)
				{
					this._referenceDescriptor.SetProjectOutputProperty(this.Group, OutputGroupProperties.Register, value == VSRegister.vsdrfCOMSelfReg);
					this._referenceDescriptor.SetProjectOutputProperty(this.Group, OutputGroupProperties.TrueType, value == VSRegister.vsdrfFont);
				}
			}
		}

		[Browsable(true)]
		[DefaultValue(false)]
		[Description("Specifies whether to mark files in a selected project output group as shared legacy files that require reference counting")]
		public bool SharedLegacyFile
		{
			get
			{
				if (this._referenceDescriptor != null)
				{
					object projectOutputProperty = this._referenceDescriptor.GetProjectOutputProperty(this.Group, OutputGroupProperties.SharedLegacy);
					if (projectOutputProperty != null)
					{
						return Convert.ToBoolean(projectOutputProperty);
					}
				}
				return false;
			}
			set
			{
				if (this._referenceDescriptor != null)
				{
					this._referenceDescriptor.SetProjectOutputProperty(this.Group, OutputGroupProperties.SharedLegacy, value);
				}
			}
		}

		[Browsable(true)]
		[DefaultValue(false)]
		[Description("Specifies whether to install files in a selected project output group as system files")]
		public bool System
		{
			get
			{
				if (this._referenceDescriptor != null)
				{
					object projectOutputProperty = this._referenceDescriptor.GetProjectOutputProperty(this.Group, OutputGroupProperties.System);
					if (projectOutputProperty != null)
					{
						return Convert.ToBoolean(projectOutputProperty);
					}
				}
				return false;
			}
			set
			{
				if (this._referenceDescriptor != null)
				{
					this._referenceDescriptor.SetProjectOutputProperty(this.Group, OutputGroupProperties.System, value);
				}
			}
		}

		[Browsable(true)]
		[DefaultValue(false)]
		[Description("Determines whether the installer will reevaluate the Condition property for the selected item when reinstalling on a target computer")]
		public override bool Transitive
		{
			get
			{
				if (this._referenceDescriptor != null)
				{
					object projectOutputProperty = this._referenceDescriptor.GetProjectOutputProperty(this.Group, OutputGroupProperties.Transitive);
					if (projectOutputProperty != null)
					{
						return Convert.ToBoolean(projectOutputProperty);
					}
				}
				return false;
			}
			set
			{
				if (this._referenceDescriptor != null)
				{
					this._referenceDescriptor.SetProjectOutputProperty(this.Group, OutputGroupProperties.Transitive, value);
				}
			}
		}

		[Browsable(true)]
		[DefaultValue(true)]
		[Description("Specifies whether a selected project output group is vital for installation")]
		public bool Vital
		{
			get
			{
				if (this._referenceDescriptor != null)
				{
					object projectOutputProperty = this._referenceDescriptor.GetProjectOutputProperty(this.Group, OutputGroupProperties.Vital);
					if (projectOutputProperty != null)
					{
						return Convert.ToBoolean(projectOutputProperty);
					}
				}
				return false;
			}
			set
			{
				if (this._referenceDescriptor != null)
				{
					this._referenceDescriptor.SetProjectOutputProperty(this.Group, OutputGroupProperties.Vital, value);
				}
			}
		}

		public VSProjectOutputVDProj(WiXProjectParser project, VSBaseFolder parent, WiXDirectory wixElement, VsWiXProject.ReferenceDescriptor referenceDescriptor, WiXComponent directoryComponent, WiXComponentRef directoryComponentRef) : base(project, parent, wixElement)
		{
			this._referenceDescriptor = referenceDescriptor;
			base.Project.ReferenceRemoved += new ReferenceRemovedEventHandler(this.DoReferenceRemoved);
			base.Project.ReferenceRenamed += new ReferenceRenamedEventHandler(this.DoReferenceRenamed);
			this._directoryComponent = directoryComponent;
			this._directoryComponentRef = directoryComponentRef;
		}

		public VSProjectOutputVDProj(VSBaseFolder parent, string id, VsWiXProject.ReferenceDescriptor referenceDescriptor) : base(parent._project, parent)
		{
			base.Project.ReferenceRemoved += new ReferenceRemovedEventHandler(this.DoReferenceRemoved);
			base.Project.ReferenceRenamed += new ReferenceRenamedEventHandler(this.DoReferenceRenamed);
			this._referenceDescriptor = referenceDescriptor;
			XmlElement xmlElement = Common.CreateXmlElementWithAttributes(base.ParentFolder.WiXElement.XmlNode.OwnerDocument, "Directory", base.ParentFolder.WiXElement.XmlNode.NamespaceURI, new string[] { "Id" }, new string[] { id }, "", false);
			base.ParentFolder.WiXElement.XmlNode.AppendChild(xmlElement);
			this._wixElement = new WiXDirectory(this._project, base.ParentFolder.WiXElement.Owner, base.ParentFolder.WiXElement, xmlElement);
			this._wixElement.Parent.SetDirty();
			base.ParentFolder.ProjectOutputs.Add(this);
			this.AdjustKeyPath();
			if (base.Project.Features.Count > 0)
			{
				bool flag = false;
				VSFeature item = base.Project.Features[0];
				int num = 0;
				while (num < item.WiXElement.ChildEntities.Count)
				{
					if (!(item.WiXElement.ChildEntities[num] is WiXComponentGroupRef) || !((item.WiXElement.ChildEntities[num] as WiXComponentGroupRef).Id == id))
					{
						num++;
					}
					else
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					XmlElement xmlElement1 = Common.CreateXmlElementWithAttributes(item.WiXElement.XmlNode.OwnerDocument, "ComponentGroupRef", item.WiXElement.XmlNode.NamespaceURI, new string[] { "Id" }, new string[] { id }, "", false);
					item.WiXElement.XmlNode.AppendChild(xmlElement1);
					WiXComponentGroupRef wiXComponentGroupRef = new WiXComponentGroupRef(base.Project, item.WiXElement.Owner, item.WiXElement, xmlElement1);
					item.WiXElement.SetDirty();
				}
			}
		}

		internal override void AdjustKeyPath()
		{
			if (!VSSpecialFolder.CheckForUserFolder(base.ParentFolder))
			{
				if (this._directoryComponent != null)
				{
					this._directoryComponent.Delete();
					this._directoryComponent = null;
				}
				if (this._directoryComponentRef != null)
				{
					this._directoryComponentRef.Delete();
					this._directoryComponentRef = null;
				}
			}
			else
			{
				if (this._directoryComponent == null)
				{
					this._directoryComponent = this.CreateDirectoryComponent();
				}
				if (this._directoryComponent != null && this._directoryComponentRef == null)
				{
					this._directoryComponentRef = this.CreateDirectoryComponentRef();
				}
			}
			if (this._referenceDescriptor != null)
			{
				if (VSSpecialFolder.CheckForUserFolder(base.ParentFolder))
				{
					this._referenceDescriptor.SetProjectOutputProperty(this.Group, OutputGroupProperties.HKCUKey, "Software\\[Manufacturer]\\[ProductName]\\Installer");
					this._referenceDescriptor.SetProjectOutputProperty(this.Group, OutputGroupProperties.KeyPath, false);
					this._referenceDescriptor.SetProjectOutputProperty(this.Group, OutputGroupProperties.IsInGAC, false);
					return;
				}
				this._referenceDescriptor.SetProjectOutputProperty(this.Group, OutputGroupProperties.HKCUKey, string.Empty);
				this._referenceDescriptor.SetProjectOutputProperty(this.Group, OutputGroupProperties.KeyPath, true);
				if (base.ParentFolder is VSGACFolder)
				{
					this._referenceDescriptor.SetProjectOutputProperty(this.Group, OutputGroupProperties.IsInGAC, true);
					return;
				}
				this._referenceDescriptor.SetProjectOutputProperty(this.Group, OutputGroupProperties.IsInGAC, false);
			}
		}

		private WiXComponent CreateDirectoryComponent()
		{
			WiXComponent wiXComponent = null;
			XmlElement xmlElement = Common.CreateXmlElementWithAttributes(base.WiXElement.XmlNode.OwnerDocument, "Component", base.WiXElement.XmlNode.NamespaceURI, new string[] { "Id", "Guid" }, new string[] { string.Concat("comp_dir_", Common.GenerateId(base.Project.ProjectType)), Common.GenerateGuid() }, "", false);
			if (!base.WiXElement.XmlNode.HasChildNodes)
			{
				base.WiXElement.XmlNode.AppendChild(xmlElement);
			}
			else
			{
				base.WiXElement.XmlNode.InsertBefore(xmlElement, base.WiXElement.XmlNode.FirstChild);
			}
			wiXComponent = new WiXComponent(base.Project, base.WiXElement.Owner, base.WiXElement, xmlElement);
			XmlElement xmlElement1 = Common.CreateXmlElementWithAttributes(base.WiXElement.XmlNode.OwnerDocument, "RegistryValue", base.WiXElement.XmlNode.NamespaceURI, new string[] { "Root", "Key", "Name", "Type", "Value", "KeyPath" }, new string[] { "HKCU", "Software\\[Manufacturer]\\[ProductName]\\Installer", wiXComponent.Id, "string", base.WiXElement.Id, "yes" }, "", false);
			wiXComponent.XmlNode.AppendChild(xmlElement1);
			WiXRegistryValue wiXRegistryValue = new WiXRegistryValue(base.Project, base.WiXElement.Owner, wiXComponent, xmlElement1);
			XmlElement xmlElement2 = Common.CreateXmlElementWithAttributes(base.WiXElement.XmlNode.OwnerDocument, "RemoveFolder", base.WiXElement.XmlNode.NamespaceURI, new string[] { "Id", "On", "Directory" }, new string[] { string.Concat("id_", Common.GenerateId(base.Project.ProjectType)), "uninstall", base.WiXElement.Id }, "", false);
			wiXComponent.XmlNode.AppendChild(xmlElement2);
			WiXRemoveFolder wiXRemoveFolder = new WiXRemoveFolder(base.Project, base.WiXElement.Owner, wiXComponent, xmlElement2);
			wiXComponent.Parent.SetDirty();
			return wiXComponent;
		}

		private WiXComponentRef CreateDirectoryComponentRef()
		{
			WiXComponentRef wiXComponentRef = null;
			if (base.Project.Features.Count > 0)
			{
				VSFeature item = base.Project.Features[0];
				XmlElement xmlElement = Common.CreateXmlElementWithAttributes(item.WiXElement.XmlNode.OwnerDocument, "ComponentRef", item.WiXElement.XmlNode.NamespaceURI, new string[] { "Id" }, new string[] { this._directoryComponent.Id }, "", false);
				item.WiXElement.XmlNode.AppendChild(xmlElement);
				wiXComponentRef = new WiXComponentRef(base.Project, item.WiXElement.Owner, item.WiXElement, xmlElement);
				item.WiXElement.SetDirty();
			}
			return wiXComponentRef;
		}

		public override void Delete()
		{
			Predicate<WiXEntity> predicate = null;
			OutputGroup group = this.Group;
			string id = base.WiXElement.Id;
			if (this._directoryComponent != null)
			{
				this._directoryComponent.Delete();
				this._directoryComponent = null;
			}
			if (this._directoryComponentRef != null)
			{
				this._directoryComponentRef.Delete();
				this._directoryComponentRef = null;
			}
			base.Delete();
			if (base.Project.FileSystem.GetProjectOutputById(id) == null)
			{
				if (this._referenceDescriptor != null && this._referenceDescriptor.ReferencedProject != null)
				{
					base.Project.ProjectManager.RemoveProjectOutput(group, this._referenceDescriptor.ReferencedProject);
				}
				if (base.Project.Features.Count > 0)
				{
					for (int i = 0; i < base.Project.Features.Count; i++)
					{
						VSFeature item = base.Project.Features[i];
						WiXEntityList childEntities = item.WiXElement.ChildEntities;
						Predicate<WiXEntity> predicate1 = predicate;
						if (predicate1 == null)
						{
							Predicate<WiXEntity> predicate2 = (WiXEntity e) => {
								if (!(e is WiXComponentGroupRef))
								{
									return false;
								}
								return (e as WiXComponentGroupRef).Id == id;
							};
							Predicate<WiXEntity> predicate3 = predicate2;
							predicate = predicate2;
							predicate1 = predicate3;
						}
						List<WiXEntity> wiXEntities = childEntities.FindAll(predicate1);
						if (wiXEntities != null && wiXEntities.Count > 0)
						{
							while (wiXEntities.Count > 0)
							{
								wiXEntities[0].Delete();
								wiXEntities.Remove(wiXEntities[0]);
							}
							item.WiXElement.SetDirty();
						}
					}
				}
			}
			this._referenceDescriptor = null;
			this._keyOutputDescriptor = null;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this._referenceDescriptor = null;
				this._keyOutputDescriptor = null;
				this._outputs = null;
				this._directoryComponent = null;
				this._directoryComponentRef = null;
			}
			base.Dispose(disposing);
		}

		private void DoReferenceRemoved(VsWiXProject.ReferenceDescriptor reference)
		{
			if (reference == this._referenceDescriptor)
			{
				this._referenceDescriptor = null;
				this._keyOutputDescriptor = null;
			}
		}

		private void DoReferenceRenamed(VsWiXProject.ReferenceDescriptor oldReference, VsWiXProject.ReferenceDescriptor newReference)
		{
		}

		protected override string GetComponentName()
		{
			OutputGroup group = this.Group;
			if (group > OutputGroup.Content)
			{
				if (group == OutputGroup.Satellites)
				{
					return "Localized Resources";
				}
				if (group == OutputGroup.Documents)
				{
					return "Documentation Files";
				}
			}
			else
			{
				switch (group)
				{
					case OutputGroup.Binaries:
					{
						return "Primary Output";
					}
					case OutputGroup.Symbols:
					{
						return "Debug Symbols";
					}
					case OutputGroup.Binaries | OutputGroup.Symbols:
					{
						break;
					}
					case OutputGroup.Sources:
					{
						return "Source Files";
					}
					default:
					{
						if (group == OutputGroup.Content)
						{
							return "Content Files";
						}
						break;
					}
				}
			}
			return "Primary output";
		}

		private OutputGroup GetOutputGroupFromId()
		{
			if (base.WiXElement != null && !string.IsNullOrEmpty(base.WiXElement.Id))
			{
				int num = base.WiXElement.Id.LastIndexOf(".");
				if (num > 0)
				{
					string str = base.WiXElement.Id.Substring(num);
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
			}
			return OutputGroup.None;
		}
	}
}