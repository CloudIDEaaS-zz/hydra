using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class SplashBitmapConverter : TypeConverter
	{
		public SplashBitmapConverter()
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
			VSBaseFile vSBaseFile = context.PropertyDescriptor.GetValue(context.Instance) as VSBaseFile;
			if (vSBaseFile != null)
			{
				if (vSBaseFile is VSBinary)
				{
					return Path.GetFileName((vSBaseFile as VSBinary).SourcePath);
				}
				if (vSBaseFile is VSFile)
				{
					return Path.GetFileName((vSBaseFile as VSFile).SourcePath);
				}
			}
			return "(None)";
		}
	}
}