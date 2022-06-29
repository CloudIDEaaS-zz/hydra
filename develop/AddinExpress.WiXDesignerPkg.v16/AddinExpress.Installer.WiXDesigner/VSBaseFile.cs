using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSBaseFile : VSComponentBase
	{
		private VSComponent _parentComponent;

		protected VSBaseFolder _parentFolder;

		protected WiXProjectParser _project;

		public readonly static VSBaseFile Empty;

		internal override bool CanRename
		{
			get
			{
				return true;
			}
		}

		[Browsable(false)]
		[DefaultValue(null)]
		[Description("Specifies the folder where where the selected file will be installed on the target computer")]
		[Editor(typeof(FolderPropertyEditor), typeof(UITypeEditor))]
		[MergableProperty(false)]
		[TypeConverter(typeof(FolderPropertyConverter))]
		public virtual VSBaseFolder Folder
		{
			get
			{
				if (this.ParentComponent == null)
				{
					return this._parentFolder;
				}
				return this.ParentComponent.Parent;
			}
			set
			{
				if (value != null)
				{
					this.MoveTo(value);
					this.DoParentChanged();
				}
			}
		}

		internal VSComponent ParentComponent
		{
			get
			{
				return this._parentComponent;
			}
			set
			{
				if (this._parentComponent != null)
				{
					this._parentComponent.Files.Remove(this);
				}
				this._parentComponent = value;
				if (this._parentComponent != null)
				{
					this._parentComponent.Files.Add(this);
				}
			}
		}

		internal VSBaseFolder ParentFolder
		{
			get
			{
				if (this.ParentComponent == null)
				{
					return this._parentFolder;
				}
				return this.ParentComponent.Parent;
			}
		}

		internal WiXProjectParser Project
		{
			get
			{
				return this._project;
			}
		}

		[Browsable(false)]
		[DefaultValue("")]
		public virtual string SourcePath
		{
			get
			{
				return string.Empty;
			}
			set
			{
			}
		}

		[Browsable(false)]
		[DefaultValue("")]
		public virtual string TargetName
		{
			get
			{
				return string.Empty;
			}
			set
			{
			}
		}

		[Browsable(false)]
		[DefaultValue(false)]
		[Description("Determines whether the installer will reevaluate the Condition property for the selected item when reinstalling on a target computer")]
		public virtual bool Transitive
		{
			get
			{
				if (this.ParentComponent == null)
				{
					return false;
				}
				return this.ParentComponent.WiXElement.Transitive == "yes";
			}
			set
			{
				if (this.ParentComponent != null)
				{
					this.ParentComponent.WiXElement.Transitive = (value ? "yes" : "no");
				}
			}
		}

		static VSBaseFile()
		{
			VSBaseFile.Empty = new VSBaseFile();
		}

		protected VSBaseFile()
		{
		}

		public VSBaseFile(WiXProjectParser project, VSBaseFolder parent) : this()
		{
			this._project = project;
			this._parentFolder = parent;
		}

		public VSBaseFile(WiXProjectParser project, VSComponent parent) : this()
		{
			this._project = project;
			this._parentComponent = parent;
		}

		protected VSComponent CreateComponent(VSBaseFolder destinationFolder)
		{
			WiXComponent wiXComponent = null;
			WiXComponentRef wiXComponentRef = null;
			VSComponentGroup vSComponentGroup = this._project.ComponentGroups.Find((VSComponentGroup e) => e.WiXElement.Directory == destinationFolder.Property);
			if (vSComponentGroup == null)
			{
				XmlElement xmlElement = Common.CreateXmlElementWithAttributes(destinationFolder.WiXElement.XmlNode.OwnerDocument, "Component", destinationFolder.WiXElement.XmlNode.NamespaceURI, new string[] { "Id", "Guid", "Permanent", "SharedDllRefCount", "Transitive" }, new string[] { string.Concat("comp_", Common.GenerateId(this._project.ProjectType)), Common.GenerateGuid(), "no", "no", "no" }, "", false);
				XmlNode itemOf = null;
				if (destinationFolder.WiXElement.XmlNode.HasChildNodes)
				{
					int num = 0;
					while (num < destinationFolder.WiXElement.XmlNode.ChildNodes.Count)
					{
						if (destinationFolder.WiXElement.XmlNode.ChildNodes[num].LocalName.ToLower() != "directory")
						{
							num++;
						}
						else
						{
							itemOf = destinationFolder.WiXElement.XmlNode.ChildNodes[num];
							break;
						}
					}
				}
				if (itemOf == null)
				{
					destinationFolder.WiXElement.XmlNode.AppendChild(xmlElement);
				}
				else
				{
					destinationFolder.WiXElement.XmlNode.InsertBefore(xmlElement, itemOf);
				}
				wiXComponent = new WiXComponent(this._project, destinationFolder.WiXElement.Owner, destinationFolder.WiXElement, xmlElement);
				wiXComponent.Parent.SetDirty();
				if (this._project.ProjectType == WiXProjectType.Product)
				{
					WiXEntity wiXElement = null;
					VSFeature vSFeature = this._project.Features.Find((VSFeature e) => e.WiXElement.Level == "1");
					if (vSFeature != null)
					{
						wiXElement = vSFeature.WiXElement;
					}
					if (wiXElement != null)
					{
						XmlElement xmlElement1 = Common.CreateXmlElementWithAttributes(wiXElement.XmlNode.OwnerDocument, "ComponentRef", wiXElement.XmlNode.NamespaceURI, new string[] { "Id" }, new string[] { wiXComponent.Id }, "", false);
						wiXElement.XmlNode.AppendChild(xmlElement1);
						wiXComponentRef = new WiXComponentRef(this._project, wiXElement.Owner, wiXElement, xmlElement1);
						wiXComponentRef.Parent.SetDirty();
					}
				}
			}
			else
			{
				XmlElement xmlElement2 = Common.CreateXmlElementWithAttributes(vSComponentGroup.WiXElement.XmlNode.OwnerDocument, "Component", vSComponentGroup.WiXElement.XmlNode.NamespaceURI, new string[] { "Id", "Guid", "Permanent", "SharedDllRefCount", "Transitive" }, new string[] { string.Concat("comp_", Common.GenerateId(this._project.ProjectType)), Common.GenerateGuid(), "no", "no", "no" }, "", false);
				vSComponentGroup.WiXElement.XmlNode.AppendChild(xmlElement2);
				wiXComponent = new WiXComponent(this._project, vSComponentGroup.WiXElement.Owner, vSComponentGroup.WiXElement, xmlElement2);
				wiXComponent.Parent.SetDirty();
			}
			if (wiXComponent == null)
			{
				return null;
			}
			if (VSSpecialFolder.CheckFor64BitFolder(destinationFolder))
			{
				wiXComponent.Win64 = "yes";
			}
			VSComponent vSComponent = new VSComponent(destinationFolder, wiXComponent);
			if (wiXComponentRef != null)
			{
				vSComponent.ComponentRefs.Add(wiXComponentRef);
			}
			destinationFolder.Components.Add(vSComponent);
			return vSComponent;
		}

		public override void Delete()
		{
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this._project = null;
				this._parentComponent = null;
				this._parentFolder = null;
			}
			base.Dispose(disposing);
		}

		internal virtual void MoveTo(VSBaseFolder destinationFolder)
		{
		}

		internal virtual void MoveTo(VSComponent destinationComponent)
		{
		}
	}
}