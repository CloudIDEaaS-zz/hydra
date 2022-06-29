using System;

namespace DllExport
{
	public static class DllExportLogginCodes
	{
		public readonly static string MethodFound;

		public readonly static string OldDeclaration;

		public readonly static string NewDeclaration;

		public readonly static string AddingVtEntry;

		public readonly static string IlAsmLogging;

		public readonly static string VerboseToolLogging;

		public readonly static string IlDasmLogging;

		public readonly static string RemovingDllExportAttribute;

		public readonly static string CreatingBinariesForEachPlatform;

		public readonly static string AmbigiguousExportName;

		public readonly static string ExportNamesHaveToBeBasicLatin;

		public readonly static string RemovingReferenceToDllExportAttributeAssembly;

		public readonly static string NoParserActionError;

		public readonly static string LibToolLooging;

		public readonly static string LibToolVerboseLooging;

		public readonly static string MethodIsNotStatic;

		public readonly static string ExportInGenericType;

		public readonly static string ExportOnGenericMethod;

		static DllExportLogginCodes()
		{
			DllExportLogginCodes.MethodFound = "EXP0001";
			DllExportLogginCodes.OldDeclaration = "EXP0002";
			DllExportLogginCodes.NewDeclaration = "EXP0003";
			DllExportLogginCodes.AddingVtEntry = "EXP0004";
			DllExportLogginCodes.IlAsmLogging = "EXP0005";
			DllExportLogginCodes.VerboseToolLogging = "EXP0006";
			DllExportLogginCodes.IlDasmLogging = "EXP0007";
			DllExportLogginCodes.RemovingDllExportAttribute = "EXP0008";
			DllExportLogginCodes.CreatingBinariesForEachPlatform = "EXP0009";
			DllExportLogginCodes.AmbigiguousExportName = "EXP0010";
			DllExportLogginCodes.ExportNamesHaveToBeBasicLatin = "EXP0011";
			DllExportLogginCodes.RemovingReferenceToDllExportAttributeAssembly = "EXP0012";
			DllExportLogginCodes.NoParserActionError = "EXP0013";
			DllExportLogginCodes.LibToolLooging = "EXP0014";
			DllExportLogginCodes.LibToolVerboseLooging = "EXP0015";
			DllExportLogginCodes.MethodIsNotStatic = "EXP0016";
			DllExportLogginCodes.ExportInGenericType = "EXP0017";
			DllExportLogginCodes.ExportOnGenericMethod = "EXP0018";
		}
	}
}