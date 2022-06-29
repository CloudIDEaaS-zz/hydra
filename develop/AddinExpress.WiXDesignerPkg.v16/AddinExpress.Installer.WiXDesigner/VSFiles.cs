using System;
using System.Collections.Generic;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSFiles : List<VSBaseFile>
	{
		private WiXProjectParser _project;

		private VSBaseFolder _parent;

		public VSFiles(WiXProjectParser project, VSBaseFolder parent)
		{
			this._project = project;
			this._parent = parent;
		}
	}
}