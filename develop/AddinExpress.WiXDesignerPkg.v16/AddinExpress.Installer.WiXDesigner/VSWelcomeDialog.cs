using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSWelcomeDialog : VSDialogBase
	{
		private WiXEntity _wixCopyrightWarning;

		private WiXEntity _wixWelcome;

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
		[Description("The copyright warning text to display at the bottom of this dialog.")]
		public string CopyrightWarning
		{
			get
			{
				return base.GetTextValue(this._wixCopyrightWarning, "Text");
			}
			set
			{
				base.SetTextValue(this._wixCopyrightWarning, "Text", value);
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("The welcome text to display at the top of this dialog.")]
		public string WelcomeText
		{
			get
			{
				return base.GetTextValue(this._wixWelcome, "Text");
			}
			set
			{
				base.SetTextValue(this._wixWelcome, "Text", value);
			}
		}

		public VSWelcomeDialog(WiXProjectParser project) : base(project)
		{
		}

		public VSWelcomeDialog(WiXProjectParser project, VSUserInterface collection, AddinExpress.Installer.WiXDesigner.WiXDialog wixElement, AddinExpress.Installer.WiXDesigner.DialogType dialogType, AddinExpress.Installer.WiXDesigner.DialogStage dialogStage, AddinExpress.Installer.WiXDesigner.DialogScope dialogScope) : base(project, collection, wixElement, dialogType, dialogStage, dialogScope)
		{
		}

		public override void Delete()
		{
			base.Delete();
		}

		private string GetCopyrightWarningValue()
		{
			if (this._wixCopyrightWarning != null)
			{
				string attributeValue = this._wixCopyrightWarning.GetAttributeValue("Text");
				if (!string.IsNullOrEmpty(attributeValue))
				{
					attributeValue = attributeValue.Trim();
					if (attributeValue.StartsWith("{\\"))
					{
						int num = attributeValue.IndexOf("}");
						if (num != -1)
						{
							attributeValue = attributeValue.Substring(num + 1);
						}
					}
					return attributeValue;
				}
			}
			return string.Empty;
		}

		private string GetWelcomeValue()
		{
			if (this._wixWelcome != null)
			{
				string attributeValue = this._wixWelcome.GetAttributeValue("Text");
				if (!string.IsNullOrEmpty(attributeValue))
				{
					attributeValue = attributeValue.Trim();
					if (attributeValue.StartsWith("{\\"))
					{
						int num = attributeValue.IndexOf("}");
						if (num != -1)
						{
							attributeValue = attributeValue.Substring(num + 1);
						}
					}
					return attributeValue;
				}
			}
			return string.Empty;
		}

		protected override void InitializeDialog()
		{
			base.InitializeDialog();
			if (base.WiXDialog != null)
			{
				this._wixCopyrightWarning = base.WiXDialog.ChildEntities.Find((WiXEntity c) => {
					if (!(c is WiXControl))
					{
						return false;
					}
					return c.GetAttributeValue("Id") == "CopyrightWarningText";
				});
				this._wixWelcome = base.WiXDialog.ChildEntities.Find((WiXEntity c) => {
					if (!(c is WiXControl))
					{
						return false;
					}
					return c.GetAttributeValue("Id") == "WelcomeText";
				});
			}
		}

		private bool SetCopyrightWarningValue(string text)
		{
			if (this._wixCopyrightWarning != null)
			{
				string attributeValue = this._wixCopyrightWarning.GetAttributeValue("Text");
				if (!string.IsNullOrEmpty(attributeValue))
				{
					string str = attributeValue.Trim();
					string empty = string.Empty;
					if (str.StartsWith("{\\"))
					{
						int num = str.IndexOf("}");
						if (num != -1)
						{
							empty = str.Substring(0, num + 1);
						}
					}
					str = string.Concat(empty, text.Trim());
					if (attributeValue != str)
					{
						this._wixCopyrightWarning.SetAttributeValue("Text", str);
						this._wixCopyrightWarning.SetDirty();
						return true;
					}
				}
			}
			return false;
		}

		private bool SetWelcomeValue(string text)
		{
			if (this._wixWelcome != null)
			{
				string attributeValue = this._wixWelcome.GetAttributeValue("Text");
				if (!string.IsNullOrEmpty(attributeValue))
				{
					string str = attributeValue.Trim();
					string empty = string.Empty;
					if (str.StartsWith("{\\"))
					{
						int num = str.IndexOf("}");
						if (num != -1)
						{
							empty = str.Substring(0, num + 1);
						}
					}
					str = string.Concat(empty, text.Trim());
					if (attributeValue != str)
					{
						this._wixWelcome.SetAttributeValue("Text", str);
						this._wixWelcome.SetDirty();
						return true;
					}
				}
			}
			return false;
		}
	}
}