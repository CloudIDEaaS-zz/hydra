using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Runtime.CompilerServices;

namespace AddinExpress.Installer.WiXDesigner
{
	public static class IVsSolutionBuildManagerExtensions
	{
		public static IVsProjectCfg FindActiveProjectCfg(this IVsSolutionBuildManager buildManager, IVsHierarchy project)
		{
			IVsProjectCfg[] vsProjectCfgArray = new IVsProjectCfg[1];
			if (ErrorHandler.Failed(buildManager.FindActiveProjectCfg(IntPtr.Zero, IntPtr.Zero, project, vsProjectCfgArray)))
			{
				return null;
			}
			return vsProjectCfgArray[0];
		}
	}
}