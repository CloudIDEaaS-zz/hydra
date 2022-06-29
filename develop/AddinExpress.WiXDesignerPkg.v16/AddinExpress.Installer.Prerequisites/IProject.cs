using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace AddinExpress.Installer.Prerequisites
{
	internal interface IProject
	{
		string OpenProjectFilter
		{
			get;
		}

		FileInfo PathToProjectFile
		{
			get;
		}

		string ProjectFileRootXML
		{
			get;
		}

		string ProjectName
		{
			get;
		}

		string ProjectTypeDescription
		{
			get;
		}

		string ProjectTypeName
		{
			get;
		}

		ToolStrip ToolstripToMerge
		{
			get;
		}

		void Build(bool showBuildUI);

		bool CloseAndExit();

		void NewProject();

		void OpenProject(FileInfo openProjectFile);

		DialogResult SaveProject();

		void SaveProjectAs();

		event BuildMessageEventHandler BuildMessage;
	}
}