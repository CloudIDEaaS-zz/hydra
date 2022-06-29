using AddinExpress.Installer.WiXDesigner.DesignTime;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class InstallerIconEditor : UITypeEditor, IWin32Window
	{
		private IWindowsFormsEditorService edSvc;

		private ListBox listBox;

		private VsWiXProject.ProjectPropertiesObject propertiesObject;

		public IntPtr Handle
		{
			get
			{
				return VsPackage.CurrentInstance.Handle;
			}
		}

		public InstallerIconEditor()
		{
			this.listBox = new ListBox()
			{
				BorderStyle = BorderStyle.None
			};
			this.listBox.MouseUp += new MouseEventHandler(this.listBox_MouseUp);
			this.listBox.DrawMode = DrawMode.OwnerDrawVariable;
			this.listBox.DrawItem += new DrawItemEventHandler(this.listBox_DrawItem);
			this.listBox.MeasureItem += new MeasureItemEventHandler(this.listBox_MeasureItem);
			this.listBox.BeginUpdate();
			this.listBox.Items.Add("(None)");
			this.listBox.Items.Add("(Browse)");
			this.listBox.EndUpdate();
			this.listBox.Height = 45;
			this.listBox.Width = 100;
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			object obj;
			if (context != null && context.Instance != null && provider != null)
			{
				using (FormSelectIcon formSelectIcon = null)
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
										if (!((string)value).Equals("(None)", StringComparison.OrdinalIgnoreCase))
										{
											this.listBox.SelectedIndex = -1;
										}
										else
										{
											this.listBox.SelectedIndex = 0;
										}
										this.edSvc.DropDownControl(this.listBox);
										if (this.listBox.SelectedIndex > -1)
										{
											int selectedIndex = this.listBox.SelectedIndex;
											if (selectedIndex == 0)
											{
												value = "(None)";
												this.propertiesObject.AddRemoveProgramsIcon = null;
											}
											else if (selectedIndex == 1)
											{
												formSelectIcon = new FormSelectIcon(this.propertiesObject.Project.RootDirectory, this.propertiesObject.AddRemoveProgramsIcon);
												if (this.ShowForm(provider, formSelectIcon) == DialogResult.OK)
												{
													this.propertiesObject.AddRemoveProgramsIcon = formSelectIcon.SelectedIcon;
													if (!string.IsNullOrEmpty(formSelectIcon.SelectedIcon))
													{
														value = "(Icon)";
													}
													else
													{
														value = "(None)";
													}
													obj = value;
													return obj;
												}
											}
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
				return obj;
			}
			return value;
		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			if (context != null && context.Instance != null)
			{
				return UITypeEditorEditStyle.DropDown;
			}
			return base.GetEditStyle(context);
		}

		private int GetLinesNumber(string text)
		{
			int num = 1;
			int num1 = 0;
			while (true)
			{
				int num2 = text.IndexOf("\r\n", num1);
				num1 = num2;
				if (num2 == -1)
				{
					break;
				}
				num++;
				num1 += 2;
			}
			return num;
		}

		private void listBox_DrawItem(object sender, DrawItemEventArgs e)
		{
			e.DrawBackground();
			e.DrawFocusRectangle();
			e.Graphics.DrawString((string)this.listBox.Items[e.Index], e.Font, new SolidBrush(e.ForeColor), e.Bounds);
		}

		private void listBox_MeasureItem(object sender, MeasureItemEventArgs e)
		{
			e.ItemHeight = 18;
		}

		private void listBox_MouseUp(object sender, MouseEventArgs e)
		{
			this.edSvc.CloseDropDown();
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

		private string ShowOpenFileDialog(string projectDir, string defaultValue)
		{
			string str;
			string empty = string.Empty;
			string fileName = string.Empty;
			OpenFileDialog openFileDialog = null;
			using (string fullPath = defaultValue)
			{
				if (!string.IsNullOrEmpty(fullPath))
				{
					try
					{
						if (!Path.IsPathRooted(fullPath))
						{
							fullPath = Path.GetFullPath(Path.Combine(projectDir, fullPath));
						}
						empty = Path.GetDirectoryName(fullPath);
						fileName = Path.GetFileName(fullPath);
					}
					catch (Exception exception)
					{
					}
				}
				openFileDialog = new OpenFileDialog()
				{
					DefaultExt = "ico",
					Filter = "Icon Files|*.ico|All Files|*.*",
					Multiselect = false,
					Title = "Icon"
				};
				if (!string.IsNullOrEmpty(empty))
				{
					openFileDialog.InitialDirectory = empty;
				}
				if (!string.IsNullOrEmpty(fileName))
				{
					openFileDialog.FileName = fileName;
				}
				if (openFileDialog.ShowDialog(this) == DialogResult.OK)
				{
					if (!openFileDialog.FileName.EndsWith(".ico", StringComparison.OrdinalIgnoreCase))
					{
						MessageBox.Show(this, "Not a valid file type for this property. Only .ico files are supported.", Common.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					}
					else
					{
						string fileName1 = openFileDialog.FileName;
						if (!string.IsNullOrEmpty(openFileDialog.FileName) && Path.IsPathRooted(openFileDialog.FileName))
						{
							fileName1 = Path.Combine(CommonUtilities.RelativizePathIfPossible(Path.GetDirectoryName(fileName1), projectDir), Path.GetFileName(fileName1));
						}
						str = fileName1;
						return str;
					}
				}
				return defaultValue;
			}
			return str;
		}
	}
}