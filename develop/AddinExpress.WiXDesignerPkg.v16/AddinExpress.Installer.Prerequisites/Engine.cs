using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace AddinExpress.Installer.Prerequisites
{
	internal class Engine
	{
		public List<CultureInfo> InstalledCultures
		{
			get
			{
				List<CultureInfo> cultureInfos = new List<CultureInfo>();
				DirectoryInfo[] directories = Paths.Binaries.GetDirectories();
				for (int i = 0; i < (int)directories.Length; i++)
				{
					cultureInfos.Add(new CultureInfo(directories[i].Name));
				}
				return cultureInfos;
			}
		}

		public Engine()
		{
		}
	}
}