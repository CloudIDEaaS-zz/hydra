using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AddinExpress.Installer.WiXDesigner
{
	internal interface IBuildManager
	{
		bool IsListeningToBuildEvents
		{
			get;
			set;
		}

		ICollection<string> AllItemsInProject(IVsProject project);

		string GetProperty(IVsProject project, string name);

		void SetProperty(IVsProject project, string name, string value);

		event BuildProjConfigBegin_EventHandler BuildProjConfigBegin;

		event BuildProjConfigDone_EventHandler BuildProjConfigDone;

		event EmptyEvent BuildStarted;

		event EmptyEvent BuildStopped;
	}
}