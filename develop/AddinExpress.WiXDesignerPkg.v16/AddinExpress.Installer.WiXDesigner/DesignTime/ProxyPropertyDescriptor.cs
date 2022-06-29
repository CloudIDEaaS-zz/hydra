using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace AddinExpress.Installer.WiXDesigner.DesignTime
{
	internal class ProxyPropertyDescriptor : AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor
	{
		public object Parent
		{
			get
			{
				return this.parentComponent;
			}
		}

		public ProxyPropertyDescriptor(Type componentType, string sName, Type propType, object value, object parent, params Attribute[] attributes) : base(componentType, sName, propType, value, parent, attributes)
		{
		}

		protected override void FillAttributes(IList attributeList)
		{
			if (this.parentComponent != null)
			{
				PropertyDescriptorCollection properties = System.ComponentModel.TypeDescriptor.GetProperties(this.parentComponent);
				if (properties != null)
				{
					System.ComponentModel.PropertyDescriptor item = properties[this.Name];
					if (item != null)
					{
						AttributeCollection attributes = item.Attributes;
						if (attributes != null)
						{
							foreach (Attribute attribute in attributes)
							{
								attributeList.Add(attribute);
							}
						}
					}
				}
			}
		}

		public override object GetValue(object component)
		{
			if (this.parentComponent == null)
			{
				return this.m_value;
			}
			return this.parentComponent.GetType().InvokeMember(this.Name, BindingFlags.GetProperty, null, this.parentComponent, null);
		}

		public override void SetValue(object component, object value)
		{
			this.m_value = value;
			if (this.parentComponent != null)
			{
				this.parentComponent.GetType().InvokeMember(this.Name, BindingFlags.SetProperty, null, this.parentComponent, new object[] { value });
			}
			base.OnValueChanged(component, new EventArgs());
		}
	}
}