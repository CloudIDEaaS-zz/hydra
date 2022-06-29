using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXCustomVariables : List<WiXCustomVariable>
	{
		private WiXProjectParser _project;

		internal WiXProjectParser Project
		{
			get
			{
				return this._project;
			}
		}

		internal WiXCustomVariables(WiXProjectParser project)
		{
			this._project = project;
		}

		internal WiXCustomVariable FindVariable(string fullName)
		{
			return base.Find((WiXCustomVariable e) => e.FullName == fullName);
		}

		internal void Parse(List<WiXEntity> list)
		{
			if (list != null && list.Count > 0)
			{
				foreach (WiXEntity wiXEntity in list)
				{
					base.Add(new WiXCustomVariable(this.Project, this, wiXEntity));
				}
			}
		}
	}
}