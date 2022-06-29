using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;

namespace AddinExpress.Installer.Prerequisites.Manifests
{
	internal class PackageFile : Element
	{
		public string FileHash = string.Empty;

		public string Homesite = string.Empty;

		public string PublicKey = string.Empty;

		public string RelativePathAndFileName = string.Empty;

		public string SourcePathandFileName = string.Empty;

		public bool CopyOnBuild = true;

		public PackageFile(string sourceFile)
		{
			this.SourcePathandFileName = sourceFile;
		}

		public PackageFile(string sourceFile, string homeSite, string publicKey)
		{
			this.SourcePathandFileName = sourceFile;
			this.Homesite = homeSite;
			this.PublicKey = publicKey;
		}

		private static string ConvertByteArrayToString(byte[] byteArray)
		{
			if (byteArray == null)
			{
				return "";
			}
			StringBuilder stringBuilder = new StringBuilder((int)byteArray.Length);
			byte[] numArray = byteArray;
			for (int i = 0; i < (int)numArray.Length; i++)
			{
				byte num = numArray[i];
				stringBuilder.Append(num.ToString("X02"));
			}
			return stringBuilder.ToString();
		}

		public override XmlElement LoadFromXML(XmlElement workingElement)
		{
			throw new NotImplementedException("NYI");
		}

		private static string ReadHash(FileInfo file)
		{
			Stream stream = file.OpenRead();
			string str = PackageFile.ConvertByteArrayToString((new SHA256Managed()).ComputeHash(stream));
			stream.Close();
			return str;
		}

		public static string ReadHashFromFile(FileInfo file)
		{
			return PackageFile.ReadHash(file);
		}

		public void ReadHashFromSourceFile()
		{
			this.FileHash = null;
			this.FileHash = PackageFile.ReadHash(new FileInfo(this.SourcePathandFileName));
		}

		private static string ReadPublicKey(FileInfo file)
		{
			X509Certificate x509Certificate;
			try
			{
				x509Certificate = new X509Certificate(file.FullName);
			}
			catch (Exception exception)
			{
				throw new PackageFile.FileHasNoCertificateException(exception.Message);
			}
			return x509Certificate.GetPublicKeyString();
		}

		public static string ReadPublicKeyFromFile(FileInfo file)
		{
			return PackageFile.ReadPublicKey(file);
		}

		public void ReadPublicKeyFromSourceFile()
		{
			this.PublicKey = null;
			this.PublicKey = PackageFile.ReadPublicKey(new FileInfo(this.SourcePathandFileName));
		}

		public override void WriteXML(XmlElement parentElement)
		{
			string lowerInvariant;
			Element.BuildMessage(string.Concat("   PackageFile: ", this.SourcePathandFileName), BootstrapperProduct.BuildErrorLevel.None);
			XmlElement xmlElement = base.AddElement(parentElement, "PackageFile");
			lowerInvariant = (string.IsNullOrEmpty(this.RelativePathAndFileName) ? Path.GetFileName(this.SourcePathandFileName) : this.RelativePathAndFileName);
			lowerInvariant = lowerInvariant.ToLowerInvariant();
			base.AddAttribute(xmlElement, "Name", lowerInvariant);
			if (!string.IsNullOrEmpty(this.Homesite))
			{
				base.AddAttribute(xmlElement, "HomeSite", this.Homesite);
			}
			if (!this.CopyOnBuild)
			{
				base.AddAttribute(xmlElement, "CopyOnBuild", "false");
			}
			if (!string.IsNullOrEmpty(this.PublicKey))
			{
				base.AddAttribute(xmlElement, "PublicKey", this.PublicKey);
			}
			if (!string.IsNullOrEmpty(this.FileHash))
			{
				base.AddAttribute(xmlElement, "Hash", this.FileHash);
			}
		}

		public class FileHasNoCertificateException : Exception
		{
			public FileHasNoCertificateException(string message) : base(message)
			{
			}
		}

		private class XMLStrings
		{
			public const string ElementName = "PackageFile";

			public const string HashAttribute = "Hash";

			public const string HomeSiteAttribute = "HomeSite";

			public const string NameAttribute = "Name";

			public const string PublicKeyAttribute = "PublicKey";

			public const string CopyOnBuildAttribute = "CopyOnBuild";

			public XMLStrings()
			{
			}
		}
	}
}