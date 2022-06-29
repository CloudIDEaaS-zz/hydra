using AbstraX.ClientFolderStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ApplicationGenerator.Client.NodeInfo
{
    public class FileNode : NodeInfoBase
    {

        public FileNode(TreeNode treeNode, File nodeParent) : base()
        {
            this.TreeNode = treeNode;
            this.NodeObject = nodeParent;
            this.Text = treeNode.Text;

            this.NodeInfoType = NodeInfoType.File;
        }
    }
}
