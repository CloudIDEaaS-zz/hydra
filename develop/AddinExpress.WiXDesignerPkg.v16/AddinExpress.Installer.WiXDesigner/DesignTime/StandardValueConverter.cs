using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace AddinExpress.Installer.WiXDesigner.DesignTime
{
	internal class StandardValueConverter : TypeConverter
	{
		public StandardValueConverter()
		{
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (context != null && context.PropertyDescriptor != null && context.PropertyDescriptor is AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor && sourceType == typeof(string))
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (context != null && context.PropertyDescriptor != null && context.PropertyDescriptor is AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor && (destinationType == typeof(string) || destinationType == typeof(StandardValue)))
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			object obj;
			if (context == null || context.PropertyDescriptor == null || !(context.PropertyDescriptor is AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor) || value == null)
			{
				return base.ConvertFrom(context, culture, value);
			}
			AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor propertyDescriptor = context.PropertyDescriptor as AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor;
			if (value is string)
			{
				using (IEnumerator<StandardValue> enumerator = propertyDescriptor.StandardValues.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						StandardValue current = enumerator.Current;
						if (string.Compare(value.ToString(), current.DisplayName, true, culture) != 0 && string.Compare(value.ToString(), current.Value.ToString(), true, culture) != 0)
						{
							continue;
						}
						obj = current.Value;
						return obj;
					}
					return System.ComponentModel.TypeDescriptor.GetConverter(context.PropertyDescriptor.PropertyType).ConvertFrom(context, culture, value);
				}
				return obj;
			}
			else if (value is StandardValue)
			{
				return (value as StandardValue).Value;
			}
			return System.ComponentModel.TypeDescriptor.GetConverter(context.PropertyDescriptor.PropertyType).ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			object displayName;
			if (context == null || context.PropertyDescriptor == null || !(context.PropertyDescriptor is AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor) || value == null)
			{
				return base.ConvertTo(context, culture, value, destinationType);
			}
			AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor propertyDescriptor = context.PropertyDescriptor as AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor;
			if (value is string)
			{
				if (destinationType == typeof(string))
				{
					return value;
				}
				if (destinationType == propertyDescriptor.PropertyType)
				{
					return this.ConvertFrom(context, culture, value);
				}
				if (destinationType == typeof(StandardValue))
				{
					foreach (StandardValue standardValue in propertyDescriptor.StandardValues)
					{
						if (string.Compare(value.ToString(), standardValue.DisplayName, true, culture) != 0 && string.Compare(value.ToString(), standardValue.Value.ToString(), true, culture) != 0)
						{
							continue;
						}
						displayName = standardValue;
						return displayName;
					}
				}
			}
			else if (value.GetType() == propertyDescriptor.PropertyType)
			{
				if (destinationType == typeof(string))
				{
					foreach (StandardValue standardValue1 in propertyDescriptor.StandardValues)
					{
						if (!standardValue1.Value.Equals(value))
						{
							continue;
						}
						displayName = standardValue1.DisplayName;
						return displayName;
					}
				}
				else if (destinationType == typeof(StandardValue))
				{
					foreach (StandardValue standardValue2 in propertyDescriptor.StandardValues)
					{
						if (!standardValue2.Value.Equals(value))
						{
							continue;
						}
						displayName = standardValue2;
						return displayName;
					}
				}
			}
			return System.ComponentModel.TypeDescriptor.GetConverter(context.PropertyDescriptor.PropertyType).ConvertTo(context, culture, value, destinationType);
		}

		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			if (context == null || context.PropertyDescriptor == null || !(context.PropertyDescriptor is AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor))
			{
				return base.GetStandardValues(context);
			}
			AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor propertyDescriptor = context.PropertyDescriptor as AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor;
			List<object> objs = new List<object>();
			foreach (StandardValue standardValue in propertyDescriptor.StandardValues)
			{
				objs.Add(standardValue.Value);
			}
			return new TypeConverter.StandardValuesCollection(objs);
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			if (context != null && context.PropertyDescriptor != null)
			{
				ExclusiveStandardValuesAttribute exclusiveStandardValuesAttribute = (ExclusiveStandardValuesAttribute)context.PropertyDescriptor.Attributes.Get(typeof(ExclusiveStandardValuesAttribute), true);
				if (exclusiveStandardValuesAttribute != null)
				{
					return exclusiveStandardValuesAttribute.Exclusive;
				}
			}
			return base.GetStandardValuesExclusive(context);
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			if (context == null || context.PropertyDescriptor == null || !(context.PropertyDescriptor is AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor))
			{
				return base.GetStandardValuesSupported(context);
			}
			return (context.PropertyDescriptor as AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor).StandardValues.Count > 0;
		}
	}
}