using System;
using System.ComponentModel;
using System.Globalization;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class OutputsPropertyConverter : TypeConverter
	{
		public OutputsPropertyConverter()
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
			if (value == null)
			{
				return "(None)";
			}
			if (value is Array && (value as Array).GetLength(0) == 0)
			{
				return "(None)";
			}
			return "(Outputs)";
		}
	}
}