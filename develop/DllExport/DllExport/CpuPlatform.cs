using System;

namespace DllExport
{
	[CLSCompliant(true)]
	public enum CpuPlatform
	{
		None,
		X86,
		X64,
		Itanium,
		AnyCpu
	}
}