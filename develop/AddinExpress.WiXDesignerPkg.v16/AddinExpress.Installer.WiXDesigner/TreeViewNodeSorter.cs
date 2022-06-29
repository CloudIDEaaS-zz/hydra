using System;
using System.Collections;
using System.Windows.Forms;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class TreeViewNodeSorter : IComparer
	{
		public TreeViewNodeSorter()
		{
		}

		public int Compare(object x, object y)
		{
			TreeNode treeNode = x as TreeNode;
			TreeNode treeNode1 = y as TreeNode;
			if (treeNode.Text == "Trial version limitations...")
			{
				return 1;
			}
			if (treeNode1.Text == "Trial version limitations...")
			{
				return -1;
			}
			if (treeNode.Tag is VSBaseFolder)
			{
				if (!(treeNode1.Tag is VSBaseFolder))
				{
					return -1;
				}
				return string.Compare(treeNode.Text, treeNode1.Text);
			}
			if (!(treeNode1.Tag is VSBaseFolder))
			{
				return string.Compare(treeNode.Text, treeNode1.Text);
			}
			if (!(treeNode.Tag is VSBaseFolder))
			{
				return 1;
			}
			return string.Compare(treeNode.Text, treeNode1.Text);
		}
	}
}