using System;
using System.Collections.Generic;
using System.Xml;

namespace AddinExpress.Installer.Prerequisites.Manifests
{
	internal class Schedule : Element
	{
		public string Name;

		public List<Schedule.BootstrapPhases> RunSchedule;

		public Schedule()
		{
			this.RunSchedule = new List<Schedule.BootstrapPhases>();
		}

		public Schedule(string scheduleName, List<Schedule.BootstrapPhases> whenToRun)
		{
			this.RunSchedule = new List<Schedule.BootstrapPhases>();
			this.Name = scheduleName;
			this.RunSchedule = whenToRun;
		}

		public static string BootstrapPhasesEnumToString(Schedule.BootstrapPhases phase)
		{
			switch (phase)
			{
				case Schedule.BootstrapPhases.BuildList:
				{
					return "BuildList";
				}
				case Schedule.BootstrapPhases.BeforePackage:
				{
					return "BeforePackage";
				}
				case Schedule.BootstrapPhases.AfterPackage:
				{
					return "AfterPackage";
				}
			}
			return null;
		}

		public override XmlElement LoadFromXML(XmlElement workingElement)
		{
			throw new NotImplementedException("NYI");
		}

		public override void WriteXML(XmlElement parentElement)
		{
			if (this.RunSchedule.Count > 0)
			{
				XmlElement xmlElement = base.AddElement(parentElement, "Schedule");
				base.AddAttribute(xmlElement, "Name", this.Name);
				foreach (Schedule.BootstrapPhases runSchedule in this.RunSchedule)
				{
					base.AddElement(xmlElement, Schedule.BootstrapPhasesEnumToString(runSchedule));
				}
			}
		}

		public enum BootstrapPhases
		{
			BuildList,
			BeforePackage,
			AfterPackage
		}

		private class XMLStrings
		{
			public const string ElementName = "Schedule";

			public const string NameAttribute = "Name";

			public XMLStrings()
			{
			}
		}
	}
}