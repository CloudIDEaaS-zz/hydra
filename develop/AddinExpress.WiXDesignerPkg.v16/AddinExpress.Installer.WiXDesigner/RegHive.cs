using Microsoft.VisualStudio.Shell;
using Microsoft.Win32;
using System;

namespace AddinExpress.Installer.WiXDesigner
{
	internal sealed class RegHive : Hive
	{
		private RegistryKey root;

		private string regRoot;

		public override string Root
		{
			get
			{
				return this.regRoot;
			}
		}

		public override bool UseMsi
		{
			get
			{
				return false;
			}
		}

		public RegHive(string regRoot)
		{
			this.regRoot = regRoot;
			this.root = Registry.LocalMachine.CreateSubKey(regRoot);
		}

		public void Close()
		{
			this.root.Close();
		}

		public override RegistrationAttribute.Key CreateKey(string name)
		{
			return new RegKey(this.root.CreateSubKey(name));
		}

		public override void RemoveKey(string name)
		{
			bool flag = false;
			using (RegistryKey registryKey = this.root.OpenSubKey(name, false))
			{
				if (registryKey != null)
				{
					flag = true;
				}
			}
			if (flag)
			{
				this.root.DeleteSubKeyTree(name);
			}
		}

		public override void RemoveValue(string name, string valuename)
		{
			using (RegistryKey registryKey = this.root.OpenSubKey(name, true))
			{
				if (registryKey != null)
				{
					registryKey.DeleteValue(valuename, false);
				}
			}
		}
	}
}