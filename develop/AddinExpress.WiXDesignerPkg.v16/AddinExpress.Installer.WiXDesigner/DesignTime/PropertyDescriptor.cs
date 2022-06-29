using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.CompilerServices;

namespace AddinExpress.Installer.WiXDesigner.DesignTime
{
	internal class PropertyDescriptor : System.ComponentModel.PropertyDescriptor
	{
		private Type m_compType;

		private Type m_PropType;

		private System.ComponentModel.PropertyDescriptor m_pd;

		private List<PropertyValueUIItem> m_colUIItem = new List<PropertyValueUIItem>();

		private int m_AppendCount;

		private Image m_ValueImage;

		protected List<StandardValue> m_StatandardValues = new List<StandardValue>();

		protected object parentComponent;

		protected object m_value;

		private readonly static char m_HiddenChar;

		private static ulong m_COUNT;

		internal readonly ulong _ID;

		internal int AppendCount
		{
			get
			{
				return this.m_AppendCount;
			}
			set
			{
				this.m_AppendCount = value;
			}
		}

		internal bool Browsable
		{
			get
			{
				BrowsableAttribute browsableAttribute = (BrowsableAttribute)this.Attributes.Get(typeof(BrowsableAttribute), true);
				if (browsableAttribute == null)
				{
					return true;
				}
				return browsableAttribute.Browsable;
			}
			set
			{
				BrowsableAttribute browsableAttribute = (BrowsableAttribute)this.Attributes.Get(typeof(BrowsableAttribute), true);
				if (browsableAttribute != null)
				{
					this.Attributes.Remove(browsableAttribute);
				}
				this.Attributes.Add(new BrowsableAttribute(value), false);
			}
		}

		public override string Category
		{
			get
			{
				string category = base.Category;
				CategoryAttribute categoryAttribute = (CategoryAttribute)this.Attributes.Get(typeof(CategoryAttribute), true);
				if (categoryAttribute != null)
				{
					category = categoryAttribute.Category;
				}
				category = category.PadLeft(category.Length + this.m_AppendCount, AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor.m_HiddenChar);
				return category;
			}
		}

		internal int CategoryId
		{
			get
			{
				SortIDAttribute sortIDAttribute = (SortIDAttribute)this.Attributes.Get(typeof(SortIDAttribute), true);
				if (sortIDAttribute == null)
				{
					return 0;
				}
				return sortIDAttribute.CategoryOrder;
			}
			set
			{
				SortIDAttribute sortIDAttribute = (SortIDAttribute)this.Attributes.Get(typeof(SortIDAttribute), true);
				if (sortIDAttribute == null)
				{
					sortIDAttribute = new SortIDAttribute();
					this.Attributes.Add(sortIDAttribute);
				}
				sortIDAttribute.CategoryOrder = value;
			}
		}

		internal string CategoryResourceKey
		{
			get
			{
				CategoryResourceKeyAttribute categoryResourceKeyAttribute = (CategoryResourceKeyAttribute)this.Attributes.Get(typeof(CategoryResourceKeyAttribute), true);
				if (categoryResourceKeyAttribute == null)
				{
					return string.Empty;
				}
				return categoryResourceKeyAttribute.ResourceKey;
			}
			set
			{
				CategoryResourceKeyAttribute categoryResourceKeyAttribute = (CategoryResourceKeyAttribute)this.Attributes.Get(typeof(CategoryResourceKeyAttribute), true);
				if (categoryResourceKeyAttribute == null)
				{
					categoryResourceKeyAttribute = new CategoryResourceKeyAttribute();
					this.Attributes.Add(categoryResourceKeyAttribute);
				}
				categoryResourceKeyAttribute.ResourceKey = value;
			}
		}

		public override Type ComponentType
		{
			get
			{
				if (this.m_pd == null)
				{
					return this.m_compType;
				}
				return this.m_pd.ComponentType;
			}
		}

