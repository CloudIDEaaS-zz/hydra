using System;
using System.Runtime.CompilerServices;

namespace AddinExpress.Installer.WiXDesigner.DesignTime
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=false)]
	internal class SortIDAttribute : Attribute
	{
		public int CategoryOrder
		{
			get;
			set;
		}

		public int PropertyOrder
		{
			get;
			set;
		}

		public SortIDAttribute()
		{
			this.PropertyOrder = 0;
			this.CategoryOrder = 0;
		}

		public SortIDAttribute(int propertyId, int categoryId)
		{
			this.PropertyOrder = propertyId;
			this.CategoryOrder = categoryId;
		}
	}
}