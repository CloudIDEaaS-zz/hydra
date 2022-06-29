using System;
using System.Runtime.InteropServices;

namespace DllExport
{
	public interface IExportInfo
	{
		System.Runtime.InteropServices.CallingConvention CallingConvention
		{
			get;
			set;
		}

		string ExportName
		{
			get;
			set;
		}

		bool IsGeneric
		{
			get;
		}

		bool IsStatic
		{
			get;
		}

		void AssignFrom(IExportInfo info);
	}
}