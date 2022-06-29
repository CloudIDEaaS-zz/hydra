using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Utils;
using Newtonsoft.Json.Linq;
using Hydra.Configuration;

namespace Net2Html5ConfigurationTool.NodeInfo
{
    public class ReferencedAssemblyNode : NodeInfoBase
    {
        public ReferencedAssemblyNode(TreeNode treeNode, _Assembly assembly)
        {
            this.TreeNode = treeNode;
            this.NodeObject = assembly;
            this.Text = treeNode.Text;
            this.NodeInfoType = NodeInfoType.None;
            base.IsReferencedAssembly = true;
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
