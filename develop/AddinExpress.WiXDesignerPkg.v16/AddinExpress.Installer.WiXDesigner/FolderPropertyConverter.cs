using System;
using System.ComponentModel;
using System.Globalization;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class FolderPropertyConverter : TypeConverter
	{
		public FolderPropertyConverter()
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
				VSBaseFolder vSBaseFolder = context.PropertyDescriptor.GetValue(context.Instance) as VSBaseFolder;
				if (vSBaseFolder != null)
				{
					return vSBaseFolder.Name;
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}