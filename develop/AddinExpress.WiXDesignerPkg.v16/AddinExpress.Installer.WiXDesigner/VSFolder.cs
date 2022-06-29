using System;
using System.ComponentModel;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSFolder : VSBaseFolder
	{
		[Browsable(true)]
		[DefaultValue("")]
		[ReadOnly(false)]
		public override string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		[Browsable(false)]
		[DefaultValue("")]
		[ReadOnly(false)]
		public override string Property
		{
			get
			{
				return base.Property;
			}
			set
			{
				base.Property = value;
			}
		}

		public VSFolder(WiXProjectParser project, VSBaseFolder parent, WiXDirectory wixElement) : base(project, parent, wixElement)
		{
		}
	}
}