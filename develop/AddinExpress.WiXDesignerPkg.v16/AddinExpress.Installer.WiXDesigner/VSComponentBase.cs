using AddinExpress.Installer.WiXDesigner.DesignTime;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

namespace AddinExpress.Installer.WiXDesigner
{
	[ToolboxItem(false)]
	internal class VSComponentBase : Component, ICustomTypeDescriptor
	{
		internal virtual bool CanDelete
		{
			get
			{
				return true;
			}
		}

		internal virtual bool CanRename
		{
			get
			{
				return false;
			}
		}

		[Browsable(true)]
		[System.ComponentModel.DisplayName("(Name)")]
		[MergableProperty(false)]
		[ReadOnly(true)]
		public virtual string Name
		{
			get
			{
				return string.Empty;
			}
			set
			{
			}
		}

		protected VSComponentBase()
		{
			AddinExpress.Installer.WiXDesigner.DesignTime.TypeDescriptor.InstallTypeDescriptor(this);
		}

		public virtual void Delete()
		{
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		protected virtual void DoParentChanged()
		{
			if (this.ParentChanged != null)
			{
				this.ParentChanged(this, EventArgs.Empty);
			}
		}

		protected virtual void DoPropertyChanged()
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, EventArgs.Empty);
			}
		}

		protected virtual string GetClassName()
		{
			return "File Installation Properties";
		}

		protected virtual string GetComponentName()
		{
			return this.Name;
		}

		AttributeCollection System.ComponentModel.ICustomTypeDescriptor.GetAttributes()
		{
			return System.ComponentModel.TypeDescriptor.GetAttributes(this, true);
		}

		string System.ComponentModel.ICustomTypeDescriptor.GetClassName()
		{
			return this.GetClassName();
		}

		string System.ComponentModel.ICustomTypeDescriptor.GetComponentName()
		{
			return this.GetComponentName();
		}

		TypeConverter System.ComponentModel.ICustomTypeDescriptor.GetConverter()
		{
			return System.ComponentModel.TypeDescriptor.GetConverter(this, true);
		}

		EventDescriptor System.ComponentModel.ICustomTypeDescriptor.GetDefaultEvent()
		{
			return System.ComponentModel.TypeDescriptor.GetDefaultEvent(this, true);
		}

		System.ComponentModel.PropertyDescriptor System.ComponentModel.ICustomTypeDescriptor.GetDefaultProperty()
		{
			return System.ComponentModel.TypeDescriptor.GetDefaultProperty(this, true);
		}

		object System.ComponentModel.ICustomTypeDescriptor.GetEditor(Type editorBaseType)
		{
			return System.ComponentModel.TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents()
		{
			return System.ComponentModel.TypeDescriptor.GetEvents(this, true);
		}

		EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
		{
			return System.ComponentModel.TypeDescriptor.GetEvents(this, attributes, true);
		}

		PropertyDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetProperties()
		{
			return System.ComponentModel.TypeDescriptor.GetProperties(this, true);
		}

		PropertyDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
		{
			return System.ComponentModel.TypeDescriptor.GetProperties(this, attributes, true);
		}

		object System.ComponentModel.ICustomTypeDescriptor.GetPropertyOwner(System.ComponentModel.PropertyDescriptor pd)
		{
			return this;
		}

		internal event EventHandler ParentChanged;

		internal event EventHandler PropertyChanged;
	}
}