using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace AddinExpress.Installer.Prerequisites.Manifests
{
	internal class Product : Element
	{
		public CommandsNode Commands = new CommandsNode();

		public List<InstallCheck> InstallChecks = new List<InstallCheck>();

		public PackageFilesNode PackageFiles = new PackageFilesNode();

		public string ProductCode;

		public Relationships RelatedProducts = new Relationships();

		public List<Schedule> Schedules = new List<Schedule>();

		public Product(string productCode)
		{
			this.ProductCode = productCode;
		}

		public override XmlElement LoadFromXML(XmlElement workingElement)
		{
			throw new NotImplementedException("NYI");
		}

		public override void WriteXML(XmlElement parentElement)
		{
			Element.BuildMessage(string.Concat("Product: ", this.ProductCode), BootstrapperProduct.BuildErrorLevel.None);
			XmlElement xmlElement = Globals.manifestWorkingFile.CreateElement("Product", "http://schemas.microsoft.com/developer/2004/01/bootstrapper");
			Globals.manifestWorkingFile.AppendChild(xmlElement);
			base.AddAttribute(xmlElement, "ProductCode", this.ProductCode);
			this.PackageFiles.WriteXML(xmlElement);
			this.RelatedProducts.WriteXML(xmlElement);
			if (this.Schedules != null && this.Schedules.Count > 0)
			{
				XmlElement xmlElement1 = base.AddElement(xmlElement, "Schedules");
				foreach (Schedule schedule in this.Schedules)
				{
					schedule.WriteXML(xmlElement1);
				}
			}
			if (this.InstallChecks != null && this.InstallChecks.Count > 0)
			{
				XmlElement xmlElement2 = base.AddElement(xmlElement, "InstallChecks");
				foreach (InstallCheck installCheck in this.InstallChecks)
				{
					installCheck.WriteXML(xmlElement2);
				}
			}
			if (this.Commands != null && this.Commands.Count > 0)
			{
				IEnumerator enumerator = null;
				XmlElement xmlElement3 = this.Commands.WriteXML(xmlElement);
				try
				{
					enumerator = this.Commands.GetEnumerator();
					while (enumerator.MoveNext())
					{
						((Command)enumerator.Current).WriteXML(xmlElement3);
					}
				}
				finally
				{
					if (enumerator is IDisposable)
					{
						(enumerator as IDisposable).Dispose();
					}
				}
			}
		}

		private class XMLStrings
		{
			public const string CommandsNode = "Commands";

			public const string ElementName = "Product";

			public const string InstallChecksNode = "InstallChecks";

			public const string ProductCodeAttribute = "ProductCode";

			public const string ReferenceFilesNode = "ReferenceFiles";

			public const string SchedulesNode = "Schedules";

			public XMLStrings()
			{
			}
		}
	}
}