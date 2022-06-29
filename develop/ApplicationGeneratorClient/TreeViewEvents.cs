using AbstraX.ClientFolderStructure;
using ApplicationGenerator.Client.NodeInfo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace ApplicationGenerator.Client
{
    public delegate void FileSelectedHandler(object sender, SelectFileEventArgs e);
    public delegate void FolderSelectedHandler(object sender, SelectFolderEventArgs e);

    public class SelectFileSystemObjectEventArgs : EventArgs
    {
        public TreeNode TreeNode { get; }
        public INodeInfo NodeInfo { get; }

        public SelectFileSystemObjectEventArgs(TreeNode treeNode, INodeInfo nodeInfo)
        {
            this.TreeNode = treeNode;
            this.NodeInfo = nodeInfo;
        }
    }

    public class SelectFileEventArgs : SelectFileSystemObjectEventArgs
    {
        public File File { get; }

        public SelectFileEventArgs(TreeNode treeNode, INodeInfo nodeInfo) : base(treeNode, nodeInfo)
        {
            this.File = (File)nodeInfo.NodeObject;
        }
    }

    public class SelectFolderEventArgs : SelectFileSystemObjectEventArgs
    {
        public Folder Folder { get; }

        public SelectFolderEventArgs(TreeNode treeNode, INodeInfo nodeInfo) : base(treeNode, nodeInfo)
        {
            this.Folder = (Folder)nodeInfo.NodeObject;
        }
    }

    public class TreeViewEvents
    {
        public TreeView TreeView { get; }
        public event FileSelectedHandler FileSelected;
        public event FolderSelectedHandler FolderSelected;

        public TreeViewEvents(TreeView treeView)
        {
            this.TreeView = treeView;
            this.TreeView.AfterSelect += TreeView_AfterSelect;
        }

        private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var treeNode = e.Node;
            var nodeInfo = treeNode.GetNodeInfo();
            
            if (nodeInfo is FileNode)
            {
                FileSelected(sender, new SelectFileEventArgs(treeNode, nodeInfo));
            }
            else if (nodeInfo is FolderNode)
            {
                FolderSelected(sender, new SelectFolderEventArgs(treeNode, nodeInfo));
            }
            else
            {
                DebugUtils.Break();
            }
        }
    }
}
