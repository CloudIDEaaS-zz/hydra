using DllExport.Parsing;
using DllExport.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Text;

namespace DllExport
{
	[PermissionSet(SecurityAction.LinkDemand, Name="FullTrust")]
	public sealed class DllExportWeaver : HasServiceProvider
	{
		private int _Timeout = 45000;
		internal AssemblyExports Exports { get; set; }
		internal AssemblyInitializers Initializers { get; set; }
		public IInputValues InputValues { get; set; }
		public int Timeout { get; set; }
		public string Files { get; set; }

		public DllExportWeaver(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		private static void CopyDirectory(string sourceDirectory, string destinationDirectory, bool overwrite = false)
		{
			sourceDirectory = DllExportWeaver.GetCleanedDirectoryPath(sourceDirectory);
			destinationDirectory = DllExportWeaver.GetCleanedDirectoryPath(destinationDirectory);
			if (Directory.Exists(destinationDirectory) && !overwrite)
			{
				throw new IOException(Resources.The_destination_directory_already_exists_);
			}
			if (!Directory.Exists(destinationDirectory))
			{
				Directory.CreateDirectory(destinationDirectory);
			}
			int sourceDirLength = sourceDirectory.Length + 1;
			string[] directories = Directory.GetDirectories(sourceDirectory, "*", SearchOption.AllDirectories);
			for (int i = 0; i < (int)directories.Length; i++)
			{
				string dirPath = directories[i];
				string newPath = Path.Combine(destinationDirectory, dirPath.Substring(sourceDirLength));
				if (!Directory.Exists(newPath))
				{
					Directory.CreateDirectory(newPath);
				}
			}
			string[] files = Directory.GetFiles(sourceDirectory, "*.*", SearchOption.AllDirectories);
			for (int j = 0; j < (int)files.Length; j++)
			{
				string filePath = files[j];
				string newPath = Path.Combine(destinationDirectory, filePath.Substring(sourceDirLength));
				File.Copy(filePath, newPath, overwrite);
			}
		}

		private static string GetCleanedDirectoryPath(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			DirectoryInfo di = new DirectoryInfo(Path.GetFullPath(path));
			if (di.Parent == null)
			{
				return di.FullName;
			}
			return Path.Combine(di.Parent.FullName, di.Name);
		}

		private IDllExportNotifier GetNotifier()
		{
			return base.ServiceProvider.GetService<IDllExportNotifier>();
		}

		private IlAsm PrepareIlAsm(string tempDirectory)
		{
			IlAsm ilAsm1;

			using (IDisposable disposable = this.GetNotifier().CreateContextName(this, "PrepareIlAsm"))
			{
				IlAsm ilAsm2 = new IlAsm(this, this.InputValues)
				{
					Timeout = this.Timeout
				};

				ilAsm1 = ilAsm2.TryInitialize<IlAsm>((IlAsm ilAsm) => {
					ilAsm.TempDirectory = tempDirectory;
					ilAsm.Exports = this.Exports;
					ilAsm.Initializers = this.Initializers;
					ilAsm.Files = this.Files;
				});
			}

			return ilAsm1;
		}

		public void Run()
		{
			if (this.Exports == null)
			{
				var inspector = Utilities.CreateAssemblyInspector(this.InputValues);

				this.Files = this.InputValues.Files;

				using (this.GetNotifier().CreateContextName(this, Resources.ExtractExportsContextName))
				{
					this.Exports = inspector.ExtractExports();
				}

				using (this.GetNotifier().CreateContextName(this, Resources.ExtractInitializersContextName))
				{
					this.Initializers = inspector.ExtractInitializers();
				}

				using (this.GetNotifier().CreateContextName(this, Resources.FindDuplicateExportMethodsContextName))
				{
					foreach (DuplicateExports duplicate in this.Exports.DuplicateExportMethods)
					{
						StringBuilder methodBuilder;

						if (duplicate.Duplicates.Count <= 0)
						{
							continue;
						}

						methodBuilder = (new StringBuilder(200)).AppendFormat("{0}.{1}", duplicate.UsedExport.ExportedClass.NullSafeCall<ExportedClass, string>((ExportedClass ec) => ec.FullTypeName), duplicate.UsedExport.Name);

						foreach (ExportedMethod method in duplicate.Duplicates)
						{
							methodBuilder.AppendFormat(", {0}.{1}", method.ExportedClass.NullSafeCall<ExportedClass, string>((ExportedClass ec) => ec.FullTypeName), method.Name);
						}
					}
				}
			}

			if (this.Exports.Count != 0 || this.Initializers.Count != 0)
			{
				using (this.GetNotifier().CreateContextName(this, Resources.CreateTempDirectoryContextName))
				{
					string testTempDirectory = null;

					if (this.InputValues.TestTempDirectory != null)
                    {
						testTempDirectory = this.InputValues.TestTempDirectory;
					}

					using (var tempDirectory = Utilities.CreateTempDirectory(testTempDirectory))
					{
						string[] strArrays;
						bool leaveIntermediateFiles;

						this.RunIlDasm(tempDirectory.Value);

						strArrays = new string[] { "true", "yes" };
						
						leaveIntermediateFiles = strArrays.Any<string>((string t) => t.Equals(this.InputValues.LeaveIntermediateFiles, StringComparison.InvariantCultureIgnoreCase));

						if (leaveIntermediateFiles)
						{
							using (this.GetNotifier().CreateContextName(this, Resources.CopyBeforeContextName))
							{
								DllExportWeaver.CopyDirectory(tempDirectory.Value, Path.Combine(Path.GetDirectoryName(this.InputValues.OutputFileName), "Before"), true);
							}
						}

						using (var ilAsm = this.PrepareIlAsm(tempDirectory.Value))
						{
							this.RunIlAsm(ilAsm);
						}

						if (leaveIntermediateFiles)
						{
							using (this.GetNotifier().CreateContextName(this, Resources.CopyAfterContextName))
							{
								DllExportWeaver.CopyDirectory(tempDirectory.Value, Path.Combine(Path.GetDirectoryName(this.InputValues.OutputFileName), "After"), true);
							}
						}
					}
				}
			}
		}

		private void RunIlAsm(IlAsm ilAsm)
		{
			using (IDisposable disposable = this.GetNotifier().CreateContextName(this, "RunIlAsm"))
			{
				if (this.InputValues.Cpu != CpuPlatform.AnyCpu)
				{
					ilAsm.ReassembleFile(this.InputValues.OutputFileName, "", this.InputValues.Cpu);
				}
				else
				{
					var fileDir = Path.GetDirectoryName(this.InputValues.OutputFileName) ?? "";
					var fileName = Path.GetFileName(this.InputValues.OutputFileName);
					string creatingBinariesForEachPlatform;
					string platformIs0CreatingBinariesForEachCPUPlatformInASeparateSubfolder;
					IDllExportNotifier notifier;
					object[] cpu;

					if (!Directory.Exists(fileDir))
					{
						throw new DirectoryNotFoundException(string.Format(Resources.Directory_0_does_not_exist, fileDir));
					}

					notifier = this.GetNotifier();
					creatingBinariesForEachPlatform = DllExportLogginCodes.CreatingBinariesForEachPlatform;
					platformIs0CreatingBinariesForEachCPUPlatformInASeparateSubfolder = Resources.Platform_is_0_creating_binaries_for_each_CPU_platform_in_a_separate_subfolder;
					cpu = new object[] { this.InputValues.Cpu };
					
					notifier.Notify(1, creatingBinariesForEachPlatform, platformIs0CreatingBinariesForEachCPUPlatformInASeparateSubfolder, cpu);
					
					ilAsm.ReassembleFile(Path.Combine(Path.Combine(fileDir, "x86"), fileName), ".x86", CpuPlatform.X86);
					ilAsm.ReassembleFile(Path.Combine(Path.Combine(fileDir, "x64"), fileName), ".x64", CpuPlatform.X64);
				}
			}
		}

		private void RunIlDasm(string tempDirectory)
		{
			using (this.GetNotifier().CreateContextName(this, "RunIlDasm"))
			{
				using (var ilDasm = new IlDasm(this, this.InputValues) { Timeout = this.Timeout })
				{
					ilDasm.TempDirectory = tempDirectory;

					ilDasm.Run();
				}
			}
		}
	}
}