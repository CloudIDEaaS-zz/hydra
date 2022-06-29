using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DllExport
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
	[Serializable]
	public class DllExportAttribute : Attribute
	{
		public System.Runtime.InteropServices.CallingConvention CallingConvention
		{
			get;
			set;
		}

		public string ExportName
		{
			get;
			set;
		}

		public DllExportAttribute()
		{
		}

		public DllExportAttribute(string exportName) : this(exportName, System.Runtime.InteropServices.CallingConvention.StdCall)
		{
		}

		public DllExportAttribute(string exportName, System.Runtime.InteropServices.CallingConvention callingConvention)
		{
			this.ExportName = exportName;
			this.CallingConvention = callingConvention;
		}
	}
}