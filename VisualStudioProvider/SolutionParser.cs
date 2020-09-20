using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Metaspec;
using System.IO;
using System.Diagnostics;
using Hydra.GACManagedAccess;
using Utils;

namespace VisualStudioProvider
{
    public class SolutionParser
    {
		private Dictionary<string, IExternalAssemblyModule> modules;
        private ISolution solution;

		public ISolution Parse(string slnFilePath)
		{
            string slnFileText = File.ReadAllText(slnFilePath);

            ISolution solution = ISolutionFactory.create();
            //solution.setLogMessageCallback(log);
            solution.setErrorMessageCallback(log);

            SolutionCallback callback = new SolutionCallback();

            callback.LoadExternalAssemblyModuleCallback = new LoadExternalAssemblyModuleDelegate(loadExternalAssemblyModule2);
            callback.LoadTextFileCallback = new LoadTextFileDelegate(loadTextFile);
            callback.PropertyEvaluatorCallback = new PropertyEvaluatorDelegate(propertyEvaluator);
            callback.BetterAssemblyCallback = new BetterAssemblyDelegate(betterAssembly);

            modules = new Dictionary<string, IExternalAssemblyModule>();

            solution.loadSlnFile(
                slnFilePath,
                slnFileText.ToCharArray(),
                callback,
                project_namespace.pn_project_namespace
                );

            //apply configuration
            ISlnFileConfiguration[] configs = solution.getSlnFileConfigurations();
            string configuration = configs[0].getConfiguration();
            string platform = configs[0].getPlatform();
            solution.applyConfiguration(
                configuration,
                platform,
                callback,
                true,
                AssemblyTypeMapping.atm_default
                );

            //parse
            //solution.setParseAccessibleTypesAndMembersOnly(true);
            solution.parse();


            //modules = new Dictionary<string, IExternalAssemblyModule>();

            //string slnFileText = File.ReadAllText(slnFilePath);

            //ISolution solution = ISolutionFactory.create();
            ////solution.setLogMessageCallback(log);

            //solution.loadSlnFile(
            //    slnFilePath,
            //    slnFileText.ToCharArray(),
            //    loadTextFile,
            //    loadExternalAssemblyModule2,
            //    project_namespace.pn_project_namespace
            //    );

            ////apply configuration
            //ISlnFileConfiguration[] configs = solution.getSlnFileConfigurations();
            //string configuration = configs[0].getConfiguration();
            //string platform = configs[0].getPlatform();
            //solution.applyConfiguration(
            //    configuration,
            //    platform,
            //    loadTextFile,
            //    loadExternalAssemblyModule2,
            //    propertyEvaluator,
            //    betterAssembly,
            //    true,
            //    AssemblyTypeMapping.atm_default
            //    );

            ////parse
            ////solution.setParseAccessibleTypesAndMembersOnly(true);
            ////solution.parse();

            return solution;
        }

        string getName(string path)
        {
            var fileInfo = new FileInfo(path);

            return fileInfo.Name;
        }

		void log(string s)
		{
			Debug.WriteLine(s);
			Console.WriteLine(s);
		}

		CharFileInfo loadTextFile(IFileOwner fileOwner, string path)
		{
			CharFileInfo cfi = new CharFileInfo();

			string basePath = Path.GetDirectoryName(fileOwner.getPath());
			string fullPath = Path.GetFullPath(Path.Combine(basePath, path));
			if (path.EndsWith(".g.cs")
				&& !File.Exists(fullPath))
			{
				return null;
			}

			if (!File.Exists(fullPath))
			{
				return null;
			}
			string text = File.ReadAllText(fullPath);
			int startIndex = 0;
			int length = text.Length;
			if (text.Length >= 1
				&& text[0] == '\xfeff')
			{
				++startIndex;
				--length;
			}

			cfi.chars = text.ToCharArray(startIndex, length);
			cfi.path = fullPath;

			return cfi;
		}

		class BetterAssemblyInfo
		{
			public Metaspec.AssemblyInfo assemblyInfo;
			public string sAssemblyInfo;
			public bool worse;

