using System;
using System.Xml;

namespace AddinExpress.Installer.Prerequisites.Manifests
{
	internal class ByPassIf : InstallCondition
	{
		public ByPassIf(BuiltInProperties.SystemChecks propertyToCheck, InstallCondition.Comparisons compare, string value) : base(propertyToCheck, compare, value)
		{
		}

		public ByPassIf(ValueProperty propertyToCheck, InstallCondition.Comparisons compare, string value) : this(propertyToCheck, compare, value, null)
		{
		}

		public ByPassIf(ValueProperty propertyToCheck, InstallCondition.Comparisons compare, string value, AddinExpress.Installer.Prerequisites.Manifests.Schedule schedule) : base(propertyToCheck, compare, value, schedule)
		{
		}

		public override XmlElement LoadFromXML(XmlElement workingElement)
		{
			throw new NotImplementedException("NYI");
		}

		public override void WriteXML(XmlElement parentElement)
		{
			Element.BuildMessage(string.Concat("   ByPassIf: ", this.Property.Name), BootstrapperProduct.BuildErrorLevel.None);
			base.WriteBaseAttributes(base.AddElement(parentElement, "BypassIf"));
		}

		private class XMLStrings
		{
			public const string ElementName = "BypassIf";

			public XMLStrings()
			{
			}
		}
	}
}