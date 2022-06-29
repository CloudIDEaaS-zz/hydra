using AddinExpress.Installer.Prerequisites;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Schema;

namespace AddinExpress.Installer.Prerequisites.Manifests
{
	internal class BootstrapperProduct
	{
		private static List<WeakReference> __ENCList;

		private string m_CurrentValidatingFileName;

		private bool m_ValidationErrorsOccurred;

		private StringCollection m_ValidationResults;

		public List<Package> packages;

		private const string PackagesPathRegValue = "PackagePath";

		public Product primaryProduct;

		static BootstrapperProduct()
		{
			BootstrapperProduct.__ENCList = new List<WeakReference>();
		}

		public BootstrapperProduct()
		{
			lock (BootstrapperProduct.__ENCList)
			{
				BootstrapperProduct.__ENCList.Add(new WeakReference(this));
			}
			this.packages = new List<Package>();
			Globals.localizedStrings.Clear();
		}

		public void AddLocalizedString(LocString str)
		{
			Globals.localizedStrings.Add(str);
		}

		public void Build(DirectoryInfo outputPath, bool validate)
		{
			string fullName;
			try
			{
				this.m_ValidationResults = new StringCollection();
				this.m_ValidationErrorsOccurred = false;
				if (!outputPath.Exists)
				{
					outputPath.Create();
				}
				Globals.manifestWorkingFile = new XmlDocument();
				XmlDeclaration xmlDeclaration = Globals.manifestWorkingFile.CreateXmlDeclaration("1.0", "utf-8", string.Empty);
				Globals.manifestWorkingFile.AppendChild(xmlDeclaration);
				this.primaryProduct.WriteXML(null);
				XmlDocument xmlDocument = Globals.manifestWorkingFile;
				string str = outputPath.FullName;
				char directorySeparatorChar = Path.DirectorySeparatorChar;
				xmlDocument.Save(string.Concat(str, directorySeparatorChar.ToString(), "product.xml"));
				if (validate)
				{
					string fullName1 = outputPath.FullName;
					directorySeparatorChar = Path.DirectorySeparatorChar;
					BootstrapperProduct.RaiseBuildMessage(string.Concat("Validating ", fullName1, directorySeparatorChar.ToString(), "product.xml"), BootstrapperProduct.BuildErrorLevel.None);
					try
					{
						string str1 = outputPath.FullName;
						directorySeparatorChar = Path.DirectorySeparatorChar;
						this.ValidateManifest(new FileInfo(string.Concat(str1, directorySeparatorChar.ToString(), "product.xml")));
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						BootstrapperProduct.RaiseBuildMessage(string.Concat("Validation Was Unable to Execute: ", exception.ToString()), BootstrapperProduct.BuildErrorLevel.Warning);
					}
				}
				foreach (PackageFile file in this.primaryProduct.PackageFiles.Files)
				{
					if (!file.CopyOnBuild)
					{
						continue;
					}
					BootstrapperProduct.RaiseBuildMessage(string.Concat("  Copying File: ", file.SourcePathandFileName, " ..."), BootstrapperProduct.BuildErrorLevel.None);
					fullName = outputPath.FullName;
					fullName = (!string.IsNullOrEmpty(file.RelativePathAndFileName) ? Path.Combine(fullName, file.RelativePathAndFileName) : Path.Combine(fullName, Path.GetFileName(file.SourcePathandFileName)));
					if (!Directory.Exists(Path.GetDirectoryName(fullName)))
					{
						Directory.CreateDirectory(Path.GetDirectoryName(fullName));
					}
					File.Copy(file.SourcePathandFileName, fullName, true);
				}
				foreach (Package package in this.packages)
				{
					List<PackageFile>.Enumerator enumerator = new List<PackageFile>.Enumerator();
					Globals.manifestWorkingFile = null;
					Globals.manifestWorkingFile = new XmlDocument();
					xmlDeclaration = Globals.manifestWorkingFile.CreateXmlDeclaration("1.0", "utf-8", string.Empty);
					Globals.manifestWorkingFile.AppendChild(xmlDeclaration);
					package.WriteXML(null);
					string fullName2 = outputPath.FullName;
					string str2 = Path.DirectorySeparatorChar.ToString();
					string name = package.Culture.Name;
					directorySeparatorChar = Path.DirectorySeparatorChar;
					string str3 = string.Concat(fullName2, str2, name, directorySeparatorChar.ToString());
					Directory.CreateDirectory(str3);
					Globals.manifestWorkingFile.Save(string.Concat(str3, "package.xml"));
					if (validate)
					{
						BootstrapperProduct.RaiseBuildMessage(string.Concat("Validating ", str3, "package.xml"), BootstrapperProduct.BuildErrorLevel.None);
						try
						{
							this.ValidateManifest(new FileInfo(string.Concat(str3, "package.xml")));
						}
						catch (Exception exception3)
						{
							Exception exception2 = exception3;
							BootstrapperProduct.RaiseBuildMessage(string.Concat("Validation Was Unable to Execute: ", exception2.ToString()), BootstrapperProduct.BuildErrorLevel.Warning);
						}
					}
					try
					{
						enumerator = package.PackageFiles.Files.GetEnumerator();
						while (enumerator.MoveNext())
						{
							PackageFile current = enumerator.Current;
							if (!current.CopyOnBuild)
							{
								continue;
							}
							BootstrapperProduct.RaiseBuildMessage(string.Concat("   Copying File: ", current.SourcePathandFileName, " ..."), BootstrapperProduct.BuildErrorLevel.None);
							fullName = str3;
							fullName = (!string.IsNullOrEmpty(current.RelativePathAndFileName) ? Path.Combine(fullName, current.RelativePathAndFileName) : Path.Combine(fullName, Path.GetFileName(current.SourcePathandFileName)));
							if (!Directory.Exists(Path.GetDirectoryName(fullName)))
							{
								Directory.CreateDirectory(Path.GetDirectoryName(fullName));
							}
							File.Copy(current.SourcePathandFileName, fullName, true);
						}
					}
					finally
					{
						enumerator.Dispose();
					}
				}
				if (this.m_ValidationResults.Count > 0)
				{
					StringBuilder stringBuilder = new StringBuilder();
					if (!this.m_ValidationErrorsOccurred)
					{
						stringBuilder.AppendLine("There were validation errors.");
					}
					else
					{
						stringBuilder.AppendLine("There were validation warnings.");
					}
					StringEnumerator stringEnumerator = this.m_ValidationResults.GetEnumerator();
					while (stringEnumerator.MoveNext())
					{
						stringBuilder.AppendLine(string.Concat("\t", stringEnumerator.Current));
					}
					BootstrapperProduct.RaiseBuildMessage(stringBuilder.ToString(), BootstrapperProduct.BuildErrorLevel.Warning);
				}
			}
			catch (Exception exception5)
			{
				Exception exception4 = exception5;
				throw new Exceptions.BuildErrorsException(string.Concat("Build Failed: ", exception4.Message), this.m_ValidationResults, exception4);
			}
		}

