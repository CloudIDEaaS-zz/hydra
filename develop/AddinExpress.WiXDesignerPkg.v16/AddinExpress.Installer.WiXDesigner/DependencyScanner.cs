using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Policy;

namespace AddinExpress.Installer.WiXDesigner
{
	[ComVisible(false)]
	public class DependencyScanner : MarshalByRefObject
	{
		private SortedList checkList = new SortedList();

		public DependencyScanner()
		{
		}

		public static Assembly AssemblyResolveEventHandler(object sender, ResolveEventArgs args)
		{
			return typeof(DependencyScanner).Assembly;
		}

		public static void GetAssemblyDependencies(string assemblyfile, ref string dependencies)
		{
			Evidence evidences = null;
			AppDomain appDomain = null;
			string fullPath = Path.GetFullPath(assemblyfile);
			DependencyScanner dependencyScanner = null;
			AppDomain currentDomain = null;
			try
			{
				AppDomainSetup appDomainSetup = new AppDomainSetup()
				{
					ShadowCopyFiles = "false",
					ApplicationBase = Path.GetDirectoryName(assemblyfile)
				};
				currentDomain = AppDomain.CurrentDomain;
				currentDomain.AssemblyResolve += new ResolveEventHandler(DependencyScanner.AssemblyResolveEventHandler);
				Guid guid = Guid.NewGuid();
				appDomain = AppDomain.CreateDomain(string.Concat("wixdesigner", guid.ToString("N")), evidences, appDomainSetup);
				if (appDomain == null)
				{
					throw new ApplicationException("Unable to create an instance of the AppDomain class.");
				}
				dependencyScanner = appDomain.CreateInstanceFromAndUnwrap(Assembly.GetAssembly(typeof(DependencyScanner)).Location, typeof(DependencyScanner).FullName) as DependencyScanner;
				if (dependencyScanner == null)
				{
					throw new ApplicationException("Unable to create proxy for the Registrator class.");
				}
				Exception exception = dependencyScanner.GetDependencies(fullPath, ref dependencies);
				if (exception != null)
				{
					throw exception;
				}
			}
			finally
			{
				if (currentDomain != null)
				{
					currentDomain.AssemblyResolve -= new ResolveEventHandler(DependencyScanner.AssemblyResolveEventHandler);
				}
				if (appDomain != null)
				{
					AppDomain.Unload(appDomain);
				}
			}
		}

		public static void GetAssemblyFiles(string assemblyfile, ref string files)
		{
			Evidence evidences = null;
			AppDomain appDomain = null;
			string fullPath = Path.GetFullPath(assemblyfile);
			DependencyScanner dependencyScanner = null;
			AppDomain currentDomain = null;
			try
			{
				AppDomainSetup appDomainSetup = new AppDomainSetup()
				{
					ShadowCopyFiles = "false",
					ApplicationBase = Path.GetDirectoryName(assemblyfile)
				};
				currentDomain = AppDomain.CurrentDomain;
				currentDomain.AssemblyResolve += new ResolveEventHandler(DependencyScanner.AssemblyResolveEventHandler);
				Guid guid = Guid.NewGuid();
				appDomain = AppDomain.CreateDomain(string.Concat("wixdesigner", guid.ToString("N")), evidences, appDomainSetup);
				if (appDomain == null)
				{
					throw new ApplicationException("Unable to create an instance of the AppDomain class.");
				}
				dependencyScanner = appDomain.CreateInstanceFromAndUnwrap(Assembly.GetAssembly(typeof(DependencyScanner)).Location, typeof(DependencyScanner).FullName) as DependencyScanner;
				if (dependencyScanner == null)
				{
					throw new ApplicationException("Unable to create proxy for the Registrator class.");
				}
				Exception exception = dependencyScanner.GetFiles(fullPath, ref files);
				if (exception != null)
				{
					throw exception;
				}
			}
			finally
			{
				if (currentDomain != null)
				{
					currentDomain.AssemblyResolve -= new ResolveEventHandler(DependencyScanner.AssemblyResolveEventHandler);
				}
				if (appDomain != null)
				{
					AppDomain.Unload(appDomain);
				}
			}
		}

