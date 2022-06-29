using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSShortcut : VSBaseFile
	{
		private WiXShortcut _wixElement;

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Specifies the command-line arguments for the shortcut")]
		public string Arguments
		{
			get
			{
				return this.WiXElement.Arguments;
			}
			set
			{
				this.WiXElement.Arguments = value;
			}
		}

		internal override bool CanRename
		{
			get
			{
				return true;
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Specifies the description that will appear in the Tooltip for the shortcut")]
		public string Description
		{
			get
			{
				return this.WiXElement.Description;
			}
			set
			{
				this.WiXElement.Description = value;
			}
		}

		[Browsable(true)]
		[DefaultValue(null)]
		[Description("Specifies the folder where the selected shortcut will be installed on the target computer")]
		public override VSBaseFolder Folder
		{
			get
			{
				return base.Folder;
			}
			set
			{
				base.Folder = value;
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Specifies an icon to be displayed for the shortcut")]
		[Editor(typeof(IconPropertyEditor), typeof(UITypeEditor))]
		[MergableProperty(false)]
		[TypeConverter(typeof(IconPropertyConverter))]
		public string Icon
		{
			get
			{
				if (!string.IsNullOrEmpty(this.WiXElement.Icon))
				{
					return this.WiXElement.Icon;
				}
				if (this.WiXElement.IconElement == null)
				{
					return string.Empty;
				}
				return this.WiXElement.IconElement.SourceFile;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					if (Common.FileExists(value, this.WiXElement.Owner as WiXProjectItem))
					{
						this.WiXElement.Icon = null;
						if (this.WiXElement.IconElement == null)
						{
							this.WiXElement.CreateIconElement();
						}
						this.WiXElement.IconElement.SourceFile = value;
						return;
					}
					if (this.WiXElement.IconElement != null)
					{
						this.WiXElement.DeleteIconElement();
					}
					this.WiXElement.Icon = value;
				}
				else
				{
					this.WiXElement.Icon = null;
					if (this.WiXElement.IconElement != null)
					{
						this.WiXElement.DeleteIconElement();
						return;
					}
				}
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Specifies the name of the shortcut as it will appear when installed on a target computer")]
		[DisplayName("(Name)")]
		[ReadOnly(false)]
		public override string Name
		{
			get
			{
				return this.WiXElement.Name;
			}
			set
			{
				this.WiXElement.Name = value;
				this.DoPropertyChanged();
			}
		}

		[Browsable(true)]
		[DefaultValue(ShowCmdMode.vsdscNormal)]
		[Description("Specifies the initial window state for an application called by the shortcut")]
		public ShowCmdMode ShowCmd
		{
			get
			{
				if (this.WiXElement.Show != null)
				{
					if (this.WiXElement.Show == "normal")
					{
						return ShowCmdMode.vsdscNormal;
					}
					if (this.WiXElement.Show == "minimized")
					{
						return ShowCmdMode.vsdscMinimized;
					}
					if (this.WiXElement.Show == "maximized")
					{
						return ShowCmdMode.vsdscMaximized;
					}
				}
				return ShowCmdMode.vsdscNormal;
			}
			set
			{
				if (value == ShowCmdMode.vsdscMinimized)
				{
					this.WiXElement.Show = "minimized";
					return;
				}
				if (value == ShowCmdMode.vsdscMaximized)
				{
					this.WiXElement.Show = "maximized";
					return;
				}
				this.WiXElement.Show = "normal";
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Specifies the application to be launched by a shortcut")]
		[Editor(typeof(TargetPropertyEditor), typeof(UITypeEditor))]
		[MergableProperty(false)]
		[TypeConverter(typeof(TargetPropertyConverter))]
		public string Target
		{
			get
			{
				return this.WiXElement.Target;
			}
			set
			{
				this.WiXElement.Target = value;
			}
		}

		[Browsable(true)]
		[DefaultValue(false)]
		public override bool Transitive
		{
			get
			{
				return base.Transitive;
			}
			set
			{
				base.Transitive = value;
			}
		}

		internal WiXShortcut WiXElement
		{
			get
			{
				return this._wixElement;
			}
		}

		[Browsable(true)]
		[DefaultValue(null)]
		[Description("Specifies the folder where the target application for the shortcut will be installed")]
		[Editor(typeof(FolderPropertyEditor), typeof(UITypeEditor))]
		[MergableProperty(false)]
		[TypeConverter(typeof(FolderPropertyConverter))]
		public VSBaseFolder WorkingFolder
		{
			get
			{
				return this._project.FileSystem.GetFolderById(this.WiXElement.WorkingDirectory);
			}
			set
			{
				if (value != null)
				{
					this.WiXElement.WorkingDirectory = value.WiXElement.Id;
				}
			}
		}

		public VSShortcut()
		{
		}

		public VSShortcut(WiXProjectParser project, VSBaseFolder parent, WiXShortcut wixElement) : base(project, parent)
		{
			this._wixElement = wixElement;
		}

		public VSShortcut(WiXProjectParser project, VSComponent parent, WiXShortcut wixElement) : base(project, parent)
		{
			this._wixElement = wixElement;
		}

		public VSShortcut(VSBaseFolder parent, VSBaseFolder targetFolder) : base(parent._project, parent)
		{
			if (base.ParentComponent == null)
			{
				base.ParentComponent = base.CreateComponent(parent);
			}
			if (base.ParentComponent == null)
			{
				throw new Exception(string.Concat("Cannot create WiX Component for ", targetFolder.Name, " folder."));
			}
			XmlElement xmlElement = Common.CreateXmlElementWithAttributes(base.ParentComponent.WiXElement.XmlNode.OwnerDocument, "Shortcut", base.ParentComponent.WiXElement.XmlNode.NamespaceURI, new string[] { "Id", "Directory", "Name", "Show", "Target", "WorkingDirectory" }, new string[] { string.Concat("_", Common.GenerateId(this._project.ProjectType)), parent.Property, string.Concat("Shortcut to ", targetFolder.Name), "normal", string.Concat("[", targetFolder.Property, "]"), parent.Property }, "", false);
			base.ParentComponent.WiXElement.XmlNode.AppendChild(xmlElement);
			this._wixElement = new WiXShortcut(this._project, base.ParentComponent.WiXElement.Owner, base.ParentComponent.WiXElement, xmlElement);
			this._wixElement.Parent.SetDirty();
			base.ParentComponent.AdjustKeyPath();
		}

		public VSShortcut(VSBaseFolder parent, VSBaseFile targetFile) : base(parent._project, parent)
		{
			string empty = string.Empty;
			if (targetFile is VSProjectOutputVDProj)
			{
				if ((targetFile as VSProjectOutputVDProj).KeyOutput != null)
				{
					empty = (targetFile as VSProjectOutputVDProj).KeyOutput.TargetName;
				}
			}
			else if (targetFile is VSProjectOutputFile)
			{
				empty = (targetFile as VSProjectOutputFile).TargetName;
			}
			else if (targetFile is VSFile)
			{
				empty = (targetFile as VSFile).TargetName;
			}
			else if (targetFile is VSAssembly)
			{
				empty = (targetFile as VSAssembly).TargetName;
			}
			if (base.ParentComponent == null)
			{
				base.ParentComponent = base.CreateComponent(parent);
			}
			if (base.ParentComponent == null)
			{
				throw new Exception(string.Concat("Cannot create WiX Component for ", empty, " file."));
			}
			XmlElement xmlElement = Common.CreateXmlElementWithAttributes(base.ParentComponent.WiXElement.XmlNode.OwnerDocument, "Shortcut", base.ParentComponent.WiXElement.XmlNode.NamespaceURI, new string[] { "Id", "Directory", "Name", "Show", "Target", "WorkingDirectory" }, new string[] { string.Concat("_", Common.GenerateId(this._project.ProjectType)), parent.Property, string.Concat("Shortcut to ", empty), "normal", string.Concat("[", targetFile.ParentFolder.Property, "]", empty), parent.Property }, "", false);
			base.ParentComponent.WiXElement.XmlNode.AppendChild(xmlElement);
			this._wixElement = new WiXShortcut(this._project, base.ParentComponent.WiXElement.Owner, base.ParentComponent.WiXElement, xmlElement);
			if (targetFile is VSProjectOutputFile)
			{
				this._wixElement.Name = string.Concat("Shortcut to ", (targetFile as VSProjectOutputFile).Name);
			}
			this._wixElement.Parent.SetDirty();
			if (!base.ParentComponent.Files.Contains(this))
			{
				base.ParentComponent.Files.Add(this);
			}
			base.ParentComponent.AdjustKeyPath();
		}

		internal void AdjustDirectories()
		{
			if (base.ParentFolder != null)
			{
				this.WiXElement.Directory = base.ParentFolder.Property;
				this.WiXElement.WorkingDirectory = base.ParentFolder.Property;
			}
		}

		public override void Delete()
		{
			if (base.ParentComponent != null)
			{
				base.ParentComponent.Files.Remove(this);
			}
			this.WiXElement.Delete();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this._wixElement = null;
			}
			base.Dispose(disposing);
		}

		internal override void MoveTo(VSBaseFolder destinationFolder)
		{
			if (base.ParentComponent.Parent != destinationFolder)
			{
				if (base.ParentComponent.Files.Count != 1 || base.ParentComponent == base.ParentFolder.GetDirectoryComponent())
				{
					VSComponent comPlusFlags = base.CreateComponent(destinationFolder);
					VSComponent parentComponent = base.ParentComponent;
					comPlusFlags.WiXElement.ComPlusFlags = parentComponent.WiXElement.ComPlusFlags;
					comPlusFlags.WiXElement.DisableRegistryReflection = parentComponent.WiXElement.DisableRegistryReflection;
					comPlusFlags.WiXElement.DiskId = parentComponent.WiXElement.DiskId;
					comPlusFlags.WiXElement.Feature = parentComponent.WiXElement.Feature;
					comPlusFlags.WiXElement.KeyPath = parentComponent.WiXElement.KeyPath;
					comPlusFlags.WiXElement.Location = parentComponent.WiXElement.Location;
					comPlusFlags.WiXElement.MultiInstance = parentComponent.WiXElement.MultiInstance;
					comPlusFlags.WiXElement.NeverOverwrite = parentComponent.WiXElement.NeverOverwrite;
					comPlusFlags.WiXElement.Permanent = parentComponent.WiXElement.Permanent;
					comPlusFlags.WiXElement.Shared = parentComponent.WiXElement.Shared;
					comPlusFlags.WiXElement.SharedDllRefCount = parentComponent.WiXElement.SharedDllRefCount;
					comPlusFlags.WiXElement.Transitive = parentComponent.WiXElement.Transitive;
					comPlusFlags.WiXElement.UninstallWhenSuperseded = parentComponent.WiXElement.UninstallWhenSuperseded;
					if (parentComponent.WiXCondition != null)
					{
						comPlusFlags.CreateWiXCondition();
						comPlusFlags.WiXCondition.Condition = parentComponent.WiXCondition.Condition;
					}
					if (parentComponent.WiXKeyPathElement != null)
					{
						comPlusFlags.CreateWiXKeyPathElement();
					}
					base.ParentComponent = comPlusFlags;
					this.WiXElement.Parent.ChildEntities.Remove(this.WiXElement);
					this.WiXElement.XmlNode.ParentNode.RemoveChild(this.WiXElement.XmlNode);
					this.WiXElement.Parent.SetDirty();
					if (this.WiXElement.Owner != comPlusFlags.WiXElement.Owner)
					{
						this.WiXElement.RebuildXmlNodes(comPlusFlags.WiXElement.XmlNode.OwnerDocument, comPlusFlags.WiXElement.Owner);
					}
					comPlusFlags.WiXElement.ChildEntities.Add(this.WiXElement);
					comPlusFlags.WiXElement.XmlNode.AppendChild(this.WiXElement.XmlNode);
					this.WiXElement.Parent = comPlusFlags.WiXElement;
					this.WiXElement.Parent.SetDirty();
				}
				else
				{
					base.ParentComponent.Parent = destinationFolder;
					base.ParentComponent.WiXElement.Parent.ChildEntities.Remove(base.ParentComponent.WiXElement);
					base.ParentComponent.WiXElement.XmlNode.ParentNode.RemoveChild(base.ParentComponent.WiXElement.XmlNode);
					base.ParentComponent.WiXElement.Parent.SetDirty();
					if (base.ParentComponent.WiXElement.Owner != destinationFolder.WiXElement.Owner)
					{
						base.ParentComponent.WiXElement.RebuildXmlNodes(destinationFolder.WiXElement.XmlNode.OwnerDocument, destinationFolder.WiXElement.Owner);
					}
					destinationFolder.WiXElement.ChildEntities.Add(base.ParentComponent.WiXElement);
					destinationFolder.WiXElement.XmlNode.AppendChild(base.ParentComponent.WiXElement.XmlNode);
					base.ParentComponent.WiXElement.Parent = destinationFolder.WiXElement;
					base.ParentComponent.WiXElement.Parent.SetDirty();
				}
				base.ParentComponent.AdjustKeyPath();
				this.AdjustDirectories();
			}
		}

		internal override void MoveTo(VSComponent destinationComponent)
		{
			VSComponent parentComponent = base.ParentComponent;
			this.WiXElement.Parent.ChildEntities.Remove(this.WiXElement);
			this.WiXElement.XmlNode.ParentNode.RemoveChild(this.WiXElement.XmlNode);
			this.WiXElement.Parent.SetDirty();
			if (this.WiXElement.Owner != destinationComponent.WiXElement.Owner)
			{
				this.WiXElement.RebuildXmlNodes(destinationComponent.WiXElement.XmlNode.OwnerDocument, destinationComponent.WiXElement.Owner);
			}
			destinationComponent.WiXElement.ChildEntities.Add(this.WiXElement);
			destinationComponent.WiXElement.XmlNode.AppendChild(this.WiXElement.XmlNode);
			this.WiXElement.Parent = destinationComponent.WiXElement;
			this.WiXElement.Parent.SetDirty();
			base.ParentComponent = destinationComponent;
			if (parentComponent.Files.Count > 0)
			{
				parentComponent.AdjustKeyPath();
			}
			else if (parentComponent.CanBeDeleted)
			{
				parentComponent.Delete();
			}
			this.AdjustDirectories();
		}
	}
}