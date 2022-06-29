using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using VisualStudioProvider;
using CodeInterfaces;
using Hydra.GenerationObjects;
using Newtonsoft.Json.Linq;
using Hydra.Configuration;


namespace Net2Html5ConfigurationTool.NodeInfo
{
    public class CSharpFileNode : CodeFileNode
    {
        private IVSProjectItem projectItem;
        public IRuntimeProjectItem VirtualProjectItem { get; private set; }
        public override bool IsVirtual { get; protected set; }

        public CSharpFileNode(TreeNode treeNode, IVSProjectItem projectItem)
        {
            this.TreeNode = treeNode;
            this.NodeObject = projectItem;
            this.Text = treeNode.Text;
            this.NodeInfoType = NodeInfoType.CSharpFile;
            this.projectItem = projectItem;
        }

        public CSharpFileNode(TreeNode treeNode, IRuntimeProjectItem projectItem)
        {
            this.TreeNode = treeNode;
            this.NodeObject = projectItem;
            this.Text = treeNode.Text;
            this.NodeInfoType = NodeInfoType.None;
            this.SpecialNodeType = SpecialNodeType.CSharpFile;
            this.VirtualProjectItem = projectItem;
            this.IsVirtual = true;
        }

        public override IRegisteredConstruct RegisteredConstruct
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override IVSProjectItem ProjectItem
        {
            get 
            {
                return projectItem;
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