		internal object DefaultValue
		{
			get
			{
				DefaultValueAttribute defaultValueAttribute = (DefaultValueAttribute)this.Attributes.Get(typeof(DefaultValueAttribute), true);
				if (defaultValueAttribute == null)
				{
					return null;
				}
				if (this.parentComponent == null || this.m_pd == null)
				{
					return defaultValueAttribute.Value;
				}
				return this.m_pd.GetValue(this.parentComponent);
			}
			set
			{
				this.Attributes.Add(new DefaultValueAttribute(value), true);
			}
		}

		public override bool DesignTimeOnly
		{
			get
			{
				return false;
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				ReadOnlyAttribute readOnlyAttribute = (ReadOnlyAttribute)this.Attributes.Get(typeof(ReadOnlyAttribute), true);
				if (readOnlyAttribute == null)
				{
					return false;
				}
				return readOnlyAttribute.IsReadOnly;
			}
		}

		internal int LCID
		{
			get;
			set;
		}

		internal int PropertyId
		{
			get
			{
				SortIDAttribute sortIDAttribute = (SortIDAttribute)this.Attributes.Get(typeof(SortIDAttribute), true);
				if (sortIDAttribute == null)
				{
					return 0;
				}
				return sortIDAttribute.PropertyOrder;
			}
			set
			{
				SortIDAttribute sortIDAttribute = (SortIDAttribute)this.Attributes.Get(typeof(SortIDAttribute), true);
				if (sortIDAttribute == null)
				{
					sortIDAttribute = new SortIDAttribute();
					this.Attributes.Add(sortIDAttribute);
				}
				sortIDAttribute.PropertyOrder = value;
			}
		}

		public override Type PropertyType
		{
			get
			{
				if (this.m_pd == null)
				{
					return this.m_PropType;
				}
				return this.m_pd.PropertyType;
			}
		}

		public virtual IList<StandardValue> StandardValues
		{
			get
			{
				return this.m_StatandardValues;
			}
		}

		public ICollection<PropertyValueUIItem> StateItems
		{
			get
			{
				return this.m_colUIItem;
			}
		}

		public Image ValueImage
		{
			get
			{
				return this.m_ValueImage;
			}
			set
			{
				this.m_ValueImage = value;
			}
		}

		static PropertyDescriptor()
		{
			AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor.m_HiddenChar = '\t';
			AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor.m_COUNT = (ulong)1;
		}

		public PropertyDescriptor(Type componentType, string sName, Type propType, object value, object parent, params Attribute[] attributes) : base(sName, attributes)
		{
			ulong mCOUNT = AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor.m_COUNT;
			AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor.m_COUNT = mCOUNT + (long)1;
			this._ID = mCOUNT;
			this.m_compType = componentType;
			this.m_value = value;
			this.m_PropType = propType;
			this.parentComponent = parent;
		}

		public PropertyDescriptor(System.ComponentModel.PropertyDescriptor pd, object parent) : base(pd)
		{
			this.m_pd = pd;
			ulong mCOUNT = AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor.m_COUNT;
			AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor.m_COUNT = mCOUNT + (long)1;
			this._ID = mCOUNT;
			this.parentComponent = parent;
		}

		public override bool CanResetValue(object component)
		{
			object defaultValue = this.DefaultValue;
			if (defaultValue == null)
			{
				return false;
			}
			return !this.GetValue(component).Equals(defaultValue);
		}

		public override object GetValue(object component)
		{
			if (this.m_pd == null)
			{
				return this.m_value;
			}
			return this.m_pd.GetValue(component);
		}

		public override void ResetValue(object component)
		{
			if (this.m_pd != null)
			{
				this.m_pd.ResetValue(component);
				return;
			}
			this.SetValue(component, this.DefaultValue);
		}

		public override void SetValue(object component, object value)
		{
			this.m_value = value;
			if (this.m_pd != null)
			{
				this.m_pd.SetValue(component, this.m_value);
			}
			base.OnValueChanged(component, new EventArgs());
		}

		public override bool ShouldSerializeValue(object component)
		{
			if (this.DefaultValue == null)
			{
				return true;
			}
			return this.CanResetValue(component);
		}
	}
}