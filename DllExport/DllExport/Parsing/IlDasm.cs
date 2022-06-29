using DllExport;
using System;
using System.Globalization;
using System.IO;
using System.Security.Permissions;

namespace DllExport.Parsing
{
	[PermissionSet(SecurityAction.LinkDemand, Name="FullTrust")]
	public sealed class IlDasm : IlToolBase
	{
		public IlDasm(IServiceProvider serviceProvider, IInputValues inputValues) : base(serviceProvider, inputValues)
		{
		}

		public int Run()
		{
			var invariantCulture = CultureInfo.InvariantCulture;
			var objArray = new object[] { Path.Combine(base.TempDirectory, base.InputValues.FileName), base.InputValues.InputFileName, null };
			string arguments;
			int result;

			objArray[2] = (base.InputValues.EmitDebugSymbols ? " /linenum " : " ");
			arguments = string.Format(invariantCulture, "/quoteallnames /unicode /nobar{2}\"/out:{0}.il\" \"{1}\"", objArray);

			result = IlParser.RunIlTool(base.InputValues.SdkPath, "ildasm.exe", null, null, "ILDasmPath", arguments, DllExportLogginCodes.IlDasmLogging, DllExportLogginCodes.VerboseToolLogging, base.Notifier, base.Timeout, null);

			return result;
		}
	}
}