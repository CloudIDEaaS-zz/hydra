using System;
using System.IO;
using System.Xml;

namespace AddinExpress.Installer.Prerequisites.Manifests
{
	internal class ExternalCheck : InstallCheck
	{
		public string Args;

		public PackageFile FileToRun;

		public string Log;

		public ExternalCheck(ValueProperty propertyToCheck, PackageFile fileToRun) : this(propertyToCheck, fileToRun, null)
		{
		}

		public ExternalCheck(ValueProperty propertyToCheck, PackageFile fileToRun, string args) : base(propertyToCheck)
		{
			this.FileToRun = fileToRun;
			this.Args = args;
		}

		public ExternalCheck(ValueProperty propertyToCheck, PackageFile fileToRun, string args, string log) : base(propertyToCheck)
		{
			this.FileToRun = fileToRun;
			this.Args = args;
			this.Log = log;
		}

		public override XmlElement LoadFromXML(XmlElement workingElement)
		{
			throw new NotImplementedException("NYI");
		}

		public override void WriteXML(XmlElement parentElement)
		{
			Element.BuildMessage(string.Concat("   External Check: ", this.FileToRun.SourcePathandFileName), BootstrapperProduct.BuildErrorLevel.None);
			XmlElement xmlElement = base.AddElement(parentElement, "ExternalCheck");
			base.AddAttribute(xmlElement, "PackageFile", Path.GetFileName(this.FileToRun.SourcePathandFileName).ToLower());
			if (this.Args != null && this.Args.Length > 0)
			{
				base.AddAttribute(xmlElement, "Arguments", this.Args);
			}
			if (!string.IsNullOrEmpty(this.Log))
			{
				base.AddAttribute(xmlElement, "Log", this.Log);
			}
			this.WriteBaseAttributes(xmlElement);
		}

		private class XMLStrings
		{
			public const string ArgsAttribute = "Arguments";

			public const string ElementName = "ExternalCheck";

			public const string LogAttribute = "Log";

			public const string PackageFile = "PackageFile";

			public XMLStrings()
			{
			}
		}
	}
}