using Microsoft.Win32;
using System;
using System.IO;

namespace AddinExpress.Installer.Prerequisites
{
	internal class Paths
	{
		private static string rootBootstrapperDir;

		public static DirectoryInfo Binaries
		{
			get
			{
				return new DirectoryInfo(Path.Combine(Paths.RootBootstrapper.FullName, "Engine"));
			}
		}

		public static DirectoryInfo Packages
		{
			get
			{
				return new DirectoryInfo(Path.Combine(Paths.RootBootstrapper.FullName, "Packages"));
			}
		}

		public static DirectoryInfo RootBootstrapper
		{
			get
			{
				return new DirectoryInfo(Paths.rootBootstrapperDir);
			}
		}

		public static DirectoryInfo Schemas
		{
			get
			{
				return new DirectoryInfo(Path.Combine(Paths.RootBootstrapper.FullName, "Schemas"));
			}
		}

		static Paths()
		{
			Paths.rootBootstrapperDir = string.Empty;
		}

		public Paths()
		{
		}

		private static string GetGenericBootstrapperDirectory(string vsVersion)
		{
			string str = "2.0";
			string empty = string.Empty;
			if (vsVersion != null)
			{
				if (vsVersion == "9.0")
				{
					str = "3.5";
				}
				else if (vsVersion == "10.0")
				{
					str = "4.0";
				}
				else if (vsVersion == "11.0")
				{
					str = "11.0";
				}
				else if (vsVersion == "12.0")
				{
					str = "12.0";
				}
				else if (vsVersion == "14.0")
				{
					str = "14.0";
				}
			}
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(string.Concat("Software\\Microsoft\\GenericBootstrapper\\", str), false))
			{
				if (registryKey != null)
				{
					empty = (string)registryKey.GetValue("Path", string.Empty);
					if (!string.IsNullOrEmpty(empty) && Directory.Exists(empty) && Directory.GetDirectories(empty).Length == 0)
					{
						empty = string.Empty;
					}
				}
			}
			return empty;
		}

