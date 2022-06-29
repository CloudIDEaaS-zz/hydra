using System;
using System.Windows.Forms;

namespace AddinExpress.Installer.WiXDesigner
{
	internal interface IApplicationContext
	{
		string ModuleName
		{
			get;
		}

		IWin32Window ParentWindowHandle
		{
			get;
		}

		string ProductName
		{
			get;
		}

		string TypeName
		{
			get;
		}
	}
}