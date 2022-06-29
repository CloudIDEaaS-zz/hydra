using System;
using System.Runtime.CompilerServices;

namespace AddinExpress.Installer.WiXDesigner.DesignTime
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=false)]
	internal class ExclusiveStandardValuesAttribute : Attribute
	{
		public bool Exclusive
		{
			get;
			set;
		}

		public ExclusiveStandardValuesAttribute(bool exclusive)
		{
			this.Exclusive = exclusive;
		}
	}
}