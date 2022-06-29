using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace AddinExpress.Installer.Prerequisites.Manifests
{
	internal class Package : Element
	{
		public CommandsNode Commands = new CommandsNode();

		public CultureInfo Culture;

		public List<InstallCheck> InstallChecks = new List<InstallCheck>();

		private PackageFile m_LicenseAgreement;

		public LocString Name;

		public PackageFilesNode PackageFiles = new PackageFilesNode();

		public List<Schedule> Schedules = new List<Schedule>();

		public PackageFile LicenseAgreement
		{
			get
			{
				return this.m_LicenseAgreement;
			}
			set
			{
				if (this.m_LicenseAgreement != null)
				{
					this.PackageFiles.Files.Remove(this.m_LicenseAgreement);
				}
				this.m_LicenseAgreement = value;
				this.PackageFiles.Files.Add(this.m_LicenseAgreement);
			}
		}

		public Package(CultureInfo culture, LocString name)
		{
			this.Culture = culture;
			this.Name = name;
		}

		public override XmlElement LoadFromXML(XmlElement workingElement)
		{
			throw new NotImplementedException("NYI");
		}

		public override void WriteXML(XmlElement parentElement)
		{
			List<LocString>.Enumerator enumerator = new List<LocString>.Enumerator();
			if (this.Name == null)
			{
				throw new Exception("The product name can't be empty.");
			}
			Element.BuildMessage(string.Concat(" Package: ", this.Name.Value), BootstrapperProduct.BuildErrorLevel.None);
			XmlElement xmlElement = Globals.manifestWorkingFile.CreateElement("Package", "http://schemas.microsoft.com/developer/2004/01/bootstrapper");
			Globals.manifestWorkingFile.AppendChild(xmlElement);
			base.AddAttribute(xmlElement, "Name", this.Name.Name);
			if (this.LicenseAgreement != null)
			{
				base.AddAttribute(xmlElement, "LicenseAgreement", Path.GetFileName(this.LicenseAgreement.SourcePathandFileName).ToLower());
			}
			this.PackageFiles.WriteXML(xmlElement);
			if (this.InstallChecks != null && this.InstallChecks.Count > 0)
			{
				XmlElement xmlElement1 = base.AddElement(xmlElement, "InstallChecks");
				foreach (InstallCheck installCheck in this.InstallChecks)
				{
					installCheck.WriteXML(xmlElement1);
				}
			}
			if (this.Commands != null && this.Commands.Count > 0)
			{
				IEnumerator enumerator1 = null;
				XmlElement xmlElement2 = this.Commands.WriteXML(xmlElement);
				try
				{
					enumerator1 = this.Commands.GetEnumerator();
					while (enumerator1.MoveNext())
					{
						((Command)enumerator1.Current).WriteXML(xmlElement2);
					}
				}
				finally
				{
					if (enumerator1 is IDisposable)
					{
						(enumerator1 as IDisposable).Dispose();
					}
				}
			}
			XmlElement xmlElement3 = null;
			if (0 == 0)
			{
				xmlElement3 = base.AddElement(xmlElement, "Strings");
			}
			base.AddAttribute(xmlElement, "Culture", "Culture");
			LocString locString = new LocString("Culture", this.Culture.Name, this.Culture);
			if (xmlElement3 != null)
			{
				locString.WriteXML(xmlElement3);
			}
			try
			{
				enumerator = Globals.localizedStrings.GetEnumerator();
				while (enumerator.MoveNext())
				{
					LocString current = enumerator.Current;
					if (!current.Culture.Equals(this.Culture))
					{
						continue;
					}
					current.WriteXML(xmlElement3);
				}
			}
			finally
			{
				enumerator.Dispose();
			}
		}

		private class XMLStrings
		{
			public const string CommandsNode = "Commands";

			public const string CultureAttributes = "Culture";

			public const string CultureStringName = "Culture";

			public const string ElementName = "Package";

			public const string InstallChecksNode = "InstallChecks";

			public const string LicenseAgreementAttribute = "LicenseAgreement";

			public const string NameAttribute = "Name";

			public const string PackageFilesNode = "PackageFiles";

			public const string ReferenceFilesNode = "ReferenceFiles";

			public const string StringsNode = "Strings";

			public XMLStrings()
			{
			}
		}
	}
}