using System;

namespace AddinExpress.Installer.WiXDesigner
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
	internal sealed class ADXDefaultRegistryRootAttribute : Attribute
	{
		private string _root;

		public string Root
		{
			get
			{
				return this._root;
			}
		}

		public ADXDefaultRegistryRootAttribute(string root)
		{
			if (root == null)
			{
				throw new ArgumentNullException("root");
			}
			this._root = root;
		}
	}
}