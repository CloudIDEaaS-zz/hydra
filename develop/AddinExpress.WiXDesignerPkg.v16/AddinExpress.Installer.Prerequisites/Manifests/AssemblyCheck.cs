using System;
using System.Globalization;
using System.Xml;

namespace AddinExpress.Installer.Prerequisites.Manifests
{
	internal class AssemblyCheck : InstallCheck
	{
		public string AssemblyName;

		public CultureInfo Culture;

		public string ProcessorArchitecture;

		public string PublicKeyToken;

		public string Version;

		internal AssemblyCheck(ValueProperty propertyForResult) : base(propertyForResult)
		{
		}

		public AssemblyCheck(ValueProperty propertyForResult, string assemblyName, string publicKeyToken, string version) : this(propertyForResult)
		{
			this.AssemblyName = assemblyName;
			this.PublicKeyToken = publicKeyToken;
			this.Version = version;
		}

		public AssemblyCheck(ValueProperty propertyForResult, string assemblyName, string publicKeyToken, string version, CultureInfo culture, string processorArchitecture) : this(propertyForResult, assemblyName, publicKeyToken, version)
		{
			this.Culture = culture;
			this.ProcessorArchitecture = processorArchitecture;
		}

		public override XmlElement LoadFromXML(XmlElement workingElement)
		{
			throw new NotImplementedException("NYI");
		}

		public override void WriteXML(XmlElement parentElement)
		{
			Element.BuildMessage(string.Concat("   GACCheck: ", this.Property.Name), BootstrapperProduct.BuildErrorLevel.None);
			XmlElement xmlElement = base.AddElement(parentElement, "AssemblyCheck");
			this.WriteBaseAttributes(xmlElement);
			base.AddAttribute(xmlElement, "Name", this.AssemblyName);
			base.AddAttribute(xmlElement, "PublicKeyToken", this.PublicKeyToken);
			base.AddAttribute(xmlElement, "Version", this.Version);
			if (this.Culture != null)
			{
				base.AddAttribute(xmlElement, "Culture", this.Culture.Name);
			}
			if (!string.IsNullOrEmpty(this.ProcessorArchitecture))
			{
				base.AddAttribute(xmlElement, "ProcessorArchitecture", this.ProcessorArchitecture);
			}
		}

		private class XMLStrings
		{
			public const string AssemblyNameAttribute = "Name";

			public const string CultureAttribute = "Culture";

			public const string ElementName = "AssemblyCheck";

			public const string ProcessorArchitectureAttribute = "ProcessorArchitecture";

			public const string PublicKeyTokenAttribute = "PublicKeyToken";

			public const string VersionAttribute = "Version";

			public XMLStrings()
			{
			}
		}
	}
}