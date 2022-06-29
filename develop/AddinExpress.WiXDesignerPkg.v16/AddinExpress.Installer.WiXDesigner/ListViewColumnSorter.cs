using System;
using System.Collections;
using System.Windows.Forms;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class ListViewColumnSorter : IComparer
	{
		private int _columnToSort;

		private SortOrder _orderOfSort = SortOrder.Ascending;

		private CaseInsensitiveComparer _objectCompare;

		public SortOrder Order
		{
			get
			{
				return this._orderOfSort;
			}
			set
			{
				this._orderOfSort = value;
			}
		}

		public int SortColumn
		{
			get
			{
				return this._columnToSort;
			}
			set
			{
				this._columnToSort = value;
			}
		}

		public ListViewColumnSorter()
		{
			this._objectCompare = new CaseInsensitiveComparer();
		}

		public int Compare(object x, object y)
		{
			int num = 0;
			ListViewItem listViewItem = (ListViewItem)x;
			ListViewItem listViewItem1 = (ListViewItem)y;
			if (this.SortColumn != 0)
			{
				num = this._objectCompare.Compare(listViewItem.SubItems[this._columnToSort].Text, listViewItem1.SubItems[this._columnToSort].Text);
			}
			else if (!(listViewItem.Tag is VSBaseFolder) && !(listViewItem1.Tag is VSBaseFolder))
			{
				num = this._objectCompare.Compare(listViewItem.SubItems[this._columnToSort].Text, listViewItem1.SubItems[this._columnToSort].Text);
			}
			else if (listViewItem1.Tag is VSBaseFolder)
			{
				num = this._objectCompare.Compare(listViewItem.SubItems[this._columnToSort].Text, listViewItem1.SubItems[this._columnToSort].Text);
			}
			else if (this._orderOfSort == SortOrder.Ascending)
			{
				num = -1;
			}
			else if (this._orderOfSort == SortOrder.Descending)
			{
				num = 1;
			}
			if (this._orderOfSort == SortOrder.Ascending)
			{
				return num;
			}
			if (this._orderOfSort != SortOrder.Descending)
			{
				return 0;
			}
			return -num;
		}
	}
}