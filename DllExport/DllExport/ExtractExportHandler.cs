using Mono.Cecil;
using System;

namespace DllExport
{
	internal delegate bool ExtractExportHandler(MethodDefinition memberInfo, out IExportInfo exportInfo);
}