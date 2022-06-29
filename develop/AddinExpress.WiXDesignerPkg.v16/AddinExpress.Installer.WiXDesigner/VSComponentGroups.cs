using System;
using System.Collections.Generic;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSComponentGroups : List<VSComponentGroup>
	{
		private WiXProjectParser _project;

		private VSBaseFolder _parent;

		public VSComponentGroups(WiXProjectParser project, VSBaseFolder parent)
		{
			this._project = project;
			this._parent = parent;
		}
	}
}