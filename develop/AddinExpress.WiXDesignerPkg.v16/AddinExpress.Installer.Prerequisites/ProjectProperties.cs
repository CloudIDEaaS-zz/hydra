using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace AddinExpress.Installer.Prerequisites
{
	internal class ProjectProperties
	{
		private ProjectProperties.BuildLocations m_buildlocation;

		private CultureInfo m_defaultLanguage;

		private string m_ProjectName = string.Empty;

		private string m_ProjectDir = string.Empty;

		private string m_sdkDir = string.Empty;

		private DirectoryInfo m_projectDirectory;

		private DirectoryInfo m_bootstrapperBuildDirectory;

		private DirectoryInfo m_bootstrapperRootDirectory;

		private const string ProjectNameAttribute = "ProjectName";

		public const string SaveFileElementName = "ProjectProperties";

		public DirectoryInfo BootstrapperBuildDirectory
		{
			get
			{
				return this.m_bootstrapperBuildDirectory;
			}
			private set
			{
				this.m_bootstrapperBuildDirectory = value;
			}
		}

		public DirectoryInfo BootstrapperRootDirectory
		{
			get
			{
				return this.m_bootstrapperRootDirectory;
			}
			private set
			{
				this.m_bootstrapperRootDirectory = value;
			}
		}

		public ProjectProperties.BuildLocations BuildLocation
		{
			get
			{
				return this.m_buildlocation;
			}
			set
			{
				this.m_buildlocation = value;
			}
		}

		public CultureInfo DefaultLangauge
		{
			get
			{
				return this.m_defaultLanguage;
			}
			set
			{
				this.m_defaultLanguage = value;
			}
		}

		public string ProjectDir
		{
			get
			{
				return this.m_ProjectDir;
			}
		}

		public DirectoryInfo ProjectDirectory
		{
			get
			{
				return this.m_projectDirectory;
			}
		}

		public string ProjectName
		{
			get
			{
				return this.m_ProjectName;
			}
		}

		public ProjectProperties(string projectName, string projectDir, string sdkDir)
		{
			this.m_ProjectName = projectName;
			this.m_ProjectDir = projectDir;
			this.m_sdkDir = sdkDir;
			this.m_projectDirectory = new DirectoryInfo(projectDir);
			if (!string.IsNullOrEmpty(this.m_sdkDir))
			{
				this.m_bootstrapperBuildDirectory = new DirectoryInfo(Path.Combine(this.m_sdkDir, "Packages"));
				this.m_bootstrapperRootDirectory = new DirectoryInfo(this.m_sdkDir);
			}
			else
			{
				this.m_bootstrapperBuildDirectory = new DirectoryInfo(Paths.Packages.FullName);
				this.m_bootstrapperRootDirectory = new DirectoryInfo(Paths.RootBootstrapper.FullName);
			}
			if (!Directory.Exists(this.m_bootstrapperBuildDirectory.FullName))
			{
				throw new DirectoryNotFoundException(string.Format("The '{0}' bootstrapper directory doesn't exist.", this.m_bootstrapperBuildDirectory.FullName));
			}
			string str = Path.Combine(this.m_bootstrapperBuildDirectory.FullName, "TestFile.DeleteMe");
			File.WriteAllText(str, "Test");
			File.Delete(str);
			this.m_buildlocation = ProjectProperties.BuildLocations.PackagesDir;
			this.SetDefaultCulture();
		}

		public void LoadFromFile(XmlElement nodeToLoadFrom)
		{
			if (nodeToLoadFrom.Attributes["BuildLocation"].Value != "PackagesDir")
			{
				this.BuildLocation = ProjectProperties.BuildLocations.SpecifiedDir;
			}
			else
			{
				this.BuildLocation = ProjectProperties.BuildLocations.PackagesDir;
			}
			string value = nodeToLoadFrom.Attributes["BuildDirectory"].Value;
			if (string.IsNullOrEmpty(value))
			{
				value = this.m_bootstrapperBuildDirectory.FullName;
			}
			else if (!Path.IsPathRooted(value))
			{
				value = Helpers.AbsolutePathFromRelative(value, this.m_ProjectDir);
			}
			if (string.IsNullOrEmpty(this.m_sdkDir))
			{
				this.BootstrapperBuildDirectory = new DirectoryInfo(value);
				this.BootstrapperRootDirectory = new DirectoryInfo(Path.GetFullPath(Path.Combine("..\\", value)));
			}
			if (nodeToLoadFrom.Attributes["DefaultLanguage"] == null)
			{
				this.SetDefaultCulture();
				return;
			}
			this.DefaultLangauge = new CultureInfo(nodeToLoadFrom.Attributes["DefaultLanguage"].Value);
		}

		public void SaveToFile(XmlElement nodeToAddTo)
		{
			XmlElement xmlElement = nodeToAddTo.OwnerDocument.CreateElement("ProjectProperties");
			XmlAttribute projectName = nodeToAddTo.OwnerDocument.CreateAttribute("ProjectName");
			projectName.Value = this.ProjectName;
			xmlElement.Attributes.Append(projectName);
			XmlAttribute xmlAttribute = nodeToAddTo.OwnerDocument.CreateAttribute("BuildLocation");
			if (this.BuildLocation != ProjectProperties.BuildLocations.PackagesDir)
			{
				xmlAttribute.Value = "SpecifiedDir";
			}
			else
			{
				xmlAttribute.Value = "PackagesDir";
			}
			xmlElement.Attributes.Append(xmlAttribute);
			XmlAttribute xmlAttribute1 = nodeToAddTo.OwnerDocument.CreateAttribute("BuildDirectory");
			xmlAttribute1.Value = Helpers.RelativizePathIfPossible(this.BootstrapperBuildDirectory.FullName, this.m_ProjectDir);
			xmlElement.Attributes.Append(xmlAttribute1);
			XmlAttribute name = nodeToAddTo.OwnerDocument.CreateAttribute("DefaultLanguage");
			name.Value = this.DefaultLangauge.Name;
			xmlElement.Attributes.Append(name);
			nodeToAddTo.AppendChild(xmlElement);
		}

		private void SetDefaultCulture()
		{
			if (Application.CurrentCulture.IsNeutralCulture)
			{
				this.m_defaultLanguage = Application.CurrentCulture;
				return;
			}
			this.m_defaultLanguage = Application.CurrentCulture.Parent;
		}

		public enum BuildLocations
		{
			PackagesDir,
			SpecifiedDir
		}

		private class BuildLocationStrings
		{
			public const string PackagesDir = "PackagesDir";

			public const string SpecifiedDir = "SpecifiedDir";

			public BuildLocationStrings()
			{
			}
		}
	}
}