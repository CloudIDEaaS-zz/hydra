using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class FilesPropertyEditor : ObjectSelectorEditor
	{
		public FilesPropertyEditor()
		{
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			object obj;
			List<string> strs = (List<string>)context.PropertyDescriptor.GetValue(context.Instance);
			if (strs != null)
			{
				VSAssembly instance = context.Instance as VSAssembly;
				if (instance != null)
				{
					if (!File.Exists(instance.SourcePath))
					{
						return strs;
					}
					List<string> fileName = null;
					try
					{
						string empty = string.Empty;
						DependencyScanner.GetAssemblyFiles(instance.SourcePath, ref empty);
						if (!string.IsNullOrEmpty(empty))
						{
							fileName = new List<string>(empty.Split(new char[] { ';' }));
							for (int i = 0; i < fileName.Count; i++)
							{
								fileName[i] = Path.GetFileName(fileName[i]);
							}
						}
						FormAssemblyFiles formAssemblyFile = new FormAssemblyFiles();
						formAssemblyFile.Initialize(instance.TargetName, fileName);
						this.ShowForm(provider, formAssemblyFile);
						formAssemblyFile.Dispose();
						return strs;
					}
					catch
					{
						obj = strs;
					}
					return obj;
				}
				else if (context.Instance is VSMergeModule || context.Instance is VSMergeModuleReference)
				{
					string sourcePath = string.Empty;
					string name = string.Empty;
					List<string> moduleFiles = null;
					VSMergeModule vSMergeModule = context.Instance as VSMergeModule;
					if (vSMergeModule == null)
					{
						VSMergeModuleReference vSMergeModuleReference = context.Instance as VSMergeModuleReference;
						if (vSMergeModuleReference != null)
						{
							sourcePath = vSMergeModuleReference.SourcePath;
							name = vSMergeModuleReference.Name;
						}
					}
					else
					{
						sourcePath = vSMergeModule.SourcePath;
						name = vSMergeModule.Name;
					}
					if (!string.IsNullOrEmpty(sourcePath))
					{
						using (MsiHelper msiHelper = new MsiHelper())
						{
							moduleFiles = msiHelper.GetModuleFiles(sourcePath);
						}
					}
					FormAssemblyFiles formAssemblyFile1 = new FormAssemblyFiles();
					formAssemblyFile1.Initialize(name, moduleFiles);
					this.ShowForm(provider, formAssemblyFile1);
					formAssemblyFile1.Dispose();
				}
			}
			return strs;
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