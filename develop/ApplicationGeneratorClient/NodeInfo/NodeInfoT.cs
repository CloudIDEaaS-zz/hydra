using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.CSharp;
using System.Windows.Forms;
using System.Reflection;
using Utils;
using Newtonsoft.Json.Linq;
using Hydra.Configuration;

namespace ApplicationGenerator.Client.NodeInfo
{
    public class NodeInfo<T> : NodeInfoBase where T : AstNode
    {
        public NodeInfo(T node, TreeNode treeNode, IncludedAssembly assembly)
        {
            this.TreeNode = treeNode;
            this.NodeObject = node;
            this.Text = treeNode.Text;
            this.Assembly = assembly;
        }

        public override NodeInfoType NodeInfoType
        {
            get
            {
                return ((AstNode)NodeObject).GetNodeInfoType();
            }
        }

        public override bool IsVirtual
        {
            get
            {
                return false;
            }

            protected set
            {
                DebugUtils.Break();
            }
        }

        public override NodeSaveState GetSaveState()
        {
            return this.DetermineSaveState();
        }

        public override bool IsSaveable
        {
            get
            {
                return this.DetermineIsSaveable();
            }
        }

        public override ConfigNode GetConfigNode()
        {
            return this.ToConfigNode();
        }
    }
}
