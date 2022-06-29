using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSTextboxesDialog : VSDialogBase
	{
		private string @alias = string.Empty;

		private WiXEntity _wixBannerText;

		private WiXEntity _wixBodyText;

		private WiXEntity _wixTextbox1;

		private WiXEntity _wixTextbox2;

		private WiXEntity _wixTextbox3;

		private WiXEntity _wixTextbox4;

		private WiXEntity _wixTextboxLabel1;

		private WiXEntity _wixTextboxLabel2;

		private WiXEntity _wixTextboxLabel3;

		private WiXEntity _wixTextboxLabel4;

		private WiXEntity _wixTextbox1CA;

		private WiXEntity _wixTextbox2CA;

		private WiXEntity _wixTextbox3CA;

		private WiXEntity _wixTextbox4CA;

		private WiXEntity _wixTextbox1Property;

		private WiXEntity _wixTextbox2Property;

		private WiXEntity _wixTextbox3Property;

		private WiXEntity _wixTextbox4Property;

		private string text1PropName = string.Empty;

		private string text2PropName = string.Empty;

		private string text3PropName = string.Empty;

		private string text4PropName = string.Empty;

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
		[Description("Label for the first edit field.")]
		public string Edit1Label
		{
			get
			{
				return base.GetTextValue(this._wixTextboxLabel1, "Text");
			}
			set
			{
				base.SetTextValue(this._wixTextboxLabel1, "Text", value);
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Property name associated with the first edit field.")]
		public string Edit1Property
		{
			get
			{
				return base.GetTextValue(this._wixTextbox1, "Property");
			}
			set
			{
				base.SetTextValue(this._wixTextbox1, "Property", value);
				base.SetTextValue(this._wixTextbox1Property, "Id", value);
				base.SetTextValue(this._wixTextbox1CA, "Property", value);
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Default contents of the first edit field.")]
		[MergableProperty(true)]
		public string Edit1Value
		{
			get
			{
				return base.GetTextValue(this._wixTextbox1CA, "Value");
			}
			set
			{
				base.SetTextValue(this._wixTextbox1CA, "Value", value, true);
			}
		}

		[Browsable(true)]
		[DefaultValue(false)]
		[Description("Determines whether the first edit field is visible or hidden.")]
		[MergableProperty(true)]
		public bool Edit1Visible
		{
			get
			{
				if (base.GetTextValue(this._wixTextbox1, "Hidden") != "yes")
				{
					return true;
				}
				return false;
			}
			set
			{
				if (value)
				{
					base.SetTextValue(this._wixTextbox1, "Hidden", null);
					base.SetTextValue(this._wixTextboxLabel1, "Hidden", null);
					return;
				}
				base.SetTextValue(this._wixTextbox1, "Hidden", "yes");
				base.SetTextValue(this._wixTextboxLabel1, "Hidden", "yes");
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Label for the second edit field.")]
		public string Edit2Label
		{
			get
			{
				return base.GetTextValue(this._wixTextboxLabel2, "Text");
			}
			set
			{
				base.SetTextValue(this._wixTextboxLabel2, "Text", value);
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Property name associated with the second edit field.")]
		public string Edit2Property
		{
			get
			{
				return base.GetTextValue(this._wixTextbox2, "Property");
			}
			set
			{
				base.SetTextValue(this._wixTextbox2, "Property", value);
				base.SetTextValue(this._wixTextbox2Property, "Id", value);
				base.SetTextValue(this._wixTextbox2CA, "Property", value);
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Default contents of the second edit field.")]
		[MergableProperty(true)]
		public string Edit2Value
		{
			get
			{
				return base.GetTextValue(this._wixTextbox2CA, "Value");
			}
			set
			{
				base.SetTextValue(this._wixTextbox2CA, "Value", value, true);
			}
		}

		[Browsable(true)]
		[DefaultValue(false)]
		[Description("Determines whether the second edit field is visible or hidden.")]
		[MergableProperty(true)]
		public bool Edit2Visible
		{
			get
			{
				if (base.GetTextValue(this._wixTextbox2, "Hidden") != "yes")
				{
					return true;
				}
				return false;
			}
			set
			{
				if (value)
				{
					base.SetTextValue(this._wixTextbox2, "Hidden", null);
					base.SetTextValue(this._wixTextboxLabel2, "Hidden", null);
					return;
				}
				base.SetTextValue(this._wixTextbox2, "Hidden", "yes");
				base.SetTextValue(this._wixTextboxLabel2, "Hidden", "yes");
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Label for the third edit field.")]
		public string Edit3Label
		{
			get
			{
				return base.GetTextValue(this._wixTextboxLabel3, "Text");
			}
			set
			{
				base.SetTextValue(this._wixTextboxLabel3, "Text", value);
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Property name associated with the third edit field.")]
		public string Edit3Property
		{
			get
			{
				return base.GetTextValue(this._wixTextbox3, "Property");
			}
			set
			{
				base.SetTextValue(this._wixTextbox3, "Property", value);
				base.SetTextValue(this._wixTextbox3Property, "Id", value);
				base.SetTextValue(this._wixTextbox3CA, "Property", value);
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Default contents of the third edit field.")]
		[MergableProperty(true)]
		public string Edit3Value
		{
			get
			{
				return base.GetTextValue(this._wixTextbox3CA, "Value");
			}
			set
			{
				base.SetTextValue(this._wixTextbox3CA, "Value", value, true);
			}
		}

		[Browsable(true)]
		[DefaultValue(false)]
		[Description("Determines whether the third edit field is visible or hidden.")]
		[MergableProperty(true)]
		public bool Edit3Visible
		{
			get
			{
				if (base.GetTextValue(this._wixTextbox3, "Hidden") != "yes")
				{
					return true;
				}
				return false;
			}
			set
			{
				if (value)
				{
					base.SetTextValue(this._wixTextbox3, "Hidden", null);
					base.SetTextValue(this._wixTextboxLabel3, "Hidden", null);
					return;
				}
				base.SetTextValue(this._wixTextbox3, "Hidden", "yes");
				base.SetTextValue(this._wixTextboxLabel3, "Hidden", "yes");
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Label for the fourth edit field.")]
		public string Edit4Label
		{
			get
			{
				return base.GetTextValue(this._wixTextboxLabel4, "Text");
			}
			set
			{
				base.SetTextValue(this._wixTextboxLabel4, "Text", value);
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Property name associated with the fourth edit field.")]
		public string Edit4Property
		{
			get
			{
				return base.GetTextValue(this._wixTextbox4, "Property");
			}
			set
			{
				base.SetTextValue(this._wixTextbox4, "Property", value);
				base.SetTextValue(this._wixTextbox4Property, "Id", value);
				base.SetTextValue(this._wixTextbox4CA, "Property", value);
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Default contents of the fourth edit field.")]
		[MergableProperty(true)]
		public string Edit4Value
		{
			get
			{
				return base.GetTextValue(this._wixTextbox4CA, "Value");
			}
			set
			{
				base.SetTextValue(this._wixTextbox4CA, "Value", value, true);
			}
		}

		[Browsable(true)]
		[DefaultValue(false)]
		[Description("Determines whether the fourth edit field is visible or hidden.")]
		[MergableProperty(true)]
		public bool Edit4Visible
		{
			get
			{
				if (base.GetTextValue(this._wixTextbox4, "Hidden") != "yes")
				{
					return true;
				}
				return false;
			}
			set
			{
				if (value)
				{
					base.SetTextValue(this._wixTextbox4, "Hidden", null);
					base.SetTextValue(this._wixTextboxLabel4, "Hidden", null);
					return;
				}
				base.SetTextValue(this._wixTextbox4, "Hidden", "yes");
				base.SetTextValue(this._wixTextboxLabel4, "Hidden", "yes");
			}
		}

		public VSTextboxesDialog(WiXProjectParser project) : base(project)
		{
		}

		public VSTextboxesDialog(WiXProjectParser project, VSUserInterface collection, AddinExpress.Installer.WiXDesigner.WiXDialog wixElement, AddinExpress.Installer.WiXDesigner.DialogType dialogType, AddinExpress.Installer.WiXDesigner.DialogStage dialogStage, AddinExpress.Installer.WiXDesigner.DialogScope dialogScope) : base(project, collection, wixElement, dialogType, dialogStage, dialogScope)
		{
		}

		public override void Delete()
		{
			if (this._wixTextbox1Property != null)
			{
				base.Project.SupportedEntities.Remove(this._wixTextbox1Property);
				this._wixTextbox1Property.Delete();
			}
			if (this._wixTextbox2Property != null)
			{
				base.Project.SupportedEntities.Remove(this._wixTextbox2Property);
				this._wixTextbox2Property.Delete();
			}
			if (this._wixTextbox3Property != null)
			{
				base.Project.SupportedEntities.Remove(this._wixTextbox3Property);
				this._wixTextbox3Property.Delete();
			}
			if (this._wixTextbox4Property != null)
			{
				base.Project.SupportedEntities.Remove(this._wixTextbox4Property);
				this._wixTextbox4Property.Delete();
			}
			if (this._wixTextbox1CA != null)
			{
				base.Project.SupportedEntities.Remove(this._wixTextbox1CA);
				this._wixTextbox1CA.Delete();
			}
			if (this._wixTextbox2CA != null)
			{
				base.Project.SupportedEntities.Remove(this._wixTextbox2CA);
				this._wixTextbox2CA.Delete();
			}
			if (this._wixTextbox3CA != null)
			{
				base.Project.SupportedEntities.Remove(this._wixTextbox3CA);
				this._wixTextbox3CA.Delete();
			}
			if (this._wixTextbox4CA != null)
			{
				base.Project.SupportedEntities.Remove(this._wixTextbox4CA);
				this._wixTextbox4CA.Delete();
			}
			if (!string.IsNullOrEmpty(this.text1PropName))
			{
				foreach (WiXEntity wiXEntity in base.Project.SupportedEntities.FindAll((WiXEntity c) => {
					if (!(c is WiXCustom))
					{
						return false;
					}
					return c.GetAttributeValue("Action") == this.text1PropName;
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
			if (!string.IsNullOrEmpty(this.text2PropName))
			{
				foreach (WiXEntity wiXEntity1 in base.Project.SupportedEntities.FindAll((WiXEntity c) => {
					if (!(c is WiXCustom))
					{
						return false;
					}
					return c.GetAttributeValue("Action") == this.text2PropName;
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
			if (!string.IsNullOrEmpty(this.text3PropName))
			{
				foreach (WiXEntity wiXEntity2 in base.Project.SupportedEntities.FindAll((WiXEntity c) => {
					if (!(c is WiXCustom))
					{
						return false;
					}
					return c.GetAttributeValue("Action") == this.text3PropName;
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
			if (!string.IsNullOrEmpty(this.text4PropName))
			{
				foreach (WiXEntity wiXEntity3 in base.Project.SupportedEntities.FindAll((WiXEntity c) => {
					if (!(c is WiXCustom))
					{
						return false;
					}
					return c.GetAttributeValue("Action") == this.text4PropName;
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
				case AddinExpress.Installer.WiXDesigner.DialogType.TextBoxesA:
				{
					this.@alias = "CustomTextA";
					break;
				}
				case AddinExpress.Installer.WiXDesigner.DialogType.TextBoxesB:
				{
					this.@alias = "CustomTextB";
					break;
				}
				case AddinExpress.Installer.WiXDesigner.DialogType.TextBoxesC:
				{
					this.@alias = "CustomTextC";
					break;
				}
			}
			if (base.WiXDialog != null)
			{
				this.text1PropName = string.Concat(this.@alias, "_SetProperty_EDIT1");
				this.text2PropName = string.Concat(this.@alias, "_SetProperty_EDIT2");
				this.text3PropName = string.Concat(this.@alias, "_SetProperty_EDIT3");
				this.text4PropName = string.Concat(this.@alias, "_SetProperty_EDIT4");
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
				this._wixTextbox1 = base.WiXDialog.ChildEntities.Find((WiXEntity c) => {
					if (!(c is WiXControl))
					{
						return false;
					}
					return c.GetAttributeValue("Id") == "Edit1";
				});
				this._wixTextboxLabel1 = base.WiXDialog.ChildEntities.Find((WiXEntity c) => {
					if (!(c is WiXControl))
					{
						return false;
					}
					return c.GetAttributeValue("Id") == "Label1";
				});
				this._wixTextbox1CA = base.Project.SupportedEntities.Find((WiXEntity c) => {
					if (!(c is WiXCustomAction))
					{
						return false;
					}
					return c.GetAttributeValue("Id") == this.text1PropName;
				});
				if (this._wixTextbox1 != null)
				{
					string attributeValue = this._wixTextbox1.GetAttributeValue("Property");
					if (!string.IsNullOrEmpty(attributeValue))
					{
						this._wixTextbox1Property = base.WiXDialog.Parent.ChildEntities.Find((WiXEntity c) => {
							if (!(c is WiXProperty))
							{
								return false;
							}
							return c.GetAttributeValue("Id") == attributeValue;
						});
					}
				}
				this._wixTextbox2 = base.WiXDialog.ChildEntities.Find((WiXEntity c) => {
					if (!(c is WiXControl))
					{
						return false;
					}
					return c.GetAttributeValue("Id") == "Edit2";
				});
				this._wixTextboxLabel2 = base.WiXDialog.ChildEntities.Find((WiXEntity c) => {
					if (!(c is WiXControl))
					{
						return false;
					}
					return c.GetAttributeValue("Id") == "Label2";
				});
				this._wixTextbox2CA = base.Project.SupportedEntities.Find((WiXEntity c) => {
					if (!(c is WiXCustomAction))
					{
						return false;
					}
					return c.GetAttributeValue("Id") == this.text2PropName;
				});
				if (this._wixTextbox2 != null)
				{
					string str = this._wixTextbox2.GetAttributeValue("Property");
					if (!string.IsNullOrEmpty(str))
					{
						this._wixTextbox2Property = base.WiXDialog.Parent.ChildEntities.Find((WiXEntity c) => {
							if (!(c is WiXProperty))
							{
								return false;
							}
							return c.GetAttributeValue("Id") == str;
						});
					}
				}
				this._wixTextbox3 = base.WiXDialog.ChildEntities.Find((WiXEntity c) => {
					if (!(c is WiXControl))
					{
						return false;
					}
					return c.GetAttributeValue("Id") == "Edit3";
				});
				this._wixTextboxLabel3 = base.WiXDialog.ChildEntities.Find((WiXEntity c) => {
					if (!(c is WiXControl))
					{
						return false;
					}
					return c.GetAttributeValue("Id") == "Label3";
				});
				this._wixTextbox3CA = base.Project.SupportedEntities.Find((WiXEntity c) => {
					if (!(c is WiXCustomAction))
					{
						return false;
					}
					return c.GetAttributeValue("Id") == this.text3PropName;
				});
				if (this._wixTextbox3 != null)
				{
					string attributeValue1 = this._wixTextbox3.GetAttributeValue("Property");
					if (!string.IsNullOrEmpty(attributeValue1))
					{
						this._wixTextbox3Property = base.WiXDialog.Parent.ChildEntities.Find((WiXEntity c) => {
							if (!(c is WiXProperty))
							{
								return false;
							}
							return c.GetAttributeValue("Id") == attributeValue1;
						});
					}
				}
				this._wixTextbox4 = base.WiXDialog.ChildEntities.Find((WiXEntity c) => {
					if (!(c is WiXControl))
					{
						return false;
					}
					return c.GetAttributeValue("Id") == "Edit4";
				});
				this._wixTextboxLabel4 = base.WiXDialog.ChildEntities.Find((WiXEntity c) => {
					if (!(c is WiXControl))
					{
						return false;
					}
					return c.GetAttributeValue("Id") == "Label4";
				});
				this._wixTextbox4CA = base.Project.SupportedEntities.Find((WiXEntity c) => {
					if (!(c is WiXCustomAction))
					{
						return false;
					}
					return c.GetAttributeValue("Id") == this.text4PropName;
				});
				if (this._wixTextbox4 != null)
				{
					string str1 = this._wixTextbox4.GetAttributeValue("Property");
					if (!string.IsNullOrEmpty(str1))
					{
						this._wixTextbox4Property = base.WiXDialog.Parent.ChildEntities.Find((WiXEntity c) => {
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