using System;
using System.ComponentModel;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSWebApplicationFolder : VSWebCustomFolder
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
		[DisplayName("(Name)")]
		[ReadOnly(true)]
		public override string Name
		{
			get
			{
				return "Web Application Folder";
			}
		}

		[Browsable(true)]
		[ReadOnly(true)]
		public override string Property
		{
			get
			{
				return "TARGETDIR";
			}
		}

		public VSWebApplicationFolder(WiXProjectParser project, VSBaseFolder parent, WiXDirectory wixElement) : base(project, parent, wixElement)
		{
		}
	}
}