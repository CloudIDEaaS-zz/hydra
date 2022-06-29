using System;
using System.Xml;

namespace AddinExpress.Installer.Prerequisites.Manifests
{
	internal class ExitCode : baseExitCode
	{
		public string Value;

		public ExitCode(string Value, baseExitCode.Results result) : this(Value, result, null)
		{
		}

		public ExitCode(string Value, baseExitCode.Results result, LocString message) : base(result)
		{
			this.Value = Value;
			this.Result = result;
			int num = (int)result;
			if ((num == 1 || num == 3) && message == null)
			{
				throw new baseExitCode.messageRequiredException();
			}
			this.Message = message;
		}

		public override XmlElement LoadFromXML(XmlElement workingElement)
		{
			throw new NotImplementedException("NYI");
		}

		public override void WriteXML(XmlElement parentElement)
		{
			Element.BuildMessage(string.Concat("   ExitCode: ", this.Value), BootstrapperProduct.BuildErrorLevel.None);
			XmlElement xmlElement = base.AddElement(parentElement, "ExitCode");
			base.AddAttribute(xmlElement, "Value", this.Value);
			base.WriteXML(xmlElement);
		}

		private class XMLStrings
		{
			public const string ElementName = "ExitCode";

			public const string ValueAttribute = "Value";

			public XMLStrings()
			{
			}
		}
	}
}