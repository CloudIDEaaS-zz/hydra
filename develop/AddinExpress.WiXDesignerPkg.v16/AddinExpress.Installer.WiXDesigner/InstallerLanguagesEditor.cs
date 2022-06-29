using AddinExpress.Installer.WiXDesigner.DesignTime;
using System;
using System.Collections;
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
	internal class InstallerLanguagesEditor : UITypeEditor, IWin32Window
	{
		private IWindowsFormsEditorService edSvc;

		private CheckedListBox listBox;

		private List<int> lcidList = new List<int>();

		private VsWiXProject.ProjectPropertiesObject propertiesObject;

		public IntPtr Handle
		{
			get
			{
				return VsPackage.CurrentInstance.Handle;
			}
		}

		public InstallerLanguagesEditor()
		{
			this.listBox = new CheckedListBox()
			{
				Sorted = false,
				CheckOnClick = true,
				BorderStyle = BorderStyle.None,
				DrawMode = DrawMode.OwnerDrawVariable
			};
			this.listBox.DrawItem += new DrawItemEventHandler(this.listBox_DrawItem);
			this.listBox.MeasureItem += new MeasureItemEventHandler(this.listBox_MeasureItem);
			this.listBox.BeginUpdate();
			SortedList<string, CultureInfo> strs = new SortedList<string, CultureInfo>();
			CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
			for (int i = 0; i < (int)cultures.Length; i++)
			{
				CultureInfo cultureInfo = cultures[i];
				int lCID = cultureInfo.LCID;
				int length = lCID.ToString().Length;
				strs[cultureInfo.EnglishName] = cultureInfo;
			}
			foreach (KeyValuePair<string, CultureInfo> str in strs)
			{
				if (ProjectUtilities.IsNeutralLCID(str.Value.LCID) || this.lcidList.Contains(str.Value.LCID))
				{
					continue;
				}
				this.listBox.Items.Add(this.MarkSupportedLanguage(str.Value.EnglishName, str.Value.LCID));
				this.lcidList.Add(str.Value.LCID);
			}
			this.listBox.EndUpdate();
			this.listBox.Height = 200;
			this.listBox.Width = 120;
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			int num;
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
										List<string> languages = this.propertiesObject.Languages;
										if (languages.Count > 0)
										{
											int num1 = 0;
											foreach (string language in languages)
											{
												if (!int.TryParse(language, out num))
												{
													continue;
												}
												int num2 = this.lcidList.IndexOf(num);
												if (num2 < 0)
												{
													continue;
												}
												this.lcidList.RemoveAt(num2);
												this.lcidList.Insert(num1, num);
												string item = (string)this.listBox.Items[num2];
												this.listBox.Items.RemoveAt(num2);
												this.listBox.Items.Insert(num1, item);
												this.listBox.SetItemChecked(num1, true);
												num1++;
											}
										}
										this.edSvc.DropDownControl(this.listBox);
										List<string> strs = new List<string>();
										if (this.listBox.CheckedIndices.Count > 0)
										{
											foreach (int checkedIndex in this.listBox.CheckedIndices)
											{
												strs.Add(this.lcidList[checkedIndex].ToString());
											}
										}
										this.propertiesObject.Languages = strs;
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