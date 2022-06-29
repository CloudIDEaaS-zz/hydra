using System;
using System.Runtime.CompilerServices;

namespace AddinExpress.Installer.WiXDesigner.DesignTime
{
	[AttributeUsage(AttributeTargets.Property)]
	internal class CategoryResourceKeyAttribute : Attribute
	{
		public string ResourceKey
		{
			get;
			set;
		}

		public CategoryResourceKeyAttribute()
		{
		}

		public CategoryResourceKeyAttribute(string resourceKey)
		{
			this.ResourceKey = resourceKey;
		}
	}
}