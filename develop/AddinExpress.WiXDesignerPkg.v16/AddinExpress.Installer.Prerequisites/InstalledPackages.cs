using System;
using System.Collections.Generic;
using System.IO;

namespace AddinExpress.Installer.Prerequisites
{
	internal class InstalledPackages
	{
		public InstalledPackages()
		{
		}

		public List<PackageInfo> FindPackages()
		{
			List<PackageInfo> packageInfos = new List<PackageInfo>();
			DirectoryInfo[] directories = Paths.Packages.GetDirectories();
			for (int i = 0; i < (int)directories.Length; i++)
			{
				packageInfos.Add(new PackageInfo(directories[i]));
			}
			return packageInfos;
		}
	}
}