using System;

namespace DllExport
{
	public interface IInputValues
	{
		CpuPlatform Cpu { get; set; }
		string DllExportAttributeAssemblyName { get; set; }
		string DllExportAttributeFullName { get; set; }
		string ModuleInitializerAttributeFullName { get; set; }
		bool EmitDebugSymbols { get; set; }
		string FileName { get; set; }
		string FrameworkPath { get; set; }
		string InputFileName { get; set; }
		string PrivateKey { get; set; }
		string KeyContainer { get; set; }
		string KeyFile { get; set; }
		string LeaveIntermediateFiles { get; set; }
		string LibToolDllPath { get; set; }
		string LibToolPath { get; set; }
		string MethodAttributes { get; set; }
		string OutputFileName { get; set; }
		string RootDirectory { get; set; }
		string SdkPath { get; set; }
		string Files { get; set; }
        string TestTempDirectory { get; set; }

        AssemblyBinaryProperties InferAssemblyBinaryProperties();
		void InferOutputFile();
	}
}