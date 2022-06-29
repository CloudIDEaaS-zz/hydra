using System;
using System.Xml;

namespace AddinExpress.Installer.Prerequisites.Manifests
{
	internal class baseExitCode : Element
	{
		public LocString Message;

		public baseExitCode.Results Result;

		public baseExitCode(baseExitCode.Results result)
		{
			this.Result = result;
		}

		public static baseExitCode.Results DisplayResultsStringToEnum(string result)
		{
			baseExitCode.Results result1 = baseExitCode.Results.Success;
			if (result != null)
			{
				if (result == "Fail")
				{
					return baseExitCode.Results.Fail;
				}
				if (result == "Fail, Reboot Needed")
				{
					return baseExitCode.Results.FailReboot;
				}
				if (result == "Success")
				{
					return baseExitCode.Results.Success;
				}
				if (result == "Success, Reboot Needed")
				{
					result1 = baseExitCode.Results.SuccessReboot;
				}
			}
			return result1;
		}

		public override XmlElement LoadFromXML(XmlElement workingElement)
		{
			throw new NotImplementedException("NYI");
		}

		public static string ResultsEnumToDisplayString(baseExitCode.Results result)
		{
			switch (result)
			{
				case baseExitCode.Results.Success:
				{
					return "Success";
				}
				case baseExitCode.Results.Fail:
				{
					return "Fail";
				}
				case baseExitCode.Results.SuccessReboot:
				{
					return "Success, Reboot Needed";
				}
				case baseExitCode.Results.FailReboot:
				{
					return "Fail, Reboot Needed";
				}
			}
			return null;
		}

		public static string ResultsEnumToManifestString(baseExitCode.Results result)
		{
			switch (result)
			{
				case baseExitCode.Results.Success:
				{
					return "Success";
				}
				case baseExitCode.Results.Fail:
				{
					return "Fail";
				}
				case baseExitCode.Results.SuccessReboot:
				{
					return "SuccessReboot";
				}
				case baseExitCode.Results.FailReboot:
				{
					return "FailReboot";
				}
			}
			return null;
		}

		public override void WriteXML(XmlElement parentElement)
		{
			base.AddAttribute(parentElement, "Result", baseExitCode.ResultsEnumToManifestString(this.Result));
			if (this.Message != null)
			{
				base.AddAttribute(parentElement, "String", this.Message.Name);
			}
		}

		internal class DisplayResultStrings
		{
			public const string Fail = "Fail";

			public const string FailReboot = "Fail, Reboot Needed";

			public const string Success = "Success";

			public const string SuccessReboot = "Success, Reboot Needed";

			public DisplayResultStrings()
			{
			}
		}

		internal class ManifestResultStrings
		{
			public const string Fail = "Fail";

			public const string FailReboot = "FailReboot";

			public const string Success = "Success";

			public const string SuccessReboot = "SuccessReboot";

			public ManifestResultStrings()
			{
			}
		}

		public class messageRequiredException : Exception
		{
			public messageRequiredException() : base("A message is required when Result = Fail or FailReboot")
			{
			}
		}

		public enum Results
		{
			Success,
			Fail,
			SuccessReboot,
			FailReboot
		}

		private class XMLStrings
		{
			public const string ResultAttribute = "Result";

			public const string StringAttribute = "String";

			public XMLStrings()
			{
			}
		}
	}
}