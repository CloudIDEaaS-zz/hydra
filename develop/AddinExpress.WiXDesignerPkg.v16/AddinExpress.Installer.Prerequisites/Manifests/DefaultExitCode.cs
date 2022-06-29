using System;
using System.Xml;

namespace AddinExpress.Installer.Prerequisites.Manifests
{
	internal class DefaultExitCode : baseExitCode
	{
		public bool FormatMessageFromSystem;

		private const bool m_DefaultFormatMessage = true;

		public DefaultExitCode(baseExitCode.Results result) : this(true, result, null)
		{
		}

		public DefaultExitCode(bool formatMessageFromSystem, baseExitCode.Results result, LocString message) : base(result)
		{
			this.FormatMessageFromSystem = true;
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
			Element.BuildMessage("   DefaultExitCode: ", BootstrapperProduct.BuildErrorLevel.None);
			XmlElement xmlElement = base.AddElement(parentElement, "DefaultExitCode");
			base.WriteXML(xmlElement);
			base.AddAttribute(xmlElement, "FormatMessageFromSystem", this.FormatMessageFromSystem.ToString().ToLower());
		}

		private class XMLStrings
		{
			public const string ElementName = "DefaultExitCode";

			public const string FormatMessageAttribute = "FormatMessageFromSystem";

			public XMLStrings()
			{
			}
		}
	}
}