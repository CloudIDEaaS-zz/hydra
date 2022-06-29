using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXAdminUISequence : WiXEntity
	{
		public override object SupportedObject
		{
			get
			{
				return this;
			}
		}

		internal WiXAdminUISequence(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}