using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AddinExpress.Installer.WiXDesigner.DesignTime
{
	internal class BooleanPropertyDescriptor : AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor
	{
		public override IList<StandardValue> StandardValues
		{
			get
			{
				return this.m_StatandardValues.AsReadOnly();
			}
		}

		public BooleanPropertyDescriptor(System.ComponentModel.PropertyDescriptor pd, object instance) : base(pd, instance)
		{
			this.m_StatandardValues.Clear();
			this.m_StatandardValues.Add(new StandardValue(true));
			this.m_StatandardValues.Add(new StandardValue(false));
		}

		public BooleanPropertyDescriptor(Type componentType, string sName, bool value, params Attribute[] attributes) : base(componentType, sName, typeof(bool), value, attributes, Array.Empty<Attribute>())
		{
			this.m_StatandardValues.Clear();
			this.m_StatandardValues.Add(new StandardValue(true));
			this.m_StatandardValues.Add(new StandardValue(false));
		}
	}
}