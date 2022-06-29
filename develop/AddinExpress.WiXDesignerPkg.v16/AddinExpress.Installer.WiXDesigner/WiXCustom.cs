using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXCustom : WiXEntity
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
				if (!string.IsNullOrEmpty(value))
				{
					base.SetAttributeValue("Before", null);
					base.SetAttributeValue("OnExit", null);
					base.SetAttributeValue("Sequence", null);
				}
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
				if (!string.IsNullOrEmpty(value))
				{
					base.SetAttributeValue("After", null);
					base.SetAttributeValue("OnExit", null);
					base.SetAttributeValue("Sequence", null);
				}
				this.SetDirty();
			}
		}

		internal string OnExit
		{
			get
			{
				return base.GetAttributeValue("OnExit");
			}
			set
			{
				base.SetAttributeValue("OnExit", value);
				if (!string.IsNullOrEmpty(value))
				{
					base.SetAttributeValue("After", null);
					base.SetAttributeValue("Before", null);
					base.SetAttributeValue("Sequence", null);
				}
				this.SetDirty();
			}
		}

		internal string Overridable
		{
			get
			{
				return base.GetAttributeValue("Overridable");
			}
			set
			{
				base.SetAttributeValue("Overridable", value);
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
				if (!string.IsNullOrEmpty(value))
				{
					base.SetAttributeValue("After", null);
					base.SetAttributeValue("Before", null);
					base.SetAttributeValue("OnExit", null);
				}
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

		internal WiXCustom(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}