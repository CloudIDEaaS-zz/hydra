using System;
using System.ComponentModel;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSCustomFolder : VSBaseFolder
	{
		[Browsable(true)]
		public override string DefaultLocation
		{
			get
			{
				return base.DefaultLocation;
			}
			set
			{
				base.DefaultLocation = value;
			}
		}

		[Browsable(true)]
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

		[Browsable(true)]
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

		public VSCustomFolder(WiXProjectParser project, VSBaseFolder parent, WiXDirectory wixElement) : base(project, parent, wixElement)
		{
		}
	}
}