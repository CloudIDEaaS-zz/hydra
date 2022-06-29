using Mono.Cecil;
using System;
using System.Runtime.CompilerServices;

namespace DllExport
{
	internal static class Set
	{
		public static bool Contains(this ModuleAttributes input, ModuleAttributes set)
		{
			return (input & set) == input;
		}

		public static bool Contains(this TargetArchitecture input, TargetArchitecture set)
		{
			return (input & set) == input;
		}
	}
}