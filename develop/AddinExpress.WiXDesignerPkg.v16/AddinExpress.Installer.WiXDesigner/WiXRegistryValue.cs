using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXRegistryValue : WiXEntity
	{
		internal string Action
		{
			get
			{
				return base.GetAttributeValue("Action");
			}
			set
			{
				base.SetAttributeValue("Action", value);
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

		internal string Key
		{
			get
			{
				return base.GetAttributeValue("Key");
			}
			set
			{
				base.SetAttributeValue("Key", value);
				this.SetDirty();
			}
		}

		internal string KeyPath
		{
			get
			{
				return base.GetAttributeValue("KeyPath");
			}
			set
			{
				base.SetAttributeValue("KeyPath", value);
				this.SetDirty();
			}
		}

		internal new string Name
		{
			get
			{
				return base.GetAttributeValue("Name");
			}
			set
			{
				base.SetAttributeValue("Name", value);
				this.SetDirty();
			}
		}

		internal string Root
		{
			get
			{
				return base.GetAttributeValue("Root");
			}
			set
			{
				base.SetAttributeValue("Root", value);
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

		internal string Type
		{
			get
			{
				return base.GetAttributeValue("Type");
			}
			set
			{
				base.SetAttributeValue("Type", value);
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

		internal WiXRegistryValue(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}