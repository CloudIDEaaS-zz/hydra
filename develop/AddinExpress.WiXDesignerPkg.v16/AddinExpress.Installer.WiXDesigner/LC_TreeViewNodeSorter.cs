using System;
using System.Collections;
using System.Windows.Forms;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class LC_TreeViewNodeSorter : IComparer
	{
		public LC_TreeViewNodeSorter()
		{
		}

		public int Compare(object x, object y)
		{
			TreeNode treeNode = x as TreeNode;
			TreeNode treeNode1 = y as TreeNode;
			if (treeNode.Level != 1)
			{
				return string.Compare(treeNode.Text, treeNode1.Text);
			}
			if (treeNode.Name == "NodeSearch")
			{
				return -1;
			}
			return 1;
		}
	}
}