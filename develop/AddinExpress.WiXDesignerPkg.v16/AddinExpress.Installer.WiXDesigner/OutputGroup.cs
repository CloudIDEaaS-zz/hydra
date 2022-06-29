using System;

namespace AddinExpress.Installer.WiXDesigner
{
	[Flags]
	internal enum OutputGroup
	{
		None = 0,
		Binaries = 1,
		Symbols = 2,
		Sources = 4,
		Content = 8,
		Satellites = 16,
		Documents = 32
	}
}