using System;

namespace AddinExpress.Installer.WiXDesigner
{
	internal delegate void BeforeShowErrorEventHandler(Exception error, ref bool cancelErrorView);
}