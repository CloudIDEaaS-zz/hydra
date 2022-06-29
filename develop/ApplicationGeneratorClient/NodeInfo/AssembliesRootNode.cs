using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using VisualStudioProvider;
using Utils;
using Newtonsoft.Json.Linq;
using Hydra.Configuration;

namespace Net2Html5ConfigurationTool.NodeInfo
{
    public class AssembliesRootNode : NodeInfoBase
    {
        public FileInfo ConfigFileInfo { get; private set; }

        public AssembliesRootNode(TreeNode treeNode, FileInfo configFileInfo)
        {
            this.TreeNode = treeNode;
            this.Text = treeNode.Text;
            this.NodeInfoType = NodeInfoType.None;
            this.ConfigFileInfo = configFileInfo;
            this.SpecialNodeType = SpecialNodeType.AssembliesRoot;
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
