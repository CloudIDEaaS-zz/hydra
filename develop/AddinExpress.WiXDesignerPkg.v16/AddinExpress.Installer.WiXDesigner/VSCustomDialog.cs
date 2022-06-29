using System;
using System.ComponentModel;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSCustomDialog : VSDialogBase
	{
		[Browsable(true)]
		[DisplayName("(Name)")]
		[ReadOnly(true)]
		public override string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		public VSCustomDialog(WiXProjectParser project) : base(project)
		{
		}

		public VSCustomDialog(WiXProjectParser project, VSUserInterface collection, AddinExpress.Installer.WiXDesigner.WiXDialog wixElement, AddinExpress.Installer.WiXDesigner.DialogType dialogType, AddinExpress.Installer.WiXDesigner.DialogStage dialogStage, AddinExpress.Installer.WiXDesigner.DialogScope dialogScope) : base(project, collection, wixElement, dialogType, dialogStage, dialogScope)
		{
		}

		public override void Delete()
		{
			base.Delete();
		}

		protected override void InitializeDialog()
		{
			base.InitializeDialog();
		}
	}
}