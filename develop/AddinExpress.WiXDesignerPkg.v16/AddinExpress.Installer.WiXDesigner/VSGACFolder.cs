using System;
using System.ComponentModel;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSGACFolder : VSSpecialFolder
	{
		[Browsable(false)]
		public override bool AlwaysCreate
		{
			get
			{
				return base.AlwaysCreate;
			}
		}

		internal override bool CanHaveSubFolders
		{
			get
			{
				return false;
			}
		}

		[Browsable(false)]
		public override string Condition
		{
			get
			{
				return base.Condition;
			}
		}

		[Browsable(true)]
		public override string Name
		{
			get
			{
				return base.Name;
			}
		}

		[Browsable(false)]
		public override string Property
		{
			get
			{
				return base.Property;
			}
		}

		[Browsable(false)]
		public override bool Transitive
		{
			get
			{
				return base.Transitive;
			}
		}

		public VSGACFolder(WiXProjectParser project, VSBaseFolder parent, WiXDirectory wixElement) : base(project, parent, wixElement)
		{
		}
	}
}