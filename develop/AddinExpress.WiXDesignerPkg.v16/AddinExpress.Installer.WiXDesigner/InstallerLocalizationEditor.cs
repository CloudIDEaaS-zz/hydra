using AddinExpress.Installer.WiXDesigner.DesignTime;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class InstallerLocalizationEditor : UITypeEditor, IWin32Window
	{
		private IWindowsFormsEditorService edSvc;

		private ListBox listBox;

		private List<int> lcidList = new List<int>();

		private VsWiXProject.ProjectPropertiesObject propertiesObject;

		public IntPtr Handle
		{
			get
			{
				return VsPackage.CurrentInstance.Handle;
			}
		}

		public InstallerLocalizationEditor()
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
			this.listBox.Items.Add("Neutral *");
			this.lcidList.Add(CultureInfo.InvariantCulture.LCID);
			this.listBox.EndUpdate();
			this.listBox.Height = 150;
			this.listBox.Width = 100;
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			int num;
			int i;
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
										string str = (string)value;
										if (!this.propertiesObject.Project.IsMultiLangSupported)
										{
											SortedList<string, CultureInfo> strs = new SortedList<string, CultureInfo>();
											CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
											for (i = 0; i < (int)cultures.Length; i++)
											{
												CultureInfo cultureInfo = cultures[i];
												strs[cultureInfo.EnglishName] = cultureInfo;
											}
											foreach (KeyValuePair<string, CultureInfo> keyValuePair in strs)
											{
												if (ProjectUtilities.IsNeutralLCID(keyValuePair.Value.LCID) || this.lcidList.Contains(keyValuePair.Value.LCID))
												{
													continue;
												}
												this.listBox.Items.Add(this.MarkSupportedLanguage(keyValuePair.Value.EnglishName, keyValuePair.Value.LCID));
												this.lcidList.Add(keyValuePair.Value.LCID);
											}
										}
										else
										{
											foreach (string language in this.propertiesObject.Languages)
											{
												CultureInfo cultureInfo1 = null;
												if (int.TryParse(language, out num))
												{
													try
													{
														cultureInfo1 = CultureInfo.GetCultureInfo(num);
													}
													catch (Exception exception)
													{
													}
												}
												if (cultureInfo1 == null)
												{
													continue;
												}
												this.listBox.Items.Add(this.MarkSupportedLanguage(cultureInfo1.EnglishName, cultureInfo1.LCID));
												this.lcidList.Add(cultureInfo1.LCID);
											}
										}
										if (this.listBox.FindString(str) != -1)
										{
											this.listBox.SelectedIndex = this.listBox.FindString(str);
										}
										else if (this.listBox.Items.Count > 0)
										{
											this.listBox.SelectedIndex = 0;
										}
										this.edSvc.DropDownControl(this.listBox);
										if (this.listBox.SelectedIndex > -1)
										{
											i = this.lcidList[this.listBox.SelectedIndex];
											value = i.ToString();
										}
									}
								}
							}
						}
					}
					catch (Exception exception2)
					{
						Exception exception1 = exception2;
						MessageBox.Show(this, exception1.Message, exception1.Source, MessageBoxButtons.OK, MessageBoxIcon.Hand);
					}
				}
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

		private string MarkSupportedLanguage(string langName, int lcid)
		{
			if (lcid <= 1049)
			{
				switch (lcid)
				{
					case 1028:
					case 1029:
					case 1031:
					case 1033:
					case 1036:
					case 1040:
					case 1041:
					case 1042:
					case 1045:
					case 1046:
					{
						break;
					}
					case 1030:
					case 1032:
					case 1034:
					case 1035:
					case 1037:
					case 1038:
					case 1039:
					case 1043:
					case 1044:
					{
						return langName;
					}
					default:
					{
						if (lcid == 1049)
						{
							break;
						}
						return langName;
					}
				}
			}
			else if (lcid != 1055 && lcid != 2052 && lcid != 3082)
			{
				return langName;
			}
			return string.Concat(langName, " *");
		}
	}
}