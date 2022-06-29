using System;
using System.IO;

namespace AddinExpress.Installer.Prerequisites
{
	internal class ProjectExceptions
	{
		public ProjectExceptions()
		{
		}

		public class CloseAndExitProjectException : Exception
		{
			public CloseAndExitProjectException(string projectName, string errorMessage) : base(string.Concat(projectName, " failed to Exit with the following message: ", errorMessage))
			{
			}
		}

		public class NewProjectException : Exception
		{
			public NewProjectException(string errorMessage) : base(string.Concat("Failed to create a new project with the following message: ", errorMessage))
			{
			}
		}

		public class OpenProjectException : Exception
		{
			public OpenProjectException(FileInfo projectFile, string errorMessage) : base(string.Concat(projectFile.FullName, " failed to Open with the following message: ", errorMessage))
			{
			}
		}

		public class SaveProjectAsException : Exception
		{
			public SaveProjectAsException(string projectName, string errorMessage) : base(string.Concat(projectName, " failed to save with the following message: ", errorMessage))
			{
			}
		}

		public class SaveProjectException : Exception
		{
			public SaveProjectException(string projectName, string errorMessage) : base(string.Concat(projectName, " failed to save with the following message: ", errorMessage))
			{
			}
		}
	}
}