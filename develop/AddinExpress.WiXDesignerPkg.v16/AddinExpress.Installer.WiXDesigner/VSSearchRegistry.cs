using System;
using System.ComponentModel;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSSearchRegistry : VSSearchBase
	{
		[Browsable(true)]
		[DefaultValue("")]
		[Description("Specifies the name used in the Launch Condition Editor to identify a selected registry search")]
		[DisplayName("(Name)")]
		[ReadOnly(false)]
		public override string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Specifies a registry key to search for")]
		public string RegKey
		{
			get
			{
				if (base.WiXElement == null)
				{
					return string.Empty;
				}
				return (base.WiXElement as WiXRegistrySearch).Key;
			}
			set
			{
				if (base.WiXElement != null)
				{
					(base.WiXElement as WiXRegistrySearch).Key = value;
				}
			}
		}

		[Browsable(true)]
		[DefaultValue(RootRegistryEntries.vsdrrHKLM)]
		[Description("Specifies the registry root for a registry search")]
		public RootRegistryEntries Root
		{
			get
			{
				if (base.WiXElement == null)
				{
					return RootRegistryEntries.vsdrrHKLM;
				}
				if ((base.WiXElement as WiXRegistrySearch).Root.ToUpper() == "HKLM")
				{
					return RootRegistryEntries.vsdrrHKLM;
				}
				if ((base.WiXElement as WiXRegistrySearch).Root.ToUpper() == "HKCU")
				{
					return RootRegistryEntries.vsdrrHKCU;
				}
				if ((base.WiXElement as WiXRegistrySearch).Root.ToUpper() == "HKCR")
				{
					return RootRegistryEntries.vsdrrHKCR;
				}
				return RootRegistryEntries.vsdrrHKU;
			}
			set
			{
				if (base.WiXElement != null)
				{
					if (value == RootRegistryEntries.vsdrrHKLM)
					{
						(base.WiXElement as WiXRegistrySearch).Root = "HKLM";
						return;
					}
					if (value == RootRegistryEntries.vsdrrHKCU)
					{
						(base.WiXElement as WiXRegistrySearch).Root = "HKCU";
						return;
					}
					if (value == RootRegistryEntries.vsdrrHKCR)
					{
						(base.WiXElement as WiXRegistrySearch).Root = "HKCR";
						return;
					}
					(base.WiXElement as WiXRegistrySearch).Root = "HKU";
				}
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Specifies a registry value to search for")]
		public string Value
		{
			get
			{
				if (base.WiXElement == null)
				{
					return string.Empty;
				}
				return (base.WiXElement as WiXRegistrySearch).Name;
			}
			set
			{
				if (base.WiXElement != null)
				{
					(base.WiXElement as WiXRegistrySearch).Name = value;
				}
			}
		}

		public VSSearchRegistry()
		{
		}

		public VSSearchRegistry(WiXProjectParser project, WiXEntity wixElement, AddinExpress.Installer.WiXDesigner.WiXProperty wixProperty, VSSearches collection) : base(project, wixElement, wixProperty, collection)
		{
		}
	}
}