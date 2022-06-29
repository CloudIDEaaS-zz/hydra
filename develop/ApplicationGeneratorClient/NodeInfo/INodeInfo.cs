using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace ApplicationGenerator.Client.NodeInfo
{
    public interface INodeInfo
    {
        SpecialNodeType SpecialNodeType { get; set; }
        NodeInfoType NodeInfoType { get; }
        object NodeObject { get; }
        string Text { get; }
        TreeNode TreeNode { get; }
        string NodePath { get; }
        string[] PathParts { get; }
        string PathStack { get; }
        string Name { get; }
    }
}
