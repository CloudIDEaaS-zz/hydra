using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text;

namespace AddinExpress.Installer.WiXDesigner.DesignTime
{
	internal class EnumConverter : System.ComponentModel.EnumConverter
	{
		public EnumConverter(Type type) : base(type)
		{
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(StandardValue))
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value == null)
			{
				return base.ConvertFrom(context, culture, value);
			}
			if (!(value is string))
			{
				if (value is StandardValue)
				{
					return (value as StandardValue).Value;
				}
				return base.ConvertFrom(context, culture, value);
			}
			string[] strArrays = (value as string).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			StringBuilder stringBuilder = new StringBuilder(1000);
			string[] strArrays1 = strArrays;
			for (int i = 0; i < (int)strArrays1.Length; i++)
			{
				string str = strArrays1[i].Trim();
				StandardValue[] allPossibleValues = this.GetAllPossibleValues(context);
				for (int j = 0; j < (int)allPossibleValues.Length; j++)
				{
					StandardValue standardValue = allPossibleValues[j];
					this.UpdateStringFromResource(context, standardValue);
					if (string.Compare(standardValue.Value.ToString(), str, true) == 0 || string.Compare(standardValue.DisplayName, str, true) == 0)
					{
						if (stringBuilder.Length > 0)
						{
							stringBuilder.Append(",");
						}
						stringBuilder.Append(standardValue.Value.ToString());
					}
				}
			}
			return Enum.Parse(base.EnumType, stringBuilder.ToString(), true);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			StandardValue[] allPossibleValues;
			int i;
			if (value == null)
			{
				return base.ConvertTo(context, culture, value, destinationType);
			}
			if (value is string)
			{
				if (destinationType == typeof(string))
				{
					return value;
				}
				if (destinationType == base.EnumType)
				{
					return this.ConvertFrom(context, culture, value);
				}
				if (destinationType == typeof(StandardValue))
				{
					allPossibleValues = this.GetAllPossibleValues(context);
					for (i = 0; i < (int)allPossibleValues.Length; i++)
					{
						StandardValue standardValue = allPossibleValues[i];
						this.UpdateStringFromResource(context, standardValue);
						if (string.Compare(value.ToString(), standardValue.DisplayName, true, culture) == 0 || string.Compare(value.ToString(), standardValue.Value.ToString(), true, culture) == 0)
						{
							return standardValue;
						}
					}
				}
			}
			else if (value.GetType() == base.EnumType)
			{
				if (destinationType == typeof(string))
				{
					string[] strArrays = Enum.Format(base.EnumType, value, "G").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
					StringBuilder stringBuilder = new StringBuilder(1000);
					string[] strArrays1 = strArrays;
					for (i = 0; i < (int)strArrays1.Length; i++)
					{
						string str = strArrays1[i].Trim();
						allPossibleValues = this.GetAllPossibleValues(context);
						for (int j = 0; j < (int)allPossibleValues.Length; j++)
						{
							StandardValue standardValue1 = allPossibleValues[j];
							this.UpdateStringFromResource(context, standardValue1);
							if (string.Compare(standardValue1.Value.ToString(), str, true) == 0 || string.Compare(standardValue1.DisplayName, str, true) == 0)
							{
								if (stringBuilder.Length > 0)
								{
									stringBuilder.Append(", ");
								}
								stringBuilder.Append(standardValue1.DisplayName);
							}
						}
					}
					return stringBuilder.ToString();
				}
				if (destinationType == typeof(StandardValue))
				{
					allPossibleValues = this.GetAllPossibleValues(context);
					for (i = 0; i < (int)allPossibleValues.Length; i++)
					{
						StandardValue standardValue2 = allPossibleValues[i];
						if (standardValue2.Value.Equals(value))
						{
							this.UpdateStringFromResource(context, standardValue2);
							return standardValue2;
						}
					}
				}
				else if (destinationType == base.EnumType)
				{
					return value;
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		private StandardValue[] GetAllPossibleValues(ITypeDescriptorContext context)
		{
			List<StandardValue> standardValues = new List<StandardValue>();
			if (context == null || context.PropertyDescriptor == null || !(context.PropertyDescriptor is AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor))
			{
				standardValues.AddRange(EnumUtil.GetStandardValues(base.EnumType));
			}
			else
			{
				standardValues.AddRange((context.PropertyDescriptor as AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor).StandardValues);
			}
			return standardValues.ToArray();
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			DefaultValueAttribute defaultValueAttribute = context.PropertyDescriptor.Attributes.Get(typeof(DefaultValueAttribute)) as DefaultValueAttribute;
			PropertyDescriptorCollection propertyDescriptorCollections = new PropertyDescriptorCollection(null, false);
			StandardValue[] allPossibleValues = this.GetAllPossibleValues(context);
			for (int i = 0; i < (int)allPossibleValues.Length; i++)
			{
				StandardValue standardValue = allPossibleValues[i];
				if (standardValue.Visible)
				{
					this.UpdateStringFromResource(context, standardValue);
					EnumChildPropertyDescriptor enumChildPropertyDescriptor = new EnumChildPropertyDescriptor(context, standardValue.Value.ToString(), standardValue.Value, Array.Empty<Attribute>());
					enumChildPropertyDescriptor.Attributes.Add(new ReadOnlyAttribute(!standardValue.Enabled), true);
					enumChildPropertyDescriptor.Attributes.Add(new DescriptionAttribute(standardValue.Description), true);
					enumChildPropertyDescriptor.Attributes.Add(new System.ComponentModel.DisplayNameAttribute(standardValue.DisplayName), true);
					enumChildPropertyDescriptor.Attributes.Add(new BrowsableAttribute(standardValue.Visible), true);
					if (defaultValueAttribute != null)
					{
						bool flag = EnumUtil.IsBitsOn(defaultValueAttribute.Value, standardValue.Value);
						enumChildPropertyDescriptor.DefaultValue = flag;
					}
					propertyDescriptorCollections.Add(enumChildPropertyDescriptor);
				}
			}
			return propertyDescriptorCollections;
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			ExpandEnumAttribute expandEnumAttribute = null;
			expandEnumAttribute = (context == null || !(context.PropertyDescriptor is AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor) ? (ExpandEnumAttribute)System.ComponentModel.TypeDescriptor.GetAttributes(base.EnumType).Get(typeof(ExpandableIEnumerationConverter), true) : (ExpandEnumAttribute)(context.PropertyDescriptor as AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor).Attributes.Get(typeof(ExpandEnumAttribute), true));
			if (expandEnumAttribute == null)
			{
				return false;
			}
			return expandEnumAttribute.Exapand;
		}

		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			List<object> objs = new List<object>();
			StandardValue[] allPossibleValues = this.GetAllPossibleValues(context);
			for (int i = 0; i < (int)allPossibleValues.Length; i++)
			{
				objs.Add(allPossibleValues[i].Value);
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

		private void UpdateStringFromResource(ITypeDescriptorContext context, StandardValue sv)
		{
			ResourceAttribute resourceAttribute = null;
			if (context != null && context.PropertyDescriptor != null)
			{
				resourceAttribute = (ResourceAttribute)context.PropertyDescriptor.Attributes.Get(typeof(ResourceAttribute));
			}
			if (resourceAttribute == null)
			{
				resourceAttribute = (ResourceAttribute)System.ComponentModel.TypeDescriptor.GetAttributes(base.EnumType).Get(typeof(ResourceAttribute));
			}
			if (resourceAttribute == null)
			{
				return;
			}
			ResourceManager resourceManager = null;
			try
			{
				if (!string.IsNullOrEmpty(resourceAttribute.BaseName) && !string.IsNullOrEmpty(resourceAttribute.AssemblyFullName))
				{
					resourceManager = new ResourceManager(resourceAttribute.BaseName, Assembly.ReflectionOnlyLoad(resourceAttribute.AssemblyFullName));
				}
				else if (string.IsNullOrEmpty(resourceAttribute.BaseName))
				{
					resourceManager = (string.IsNullOrEmpty(resourceAttribute.BaseName) ? new ResourceManager(base.EnumType) : new ResourceManager(resourceAttribute.BaseName, base.EnumType.Assembly));
				}
				else
				{
					resourceManager = new ResourceManager(resourceAttribute.BaseName, base.EnumType.Assembly);
				}
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception.Message);
				return;
			}
			string str = string.Concat(resourceAttribute.KeyPrefix, sv.Value.ToString(), "_Name");
			string str1 = string.Concat(resourceAttribute.KeyPrefix, sv.Value.ToString(), "_Desc");
			string empty = string.Empty;
			string empty1 = string.Empty;
			try
			{
				empty = resourceManager.GetString(str);
			}
			catch (Exception exception1)
			{
				Console.WriteLine(exception1.Message);
			}
			if (!string.IsNullOrEmpty(empty))
			{
				sv.DisplayName = empty;
			}
			try
			{
				empty1 = resourceManager.GetString(str1);
			}
			catch (Exception exception2)
			{
				Console.WriteLine(exception2.Message);
			}
			if (!string.IsNullOrEmpty(empty1))
			{
				sv.Description = empty1;
			}
		}
	}
}