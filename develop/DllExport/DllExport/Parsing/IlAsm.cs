using DllExport;
using DllExport.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;

namespace DllExport.Parsing
{
	[PermissionSet(SecurityAction.LinkDemand, Name="FullTrust")]
	public sealed class IlAsm : IlToolBase
	{
		private readonly static Regex _NormalizeIlErrorLineRegex;
		public AssemblyExports Exports { get; set; }
		public AssemblyInitializers Initializers { get; set; }
		public string Files { get; set; }

		static IlAsm()
		{
			IlAsm._NormalizeIlErrorLineRegex = new Regex("(?:\\n|\\s|\\t|\\r|\\-|\\:|\\,)+", RegexOptions.Compiled);
		}

		public IlAsm(IServiceProvider serviceProvider, IInputValues inputValues) : base(serviceProvider, inputValues)
		{
		}

		private string CreateDefFile(CpuPlatform cpu, string directory, string libraryName)
		{
			string result;
			object[] objArray = new object[] { libraryName, ".", cpu, ".def" };
			string defFileName = Path.Combine(directory, string.Concat(objArray));

			try
			{
				using (FileStream fs = new FileStream(defFileName, FileMode.Create))
				{
					using (StreamWriter wrt = new StreamWriter(fs, Encoding.UTF8))
					{
						wrt.WriteLine("LIBRARY {0}.dll", libraryName);
						wrt.WriteLine();
						wrt.WriteLine("EXPORTS");

						foreach (ExportedClass exportedClass in this.Exports.ClassesByName.Values)
						{
							foreach (ExportedMethod exportedMethod in exportedClass.Methods)
							{
								wrt.WriteLine(exportedMethod.ExportName);
							}
						}

						wrt.WriteLine();
						wrt.WriteLine("SECTIONS");
						wrt.WriteLine(".rdata READ WRITE");
					}
				}

				result = defFileName;
			}
			catch (Exception exception)
			{
				if (File.Exists(defFileName))
				{
					File.Delete(defFileName);
				}
				throw;
			}

			return result;
		}

		[Localizable(false)]
		private string GetCommandLineArguments(CpuPlatform cpu, string fileName, string ressourceParam, string ilSuffix, string keyFile)
		{
			object obj;
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			object[] objArray = new object[] { fileName, string.Concat(Path.Combine(base.TempDirectory, Path.GetFileNameWithoutExtension(base.InputValues.InputFileName)), ilSuffix), ressourceParam, null, null, null };
			objArray[3] = (base.InputValues.EmitDebugSymbols ? "/debug" : "/optimize");
			objArray[4] = (cpu == CpuPlatform.X86 ? "" : string.Concat(" /PE64 ", (cpu == CpuPlatform.Itanium ? " /ITANIUM" : " /X64")));
			object[] objArray1 = objArray;
			if (!string.IsNullOrEmpty(keyFile))
			{
				obj = string.Concat("\"/Key=", keyFile, '\"');
			}
			else if (!string.IsNullOrEmpty(base.InputValues.KeyContainer))
			{
				obj = string.Concat("\"/Key=@", base.InputValues.KeyContainer, "\"");
			}
			else
			{
				obj = null;
			}
			objArray1[5] = obj;
			return string.Format(invariantCulture, "/nologo \"/out:{0}\" \"{1}.il\" /DLL{2} {3} {4} {5}", objArray);
		}

		private static string GetLibraryFileNameRoot(string fileName)
		{
			string extension = Path.GetExtension(fileName);
			char[] chrArray = new char[] { '.' };
			if (!string.Equals(extension.TrimStart(chrArray), "dll", StringComparison.InvariantCultureIgnoreCase))
			{
				fileName = Path.GetFileName(fileName);
			}
			else
			{
				fileName = Path.GetFileNameWithoutExtension(fileName);
			}
			return fileName;
		}

		public int ReassembleFile(string outputFile, string ilSuffix, CpuPlatform cpu)
		{
			int result;
			var oldCurrentDir = Directory.GetCurrentDirectory();
			
			Directory.SetCurrentDirectory(base.TempDirectory);

			try
			{
				var fileDir = Path.GetDirectoryName(outputFile);

				if (fileDir != null && !Directory.Exists(fileDir))
				{
					Directory.CreateDirectory(fileDir);
				}

				using (var il = new IlParser(base.ServiceProvider))
				{
					List<string> ilFileLines;

					il.Exports = this.Exports;
					il.Initializers = this.Initializers;
					il.Files = this.Files;
					il.InputValues = base.InputValues;
					il.TempDirectory = base.TempDirectory;
					
					ilFileLines = new List<string>(il.GetLines(cpu));

					if (ilFileLines.Count > 0)
					{
						var lastLine = ilFileLines[ilFileLines.Count - 1];

						if (!lastLine.NullSafeCall<string, bool>((string l) => {

							if (l.EndsWith("\\r"))
							{
								return true;
							}

							return l.EndsWith("\\n");
						}))
						{
							ilFileLines[ilFileLines.Count - 1] = string.Concat(lastLine, Environment.NewLine);
						}
					}

					using (var stream = new FileStream(Path.Combine(base.TempDirectory, string.Concat(base.InputValues.FileName, ilSuffix, ".il")), FileMode.Create))
					{
						using (var writer = new StreamWriter(stream, Encoding.Unicode))
						{
							foreach (string line in ilFileLines)
							{
								writer.WriteLine(line);
							}
						}
					}
				}

				result = this.Run(outputFile, ilSuffix, cpu);
			}
			finally
			{
				Directory.SetCurrentDirectory(oldCurrentDir);
			}

			return result;
		}

