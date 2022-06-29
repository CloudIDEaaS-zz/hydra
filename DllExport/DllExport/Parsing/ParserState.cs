using System;

namespace DllExport.Parsing
{
	public enum ParserState
	{
		Normal,
		ClassDeclaration,
		Class,
		DeleteExportDependency,
		MethodDeclaration,
		MethodProperties,
		Method,
		Module,
		DeleteExportAttribute
	}
}