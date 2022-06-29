using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXInstallExecuteSequence : WiXEntity
	{
		public override object SupportedObject
		{
			get
			{
				return this;
			}
		}

		internal WiXInstallExecuteSequence(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}