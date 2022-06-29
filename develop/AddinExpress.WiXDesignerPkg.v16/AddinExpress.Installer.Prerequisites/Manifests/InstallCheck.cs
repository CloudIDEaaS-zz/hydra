using System;
using System.Xml;

namespace AddinExpress.Installer.Prerequisites.Manifests
{
	internal abstract class InstallCheck : Element
	{
		public ValueProperty Property;

		public InstallCheck(ValueProperty propertyForResult)
		{
			this.Property = propertyForResult;
		}

		protected virtual void WriteBaseAttributes(XmlElement parentElement)
		{
			base.AddAttribute(parentElement, "Property", this.Property.Name);
		}

		private class XMLStrings
		{
			public const string PropertyAttribute = "Property";

			public XMLStrings()
			{
			}
		}
	}
}