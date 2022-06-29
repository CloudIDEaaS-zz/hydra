using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class IISWebServiceExtension : WiXEntity
	{
		internal string Allow
		{
			get
			{
				return base.GetAttributeValue("Allow");
			}
			set
			{
				base.SetAttributeValue("Allow", value);
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

		internal string File
		{
			get
			{
				return base.GetAttributeValue("File");
			}
			set
			{
				base.SetAttributeValue("File", value);
				this.SetDirty();
			}
		}

		internal string Group
		{
			get
			{
				return base.GetAttributeValue("Group");
			}
			set
			{
				base.SetAttributeValue("Group", value);
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

		public override object SupportedObject
		{
			get
			{
				return this;
			}
		}

		internal string UIDeletable
		{
			get
			{
				return base.GetAttributeValue("UIDeletable");
			}
			set
			{
				base.SetAttributeValue("UIDeletable", value);
				this.SetDirty();
			}
		}

		internal IISWebServiceExtension(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}