			public BetterAssemblyInfo(Metaspec.AssemblyInfo assemblyInfo, string sAssemblyInfo)
			{
				this.assemblyInfo = assemblyInfo;
				this.sAssemblyInfo = sAssemblyInfo;
			}
		}

		//The rules Visual Studio uses to resolve references are as follows:
		//1. Assemblies that are visible in the project as project items or links are considered.
		//   If Visual Studio .NET 2005 with MSBuild is being used, these must have a Build Action of Content or None.
		//2. Assemblies in Reference Path directories are considered.
		//   These are stored in .user files and are visible under project properties.
		//3. The HintPath of the reference is considered.
		//   This is a path to the referenced assembly (relative to the project).
		//   It is stored when the reference is originally created.
		//4. Assemblies in the native framework directory are considered
		//   (e.g., \Windows\Microsoft.NET\Framework\v1.1.4322 for Visual Studio .NET 2003).
		//5. Assemblies in the registered assembly folders are considered.
		//   These are the directories discussed in the last section about adding assemblies to the list of .NET assemblies.
		//   If Visual Studio .NET 2005 with MSBuild is being used,
		//   HKLM\Software\Microsoft\ .NETFramework\v2.x.xxxxx\AssemblyFoldersEx will be considered first.
		//6. If Visual Studio .NET 2005 with MSBuild is being used and the assembly has a strong name,
		//   Visual Studio will look in the GAC for the assembly.

