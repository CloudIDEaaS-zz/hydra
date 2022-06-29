using System;
using System.ComponentModel;
using System.Globalization;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class KeyOutputPropertyConverter : ExpandableObjectConverter
	{
		public KeyOutputPropertyConverter()
		{
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return base.CanConvertTo(context, destinationType);
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
			KeyOutputDescriptorClass keyOutputDescriptorClass = context.PropertyDescriptor.GetValue(context.Instance) as KeyOutputDescriptorClass;
			if (keyOutputDescriptorClass != null && string.IsNullOrEmpty(keyOutputDescriptorClass.SourcePath))
			{
				return "(None)";
			}
			return string.Empty;
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			KeyOutputDescriptorClass value = (KeyOutputDescriptorClass)context.PropertyDescriptor.GetValue(context.Instance);
			if (value != null && string.IsNullOrEmpty(value.SourcePath))
			{
				return false;
			}
			return true;
		}
	}
}