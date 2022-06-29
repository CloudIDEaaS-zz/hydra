using System;
using System.ComponentModel;
using System.Globalization;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class IconPropertyConverter : TypeConverter
	{
		public IconPropertyConverter()
		{
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type sourceType)
		{
			return true;
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType != typeof(string))
			{
				return base.ConvertTo(context, culture, value, destinationType);
			}
			if (string.IsNullOrEmpty((string)context.PropertyDescriptor.GetValue(context.Instance)))
			{
				return "(None)";
			}
			return "(Icon)";
		}
	}
}