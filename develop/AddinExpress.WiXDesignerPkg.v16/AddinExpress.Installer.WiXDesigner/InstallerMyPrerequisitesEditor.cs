using AddinExpress.Installer.Prerequisites;
using AddinExpress.Installer.WiXDesigner.DesignTime;
using EnvDTE;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class InstallerMyPrerequisitesEditor : UITypeEditor, IWin32Window
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

		public InstallerMyPrerequisitesEditor()
		{
		}

		private bool CheckForPermissions()
		{
			bool flag = true;
			try
			{
				using (RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey("CLSID", true))
				{
					if (registryKey != null)
					{
						using (RegistryKey registryKey1 = registryKey.CreateSubKey("CheckForPermissions"))
						{
							if (registryKey1 != null)
							{
								flag = false;
							}
						}
						if (!flag)
						{
							registryKey.DeleteSubKey("CheckForPermissions", false);
						}
					}
				}
			}
			catch
			{
			}
			if (!flag)
			{
				flag = true;
				try
				{
					using (RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey("Software\\Classes\\CLSID", true))
					{
						if (registryKey2 != null)
						{
							using (RegistryKey registryKey3 = registryKey2.CreateSubKey("CheckForPermissions"))
							{
								if (registryKey3 != null)
								{
									flag = false;
								}
							}
							if (!flag)
							{
								registryKey2.DeleteSubKey("CheckForPermissions", false);
							}
						}
					}
				}
				catch
				{
				}
			}
			if (!flag)
			{
				return true;
			}
			MessageBox.Show(this, "Requested registry access is not allowed.\r\nPlease make sure you're running Visual Studio using the 'Run as administrator' command.", Helpers.GetProductName(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return false;
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				using (PrerequisitesForm prerequisitesForm = null)
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
									if (this.propertiesObject != null && this.CheckForPermissions())
									{
										prerequisitesForm = new PrerequisitesForm(this.propertiesObject.Project.VsProject.DTE.Version, Path.GetFileNameWithoutExtension(this.propertiesObject.Project.VsProject.FullName), this.propertiesObject.Project.RootDirectory, this.propertiesObject.RootBootstrapperDir);
										if (this.ShowForm(provider, prerequisitesForm) == DialogResult.OK)
										{
											this.propertiesObject.MyPrerequisites = PrerequisitesForm.GetPrerequisites(Path.GetFileNameWithoutExtension(this.propertiesObject.Project.VsProject.FullName), this.propertiesObject.Project.RootDirectory, this.propertiesObject.Project.VsProject.DTE.Version);
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