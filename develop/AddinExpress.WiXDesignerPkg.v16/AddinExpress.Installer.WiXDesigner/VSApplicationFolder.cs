using System;
using System.ComponentModel;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSApplicationFolder : VSBaseFolder
	{
		internal override bool CanDelete
		{
			get
			{
				return false;
			}
		}

		internal override bool CanRename
		{
			get
			{
				return false;
			}
		}

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
		public override string Name
		{
			get
			{
				return "Application Folder";
			}
		}

		[Browsable(true)]
		public override string Property
		{
			get
			{
				return "TARGETDIR";
			}
		}

		public VSApplicationFolder(WiXProjectParser project, VSBaseFolder parent, WiXDirectory wixElement) : base(project, parent, wixElement)
		{
		}
	}
}