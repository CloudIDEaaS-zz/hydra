using System;
using System.Collections;
using System.ComponentModel;

namespace AddinExpress.Installer.WiXDesigner.DesignTime
{
	internal class ExpandableIEnumerationConverter : TypeConverter
	{
		public ExpandableIEnumerationConverter()
		{
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			if (value == null)
			{
				return base.GetProperties(context, value, attributes);
			}
			PropertyDescriptorCollection propertyDescriptorCollections = new PropertyDescriptorCollection(null, false);
			int num = -1;
			IEnumerable enumerable = value as IEnumerable;
			if (enumerable != null)
			{
				IEnumerator enumerator = enumerable.GetEnumerator();
				enumerator.Reset();
				while (enumerator.MoveNext())
				{
					num++;
					string str = enumerator.Current.ToString();
					IComponent current = enumerator.Current as IComponent;
					if (current != null && current.Site != null && !string.IsNullOrEmpty(current.Site.Name))
					{
						str = current.Site.Name;
					}
					else if (value.GetType().IsArray)
					{
						str = string.Concat("[", num.ToString(), "]");
					}
					propertyDescriptorCollections.Add(new AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor(value.GetType(), str, enumerator.Current.GetType(), enumerator.Current, System.ComponentModel.TypeDescriptor.GetAttributes(enumerator.Current).ToArray(), Array.Empty<Attribute>()));
				}
			}
			return propertyDescriptorCollections;
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			if (context == null)
			{
				return base.GetPropertiesSupported(context);
			}
			return context.PropertyDescriptor.GetValue(context.Instance) is IEnumerable;
		}
	}
}