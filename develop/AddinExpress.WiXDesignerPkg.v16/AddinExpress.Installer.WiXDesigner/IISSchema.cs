using System;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class IISSchema
	{
		internal static string TargetNamespace;

		static IISSchema()
		{
			IISSchema.TargetNamespace = "http://schemas.microsoft.com/wix/IIsExtension";
		}

		public IISSchema()
		{
		}
	}
}