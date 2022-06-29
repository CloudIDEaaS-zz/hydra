using System;
using System.Collections.Generic;
using System.Xml;

namespace AddinExpress.Installer.Prerequisites.Manifests
{
	internal class PackageFilesNode : Element
	{
		public List<PackageFile> Files = new List<PackageFile>();

		private PackageFilesNode.CopyAllFilesValues m_CopyAllFiles;

		private PackageFilesNode.CopyAllFilesValues m_CopyAllFilesDefault;

		private string m_IfNotHomesiteString = "IfNotHomeSite";

		public PackageFilesNode.CopyAllFilesValues CopyAllPackageFiles
		{
			get
			{
				return this.m_CopyAllFiles;
			}
			set
			{
				this.m_CopyAllFiles = value;
			}
		}

		public PackageFilesNode()
		{
		}

		public string ConvertCopyAllFilesToString(PackageFilesNode.CopyAllFilesValues value)
		{
			switch (value)
			{
				case PackageFilesNode.CopyAllFilesValues.True:
				case PackageFilesNode.CopyAllFilesValues.Default:
				{
					return bool.TrueString.ToLower();
				}
				case PackageFilesNode.CopyAllFilesValues.False:
				{
					return bool.FalseString.ToLower();
				}
				case PackageFilesNode.CopyAllFilesValues.IfNotHomesite:
				{
					return this.m_IfNotHomesiteString;
				}
			}
			throw new ArgumentException("Unknown Value Passed in. Use a valid CopyAllFilesValues enum");
		}

		public PackageFilesNode.CopyAllFilesValues ConvertStringToCopyAllFiles(string input)
		{
			string lower = input.ToLower();
			if (lower == bool.TrueString.ToLower())
			{
				return PackageFilesNode.CopyAllFilesValues.True;
			}
			if (lower == bool.FalseString.ToLower())
			{
				return PackageFilesNode.CopyAllFilesValues.False;
			}
			if (lower != this.m_IfNotHomesiteString.ToLower())
			{
				throw new ArgumentException(string.Concat(new string[] { "Unknown Value: Try ", bool.TrueString, " or ", bool.FalseString, " or ", this.m_IfNotHomesiteString }));
			}
			return PackageFilesNode.CopyAllFilesValues.IfNotHomesite;
		}

		public override XmlElement LoadFromXML(XmlElement workingElement)
		{
			throw new NotImplementedException("Not Yet Implemented");
		}

		public override void WriteXML(XmlElement parentElement)
		{
			Element.BuildMessage("   PackageFiles: ", BootstrapperProduct.BuildErrorLevel.None);
			if (this.Files.Count > 0)
			{
				XmlElement xmlElement = base.AddElement(parentElement, "PackageFiles");
				if (this.CopyAllPackageFiles != PackageFilesNode.CopyAllFilesValues.Default)
				{
					XmlAttribute str = xmlElement.OwnerDocument.CreateAttribute("CopyAllPackageFiles");
					str.Value = this.ConvertCopyAllFilesToString(this.CopyAllPackageFiles);
					xmlElement.Attributes.Append(str);
				}
				foreach (PackageFile file in this.Files)
				{
					file.WriteXML(xmlElement);
				}
			}
		}

		public enum CopyAllFilesValues
		{
			True,
			False,
			IfNotHomesite,
			Default
		}

		private class XMLStrings
		{
			public const string CopyAllPackageFilesAttribute = "CopyAllPackageFiles";

			public const string ElementName = "PackageFiles";

			public const string NameAttribute = "Name";

			public XMLStrings()
			{
			}
		}
	}
}