using System;
using System.Runtime.CompilerServices;

namespace AddinExpress.Installer.WiXDesigner.DesignTime
{
	[AttributeUsage(AttributeTargets.Enum | AttributeTargets.Property, AllowMultiple=false, Inherited=true)]
	internal class ExpandEnumAttribute : Attribute
	{
		public bool Exapand
		{
			get;
			set;
		}

		public ExpandEnumAttribute(bool expand)
		{
			this.Exapand = expand;
		}
	}
}