using System;
using System.ComponentModel;
using System.Reflection;

namespace AddinExpress.Installer.WiXDesigner.DesignTime
{
	internal class EnumChildPropertyDescriptor : BooleanPropertyDescriptor
	{
		private ITypeDescriptorContext m_context;

		private object m_enumField;

		public EnumChildPropertyDescriptor(ITypeDescriptorContext context, string sName, object enumFieldvalue, params Attribute[] attributes) : base(enumFieldvalue.GetType(), sName, false, attributes)
		{
			this.m_context = context;
			this.m_enumField = enumFieldvalue;
		}

		public override object GetValue(object component)
		{
			return EnumUtil.IsBitsOn(component, this.m_enumField);
		}

		public override void SetValue(object component, object value)
		{
			object obj = this.m_context.PropertyDescriptor.GetValue(this.m_context.Instance);
			bool flag = false;
			flag = (!(bool)value ? EnumUtil.TurnOffBits(ref obj, this.m_enumField) : EnumUtil.TurnOnBits(ref obj, this.m_enumField));
			if (flag)
			{
				component.GetType().GetField("value__", BindingFlags.Instance | BindingFlags.Public).SetValue(component, obj);
				this.m_context.PropertyDescriptor.SetValue(this.m_context.Instance, component);
			}
		}
	}
}