using System;
using System.ComponentModel;
using System.Globalization;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class TargetPropertyConverter : TypeConverter
	{
		public TargetPropertyConverter()
		{
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type sourceType)
		{
			return true;
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string) && !string.IsNullOrEmpty(value.ToString()))
			{
				if (!value.ToString().StartsWith("[") || !value.ToString().EndsWith("]"))
				{
					if (!value.ToString().StartsWith("["))
					{
						return value;
					}
					return value.ToString().Substring(value.ToString().LastIndexOf("]") + 1);
				}
				string str = value.ToString().Substring(1, value.ToString().Length - 2);
				if (context.Instance is VSShortcut)
				{
					VSBaseFolder folderById = (context.Instance as VSShortcut).Project.FileSystem.GetFolderById(str);
					if (folderById != null)
					{
						return folderById.Name;
					}
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}