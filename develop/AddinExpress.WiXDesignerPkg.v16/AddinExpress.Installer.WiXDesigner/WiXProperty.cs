using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXProperty : WiXEntity
	{
		internal string Admin
		{
			get
			{
				return base.GetAttributeValue("Admin");
			}
			set
			{
				base.SetAttributeValue("Admin", value);
				this.SetDirty();
			}
		}

		internal string ComplianceCheck
		{
			get
			{
				return base.GetAttributeValue("ComplianceCheck");
			}
			set
			{
				base.SetAttributeValue("ComplianceCheck", value);
				this.SetDirty();
			}
		}

		internal string Hidden
		{
			get
			{
				return base.GetAttributeValue("Hidden");
			}
			set
			{
				base.SetAttributeValue("Hidden", value);
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

		internal string Secure
		{
			get
			{
				return base.GetAttributeValue("Secure");
			}
			set
			{
				base.SetAttributeValue("Secure", value);
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

		internal string Value
		{
			get
			{
				string attributeValue = base.GetAttributeValue("Value");
				if (attributeValue == null && base.XmlNode.HasChildNodes)
				{
					if (base.XmlNode.FirstChild is XmlText)
					{
						attributeValue = (base.XmlNode.FirstChild as XmlText).Value;
					}
					if (base.XmlNode.FirstChild is XmlCDataSection)
					{
						attributeValue = (base.XmlNode.FirstChild as XmlCDataSection).Value;
					}
				}
				return attributeValue;
			}
			set
			{
				if (!base.XmlNode.HasChildNodes)
				{
					base.SetAttributeValue("Value", value);
				}
				else if (base.XmlNode.FirstChild is XmlText)
				{
					(base.XmlNode.FirstChild as XmlText).Value = value;
				}
				else if (!(base.XmlNode.FirstChild is XmlCDataSection))
				{
					base.SetAttributeValue("Value", value);
				}
				else
				{
					(base.XmlNode.FirstChild as XmlCDataSection).Value = value;
				}
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

		internal WiXProperty(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}