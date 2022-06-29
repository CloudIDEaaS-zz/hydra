using System;
using System.Collections.Generic;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXShortcut : WiXEntity
	{
		private WiXIcon _wixIconElement;

		internal string Advertise
		{
			get
			{
				return base.GetAttributeValue("Advertise");
			}
			set
			{
				base.SetAttributeValue("Advertise", value);
				this.SetDirty();
			}
		}

		internal string Arguments
		{
			get
			{
				return base.GetAttributeValue("Arguments");
			}
			set
			{
				base.SetAttributeValue("Arguments", value);
				this.SetDirty();
			}
		}

		internal string Description
		{
			get
			{
				return base.GetAttributeValue("Description");
			}
			set
			{
				base.SetAttributeValue("Description", value);
				this.SetDirty();
			}
		}

		internal string DescriptionResourceDll
		{
			get
			{
				return base.GetAttributeValue("DescriptionResourceDll");
			}
			set
			{
				base.SetAttributeValue("DescriptionResourceDll", value);
				this.SetDirty();
			}
		}

		internal string DescriptionResourceId
		{
			get
			{
				return base.GetAttributeValue("DescriptionResourceId");
			}
			set
			{
				base.SetAttributeValue("DescriptionResourceId", value);
				this.SetDirty();
			}
		}

		internal string Directory
		{
			get
			{
				return base.GetAttributeValue("Directory");
			}
			set
			{
				base.SetAttributeValue("Directory", value);
				this.SetDirty();
			}
		}

		internal string DisplayResourceDll
		{
			get
			{
				return base.GetAttributeValue("DisplayResourceDll");
			}
			set
			{
				base.SetAttributeValue("DisplayResourceDll", value);
				this.SetDirty();
			}
		}

		internal string DisplayResourceId
		{
			get
			{
				return base.GetAttributeValue("DisplayResourceId");
			}
			set
			{
				base.SetAttributeValue("DisplayResourceId", value);
				this.SetDirty();
			}
		}

		internal string Hotkey
		{
			get
			{
				return base.GetAttributeValue("Hotkey");
			}
			set
			{
				base.SetAttributeValue("Hotkey", value);
				this.SetDirty();
			}
		}

		internal string Icon
		{
			get
			{
				return base.GetAttributeValue("Icon");
			}
			set
			{
				base.SetAttributeValue("Icon", value);
				this.SetDirty();
			}
		}

		internal WiXIcon IconElement
		{
			get
			{
				return this._wixIconElement;
			}
			set
			{
				this._wixIconElement = value;
			}
		}

		internal string IconIndex
		{
			get
			{
				return base.GetAttributeValue("IconIndex");
			}
			set
			{
				base.SetAttributeValue("IconIndex", value);
				this.SetDirty();
			}
		}

		internal string Id
		{
			get
			{
				return base.GetAttributeValue("Id");
			}
			set
			{
				base.SetAttributeValue("Id", value);
				this.SetDirty();
			}
		}

		internal string LongName
		{
			get
			{
				return base.GetAttributeValue("LongName");
			}
			set
			{
				base.SetAttributeValue("LongName", value);
				this.SetDirty();
			}
		}

		internal new string Name
		{
			get
			{
				return base.GetAttributeValue("Name");
			}
			set
			{
				base.SetAttributeValue("Name", value);
				this.SetDirty();
			}
		}

		internal string ShortName
		{
			get
			{
				return base.GetAttributeValue("ShortName");
			}
			set
			{
				base.SetAttributeValue("ShortName", value);
				this.SetDirty();
			}
		}

		internal string Show
		{
			get
			{
				return base.GetAttributeValue("Show");
			}
			set
			{
				base.SetAttributeValue("Show", value);
				this.SetDirty();
			}
		}

		public override object SupportedObject
		{
			get
			{
				return this;
			}
		}

		internal string Target
		{
			get
			{
				return base.GetAttributeValue("Target");
			}
			set
			{
				base.SetAttributeValue("Target", value);
				this.SetDirty();
			}
		}

		internal string WorkingDirectory
		{
			get
			{
				return base.GetAttributeValue("WorkingDirectory");
			}
			set
			{
				base.SetAttributeValue("WorkingDirectory", value);
				this.SetDirty();
			}
		}

		internal WiXShortcut(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}

		internal void CreateIconElement()
		{
			System.Xml.XmlNode xmlNodes = Common.CreateXmlElementWithAttributes(base.XmlNode.OwnerDocument, "Icon", base.XmlNode.NamespaceURI, new string[] { "Id" }, new string[] { Common.GenerateIconId() }, "", false);
			this._wixIconElement = new WiXIcon(base.Project, this.Owner, this, xmlNodes);
			base.XmlNode.AppendChild(this.IconElement.XmlNode);
			this.SetDirty();
		}

		internal void DeleteIconElement()
		{
			this.ChildEntities.Remove(this.IconElement);
			base.XmlNode.RemoveChild(this.IconElement.XmlNode);
			this._wixIconElement = null;
			this.SetDirty();
		}
	}
}