		private static string GetSDKRootDirectory(string sdkVersion)
		{
			string empty = string.Empty;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(string.Concat("SOFTWARE\\Microsoft\\Microsoft SDKs\\Windows\\", sdkVersion), false))
			{
				if (registryKey != null)
				{
					empty = (string)registryKey.GetValue("InstallationFolder", string.Empty);
					if (!string.IsNullOrEmpty(empty))
					{
						if (!Directory.Exists(Path.Combine(empty, "BootStrapper")))
						{
							empty = string.Empty;
							goto Label0;
						}
						else if (Directory.GetDirectories(Path.Combine(empty, "BootStrapper")).Length == 0)
						{
							empty = string.Empty;
						}
					}
				}
			}
		Label0:
			if (string.IsNullOrEmpty(empty))
			{
				using (RegistryKey registryKey1 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Microsoft SDKs\\Windows", false))
				{
					if (registryKey1 != null)
					{
						empty = (string)registryKey1.GetValue("CurrentInstallFolder", string.Empty);
						if (!string.IsNullOrEmpty(empty))
						{
							if (!Directory.Exists(Path.Combine(empty, "BootStrapper")))
							{
								empty = string.Empty;
								if (string.IsNullOrEmpty(empty))
								{
									using (RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\GenericBootstrapper\\3.5", false))
									{
										if (registryKey2 != null)
										{
											empty = (string)registryKey2.GetValue("Path", string.Empty);
											if (!string.IsNullOrEmpty(empty))
											{
												empty = Path.GetFullPath(Path.Combine(empty, "..\\"));
											}
										}
									}
								}
								return empty;
							}
							else if (Directory.GetDirectories(Path.Combine(empty, "BootStrapper")).Length == 0)
							{
								empty = string.Empty;
							}
						}
					}
				}
			}
			if (string.IsNullOrEmpty(empty))
			{
				using (registryKey2 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\GenericBootstrapper\\3.5", false))
				{
					if (registryKey2 != null)
					{
						empty = (string)registryKey2.GetValue("Path", string.Empty);
						if (!string.IsNullOrEmpty(empty))
						{
							empty = Path.GetFullPath(Path.Combine(empty, "..\\"));
						}
					}
				}
			}
			return empty;
		}

		private static string GetVSSDKRootDirectory(string vsVersion)
		{
			string empty = string.Empty;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(string.Concat("SOFTWARE\\Microsoft\\VisualStudio\\", vsVersion), false))
			{
				if (registryKey != null)
				{
					empty = (string)registryKey.GetValue("ShellFolder", string.Empty);
					if (string.IsNullOrEmpty(empty))
					{
						empty = (string)registryKey.GetValue("InstallDir", string.Empty);
						if (!string.IsNullOrEmpty(empty))
						{
							empty = Path.GetFullPath(Path.Combine(empty, "..\\..\\SDK"));
						}
					}
					else
					{
						empty = Path.Combine(empty, "SDK");
					}
					if (!string.IsNullOrEmpty(empty))
					{
						string str = Path.Combine(empty, "BootStrapper");
						if (!Directory.Exists(str))
						{
							empty = string.Empty;
							return empty;
						}
						else if (Directory.GetDirectories(str).Length == 0)
						{
							empty = string.Empty;
						}
					}
				}
			}
			return empty;
		}

		public static void Init(string dteVersion)
		{
			if (string.IsNullOrEmpty(Paths.rootBootstrapperDir))
			{
				string fileName = Path.GetFileName(dteVersion);
				Paths.rootBootstrapperDir = Paths.GetGenericBootstrapperDirectory(fileName);
				if (string.IsNullOrEmpty(Paths.rootBootstrapperDir))
				{
					Paths.rootBootstrapperDir = Paths.GetVSSDKRootDirectory(fileName);
					if (string.IsNullOrEmpty(Paths.rootBootstrapperDir))
					{
						if (fileName == "9.0")
						{
							Paths.rootBootstrapperDir = Paths.GetSDKRootDirectory("v6.0A");
							if (string.IsNullOrEmpty(Paths.rootBootstrapperDir))
							{
								Paths.rootBootstrapperDir = Paths.GetSDKRootDirectory("v7.0A");
							}
						}
						else if (fileName == "10.0")
						{
							Paths.rootBootstrapperDir = Paths.GetSDKRootDirectory("v7.0A");
						}
						else if (fileName == "11.0")
						{
							Paths.rootBootstrapperDir = Paths.GetSDKRootDirectory("v8.0A");
							if (string.IsNullOrEmpty(Paths.rootBootstrapperDir))
							{
								Paths.rootBootstrapperDir = Paths.GetSDKRootDirectory("v7.1A");
							}
							if (string.IsNullOrEmpty(Paths.rootBootstrapperDir))
							{
								Paths.rootBootstrapperDir = Paths.GetSDKRootDirectory("v7.0A");
							}
						}
						else if (fileName == "12.0")
						{
							Paths.rootBootstrapperDir = Paths.GetSDKRootDirectory("v8.1A");
							if (string.IsNullOrEmpty(Paths.rootBootstrapperDir))
							{
								Paths.rootBootstrapperDir = Paths.GetSDKRootDirectory("v8.0A");
							}
							if (string.IsNullOrEmpty(Paths.rootBootstrapperDir))
							{
								Paths.rootBootstrapperDir = Paths.GetSDKRootDirectory("v7.1A");
							}
							if (string.IsNullOrEmpty(Paths.rootBootstrapperDir))
							{
								Paths.rootBootstrapperDir = Paths.GetSDKRootDirectory("v7.0A");
							}
						}
						else if (fileName != "14.0")
						{
							using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\.NETFramework", false))
							{
								if (registryKey != null)
								{
									Paths.rootBootstrapperDir = (string)registryKey.GetValue("sdkInstallRootv2.0", string.Empty);
								}
							}
						}
						else
						{
							Paths.rootBootstrapperDir = Paths.GetSDKRootDirectory("v10.0A");
							if (string.IsNullOrEmpty(Paths.rootBootstrapperDir))
							{
								Paths.rootBootstrapperDir = Paths.GetSDKRootDirectory("v8.1A");
							}
							if (string.IsNullOrEmpty(Paths.rootBootstrapperDir))
							{
								Paths.rootBootstrapperDir = Paths.GetSDKRootDirectory("v8.0A");
							}
							if (string.IsNullOrEmpty(Paths.rootBootstrapperDir))
							{
								Paths.rootBootstrapperDir = Paths.GetSDKRootDirectory("v7.1A");
							}
							if (string.IsNullOrEmpty(Paths.rootBootstrapperDir))
							{
								Paths.rootBootstrapperDir = Paths.GetSDKRootDirectory("v7.0A");
							}
						}
					}
					if (!string.IsNullOrEmpty(Paths.rootBootstrapperDir))
					{
						Paths.rootBootstrapperDir = Path.Combine(Paths.rootBootstrapperDir, "BootStrapper");
					}
				}
			}
		}
	}
}