		IExternalAssemblyModuleInfo loadExternalAssemblyModule2(
			IExternalAssemblyOwner externalAssemblyOwner,
			ExternalAssemblyReferenceInfo externalAssemblyReferenceInfo
		)
		{
			string path = null;

			if (externalAssemblyReferenceInfo.moduleFrom == null)
			{
				//load assembly
				//priority:
				//1.HintPath
				//2.include
				//3.NET Framework directories
				//4.AssemblyFoldersEx directories
				//5.GAC

				//Hint path
				path = getPathAndCheckExists(externalAssemblyOwner, externalAssemblyReferenceInfo.hintPath);

				//include
				if (path == null
					&& externalAssemblyReferenceInfo.assemblyInfo == null)
				{
					path = getPathAndCheckExists(externalAssemblyOwner, externalAssemblyReferenceInfo.include);
				}

				//reference path (.user file)
				if (path == null
					&& externalAssemblyReferenceInfo.referencePath != null
					&& externalAssemblyReferenceInfo.include != null
					&& externalAssemblyReferenceInfo.include.Contains(".dll"))
				{
					string s = Path.Combine(externalAssemblyReferenceInfo.referencePath, externalAssemblyReferenceInfo.include);
					path = getPathAndCheckExists(externalAssemblyOwner, s);
				}

				//.NET Framework directories
				if (path == null
					&& externalAssemblyReferenceInfo.assemblyInfo != null
					&& externalAssemblyReferenceInfo.assemblyInfo.Name != null)
				{
					string frameworkDir;

					if (string.Equals(externalAssemblyReferenceInfo.assemblyInfo.ProcessorArchitecture, "X86", StringComparison.InvariantCultureIgnoreCase))
					{
						frameworkDir = @"C:\Windows\Microsoft.NET\Framework";
					}
					else if (string.Equals(externalAssemblyReferenceInfo.assemblyInfo.ProcessorArchitecture, "AMD64", StringComparison.InvariantCultureIgnoreCase))
					{
						frameworkDir = @"C:\Windows\Microsoft.NET\Framework64";
					}
					else
					{
						unsafe
						{
							if (sizeof(IntPtr) == 4)
							{
								frameworkDir = @"C:\Windows\Microsoft.NET\Framework";
							}
							else
							{
								frameworkDir = @"C:\Windows\Microsoft.NET\Framework64";
							}
						}
					}

					string requiredTargetFramework = externalAssemblyReferenceInfo.solutionRequiredTargetFramework;
					AssemblyVersion av;
					if (requiredTargetFramework != null)
					{
						av = getMinAssemblyVersionFromRequiredTargetFramework(requiredTargetFramework);
					}
					else
					{
						switch (externalAssemblyReferenceInfo.vs_version)
						{
							case Metaspec.vs_version.vsv_vs2002:
								av = new AssemblyVersion(1, 0, 0, 0);
								break;
							case Metaspec.vs_version.vsv_vs2003:
								av = new AssemblyVersion(1, 1, 0, 0);
								break;
							case Metaspec.vs_version.vsv_vs2005:
								av = new AssemblyVersion(2, 0, 0, 0);
								break;
							case Metaspec.vs_version.vsv_vs2008:
								av = new AssemblyVersion(3, 5, 0, 0);
								break;
							case Metaspec.vs_version.vsv_vs2010:
								av = new AssemblyVersion(4, 0, 0, 0);
								break;
							default:
								av = new AssemblyVersion(0, 0, 0, 0);
								break;
						}
					}
					if (externalAssemblyReferenceInfo.assemblyInfo.Version > av)
					{
						av = externalAssemblyReferenceInfo.assemblyInfo.Version;
					}

					vs_version vs_version = externalAssemblyReferenceInfo.vs_version;
					if (av.MajorVersion >= 4)
					{
						vs_version = Metaspec.vs_version.vsv_vs2010;
					}
					else if (av.MajorVersion >= 3
						&& av.MinorVersion >= 5)
					{
						vs_version = Metaspec.vs_version.vsv_vs2008;
					}
					else if (av.MajorVersion >= 2)
					{
						vs_version = Metaspec.vs_version.vsv_vs2005;
					}
					else if (av.MajorVersion >= 1
						&& av.MinorVersion >= 1)
					{
						vs_version = Metaspec.vs_version.vsv_vs2003;
					}
					else if (av.MajorVersion >= 1)
					{
						vs_version = Metaspec.vs_version.vsv_vs2002;
					}

					bool loadFromAssembliesFoldersFlag = false;

					{
						while (vs_version != vs_version.vsv_unknown)
						{
							string dotnetVersionDir = null;

							dotnetVersionDir = GetNetVersionDirectory(vs_version);

							if (dotnetVersionDir != null)
							{
								string s = Path.Combine(frameworkDir, dotnetVersionDir);
								s = Path.Combine(s, externalAssemblyReferenceInfo.assemblyInfo.Name + ".dll");
								path = getPathAndCheckExists(externalAssemblyOwner, s);
								if (path != null)
									break;
							}

							if (path == null
								&& !loadFromAssembliesFoldersFlag)
							{
								//AssembliesFolders
								path = LoadFromAssembliesFolders(externalAssemblyOwner, externalAssemblyReferenceInfo, vs_version);
								if (path != null)
									break;
								loadFromAssembliesFoldersFlag = true;
							}

							--vs_version;
						}
					}
				}

				//GAC
				if (path == null)
				{
					if (externalAssemblyReferenceInfo.assemblyInfo != null)
					{
						List<BetterAssemblyInfo> betterAssemblyInfo = new List<BetterAssemblyInfo>();

						AssemblyCacheEnum ace = new AssemblyCacheEnum(externalAssemblyReferenceInfo.assemblyInfo.Name);
						bool presentInCache = false;
						while (true)
						{
							string s = ace.GetNextAssembly();
							if (string.IsNullOrEmpty(s))
							{
								break;
							}
							presentInCache = true;

							Metaspec.AssemblyInfo ai = Metaspec.AssemblyInfo.parse(s);

							if (ai != null)
							{
								betterAssemblyInfo.Add(new BetterAssemblyInfo(ai, s));
							}
						}

						string sAssemblyInfo = selectBetterAssembly(externalAssemblyReferenceInfo, betterAssemblyInfo);
						if (sAssemblyInfo != null)
						{
							path = AssemblyCache.QueryAssemblyInfo(sAssemblyInfo);
						}
					}
				}

				//OutputPath
				if (path == null
					&& externalAssemblyReferenceInfo.outputPath != null
					&& (externalAssemblyReferenceInfo.assemblyInfo.Name != null
						|| (externalAssemblyReferenceInfo.include != null)
							&& externalAssemblyReferenceInfo.include.Contains(".dll")))
				{
					if (externalAssemblyReferenceInfo.assemblyInfo.Name != null)
					{
						string s = Path.Combine(externalAssemblyReferenceInfo.outputPath, externalAssemblyReferenceInfo.assemblyInfo.Name + ".dll");
						path = getPathAndCheckExists(externalAssemblyOwner, s);
					}
					if (path == null
						&& externalAssemblyReferenceInfo.include != null
						&& externalAssemblyReferenceInfo.include.Contains(".dll"))
					{
						string s = Path.Combine(externalAssemblyReferenceInfo.outputPath, externalAssemblyReferenceInfo.include);
						path = getPathAndCheckExists(externalAssemblyOwner, s);
					}
				}
			}
			else
			{
				//load module
				path = Path.GetDirectoryName(externalAssemblyReferenceInfo.moduleFrom.getPath());
				path = Path.Combine(path, externalAssemblyReferenceInfo.modulePath);
			}

			if (path == null
				|| !File.Exists(path))
			{
				return null;
			}

			IExternalAssemblyModuleInfo module_info = new IExternalAssemblyModuleInfo();
			module_info.shared_assembly = true;

			string pathUpper = path.ToUpper();
			if (!modules.ContainsKey(pathUpper))
			{
				byte[] bytes = File.ReadAllBytes(path);
				IExternalAssemblyModule module = IExternalAssemblyModuleFactory.create(bytes, path);
				module_info.module = module;
				modules.Add(pathUpper, module);
			}
			else
			{
				module_info.module = modules[pathUpper];
			}

			return module_info;
		}

