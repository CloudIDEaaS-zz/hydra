using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace AddinExpress.Installer.WiXDesigner.DesignTime
{
	internal class PropertyValuePaintEditor : UITypeEditor
	{
		public PropertyValuePaintEditor()
		{
		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.None;
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