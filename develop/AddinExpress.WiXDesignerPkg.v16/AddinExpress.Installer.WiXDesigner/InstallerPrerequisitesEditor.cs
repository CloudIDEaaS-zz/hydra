using AddinExpress.Installer.WiXDesigner.DesignTime;
using EnvDTE;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class InstallerPrerequisitesEditor : UITypeEditor, IWin32Window
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

		public InstallerPrerequisitesEditor()
		{
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				using (FormPrerequisites formPrerequisite = null)
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
									if (this.propertiesObject != null)
									{
										formPrerequisite = new FormPrerequisites(this.propertiesObject.Prerequisites)
										{
											DTEVersion = this.propertiesObject.Project.VsProject.DTE.Version,
											ApplicationName = this.propertiesObject.ProductName,
											RequiresElevation = this.propertiesObject.InstallAllUsers,
											Culture = this.propertiesObject.GetCulture()
										};
										string configurationName = this.propertiesObject.Project.VsProject.ConfigurationManager.ActiveConfiguration.ConfigurationName;
										configurationName = string.Concat(configurationName, "|", this.propertiesObject.Project.VsProject.ConfigurationManager.ActiveConfiguration.PlatformName);
										formPrerequisite.Configuration = configurationName;
										formPrerequisite.ParsePrerequisites();
										if (this.ShowForm(provider, formPrerequisite) == DialogResult.OK)
										{
											this.propertiesObject.Prerequisites = formPrerequisite.GetPrerequisites();
										}
									}
								}
							}
						}
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						MessageBox.Show(this, exception.Message, exception.Source, MessageBoxButtons.OK, MessageBoxIcon.Hand);
					}
				}
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