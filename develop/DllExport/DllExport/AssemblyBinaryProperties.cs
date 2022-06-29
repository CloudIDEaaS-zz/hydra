using Mono.Cecil;
using System;

namespace DllExport
{
	public sealed class AssemblyBinaryProperties
	{
		public readonly static AssemblyBinaryProperties EmptyNotScanned;

		private readonly bool _BinaryWasScanned;

		private readonly bool _IsSigned;

		private readonly string _KeyContainer;

		private readonly string _KeyFileName;

		private readonly TargetArchitecture _MachineKind;

		private readonly ModuleAttributes _PeKind;

		public bool BinaryWasScanned
		{
			get
			{
				return this._BinaryWasScanned;
			}
		}

		public DllExport.CpuPlatform CpuPlatform
		{
			get
			{
				if (this.PeKind.Contains(ModuleAttributes.ILOnly))
				{
					if (!this.MachineKind.Contains(TargetArchitecture.IA64))
					{
						return DllExport.CpuPlatform.X64;
					}
					return DllExport.CpuPlatform.Itanium;
				}
				if (!this.PeKind.Contains(ModuleAttributes.Required32Bit))
				{
					return DllExport.CpuPlatform.AnyCpu;
				}
				return DllExport.CpuPlatform.X86;
			}
		}

		public bool IsIlOnly
		{
			get
			{
				return this.PeKind.Contains(ModuleAttributes.ILOnly);
			}
		}

		public bool IsSigned
		{
			get
			{
				return this._IsSigned;
			}
		}

		public string KeyContainer
		{
			get
			{
				return this._KeyContainer;
			}
		}

		public string KeyFileName
		{
			get
			{
				return this._KeyFileName;
			}
		}

		internal TargetArchitecture MachineKind
		{
			get
			{
				return this._MachineKind;
			}
		}

		internal ModuleAttributes PeKind
		{
			get
			{
				return this._PeKind;
			}
		}

		static AssemblyBinaryProperties()
		{
			AssemblyBinaryProperties.EmptyNotScanned = new AssemblyBinaryProperties(0, TargetArchitecture.I386, false, null, null, false);
		}

		internal AssemblyBinaryProperties(ModuleAttributes peKind, TargetArchitecture machineKind, bool isSigned, string keyFileName, string keyContainer, bool binaryWasScanned)
		{
			this._PeKind = peKind;
			this._MachineKind = machineKind;
			this._IsSigned = isSigned;
			this._KeyFileName = keyFileName;
			this._KeyContainer = keyContainer;
			this._BinaryWasScanned = binaryWasScanned;
		}

		internal AssemblyBinaryProperties(ModuleAttributes peKind, TargetArchitecture machineKind, bool isSigned, string keyFileName, string keyContainer) : this(peKind, machineKind, isSigned, keyFileName, keyContainer, true)
		{
		}

		public static AssemblyBinaryProperties GetEmpty()
		{
			return AssemblyBinaryProperties.EmptyNotScanned;
		}
	}
}