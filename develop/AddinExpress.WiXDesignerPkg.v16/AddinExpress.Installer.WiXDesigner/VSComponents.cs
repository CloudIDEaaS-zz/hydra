using System;
using System.Collections.Generic;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSComponents : List<VSComponent>
	{
		private VSBaseFolder _parent;

		public VSComponents(VSBaseFolder parent)
		{
			this._parent = parent;
		}
	}
}