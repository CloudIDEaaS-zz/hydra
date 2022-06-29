using AddinExpress.Installer.WiXDesigner.DesignTime;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSInstallFolderDialog : VSDialogBase
	{
		private WiXEntity _wixAllUsersProperty;

		private WiXEntity _wixAllUsersVisibleProperty;

		private WiXEntity _wixRadioButtonGroup;

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
		[Description("Determines whether the Everyone / Just Me controls are visible or hidden.")]
		[MergableProperty(false)]
		public bool InstallAllUsersVisible
		{
			get
			{
				if (base.DialogScope != AddinExpress.Installer.WiXDesigner.DialogScope.UserInstall)
				{
					return false;
				}
				if (base.GetTextValue(this._wixAllUsersVisibleProperty, "Value") != "1")
				{
					return false;
				}
				return true;
			}
			set
			{
				if (base.DialogScope == AddinExpress.Installer.WiXDesigner.DialogScope.UserInstall)
				{
					if (value)
					{
						base.SetTextValue(this._wixAllUsersVisibleProperty, "Value", "1");
						return;
					}
					base.SetTextValue(this._wixAllUsersVisibleProperty, "Value", "0");
				}
			}
		}

		public VSInstallFolderDialog(WiXProjectParser project) : base(project)
		{
		}

		public VSInstallFolderDialog(WiXProjectParser project, VSUserInterface collection, AddinExpress.Installer.WiXDesigner.WiXDialog wixElement, AddinExpress.Installer.WiXDesigner.DialogType dialogType, AddinExpress.Installer.WiXDesigner.DialogStage dialogStage, AddinExpress.Installer.WiXDesigner.DialogScope dialogScope) : base(project, collection, wixElement, dialogType, dialogStage, dialogScope)
		{
		}

		public override void Delete()
		{
			if (this._wixAllUsersProperty != null)
			{
				base.Project.SupportedEntities.Remove(this._wixAllUsersProperty);
				this._wixAllUsersProperty.Delete();
			}
			if (this._wixAllUsersVisibleProperty != null)
			{
				base.Project.SupportedEntities.Remove(this._wixAllUsersVisibleProperty);
				this._wixAllUsersVisibleProperty.Delete();
			}
			if (this._wixRadioButtonGroup != null)
			{
				this._wixRadioButtonGroup.Delete();
			}
			base.Delete();
		}

		protected override void InitializeDialog()
		{
			base.InitializeDialog();
			if (base.DialogScope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall)
			{
				AddinExpress.Installer.WiXDesigner.DesignTime.TypeDescriptor typeDescriptor = AddinExpress.Installer.WiXDesigner.DesignTime.TypeDescriptor.GetTypeDescriptor(this);
				if (typeDescriptor != null)
				{
					(typeDescriptor.GetProperties().Find("InstallAllUsersVisible", true) as AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor).Browsable = false;
				}
			}
			if (base.WiXDialog != null && base.DialogScope == AddinExpress.Installer.WiXDesigner.DialogScope.UserInstall)
			{
				this._wixAllUsersProperty = base.WiXDialog.Parent.ChildEntities.Find((WiXEntity c) => {
					if (!(c is WiXProperty))
					{
						return false;
					}
					return c.GetAttributeValue("Id") == "FolderForm_AllUsers";
				});
				this._wixAllUsersVisibleProperty = base.WiXDialog.Parent.ChildEntities.Find((WiXEntity c) => {
					if (!(c is WiXProperty))
					{
						return false;
					}
					return c.GetAttributeValue("Id") == "FolderForm_AllUsersVisible";
				});
				this._wixRadioButtonGroup = base.WiXDialog.Parent.ChildEntities.Find((WiXEntity c) => {
					if (!(c is WiXRadioButtonGroup))
					{
						return false;
					}
					return c.GetAttributeValue("Property") == "FolderForm_AllUsers";
				});
			}
		}
	}
}