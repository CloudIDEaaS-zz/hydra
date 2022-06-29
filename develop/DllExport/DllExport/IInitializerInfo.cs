using System;
using System.Runtime.InteropServices;

namespace DllExport
{
	public interface IInitializerInfo
	{
		System.Runtime.InteropServices.CallingConvention CallingConvention
		{
			get;
			set;
		}

		string InitializerName
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

		void AssignFrom(IInitializerInfo info);
	}
}