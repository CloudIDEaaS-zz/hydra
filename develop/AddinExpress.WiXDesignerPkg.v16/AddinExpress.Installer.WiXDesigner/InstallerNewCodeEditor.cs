using AddinExpress.Installer.WiXDesigner.DesignTime;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class InstallerNewCodeEditor : UITypeEditor, IWin32Window
	{
		private IWindowsFormsEditorService edSvc;

		private VsWiXProject.ProjectPropertiesObject propertiesObject;

		public IntPtr Handle
		{
			get
			{
				return VsPackage.CurrentInstance.Handle;
			}
		}

		public InstallerNewCodeEditor()
		{
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			object code;
			if (context != null && context.Instance != null && provider != null)
			{
				using (FormNewCode formNewCode = null)
				{
					try
					{
						this.edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
						if (this.edSvc != null && context.PropertyDescriptor != null)
						{
							FieldInfo field = context.PropertyDescriptor.GetType().GetField("property", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField);
							if (field != null)
							{
								ProxyPropertyDescriptor proxyPropertyDescriptor = field.GetValue(context.PropertyDescriptor) as ProxyPropertyDescriptor;
								if (proxyPropertyDescriptor != null)
								{
									this.propertiesObject = proxyPropertyDescriptor.Parent as VsWiXProject.ProjectPropertiesObject;
									if (this.propertiesObject != null && this.propertiesObject.Project != null && this.propertiesObject.Project.WiXModel != null)
									{
										formNewCode = new FormNewCode()
										{
											Code = (string)value
										};
										string name = context.PropertyDescriptor.Name;
										if (name != null)
										{
											if (name == "ProductCode")
											{
												formNewCode.Text = "Product Code";
											}
											else if (name == "UpgradeCode")
											{
												formNewCode.Text = "Upgrade Code";
											}
											else if (name == "ModuleSignature")
											{
												formNewCode.Text = "Module Signature";
												formNewCode.ButtonText = "&New Signature";
												formNewCode.GenerateMergeModuleSignature = true;
											}
										}
										if (this.ShowForm(provider, formNewCode) == DialogResult.OK)
										{
											code = formNewCode.Code;
											return code;
										}
									}
								}
							}
						}
						return value;
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						MessageBox.Show(this, exception.Message, exception.Source, MessageBoxButtons.OK, MessageBoxIcon.Hand);
						return value;
					}
				}
				return code;
			}
			return value;
		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			if (context != null && context.Instance != null)
			{
				return UITypeEditorEditStyle.Modal;
			}
			return base.GetEditStyle(context);
		}

		private DialogResult ShowForm(IServiceProvider provider, Form form)
		{
			if (form == null)
			{
				return DialogResult.Cancel;
			}
			IUIService service = (IUIService)provider.GetService(typeof(IUIService));
			if (service == null)
			{
				return form.ShowDialog();
			}
			return service.ShowDialog(form);
		}
	}
}