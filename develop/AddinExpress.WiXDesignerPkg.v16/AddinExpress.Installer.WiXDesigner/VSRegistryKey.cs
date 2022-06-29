using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSRegistryKey : VSComponentBase
	{
		private WiXProjectParser _project;

		private WiXRegistryKey _wixElement;

		private VSComponent _parentComponent;

		private VSRegistryKeys _collection;

		private VSRegistryKeys _keys;

		private VSRegistryValues _values;

		[Browsable(true)]
		[DefaultValue(false)]
		[Description("Specifies whether to create the selected registry key as part of every installation, even if the registry key is empty")]
		public bool AlwaysCreate
		{
			get
			{
				if (this.WiXElement == null)
				{
					return false;
				}
				return this.WiXElement.ForceCreateOnInstall == "yes";
			}
			set
			{
				if (this.WiXElement != null)
				{
					this.WiXElement.ForceCreateOnInstall = (value ? "yes" : "no");
				}
			}
		}

		internal override bool CanRename
		{
			get
			{
				return true;
			}
		}

		internal VSRegistryKeys Collection
		{
			get
			{
				return this._collection;
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Specifies a Windows Installer condition that must be satisfied (evaluate to true) in order for the selected item to be installed at installation time")]
		public string Condition
		{
			get
			{
				if (this.ParentComponent == null || this.ParentComponent.WiXCondition == null)
				{
					return string.Empty;
				}
				return this.ParentComponent.WiXCondition.Condition;
			}
			set
			{
				if (this.ParentComponent != null)
				{
					if (string.IsNullOrEmpty(value))
					{
						this.ParentComponent.DeleteWiXCondition();
						return;
					}
					this.ParentComponent.CreateWiXCondition();
					this.ParentComponent.WiXCondition.Condition = value;
				}
			}
		}

		[Browsable(true)]
		[DefaultValue(false)]
		[Description("If False, only registry keys created during installation will be deleted.\nIf True, deletes the selected registry key and all subkeys when the product is uninstalled. Also deletes registry keys created manually.")]
		public bool DeleteAtUninstall
		{
			get
			{
				if (this.WiXElement == null)
				{
					return false;
				}
				return this.WiXElement.ForceDeleteOnUninstall == "yes";
			}
			set
			{
				if (this.WiXElement != null)
				{
					this.WiXElement.ForceDeleteOnUninstall = (value ? "yes" : "no");
				}
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Contains the full registry path for the selected registry key")]
		public string FullPath
		{
			get
			{
				if (this.WiXElement == null)
				{
					return string.Empty;
				}
				return string.Concat(this.WiXElement.Root, "\\", this.WiXElement.Key);
			}
		}

		[Browsable(false)]
		public VSRegistryKeys Keys
		{
			get
			{
				return this._keys;
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Specifies the name of the selected registry key or value")]
		[DisplayName("(Name)")]
		[ReadOnly(false)]
		public override string Name
		{
			get
			{
				if (this.WiXElement == null || string.IsNullOrEmpty(this.WiXElement.Key))
				{
					return string.Empty;
				}
				if (!this.WiXElement.Key.Contains("\\"))
				{
					return this.WiXElement.Key;
				}
				string key = this.WiXElement.Key;
				return key.Substring(key.LastIndexOf("\\") + 1);
			}
			set
			{
				if (!string.IsNullOrEmpty(value) && !value.Contains("\\") && this.WiXElement != null)
				{
					if (!this.WiXElement.Key.Contains("\\"))
					{
						this.WiXElement.Key = value;
					}
					else
					{
						string str = this.WiXElement.Key.Substring(0, this.WiXElement.Key.LastIndexOf("\\"));
						this.WiXElement.Key = string.Concat(str, "\\", value);
					}
				}
				this.DoPropertyChanged();
			}
		}

		internal VSComponent ParentComponent
		{
			get
			{
				return this._parentComponent;
			}
		}

		internal WiXProjectParser Project
		{
			get
			{
				return this._project;
			}
		}

		[Browsable(true)]
		[DefaultValue(false)]
		[Description("Determines whether the installer will reevaluate the Condition property for the selected item when reinstalling on a target computer")]
		public bool Transitive
		{
			get
			{
				if (this.ParentComponent == null)
				{
					return false;
				}
				return this.ParentComponent.WiXElement.Transitive == "yes";
			}
			set
			{
				if (this.ParentComponent != null)
				{
					this.ParentComponent.WiXElement.Transitive = (value ? "yes" : "no");
				}
			}
		}

		[Browsable(false)]
		public VSRegistryValues Values
		{
			get
			{
				return this._values;
			}
		}

		internal WiXRegistryKey WiXElement
		{
			get
			{
				return this._wixElement;
			}
		}

		public VSRegistryKey()
		{
		}

		public VSRegistryKey(WiXProjectParser project, WiXRegistryKey wixElement, VSComponent parentComponent, VSRegistryKeys collection) : this()
		{
			this._project = project;
			this._wixElement = wixElement;
			this._parentComponent = parentComponent;
			this._collection = collection;
			this._collection.Add(this);
			this._keys = new VSRegistryKeys(this.Project, this);
			this._values = new VSRegistryValues(this.Project, this);
		}

		public override void Delete()
		{
			if (this.Values.Count > 0)
			{
				for (int i = this.Values.Count - 1; i >= 0; i--)
				{
					this.Values[i].Delete();
				}
			}
			if (this.Keys.Count > 0)
			{
				for (int j = this.Keys.Count - 1; j >= 0; j--)
				{
					this.Keys[j].Delete();
				}
			}
			this.WiXElement.Delete();
			this._collection.Remove(this);
			if (this.ParentComponent != null && this.ParentComponent.CanBeDeleted)
			{
				this.ParentComponent.Delete();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this._project = null;
				this._wixElement = null;
				this._parentComponent = null;
				this._collection = null;
			}
			base.Dispose(disposing);
		}

		protected override string GetClassName()
		{
			return "Registry Settings Properties";
		}

		internal void MoveTo(VSRegistryKeys destinationCollection)
		{
			this.Collection.Remove(this);
			destinationCollection.Add(this);
			string name = this.Name;
			if (destinationCollection.ParentKey != null)
			{
				this.WiXElement.Root = destinationCollection.ParentKey.WiXElement.Root;
				this.WiXElement.Key = string.Concat(destinationCollection.ParentKey.WiXElement.Key, "\\", name);
			}
			else
			{
				if (destinationCollection == this.Project.Registry.HKCR)
				{
					this.WiXElement.Root = "HKCR";
				}
				else if (destinationCollection == this.Project.Registry.HKCU)
				{
					this.WiXElement.Root = "HKCU";
				}
				else if (destinationCollection == this.Project.Registry.HKLM)
				{
					this.WiXElement.Root = "HKLM";
				}
				else if (destinationCollection == this.Project.Registry.HKU)
				{
					this.WiXElement.Root = "HKU";
				}
				else if (destinationCollection == this.Project.Registry.HKMU)
				{
					this.WiXElement.Root = "HKMU";
				}
				this.WiXElement.Key = name;
			}
			if (this.Keys.Count > 0)
			{
				this.RebuildSubKeys();
			}
			if (this.Values.Count > 0)
			{
				this.RebuildValues();
			}
		}

		private void RebuildSubKeys()
		{
			foreach (VSRegistryKey key in this.Keys)
			{
				key.WiXElement.Root = this.WiXElement.Root;
				string name = key.Name;
				key.WiXElement.Key = string.Concat(this.WiXElement.Key, "\\", name);
				if (key.Keys.Count > 0)
				{
					key.RebuildSubKeys();
				}
				if (key.Values.Count <= 0)
				{
					continue;
				}
				key.RebuildValues();
			}
		}

		private void RebuildValues()
		{
			foreach (VSRegistryValueBase value in this.Values)
			{
				if (value.ParentComponent == null)
				{
					continue;
				}
				value.WiXElement.Root = this.WiXElement.Root;
				value.WiXElement.Key = this.WiXElement.Key;
			}
		}
	}
}