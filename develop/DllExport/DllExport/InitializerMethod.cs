using System;
using System.Runtime.CompilerServices;

namespace DllExport
{
	public sealed class InitializerMethod : InitializeInfo
	{
		private readonly DllExport.InitializerClass _InitializerClass;

		public DllExport.InitializerClass InitializerClass
		{
			get
			{
				return this._InitializerClass;
			}
		}

		public override string InitializerName
		{
			get
			{
				return base.InitializerName ?? this.Name;
			}
			set
			{
				base.InitializerName = value;
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

		public InitializerMethod(DllExport.InitializerClass exportedClass)
		{
			this._InitializerClass = exportedClass;
		}
	}
}