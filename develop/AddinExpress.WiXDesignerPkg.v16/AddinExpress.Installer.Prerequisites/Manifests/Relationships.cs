using System;
using System.Collections.Specialized;
using System.Xml;

namespace AddinExpress.Installer.Prerequisites.Manifests
{
	internal class Relationships : Element
	{
		public StringCollection DependedOnProducts = new StringCollection();

		public StringCollection IncludedProducts = new StringCollection();

		public Relationships()
		{
		}

		public override XmlElement LoadFromXML(XmlElement workingElement)
		{
			throw new NotImplementedException("NYI");
		}

		public override void WriteXML(XmlElement parentElement)
		{
			XmlElement xmlElement;
			if (this.DependedOnProducts != null && this.DependedOnProducts.Count > 0 || this.IncludedProducts != null && this.IncludedProducts.Count > 0)
			{
				XmlElement xmlElement1 = parentElement.OwnerDocument.CreateElement("RelatedProducts", "http://schemas.microsoft.com/developer/2004/01/bootstrapper");
				parentElement.AppendChild(xmlElement1);
				if (this.DependedOnProducts != null && this.DependedOnProducts.Count > 0)
				{
					StringEnumerator enumerator = this.DependedOnProducts.GetEnumerator();
					while (enumerator.MoveNext())
					{
						string current = enumerator.Current;
						xmlElement = base.AddElement(xmlElement1, "DependsOnProduct");
						base.AddAttribute(xmlElement, "Code", current);
					}
				}
				if (this.IncludedProducts != null && this.IncludedProducts.Count > 0)
				{
					StringEnumerator stringEnumerator = this.IncludedProducts.GetEnumerator();
					while (stringEnumerator.MoveNext())
					{
						string str = stringEnumerator.Current;
						xmlElement = base.AddElement(xmlElement1, "IncludesProduct");
						base.AddAttribute(xmlElement, "Code", str);
					}
				}
			}
		}

		private class XMLStrings
		{
			public const string CodeAttribute = "Code";

			public const string DependsOn = "DependsOnProduct";

			public const string ElementName = "RelatedProducts";

			public const string IncludesProduct = "IncludesProduct";

			public XMLStrings()
			{
			}
		}
	}
}