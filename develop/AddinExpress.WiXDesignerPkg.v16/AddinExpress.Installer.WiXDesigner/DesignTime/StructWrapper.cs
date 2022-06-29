using System;
using System.ComponentModel;

namespace AddinExpress.Installer.WiXDesigner.DesignTime
{
	internal class StructWrapper : ICustomTypeDescriptor
	{
		private object m_Struct;

		[Browsable(false)]
		public object Struct
		{
			get
			{
				return this.m_Struct;
			}
			set
			{
				this.m_Struct = value;
			}
		}

		public StructWrapper()
		{
		}

		public StructWrapper(object structObject)
		{
			this.m_Struct = structObject;
		}

		AttributeCollection System.ComponentModel.ICustomTypeDescriptor.GetAttributes()
		{
			return System.ComponentModel.TypeDescriptor.GetAttributes(this.m_Struct);
		}

		string System.ComponentModel.ICustomTypeDescriptor.GetClassName()
		{
			return System.ComponentModel.TypeDescriptor.GetClassName(this.m_Struct);
		}

		string System.ComponentModel.ICustomTypeDescriptor.GetComponentName()
		{
			return System.ComponentModel.TypeDescriptor.GetComponentName(this.m_Struct);
		}

		TypeConverter System.ComponentModel.ICustomTypeDescriptor.GetConverter()
		{
			return System.ComponentModel.TypeDescriptor.GetConverter(this.m_Struct);
		}

		EventDescriptor System.ComponentModel.ICustomTypeDescriptor.GetDefaultEvent()
		{
			return System.ComponentModel.TypeDescriptor.GetDefaultEvent(this.m_Struct);
		}

		System.ComponentModel.PropertyDescriptor System.ComponentModel.ICustomTypeDescriptor.GetDefaultProperty()
		{
			return System.ComponentModel.TypeDescriptor.GetDefaultProperty(this.m_Struct);
		}

		object System.ComponentModel.ICustomTypeDescriptor.GetEditor(Type editorBaseType)
		{
			return System.ComponentModel.TypeDescriptor.GetEditor(this.m_Struct, editorBaseType);
		}

		EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
		{
			return System.ComponentModel.TypeDescriptor.GetEvents(this.m_Struct, attributes);
		}

		EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents()
		{
			return System.ComponentModel.TypeDescriptor.GetEvents(this.m_Struct);
		}

		PropertyDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
		{
			return System.ComponentModel.TypeDescriptor.GetProperties(this.m_Struct, attributes);
		}

		PropertyDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetProperties()
		{
			return System.ComponentModel.TypeDescriptor.GetProperties(this.m_Struct);
		}

		object System.ComponentModel.ICustomTypeDescriptor.GetPropertyOwner(System.ComponentModel.PropertyDescriptor pd)
		{
			return this.m_Struct;
		}
	}
}