using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace AddinExpress.Installer.WiXDesigner.DesignTime
{
	internal class StandardValueEditor : UITypeEditor
	{
		private StandardValueEditorUI m_ui = new StandardValueEditorUI();

		public override bool IsDropDownResizable
		{
			get
			{
				return true;
			}
		}

		public StandardValueEditor()
		{
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (provider != null)
			{
				IWindowsFormsEditorService service = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
				if (service == null)
				{
					return value;
				}
				this.m_ui.SetData(context, service, value);
				service.DropDownControl(this.m_ui);
				value = this.m_ui.GetValue();
			}
			return value;
		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}

		public override bool GetPaintValueSupported(ITypeDescriptorContext context)
		{
			if (context == null || !(context.PropertyDescriptor is AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor))
			{
				return base.GetPaintValueSupported(context);
			}
			return (context.PropertyDescriptor as AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor).ValueImage != null;
		}

		public override void PaintValue(PaintValueEventArgs pe)
		{
			if (pe.Context != null && pe.Context.PropertyDescriptor != null && pe.Context.PropertyDescriptor is AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor)
			{
				AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor propertyDescriptor = pe.Context.PropertyDescriptor as AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor;
				if (propertyDescriptor.ValueImage != null)
				{
					pe.Graphics.DrawImage(propertyDescriptor.ValueImage, pe.Bounds);
					return;
				}
			}
			base.PaintValue(pe);
		}
	}
}