		private string GetNetVersionDirectory(vs_version vs_version)
		{
			string dotnetVersionDir = null;

			switch (vs_version)
			{
				case vs_version.vsv_vs2002:
					dotnetVersionDir = "v1.0.3705";
					break;
				case vs_version.vsv_vs2003:
					dotnetVersionDir = "v1.1.4322";
					break;
				case vs_version.vsv_vs2005:
					dotnetVersionDir = "v2.0.50727";
					break;
				case vs_version.vsv_vs2008:
					dotnetVersionDir = "v3.5";
					break;
				case vs_version.vsv_vs2010:
					dotnetVersionDir = "v4.0.30319";
					break;
			}

			return dotnetVersionDir;
		}

		private string GetReferenceNetVersionDirectory(vs_version vs_version)
		{
			string dotnetVersionDir = null;

			switch (vs_version)
			{
				//case vs_version.vsv_vs2002:
				//  dotnetVersionDir = "v1.0.3705";
				//  break;
				//case vs_version.vsv_vs2003:
				//  dotnetVersionDir = "v1.1.4322";
				//  break;
				//case vs_version.vsv_vs2005:
				//  dotnetVersionDir = "v2.0.50727";
				//  break;
				case vs_version.vsv_vs2008:
					dotnetVersionDir = "v3.5";
					break;
				case vs_version.vsv_vs2010:
					dotnetVersionDir = "v4.0";
					break;
			}

			return dotnetVersionDir;
		}

