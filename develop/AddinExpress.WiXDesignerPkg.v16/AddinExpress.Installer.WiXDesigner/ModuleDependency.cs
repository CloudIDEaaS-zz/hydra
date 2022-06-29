using System;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class ModuleDependency
	{
		internal string Path;

		internal string RequiredID;

		internal string RequiredLanguage;

		internal string RequiredVersion;

		internal ModuleDependency(string path, string id, string language, string version)
		{
			this.Path = path;
			this.RequiredID = id;
			this.RequiredLanguage = language;
			this.RequiredVersion = version;
		}
	}
}