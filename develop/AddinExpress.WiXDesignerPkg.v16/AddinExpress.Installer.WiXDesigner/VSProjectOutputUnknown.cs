using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSProjectOutputUnknown : VSBaseFile
	{
		protected WiXDirectory _wixElement;

		internal override bool CanRename
		{
			get
			{
				return false;
			}
		}

		[Browsable(true)]
		[DefaultValue(null)]
		[Description("Specifies the folder where where the selected files will be installed on the target computer")]
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
		[DisplayName("(Name)")]
		[ReadOnly(true)]
		public override string Name
		{
			get
			{
				return string.Concat("Primary output from ", this.WiXElement.Id);
			}
			set
			{
			}
		}

		internal WiXDirectory WiXElement
		{
			get
			{
				return this._wixElement;
			}
		}

		public VSProjectOutputUnknown(WiXProjectParser project, VSBaseFolder parent) : base(project, parent)
		{
		}

		public VSProjectOutputUnknown(WiXProjectParser project, VSBaseFolder parent, WiXDirectory wixElement) : base(project, parent)
		{
			this._wixElement = wixElement;
		}

		internal virtual void AdjustKeyPath()
		{
		}

		public override void Delete()
		{
			if (base.ParentComponent != null)
			{
				base.ParentComponent.Files.Remove(this);
			}
			else if (base.ParentFolder != null)
			{
				base.ParentFolder.ProjectOutputs.Remove(this);
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

		protected override string GetComponentName()
		{
			return "Primary output";
		}

		internal override void MoveTo(VSBaseFolder destinationFolder)
		{
			if (base.ParentFolder != destinationFolder)
			{
				base.ParentFolder.ProjectOutputs.Remove(this);
				destinationFolder.ProjectOutputs.Add(this);
				this._parentFolder = destinationFolder;
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