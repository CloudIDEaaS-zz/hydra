using System;
using System.ComponentModel;
using System.Globalization;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class FilesPropertyConverter : TypeConverter
	{
		public FilesPropertyConverter()
		{
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type sourceType)
		{
			return true;
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				return "(Files)";
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}