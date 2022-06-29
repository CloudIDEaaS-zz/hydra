using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSSearchBase : VSComponentBase
	{
		private WiXProjectParser _project;

		private WiXEntity _wixElement;

		private AddinExpress.Installer.WiXDesigner.WiXProperty _wixProperty;

		private VSSearches _collection;

		internal override bool CanRename
		{
			get
			{
				return true;
			}
		}

		[Browsable(true)]
		[DisplayName("(Name)")]
		[ReadOnly(false)]
		public override string Name
		{
			get
			{
				if (!string.IsNullOrEmpty(this.WiXProperty.VSName))
				{
					return this.WiXProperty.VSName;
				}
				string str = "Search #";
				if (!(this is VSSearchComponent))
				{
					str = (!(this is VSSearchFile) ? "Search for RegistryEntry #" : "Search for File #");
				}
				else
				{
					str = "Search for Component #";
				}
				int num = this._project.Searches.IndexOf(this) + 1;
				return string.Concat(str, num.ToString());
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					this._project.ProjectManager.AddWiXExtensionReference("VDWExtension", false);
					this.WiXProperty.VSName = value;
					this.DoPropertyChanged();
				}
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Specifies a named property that can be accessed at installation run time to modify the installation based on search results")]
		public string Property
		{
			get
			{
				if (this._wixProperty == null)
				{
					return string.Empty;
				}
				return this._wixProperty.Id;
			}
			set
			{
				if (this._wixProperty != null)
				{
					this._wixProperty.Id = value;
					this.DoPropertyChanged();
				}
			}
		}

		internal WiXEntity WiXElement
		{
			get
			{
				return this._wixElement;
			}
		}

		internal AddinExpress.Installer.WiXDesigner.WiXProperty WiXProperty
		{
			get
			{
				return this._wixProperty;
			}
		}

		public VSSearchBase()
		{
		}

		public VSSearchBase(WiXProjectParser project, WiXEntity wixElement, AddinExpress.Installer.WiXDesigner.WiXProperty wixProperty, VSSearches collection) : this()
		{
			this._project = project;
			this._wixElement = wixElement;
			this._wixProperty = wixProperty;
			this._collection = collection;
			this._collection.Add(this);
		}

		public override void Delete()
		{
			this.WiXElement.Delete();
			this.WiXProperty.Delete();
			this._collection.Remove(this);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this._project = null;
				this._wixElement = null;
				this._wixProperty = null;
				this._collection = null;
			}
			base.Dispose(disposing);
		}

		protected override string GetClassName()
		{
			return "Launch Condition Properties";
		}
	}
}