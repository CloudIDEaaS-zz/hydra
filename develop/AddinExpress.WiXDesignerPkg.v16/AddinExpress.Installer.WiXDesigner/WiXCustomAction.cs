using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXCustomAction : WiXEntity
	{
		internal string BinaryKey
		{
			get
			{
				return base.GetAttributeValue("BinaryKey");
			}
			set
			{
				base.SetAttributeValue("BinaryKey", value);
				this.SetDirty();
			}
		}

		internal string Directory
		{
			get
			{
				return base.GetAttributeValue("Directory");
			}
			set
			{
				base.SetAttributeValue("Directory", value);
				this.SetDirty();
			}
		}

		internal string DllEntry
		{
			get
			{
				return base.GetAttributeValue("DllEntry");
			}
			set
			{
				base.SetAttributeValue("DllEntry", value);
				this.SetDirty();
			}
		}

		internal string Error
		{
			get
			{
				return base.GetAttributeValue("Error");
			}
			set
			{
				base.SetAttributeValue("Error", value);
				this.SetDirty();
			}
		}

		internal string ExeCommand
		{
			get
			{
				return base.GetAttributeValue("ExeCommand");
			}
			set
			{
				base.SetAttributeValue("ExeCommand", value);
				this.SetDirty();
			}
		}

		internal string Execute
		{
			get
			{
				return base.GetAttributeValue("Execute");
			}
			set
			{
				base.SetAttributeValue("Execute", value);
				this.SetDirty();
			}
		}

		internal string FileKey
		{
			get
			{
				return base.GetAttributeValue("FileKey");
			}
			set
			{
				base.SetAttributeValue("FileKey", value);
				this.SetDirty();
			}
		}

		internal string HideTarget
		{
			get
			{
				return base.GetAttributeValue("HideTarget");
			}
			set
			{
				base.SetAttributeValue("HideTarget", value);
				this.SetDirty();
			}
		}

		internal string Id
		{
			get
			{
				return base.GetAttributeValue("Id");
			}
			set
			{
				base.SetAttributeValue("Id", value);
				this.SetDirty();
			}
		}

		internal string Impersonate
		{
			get
			{
				return base.GetAttributeValue("Impersonate");
			}
			set
			{
				base.SetAttributeValue("Impersonate", value);
				this.SetDirty();
			}
		}

		internal string JScriptCall
		{
			get
			{
				return base.GetAttributeValue("JScriptCall");
			}
			set
			{
				base.SetAttributeValue("JScriptCall", value);
				this.SetDirty();
			}
		}

		internal string PatchUninstall
		{
			get
			{
				return base.GetAttributeValue("PatchUninstall");
			}
			set
			{
				base.SetAttributeValue("PatchUninstall", value);
				this.SetDirty();
			}
		}

		internal string Property
		{
			get
			{
				return base.GetAttributeValue("Property");
			}
			set
			{
				base.SetAttributeValue("Property", value);
				this.SetDirty();
			}
		}

		internal string Return
		{
			get
			{
				return base.GetAttributeValue("Return");
			}
			set
			{
				base.SetAttributeValue("Return", value);
				this.SetDirty();
			}
		}

		internal string Script
		{
			get
			{
				return base.GetAttributeValue("Script");
			}
			set
			{
				base.SetAttributeValue("Script", value);
				this.SetDirty();
			}
		}

		public override object SupportedObject
		{
			get
			{
				return this;
			}
		}

		internal string SuppressModularization
		{
			get
			{
				return base.GetAttributeValue("SuppressModularization");
			}
			set
			{
				base.SetAttributeValue("SuppressModularization", value);
				this.SetDirty();
			}
		}

		internal string TerminalServerAware
		{
			get
			{
				return base.GetAttributeValue("TerminalServerAware");
			}
			set
			{
				base.SetAttributeValue("TerminalServerAware", value);
				this.SetDirty();
			}
		}

		internal string Text
		{
			get
			{
				if (base.XmlNode.HasChildNodes)
				{
					if (base.XmlNode.FirstChild is XmlText)
					{
						return (base.XmlNode.FirstChild as XmlText).Value;
					}
					if (base.XmlNode.FirstChild is XmlCDataSection)
					{
						return (base.XmlNode.FirstChild as XmlCDataSection).Value;
					}
				}
				return string.Empty;
			}
			set
			{
				if (!base.XmlNode.HasChildNodes)
				{
					XmlCDataSection xmlCDataSection = base.XmlNode.OwnerDocument.CreateCDataSection(value);
					base.XmlNode.AppendChild(xmlCDataSection);
				}
				else
				{
					if (base.XmlNode.FirstChild is XmlText)
					{
						(base.XmlNode.FirstChild as XmlText).Value = value;
					}
					if (base.XmlNode.FirstChild is XmlCDataSection)
					{
						(base.XmlNode.FirstChild as XmlCDataSection).Value = value;
					}
				}
				this.SetDirty();
			}
		}

		internal string Value
		{
			get
			{
				return base.GetAttributeValue("Value");
			}
			set
			{
				base.SetAttributeValue("Value", value);
				this.SetDirty();
			}
		}

		internal string VBScriptCall
		{
			get
			{
				return base.GetAttributeValue("VBScriptCall");
			}
			set
			{
				base.SetAttributeValue("VBScriptCall", value);
				this.SetDirty();
			}
		}

		internal string VSName
		{
			get
			{
				return base.GetAttributeValue("VSName", "http://schemas.add-in-express.com/wixdesigner");
			}
			set
			{
				base.SetAttributeValue("VSName", "http://schemas.add-in-express.com/wixdesigner", value);
				this.SetDirty();
			}
		}

		internal string Win64
		{
			get
			{
				return base.GetAttributeValue("Win64");
			}
			set
			{
				base.SetAttributeValue("Win64", value);
				this.SetDirty();
			}
		}

		internal WiXCustomAction(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}