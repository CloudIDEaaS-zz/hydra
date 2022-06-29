using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Assemblies;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class ProjectDescriptor : IDisposable
	{
		private bool disposed;

		private IVsProject project;

		private VsWiXProject.ReferenceDescriptor referenceDescriptor;

		private ProjectDescriptor.KeyOutputDescriptor keyOutputData;

		private VsWiXProject owner;

		public string ActiveConfiguration
		{
			get
			{
				string str;
				IVsProjectCfg2 projectActiveConfiguration = ProjectUtilities.GetProjectActiveConfiguration(this.project);
				if (projectActiveConfiguration == null)
				{
					return null;
				}
				projectActiveConfiguration.get_CanonicalName(out str);
				return str;
			}
		}

		public string[] AvailableConfigurations
		{
			get
			{
				IVsCfg vsCfg;
				string str;
				IVsCfgProvider2 projectConfigurationProvider = ProjectUtilities.GetProjectConfigurationProvider(this.project);
				if (projectConfigurationProvider != null)
				{
					uint[] numArray = new uint[1];
					uint[] numArray1 = new uint[1];
					projectConfigurationProvider.GetCfgNames(0, null, numArray);
					projectConfigurationProvider.GetPlatformNames(0, null, numArray1);
					if (numArray[0] > 0)
					{
						string[] strArrays = new string[numArray[0]];
						string[] strArrays1 = new string[numArray1[0]];
						projectConfigurationProvider.GetCfgNames((uint)strArrays.Length, strArrays, null);
						if (strArrays1.Length != 0)
						{
							projectConfigurationProvider.GetPlatformNames((uint)strArrays1.Length, strArrays1, null);
						}
						List<string> strs = new List<string>();
						string[] strArrays2 = strArrays;
						for (int i = 0; i < (int)strArrays2.Length; i++)
						{
							string str1 = strArrays2[i];
							if (!string.IsNullOrEmpty(str1))
							{
								if (strArrays1.Length != 0)
								{
									string[] strArrays3 = strArrays1;
									for (int j = 0; j < (int)strArrays3.Length; j++)
									{
										string str2 = strArrays3[j];
										if (!string.IsNullOrEmpty(str2) && ErrorHandler.Succeeded(projectConfigurationProvider.GetCfgOfName(str1, str2, out vsCfg)) && vsCfg is IVsProjectCfg)
										{
											(vsCfg as IVsProjectCfg).get_CanonicalName(out str);
											if (!string.IsNullOrEmpty(str))
											{
												strs.Add(str);
											}
										}
									}
								}
								else if (ErrorHandler.Succeeded(projectConfigurationProvider.GetCfgOfName(str1, null, out vsCfg)) && vsCfg is IVsProjectCfg)
								{
									(vsCfg as IVsProjectCfg).get_CanonicalName(out str);
									if (!string.IsNullOrEmpty(str))
									{
										strs.Add(str);
									}
								}
							}
						}
						return strs.ToArray();
					}
				}
				return null;
			}
		}

		public string FilePath
		{
			get
			{
				return ProjectUtilities.GetProjectFilePath(this.project);
			}
		}

		public System.Guid Guid
		{
			get
			{
				System.Guid guid;
				if (!(this.project is IVsHierarchy))
				{
					return System.Guid.Empty;
				}
				(this.project as IVsHierarchy).GetGuidProperty(-2, -2059, out guid);
				return guid;
			}
		}

		public IVsProject InternalProject
		{
			get
			{
				return this.project;
			}
			set
			{
				this.project = value;
			}
		}

		public ProjectDescriptor.KeyOutputDescriptor KeyOutput
		{
			get
			{
				IVsOutputGroup vsOutputGroup;
				string str;
				string str1;
				string str2;
				string str3;
				string str4;
				string str5;
				this.keyOutputData.SourcePath = null;
				this.keyOutputData.TargetName = null;
				this.keyOutputData.Version = null;
				this.keyOutputData.PublicKeyToken = null;
				this.keyOutputData.PublicKey = null;
				this.keyOutputData.Language = null;
				this.keyOutputData.HashAlgorithm = null;
				this.keyOutputData.DisplayName = null;
				this.keyOutputData.Name = null;
				IVsSolutionBuildManager service = ProjectUtilities.ServiceProvider.GetService(typeof(SVsSolutionBuildManager)) as IVsSolutionBuildManager;
				if (service != null)
				{
					IVsProjectCfg2 vsProjectCfg2 = service.FindActiveProjectCfg(this.project as IVsHierarchy) as IVsProjectCfg2;
					if (vsProjectCfg2 != null && ErrorHandler.Succeeded(vsProjectCfg2.OpenOutputGroup(OutputGroupCanonical.Built.ToString(), out vsOutputGroup)))
					{
						vsOutputGroup.get_KeyOutput(out str);
						if (!string.IsNullOrEmpty(str))
						{
							this.keyOutputData.SourcePath = str;
							if (this.IsAssembly(str))
							{
								this.keyOutputData.TargetName = Path.GetFileName(str);
								this.keyOutputData.DisplayName = this.keyOutputData.TargetName;
								this.keyOutputData.Name = Path.GetFileNameWithoutExtension(str);
								if (this.GetAssemblyInfo(str, out str1, out str2, out str3, out str4, out str5))
								{
									this.keyOutputData.Version = str1;
									this.keyOutputData.PublicKeyToken = str2;
									this.keyOutputData.PublicKey = str3;
									this.keyOutputData.Language = str4;
									this.keyOutputData.HashAlgorithm = str5;
								}
							}
						}
					}
				}
				return this.keyOutputData;
			}
		}

		public string Kind
		{
			get
			{
				Project vsProject = this.VsProject;
				if (vsProject == null)
				{
					return null;
				}
				return vsProject.Kind;
			}
		}

		public string Name
		{
			get
			{
				return ProjectUtilities.GetUniqueUIName(this.project);
			}
		}

		public VsWiXProject Owner
		{
			get
			{
				return this.owner;
			}
		}

		public VsWiXProject.ReferenceDescriptor ReferenceDescriptor
		{
			get
			{
				return this.referenceDescriptor;
			}
		}

		public string RootDirectory
		{
			get
			{
				object obj;
				if (!(this.project is IVsHierarchy))
				{
					return null;
				}
				(this.project as IVsHierarchy).GetProperty(-2, -2021, out obj);
				return (string)obj;
			}
		}

		public string UniqueName
		{
			get
			{
				string str;
				IVsSolution vsSolution = ProjectUtilities.GetVsSolution();
				if (vsSolution == null)
				{
					return null;
				}
				vsSolution.GetUniqueNameOfProject((IVsHierarchy)this.project, out str);
				return str;
			}
		}

		public Project VsProject
		{
			get
			{
				object obj;
				if (!(this.project is IVsHierarchy))
				{
					return null;
				}
				(this.project as IVsHierarchy).GetProperty(-2, -2027, out obj);
				return obj as Project;
			}
		}

		public ProjectDescriptor(IVsProject project, VsWiXProject owner)
		{
			this.project = project;
			this.owner = owner;
		}

		public ProjectDescriptor(IVsProject project, VsWiXProject owner, VsWiXProject.ReferenceDescriptor referenceDescriptor)
		{
			this.project = project;
			this.referenceDescriptor = referenceDescriptor;
			this.owner = owner;
		}

		public string[] Dependencies(AddinExpress.Installer.WiXDesigner.OutputGroup group)
		{
			IVsOutputGroup vsOutputGroup;
			string str;
			IVsSolutionBuildManager service = ProjectUtilities.ServiceProvider.GetService(typeof(SVsSolutionBuildManager)) as IVsSolutionBuildManager;
			if (service != null)
			{
				IVsProjectCfg2 vsProjectCfg2 = service.FindActiveProjectCfg(this.project as IVsHierarchy) as IVsProjectCfg2;
				if (vsProjectCfg2 != null)
				{
					OutputGroupCanonical[] canonicalGroup = this.WiXGroupToCanonicalGroup(group);
					if (canonicalGroup != null)
					{
						bool flag = false;
						List<string> strs = new List<string>();
						OutputGroupCanonical[] outputGroupCanonicalArray = canonicalGroup;
						for (int i = 0; i < (int)outputGroupCanonicalArray.Length; i++)
						{
							if (ErrorHandler.Succeeded(vsProjectCfg2.OpenOutputGroup(outputGroupCanonicalArray[i].ToString(), out vsOutputGroup)))
							{
								uint[] numArray = new uint[1];
								if (ErrorHandler.Succeeded(vsOutputGroup.get_DeployDependencies(0, null, numArray)))
								{
									IVsDeployDependency[] vsDeployDependencyArray = new IVsDeployDependency[numArray[0]];
									if (ErrorHandler.Succeeded(vsOutputGroup.get_DeployDependencies((uint)vsDeployDependencyArray.Length, vsDeployDependencyArray, null)))
									{
										IVsDeployDependency[] vsDeployDependencyArray1 = vsDeployDependencyArray;
										for (int j = 0; j < (int)vsDeployDependencyArray1.Length; j++)
										{
											IVsDeployDependency vsDeployDependency = vsDeployDependencyArray1[j];
											if (vsDeployDependency != null)
											{
												vsDeployDependency.get_DeployDependencyURL(out str);
												if (!string.IsNullOrEmpty(str))
												{
													Uri uri = new Uri(str);
													if (uri.IsFile)
													{
														if (!this.IsMicrosoftAssembly(uri.LocalPath))
														{
															strs.Add(str);
														}
														else
														{
															flag = true;
														}
													}
												}
											}
										}
									}
								}
							}
						}
						if (flag)
						{
							strs.Add("Microsoft .NET Framework");
						}
						return strs.ToArray();
					}
				}
			}
			return null;
		}

		public void Dispose()
		{
			if (!this.disposed)
			{
				this.disposed = true;
			}
			GC.SuppressFinalize(this);
		}

		~ProjectDescriptor()
		{
			this.Dispose();
		}

		private bool GetAssemblyInfo(string filePath, out string version, out string publicKeyToken, out string publicKey, out string language, out string hashAlgorithm)
		{
			string empty = string.Empty;
			string str = empty;
			hashAlgorithm = empty;
			string str1 = str;
			str = str1;
			language = str1;
			string str2 = str;
			str = str2;
			publicKey = str2;
			string str3 = str;
			str = str3;
			publicKeyToken = str3;
			version = str;
			Assembly assembly = null;
			try
			{
				assembly = Assembly.ReflectionOnlyLoad(File.ReadAllBytes(filePath));
			}
			catch (Exception exception)
			{
			}
			if (assembly == null)
			{
				return false;
			}
			AssemblyName name = assembly.GetName();
			version = name.Version.ToString(4);
			byte[] numArray = name.GetPublicKeyToken();
			if (numArray != null)
			{
				for (int i = 0; i < (int)numArray.Length; i++)
				{
					if (numArray[i] <= 15)
					{
						publicKeyToken = string.Concat(publicKeyToken, "0", numArray[i].ToString("x"));
					}
					else
					{
						publicKeyToken = string.Concat(publicKeyToken, numArray[i].ToString("x"));
					}
				}
			}
			if (!string.IsNullOrEmpty(publicKeyToken))
			{
				publicKeyToken = publicKeyToken.ToUpper();
			}
			byte[] numArray1 = name.GetPublicKey();
			if (numArray1 != null)
			{
				for (int j = 0; j < (int)numArray1.Length; j++)
				{
					if (numArray1[j] <= 15)
					{
						publicKey = string.Concat(publicKey, "0", numArray1[j].ToString("x"));
					}
					else
					{
						publicKey = string.Concat(publicKey, numArray1[j].ToString("x"));
					}
				}
			}
			if (!string.IsNullOrEmpty(publicKey))
			{
				publicKey = publicKey.ToUpper();
			}
			language = name.CultureInfo.DisplayName;
			if (name.HashAlgorithm != AssemblyHashAlgorithm.None)
			{
				hashAlgorithm = name.HashAlgorithm.ToString();
			}
			return true;
		}

		public object GetReferenceNodeById(uint itemId)
		{
			object obj = null;
			object obj1 = null;
			try
			{
				MethodInfo method = this.Owner.VsProject.Object.GetType().GetMethod("GetReferenceContainer", BindingFlags.Instance | BindingFlags.Public);
				if (method != null)
				{
					obj1 = method.Invoke(this.Owner.VsProject.Object, null);
				}
				if (obj1 != null)
				{
					IEnumerable enumerable = null;
					method = obj1.GetType().GetMethod("EnumReferences", BindingFlags.Instance | BindingFlags.Public);
					if (method != null)
					{
						enumerable = method.Invoke(obj1, null) as IEnumerable;
					}
					if (enumerable != null)
					{
						foreach (object obj2 in enumerable)
						{
							PropertyInfo property = obj2.GetType().GetProperty("ID", BindingFlags.Instance | BindingFlags.Public);
							if (!(property != null) || (uint)property.GetValue(obj2, null) != itemId)
							{
								continue;
							}
							obj = obj2;
							break;
						}
					}
				}
			}
			catch (Exception exception)
			{
			}
			return obj;
		}

		private bool IsAssembly(string filePath)
		{
			bool flag;
			try
			{
				AssemblyName.GetAssemblyName(filePath);
				flag = true;
			}
			catch (Exception exception)
			{
				return false;
			}
			return flag;
		}

		private bool IsMicrosoftAssembly(string filePath)
		{
			if (this.IsAssembly(filePath))
			{
				Assembly assembly = null;
				try
				{
					assembly = Assembly.ReflectionOnlyLoadFrom(filePath);
				}
				catch (Exception exception)
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
								catch (Exception exception1)
								{
								}
								return product;
							}
						}
					}
				}
			}
			return false;
		}

		private AddinExpress.Installer.WiXDesigner.OutputGroup OutputGroupCanonicalName2WiXGroup(string name)
		{
			if (name != null)
			{
				if (name == "Built" || name == "XmlSerializer")
				{
					return AddinExpress.Installer.WiXDesigner.OutputGroup.Binaries;
				}
				if (name == "ContentFiles")
				{
					return AddinExpress.Installer.WiXDesigner.OutputGroup.Content;
				}
				if (name == "LocalizedResourceDlls")
				{
					return AddinExpress.Installer.WiXDesigner.OutputGroup.Satellites;
				}
				if (name == "Documentation")
				{
					return AddinExpress.Installer.WiXDesigner.OutputGroup.Documents;
				}
				if (name == "Symbols")
				{
					return AddinExpress.Installer.WiXDesigner.OutputGroup.Symbols;
				}
				if (name == "SourceFiles")
				{
					return AddinExpress.Installer.WiXDesigner.OutputGroup.Sources;
				}
			}
			return AddinExpress.Installer.WiXDesigner.OutputGroup.None;
		}

		public string[] Outputs(AddinExpress.Installer.WiXDesigner.OutputGroup group, string configuration, bool returnFullPath)
		{
			IVsCfg vsCfg;
			IVsOutputGroup vsOutputGroup;
			string str;
			IVsCfgProvider2 projectConfigurationProvider = ProjectUtilities.GetProjectConfigurationProvider(this.project);
			if (projectConfigurationProvider != null)
			{
				string[] strArrays = configuration.Split(new char[] { '|' });
				if (strArrays.Length != 0)
				{
					string str1 = null;
					string str2 = strArrays[0];
					if ((int)strArrays.Length > 1)
					{
						str1 = strArrays[1];
					}
					if (!string.IsNullOrEmpty(str2))
					{
						uint[] numArray = new uint[1];
						if (ErrorHandler.Succeeded(projectConfigurationProvider.GetCfgOfName(str2, str1, out vsCfg)) && vsCfg is IVsProjectCfg2)
						{
							IVsProjectCfg2 vsProjectCfg2 = (IVsProjectCfg2)vsCfg;
							List<string> strs = new List<string>();
							OutputGroupCanonical[] canonicalGroup = this.WiXGroupToCanonicalGroup(group);
							if (canonicalGroup != null)
							{
								OutputGroupCanonical[] outputGroupCanonicalArray = canonicalGroup;
								for (int i = 0; i < (int)outputGroupCanonicalArray.Length; i++)
								{
									if (ErrorHandler.Succeeded(vsProjectCfg2.OpenOutputGroup(outputGroupCanonicalArray[i].ToString(), out vsOutputGroup)))
									{
										try
										{
											if (ErrorHandler.Succeeded(vsOutputGroup.get_Outputs(0, null, numArray)))
											{
												IVsOutput2[] vsOutput2Array = new IVsOutput2[numArray[0]];
												vsOutputGroup.get_Outputs((uint)vsOutput2Array.Length, vsOutput2Array, null);
												IVsOutput2[] vsOutput2Array1 = vsOutput2Array;
												for (int j = 0; j < (int)vsOutput2Array1.Length; j++)
												{
													IVsOutput2 vsOutput2 = vsOutput2Array1[j];
													if (vsOutput2 != null)
													{
														if (!returnFullPath)
														{
															vsOutput2.get_RootRelativeURL(out str);
														}
														else
														{
															vsOutput2.get_DeploySourceURL(out str);
														}
														if (!string.IsNullOrEmpty(str))
														{
															strs.Add(str);
														}
													}
												}
											}
										}
										catch (Exception exception)
										{
										}
									}
								}
							}
							return strs.ToArray();
						}
					}
				}
			}
			return null;
		}

		public Dictionary<string, string> Outputs(AddinExpress.Installer.WiXDesigner.OutputGroup group, string configuration)
		{
			IVsCfg vsCfg;
			IVsOutputGroup vsOutputGroup;
			string str;
			string str1;
			Dictionary<string, string> strs = null;
			IVsCfgProvider2 projectConfigurationProvider = ProjectUtilities.GetProjectConfigurationProvider(this.project);
			if (projectConfigurationProvider != null)
			{
				string[] strArrays = configuration.Split(new char[] { '|' });
				if (strArrays.Length != 0)
				{
					string str2 = null;
					string str3 = strArrays[0];
					if ((int)strArrays.Length > 1)
					{
						str2 = strArrays[1];
					}
					if (!string.IsNullOrEmpty(str3))
					{
						uint[] numArray = new uint[1];
						if (ErrorHandler.Succeeded(projectConfigurationProvider.GetCfgOfName(str3, str2, out vsCfg)) && vsCfg is IVsProjectCfg2)
						{
							strs = new Dictionary<string, string>();
							IVsProjectCfg2 vsProjectCfg2 = (IVsProjectCfg2)vsCfg;
							OutputGroupCanonical[] canonicalGroup = this.WiXGroupToCanonicalGroup(group);
							if (canonicalGroup != null)
							{
								OutputGroupCanonical[] outputGroupCanonicalArray = canonicalGroup;
								for (int i = 0; i < (int)outputGroupCanonicalArray.Length; i++)
								{
									if (ErrorHandler.Succeeded(vsProjectCfg2.OpenOutputGroup(outputGroupCanonicalArray[i].ToString(), out vsOutputGroup)) && ErrorHandler.Succeeded(vsOutputGroup.get_Outputs(0, null, numArray)))
									{
										IVsOutput2[] vsOutput2Array = new IVsOutput2[numArray[0]];
										vsOutputGroup.get_Outputs((uint)vsOutput2Array.Length, vsOutput2Array, null);
										IVsOutput2[] vsOutput2Array1 = vsOutput2Array;
										for (int j = 0; j < (int)vsOutput2Array1.Length; j++)
										{
											IVsOutput2 vsOutput2 = vsOutput2Array1[j];
											if (vsOutput2 != null)
											{
												vsOutput2.get_DeploySourceURL(out str);
												vsOutput2.get_RootRelativeURL(out str1);
												if (!string.IsNullOrEmpty(str1) && !string.IsNullOrEmpty(str))
												{
													strs.Add(str1, str);
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return strs;
		}

		private OutputGroupCanonical[] WiXGroupToCanonicalGroup(AddinExpress.Installer.WiXDesigner.OutputGroup groupItem)
		{
			OutputGroupCanonical[] outputGroupCanonicalArray = null;
			if (groupItem <= AddinExpress.Installer.WiXDesigner.OutputGroup.Content)
			{
				switch (groupItem)
				{
					case AddinExpress.Installer.WiXDesigner.OutputGroup.Binaries:
					{
						outputGroupCanonicalArray = new OutputGroupCanonical[] { OutputGroupCanonical.Built, OutputGroupCanonical.XmlSerializer };
						break;
					}
					case AddinExpress.Installer.WiXDesigner.OutputGroup.Symbols:
					{
						outputGroupCanonicalArray = new OutputGroupCanonical[] { OutputGroupCanonical.Symbols };
						break;
					}
					case AddinExpress.Installer.WiXDesigner.OutputGroup.Binaries | AddinExpress.Installer.WiXDesigner.OutputGroup.Symbols:
					{
						break;
					}
					case AddinExpress.Installer.WiXDesigner.OutputGroup.Sources:
					{
						outputGroupCanonicalArray = new OutputGroupCanonical[] { OutputGroupCanonical.SourceFiles };
						break;
					}
					default:
					{
						if (groupItem == AddinExpress.Installer.WiXDesigner.OutputGroup.Content)
						{
							outputGroupCanonicalArray = new OutputGroupCanonical[] { OutputGroupCanonical.ContentFiles };
							break;
						}
						else
						{
							break;
						}
					}
				}
			}
			else if (groupItem == AddinExpress.Installer.WiXDesigner.OutputGroup.Satellites)
			{
				outputGroupCanonicalArray = new OutputGroupCanonical[] { OutputGroupCanonical.LocalizedResourceDlls };
			}
			else if (groupItem == AddinExpress.Installer.WiXDesigner.OutputGroup.Documents)
			{
				outputGroupCanonicalArray = new OutputGroupCanonical[] { OutputGroupCanonical.Documentation };
			}
			return outputGroupCanonicalArray;
		}

		public struct KeyOutputDescriptor
		{
			public string SourcePath;

			public string TargetName;

			public string Version;

			public string PublicKeyToken;

			public string PublicKey;

			public string Language;

			public string HashAlgorithm;

			public string DisplayName;

			public string Name;
		}
	}
}