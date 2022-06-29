using Microsoft.Win32;
using DllExport.Properties;
using System;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DllExport
{
	public static class Utilities
	{
		public readonly static string DllExportAttributeAssemblyName;
		public readonly static string DllExportAttributeFullName;
		public readonly static string ModuleInitializerAttributeAssemblyName;
		public readonly static string ModuleInitializerAttributeFullName;

		static Utilities()
		{
			Utilities.DllExportAttributeAssemblyName = "DllExport.Metadata";
			Utilities.DllExportAttributeFullName = "DllExport.DllExportAttribute";
			Utilities.ModuleInitializerAttributeAssemblyName = "DllExport.Metadata";
			Utilities.ModuleInitializerAttributeFullName = "DllExport.ModuleInitializerAttribute";
		}

		internal static IAssemblyInspector CreateAssemblyInspector(IInputValues inputValues)
		{
			return new AssemblyInspector(inputValues);
		}

		public static ValueDisposable<string> CreateTempDirectory(string testTempDirectory = null)
		{
			return new ValueDisposable<string>(Utilities.CreateTempDirectoryCore(testTempDirectory), (string dir) => Directory.Delete(dir, true));
		}

		private static string CreateTempDirectoryCore(string testTempDirectory = null)
		{
			string str = null;

			if (testTempDirectory != null)
            {
				return testTempDirectory;
            }

			try
			{
				string tempFileName = Path.GetTempFileName();
				
				if (!string.IsNullOrEmpty(tempFileName) && File.Exists(tempFileName))
				{
					File.Delete(tempFileName);
				}

				string path = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(tempFileName)), Path.GetFileNameWithoutExtension(tempFileName));
				Directory.CreateDirectory(path);
				str = path;
			}
			catch
			{
				if (!string.IsNullOrEmpty(str) && Directory.Exists(str))
				{
					Directory.Delete(str, true);
				}
				throw;
			}
			return str;
		}

		public static int GetCoreFlagsForPlatform(CpuPlatform cpu)
		{
			if (cpu != CpuPlatform.X86)
			{
				return 0;
			}
			return 2;
		}

		public static MethodInfo GetMethodInfo<TResult>(Expression<Func<TResult>> expression)
		{
			return ((MethodCallExpression)expression.Body).Method;
		}

		public static string GetSdkPath(Version frameworkVersion)
		{
			string str;
			using (RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\.NETFramework", false))
			{
				if (key != null)
				{
					string sdkRoot = key.GetValue(string.Concat("sdkInstallRootv", frameworkVersion.ToString(2)), "").NullSafeToString();
					if (string.IsNullOrEmpty(sdkRoot))
					{
						str = null;
					}
					else
					{
						str = sdkRoot;
					}
				}
				else
				{
					str = null;
				}
			}
			return str;
		}

		public static string GetSdkPath()
		{
			string systemVersion = RuntimeEnvironment.GetSystemVersion();
			char[] chrArray = new char[] { 'v' };
			Version v = new Version(systemVersion.NullSafeTrimStart(chrArray));
			return Utilities.GetSdkPath(v);
		}

		public static CpuPlatform ToCpuPlatform(string platformTarget)
		{
			if (!string.IsNullOrEmpty(platformTarget))
			{
				string text = platformTarget.NullSafeToLowerInvariant();
				if (text != null)
				{
					if (text == "anycpu")
					{
						return CpuPlatform.AnyCpu;
					}
					if (text == "x86")
					{
						return CpuPlatform.X86;
					}
					if (text == "x64")
					{
						return CpuPlatform.X64;
					}
					if (text == "ia64")
					{
						return CpuPlatform.Itanium;
					}
				}
			}
			throw new ArgumentException(string.Format(Resources.Unknown_cpu_platform_0_, platformTarget), "platformTarget");
		}

		public static T TryInitialize<T>(this T instance, Action<T> call)
		where T : IDisposable
		{
			T t;
			try
			{
				call(instance);
				t = instance;
			}
			catch (Exception exception)
			{
				instance.Dispose();
				throw;
			}
			return t;
		}
	}
}