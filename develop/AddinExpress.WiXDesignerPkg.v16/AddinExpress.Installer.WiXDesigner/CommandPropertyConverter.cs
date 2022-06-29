using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class CommandPropertyConverter : TypeConverter
	{
		public CommandPropertyConverter()
		{
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type sourceType)
		{
			return true;
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string) && value != null && !string.IsNullOrEmpty(value.ToString()))
			{
				VSFileType instance = context.Instance as VSFileType;
				if (instance != null)
				{
					if (instance.ParentOutput != null)
					{
						return instance.ParentOutput.KeyOutput.TargetName;
					}
					if (instance.ParentComponent != null && instance.ParentComponent.Files.Count > 0 && instance.ParentComponent.Files[0] is VSFile)
					{
						return (instance.ParentComponent.Files[0] as VSFile).TargetName;
					}
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}