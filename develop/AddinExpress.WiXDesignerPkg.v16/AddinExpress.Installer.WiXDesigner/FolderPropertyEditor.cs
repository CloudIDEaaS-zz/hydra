using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class FolderPropertyEditor : ObjectSelectorEditor
	{
		public FolderPropertyEditor()
		{
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			object selectedFolder;
			VSBaseFolder vSBaseFolder = context.PropertyDescriptor.GetValue(context.Instance) as VSBaseFolder;
			if (vSBaseFolder != null)
			{
				FormSelectFolder formSelectFolder = new FormSelectFolder();
				try
				{
					formSelectFolder.Initialize(vSBaseFolder._project.FileSystem, vSBaseFolder);
					if (this.ShowForm(provider, formSelectFolder) != DialogResult.OK)
					{
						return vSBaseFolder;
					}
					else
					{
						selectedFolder = formSelectFolder.SelectedFolder;
					}
				}
				finally
				{
					formSelectFolder.Dispose();
				}
				return selectedFolder;
			}
			return vSBaseFolder;
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