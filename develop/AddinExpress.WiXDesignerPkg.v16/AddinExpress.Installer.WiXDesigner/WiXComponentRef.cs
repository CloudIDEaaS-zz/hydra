using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXComponentRef : WiXEntity
	{
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

		internal string Primary
		{
			get
			{
				return base.GetAttributeValue(this.Primary);
			}
			set
			{
				base.SetAttributeValue("Primary", value);
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

		internal WiXComponentRef(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}