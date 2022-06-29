using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXCondition : WiXEntity
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

		internal string Condition
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

		internal string Level
		{
			get
			{
				return base.GetAttributeValue("Level");
			}
			set
			{
				base.SetAttributeValue("Level", value);
				this.SetDirty();
			}
		}

		internal string Message
		{
			get
			{
				return base.GetAttributeValue("Message");
			}
			set
			{
				base.SetAttributeValue("Message", value);
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

		internal WiXCondition(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}