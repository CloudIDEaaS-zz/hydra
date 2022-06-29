using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing.Design;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class DependenciesPropertyEditor : ObjectSelectorEditor
	{
		public DependenciesPropertyEditor()
		{
		}

		private string CheckAssemblyVersion(string filePath)
		{
			string str;
			try
			{
				str = AssemblyName.GetAssemblyName(filePath).Version.ToString();
			}
			catch
			{
				return string.Empty;
			}
			return str;
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			object obj;
			if (context.Instance is VSProjectOutputVDProj)
			{
				string[] strArrays = (string[])context.PropertyDescriptor.GetValue(context.Instance);
				if (strArrays != null)
				{
					VSProjectOutputVDProj instance = context.Instance as VSProjectOutputVDProj;
					if (instance != null)
					{
						string[] fileNameWithoutExtension = null;
						string[] empty = null;
						if (instance.ReferenceDescriptor != null && instance.ReferenceDescriptor.ReferencedProject != null)
						{
							fileNameWithoutExtension = instance.ReferenceDescriptor.ReferencedProject.Dependencies(instance.Group);
						}
						if (fileNameWithoutExtension != null && fileNameWithoutExtension.Length != 0)
						{
							empty = new string[(int)fileNameWithoutExtension.Length];
							for (int i = 0; i < (int)fileNameWithoutExtension.Length; i++)
							{
								empty[i] = string.Empty;
								string str = fileNameWithoutExtension[i];
								if (!File.Exists(str))
								{
									try
									{
										Uri uri = new Uri(str);
										fileNameWithoutExtension[i] = Path.GetFileNameWithoutExtension(uri.LocalPath);
										empty[i] = FileVersionInfo.GetVersionInfo(uri.LocalPath).FileVersion;
										if (string.IsNullOrEmpty(empty[i]))
										{
											fileNameWithoutExtension[i] = uri.LocalPath;
										}
									}
									catch
									{
									}
								}
								else
								{
									fileNameWithoutExtension[i] = Path.GetFileNameWithoutExtension(str);
									empty[i] = FileVersionInfo.GetVersionInfo(str).FileVersion;
								}
							}
						}
						FormDependenciesOutputs formDependenciesOutput = new FormDependenciesOutputs();
						formDependenciesOutput.Initialize(instance.Name, fileNameWithoutExtension, empty);
						this.ShowForm(provider, formDependenciesOutput);
						formDependenciesOutput.Dispose();
					}
				}
				return strArrays;
			}
			if (context.Instance is VSAssembly)
			{
				List<string> strs = (List<string>)context.PropertyDescriptor.GetValue(context.Instance);
				if (strs != null)
				{
					VSAssembly vSAssembly = context.Instance as VSAssembly;
					if (vSAssembly != null)
					{
						string[] fileNameWithoutExtension1 = null;
						string[] strArrays1 = null;
						if (!File.Exists(vSAssembly.SourcePath))
						{
							return strs;
						}
						try
						{
							string empty1 = string.Empty;
							DependencyScanner.GetAssemblyDependencies(vSAssembly.SourcePath, ref empty1);
							if (!string.IsNullOrEmpty(empty1))
							{
								fileNameWithoutExtension1 = empty1.Split(new char[] { ';' });
								strArrays1 = new string[(int)fileNameWithoutExtension1.Length];
								for (int j = 0; j < (int)fileNameWithoutExtension1.Length; j++)
								{
									strArrays1[j] = this.CheckAssemblyVersion(fileNameWithoutExtension1[j]);
									fileNameWithoutExtension1[j] = Path.GetFileNameWithoutExtension(fileNameWithoutExtension1[j]);
								}
							}
							FormDependenciesOutputs formDependenciesOutput1 = new FormDependenciesOutputs();
							formDependenciesOutput1.Initialize(vSAssembly.TargetName, fileNameWithoutExtension1, strArrays1);
							this.ShowForm(provider, formDependenciesOutput1);
							formDependenciesOutput1.Dispose();
							return strs;
						}
						catch
						{
							obj = strs;
						}
						return obj;
					}
				}
				return strs;
			}
			if (!(context.Instance is VSMergeModule) && !(context.Instance is VSMergeModuleReference))
			{
				return null;
			}
			List<string> strs1 = (List<string>)context.PropertyDescriptor.GetValue(context.Instance);
			if (strs1 != null)
			{
				string[] requiredID = null;
				string[] requiredVersion = null;
				string sourcePath = string.Empty;
				string name = string.Empty;
				List<ModuleDependency> moduleDependencies = null;
				if (context.Instance is VSMergeModule)
				{
					sourcePath = (context.Instance as VSMergeModule).SourcePath;
					name = (context.Instance as VSMergeModule).Name;
				}
				else if (context.Instance is VSMergeModuleReference)
				{
					sourcePath = (context.Instance as VSMergeModuleReference).SourcePath;
					name = (context.Instance as VSMergeModuleReference).Name;
				}
				if (!string.IsNullOrEmpty(sourcePath))
				{
					using (MsiHelper msiHelper = new MsiHelper())
					{
						moduleDependencies = msiHelper.GetModuleDependencies(sourcePath);
					}
				}
				if (moduleDependencies != null)
				{
					requiredID = new string[moduleDependencies.Count];
					requiredVersion = new string[moduleDependencies.Count];
					using (MsiHelper msiHelper1 = new MsiHelper())
					{
						for (int k = 0; k < moduleDependencies.Count; k++)
						{
							string path = moduleDependencies[k].Path;
							if (string.IsNullOrEmpty(path))
							{
								path = msiHelper1.FindStandardMSM(moduleDependencies[k].RequiredID, moduleDependencies[k].RequiredLanguage, moduleDependencies[k].RequiredVersion);
							}
							if (string.IsNullOrEmpty(path))
							{
								requiredID[k] = moduleDependencies[k].RequiredID;
							}
							else
							{
								requiredID[k] = Path.GetFileName(path);
							}
							requiredVersion[k] = moduleDependencies[k].RequiredVersion;
						}
					}
				}
				FormDependenciesOutputs formDependenciesOutput2 = new FormDependenciesOutputs();
				formDependenciesOutput2.Initialize(name, requiredID, requiredVersion);
				this.ShowForm(provider, formDependenciesOutput2);
				formDependenciesOutput2.Dispose();
			}
			return strs1;
		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}

		private DialogResult ShowForm(IServiceProvider provider, Form form)
		{
			IUIService service = (IUIService)provider.GetService(typeof(IUIService));
			if (service == null)
			{
				return form.ShowDialog();
			}
			return service.ShowDialog(form);
		}
	}
}