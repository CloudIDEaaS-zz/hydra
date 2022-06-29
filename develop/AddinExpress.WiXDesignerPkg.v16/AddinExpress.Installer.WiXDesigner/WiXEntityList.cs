using System;
using System.Collections.Generic;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXEntityList : List<WiXEntity>
	{
		private IWiXEntity _owner;

		internal WiXEntityList(IWiXEntity owner)
		{
			this._owner = owner;
		}
	}
}