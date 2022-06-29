using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class TargetPropertyEditor : ObjectSelectorEditor
	{
		public TargetPropertyEditor()
		{
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value1)
		{
			object selectedItem;
			string value = (string)context.PropertyDescriptor.GetValue(context.Instance);
			if (!string.IsNullOrEmpty(value))
			{
				if (context.Instance is VSShortcut)
				{
					if (!value.StartsWith("[") || !value.EndsWith("]"))
					{
						VSBaseFolder folderById = null;
						string str = value;
						if (value.StartsWith("["))
						{
							string str1 = value.Substring(1, value.IndexOf("]") - 1);
							folderById = (context.Instance as VSShortcut).Project.FileSystem.GetFolderById(str1);
							str = value.Substring(value.LastIndexOf("]") + 1);
							FormSelectItemInProject formSelectItemInProject = new FormSelectItemInProject();
							try
							{
								formSelectItemInProject.Initialize(folderById, str, (context.Instance as VSShortcut).Project.FileSystem, new string[] { "All Files (*.*)" });
								if (this.ShowForm(provider, formSelectItemInProject) == DialogResult.OK)
								{
									selectedItem = formSelectItemInProject.SelectedItem;
									return selectedItem;
								}
							}
							finally
							{
								formSelectItemInProject.Dispose();
							}
						}
					}
					else
					{
						string str2 = value.Substring(1, value.Length - 2);
						VSBaseFolder vSBaseFolder = (context.Instance as VSShortcut).Project.FileSystem.GetFolderById(str2);
						if (vSBaseFolder != null)
						{
							FormSelectFolder formSelectFolder = new FormSelectFolder();
							try
							{
								formSelectFolder.Initialize(vSBaseFolder._project.FileSystem, vSBaseFolder);
								if (this.ShowForm(provider, formSelectFolder) == DialogResult.OK)
								{
									selectedItem = string.Concat("[", formSelectFolder.SelectedFolder, "]");
									return selectedItem;
								}
							}
							finally
							{
								formSelectFolder.Dispose();
							}
						}
					}
				}
				if (context.Instance is VSFileType)
				{
					VSBaseFolder parent = null;
					if ((context.Instance as VSFileType).ParentComponent != null)
					{
						parent = (context.Instance as VSFileType).ParentComponent.Parent;
					}
					if ((context.Instance as VSFileType).ParentOutput != null)
					{
						parent = (context.Instance as VSFileType).ParentOutput.Folder;
					}
					string str3 = value;
					FormSelectItemInProject formSelectItemInProject1 = new FormSelectItemInProject();
					try
					{
						formSelectItemInProject1.Initialize(parent, str3, (context.Instance as VSFileType).Project.FileSystem, new string[] { "Executable Files (*.exe)", "All Files (*.*)" });
						if (this.ShowForm(provider, formSelectItemInProject1) != DialogResult.OK)
						{
							return value;
						}
						else
						{
							selectedItem = formSelectItemInProject1.SelectedFile;
						}
					}
					finally
					{
						formSelectItemInProject1.Dispose();
					}
				}
				else
				{
					return value;
				}
				return selectedItem;
			}
			return value;
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