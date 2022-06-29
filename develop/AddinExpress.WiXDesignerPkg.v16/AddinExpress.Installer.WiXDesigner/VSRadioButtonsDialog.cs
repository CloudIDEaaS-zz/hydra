using AddinExpress.Installer.WiXDesigner.DesignTime;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSRadioButtonsDialog : VSDialogBase
	{
		private WiXEntity _wixBannerText;

		private WiXEntity _wixBodyText;

		private WiXEntity _wixButton1;

		private WiXEntity _wixButton2;

		private WiXEntity _wixButton3;

		private WiXEntity _wixButton4;

		private WiXEntity _wixGroup;

		private WiXEntity _wixGroupControl;

		private WiXEntity _wixGroupProperty;

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
		[Description("Text to display in the banner area of this dialog.")]
		public string BannerText
		{
			get
			{
				return base.GetTextValue(this._wixBannerText, "Text");
			}
			set
			{
				base.SetTextValue(this._wixBannerText, "Text", value);
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Text to display in the body area of this dialog.")]
		public string BodyText
		{
			get
			{
				return base.GetTextValue(this._wixBodyText, "Text");
			}
			set
			{
				base.SetTextValue(this._wixBodyText, "Text", value);
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Label for the first button in the radio button group.")]
		public string Button1Label
		{
			get
			{
				return base.GetTextValue(this._wixButton1, "Text");
			}
			set
			{
				base.SetTextValue(this._wixButton1, "Text", value);
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Value assigned to the property when first button is checked.")]
		public string Button1Value
		{
			get
			{
				return base.GetTextValue(this._wixButton1, "Value");
			}
			set
			{
				base.SetTextValue(this._wixButton1, "Value", value);
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Label for the second button in the radio button group.")]
		public string Button2Label
		{
			get
			{
				return base.GetTextValue(this._wixButton2, "Text");
			}
			set
			{
				base.SetTextValue(this._wixButton2, "Text", value);
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Value assigned to the property when second button is checked.")]
		public string Button2Value
		{
			get
			{
				return base.GetTextValue(this._wixButton2, "Value");
			}
			set
			{
				base.SetTextValue(this._wixButton2, "Value", value);
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Label for the third button in the radio button group.")]
		public string Button3Label
		{
			get
			{
				return base.GetTextValue(this._wixButton3, "Text");
			}
			set
			{
				base.SetTextValue(this._wixButton3, "Text", value);
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Value assigned to the property when third button is checked.")]
		public string Button3Value
		{
			get
			{
				return base.GetTextValue(this._wixButton3, "Value");
			}
			set
			{
				base.SetTextValue(this._wixButton3, "Value", value);
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Label for the fourth button in the radio button group.")]
		public string Button4Label
		{
			get
			{
				return base.GetTextValue(this._wixButton4, "Text");
			}
			set
			{
				base.SetTextValue(this._wixButton4, "Text", value);
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Value assigned to the property when fourth button is checked.")]
		public string Button4Value
		{
			get
			{
				return base.GetTextValue(this._wixButton4, "Value");
			}
			set
			{
				base.SetTextValue(this._wixButton4, "Value", value);
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Property name associated with the radio button group.")]
		public string ButtonProperty
		{
			get
			{
				return base.GetTextValue(this._wixGroupControl, "Property");
			}
			set
			{
				base.SetTextValue(this._wixGroupControl, "Property", value);
				base.SetTextValue(this._wixGroup, "Property", value);
				base.SetTextValue(this._wixGroupProperty, "Id", value);
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Identifies which button is checked by default in the radio button group.")]
		public string DefaultValue
		{
			get
			{
				return base.GetTextValue(this._wixGroupProperty, "Value");
			}
			set
			{
				base.SetTextValue(this._wixGroupProperty, "Value", value);
			}
		}

		public VSRadioButtonsDialog(WiXProjectParser project) : base(project)
		{
		}

		public VSRadioButtonsDialog(WiXProjectParser project, VSUserInterface collection, AddinExpress.Installer.WiXDesigner.WiXDialog wixElement, AddinExpress.Installer.WiXDesigner.DialogType dialogType, AddinExpress.Installer.WiXDesigner.DialogStage dialogStage, AddinExpress.Installer.WiXDesigner.DialogScope dialogScope) : base(project, collection, wixElement, dialogType, dialogStage, dialogScope)
		{
		}

		public override void Delete()
		{
			if (this._wixGroupProperty != null)
			{
				base.Project.SupportedEntities.Remove(this._wixGroupProperty);
				this._wixGroupProperty.Delete();
			}
			if (this._wixGroup != null)
			{
				this._wixGroup.Delete();
			}
			base.Delete();
		}

		protected override void InitializeDialog()
		{
			base.InitializeDialog();
			if (base.DialogType != AddinExpress.Installer.WiXDesigner.DialogType.RadioButtons4)
			{
				AddinExpress.Installer.WiXDesigner.DesignTime.TypeDescriptor typeDescriptor = AddinExpress.Installer.WiXDesigner.DesignTime.TypeDescriptor.GetTypeDescriptor(this);
				if (typeDescriptor != null)
				{
					PropertyDescriptorCollection properties = typeDescriptor.GetProperties();
					if (base.DialogType == AddinExpress.Installer.WiXDesigner.DialogType.RadioButtons2)
					{
						(properties.Find("Button3Label", true) as AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor).Browsable = false;
						(properties.Find("Button3Value", true) as AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor).Browsable = false;
					}
					if (base.DialogType == AddinExpress.Installer.WiXDesigner.DialogType.RadioButtons2 || base.DialogType == AddinExpress.Installer.WiXDesigner.DialogType.RadioButtons3)
					{
						(properties.Find("Button4Label", true) as AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor).Browsable = false;
						(properties.Find("Button4Value", true) as AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor).Browsable = false;
					}
				}
			}
			if (base.WiXDialog != null)
			{
				this._wixBannerText = base.WiXDialog.ChildEntities.Find((WiXEntity c) => {
					if (!(c is WiXControl))
					{
						return false;
					}
					return c.GetAttributeValue("Id") == "BannerText";
				});
				this._wixBodyText = base.WiXDialog.ChildEntities.Find((WiXEntity c) => {
					if (!(c is WiXControl))
					{
						return false;
					}
					return c.GetAttributeValue("Id") == "BodyText";
				});
				this._wixGroupControl = base.WiXDialog.ChildEntities.Find((WiXEntity c) => {
					if (!(c is WiXControl))
					{
						return false;
					}
					return c.GetAttributeValue("Id") == "RadioButtonGroup";
				});
				if (this._wixGroupControl != null)
				{
					string attributeValue = this._wixGroupControl.GetAttributeValue("Property");
					if (!string.IsNullOrEmpty(attributeValue))
					{
						this._wixGroup = base.WiXDialog.Parent.ChildEntities.Find((WiXEntity c) => {
							if (!(c is WiXRadioButtonGroup))
							{
								return false;
							}
							return c.GetAttributeValue("Property") == attributeValue;
						});
						if (this._wixGroup != null && this._wixGroup.ChildEntities.Count > 0)
						{
							this._wixButton1 = this._wixGroup.ChildEntities[0];
							if (this._wixGroup.ChildEntities.Count > 1)
							{
								this._wixButton2 = this._wixGroup.ChildEntities[1];
							}
							if (this._wixGroup.ChildEntities.Count > 2)
							{
								this._wixButton3 = this._wixGroup.ChildEntities[2];
							}
							if (this._wixGroup.ChildEntities.Count > 3)
							{
								this._wixButton4 = this._wixGroup.ChildEntities[3];
							}
						}
					}
					this._wixGroupProperty = base.WiXDialog.Parent.ChildEntities.Find((WiXEntity c) => {
						if (!(c is WiXProperty))
						{
							return false;
						}
						return c.GetAttributeValue("Id") == attributeValue;
					});
				}
			}
		}
	}
}