using System;
using System.ComponentModel;

namespace AddinExpress.Installer.WiXDesigner.DesignTime
{
	[AttributeUsage(AttributeTargets.All)]
	internal class DisplayNameAttribute : System.ComponentModel.DisplayNameAttribute
	{
		public DisplayNameAttribute()
		{
		}

		public DisplayNameAttribute(string displayName) : base(displayName)
		{
		}
	}
}