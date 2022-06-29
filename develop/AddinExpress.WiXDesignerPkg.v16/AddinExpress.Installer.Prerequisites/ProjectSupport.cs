using System;
using System.Xml;

namespace AddinExpress.Installer.Prerequisites
{
	internal class ProjectSupport
	{
		public ProjectSupport()
		{
		}

		public static IProject GetCorrectProjectForm(XmlDocument projectFile)
		{
			bool name = projectFile.ChildNodes[0].Name != PrerequisitesForm.Instance.ProjectFileRootXML;
			return new PrerequisitesForm();
		}
	}
}