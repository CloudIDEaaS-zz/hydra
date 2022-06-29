using DllExport.MSBuild.Properties;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DllExport.MSBuild
{
	internal static class AssemblyLoadingRedirection
	{
		public readonly static bool IsSetup;

		static AssemblyLoadingRedirection()
		{
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler((object sender, ResolveEventArgs args) => {
				AssemblyName asmName = new AssemblyName(args.Name);
				if ((new string[] { "Mono.Cecil", "DllExport" }).Contains<string>(asmName.Name))
				{
					string asmFileName = Path.Combine(Path.GetDirectoryName((new Uri(typeof(AssemblyLoadingRedirection).Assembly.EscapedCodeBase)).AbsolutePath), string.Concat(asmName.Name, ".dll"));
					if (File.Exists(asmFileName))
					{
						return Assembly.LoadFrom(asmFileName);
					}
				}
				return null;
			});
			AssemblyLoadingRedirection.IsSetup = true;
		}

		public static void EnsureSetup()
		{
			if (!AssemblyLoadingRedirection.IsSetup)
			{
				throw new InvalidOperationException(string.Format(Resources.AssemblyRedirection_for_0_has_not_been_setup_, typeof(AssemblyLoadingRedirection).FullName));
			}
		}
	}
}