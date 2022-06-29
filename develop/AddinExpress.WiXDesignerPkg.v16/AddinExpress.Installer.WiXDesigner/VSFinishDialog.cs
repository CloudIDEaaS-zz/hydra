using AddinExpress.Installer.WiXDesigner.DesignTime;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSFinishDialog : VSDialogBase
	{
		private WiXEntity _wixUpdateText;

		[Browsable(true)]
		[DefaultValue(null)]
		[Description("The bitmap or JPEG file containing the background image for the banner across the top of this dialog.")]
		[Editor(typeof(BannerBitmapEditor), typeof(UITypeEditor))]
		[MergableProperty(false)]
		[TypeConverter(typeof(BannerBitmapConverter))]
		public VSBaseFile BannerBitmap
		{
			get
			{
				return base.GetBannerBitmapValue();
			}
			set
			{
				base.SetBannerBitmapValue(value);
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("The update text to show at the bottom of this dialog.")]
		[MergableProperty(false)]
		public string UpdateText
		{
			get
			{
				if (base.DialogScope != AddinExpress.Installer.WiXDesigner.DialogScope.UserInstall)
				{
					return string.Empty;
				}
				return base.GetTextValue(this._wixUpdateText, "Text");
			}
			set
			{
				if (base.DialogScope == AddinExpress.Installer.WiXDesigner.DialogScope.UserInstall)
				{
					base.SetTextValue(this._wixUpdateText, "Text", value, true);
				}
			}
		}

		public VSFinishDialog(WiXProjectParser project) : base(project)
		{
		}

		public VSFinishDialog(WiXProjectParser project, VSUserInterface collection, AddinExpress.Installer.WiXDesigner.WiXDialog wixElement, AddinExpress.Installer.WiXDesigner.DialogType dialogType, AddinExpress.Installer.WiXDesigner.DialogStage dialogStage, AddinExpress.Installer.WiXDesigner.DialogScope dialogScope) : base(project, collection, wixElement, dialogType, dialogStage, dialogScope)
		{
		}

		public override void Delete()
		{
			base.Delete();
			foreach (WiXEntity wiXEntity in base.Project.SupportedEntities.FindAll((WiXEntity c) => {
				if (!(c is WiXShow))
				{
					return false;
				}
				return c.GetAttributeValue("Dialog") == base.WiXDialog.Id;
			}))
			{
				base.Project.SupportedEntities.Remove(wiXEntity);
				wiXEntity.Delete();
			}
		}

		protected override void InitializeDialog()
		{
			base.InitializeDialog();
			if (base.DialogScope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall)
			{
				AddinExpress.Installer.WiXDesigner.DesignTime.TypeDescriptor typeDescriptor = AddinExpress.Installer.WiXDesigner.DesignTime.TypeDescriptor.GetTypeDescriptor(this);
				if (typeDescriptor != null)
				{
					(typeDescriptor.GetProperties().Find("UpdateText", true) as AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor).Browsable = false;
				}
			}
			if (base.WiXDialog != null)
			{
				this._wixUpdateText = base.WiXDialog.ChildEntities.Find((WiXEntity c) => {
					if (!(c is WiXControl))
					{
						return false;
					}
					return c.GetAttributeValue("Id") == "UpdateText";
				});
				AddinExpress.Installer.WiXDesigner.DialogScope dialogScope = base.DialogScope;
			}
		}
	}
}