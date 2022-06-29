using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSProgressDialog : VSDialogBase
	{
		private WiXEntity _wixProgressBar;

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
		[DefaultValue(true)]
		[Description("Determines whether the progress bar is visible or hidden.")]
		[MergableProperty(false)]
		public bool ShowProgressBar
		{
			get
			{
				if (base.GetTextValue(this._wixProgressBar, "Hidden") != "yes")
				{
					return true;
				}
				return false;
			}
			set
			{
				if (value)
				{
					base.SetTextValue(this._wixProgressBar, "Hidden", null);
					return;
				}
				base.SetTextValue(this._wixProgressBar, "Hidden", "yes");
			}
		}

		public VSProgressDialog(WiXProjectParser project) : base(project)
		{
		}

		public VSProgressDialog(WiXProjectParser project, VSUserInterface collection, AddinExpress.Installer.WiXDesigner.WiXDialog wixElement, AddinExpress.Installer.WiXDesigner.DialogType dialogType, AddinExpress.Installer.WiXDesigner.DialogStage dialogStage, AddinExpress.Installer.WiXDesigner.DialogScope dialogScope) : base(project, collection, wixElement, dialogType, dialogStage, dialogScope)
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
				this._wixProgressBar = base.WiXDialog.ChildEntities.Find((WiXEntity c) => {
					if (!(c is WiXControl))
					{
						return false;
					}
					return c.GetAttributeValue("Id") == "ProgressBar";
				});
			}
		}
	}
}