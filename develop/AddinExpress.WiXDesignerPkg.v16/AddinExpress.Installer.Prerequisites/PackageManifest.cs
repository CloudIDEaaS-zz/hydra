using System;
using System.Globalization;
using System.IO;
using System.Xml;

namespace AddinExpress.Installer.Prerequisites
{
	internal class PackageManifest
	{
		private FileInfo filePath;

		public CultureInfo Culture
		{
			get
			{
				if (!this.filePath.Exists)
				{
					return null;
				}
				string str = string.Concat("/bs:Package/", "bs:Strings/bs:String[@Name='Culture']");
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(this.filePath.FullName);
				return new CultureInfo(xmlDocument.SelectSingleNode(str, PackageInfo.GetXMLNamespaceManager(xmlDocument, "bs", "http://schemas.microsoft.com/developer/2004/01/bootstrapper")).InnerXml);
			}
		}

		public PackageManifest(FileInfo packageManifestFilePath)
		{
			this.filePath = packageManifestFilePath;
		}
	}
}