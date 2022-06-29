using System;
using System.Xml;

namespace AddinExpress.Installer.Prerequisites.Manifests
{
	internal abstract class InstallCondition : Element
	{
		public InstallCondition.Comparisons Compare;

		public ValueProperty Property;

		public AddinExpress.Installer.Prerequisites.Manifests.Schedule Schedule;

		public string Value;

		public InstallCondition() : this(null, InstallCondition.Comparisons.ValueEqualTo, null, null)
		{
		}

		public InstallCondition(BuiltInProperties.SystemChecks propertyToCheck, InstallCondition.Comparisons compare, string value)
		{
			this.Property = new ValueProperty(propertyToCheck);
			this.Compare = compare;
			if (value != null && value.Length > 0)
			{
				this.Value = value;
			}
		}

		public InstallCondition(ValueProperty propertyToCheck, InstallCondition.Comparisons compare, string value) : this(propertyToCheck, compare, value, null)
		{
		}

		public InstallCondition(ValueProperty propertyToCheck, InstallCondition.Comparisons compare, string value, AddinExpress.Installer.Prerequisites.Manifests.Schedule schedule)
		{
			if (propertyToCheck != null)
			{
				this.Property = propertyToCheck;
			}
			this.Compare = compare;
			if (value != null && value.Length > 0)
			{
				this.Value = value;
			}
			if (schedule != null)
			{
				this.Schedule = schedule;
			}
		}

		public static string ComparisonsEnumToString(InstallCondition.Comparisons comparison)
		{
			switch (comparison)
			{
				case InstallCondition.Comparisons.ValueEqualTo:
				{
					return "ValueEqualTo";
				}
				case InstallCondition.Comparisons.ValueGreaterThan:
				{
					return "ValueGreaterThan";
				}
				case InstallCondition.Comparisons.ValueGreaterThanOrEqualTo:
				{
					return "ValueGreaterThanOrEqualTo";
				}
				case InstallCondition.Comparisons.ValueLessThan:
				{
					return "ValueLessThan";
				}
				case InstallCondition.Comparisons.ValueLessThanOrEqualTo:
				{
					return "ValueLessThanOrEqualTo";
				}
				case InstallCondition.Comparisons.ValueNotEqualTo:
				{
					return "ValueNotEqualTo";
				}
				case InstallCondition.Comparisons.ValueExists:
				{
					return "ValueExists";
				}
				case InstallCondition.Comparisons.ValueNotExists:
				{
					return "ValueNotExists";
				}
				case InstallCondition.Comparisons.VersionEqualTo:
				{
					return "VersionEqualTo";
				}
				case InstallCondition.Comparisons.VersionGreaterThan:
				{
					return "VersionGreaterThan";
				}
				case InstallCondition.Comparisons.VersionGreaterThanOrEqualTo:
				{
					return "VersionGreaterThanOrEqualTo";
				}
				case InstallCondition.Comparisons.VersionLessThan:
				{
					return "VersionLessThan";
				}
				case InstallCondition.Comparisons.VersionLessThanOrEqualTo:
				{
					return "VersionLessThanOrEqualTo";
				}
				case InstallCondition.Comparisons.VersionNotEqualTo:
				{
					return "VersionNotEqualTo";
				}
			}
			return null;
		}

		public static InstallCondition.Comparisons ComparisonsStringToEnum(string comparison)
		{
			InstallCondition.Comparisons comparison1 = InstallCondition.Comparisons.ValueExists;
			if (comparison != null)
			{
				if (comparison == "ValueEqualTo")
				{
					return InstallCondition.Comparisons.ValueEqualTo;
				}
				if (comparison == "ValueExists")
				{
					return InstallCondition.Comparisons.ValueExists;
				}
				if (comparison == "ValueNotExists")
				{
					return InstallCondition.Comparisons.ValueNotExists;
				}
				if (comparison == "ValueGreaterThan")
				{
					return InstallCondition.Comparisons.ValueGreaterThan;
				}
				if (comparison == "ValueGreaterThanOrEqualTo")
				{
					return InstallCondition.Comparisons.ValueGreaterThanOrEqualTo;
				}
				if (comparison == "ValueLessThan")
				{
					return InstallCondition.Comparisons.ValueLessThan;
				}
				if (comparison == "ValueLessThanOrEqualTo")
				{
					return InstallCondition.Comparisons.ValueLessThanOrEqualTo;
				}
				if (comparison == "ValueNotEqualTo")
				{
					return InstallCondition.Comparisons.ValueNotEqualTo;
				}
				if (comparison == "VersionEqualTo")
				{
					return InstallCondition.Comparisons.VersionEqualTo;
				}
				if (comparison == "VersionGreaterThan")
				{
					return InstallCondition.Comparisons.VersionGreaterThan;
				}
				if (comparison == "VersionGreaterThanOrEqualTo")
				{
					return InstallCondition.Comparisons.VersionGreaterThanOrEqualTo;
				}
				if (comparison == "VersionLessThan")
				{
					return InstallCondition.Comparisons.VersionLessThan;
				}
				if (comparison == "VersionLessThanOrEqualTo")
				{
					return InstallCondition.Comparisons.VersionLessThanOrEqualTo;
				}
				if (comparison == "VersionNotEqualTo")
				{
					comparison1 = InstallCondition.Comparisons.VersionNotEqualTo;
				}
			}
			return comparison1;
		}

