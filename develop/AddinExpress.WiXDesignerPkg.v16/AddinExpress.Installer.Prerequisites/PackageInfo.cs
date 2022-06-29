using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml;

namespace AddinExpress.Installer.Prerequisites
{
	internal class PackageInfo
	{
		internal const string BootstrapperNamespace = "http://schemas.microsoft.com/developer/2004/01/bootstrapper";

		internal const string BootstrapperPrefix = "bs";

		private DirectoryInfo m_RootPath;

		public List<PackageInfo> DependentPackages
		{
			get
			{
				IEnumerator enumerator = null;
				XmlDocument xmlDocument = this.LoadProductXMLFile();
				string str = "/bs:Product/bs:RelatedProducts/bs:DependsOnProduct";
				XmlNodeList xmlNodeLists = xmlDocument.SelectNodes(str, PackageInfo.GetXMLNamespaceManager(xmlDocument, "bs", "http://schemas.microsoft.com/developer/2004/01/bootstrapper"));
				if (xmlNodeLists.Count == 0)
				{
					return null;
				}
				List<PackageInfo> packageInfos = new List<PackageInfo>();
				try
				{
					enumerator = xmlNodeLists.GetEnumerator();
					while (enumerator.MoveNext())
					{
						PackageInfo packageInfo = new PackageInfo(((XmlElement)enumerator.Current).Attributes["Code"].Value);
						packageInfos.Add(packageInfo);
					}
				}
				finally
				{
					if (enumerator is IDisposable)
					{
						(enumerator as IDisposable).Dispose();
					}
				}
				return packageInfos;
			}
		}

		public DirectoryInfo Directory
		{
			get
			{
				return this.m_RootPath;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.get_DisplayName(Thread.CurrentThread.CurrentUICulture, false);
			}
		}

		public List<PackageInfo> IncludedPackages
		{
			get
			{
				IEnumerator enumerator = null;
				XmlDocument xmlDocument = this.LoadProductXMLFile();
				string str = "/bs:Product/bs:RelatedProducts/bs:IncludesProduct";
				XmlNodeList xmlNodeLists = xmlDocument.SelectNodes(str, PackageInfo.GetXMLNamespaceManager(xmlDocument, "bs", "http://schemas.microsoft.com/developer/2004/01/bootstrapper"));
				if (xmlNodeLists.Count == 0)
				{
					return null;
				}
				List<PackageInfo> packageInfos = new List<PackageInfo>();
				try
				{
					enumerator = xmlNodeLists.GetEnumerator();
					while (enumerator.MoveNext())
					{
						PackageInfo packageInfo = new PackageInfo(((XmlElement)enumerator.Current).Attributes["Code"].Value);
						packageInfos.Add(packageInfo);
					}
				}
				finally
				{
					if (enumerator is IDisposable)
					{
						(enumerator as IDisposable).Dispose();
					}
				}
				return packageInfos;
			}
		}

		public List<CultureInfo> InstalledCultures
		{
			get
			{
				List<CultureInfo> cultureInfos = new List<CultureInfo>();
				DirectoryInfo[] directories = this.m_RootPath.GetDirectories();
				for (int i = 0; i < (int)directories.Length; i++)
				{
					FileInfo[] files = directories[i].GetFiles("package.xml");
					if ((int)files.Length == 1)
					{
						cultureInfos.Add((new PackageManifest(files[0])).Culture);
					}
				}
				return cultureInfos;
			}
		}

		public string this[CultureInfo uiCulture]
		{
			get
			{
				return this.get_DisplayName(uiCulture, false);
			}
		}

		public string ProductCode
		{
			get
			{
				FileInfo files = this.m_RootPath.GetFiles("product.xml")[0];
				if (!files.Exists)
				{
					return null;
				}
				string str = "/bs:Product/@ProductCode";
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(files.FullName);
				return xmlDocument.SelectSingleNode(str, PackageInfo.GetXMLNamespaceManager(xmlDocument, "bs", "http://schemas.microsoft.com/developer/2004/01/bootstrapper")).InnerXml;
			}
		}

		public PackageInfo(DirectoryInfo productDirectory)
		{
			this.m_RootPath = productDirectory;
		}

		public PackageInfo(string productCode)
		{
			if (this.SearchForProductCode(productCode) == null)
			{
				throw new Exception(string.Concat("Requested Product Code could not be found: ", productCode));
			}
		}

