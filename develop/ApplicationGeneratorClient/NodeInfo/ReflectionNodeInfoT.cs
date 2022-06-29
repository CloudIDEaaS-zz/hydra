using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.CSharp;
using System.Windows.Forms;
using ICSharpCode.NRefactory.TypeSystem;
using System.Reflection;
using Utils;
using Newtonsoft.Json.Linq;
using Hydra.Configuration;


namespace Net2Html5ConfigurationTool.NodeInfo
{
    public class ReflectionNodeInfo<T> : NodeInfoBase, Net2Html5ConfigurationTool.NodeInfo.IReflectionNodeInfo where T : MemberInfo
    {
        public IAssembly NotYetIncludedAssembly { get; private set; }
        public AstNode AstNode { get; private set; }

        public ReflectionNodeInfo(T memberInfo, AstNode node, TreeNode treeNode, IAssembly assembly)
        {
            this.TreeNode = treeNode;
            this.NodeObject = node;
            this.Text = treeNode.Text;
            this.NotYetIncludedAssembly = assembly;
            this.AstNode = node;
        }

        public override NodeInfoType NodeInfoType
        {
            get
            {
                return this.AstNode.GetNodeInfoType();
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
