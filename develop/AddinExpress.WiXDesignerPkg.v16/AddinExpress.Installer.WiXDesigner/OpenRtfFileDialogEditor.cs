using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class OpenRtfFileDialogEditor : UITypeEditor, IWin32Window
	{
		private IWindowsFormsEditorService edSvc;

		private ListBox listBox;

		private IntPtr hwnd = IntPtr.Zero;

		public IntPtr Handle
		{
			get
			{
				return VsPackage.CurrentInstance.Handle;
			}
		}

		public OpenRtfFileDialogEditor()
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
			if (context != null && context.Instance != null && provider != null)
			{
				try
				{
					this.edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
					if (this.edSvc != null)
					{
						string str = (string)value;
						if (!string.IsNullOrEmpty(str))
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
							if (this.listBox.SelectedIndex != 0)
							{
								OpenFileDialog openFileDialog = new OpenFileDialog()
								{
									DefaultExt = "rtf",
									Filter = "Rich Text Format Files|*.rtf|All Files|*.*",
									Multiselect = false
								};
								if (context.Instance is VSEulaDialog)
								{
									openFileDialog.Title = "Select License File";
								}
								else if (context.Instance is VSReadmeDialog)
								{
									openFileDialog.Title = "Select Readme File";
								}
								if (!string.IsNullOrEmpty(str))
								{
									try
									{
										openFileDialog.InitialDirectory = Path.GetDirectoryName(str);
										openFileDialog.FileName = Path.GetFileName(str);
									}
									catch (Exception exception)
									{
									}
								}
								if (openFileDialog.ShowDialog(this) == DialogResult.OK)
								{
									if (!openFileDialog.FileName.EndsWith(".rtf", StringComparison.OrdinalIgnoreCase))
									{
										MessageBox.Show(this, "Not a valid file type for this property. Only .rtf files are supported.", Common.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
									}
									else
									{
										value = openFileDialog.FileName;
									}
								}
							}
							else
							{
								value = string.Empty;
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
	}
}