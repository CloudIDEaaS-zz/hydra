using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class IISWebAddress : WiXEntity
	{
		internal string Header
		{
			get
			{
				return base.GetAttributeValue("Header");
			}
			set
			{
				base.SetAttributeValue("Header", value);
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

		internal string IP
		{
			get
			{
				return base.GetAttributeValue("IP");
			}
			set
			{
				base.SetAttributeValue("IP", value);
				this.SetDirty();
			}
		}

		internal string Port
		{
			get
			{
				return base.GetAttributeValue("Port");
			}
			set
			{
				base.SetAttributeValue("Port", value);
				this.SetDirty();
			}
		}

		internal string Secure
		{
			get
			{
				return base.GetAttributeValue("Secure");
			}
			set
			{
				base.SetAttributeValue("Secure", value);
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

		internal IISWebAddress(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}