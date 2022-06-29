using EnvDTE;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSEulaDialog : VSDialogBase
	{
		private WiXEntity _wixLicenseText;

		private WiXEntity _wixEulaFormProperty;

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
		[DefaultValue("")]
		[Description("The rich text file containing the license agreement text to display in this dialog.")]
		[Editor(typeof(OpenRtfFileDialogEditor), typeof(UITypeEditor))]
		[MergableProperty(false)]
		[TypeConverter(typeof(OpenRtfFileConverter))]
		public string LicenseFile
		{
			get
			{
				string empty = string.Empty;
				if (this._wixLicenseText != null && this._wixLicenseText.FirstChild != null)
				{
					empty = base.GetTextValue(this._wixLicenseText.FirstChild as WiXEntity, "SourceFile");
					if (!string.IsNullOrEmpty(empty) && !Path.IsPathRooted(empty) && base.Project.ProjectManager.VsProject != null)
					{
						string directoryName = Path.GetDirectoryName(base.Project.ProjectManager.VsProject.FullName);
						empty = Path.GetFullPath(Path.Combine(CommonUtilities.AbsolutePathFromRelative(Path.GetDirectoryName(empty), directoryName), Path.GetFileName(empty)));
					}
				}
				return empty;
			}
			set
			{
				if (this._wixLicenseText != null)
				{
					if (!string.IsNullOrEmpty(value))
					{
						string str = value;
						if (Path.IsPathRooted(str) && base.Project.ProjectManager.VsProject != null)
						{
							string directoryName = Path.GetDirectoryName(base.Project.ProjectManager.VsProject.FullName);
							str = Path.Combine(CommonUtilities.RelativizePathIfPossible(Path.GetDirectoryName(str), directoryName), Path.GetFileName(str));
						}
						if (this._wixLicenseText.FirstChild != null)
						{
							base.SetTextValue(this._wixLicenseText.FirstChild as WiXEntity, "SourceFile", str);
							return;
						}
						XmlElement xmlElement = Common.CreateXmlElementWithAttributes(this._wixLicenseText.XmlNode.OwnerDocument, "Text", this._wixLicenseText.XmlNode.NamespaceURI, new string[] { "SourceFile" }, new string[] { str }, "", false);
						this._wixLicenseText.XmlNode.AppendChild(xmlElement);
						WiXEntity wiXEntity = new WiXEntity(base.Project, this._wixLicenseText.Owner, this._wixLicenseText, xmlElement);
						this._wixLicenseText.SetDirty();
					}
					else if (this._wixLicenseText.HasChildEntities)
					{
						(this._wixLicenseText.FirstChild as WiXEntity).Delete();
						this._wixLicenseText.SetDirty();
						return;
					}
				}
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
				if (base.GetTextValue(this._wixLicenseText, "Sunken") != "yes")
				{
					return false;
				}
				return true;
			}
			set
			{
				if (value)
				{
					base.SetTextValue(this._wixLicenseText, "Sunken", "yes");
					return;
				}
				base.SetTextValue(this._wixLicenseText, "Sunken", "no");
			}
		}

		public VSEulaDialog(WiXProjectParser project) : base(project)
		{
		}

		public VSEulaDialog(WiXProjectParser project, VSUserInterface collection, AddinExpress.Installer.WiXDesigner.WiXDialog wixElement, AddinExpress.Installer.WiXDesigner.DialogType dialogType, AddinExpress.Installer.WiXDesigner.DialogStage dialogStage, AddinExpress.Installer.WiXDesigner.DialogScope dialogScope) : base(project, collection, wixElement, dialogType, dialogStage, dialogScope)
		{
		}

		public override void Delete()
		{
			if (this._wixEulaFormProperty != null)
			{
				base.Project.SupportedEntities.Remove(this._wixEulaFormProperty);
				this._wixEulaFormProperty.Delete();
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
			if (base.WiXDialog != null)
			{
				this._wixLicenseText = base.WiXDialog.ChildEntities.Find((WiXEntity c) => {
					if (!(c is WiXControl))
					{
						return false;
					}
					return c.GetAttributeValue("Id") == "LicenseText";
				});
				if (base.DialogScope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall)
				{
					this._wixEulaFormProperty = base.WiXDialog.Parent.ChildEntities.Find((WiXEntity c) => {
						if (!(c is WiXProperty))
						{
							return false;
						}
						return c.GetAttributeValue("Id") == "AdminEulaForm_Property";
					});
					this._wixRadioButtonGroup = base.WiXDialog.Parent.ChildEntities.Find((WiXEntity c) => {
						if (!(c is WiXRadioButtonGroup))
						{
							return false;
						}
						return c.GetAttributeValue("Property") == "AdminEulaForm_Property";
					});
					return;
				}
				this._wixEulaFormProperty = base.WiXDialog.Parent.ChildEntities.Find((WiXEntity c) => {
					if (!(c is WiXProperty))
					{
						return false;
					}
					return c.GetAttributeValue("Id") == "EulaForm_Property";
				});
				this._wixRadioButtonGroup = base.WiXDialog.Parent.ChildEntities.Find((WiXEntity c) => {
					if (!(c is WiXRadioButtonGroup))
					{
						return false;
					}
					return c.GetAttributeValue("Property") == "EulaForm_Property";
				});
			}
		}
	}
}