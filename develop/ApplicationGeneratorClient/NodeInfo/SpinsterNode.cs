using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Utils;

namespace ApplicationGenerator.Client.NodeInfo
{
    public class SpinsterNode : SpinsterNodeBase
    {
        public SpinsterNode(TreeNode treeNode) : base(treeNode)
        {
            this.TreeNode = treeNode;
            this.Text = treeNode.Text;
            this.NodeInfoType = NodeInfoType.None;
            this.SpecialNodeType = SpecialNodeType.Spinster;
        }
    }
}
