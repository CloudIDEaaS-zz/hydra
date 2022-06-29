using Microsoft.VisualStudio.Shell;
using Microsoft.Win32;
using System;

namespace AddinExpress.Installer.WiXDesigner
{
	internal sealed class RegKey : RegistrationAttribute.Key
	{
		private RegistryKey key;

		internal RegKey(RegistryKey key)
		{
			this.key = key;
		}

		public override void Close()
		{
			this.key.Close();
		}

		public override RegistrationAttribute.Key CreateSubkey(string name)
		{
			return new RegKey(this.key.CreateSubKey(name));
		}

		public override void SetValue(string valueName, object value)
		{
			if (value is short)
			{
				value = (int)((short)value);
			}
			this.key.SetValue(valueName, value);
		}
	}
}