using DllExport.Parsing;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace DllExport
{
	public class InputValuesCore : HasServiceProvider, IInputValues
	{
		private string _DllExportAttributeAssemblyName = Utilities.DllExportAttributeAssemblyName;

		private string _DllExportAttributeFullName = Utilities.DllExportAttributeFullName;
		private string _ModuleInitializerAttributeFullName = Utilities.ModuleInitializerAttributeFullName;

		private string _Filename;

		public CpuPlatform Cpu
		{
			get;
			set;
		}

		public string DllExportAttributeAssemblyName
		{
			get
			{
				return this._DllExportAttributeAssemblyName;
			}
			set
			{
				this._DllExportAttributeAssemblyName = value;
			}
		}

		public string DllExportAttributeFullName
		{
			get
			{
				return this._DllExportAttributeFullName;
			}
			set
			{
				this._DllExportAttributeFullName = value;
			}
		}

		public string ModuleInitializerAttributeFullName
		{
			get
			{
				return this._ModuleInitializerAttributeFullName;
			}
			set
			{
				this._ModuleInitializerAttributeFullName = value;
			}
		}

		public bool EmitDebugSymbols
		{
			get;
			set;
		}

		public string FileName
		{
			get
			{
				Monitor.Enter(this);
				try
				{
					if (string.IsNullOrEmpty(this._Filename))
					{
						this._Filename = Path.GetFileNameWithoutExtension(this.InputFileName);
					}
				}
				finally
				{
					Monitor.Exit(this);
				}
				return this._Filename;
			}
			set
			{
				Monitor.Enter(this);
				try
				{
					this._Filename = value;
				}
				finally
				{
					Monitor.Exit(this);
				}
			}
		}

		public string FrameworkPath { get; set; }
		public string InputFileName { get; set; }
		public string PrivateKey { get; set; }
		public string KeyContainer { get; set; }
		public string KeyFile { get; set; }
		public string LeaveIntermediateFiles { get; set; }
		public string LibToolDllPath { get; set; }
		public string LibToolPath { get; set; }
		public string MethodAttributes { get; set; }
		public string OutputFileName { get; set; }
		public string RootDirectory { get; set; }
		public string SdkPath { get; set; }
        public string Files { get; set; }
		public string TestTempDirectory { get; set; }

		public InputValuesCore(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		public AssemblyBinaryProperties InferAssemblyBinaryProperties()
		{
			IAssemblyInspector inspector = Utilities.CreateAssemblyInspector(this);
			AssemblyBinaryProperties binaryProperties = inspector.GetAssemblyBinaryProperties(this.InputFileName);
			if (this.Cpu == CpuPlatform.None && binaryProperties.BinaryWasScanned)
			{
				this.Cpu = binaryProperties.CpuPlatform;
			}
			return binaryProperties;
		}

		public void InferOutputFile()
		{
			if (string.IsNullOrEmpty(this.OutputFileName))
			{
				this.OutputFileName = this.InputFileName;
			}
		}
	}
}