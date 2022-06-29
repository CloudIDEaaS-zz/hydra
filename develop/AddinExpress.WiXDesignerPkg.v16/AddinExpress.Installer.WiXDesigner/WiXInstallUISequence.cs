using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXInstallUISequence : WiXEntity
	{
		public override object SupportedObject
		{
			get
			{
				return this;
			}
		}

		internal WiXInstallUISequence(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}