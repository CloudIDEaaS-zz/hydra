using System;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSCustomActionScript : VSCustomActionBase
	{
		public VSCustomActionScript()
		{
		}

		public VSCustomActionScript(WiXProjectParser project, VSCustomActions collection, AddinExpress.Installer.WiXDesigner.WiXCustomAction wixElement, AddinExpress.Installer.WiXDesigner.WiXCustomAction wixElementSetProperty, AddinExpress.Installer.WiXDesigner.WiXCustom wixCustom, AddinExpress.Installer.WiXDesigner.WiXCustom wixCustomSetProperty, VSBaseFile vsFile) : base(project, collection, wixElement, wixElementSetProperty, wixCustom, wixCustomSetProperty, vsFile)
		{
		}
	}
}