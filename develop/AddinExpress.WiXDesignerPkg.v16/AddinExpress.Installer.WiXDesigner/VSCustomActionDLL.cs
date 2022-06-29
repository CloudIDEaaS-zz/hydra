using System;
using System.ComponentModel;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSCustomActionDLL : VSCustomActionBase
	{
		[Browsable(true)]
		public override string EntryPoint
		{
			get
			{
				return base.EntryPoint;
			}
			set
			{
				base.EntryPoint = value;
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

		public VSCustomActionDLL()
		{
		}

		public VSCustomActionDLL(WiXProjectParser project, VSCustomActions collection, AddinExpress.Installer.WiXDesigner.WiXCustomAction wixElement, AddinExpress.Installer.WiXDesigner.WiXCustomAction wixElementSetProperty, AddinExpress.Installer.WiXDesigner.WiXCustom wixCustom, AddinExpress.Installer.WiXDesigner.WiXCustom wixCustomSetProperty, VSBaseFile vsFile) : base(project, collection, wixElement, wixElementSetProperty, wixCustom, wixCustomSetProperty, vsFile)
		{
		}
	}
}