using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class OutputsPropertyEditor : ObjectSelectorEditor
	{
		public OutputsPropertyEditor()
		{
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			string[] strArrays = (string[])context.PropertyDescriptor.GetValue(context.Instance);
			if (strArrays != null)
			{
				VSProjectOutputVDProj instance = context.Instance as VSProjectOutputVDProj;
				if (instance != null)
				{
					Dictionary<string, string> strs = null;
					if (instance.ReferenceDescriptor != null && instance.ReferenceDescriptor.ReferencedProject != null)
					{
						strs = instance.ReferenceDescriptor.ReferencedProject.Outputs(instance.Group, instance.ReferenceDescriptor.ReferencedProject.ActiveConfiguration);
					}
					FormDependenciesOutputs formDependenciesOutput = new FormDependenciesOutputs();
					formDependenciesOutput.Initialize(instance.Name, strs);
					this.ShowForm(provider, formDependenciesOutput);
					formDependenciesOutput.Dispose();
				}
			}
			return strArrays;
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