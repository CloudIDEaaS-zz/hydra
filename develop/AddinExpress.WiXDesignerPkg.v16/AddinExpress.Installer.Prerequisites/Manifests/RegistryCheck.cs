using System;
using System.Xml;

namespace AddinExpress.Installer.Prerequisites.Manifests
{
	internal class RegistryCheck : InstallCheck
	{
		public string Key;

		public string Value;

		public RegistryCheck(ValueProperty propertyForResult, string key) : this(propertyForResult, key, null)
		{
		}

		public RegistryCheck(ValueProperty propertyForResult, string key, string value) : base(propertyForResult)
		{
			this.Key = key;
			this.Value = value;
		}

		public override XmlElement LoadFromXML(XmlElement workingElement)
		{
			throw new NotImplementedException("NYI");
		}

		protected override void WriteBaseAttributes(XmlElement parentElement)
		{
			base.WriteBaseAttributes(parentElement);
			base.AddAttribute(parentElement, "Key", this.Key);
			if (!string.IsNullOrEmpty(this.Value))
			{
				base.AddAttribute(parentElement, "Value", this.Value);
			}
		}

		public override void WriteXML(XmlElement parentElement)
		{
			Element.BuildMessage(string.Concat("   Registry Check: ", this.Property.Name), BootstrapperProduct.BuildErrorLevel.None);
			this.WriteBaseAttributes(base.AddElement(parentElement, "RegistryCheck"));
		}

		private class XMLStrings
		{
			public const string ElementName = "RegistryCheck";

			public const string KeyAttribute = "Key";

			public const string ValueAttribute = "Value";

			public XMLStrings()
			{
			}
		}
	}
}