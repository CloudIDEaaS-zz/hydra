using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal abstract class VSRegistryValueBase : VSComponentBase
	{
		private WiXProjectParser _project;

		private WiXRegistryValue _wixElement;

		private VSComponent _parentComponent;

		private VSRegistryValues _collection;

		internal override bool CanRename
		{
			get
			{
				return true;
			}
		}

		internal VSRegistryValues Collection
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
				if (this.ParentComponent != null)
				{
					if (this.ParentComponent.WiXCondition != null)
					{
						return this.ParentComponent.WiXCondition.Condition;
					}
				}
				else if (this.Collection.ParentKey != null && this.Collection.ParentKey.ParentComponent != null && this.Collection.ParentKey.ParentComponent.WiXCondition != null)
				{
					return this.Collection.ParentKey.ParentComponent.WiXCondition.Condition;
				}
				return string.Empty;
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
					return;
				}
				if (this.Collection.ParentKey != null && this.Collection.ParentKey.ParentComponent != null)
				{
					if (string.IsNullOrEmpty(value))
					{
						this.Collection.ParentKey.ParentComponent.DeleteWiXCondition();
						return;
					}
					this.Collection.ParentKey.ParentComponent.CreateWiXCondition();
					this.Collection.ParentKey.ParentComponent.WiXCondition.Condition = value;
				}
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
				if (this.WiXElement == null)
				{
					return string.Empty;
				}
				if (string.IsNullOrEmpty(this.WiXElement.Name))
				{
					return "(Default)";
				}
				return this.WiXElement.Name;
			}
			set
			{
				if (this.WiXElement != null)
				{
					this.WiXElement.Name = value;
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
		[Description("Specifies whether the installer will reevaluate the Condition property for the selected item when reinstalling on a target computer")]
		public bool Transitive
		{
			get
			{
				if (this.ParentComponent != null)
				{
					return this.ParentComponent.WiXElement.Transitive == "yes";
				}
				if (this.Collection.ParentKey == null || this.Collection.ParentKey.ParentComponent == null)
				{
					return false;
				}
				return this.Collection.ParentKey.ParentComponent.WiXElement.Transitive == "yes";
			}
			set
			{
				if (this.ParentComponent != null)
				{
					this.ParentComponent.WiXElement.Transitive = (value ? "yes" : "no");
					return;
				}
				if (this.Collection.ParentKey != null && this.Collection.ParentKey.ParentComponent != null)
				{
					this.Collection.ParentKey.ParentComponent.WiXElement.Transitive = (value ? "yes" : "no");
				}
			}
		}

		[Browsable(false)]
		[DefaultValue(null)]
		[Description("Specifies the data stored in the selected registry value")]
		public abstract object Value
		{
			get;
			set;
		}

		[Browsable(true)]
		[DefaultValue(VSRegistryValueType.vsdrvtString)]
		[Description("Specifies the data type of a selected registry value")]
		public VSRegistryValueType ValueType
		{
			get
			{
				return this.GetValueType();
			}
		}

		internal WiXRegistryValue WiXElement
		{
			get
			{
				return this._wixElement;
			}
		}

		internal string WiXType
		{
			get
			{
				return this.GetWiXValueType();
			}
		}

		public VSRegistryValueBase()
		{
		}

		public VSRegistryValueBase(WiXProjectParser project, WiXRegistryValue wixElement, VSComponent parentComponent, VSRegistryValues collection) : this()
		{
			this._project = project;
			this._wixElement = wixElement;
			this._parentComponent = parentComponent;
			this._collection = collection;
			this._collection.Add(this);
		}

		public override void Delete()
		{
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

		protected abstract VSRegistryValueType GetValueType();

		protected abstract string GetWiXValueType();

		internal void MoveTo(VSRegistryKey newParent)
		{
			this.Collection.Remove(this);
			newParent.Values.Add(this);
			if (this.ParentComponent != null)
			{
				this.WiXElement.Root = newParent.WiXElement.Root;
				this.WiXElement.Key = newParent.WiXElement.Key;
				return;
			}
			this.WiXElement.Parent.ChildEntities.Remove(this.WiXElement);
			this.WiXElement.XmlNode.ParentNode.RemoveChild(this.WiXElement.XmlNode);
			this.WiXElement.Parent.SetDirty();
			if (this.WiXElement.Owner != newParent.WiXElement.Owner)
			{
				this.WiXElement.RebuildXmlNodes(newParent.WiXElement.XmlNode.OwnerDocument, newParent.WiXElement.Owner);
			}
			newParent.WiXElement.ChildEntities.Add(this.WiXElement);
			newParent.WiXElement.XmlNode.AppendChild(this.WiXElement.XmlNode);
			this.WiXElement.Parent = newParent.WiXElement;
			this.WiXElement.Parent.SetDirty();
		}
	}
}