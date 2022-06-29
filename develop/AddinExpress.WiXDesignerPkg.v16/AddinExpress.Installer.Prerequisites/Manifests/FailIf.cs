using System;
using System.Xml;

namespace AddinExpress.Installer.Prerequisites.Manifests
{
	internal class FailIf : InstallCondition
	{
		public LocString String;

		public FailIf(BuiltInProperties.SystemChecks propertyToCheck, InstallCondition.Comparisons compare, string value, LocString message) : base(propertyToCheck, compare, value)
		{
			this.String = message;
		}

		public FailIf(ValueProperty propertyToCheck, InstallCondition.Comparisons compare, string value, LocString message) : this(propertyToCheck, compare, value, message, null)
		{
		}

		public FailIf(ValueProperty propertyToCheck, InstallCondition.Comparisons compare, string value, LocString message, AddinExpress.Installer.Prerequisites.Manifests.Schedule schedule) : base(propertyToCheck, compare, value, schedule)
		{
			this.String = message;
		}

		public override XmlElement LoadFromXML(XmlElement workingElement)
		{
			throw new NotImplementedException("NYI");
		}

		public override void WriteXML(XmlElement parentElement)
		{
			Element.BuildMessage(string.Concat("   FailIf: ", this.Property.Name), BootstrapperProduct.BuildErrorLevel.None);
			XmlElement xmlElement = base.AddElement(parentElement, "FailIf");
			base.WriteBaseAttributes(xmlElement);
			base.AddAttribute(xmlElement, "String", this.String.Name);
		}

		private class XMLStrings
		{
			public const string ElementName = "FailIf";

			public const string StringAttribute = "String";

			public XMLStrings()
			{
			}
		}
	}
}