using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSCustomerInfoDialog : VSDialogBase
	{
		private WiXEntity _wixSerialEdit;

		private WiXEntity _wixOrganizationEdit;

		private WiXEntity _wixShowSerialProperty;

		private WiXEntity _wixShowOrgProperty;

		private WiXEntity _wixSerialEditTemplateProperty;

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
		[Description("The format used to validate the user's serial number.")]
		[MergableProperty(false)]
		public string SerialNumberTemplate
		{
			get
			{
				return base.GetTextValue(this._wixSerialEditTemplateProperty, "Value");
			}
			set
			{
				base.SetTextValue(this._wixSerialEditTemplateProperty, "Value", value, true);
			}
		}

		[Browsable(true)]
		[DefaultValue(true)]
		[Description("Determines whether this dialog displays a prompt for the customer's organization.")]
		[MergableProperty(false)]
		public bool ShowOrganization
		{
			get
			{
				if (base.GetTextValue(this._wixOrganizationEdit, "Hidden") != "yes")
				{
					return true;
				}
				return false;
			}
			set
			{
				if (value)
				{
					base.SetTextValue(this._wixOrganizationEdit, "Hidden", null);
					base.SetTextValue(this._wixShowOrgProperty, "Value", "1");
					return;
				}
				base.SetTextValue(this._wixOrganizationEdit, "Hidden", "yes");
				base.SetTextValue(this._wixShowOrgProperty, "Value", "0");
			}
		}

		[Browsable(true)]
		[DefaultValue(true)]
		[Description("Determines whether this dialog displays a prompt for the product's serial number.")]
		[MergableProperty(false)]
		public bool ShowSerialNumber
		{
			get
			{
				if (base.GetTextValue(this._wixSerialEdit, "Hidden") != "yes")
				{
					return true;
				}
				return false;
			}
			set
			{
				if (value)
				{
					base.SetTextValue(this._wixSerialEdit, "Hidden", null);
					base.SetTextValue(this._wixShowSerialProperty, "Value", "1");
					return;
				}
				base.SetTextValue(this._wixSerialEdit, "Hidden", "yes");
				base.SetTextValue(this._wixShowSerialProperty, "Value", "0");
			}
		}

		public VSCustomerInfoDialog(WiXProjectParser project) : base(project)
		{
		}

		public VSCustomerInfoDialog(WiXProjectParser project, VSUserInterface collection, AddinExpress.Installer.WiXDesigner.WiXDialog wixElement, AddinExpress.Installer.WiXDesigner.DialogType dialogType, AddinExpress.Installer.WiXDesigner.DialogStage dialogStage, AddinExpress.Installer.WiXDesigner.DialogScope dialogScope) : base(project, collection, wixElement, dialogType, dialogStage, dialogScope)
		{
		}

		public override void Delete()
		{
			if (this._wixShowSerialProperty != null)
			{
				base.Project.SupportedEntities.Remove(this._wixShowSerialProperty);
				this._wixShowSerialProperty.Delete();
			}
			if (this._wixShowOrgProperty != null)
			{
				base.Project.SupportedEntities.Remove(this._wixShowOrgProperty);
				this._wixShowOrgProperty.Delete();
			}
			if (this._wixSerialEditTemplateProperty != null)
			{
				base.Project.SupportedEntities.Remove(this._wixSerialEditTemplateProperty);
				this._wixSerialEditTemplateProperty.Delete();
			}
			base.Delete();
		}

		protected override void InitializeDialog()
		{
			base.InitializeDialog();
			if (base.WiXDialog != null)
			{
				this._wixSerialEdit = base.WiXDialog.ChildEntities.Find((WiXEntity c) => {
					if (!(c is WiXControl))
					{
						return false;
					}
					return c.GetAttributeValue("Id") == "SerialEdit";
				});
				this._wixOrganizationEdit = base.WiXDialog.ChildEntities.Find((WiXEntity c) => {
					if (!(c is WiXControl))
					{
						return false;
					}
					return c.GetAttributeValue("Id") == "OrganizationEdit";
				});
				this._wixShowSerialProperty = base.WiXDialog.Parent.ChildEntities.Find((WiXEntity c) => {
					if (!(c is WiXProperty))
					{
						return false;
					}
					return c.GetAttributeValue("Id") == "CustomerInfoForm_ShowSerial";
				});
				this._wixShowOrgProperty = base.WiXDialog.Parent.ChildEntities.Find((WiXEntity c) => {
					if (!(c is WiXProperty))
					{
						return false;
					}
					return c.GetAttributeValue("Id") == "CustomerInfoForm_ShowOrg";
				});
				this._wixSerialEditTemplateProperty = base.WiXDialog.Parent.ChildEntities.Find((WiXEntity c) => {
					if (!(c is WiXProperty))
					{
						return false;
					}
					return c.GetAttributeValue("Id") == "PIDTemplate";
				});
			}
		}
	}
}