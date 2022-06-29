using System;

namespace AddinExpress.Installer.WiXDesigner
{
	internal static class Constants
	{
		public const string VsRegistryRoot = "Software\\Microsoft\\VisualStudio";

		public const string ProductName = "Designer for Visual Studio WiX Setup Projects";

		public const string ProductShortName = "Designer for WiX Setup Projects";

		public const string ProductResourcesName = "Designer for WiX Setup Projects Resources Library";

		public const string ProductTrialRegKey = "SOFTWARE\\Add-in Express\\ADX Designer for WiX 2 Trial\\";

		public const string ProductRegKey = "SOFTWARE\\Add-in Express\\Designer for Visual Studio WiX Setup Projects\\";

		public const string ProductInstRegKey = "SOFTWARE\\Add-in Express\\InstalledProducts\\Designer for Visual Studio WiX Setup Projects\\";

		public const string ProductSettingsRegKey = "SOFTWARE\\Add-in Express\\User Settings\\Designer for Visual Studio WiX Setup Projects\\";

		public const string VDProjProductInstRegKey = "SOFTWARE\\Add-in Express\\InstalledProducts\\VDProj to WiX Converter\\";

		public const string WiXDesignerSchema = "http://schemas.add-in-express.com/wixdesigner";

		public const string HiveRedirect = "";

		public const string Yes = "yes";

		public const string No = "no";

		public const int MajorMinimumVersion = 3;

		public const int MinorMinimumVersion = 6;

		public const int WiXMajorMinimumVersion = 3;

		public const int WiXMinorMinimumVersion = 6;

		public const int DefaultXMLEditorIndentSize = 2;

		public const string CSharpProjectKind = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";

		public const string VBProjectKind = "{F184B08F-C81C-45F6-A57F-5ABD9991F28F}";

		public const string VCProjectKind = "{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}";

		public const string JSharpProjectKind = "{E6FDF86B-F3D1-11D4-8576-0002A516ECE8}";

		public const string ChromeProjectKind = "{656346D9-4656-40DA-A068-22D5425D4639}";

		public const string DeploymentProjectKind = "{54435603-DBB4-11D2-8724-00A0C9A8B90C}";

		public const string WiXProjectKind = "{930C7802-8A8C-48F9-8165-68863BCCD9DD}";

		public const string SolutionFolderKind = "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}";

		public const string ProjectFolderKind = "{6BB5F8EF-4483-11D3-8BCF-00C04F8EC28C}";

		public const string MiscellaneousProjectKind = "{66A2671D-8FB5-11D2-AA7E-00C04F688DDE}";

		public const string SolutionUserOptionsKey = "DesignerVSWiXUserOptions";

		public const string SolutionPersistanceKey = "DesignerVSWiXSolutionOptions";

		public const string XSLTFolderName = "XSLT";

		public const string UIResourcesFolderName = "Resources";

		public const string XslProjectOutputFileName = "XslProjectOutput.xslt";

		public const string XslCOMFileName = "XslFile.xslt";

		public const string XslRegisterForCOMFileName = "XslRegisterForCOM.xslt";

		public const string XslRegisterForCOMListFileName = "RegisterForCOM.xml";

		public const string TxtReadMeFileName = "readme.txt";

		public const int NOTIFY_ALL_DESIGNERS_ID = 0;

		public const int NO_NOTIFICATION_ID = -1;
	}
}