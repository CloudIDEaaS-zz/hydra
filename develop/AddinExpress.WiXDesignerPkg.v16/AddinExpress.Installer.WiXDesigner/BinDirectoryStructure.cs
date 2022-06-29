using System;
using System.IO;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class BinDirectoryStructure
	{
		private WixSettings.WixData wixData;

		public string BinDirectory
		{
			get
			{
				return this.wixData.BinDirectory;
			}
			set
			{
				this.wixData.BinDirectory = value;
			}
		}

		public string Candle
		{
			get
			{
				if (this.wixData.CandleLocation != null && this.wixData.CandleLocation.Length != 0)
				{
					return this.wixData.CandleLocation;
				}
				if (this.wixData.BinDirectory == null || this.wixData.BinDirectory.Length == 0)
				{
					return string.Empty;
				}
				return Path.Combine(this.wixData.BinDirectory, "candle.exe");
			}
			set
			{
				this.wixData.CandleLocation = value;
			}
		}

		public string Dark
		{
			get
			{
				if (this.wixData.DarkLocation != null && this.wixData.DarkLocation.Length != 0)
				{
					return this.wixData.DarkLocation;
				}
				if (this.wixData.BinDirectory == null || this.wixData.BinDirectory.Length == 0)
				{
					return string.Empty;
				}
				return Path.Combine(this.wixData.BinDirectory, "dark.exe");
			}
			set
			{
				this.wixData.DarkLocation = value;
			}
		}

		public string Heat
		{
			get
			{
				if (this.wixData.HeatLocation != null && this.wixData.HeatLocation.Length != 0)
				{
					return this.wixData.HeatLocation;
				}
				if (this.wixData.BinDirectory == null || this.wixData.BinDirectory.Length == 0)
				{
					return string.Empty;
				}
				return Path.Combine(this.wixData.BinDirectory, "heat.exe");
			}
			set
			{
				this.wixData.HeatLocation = value;
			}
		}

		public string Light
		{
			get
			{
				if (this.wixData.LightLocation != null && this.wixData.LightLocation.Length != 0)
				{
					return this.wixData.LightLocation;
				}
				if (this.wixData.BinDirectory == null || this.wixData.BinDirectory.Length == 0)
				{
					return string.Empty;
				}
				return Path.Combine(this.wixData.BinDirectory, "light.exe");
			}
			set
			{
				this.wixData.LightLocation = value;
			}
		}

		public string Xsds
		{
			get
			{
				if (this.wixData.XsdsLocation != null && this.wixData.XsdsLocation.Length != 0)
				{
					return this.wixData.XsdsLocation;
				}
				if (this.wixData.BinDirectory == null || this.wixData.BinDirectory.Length == 0)
				{
					return string.Empty;
				}
				DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(this.wixData.BinDirectory, "doc"));
				DirectoryInfo directoryInfo1 = new DirectoryInfo(Path.Combine(directoryInfo.Parent.Parent.FullName, "doc"));
				string fullName = directoryInfo.FullName;
				if (directoryInfo1.Exists)
				{
					fullName = directoryInfo1.FullName;
				}
				return fullName;
			}
			set
			{
				this.wixData.XsdsLocation = value;
			}
		}

		public BinDirectoryStructure(WixSettings.WixData data)
		{
			this.wixData = data;
		}

		public bool HasSameBinDirectory()
		{
			if (this.wixData.CandleLocation == null && this.wixData.DarkLocation == null && this.wixData.HeatLocation == null && this.wixData.LightLocation == null && this.wixData.XsdsLocation == null)
			{
				return true;
			}
			if (this.Candle == null || this.Dark == null || this.Heat == null || this.Light == null || this.Xsds == null)
			{
				return false;
			}
			if (!((new FileInfo(this.Heat)).Directory.FullName == (new FileInfo(this.Candle)).Directory.FullName) || !((new FileInfo(this.Heat)).Directory.FullName == (new FileInfo(this.Dark)).Directory.FullName) || !((new FileInfo(this.Heat)).Directory.FullName == (new FileInfo(this.Light)).Directory.FullName))
			{
				return false;
			}
			return this.Xsds.StartsWith((new FileInfo(this.Heat)).Directory.FullName);
		}
	}
}