		private int Run(string outputFile, string ilSuffix, CpuPlatform cpu)
		{
			int result;
			var builder = new StringBuilder(100);
			var files = Directory.GetFiles(base.TempDirectory, "*.res");

			for (int num = 0; num < (int)files.Length; num++)
			{
				string resFile = files[num];
				string extension = Path.GetExtension(resFile);
				char[] chrArray = new char[] { '.' };
				if (string.Equals(extension.NullSafeTrimStart(chrArray), "res", StringComparison.OrdinalIgnoreCase))
				{
					builder.Append(" \"/resource=").Append(resFile).Append("\" ");
				}
			}

			string ressourceParam = builder.ToString();

			if (string.IsNullOrEmpty(ressourceParam))
			{
				ressourceParam = " ";
			}

			string backupFile = "";

			if (string.Equals(base.InputValues.InputFileName, outputFile, StringComparison.OrdinalIgnoreCase))
			{
				string backupFileRoot = string.Concat(base.InputValues.InputFileName, ".bak");
				int i = 1;
				do
				{
					backupFile = string.Concat(backupFileRoot, i);
					i++;
				}
				while (File.Exists(backupFile));

				File.Move(base.InputValues.InputFileName, backupFile);
			}
			try
			{
				result = this.RunCore(cpu, outputFile, ressourceParam, ilSuffix);
			}
			finally
			{
				if (!string.IsNullOrEmpty(backupFile) && File.Exists(backupFile))
				{
					File.Delete(backupFile);
				}
			}

			return result;
		}

		private int RunCore(CpuPlatform cpu, string fileName, string ressourceParam, string ilSuffix)
		{
			string keyFile = null;

			if (!string.IsNullOrEmpty(base.InputValues.KeyFile))
			{
				keyFile = Path.GetFullPath(base.InputValues.KeyFile);
			}

			if (!string.IsNullOrEmpty(keyFile) && !File.Exists(keyFile))
			{
				if (!string.IsNullOrEmpty(base.InputValues.RootDirectory) && Directory.Exists(base.InputValues.RootDirectory))
				{
					keyFile = Path.Combine(base.InputValues.RootDirectory, base.InputValues.KeyFile);
				}
				if (!File.Exists(keyFile))
				{
					throw new FileNotFoundException(string.Format(Resources.Provided_key_file_0_cannot_be_found, keyFile));
				}
			}

			string directory = Path.GetFullPath(Path.GetDirectoryName(fileName));
			string arguments = this.GetCommandLineArguments(cpu, fileName, ressourceParam, ilSuffix, keyFile);

			var result = IlParser.RunIlTool(base.InputValues.FrameworkPath, "ILAsm.exe", null, null, "ILAsmPath", arguments, DllExportLogginCodes.IlAsmLogging, DllExportLogginCodes.VerboseToolLogging, base.Notifier, base.Timeout, (string line) => {
			
				int colonIndex = line.IndexOf(": ");

				if (colonIndex > 0)
				{
					line = line.Substring(colonIndex + 1);
				}

				return IlAsm._NormalizeIlErrorLineRegex.Replace(line, "").ToLowerInvariant().StartsWith("warningnonvirtualnonabstractinstancemethodininterfacesettosuch");
			});

			if (result == 0)
			{
				this.RunLibTool(cpu, fileName, directory);
			}

			return result;
		}

		private int RunLibTool(CpuPlatform cpu, string fileName, string directory)
		{
			int num;
			if (string.IsNullOrEmpty(base.InputValues.LibToolPath))
			{
				return 0;
			}
			string libraryName = IlAsm.GetLibraryFileNameRoot(fileName);
			string defFileName = this.CreateDefFile(cpu, directory, libraryName);
			try
			{
				try
				{
					num = this.RunLibToolCore(cpu, directory, defFileName);
				}
				catch (Exception exception)
				{
					Exception ex = exception;
					IDllExportNotifier notifier = base.Notifier;
					string libToolLooging = DllExportLogginCodes.LibToolLooging;
					string anErrorOccurredWhileCalling01_ = Resources.An_error_occurred_while_calling_0_1_;
					object[] message = new object[] { "lib.exe", ex.Message };
					notifier.Notify(1, libToolLooging, anErrorOccurredWhileCalling01_, message);
					num = -1;
				}
			}
			finally
			{
				if (File.Exists(defFileName))
				{
					File.Delete(defFileName);
				}
			}
			return num;
		}

		[Localizable(false)]
		private int RunLibToolCore(CpuPlatform cpu, string directory, string defFileName)
		{
			int result;
			string libToolDllPath;
			string libFilename = string.Concat(Path.Combine(directory, Path.GetFileNameWithoutExtension(base.InputValues.OutputFileName)), ".lib");
			try
			{
				if (string.IsNullOrEmpty(base.InputValues.LibToolDllPath) || !Directory.Exists(base.InputValues.LibToolDllPath))
				{
					libToolDllPath = null;
				}
				else
				{
					libToolDllPath = base.InputValues.LibToolDllPath;
				}
				string dllPath = libToolDllPath;
				result = IlParser.RunIlTool(base.InputValues.LibToolPath, "Lib.exe", dllPath, null, "LibToolPath", string.Format("\"/def:{0}\" /machine:{1} \"/out:{2}\"", defFileName, cpu, libFilename), DllExportLogginCodes.LibToolLooging, DllExportLogginCodes.LibToolVerboseLooging, base.Notifier, base.Timeout, null);
			}
			catch (Exception exception)
			{
				if (File.Exists(libFilename))
				{
					File.Delete(libFilename);
				}
				throw;
			}
			return result;
		}
	}
}