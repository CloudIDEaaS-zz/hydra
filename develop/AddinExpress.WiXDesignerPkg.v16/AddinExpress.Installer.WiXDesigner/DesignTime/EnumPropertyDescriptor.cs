using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AddinExpress.Installer.WiXDesigner.DesignTime
{
	internal class EnumPropertyDescriptor : AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor
	{
		public override IList<StandardValue> StandardValues
		{
			get
			{
				return this.m_StatandardValues.AsReadOnly();
			}
		}

		public EnumPropertyDescriptor(System.ComponentModel.PropertyDescriptor pd, object instance) : base(pd, instance)
		{
			this.m_StatandardValues.Clear();
			StandardValue[] standardValues = EnumUtil.GetStandardValues(this.PropertyType);
			this.m_StatandardValues.AddRange(standardValues);
		}

		public EnumPropertyDescriptor(Type componentType, string sName, Type propType, object value, object parent, params Attribute[] attributes) : base(componentType, sName, propType, value, parent, attributes)
		{
			this.m_StatandardValues.Clear();
			StandardValue[] standardValues = EnumUtil.GetStandardValues(this.PropertyType);
			this.m_StatandardValues.AddRange(standardValues);
		}
	}
}