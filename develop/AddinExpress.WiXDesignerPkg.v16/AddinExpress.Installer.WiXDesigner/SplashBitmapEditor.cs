using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class SplashBitmapEditor : UITypeEditor
	{
		private IWindowsFormsEditorService edSvc;

		private ListBox listBox;

		private VSDialogBase vsDialog;

		public SplashBitmapEditor()
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
						this.vsDialog = context.Instance as VSDialogBase;
						if (this.vsDialog != null && this.vsDialog.Project != null)
						{
							if (value == null || value == VSBaseFile.Empty)
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
								int selectedIndex = this.listBox.SelectedIndex;
								if (selectedIndex == 0)
								{
									value = null;
								}
								else if (selectedIndex == 1)
								{
									VSBaseFolder folderById = null;
									string empty = string.Empty;
									if (value is VSBinary)
									{
										folderById = this.vsDialog.Project.FileSystem.GetFolderById("TARGETDIR");
										empty = Path.GetFileName((value as VSBinary).SourcePath);
									}
									else if (value is VSFile)
									{
										folderById = (value as VSFile).ParentFolder;
										empty = Path.GetFileName((value as VSFile).SourcePath);
									}
									value = this.ShowDialog(folderById, empty, value);
								}
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

		private object ShowDialog(VSBaseFolder parent, string fileName, object defaultValue)
		{
			object selectedFile;
			FormSelectItemInProject formSelectItemInProject = new FormSelectItemInProject();
			try
			{
				formSelectItemInProject.Initialize(parent, fileName, this.vsDialog.Project.FileSystem, this.vsDialog.Project.Binaries, new string[] { "Image Files (*.bmp;*.jpg)", "All Files (*.*)" });
				if (formSelectItemInProject.ShowDialog() == DialogResult.OK)
				{
					string empty = string.Empty;
					string sourcePath = string.Empty;
					if (formSelectItemInProject.SelectedFile is VSFile)
					{
						sourcePath = (formSelectItemInProject.SelectedFile as VSFile).SourcePath;
					}
					else if (formSelectItemInProject.SelectedFile is VSBinary)
					{
						sourcePath = (formSelectItemInProject.SelectedFile as VSBinary).SourcePath;
					}
					if (string.IsNullOrEmpty(sourcePath) || !sourcePath.EndsWith(".bmp") && !sourcePath.EndsWith(".jpg"))
					{
						MessageBox.Show("Not a valid file type for an image.", Common.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
					}
					else
					{
						selectedFile = formSelectItemInProject.SelectedFile;
						return selectedFile;
					}
				}
				return defaultValue;
			}
			finally
			{
				formSelectItemInProject.Dispose();
			}
			return selectedFile;
		}
	}
}