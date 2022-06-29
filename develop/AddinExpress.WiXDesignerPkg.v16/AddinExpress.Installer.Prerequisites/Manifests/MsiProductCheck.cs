using System;
using System.Xml;

namespace AddinExpress.Installer.Prerequisites.Manifests
{
	internal class MsiProductCheck : InstallCheck
	{
		public Guid ProductId;

		public Guid FeatureId;

		public MsiProductCheck(ValueProperty propertyForResult, Guid msiProductID, Guid msiFeatureID) : base(propertyForResult)
		{
			this.ProductId = msiProductID;
			this.FeatureId = msiFeatureID;
		}

		public override XmlElement LoadFromXML(XmlElement workingElement)
		{
			throw new NotImplementedException("NYI");
		}

		public override void WriteXML(XmlElement parentElement)
		{
			Element.BuildMessage(string.Concat("   MSI Product Check: ", this.Property.Name), BootstrapperProduct.BuildErrorLevel.None);
			XmlElement xmlElement = base.AddElement(parentElement, "MsiProductCheck");
			this.WriteBaseAttributes(xmlElement);
			base.AddAttribute(xmlElement, "Product", string.Concat("{", this.ProductId.ToString().ToUpper(), "}"));
			if (!this.FeatureId.Equals(Guid.Empty))
			{
				base.AddAttribute(xmlElement, "Feature", string.Concat("{", this.FeatureId.ToString().ToUpper(), "}"));
			}
		}

		private class XMLStrings
		{
			public const string ComponentIDName = "Product";

			public const string FeatureIDName = "Feature";

			public const string ElementName = "MsiProductCheck";

			public XMLStrings()
			{
			}
		}
	}
}