		private string LoadFromAssembliesFolders(
			IExternalAssemblyOwner externalAssemblyOwner,
			ExternalAssemblyReferenceInfo externalAssemblyReferenceInfo,
			vs_version vs_version
			)
		{
			string netReferenceDirectory = GetReferenceNetVersionDirectory(vs_version);
			if (netReferenceDirectory != null)
			{
				netReferenceDirectory = Path.Combine(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\", netReferenceDirectory);
				string path = LoadFromDirectory(externalAssemblyOwner, externalAssemblyReferenceInfo, netReferenceDirectory);
				if (path != null)
					return path;
			}

			List<string> registryFolders = new List<string>();
			switch (externalAssemblyReferenceInfo.vs_version)
			{
				case vs_version.vsv_vs2010:
					registryFolders.Add(@"Software\Wow6432Node\Microsoft\.NETFramework\v4.0.30319\AssemblyFoldersEx");
					break;
			}
			registryFolders.Add(@"Software\Microsoft\.NetFramework\AssemblyFolders");

			foreach (string registryFolder in registryFolders)
			{
				string path = LoadFromAssembliesFolders(externalAssemblyOwner, externalAssemblyReferenceInfo, registryFolder);
				if (path != null)
				{
					return path;
				}
			}
			return null;
		}

		private string LoadFromAssembliesFolders(
			IExternalAssemblyOwner externalAssemblyOwner,
			ExternalAssemblyReferenceInfo externalAssemblyReferenceInfo,
			string registryFolder
		)
		{
			Microsoft.Win32.RegistryKey rkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(registryFolder, false);
			if (rkey != null)
			{
				string[] subkeys = rkey.GetSubKeyNames();
				foreach (string sk in subkeys)
				{
					Microsoft.Win32.RegistryKey skey = rkey.OpenSubKey(sk, false);
					string valueName = "";
					if (skey.GetValue(valueName) == null)
					{
						valueName = "All Assemblies In";
					}
					if (skey.GetValue(valueName) != null)
					{
						if (skey.GetValueKind(valueName) == Microsoft.Win32.RegistryValueKind.String)
						{
							string value = skey.GetValue(valueName, "").ToString();
							if (!string.IsNullOrEmpty(value))
							{
								string path = LoadFromDirectory(externalAssemblyOwner, externalAssemblyReferenceInfo, value);
								if (path != null)
								{
									return path;
								}
							}
						}
					}
				}
			}
			return null;
		}

		private string LoadFromDirectory(
			IExternalAssemblyOwner externalAssemblyOwner,
			ExternalAssemblyReferenceInfo externalAssemblyReferenceInfo,
			string directory
			)
		{
			string s = Path.Combine(directory, externalAssemblyReferenceInfo.assemblyInfo.Name + ".dll");
			string path = getPathAndCheckExists(externalAssemblyOwner, s);
			if (path != null)
			{
				return path;
			}
			return null;
		}

		private string getPath(IExternalAssemblyOwner externalAssemblyOwner, string relativePath)
		{
			string path = null;

			if (relativePath != null)
			{
				if (Path.IsPathRooted(relativePath))
				{
					path = relativePath;
				}
				else
				{
                    if (relativePath.Contains(@"\..\") && relativePath.Contains(@"\Windows\Microsoft.NET\assembly\GAC_MSIL\"))
                    {
                        var rootDirectory = new DirectoryInfo(externalAssemblyOwner.getPath());
                        var index = relativePath.IndexOf(@"\Windows\Microsoft.NET\assembly\GAC_MSIL\");

                        path = rootDirectory.FullName.Substring(0, 2) + relativePath.Substring(index);
                    }
                    else if (relativePath.Contains(@"\..\") && relativePath.Contains(@"\Program Files\"))
                    {
                        var rootDirectory = new DirectoryInfo(externalAssemblyOwner.getPath());
                        var index = relativePath.IndexOf(@"\Program Files\");

                        path = rootDirectory.FullName.Substring(0, 2) + relativePath.Substring(index);
                    }
                    else if (relativePath.Contains(@"\..\") && relativePath.Contains(@"\Program Files (x86)\"))
                    {
                        var rootDirectory = new DirectoryInfo(externalAssemblyOwner.getPath());
                        var index = relativePath.IndexOf(@"\Program Files (x86)\");

                        path = rootDirectory.FullName.Substring(0, 2) + relativePath.Substring(index);
                    }
                    else if (relativePath.Contains(@"%Program Files%"))
                    {
                        var rootDirectory = new DirectoryInfo(externalAssemblyOwner.getPath());
                        var index = relativePath.IndexOf(@"%Program Files%");

                        path = relativePath.Substring(index);
                    }
                    else if (relativePath.Contains(@"%Windir%"))
                    {
                        var rootDirectory = new DirectoryInfo(externalAssemblyOwner.getPath());
                        var index = relativePath.IndexOf(@"%Windir%");

                        path = relativePath.Substring(index);
                    }
                    else if (relativePath.Contains(@"%Windows%"))
                    {
                        Debugger.Break();
                    }
                    else
                    {
                        path = Path.Combine(Path.GetDirectoryName(externalAssemblyOwner.getPath()), relativePath);
                        path = Path.GetFullPath(path);
                    }
				}
			}

            if (path != null)
            {
                return path.Expand();
            }
            else
            {
                return path;
            }
		}

		private string getPathAndCheckExists(IExternalAssemblyOwner externalAssemblyOwner, string hintpath)
		{
			string path = getPath(externalAssemblyOwner, hintpath);

			if (path != null
				&& !File.Exists(path))
			{
				path = null;
			}

			return path;
		}

		private bool isApplicableAssembly(
			Metaspec.AssemblyInfo assemblyInfo,
			ExternalAssemblyReferenceInfo csProjAssemblyReferenceInfo
			)
		{
			//system dll
			if (csProjAssemblyReferenceInfo.assemblyInfo.getVersion().isNull())
			{
				string requiredTargetFramework = csProjAssemblyReferenceInfo.solutionRequiredTargetFramework;
				if (requiredTargetFramework != null)
				{
					AssemblyVersion av = getMinAssemblyVersionFromRequiredTargetFramework(requiredTargetFramework);

					if (assemblyInfo.getVersion() > av)
					{
						return false;
					}
				}
			}

			if (csProjAssemblyReferenceInfo.specificVersion)
			{
				if (assemblyInfo.Version.MajorVersion != csProjAssemblyReferenceInfo.assemblyInfo.Version.MajorVersion
					|| assemblyInfo.Version.MinorVersion != csProjAssemblyReferenceInfo.assemblyInfo.Version.MinorVersion
					|| assemblyInfo.Version.BuildNumber != csProjAssemblyReferenceInfo.assemblyInfo.Version.BuildNumber
					|| assemblyInfo.Version.RevisionNumber != csProjAssemblyReferenceInfo.assemblyInfo.Version.RevisionNumber)
				{
					return false;
				}
			}

			return true;
		}

		string selectBetterAssembly(
			ExternalAssemblyReferenceInfo csProjAssemblyReferenceInfo,
			List<BetterAssemblyInfo> betterAssemblyInfo
			)
		{
			for (int i = 0; i != betterAssemblyInfo.Count; ++i)
			{
				if (betterAssemblyInfo[i].worse)
				{
					continue;
				}
				for (int j = 0; j != betterAssemblyInfo.Count; ++j)
				{
					if (i == j)
						continue;

					int better = selectBetterAssembly(
						csProjAssemblyReferenceInfo,
						betterAssemblyInfo[i].assemblyInfo,
						betterAssemblyInfo[j].assemblyInfo
						);

					switch (better)
					{
						case 1:
							betterAssemblyInfo[j].worse = true;
							break;
						case 2:
							betterAssemblyInfo[i].worse = true;
							goto next_i;
					}
				}
				return betterAssemblyInfo[i].sAssemblyInfo;

			next_i:
				;
			}

			return null;
		}

		int selectBetterAssembly(
			ExternalAssemblyReferenceInfo csProjAssemblyReferenceInfo,
			Metaspec.AssemblyInfo assemblyInfo1,
			Metaspec.AssemblyInfo assemblyInfo2
			)
		{
			bool ap1 = isApplicableAssembly(assemblyInfo1, csProjAssemblyReferenceInfo);
			bool ap2 = isApplicableAssembly(assemblyInfo2, csProjAssemblyReferenceInfo);
			if (ap1 != ap2)
			{
				if (ap1)
					return 1;
				else
					return 2;
			}

			bool csProjAssemblyReferenceInfoAssemblyInfoIsNull = csProjAssemblyReferenceInfo.assemblyInfo.getVersion().isNull();
			string requiredTargetFramework = csProjAssemblyReferenceInfo.solutionRequiredTargetFramework;
			AssemblyVersion av1 = assemblyInfo1.getVersion();
			AssemblyVersion av2 = assemblyInfo2.getVersion();

			//system dll
			if (csProjAssemblyReferenceInfoAssemblyInfoIsNull
				&& requiredTargetFramework != null)
			{
				AssemblyVersion av = getMinAssemblyVersionFromRequiredTargetFramework(requiredTargetFramework);
				bool g1 = av1 >= av;
				bool g2 = av2 >= av;
				if (g1 != g2)
				{
					if (g1)
					{
						return 1;
					}
					else
					{
						return 2;
					}
				}
			}

			//user dll
			else
			{
				AssemblyVersion av = csProjAssemblyReferenceInfo.assemblyInfo.getVersion();

				bool g1 = av1 >= av;
				bool g2 = av2 >= av;
				if (g1 != g2)
				{
					if (g1)
					{
						return 1;
					}
					else
					{
						return 2;
					}
				}
				if (g1 && g2)
				{
					if (av1 > av2)
						return 1;
					else if (av2 > av1)
						return 2;
				}
			}

			string processorArhitecture;
			if (csProjAssemblyReferenceInfo.assemblyInfo.ProcessorArchitecture != null)
			{
				processorArhitecture = csProjAssemblyReferenceInfo.assemblyInfo.ProcessorArchitecture;
			}
			else
			{
				processorArhitecture = System.Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
			}
			if (assemblyInfo1.ProcessorArchitecture == processorArhitecture)
			{
				return 1;
			}
			else if (assemblyInfo2.ProcessorArchitecture == processorArhitecture)
			{
				return 2;
			}

			return 0;
		}

		AssemblyVersion getMinAssemblyVersionFromRequiredTargetFramework(string requiredTargetFramework)
		{
			AssemblyVersion av = new AssemblyVersion();

			if (requiredTargetFramework.Length >= 1
				&& requiredTargetFramework[0] == 'v')
			{
				requiredTargetFramework = requiredTargetFramework.Substring(1);
			}

			switch (requiredTargetFramework)
			{
				case "4.0":
					av.MajorVersion = 4;
					break;
				case "3.5":
					av.MajorVersion = 3;
					av.MinorVersion = 5;
					break;
				case "3.0":
					av.MajorVersion = 3;
					break;
				case "2.0":
					av.MajorVersion = 2;
					break;
				case "1.1":
					av.MajorVersion = 1;
					av.MinorVersion = 1;
					break;
				case "1.0":
					av.MajorVersion = 1;
					break;
			}

			return av;
		}

		string propertyEvaluator(string property)
		{
			switch (property)
			{
				case "DevEnvDir":
					{
						return @"c:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\IDE";
					}
				case "WindowsBaseRef":
					{
						return @"WindowsBase";
					}
				case "UnitTestLibHintPath":
					{
						return @"Microsoft.VisualStudio.QualityTools.UnitTestFramework";
					}
			}
			return null;
		}

		IExternalAssembly betterAssembly(Metaspec.AssemblyInfo info, IExternalAssembly[] assemblies, bool any)
		{
			AssemblyVersion version = info.Version;
			bool system = false;
			if (info.Name == "mscorlib"
				|| info.Name == "System.Core")
			{
				version.MinorVersion = 0;
				version.BuildNumber = 0;
				version.RevisionNumber = 0;
				system = true;
			}

			bool bestFound = true;
			for (int i = 0; i != assemblies.Length; ++i)
			{
				bestFound = true;
				for (int j = 0; j != assemblies.Length; ++j)
				{
					if (i == j)
						continue;

					int better = 0;
					Metaspec.AssemblyInfo ai = getAssemblyKey(assemblies[i]);
					Metaspec.AssemblyInfo aj = getAssemblyKey(assemblies[j]);
					if (ai != null
						&& aj != null)
					{
						better = betterAssembly2(info.Version, ai.Version, aj.Version, system);
					}
					if (better != 1)
					{
						bestFound = false;
						break;
					}
				}
				if (bestFound)
				{
					return assemblies[i];
				}
			}

			if (any)
			{
				return assemblies[0];
			}

			return null;
		}

		Metaspec.AssemblyInfo getAssemblyKey(IExternalAssembly assembly)
		{
			Metaspec.AssemblyInfo key = null;
			Metaspec.AssemblyInfo[] keys = assembly.getAssemblyKeys();
			if (keys != null
				&& keys.Length >= 1)
			{
				key = keys[0];
			}
			return key;
		}

		int betterAssembly2(AssemblyVersion av, AssemblyVersion av1, AssemblyVersion av2, bool system)
		{
			if (av1 == av2)
				return 0;
			if (av == av1)
				return 1;
			if (av == av2)
				return 2;
			bool g1 = av1 >= av;
			bool g2 = av2 >= av;
			if (g1 != g2)
			{
				if (g1)
				{
					return 1;
				}
				else
				{
					return 2;
				}
			}
			if (system)
			{
				if (av1 < av2)
					return 1;
				else
					return 2;
			}
			return 0;
		}
	}
}
