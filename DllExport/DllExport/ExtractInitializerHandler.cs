using Mono.Cecil;
using System;

namespace DllExport
{
	internal delegate bool ExtractInitializerHandler(MethodDefinition memberInfo, out IInitializerInfo exportInfo);
}