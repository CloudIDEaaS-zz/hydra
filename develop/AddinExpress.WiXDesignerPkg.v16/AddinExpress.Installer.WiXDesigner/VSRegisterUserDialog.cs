using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSRegisterUserDialog : VSDialogBase
	{
		private WiXEntity _wixRegisterCA;

		[Browsable(true)]
		[DefaultValue("")]
		[Description("The arguments passed to the executable (*.exe) when the user selects the \\'register now'\\ button on this dialog.")]
		[MergableProperty(false)]
		public string Arguments
		{
			get
			{
				return base.GetTextValue(this._wixRegisterCA, "ExeCommand");
			}
			set
			{
				base.SetTextValue(this._wixRegisterCA, "ExeCommand", value, true);
			}
		}

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
		[DefaultValue(null)]
		[Description("The executable that runs when the user selects the \\'register now'\\ button on this dialog.")]
		[Editor(typeof(ExecutableEditor), typeof(UITypeEditor))]
		[MergableProperty(false)]
		[TypeConverter(typeof(ExecutableConverter))]
		public VSBaseFile Executable
		{
			get
			{
				return this.GetExecutableValue();
			}
			set
			{
				this.SetExecutableValue(value);
			}
		}

		public VSRegisterUserDialog(WiXProjectParser project) : base(project)
		{
		}

		public VSRegisterUserDialog(WiXProjectParser project, VSUserInterface collection, AddinExpress.Installer.WiXDesigner.WiXDialog wixElement, AddinExpress.Installer.WiXDesigner.DialogType dialogType, AddinExpress.Installer.WiXDesigner.DialogStage dialogStage, AddinExpress.Installer.WiXDesigner.DialogScope dialogScope) : base(project, collection, wixElement, dialogType, dialogStage, dialogScope)
		{
		}

		private WiXCustomAction CreateRegisterUserCustomAction(string keyType)
		{
			WiXCustomAction wiXCustomAction = null;
			WiXEntity parent = base.WiXDialog.Parent as WiXEntity;
			WiXEntity wiXEntity = parent.Parent as WiXEntity;
			XmlElement xmlElement = Common.CreateXmlElementWithAttributes(base.WiXDialog.XmlNode.OwnerDocument, "CustomAction", base.WiXDialog.XmlNode.NamespaceURI, new string[] { "Id", keyType, "ExeCommand", "Return" }, new string[] { "RegisterUserExeForm_Action", string.Empty, string.Empty, "ignore" }, "", true);
			if (xmlElement != null)
			{
				wiXEntity.XmlNode.InsertBefore(xmlElement, parent.XmlNode);
				wiXCustomAction = new WiXCustomAction(base.Project, base.WiXDialog.Owner, wiXEntity, xmlElement);
				base.Project.SupportedEntities.Add(wiXCustomAction);
				wiXEntity.SetDirty();
				this.UpdateRegisterButtonCondition(string.Empty);
			}
			return wiXCustomAction;
		}

		public override void Delete()
		{
			if (this._wixRegisterCA != null)
			{
				base.Project.SupportedEntities.Remove(this._wixRegisterCA);
				this._wixRegisterCA.Delete();
			}
			base.Delete();
		}

		internal VSBaseFile GetExecutableValue()
		{
			VSBaseFile fileById;
			if (this._wixRegisterCA != null)
			{
				bool flag = false;
				string attributeValue = this._wixRegisterCA.GetAttributeValue("FileKey");
				if (string.IsNullOrEmpty(attributeValue))
				{
					flag = true;
					attributeValue = this._wixRegisterCA.GetAttributeValue("BinaryKey");
				}
				if (!string.IsNullOrEmpty(attributeValue))
				{
					if (!flag)
					{
						fileById = base.Project.FileSystem.GetFileById(attributeValue);
						if (fileById != null)
						{
							return fileById;
						}
					}
					else
					{
						List<VSBinary> binaries = base.Project.Binaries;
						if (binaries != null && binaries.Count > 0)
						{
							fileById = binaries.Find((VSBinary b) => b.WiXElement.Id == attributeValue);
							if (fileById != null)
							{
								return fileById;
							}
						}
					}
				}
			}
			return VSBaseFile.Empty;
		}

		protected override void InitializeDialog()
		{
			base.InitializeDialog();
			if (base.WiXDialog != null)
			{
				WiXEntity wiXEntity = base.WiXDialog.ChildEntities.Find((WiXEntity c) => {
					if (!(c is WiXControl))
					{
						return false;
					}
					return c.GetAttributeValue("Id") == "RegisterButton";
				});
				if (wiXEntity != null)
				{
					WiXEntity wiXEntity1 = wiXEntity.ChildEntities.Find((WiXEntity c) => {
						if (!(c is WiXPublish))
						{
							return false;
						}
						return c.GetAttributeValue("Event") == "DoAction";
					});
					if (wiXEntity1 != null)
					{
						string attributeValue = wiXEntity1.GetAttributeValue("Value");
						if (!string.IsNullOrEmpty(attributeValue))
						{
							this._wixRegisterCA = base.Project.SupportedEntities.Find((WiXEntity c) => {
								if (!(c is WiXCustomAction))
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

		internal bool SetExecutableValue(VSBaseFile value)
		{
			if (value == null || value == VSBaseFile.Empty)
			{
				if (this._wixRegisterCA != null)
				{
					WiXEntity parent = this._wixRegisterCA.Parent as WiXEntity;
					base.Project.SupportedEntities.Remove(this._wixRegisterCA);
					this._wixRegisterCA.Delete();
					this._wixRegisterCA = null;
					if (parent != null)
					{
						parent.SetDirty();
					}
				}
				this.UpdateRegisterButtonCondition(null);
				return true;
			}
			string empty = string.Empty;
			if (this._wixRegisterCA != null)
			{
				empty = this._wixRegisterCA.GetAttributeValue("FileKey");
				if (string.IsNullOrEmpty(empty))
				{
					empty = this._wixRegisterCA.GetAttributeValue("BinaryKey");
				}
			}
			if (value is VSBinary)
			{
				if (this._wixRegisterCA == null)
				{
					this._wixRegisterCA = this.CreateRegisterUserCustomAction("BinaryKey");
				}
				if (this._wixRegisterCA != null)
				{
					string id = (value as VSBinary).WiXElement.Id;
					if (id != empty)
					{
						this._wixRegisterCA.SetAttributeValue("FileKey", null);
						this._wixRegisterCA.SetAttributeValue("BinaryKey", id);
						this._wixRegisterCA.SetDirty();
						this.UpdateRegisterButtonCondition(id);
						return true;
					}
				}
			}
			else if (value is VSFile)
			{
				if (this._wixRegisterCA == null)
				{
					this._wixRegisterCA = this.CreateRegisterUserCustomAction("FileKey");
				}
				if (this._wixRegisterCA != null)
				{
					string str = (value as VSFile).WiXElement.Id;
					if (str != empty)
					{
						this._wixRegisterCA.SetAttributeValue("BinaryKey", null);
						this._wixRegisterCA.SetAttributeValue("FileKey", str);
						this._wixRegisterCA.SetDirty();
						this.UpdateRegisterButtonCondition(str);
						return true;
					}
				}
			}
			return false;
		}

		private void UpdateRegisterButtonCondition(string fileID)
		{
			WiXEntity wiXEntity = base.WiXDialog.ChildEntities.Find((WiXEntity c) => {
				if (!(c is WiXControl))
				{
					return false;
				}
				return c.GetAttributeValue("Id") == "RegisterButton";
			});
			if (wiXEntity != null)
			{
				WiXEntity wiXEntity1 = wiXEntity.ChildEntities.Find((WiXEntity c) => {
					if (!(c is WiXPublish))
					{
						return false;
					}
					return c.GetAttributeValue("Event") == "DoAction";
				});
				WiXCondition wiXCondition = wiXEntity.ChildEntities.Find((WiXEntity c) => {
					if (!(c is WiXCondition))
					{
						return false;
					}
					return c.GetAttributeValue("Action") == "enable";
				}) as WiXCondition;
				if (fileID != null)
				{
					if (wiXEntity1 == null)
					{
						XmlElement xmlElement = Common.CreateXmlElementWithAttributes(base.WiXDialog.XmlNode.OwnerDocument, "Publish", base.WiXDialog.XmlNode.NamespaceURI, new string[] { "Event", "Value" }, new string[] { "DoAction", "RegisterUserExeForm_Action" }, "", false);
						if (xmlElement != null)
						{
							wiXEntity.XmlNode.AppendChild(xmlElement);
							WiXPublish wiXPublish = new WiXPublish(base.Project, base.WiXDialog.Owner, wiXEntity, xmlElement);
							wiXEntity.SetDirty();
						}
					}
					if (wiXCondition != null)
					{
						wiXCondition.Condition = string.Concat("\"\"<>\"", fileID, "\"");
					}
				}
				else
				{
					if (wiXEntity1 != null)
					{
						wiXEntity1.Delete();
						wiXEntity.SetDirty();
					}
					if (wiXCondition != null)
					{
						wiXCondition.Condition = "\"\"<>\"\"";
						return;
					}
				}
			}
		}
	}
}