using System;
using System.ComponentModel;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSSearchComponent : VSSearchBase
	{
		[Browsable(true)]
		[DefaultValue("")]
		[Description("Specifies the GUID of the component to search for")]
		public string ComponentId
		{
			get
			{
				if (base.WiXElement == null || !(base.WiXElement is WiXComponentSearch))
				{
					return string.Empty;
				}
				return (base.WiXElement as WiXComponentSearch).Guid;
			}
			set
			{
				if (base.WiXElement != null && base.WiXElement is WiXComponentSearch)
				{
					(base.WiXElement as WiXComponentSearch).Guid = value;
				}
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Specifies the name used in the Launch Condition Editor to identify a selected component search")]
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

		public VSSearchComponent()
		{
		}

		public VSSearchComponent(WiXProjectParser project, WiXEntity wixElement, AddinExpress.Installer.WiXDesigner.WiXProperty wixProperty, VSSearches collection) : base(project, wixElement, wixProperty, collection)
		{
		}
	}
}