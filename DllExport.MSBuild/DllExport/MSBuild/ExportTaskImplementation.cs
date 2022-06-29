using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Win32;
using DllExport;
using DllExport.MSBuild.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TCallbackTool = System.Func<System.Version, string, string>;
using System.Text;

namespace DllExport.MSBuild
{
	public class ExportTaskImplementation<TTask> : IInputValues, IServiceContainer, IServiceProvider where TTask : IDllExportTask, ITask
	{
		private const string ToolLocationHelperTypeName = "Microsoft.Build.Utilities.ToolLocationHelper";
		private const string UndefinedPropertyValue = "*Undefined*";
		private readonly IServiceContainer _ServiceProvider = (IServiceContainer)new ServiceContainer();
		private readonly TTask _ActualTask;
		private readonly Dictionary<object, string> _LoggedMessages = new Dictionary<object, string>();
		private readonly IInputValues _Values;
		private int _ErrorCount;
		private int _Timeout;
		private static readonly Version _VersionUsingToolLocationHelper = new Version(4, 5);
		private static readonly IDictionary<string, TCallbackTool> _GetFrameworkToolPathByMethodName = (IDictionary<string, TCallbackTool>)new Dictionary<string, TCallbackTool>();
		private static readonly MethodInfo WrapGetToolPathCallMethodInfo = Utilities.GetMethodInfo<Func<string, int, string>>((Expression<Func<Func<string, int, string>>>)(() => ExportTaskImplementation<TTask>.WrapGetToolPathCall<int>(default(MethodInfo)))).GetGenericMethodDefinition();
		private static readonly Regex MsbuildTaskLibRegex = new Regex("(?<=^|\\\\)Microsoft\\.Build\\.Utilities\\.v(\\d+(?:\\.(\\d+))?)(?:\\.dll)?$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		public string TestTempDirectory
		{
			get
			{
				return this._Values.TestTempDirectory;
			}

			set
			{
				this._Values.TestTempDirectory = value;
			}
		}

		public string Files 
		{
			get
            {
				return this._Values.Files;
            }

			set
            {
				this._Values.Files = value;
			}
		}

		public string AssemblyKeyContainerName
		{
			get
			{
				return this._Values.KeyContainer;
			}
			set
			{
				this._Values.KeyContainer = value;
			}
		}

		public CpuPlatform Cpu
		{
			get
			{
				return this._Values.Cpu;
			}
			set
			{
				this._Values.Cpu = value;
			}
		}

		public string CpuType
		{
			get;
			set;
		}

		public string DllExportAttributeAssemblyName
		{
			get
			{
				return this._Values.DllExportAttributeAssemblyName;
			}
			set
			{
				this._Values.DllExportAttributeAssemblyName = value;
			}
		}

		public string DllExportAttributeFullName
		{
			get
			{
				return this._Values.DllExportAttributeFullName;
			}
			set
			{
				this._Values.DllExportAttributeFullName = value;
			}
		}

		public string ModuleInitializerAttributeFullName
		{
			get
			{
				return this._Values.ModuleInitializerAttributeFullName;
			}
			set
			{
				this._Values.ModuleInitializerAttributeFullName = value;
			}
		}

		public bool EmitDebugSymbols
		{
			get
			{
				return this._Values.EmitDebugSymbols;
			}

			set
			{
				this._Values.EmitDebugSymbols = value;
			}
		}

		public string FileName
		{
			get
			{
				return this._Values.FileName;
			}

			set
			{
				this._Values.FileName = value;
			}
		}

		[Required]
		public string FrameworkPath
		{
			get
			{
				return this._Values.FrameworkPath;
			}

			set
			{
				this._Values.FrameworkPath = value;
			}
		}

		private Func<Version, string, string> GetFrameworkToolPath
		{
			get
			{
				return this.GetGetToolPath("GetPathToDotNetFrameworkFile");
			}
		}

		private Func<Version, string, string> GetSdkToolPath
		{
			get
			{
				return this.GetGetToolPath("GetPathToDotNetFrameworkSdkFile");
			}
		}

		[Required]
		public string InputFileName
		{
			get
			{
				return this._Values.InputFileName;
			}
			set
			{
				this._Values.InputFileName = value;
			}
		}

		[Required]
		public string PrivateKey
		{
			get
			{
				return this._Values.PrivateKey;
			}
			set
			{
				this._Values.PrivateKey = value;
			}
		}

		public string KeyContainer
		{
			get
			{
				return this._Values.KeyContainer;
			}
			set
			{
				this._Values.KeyContainer = value;
			}
		}

		public string KeyFile
		{
			get
			{
				return this._Values.KeyFile;
			}
			set
			{
				this._Values.KeyFile = value;
			}
		}

		public string LeaveIntermediateFiles
		{
			get
			{
				return this._Values.LeaveIntermediateFiles;
			}
			set
			{
				this._Values.LeaveIntermediateFiles = value;
			}
		}

		public string LibToolDllPath
		{
			get
			{
				return this._Values.LibToolDllPath;
			}
			set
			{
				this._Values.LibToolDllPath = value;
			}
		}

		public string LibToolPath
		{
			get
			{
				return this._Values.LibToolPath;
			}
			set
			{
				this._Values.LibToolPath = value;
			}
		}

		public string MethodAttributes
		{
			get
			{
				return this._Values.MethodAttributes;
			}
			set
			{
				this._Values.MethodAttributes = value;
			}
		}

		public string OutputFileName
		{
			get
			{
				return this._Values.OutputFileName;
			}
			set
			{
				this._Values.OutputFileName = value;
			}
		}

		public string Platform
		{
			get;
			set;
		}

		public string PlatformTarget
		{
			get;
			set;
		}

		public string ProjectDirectory
		{
			get
			{
				return this._Values.RootDirectory;
			}
			set
			{
				this._Values.RootDirectory = value;
			}
		}

		public string RootDirectory
		{
			get
			{
				return this._Values.RootDirectory;
			}
			set
			{
				this._Values.RootDirectory = value;
			}
		}

		public string SdkPath
		{
			get
			{
				return this._Values.SdkPath;
			}
			set
			{
				this._Values.SdkPath = value;
			}
		}

		public bool? SkipOnAnyCpu
		{
			get;
			set;
		}

		public string TargetFrameworkVersion
		{
			get;
			set;
		}

		public int Timeout
		{
			get
			{
				return this._Timeout;
			}
			set
			{
				this._Timeout = value;
			}
		}


        static ExportTaskImplementation()
		{
			ExportTaskImplementation<TTask>._VersionUsingToolLocationHelper = new Version(4, 5);
			ExportTaskImplementation<TTask>._GetFrameworkToolPathByMethodName = new Dictionary<string, Func<Version, string, string>>();
			ExportTaskImplementation<TTask>.WrapGetToolPathCallMethodInfo = Utilities.GetMethodInfo<Func<string, int, string>>(() => ExportTaskImplementation<TTask>.WrapGetToolPathCall<int>((MethodInfo)null)).GetGenericMethodDefinition();
			ExportTaskImplementation<TTask>.MsbuildTaskLibRegex = new Regex("(?<=^|\\\\)Microsoft\\.Build\\.Utilities\\.v(\\d+(?:\\.(\\d+))?)(?:\\.dll)?$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
			AssemblyLoadingRedirection.EnsureSetup();
		}

		public ExportTaskImplementation(TTask actualTask)
		{
			//MessageBox.Show("ExportTaskImplementation.ctor");

			this._ActualTask = actualTask;
			this.AddServiceFactory<ExportTaskImplementation<TTask>, IDllExportNotifier>((IServiceProvider sp) => new ExportTaskImplementation<TTask>.DllExportNotifierWithTask(this._ActualTask));
			this._Values = new InputValuesCore(this);
			this.GetNotifier().Notification += new EventHandler<DllExportNotificationEventArgs>(this.OnDllWrapperNotification);
		}

		public bool Execute()
		{
			bool flag;
			this._ErrorCount = 0;
			var ildasm = Microsoft.Build.Utilities.ToolLocationHelper.GetPathToDotNetFrameworkSdkFile("ildasm.exe", TargetDotNetFrameworkVersion.VersionLatest);
			var ilasm = Microsoft.Build.Utilities.ToolLocationHelper.GetPathToDotNetFrameworkFile("ilasm.exe", TargetDotNetFrameworkVersion.VersionLatest);

			if (this.ValidateInputValues())
			{
				this._Values.Cpu = Utilities.ToCpuPlatform(this.Platform);
				this._Values.SdkPath = Path.GetDirectoryName(ildasm);
				this._Values.FrameworkPath = Path.GetDirectoryName(ilasm);

				this._ActualTask.Cpu = this._Values.Cpu;
				this._ActualTask.SdkPath = this._Values.SdkPath;
				this._ActualTask.FrameworkPath = this._Values.FrameworkPath;

				try
				{
					bool? skipOnAnyCpu = false;
					//////bool? skipOnAnyCpu = this.SkipOnAnyCpu;
					
					if (this._Values.Files != null)
                    {
						var files = this._Values.Files.Split(';');
						var builder = new StringBuilder();

						foreach (var file in files)
                        {
							var newFile = Path.GetFullPath(Path.Combine(this.ProjectDirectory, file));

							if (!File.Exists(newFile))
							{
								var tTask = this._ActualTask;

								tTask.Log.LogMessage(Resources.InputFile_0_does_not_exist, newFile);
							}
							else
							{
								if (builder.Length > 0)
								{
									builder.Append(";");
								}

								builder.Append(newFile);
							}
                        }

						this._Values.Files = builder.ToString();
					}

					if ((skipOnAnyCpu.HasValue ? !skipOnAnyCpu.GetValueOrDefault() : false) || this._Values.Cpu != CpuPlatform.AnyCpu)
					{
						var binaryProperties = this._Values.InferAssemblyBinaryProperties();

						if (string.IsNullOrEmpty(this.KeyFile) && !string.IsNullOrEmpty(binaryProperties.KeyFileName))
						{
							this.KeyFile = binaryProperties.KeyFileName;
						}

						if (string.IsNullOrEmpty(this.KeyContainer) && !string.IsNullOrEmpty(binaryProperties.KeyContainer))
						{
							this.KeyContainer = binaryProperties.KeyContainer;
						}

						this._Values.InferOutputFile();
						this.ValidateKeyFiles(binaryProperties.IsSigned);

						using (var dllExportWeaver = new DllExportWeaver(this){ Timeout = this.Timeout })
						{
							dllExportWeaver.InputValues = this._ActualTask;
							dllExportWeaver.Run();
						}

						flag = this._ErrorCount == 0;
					}
					else
					{
						var tTask = this._ActualTask;

						tTask.Log.LogMessage(Resources.Skipped_Method_Exports, new object[0]);

						flag = true;
					}
				}
				catch (Exception exception)
				{
					TTask task;
					Exception ex = exception;

					this._ActualTask.Log.LogErrorFromException(ex);

					task = this._ActualTask;
					
					task.Log.LogMessage(ex.StackTrace, new object[0]);

					this._LoggedMessages.Clear();

					return false;
				}

				return flag;
			}

			return false;
		}

		private Version GetFrameworkVersion()
		{
			string targetFrameworkVersion = this.TargetFrameworkVersion;
			if (!ExportTaskImplementation<TTask>.PropertyHasValue(targetFrameworkVersion))
			{
				return null;
			}
			char[] chrArray = new char[] { 'v', 'V' };
			targetFrameworkVersion = targetFrameworkVersion.TrimStart(chrArray);
			return new Version(targetFrameworkVersion);
		}

		private Func<Version, string, string> GetGetToolPath(string methodName)
		{
			Func<Version, string, string> result;
			Func<Version, string, string> func;

			lock (ExportTaskImplementation<TTask>._GetFrameworkToolPathByMethodName)
			{
				if (!ExportTaskImplementation<TTask>._GetFrameworkToolPathByMethodName.TryGetValue(methodName, out result))
				{
					var strs = ExportTaskImplementation<TTask>._GetFrameworkToolPathByMethodName;
					var getToolPathInternal = this.GetGetToolPathInternal(methodName);

					result = getToolPathInternal;

					strs.Add(methodName, getToolPathInternal);
				}

				func = result;
			}

			return func;
		}

		private Func<Version, string, string> GetGetToolPathInternal(string methodName)
		{
			Assembly asm;
			Type toolLocationHelperType = this.GetToolLocationHelperTypeFromRegsitry();
			if (toolLocationHelperType == null)
			{
				toolLocationHelperType = Type.GetType("Microsoft.Build.Utilities.ToolLocationHelper") ?? Type.GetType("Microsoft.Build.Utilities.ToolLocationHelper, Microsoft.Build.Utilities.v4.0, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
				if (toolLocationHelperType == null)
				{
					try
					{
						asm = Assembly.Load("Microsoft.Build.Utilities.v4.0, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
					}
					catch (FileNotFoundException fileNotFoundException)
					{
						asm = null;
					}
					if (asm != null)
					{
						toolLocationHelperType = asm.GetType("Microsoft.Build.Utilities.ToolLocationHelper");
					}
				}
			}
			if (toolLocationHelperType == null)
			{
				return null;
			}
			Type type = toolLocationHelperType.Assembly.GetType("Microsoft.Build.Utilities.TargetDotNetFrameworkVersion");
			Type[] typeArray = new Type[] { typeof(string), type };
			MethodInfo getPathToDotNetFrameworkFileMethod = toolLocationHelperType.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public, null, typeArray, null);
			if (getPathToDotNetFrameworkFileMethod == null)
			{
				return null;
			}
			MethodInfo methodInfo = ExportTaskImplementation<TTask>.WrapGetToolPathCallMethodInfo.MakeGenericMethod(new Type[] { type });
			object[] objArray = new object[] { getPathToDotNetFrameworkFileMethod };
			Func<string, int, string> func = (Func<string, int, string>)methodInfo.Invoke(null, objArray);
			Func<string, int, string> func1 = (string n, int v) => {
				string str;
				try
				{
					str = func(n, v);
				}
				catch (ArgumentException argumentException)
				{
					str = null;
				}
				return str;
			};
			return (Version version, string toolName) => {
				string toolPath;
				int enumAsInt = (int)Enum.Parse(type, string.Concat("Version", version.Major, version.Minor));
				for (toolPath = func1(toolName, enumAsInt); toolPath == null; toolPath = func1(toolName, enumAsInt))
				{
					enumAsInt++;
					if (!Enum.IsDefined(type, enumAsInt))
					{
						return null;
					}
				}
				while (toolPath != null && !File.Exists(toolPath))
				{
					enumAsInt--;
					if (!Enum.IsDefined(type, enumAsInt))
					{
						return null;
					}
					toolPath = func1(toolName, enumAsInt);
				}
				if (toolPath != null && !File.Exists(toolPath))
				{
					return null;
				}
				return toolPath;
			};
		}

		private static MessageImportance GetMessageImportance(int severity)
		{
			MessageImportance result = MessageImportance.Normal;
			if (severity < -1)
			{
				result = MessageImportance.Low;
			}
			else if (severity == 0)
			{
				result = MessageImportance.High;
			}
			return result;
		}

		public IDllExportNotifier GetNotifier()
		{
			return this.GetService<IDllExportNotifier>();
		}

		protected Type GetToolLocationHelperTypeFromRegsitry()
		{
			RegistryKey toolVersionsKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\MSBuild\\ToolsVersions", false);
			try
			{
				if (toolVersionsKey == null)
				{
					return (Type)null;
				}
				Regex nameRegex = new Regex("^v?(\\d+(?:\\.\\d+))$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
				HashSet<string> utilFileNames = new HashSet<string>((IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase)
				{
				"Microsoft.Build.Utilities.Core",
				"Microsoft.Build.Utilities"
			};
				Func<string, Type> getToolhelperType = (Func<string, Type>)(n =>
				{
					using (RegistryKey registryKey = toolVersionsKey.OpenSubKey(n, false))
					{
						if (registryKey == null)
						{
							return (Type)null;
						}
						string path = (string)registryKey.GetValue("MSBuildToolsPath");
						if (path == null)
						{
							return (Type)null;
						}
						string[] files = Directory.GetFiles(path, "*.dll");
						if (files.LongLength == 0L)
						{
							return (Type)null;
						}
						string assemblyFile = ((IEnumerable<string>)files).Select(dllName => new
						{
							dllName = dllName,
							fileName = Path.GetFileNameWithoutExtension(dllName)
						}).Select(param0 => new
						{
							__cc__h__TransparentIdentifierc = param0,
							isUtil = utilFileNames.Contains(param0.fileName)
						}).Select(param0 => new
						{
							__cc__h__TransparentIdentifierd = param0,
							utilMatch = ExportTaskImplementation<TTask>.MsbuildTaskLibRegex.Match(param0.__cc__h__TransparentIdentifierc.fileName)
						}).Select(param0 => new
						{
							__cc__h__TransparentIdentifiere = param0,
							utilVersion = param0.utilMatch.Success ? new Version(param0.utilMatch.Groups[1].Value) : (Version)null
						}).Where(param0 =>
						{
							if (!param0.__cc__h__TransparentIdentifiere.__cc__h__TransparentIdentifierd.isUtil)
							{
								return param0.utilVersion != (Version)null;
							}
							return true;
						}).OrderBy(param0 => !(param0.utilVersion != (Version)null) ? 2 : 1).ThenBy(param0 => !param0.__cc__h__TransparentIdentifiere.__cc__h__TransparentIdentifierd.isUtil ? 2 : 1).ThenByDescending(param0 => param0.utilVersion).Select(param0 => new
						{
							__cc__h__TransparentIdentifierf = param0,
							asm = Assembly.ReflectionOnlyLoadFrom(param0.__cc__h__TransparentIdentifiere.__cc__h__TransparentIdentifierd.__cc__h__TransparentIdentifierc.dllName)
						}).Select(param0 => new
						{
							__cc__h__TransparentIdentifier10 = param0,
							t = param0.asm.GetType("Microsoft.Build.Utilities.ToolLocationHelper")
						}).Where(param0 => param0.t != null).Select(param0 => param0.__cc__h__TransparentIdentifier10.__cc__h__TransparentIdentifierf.__cc__h__TransparentIdentifiere.__cc__h__TransparentIdentifierd.__cc__h__TransparentIdentifierc.dllName).FirstOrDefault<string>();
						if (assemblyFile == null)
						{
							return (Type)null;
						}
						return Assembly.LoadFrom(assemblyFile).GetType("Microsoft.Build.Utilities.ToolLocationHelper");
					}
				});
				return ((IEnumerable<string>)toolVersionsKey.GetSubKeyNames()).Select(n => new
				{
					n = n,
					m = nameRegex.Match(n)
				}).Where(param0 => param0.m.Success).Select(param0 => new
				{
					__cc__h__TransparentIdentifier12 = param0,
					version = new Version(param0.m.Groups[1].Value)
				}).OrderByDescending(param0 => param0.version).Select(param0 => new
				{
					__cc__h__TransparentIdentifier13 = param0,
					t = getToolhelperType(param0.__cc__h__TransparentIdentifier12.n)
				}).Where(param0 => param0.t != null).Select(param0 => param0.t).FirstOrDefault<Type>();
			}
			finally
			{
				if (toolVersionsKey != null)
				{
					((IDisposable)toolVersionsKey).Dispose();
				}
			}
		}

		private static string GetVsPath()
		{
			string vsVersionText = Environment.GetEnvironmentVariable("VisualStudioVersion").NullIfEmpty();
			if (vsVersionText == null)
			{
				return null;
			}
			Version vsVersion = new Version(vsVersionText);
			string vsToolsDirText = Environment.GetEnvironmentVariable(string.Format("VS{0}{1}COMNTOOLS", vsVersion.Major, vsVersion.Minor)).NullIfEmpty();
			if (vsToolsDirText == null)
			{
				return null;
			}
			DirectoryInfo vsToolsDir = new DirectoryInfo(vsToolsDirText);
			if (vsToolsDir.Name.Equals("tools", StringComparison.InvariantCultureIgnoreCase) && vsToolsDir.Exists)
			{
				DirectoryInfo common7Dir = vsToolsDir.Parent;
				if (common7Dir != null && common7Dir.Parent != null && common7Dir.Name.Equals("common7", StringComparison.InvariantCultureIgnoreCase))
				{
					return common7Dir.Parent.FullName;
				}
			}
			return null;
		}

		[CLSCompliant(false)]
		public AssemblyBinaryProperties InferAssemblyBinaryProperties()
		{
			return this._Values.InferAssemblyBinaryProperties();
		}

		public void InferOutputFile()
		{
			this._Values.InferOutputFile();
		}

		public void Notify(int severity, string code, string message, params object[] values)
		{
			this.GetNotifier().Notify(severity, code, message, values);
		}

		public void Notify(int severity, string code, string fileName, SourceCodePosition? startPosition, SourceCodePosition? endPosition, string message, params object[] values)
		{
			this.GetNotifier().Notify(severity, code, fileName, startPosition, endPosition, message, values);
		}

		private void OnDllWrapperNotification(object sender, DllExportNotificationEventArgs e)
		{
			MessageImportance messageImportance = ExportTaskImplementation<TTask>.GetMessageImportance(e.Severity);
			string fileName = (string.IsNullOrEmpty(e.FileName) ? this._ActualTask.BuildEngine.ProjectFileOfTaskNode : e.FileName);
			SourceCodePosition? startPosition = e.StartPosition;
			SourceCodePosition startPos = (startPosition.HasValue ? startPosition.GetValueOrDefault() : new SourceCodePosition(0, 0));
			SourceCodePosition? endPosition = e.EndPosition;
			SourceCodePosition endPos = (endPosition.HasValue ? endPosition.GetValueOrDefault() : new SourceCodePosition(0, 0));
			string messageArgs = e.Message;
			if (e.Severity > 0 && e.Context != null && e.Context.Name != null)
			{
				messageArgs = string.Concat(e.Context.Name, ": ", messageArgs);
			}
			var x = new { startPos = startPos, endPos = endPos, fileName = fileName, Severity = e.Severity, Code = e.Code, Message = messageArgs };
			if (this._LoggedMessages.ContainsKey(x))
			{
				return;
			}
			this._LoggedMessages.Add(x, messageArgs);
			if (e.Severity == 1)
			{
				TTask tTask = this._ActualTask;
				tTask.Log.LogWarning("Export", e.Code, null, fileName, startPos.Line, startPos.Character, endPos.Line, endPos.Character, messageArgs, new object[0]);
				return;
			}
			if (e.Severity <= 1)
			{
				TTask tTask1 = this._ActualTask;
				tTask1.Log.LogMessage(messageImportance, messageArgs, new object[0]);
				return;
			}
			this._ErrorCount++;
			TTask tTask2 = this._ActualTask;
			tTask2.Log.LogError("Export", e.Code, null, fileName, startPos.Line, startPos.Character, endPos.Line, endPos.Character, messageArgs, new object[0]);
		}

		private static bool PropertyHasValue(string propertyValue)
		{
			if (string.IsNullOrEmpty(propertyValue))
			{
				return false;
			}
			return !propertyValue.Contains("*Undefined*");
		}

		void System.ComponentModel.Design.IServiceContainer.AddService(Type serviceType, object serviceInstance)
		{
			this._ServiceProvider.AddService(serviceType, serviceInstance);
		}

		void System.ComponentModel.Design.IServiceContainer.AddService(Type serviceType, object serviceInstance, bool promote)
		{
			this._ServiceProvider.AddService(serviceType, serviceInstance, promote);
		}

		void System.ComponentModel.Design.IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback)
		{
			this._ServiceProvider.AddService(serviceType, callback);
		}

		void System.ComponentModel.Design.IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
		{
			this._ServiceProvider.AddService(serviceType, callback, promote);
		}

		void System.ComponentModel.Design.IServiceContainer.RemoveService(Type serviceType)
		{
			this._ServiceProvider.RemoveService(serviceType);
		}

		void System.ComponentModel.Design.IServiceContainer.RemoveService(Type serviceType, bool promote)
		{
			this._ServiceProvider.RemoveService(serviceType, promote);
		}

		object System.IServiceProvider.GetService(Type serviceType)
		{
			return this._ServiceProvider.GetService(serviceType);
		}

		public static bool TrySearchToolPath(string toolPath, string toolFilename, out string value)
		{
			value = null;
			while (toolPath.Contains("\\\\"))
			{
				toolPath = toolPath.Replace("\\\\", "\\");
			}
			char[] chrArray = new char[] { ';' };
			string[] array = toolPath.Split(chrArray, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < (int)array.Length; i++)
			{
				string path = array[i];
				if (!string.IsNullOrEmpty(path))
				{
					string fullPath = path;
					if (File.Exists(Path.Combine(fullPath, toolFilename)))
					{
						value = fullPath;
						return true;
					}
					fullPath = Path.GetFullPath(path);
					if (File.Exists(Path.Combine(fullPath, toolFilename)))
					{
						value = fullPath;
						return true;
					}
				}
			}
			return false;
		}

		private bool TryToGetToolDirForFxVersion(string toolFileName, Func<Version, string, string> getToolPath, ref string toolDirectory)
		{
			Version version = this.GetFrameworkVersion();
			if (getToolPath == null)
			{
				if (version < ExportTaskImplementation<TTask>._VersionUsingToolLocationHelper)
				{
					return false;
				}
				TTask tTask = this._ActualTask;
				tTask.Log.LogError(string.Format(Resources.Cannot_get_a_reference_to_ToolLocationHelper, "Microsoft.Build.Utilities.ToolLocationHelper"), new object[0]);
				return false;
			}
			string foundIlAsmPath = getToolPath(version, toolFileName);
			if (foundIlAsmPath != null && File.Exists(foundIlAsmPath))
			{
				toolDirectory = Path.GetDirectoryName(foundIlAsmPath);
				return true;
			}
			TTask tTask1 = this._ActualTask;
			tTask1.Log.LogError(string.Format(Resources.ToolLocationHelperTypeName_could_not_find_1, "Microsoft.Build.Utilities.ToolLocationHelper", toolFileName), new object[0]);
			return false;
		}

		private bool ValidateFileName()
		{
			bool result = false;
			if (string.IsNullOrEmpty(this._Values.InputFileName))
			{
				TTask tTask = this._ActualTask;
				tTask.Log.LogWarning(Resources.Input_file_is_empty__cannot_create_unmanaged_exports, new object[0]);
			}
			else if (File.Exists(this._Values.InputFileName))
			{
				string extension = Path.GetExtension(this._Values.InputFileName);
				char[] chrArray = new char[] { '.' };
				if (!string.Equals(extension.TrimStart(chrArray), "dll", StringComparison.OrdinalIgnoreCase))
				{
					TaskLoggingHelper log = this._ActualTask.Log;
					string inputFile0IsNotADLLHint = Resources.Input_file_0_is_not_a_DLL_hint;
					object[] inputFileName = new object[] { this._Values.InputFileName };
					log.LogMessage(inputFile0IsNotADLLHint, inputFileName);
				}
				result = true;
			}
			else
			{
				TaskLoggingHelper taskLoggingHelper = this._ActualTask.Log;
				string inputFile0DoesNotExist_cannotCreateUnmanagedExports = Resources.Input_file_0_does_not_exist__cannot_create_unmanaged_exports;
				object[] objArray = new object[] { this._Values.InputFileName };
				taskLoggingHelper.LogWarning(inputFile0DoesNotExist_cannotCreateUnmanagedExports, objArray);
			}
			return result;
		}

		private bool ValidateFrameworkPath()
		{
			string foundFrameworkPath;

			if (!this.ValidateToolPath("ilasm.exe", this.FrameworkPath, this.GetFrameworkToolPath, out foundFrameworkPath))
			{
				return false;
			}

			this.FrameworkPath = foundFrameworkPath;

			return true;
		}

		private bool ValidateInputValues()
		{
			bool result;

			result = true;

			//////this.ValidateLibToolPath();
			
			//////result = this.ValidateFrameworkPath();
			//////result &= this.ValidateSdkPath();

			//////if (!string.IsNullOrEmpty(this.CpuType) && (string.IsNullOrEmpty(this.Platform) || string.Equals(this.Platform, "anycpu", StringComparison.OrdinalIgnoreCase)))
			//////{
			//////	this.Platform = this.CpuType;
			//////}

			//////if (!string.IsNullOrEmpty(this.PlatformTarget) && (string.IsNullOrEmpty(this.Platform) || string.Equals(this.Platform, "anycpu", StringComparison.OrdinalIgnoreCase)))
			//////{
			//////	this.Platform = this.PlatformTarget;
			//////}

			//////result &= this.ValidateFileName();

			return result;
		}

		private void ValidateKeyFiles(bool isSigned)
		{
			if (isSigned && string.IsNullOrEmpty(this.KeyContainer) && string.IsNullOrEmpty(this.KeyFile))
			{
				TTask tTask = this._ActualTask;
				tTask.Log.LogWarning(Resources.Output_assembly_was_signed_however_neither_keyfile_nor_keycontainer_could_be_inferred__Reading_those_values_from_assembly_attributes_is_not__yet__supported__they_have_to_be_defined_inside_the_MSBuild_project_file, new object[0]);
			}
			if (!string.IsNullOrEmpty(this._Values.KeyContainer) && !string.IsNullOrEmpty(this._Values.KeyFile))
			{
				TaskLoggingHelper log = this._ActualTask.Log;
				string bothKeyValuesKeyContainer0AndKeyFile0ArePresentOnlyOneCanBeSpecified = Resources.Both_key_values_KeyContainer_0_and_KeyFile_0_are_present_only_one_can_be_specified;
				object[] keyContainer = new object[] { this._Values.KeyContainer, this._Values.KeyFile };
				log.LogError(bothKeyValuesKeyContainer0AndKeyFile0ArePresentOnlyOneCanBeSpecified, keyContainer);
			}
		}

		private bool ValidateLibToolPath()
		{
			string newLibToolPath = null;

			this.LibToolPath = Path.GetFullPath(Environment.ExpandEnvironmentVariables(this.LibToolPath));

			if (!ExportTaskImplementation<TTask>.PropertyHasValue(this.LibToolPath))
			{
				this.LibToolPath = null;

				string path = Environment.GetEnvironmentVariable("DevEnvDir").NullIfEmpty() ?? ExportTaskImplementation<TTask>.GetVsPath();

				if (ExportTaskImplementation<TTask>.PropertyHasValue(path))
				{
					if (!ExportTaskImplementation<TTask>.TrySearchToolPath(path, "lib.exe", out newLibToolPath) && !ExportTaskImplementation<TTask>.TrySearchToolPath(Path.Combine(path, "VC"), "lib.exe", out newLibToolPath) && !ExportTaskImplementation<TTask>.TrySearchToolPath(Path.Combine(Path.Combine(path, "VC"), "bin"), "lib.exe", out newLibToolPath))
					{
						TaskLoggingHelper log = this._ActualTask.Log;
						string cannotFindLibExeIn0_ = Resources.Cannot_find_lib_exe_in_0_;
						object[] libToolPath = new object[] { this.LibToolPath };
						log.LogMessage(MessageImportance.Low, cannotFindLibExeIn0_, libToolPath);
						this.LibToolPath = null;
						return true;
					}

					this.LibToolPath = newLibToolPath;
				}
			}
			else
			{
				if (!ExportTaskImplementation<TTask>.TrySearchToolPath(this.LibToolPath, "lib.exe", out newLibToolPath))
				{
					TaskLoggingHelper taskLoggingHelper = this._ActualTask.Log;
					string str = Resources.Cannot_find_lib_exe_in_0_;
					object[] objArray = new object[] { this.LibToolPath };
					taskLoggingHelper.LogMessage(MessageImportance.Normal, str, objArray);
					this.LibToolPath = null;
					return false;
				}

				this.LibToolPath = newLibToolPath;
			}

			if (!ExportTaskImplementation<TTask>.PropertyHasValue(newLibToolPath))
			{
				newLibToolPath = null;
			}

			if (!ExportTaskImplementation<TTask>.PropertyHasValue(this.LibToolDllPath))
			{
				if (ExportTaskImplementation<TTask>.PropertyHasValue(newLibToolPath))
				{
					DirectoryInfo d = new DirectoryInfo(newLibToolPath);
					while (d != null && !Directory.Exists(Path.Combine(Path.Combine(d.FullName, "Common7"), "IDE")))
					{
						d = d.Parent;
					}
					if (d != null)
					{
						string newLibToolDllPath = Path.Combine(Path.Combine(d.FullName, "Common7"), "IDE");
						if (Directory.Exists(newLibToolDllPath))
						{
							this.LibToolDllPath = newLibToolDllPath;
						}
					}
				}
				else
				{
					this.LibToolDllPath = null;
				}
			}

			return true;
		}

		private bool ValidateSdkPath()
		{
			string foundPath;

			if (!this.ValidateToolPath("ildasm.exe", this.SdkPath, this.GetSdkToolPath, out foundPath))
			{
				return false;
			}

			this.SdkPath = foundPath;

			return true;
		}

		private bool ValidateToolPath(string toolFileName, string currentValue, Func<Version, string, string> getToolPath, out string foundPath)
		{
			if (ExportTaskImplementation<TTask>.PropertyHasValue(this.TargetFrameworkVersion))
			{
				string newDir = currentValue;
				if (this.TryToGetToolDirForFxVersion(toolFileName, getToolPath, ref newDir))
				{
					foundPath = newDir;
					return true;
				}
			}

			if (ExportTaskImplementation<TTask>.PropertyHasValue(currentValue) && ExportTaskImplementation<TTask>.TrySearchToolPath(currentValue, toolFileName, out foundPath))
			{
				return true;
			}

			foundPath = null;

			return false;
		}

		private static Func<string, int, string> WrapGetToolPathCall<TTargetDotNetFrameworkVersion>(MethodInfo methodInfo)
		{
			Func<string, TTargetDotNetFrameworkVersion, string> func = (Func<string, TTargetDotNetFrameworkVersion, string>)Delegate.CreateDelegate(typeof(Func<string, TTargetDotNetFrameworkVersion, string>), methodInfo);
			return (string fileName, int versionValue) => {
				TTargetDotNetFrameworkVersion enumValue = (TTargetDotNetFrameworkVersion)Enum.ToObject(typeof(TTargetDotNetFrameworkVersion), versionValue);
				return func(fileName, enumValue);
			};
		}

		private sealed class DllExportNotifierWithTask : DllExportNotifier
		{
			public TTask ActualTask
			{
				get;
				private set;
			}

			public DllExportNotifierWithTask(TTask actualTask)
			{
				this.ActualTask = actualTask;
			}
		}
	}
}