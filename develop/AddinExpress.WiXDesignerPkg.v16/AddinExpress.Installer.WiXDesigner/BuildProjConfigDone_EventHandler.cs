using System;

namespace AddinExpress.Installer.WiXDesigner
{
	internal delegate void BuildProjConfigDone_EventHandler(string Project, string ProjectConfig, string Platform, string SolutionConfig, bool Success);
}