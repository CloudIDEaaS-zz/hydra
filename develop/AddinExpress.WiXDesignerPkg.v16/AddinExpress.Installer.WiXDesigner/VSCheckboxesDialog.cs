using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSCheckboxesDialog : VSDialogBase
	{
		private string @alias = string.Empty;

		private WiXEntity _wixBannerText;

		private WiXEntity _wixBodyText;

		private WiXEntity _wixCheckbox1;

		private WiXEntity _wixCheckbox2;

		private WiXEntity _wixCheckbox3;

		private WiXEntity _wixCheckbox4;

		private WiXEntity _wixCheckbox1CA;

		private WiXEntity _wixCheckbox2CA;

		private WiXEntity _wixCheckbox3CA;

		private WiXEntity _wixCheckbox4CA;

		private WiXEntity _wixCheckbox1Property;

		private WiXEntity _wixCheckbox2Property;

		private WiXEntity _wixCheckbox3Property;

		private WiXEntity _wixCheckbox4Property;

		private string check1PropName = string.Empty;

		private string check2PropName = string.Empty;

		private string check3PropName = string.Empty;

		private string check4PropName = string.Empty;

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
		[Description("Label for the first checkbox.")]
		public string Checkbox1Label
		{
			get
			{
				return base.GetTextValue(this._wixCheckbox1, "Text");
			}
			set
			{
				base.SetTextValue(this._wixCheckbox1, "Text", value);
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Property name associated with the first checkbox.")]
		public string Checkbox1Property
		{
			get
			{
				return base.GetTextValue(this._wixCheckbox1, "Property");
			}
			set
			{
				base.SetTextValue(this._wixCheckbox1, "Property", value);
				base.SetTextValue(this._wixCheckbox1Property, "Id", value);
				base.SetTextValue(this._wixCheckbox1CA, "Property", value);
			}
		}

		[Browsable(true)]
		[DefaultValue(false)]
		[Description("Determines whether the first checkbox is initially checked or unchecked.")]
		[Editor(typeof(CheckboxValueEditor), typeof(UITypeEditor))]
		[MergableProperty(true)]
		[TypeConverter(typeof(CheckboxValueConverter))]
		public bool Checkbox1Value
		{
			get
			{
				if (base.GetTextValue(this._wixCheckbox1CA, "Value") != "1")
				{
					return false;
				}
				return true;
			}
			set
			{
				if (value)
				{
					base.SetTextValue(this._wixCheckbox1CA, "Value", "1");
					return;
				}
				base.SetTextValue(this._wixCheckbox1CA, "Value", "", true);
			}
		}

		[Browsable(true)]
		[DefaultValue(false)]
		[Description("Determines whether the first checkbox is visible or hidden.")]
		[MergableProperty(true)]
		public bool Checkbox1Visible
		{
			get
			{
				if (base.GetTextValue(this._wixCheckbox1, "Hidden") != "yes")
				{
					return true;
				}
				return false;
			}
			set
			{
				if (value)
				{
					base.SetTextValue(this._wixCheckbox1, "Hidden", null);
					return;
				}
				base.SetTextValue(this._wixCheckbox1, "Hidden", "yes");
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Label for the second checkbox.")]
		public string Checkbox2Label
		{
			get
			{
				return base.GetTextValue(this._wixCheckbox2, "Text");
			}
			set
			{
				base.SetTextValue(this._wixCheckbox2, "Text", value);
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Property name associated with the second checkbox.")]
		public string Checkbox2Property
		{
			get
			{
				return base.GetTextValue(this._wixCheckbox2, "Property");
			}
			set
			{
				base.SetTextValue(this._wixCheckbox2, "Property", value);
				base.SetTextValue(this._wixCheckbox2Property, "Id", value);
				base.SetTextValue(this._wixCheckbox2CA, "Property", value);
			}
		}

		[Browsable(true)]
		[DefaultValue(false)]
		[Description("Determines whether the second checkbox is initially checked or unchecked.")]
		[Editor(typeof(CheckboxValueEditor), typeof(UITypeEditor))]
		[MergableProperty(true)]
		[TypeConverter(typeof(CheckboxValueConverter))]
		public bool Checkbox2Value
		{
			get
			{
				if (base.GetTextValue(this._wixCheckbox2CA, "Value") != "1")
				{
					return false;
				}
				return true;
			}
			set
			{
				if (value)
				{
					base.SetTextValue(this._wixCheckbox2CA, "Value", "1");
					return;
				}
				base.SetTextValue(this._wixCheckbox2CA, "Value", "", true);
			}
		}

		[Browsable(true)]
		[DefaultValue(false)]
		[Description("Determines whether the second checkbox is visible or hidden.")]
		[MergableProperty(true)]
		public bool Checkbox2Visible
		{
			get
			{
				if (base.GetTextValue(this._wixCheckbox2, "Hidden") != "yes")
				{
					return true;
				}
				return false;
			}
			set
			{
				if (value)
				{
					base.SetTextValue(this._wixCheckbox2, "Hidden", null);
					return;
				}
				base.SetTextValue(this._wixCheckbox2, "Hidden", "yes");
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Label for the third checkbox.")]
		public string Checkbox3Label
		{
			get
			{
				return base.GetTextValue(this._wixCheckbox3, "Text");
			}
			set
			{
				base.SetTextValue(this._wixCheckbox3, "Text", value);
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Property name associated with the third checkbox.")]
		public string Checkbox3Property
		{
			get
			{
				return base.GetTextValue(this._wixCheckbox3, "Property");
			}
			set
			{
				base.SetTextValue(this._wixCheckbox3, "Property", value);
				base.SetTextValue(this._wixCheckbox3Property, "Id", value);
				base.SetTextValue(this._wixCheckbox3CA, "Property", value);
			}
		}

		[Browsable(true)]
		[DefaultValue(false)]
		[Description("Determines whether the third checkbox is initially checked or unchecked.")]
		[Editor(typeof(CheckboxValueEditor), typeof(UITypeEditor))]
		[MergableProperty(true)]
		[TypeConverter(typeof(CheckboxValueConverter))]
		public bool Checkbox3Value
		{
			get
			{
				if (base.GetTextValue(this._wixCheckbox3CA, "Value") != "1")
				{
					return false;
				}
				return true;
			}
			set
			{
				if (value)
				{
					base.SetTextValue(this._wixCheckbox3CA, "Value", "1");
					return;
				}
				base.SetTextValue(this._wixCheckbox3CA, "Value", "", true);
			}
		}

		[Browsable(true)]
		[DefaultValue(false)]
		[Description("Determines whether the third checkbox is visible or hidden.")]
		[MergableProperty(true)]
		public bool Checkbox3Visible
		{
			get
			{
				if (base.GetTextValue(this._wixCheckbox3, "Hidden") != "yes")
				{
					return true;
				}
				return false;
			}
			set
			{
				if (value)
				{
					base.SetTextValue(this._wixCheckbox3, "Hidden", null);
					return;
				}
				base.SetTextValue(this._wixCheckbox3, "Hidden", "yes");
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Label for the fourth checkbox.")]
		public string Checkbox4Label
		{
			get
			{
				return base.GetTextValue(this._wixCheckbox4, "Text");
			}
			set
			{
				base.SetTextValue(this._wixCheckbox4, "Text", value);
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Property name associated with the fourth checkbox.")]
		public string Checkbox4Property
		{
			get
			{
				return base.GetTextValue(this._wixCheckbox4, "Property");
			}
			set
			{
				base.SetTextValue(this._wixCheckbox4, "Property", value);
				base.SetTextValue(this._wixCheckbox4Property, "Id", value);
				base.SetTextValue(this._wixCheckbox4CA, "Property", value);
			}
		}

		[Browsable(true)]
		[DefaultValue(false)]
		[Description("Determines whether the fourth checkbox is initially checked or unchecked.")]
		[Editor(typeof(CheckboxValueEditor), typeof(UITypeEditor))]
		[MergableProperty(true)]
		[TypeConverter(typeof(CheckboxValueConverter))]
		public bool Checkbox4Value
		{
			get
			{
				if (base.GetTextValue(this._wixCheckbox4CA, "Value") != "1")
				{
					return false;
				}
				return true;
			}
			set
			{
				if (value)
				{
					base.SetTextValue(this._wixCheckbox4CA, "Value", "1");
					return;
				}
				base.SetTextValue(this._wixCheckbox4CA, "Value", "", true);
			}
		}

		[Browsable(true)]
		[DefaultValue(false)]
		[Description("Determines whether the fourth checkbox is visible or hidden.")]
		[MergableProperty(true)]
		public bool Checkbox4Visible
		{
			get
			{
				if (base.GetTextValue(this._wixCheckbox4, "Hidden") != "yes")
				{
					return true;
				}
				return false;
			}
			set
			{
				if (value)
				{
					base.SetTextValue(this._wixCheckbox4, "Hidden", null);
					return;
				}
				base.SetTextValue(this._wixCheckbox4, "Hidden", "yes");
			}
		}

		public VSCheckboxesDialog(WiXProjectParser project) : base(project)
		{
		}

		public VSCheckboxesDialog(WiXProjectParser project, VSUserInterface collection, AddinExpress.Installer.WiXDesigner.WiXDialog wixElement, AddinExpress.Installer.WiXDesigner.DialogType dialogType, AddinExpress.Installer.WiXDesigner.DialogStage dialogStage, AddinExpress.Installer.WiXDesigner.DialogScope dialogScope) : base(project, collection, wixElement, dialogType, dialogStage, dialogScope)
		{
		}

		public override void Delete()
		{
			if (this._wixCheckbox1Property != null)
			{
				base.Project.SupportedEntities.Remove(this._wixCheckbox1Property);
				this._wixCheckbox1Property.Delete();
			}
			if (this._wixCheckbox2Property != null)
			{
				base.Project.SupportedEntities.Remove(this._wixCheckbox2Property);
				this._wixCheckbox2Property.Delete();
			}
			if (this._wixCheckbox3Property != null)
			{
				base.Project.SupportedEntities.Remove(this._wixCheckbox3Property);
				this._wixCheckbox3Property.Delete();
			}
			if (this._wixCheckbox4Property != null)
			{
				base.Project.SupportedEntities.Remove(this._wixCheckbox4Property);
				this._wixCheckbox4Property.Delete();
			}
			if (this._wixCheckbox1CA != null)
			{
				base.Project.SupportedEntities.Remove(this._wixCheckbox1CA);
				this._wixCheckbox1CA.Delete();
			}
			if (this._wixCheckbox2CA != null)
			{
				base.Project.SupportedEntities.Remove(this._wixCheckbox2CA);
				this._wixCheckbox2CA.Delete();
			}
			if (this._wixCheckbox3CA != null)
			{
				base.Project.SupportedEntities.Remove(this._wixCheckbox3CA);
				this._wixCheckbox3CA.Delete();
			}
			if (this._wixCheckbox4CA != null)
			{
				base.Project.SupportedEntities.Remove(this._wixCheckbox4CA);
				this._wixCheckbox4CA.Delete();
			}
			if (!string.IsNullOrEmpty(this.check1PropName))
			{
				foreach (WiXEntity wiXEntity in base.Project.SupportedEntities.FindAll((WiXEntity c) => {
					if (!(c is WiXCustom))
					{
						return false;
					}
					return c.GetAttributeValue("Action") == this.check1PropName;
				}))
				{
					WiXEntity parent = wiXEntity.Parent as WiXEntity;
					base.Project.SupportedEntities.Remove(wiXEntity);
					wiXEntity.Delete();
					if (parent == null)
					{
						continue;
					}
					base.Project.SupportedEntities.Remove(parent);
					parent.Delete();
				}
			}
			if (!string.IsNullOrEmpty(this.check2PropName))
			{
				foreach (WiXEntity wiXEntity1 in base.Project.SupportedEntities.FindAll((WiXEntity c) => {
					if (!(c is WiXCustom))
					{
						return false;
					}
					return c.GetAttributeValue("Action") == this.check2PropName;
				}))
				{
					WiXEntity parent1 = wiXEntity1.Parent as WiXEntity;
					base.Project.SupportedEntities.Remove(wiXEntity1);
					wiXEntity1.Delete();
					if (parent1 == null)
					{
						continue;
					}
					base.Project.SupportedEntities.Remove(parent1);
					parent1.Delete();
				}
			}
			if (!string.IsNullOrEmpty(this.check3PropName))
			{
				foreach (WiXEntity wiXEntity2 in base.Project.SupportedEntities.FindAll((WiXEntity c) => {
					if (!(c is WiXCustom))
					{
						return false;
					}
					return c.GetAttributeValue("Action") == this.check3PropName;
				}))
				{
					WiXEntity parent2 = wiXEntity2.Parent as WiXEntity;
					base.Project.SupportedEntities.Remove(wiXEntity2);
					wiXEntity2.Delete();
					if (parent2 == null)
					{
						continue;
					}
					base.Project.SupportedEntities.Remove(parent2);
					parent2.Delete();
				}
			}
			if (!string.IsNullOrEmpty(this.check4PropName))
			{
				foreach (WiXEntity wiXEntity3 in base.Project.SupportedEntities.FindAll((WiXEntity c) => {
					if (!(c is WiXCustom))
					{
						return false;
					}
					return c.GetAttributeValue("Action") == this.check4PropName;
				}))
				{
					WiXEntity parent3 = wiXEntity3.Parent as WiXEntity;
					base.Project.SupportedEntities.Remove(wiXEntity3);
					wiXEntity3.Delete();
					if (parent3 == null)
					{
						continue;
					}
					base.Project.SupportedEntities.Remove(parent3);
					parent3.Delete();
				}
			}
			base.Delete();
		}

		protected override void InitializeDialog()
		{
			base.InitializeDialog();
			switch (base.DialogType)
			{
				case AddinExpress.Installer.WiXDesigner.DialogType.CheckBoxesA:
				{
					this.@alias = "CustomCheckA";
					break;
				}
				case AddinExpress.Installer.WiXDesigner.DialogType.CheckBoxesB:
				{
					this.@alias = "CustomCheckB";
					break;
				}
				case AddinExpress.Installer.WiXDesigner.DialogType.CheckBoxesC:
				{
					this.@alias = "CustomCheckC";
					break;
				}
			}
			if (base.WiXDialog != null)
			{
				this.check1PropName = string.Concat(this.@alias, "_SetProperty_CHECKBOX1");
				this.check2PropName = string.Concat(this.@alias, "_SetProperty_CHECKBOX2");
				this.check3PropName = string.Concat(this.@alias, "_SetProperty_CHECKBOX3");
				this.check4PropName = string.Concat(this.@alias, "_SetProperty_CHECKBOX4");
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
				this._wixCheckbox1 = base.WiXDialog.ChildEntities.Find((WiXEntity c) => {
					if (!(c is WiXControl))
					{
						return false;
					}
					return c.GetAttributeValue("Id") == "Checkbox1";
				});
				this._wixCheckbox1CA = base.Project.SupportedEntities.Find((WiXEntity c) => {
					if (!(c is WiXCustomAction))
					{
						return false;
					}
					return c.GetAttributeValue("Id") == this.check1PropName;
				});
				if (this._wixCheckbox1 != null)
				{
					string attributeValue = this._wixCheckbox1.GetAttributeValue("Property");
					if (!string.IsNullOrEmpty(attributeValue))
					{
						this._wixCheckbox1Property = base.WiXDialog.Parent.ChildEntities.Find((WiXEntity c) => {
							if (!(c is WiXProperty))
							{
								return false;
							}
							return c.GetAttributeValue("Id") == attributeValue;
						});
					}
				}
				this._wixCheckbox2 = base.WiXDialog.ChildEntities.Find((WiXEntity c) => {
					if (!(c is WiXControl))
					{
						return false;
					}
					return c.GetAttributeValue("Id") == "Checkbox2";
				});
				this._wixCheckbox2CA = base.Project.SupportedEntities.Find((WiXEntity c) => {
					if (!(c is WiXCustomAction))
					{
						return false;
					}
					return c.GetAttributeValue("Id") == this.check2PropName;
				});
				if (this._wixCheckbox2 != null)
				{
					string str = this._wixCheckbox2.GetAttributeValue("Property");
					if (!string.IsNullOrEmpty(str))
					{
						this._wixCheckbox2Property = base.WiXDialog.Parent.ChildEntities.Find((WiXEntity c) => {
							if (!(c is WiXProperty))
							{
								return false;
							}
							return c.GetAttributeValue("Id") == str;
						});
					}
				}
				this._wixCheckbox3 = base.WiXDialog.ChildEntities.Find((WiXEntity c) => {
					if (!(c is WiXControl))
					{
						return false;
					}
					return c.GetAttributeValue("Id") == "Checkbox3";
				});
				this._wixCheckbox3CA = base.Project.SupportedEntities.Find((WiXEntity c) => {
					if (!(c is WiXCustomAction))
					{
						return false;
					}
					return c.GetAttributeValue("Id") == this.check3PropName;
				});
				if (this._wixCheckbox3 != null)
				{
					string attributeValue1 = this._wixCheckbox3.GetAttributeValue("Property");
					if (!string.IsNullOrEmpty(attributeValue1))
					{
						this._wixCheckbox3Property = base.WiXDialog.Parent.ChildEntities.Find((WiXEntity c) => {
							if (!(c is WiXProperty))
							{
								return false;
							}
							return c.GetAttributeValue("Id") == attributeValue1;
						});
					}
				}
				this._wixCheckbox4 = base.WiXDialog.ChildEntities.Find((WiXEntity c) => {
					if (!(c is WiXControl))
					{
						return false;
					}
					return c.GetAttributeValue("Id") == "Checkbox4";
				});
				this._wixCheckbox4CA = base.Project.SupportedEntities.Find((WiXEntity c) => {
					if (!(c is WiXCustomAction))
					{
						return false;
					}
					return c.GetAttributeValue("Id") == this.check4PropName;
				});
				if (this._wixCheckbox4 != null)
				{
					string str1 = this._wixCheckbox4.GetAttributeValue("Property");
					if (!string.IsNullOrEmpty(str1))
					{
						this._wixCheckbox4Property = base.WiXDialog.Parent.ChildEntities.Find((WiXEntity c) => {
							if (!(c is WiXProperty))
							{
								return false;
							}
							return c.GetAttributeValue("Id") == str1;
						});
					}
				}
			}
		}
	}
}