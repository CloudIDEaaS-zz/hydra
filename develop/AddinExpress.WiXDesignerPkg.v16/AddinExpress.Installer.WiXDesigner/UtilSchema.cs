using System;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class UtilSchema
	{
		internal static string TargetNamespace;

		static UtilSchema()
		{
			UtilSchema.TargetNamespace = "http://schemas.microsoft.com/wix/UtilExtension";
		}

		public UtilSchema()
		{
		}
	}
}