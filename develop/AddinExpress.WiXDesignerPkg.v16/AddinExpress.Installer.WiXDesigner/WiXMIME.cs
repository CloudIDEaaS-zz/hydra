using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXMIME : WiXEntity
	{
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

		internal string Class
		{
			get
			{
				return base.GetAttributeValue("Class");
			}
			set
			{
				base.SetAttributeValue("Class", value);
				this.SetDirty();
			}
		}

		internal string ContentType
		{
			get
			{
				return base.GetAttributeValue("ContentType");
			}
			set
			{
				base.SetAttributeValue("ContentType", value);
				this.SetDirty();
			}
		}

		internal string Default
		{
			get
			{
				return base.GetAttributeValue("Default");
			}
			set
			{
				base.SetAttributeValue("Default", value);
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

		internal WiXMIME(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}