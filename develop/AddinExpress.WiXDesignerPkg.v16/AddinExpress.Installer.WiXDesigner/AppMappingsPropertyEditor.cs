using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class AppMappingsPropertyEditor : ObjectSelectorEditor
	{
		public AppMappingsPropertyEditor()
		{
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			object extensions;
			object obj = context.PropertyDescriptor.GetValue(context.Instance);
			if (obj != null && obj is List<VSWebApplicationExtension>)
			{
				FormMappings formMapping = new FormMappings();
				try
				{
					List<VSWebApplicationExtension> vSWebApplicationExtensions = new List<VSWebApplicationExtension>();
					vSWebApplicationExtensions.AddRange(obj as List<VSWebApplicationExtension>);
					formMapping.Initialize(provider, context.Instance as VSWebCustomFolder, vSWebApplicationExtensions);
					if (this.ShowForm(provider, formMapping) != DialogResult.OK)
					{
						return obj;
					}
					else
					{
						extensions = formMapping.Extensions;
					}
				}
				finally
				{
					formMapping.Dispose();
				}
				return extensions;
			}
			return obj;
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