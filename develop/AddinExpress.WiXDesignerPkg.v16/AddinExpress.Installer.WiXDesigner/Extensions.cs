using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AddinExpress.Installer.WiXDesigner
{
	[ComVisible(false)]
	internal static class Extensions
	{
		internal static void MoveDown(this TreeNode node)
		{
			TreeNode parent = node.Parent;
			TreeView treeView = node.TreeView;
			treeView.BeginUpdate();
			try
			{
				if (parent != null)
				{
					int num = parent.Nodes.IndexOf(node);
					if (num < parent.Nodes.Count - 1)
					{
						parent.Nodes.RemoveAt(num);
						parent.Nodes.Insert(num + 1, node);
					}
				}
				else if (treeView != null && treeView.Nodes.Contains(node))
				{
					int num1 = treeView.Nodes.IndexOf(node);
					if (num1 < treeView.Nodes.Count - 1)
					{
						treeView.Nodes.RemoveAt(num1);
						treeView.Nodes.Insert(num1 + 1, node);
					}
				}
				node.TreeView.SelectedNode = node;
			}
			finally
			{
				treeView.EndUpdate();
			}
		}

		internal static void MoveUp(this TreeNode node)
		{
			TreeNode parent = node.Parent;
			TreeView treeView = node.TreeView;
			treeView.BeginUpdate();
			try
			{
				if (parent != null)
				{
					int num = parent.Nodes.IndexOf(node);
					if (num > 0)
					{
						parent.Nodes.RemoveAt(num);
						parent.Nodes.Insert(num - 1, node);
					}
				}
				else if (node.TreeView.Nodes.Contains(node))
				{
					int num1 = treeView.Nodes.IndexOf(node);
					if (num1 > 0)
					{
						treeView.Nodes.RemoveAt(num1);
						treeView.Nodes.Insert(num1 - 1, node);
					}
				}
				node.TreeView.SelectedNode = node;
			}
			finally
			{
				treeView.EndUpdate();
			}
		}
	}
}