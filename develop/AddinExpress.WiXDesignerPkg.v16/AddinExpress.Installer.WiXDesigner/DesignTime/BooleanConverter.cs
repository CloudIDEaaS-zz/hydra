using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace AddinExpress.Installer.WiXDesigner.DesignTime
{
	internal class BooleanConverter : System.ComponentModel.BooleanConverter
	{
		public BooleanConverter()
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
			if (value is string)
			{
				string str = value as string;
				str = str.Trim();
				StandardValue[] allPossibleValues = this.GetAllPossibleValues(context);
				for (int i = 0; i < (int)allPossibleValues.Length; i++)
				{
					StandardValue standardValue = allPossibleValues[i];
					this.UpdateStringFromResource(context, standardValue);
					if (string.Compare(standardValue.Value.ToString(), str, true) == 0 || string.Compare(standardValue.DisplayName, str, true) == 0)
					{
						return standardValue.Value;
					}
				}
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			StandardValue[] allPossibleValues;
			int i;
			if (value is string)
			{
				if (destinationType == typeof(string))
				{
					return value;
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
			else if (value.GetType() == typeof(bool))
			{
				if (destinationType == typeof(string))
				{
					allPossibleValues = this.GetAllPossibleValues(context);
					for (i = 0; i < (int)allPossibleValues.Length; i++)
					{
						StandardValue standardValue1 = allPossibleValues[i];
						if (standardValue1.Value.Equals(value))
						{
							this.UpdateStringFromResource(context, standardValue1);
							return standardValue1.DisplayName;
						}
					}
				}
				else if (destinationType == typeof(StandardValue))
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
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		private StandardValue[] GetAllPossibleValues(ITypeDescriptorContext context)
		{
			List<StandardValue> standardValues = new List<StandardValue>();
			if (context == null || context.PropertyDescriptor == null || !(context.PropertyDescriptor is AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor))
			{
				standardValues.Add(new StandardValue(true));
				standardValues.Add(new StandardValue(false));
			}
			else
			{
				standardValues.AddRange((context.PropertyDescriptor as AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor).StandardValues);
			}
			return standardValues.ToArray();
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

		private void UpdateStringFromResource(ITypeDescriptorContext context, StandardValue sv)
		{
			ResourceAttribute resourceAttribute = null;
			if (context != null && context.PropertyDescriptor != null)
			{
				resourceAttribute = (ResourceAttribute)context.PropertyDescriptor.Attributes.Get(typeof(ResourceAttribute));
			}
			if (resourceAttribute == null)
			{
				resourceAttribute = (ResourceAttribute)System.ComponentModel.TypeDescriptor.GetAttributes(typeof(bool)).Get(typeof(ResourceAttribute));
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
				else if (!string.IsNullOrEmpty(resourceAttribute.BaseName))
				{
					resourceManager = new ResourceManager(resourceAttribute.BaseName, typeof(bool).Assembly);
				}
				else if (!string.IsNullOrEmpty(resourceAttribute.BaseName))
				{
					resourceManager = new ResourceManager(resourceAttribute.BaseName, typeof(bool).Assembly);
				}
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception.Message);
				return;
			}
			if (resourceManager == null)
			{
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