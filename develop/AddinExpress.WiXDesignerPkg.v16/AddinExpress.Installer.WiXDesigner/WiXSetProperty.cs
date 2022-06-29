using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXSetProperty : WiXEntity
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

		internal string After
		{
			get
			{
				return base.GetAttributeValue("After");
			}
			set
			{
				base.SetAttributeValue("After", value);
				this.SetDirty();
			}
		}

		internal string Before
		{
			get
			{
				return base.GetAttributeValue("Before");
			}
			set
			{
				base.SetAttributeValue("Before", value);
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
				if (base.XmlNode.HasChildNodes)
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

		internal string Sequence
		{
			get
			{
				return base.GetAttributeValue("Sequence");
			}
			set
			{
				base.SetAttributeValue("Sequence", value);
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

		internal WiXSetProperty(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}