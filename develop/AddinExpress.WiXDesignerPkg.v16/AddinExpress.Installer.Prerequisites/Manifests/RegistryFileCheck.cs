using System;
using System.Xml;

namespace AddinExpress.Installer.Prerequisites.Manifests
{
	internal class RegistryFileCheck : AddinExpress.Installer.Prerequisites.Manifests.RegistryCheck
	{
		public string Filename;

		public int SearchDepth;

		public RegistryFileCheck(ValueProperty propertyForResult, string key) : this(propertyForResult, key, null, null, 0)
		{
		}

		public RegistryFileCheck(ValueProperty propertyForResult, string key, string fileName) : this(propertyForResult, key, null, fileName, 0)
		{
		}

		public RegistryFileCheck(ValueProperty propertyForResult, string key, string value, string fileName, int searchDepth) : base(propertyForResult, key, value)
		{
			this.Filename = fileName;
			this.SearchDepth = searchDepth;
		}

		public override XmlElement LoadFromXML(XmlElement workingElement)
		{
			throw new NotImplementedException("NYI");
		}

		public override void WriteXML(XmlElement parentElement)
		{
			Element.BuildMessage(string.Concat("   Registry File Check: ", this.Property.Name), BootstrapperProduct.BuildErrorLevel.None);
			XmlElement xmlElement = base.AddElement(parentElement, "RegistryFileCheck");
			this.WriteBaseAttributes(xmlElement);
			if (!string.IsNullOrEmpty(this.Filename))
			{
				base.AddAttribute(xmlElement, "FileName", this.Filename);
			}
			if (this.SearchDepth > 0)
			{
				base.AddAttribute(xmlElement, "SearchDepth", this.SearchDepth.ToString());
			}
		}

		private class XMLStrings
		{
			public const string ElementName = "RegistryFileCheck";

			public const string FilenameAttribute = "FileName";

			public const string SearchDepth = "SearchDepth";

			public XMLStrings()
			{
			}
		}
	}
}