		public Exception GetDependencies(string assemblyfile, ref string dependencies)
		{
			Exception exception = null;
			Assembly assembly = null;
			try
			{
				try
				{
					assembly = Assembly.ReflectionOnlyLoadFrom(assemblyfile);
					if (assembly != null)
					{
						SortedList sortedLists = new SortedList();
						AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies();
						for (int i = 0; i < (int)referencedAssemblies.Length; i++)
						{
							this.ScanDependencies(referencedAssemblies[i], sortedLists);
						}
						for (int j = 0; j < sortedLists.Count; j++)
						{
							if (!string.IsNullOrEmpty(dependencies))
							{
								dependencies = string.Concat(dependencies, ";");
							}
							dependencies = string.Concat(dependencies, sortedLists.GetValueList()[j]);
						}
					}
				}
				catch (Exception exception1)
				{
					exception = new Exception(exception1.Message);
				}
			}
			finally
			{
				assembly = null;
			}
			return exception;
		}

		public Exception GetFiles(string assemblyfile, ref string files)
		{
			Exception exception = null;
			Assembly assembly = null;
			try
			{
				try
				{
					assembly = Assembly.ReflectionOnlyLoadFrom(assemblyfile);
					if (assembly != null)
					{
						FileStream[] fileStreamArray = assembly.GetFiles();
						for (int i = 0; i < (int)fileStreamArray.Length; i++)
						{
							FileStream fileStream = fileStreamArray[i];
							if (!string.IsNullOrEmpty(files))
							{
								files = string.Concat(files, ";");
							}
							files = string.Concat(files, Path.GetFileName(fileStream.Name));
							fileStream.Close();
							fileStream.Dispose();
						}
					}
				}
				catch (Exception exception1)
				{
					exception = new Exception(exception1.Message);
				}
			}
			finally
			{
				assembly = null;
			}
			return exception;
		}

		private bool IsMicrosoftAssembly(string filePath)
		{
			Assembly assembly = null;
			try
			{
				assembly = Assembly.ReflectionOnlyLoadFrom(filePath);
			}
			catch
			{
			}
			if (assembly != null)
			{
				byte[] publicKeyToken = assembly.GetName().GetPublicKeyToken();
				if (publicKeyToken != null && (int)publicKeyToken.Length == 8)
				{
					byte[][] numArray = new byte[][] { new byte[] { 183, 122, 92, 86, 25, 52, 224, 137 }, new byte[] { 176, 63, 95, 127, 17, 213, 10, 58 }, new byte[] { 49, 191, 56, 86, 173, 54, 78, 53 } };
					for (int i = 0; i < (int)numArray.Length; i++)
					{
						if (publicKeyToken[0] == numArray[i][0] && publicKeyToken[1] == numArray[i][1] && publicKeyToken[2] == numArray[i][2] && publicKeyToken[3] == numArray[i][3] && publicKeyToken[4] == numArray[i][4] && publicKeyToken[5] == numArray[i][5] && publicKeyToken[6] == numArray[i][6] && publicKeyToken[7] == numArray[i][7])
						{
							bool product = true;
							try
							{
								assembly = Assembly.LoadFrom(filePath);
								product = (assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false)[0] as AssemblyProductAttribute).Product == "Microsoft® .NET Framework";
							}
							catch
							{
							}
							return product;
						}
					}
				}
			}
			return false;
		}

		private void ScanDependencies(AssemblyName assembly, SortedList dpnList)
		{
			if (assembly.FullName.ToLower().StartsWith("stdole,"))
			{
				return;
			}
			Assembly location = null;
			try
			{
				location = Assembly.ReflectionOnlyLoad(assembly.FullName);
			}
			catch
			{
			}
			if (location == null)
			{
				try
				{
					string str = assembly.FullName.Substring(0, assembly.FullName.IndexOf(','));
					location = Assembly.ReflectionOnlyLoadFrom(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Concat(str, ".dll")));
				}
				catch
				{
				}
			}
			if (location != null)
			{
				bool flag = false;
				if (this.IsMicrosoftAssembly(location.Location))
				{
					flag = true;
				}
				else if (!dpnList.ContainsKey(location.GetName().Name))
				{
					dpnList[location.GetName().Name] = location.Location;
				}
				if (!flag && !this.checkList.ContainsKey(location.GetName().Name))
				{
					this.checkList[location.GetName().Name] = location.Location;
					AssemblyName[] referencedAssemblies = location.GetReferencedAssemblies();
					for (int i = 0; i < (int)referencedAssemblies.Length; i++)
					{
						this.ScanDependencies(referencedAssemblies[i], dpnList);
					}
				}
			}
		}
	}
}