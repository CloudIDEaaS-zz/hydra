using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSSplashDialog : VSDialogBase
	{
		[Browsable(true)]
		[DefaultValue(null)]
		[Description("The bitmap or JPEG file containing the image for display in this dialog.")]
		[Editor(typeof(SplashBitmapEditor), typeof(UITypeEditor))]
		[MergableProperty(false)]
		[TypeConverter(typeof(SplashBitmapConverter))]
		public VSBaseFile SplashBitmap
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
		[DefaultValue(true)]
		[Description("Determines whether the sunken border is visible or hidden.")]
		[MergableProperty(false)]
		public bool Sunken
		{
			get
			{
				if (base.GetTextValue(this._wixBannerBitmap, "Sunken") != "yes")
				{
					return false;
				}
				return true;
			}
			set
			{
				if (value)
				{
					base.SetTextValue(this._wixBannerBitmap, "Sunken", "yes");
					return;
				}
				base.SetTextValue(this._wixBannerBitmap, "Sunken", "no");
			}
		}

		public VSSplashDialog(WiXProjectParser project) : base(project)
		{
		}

		public VSSplashDialog(WiXProjectParser project, VSUserInterface collection, AddinExpress.Installer.WiXDesigner.WiXDialog wixElement, AddinExpress.Installer.WiXDesigner.DialogType dialogType, AddinExpress.Installer.WiXDesigner.DialogStage dialogStage, AddinExpress.Installer.WiXDesigner.DialogScope dialogScope) : base(project, collection, wixElement, dialogType, dialogStage, dialogScope)
		{
		}

		public override void Delete()
		{
			base.Delete();
		}

		protected override void InitializeDialog()
		{
			base.InitializeDialog();
			if (base.WiXDialog != null)
			{
				this._wixBannerBitmap = base.WiXDialog.ChildEntities.Find((WiXEntity c) => {
					if (!(c is WiXControl))
					{
						return false;
					}
					return c.GetAttributeValue("Id") == "SplashBmp";
				}) as WiXControl;
			}
		}
	}
}