using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class IISRecycleTime : WiXEntity
	{
		public override object SupportedObject
		{
			get
			{
				return this;
			}
		}

		internal string Value
		{
			get
			{
				return base.GetAttributeValue("Value");
			}
			set
			{
				base.SetAttributeValue("Value", value);
				this.SetDirty();
			}
		}

		internal IISRecycleTime(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}