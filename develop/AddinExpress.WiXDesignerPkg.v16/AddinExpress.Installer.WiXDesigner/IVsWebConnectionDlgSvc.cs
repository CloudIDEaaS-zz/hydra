using System;
using System.Runtime.InteropServices;

namespace AddinExpress.Installer.WiXDesigner
{
	[Guid("E4A1263A-11CF-4c5f-B3D5-E41AFA0ADE59")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IVsWebConnectionDlgSvc
	{
		void WebConnectionDlg([In] uint dwFlags, out string pbstrUrl, out int pfUseFrontpageForLocalUrl, out object ppunkConnectionInfo, out int pfCanceled);
	}
}