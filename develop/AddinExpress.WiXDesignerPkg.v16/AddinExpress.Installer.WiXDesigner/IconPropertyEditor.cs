using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class IconPropertyEditor : ObjectSelectorEditor
	{
		private IWindowsFormsEditorService _edSvc;

		private ListBox _listBox;

		private string _strNone = "(None)";

		private string _strBrowse = "(Browse...)";

		public IconPropertyEditor()
		{
			this._listBox = new ListBox();
			this._listBox.MouseUp += new MouseEventHandler(this.listBoxMouseUp);
			this._listBox.BorderStyle = BorderStyle.None;
			this._listBox.Items.Add(this._strNone);
			this._listBox.Items.Add(this._strBrowse);
		}

		public IconPropertyEditor(bool subObjectSelector) : base(subObjectSelector)
		{
			this._listBox = new ListBox();
			this._listBox.MouseUp += new MouseEventHandler(this.listBoxMouseUp);
			this._listBox.BorderStyle = BorderStyle.None;
			this._listBox.Items.Add(this._strNone);
			this._listBox.Items.Add(this._strBrowse);
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			string str = (string)context.PropertyDescriptor.GetValue(context.Instance);
			this._edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			if (this._edSvc != null)
			{
				this._listBox.SelectedIndex = -1;
				this._edSvc.DropDownControl(this._listBox);
				if (this._listBox.SelectedIndex > -1)
				{
					if ((string)this._listBox.Items[this._listBox.SelectedIndex] == this._strNone)
					{
						return string.Empty;
					}
					if ((string)this._listBox.Items[this._listBox.SelectedIndex] == this._strBrowse)
					{
						FormSelectIcon formSelectIcon = null;
						if (context.Instance is VSShortcut)
						{
							formSelectIcon = new FormSelectIcon(context.Instance as VSShortcut, str);
						}
						else if (context.Instance is VSFileType)
						{
							formSelectIcon = new FormSelectIcon(context.Instance as VSFileType, str);
						}
						if (this.ShowForm(provider, formSelectIcon) == DialogResult.OK)
						{
							return formSelectIcon.SelectedIcon;
						}
						formSelectIcon.Dispose();
					}
				}
			}
			return str;
		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}

		private void listBoxMouseUp(object sender, MouseEventArgs e)
		{
			if (this._edSvc != null)
			{
				this._edSvc.CloseDropDown();
			}
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