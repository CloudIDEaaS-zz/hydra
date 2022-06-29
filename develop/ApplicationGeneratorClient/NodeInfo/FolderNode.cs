using AbstraX.ClientFolderStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Utils;


namespace ApplicationGenerator.Client.NodeInfo
{
    public class FolderNode : NodeInfoBase
    {
        public FolderType FolderType { get; private set; }

        public FolderNode(TreeNode treeNode, Folder nodeParent, FolderType folderType)
        {
            this.TreeNode = treeNode;
            this.NodeObject = nodeParent;
            this.Text = treeNode.Text;
            this.SpecialNodeType = SpecialNodeType.Folder;
            this.FolderType = folderType;

            //if (folderType == FolderType.ProjectFolder && !isWebProjectFolder)
            //{
            //    this.NodeInfoType = NodeInfoType.ProjectFolder;
            //}
            //else if (folderType == FolderType.Members)
            //{
            //    this.NodeInfoType = NodeInfoType.Folder;
            //}
            //else
            //{
                this.NodeInfoType = NodeInfoType.Folder;
            //}
        }
    }
}
