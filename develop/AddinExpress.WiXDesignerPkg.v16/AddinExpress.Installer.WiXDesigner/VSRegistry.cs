using System;
using System.Collections.Generic;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSRegistry
	{
		private WiXProjectParser _project;

		private VSRegistryKeys _HKCR;

		private VSRegistryKeys _HKCU;

		private VSRegistryKeys _HKLM;

		private VSRegistryKeys _HKU;

		private VSRegistryKeys _HKMU;

		internal VSRegistryKeys HKCR
		{
			get
			{
				return this._HKCR;
			}
		}

		internal VSRegistryKeys HKCU
		{
			get
			{
				return this._HKCU;
			}
		}

		internal VSRegistryKeys HKLM
		{
			get
			{
				return this._HKLM;
			}
		}

		internal VSRegistryKeys HKMU
		{
			get
			{
				return this._HKMU;
			}
		}

		internal VSRegistryKeys HKU
		{
			get
			{
				return this._HKU;
			}
		}

		public VSRegistry(WiXProjectParser project)
		{
			this._project = project;
			this._HKCR = new VSRegistryKeys(this._project, null);
			this._HKCU = new VSRegistryKeys(this._project, null);
			this._HKLM = new VSRegistryKeys(this._project, null);
			this._HKU = new VSRegistryKeys(this._project, null);
			this._HKMU = new VSRegistryKeys(this._project, null);
		}

		internal void Clean()
		{
			this._HKCR.Clear();
			this._HKCU.Clear();
			this._HKLM.Clear();
			this._HKU.Clear();
			this._HKMU.Clear();
		}

		internal VSRegistryKey FindRegistryKey(string root, string fullPath)
		{
			if (root != null)
			{
				if (root == "HKCR")
				{
					return this.HKCR.FindRegistryKey(fullPath);
				}
				if (root == "HKCU")
				{
					return this.HKCU.FindRegistryKey(fullPath);
				}
				if (root == "HKLM")
				{
					return this.HKLM.FindRegistryKey(fullPath);
				}
				if (root == "HKU")
				{
					return this.HKU.FindRegistryKey(fullPath);
				}
				if (root == "HKMU")
				{
					return this.HKMU.FindRegistryKey(fullPath);
				}
			}
			return null;
		}
	}
}