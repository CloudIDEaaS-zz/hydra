using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class CheckboxValueEditor : UITypeEditor
	{
		private IWindowsFormsEditorService edSvc;

		private ListBox listBox;

		public CheckboxValueEditor()
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
			this.listBox.Items.Add("Unchecked");
			this.listBox.Items.Add("Checked");
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
						if (!(bool)value)
						{
							this.listBox.SelectedIndex = 0;
						}
						else
						{
							this.listBox.SelectedIndex = 1;
						}
						this.edSvc.DropDownControl(this.listBox);
						if (this.listBox.SelectedIndex > -1)
						{
							if (this.listBox.SelectedIndex != 0)
							{
								value = true;
							}
							else
							{
								value = false;
							}
						}
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					MessageBox.Show(exception.Message, exception.Source, MessageBoxButtons.OK, MessageBoxIcon.Hand);
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