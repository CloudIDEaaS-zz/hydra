using System;
using System.ComponentModel;
using System.Globalization;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class TargetNamePropertyConverter : TypeConverter
	{
		public TargetNamePropertyConverter()
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
				VsWiXProject.ReferenceDescriptor referenceDescriptor = (context.Instance as VSProjectOutputFile).ReferenceDescriptor;
				if (referenceDescriptor != null && referenceDescriptor.ReferencedProject != null)
				{
					return referenceDescriptor.ReferencedProject.KeyOutput.TargetName;
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}