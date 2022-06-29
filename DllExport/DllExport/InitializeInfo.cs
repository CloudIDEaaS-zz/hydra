using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DllExport
{
	[Serializable]
	public class InitializeInfo : IInitializerInfo
	{
		public System.Runtime.InteropServices.CallingConvention CallingConvention
		{
			get;
			set;
		}

		public virtual string InitializerName
		{
			get;
			set;
		}

		public bool IsGeneric
		{
			get
			{
				return JustDecompileGenerated_get_IsGeneric();
			}
			set
			{
				JustDecompileGenerated_set_IsGeneric(value);
			}
		}

		private bool JustDecompileGenerated_IsGeneric_k__BackingField;

		public bool JustDecompileGenerated_get_IsGeneric()
		{
			return this.JustDecompileGenerated_IsGeneric_k__BackingField;
		}

		public void JustDecompileGenerated_set_IsGeneric(bool value)
		{
			this.JustDecompileGenerated_IsGeneric_k__BackingField = value;
		}

		public bool IsStatic
		{
			get
			{
				return JustDecompileGenerated_get_IsStatic();
			}
			set
			{
				JustDecompileGenerated_set_IsStatic(value);
			}
		}

		private bool JustDecompileGenerated_IsStatic_k__BackingField;

		public bool JustDecompileGenerated_get_IsStatic()
		{
			return this.JustDecompileGenerated_IsStatic_k__BackingField;
		}

		public void JustDecompileGenerated_set_IsStatic(bool value)
		{
			this.JustDecompileGenerated_IsStatic_k__BackingField = value;
		}

		public InitializeInfo()
		{
		}

		public void AssignFrom(IInitializerInfo info)
		{
			if (info != null)
			{
				this.CallingConvention = ((int)info.CallingConvention != 0 ? info.CallingConvention : System.Runtime.InteropServices.CallingConvention.StdCall);
				this.InitializerName = info.InitializerName;
			}
		}
	}
}