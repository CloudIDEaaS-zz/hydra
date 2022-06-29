using System;
using System.Collections.Generic;
using System.Xml;

namespace AddinExpress.Installer.Prerequisites.Manifests
{
	internal sealed class Globals
	{
		public const string BootstrapperXMLNamespace = "http://schemas.microsoft.com/developer/2004/01/bootstrapper";

		public static List<LocString> localizedStrings;

		internal static XmlDocument manifestWorkingFile;

		static Globals()
		{
			Globals.localizedStrings = new List<LocString>();
		}

		public Globals()
		{
		}
	}
}