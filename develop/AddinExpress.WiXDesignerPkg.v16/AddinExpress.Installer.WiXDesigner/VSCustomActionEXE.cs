using System;
using System.ComponentModel;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSCustomActionEXE : VSCustomActionBase
	{
		[Browsable(true)]
		public override string Arguments
		{
			get
			{
				return base.Arguments;
			}
			set
			{
				base.Arguments = value;
			}
		}

		[Browsable(true)]
		public override bool InstallerClass
		{
			get
			{
				return base.InstallerClass;
			}
			set
			{
				base.InstallerClass = value;
			}
		}

		public VSCustomActionEXE()
		{
		}

		public VSCustomActionEXE(WiXProjectParser project, VSCustomActions collection, AddinExpress.Installer.WiXDesigner.WiXCustomAction wixElement, AddinExpress.Installer.WiXDesigner.WiXCustomAction wixElementSetProperty, AddinExpress.Installer.WiXDesigner.WiXCustom wixCustom, AddinExpress.Installer.WiXDesigner.WiXCustom wixCustomSetProperty, VSBaseFile vsFile) : base(project, collection, wixElement, wixElementSetProperty, wixCustom, wixCustomSetProperty, vsFile)
		{
		}
	}
}