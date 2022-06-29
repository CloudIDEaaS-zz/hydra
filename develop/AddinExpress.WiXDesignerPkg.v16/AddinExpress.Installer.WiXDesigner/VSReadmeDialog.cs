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
	internal class VSReadmeDialog : VSDialogBase
	{
		private WiXEntity _wixReadmeText;

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
		[Description("The rich text file containing the text to display in this dialog.")]
		[Editor(typeof(OpenRtfFileDialogEditor), typeof(UITypeEditor))]
		[MergableProperty(false)]
		[TypeConverter(typeof(OpenRtfFileConverter))]
		public string ReadmeFile
		{
			get
			{
				string empty = string.Empty;
				if (this._wixReadmeText != null && this._wixReadmeText.FirstChild != null)
				{
					empty = base.GetTextValue(this._wixReadmeText.FirstChild as WiXEntity, "SourceFile");
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
				if (this._wixReadmeText != null)
				{
					if (!string.IsNullOrEmpty(value))
					{
						string str = value;
						if (Path.IsPathRooted(str) && base.Project.ProjectManager.VsProject != null)
						{
							string directoryName = Path.GetDirectoryName(base.Project.ProjectManager.VsProject.FullName);
							str = Path.Combine(CommonUtilities.RelativizePathIfPossible(Path.GetDirectoryName(str), directoryName), Path.GetFileName(str));
						}
						if (this._wixReadmeText.FirstChild != null)
						{
							base.SetTextValue(this._wixReadmeText.FirstChild as WiXEntity, "SourceFile", str);
							return;
						}
						XmlElement xmlElement = Common.CreateXmlElementWithAttributes(this._wixReadmeText.XmlNode.OwnerDocument, "Text", this._wixReadmeText.XmlNode.NamespaceURI, new string[] { "SourceFile" }, new string[] { str }, "", false);
						this._wixReadmeText.XmlNode.AppendChild(xmlElement);
						WiXEntity wiXEntity = new WiXEntity(base.Project, this._wixReadmeText.Owner, this._wixReadmeText, xmlElement);
						this._wixReadmeText.SetDirty();
					}
					else if (this._wixReadmeText.HasChildEntities)
					{
						(this._wixReadmeText.FirstChild as WiXEntity).Delete();
						this._wixReadmeText.SetDirty();
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
				if (base.GetTextValue(this._wixReadmeText, "Sunken") != "yes")
				{
					return false;
				}
				return true;
			}
			set
			{
				if (value)
				{
					base.SetTextValue(this._wixReadmeText, "Sunken", "yes");
					return;
				}
				base.SetTextValue(this._wixReadmeText, "Sunken", "no");
			}
		}

		public VSReadmeDialog(WiXProjectParser project) : base(project)
		{
		}

		public VSReadmeDialog(WiXProjectParser project, VSUserInterface collection, AddinExpress.Installer.WiXDesigner.WiXDialog wixElement, AddinExpress.Installer.WiXDesigner.DialogType dialogType, AddinExpress.Installer.WiXDesigner.DialogStage dialogStage, AddinExpress.Installer.WiXDesigner.DialogScope dialogScope) : base(project, collection, wixElement, dialogType, dialogStage, dialogScope)
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
				this._wixReadmeText = base.WiXDialog.ChildEntities.Find((WiXEntity c) => {
					if (!(c is WiXControl))
					{
						return false;
					}
					return c.GetAttributeValue("Id") == "ReadmeText";
				});
			}
		}
	}
}