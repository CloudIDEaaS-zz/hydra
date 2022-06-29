using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace AddinExpress.Installer.Prerequisites.Manifests
{
	internal class Command : Element
	{
		public string Args;

		public int EstimatedDiskSpace;

		public int EstimatedInstallTime;

		public int EstimatedTempSpace;

		public List<baseExitCode> ExitCodes;

		public List<InstallCondition> InstallConditions;

		public PackageFile PackageFileToRun;

		public Command(PackageFile packageFileToRun) : this(packageFileToRun, null, 0, 0, 0)
		{
		}

		public Command(PackageFile packageFileToRun, string args, int estimatedDiskSpace, int estimatedInstallTime, int estimatedTempSpace)
		{
			this.EstimatedDiskSpace = 0;
			this.EstimatedInstallTime = 0;
			this.EstimatedTempSpace = 0;
			this.ExitCodes = new List<baseExitCode>();
			this.InstallConditions = new List<InstallCondition>();
			this.PackageFileToRun = packageFileToRun;
			this.Args = args;
			this.EstimatedDiskSpace = estimatedDiskSpace;
			this.EstimatedInstallTime = estimatedInstallTime;
			this.EstimatedTempSpace = estimatedTempSpace;
		}

		public override XmlElement LoadFromXML(XmlElement workingElement)
		{
			throw new NotImplementedException("NYI");
		}

		public override void WriteXML(XmlElement parentElement)
		{
			Element.BuildMessage(string.Concat("   Command: ", this.PackageFileToRun.SourcePathandFileName), BootstrapperProduct.BuildErrorLevel.None);
			XmlElement xmlElement = base.AddElement(parentElement, "Command");
			base.AddAttribute(xmlElement, "PackageFile", Path.GetFileName(this.PackageFileToRun.SourcePathandFileName).ToLower());
			if (this.Args != null && this.Args.Length > 0)
			{
				base.AddAttribute(xmlElement, "Arguments", this.Args);
			}
			if (this.EstimatedDiskSpace > 0)
			{
				base.AddAttribute(xmlElement, "EstimatedInstalledBytes", this.EstimatedDiskSpace.ToString());
			}
			if (this.EstimatedInstallTime > 0)
			{
				base.AddAttribute(xmlElement, "EstimatedInstallSeconds", this.EstimatedInstallTime.ToString());
			}
			if (this.EstimatedTempSpace > 0)
			{
				base.AddAttribute(xmlElement, "EstimatedTempBytes", this.EstimatedTempSpace.ToString());
			}
			if (this.InstallConditions != null && this.InstallConditions.Count > 0)
			{
				XmlElement xmlElement1 = base.AddElement(xmlElement, "InstallConditions");
				foreach (InstallCondition installCondition in this.InstallConditions)
				{
					installCondition.WriteXML(xmlElement1);
				}
			}
			if (this.ExitCodes != null && this.ExitCodes.Count > 0)
			{
				XmlElement xmlElement2 = base.AddElement(xmlElement, "ExitCodes");
				bool flag = false;
				foreach (baseExitCode exitCode in this.ExitCodes)
				{
					exitCode.WriteXML(xmlElement2);
					if (!(exitCode is DefaultExitCode))
					{
						continue;
					}
					if (!flag)
					{
						flag = true;
					}
					else
					{
						Element.BuildMessage("", BootstrapperProduct.BuildErrorLevel.BuildError);
						return;
					}
				}
			}
		}

		private class XMLStrings
		{
			public const string ArgsAttribute = "Arguments";

			public const string ElementName = "Command";

			public const string EstimatedDiskSpaceAttribute = "EstimatedInstalledBytes";

			public const string EstimatedInstallTempSpaceAttribute = "EstimatedTempBytes";

			public const string EstimatedInstallTimeAttribute = "EstimatedInstallSeconds";

			public const string ExitCodesNode = "ExitCodes";

			public const string InstallConditionsNode = "InstallConditions";

			public const string PackageFileAttribute = "PackageFile";

			public XMLStrings()
			{
			}
		}
	}
}