using System;
using System.Collections;
using System.ComponentModel;

namespace AddinExpress.Installer.WiXDesigner.DesignTime
{
	internal class PropertySorter : IComparer
	{
		private SortOrder m_PropertySortOrder = SortOrder.ByNameAscending;

		private SortOrder m_CategorySortOrder = SortOrder.ByNameAscending;

		public SortOrder CategorySortOrder
		{
			get
			{
				return this.m_CategorySortOrder;
			}
			set
			{
				this.m_CategorySortOrder = value;
			}
		}

		public SortOrder PropertySortOrder
		{
			get
			{
				return this.m_PropertySortOrder;
			}
			set
			{
				this.m_PropertySortOrder = value;
			}
		}

		public PropertySorter()
		{
		}

		public int Compare(object x, object y)
		{
			int categoryId;
			AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor propertyDescriptor = x as AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor;
			AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor propertyDescriptor1 = y as AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor;
			propertyDescriptor.AppendCount = 0;
			propertyDescriptor1.AppendCount = 0;
			int num = 0;
			switch (this.m_CategorySortOrder)
			{
				case SortOrder.None:
				{
					num = 0;
					break;
				}
				case SortOrder.ByNameAscending:
				{
					num = propertyDescriptor.Category.CompareTo(propertyDescriptor1.Category);
					break;
				}
				case SortOrder.ByNameDescending:
				{
					num = propertyDescriptor.Category.CompareTo(propertyDescriptor1.Category) * -1;
					break;
				}
				case SortOrder.ByIdAscending:
				{
					categoryId = propertyDescriptor.CategoryId;
					num = categoryId.CompareTo(propertyDescriptor1.CategoryId);
					break;
				}
				case SortOrder.ByIdDescending:
				{
					categoryId = propertyDescriptor.CategoryId;
					num = categoryId.CompareTo(propertyDescriptor1.CategoryId) * -1;
					break;
				}
			}
			if (num == 0)
			{
				num = this.CompareProperty(propertyDescriptor, propertyDescriptor1);
			}
			return num;
		}

		private int CompareProperty(AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor xCpd, AddinExpress.Installer.WiXDesigner.DesignTime.PropertyDescriptor yCpd)
		{
			int propertyId;
			int num = 0;
			switch (this.m_PropertySortOrder)
			{
				case SortOrder.None:
				{
					num = xCpd._ID.CompareTo(yCpd._ID);
					break;
				}
				case SortOrder.ByNameAscending:
				{
					num = xCpd.DisplayName.CompareTo(yCpd.DisplayName);
					break;
				}
				case SortOrder.ByNameDescending:
				{
					num = xCpd.DisplayName.CompareTo(yCpd.DisplayName) * -1;
					break;
				}
				case SortOrder.ByIdAscending:
				{
					propertyId = xCpd.PropertyId;
					num = propertyId.CompareTo(yCpd.PropertyId);
					break;
				}
				case SortOrder.ByIdDescending:
				{
					propertyId = xCpd.PropertyId;
					num = propertyId.CompareTo(yCpd.PropertyId) * -1;
					break;
				}
			}
			return num;
		}
	}
}