		private static string ExtractDisplayName(FileInfo filePath)
		{
			string str;
			if (!filePath.Exists)
			{
				return null;
			}
			str = (filePath.Name.ToLower() != "product.xml" ? "/bs:Package/" : "/bs:Product/");
			str = string.Concat(str, "bs:Strings/bs:String[@Name='DisplayName']");
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(filePath.FullName);
			return xmlDocument.SelectSingleNode(str, PackageInfo.GetXMLNamespaceManager(xmlDocument, "bs", "http://schemas.microsoft.com/developer/2004/01/bootstrapper")).InnerXml;
		}

		public string get_DisplayName(CultureInfo uiCulture, bool fallBack)
		{
			string str;
			string str1 = string.Concat(new string[] { this.m_RootPath.FullName, Path.DirectorySeparatorChar.ToString(), uiCulture.Name, Path.DirectorySeparatorChar.ToString(), "package.xml" });
			if (File.Exists(str1))
			{
				return PackageInfo.ExtractDisplayName(new FileInfo(str1));
			}
			try
			{
				CultureInfo cultureInfo = uiCulture;
				if (cultureInfo.Parent != null)
				{
					str1 = string.Concat(new string[] { this.m_RootPath.FullName, Path.DirectorySeparatorChar.ToString(), cultureInfo.Parent.Name, Path.DirectorySeparatorChar.ToString(), "package.xml" });
					if (File.Exists(str1))
					{
						str = PackageInfo.ExtractDisplayName(new FileInfo(str1));
						return str;
					}
					else if (fallBack)
					{
						string fullName = this.m_RootPath.GetDirectories()[0].FullName;
						char directorySeparatorChar = Path.DirectorySeparatorChar;
						str1 = string.Concat(fullName, directorySeparatorChar.ToString(), "package.xml");
						if (File.Exists(str1))
						{
							str = PackageInfo.ExtractDisplayName(new FileInfo(str1));
							return str;
						}
					}
				}
				throw new PackageInfo.Exceptions.PackageNotFoundException(string.Concat("A Package was not found in ", str1));
			}
			catch (Exception exception)
			{
				throw new PackageInfo.Exceptions.PackageNotFoundException(string.Concat("A Package was not found in ", str1), exception);
			}
			return str;
		}

		internal static XmlNamespaceManager GetXMLNamespaceManager(XmlDocument xmlDocument, string prefix, string uriNamespace)
		{
			XmlNamespaceManager xmlNamespaceManagers = new XmlNamespaceManager(xmlDocument.NameTable);
			xmlNamespaceManagers.AddNamespace(prefix, uriNamespace);
			return xmlNamespaceManagers;
		}

		private XmlDocument LoadProductXMLFile()
		{
			FileInfo files = this.m_RootPath.GetFiles("product.xml")[0];
			if (!files.Exists)
			{
				return null;
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(files.FullName);
			return xmlDocument;
		}

		public DirectoryInfo SearchForProductCode(string productCode)
		{
			DirectoryInfo directoryInfo = null;
			DirectoryInfo[] directories = Paths.Packages.GetDirectories();
			int num = 0;
			while (num < (int)directories.Length)
			{
				DirectoryInfo directoryInfo1 = directories[num];
				this.m_RootPath = directoryInfo1;
				if (this.ProductCode != productCode)
				{
					num++;
				}
				else
				{
					directoryInfo = directoryInfo1;
					break;
				}
			}
			this.m_RootPath = directoryInfo;
			return directoryInfo;
		}

		public override string ToString()
		{
			string fullName;
			try
			{
				return this.get_DisplayName(Thread.CurrentThread.CurrentUICulture, true);
			}
			catch (Exception exception)
			{
				fullName = this.Directory.FullName;
			}
			return fullName;
		}

		public class Exceptions
		{
			public Exceptions()
			{
			}

			[Serializable]
			public class PackageNotFoundException : Exception
			{
				public PackageNotFoundException(string message) : base(message)
				{
				}

				public PackageNotFoundException(string message, Exception innerException) : base(message, innerException)
				{
				}
			}
		}

		public class ManifestFileNames
		{
			public const string Package = "package.xml";

			public const string Product = "product.xml";

			public ManifestFileNames()
			{
			}
		}
	}
}