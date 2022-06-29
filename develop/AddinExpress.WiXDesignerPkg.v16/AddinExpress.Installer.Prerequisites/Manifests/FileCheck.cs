using System;
using System.Xml;

namespace AddinExpress.Installer.Prerequisites.Manifests
{
	internal class FileCheck : InstallCheck
	{
		public string FileName;

		public int SearchDepth;

		public string SearchPath;

		public string SpecialFolder;

		public FileCheck(ValueProperty propertyForResult, string fileName, string searchPath) : this(propertyForResult, fileName, searchPath, null, 0)
		{
		}

		public FileCheck(ValueProperty propertyForResult, string fileName, string searchPath, string specialFolder, int searchDepth) : base(propertyForResult)
		{
			this.FileName = fileName;
			this.SearchPath = searchPath;
			this.SpecialFolder = specialFolder;
			this.SearchDepth = searchDepth;
		}

		public override XmlElement LoadFromXML(XmlElement workingElement)
		{
			throw new NotImplementedException("NYI");
		}

		public override void WriteXML(XmlElement parentElement)
		{
			Element.BuildMessage(string.Concat("   FileCheck: ", this.Property.Name), BootstrapperProduct.BuildErrorLevel.None);
			XmlElement xmlElement = base.AddElement(parentElement, "FileCheck");
			this.WriteBaseAttributes(xmlElement);
			if (!string.IsNullOrEmpty(this.SpecialFolder))
			{
				base.AddAttribute(xmlElement, "SpecialFolder", this.SpecialFolder);
			}
			base.AddAttribute(xmlElement, "SearchPath", this.SearchPath);
			base.AddAttribute(xmlElement, "FileName", this.FileName);
			if (this.SearchDepth > 0)
			{
				base.AddAttribute(xmlElement, "SearchDepth", this.SearchDepth.ToString());
			}
		}

		private class XMLStrings
		{
			public const string ElementName = "FileCheck";

			public const string FileName = "FileName";

			public const string SearchDepth = "SearchDepth";

			public const string SearchPathAttribute = "SearchPath";

			public const string SpecialFolder = "SpecialFolder";

			public XMLStrings()
			{
			}
		}
	}
}