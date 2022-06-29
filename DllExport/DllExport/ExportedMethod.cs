using System;
using System.Runtime.CompilerServices;

namespace DllExport
{
	public sealed class ExportedMethod : ExportInfo
	{
		private readonly DllExport.ExportedClass _ExportedClass;

		public DllExport.ExportedClass ExportedClass
		{
			get
			{
				return this._ExportedClass;
			}
		}

		public override string ExportName
		{
			get
			{
				return base.ExportName ?? this.Name;
			}
			set
			{
				base.ExportName = value;
			}
		}

		public string MemberName
		{
			get;
			set;
		}

		public string Name
		{
			get
			{
				return this.MemberName;
			}
		}

		public int VTableOffset
		{
			get;
			set;
		}

		public ExportedMethod(DllExport.ExportedClass exportedClass)
		{
			this._ExportedClass = exportedClass;
		}
	}
}