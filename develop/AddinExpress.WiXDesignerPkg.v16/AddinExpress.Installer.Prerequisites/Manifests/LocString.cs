using System;
using System.Globalization;
using System.Xml;

namespace AddinExpress.Installer.Prerequisites.Manifests
{
	internal class LocString : Element
	{
		public CultureInfo Culture;

		private string m_name;

		public string Value;

		public string Name
		{
			get
			{
				return this.m_name;
			}
		}

		public LocString(string name, string value, CultureInfo culture)
		{
			this.m_name = name;
			this.Value = value;
			this.Culture = culture;
		}

		public override XmlElement LoadFromXML(XmlElement workingElement)
		{
			throw new NotImplementedException("NYI");
		}

		public override void WriteXML(XmlElement parentElement)
		{
			XmlElement value = base.AddElement(parentElement, "String");
			base.AddAttribute(value, "Name", this.Name);
			value.InnerXml = this.Value;
		}

		private class XMLStrings
		{
			public const string CultureAttribute = "Culture";

			public const string ElementName = "String";

			public const string NameAttribute = "Name";

			public const string ValueAttribute = "Value";

			public XMLStrings()
			{
			}
		}
	}
}