using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WixSettings
	{
		protected WixSettings.WixData data = new WixSettings.WixData();

		public readonly static WixSettings Instance;

		public bool BackupChangedIncludes
		{
			get
			{
				return this.data.BackupChangedIncludes;
			}
			set
			{
				this.data.BackupChangedIncludes = value;
			}
		}

		public string ExternalXmlEditor
		{
			get
			{
				if (this.data.ExternalXmlEditor == null || this.data.ExternalXmlEditor.Length <= 0)
				{
					return string.Empty;
				}
				return this.data.ExternalXmlEditor;
			}
			set
			{
				this.data.ExternalXmlEditor = value;
			}
		}

		public ArrayList IgnoreFilesAndDirectories
		{
			get
			{
				if (this.data.IgnoreFilesAndDirectories == null)
				{
					this.data.IgnoreFilesAndDirectories = new ArrayList(new string[] { "*~", "#*#", ".#*", "%*%", "._*", "CVS", ".cvsignore", "SCCS", "vssver.scc", ".svn", ".DS_Store" });
				}
				return this.data.IgnoreFilesAndDirectories;
			}
			set
			{
				if (value == null)
				{
					this.data.IgnoreFilesAndDirectories = new ArrayList();
					return;
				}
				this.data.IgnoreFilesAndDirectories = value;
			}
		}

		public string TemplateDirectory
		{
			get
			{
				if (this.data.TemplateDirectory != null && this.data.TemplateDirectory.Length > 0)
				{
					return this.data.TemplateDirectory;
				}
				DirectoryInfo parent = (new FileInfo(Assembly.GetExecutingAssembly().Location)).Directory.Parent;
				if (parent != null)
				{
					string str = Path.Combine(parent.FullName, "wizard");
					if (Directory.Exists(str))
					{
						return str;
					}
				}
				return string.Empty;
			}
			set
			{
				this.data.TemplateDirectory = value;
			}
		}

		public BinDirectoryStructure WixBinariesDirectory
		{
			get
			{
				if (this.data.BinDirectory == null && this.data.CandleLocation == null && this.data.DarkLocation == null && this.data.HeatLocation == null && this.data.LightLocation == null && this.data.XsdsLocation == null)
				{
					List<DirectoryInfo> directoryInfos = new List<DirectoryInfo>();
					DirectoryInfo parent = (new FileInfo(Assembly.GetExecutingAssembly().Location)).Directory.Parent;
					if (parent != null)
					{
						directoryInfos.AddRange(parent.GetDirectories("wix*"));
						directoryInfos.AddRange(parent.GetDirectories("Windows Installer XML*"));
					}
					if (Environment.GetEnvironmentVariable("PROGRAMFILES(X86)") == null)
					{
						parent = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
						directoryInfos.AddRange(parent.GetDirectories("Windows Installer XML*"));
						directoryInfos.AddRange(parent.GetDirectories("wix*"));
					}
					else
					{
						parent = new DirectoryInfo(Environment.GetEnvironmentVariable("PROGRAMFILES(X86)"));
						directoryInfos.AddRange(parent.GetDirectories("Windows Installer XML*"));
						directoryInfos.AddRange(parent.GetDirectories("wix*"));
						parent = new DirectoryInfo(Environment.GetEnvironmentVariable("PROGRAMFILES"));
						directoryInfos.AddRange(parent.GetDirectories("Windows Installer XML*"));
						directoryInfos.AddRange(parent.GetDirectories("wix*"));
					}
					parent = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Programs));
					directoryInfos.AddRange(parent.GetDirectories("Windows Installer XML*"));
					directoryInfos.AddRange(parent.GetDirectories("wix*"));
					foreach (DirectoryInfo directoryInfo in directoryInfos)
					{
						FileInfo[] files = directoryInfo.GetFiles("*.exe", SearchOption.AllDirectories);
						int num = 0;
						while (num < (int)files.Length)
						{
							FileInfo fileInfo = files[num];
							if (!fileInfo.Name.ToLower().Equals("candle.exe"))
							{
								num++;
							}
							else
							{
								this.data.BinDirectory = fileInfo.Directory.FullName;
								break;
							}
						}
						if (this.data.BinDirectory == null)
						{
							continue;
						}
						return new BinDirectoryStructure(this.data);
					}
				}
				return new BinDirectoryStructure(this.data);
			}
			set
			{
				if (value.HasSameBinDirectory())
				{
					this.data.BinDirectory = (new FileInfo(value.Candle)).Directory.FullName;
					return;
				}
				this.data.CandleLocation = value.Candle;
				this.data.LightLocation = value.Light;
				this.data.DarkLocation = value.Dark;
				this.data.HeatLocation = value.Heat;
				this.data.XsdsLocation = value.Xsds;
				this.data.BinDirectory = value.BinDirectory;
			}
		}

		public string WixBinariesVersion
		{
			get
			{
				BinDirectoryStructure wixBinariesDirectory = this.WixBinariesDirectory;
				if (!File.Exists(wixBinariesDirectory.Candle) || !File.Exists(wixBinariesDirectory.Light) || !File.Exists(wixBinariesDirectory.Heat) || !File.Exists(wixBinariesDirectory.Dark))
				{
					return "(Not all files present)";
				}
				bool flag = true;
				bool flag1 = true;
				bool flag2 = true;
				bool flag3 = true;
				FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(wixBinariesDirectory.Candle);
				int productMajorPart = versionInfo.ProductMajorPart;
				int productMinorPart = versionInfo.ProductMinorPart;
				int productBuildPart = versionInfo.ProductBuildPart;
				int productPrivatePart = versionInfo.ProductPrivatePart;
				string[] light = new string[] { wixBinariesDirectory.Light, wixBinariesDirectory.Dark, wixBinariesDirectory.Heat };
				for (int i = 0; i < (int)light.Length; i++)
				{
					versionInfo = FileVersionInfo.GetVersionInfo(light[i]);
					if (productMajorPart != versionInfo.ProductMajorPart)
					{
						flag = false;
					}
					if (productMinorPart != versionInfo.ProductMinorPart)
					{
						flag1 = false;
					}
					if (productBuildPart != versionInfo.ProductBuildPart)
					{
						flag2 = false;
					}
					if (productPrivatePart != versionInfo.ProductPrivatePart)
					{
						flag3 = false;
					}
				}
				if (!flag)
				{
					return "WARNING: Using mixed up versions of WiX!";
				}
				if (!flag1)
				{
					return string.Format("{0}.* (Minor part of versions differ)", productMajorPart);
				}
				if (!flag2)
				{
					return string.Format("{0}.{1}.* (Build part of versions differ)", productMajorPart, productMinorPart);
				}
				if (!flag3)
				{
					return string.Format("{0}.{1}.{2}.*", productMajorPart, productMinorPart, productBuildPart);
				}
				return string.Format("{0}.{1}.{2}.{3}", new object[] { productMajorPart, productMinorPart, productBuildPart, productPrivatePart });
			}
		}

		public FileVersionInfo WixBinariesVersionInfo
		{
			get
			{
				BinDirectoryStructure wixBinariesDirectory = this.WixBinariesDirectory;
				if (!File.Exists(wixBinariesDirectory.Candle) || !File.Exists(wixBinariesDirectory.Light) || !File.Exists(wixBinariesDirectory.Heat) || !File.Exists(wixBinariesDirectory.Dark))
				{
					return null;
				}
				return FileVersionInfo.GetVersionInfo(wixBinariesDirectory.Heat);
			}
		}

		public int XmlIndentation
		{
			get
			{
				return Math.Max(0, this.data.XmlIndentation);
			}
			set
			{
				this.data.XmlIndentation = Math.Max(0, value);
			}
		}

		static WixSettings()
		{
			WixSettings.Instance = new WixSettings();
		}

		private WixSettings()
		{
		}

		public WixSettings.WixData GetInternalDataStructure()
		{
			return this.data;
		}

		public string GetWixXsdLocation()
		{
			return Path.Combine(this.WixBinariesDirectory.Xsds, "wix.xsd");
		}

		public bool IsUsingWix2()
		{
			return this.WixBinariesVersion.StartsWith("2");
		}

		public bool IsUsingWix3()
		{
			return this.WixBinariesVersion.StartsWith("3");
		}

		public bool IsUsingWix4()
		{
			return this.WixBinariesVersion.StartsWith("4");
		}

		public bool IsWixVersionOk()
		{
			FileVersionInfo wixBinariesVersionInfo = this.WixBinariesVersionInfo;
			if (wixBinariesVersionInfo != null && wixBinariesVersionInfo.FileMajorPart >= 3 && (wixBinariesVersionInfo.FileMajorPart != 3 || wixBinariesVersionInfo.FileMinorPart >= 6))
			{
				return true;
			}
			return false;
		}

		public class WixData
		{
			public string BinDirectory;

			public string DarkLocation;

			public string HeatLocation;

			public string CandleLocation;

			public string LightLocation;

			public string XsdsLocation;

			public string TemplateDirectory;

			public string ExternalXmlEditor;

			public bool UseInstanceOnly;

			public bool WordWrapInResultsPanel;

			public string Version;

			public PathHandling UseRelativeOrAbsolutePaths;

			public string[] RecentOpenedFiles;

			public bool DisplayFullPathInTitlebar;

			public int XmlIndentation;

			public IncludeChangesHandling AllowIncludeChanges;

			public bool BackupChangedIncludes;

			public ArrayList IgnoreFilesAndDirectories;

			public WixData()
			{
				this.UseRelativeOrAbsolutePaths = PathHandling.UseRelativePathsWhenPossible;
				this.ExternalXmlEditor = Path.Combine(Environment.SystemDirectory, "notepad.exe");
				this.UseInstanceOnly = false;
				this.WordWrapInResultsPanel = false;
				this.RecentOpenedFiles = new string[0];
				this.DisplayFullPathInTitlebar = false;
				this.XmlIndentation = 4;
				this.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
				this.AllowIncludeChanges = IncludeChangesHandling.AskForEachFile;
				this.BackupChangedIncludes = true;
			}
		}
	}
}