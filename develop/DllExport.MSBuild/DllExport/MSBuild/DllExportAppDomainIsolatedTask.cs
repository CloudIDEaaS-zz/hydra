using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using DllExport;
using System;
using System.Resources;
using System.Security.Permissions;
using System.Windows.Forms;

namespace DllExport.MSBuild
{
	[LoadInSeparateAppDomain]
	[PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust")]
	public class DllExportAppDomainIsolatedTask : AppDomainIsolatedTask, IDllExportTask, IInputValues, IServiceProvider
	{
		private readonly ExportTaskImplementation<DllExportAppDomainIsolatedTask> _ExportTaskImplementation;
		
		public string Files 
		{
			get
            {
				return this._ExportTaskImplementation.Files;
			}

			set
            {
				this._ExportTaskImplementation.Files = value;

			}
		}

		public string TestTempDirectory
		{
			get
			{
				return this._ExportTaskImplementation.TestTempDirectory;
			}

			set
			{
				this._ExportTaskImplementation.TestTempDirectory = value;

			}
		}

		public string AssemblyKeyContainerName
		{
			get
			{
				return this._ExportTaskImplementation.AssemblyKeyContainerName;
			}
			set
			{
				this._ExportTaskImplementation.AssemblyKeyContainerName = value;
			}
		}

		public CpuPlatform Cpu
		{
			get
			{
				return this._ExportTaskImplementation.Cpu;
			}
			set
			{
				this._ExportTaskImplementation.Cpu = value;
			}
		}

		//////public string CpuType
		//////{
		//////	get
		//////	{
		//////		return this._ExportTaskImplementation.CpuType;
		//////	}
		//////	set
		//////	{
		//////		this._ExportTaskImplementation.CpuType = value;
		//////	}
		//////}

		public string DllExportAttributeAssemblyName
		{
			get
			{
				return this._ExportTaskImplementation.DllExportAttributeAssemblyName;
			}
			set
			{
				this._ExportTaskImplementation.DllExportAttributeAssemblyName = value;
			}
		}

		public string DllExportAttributeFullName
		{
			get
			{
				return this._ExportTaskImplementation.DllExportAttributeFullName;
			}
			set
			{
				this._ExportTaskImplementation.DllExportAttributeFullName = value;
			}
		}

		public string ModuleInitializerAttributeFullName
		{
			get
			{
				return this._ExportTaskImplementation.ModuleInitializerAttributeFullName;
			}
			set
			{
				this._ExportTaskImplementation.ModuleInitializerAttributeFullName = value;
			}
		}

        public bool EmitDebugSymbols
        {
            get
            {
                return this._ExportTaskImplementation.EmitDebugSymbols;
            }
            set
            {
                this._ExportTaskImplementation.EmitDebugSymbols = value;
            }
        }

        public string FileName
		{
			get
			{
				return this._ExportTaskImplementation.FileName;
			}
			set
			{
				this._ExportTaskImplementation.FileName = value;
			}
		}

        public string FrameworkPath
        {
            get
            {
                return this._ExportTaskImplementation.FrameworkPath;
            }
            set
            {
                this._ExportTaskImplementation.FrameworkPath = value;
            }
        }

        [Required]
		public string InputFileName
		{
			get
			{
				return this._ExportTaskImplementation.InputFileName;
			}
			set
			{
				this._ExportTaskImplementation.InputFileName = value;
			}
		}

		public string PrivateKey
		{
			get
			{
				return this._ExportTaskImplementation.PrivateKey;
			}
			set
			{
				this._ExportTaskImplementation.PrivateKey = value;
			}
		}

		public string KeyContainer
		{
			get
			{
				return this._ExportTaskImplementation.KeyContainer;
			}
			set
			{
				this._ExportTaskImplementation.KeyContainer = value;
			}
		}

		public string KeyFile
		{
			get
			{
				return this._ExportTaskImplementation.KeyFile;
			}
			set
			{
				this._ExportTaskImplementation.KeyFile = value;
			}
		}

		public string LeaveIntermediateFiles
		{
			get
			{
				return this._ExportTaskImplementation.LeaveIntermediateFiles;
			}
			set
			{
				this._ExportTaskImplementation.LeaveIntermediateFiles = value;
			}
		}

        public string LibToolDllPath
        {
            get
            {
                return this._ExportTaskImplementation.LibToolDllPath;
            }
            set
            {
                this._ExportTaskImplementation.LibToolDllPath = value;
            }
        }

        public string LibToolPath
        {
            get
            {
                return this._ExportTaskImplementation.LibToolPath;
            }
            set
            {
                this._ExportTaskImplementation.LibToolPath = value;
            }
        }

        public string MethodAttributes
		{
			get
			{
				return this._ExportTaskImplementation.MethodAttributes;
			}
			set
			{
				this._ExportTaskImplementation.MethodAttributes = value;
			}
		}

		public string OutputFileName
		{
			get
			{
				return this._ExportTaskImplementation.OutputFileName;
			}
			set
			{
				this._ExportTaskImplementation.OutputFileName = value;
			}
		}

        public string Platform
        {
            get
            {
                return this._ExportTaskImplementation.Platform;
            }
            set
            {
                this._ExportTaskImplementation.Platform = value;
            }
        }

        //////public string PlatformTarget
        //////{
        //////	get
        //////	{
        //////		return this._ExportTaskImplementation.PlatformTarget;
        //////	}
        //////	set
        //////	{
        //////		this._ExportTaskImplementation.PlatformTarget = value;
        //////	}
        //////}

        public string ProjectDirectory
		{
			get
			{
				return this._ExportTaskImplementation.ProjectDirectory;
			}
			set
			{
				this._ExportTaskImplementation.ProjectDirectory = value;
			}
		}

		bool? DllExport.MSBuild.IDllExportTask.SkipOnAnyCpu
		{
			get
			{
				return this._ExportTaskImplementation.SkipOnAnyCpu;
			}
			set
			{
				this._ExportTaskImplementation.SkipOnAnyCpu = value;
			}
		}

		public string RootDirectory
		{
			get
			{
				return this._ExportTaskImplementation.RootDirectory;
			}
			set
			{
				this._ExportTaskImplementation.RootDirectory = value;
			}
		}

        public string SdkPath
        {
            get
            {
                return this._ExportTaskImplementation.SdkPath;
            }
            set
            {
                this._ExportTaskImplementation.SdkPath = value;
            }
        }

        //////public string SkipOnAnyCpu
        //////{
        //////	get
        //////	{
        //////		return Convert.ToString(this._ExportTaskImplementation.SkipOnAnyCpu);
        //////	}
        //////	set
        //////	{
        //////		if (string.IsNullOrEmpty(value))
        //////		{
        //////			this._ExportTaskImplementation.SkipOnAnyCpu = null;
        //////			return;
        //////		}
        //////		this._ExportTaskImplementation.SkipOnAnyCpu = new bool?(Convert.ToBoolean(value));
        //////	}
        //////}

        //////public string TargetFrameworkVersion
        //////{
        //////	get
        //////	{
        //////		return this._ExportTaskImplementation.TargetFrameworkVersion;
        //////	}
        //////	set
        //////	{
        //////		this._ExportTaskImplementation.TargetFrameworkVersion = value;
        //////	}
        //////}

        public int Timeout
		{
			get
			{
				return this._ExportTaskImplementation.Timeout;
			}
			set
			{
				this._ExportTaskImplementation.Timeout = value;
			}
		}

		static DllExportAppDomainIsolatedTask()
		{
			AssemblyLoadingRedirection.EnsureSetup();
		}

		public DllExportAppDomainIsolatedTask()
		{
			this._ExportTaskImplementation = new ExportTaskImplementation<DllExportAppDomainIsolatedTask>(this);
		}

		public DllExportAppDomainIsolatedTask(ResourceManager taskResources) : base(taskResources)
		{
			this._ExportTaskImplementation = new ExportTaskImplementation<DllExportAppDomainIsolatedTask>(this);
		}

		public DllExportAppDomainIsolatedTask(ResourceManager taskResources, string helpKeywordPrefix) : base(taskResources, helpKeywordPrefix)
		{
			this._ExportTaskImplementation = new ExportTaskImplementation<DllExportAppDomainIsolatedTask>(this);
		}

		public override bool Execute()
		{
			//MessageBox.Show("DllExportTask");

			return this._ExportTaskImplementation.Execute();
		}

		public IDllExportNotifier GetNotifier()
		{
			return this._ExportTaskImplementation.GetNotifier();
		}

		[CLSCompliant(false)]
		public AssemblyBinaryProperties InferAssemblyBinaryProperties()
		{
			return this._ExportTaskImplementation.InferAssemblyBinaryProperties();
		}

		public void InferOutputFile()
		{
			this._ExportTaskImplementation.InferOutputFile();
		}

		public void Notify(int severity, string code, string message, params object[] values)
		{
			this._ExportTaskImplementation.Notify(severity, code, message, values);
		}

		public void Notify(int severity, string code, string fileName, SourceCodePosition? startPosition, SourceCodePosition? endPosition, string message, params object[] values)
		{
			this._ExportTaskImplementation.Notify(severity, code, fileName, startPosition, endPosition, message, values);
		}

		TaskLoggingHelper DllExport.MSBuild.IDllExportTask.Log
		{
			get
			{
				return base.Log;
			}
		}

        public string PlatformTarget { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string TargetFrameworkVersion { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        object System.IServiceProvider.GetService(Type serviceType)
		{
			return this._ExportTaskImplementation.GetService(serviceType);
		}
	}
}