		public LocString GetLocalizedString(CultureInfo culture, string name)
		{
			LocString locString;
			List<LocString>.Enumerator enumerator = Globals.localizedStrings.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					LocString current = enumerator.Current;
					if (!current.Culture.Equals(culture) || !(current.Name.ToLower() == name.ToLower()))
					{
						continue;
					}
					locString = current;
					return locString;
				}
				return null;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return locString;
		}

		public Package GetPackage(CultureInfo culture, LocString displayName)
		{
			Package package;
			List<Package>.Enumerator enumerator = this.packages.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Package current = enumerator.Current;
					if (!current.Culture.Equals(culture))
					{
						continue;
					}
					package = current;
					return package;
				}
				Package package1 = new Package(culture, displayName);
				this.packages.Add(package1);
				return package1;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return package;
		}

		public static void RaiseBuildMessage(string message, BootstrapperProduct.BuildErrorLevel ErrorLevel)
		{
			BootstrapperProduct.BuildMessageEventHandler buildMessageEventHandler = BootstrapperProduct.BuildMessage;
			if (buildMessageEventHandler != null)
			{
				buildMessageEventHandler(message, ErrorLevel);
			}
		}

		public void SchemaValidationEventHandler(object sender, ValidationEventArgs e)
		{
			string str;
			if (e.Severity != XmlSeverityType.Warning)
			{
				str = "Error";
				this.m_ValidationErrorsOccurred = true;
			}
			else
			{
				str = "Warning";
			}
			this.m_ValidationResults.Add(string.Concat("Schema Validation ", str, ": ", e.Message));
		}

		private StringCollection ValidateManifest(FileInfo manifest)
		{
			StringCollection stringCollections = new StringCollection();
			XmlReader xmlReader = null;
			try
			{
				try
				{
					this.m_CurrentValidatingFileName = manifest.FullName;
					XmlReaderSettings xmlReaderSetting = new XmlReaderSettings()
					{
						ValidationType = ValidationType.Schema
					};
					xmlReaderSetting.Schemas.Add("http://schemas.microsoft.com/developer/2004/01/bootstrapper", Paths.Schemas.GetFiles("package.xsd")[0].FullName);
					xmlReaderSetting.ValidationEventHandler += new ValidationEventHandler(this.SchemaValidationEventHandler);
					xmlReader = XmlReader.Create(manifest.FullName, xmlReaderSetting);
					while (xmlReader.Read())
					{
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					this.m_ValidationErrorsOccurred = true;
					this.m_ValidationResults.Add(exception.Message);
				}
			}
			finally
			{
				if (xmlReader != null)
				{
					xmlReader.Close();
				}
			}
			return stringCollections;
		}

		public static event BootstrapperProduct.BuildMessageEventHandler BuildMessage;

		public enum BuildErrorLevel
		{
			None,
			Warning,
			BuildError
		}

		public delegate void BuildMessageEventHandler(string message, BootstrapperProduct.BuildErrorLevel ErrorLevel);
	}
}