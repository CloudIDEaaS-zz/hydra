using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXExtension : WiXEntity
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

		internal WiXExtension(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}