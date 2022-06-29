using Microsoft.VisualStudio.Shell;
using System;

namespace AddinExpress.Installer.WiXDesigner
{
	internal abstract class Hive
	{
		public abstract string Root
		{
			get;
		}

		public abstract bool UseMsi
		{
			get;
		}

		protected Hive()
		{
		}

		public abstract RegistrationAttribute.Key CreateKey(string name);

		public abstract void RemoveKey(string name);

		public abstract void RemoveValue(string name, string valuename);
	}
}