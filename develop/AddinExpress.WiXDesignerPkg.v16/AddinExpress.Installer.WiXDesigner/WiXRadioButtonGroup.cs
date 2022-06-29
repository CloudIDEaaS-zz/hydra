using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXRadioButtonGroup : WiXEntity
	{
		internal string Property
		{
			get
			{
				return base.GetAttributeValue("Property");
			}
			set
			{
				base.SetAttributeValue("Property", value);
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

		internal WiXRadioButtonGroup(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}