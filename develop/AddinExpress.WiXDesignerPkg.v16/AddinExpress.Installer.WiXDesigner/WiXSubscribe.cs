using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXSubscribe : WiXEntity
	{
		internal string Attribute
		{
			get
			{
				return base.GetAttributeValue("Attribute");
			}
			set
			{
				base.SetAttributeValue("Attribute", value);
				this.SetDirty();
			}
		}

		internal string Event
		{
			get
			{
				return base.GetAttributeValue("Event");
			}
			set
			{
				base.SetAttributeValue("Event", value);
				this.SetDirty();
			}
		}

		public override object SupportedObject
		{
			get
			{
				return null;
			}
		}

		internal WiXSubscribe(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}