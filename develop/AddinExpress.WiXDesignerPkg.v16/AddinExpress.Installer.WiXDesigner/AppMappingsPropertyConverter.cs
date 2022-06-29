using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class AppMappingsPropertyConverter : TypeConverter
	{
		public AppMappingsPropertyConverter()
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
			if (value == VSBaseFile.Empty)
			{
				return "(None)";
			}
			if (value is List<VSWebApplicationExtension> && (value as List<VSWebApplicationExtension>).Count == 0)
			{
				return "(None)";
			}
			if (value is Array && (value as Array).GetLength(0) == 0)
			{
				return "(None)";
			}
			return "(Mappings)";
		}
	}
}