		public static InstallCondition.Comparisons ShortComparisonsStringToEnum(string shortComparison, bool isVersionField)
		{
			InstallCondition.Comparisons comparison = InstallCondition.Comparisons.ValueExists;
			if (isVersionField)
			{
				if (shortComparison != null)
				{
					if (shortComparison == "=")
					{
						return InstallCondition.Comparisons.VersionEqualTo;
					}
					if (shortComparison == ">")
					{
						return InstallCondition.Comparisons.VersionGreaterThan;
					}
					if (shortComparison == ">=")
					{
						return InstallCondition.Comparisons.VersionGreaterThanOrEqualTo;
					}
					if (shortComparison == "<")
					{
						return InstallCondition.Comparisons.VersionLessThan;
					}
					if (shortComparison == "<=")
					{
						return InstallCondition.Comparisons.VersionLessThanOrEqualTo;
					}
					if (shortComparison == "<>")
					{
						comparison = InstallCondition.Comparisons.VersionNotEqualTo;
					}
				}
				return comparison;
			}
			if (shortComparison != null)
			{
				if (shortComparison == "Exists")
				{
					return InstallCondition.Comparisons.ValueExists;
				}
				if (shortComparison == "Doesn't Exist")
				{
					return InstallCondition.Comparisons.ValueNotExists;
				}
				if (shortComparison == "=")
				{
					return InstallCondition.Comparisons.ValueEqualTo;
				}
				if (shortComparison == ">")
				{
					return InstallCondition.Comparisons.ValueGreaterThan;
				}
				if (shortComparison == ">=")
				{
					return InstallCondition.Comparisons.ValueGreaterThanOrEqualTo;
				}
				if (shortComparison == "<")
				{
					return InstallCondition.Comparisons.ValueLessThan;
				}
				if (shortComparison == "<=")
				{
					return InstallCondition.Comparisons.ValueLessThanOrEqualTo;
				}
				if (shortComparison == "<>")
				{
					return InstallCondition.Comparisons.ValueNotEqualTo;
				}
				if (shortComparison == "Version =")
				{
					return InstallCondition.Comparisons.VersionEqualTo;
				}
				if (shortComparison == "Version >")
				{
					return InstallCondition.Comparisons.VersionGreaterThan;
				}
				if (shortComparison == "Version >=")
				{
					return InstallCondition.Comparisons.VersionGreaterThanOrEqualTo;
				}
				if (shortComparison == "Version <")
				{
					return InstallCondition.Comparisons.VersionLessThan;
				}
				if (shortComparison == "Version <=")
				{
					return InstallCondition.Comparisons.VersionLessThanOrEqualTo;
				}
				if (shortComparison == "Version <>")
				{
					comparison = InstallCondition.Comparisons.VersionNotEqualTo;
				}
			}
			return comparison;
		}

		protected void WriteBaseAttributes(XmlElement parentElement)
		{
			if (this.Property != null)
			{
				base.AddAttribute(parentElement, "Property", this.Property.Name);
				base.AddAttribute(parentElement, "Compare", InstallCondition.ComparisonsEnumToString(this.Compare));
				base.AddAttribute(parentElement, "Value", this.Value);
			}
			if (this.Schedule != null)
			{
				base.AddAttribute(parentElement, "Schedule", this.Schedule.Name);
			}
		}

		public enum Comparisons
		{
			ValueEqualTo,
			ValueGreaterThan,
			ValueGreaterThanOrEqualTo,
			ValueLessThan,
			ValueLessThanOrEqualTo,
			ValueNotEqualTo,
			ValueExists,
			ValueNotExists,
			VersionEqualTo,
			VersionGreaterThan,
			VersionGreaterThanOrEqualTo,
			VersionLessThan,
			VersionLessThanOrEqualTo,
			VersionNotEqualTo
		}

		private class XMLStrings
		{
			public const string CompareAttribute = "Compare";

			public const string PropertyAttribute = "Property";

			public const string ScheduleAttribute = "Schedule";

			public const string ValueAttribute = "Value";

			public XMLStrings()
			{
			}
		}
	}
}