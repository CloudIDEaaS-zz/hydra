using System;

namespace AddinExpress.Installer.WiXDesigner
{
	internal static class DesignerFactory
	{
		private static IServiceProvider _serviceProvider;

		private static IBuildManager _buildManager;

		public static IServiceProvider ServiceProvider
		{
			get
			{
				return DesignerFactory._serviceProvider;
			}
			set
			{
				DesignerFactory._serviceProvider = value;
			}
		}

		public static void CleanupFactory()
		{
			DesignerFactory._buildManager = null;
		}

		public static IBuildManager GetBuildManager()
		{
			if (DesignerFactory._buildManager == null)
			{
				DesignerFactory._buildManager = new BuildManager(DesignerFactory._serviceProvider);
			}
			return DesignerFactory._buildManager;
		}
	}
}