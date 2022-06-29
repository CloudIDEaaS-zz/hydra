using Mono.Cecil;
using System;
using System.IO;

namespace DllExport
{
	internal interface IAssemblyInspector
	{
		IInputValues InputValues
		{
			get;
		}
		AssemblyExports ExtractExports();
		AssemblyExports ExtractExports(AssemblyDefinition assemblyDefinition);
		AssemblyExports ExtractExports(string fileName);
		AssemblyExports ExtractExports(AssemblyDefinition assemblyDefinition, ExtractExportHandler exportFilter);
		AssemblyExports ExtractExports(string fileName, ExtractExportHandler exportFilter);
		bool SafeExtractExports(string fileName, Stream stream);
		AssemblyInitializers ExtractInitializers();
		AssemblyInitializers ExtractInitializers(AssemblyDefinition assemblyDefinition);
		AssemblyInitializers ExtractInitializers(string fileName);
		AssemblyInitializers ExtractInitializers(AssemblyDefinition assemblyDefinition, ExtractInitializerHandler exportFilter);
		AssemblyInitializers ExtractInitializers(string fileName, ExtractInitializerHandler exportFilter);
		AssemblyBinaryProperties GetAssemblyBinaryProperties(string assemblyFileName);
		AssemblyDefinition LoadAssembly(string fileName);
		bool SafeExtractInitializers(string fileName, Stream stream);
	}
}