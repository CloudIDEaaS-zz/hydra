using System;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXSchema
	{
		internal static string TargetNamespace;

		internal static string TargetNamespace4;

		static WiXSchema()
		{
			WiXSchema.TargetNamespace = "http://schemas.microsoft.com/wix/2006/wi";
			WiXSchema.TargetNamespace4 = "http://wixtoolset.org/schemas/v4/wxs";
		}

		public WiXSchema()
		{
		}
	}
}