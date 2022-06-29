using System;
using System.Collections.Specialized;

namespace AddinExpress.Installer.Prerequisites.Manifests
{
	internal class BuiltInProperties
	{
		public BuiltInProperties()
		{
		}

		public static bool IsSystemCheck(string propertyName)
		{
			return BuiltInProperties.ListSystemChecks().Contains(propertyName);
		}

		public static StringCollection ListLongComparisons()
		{
			StringCollection stringCollections = new StringCollection();
			stringCollections.Add("ValueEqualTo");
			stringCollections.Add("ValueGreaterThan");
			stringCollections.Add("ValueGreaterThanOrEqualTo");
			stringCollections.Add("ValueLessThan");
			stringCollections.Add("ValueLessThanOrEqualTo");
			stringCollections.Add("ValueNotEqualTo");
			stringCollections.Add("ValueExists");
			stringCollections.Add("ValueNotExists");
			stringCollections.Add("VersionEqualTo");
			stringCollections.Add("VersionGreaterThan");
			stringCollections.Add("VersionGreaterThanOrEqualTo");
			stringCollections.Add("VersionLessThan");
			stringCollections.Add("VersionLessThanOrEqualTo");
			stringCollections.Add("VersionNotEqualTo");
			return stringCollections;
		}

		public static StringCollection ListRebootOptions()
		{
			StringCollection stringCollections = new StringCollection();
			stringCollections.Add("Defer");
			stringCollections.Add("Force");
			stringCollections.Add("Immediate");
			stringCollections.Add("None");
			return stringCollections;
		}

		public static StringCollection ListResults()
		{
			StringCollection stringCollections = new StringCollection();
			stringCollections.Add("Success");
			stringCollections.Add("Success, Reboot Needed");
			stringCollections.Add("Fail");
			stringCollections.Add("Fail, Reboot Needed");
			return stringCollections;
		}

		public static StringCollection ListShortComparisons(bool includeVersions)
		{
			StringCollection stringCollections = new StringCollection();
			stringCollections.Add("=");
			stringCollections.Add(">=");
			stringCollections.Add(">");
			stringCollections.Add("<");
			stringCollections.Add("<=");
			stringCollections.Add("<>");
			if (includeVersions)
			{
				stringCollections.Add("Version =");
				stringCollections.Add("Version >=");
				stringCollections.Add("Version >");
				stringCollections.Add("Version <");
				stringCollections.Add("Version <=");
				stringCollections.Add("Version <>");
			}
			stringCollections.Add("Exists");
			stringCollections.Add("Doesn't Exist");
			return stringCollections;
		}

		public static StringCollection ListSystemChecks()
		{
			StringCollection stringCollections = new StringCollection();
			stringCollections.Add("AdminUser");
			stringCollections.Add("ProcessorArchitecture");
			stringCollections.Add("Version9x");
			stringCollections.Add("VersionNT");
			stringCollections.Add("VersionMsi");
			stringCollections.Add("VersionNT64");
			stringCollections.Add("InstallMode");
			return stringCollections;
		}

		public static string RebootEnumToString(CommandsNode.RebootOptions value)
		{
			switch (value)
			{
				case CommandsNode.RebootOptions.Defer:
				{
					return "Defer";
				}
				case CommandsNode.RebootOptions.Immediate:
				{
					return "Immediate";
				}
				case CommandsNode.RebootOptions.Force:
				{
					return "Force";
				}
				case CommandsNode.RebootOptions.None:
				{
					return "None";
				}
			}
			return null;
		}

		public static CommandsNode.RebootOptions RebootStringToEnum(string value)
		{
			CommandsNode.RebootOptions rebootOption = CommandsNode.RebootOptions.None;
			if (value != null)
			{
				if (value == "Defer")
				{
					return CommandsNode.RebootOptions.Defer;
				}
				if (value == "Force")
				{
					return CommandsNode.RebootOptions.Force;
				}
				if (value == "Immediate")
				{
					return CommandsNode.RebootOptions.Immediate;
				}
				if (value == "None")
				{
					rebootOption = CommandsNode.RebootOptions.None;
				}
			}
			return rebootOption;
		}

		public static string SystemChecksEnumToString(BuiltInProperties.SystemChecks systemCheck)
		{
			switch (systemCheck)
			{
				case BuiltInProperties.SystemChecks.AdminUser:
				{
					return "AdminUser";
				}
				case BuiltInProperties.SystemChecks.Version9x:
				{
					return "Version9x";
				}
				case BuiltInProperties.SystemChecks.VersionNT:
				{
					return "VersionNT";
				}
				case BuiltInProperties.SystemChecks.VersionMsi:
				{
					return "VersionMsi";
				}
				case BuiltInProperties.SystemChecks.VersionNT64:
				{
					return "VersionNT64";
				}
				case BuiltInProperties.SystemChecks.ProcessorArchitecture:
				{
					return "ProcessorArchitecture";
				}
				case BuiltInProperties.SystemChecks.InstallMode:
				{
					return "InstallMode";
				}
			}
			return null;
		}

		public static BuiltInProperties.SystemChecks SystemChecksStringToEnum(string systemCheck)
		{
			if (systemCheck != null)
			{
				if (systemCheck == "AdminUser")
				{
					return BuiltInProperties.SystemChecks.AdminUser;
				}
				if (systemCheck == "Version9x")
				{
					return BuiltInProperties.SystemChecks.Version9x;
				}
				if (systemCheck == "VersionMsi")
				{
					return BuiltInProperties.SystemChecks.VersionMsi;
				}
				if (systemCheck == "VersionNT")
				{
					return BuiltInProperties.SystemChecks.VersionNT;
				}
				if (systemCheck == "VersionNT64")
				{
					return BuiltInProperties.SystemChecks.VersionNT64;
				}
				if (systemCheck == "ProcessorArchitecture")
				{
					return BuiltInProperties.SystemChecks.ProcessorArchitecture;
				}
				if (systemCheck == "InstallMode")
				{
					return BuiltInProperties.SystemChecks.InstallMode;
				}
			}
			throw new ArgumentException(string.Concat(systemCheck, " isn't a valid system check string. Make sure your capitalization is correct."));
		}

		public enum SystemChecks
		{
			AdminUser,
			Version9x,
			VersionNT,
			VersionMsi,
			VersionNT64,
			ProcessorArchitecture,
			InstallMode
		}

		internal class SystemChecksStrings
		{
			public const string AdminUser = "AdminUser";

			public const string ProcessorArch = "ProcessorArchitecture";

			public const string Version9x = "Version9x";

			public const string VersionMsi = "VersionMsi";

			public const string VersionNT = "VersionNT";

			public const string VersionNT64 = "VersionNT64";

			public const string InstallMode = "InstallMode";

			public SystemChecksStrings()
			{
			}
		}
	}
}